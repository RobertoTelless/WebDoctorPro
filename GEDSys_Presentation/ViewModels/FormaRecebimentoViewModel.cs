using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class FormaRecebimentoViewModel
    {
        [Key]
        public int FORE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string FORE_NM_FORMA { get; set; }
        public int FORE_IN_ATIVO { get; set; }
        public Nullable<int> FORE_IN_PADRAO { get; set; }
        public Nullable<int> FORE_IN_FIXO { get; set; }

        public String Padrao
        {
            get
            {
                if (FORE_IN_PADRAO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Fixo
        {
            get
            {
                if (FORE_IN_FIXO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONSULTA_RECEBIMENTO> CONSULTA_RECEBIMENTO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}