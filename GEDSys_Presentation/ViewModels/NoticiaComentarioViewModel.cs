using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class NoticiaComentarioViewModel
    {
        [Key]
        public int NOCO_CD_ID { get; set; }
        public int NOTC_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> NOCO_DT_COMENTARIO { get; set; }
        [Required(ErrorMessage = "Campo COMENTÁRIO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "O COMENTÁRIO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string NOCO_DS_COMENTARIO { get; set; }
        public Nullable<int> NOCO_IN_ATIVO { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }

        public virtual NOTICIA NOTICIA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual PACIENTE PACIENTE { get; set; }
    }
}