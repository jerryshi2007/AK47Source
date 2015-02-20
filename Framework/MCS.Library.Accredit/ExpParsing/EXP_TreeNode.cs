using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// �������Ľڵ�
	/// </summary>
	public class EXP_TreeNode
	{
		internal EXP_TreeNode _Left = null;
		internal EXP_TreeNode _Right = null;
		internal int _Position = 0;
		internal Operation_IDs _OperationID = Operation_IDs.OI_NONE;
		internal Object _Value = null;
		internal ArrayList _Params = null;
		internal string _FunctionName = string.Empty;

		/// <summary>
		/// ��ڵ�
		/// </summary>
		public EXP_TreeNode Left
		{
			get
			{
				return _Left;
			}
		}

		/// <summary>
		/// �ҽڵ�
		/// </summary>
		public EXP_TreeNode Right
		{
			get
			{
				return _Right;
			}
		}

		/// <summary>
		/// ���λ��
		/// </summary>
		public int Position
		{
			get
			{
				return _Position;
			}
		}

		/// <summary>
		/// ������
		/// </summary>
		public Operation_IDs OperationID
		{
			get
			{
				return _OperationID;
			}
		}

		/// <summary>
		/// ����ֵ
		/// </summary>
		public Object Value
		{
			get
			{
				return _Value;
			}
		}

		/// <summary>
		/// �������
		/// </summary>
		public ArrayList Params
		{
			get
			{
				return _Params;
			}
		}

		/// <summary>
		/// ��������
		/// </summary>
		public string FunctionName
		{
			get
			{
				return _FunctionName;
			}
		}
	}
}
