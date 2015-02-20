using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// ���ʽʶ����
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
		/// �����
		/// </summary>
		public Operation_IDs OperationID
		{
			get
			{
				return _OperationID;
			}
		}

		/// <summary>
		/// ʶ��
		/// </summary>
		public string Identifier
		{
			get
			{
				return _Identifier;
			}
		}

		/// <summary>
		/// ��Ա��ʽ�ַ���λ��
		/// </summary>
		public int Position
		{
			get
			{
				return _Position;
			}
		}

		/// <summary>
		/// ǰʶ��
		/// </summary>
		public ParseIdentifier PrevIdentifier
		{
			get
			{
				return _PrevIdentifier;
			}
		}

		/// <summary>
		/// ��ʶ��
		/// </summary>
		public ParseIdentifier NextIdentifier
		{
			get
			{
				return _NextIdentifier;
			}
		}

		/// <summary>
		/// ��ʶ��
		/// </summary>
		public ParseIdentifier SubIdentifier
		{
			get
			{
				return _SubIdentifier;
			}
		}

		/// <summary>
		/// ��ʶ��
		/// </summary>
		public ParseIdentifier ParentIdentifier
		{
			get
			{
				return _ParentIdentifier;
			}
		}

		/// <summary>
		/// ���캯��
		/// </summary>
		public ParseIdentifier()
		{
		}

		/// <summary>
		/// ���캯��
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
