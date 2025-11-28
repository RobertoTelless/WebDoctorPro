using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class TemplateEMailViewModel
    {
        [Key]
        public int TEEM_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50 caracteres.")]
        public string TEEM_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo SIGLA obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "A SIGLA deve conter no minimo 1 caracteres e no máximo 10 caracteres.")]
        public string TEEM_SG_SIGLA { get; set; }
        [StringLength(50, ErrorMessage = "O LINK deve conter máximo 50 caracteres.")]
        public string TEEM_LK_LINK { get; set; }
        public Nullable<int> TEEM_IN_ATIVO { get; set; }
        public string TEEM_TX_CABECALHO { get; set; }
        [Required(ErrorMessage = "Campo CORPO obrigatorio")]
        public string TEEM_TX_CORPO { get; set; }
        public string TEEM_TX_DADOS { get; set; }
        [StringLength(250, ErrorMessage = "O NOME DO ARQUIVO deve conter no máximo 250 caracteres.")]
        public string TEEM_AQ_ARQUIVO { get; set; }
        [AllowHtml]
        public Nullable<int> TEEM_IN_HTML { get; set; }
        [AllowHtml]
        public Nullable<int> TEEM_IN_FIXO { get; set; }
        [AllowHtml]
        public Nullable<int> TEEM_IN_IMAGEM { get; set; }
        public Nullable<int> TEEM_IN_SISTEMA { get; set; }
        public Nullable<int> TEEM_IN_ROBOT { get; set; }
        public Nullable<int> TEEM_IN_EDITAVEL { get; set; }
        public Nullable<int> TEEM_IN_ANIVERSARIO { get; set; }

        public string file { get; set; }
        public Int32 html { get; set; }

        public String TIPO
        {
            get
            {
                if (TEEM_IN_HTML == 1)
                {
                    return "Texto HTML Digitado";
                }
                return "Arquivo HTML";
            }
        }
        public String IMAGEM
        {
            get
            {
                if (TEEM_IN_IMAGEM == 2)
                {
                    return "Imagens Embutidas";
                }
                if (TEEM_IN_IMAGEM == 1)
                {
                    return "Imagens Externas";
                }
                return "-";
            }
        }

        public String ROBOT
        {
            get
            {
                if (TEEM_IN_ROBOT == 0)
                {
                    return "-";
                }
                if (TEEM_IN_ROBOT == 1)
                {
                    return "Ação em Atraso";
                }
                if (TEEM_IN_ROBOT == 2)
                {
                    return "Ação no Dia";
                }
                if (TEEM_IN_ROBOT == 3)
                {
                    return "Proposta em Atraso";
                }
                if (TEEM_IN_ROBOT == 4)
                {
                    return "Proposta no Dia";
                }
                if (TEEM_IN_ROBOT == 5)
                {
                    return "Agenda no Dia";
                }
                if (TEEM_IN_ROBOT == 6)
                {
                    return "Tarefa no Dia";
                }
                return "-";
            }
        }

        public String Editavel
        {
            get
            {
                if (TEEM_IN_EDITAVEL == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Fixo
        {
            get
            {
                if (TEEM_IN_FIXO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Aniversario
        {
            get
            {
                if (TEEM_IN_ANIVERSARIO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA> CRM_PEDIDO_VENDA { get; set; }
        public virtual EMPRESA EMPRESA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGEM_AUTOMACAO> MENSAGEM_AUTOMACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS> MENSAGENS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESQUISA> PESQUISA { get; set; }

    }
}