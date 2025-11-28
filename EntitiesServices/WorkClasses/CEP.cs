using System;

namespace EntitiesServices.Work_Classes
{
    // Classe para mapear o resultado do ViaCEP
    public class CepData
    {
        public string cep { get; set; }
        public string logradouro { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string localidade { get; set; }
        public string uf { get; set; }
        public bool erro { get; set; } // Propriedade indicando erro
    }
}
