using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class FaleConoscoViewModel
    {
        public String Telefone { get; set; }
        public String WhatsApp { get; set; }
        public String EMail { get; set; }
        public String Nome { get; set; }
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public String Resposta { get; set; }

        public String Mensagem { get; set; }
        public Nullable<int> TipoMensagem { get; set; }
        public Nullable<int> Assunto { get; set; }
        [StringLength(20, ErrorMessage = "CPF deve conter no máximo 20 caracteres.")]
        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        public String CPF { get; set; }
        [StringLength(14, MinimumLength = 13, ErrorMessage = "CELULAR deve ter 14 caracteres (99)99999-9999")]
        public String Celular { get; set; }
        [StringLength(20, ErrorMessage = "CNPJ deve conter no máximo 20 caracteres.")]
        [CustomValidationCNPJ(ErrorMessage = "CNPJ inválido")]
        public String CNPJ { get; set; }
        public Nullable<int> Tipo { get; set; }
        public String Razao { get; set; }
        [StringLength(13, MinimumLength = 13, ErrorMessage = "TELEFONE deve ter 13 caracteres (99)9999-9999")]
        public String TelefoneFixo { get; set; }
        public String Endereco { get; set; }
        public String Numero { get; set; }
        public String Complemento { get; set; }
        public String Bairro { get; set; }
        public String Cidade { get; set; }
        public String CEP { get; set; }
        public Int32 UF { get; set; }

        public Int32 Demo { get; set; }
        public String Login { get; set; }
        public String CPFDemo { get; set; }
        public Nullable<int> Plano { get; set; }
        public String Especialidade { get; set; }
        public String NomePlano { get; set; }
        public String LoginFinal { get; set; }
        public String Senha { get; set; }
        public Nullable<System.DateTime> Inicio { get; set; }
        public Nullable<System.DateTime> Termino { get; set; }
        public String Comeco { get; set; }
        public String Fim { get; set; }
        [StringLength(10, ErrorMessage = "LOGIN deve conter no máximo 10 caracteres.")]
        public String LoginBase { get; set; }
        public String SenhaBase { get; set; }
        public String SenhaBaseConfirma { get; set; }
        public Int32 TipoAssinatura { get; set; }
        public String CEPBase { get; set; }
        public Decimal Preco { get; set; }
        public String TipoPessoa { get; set; }
        public String UFSigla { get; set; }
        public String Informacoes { get; set; }

    }
}