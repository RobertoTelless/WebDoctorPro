using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;
using EntitiesServices.Attributes;
using System.Web.Helpers;
using Newtonsoft.Json;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacienteExameFisicoViewModel
    {
        [Key]
        public int PAEF_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public Nullable<int> PACO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAEF_DT_DATA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PAEF_NR_PA_ALTA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PAEF_NR_PA_BAIXA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PAEF_NR_PESO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PAEF_NR_ALTURA { get; set; }
        public Nullable<decimal> PAEF_VL_IMC { get; set; }
        public Nullable<int> PAEF_IN_ATIVO { get; set; }
        public Nullable<int> PAEF_NR_FREQUENCIA_CARDIACA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PAEF_NR_TEMPERATURA { get; set; }
        public Nullable<int> PAEF_IN_DIABETE { get; set; }
        public Nullable<int> PAEF_IN_HIPERTENSAO { get; set; }
        public Nullable<int> PAEF_IN_TABAGISMO { get; set; }
        public Nullable<int> PAEF_IN_VARIZES { get; set; }
        public Nullable<int> PAEF_IN_EPILEPSIA { get; set; }
        public Nullable<int> PAEF_IN_GESTANTE { get; set; }
        public Nullable<int> PAEF_IN_HIPOTENSAO { get; set; }
        public Nullable<int> PAEF_IN_CIRURGIAS { get; set; }
        [StringLength(50000, ErrorMessage = "CIRURGIAS deve conter no máximo 50000 caracteres.")]
        public string PAEF_TX_CIRURGIAS { get; set; }
        public Nullable<int> PAEF_IN_EXERCICIO_FISICO { get; set; }
        public Nullable<int> PAEF_IN_EXERCICIO_FISICO_FREQUENCIA { get; set; }
        public Nullable<int> PAEF_IN_ALCOOLISMO { get; set; }
        public Nullable<int> PAEF_IN_ALCOOLISMO_FREQUENCIA { get; set; }
        public Nullable<int> PAEF_IN_ANTE_ALERGICO { get; set; }
        public Nullable<int> PAEF_IN_ANTE_ONCOLOGICO { get; set; }
        public Nullable<int> PAEF_IN_ANTICONCEPCIONAL { get; set; }
        [StringLength(50000, ErrorMessage = "ANTICONCEPCIONAL deve conter no máximo 50000 caracteres.")]
        public string PAEF_DS_ANTICONCEPCIONAL { get; set; }
        public Nullable<int> PAEF_IN_MARCAPASSO { get; set; }
        [StringLength(5000, ErrorMessage = "EXAME FÍSICO deve conter no máximo 5000 caracteres.")]
        public string PAEF_DS_EXAME_FISICO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PAEF_NR_MES_GESTANTE { get; set; }
        [StringLength(250, ErrorMessage = "FREQUENCIA DO EXERCÍCIO deve conter no máximo 250 caracteres.")]
        public string PAEF_DS_EXERCICIO_FISICO { get; set; }
        [StringLength(250, ErrorMessage = "FREQUENCIA DO ALCOOLISMO deve conter no máximo 250 caracteres.")]
        public string PAEF_DS_ALCOOLISMO { get; set; }
        [StringLength(500, ErrorMessage = "ANTECEDENTES ALÉRGICOS deve conter no máximo 500 caracteres.")]
        public string PAEF_DS_ALERGICO { get; set; }
        [StringLength(500, ErrorMessage = "ANTECEDENTES ONCOLÓGICOS deve conter no máximo 500 caracteres.")]
        public string PAEF_DS_ONCOLOGICO { get; set; }
        [StringLength(250, ErrorMessage = "FREQUENCIA DO TABAGISMO deve conter no máximo 250 caracteres.")]
        public string PAEF_DS_TABAGISMO { get; set; }
        [StringLength(500, ErrorMessage = "OBSERVAÇÕES DO MARCAPASSO deve conter no máximo 500 caracteres.")]
        public string PAEF_DS_MARCAPASSO { get; set; }
        public Nullable<int> PAEF_IN_PREENCHIDO { get; set; }
        public Nullable<System.DateTime> PAEF_DT_ORIGINAL { get; set; }

        public System.DateTime PACO_DT_CONSULTA { get; set; }
        public int PACO_IN_TIPO { get; set; }
        public int PACO_IN_ENCERRADA { get; set; }
        public Nullable<System.DateTime> PAEF_DT_COPIA { get; set; }
        [StringLength(50000, ErrorMessage = "RESULTADOS DE EXAMES deve conter no máximo 50000 caracteres.")]
        public string PAEF_TX_RESULTADOS { get; set; }
        public Nullable<int> Idade { get; set; }
        public string PAEF_TX_RESULTADOS_OLD { get; set; }
        public string PAEF_DS_FICHA_AVALIACAO { get; set; }

        [StringLength(50000, ErrorMessage = "FREQUENCIA DO EXERCÍCIO deve conter no máximo 50000 caracteres.")]
        public string PAEF_DS_EXERCICIO_FISICO_LONG { get; set; }
        [StringLength(50000, ErrorMessage = "FREQUENCIA DO ALCOOLISMO deve conter no máximo 50000 caracteres.")]
        public string PAEF_DS_ALCOOLISMO_LONG { get; set; }
        [StringLength(50000, ErrorMessage = "ANTECEDENTES ALÉRGICOS deve conter no máximo 50000 caracteres.")]
        public string PAEF_DS_ALERGICO_LONG { get; set; }
        [StringLength(50000, ErrorMessage = "ANTECEDENTES ONCOLÓGICOS deve conter no máximo 50000 caracteres.")]
        public string PAEF_DS_ONCOLOGICO_LONG { get; set; }
        [StringLength(50000, ErrorMessage = "FREQUENCIA DO TABAGISMO deve conter no máximo 50000 caracteres.")]
        public string PAEF_DS_TABAGISMO_LONG { get; set; }
        [StringLength(50000, ErrorMessage = "MARCAPASSO deve conter no máximo 50000 caracteres.")]
        public string PAEF_DS_MARCAPASSO_LONG { get; set; }

        public String TipoConsulta
        {
            get
            {
                if (PACO_IN_TIPO == 1)
                {
                    return "Presencial";
                }
                return "Remota";
            }
        }
        public String Encerrada
        {
            get
            {
                if (PACO_IN_ENCERRADA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Preenchida
        {
            get
            {
                if (PAEF_IN_PREENCHIDO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        public bool Diabete
        {
            get
            {
                if (PAEF_IN_DIABETE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAEF_IN_DIABETE = (value == true) ? 1 : 0;
            }
        }
        public bool Hipertensao
        {
            get
            {
                if (PAEF_IN_HIPERTENSAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAEF_IN_HIPERTENSAO = (value == true) ? 1 : 0;
            }
        }
        public bool Tabagismo
        {
            get
            {
                if (PAEF_IN_TABAGISMO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAEF_IN_TABAGISMO = (value == true) ? 1 : 0;
            }
        }
        public bool Varizes
        {
            get
            {
                if (PAEF_IN_VARIZES == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAEF_IN_VARIZES = (value == true) ? 1 : 0;
            }
        }
        public bool Epilepsia
        {
            get
            {
                if (PAEF_IN_EPILEPSIA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAEF_IN_EPILEPSIA = (value == true) ? 1 : 0;
            }
        }
        public bool Gestante
        {
            get
            {
                if (PAEF_IN_GESTANTE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAEF_IN_GESTANTE = (value == true) ? 1 : 0;
            }
        }
        public bool Hipotensao
        {
            get
            {
                if (PAEF_IN_HIPOTENSAO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAEF_IN_HIPOTENSAO = (value == true) ? 1 : 0;
            }
        }
        public bool Cirurgia
        {
            get
            {
                if (PAEF_IN_CIRURGIAS == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAEF_IN_CIRURGIAS = (value == true) ? 1 : 0;
            }
        }
        public bool ExercicioFisico
        {
            get
            {
                if (PAEF_IN_EXERCICIO_FISICO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAEF_IN_EXERCICIO_FISICO = (value == true) ? 1 : 0;
            }
        }
        public bool Alcoolismo
        {
            get
            {
                if (PAEF_IN_ALCOOLISMO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAEF_IN_ALCOOLISMO = (value == true) ? 1 : 0;
            }
        }
        public bool AntecedenteAlergico
        {
            get
            {
                if (PAEF_IN_ANTE_ALERGICO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAEF_IN_ANTE_ALERGICO = (value == true) ? 1 : 0;
            }
        }
        public bool AntecedenteOncologico
        {
            get
            {
                if (PAEF_IN_ANTE_ONCOLOGICO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAEF_IN_ANTE_ONCOLOGICO = (value == true) ? 1 : 0;
            }
        }
        public bool Anticoncepcional
        {
            get
            {
                if (PAEF_IN_ANTICONCEPCIONAL == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAEF_IN_ANTICONCEPCIONAL = (value == true) ? 1 : 0;
            }
        }
        public bool Marcapasso
        {
            get
            {
                if (PAEF_IN_MARCAPASSO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PAEF_IN_MARCAPASSO = (value == true) ? 1 : 0;
            }
        }

        public String HipertensaoT
        {
            get
            {
                if (PAEF_IN_HIPERTENSAO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String HipotensaoT
        {
            get
            {
                if (PAEF_IN_HIPOTENSAO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String DiabeteT
        {
            get
            {
                if (PAEF_IN_DIABETE == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String VarizerT
        {
            get
            {
                if (PAEF_IN_VARIZES == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String EpilepsiaT
        {
            get
            {
                if (PAEF_IN_EPILEPSIA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String TabagismoT
        {
            get
            {
                if (PAEF_IN_TABAGISMO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String GestanteT
        {
            get
            {
                if (PAEF_IN_GESTANTE == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String AlcoolismoT
        {
            get
            {
                if (PAEF_IN_ALCOOLISMO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String ExercicioFisicoT
        {
            get
            {
                if (PAEF_IN_EXERCICIO_FISICO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String AnticoncepcionalT
        {
            get
            {
                if (PAEF_IN_ANTICONCEPCIONAL == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String MarcapassoT
        {
            get
            {
                if (PAEF_IN_MARCAPASSO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String CirurgiaT
        {
            get
            {
                if (PAEF_IN_CIRURGIAS == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String AntecedenteAlergicoT
        {
            get
            {
                if (PAEF_IN_ANTE_ALERGICO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String AntecedenteOncologicoT
        {
            get
            {
                if (PAEF_IN_ANTE_ONCOLOGICO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }


        [JsonIgnore]
        public virtual PACIENTE PACIENTE { get; set; }
        [JsonIgnore]
        public virtual PACIENTE_CONSULTA PACIENTE_CONSULTA { get; set; }
        [JsonIgnore]
        public virtual USUARIO USUARIO { get; set; }
    }
}