using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.WorkClasses
{
    public class DTO_CP_RATEIO
    {
        public string NUMERO { get; set; }
        public Nullable<int> ID_CAPA { get; set; }
        public Nullable<int> ID_CECU { get; set; }
        public Nullable<int> VINCULADOS { get; set; }
        public Nullable<int> RATEADOS { get; set; }
        public Nullable<decimal> TOTAL_VINCULADO { get; set; }
        public Nullable<decimal> TOTAL_RATEIO { get; set; }
        public Nullable<decimal> TOTAL_RATEADO{ get; set; }
        public Nullable<decimal> PERCENTUAL { get; set; }
        public string NOME_CC { get; set; }
        public string NUMERO_CC { get; set; }

    }
}
