using ApplicationServices.Interfaces;
using Azure.Communication.Email;
using CrossCutting;
using EntitiesServices.Model;
using ERP_Condominios_Solution.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using XidNet;

namespace ERP_Condominios_Solution.Classes
{
    public class VerificacaoDataCalendario
    {
        public static Int32 ValidaDiaUtil(Int32 dia, CONFIGURACAO_CALENDARIO conf)
        {
            Int32 volta = 0;
            if (dia == 0)
            {
                volta = conf.COCA_IN_DOMINGO == 1 ? 0 : 1;
            }
            if (dia == 1)
            {
                volta = conf.COCA_IN_SEGUNDA_FEIRA == 1 ? 0 : 1;
            }
            if (dia == 2)
            {
                volta = conf.COCA_IN_TERCA_FEIRA == 1 ? 0 : 1;
            }
            if (dia == 3)
            {
                volta = conf.COCA_IN_QUARTA_FEIRA == 1 ? 0 : 1;
            }
            if (dia == 4)
            {
                volta = conf.COCA_IN_QUINTA_FEIRA == 1 ? 0 : 1;
            }
            if (dia == 5)
            {
                volta = conf.COCA_IN_SEXTA_FEIRA == 1 ? 0 : 1;
            }
            if (dia == 6)
            {
                volta = conf.COCA_IN_SABADO == 1 ? 0 : 1;
            }
            return volta;
        }

