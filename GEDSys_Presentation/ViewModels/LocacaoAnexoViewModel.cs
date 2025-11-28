using System;
using EntitiesServices.Model;
using Newtonsoft.Json;

namespace ERP_Condominios_Solution.ViewModels
{
    public class LocacaoAnexoViewModel
    {
        public int LOAX_CD_ID { get; set; }
        public int LOCA_CD_ID { get; set; }
        public Nullable<System.DateTime> LOAX_DT_ANEXO { get; set; }
        public string LOAX_NM_TITULO { get; set; }
        public Nullable<int> LOAX_IN_TIPO { get; set; }
        public string LOAX_AQ_ARQUIVO { get; set; }
        public int LOAX_IN_ATIVO { get; set; }

        [JsonIgnore]
        public virtual LOCACAO LOCACAO { get; set; }
    }
}