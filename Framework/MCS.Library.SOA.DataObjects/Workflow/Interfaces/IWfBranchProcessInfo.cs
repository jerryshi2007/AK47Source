using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 分支流程信息的接口定义
	/// </summary>
	public interface IWfBranchProcessInfo
	{
		/// <summary>
		/// 启动分支流程的父节点
		/// </summary>
		IWfActivity OwnerActivity
		{
			get;
		}

		/// <summary>
		/// 分支流程实例
		/// </summary>
		IWfProcess Process
		{
			get;
		}
	}
}
