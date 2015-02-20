using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    public interface IPropertyComparer
    {
        bool AreEqual(SyncSession session, PropertyMapping mapping, NameObjectCollection srcValues, SchemaObjectBase targetObj);
    }
}
