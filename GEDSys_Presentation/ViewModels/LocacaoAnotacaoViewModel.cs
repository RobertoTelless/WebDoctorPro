using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;
using Newtonsoft.Json;

namespace ERP_Condominios_Solution.ViewModels
{
    public class LocacaoAnotacaoViewModel
    {
        [Key]
        public int LOAN_CD_ID { get; set; }
        public int LOCA_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LOAN_DT_ANOTACAO { get; set; }
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O TEXTO deve conter no minimo 1 e no máximo 500 caracteres.")]
        public string LOAN_TX_ANOTACAO { get; set; }
        public int LOAN_IN_ATIVO { get; set; }

        [JsonIgnore]
        public virtual LOCACAO LOCACAO { get; set; }
        [JsonIgnore]
        public virtual USUARIO USUARIO { get; set; }
    }
}