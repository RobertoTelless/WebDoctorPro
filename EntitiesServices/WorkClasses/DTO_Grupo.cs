using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Grupo
    {
        public int GRUP_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> TIPA_CD_ID { get; set; }
        public Nullable<int> SEXO_CD_ID { get; set; }
        public string GRUP_NM_NOME { get; set; }
        public Nullable<System.DateTime> GRUP_DT_CADASTRO { get; set; }
        public string GRUP_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        public Nullable<System.DateTime> GRUP_DT_NASCIMENTO { get; set; }
        public string GRUP_NR_DIA { get; set; }
        public string GRUP_NR_MES { get; set; }
        public string GRUP_NR_ANO { get; set; }
        public Nullable<int> GRUP_IN_ATIVO { get; set; }
    }
}
