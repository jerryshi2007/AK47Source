using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Web.Library.Script.Mechanism
{
    public abstract class JavaScriptConverter
    {
        // Methods
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        protected JavaScriptConverter()
        {
        }

        public abstract object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer);

        public abstract IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer);

        // Properties
        public abstract IEnumerable<Type> SupportedTypes { get; }
    }


}
