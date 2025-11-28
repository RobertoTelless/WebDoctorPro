using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacienteExameViewModel
    {
        [Key]
        public int PAEX_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PACIENTE obrigatorio")]
        public int PACI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE EXAME obrigatorio")]
        public Nullable<int> TIEX_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAEX_DT_DATA { get; set; }
        [StringLength(250, MinimumLength = 1, ErrorMessage = "EXAME deve conter no minimo 1 e no máximo 250 caracteres.")]
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        public string PAEX_NM_NOME { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "RESULTADO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string PAEX_DS_COMENTARIOS { get; set; }
        [StringLength(250, MinimumLength = 1, ErrorMessage = "ARQUIVO deve conter no minimo 1 e no máximo 250 caracteres.")]
        public string PAEX_AQ_ARQUIVO { get; set; }
        public Nullable<int> PAEX_IN_TIPO { get; set; }
        public Nullable<int> PAEX_IN_ATIVO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "DIAGNÓSTICO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string PAEX_DS_DIAGNOSTICO { get; set; }
        public Nullable<int> PACO_CD_ID { get; set; }
        public Nullable<int> LABS_CD_ID { get; set; }
        public Nullable<int> PASO_CD_ID { get; set; }

        public virtual PACIENTE PACIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_EXAME_ANEXO> PACIENTE_EXAME_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_EXAME_ANOTACAO> PACIENTE_EXAME_ANOTACAO { get; set; }
        public virtual TIPO_EXAME TIPO_EXAME { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual PACIENTE_CONSULTA PACIENTE_CONSULTA { get; set; }
        public virtual LABORATORIO LABORATORIO { get; set; }
        public virtual PACIENTE_SOLICITACAO PACIENTE_SOLICITACAO { get; set; }
    }
}