using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// ���ʽ�쳣
	/// </summary>
	public class ParsingException : System.Exception
	{
		private Parse_Error _Reason = Parse_Error.peNone;
		private int _Position = -1;

		/// <summary>
		/// ���캯��
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
		/// �쳣ԭ��
		/// </summary>
		public Parse_Error Reason
		{
			get
			{
				return _Reason;
			}
		}

		/// <summary>
		/// �쳣λ��
		/// </summary>
		public int Position
		{
			get
			{
				return _Position;
			}
		}

		/// <summary>
		/// �����׳����ʽ�쳣
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
