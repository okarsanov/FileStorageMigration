using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileStorageMigration.Helpers
{
    public class LoggerHelper
    {
        public static void LogError(string message, Exception e)
        {
            var dtn = DateTime.Now.Date;
            var result = $"{message}{Environment.NewLine}{GetError(e)}";

            var fileName = $"error_{dtn.ToString("yyyy_MM_dd")}.log";

            File.AppendAllText(fileName, result);
        }

        static string GetError(Exception e)
        {
            if (e == null)
                return string.Empty;

            var r = $"{e.Message}{Environment.NewLine}--------------------- Stack Trace ---------------------{e.StackTrace}{Environment.NewLine}-------------------------------------------------------";

            r += GetError(e.InnerException);

            return r;
        }
    }
}
