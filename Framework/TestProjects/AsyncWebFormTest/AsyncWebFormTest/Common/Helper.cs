using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace AsyncWebFormTest.Common
{
    internal static class Helper
    {
        public static string GetThreadInfo(string phaseName)
        {
            return string.Format("Phase {0}, Thread Id: {1}, Time: {2}",
                phaseName,
                Thread.CurrentThread.ManagedThreadId,
                DateTime.Now.ToString("HH:mm:ss.fff"));
        }

        public static void WriteThreadInfo(string phaseName)
        {
            TextWriter writer = GetWriter();

            writer.WriteLine("Phase {0}, Thread Id: {1}, Time: {2}",
                phaseName,
                Thread.CurrentThread.ManagedThreadId,
                DateTime.Now.ToString("HH:mm:ss.fff"));
        }

        public static TextWriter GetWriter()
        {
            TextWriter writer = (TextWriter)HttpContext.Current.Items["Writer"];

            if (writer == null)
            {
                StringBuilder strB = new StringBuilder();

                writer = new StringWriter(strB);

                HttpContext.Current.Items["Writer"] = writer;
                HttpContext.Current.Items["TextBuffer"] = strB;
            }

            return writer;
        }

        public static string GetOutputText()
        {
            string result = string.Empty;

            StringBuilder strB = (StringBuilder)HttpContext.Current.Items["TextBuffer"];

            if (strB.Length > 0)
                result = strB.ToString();

            return result;
        }
    }
}