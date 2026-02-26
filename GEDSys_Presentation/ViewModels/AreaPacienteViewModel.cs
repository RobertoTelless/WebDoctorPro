using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class AreaPacienteViewModel
    {
        [Key]
        public int AREA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<System.DateTime> AREA_DT_ENTRADA { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> AREA_IN_TIPO { get; set; }
        public Nullable<int> AREA_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo DATA DA CONSULTA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "DATA DA CONSULTA Deve ser uma data válida")]
        public Nullable<System.DateTime> AREA_DT_CONSULTA { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> AREA_HR_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> AREA_HR_FINAL { get; set; }
        [Required(ErrorMessage = "Campo TITULO obrigatorio")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O TÍTULO DO ITEM deve conter no minimo 1 caracteres e no máximo 500 caracteres.")]
        public string AREA_NM_TITULO { get; set; }
        public string AREA_TX_CONTEUDO { get; set; }
        public string AREA_GU_IDENTIFICADOR { get; set; }

        public string NOME_PACIENTE { get; set; }
        public string NOME_PROFISSIONAL { get; set; }
        public string HORARIO { get; set; }
        public string EMAIL_PROFISSIONAL { get; set; }

        public virtual PACIENTE PACIENTE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}