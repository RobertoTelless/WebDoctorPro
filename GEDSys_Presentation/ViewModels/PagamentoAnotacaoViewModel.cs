using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PagamentoAnotacaoViewModel
    {
        [Key]
        public int PGAN_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int COPA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PGAN_DT_ANOTACAO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 e no máximo 500 caracteres.")]
        public string PGAN_TX_ANOTACAO { get; set; }
        public Nullable<int> PGAN_IN_ATIVO { get; set; }

        public virtual CONSULTA_PAGAMENTO CONSULTA_PAGAMENTO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}