using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class QuestionarioEpworthViewModel
    {
        [Key]
        public int QUEP_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public Nullable<int> QUEP_IN_LIVRO { get; set; }
        public Nullable<int> QUEP_IN_TV { get; set; }
        public Nullable<int> QUEP_IN_INATIVO { get; set; }
        public Nullable<int> QUEP_IN_CARRO { get; set; }
        public Nullable<int> QUEP_IN_DEITADO { get; set; }
        public Nullable<int> QUEP_IN_CONVERSA { get; set; }
        public Nullable<int> QUEP_IN_ALMOCO { get; set; }
        public Nullable<int> QUEP_IN_VOLANTE { get; set; }
        public Nullable<int> QUEP_IN_RESULTADO { get; set; }
        public Nullable<int> QUEP_IN_ATIVO { get; set; }

        public virtual PACIENTE PACIENTE { get; set; }
    }
}