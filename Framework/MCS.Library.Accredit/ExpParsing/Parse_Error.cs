using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// 错误类别
	/// </summary>
	public enum Parse_Error
	{
		/// <summary>
		/// 未明确
		/// </summary>
		peNone = 0,
		/// <summary>
		/// 非法字符
		/// </summary>
		peInvalidChar,
		/// <summary>
		/// 非法字符串
		/// </summary>
		peInvalidString,
		/// <summary>
		/// 非法计算符
		/// </summary>
		peInvalidOperator,
		/// <summary>
		/// 无法匹配
		/// </summary>
		peTypeMismatch,
		/// <summary>
		/// 非法参数
		/// </summary>
		peInvalidParam,
		/// <summary>
		/// 非法用户自定义值
		/// </summary>
		peInvalidUFValue,
		/// <summary>
		/// syntax错误
		/// </summary>
		peSyntaxError,
		/// <summary>
		/// 浮点溢出
		/// </summary>
		peFloatOverflow,
		/// <summary>
		/// 缺内容
		/// </summary>
		peCharExpected,
		/// <summary>
		/// 函数错误
		/// </summary>
		peFuncError,
		/// <summary>
		/// 缺运算符
		/// </summary>
		peNeedOperand,
		/// <summary>
		/// 无法格式化
		/// </summary>
		peFormatError,
	}
}
