using System;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacienteFichaViewModel
    {
        public int PAFC_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public System.DateTime PAFC_DT_FICHA { get; set; }
        public string PAFC_NM_TITULO { get; set; }
        public int PAFC_IN_TIPO { get; set; }
        public string PAFC_AQ_ARQUIVO { get; set; }
        public int PAFC_IN_ATIVO { get; set; }
        public int ASSI_CD_ID { get; set; }

        public virtual PACIENTE PACIENTE { get; set; }

    }
}