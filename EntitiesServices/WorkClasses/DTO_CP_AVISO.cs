using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.WorkClasses
{
    public class DTO_CP_AVISO
    {
        public Nullable<int> TIPO { get; set; }
        public Nullable<int> ID_FORN { get; set; }
        public Nullable<int> _ID_USUARIO { get; set; }
        public string NUMERO { get; set; }
        public Nullable<decimal> VALOR { get; set; }
        public Nullable<decimal> SALDO { get; set; }
        public Nullable<System.DateTime> VENCIMENTO { get; set; }
        public Nullable<System.DateTime> PAGAMENTO { get; set; }
        public string PARCELA { get; set; }
        public Nullable<int> TIPO_ENVIO { get; set; }
    }
}
