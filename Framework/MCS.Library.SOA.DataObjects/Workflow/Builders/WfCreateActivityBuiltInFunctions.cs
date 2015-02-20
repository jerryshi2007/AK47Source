using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Expression;

namespace MCS.Library.SOA.DataObjects.Workflow.Builders
{
	/// <summary>
	/// 创建活动时内嵌函数的实现
	/// </summary>
	internal class WfCreateActivityBuiltInFunctions
	{
		public static readonly WfCreateActivityBuiltInFunctions Instance = new WfCreateActivityBuiltInFunctions();

		private WfCreateActivityBuiltInFunctions()
		{
		}

		/// <summary>
		/// 是否是内置函数
		/// </summary>
		/// <param name="funcName"></param>
		/// <returns></returns>
		public bool IsFunction(string funcName)
		{
			funcName.CheckStringIsNullOrEmpty("funcName");

			BuiltInFunctionInfoCollection funcsInfo = BuiltInFunctionHelper.GetBuiltInFunctionsInfo(this.GetType());

			return funcsInfo.Contains(funcName);
		}

		/// <summary>
		/// 计算内置函数
		/// </summary>
		/// <param name="funcName"></param>
		/// <param name="arrParams"></param>
		/// <param name="callerContext"></param>
		/// <returns></returns>
		public object Calculate(string funcName, ParamObjectCollection arrParams, object callerContext)
		{
			object result = null;

			if (WfCreateActivityBuiltInFunctions.Instance.IsFunction(funcName) == false)
			{
				result = MatchedActivityKey(funcName, (WfCreateActivityFunctionContext)callerContext);

				if (result == null || (string)result == string.Empty)
					throw new InvalidOperationException(string.Format("\"{0}\"是非法的函数名称", funcName));
			}
			else
				result = BuiltInFunctionHelper.ExecuteFunction(funcName, this, arrParams, callerContext);

			return result;
		}

		#region BuiltInFunctions
		[BuiltInFunction("FirstActivity", "第一个活动")]
		private string FirstActivityKey(WfCreateActivityFunctionContext callerContext)
		{
			string result = string.Empty;

			if (callerContext.ProcessDescriptor.InitialActivity != null)
				result = callerContext.ProcessDescriptor.InitialActivity.Key;

			return result;
		}

		[BuiltInFunction("LastActivity", "最后一个活动")]
		private string LastActivityKey(WfCreateActivityFunctionContext callerContext)
		{
			string result = string.Empty;

			if (callerContext.ProcessDescriptor.CompletedActivity != null)
				result = callerContext.ProcessDescriptor.CompletedActivity.Key;

			return result;
		}

		[BuiltInFunction("Key", "指定Key的活动")]
		private string MatchedActivityKey(string key, WfCreateActivityFunctionContext callerContext)
		{
			string result = string.Empty;

			IWfActivityDescriptor matchedActivityDesp = callerContext.ProcessDescriptor.Activities[key];

			if (matchedActivityDesp != null)
				result = matchedActivityDesp.Key;

			return result;
		}

		[BuiltInFunction("CodeName", "根据CodeName得到符合条件的第一个Activity")]
		private string MatchedCodeNameActivityKey(string codeName, WfCreateActivityFunctionContext callerContext)
		{
			string result = string.Empty;

			IWfActivityDescriptor matchedActivityDesp = callerContext.ProcessDescriptor.Activities.Find(actDesp => actDesp.CodeName == codeName);

			if (matchedActivityDesp != null)
				result = matchedActivityDesp.Key;

			return result;
		}

		[BuiltInFunction("SN", "根据动态活动参数的序号得到已创建的活动的Key")]
		private string SNActivityKey(int sn, WfCreateActivityFunctionContext callerContext)
		{
			string result = string.Empty;

			WfCreateActivityParam matchParam = callerContext.CreateActivityParams.FindByActivitySN(sn);

			if (matchParam != null && matchParam.CreatedDescriptor != null)
				result = matchParam.CreatedDescriptor.Key;

			return result;
		}

		[BuiltInFunction(WfCreateActivityParam.DefaultNextActivityDescription, "动态创建活动的下一个活动")]
		private string DefaultNextActivityKey(WfCreateActivityFunctionContext callerContext)
		{
			string result = string.Empty;

			if (callerContext.CurrentActivityParam.DefaultNextDescriptor != null)
				result = callerContext.CurrentActivityParam.DefaultNextDescriptor.Key;

			return result;
		}
		#endregion
	}
}
