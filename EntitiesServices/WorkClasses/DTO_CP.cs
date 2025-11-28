using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.WorkClasses
{
    public class DTO_CP
    {
        public string FORNECEDOR { get; set; }
        public string CENTRO_CUSTO { get; set; }
        public Nullable<System.DateTime> LANCAMENTO { get; set; }
        public Nullable<System.DateTime> VENCIMENTO { get; set; }
        public Nullable<decimal> VALOR { get; set; }
        public Nullable<decimal> SALDO { get; set; }
        public string DESCRICAO { get; set; }
        public Nullable<int> ATRASO { get; set; }
        public string PARCELA { get; set; }

    }
}
