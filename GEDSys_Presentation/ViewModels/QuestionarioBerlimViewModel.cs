using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class QuestionarioBerlimViewModel
    {
        [Key]
        public int QUBE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public Nullable<int> QUBE_IN_RONCO { get; set; }
        public Nullable<int> QUBE_IN_TIPO_RONCO { get; set; }
        public Nullable<int> QUBE_IN_FREQUENCIA_RONCO { get; set; }
        public Nullable<int> QUBE_IN_INCOMODA_RONCO { get; set; }
        public Nullable<int> QUBE_IN_PARA_RESPIRA { get; set; }
        public Nullable<int> QUBE_IN_CANSACO { get; set; }
        public Nullable<int> QUBE_IN_CANSACO_ACORDADO { get; set; }
        public Nullable<int> QUBE_IN_COCHILO { get; set; }
        public Nullable<int> QUBE_IN_PRESSAO { get; set; }
        public Nullable<decimal> QUBE_NR_IMC { get; set; }
        public Nullable<int> QUBE_IN_CATEGORIA_1 { get; set; }
        public Nullable<int> QUBE_IN_CATEGORIA_2 { get; set; }
        public Nullable<int> QUBE_IN_CATEGORIA_3 { get; set; }
        public Nullable<int> QUBE_IN_PONTUACAO { get; set; }

        public virtual PACIENTE PACIENTE { get; set; }
    }
}