using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Entity
{
	public class DelayRenameCodeNameAction : IDelayAction
	{
		private OGUPermission.IOguObject oguObject;
		private string adObjectID;
		private string adPropertyName;
		private string oguPropertyName;

		public DelayRenameCodeNameAction(OGUPermission.IOguObject oguObject, string oguPropertyName, string adObjectID, string adPropertyName)
		{
			this.oguObject = oguObject;
			this.oguPropertyName = oguPropertyName;
			this.adObjectID = adObjectID;
			this.adPropertyName = adPropertyName;
		}

		public void DoAction(SynchronizeContext context)
		{
			try
			{
				var result = SynchronizeHelper.GetSearchResultByID(context.ADHelper, this.adObjectID);
				using (System.DirectoryServices.DirectoryEntry ent = result.GetDirectoryEntry())
				{
					ent.Properties[this.adPropertyName].Value = oguObject.Properties[oguPropertyName];
					ent.CommitChanges();
				}
			}
			catch
			{
				this.WriteLog(context);
			}
		}

		public OGUPermission.IOguObject OguObject
		{
			get { return this.oguObject; }
		}


		public void WriteLog(SynchronizeContext context)
		{
			context.ExceptionCount++;
			SynchronizeHelper.WithDirectoryEntry(context.ADHelper, this.adObjectID, targetObject =>
			{
				LogHelper.WriteSynchronizeDBLogDetail(SynchronizeContext.Current.SynchronizeID, "重命名", oguObject.ID, oguObject.Name,
								 context.ADHelper.GetPropertyStrValue("objectGuid", targetObject),
								 context.ADHelper.GetPropertyStrValue("distinguishedName", targetObject),
								 string.Format("延迟修改：登录名修改执行错误 AD对象：{0}，权限中心对象：{1}。", targetObject.Name, this.oguObject.Name));

			}, null);
		}
	}
}
