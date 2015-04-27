using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace MCS.Web.Library.Script.Mechanism
{
    internal static class AppSettings
    {
        public static bool AllowRelaxedUnicodeDecoding
        {
            get;
            set;
        }

        public static int MaxJsonDeserializerMembers
        {
            get;
            set;
        }
    }
}
