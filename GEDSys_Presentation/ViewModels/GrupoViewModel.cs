using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class GrupoViewModel
    {
        [Key]
        public int GRUP_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<int> TIPA_CD_ID { get; set; }
        public Nullable<int> SEXO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O NOME deve conter no minimo 2 e no máximo 50 caracteres.")]
        public string GRUP_NM_NOME { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CADASTRO deve ser uma data válida")]
        public Nullable<System.DateTime> GRUP_DT_CADASTRO { get; set; }
        public string GRUP_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE NASCIMENTO deve ser uma data válida")]
        public Nullable<System.DateTime> GRUP_DT_NASCIMENTO { get; set; }
        public string GRUP_NR_DIA { get; set; }
        public string GRUP_NR_MES { get; set; }
        public string GRUP_NR_ANO { get; set; }
        public Nullable<int> GRUP_IN_ATIVO { get; set; }

        public string NOME { get; set; }
        public Int32? ID { get; set; }
        public string CIDADE { get; set; }
        public Nullable<System.DateTime> DATA_NASC { get; set; }
        public Int32? CATEGORIA { get; set; }
        public Int32? STATUS { get; set; }
        public String LINK { get; set; }
        public Int32? GRUPO { get; set; }
        public String MODELO { get; set; }
        public string DIA { get; set; }
        public string MES { get; set; }
        public string ANO { get; set; }

        public String MesInteiro
        {
            get
            {
                if (GRUP_NR_MES == "1")
                {
                    return "Janeiro";
                }
                if (GRUP_NR_MES == "2")
                {
                    return "Fevereiro";
                }
                if (GRUP_NR_MES == "3")
                {
                    return "Março";
                }
                if (GRUP_NR_MES == "4")
                {
                    return "Abril";
                }
                if (GRUP_NR_MES == "5")
                {
                    return "Maio";
                }
                if (GRUP_NR_MES == "6")
                {
                    return "Junho";
                }
                if (GRUP_NR_MES == "7")
                {
                    return "Julho";
                }
                if (GRUP_NR_MES == "8")
                {
                    return "Agosto";
                }
                if (GRUP_NR_MES == "9")
                {
                    return "Setembro";
                }
                if (GRUP_NR_MES == "10")
                {
                    return "Outubro";
                }
                if (GRUP_NR_MES == "11")
                {
                    return "Novembro";
                }
                return "Dezembro";
            }
        }

        public virtual SEXO SEXO { get; set; }
        public virtual TIPO_PACIENTE TIPO_PACIENTE { get; set; }
        public virtual UF UF { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRUPO_PACIENTE> GRUPO_PACIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MENSAGENS> MENSAGENS { get; set; }
    }
}