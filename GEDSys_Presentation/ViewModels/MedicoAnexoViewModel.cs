using System;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class MedicoAnexoViewModel
    {
        public int MVAN_CD_ID { get; set; }
        public int MEEV_CD_ID { get; set; }
        public Nullable<System.DateTime> MVAN_DT_ANEXO { get; set; }
        public string MVAN_NM_TITULO { get; set; }
        public Nullable<int> MVAN_IN_TIPO { get; set; }
        public string MVAN_AQ_ARQUIVO { get; set; }
        public int MVAN_IN_ATIVO { get; set; }

        public virtual MEDICOS_ENVIO MEDICOS_ENVIO { get; set; }

    }
}