        public static Int32 ValidaHoraUtil(Int32 dia, PacienteConsultaViewModel vm, CONFIGURACAO_CALENDARIO conf)
        {
            // Prepara ambiente
            Int32 volta = 0;
            TimeSpan? inicio = vm.PACO_HR_INICIO;
            TimeSpan? final = vm.PACO_HR_FINAL;
            TimeSpan? utilInicio = new TimeSpan();
            TimeSpan? utilFinal = new TimeSpan();
            TimeSpan? IntvInicio = new TimeSpan();
            TimeSpan? IntvFinal = new TimeSpan();

            // Recupera parametros do dia
            if (dia == 0)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_DOM_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_DOM_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_DOM_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_DOM_FINAL;
            }
            if (dia == 1)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_SEG_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_SEG_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_SEG_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_SEG_FINAL;
            }
            if (dia == 2)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_TER_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_TER_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_TER_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_TER_FINAL;
            }
            if (dia == 3)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_QUA_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_QUA_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_QUA_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_QUA_FINAL;
            }
            if (dia == 4)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_QUI_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_QUI_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_QUI_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_QUI_FINAL;
            }
            if (dia == 5)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_SEX_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_SEX_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_SEX_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_SEX_FINAL;
            }
            if (dia == 6)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_SAB_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_SAB_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_SAB_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_SAB_FINAL;
            }

            // Comparações
            if (inicio < utilInicio || inicio > utilFinal)
            {
                volta = 1;
                return volta;
            }
            if (final < utilInicio || final > utilFinal)
            {
                volta = 1;
                return volta;
            }
            if (inicio >= IntvInicio & final <= IntvFinal)
            {
                volta = 2;
                return volta;
            }
            if (inicio < IntvInicio & (final < IntvFinal & final > IntvInicio))
            {
                volta = 2;
                return volta;
            }
            if (final > IntvFinal & (inicio < IntvFinal & inicio > IntvInicio))
            {
                volta = 2;
                return volta;
            }
            if (inicio < IntvFinal & inicio > IntvInicio)
            {
                volta = 2;
                return volta;
            }
            if (final < IntvFinal & final > IntvInicio)
            {
                volta = 2;
                return volta;
            }
            if (inicio < IntvInicio & final > IntvFinal & IntvInicio > inicio & IntvFinal < final)
            {
                volta = 2;
                return volta;
            }

            // Retorno
            return volta;
        }

        public static Int32 ValidaHoraUtil(Int32 dia, PacienteViewModel vm, CONFIGURACAO_CALENDARIO conf)
        {
            // Prepara ambiente
            Int32 volta = 0;
            TimeSpan? inicio = vm.HORA_INICIO;
            TimeSpan? final = vm.HORA_FINAL;
            TimeSpan? utilInicio = new TimeSpan();
            TimeSpan? utilFinal = new TimeSpan();
            TimeSpan? IntvInicio = new TimeSpan();
            TimeSpan? IntvFinal = new TimeSpan();

            // Recupera parametros do dia
            if (dia == 0)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_DOM_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_DOM_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_DOM_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_DOM_FINAL;
            }
            if (dia == 1)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_SEG_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_SEG_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_SEG_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_SEG_FINAL;
            }
            if (dia == 2)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_TER_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_TER_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_TER_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_TER_FINAL;
            }
            if (dia == 3)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_QUA_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_QUA_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_QUA_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_QUA_FINAL;
            }
            if (dia == 4)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_QUI_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_QUI_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_QUI_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_QUI_FINAL;
            }
            if (dia == 5)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_SEX_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_SEX_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_SEX_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_SEX_FINAL;
            }
            if (dia == 6)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_SAB_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_SAB_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_SAB_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_SAB_FINAL;
            }

            // Comparações
            if (inicio < utilInicio || inicio > utilFinal)
            {
                volta = 1;
                return volta;
            }
            if (final < utilInicio || final > utilFinal)
            {
                volta = 1;
                return volta;
            }
            if (inicio >= IntvInicio & final <= IntvFinal)
            {
                volta = 2;
                return volta;
            }
            if (inicio < IntvInicio & (final < IntvFinal & final > IntvInicio))
            {
                volta = 2;
                return volta;
            }
            if (final > IntvFinal & (inicio < IntvFinal & inicio > IntvInicio))
            {
                volta = 2;
                return volta;
            }
            if (inicio < IntvFinal & inicio > IntvInicio)
            {
                volta = 2;
                return volta;
            }
            if (final < IntvFinal & final > IntvInicio)
            {
                volta = 2;
                return volta;
            }
            if (inicio < IntvInicio & final > IntvFinal & IntvInicio > inicio & IntvFinal < final)
            {
                volta = 2;
                return volta;
            }

            // Retorno
            return volta;
        }

        public static Int32 ValidaHoraUtilFaixa(Int32 dia, TimeSpan? inicio, TimeSpan? final, CONFIGURACAO_CALENDARIO conf)
        {
            // Prepara ambiente
            Int32 volta = 0;
            TimeSpan? utilInicio = new TimeSpan();
            TimeSpan? utilFinal = new TimeSpan();
            TimeSpan? IntvInicio = new TimeSpan();
            TimeSpan? IntvFinal = new TimeSpan();

            // Recupera parametros do dia
            if (dia == 0)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_DOM_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_DOM_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_DOM_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_DOM_FINAL;
            }
            if (dia == 1)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_SEG_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_SEG_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_SEG_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_SEG_FINAL;
            }
            if (dia == 2)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_TER_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_TER_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_TER_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_TER_FINAL;
            }
            if (dia == 3)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_QUA_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_QUA_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_QUA_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_QUA_FINAL;
            }
            if (dia == 4)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_QUI_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_QUI_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_QUI_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_QUI_FINAL;
            }
            if (dia == 5)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_SEX_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_SEX_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_SEX_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_SEX_FINAL;
            }
            if (dia == 6)
            {
                utilInicio = conf.COCA_HR_COMERCIAL_SAB_INICIO;
                utilFinal = conf.COCA_HR_COMERCIAL_SAB_FINAL;
                IntvInicio = conf.COCA_HR_INTERVALO_SAB_INICIO;
                IntvFinal = conf.COCA_HR_INTERVALO_SAB_FINAL;
            }

            // Comparações
            if (inicio < utilInicio || inicio > utilFinal)
            {
                volta = 2;
                return volta;
            }
            if (final < utilInicio || final > utilFinal)
            {
                volta = 2;
                return volta;
            }
            if (inicio >= IntvInicio & final <= IntvFinal)
            {
                volta = 3;
                return volta;
            }
            if (inicio < IntvInicio & (final < IntvFinal & final > IntvInicio))
            {
                volta = 3;
                return volta;
            }
            if (final > IntvFinal & (inicio < IntvFinal & inicio > IntvInicio))
            {
                volta = 3;
                return volta;
            }
            if (inicio < IntvFinal & inicio > IntvInicio)
            {
                volta = 3;
                return volta;
            }
            if (final < IntvFinal & final > IntvInicio)
            {
                volta = 3;
                return volta;
            }
            if (inicio < IntvInicio & final > IntvFinal & IntvInicio > inicio & IntvFinal < final)
            {
                volta = 3;
                return volta;
            }

            // Retorno
            return volta;
        }

    }
}