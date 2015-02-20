using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// 小危写的，在SiteMap中使用
    /// </summary>
    class SR
    {
        private ResourceManager resources;
        private static SR loader;
        internal SR()
        {
            this.resources = new ResourceManager("System.Web", typeof(System.Web.IHttpHandler).Assembly);
        }

        private static SR GetLoader()
        {
            if (loader == null)
            {
                SR sr = new SR();
                Interlocked.CompareExchange<SR>(ref loader, sr, null);
            }
            return loader;
        }

        public static object GetObject(string name)
        {
            SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetObject(name, Culture);
        }

        public static string GetString(string name)
        {
            SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetString(name, Culture);
        }

        public static string GetString(string name, params object[] args)
        {
            SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            string format = loader.resources.GetString(name, Culture);
            if ((args == null) || (args.Length <= 0))
            {
                return format;
            }
            for (int i = 0; i < args.Length; i++)
            {
                string str2 = args[i] as string;
                if ((str2 != null) && (str2.Length > 0x400))
                {
                    args[i] = str2.Substring(0, 0x3fd) + "...";
                }
            }
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        public static string GetString(string name, out bool usedFallback)
        {
            usedFallback = false;
            return GetString(name);
        }

        private static CultureInfo Culture
        {
            get
            {
                return null;
            }
        }

        public static ResourceManager Resources
        {
            get
            {
                return GetLoader().resources;
            }
        }
    }
}
