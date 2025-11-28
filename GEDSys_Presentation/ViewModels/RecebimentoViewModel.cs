using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class RecebimentoViewModel
    {
        public int CORE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<int> PACO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE RECEBIMENTO obrigatorio")]
        public Nullable<int> VACO_CD_ID { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CORE_VL_VALOR { get; set; }
        [Required(ErrorMessage = "Campo FORMA DE RECEBIMENTO obrigatorio")]
        public Nullable<int> FORE_CD_ID { get; set; }
        public Nullable<int> SERV_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> CORE_DT_RECEBIMENTO { get; set; }
        public int CORE_IN_ATIVO { get; set; }
        public Nullable<int> CORE_IN_CONFERIDO { get; set; }
        public Nullable<int> VASE_CD_ID { get; set; }
        public Nullable<int> VACV_CD_ID { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CORE_VL_SERVICO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CORE_VL_CONVENIO { get; set; }
        [Required(ErrorMessage = "Campo PACIENTE obrigatorio")]
        public Nullable<int> PACI_CD_ID { get; set; }
        [StringLength(150, ErrorMessage = "NOME DO RECEBIMENTO deve conter no máximo 150 caracteres.")]
        public string CORE_NM_RECEBIMENTO { get; set; }
        public Nullable<System.DateTime> CORE_DT_DUMMY { get; set; }
        public string CORE_GU_GUID { get; set; }

        public virtual FORMA_RECEBIMENTO FORMA_RECEBIMENTO { get; set; }
        public virtual PACIENTE PACIENTE { get; set; }
        public virtual PACIENTE_CONSULTA PACIENTE_CONSULTA { get; set; }
        public virtual TIPO_SERVICO_CONSULTA TIPO_SERVICO_CONSULTA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual VALOR_CONSULTA VALOR_CONSULTA { get; set; }
        public virtual VALOR_CONVENIO VALOR_CONVENIO { get; set; }
        public virtual VALOR_SERVICO VALOR_SERVICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RECEBIMENTO_ANEXO> RECEBIMENTO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RECEBIMENTO_ANOTACAO> RECEBIMENTO_ANOTACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RECEBIMENTO_RECIBO> RECEBIMENTO_RECIBO { get; set; }
    }
}