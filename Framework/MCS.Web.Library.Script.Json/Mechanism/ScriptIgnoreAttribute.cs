using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Web.Library.Script.Mechanism
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ScriptIgnoreAttribute : Attribute
    {
        public bool ApplyToOverrides
        {
            get;
            set;
        }
    }
}
