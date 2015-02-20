using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// ��������
	/// </summary>
	public enum Operation_IDs
	{
		/// <summary>
		/// δ��ȷ
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
		/// �û��Զ���
		/// </summary>
		OI_USERDEFINE,
		/// <summary>
		/// �ַ���
		/// </summary>
		OI_STRING,
		/// <summary>
		/// ����
		/// </summary>
		OI_NUMBER,
		/// <summary>
		/// ����
		/// </summary>
		OI_BOOLEAN,
		/// <summary>
		/// ʱ��
		/// </summary>
		OI_DATETIME,
	}
}
