using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// ����������
	/// </summary>
	public class RankDefine : Interfaces.IRankDefine
	{
		/// <summary>
		/// ���캯������ʼ��һ����������
		/// </summary>
		/// <param name="strCodeName">Ӣ�ı�־����</param>
		/// <param name="iSortID">�ڲ������</param>
		/// <param name="strName">��ʾ����</param>
		/// <param name="iVisible">�Ƿ�Ҫ��չ�֣�0�����ɼ���1���ɼ���</param>
		public RankDefine(string strCodeName, int iSortID, string strName, int iVisible)
		{
			_StrCodeName = strCodeName;
			_StrName = strName;
			_ISortID = iSortID;

			if (iVisible == 1)
				_BVisible = true;
		}

		private string _StrCodeName = string.Empty;
		/// <summary>
		/// �����������Ӧ��Ӣ�ı�ʶ
		/// </summary>
		public string CodeName
		{
			get
			{
				return _StrCodeName;
			}
		}

		private int _ISortID = 0;
		/// <summary>
		/// ����������Ĵ��򣨸߼���ʹ���
		/// </summary>
		public int SortID
		{
			get
			{
				return _ISortID;
			}
		}

		private string _StrName = string.Empty;
		/// <summary>
		/// ���������������
		/// </summary>
		public string Name
		{

			get
			{
				return _StrName;
			}
		}

		private bool _BVisible = false;
		/// <summary>
		/// �����������Ƿ�ɼ�
		/// </summary>
		public bool Visible
		{
			get
			{
				return _BVisible;
			}
		}
	}
}
