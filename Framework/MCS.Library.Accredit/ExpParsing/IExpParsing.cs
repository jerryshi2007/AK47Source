using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// 表达式借口定义
	/// </summary>
	/// <remarks>
	/// 表达式借口定义
	/// </remarks>
	public interface IExpParsing
	{
		/// <summary>
		/// 表达式计算
		/// </summary>
		/// <param name="strFuncName">函数名称</param>
		/// <param name="arrParams">函数中使用的参数组</param>
		/// <param name="parseObj">表达解析式</param>
		/// <returns>表达式解析结果</returns>
		object CalculateUserFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj);

		/// <summary>
		/// 校验用户自定义表达式
		/// </summary>
		/// <param name="strFuncName">函数名称</param>
		/// <param name="arrParams">函数中使用的参数组</param>
		/// <param name="parseObj">表达解析式</param>
		/// <returns>用户自定义表达式的解析结果</returns>
		object CheckUserFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj);
	}
}
