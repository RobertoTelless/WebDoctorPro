using System;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class LeadAnexoViewModel
    {
        public int LEAX_CD_ID { get; set; }
        public int LEAD_CD_ID { get; set; }
        public Nullable<System.DateTime> LEAX_DT_ANEXO { get; set; }
        public string LEAX_NM_TITULO { get; set; }
        public Nullable<int> LEAX_IN_TIPO { get; set; }
        public string LEAX_AQ_ARQUIVO { get; set; }
        public Nullable<int> LEAX_IN_ATIVO { get; set; }

        public virtual LEAD LEAD { get; set; }

    }
}