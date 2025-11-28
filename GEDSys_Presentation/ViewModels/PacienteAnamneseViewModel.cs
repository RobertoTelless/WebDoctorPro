using System;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;
using Newtonsoft.Json;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacienteAnamneseViewModel
    {
        [Key]
        public int PAAM_CD_ID { get; set; }
        public int PACI_CD_ID { get; set; }
        public Nullable<int> PACO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime PAAM_DT_DATA { get; set; }
        [StringLength(200, MinimumLength = 1, ErrorMessage = "ACV deve conter no minimo 1 e no máximo 200 caracteres.")]
        public string PAAN_NM_ACV { get; set; }
        [StringLength(200, MinimumLength = 1, ErrorMessage = "AR deve conter no minimo 1 e no máximo 200 caracteres.")]
        public string PAAN_NM_AR { get; set; }
        [StringLength(5000, ErrorMessage = "ABDOMEM deve conter no máximo 5000 caracteres.")]
        public string PAAN_NM_ABDOMEM { get; set; }
        [StringLength(200, MinimumLength = 1, ErrorMessage = "MMII deve conter no minimo 1 e no máximo 200 caracteres.")]
        public string PAAN_NM_MMII { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "QP deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string PAAM_DS_QP { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "HDA deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string PAAM_DS_HDA { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "HPP deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string PAAM_DS_HPP { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "HFAM deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string PAAM_DS_HFAM { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "HSOCIAL deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string PAAM_DS_HSOCIAL { get; set; }
        [StringLength(5000, ErrorMessage = "DIAGNÓSTICO deve conter no máximo 5000 caracteres.")]
        public string PAAM_DS_DIAGNOSTICO_1 { get; set; }
        [StringLength(200, MinimumLength = 1, ErrorMessage = "DIAGNÓSTICO 2 deve conter no minimo 1 e no máximo 200 caracteres.")]
        public string PAAM_DS_DIAGNOSTICO_2 { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "EXAMES deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string PAAM_TX_EXAMES { get; set; }
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "OBSERVAÇÕES deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string PAAM_TX_TEXTO { get; set; }
        public int PAAM_IN_ATIVO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> PAAM_IN_PREENCHIDA { get; set; }
        public Nullable<System.DateTime> PAAM_DT_COPIA { get; set; }
        public string PAAM_TX_COMPLETA { get; set; }
        public Nullable<System.DateTime> PAAM_DT_ORIGINAL { get; set; }

        [StringLength(50000, MinimumLength = 1, ErrorMessage = "MOTIVO DA CONSULTA deve conter no minimo 1 e no máximo 50000 caracteres.")]
        public string PAAM_DS_MOTIVO_CONSULTA { get; set; }
        [StringLength(50000, ErrorMessage = "HISTÓRIA FAMILIAR deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_HISTORIA_FAMILIAR { get; set; }
        [StringLength(50000, ErrorMessage = "HISTÓRIA SOCIAL deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_HISTORIA_SOCIAL { get; set; }
        [StringLength(50000, ErrorMessage = "AVALIAÇÃO CARDIOLÓGICA deve conter no máximo 50000 caracteres.")]
        public string PAAN_NM_AVALIACAO_CARDIOLOGICA { get; set; }
        [StringLength(5000, ErrorMessage = "AVALIAÇÃO RESPIRATÓRIA deve conter no máximo 5000 caracteres.")]
        public string PAAN_NM_RESPIRATORIO { get; set; }
        [StringLength(50000, ErrorMessage = "MEMBROS INFERIORES deve conter no máximo 50000 caracteres.")]
        public string PAAN_NM_MEMBROS_INFERIORES { get; set; }
        [StringLength(50000, MinimumLength = 1, ErrorMessage = "QUEIXA PRINCIPAL deve conter no minimo 1 e no máximo 50000 caracteres.")]
        public string PAAM_DS_QUEIXA_PRINCIPAL { get; set; }
        [StringLength(50000, ErrorMessage = "DOENÇA ATUAL deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_HISTORIA_DOENCA_ATUAL { get; set; }
        [StringLength(50000, ErrorMessage = "HPP deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_HISTORIA_PATOLOGICA_PROGRESSIVA { get; set; }
        [StringLength(50000, ErrorMessage = "CONDUTA deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_CONDUTA { get; set; }
        [StringLength(50000, ErrorMessage = "OBSERVAÇÕES deve conter no máximo 50000 caracteres.")]
        public string PAAM_TX_OBSERVACOES { get; set; }
        [StringLength(50000, ErrorMessage = "MEDICAMENTOS deve conter no máximo 50000 caracteres.")]
        public string PAAM_NM_MEDICAMENTO { get; set; }
        public Nullable<int> PAAM_IN_ALTERADA { get; set; }
        [StringLength(50000, ErrorMessage = "AVALIAÇÃO CARDIOLÓGICA deve conter no máximo 50000 caracteres.")]
        public string PAAN_NM_AVALIACAO_CARDIOLOGICA_LONG { get; set; }
        [StringLength(50000, ErrorMessage = "AVALIAÇÃO RESPIRATÓRIA deve conter no máximo 50000 caracteres.")]
        public string PAAN_NM_RESPIRATORIO_LONG { get; set; }
        [StringLength(50000, ErrorMessage = "DIAGNÓSTICO deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_DIAGNOSTICO_1_LONG { get; set; }
        [StringLength(50000, ErrorMessage = "DIAGNÓSTICO deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_DIAGNOSTICO_2_LONG { get; set; }

        public string PAAM_DS_MOTIVO_CONSULTA_OLD { get; set; }
        public string PAAM_DS_HISTORIA_FAMILIAR_OLD { get; set; }
        public string PAAM_DS_HISTORIA_SOCIAL_OLD { get; set; }
        public string PAAN_NM_AVALIACAO_CARDIOLOGICA_OLD { get; set; }
        public string PAAN_NM_RESPIRATORIO_OLD { get; set; }
        public string PAAN_NM_MEMBROS_INFERIORES_OLD { get; set; }
        public string PAAM_NM_MEDICAMENTO_OLD { get; set; }
        public string PAAM_DS_QUEIXA_PRINCIPAL_OLD { get; set; }
        public string PAAM_DS_HISTORIA_DOENCA_ATUAL_OLD { get; set; }
        public string PAAM_DS_HISTORIA_PATOLOGICA_PROGRESSIVA_OLD { get; set; }
        public string PAAM_DS_CONDUTA_OLD { get; set; }
        public string PAAM_DS_DIAGNOSTICO_1_OLD { get; set; }
        public string PAAM_TX_OBSERVACOES_OLD { get; set; }
        public string PAAN_NM_ABDOMEM_OLD { get; set; }
        public string PAAN_TX_COMPLETA_OLD { get; set; }

        public System.DateTime PACO_DT_CONSULTA{ get; set; }
        public int PACO_IN_TIPO { get; set; }
        public int PACO_IN_ENCERRADA { get; set; }

        public Nullable<int> PAAM_IN_FLAG_MOTIVO_CONSULTA { get; set; }
        public Nullable<int> PAAM_IN_FLAG__HISTORIA_FAMILIAR { get; set; }
        public Nullable<int> PAAM_IN_FLAG_HISTORIA_SOCIAL { get; set; }
        public Nullable<int> PAAM_IN_FLAG_AVALIACAO_CARDIOLOGICA { get; set; }
        public Nullable<int> PAAM_IN_FLAG_RESPIRATORIO { get; set; }
        public Nullable<int> PAAM_IN_FLAG_ABDOMEM { get; set; }
        public Nullable<int> PAAM_IN_FLAG_MEMBROS_INFERIORES { get; set; }
        public Nullable<int> PAAM_IN_FLAG_QUEIXA_PRINCIPAL { get; set; }
        public Nullable<int> PAAM_IN_FLAG_HISTORIA_DOENCA_ATUAL { get; set; }
        public Nullable<int> PAAM_IN_FLAG_MEDICAMENTO { get; set; }
        public Nullable<int> PAAM_IN_FLAG_HISTORIA_PROGRESSIVA { get; set; }
        public Nullable<int> PAAM_IN_FLAG_DIAGNOSTICO_1 { get; set; }
        public Nullable<int> PAAM_IN_FLAG_DIAGNOSTICO_2 { get; set; }
        [StringLength(50, ErrorMessage = "O NOME CAMPO CUSTOMIZADO DA ANAMNESE deve conter no máximo 50 caracteres.")]
        public string PAAM_NM_CAMPO_1 { get; set; }
        [StringLength(50000, ErrorMessage = "CAMPO CUSTOMIZADO deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_CAMPO_1 { get; set; }
        public string PAAM_DS_CAMPO_1_OLD { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_1 { get; set; }
        [StringLength(50, ErrorMessage = "O NOME CAMPO CUSTOMIZADO DA ANAMNESE deve conter no máximo 50 caracteres.")]
        public string PAAM_NM_CAMPO_2 { get; set; }
        [StringLength(50000, ErrorMessage = "CAMPO CUSTOMIZADO deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_CAMPO_2 { get; set; }
        public string PAAM_DS_CAMPO_2_OLD { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_2 { get; set; }
        [StringLength(50, ErrorMessage = "O NOME CAMPO CUSTOMIZADO DA ANAMNESE deve conter no máximo 50 caracteres.")]
        public string PAAM_NM_CAMPO_3 { get; set; }
        [StringLength(50000, ErrorMessage = "CAMPO CUSTOMIZADO deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_CAMPO_3 { get; set; }
        public string PAAM_DS_CAMPO_3_OLD { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_3 { get; set; }

        [StringLength(50, ErrorMessage = "O NOME CAMPO CUSTOMIZADO DA ANAMNESE deve conter no máximo 50 caracteres.")]
        public string PAAM_NM_CAMPO_4 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_4 { get; set; }
        [StringLength(50000, ErrorMessage = "CAMPO CUSTOMIZADO deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_CAMPO_4 { get; set; }

        [StringLength(50, ErrorMessage = "O NOME CAMPO CUSTOMIZADO DA ANAMNESE deve conter no máximo 50 caracteres.")]
        public string PAAM_NM_CAMPO_5 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_5 { get; set; }
        [StringLength(50000, ErrorMessage = "CAMPO CUSTOMIZADO deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_CAMPO_5 { get; set; }
        public string PAAM_DS_CAMPO_4_OLD { get; set; }
        public string PAAM_DS_CAMPO_5_OLD { get; set; }

        [StringLength(50, ErrorMessage = "O NOME CAMPO CUSTOMIZADO DA ANAMNESE deve conter no máximo 50 caracteres.")]
        public string PAAM_NM_CAMPO_6 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_6 { get; set; }
        [StringLength(50000, ErrorMessage = "CAMPO CUSTOMIZADO deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_CAMPO_6 { get; set; }
        public string PAAM_DS_CAMPO_6_OLD { get; set; }

        [StringLength(50, ErrorMessage = "O NOME CAMPO CUSTOMIZADO DA ANAMNESE deve conter no máximo 50 caracteres.")]
        public string PAAM_NM_CAMPO_7 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_7 { get; set; }
        [StringLength(50000, ErrorMessage = "CAMPO CUSTOMIZADO deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_CAMPO_7 { get; set; }
        public string PAAM_DS_CAMPO_7_OLD { get; set; }

        [StringLength(50, ErrorMessage = "O NOME CAMPO CUSTOMIZADO DA ANAMNESE deve conter no máximo 50 caracteres.")]
        public string PAAM_NM_CAMPO_8 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_8 { get; set; }
        [StringLength(50000, ErrorMessage = "CAMPO CUSTOMIZADO deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_CAMPO_8 { get; set; }
        public string PAAM_DS_CAMPO_8_OLD { get; set; }

        [StringLength(50, ErrorMessage = "O NOME CAMPO CUSTOMIZADO DA ANAMNESE deve conter no máximo 50 caracteres.")]
        public string PAAM_NM_CAMPO_9 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_9 { get; set; }
        [StringLength(50000, ErrorMessage = "CAMPO CUSTOMIZADO deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_CAMPO_9 { get; set; }
        public string PAAM_DS_CAMPO_9_OLD { get; set; }

        [StringLength(50, ErrorMessage = "O NOME CAMPO CUSTOMIZADO DA ANAMNESE deve conter no máximo 50 caracteres.")]
        public string PAAM_NM_CAMPO_10 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_10 { get; set; }
        [StringLength(50000, ErrorMessage = "CAMPO CUSTOMIZADO deve conter no máximo 50000 caracteres.")]
        public string PAAM_DS_CAMPO_10 { get; set; }
        public string PAAM_DS_CAMPO_10_OLD { get; set; }

        // Sono
        [StringLength(5000, ErrorMessage = "PRINCIPAL QUEIXA DO SONO deve conter no máximo 5000 caracteres.")]
        public string PAAM_DS_SONO_PRINCIPAL_QUEIXA { get; set; }
        [StringLength(5000, ErrorMessage = "SINTOMAS deve conter no máximo 5000 caracteres.")]
        public string PAAM_DS_SONO_SINTOMAS { get; set; }
        public Nullable<int> PAAM_DS_SONO_HORARIO_REGULAR { get; set; }
        public Nullable<int> PAAM_DS_SONO_LATENCIA { get; set; }
        [StringLength(500, ErrorMessage = "DURAÇÃO DO SONO deve conter no máximo 500 caracteres.")]
        public string PAAM_DS_SONO_DURACAO { get; set; }
        public Nullable<int> PAAM_DS_SONO_ROTINA_FDS { get; set; }
        public Nullable<int> PAAM_DS_SONO_COCHILOS { get; set; }
        public Nullable<int> PAAM_DS_SONO_TVCAMA { get; set; }
        public Nullable<int> PAAM_DS_SONO_DEITADO_SONO { get; set; }
        public Nullable<int> PAAM_DS_SONO_CELULAR_CAMA { get; set; }
        public Nullable<int> PAAM_DS_SONO_LECAMA { get; set; }
        public Nullable<int> PAAM_DS_SONO_FUMA_NOITE { get; set; }
        [StringLength(500, ErrorMessage = "ÚLTIMO CONSUMO DE ALCOOL deve conter no máximo 500 caracteres.")]
        public string PAAM_DS_SONO_ULTIMO_ALCOOL { get; set; }
        public Nullable<int> PAAM_DS_SONO_REFEICAO_PESADA { get; set; }
        public Nullable<int> PAAM_DS_SONO_TODAS_REFEICOES { get; set; }
        public Nullable<int> PAAM_DS_SONO_CAFE { get; set; }
        public Nullable<int> PAAM_DS_SONO_ALMOCO { get; set; }
        public Nullable<int> PAAM_DS_SONO_LANCHE { get; set; }
        public Nullable<int> PAAM_DS_SONO_JANTAR { get; set; }
        public Nullable<int> PAAM_DS_SONO_EXERCICIO { get; set; }
        [StringLength(500, ErrorMessage = "FREQUENCIA DO EXERCÍCIO deve conter no máximo 500 caracteres.")]
        public string PAAM_DS_SONO_EXERCICIO_FREQ { get; set; }
        [StringLength(500, ErrorMessage = "QUANTIDADE DESPERTA deve conter no máximo 500 caracteres.")]
        public string PAAM_DS_SONO_QUANTAS_DESPERTA { get; set; }
        [StringLength(500, ErrorMessage = "MOTIVOS DESPERTA deve conter no máximo 500 caracteres.")]
        public string PAAM_DS_SONO_MOTIVOS_DESPERTA { get; set; }
        [StringLength(500, ErrorMessage = "TEMPO RETORNO SONO deve conter no máximo 500 caracteres.")]
        public string PAAM_DS_SONO_TEMPO_PEGAR_SONO { get; set; }
        public Nullable<int> PAAM_DS_SONO_DEITADO_PERDE_SONO { get; set; }
        [StringLength(500, ErrorMessage = "URINA NOITE deve conter no máximo 500 caracteres.")]
        public string PAAM_DS_SONO_URINA_NOITE { get; set; }
        public Nullable<int> PAAM_DS_SONO_ENGASGOS { get; set; }
        public Nullable<int> PAAM_DS_SONO_TOSSE { get; set; }
        public Nullable<int> PAAM_DS_SONO_REFLUXO { get; set; }
        public Nullable<int> PAAM_DS_SONO_SUDORESE { get; set; }
        [StringLength(500, ErrorMessage = "POSIÇÃO DORMIR deve conter no máximo 500 caracteres.")]
        public string PAAM_DS_SONO_POSICAO_DORMIR { get; set; }
        public Nullable<int> PAAM_DS_SONO_RANGE { get; set; }
        public Nullable<int> PAAM_DS_SONO_RIGIDEZ_FACE { get; set; }
        public Nullable<int> PAAM_DS_SONO_APNEIA { get; set; }
        public Nullable<int> PAAM_DS_SONO_RONCO { get; set; }
        public Nullable<int> PAAM_DS_SONO_AGRESSIVO { get; set; }
        public Nullable<int> PAAM_DS_SONO_FALA { get; set; }
        public Nullable<int> PAAM_DS_SONO_PESADELO { get; set; }
        public Nullable<int> PAAM_DS_SONO_SONANBULISMO { get; set; }
        public Nullable<int> PAAM_DS_SONO_ENCENACAO { get; set; }
        public Nullable<int> PAAM_DS_SONO_MOVE_MEMBRO { get; set; }
        public Nullable<int> PAAM_DS_SONO_CAIBRAS { get; set; }
        public Nullable<int> PAAM_DS_SONO_ACONCHEGANTE { get; set; }
        public Nullable<int> PAAM_DS_SONO_BARULHO { get; set; }
        public Nullable<int> PAAM_DS_SONO_TEMPERATURA { get; set; }
        public Nullable<int> PAAM_DS_SONO_PESSOAS { get; set; }
        public Nullable<int> PAAM_DS_SONO_ANIMAIS { get; set; }
        [StringLength(5000, ErrorMessage = "ATIVIDADES deve conter no máximo 5000 caracteres.")]
        public string PAAM_DS_SONO_ATIVIDADES { get; set; }
        public Nullable<int> PAAM_DS_SONO_FINANCAS { get; set; }
        public Nullable<int> PAAM_DS_SONO_ACESSO_SAUDE { get; set; }
        public Nullable<int> PAAM_DS_SONO_REPARADOR { get; set; }
        public Nullable<int> PAAM_DS_SONO_SONOLENCIA { get; set; }
        public Nullable<int> PAAM_DS_SONO_BOCA_SECA { get; set; }
        public Nullable<int> PAAM_DS_SONO_DOR_CABECA { get; set; }
        public Nullable<int> PAAM_DS_SONO_CONGESTAO { get; set; }
        public Nullable<int> PAAM_DS_SONO_AZIA { get; set; }
        public Nullable<int> PAAM_DS_SONO_SONOLENCIA_DIURNA { get; set; }
        public Nullable<int> PAAM_DS_SONO_CANSACO { get; set; }
        public Nullable<int> PAAM_DS_SONO_DEFICIT { get; set; }
        public Nullable<int> PAAM_DS_SONO_FADIGA { get; set; }
        public Nullable<int> PAAM_DS_SONO_IRRITA { get; set; }
        public Nullable<int> PAAM_DS_SONO_DOR { get; set; }
        public Nullable<int> PAAM_DS_SONO_TIPO_RESPIRACAO { get; set; }
        public Nullable<int> PAAM_DS_SONO_DISFUNCAO { get; set; }
        public Nullable<int> PAAM_DS_SONO_PONDERAL { get; set; }
        [StringLength(5000, ErrorMessage = "MEDICAMENTOS deve conter no máximo 5000 caracteres.")]
        public string PAAM_DS_SONO_MEDICAMENTOS { get; set; }
        [StringLength(5000, ErrorMessage = "COMORBIDADES deve conter no máximo 5000 caracteres.")]
        public string PAAM_DS_SONO_COMORBIDADES { get; set; }
        [StringLength(5000, ErrorMessage = "CIRURGIAS deve conter no máximo 5000 caracteres.")]
        public string PAAM_DS_SONO_CIRURGIAS { get; set; }
        public Nullable<int> PAAM_DS_SONO_SENSACAO_PERNA { get; set; }
        public Nullable<int> PAAM_DS_SONO_TURNO { get; set; }
        [StringLength(5000, ErrorMessage = "MALLAMPATI deve conter no máximo 5000 caracteres.")]
        public string PAAM_DS_SONO_MALLAMPATI { get; set; }
        [StringLength(5000, ErrorMessage = "OVERLAP deve conter no máximo 5000 caracteres.")]
        public string PAAM_DS_SONO_OVERLAP { get; set; }
        [StringLength(5000, ErrorMessage = "PATOLOGIAS deve conter no máximo 5000 caracteres.")]
        public string PAAM_DS_SONO_PATOLOGIAS { get; set; }
        public string PAAM_TX_TEXTO_LIVRE { get; set; }

        public string PAAM_DS_SONO_PRINCIPAL_QUEIXA_OLD { get; set; }
        public string PAAM_DS_SONO_SINTOMAS_OLD { get; set; }
        public string PAAM_DS_SONO_DURACAO_OLD { get; set; }
        public string PAAM_DS_SONO_ULTIMO_ALCOOL_OLD { get; set; }
        public string PAAM_DS_SONO_EXERCICIO_FREQ_OLD { get; set; }
        public string PAAM_DS_SONO_QUANTAS_DESPERTA_OLD { get; set; }
        public string PAAM_DS_SONO_MOTIVOS_DESPERTA_OLD { get; set; }
        public string PAAM_DS_SONO_TEMPO_PEGAR_SONO_OLD { get; set; }
        public string PAAM_DS_SONO_URINA_NOITE_OLD { get; set; }
        public string PAAM_DS_SONO_POSICAO_DORMIR_OLD { get; set; }
        public string PAAM_DS_SONO_ATIVIDADES_OLD { get; set; }
        public string PAAM_DS_SONO_MEDICAMENTOS_OLD { get; set; }
        public string PAAM_DS_SONO_COMORBIDADES_OLD { get; set; }
        public string PAAM_DS_SONO_CIRURGIAS_OLD { get; set; }
        public string PAAM_DS_SONO_MALLAMPATI_OLD { get; set; }
        public string PAAM_DS_SONO_OVERLAP_OLD { get; set; }
        public string PAAM_DS_SONO_PATOLOGIAS_OLD { get; set; }
        public string PAAM_TX_TEXTO_LIVRE_OLD { get; set; }
        public Nullable<int> PAAM_DS_SONO_DEFICIT_MEMORIA { get; set; }
        public string PAAM_DS_SONO_EXERCICIO_HORARIO { get; set; }
        [StringLength(500, ErrorMessage = "HORÁRIO REGULAR deve conter no máximo 500 caracteres.")]
        public string PAAM_DS_SONO_HORARIO_REGULAR_NOVO { get; set; }
        public string PAAM_DS_SONO_HORARIO_REGULAR_NOVO_OLD { get; set; }
        [StringLength(500, ErrorMessage = "LATENCIA deve conter no máximo 500 caracteres.")]
        public string PAAM_DS_SONO_LATENCIA_NOVO { get; set; }
        public string PAAM_DS_SONO_LATENCIA_NOVO_OLD { get; set; }
        public Nullable<int> PAAM_DS_SONO_DEFICIT_CONCENTRA { get; set; }
        public Nullable<int> PAAM_DS_SONO_DEFICIT_MEMO { get; set; }
        public Nullable<int> PAAM_DS_SONO_RIGIDEZ_FACE_OUTROS { get; set; }
        [StringLength(1000, ErrorMessage = "POLISSONOGRAFIA REGULAR deve conter no máximo 1000 caracteres.")]
        public string PAAM_DS_SONO_POLISONO { get; set; }
        public string PAAM_DS_SONO_POLISONO_OLD { get; set; }

        // Automacao
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
                if (PAAM_IN_PREENCHIDA == 1)
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