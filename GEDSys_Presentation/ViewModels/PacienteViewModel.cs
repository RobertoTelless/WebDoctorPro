using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;
using EntitiesServices.Attributes;
using System.ComponentModel;
using Newtonsoft.Json;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacienteViewModel
    {
        [Key]
        public int PACI__CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO obrigatorio")]
        public int TIPA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "NOME deve conter no minimo 1 e no máximo 150 caracteres.")]
        public string PACI_NM_NOME { get; set; }
        [StringLength(150, ErrorMessage = "NOME SOCIAL deve conter no máximo 150 caracteres.")]
        public string PACI_NM_SOCIAL { get; set; }
        public Nullable<int> SEXO_CD_ID { get; set; }
        public Nullable<int> COR1_CD_ID { get; set; }
        public Nullable<int> ESCI_CD_ID { get; set; }
        public Nullable<int> CONV_CD_ID { get; set; }
        [StringLength(150, ErrorMessage = "NOME DO PAI deve conter no máximo 150 caracteres.")]
        public string PACI_NM_PAI { get; set; }
        [StringLength(150, ErrorMessage = "NOME DA MÃE deve conter no máximo 150 caracteres.")]
        public string PACI_NM_MAE { get; set; }
        [StringLength(10, ErrorMessage = "CEP deve conter no máximo 10 caracteres.")]
        public string PACI_NR_CEP { get; set; }
        [StringLength(100, ErrorMessage = "ENDEREÇO deve conter no máximo 100 caracteres.")]
        public string PACI_NM_ENDERECO { get; set; }
        [StringLength(20, ErrorMessage = "NÚMERO deve conter no máximo 20 caracteres.")]
        public string PACI_NR_NUMERO { get; set; }
        [StringLength(20, ErrorMessage = "COMPLEMENTO deve conter no máximo 20 caracteres.")]
        public string PACI_NR_COMPLEMENTO { get; set; }
        [StringLength(50, ErrorMessage = "BAIRRO deve conter no máximo 50 caracteres.")]
        public string PACI_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "CIDADE deve conter no máximo 50 caracteres.")]
        public string PACI_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        [StringLength(50, ErrorMessage = "PROFISSÃO deve conter no máximo 50 caracteres.")]
        public string PACI_NM_PROFISSAO { get; set; }
        [Required(ErrorMessage = "Campo DATA DE NASCIMENTO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PACI_DT_NASCIMENTO { get; set; }
        [StringLength(50, ErrorMessage = "NATURALIDADE deve conter no máximo 50 caracteres.")]
        public string PACI_NM_NATURALIDADE { get; set; }
        [StringLength(50, ErrorMessage = "NACIONALIDADE deve conter no máximo 50 caracteres.")]
        public string PACI_NM_NACIONALIDADE { get; set; }
        [Required(ErrorMessage = "Campo CPF obrigatorio")]
        [StringLength(20, ErrorMessage = "CPF deve conter no máximo 20 caracteres.")]
        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        public string PACI_NR_CPF { get; set; }
        [StringLength(20, ErrorMessage = "RG deve conter no máximo 20 caracteres.")]
        public string PACI_NR_RG { get; set; }
        [StringLength(20, ErrorMessage = "TELEFONE deve conter no máximo 20 caracteres.")]
        public string PACI_NR_TELEFONE { get; set; }
        [StringLength(20, ErrorMessage = "CELULAR deve conter no máximo 20 caracteres.")]
        public string PACI_NR_CELULAR { get; set; }
        [StringLength(150, ErrorMessage = "E-MAIL deve conter no máximo 150 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string PACI_NM_EMAIL { get; set; }
        public Nullable<int> PACI_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PACI_DT_CADASTRO { get; set; }
        public string PACI_AQ_FOTO { get; set; }
        [StringLength(20, ErrorMessage = "MATRÍCULA deve conter no máximo 20 caracteres.")]
        public string PACI_NR_MATRICULA { get; set; }
        public Nullable<int> GRAU_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PACI_DT_PREVISAO_RETORNO { get; set; }
        [StringLength(100, ErrorMessage = "INDICAÇÃO deve conter no máximo 100 caracteres.")]
        public string PACI_NM_INDICACAO { get; set; }
        [StringLength(5000, ErrorMessage = "OBSERVAÇÕES deve conter no máximo 5000 caracteres.")]
        public string PACI_TX_OBSERVACOES { get; set; }
        public Nullable<int> NACI_CD_ID { get; set; }
        public Nullable<int> MUNI_CD_ID { get; set; }
        public Nullable<int> MUNI_SG_UF { get; set; }
        public string PACI_GU_GUID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PACI_DT_ALTERACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PACI_DT_CONSULTA { get; set; }
        [StringLength(10, ErrorMessage = "UF NATURALIDADE deve conter no máximo 10 caracteres.")]
        public string PACI_SG_NATURALIDADE_UF { get; set; }
        public Nullable<int> PACI_NR_IDADE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PACI_DT_ULTIMO_ACESSO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PACI_DT_ACESSO { get; set; }
        public Nullable<int> PACI_IN_COMPLETADO { get; set; }
        [Required(ErrorMessage = "Campo MENOR obrigatorio")]
        public Nullable<int> PACI_IN_MENOR { get; set; }
        [StringLength(60, ErrorMessage = "RESPONSÁVEL deve conter no máximo 60 caracteres.")]
        public string PACI_NM_RESPONSAVEL { get; set; }
        public Nullable<int> PACI_IN_FICHAS { get; set; }
        [Required(ErrorMessage = "Campo PADRÃO DE ANAMNESE obrigatorio")]
        public Nullable<int> PACI_IN_PADRAO_ANAMNESE { get; set; }
        [Required(ErrorMessage = "Campo PADRÃO DE ANAMNESE CONTÍNUA obrigatorio")]
        public Nullable<int> PACI_IN_PADRAO_CONTINUA { get; set; }
        public String CEPBase { get; set; }
        public Nullable<int> VACO_CD_ID { get; set; }
        public Nullable<System.DateTime> PACI_DT_PRECO { get; set; }
        public string PACI_NM_LOGIN { get; set; }
        public string PACI_NM_SENHA { get; set; }
        public string PACI_NM_NOVA_SENHA { get; set; }
        public string PACI_NM_SENHA_CONFIRMA { get; set; }
        public Nullable<int> PACI_IN_MENSAGEM_ATRASO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PACI_IN_NUMERO_ENVIO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PACI_DT_ULTIMO_ENVIO { get; set; }
        public Nullable<int> PACI_IN_FIM_ENVIO { get; set; }
        public Nullable<int> RACA_CD_ID { get; set; }

        public System.DateTime PAAN_DT_DATA { get; set; }
        public string PAAN_NM_ABDOMEM { get; set; }
        public string PAAN_DS_DIAGNOSTICO_1 { get; set; }
        public string PAAN_DS_MOTIVO_CONSULTA { get; set; }
        public string PAAN_DS_HISTORIA_FAMILIAR { get; set; }
        public string PAAN_DS_HISTORIA_SOCIAL { get; set; }
        public string PAAN_NM_AVALIACAO_CARDIOLOGICA { get; set; }
        public string PAAN_NM_RESPIRATORIO { get; set; }
        public string PAAN_NM_MEMBROS_INFERIORES { get; set; }
        public string PAAN_DS_QUEIXA_PRINCIPAL { get; set; }
        public string PAAN_DS_HISTORIA_DOENCA_ATUAL { get; set; }
        public string PAAN_DS_HISTORIA_PATOLOGICA_PROGRESSIVA { get; set; }
        public string PAAN_DS_CONDUTA { get; set; }
        public string PAAN_NM_AVALIACAO_CARDIOLOGICA_LONG { get; set; }
        public string PAAN_NM_RESPIRATORIO_LONG { get; set; }
        public string PAAM_DS_DIAGNOSTICO_1_LONG { get; set; }
        public string PAAM_DS_DIAGNOSTICO_2_LONG { get; set; }
        public string PAAN_TX_OBSERVACOES { get; set; }
        public string PAAM_NM_MEDICAMENTO { get; set; }
        public string PAAM_NM_CAMPO_1 { get; set; }
        public string PAAM_DS_CAMPO_1 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_1 { get; set; }
        public string PAAM_NM_CAMPO_2 { get; set; }
        public string PAAM_DS_CAMPO_2 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_2 { get; set; }
        public string PAAM_NM_CAMPO_3 { get; set; }
        public string PAAM_DS_CAMPO_3 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_3 { get; set; }
        public string PAAM_NM_CAMPO_4 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_4 { get; set; }
        public string PAAM_DS_CAMPO_4 { get; set; }
        public string PAAM_NM_CAMPO_5 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_5 { get; set; }
        public string PAAM_DS_CAMPO_5 { get; set; }
        public string PAAM_TX_COMPLETA { get; set; }
        public string PAAM_NM_CAMPO_6 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_6 { get; set; }
        public string PAAM_DS_CAMPO_6 { get; set; }
        public string PAAM_DS_CAMPO_6_OLD { get; set; }

        public string PAAM_NM_CAMPO_7 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_7 { get; set; }
        public string PAAM_DS_CAMPO_7 { get; set; }
        public string PAAM_DS_CAMPO_7_OLD { get; set; }
        public string PAAM_NM_CAMPO_8 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_8 { get; set; }
        public string PAAM_DS_CAMPO_8 { get; set; }
        public string PAAM_DS_CAMPO_8_OLD { get; set; }

        public string PAAM_NM_CAMPO_9 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_9 { get; set; }
        public string PAAM_DS_CAMPO_9 { get; set; }
        public string PAAM_DS_CAMPO_9_OLD { get; set; }

        public string PAAM_NM_CAMPO_10 { get; set; }
        public Nullable<int> PAAM_IN_CAMPO_10 { get; set; }
        public string PAAM_DS_CAMPO_10 { get; set; }
        public string PAAM_DS_CAMPO_10_OLD { get; set; }

        public Nullable<System.DateTime> PAEF_DT_DATA { get; set; }
        public Nullable<int> PAEF_NR_PA_ALTA { get; set; }
        public Nullable<int> PAEF_NR_PA_BAIXA { get; set; }
        public Nullable<decimal> PAEF_NR_PESO { get; set; }
        public Nullable<decimal> PAEF_NR_ALTURA { get; set; }
        public Nullable<decimal> PAEF_VL_IMC { get; set; }
        public Nullable<int> PAEF_IN_ATIVO { get; set; }
        public Nullable<int> PAEF_NR_FREQUENCIA_CARDIACA { get; set; }
        public Nullable<decimal> PAEF_NR_TEMPERATURA { get; set; }
        public string PAEF_DS_EXAME_FISICO { get; set; }

        public Nullable<int> PAEF_IN_DIABETE { get; set; }
        public Nullable<int> PAEF_IN_HIPERTENSAO { get; set; }
        public Nullable<int> PAEF_IN_TABAGISMO { get; set; }
        public Nullable<int> PAEF_IN_VARIZES { get; set; }
        public Nullable<int> PAEF_IN_EPILEPSIA { get; set; }
        public Nullable<int> PAEF_IN_GESTANTE { get; set; }
        public Nullable<int> PAEF_IN_HIPOTENSAO { get; set; }
        public Nullable<int> PAEF_IN_CIRURGIAS { get; set; }
        [StringLength(1000, ErrorMessage = "CIRURGIAS deve conter no máximo 1000 caracteres.")]
        public string PAEF_TX_CIRURGIAS { get; set; }
        public Nullable<int> PAEF_IN_EXERCICIO_FISICO { get; set; }
        public Nullable<int> PAEF_IN_EXERCICIO_FISICO_FREQUENCIA { get; set; }
        public Nullable<int> PAEF_IN_ALCOOLISMO { get; set; }
        public Nullable<int> PAEF_IN_ALCOOLISMO_FREQUENCIA { get; set; }
        public Nullable<int> PAEF_IN_ANTE_ALERGICO { get; set; }
        public Nullable<int> PAEF_IN_ANTE_ONCOLOGICO { get; set; }
        public Nullable<int> PAEF_IN_ANTICONCEPCIONAL { get; set; }
        [StringLength(500, ErrorMessage = "ANTICONCEPCIONAL deve conter no máximo 500 caracteres.")]
        public string PAEF_DS_ANTICONCEPCIONAL { get; set; }
        public Nullable<int> PAEF_IN_MARCAPASSO { get; set; }
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
        public string PAEF_TX_RESULTADOS{ get; set; }
        public string PAEF_DS_EXERCICIO_FISICO_LONG { get; set; }
        public string PAEF_DS_ALCOOLISMO_LONG { get; set; }
        public string PAEF_DS_ALERGICO_LONG { get; set; }
        public string PAEF_DS_ONCOLOGICO_LONG { get; set; }
        public string PAEF_DS_TABAGISMO_LONG { get; set; }
        public string PAEF_DS_MARCAPASSO_LONG { get; set; }
        public Nullable<int> PACI_IN_HUMANO { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PACO_DT_CONSULTA { get; set; }
        public Nullable<int> CRIA_CONSULTA { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> HORA_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> HORA_FINAL { get; set; }
        public Nullable<int> VACO_CD_ID_1 { get; set; }
        public Nullable<int> Tipo_Importacao { get; set; }
        public String Ficha_Importar { get; set; }
        public Nullable<int> Paciente { get; set; }
        public string SelectedArq { get; set; }
        public List<string> Arqs { get; set; }

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

        public String Ronco
        {
            get
            {
                if (QUBE_IN_RONCO == 1)
                {
                    return "Sim";
                }
                if (QUBE_IN_RONCO == 2)
                {
                    return "Não";
                }
                if (QUBE_IN_RONCO == 3)
                {
                    return "Não Sei";
                }
                return "Não informado";
            }
        }
        public String TipoRonco
        {
            get
            {
                if (QUBE_IN_TIPO_RONCO == 1)
                {
                    return "Pouco mais alto que respirando";
                }
                if (QUBE_IN_TIPO_RONCO == 2)
                {
                    return "Tão alto quanto falando";
                }
                if (QUBE_IN_TIPO_RONCO == 3)
                {
                    return "Mais alto quanto falando";
                }
                if (QUBE_IN_TIPO_RONCO == 4)
                {
                    return "Muito alto que pode ser ouvido nos quartos próximos";
                }
                return "Não informado";
            }
        }
        public String FreqRonco
        {
            get
            {
                if (QUBE_IN_FREQUENCIA_RONCO == 1)
                {
                    return "Praticamente todos os dias";
                }
                if (QUBE_IN_FREQUENCIA_RONCO == 2)
                {
                    return "3-4 vezes por semana";
                }
                if (QUBE_IN_FREQUENCIA_RONCO == 3)
                {
                    return "1-2 vezes por semana";
                }
                if (QUBE_IN_FREQUENCIA_RONCO == 4)
                {
                    return "Nunca ou praticamente nunca";
                }
                return "Não informado";
            }
        }
        public String IncomodaRonco
        {
            get
            {
                if (QUBE_IN_INCOMODA_RONCO == 1)
                {
                    return "Sim";
                }
                if (QUBE_IN_INCOMODA_RONCO == 2)
                {
                    return "Não";
                }
                return "Não informado";
            }
        }
        public String RespiraRonco
        {
            get
            {
                if (QUBE_IN_PARA_RESPIRA == 1)
                {
                    return "Praticamente todos os dias";
                }
                if (QUBE_IN_PARA_RESPIRA == 2)
                {
                    return "3-4 vezes por semana";
                }
                if (QUBE_IN_PARA_RESPIRA == 3)
                {
                    return "1-2 vezes por semana";
                }
                if (QUBE_IN_PARA_RESPIRA == 4)
                {
                    return "Nunca ou praticamente nunca";
                }
                return "Não informado";
            }
        }
        public String Cansaco
        {
            get
            {
                if (QUBE_IN_CANSACO == 1)
                {
                    return "Praticamente todos os dias";
                }
                if (QUBE_IN_CANSACO == 2)
                {
                    return "3-4 vezes por semana";
                }
                if (QUBE_IN_CANSACO == 3)
                {
                    return "1-2 vezes por semana";
                }
                if (QUBE_IN_CANSACO == 4)
                {
                    return "Nunca ou praticamente nunca";
                }
                return "Não informado";
            }
        }
        public String CansacoAcordado
        {
            get
            {
                if (QUBE_IN_CANSACO_ACORDADO == 1)
                {
                    return "Praticamente todos os dias";
                }
                if (QUBE_IN_CANSACO_ACORDADO == 2)
                {
                    return "3-4 vezes por semana";
                }
                if (QUBE_IN_CANSACO_ACORDADO == 3)
                {
                    return "1-2 vezes por semana";
                }
                if (QUBE_IN_CANSACO_ACORDADO == 4)
                {
                    return "Nunca ou praticamente nunca";
                }
                return "Não informado";
            }
        }
        public String Cochilo
        {
            get
            {
                if (QUBE_IN_COCHILO == 1)
                {
                    return "Sim";
                }
                if (QUBE_IN_COCHILO == 2)
                {
                    return "Não";
                }
                return "Não informado";
            }
        }
        public String PressaoAlta
        {
            get
            {
                if (QUBE_IN_PRESSAO == 1)
                {
                    return "Sim";
                }
                if (QUBE_IN_PRESSAO == 2)
                {
                    return "Não";
                }
                if (QUBE_IN_PRESSAO == 3)
                {
                    return "Não Sei";
                }
                return "Não informado";
            }
        }
        public String Categoria1
        {
            get
            {
                if (QUBE_IN_CATEGORIA_1 == 1)
                {
                    return "Positiva";
                }
                if (QUBE_IN_CATEGORIA_1 == 2)
                {
                    return "Não";
                }
                return "Não informado";
            }
        }
        public String Categoria2
        {
            get
            {
                if (QUBE_IN_CATEGORIA_2 == 1)
                {
                    return "Positiva";
                }
                if (QUBE_IN_CATEGORIA_2 == 2)
                {
                    return "Não";
                }
                return "Não informado";
            }
        }
        public String Categoria3
        {
            get
            {
                if (QUBE_IN_CATEGORIA_3 == 1)
                {
                    return "Positiva";
                }
                if (QUBE_IN_CATEGORIA_3 == 2)
                {
                    return "Não";
                }
                return "Não informado";
            }
        }
        public String Risco
        {
            get
            {
                if (QUBE_IN_PONTUACAO == 1)
                {
                    return "Alto risco para apnéia obstrutiva do sono";
                }
                if (QUBE_IN_PONTUACAO == 2)
                {
                    return "Não";
                }
                return "Não informado";
            }
        }

        public Nullable<int> QUEP_IN_LIVRO { get; set; }
        public Nullable<int> QUEP_IN_TV { get; set; }
        public Nullable<int> QUEP_IN_INATIVO { get; set; }
        public Nullable<int> QUEP_IN_CARRO { get; set; }
        public Nullable<int> QUEP_IN_DEITADO { get; set; }
        public Nullable<int> QUEP_IN_CONVERSA { get; set; }
        public Nullable<int> QUEP_IN_ALMOCO { get; set; }
        public Nullable<int> QUEP_IN_VOLANTE { get; set; }
        public Nullable<int> QUEP_IN_RESULTADO { get; set; }

        [DisplayName("Você ronca alto (alto o suficiente para que possa ser ouvido através de portas fechadas ou seu companheiro cutuca você à noite para parar de roncar)?")]
        public Nullable<int> QUSB_IN_RONCO { get; set; }
        [DisplayName("Você frequentemente se sente cansado, exausto ou sonolento durante o dia (como, por exemplo, adormecer enquanto dirige)?")]
        public Nullable<int> QUSB_IN_CANSADO { get; set; }
        [DisplayName("Alguém observou que você para de respirar ou engasga/fica ofegante durante o seu sono?")]
        public Nullable<int> QUSB_IN_OBSERVA { get; set; }
        [DisplayName("Você tem ou está sendo tratado para pressão sanguínea alta?")]
        public Nullable<int> QUSB_IN_PRESSAO { get; set; }
        [DisplayName("O colar é de 43 cm ou mais (homens) / 41 cm ou mais (mulheres)?")]
        public Nullable<int> QUSB_IN_PESCOCO { get; set; }
        [DisplayName("IMC maior que 35 kg/m²?")]
        public Nullable<int> QUSB_IN_IMC { get; set; }
        [DisplayName("Idade acima de 50 anos?")]
        public Nullable<int> QUSB_IN_IDADE { get; set; }
        [DisplayName("Sexo masculino?")]
        public Nullable<int> QUSB_IN_MASCULINO { get; set; }
        public Nullable<int> QUSB_IN_PONTUACAO { get; set; }
        public string QUSB_DS_PONTUACAO { get; set; }

        public String SBRonco
        {
            get
            {
                if (QUSB_IN_RONCO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String SBCansado
        {
            get
            {
                if (QUSB_IN_CANSADO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String SBObserva
        {
            get
            {
                if (QUSB_IN_OBSERVA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String SBPressao
        {
            get
            {
                if (QUSB_IN_PRESSAO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String SBPescoco
        {
            get
            {
                if (QUSB_IN_PESCOCO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String SBIMC
        {
            get
            {
                if (QUSB_IN_IMC == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String SBIdade
        {
            get
            {
                if (QUSB_IN_IDADE == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String SBMasculino
        {
            get
            {
                if (QUSB_IN_MASCULINO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        public String HorarioRegular
        {
            get
            {
                if (PAAM_DS_SONO_HORARIO_REGULAR == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        public String LatenciaSono
        {
            get
            {
                if (PAAM_DS_SONO_LATENCIA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        public bool Humano
        {
            get
            {
                if (PACI_IN_HUMANO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PACI_IN_HUMANO = (value == true) ? 1 : 0;
            }
        }

        public Int32 Idade
        {
            get
            {
                if (PACI_DT_NASCIMENTO != null)
                {
                    int age = 0;
                    age = DateTime.Now.Subtract(PACI_DT_NASCIMENTO.Value).Days;
                    age = age / 365;
                    return age;
                }
                return 0;
            }
        }
        public String TipoAnamnese
        {
            get
            {
                if (PACI_IN_PADRAO_ANAMNESE == 1)
                {
                    return "Padrão - Segmentada";
                }
                return "Contínua";
            }
        }
        public String TipoContinua
        {
            get
            {
                if (PACI_IN_PADRAO_CONTINUA == 1)
                {
                    return "Por Data";
                }
                return "Livre";
            }
        }
        public String Menor
        {
            get
            {
                if (PACI_IN_MENOR == 1)
                {
                    return "Sim";
                }
                return "Não";
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<AGENDA> AGENDA { get; set; }
        [JsonIgnore]
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<AVISO_LEMBRETE> AVISO_LEMBRETE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<CONSULTA_RECEBIMENTO> CONSULTA_RECEBIMENTO { get; set; }
        [JsonIgnore]
        public virtual CONVENIO CONVENIO { get; set; }
        [JsonIgnore]
        public virtual COR COR { get; set; }
        [JsonIgnore]
        public virtual ESTADO_CIVIL ESTADO_CIVIL { get; set; }
        [JsonIgnore]
        public virtual GRAU_INSTRUCAO GRAU_INSTRUCAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<GRUPO_PACIENTE> GRUPO_PACIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<MENSAGENS> MENSAGENS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<MENSAGENS_DESTINOS> MENSAGENS_DESTINOS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS_ENVIADAS_SISTEMA> MENSAGENS_ENVIADAS_SISTEMA { get; set; }
        public virtual MUNICIPIO MUNICIPIO { get; set; }
        public virtual NACIONALIDADE NACIONALIDADE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_ANAMNESE> PACIENTE_ANAMNESE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_ANEXO> PACIENTE_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_ANOTACAO> PACIENTE_ANOTACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_ANOTACAO> PACIENTE_ANOTACAO1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_ATESTADO> PACIENTE_ATESTADO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_CONSULTA> PACIENTE_CONSULTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_CONTATO> PACIENTE_CONTATO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_EXAME_ANEXO> PACIENTE_EXAME_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_EXAME_ANOTACAO> PACIENTE_EXAME_ANOTACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_EXAME_FISICOS> PACIENTE_EXAME_FISICOS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_EXAMES> PACIENTE_EXAMES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_HISTORICO> PACIENTE_HISTORICO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_PRESCRICAO_ITEM> PACIENTE_PRESCRICAO_ITEM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_PRESCRICAO> PACIENTE_PRESCRICAO { get; set; }
        [JsonIgnore]
        public virtual SEXO SEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_SOLICITACAO> PACIENTE_SOLICITACAO { get; set; }
        [JsonIgnore]
        public virtual TIPO_PACIENTE TIPO_PACIENTE { get; set; }
        [JsonIgnore]
        public virtual UF UF { get; set; }
        [JsonIgnore]
        public virtual UF UF1 { get; set; }
        [JsonIgnore]
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<RECURSIVIDADE_DESTINO> RECURSIVIDADE_DESTINO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<RESULTADO_ROBOT> RESULTADO_ROBOT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<ACESSO_METODO> ACESSO_METODO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_FICHA> PACIENTE_FICHA { get; set; }
        [JsonIgnore]
        public virtual VALOR_CONSULTA VALOR_CONSULTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MEDICOS_ENVIO> MEDICOS_ENVIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUESTIONARIO_BERLIM> QUESTIONARIO_BERLIM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUESTIONARIO_EPWORTH> QUESTIONARIO_EPWORTH { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUESTIONARIO_STOPBANG> QUESTIONARIO_STOPBANG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOCACAO> LOCACAO { get; set; }
        public virtual RACA RACA { get; set; }
    }
}