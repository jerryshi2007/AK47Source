using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// ���ʽ��ڶ���
	/// </summary>
	/// <remarks>
	/// ���ʽ��ڶ���
	/// </remarks>
	public interface IExpParsing
	{
		/// <summary>
		/// ���ʽ����
		/// </summary>
		/// <param name="strFuncName">��������</param>
		/// <param name="arrParams">������ʹ�õĲ�����</param>
		/// <param name="parseObj">������ʽ</param>
		/// <returns>���ʽ�������</returns>
		object CalculateUserFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj);

		/// <summary>
		/// У���û��Զ�����ʽ
		/// </summary>
		/// <param name="strFuncName">��������</param>
		/// <param name="arrParams">������ʹ�õĲ�����</param>
		/// <param name="parseObj">������ʽ</param>
		/// <returns>�û��Զ�����ʽ�Ľ������</returns>
		object CheckUserFunction(string strFuncName, ParamObject[] arrParams, ParseExpression parseObj);
	}
}
