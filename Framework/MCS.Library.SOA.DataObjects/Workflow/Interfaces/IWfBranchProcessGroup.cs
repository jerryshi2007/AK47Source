using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public interface IWfBranchProcessGroup
	{
		/// <summary>
		/// 分支流程模板
		/// </summary>
		IWfBranchProcessTemplateDescriptor ProcessTemplate
		{
			get;
		}

		/// <summary>
		/// 分支流程实例
		/// </summary>
		WfProcessCollection Branches
		{
			get;
		}

		///// <summary>
		///// 分支流程的统计信息，为了保持兼容性，对于老流程，会生成此属性，然后根据流程数目进行初始化
		///// </summary>
		//WfBranchProcessStatistics BranchProcessStatistics
		//{
		//    get;
		//}

		/// <summary>
		/// 启动分支流程的父节点
		/// </summary>
		IWfActivity OwnerActivity
		{
			get;
		}

		/// <summary>
		/// 加载方式
		/// </summary>
		DataLoadingType LoadingType
		{
			get;
		}

		/// <summary>
		/// 组中的分支流程是否还在阻塞主流程
		/// </summary>
		/// <returns></returns>
		bool IsBlocking();
	}
}
