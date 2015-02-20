using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// �������
	/// </summary>
	public enum Parse_Error
	{
		/// <summary>
		/// δ��ȷ
		/// </summary>
		peNone = 0,
		/// <summary>
		/// �Ƿ��ַ�
		/// </summary>
		peInvalidChar,
		/// <summary>
		/// �Ƿ��ַ���
		/// </summary>
		peInvalidString,
		/// <summary>
		/// �Ƿ������
		/// </summary>
		peInvalidOperator,
		/// <summary>
		/// �޷�ƥ��
		/// </summary>
		peTypeMismatch,
		/// <summary>
		/// �Ƿ�����
		/// </summary>
		peInvalidParam,
		/// <summary>
		/// �Ƿ��û��Զ���ֵ
		/// </summary>
		peInvalidUFValue,
		/// <summary>
		/// syntax����
		/// </summary>
		peSyntaxError,
		/// <summary>
		/// �������
		/// </summary>
		peFloatOverflow,
		/// <summary>
		/// ȱ����
		/// </summary>
		peCharExpected,
		/// <summary>
		/// ��������
		/// </summary>
		peFuncError,
		/// <summary>
		/// ȱ�����
		/// </summary>
		peNeedOperand,
		/// <summary>
		/// �޷���ʽ��
		/// </summary>
		peFormatError,
	}
}
