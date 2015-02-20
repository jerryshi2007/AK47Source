using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow.Builders;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public interface IWfCreateActivityParamsGenerator
	{
		void Fill(WfCreateActivityParamCollection capc, PropertyDefineCollection definedCollection);

		bool UseCreateActivityParams
		{
			get;
		}
	}
}
