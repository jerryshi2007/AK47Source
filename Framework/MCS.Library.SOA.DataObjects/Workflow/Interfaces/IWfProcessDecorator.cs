using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 流程活动的装饰者。在流程的Executor执行完后，对流程有个二次加工过程，例如扩展一些活动、添加秘书活动等
	/// </summary>
	public interface IWfProcessDecorator
	{
		/// <summary>
		/// 修饰流程
		/// </summary>
		/// <param name="process"></param>
		void Decorate(IWfProcess process);
	}

	public class WfProcessDecoratorCollection : EditableDataObjectCollectionBase<IWfProcessDecorator>
	{
		/// <summary>
		/// 修饰流程
		/// </summary>
		/// <param name="process"></param>
		public void Decorate(IWfProcess process)
		{
			this.ForEach(d => d.Decorate(process));
		}
	}
}
