using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Globalization;
using MCS.Web.WebControls;
using MCS.Library.Data.Builder;

namespace MCS.Web.Library.MVC
{
	/// <summary>
	/// 默认的用户Acl的检查器
	/// </summary>
	internal class DefaultUserProcessAclChecker : IUserProcessAclChecker
	{
		public static readonly DefaultUserProcessAclChecker Instance = new DefaultUserProcessAclChecker();

		private DefaultUserProcessAclChecker()
		{
		}

		public void CheckUserInAcl(IUser user, IWfProcess process, ref bool continueCheck)
		{
			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("RESOURCE_ID");

			inBuilder.AppendItem(process.ResourceID, process.ID);

			WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();

			whereBuilder.AppendItem("OBJECT_TYPE", "Users");
			whereBuilder.AppendItem("OBJECT_ID", user.ID);

			ConnectiveSqlClauseCollection connective = new ConnectiveSqlClauseCollection(inBuilder, whereBuilder);

			WfAclItemCollection aclItems = WfAclAdapter.Instance.LoadByBuilder(connective);

			(aclItems.Count > 0).FalseThrow(
							Translator.Translate(Define.DefaultCulture, "用户没有权限打开此文件"));
		}
	}
}
