using System;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class IndicacaoAnexoViewModel
    {
        public int INAN_CD_ID { get; set; }
        public int INDI_CD_ID { get; set; }
        public string INAN_NM_TITULO { get; set; }
        public Nullable<System.DateTime> INAN_DT_ANEXO { get; set; }
        public Nullable<int> INAN_IN_TIPO { get; set; }
        public string INAN_AQ_ARQUIVO { get; set; }
        public Nullable<int> INAN_IN_ATIVO { get; set; }

        public virtual INDICACAO INDICACAO { get; set; }

    }
}