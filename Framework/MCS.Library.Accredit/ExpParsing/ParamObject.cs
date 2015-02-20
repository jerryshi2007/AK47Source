using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// 参数对象
	/// </summary>
	public class ParamObject
	{
		internal object _Value = null;
		internal int _Position = -1;

		/// <summary>
		/// 构造函数
		/// </summary>
		public ParamObject()
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="v"></param>
		/// <param name="nPos"></param>
		public ParamObject(object v, int nPos)
		{
			_Value = v;
			_Position = nPos;
		}

		/// <summary>
		/// 参数值
		/// </summary>
		public object Value
		{
			get
			{
				return _Value;
			}
		}

		/// <summary>
		/// 参数位置
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
