using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ConfiguracaoCalendarioBloqueioViewModel
    {
        [Key]
        public int COCB_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int COCA_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> COCB_DT_BLOQUEIO_INICIO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> COCB_DT_BLOQUEIO_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCB_HR_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCB_HR_FINAL { get; set; }
        public int COCB_IN_ATIVO { get; set; }
        public string COCB_GU_IDENTIFICADOR { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }

        public virtual CONFIGURACAO_CALENDARIO CONFIGURACAO_CALENDARIO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}