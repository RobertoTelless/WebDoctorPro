using System;

namespace EntitiesServices.Work_Classes
{
    public class DTO_Paciente_Fisico
    {
        public int PAEF_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public Nullable<int> PACO_CD_ID { get; set; }
        public Nullable<System.DateTime> PAEF_DT_DATA { get; set; }
        public Nullable<int> PAEF_NR_PA_ALTA { get; set; }
        public Nullable<int> PAEF_NR_PA_BAIXA { get; set; }
        public Nullable<decimal> PAEF_NR_PESO { get; set; }
        public Nullable<decimal> PAEF_NR_ALTURA { get; set; }
        public Nullable<decimal> PAEF_VL_IMC { get; set; }
        public Nullable<int> PAEF_IN_ATIVO { get; set; }
        public Nullable<int> PAEF_NR_FREQUENCIA_CARDIACA { get; set; }
        public Nullable<decimal> PAEF_NR_TEMPERATURA { get; set; }
        public Nullable<int> PAEF_IN_DIABETE { get; set; }
        public Nullable<int> PAEF_IN_HIPERTENSAO { get; set; }
        public Nullable<int> PAEF_IN_TABAGISMO { get; set; }
        public Nullable<int> PAEF_IN_VARIZES { get; set; }
        public Nullable<int> PAEF_IN_EPILEPSIA { get; set; }
        public Nullable<int> PAEF_IN_GESTANTE { get; set; }
        public Nullable<int> PAEF_IN_HIPOTENSAO { get; set; }
        public Nullable<int> PAEF_IN_CIRURGIAS { get; set; }
        public string PAEF_TX_CIRURGIAS { get; set; }
        public Nullable<int> PAEF_IN_EXERCICIO_FISICO { get; set; }
        public Nullable<int> PAEF_IN_EXERCICIO_FISICO_FREQUENCIA { get; set; }
        public Nullable<int> PAEF_IN_ALCOOLISMO { get; set; }
        public Nullable<int> PAEF_IN_ALCOOLISMO_FREQUENCIA { get; set; }
        public Nullable<int> PAEF_IN_ANTE_ALERGICO { get; set; }
        public Nullable<int> PAEF_IN_ANTE_ONCOLOGICO { get; set; }
        public Nullable<int> PAEF_IN_ANTICONCEPCIONAL { get; set; }
        public string PAEF_DS_ANTICONCEPCIONAL { get; set; }
        public Nullable<int> PAEF_IN_MARCAPASSO { get; set; }
        public string PAEF_DS_EXAME_FISICO { get; set; }
        public Nullable<int> PAEF_NR_MES_GESTANTE { get; set; }
        public string PAEF_DS_EXERCICIO_FISICO { get; set; }
        public string PAEF_DS_ALCOOLISMO { get; set; }
        public string PAEF_DS_ALERGICO { get; set; }
        public string PAEF_DS_ONCOLOGICO { get; set; }
        public string PAEF_DS_TABAGISMO { get; set; }
        public string PAEF_DS_MARCAPASSO { get; set; }
        public string PAEF_TX_RESULTADOS { get; set; }
        public string PAEF_DS_FICHA_AVALIACAO { get; set; }
        public string PAEF_NM_TIPO_SANGUE { get; set; }

    }
}
