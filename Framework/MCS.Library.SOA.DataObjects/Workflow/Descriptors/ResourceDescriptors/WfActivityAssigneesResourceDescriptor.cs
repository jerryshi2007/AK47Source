using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[XElementSerializable]
	public class WfActivityAssigneesResourceDescriptor : WfActivityResourceDescriptorBase
	{
        public static readonly WfActivityAssigneesResourceDescriptor EmptyInstance = new WfActivityAssigneesResourceDescriptor();

		public WfActivityAssigneesResourceDescriptor()
		{ }

		protected internal override void FillUsers(OguDataCollection<IUser> users)
		{
			IWfActivity target = TargetActivity;

			if (target != null)
				target.Assignees.ForEach(a => users.Add(a.User));
		}
	}
}
