using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class WfProcessContext : WfContextDictionaryBase<string>
    {
        internal WfProcessContext()
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class WfActivityContext : WfContextDictionaryBase<string>
    {
        internal WfActivityContext()
        {
        }
    }

	[Serializable]
	public class WfBranchProcessInfoContext : WfContextDictionaryBase<string>
	{
		internal WfBranchProcessInfoContext()
        {
        }
	}

	[Serializable]
	public class WfOperationContext : WfContextDictionaryBase<string>
	{
		internal WfOperationContext()
        {
        }
	}
}
