using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MCS.Library.Office.OpenXml.Word
{
    public class GeneralFormatter
    {
        public static string ToString(object o,string formatStr)
        {
            Type type=o.GetType();
            MethodInfo method=type.GetMethod("ToString",new Type[]{typeof(string)});
            if(null==method)
                return o.ToString();
            return method.Invoke(o, new object[] { formatStr }).ToString();
        }
    }
}
