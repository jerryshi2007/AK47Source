using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using MCS.Library.Expression;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.UserFunction
{
	public class WfTestUserFunction : IWfCalculateUserFunction
	{
		public object CalculateUserFunction(string funcName, ParamObjectCollection arrParams, object callerContext)
		{
			object result = null;

			if (this.IsFunction(funcName))
				result = BuiltInFunctionHelper.ExecuteFunction(funcName, this, arrParams, callerContext);

			return result;
		}

		public bool IsFunction(string funcName)
		{
			funcName.CheckStringIsNullOrEmpty("funcName");

			BuiltInFunctionInfoCollection funcsInfo = BuiltInFunctionHelper.GetBuiltInFunctionsInfo(this.GetType());

			return funcsInfo.Contains(funcName);
		}

		[BuiltInFunction("InlineUser", "内置的用户")]
		public IUser InlineUser(string configKey)
		{
			return (IUser)OguObjectSettings.GetConfig().Objects[configKey].Object;
		}
	}
}
