using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Permissions;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Operations
{
	public interface IAUAclOperations
	{
		SCAclContainer UpdateObjectAcl(SCAclContainer container);
		ISCAclContainer ReplaceAclRecursively(ISCAclContainer container, bool forceReplace);
	}
}
