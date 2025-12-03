using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente
    {
        public int PACI__CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int TIPA_CD_ID { get; set; }
        public string PACI_NM_NOME { get; set; }
        public string PACI_NM_SOCIAL { get; set; }
        public Nullable<int> SEXO_CD_ID { get; set; }
        public Nullable<int> COR1_CD_ID { get; set; }
        public Nullable<int> ESCI_CD_ID { get; set; }
        public Nullable<int> CONV_CD_ID { get; set; }
        public string PACI_NM_PAI { get; set; }
        public string PACI_NM_MAE { get; set; }
        public string PACI_NR_CEP { get; set; }
        public string PACI_NM_ENDERECO { get; set; }
        public string PACI_NR_NUMERO { get; set; }
        public string PACI_NR_COMPLEMENTO { get; set; }
        public string PACI_NM_BAIRRO { get; set; }
        public string PACI_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        public string PACI_NM_PROFISSAO { get; set; }
        public Nullable<System.DateTime> PACI_DT_NASCIMENTO { get; set; }
        public string PACI_NM_NATURALIDADE { get; set; }
        public string PACI_NM_NACIONALIDADE { get; set; }
        public string PACI_NR_CPF { get; set; }
        public string PACI_NR_RG { get; set; }
        public string PACI_NR_TELEFONE { get; set; }
        public string PACI_NR_CELULAR { get; set; }
        public string PACI_NM_EMAIL { get; set; }
        public Nullable<int> PACI_IN_ATIVO { get; set; }
        public Nullable<System.DateTime> PACI_DT_CADASTRO { get; set; }
        public string PACI_AQ_FOTO { get; set; }
        public string PACI_NR_MATRICULA { get; set; }
        public Nullable<int> GRAU_CD_ID { get; set; }
        public Nullable<System.DateTime> PACI_DT_PREVISAO_RETORNO { get; set; }
        public string PACI_NM_INDICACAO { get; set; }
        public string PACI_TX_OBSERVACOES { get; set; }
        public Nullable<int> NACI_CD_ID { get; set; }
        public Nullable<int> MUNI_CD_ID { get; set; }
        public Nullable<int> MUNI_SG_UF { get; set; }
        public string PACI_GU_GUID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<System.DateTime> PACI_DT_ALTERACAO { get; set; }
        public Nullable<System.DateTime> PACI_DT_CONSULTA { get; set; }
        public string PACI_SG_NATURALIDADE_UF { get; set; }
        public Nullable<int> PACI_NR_IDADE { get; set; }
        public Nullable<System.DateTime> PACI_DT_ULTIMO_ACESSO { get; set; }
        public Nullable<System.DateTime> PACI_DT_ACESSO { get; set; }
        public Nullable<int> PACI_IN_COMPLETADO { get; set; }
        public Nullable<int> PACI_IN_MENOR { get; set; }
        public string PACI_NM_RESPONSAVEL { get; set; }
        public Nullable<int> PACI_IN_FICHAS { get; set; }
        public Nullable<int> PACI_IN_PADRAO_ANAMNESE { get; set; }
        public Nullable<int> PACI_IN_PADRAO_CONTINUA { get; set; }
        public Nullable<int> VACO_CD_ID { get; set; }
        public Nullable<System.DateTime> PACI_DT_PRECO { get; set; }
        public string PACI_NM_LOGIN { get; set; }
        public string PACI_NM_SENHA { get; set; }
        public Nullable<int> PACI_IN_HUMANO { get; set; }
        public string PACI_NM_NOVA_SENHA { get; set; }
        public string PACI_NM_SENHA_CONFIRMA { get; set; }
        public Nullable<int> PACI_IN_MENSAGEM_ATRASO { get; set; }
        public Nullable<int> PACI_IN_NUMERO_ENVIO { get; set; }
        public Nullable<System.DateTime> PACI_DT_ULTIMO_ENVIO { get; set; }
        public Nullable<int> PACI_IN_FIM_ENVIO { get; set; }
    }
}
