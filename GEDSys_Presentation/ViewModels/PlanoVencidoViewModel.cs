using System;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PlanoVencidoViewModel
    {
        public DateTime? DataMensagem { get; set; }
        public Int32 Plano { get; set; }
        public Int32 Assinante { get; set; }
        public DateTime? Vencimento { get; set; }
        public Int32 Tipo { get; set; }
    }
}