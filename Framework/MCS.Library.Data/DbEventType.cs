using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Data
{
	/// <summary>
	/// ���ݷ��ʵ���ʱ������
	/// </summary>
	public enum DbEventType
	{
		/// <summary>
		/// ����ִ��ǰ
		/// </summary>
		BeforeExecution,
		/// <summary>
		/// ����ִ�к�
		/// </summary>
		AfterExecution,
		/// <summary>
		/// �����쳣�׶�
		/// </summary>
		Exception
	}
}
