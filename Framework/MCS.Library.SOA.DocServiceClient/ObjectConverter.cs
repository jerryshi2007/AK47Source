using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MCS.Library.SOA.DocServiceClient
{
    public static class ObjectConverter
    {
        public static T To<T>(this object o)
            where T:new()
        {
            Type sourceType = o.GetType();
            Type targetType=typeof(T);
            T result=new T();
            PropertyInfo[] pInfoTargetArray = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo pInfoTarget in pInfoTargetArray)
            {
                string pTargetName = pInfoTarget.Name;
                PropertyInfo pInfoSource = sourceType.GetProperty(pTargetName, BindingFlags.Instance | BindingFlags.Public);
                if (null == pInfoSource)
                    continue;
                if (pInfoSource.PropertyType != pInfoTarget.PropertyType)
                    continue;
                pInfoTarget.SetValue(result, pInfoSource.GetValue(o, null), null);
            }
            return result;
        }
    }
}
