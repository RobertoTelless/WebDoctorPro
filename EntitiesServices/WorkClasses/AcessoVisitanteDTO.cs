using System;

namespace EntitiesServices.Work_Classes
{
    public class AcessoVisitanteDTO
    {
        public string IP { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string Provedor { get; set; }
        public DateTime DataAcesso { get; set; }
        public Int32 Sistema { get; set; }
    }
}
