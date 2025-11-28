using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class GrupoContatoViewModel
    {
        [Key]
        public int GRCL_CD_ID { get; set; }
        public Nullable<int> GRUP_CD_ID { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }
        public Nullable<int> GRCL_IN_ATIVO { get; set; }
        public Nullable<int> PACI__CD_ID { get; set; }

        public virtual GRUPO_PAC GRUPO_PAC { get; set; }
        public virtual PACIENTE PACIENTE { get; set; }
    }
}