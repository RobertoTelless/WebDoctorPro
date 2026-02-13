using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacienteConsultaViewModel
    {
        public int PACO_CD_ID { get; set; }
        public Nullable<int> PACI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime PACO_DT_CONSULTA { get; set; }
        public int PACO_IN_ATIVO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> PACO_HR_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> PACO_HR_FINAL { get; set; }
        public Nullable<int> PACO_IN_ENCERRADA { get; set; }
        public Nullable<int> PACO_IN_TIPO { get; set; }
        [StringLength(5000, ErrorMessage = "RESUMO deve conter no máximo 5000 caracteres.")]
        public string PACO_TX_RESUMO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PACO_DT_PROXIMA { get; set; }
        public Nullable<int> PACO_IN_CONFIRMADA { get; set; }
        public Nullable<int> VACO_CD_ID { get; set; }
        public Nullable<int> PACO_IN_RECORRENTE { get; set; }
        public Nullable<int> PACO_IN_RECEBE { get; set; }
        [StringLength(5000, ErrorMessage = "A JUSTIFICATIVA DO CANCELAMENTO deve conter no máximo 5000 caracteres.")]
        public string PACO_TX_JUSTIFICATIVA_CANCELA { get; set; }

        public Nullable<int> PACO_IN_NOVO_PACIENTE { get; set; }

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
        public string PAAN_TX_OBSERVACOES { get; set; }
        public string PAAN_TX_COMPLETA { get; set; }
        public string PAAM_TX_TEXTO_LIVRE { get; set; }
        public Nullable<System.DateTime> PAAN_DT_COPIA { get; set; }
        public string PAAN_NM_AVALIACAO_CARDIOLOGICA_LONG { get; set; }
        public string PAAN_NM_RESPIRATORIO_LONG { get; set; }
        public string PAAM_DS_DIAGNOSTICO_1_LONG { get; set; }
        public string PAAM_DS_DIAGNOSTICO_2_LONG { get; set; }
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

        [StringLength(5000, ErrorMessage = "PRINCIPAL QUEIXA DO SONO deve conter no máximo 5000 caracteres.")]
        public string PAAM_DS_SONO_PRINCIPAL_QUEIXA { get; set; }
        [StringLength(5000, ErrorMessage = "SINTOMAS deve conter no máximo 5000 caracteres.")]
        public string PAAM_DS_SONO_SINTOMAS { get; set; }
        [StringLength(5000, ErrorMessage = "MEDICAMENTOS deve conter no máximo 5000 caracteres.")]
        public string PAAM_DS_SONO_MEDICAMENTOS { get; set; }
        [StringLength(5000, ErrorMessage = "COMORBIDADES deve conter no máximo 5000 caracteres.")]
        public string PAAM_DS_SONO_COMORBIDADES { get; set; }
        public string PAAM_DS_SONO_POLISONO { get; set; }

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
        public string PAEF_TX_RESULTADOS { get; set; }
        public string PAEF_DS_EXERCICIO_FISICO_LONG { get; set; }
        public string PAEF_DS_ALCOOLISMO_LONG { get; set; }
        public string PAEF_DS_ALERGICO_LONG { get; set; }
        public string PAEF_DS_ONCOLOGICO_LONG { get; set; }
        public string PAEF_DS_TABAGISMO_LONG { get; set; }
        public string PAEF_DS_MARCAPASSO_LONG { get; set; }
        public string PAEF_DS_FICHA_AVALIACAO { get; set; }
        public string PAEF_NM_TIPO_SANGUE { get; set; }

        [StringLength(150, ErrorMessage = "NOME deve conter no máximo 150 caracteres.")]
        public string PACI_NM_NOME { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PACI_DT_NASCIMENTO { get; set; }
        [StringLength(20, ErrorMessage = "CPF deve conter no máximo 20 caracteres.")]
        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        public string PACI_NR_CPF { get; set; }
        [StringLength(20, ErrorMessage = "CELULAR deve conter no máximo 20 caracteres.")]
        public string PACI_NR_CELULAR { get; set; }
        [StringLength(150, ErrorMessage = "E-MAIL deve conter no máximo 150 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string PACI_NM_EMAIL { get; set; }
        [StringLength(100, ErrorMessage = "INDICAÇÃO deve conter no máximo 100 caracteres.")]
        public string PACI_NM_INDICADO { get; set; }
        public Nullable<int> PACI_IN_COMPLETADO { get; set; }
        public Nullable<int> PACI_IN_MENOR { get; set; }
        public Nullable<int> Idade { get; set; }

        public int TIPO_VOLTA { get; set; }

        public Nullable<int> MODO_CONSULTA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> DATA_INICIO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> REPETE { get; set; }

        public Nullable<System.DateTime> DATA_BASE { get; set; }
        public Nullable<System.TimeSpan> INICIO_BASE { get; set; }
        public Nullable<System.TimeSpan> FINAL_BASE { get; set; }


        public Nullable<int> SEGUNDA_FEIRA { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> SEG_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> SEG_FINAL { get; set; }
        public Nullable<int> TERCA_FEIRA { get; set; }
        public Nullable<int> QUARTA_FEIRA { get; set; }
        public Nullable<int> QUINTA_FEIRA { get; set; }
        public Nullable<int> SEXTA_FEIRA { get; set; }
        public Nullable<int> SABADO { get; set; }
        public Nullable<int> DOMINGO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> TER_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> TER_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> QUA_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> QUA_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> QUI_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> QUI_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> SEX_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> SEX_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> SAB_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> SAB_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> DOM_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> DOM_FINAL { get; set; }
        public Nullable<int> VACO1_CD_ID { get; set; }

        public String Completado
        {
            get
            {
                if (PACI_IN_COMPLETADO == 1)
                {
                    return "Completado";
                }
                return "Requer Atualização";
            }
        }
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
        public String Recorrente
        {
            get
            {
                if (PACO_IN_RECORRENTE == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Recebe
        {
            get
            {
                if (PACO_IN_RECEBE == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Situacao
        {
            get
            {
                if (PACO_IN_CONFIRMADA == 1)
                {
                    return "Confirmada";
                }
                if (PACO_IN_CONFIRMADA == 2)
                {
                    return "Cancelada";
                }
                if (PACO_IN_CONFIRMADA == 3)
                {
                    return "Encerrada";
                }
                return "Pendente";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONSULTA_RECEBIMENTO> CONSULTA_RECEBIMENTO { get; set; }
        public virtual PACIENTE PACIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_ANAMNESE> PACIENTE_ANAMNESE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_ATESTADO> PACIENTE_ATESTADO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual VALOR_CONSULTA VALOR_CONSULTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_EXAME_FISICOS> PACIENTE_EXAME_FISICOS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_EXAMES> PACIENTE_EXAMES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_PRESCRICAO> PACIENTE_PRESCRICAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PACIENTE_SOLICITACAO> PACIENTE_SOLICITACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RESPOSTA_CONSULTA> RESPOSTA_CONSULTA { get; set; }

    }
}