using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest
{
	[TestClass]
	public class ProcessDescriptorPropertiesTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.Data)]
		[Description("加载流程描述，更新描述中Properties的配置属性")]
		public void LoadProcessDesp()
		{
			IWfProcessDescriptor procDesp = WfProcessTestCommon.CreateProcessDescriptor();
			WfProcessDescriptorManager.SaveDescriptor(procDesp);
			string key = procDesp.Key; ;
			IWfProcessDescriptor desp = WfProcessDescriptorManager.LoadDescriptor(key);

			//本机修改过app.config，确实为此。
			Assert.AreEqual(8, desp.Properties.Count, "流程在配置文件中的属性项。任意添加|减少项就会报错");

			Assert.AreEqual(9, desp.InitialActivity.Properties.Count, "活动在配置文件中的属性项。任意添加|减少项就会报错");

			if (desp.InitialActivity.BranchProcessTemplates.Count != 0)
				Assert.AreEqual(7, desp.InitialActivity.BranchProcessTemplates[0].Properties.Count, "分支流程在配置文件中的属性项。任意添加|减少项就会报错");

			Assert.AreEqual(6, desp.InitialActivity.ToTransitions[0].Properties.Count, "线在配置文件中的属性项。任意添加|减少项就会报错");


		}

	}
}
