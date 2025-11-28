using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class AvisoLembreteViewModel
    {
        [Key]
        public int AVIS_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo USUÁRIO obrigatorio")]
        public int USUA_CD_ID { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> AVIS_DT_CRIACAO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE AVISO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> AVIS_DT_AVISO { get; set; }
        [Required(ErrorMessage = "Campo TÍTULO obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "TÍTULO deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string AVIS_NM_TITULO { get; set; }
        [Required(ErrorMessage = "Campo TEXTO DO AVISO obrigatorio")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "TEXTO DO AVISO deve conter no minimo 1 e no máximo 500 caracteres.")]
        public string AVIS_DS_AVISO { get; set; }
        public Nullable<int> AVIS_IN_CIENTE { get; set; }
        public Nullable<int> AVIS_IN_SISTEMA { get; set; }
        public int AVIS_IN_ATIVO { get; set; }
        public Nullable<int> PROD_CD_ID { get; set; }

        public String Situacao
        {
            get
            {
                if (AVIS_IN_CIENTE == 1)
                {
                    return "Fechado";
                }
                return "Pendente";
            }
        }

        public virtual PACIENTE PACIENTE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }
    }
}