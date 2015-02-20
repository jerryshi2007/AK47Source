using System;
using System.Web.Services;
using MCS.Library.Data.Builder;
using MCS.Library.Expression;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter.Services
{
	/// <summary>
	/// 提供群组条件编辑的客户端服务
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]
	public class ConditionSvc : System.Web.Services.WebService
	{
		[WebMethod(Description = "获取指定群组的条件")]
		public SCConditionCollection GetGroupConditions(string groupId)
		{
			var grp = DbUtil.GetEffectiveObject<SCGroup>(groupId);

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("OwnerID", grp.ID);
			where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			return PC.Adapters.SCConditionAdapter.Instance.Load(where, DateTime.MinValue);
		}

		[WebMethod(Description = "获取指定角色的条件")]
		public SCConditionCollection GetRoleConditions(string roleId)
		{
			var role = DbUtil.GetEffectiveObject<SCRole>(roleId);

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("OwnerID", role.ID);
			where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			return PC.Adapters.SCConditionAdapter.Instance.Load(where, DateTime.MinValue);
		}

		[WebMethod(Description = "校验表达式的有效性")]
		public bool ValidateExpression(string expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");
			try
			{
				var result = ExpressionParser.Parse(expression);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}