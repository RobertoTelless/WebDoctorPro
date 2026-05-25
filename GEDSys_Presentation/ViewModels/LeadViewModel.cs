using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP_Condominios_Solution.ViewModels
{
    public class LeadViewModel
    {
        public int LEAD_CD_ID { get; set; }
        public Nullable<System.DateTime> LEAD_DT_ENTRADA { get; set; }
        public string LEAD_NM_NOME { get; set; }
        public string LEAD_EM_EMAIL { get; set; }
        public string LEAD_NR_CELULAR { get; set; }
        public Nullable<int> LEAD_IN_STATUS { get; set; }
        public string LEAD_NR_CPF { get; set; }
        public string LEAD_NR_CNPJ { get; set; }
        public Nullable<System.DateTime> LEAD_DT_NASCIMENTO { get; set; }
        public Nullable<int> SEXO_CD_ID { get; set; }
        public Nullable<int> LEAD_IN_SISTEMA { get; set; }
        public string LEAD_NM_ENDERECO { get; set; }
        public string LEAD_NR_NUMERO { get; set; }
        public string LEAD_NM_COMPLEMENTO { get; set; }
        public string LEAD_NM_BAIRRO { get; set; }
        public string LEAD_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        public string LEAD_NR_CEP { get; set; }
        public Nullable<int> LEAD_IN_ATIVO { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> CRM1_CD_ID { get; set; }
        public Nullable<System.DateTime> LEAD_DT_DUMMY { get; set; }
        public Nullable<int> LEAD_IN_HUMANO { get; set; }
        public string LEAD_NM_SENHA { get; set; }
        public string LEAD_NM_LOGIN { get; set; }

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

    }
}