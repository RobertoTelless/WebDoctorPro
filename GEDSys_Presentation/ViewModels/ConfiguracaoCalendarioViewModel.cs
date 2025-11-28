using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ConfiguracaoCalendarioViewModel
    {
        [Key]
        public int COCA_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> COCA_IN_ATIVO { get; set; }
        public Nullable<int> COCA_IN_SEGUNDA_FEIRA { get; set; }
        public Nullable<int> COCA_IN_TERCA_FEIRA { get; set; }
        public Nullable<int> COCA_IN_QUARTA_FEIRA { get; set; }
        public Nullable<int> COCA_IN_QUINTA_FEIRA { get; set; }
        public Nullable<int> COCA_IN_SEXTA_FEIRA { get; set; }
        public Nullable<int> COCA_IN_SABADO { get; set; }
        public Nullable<int> COCA_IN_DOMINGO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_COMERCIAL_SEG_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_COMERCIAL_SEG_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_COMERCIAL_TER_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_COMERCIAL_TER_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_COMERCIAL_QUA_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_COMERCIAL_QUA_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_COMERCIAL_QUI_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_COMERCIAL_QUI_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_COMERCIAL_SEX_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_COMERCIAL_SEX_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_COMERCIAL_SAB_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_COMERCIAL_SAB_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_COMERCIAL_DOM_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_COMERCIAL_DOM_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_SEG_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_SEG_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_TER_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_QUA_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_QUI_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_SEX_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_SAB_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_DOM_INICIO { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_TER_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_QUA_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_QUI_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_SEX_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_SAB_FINAL { get; set; }
        [CheckTimeAttributeMain(ErrorMessage = "Hora:Minuto inválido")]
        public Nullable<System.TimeSpan> COCA_HR_INTERVALO_DOM_FINAL { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }

        public String Segunda
        {
            get
            {
                if (COCA_IN_SEGUNDA_FEIRA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Terca
        {
            get
            {
                if (COCA_IN_TERCA_FEIRA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Quarta
        {
            get
            {
                if (COCA_IN_QUARTA_FEIRA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Quinta
        {
            get
            {
                if (COCA_IN_QUINTA_FEIRA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Sexta
        {
            get
            {
                if (COCA_IN_SEXTA_FEIRA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Sabado
        {
            get
            {
                if (COCA_IN_SABADO == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }
        public String Domingo
        {
            get
            {
                if (COCA_IN_SEGUNDA_FEIRA == 1)
                {
                    return "Sim";
                }
                return "Não";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONFIGURACAO_CALENDARIO_BLOQUEIO> CONFIGURACAO_CALENDARIO_BLOQUEIO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}