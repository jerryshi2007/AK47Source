using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Entity
{
	public class DelayMoveToAction : IDelayAction
	{
		private OGUPermission.IOguObject oguObject;
		private string parentDn;
		private Guid targetObjectID;

		/// <summary>
		/// 初始化此类的新实例
		/// </summary>
		/// <param name="oguObject">要移动的OGU对象</param>
		/// <param name="parentDn">要移动到的父级DN</param>
		/// <param name="targetObjectID">要移动的AD对象ID（必须已经存在的）</param>
		public DelayMoveToAction(OGUPermission.IOguObject oguObject, string parentDn, Guid targetObjectID)
		{
			this.oguObject = oguObject;
			this.parentDn = parentDn;
			this.targetObjectID = targetObjectID;
		}

		public void DoAction(SynchronizeContext synchronizeContext)
		{
			using (DirectoryEntry parentEntry = synchronizeContext.ADHelper.NewEntry(parentDn))
			{
				using (DirectoryEntry targetObject = SynchronizeHelper.GetSearchResultByID(this.targetObjectID).GetDirectoryEntry())
				{
					try
					{
						targetObject.MoveTo(parentEntry);
					}
					catch (DirectoryServicesCOMException comEx)
					{
						switch (comEx.ErrorCode)
						{
							case -2147019886:
								//重名
								SynchronizeHelper.SolveConflictAboutMove(oguObject, targetObject, parentEntry);
								break;
							default:
								WriteLog(synchronizeContext, comEx.Message);
								break;
						}
					}
				}
			}
		}

		public OGUPermission.IOguObject OguObject
		{
			get { return this.oguObject; }
		}

		public void WriteLog(SynchronizeContext context, string message)
		{
			context.ExceptionCount++;
			SynchronizeHelper.WithDirectoryEntry(this.targetObjectID, targetObject =>
			{
				LogHelper.WriteDBLogDetail(SynchronizeContext.Current.SynchronizeID, "移动对象", this.oguObject.ID, this.oguObject.Name,
								 context.ADHelper.GetPropertyStrValue("objectGuid", targetObject),
								 context.ADHelper.GetPropertyStrValue("distinguishedName", targetObject),
								 string.Format("无法移动{0}到{1},错误消息{2}。", this.oguObject.Name, this.parentDn, message));

			}, null);
		}
	}
}
