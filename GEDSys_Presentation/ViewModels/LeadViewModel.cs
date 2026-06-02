using EntitiesServices.Attributes;
using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP_Condominios_Solution.ViewModels
{
    public class LeadViewModel
    {
        public int LEAD_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LEAD_DT_ENTRADA { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 100 caracteres.")]
        public string LEAD_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo E-MAIL obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 caracteres e no máximo 100 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string LEAD_EM_EMAIL { get; set; }
        [StringLength(30, ErrorMessage = "O CELULAR deve conter no máximo 30 caracteres.")]
        public string LEAD_NR_CELULAR { get; set; }
        public Nullable<int> LEAD_IN_STATUS { get; set; }
        [StringLength(30, ErrorMessage = "O CPF deve conter no máximo 30 caracteres.")]
        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        public string LEAD_NR_CPF { get; set; }
        [StringLength(30, ErrorMessage = "O CNPJ deve conter no máximo 30 caracteres.")]
        [CustomValidationCNPJ(ErrorMessage = "CNPJ inválido")]
        public string LEAD_NR_CNPJ { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LEAD_DT_NASCIMENTO { get; set; }
        public Nullable<int> SEXO_CD_ID { get; set; }
        public Nullable<int> LEAD_IN_SISTEMA { get; set; }
        [StringLength(100, ErrorMessage = "O ENDEREÇO deve conter no máximo 100 caracteres.")]
        public string LEAD_NM_ENDERECO { get; set; }
        [StringLength(30, ErrorMessage = "O NUMERO deve conter no máximo 30 caracteres.")]
        public string LEAD_NR_NUMERO { get; set; }
        [StringLength(30, ErrorMessage = "O COMPLEMENTO deve conter no máximo 30 caracteres.")]
        public string LEAD_NM_COMPLEMENTO { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO deve conter no máximo 50 caracteres.")]
        public string LEAD_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve conter no máximo 50 caracteres.")]
        public string LEAD_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        [StringLength(30, ErrorMessage = "O CEP deve conter no máximo 30 caracteres.")]
        public string LEAD_NR_CEP { get; set; }
        public Nullable<int> LEAD_IN_ATIVO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> CRM1_CD_ID { get; set; }
        public Nullable<System.DateTime> LEAD_DT_DUMMY { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LEAD_DT_FECHAMENTO { get; set; }
        [StringLength(5000, ErrorMessage = "AS OBSERVAÇÕES devem conter no máximo 5000 caracteres.")]
        public string LEAD_TX_OBSERVACOES { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LEAD_DT_MOVIMENTO { get; set; }
        [StringLength(300, ErrorMessage = "A DESCRIÇÂO deve conter no máximo 300 caracteres.")]
        public string LEAD_DS_DESCRICAO { get; set; }
        public Nullable<int> LEAD_IN_ENVIOS { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LEAD_DT_EXCLUSAO { get; set; }
        public string LEAD_GU_IDENTIFICADOR { get; set; }
        public string LEAD_DS_RESUMO_MOVIMENTO { get; set; }
        public string LEAD_DS_MOTIVO_EXCLUSAO { get; set; }

        public String Status
        {
            get
            {
                if (LEAD_IN_STATUS == 0)
                {
                    return "Em Análise";
                }
                if (LEAD_IN_STATUS == 1)
                {
                    return "Qualificado";
                }
                if (LEAD_IN_STATUS == 2)
                {
                    return "Convertido";
                }
                if (LEAD_IN_STATUS == 3)
                {
                    return "Perdido";
                }
                return "Excluido";
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LEAD_ANEXO> LEAD_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LEAD_ANOTACAO> LEAD_ANOTACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LEAD_CRM> LEAD_CRM { get; set; }
        public virtual SEXO SEXO { get; set; }
        public virtual UF UF { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM> CRM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FUNIL> FUNIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PROSPECTA_MAIL> PROSPECTA_MAIL { get; set; }

    }
}