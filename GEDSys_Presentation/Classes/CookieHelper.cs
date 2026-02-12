using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using ERP_Condominios_Solution.Classes;
using Newtonsoft.Json;
using System.Net;
using EntitiesServices.Work_Classes;

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

        /// <summary>
        /// Obtém o IP do usuário e consulta a localização/provedor para preencher a DTO.
        /// </summary>
        public static AcessoVisitanteDTO CapturarDadosAcesso()
        {
            var dto = new AcessoVisitanteDTO { DataAcesso = DateTime.Now };

            try
            {
                // Obtém o IP Real (considerando Proxy/Load Balancers como o do Azure)
                dto.IP = ObterIpUsuario();

                // Ignora consulta em localhost (IP ::1 ou 127.0.0.1) pois APIs de GeoIP falham neles
                if (dto.IP == "::1" || dto.IP == "127.0.0.1")
                {
                    dto.Cidade = "Localhost";
                    dto.Provedor = "Desenvolvimento";
                    return dto;
                }

                // Consulta API externa (ip-api.com é gratuita para uso moderado)
                using (WebClient client = new WebClient())
                {
                    string url = $"http://ip-api.com/json/{dto.IP}?fields=status,country,regionName,city,isp";
                    string json = client.DownloadString(url);

                    dynamic result = JsonConvert.DeserializeObject(json);

                    if (result.status == "success")
                    {
                        dto.Pais = result.country;
                        dto.Estado = result.regionName;
                        dto.Cidade = result.city;
                        dto.Provedor = result.isp;
                    }
                }
            }
            catch (Exception)
            {
                // Em caso de erro na API, retorna a DTO apenas com o IP
            }

            return dto;
        }

        private static string ObterIpUsuario()
        {
            var request = HttpContext.Current.Request;

            // Tenta pegar o IP caso o site esteja atrás de um Proxy (Azure/Cloudflare)
            string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
                ip = request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(ip))
                ip = request.UserHostAddress;

            return ip;
        }
    }
}