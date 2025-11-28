using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MedicoEnvioViewModel
    {
        public int MEEV_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public int MEDC_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> MEEV_DT_ENVIO { get; set; }
        [StringLength(5000, ErrorMessage = "O TEXTO DA MENSAGEM deve conter no máximo 5000 caracteres.")]
        public string MEEV_TX_MENSAGEM { get; set; }
        public int MEEV_IN_ATIVO { get; set; }
        public string MEEV_GU_IDENTIFICADOR { get; set; }
        [Required(ErrorMessage = "Campo PACIENTE obrigatorio")]
        public Nullable<int> PACI_CD_ID { get; set; }
        public Nullable<int> TIEN_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TITULO obrigatorio")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "O TEXTO DA MENSAGEM deve conter no minimo 1 e no máximo 250 caracteres.")]
        public string MEEV_NM_TITULO { get; set; }
        public Nullable<int> MEEV_IN_ANAMNESE { get; set; }

        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MEEV_NR_PRESSAO_POSITIVA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> MEEV_IN_IAH { get; set; }
        [StringLength(50, ErrorMessage = "EQUIPAMENTO deve conter no máximo 50 caracteres.")]
        public string MEEV_NM_EQUIPAMENTO { get; set; }
        [StringLength(50, ErrorMessage = "MÁSCARA deve conter no máximo 50 caracteres.")]
        public string MEEV_NM_MASCARA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> MEEV_DT_NOITE_INICIO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> MEEV_DT_NOITE_FINAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> MEEV_IN_NUM_NOITES { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MEEV_NR_PARAM_PRESSAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MEEV_NR_MEDIA_USO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MEEV_IN_PERCENTUAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> MEEV_NR_IAH_RESIDUAL { get; set; }
        [StringLength(5000, ErrorMessage = "SINTOMAS CLÍNICOS deve conter no máximo 5000 caracteres.")]
        public string MEEV_DS_SINTOMAS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> MEEV_IN_IAH_RESIDUAL { get; set; }
        public Nullable<int> MEEV_IN_ENVIADO { get; set; }
        public Nullable<int> METX_CD_ID { get; set; }
        public Nullable<int> MEEV_IN_MODELO { get; set; }

        public string NOME_MEDICO { get; set; }
        public string CRM { get; set; }
        public string MAIL { get; set; }

        public string Modelo
        {
            get
            {
                if (MEEV_IN_MODELO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        public virtual MEDICOS MEDICOS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MEDICOS_ENVIO_ANEXO> MEDICOS_ENVIO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MEDICOS_ENVIO_ANOTACAO> MEDICOS_ENVIO_ANOTACAO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual PACIENTE PACIENTE { get; set; }
        public virtual TIPO_ENVIO TIPO_ENVIO { get; set; }
        public virtual MEDICOS_MENSAGEM MEDICOS_MENSAGEM { get; set; }


    }
}