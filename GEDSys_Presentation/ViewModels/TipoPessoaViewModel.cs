using System;
using System.ComponentModel.DataAnnotations;

namespace SystemBRPresentation.ViewModels
{
    public class TipoPessoaViewModel
    {
        [Key]
        public int TIPE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve ter no minimo 1 caractere e no máximo 50.")]
        public string TIPE_NM_NOME { get; set; }
        public int TIPE_IN_ATIVO { get; set; }

    }
}