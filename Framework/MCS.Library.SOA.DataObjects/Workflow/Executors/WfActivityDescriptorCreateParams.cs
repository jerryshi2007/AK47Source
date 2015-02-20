using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfActivityDescriptorCreateParams
	{
		public string Operation { get; set; }
		public string CurrentActivityKey { get; set; }
		public string Name { get; set; }
		public bool AllAgreeWhenConsign { get; set; }
		public WfVariableDescriptor[] Variables { get; set; }
		public OguDataCollection<IUser> Users { get; set; }
		public OguDataCollection<IUser> CirculateUsers { get; set; }
	}
}
