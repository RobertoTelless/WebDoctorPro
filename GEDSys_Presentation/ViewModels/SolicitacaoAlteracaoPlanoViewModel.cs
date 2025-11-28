using System;
using System.ComponentModel.DataAnnotations;

namespace ERP_Condominios_Solution.ViewModels
{
    public class SolicitacaoAlteracaoPlanoViewModel
    {
        public Int32 ASSI_CD_ID { get; set; }
        public String ASSI_NM_NOME { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> SOPL_DT_CONTATO { get; set; }
        public String SOPL_TX_OBSERVACOES { get; set; }
        [Required(ErrorMessage = "Campo MENSAGEM obrigatorio")]
        [StringLength(1000, ErrorMessage = "A MENSAGEM deve conter no máximo 1000 caracteres.")]
        public String SOPL_TX_MENSAGEM { get; set; }
        [Required(ErrorMessage = "Campo PLANO obrigatorio")]
        public Nullable<Int32> PLAN_CD_ID { get; set; }
        public Nullable<Int32> PLAN_CD_ID_ATUAL { get; set; }
        [Required(ErrorMessage = "Campo FORMA DE RESPOSTA obrigatorio")]
        public Nullable<Int32> SOPL_IN_RESPOSTA { get; set; }
        [Required(ErrorMessage = "Campo HORÁRIO obrigatorio")]
        public Nullable<Int32> SOPL_IN_HORARIO { get; set; }
        [Required(ErrorMessage = "Campo PRIORIDADE obrigatorio")]
        public Nullable<Int32> SOPL_IN_PRIORIDADE { get; set; }
        [StringLength(150, ErrorMessage = "O E-MAIL deve conter no máximo 150 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public String SOPL_EM_MAIL { get; set; }
        [StringLength(20, ErrorMessage = "O TELEFONE deve conter no máximo 20 caracteres.")]
        public String SOPL_NR_TELEFONE { get; set; }
        [StringLength(20, ErrorMessage = "O TELEFONE deve conter no máximo 20 caracteres.")]
        public String SOPL_NR_CELULAR { get; set; }
        public String SOPL_NM_PLANO_ATUAL { get; set; }
        public Nullable<System.DateTime> SOPL_DT_VALIDADE { get; set; }
        [Required(ErrorMessage = "Campo MOTIVO obrigatorio")]
        public String SOPL_NM_MOTIVO { get; set; }

    }
}