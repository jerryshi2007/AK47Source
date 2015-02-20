using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.DescriptorTest
{
	[TestClass]
	public class WfActivityDescriptorTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.Descriptor)]
		public void ReplaceUserResourceTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateProcessDescriptorWithBranchTemplateAndUsers();

			IUser approver1 = (IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object;

			List<IUser> replaceUsers = new List<IUser>();

			IUser ceo = (IUser)OguObjectSettings.GetConfig().Objects["ceo"].Object;

			replaceUsers.Add((IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object);
			replaceUsers.Add(ceo);

			int affectedUsers = processDesp.ReplaceAllUserResourceDescriptors(approver1, replaceUsers);

			Console.WriteLine(affectedUsers);

			Assert.AreEqual(2, processDesp.InitialActivity.Resources.Count);
			Assert.IsTrue(ContainsUserResource(processDesp.InitialActivity.Resources, ceo));

			IWfActivityDescriptor normalActivity = processDesp.Activities["NormalActivity"];

			Assert.AreEqual(1, normalActivity.BranchProcessTemplates["Consign"].Resources.Count);
			Assert.IsTrue(ContainsUserResource(normalActivity.BranchProcessTemplates["Consign"].Resources, ceo));

			IUser approver2 = (IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object;

			affectedUsers = processDesp.ReplaceAllUserResourceDescriptors(approver2, new IUser[] { });

			Assert.AreEqual(0, normalActivity.BranchProcessTemplates["Distribute"].Resources.Count);
		}

		private static bool ContainsUserResource(WfResourceDescriptorCollection resources, IUser user)
		{
			bool result = false;

			foreach (WfResourceDescriptor resource in resources)
			{
				if (resource is WfUserResourceDescriptor)
				{
					result = (((WfUserResourceDescriptor)resource).IsSameUser(user));

					if (result)
						break;
				}
			}

			return result;
		}
	}
}
