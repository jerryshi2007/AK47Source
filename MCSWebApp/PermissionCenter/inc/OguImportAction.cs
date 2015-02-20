using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	public abstract class OguImportAction : ImportAction
	{
		public OguImportAction(PC.SCOrganization parent)
		{
			this.Parent = parent;
		}

		public PC.SCOrganization Parent { get; set; }
	}
}