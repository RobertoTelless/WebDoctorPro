using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;

namespace ERP_Condominios_Solution.Classes
{
    public class CookieManager
    {
        private const string CookieName = "WebDoctorProInicioBase";
        private const string RawValue = "WEBDOCTORPRO";
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Grava o cookie com a data de expiração embutida no valor para permitir conferência posterior.
        /// </summary>
        public static void GravarCookieInicioBase()
        {
            HttpCookie baseCookie = new HttpCookie(CookieName);

            // Calculamos a data de expiração (1 ano)
            DateTime dataExpiracao = DateTime.Now.AddYears(1);

            // Armazenamos o Conteúdo + Data no Valor do Cookie
            // Exemplo de valor final: "WEBDOCTORPRO|2027-02-11 14:30:00"
            baseCookie.Value = $"{RawValue}|{dataExpiracao.ToString(DateFormat)}";

            baseCookie.Expires = dataExpiracao;
            baseCookie.HttpOnly = true;

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Response.Cookies.Add(baseCookie);
            }
        }

        /// <summary>
        /// Recupera o cookie e verifica se a data embutida no valor ainda é válida.
        /// </summary>
        /// <returns>True se o cookie existir e a data interna for futura.</returns>
        public static bool VerificarValidadeCookie()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];

            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                return false;

            try
            {
                // Separamos o identificador da data
                string[] partes = cookie.Value.Split('|');

                if (partes.Length < 2) return false;

                string dataStr = partes[1];
                DateTime dataExpiracaoInterna;

                // Tentamos converter a string de volta para DateTime
                if (DateTime.TryParseExact(dataStr, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dataExpiracaoInterna))
                {
                    // Comparamos a data interna com a data corrente do servidor
                    return dataExpiracaoInterna > DateTime.Now;
                }
            }
            catch
            {
                // Caso o valor tenha sido corrompido ou alterado manualmente
                return false;
            }

            return false;
        }

        /// <summary>
        /// Retorna apenas o conteúdo limpo do cookie (sem a data).
        /// </summary>
        public static string LerConteudoLimpo()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieName];
            if (cookie != null && cookie.Value.Contains("|"))
            {
                return cookie.Value.Split('|')[0];
            }
            return cookie?.Value ?? "Não encontrado";
        }
    }
}