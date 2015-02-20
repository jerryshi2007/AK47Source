using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Entity
{
	public class DelayRenameAction : IDelayAction
	{
		private OGUPermission.IOguObject oguObject;
		private string adObjectID;

		public DelayRenameAction(OGUPermission.IOguObject oguObject, string adObjectID)
		{
			this.oguObject = oguObject;
			this.adObjectID = adObjectID;
		}

		public void DoAction(SynchronizeContext context)
		{
			try
			{
				var result = SynchronizeHelper.GetSearchResultByID(context.ADHelper, this.adObjectID);
				using (System.DirectoryServices.DirectoryEntry ent = result.GetDirectoryEntry())
				{
					ent.Rename(oguObject.ObjectType.SchemaTypeToPrefix() + "=" + ADHelper.EscapeString(oguObject.Name));
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
								 string.Format("延迟修改：重命名{0}为{1}未能执行。", targetObject.Name, this.oguObject.Name));

			}, null);
		}
	}
}
