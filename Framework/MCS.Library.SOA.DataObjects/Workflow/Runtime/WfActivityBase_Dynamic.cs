using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow.Builders;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public abstract partial class WfActivityBase
    {
        /// <summary>
        /// 根据此活动生成动态活动
        /// </summary>
        internal void GenerateDynamicActivities()
        {
            IList<IWfActivityDescriptor> createdInstActivities = this.Descriptor.GenerateDynamicActivities();

            IWfActivityDescriptor msActDesp = this.GetMainStreamActivityDescriptor();

			//能够找到对应的主线活动，且该动态活动没有执行过
			if (createdInstActivities != null && msActDesp != null)
			{
				IList<IWfActivityDescriptor> createdMSActivities = msActDesp.GenerateDynamicActivities();

				(createdInstActivities.Count == createdMSActivities.Count).FalseThrow("Create Dynamic activities not matched between instance and mainstream.");

				for (int i = 0; i < createdInstActivities.Count; i++)
				{
					((WfActivityBase)createdInstActivities[i].Instance).MainStreamActivityKey = createdMSActivities[i].Key;
				}
			}
        }
    }
}
