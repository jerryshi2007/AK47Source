using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Web.Library.Script.Mechanism
{
    public class SimpleTypeResolver : JavaScriptTypeResolver
    {
        // Methods
        public override Type ResolveType(string id)
        {
            return Type.GetType(id);
        }

        public override string ResolveTypeId(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type.AssemblyQualifiedName;
        }
    }


}
