using EntitiesServices.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace ERP_Condominios_Solution.ViewModels
{
    public class SolicitacaoViewModel
    {
        [Key]
        public int SOLI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE EXAME obrigatorio")]
        public int TIEX_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME DO EXAME obrigatorio")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O NOME DO EXAME deve conter no minimo 1 caracteres e no máximo 500 caracteres.")]
        public string SOLI_NM_TITULO { get; set; }
        public string SOLI_NM_INDICACAO { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÃO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "A DESCRIÇÃO DO EXAME deve conter no minimo 1 caracteres e no máximo 5000 caracteres.")]
        public string SOLI_DS_DESCRICAO { get; set; }
        public int SOLI_IN_ATIVO { get; set; }

        public virtual TIPO_EXAME TIPO_EXAME { get; set; }
        public virtual USUARIO USUARIO { get; set; }

    }
}