using System;

namespace CrossCutting
{
    /// <summary>
    /// The exception management class.
    /// </summary>
    public static class ExceptionManagement
    {
        /// <summary>
        /// Build the exception message.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>The <see cref="String"/>.</returns>
        public static String BuildExceptionMessage(Exception ex)
        {
            String msg;
            if (ex.InnerException != null)
            {
                msg = "Erro Superior:" + ex.InnerException.Message + " || Erro Principal: " + ex.Message;
            }
            else
            {
                msg = "Erro:" + ex.Message;
            }
            return msg;
        }
    }
}
