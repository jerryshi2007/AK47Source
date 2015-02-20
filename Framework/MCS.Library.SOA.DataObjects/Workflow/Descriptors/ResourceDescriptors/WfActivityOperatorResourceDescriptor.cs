using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 某个流程环节的操作人
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class WfActivityOperatorResourceDescriptor : WfActivityResourceDescriptorBase
	{
        public static readonly WfActivityOperatorResourceDescriptor EmptyInstance = new WfActivityOperatorResourceDescriptor();

		public WfActivityOperatorResourceDescriptor()
		{ }

		protected internal override void FillUsers(OguDataCollection<IUser> users)
		{
			IWfActivity target = TargetActivity;

			if (target != null)
			{
				//如果该环节存在操作人，则使用操作人。否则使用候选人
				if (OguUser.IsNotNullOrEmpty(target.Operator))
					users.Add(target.Operator);
				else
					target.Candidates.ForEach(a => users.Add(a.User));
			}
		}
	}
}
