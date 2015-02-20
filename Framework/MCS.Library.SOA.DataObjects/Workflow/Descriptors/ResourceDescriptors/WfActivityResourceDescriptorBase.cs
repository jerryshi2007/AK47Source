using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 和流程活动相关的资源描述类的基类
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public abstract class WfActivityResourceDescriptorBase : WfResourceDescriptor
	{
		/// <summary>
		/// 流程环节的Key
		/// </summary>
		public string ActivityKey { get; set; }

		//根据ActivityKey和OwnerProcess得到TargetActivity
		public IWfActivity TargetActivity
		{
			get
			{
				IWfActivity result = null;

				if (ActivityKey.IsNotEmpty() && this.ProcessInstance != null)
				{
					result = this.ProcessInstance.Activities.FindActivityByDescriptorKey(ActivityKey);
				}

				return result;
			}
		}

		protected override void ToXElement(XElement element)
		{
			if (this.ActivityKey.IsNotEmpty())
				element.SetAttributeValue("activityKey", this.ActivityKey);
		}
	}
}
