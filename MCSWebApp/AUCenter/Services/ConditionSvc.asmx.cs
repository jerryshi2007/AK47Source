using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.Expression;

namespace AUCenter.Services
{
	/// <summary>
	/// ConditionSvc 的摘要说明
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]
	// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
	// [System.Web.Script.Services.ScriptService]
	public class ConditionSvc : System.Web.Services.WebService
	{
		[WebMethod(Description = "获取指定单元的条件")]
		public SCConditionCollection GetUnitConditions(string groupId)
		{
			var grp = DbUtil.GetEffectiveObject<AU.AdminUnit>(groupId);

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("OwnerID", grp.ID);
			where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			return AU.Adapters.AUConditionAdapter.Instance.Load(where, DateTime.MinValue);
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
