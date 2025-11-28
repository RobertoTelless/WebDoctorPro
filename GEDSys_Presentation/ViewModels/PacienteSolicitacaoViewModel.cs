using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacienteSolicitacaoViewModel
    {
        [Key]
        public int PASO_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PACIENTE obrigatorio")]
        public Nullable<int> PACI_CD_ID { get; set; }
        public string PASO_GU_GUID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PASO_DT_EMISSAO { get; set; }
        [Required(ErrorMessage = "Campo NOME DO EXAME obrigatorio")]
        [StringLength(500, ErrorMessage = "NOME DO EXAME deve conter  no máximo 500 caracteres.")]
        public string PASO_NM_TITULO { get; set; }
        [Required(ErrorMessage = "Campo CONTEÚDO DA SOLICITAÇÃO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "TEXTO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string PASO_TX_TEXTO { get; set; }
        public Nullable<int> PASO_IN_ENVIADO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PASO_DT_ENVIO { get; set; }
        public string PASO_GU_GUID_ENVIO { get; set; }
        public Nullable<int> PASO_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE EXAME obrigatorio")]
        public Nullable<int> TIEX_CD_ID { get; set; }
        public Nullable<int> PASO_NR_ENVIOS { get; set; }
        public Nullable<int> PASO_IN_PDF { get; set; }
        public string PASO_HT_TEXT_HTML { get; set; }
        public string PASO_AQ_ARQUIVO_HTML { get; set; }
        public string PASO_AQ_ARQUIVO_PDF { get; set; }
        public string PASO_AQ_ARQUIVO_QRCODE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PASO_DT_GERACAO_PDF { get; set; }
        public Nullable<int> PACO_CD_ID { get; set; }
        public Nullable<int> PASO_IN_DATA { get; set; }
        [Required(ErrorMessage = "Campo INDICAÇÂO CLÍNICA obrigatorio")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "INDICAÇÃO CLÍNICA deve conter no minimo 1 e no máximo 1000 caracteres.")]
        public string PASO_DS_INDICACAO_CLINICA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PASO_IN_QUANTIDADE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PASO_DT_EMISSAO_COMPLETA { get; set; }
        public string PASO_TK_TOKEN { get; set; }
        public Int32? SOLI_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PASO_DT_VALIDACAO { get; set; }
        public string PASO_IP_VALIDACAO { get; set; }
        public Nullable<int> PASO_NR_VALIDACAO { get; set; }
        public Nullable<int> PASO_IN_VALIDACAO { get; set; }
        public string PASO_IP_DENUNCIA { get; set; }
        public Nullable<int> PASO_NR_DENUNCIA { get; set; }
        public string PASO_TX_DENUNCIA { get; set; }
        public Nullable<int> PASO_IN_ASSINADO_DIGITAL { get; set; }
        public Nullable<int> PASO_IN_DENUNCIA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PASO_DT_DENUNCIA { get; set; }

        public String TrataData
        {
            get
            {
                if (PASO_IN_DATA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Validado
        {
            get
            {
                if (PASO_IN_VALIDACAO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Denuncia
        {
            get
            {
                if (PASO_IN_DENUNCIA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Digital
        {
            get
            {
                if (PASO_IN_ASSINADO_DIGITAL == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        [JsonIgnore]
        public virtual PACIENTE PACIENTE { get; set; }
        [JsonIgnore]
        public virtual TIPO_EXAME TIPO_EXAME { get; set; }
        [JsonIgnore]
        public virtual USUARIO USUARIO { get; set; }
        [JsonIgnore]
        public virtual PACIENTE_CONSULTA PACIENTE_CONSULTA { get; set; }
        [JsonIgnore]
        public virtual ICollection<PACIENTE_EXAMES> PACIENTE_EXAMES { get; set; }
    }
}