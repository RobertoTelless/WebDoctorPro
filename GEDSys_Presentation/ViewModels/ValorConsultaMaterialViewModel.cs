using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ValorConsulta1MaterialViewModel
    {
        [Key]
        public int VCMA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int VACO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PRODUTO obrigatorio")]
        public int PROD_CD_ID { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> VCMA_QN_QUANTIDADE { get; set; }
        public int VCMA_IN_ATIVO { get; set; }

        public virtual PRODUTO PRODUTO { get; set; }
        public virtual VALOR_CONSULTA VALOR_CONSULTA { get; set; }
    }
}