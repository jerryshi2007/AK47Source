using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// 表达式识别器
	/// </summary>
	public class ParseIdentifier
	{
		internal Operation_IDs _OperationID = Operation_IDs.OI_NONE;
		internal string _Identifier = string.Empty;
		internal int _Position = -1;
		internal ParseIdentifier _PrevIdentifier = null;
		internal ParseIdentifier _NextIdentifier = null;
		internal ParseIdentifier _SubIdentifier = null;
		internal ParseIdentifier _ParentIdentifier = null;

		/// <summary>
		/// 运算符
		/// </summary>
		public Operation_IDs OperationID
		{
			get
			{
				return _OperationID;
			}
		}

		/// <summary>
		/// 识别
		/// </summary>
		public string Identifier
		{
			get
			{
				return _Identifier;
			}
		}

		/// <summary>
		/// 相对表达式字符串位置
		/// </summary>
		public int Position
		{
			get
			{
				return _Position;
			}
		}

		/// <summary>
		/// 前识别
		/// </summary>
		public ParseIdentifier PrevIdentifier
		{
			get
			{
				return _PrevIdentifier;
			}
		}

		/// <summary>
		/// 后识别
		/// </summary>
		public ParseIdentifier NextIdentifier
		{
			get
			{
				return _NextIdentifier;
			}
		}

		/// <summary>
		/// 子识别
		/// </summary>
		public ParseIdentifier SubIdentifier
		{
			get
			{
				return _SubIdentifier;
			}
		}

		/// <summary>
		/// 父识别
		/// </summary>
		public ParseIdentifier ParentIdentifier
		{
			get
			{
				return _ParentIdentifier;
			}
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public ParseIdentifier()
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="oID"></param>
		/// <param name="strID"></param>
		/// <param name="nPos"></param>
		/// <param name="prev"></param>
		public ParseIdentifier(Operation_IDs oID, string strID, int nPos, ParseIdentifier prev)
		{
			_OperationID = oID;
			_Identifier = strID;
			_Position = nPos;
			_PrevIdentifier = prev;
		}
	}
}
