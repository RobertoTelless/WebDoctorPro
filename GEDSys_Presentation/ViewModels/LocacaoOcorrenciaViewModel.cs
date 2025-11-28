using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;
using Newtonsoft.Json;

namespace ERP_Condominios_Solution.ViewModels
{
    public class LocacaoOcorrenciaViewModel
    {
        [Key]
        public int LOOC_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int LOCA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE OCORRÊNCIA obrigatorio")]
        public Nullable<int> TIOC_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LOOC_DT_OCORRENCIA { get; set; }
        [Required(ErrorMessage = "Campo TÍTULO obrigatorio")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "O TÍTULO deve conter no minimo 1 e no máximo 250 caracteres.")]
        public string LOOC_NM_TITULO { get; set; }
        [StringLength(500, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 500 caracteres.")]
        public string LOOC_DS_DESCRICAO { get; set; }
        [StringLength(50, ErrorMessage = "O NÚMERO DE SÉRIE deve conter no máximo 50 caracteres.")]
        public string LOOC_SERIE_ENTRADA { get; set; }
        [StringLength(50, ErrorMessage = "O NÚMERO DE SÉRIE deve conter no máximo 50 caracteres.")]
        public string LOOC_SERIE_SAIDA { get; set; }
        public int LOOC_IN_ATIVO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public PACIENTE PACIENTE_BASE { get; set; }
        public PRODUTO PRODUTO_BASE { get; set; }

        public virtual LOCACAO LOCACAO { get; set; }
        public virtual TIPO_OCORRENCIA TIPO_OCORRENCIA { get; set; }
        public virtual USUARIO USUARIO { get; set; }

    }
}