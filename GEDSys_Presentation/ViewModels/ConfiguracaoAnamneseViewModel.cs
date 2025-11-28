using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ConfiguracaoAnamneseViewModel
    {
        [Key]
        public int COAN_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> COAN_IN_MOTIVO_CONSULTA { get; set; }
        public Nullable<int> COAN_IN_HISTORIA_FAMILIAR { get; set; }
        public Nullable<int> COAN_IN_HISTORIA_SOCIAL { get; set; }
        public Nullable<int> COAN_IN_CARDIOLOGICA { get; set; }
        public Nullable<int> COAN_IN_RESPIRATORIA { get; set; }
        public Nullable<int> COAN_IN_ABDOMEM { get; set; }
        public Nullable<int> COAN_IN_MEMBROS { get; set; }
        public Nullable<int> COAN_IN_QUEIXA { get; set; }
        public Nullable<int> COAN_IN_HISTORIA_DOENCA { get; set; }
        public Nullable<int> COAN_IN_HISTORIA_PATOLOGIA { get; set; }
        public Nullable<int> COAN_IN_DIAGNOSTICO_1 { get; set; }
        public Nullable<int> COAN_IN_DIAGNOSTICO_2 { get; set; }
        public Nullable<int> COAN_IN_CONDUTA { get; set; }
        public Nullable<int> COAN_IN_OBSERVACOES { get; set; }
        public Nullable<int> COAN_IN_CAMPO_1 { get; set; }
        [StringLength(50, ErrorMessage = "O TÍTULO DO ITEM deve conter no máximo 50 caracteres.")]
        public string COAN_NM_CAMPO_1 { get; set; }
        public Nullable<int> COAN_IN_CAMPO_2 { get; set; }
        [StringLength(50, ErrorMessage = "O TÍTULO DO ITEM deve conter no máximo 50 caracteres.")]
        public string COAN_NM_CAMPO_2 { get; set; }
        public Nullable<int> COAN_IN_CAMPO_3 { get; set; }
        [StringLength(50, ErrorMessage = "O TÍTULO DO ITEM deve conter no máximo 50 caracteres.")]
        public string COAN_NM_CAMPO_3 { get; set; }
        public Nullable<int> COAN_IN_ATIVO { get; set; }
        public Nullable<int> COAN_IN_MEDICAMENTO { get; set; }
        public Nullable<int> COAN_IN_CAMPO_4 { get; set; }
        public Nullable<int> COAN_IN_CAMPO_5 { get; set; }
        [StringLength(50, ErrorMessage = "O TÍTULO DO ITEM deve conter no máximo 50 caracteres.")]
        public string COAN_NM_CAMPO_4 { get; set; }
        [StringLength(50, ErrorMessage = "O TÍTULO DO ITEM deve conter no máximo 50 caracteres.")]
        public string COAN_NM_CAMPO_5 { get; set; }
        public Nullable<int> COAN_IN_CAMPO_6 { get; set; }
        public Nullable<int> COAN_IN_CAMPO_7 { get; set; }
        public Nullable<int> COAN_IN_CAMPO_8 { get; set; }
        public Nullable<int> COAN_IN_CAMPO_9 { get; set; }
        public Nullable<int> COAN_IN_CAMPO_10 { get; set; }
        [StringLength(50, ErrorMessage = "O TÍTULO DO ITEM deve conter no máximo 50 caracteres.")]
        public string COAN_NM_CAMPO_6 { get; set; }
        [StringLength(50, ErrorMessage = "O TÍTULO DO ITEM deve conter no máximo 50 caracteres.")]
        public string COAN_NM_CAMPO_7 { get; set; }
        [StringLength(50, ErrorMessage = "O TÍTULO DO ITEM deve conter no máximo 50 caracteres.")]
        public string COAN_NM_CAMPO_8 { get; set; }
        [StringLength(50, ErrorMessage = "O TÍTULO DO ITEM deve conter no máximo 50 caracteres.")]
        public string COAN_NM_CAMPO_9 { get; set; }
        [StringLength(50, ErrorMessage = "O TÍTULO DO ITEM deve conter no máximo 50 caracteres.")]
        public string COAN_NM_CAMPO_10 { get; set; }
        public Nullable<int> COAN_IN_PADRAO_FORMATO { get; set; }
        public Nullable<int> COAN_IN_FORMATO_CONTINUA { get; set; }
        public Nullable<int> COAN_IN_BLOCO_COMUM { get; set; }
        public Nullable<int> COAN_IN_BLOCO_SONO { get; set; }

        public String Motivo
        {
            get
            {
                return "Motivo da Consulta";
            }
        }
        public String Familia
        {
            get
            {
                return "História Familiar";
            }
        }
        public String Social
        {
            get
            {
                return "História Social";
            }
        }
        public String Cardio
        {
            get
            {
                return "Avaliação Cardiológica";
            }
        }
        public String Respira
        {
            get
            {
                return "Avaliação Respiratória";
            }
        }
        public String Abdomem
        {
            get
            {
                return "Avaliação do Abdômem";
            }
        }
        public String Membros
        {
            get
            {
                return "Avaliação dos Membros Inferiores";
            }
        }
        public String Queixa
        {
            get
            {
                return "Queixa Principal";
            }
        }
        public String Doenca
        {
            get
            {
                return "História da Doença Atual";
            }
        }
        public String Patologia
        {
            get
            {
                return "História da Patologia Prograssiva";
            }
        }
        public String Diag
        {
            get
            {
                return "Diagnóstico";
            }
        }
        public String Conduta
        {
            get
            {
                return "Conduta Adotada";
            }
        }
        public String Sim
        {
            get
            {
                return "Sim";
            }
        }
        public String Medic
        {
            get
            {
                return "Medicamentos Em Uso";
            }
        }
    }
}