using System;
using System.Text.RegularExpressions;

namespace CrossCutting
{
    public static class ValidarItensDiversos
    {
        public static bool IsValidEmail(String strIn)
        {
            return Regex.IsMatch(strIn,
                   @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

        public static bool IsValidCep(string cep)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(cep, ("[0-9]{5}-[0-9]{3}"));
        }
        public static bool IsDateTime(String txtDate)
        {
            DateTime tempDate;
            return DateTime.TryParse(txtDate, out tempDate);
        }

        public static bool IsValidPhone(string phone)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phone, (@"^\([0-9]{2}\)[0-9]?[0-9]{4}-[0-9]{4}$"));
        }

        public static bool IsValidMobile(string phone)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phone, (@"^\([0-9]{2}\)[0-9]?[0-9]{5}-[0-9]{4}$"));
        }
    }
}
