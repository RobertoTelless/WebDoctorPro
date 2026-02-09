using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ValorConsulta1ViewModel
    {
        [Key]
        public int VACO_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE CONSULTA obrigatorio")]
        public Nullable<int> TIVL_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE REFERÊNCIA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE REFERÊNCIA deve ser uma data válida")]
        public Nullable<System.DateTime> VACO_DT_REFERENCIA { get; set; }
        public string VACO_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> VACO_NR_VALOR { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> VACO_NR_DESCONTO { get; set; }
        public int VACO_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo PADRÃO obrigatorio")]
        public Nullable<int> VACO_IN_PADRAO { get; set; }
        public string VACO_NM_EXIBE { get; set; }
        public Nullable<int> VACO_IN_MATERIAL { get; set; }

        public String Consumo
        {
            get
            {
                if (VACO_IN_MATERIAL == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        public string PACI_NM_NOME { get; set; }
        public string PACI_NR_CPF { get; set; }
        public string PACI_NR_CELULAR { get; set; }
        public string PACI_NR_TELEFONE { get; set; }
        public Nullable<System.DateTime> PACI_DT_NASCIMENTO { get; set; }

        public Nullable<System.DateTime> PACO_DT_CONSULTA { get; set; }
        public Nullable<System.TimeSpan> PACO_HR_INICIO { get; set; }
        public Nullable<System.TimeSpan> PACO_HR_FINAL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONSULTA_RECEBIMENTO> CONSULTA_RECEBIMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE> PACIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_CONSULTA> PACIENTE_CONSULTA { get; set; }
        public virtual TIPO_VALOR_CONSULTA TIPO_VALOR_CONSULTA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VALOR_CONSULTA_MATERIAL> VALOR_CONSULTA_MATERIAL { get; set; }

    }
}