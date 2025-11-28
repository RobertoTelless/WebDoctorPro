using EntitiesServices.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ERP_Condominios_Solution.ViewModels
{
    public class LocacaoViewModel
    {
        [Key]
        public int LOCA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PACIENTE obrigatorio")]
        public int PACI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PRODUTO obrigatorio")]
        public int PROD_CD_ID { get; set; }
        public int PETA_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA INICIAL obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LOCA_DT_INICIO { get; set; }
        [Required(ErrorMessage = "Campo PRAZO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> LOCA_NR_PRAZO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        [Required(ErrorMessage = "Campo QUANTIDADE obrigatorio")]
        public Nullable<int> LOCA_IN_QUANTIDADE { get; set; }
        [Required(ErrorMessage = "Campo VALOR DA PARCELA obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> LOCA_VL_PARCELA { get; set; }
        public Nullable<int> LOCA_IN_RENOVACAO { get; set; }
        public Nullable<int> LOCA_IN_ATIVO { get; set; }
        public Nullable<int> LOCA_IN_ENCERRADO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> LOCA_VL_TOTAL { get; set; }
        public string LOCA_GU_GUID { get; set; }
        public Nullable<int> LOCA_IN_STATUS { get; set; }
        [StringLength(15, ErrorMessage = "O NÚMERO DA PARCELA deve conter no máximo 50 caracteres.")]
        public string LOCA_NR_NUMERO { get; set; }
        [StringLength(250, ErrorMessage = "O TÍTULO deve conter no máximo 250 caracteres.")]
        public string LOCA_NM_TITULO { get; set; }
        [StringLength(5000, ErrorMessage = "A DECRIÇÃO deve conter no máximo 5000 caracteres.")]
        public string LOCA_DS_DESCRICAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LOCA_DT_DUMMY { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        [Required(ErrorMessage = "O DIA DO VENCIMENTO é obrigatório.")]
        [Range(1, 30, ErrorMessage = "O DIA DE VENCIMENTO deve ser um número entre 1 e 30")]
        public Nullable<int> LOCA_NR_DIA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LOCA_DT_CANCELAMENTO { get; set; }
        [StringLength(500, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 500 caracteres.")]
        public string LOCA_DS_JUSTIFICATIVA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LOCA_DT_ENCERRAMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LOCA_DT_FINAL { get; set; }
        public Nullable<int> LOCA_NR_ATRASO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LOCA_DT_RENOVACAO { get; set; }
        public Nullable<int> LOCA_IN_RENOVACOES { get; set; }
        [StringLength(50, ErrorMessage = "O NÚMERO DE SÉRIE deve conter no máximo 50 caracteres.")]
        public string LOCA_NR_SERIE { get; set; }
        public Nullable<int> LOCA_IN_GARANTIA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LOCA_DT_GARANTIA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LOCA_DT_APROVACAO { get; set; }
        public Nullable<int> LOCA_IN_CONTRATO { get; set; }
        public string LOCA_XM_NOTA_FISCAL { get; set; }
        public Nullable<int> LOCA_NR_GARANTIA { get; set; }
        public string LOCA_TK_TOKEN { get; set; }
        public string LOCA_AQ_ARQUIVO_QRCODE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LOCA_DT_EMISSAO { get; set; }
        public Nullable<int> LOCA_IN_ASSINADO_DIGITAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> LOCA_VL_VALOR_ATRASO { get; set; }
        public Nullable<int> ENTRA_ESTOQUE { get; set; }
        public Nullable<int> LOCA_IN_ESTOQUE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> LOCA_DT_ENTREGA { get; set; }
        [StringLength(500, ErrorMessage = "A OBSERVAÇÃO deve conter no máximo 500 caracteres.")]
        public string LOCA_DS_ENTREGA { get; set; }
        public string CONTRATO_ASSINA { get; set; }
        [StringLength(500, ErrorMessage = "A OBSERVAÇÃO deve conter no máximo 500 caracteres.")]
        public string LOCA_DS_ENCERRA { get; set; }
        public Nullable<int> TIPO_PRECO { get; set; }
        public Nullable<decimal> PRECO_PROMOCAO { get; set; }
        public Nullable<int> TIPO_CONTRATO { get; set; }
        public Nullable<int> COLO_CD_ID { get; set; }
        public Nullable<int> LOCA_CD_DISTRATO_ID { get; set; }
        public Nullable<int> LOCA_CD_ENCERRA_ID { get; set; }

        public bool Encerrado
        {
            get
            {
                if (LOCA_IN_ENCERRADO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                LOCA_IN_ENCERRADO = (value == true) ? 1 : 0;
            }
        }

        public string ValorParcela
        {
            get
            {
                return LOCA_VL_PARCELA.HasValue ? CrossCutting.Formatters.DecimalFormatter(LOCA_VL_PARCELA.Value) : string.Empty;
            }
            set
            {
                LOCA_VL_PARCELA = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }

        public string ValorTotal
        {
            get
            {
                return LOCA_VL_TOTAL.HasValue ? CrossCutting.Formatters.DecimalFormatter(LOCA_VL_TOTAL.Value) : string.Empty;
            }
            set
            {
                LOCA_VL_TOTAL = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }

        public string Status
        {
            get
            {
                if (LOCA_IN_STATUS == 0)
                {
                    return "Pendente";
                }
                if (LOCA_IN_STATUS == 1)
                {
                    return "Ativa";
                }
                if (LOCA_IN_STATUS == 2)
                {
                    return "Encerrada";
                }
                if (LOCA_IN_STATUS == 3)
                {
                    return "Vencida";
                }
                return "Cancelada";
            }
        }

        public string Garantia
        {
            get
            {
                if (LOCA_IN_GARANTIA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        public string Estoque
        {
            get
            {
                if (LOCA_IN_ESTOQUE == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        public string Contrato
        {
            get
            {
                if (LOCA_IN_CONTRATO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        public string Assinado
        {
            get
            {
                if (LOCA_IN_ASSINADO_DIGITAL == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        public string NumParcelas
        {
            get
            {
                if (LOCACAO_PARCELA != null)
                {
                    return LOCACAO_PARCELA.Count.ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        public string Quitadas
        {
            get
            {
                return LOCACAO_PARCELA.Where(p => p.LOPA_IN_QUITADA == 1).ToList().Count().ToString();
            }
        }

        public string Atrasadas
        {
            get
            {
                return LOCACAO_PARCELA.Where(p => p.LOPA_IN_QUITADA == 0 & p.LOPA_DT_VENCIMENTO.Value.Date < DateTime.Today.Date).ToList().Count().ToString();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOCACAO_ANEXO> LOCACAO_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOCACAO_ANOTACAO> LOCACAO_ANOTACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOCACAO_HISTORICO> LOCACAO_HISTORICO { get; set; }
        public virtual PACIENTE PACIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOCACAO_PARCELA> LOCACAO_PARCELA { get; set; }
        public virtual PERIODICIDADE_TAREFA PERIODICIDADE_TAREFA { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LOCACAO_OCORRENCIA> LOCACAO_OCORRENCIA { get; set; }
        public virtual CONTRATO_LOCACAO CONTRATO_LOCACAO { get; set; }
        public virtual CONTRATO_LOCACAO CONTRATO_LOCACAO1 { get; set; }
        public virtual CONTRATO_LOCACAO CONTRATO_LOCACAO2 { get; set; }


    }
}