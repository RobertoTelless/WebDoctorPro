using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Solicitacao_Modelo
    {
        public int SOLI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public int TIEX_CD_ID { get; set; }
        public string SOLI_NM_TITULO { get; set; }
        public string SOLI_NM_INDICACAO { get; set; }
        public string SOLI_DS_DESCRICAO { get; set; }
        public int SOLI_IN_ATIVO { get; set; }

    }
}
