using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP_Condominios_Solution.ViewModels
{
    public class VideoViewModel
    {
        public int VIDE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> TIVE_CD_ID { get; set; }
        public Nullable<System.DateTime> VIDE_DT_INCLUSAO { get; set; }
        [Required(ErrorMessage = "Campo TÍTULO obrigatorio")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "O TÍTULO deve ter no minimo 1 e no máximo 250 caracteres.")]
        public string VIDE_NM_TITULO { get; set; }
        [StringLength(250, ErrorMessage = "O ARQUIVO deve ter no máximo 250 caracteres.")]
        public string VIDE_AQ_ARQUIVO { get; set; }
        public Nullable<int> VIDE_IN_ATIVO { get; set; }
        [StringLength(1000, ErrorMessage = "A DESCRIÇÃO deve ter no máximo 1000 caracteres.")]
        public string VIDE_DS_DESCRICAO { get; set; }

        public virtual TIPO_VIDEO TIPO_VIDEO { get; set; }


    }
}