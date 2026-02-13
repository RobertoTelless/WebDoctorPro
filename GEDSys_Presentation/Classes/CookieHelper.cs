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

        // Chave para a flag de sessão que impede duplicidade
        private const string SESSION_FLAG = "Acesso_Site_Registrado_Flag";

        /// <summary>
        /// Verifica se este visitante já teve o seu acesso gravado na sessão atual.
        /// </summary>
        public static bool JaRegistradoNestaSessao()
        {
            if (HttpContext.Current.Session == null) return false;
            return HttpContext.Current.Session[SESSION_FLAG] != null;
        }

        /// <summary>
        /// Marca a sessão do utilizador como já processada para evitar logs duplos.
        /// </summary>
        public static void MarcarComoRegistrado()
        {
            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[SESSION_FLAG] = true;
            }
        }

        /// <summary>
        /// Obtém o IP do usuário e consulta a localização/provedor para preencher a DTO.
        /// </summary>
        /// <summary>
        /// Obtém o IP limpo e consulta a localização/provedor para preencher a DTO.
        /// </summary>
        public static AcessoVisitanteDTO CapturarDadosAcesso()
        {
            var dto = new AcessoVisitanteDTO { DataAcesso = DateTime.Now };

            try
            {
                // 1. Obtém o IP bruto dos cabeçalhos do servidor
                string ipBruto = ObterIpUsuario();

                // 2. Limpa o IP (Remove portas e trata listas de IPs)
                dto.IP = LimparIp(ipBruto);

                // 3. Validação para Localhost (APIs de GeoIP não funcionam com IPs locais)
                if (string.IsNullOrEmpty(dto.IP) || dto.IP == "::1" || dto.IP == "127.0.0.1")
                {
                    dto.IP = "127.0.0.1";
                    dto.Cidade = "Localhost";
                    dto.Provedor = "Ambiente de Desenvolvimento";
                    return dto;
                }

                // 4. Consulta a API externa com o IP já formatado
                using (WebClient client = new WebClient())
                {
                    // Define UTF8 para suportar acentuação nas cidades portuguesas/brasileiras
                    client.Encoding = System.Text.Encoding.UTF8;

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
                // Em caso de falha na API, o DTO segue apenas com o IP
            }

            return dto;
        }

        /// <summary>
        /// Recupera o endereço IP dos cabeçalhos da requisição HTTP.
        /// </summary>
        private static string ObterIpUsuario()
        {
            var request = HttpContext.Current.Request;

            // Verifica primeiro se existe um IP encaminhado por Proxy/Azure
            string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
                ip = request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(ip))
                ip = request.UserHostAddress;

            return ip;
        }

        /// <summary>
        /// Trata a string do IP para remover portas e extrair apenas o IP principal de uma lista.
        /// Resolve casos como "179.218.12.31:1716, 179.218.12.31"
        /// </summary>
        private static string LimparIp(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return string.Empty;

            // Se houver vírgula, significa que há múltiplos IPs (Client, Proxy1, Proxy2)
            // Pegamos sempre o primeiro, que é o IP original do cliente.
            if (ip.Contains(","))
            {
                ip = ip.Split(',')[0].Trim();
            }

            // Se houver dois pontos (:) e for um padrão IPv4 (contém pontos), removemos a porta
            // Exemplo: "179.218.12.31:1716" -> "179.218.12.31"
            if (ip.Contains(":") && ip.Contains("."))
            {
                ip = ip.Split(':')[0].Trim();
            }

            return ip;
        }
    }
}