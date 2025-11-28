using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Medicamento
    {
        public int MEDI_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        public Nullable<int> TIFO_CD_ID { get; set; }
        public string MEDI_NM_MEDICAMENTO { get; set; }
        public string MEDI_NM_GENERICO { get; set; }
        public string MEDI_NM_APRESENTACAO { get; set; }
        public string MEDI_NM_LABORATORIO { get; set; }
        public int MEDI_IN_ATIVO { get; set; }
        public string MEDI_DS_DESCRICAO { get; set; }
        public string MEDI_DS_POSOLOGIA { get; set; }
        public Nullable<int> TICO_CD_ID { get; set; }

    }
}
