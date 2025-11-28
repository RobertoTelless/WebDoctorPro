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
using System.Web;
using System.Web.Mvc;
using XidNet;

namespace ERP_Condominios_Solution.Classes
{
    public class ControleAcessoMetodo
    {
        private readonly IAcessoMetodoAppService aceApp;

        public ControleAcessoMetodo(IAcessoMetodoAppService aceApps)
        {
            aceApp = aceApps;
        }

        public Int32 GravaAcesso(Int32 usuario, Int32 assinante, String sigla, String controller, String metodo)
        {
            String ip = (String)HttpContext.Current.Session["IPBase"];    
            ACESSO_METODO acesso = new ACESSO_METODO();
            acesso.ASSI_CD_ID = assinante;
            acesso.USUA_CD_ID = usuario;
            acesso.ACES_DT_ACESSO = DateTime.Now;
            acesso.ACES_IN_SISTEMA = 6;
            acesso.ACES_IN_ATIVO = 1;
            acesso.ACES_SG_ACESSO = sigla;
            acesso.ACES_NM_CONTROLLER = controller;
            acesso.ACES_NM_METHOD = metodo;
            acesso.ACES_IP_IP_LOGIN = ip;
            Int32 voltaAcesso = aceApp.ValidateCreate(acesso);
            return 0;
        }

        public Int32 GravaAcesso(String controller, String metodo, String ip)
        {
            String ipNovo = (String)HttpContext.Current.Session["IPBase"];
            ACESSO_METODO acesso = new ACESSO_METODO();
            acesso.ASSI_CD_ID = 83;
            acesso.USUA_CD_ID = 94;
            acesso.ACES_DT_ACESSO = DateTime.Now;
            acesso.ACES_IN_SISTEMA = 6;
            acesso.ACES_IN_ATIVO = 1;
            acesso.ACES_SG_ACESSO = "SITE";
            acesso.ACES_NM_CONTROLLER = controller;
            acesso.ACES_NM_METHOD = metodo;
            acesso.ACES_IP_IP_LOGIN = ipNovo;
            Int32 voltaAcesso = aceApp.ValidateCreate(acesso);
            return 0;
        }

        public Int32 GravaAcesso(Int32 usuario, Int32 assinante, String sigla, String controller, String metodo, String ip)
        {
            ACESSO_METODO acesso = new ACESSO_METODO();
            acesso.ASSI_CD_ID = assinante;
            acesso.USUA_CD_ID = usuario;
            acesso.ACES_DT_ACESSO = DateTime.Now;
            acesso.ACES_IN_SISTEMA = 6;
            acesso.ACES_IN_ATIVO = 1;
            acesso.ACES_SG_ACESSO = sigla;
            acesso.ACES_NM_CONTROLLER = controller;
            acesso.ACES_NM_METHOD = metodo;
            acesso.ACES_IP_IP_LOGIN = ip;
            Int32 voltaAcesso = aceApp.ValidateCreate(acesso);
            return 0;
        }

        public Int32 GravaAcesso(Int32 usuario, Int32 assinante, String sigla, String controller, String metodo, Int32? tipo, Int32? chave)
        {
            String ip = (String)HttpContext.Current.Session["IPBase"];
            ACESSO_METODO acesso = new ACESSO_METODO();
            acesso.ASSI_CD_ID = assinante;
            acesso.USUA_CD_ID = usuario;
            acesso.ACES_DT_ACESSO = DateTime.Now;
            acesso.ACES_IN_SISTEMA = 6;
            acesso.ACES_IN_ATIVO = 1;
            acesso.ACES_SG_ACESSO = sigla;
            acesso.ACES_NM_CONTROLLER = controller;
            acesso.ACES_NM_METHOD = metodo;
            acesso.ACES_IP_IP_LOGIN = ip;
            if (tipo == 1)
            {
                acesso.PACI_CD_ID = chave;
            }
            if (tipo == 2)
            {
                acesso.PAAM_CD_ID = chave;
            }
            if (tipo == 3)
            {
                acesso.PAAT_CD_ID = chave;
            }
            if (tipo == 4)
            {
                acesso.PACO_CD_ID = chave;
            }
            if (tipo == 5)
            {
                acesso.PAEX_CD_ID = chave;
            }
            if (tipo == 6)
            {
                acesso.PAEF_CD_ID = chave;
            }
            if (tipo == 7)
            {
                acesso.PAPR_CD_ID = chave;
            }
            if (tipo == 8)
            {
                acesso.PASO_CD_ID = chave;
            }
            if (tipo == 9)
            {
                acesso.USUT_CD_ID = chave;
            }
            if (tipo == 10)
            {
                acesso.ASST_CD_ID = chave;
            }
            if (tipo == 11)
            {
                acesso.AVIS_CD_ID = chave;
            }
            if (tipo == 12)
            {
                acesso.MEDI_CD_ID = chave;
            }
            if (tipo == 13)
            {
                acesso.MENS__CD_ID = chave;
            }
            if (tipo == 14)
            {
                acesso.PERF_CD_ID = chave;
            }
            if (tipo == 15)
            {
                acesso.SOLI_CD_ID = chave;
            }
            if (tipo == 16)
            {
                acesso.TEEM_CD_ID = chave;
            }
            if (tipo == 17)
            {
                acesso.TSMS_CD_ID = chave;
            }
            Int32 voltaAcesso = aceApp.ValidateCreate(acesso);
            return 0;
        }

    }
}