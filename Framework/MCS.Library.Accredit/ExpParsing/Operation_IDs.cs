using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// 操作类型
	/// </summary>
	public enum Operation_IDs
	{
		/// <summary>
		/// 未明确
		/// </summary>
		OI_NONE = 0,
		/// <summary>
		/// Not
		/// </summary>
		OI_NOT = 120,
		/// <summary>
		/// +
		/// </summary>
		OI_ADD,
		/// <summary>
		/// -
		/// </summary>
		OI_MINUS,
		/// <summary>
		/// *
		/// </summary>
		OI_MUL,
		/// <summary>
		/// /
		/// </summary>
		OI_DIV,
		/// <summary>
		/// Neg
		/// </summary>
		OI_NEG,
		/// <summary>
		/// =
		/// </summary>
		OI_EQUAL,
		/// <summary>
		/// !=
		/// </summary>
		OI_NOT_EQUAL,
		/// <summary>
		/// &gt;
		/// </summary>
		OI_GREAT,
		/// <summary>
		/// &gt;=
		/// </summary>
		OI_GREATEQUAL,
		/// <summary>
		/// &lt;
		/// </summary>
		OI_LESS,
		/// <summary>
		/// &lt;=
		/// </summary>
		OI_LESSEQUAL,
		/// <summary>
		/// and
		/// </summary>
		OI_LOGICAL_AND,
		/// <summary>
		/// ||
		/// </summary>
		OI_LOGICAL_OR,
		/// <summary>
		/// (
		/// </summary>
		OI_LBRACKET,
		/// <summary>
		/// )
		/// </summary>
		OI_RBRACKET,
		/// <summary>
		/// comma
		/// </summary>
		OI_COMMA,
		/// <summary>
		/// 用户自定义
		/// </summary>
		OI_USERDEFINE,
		/// <summary>
		/// 字符串
		/// </summary>
		OI_STRING,
		/// <summary>
		/// 数字
		/// </summary>
		OI_NUMBER,
		/// <summary>
		/// 布尔
		/// </summary>
		OI_BOOLEAN,
		/// <summary>
		/// 时间
		/// </summary>
		OI_DATETIME,
	}
}
