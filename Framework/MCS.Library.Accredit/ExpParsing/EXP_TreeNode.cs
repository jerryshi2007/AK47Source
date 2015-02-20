using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// 二叉树的节点
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
		/// 左节点
		/// </summary>
		public EXP_TreeNode Left
		{
			get
			{
				return _Left;
			}
		}

		/// <summary>
		/// 右节点
		/// </summary>
		public EXP_TreeNode Right
		{
			get
			{
				return _Right;
			}
		}

		/// <summary>
		/// 相对位置
		/// </summary>
		public int Position
		{
			get
			{
				return _Position;
			}
		}

		/// <summary>
		/// 操作符
		/// </summary>
		public Operation_IDs OperationID
		{
			get
			{
				return _OperationID;
			}
		}

		/// <summary>
		/// 计算值
		/// </summary>
		public Object Value
		{
			get
			{
				return _Value;
			}
		}

		/// <summary>
		/// 计算参数
		/// </summary>
		public ArrayList Params
		{
			get
			{
				return _Params;
			}
		}

		/// <summary>
		/// 函数名称
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
