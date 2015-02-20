using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// 表达式异常
	/// </summary>
	public class ParsingException : System.Exception
	{
		private Parse_Error _Reason = Parse_Error.peNone;
		private int _Position = -1;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="pe"></param>
		/// <param name="nPosition"></param>
		/// <param name="strMsg"></param>
		public ParsingException(Parse_Error pe, int nPosition, string strMsg)
			: base(strMsg)
		{
			_Reason = pe;
			_Position = nPosition;
		}

		/// <summary>
		/// 异常原因
		/// </summary>
		public Parse_Error Reason
		{
			get
			{
				return _Reason;
			}
		}

		/// <summary>
		/// 异常位置
		/// </summary>
		public int Position
		{
			get
			{
				return _Position;
			}
		}

		/// <summary>
		/// 主动抛出表达式异常
		/// </summary>
		/// <param name="pe"></param>
		/// <param name="nPosition"></param>
		/// <param name="strParams"></param>
		/// <returns></returns>
		static public ParsingException NewParsingException(Parse_Error pe, int nPosition, params string[] strParams)
		{
			ResourceManager rm = new ResourceManager("ExpParsing.Resource", Assembly.GetExecutingAssembly());

			string strID = pe.ToString();

			string strText = rm.GetString(strID);

			strText = string.Format(strText, strParams);

			if (nPosition >= 0)
			{
				strText = string.Format(rm.GetString("position"), nPosition + 1) + ", " + strText;
			}

			return new ParsingException(pe, nPosition, strText);
		}
	}
}
