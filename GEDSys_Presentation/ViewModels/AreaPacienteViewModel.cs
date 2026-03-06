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
        [StringLength(250, ErrorMessage = "O NOME DO EXAME deve conter no máximo 250 caracteres.")]
        public string AREA_NM_EXAME { get; set; }
        public Nullable<int> TIEX_CD_ID { get; set; }
        public Nullable<int> AREA_IN_TIPO_EXAME { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DA CONSULTA Deve ser uma data válida")]
        public Nullable<System.DateTime> AREA_DT_DATA_EXAME { get; set; }
        public Nullable<int> LABS_CD_ID { get; set; }
        [StringLength(5000, ErrorMessage = "Os COMENTÁRIOS devem conter no máximo 5000 caracteres.")]
        public string AREA_TX_COMENTARIOS_DOCUMENTO { get; set; }
        public Nullable<int> LOCA_CD_ID { get; set; }
        public Nullable<int> AREA_IN_TIPO_CONSULTA { get; set; }
        public Nullable<int> AREA_IN_VISTA { get; set; }
        public Nullable<int> AREA_IN_PROCESSADA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE PROCESSAMENTO Deve ser uma data válida")]
        public Nullable<System.DateTime> AREA_DT_PROCESSO { get; set; }
        public string AREA_NM_PACIENTE_DUMMY { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE PROCESSAMENTO Deve ser uma data válida")]
        public Nullable<System.DateTime> AREA_DT_DUMMY { get; set; }

        public string NOME_PACIENTE { get; set; }
        public string NOME_PROFISSIONAL { get; set; }
        public string HORARIO { get; set; }
        public string EMAIL_PROFISSIONAL { get; set; }

        public string Vista
        {
            get
            {
                if (AREA_IN_VISTA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public string Processada
        {
            get
            {
                if (AREA_IN_PROCESSADA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public string TipoConsulta
        {
            get
            {
                if (AREA_IN_TIPO_CONSULTA == 1)
                {
                    return "Presencial";
                }
                return "Remota";
            }
        }

        public virtual PACIENTE PACIENTE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AREA_PACIENTE_ANEXO> AREA_PACIENTE_ANEXO { get; set; }
        public virtual LABORATORIO LABORATORIO { get; set; }
        public virtual TIPO_EXAME TIPO_EXAME { get; set; }
        public virtual LOCACAO LOCACAO { get; set; }
    }
}