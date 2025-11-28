using System;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PagamentoPagBankViewModel
    {
        public string NomeCliente { get; set; }
        public string CpfCliente { get; set; }
        public decimal ValorTotal { get; set; }
        public string DescricaoItem { get; set; }
        public int QuantidadeItem { get; set; }
        public string CelularCliente { get; set; }
        public string EMailCliente { get; set; }
    }
}