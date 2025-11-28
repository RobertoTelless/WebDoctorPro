using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacienteAtestadoViewModel
    {
        [Key]
        public int PAAT_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PACIENTE obrigatorio")]
        public Nullable<int> PACI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> TIAT_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAAT_DT_DATA { get; set; }
        [StringLength(100, MinimumLength = 1, ErrorMessage = "TÍTULO deve conter no minimo 1 e no máximo 100 caracteres.")]
        public string PAAT_NM_TITULO { get; set; }
        [Required(ErrorMessage = "Campo DESTINATÁRIO obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "DESTINO deve conter no minimo 1 e no máximo 100 caracteres.")]
        public string PAAT_NM_DESTINO { get; set; }
        [Required(ErrorMessage = "Campo TEXTO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "TEXTO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string PAAT_TX_TEXTO { get; set; }
        public string PAAT_GU_GUID { get; set; }
        public Nullable<int> PAAT_IN_ATIVO { get; set; }
        public Nullable<int> PAAT__IN_ENVIADO { get; set; }
        public string PAAT_GU_GUID_ENVIO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAAT_DT_ENVIO { get; set; }
        public Nullable<int> PAAT_NR_ENVIOS { get; set; }
        public Nullable<int> PAAT_IN_PDF { get; set; }
        public string PAAT_HT_TEXTO_HTML { get; set; }
        public string PAAT_AQ_ARQUIVO_HTML { get; set; }
        public string PAAT_AQ_ARQUIVO_PDF { get; set; }
        public string PAAT_AQ_ARQUIVO_QRCODE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAAT_DT_GERACAO_PDF { get; set; }
        public Nullable<int> PACO_CD_ID { get; set; }
        public Nullable<int> PAAT_IN_DATA { get; set; }
        public Nullable<System.DateTime> PAAT_DT_DUMMY { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAAT_DT_EMISSAO_COMPLETA { get; set; }
        public string PAAT_TK_TOKEN { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAAT_DT_VALIDACAO { get; set; }
        public string PAAT_IP_VALIDACAO { get; set; }
        public Nullable<int> PAAT_NR_VALIDACAO { get; set; }
        public Nullable<int> PAAT_IN_VALIDACAO { get; set; }
        public Nullable<int> PAAT_IN_DENUNCIA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAAT_DT_DENUNCIA { get; set; }
        public string PAAT_IP_DENUNCIA { get; set; }
        public Nullable<int> PAAT_NR_DENUNCIA { get; set; }
        [StringLength(5000, ErrorMessage = "TEXTO DA DENUNCIA deve conter no máximo 5000 caracteres.")]
        public string PAAT_TX_DENUNCIA { get; set; }
        public Nullable<int> PAAT_IN_ASSINADO_DIGITAL { get; set; }

        public String TrataData
        {
            get
            {
                if (PAAT_IN_DATA == 1)
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
                if (PAAT_IN_VALIDACAO == 1)
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
                if (PAAT_IN_DENUNCIA == 1)
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
                if (PAAT_IN_ASSINADO_DIGITAL == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        [JsonIgnore]
        public virtual PACIENTE PACIENTE { get; set; }
        [JsonIgnore]
        public virtual TIPO_ATESTADO TIPO_ATESTADO { get; set; }
        [JsonIgnore]
        public virtual USUARIO USUARIO { get; set; }
        [JsonIgnore]
        public virtual PACIENTE_CONSULTA PACIENTE_CONSULTA { get; set; }
    }
}