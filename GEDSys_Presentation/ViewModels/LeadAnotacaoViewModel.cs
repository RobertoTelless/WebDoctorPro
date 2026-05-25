using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class LeadAnotacaoViewModel
    {
        [Key]
        public int LEAN_CD_ID { get; set; }
        public int LEAD_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<System.DateTime> LEAN_DT_ANOTACAO { get; set; }
        public string LEAN_TX_ANOTACAO { get; set; }
        public Nullable<int> LEAN_IN_ATIVO { get; set; }

        public virtual LEAD LEAD { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}