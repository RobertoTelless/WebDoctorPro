using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MedicamentoBaseViewModel
    {
        [Key]
        public int MEDI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FORMA DE USO obrigatorio")]
        public Nullable<int> TIFO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo MEDICAMENTO obrigatorio")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 250 caracteres.")]
        public string MEDI_NM_MEDICAMENTO { get; set; }
        [StringLength(250, ErrorMessage = "O NOME GENÉRICO deve conter no máximo 250 caracteres.")]
        public string MEDI_NM_GENERICO { get; set; }
        [StringLength(250, ErrorMessage = "A APRESENTAÇÃO deve conter no máximo 250 caracteres.")]
        public string MEDI_NM_APRESENTACAO { get; set; }
        [StringLength(250, ErrorMessage = "O LABORATÓRIO deve conter no máximo 250 caracteres.")]
        public string MEDI_NM_LABORATORIO { get; set; }
        public int MEDI_IN_ATIVO { get; set; }
        public string MEDI_DS_DESCRICAO { get; set; }
        [StringLength(1500, ErrorMessage = "A POSOLOGIA deve conter no máximo 1500 caracteres.")]
        public string MEDI_DS_POSOLOGIA { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE CONTROLE obrigatorio")]
        public Nullable<int> TICO_CD_ID { get; set; }

        public virtual TIPO_FORMA TIPO_FORMA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual TIPO_CONTROLE TIPO_CONTROLE { get; set; }
    }
}