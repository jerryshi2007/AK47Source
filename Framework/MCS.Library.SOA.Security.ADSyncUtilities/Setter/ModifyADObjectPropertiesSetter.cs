using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;
using System.Diagnostics;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class ModifyADObjectPropertiesSetter : OguAndADObjectSetterBase
	{
		public override void Convert(ObjectModifyType modifyType, IOguObject srcObject, DirectoryEntry targetObject, string context)
		{
			SetterContext setterContext = new SetterContext();

			ConvertProperties(srcObject, targetObject, setterContext);

			targetObject.CommitChanges();

			if (!ObjectComparerHelper.AreParentPathEqaul(srcObject, targetObject))
				MoveItemToNewPath(srcObject, targetObject);

			DoAfterObjectUpdatedOP(srcObject, targetObject, setterContext);
		}

		private static void MoveItemToNewPath(IOguObject oguObject, DirectoryEntry targetObject)
		{
			ADHelper adHelper = SynchronizeContext.Current.ADHelper;

			IDMapping mapping = null;
			var oguParent = oguObject.Parent;
			if (oguParent != null)
			{
				mapping = SynchronizeContext.Current.IDMapper.GetAdObjectMapping(oguParent.ID, false);
			}

			if (mapping != null)
			{
				using (var adEntry = SynchronizeHelper.GetDirectoryEntryByID(SynchronizeContext.Current.ADHelper, mapping.ADObjectGuid))
				{
					if (adEntry != null)
					{
						try
						{
							targetObject.MoveTo(adEntry);
						}
						catch (DirectoryServicesCOMException comEx)
						{
							switch (comEx.ErrorCode)
							{
								case -2147019886:
									//重名
									SynchronizeHelper.SolveConflictAboutMove(oguObject, targetObject, adEntry);
									break;
								case -2147016656:
									Trace.WriteLine("修改对象属性后，移动对象说对象不存在??被移动的对象:" + oguObject.Name);
									throw;
								default:
									throw;
							}
						}
					}
				}
			}
			else
			{
				var parentDn = SynchronizeHelper.GetParentObjectDN(oguObject);

				Trace.WriteLine("不可靠的方法被执行,目标不存在时，靠DN来延迟移动。Careful !");
				// 父对象还不存在，这可能是被重命名了
				//SynchronizeContext.Current.DelayActions.Add(new DelayMoveToAction(oguObject, parentDn, targetObject.Guid));

				// 没有这种可能性
				var context = SynchronizeContext.Current;
				context.ExceptionCount++;

				LogHelper.WriteSynchronizeDBLogDetail(SynchronizeContext.Current.SynchronizeID, "移动对象", oguObject.ID, oguObject.Name,
								 context.ADHelper.GetPropertyStrValue("objectGuid", targetObject),
								 context.ADHelper.GetPropertyStrValue("distinguishedName", targetObject),
								 string.Format("父对象:{0}无映射，无法移动。", parentDn));
			}
		}
	}
}