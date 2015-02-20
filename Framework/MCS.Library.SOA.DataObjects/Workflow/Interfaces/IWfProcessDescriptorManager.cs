using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public interface IWfProcessDescriptorManager
	{
		IWfProcessDescriptor LoadDescriptor(string processKey);
		IWfProcessDescriptor GetDescriptor(string processKey);
		void DeleteDescriptor(string processKey);
		void SaveDescriptor(IWfProcessDescriptor processDesp);
		bool ExsitsProcessKey(string processKey);
	}
}
