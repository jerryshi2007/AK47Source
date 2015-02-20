using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Archive
{
	public class AclArchiveOperation : IArchiveOperation
	{
		public static readonly AclArchiveOperation Instance = new AclArchiveOperation();

		private AclArchiveOperation()
		{
		}

		#region IArchiveOperation Members

		public void LoadOriginalData(ArchiveBasicInfo info)
		{
			info.Context["Acl"] = WfAclAdapter.Instance.LoadByResourceID(info.ResourceID);
		}

		public void SaveArchiveData(ArchiveBasicInfo info)
		{
			info.Context.DoAction<WfAclItemCollection>("Acl", acl =>
			{
				WfAclAdapter.Instance.Delete(b => b.AppendItem("RESOURCE_ID", info.ResourceID));

				WfAclAdapter.Instance.Update(acl);
			});
		}

		public void DeleteOriginalData(ArchiveBasicInfo info)
		{
			info.Context.DoAction<WfAclItemCollection>("Acl", acl =>
			{
				WfAclAdapter.Instance.Delete(b => b.AppendItem("RESOURCE_ID", info.ResourceID));
			});
		}

		#endregion
	}
}
