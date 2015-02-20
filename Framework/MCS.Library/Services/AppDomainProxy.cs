using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;

namespace MCS.Library.Services
{
    internal class AppDomainProxy : MarshalByRefObject
    {
        public object CreateInstance(string assemblyName, string typeName, params object[] args)
        {
            Assembly a = Assembly.Load(assemblyName);

            Type type = a.GetType(typeName);

            return TypeCreator.CreateInstance(type, args);
        }

        public object CreateInstance(string typeDesp, params object[] args)
        {
            return TypeCreator.CreateInstance(typeDesp, args);
        }
    }
}
