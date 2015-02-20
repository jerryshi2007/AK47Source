using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.Workflow.Descriptors
{
	/// <summary>
	/// 简单的流程节点描述的集合
	/// </summary>
	public class WfSimpleActivityDescriptorCollection : DataObjectCollectionBase<IWfActivityDescriptor>
	{
		internal void Add(IWfActivityDescriptor actDesp)
		{
			InnerAdd(actDesp);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public IWfActivityDescriptor this[int index]
		{
			get
			{
				return (IWfActivityDescriptor)List[index];
			}
		}
	}
}
