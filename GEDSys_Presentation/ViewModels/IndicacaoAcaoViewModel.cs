using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class IndicacaoAcaoViewModel
    {
        [Key]
        public int INAC_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int INDI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE AÇÃO obrigatorio")]
        public Nullable<int> TIAC_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        [Required(ErrorMessage = "Campo DATA INICIAL obrigatorio")]
        public Nullable<System.DateTime> INAC_DT_INICIO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> INAC_DT_FINAL { get; set; }
        [StringLength(5000, ErrorMessage = "DESCRIÇÃO DA AÇÃO deve conter no máximo 5000 caracteres.")]
        public string INAC_TX_ACAO { get; set; }
        [Required(ErrorMessage = "Campo STATUS obrigatorio")]
        public Nullable<int> INAC_IN_STATUS { get; set; }
        public int INAC_IN_ATIVO { get; set; }
        [StringLength(250, MinimumLength = 2, ErrorMessage = "NOME DA AÇÃO deve conter no minimo 2 e no máximo 250 caracteres.")]
        public string INAC_NM_NOME { get; set; }

        public String StatusAcao
        {
            get
            {
                if (INAC_IN_STATUS == 1)
                {
                    return "Em Andamento";
                }
                if (INAC_IN_STATUS == 2)
                {
                    return "Pendente";
                }
                if (INAC_IN_STATUS == 3)
                {
                    return "Cancelada";
                }
                return "Encerrada";
            }
        }

        public virtual INDICACAO INDICACAO { get; set; }
        public virtual TIPO_ACAO TIPO_ACAO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}