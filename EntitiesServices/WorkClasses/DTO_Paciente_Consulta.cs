using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Consulta
    {
        public int PACO_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public System.DateTime PACO_DT_CONSULTA { get; set; }
        public int PACO_IN_ATIVO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<System.TimeSpan> PACO_HR_INICIO { get; set; }
        public Nullable<System.TimeSpan> PACO_HR_FINAL { get; set; }
        public Nullable<int> PACO_IN_ENCERRADA { get; set; }
        public Nullable<int> PACO_IN_TIPO { get; set; }
        public string PACO_TX_RESUMO { get; set; }
        public Nullable<System.DateTime> PACO_DT_PROXIMA { get; set; }
        public Nullable<System.DateTime> PACO_DT_DUMMY { get; set; }
        public Nullable<int> PACO_IN_CONFIRMADA { get; set; }
        public Nullable<int> VACO_CD_ID { get; set; }
        public Nullable<int> PACO_IN_RECORRENTE { get; set; }
        public Nullable<int> PACO_IN_RECEBE { get; set; }
        public string PACO_TX_JUSTIFICATIVA_CANCELA { get; set; }
    }
}
