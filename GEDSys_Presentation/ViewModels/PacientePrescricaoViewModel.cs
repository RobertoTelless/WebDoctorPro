using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Model;
using Newtonsoft.Json;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PacientePrescricaoViewModel
    {
        [Key]
        public int PAPR_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PACIENTE obrigatorio")]
        public int PACI_CD_ID { get; set; }
        public Nullable<int> PACO_CD_ID { get; set; }
        public Nullable<int> TICO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime PAPR_DT_DATA { get; set; }
        public string PAPR_NM_REMEDIO { get; set; }
        public string PAPR_NM_DOSAGEM { get; set; }
        public string PAPR_NM_FORMA { get; set; }
        public string PAPR_NM_POSOLOGIA { get; set; }
        [StringLength(10000, MinimumLength = 1, ErrorMessage = "CONTEÚDO deve conter no minimo 1 e no máximo 10000 caracteres.")]
        public string PAPR_DS_TEXTO { get; set; }
        public int PAPR_IN_ATIVO { get; set; }
        public string PAPR_GU_GUID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> PAPR_IN_ENVIADO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAPR_DT_ENVIO { get; set; }
        public string PAPR_GU_GUID_ENVIO { get; set; }
        public string PAPR_HT_TEXTO_HTML { get; set; }
        public string PAPR_AQ_ARQUIVO_HTML { get; set; }
        public string PAPR_AQ_ARQUIVO_PDF { get; set; }
        public string PAPR_AQ_ARQUIVO_QRCODE { get; set; }
        public Nullable<int> PAPR_IN_PDF { get; set; }
        public Nullable<int> PAPR_NR_ENVIOS { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAPR_DT_GERACAO_PDF { get; set; }
        public Nullable<int> PAPR_IN_DATA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAPR_DT_EMISSAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PAPR_DT_EMISSAO_COMPLETA { get; set; }
        public string PAPR_TK_TOKEN { get; set; }
        public Nullable<int> PAPR_IN_ASSINADO_DIGITAL { get; set; }
        public Nullable<System.DateTime> PAPR_DT_VALIDACAO { get; set; }
        public string PAPR_IP_VALIDACAO { get; set; }
        public Nullable<int> PAPR_NR_VALIDACAO { get; set; }
        public Nullable<int> PAPR_IN_VALIDACAO { get; set; }
        public Nullable<int> PAPR_IN_DENUNCIA { get; set; }
        public Nullable<System.DateTime> PAPR_DT_DENUNCIA { get; set; }
        public string PAPR_IP_DENUNCIA { get; set; }
        public Nullable<int> PAPR_NR_DENUNCIA { get; set; }
        public string PAPR_TX_DENUNCIA { get; set; }
        public Nullable<int> PAPR_IN_ASSINANDO { get; set; }

        public String TrataData
        {
            get
            {
                if (PAPR_IN_DATA == 1)
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
                if (PAPR_IN_VALIDACAO == 1)
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
                if (PAPR_IN_DENUNCIA == 1)
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
                if (PAPR_IN_ASSINADO_DIGITAL == 1)
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
        public virtual TIPO_CONTROLE TIPO_CONTROLE { get; set; }
        [JsonIgnore]
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<PACIENTE_PRESCRICAO_ITEM> PACIENTE_PRESCRICAO_ITEM { get; set; }
    }
}