using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacienteVacinaViewModel
    {
        [Key]
        public int PAVI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PACIENTE obrigatorio")]
        public int PACI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo VACINA obrigatorio")]
        public Nullable<int> VACI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAVI_DT_DATA { get; set; }
        public Nullable<int> PAVI_IN_ATIVO { get; set; }
        [StringLength(500, ErrorMessage = "A DESCRI^ÇÃO deve conter no máximo 500 caracteres.")]
        public string PAVI_DS_DESCRICAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAVI_DT_PROXIMA { get; set; }

        public virtual PACIENTE PACIENTE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual VACINA VACINA { get; set; }
    }
}