using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// ��¼����ʹ�õ������ж�
	/// </summary>
	public enum LogonType
	{
		/// <summary>
		/// �Ƿ���ʽ
		/// </summary>
		[EnumItemDescription("�Ƿ���ʽ��¼")]
		None = 0,
		/// <summary>
		/// ʹ��LOGON_NAME��¼ϵͳ
		/// </summary>
		[EnumItemDescription("LOGON_NAME��¼")]
		LOGON_NAME = 1,
		/// <summary>
		/// ʹ��IC����¼ϵͳ
		/// </summary>
		[EnumItemDescription("IC����¼")]
		IC_CARD = 2,
		/// <summary>
		/// ʹ��UsbKey��ʽ��¼
		/// </summary>
		[EnumItemDescription("UsbKey��ʽ��¼")]
		USB_KEY = 4,
		/// <summary>
		/// Ա���ŷ�ʽ
		/// </summary>
		[EnumItemDescription("Ա���ŷ�ʽ")]
		POSTAL = 8
	}
}
