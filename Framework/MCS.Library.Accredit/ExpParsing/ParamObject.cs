using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// ��������
	/// </summary>
	public class ParamObject
	{
		internal object _Value = null;
		internal int _Position = -1;

		/// <summary>
		/// ���캯��
		/// </summary>
		public ParamObject()
		{
		}

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="v"></param>
		/// <param name="nPos"></param>
		public ParamObject(object v, int nPos)
		{
			_Value = v;
			_Position = nPos;
		}

		/// <summary>
		/// ����ֵ
		/// </summary>
		public object Value
		{
			get
			{
				return _Value;
			}
		}

		/// <summary>
		/// ����λ��
		/// </summary>
		public int Position
		{
			get
			{
				return _Position;
			}
		}
	}
}
