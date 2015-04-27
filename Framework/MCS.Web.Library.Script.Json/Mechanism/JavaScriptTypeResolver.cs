using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Web.Library.Script.Mechanism
{
    public abstract class JavaScriptTypeResolver
    {
        // Methods
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        protected JavaScriptTypeResolver()
        {
        }

        public abstract Type ResolveType(string id);
        public abstract string ResolveTypeId(Type type);
    }


}
