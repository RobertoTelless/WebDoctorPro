using System;

namespace ERP_Condominios_Solution.ViewModels
{
    public class FaixaHoraViewModel
    {
        public Nullable<System.TimeSpan> INICIO { get; set; }
        public Nullable<System.TimeSpan> FINAL { get; set; }
        public String FAIXA { get; set; }
    }
}