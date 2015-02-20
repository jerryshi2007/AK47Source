using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;

namespace MCS.Web.Library.MVC
{
	/// <summary>
	/// 表单相关的CommandState的泛型虚基类，携带强类型Data属性
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public class FormCommandStateGenericBase<T> : FormCommandState
	{
		public new T Data
		{
			get
			{
				return (T)base.Data;
			}
			set
			{
				base.Data = value;
			}
		}

		/// <summary>
		/// 生成全文数据对象
		/// </summary>
		/// <returns></returns>
		public override AppCommonInfo ToAppCommonInfo(string content)
		{
			AppCommonInfo commonInfo = new AppCommonInfo();

			if (Data is IBusinessObject)
			{
				IBusinessObject wob = Data as IBusinessObject;
				//commonInfo.ResourceID = wob.ID;
				commonInfo.Subject = wob.Subject;
			}

			if (DeluxePrincipal.IsAuthenticated)
			{
				commonInfo.Creator = DeluxeIdentity.CurrentUser;
				commonInfo.TopOU = DeluxeIdentity.CurrentUser.TopOU;
				commonInfo.Department = DeluxeIdentity.CurrentUser.TopOU;
			}

			commonInfo.Content = content;

			if (WfClientContext.Current.OriginalActivity != null)
			{
				commonInfo.ResourceID = WfClientContext.Current.OriginalActivity.Process.SearchID;
				commonInfo.TopOU = WfClientContext.Current.User.TopOU;
				commonInfo.Department = WfClientContext.Current.User.Parent;
				commonInfo.ApplicationName = WfClientContext.Current.OriginalActivity.Process.Descriptor.ApplicationName;
				commonInfo.ProgramName = WfClientContext.Current.OriginalActivity.Process.Descriptor.ProgramName;
				//commonInfo.Url = string.Format("{0}?resourceID={1}",
				//    WfClientContext.Current.EntryUri.ToString(),
				//    commonInfo.ResourceID);
				commonInfo.Url = string.Format("{0}?resourceID={1}&processID={2}",
					WfClientContext.Current.EntryUri.ToString(),
					WfClientContext.Current.OriginalActivity.Process.ResourceID,
					WfClientContext.Current.OriginalActivity.Process.ID);


				if (WfClientContext.Current.OriginalActivity.Process.OwnerDepartment != null)
					commonInfo.DraftDepartmentName = WfClientContext.Current.OriginalActivity.Process.OwnerDepartment.GetDepartmentDescription();

				commonInfo.Content += " " + commonInfo.DraftDepartmentName;

				commonInfo.SyncProcessStatus(WfClientContext.Current.OriginalActivity.Process.ApprovalRootProcess);
			}

			return commonInfo;
		}
	}
}
