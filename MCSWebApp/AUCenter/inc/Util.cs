using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.Principal;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;
using System.Text;

namespace AUCenter
{
	internal static class Util
	{
		internal static IUser CurrentUser
		{
			get
			{
				return MCS.Library.Principal.DeluxeIdentity.CurrentUser;
			}
		}

		public static bool SuperVisiorMode
		{
			get
			{
				return AUPrincipalExtension.IsSupervisor(DeluxePrincipal.Current);
			}
		}

		internal static bool ContainsPermission(SCContainerAndPermissionCollection permissions, string containerID, string permission)
		{
			if (permissions == null || string.IsNullOrEmpty(containerID))
				return false;
			else
				return permissions.ContainsKey(containerID, permission);
		}

		internal static void EnsureOperationSafe()
		{
			if (TimePointContext.Current.UseCurrentTime == false)
				throw new InvalidOperationException("不在当前时间上下文不可以进行操作。");
		}

		internal static void SaveSearchCondition(MCS.Web.WebControls.SearchEventArgs e, MCS.Web.WebControls.DeluxeSearch control, string pageKey, object dataToSave)
		{
			if (e.IsSaveCondition && string.IsNullOrEmpty(e.ConditionName) == false)
			{
				UserCustomSearchCondition condition = Util.NewSearchCondition(pageKey, "Default", e.ConditionName);

				condition.ConditionContent = JSONSerializerExecute.Serialize(dataToSave);

				UserCustomSearchConditionAdapter.Instance.Update(condition);

				control.UserCustomSearchConditions = DbUtil.LoadSearchCondition(pageKey, "Default");
			}
		}

		internal static UserCustomSearchCondition NewSearchCondition(string resourceKey, string conditionType, string conditionName)
		{
			UserCustomSearchCondition condition = new UserCustomSearchCondition()
			{
				ID = Guid.NewGuid().ToString(),
				UserID = Util.CurrentUser.ID,
				ResourceID = resourceKey,
				ConditionName = conditionName,
				ConditiontType = conditionType,
				CreateTime = DateTime.Now
			};

			return condition;
		}

		/// <summary>
		/// 将脚本放在块中
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		internal static string SurroundScriptBlock(string script)
		{
			return "<script type=\"text/javascript\">" + script + "</script>";
		}

		internal static StringBuilder BeginScriptBlock()
		{
			StringBuilder strB = new StringBuilder("<script type=\"text/javascript\">");
			strB.AppendLine();
			return strB;
		}

		internal static string EndScriptBlock(StringBuilder builder)
		{
			builder.AppendLine("</script>");
			return builder.ToString();
		}

		public static DateTime FromJavascriptTime(long point)
		{
			DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			dt = dt.AddTicks(point * 10000).ToLocalTime(); // JavaScript返回UTC时刻从1970年1月1日 0点开始到目前为止的毫秒数
			return dt;
		}
	}
}