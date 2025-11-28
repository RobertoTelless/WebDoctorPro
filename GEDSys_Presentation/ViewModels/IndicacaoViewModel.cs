using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class IndicacaoViewModel
    {
        [Key]
        public int INDI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        [Required(ErrorMessage = "Campo DATA DA INDICAÇÃO obrigatorio")]
        public Nullable<System.DateTime> INDI_DT_DATA { get; set; }
        [Required(ErrorMessage = "Campo NOME DO INDICADO obrigatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "NOME DO INDICADO deve conter no minimo 2 e no máximo 100 caracteres.")]
        public string INDI_NM_INDICADO { get; set; }
        [Required(ErrorMessage = "Campo E-MAIL obrigatorio")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "E-MAIL deve conter no minimo 1 e no máximo 150 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string INDI_NM_EMAIL { get; set; }
        [StringLength(5000, ErrorMessage = "TEXTO DA MENSAGEM deve conter no máximo 5000 caracteres.")]
        public string INDI_TX_MENSAGEM { get; set; }
        public int INDI_IN_ATIVO { get; set; }
        public string INDI_GU_IDENTIFICADOR { get; set; }
        public Nullable<int> INDI_IN_STATUS { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> INDI_DT_DESFECHO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> INDI_DT_PAGAMENTO { get; set; }
        public Nullable<int> INDI_IN_PAGAMENTO { get; set; }
        public Nullable<int> INDI_IN_SISTEMA { get; set; }
        [StringLength(20, ErrorMessage = "TELEFONE deve conter no máximo 20 caracteres.")]
        public string INDI_NR_TELEFONE { get; set; }
        [StringLength(20, ErrorMessage = "CELULAR deve conter no máximo 20 caracteres.")]
        public string INDI_NR_CELULAR { get; set; }
        [StringLength(5000, ErrorMessage = "JUSTIFICATIVA deve conter no máximo 5000 caracteres.")]
        public string INDI_DS_JUSTIFICATIVA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> INDI_DT_ATUALIZACAO { get; set; }
        [StringLength(5000, ErrorMessage = "INFORMAÇÃO DE ENCERRAMENTO deve conter no máximo 5000 caracteres.")]
        public string INDI_DS_ENCERRAMENTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> INDI_VL_PAGAMENTO { get; set; }

        public String Status
        {
            get
            {
                if (INDI_IN_STATUS == 1)
                {
                    return "Indicado";
                }
                if (INDI_IN_STATUS == 2)
                {
                    return "Em Processamento";
                }
                if (INDI_IN_STATUS == 3)
                {
                    return "Recusado";
                }
                if (INDI_IN_STATUS == 4)
                {
                    return "Cancelado";
                }
                if (INDI_IN_STATUS == 5)
                {
                    return "Pausado";
                }
                return "Sucesso";
            }
        }
        public String StatusPagamento
        {
            get
            {
                if (INDI_IN_PAGAMENTO == 1)
                {
                    return "Pendente";
                }
                if (INDI_IN_PAGAMENTO == 2)
                {
                    return "Pago";
                }
                return "-";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDICACAO_ACAO> INDICACAO_ACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INDICACAO_ANEXO> INDICACAO_ANEXO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}