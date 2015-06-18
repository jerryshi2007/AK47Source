﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Test.Executor;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow.Builders;
using MCS.Web.Library.Script;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.WfCreateParams
{
    /// <summary>
    /// 已经迁移到MCS.Library.SOA.DataObjects.Tenant.Test
    /// </summary>
	[TestClass]
	public class WfCreateTransitionParamTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void WfTransitionJsonToCreateTransitionParams()
		{
			WfForwardTransitionDescriptor transition1 = new WfForwardTransitionDescriptor("T1");
			transition1.Name = "Transition1";
			transition1.Condition.Expression = "Amount > 4000";
			transition1.Variables.Add(new WfVariableDescriptor("V1", "Shen Zheng"));
			transition1.Variables.Add(new WfVariableDescriptor("Level", "10", DataType.Int));

			WfForwardTransitionDescriptor transition2 = new WfForwardTransitionDescriptor("T2");
			transition2.Name = "Transition2";

			JSONSerializerExecute.RegisterConverter(typeof(EasyWfForwardTransitionDescriptorConverter));
			JSONSerializerExecute.RegisterConverter(typeof(WfConditionDescriptorConverter));
			JSONSerializerExecute.RegisterConverter(typeof(EasyWfVariableDescriptorConverter));

			string json = JSONSerializerExecute.Serialize(new WfForwardTransitionDescriptor[] { transition1, transition2 });

			Console.WriteLine(json);

			WfCreateTransitionParamCollection transitionParams = new WfCreateTransitionParamCollection(json);

			Assert.AreEqual(2, transitionParams.Count);

			Assert.AreEqual(transition1.Key, transitionParams[0].Parameters["Key"]);
			Assert.AreEqual(transition1.Name, transitionParams[0].Parameters["Name"]);
			Assert.AreEqual(transition1.Condition.Expression, transitionParams[0].Parameters["Condition"]);

			Assert.AreEqual(transition2.Key, transitionParams[1].Parameters["Key"]);
			Assert.AreEqual(transition2.Name, transitionParams[1].Parameters["Name"]);
			Assert.IsFalse(transitionParams[1].Parameters.ContainsKey("Condition"));

			Assert.AreEqual(transition1.Variables.Count, ((Dictionary<string, object>[])transitionParams[0].Parameters["Variables"]).Length);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void JsonToCreateTransitionParams()
		{
			Dictionary<string, object> jsonTemplate = new Dictionary<string, object>();

			jsonTemplate["Key"] = "T1";
			jsonTemplate["Name"] = "Transition1";
			jsonTemplate["Condition"] = "Amount > 4000";
			jsonTemplate["DefaultSelect"] = true;

			Dictionary<string, object>[] variableDicts = new Dictionary<string, object>[1];

			variableDicts[0] = new Dictionary<string, object>();

			variableDicts[0]["Key"] = "V1";
			variableDicts[0]["OriginalType"] = DataType.Int;
			variableDicts[0]["OriginalValue"] = 10;

			jsonTemplate["Variables"] = variableDicts;

			string json = JSONSerializerExecute.Serialize(new Dictionary<string, object>[] { jsonTemplate });

			Console.WriteLine(json);

			WfCreateTransitionParamCollection transitionParams = new WfCreateTransitionParamCollection(json);

			Assert.AreEqual(1, transitionParams.Count);
			Assert.AreEqual(jsonTemplate["Key"], transitionParams[0].Parameters["Key"]);
			Assert.AreEqual(jsonTemplate["Name"], transitionParams[0].Parameters["Name"]);
			Assert.AreEqual(jsonTemplate["Condition"], transitionParams[0].Parameters["Condition"]);

			Assert.AreEqual(((Dictionary<string, object>[])jsonTemplate["Variables"]).Length,
				((object[])transitionParams[0].Parameters["Variables"]).Length);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void CreateTransitionByTransitionParamsTest()
		{
			WfCreateTransitionParamCollection transitionParams = new WfCreateTransitionParamCollection();

			WfCreateTransitionParam tp = new WfCreateTransitionParam();

			tp.Parameters["Key"] = "T1";
			tp.Parameters["Name"] = "Transition1";
			tp.Parameters["Condition"] = "Amount > 4000";

			Dictionary<string, object>[] variableDicts = new Dictionary<string, object>[1];

			variableDicts[0] = new Dictionary<string, object>();

			variableDicts[0]["Key"] = "V1";
			variableDicts[0]["OriginalType"] = DataType.Int;
			variableDicts[0]["OriginalValue"] = 10;

			tp.Parameters["Variables"] = variableDicts;

			IWfTransitionDescriptor transition = tp.CreateTransition();

			Assert.AreEqual(tp.Parameters["Key"], transition.Key);
			Assert.AreEqual(tp.Parameters["Name"], transition.Name);
			Assert.AreEqual(tp.Parameters["Condition"], transition.Condition.Expression);
			Assert.AreEqual(10, transition.Variables.GetValue("V1", 0));
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void CreateTransitionAndConnectActivitiesTest()
		{
			//创建一个三个活动的流程
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];

			normalActDesp.ToTransitions.RemoveByToActivity(processDesp.CompletedActivity);

			WfCreateTransitionParamCollection transitionParams = new WfCreateTransitionParamCollection();

			WfCreateTransitionParam tp = new WfCreateTransitionParam();

			tp.Parameters["Key"] = "T1";
			tp.Parameters["Name"] = "Transition1";

			tp.CreateTransitionAndConnectActivities(normalActDesp, processDesp.CompletedActivity);

			processDesp.Output();

			Assert.AreEqual(tp.Parameters["Name"], normalActDesp.ToTransitions.FirstOrDefault().Name);
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		[Description("测试内置函数FirstActivity的功能")]
		public void FirstActivityDescriptionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateInitAndCompletedProcessDescriptor();

			WfCreateActivityParamCollection caps = PrepareTwoActivitiesCreateParams();

			caps.CreateActivities(processDesp, false);

			IWfActivityDescriptor matchedActDesp = caps.FindActivityByActivityDescription(processDesp, null, "FirstActivity");

			Assert.AreEqual(processDesp.InitialActivity.Key, matchedActDesp.Key);
		}

		[TestMethod]
		[Description("测试内置函数LastActivity的功能")]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void LastActivityDescriptionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateInitAndCompletedProcessDescriptor();

			WfCreateActivityParamCollection caps = PrepareTwoActivitiesCreateParams();

			caps.CreateActivities(processDesp, false);

			IWfActivityDescriptor matchedActDesp = caps.FindActivityByActivityDescription(processDesp, null, "LastActivity");

			Assert.AreEqual(processDesp.CompletedActivity.Key, matchedActDesp.Key);
		}

		[TestMethod]
		[Description("测试内置函数Key的功能")]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void SNActivityDescriptionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateInitAndCompletedProcessDescriptor();

			WfCreateActivityParamCollection caps = PrepareTwoActivitiesCreateParams();

			caps.CreateActivities(processDesp, false);

			IWfActivityDescriptor matchedActDesp = caps.FindActivityByActivityDescription(processDesp, null, "SN(10)");

			Assert.AreEqual(processDesp.InitialActivity.ToTransitions.FirstOrDefault().ToActivity.Key, matchedActDesp.Key);
		}

		[TestMethod]
		[Description("测试内置函数SN的扩展功能，直接使用整数")]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void IntSNActivityDescriptionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateInitAndCompletedProcessDescriptor();

			WfCreateActivityParamCollection caps = PrepareTwoActivitiesCreateParams();

			caps.CreateActivities(processDesp, false);

			IWfActivityDescriptor matchedActDesp = caps.FindActivityByActivityDescription(processDesp, null, "10");

			Assert.AreEqual(processDesp.InitialActivity.ToTransitions.FirstOrDefault().ToActivity.Key, matchedActDesp.Key);
		}

		[TestMethod]
		[Description("非法内置函数的异常测试")]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		[ExpectedException(typeof(InvalidOperationException))]
		public void InvalidActivityDescriptionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateInitAndCompletedProcessDescriptor();

			WfCreateActivityParamCollection caps = PrepareTwoActivitiesCreateParams();

			caps.CreateActivities(processDesp, false);

			string targetKey = caps[0].CreatedDescriptor.Key;

			caps.FindActivityByActivityDescription(processDesp, null, string.Format("Invalid(\"{0}\")", targetKey));
		}

		[TestMethod]
		[Description("测试内置函数SN的功能")]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void KeyActivityDescriptionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateInitAndCompletedProcessDescriptor();

			WfCreateActivityParamCollection caps = PrepareTwoActivitiesCreateParams();

			caps.CreateActivities(processDesp, false);

			string targetKey = caps[0].CreatedDescriptor.Key;

			IWfActivityDescriptor matchedActDesp = caps.FindActivityByActivityDescription(processDesp, null, string.Format("Key(\"{0}\")", targetKey));

			Assert.AreEqual(processDesp.InitialActivity.ToTransitions.FirstOrDefault().ToActivity.Key, matchedActDesp.Key);
		}

		[TestMethod]
		[Description("测试内置函数SN的功能")]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void DirectKeyActivityDescriptionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateInitAndCompletedProcessDescriptor();

			WfCreateActivityParamCollection caps = PrepareTwoActivitiesCreateParams();

			caps.CreateActivities(processDesp, false);

			string targetKey = caps[0].CreatedDescriptor.Key;

			IWfActivityDescriptor matchedActDesp = caps.FindActivityByActivityDescription(processDesp, null, string.Format("{0}", targetKey));

			Assert.AreEqual(processDesp.InitialActivity.ToTransitions.FirstOrDefault().ToActivity.Key, matchedActDesp.Key);
		}

		[TestMethod]
		[Description("测试内置函数CodeName的功能")]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void CodeNameActivityDescriptionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateInitAndCompletedProcessDescriptor();

			WfCreateActivityParamCollection caps = PrepareTwoActivitiesCreateParams();

			caps.CreateActivities(processDesp, false);

			IWfActivityDescriptor matchedActDesp = caps.FindActivityByActivityDescription(processDesp, null, "CodeName(\"TestCodeName\")");

			Assert.AreEqual(processDesp.Activities.Find(actDesp => actDesp.CodeName == "TestCodeName").Key, matchedActDesp.Key);
		}

		[TestMethod]
		[Description("测试内置函数DefaultNextActivity的功能")]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void DefaultNextActivityDescriptionTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateInitAndCompletedProcessDescriptor();

			WfCreateActivityParamCollection caps = PrepareTwoActivitiesCreateParams();

			caps.CreateActivities(processDesp, false);

			IWfActivityDescriptor matchedActDesp = caps.FindActivityByActivityDescription(processDesp, caps[0],
				WfCreateActivityParam.DefaultNextActivityDescription);

			Assert.AreEqual(caps[0].DefaultNextDescriptor.Key, matchedActDesp.Key);
		}

		[TestMethod]
		[Description("测试创建的第一个动态活动中连线的测试")]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void FirstCapTransitionsTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateInitAndCompletedProcessDescriptor();

			WfCreateActivityParamCollection caps = PrepareTwoActivitiesWithTransitionsCreateParams();

			caps.CreateActivities(processDesp, false);

			processDesp.Output();

			ValidateFirstCapTwoTransitionsProperties(caps[0]);
		}

		[TestMethod]
		[Description("测试创建的第二个动态活动中连线的测试")]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void SecondCapTransitionsTest()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateInitAndCompletedProcessDescriptor();

			WfCreateActivityParamCollection caps = PrepareTwoActivitiesWithTransitionsCreateParams();

			caps.CreateActivities(processDesp, false);

			processDesp.Output();

			ValidateSecondCapTwoTransitionsProperties(caps[1]);
		}

		[TestMethod]
		[Description("测试创建的动态活动中连线的测试，创建两个动态活动点")]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void DynamicCapTransitionsTest()
		{
			IWfProcessDescriptor processDesp = PrepareDynamicProcessDesp();

			int originalActivityCount = processDesp.Activities.Count;

			WfCreateActivityParamCollection caps = PrepareTwoActivitiesWithTransitionsCreateParams();

			IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];

			normalActDesp.GenerateDynamicActivities(caps);

			Assert.AreEqual(originalActivityCount + caps.Count, processDesp.Activities.Count);
			ValidateFirstCapTwoTransitionsProperties(caps[0]);
			ValidateSecondCapTwoTransitionsProperties(caps[1]);
		}

		[TestMethod]
		[Description("测试创建的动态活动中连线的测试，创建一个动态活动点")]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void DynamicOneCapTransitionsTest()
		{
			IWfProcessDescriptor processDesp = PrepareDynamicProcessDesp();

			int originalActivityCount = processDesp.Activities.Count;

			WfCreateActivityParamCollection caps = PrepareOneActivityWithTransitionsCreateParams();

			IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];

			normalActDesp.GenerateDynamicActivities(caps);

			Assert.AreEqual(originalActivityCount + caps.Count, processDesp.Activities.Count);
			ValidateFirstCapTwoTransitionsProperties(caps[0]);
		}

		[TestMethod]
		[Description("测试创建简单流程活动")]
		[TestCategory(ProcessTestHelper.CreateActivityParams)]
		public void SimpleCreateProcessParamTest()
		{
			WfCreateProcessParam cpp = new WfCreateProcessParam();

			cpp.Key = "SimpleCreateProcessParam";
			cpp.Name = "SimpleCreateProcessParam";

			cpp.ActivityTemplates.CopyFrom(PrepareTwoActivitiesCreateParams());

			IWfProcessDescriptor processDesp = cpp.CreateProcess(false);

			processDesp.Output();

			Assert.AreEqual(cpp.Key, processDesp.Key);
			Assert.AreEqual(cpp.Name, processDesp.Name);
			Assert.AreEqual(cpp.ActivityTemplates.Count + 2, processDesp.Activities.Count);
		}

		/// <summary>
		/// 准备两个活动的创建参数，没有线参数
		/// </summary>
		/// <returns></returns>
		private static WfCreateActivityParamCollection PrepareTwoActivitiesCreateParams()
		{
			WfCreateActivityParamCollection result = new WfCreateActivityParamCollection();

			WfCreateActivityParam cap1 = new WfCreateActivityParam();

			cap1.ActivitySN = 10;

			result.Add(cap1);

			WfCreateActivityParam cap2 = new WfCreateActivityParam();

			cap2.ActivitySN = 20;

			cap2.Template.CodeName = "TestCodeName";

			result.Add(cap2);

			return result;
		}

		/// <summary>
		/// 准备带线的两个活动的创建参数
		/// </summary>
		/// <returns></returns>
		private static WfCreateActivityParamCollection PrepareTwoActivitiesWithTransitionsCreateParams()
		{
			WfCreateActivityParamCollection result = new WfCreateActivityParamCollection();

			WfCreateActivityParam cap1 = new WfCreateActivityParam();

			cap1.ActivitySN = 10;
			PrepareFirstCapTwoTransitions(cap1);

			result.Add(cap1);

			WfCreateActivityParam cap2 = new WfCreateActivityParam();

			cap2.ActivitySN = 20;

			cap2.Template.CodeName = "TestCodeName";

			PrepareSecondCapTwoTransitions(cap2);

			result.Add(cap2);

			return result;
		}

		/// <summary>
		/// 准备带线的一个活动的创建参数，一条退回到首节点，一条继续往下（默认选择）
		/// </summary>
		/// <returns></returns>
		private static WfCreateActivityParamCollection PrepareOneActivityWithTransitionsCreateParams()
		{
			WfCreateActivityParamCollection result = new WfCreateActivityParamCollection();

			WfCreateActivityParam cap1 = new WfCreateActivityParam();

			cap1.ActivitySN = 10;
			PrepareFirstCapTwoTransitions(cap1);

			result.Add(cap1);

			return result;
		}

		/// <summary>
		/// 准备两条出线，第一条是默认前进线，第二条是退回到首节点
		/// </summary>
		/// <param name="cap"></param>
		/// <param name="processDesp"></param>
		private static void PrepareFirstCapTwoTransitions(WfCreateActivityParam cap)
		{
			WfCreateTransitionParam transition1 = new WfCreateTransitionParam();

			transition1.Parameters["DefaultSelect"] = true;

			WfCreateTransitionParam transition2 = new WfCreateTransitionParam();

			transition2.Parameters["DefaultSelect"] = false;
			transition2.Parameters["ToActivityKey"] = "FirstActivity";
			transition2.Parameters["IsReturn"] = true;

			cap.TransitionTemplates.Add(transition1);
			cap.TransitionTemplates.Add(transition2);
		}

		private static void PrepareSecondCapTwoTransitions(WfCreateActivityParam cap)
		{
			WfCreateTransitionParam transition1 = new WfCreateTransitionParam();

			transition1.Parameters["DefaultSelect"] = true;

			WfCreateTransitionParam transition2 = new WfCreateTransitionParam();

			transition2.Parameters["DefaultSelect"] = false;
			transition2.Parameters["ToActivityKey"] = "FirstActivity";
			transition2.Parameters["IsReturn"] = true;

			cap.TransitionTemplates.Add(transition1);
			cap.TransitionTemplates.Add(transition2);
		}

		private static IWfProcessDescriptor PrepareDynamicProcessDesp()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			IWfActivityDescriptor normalActDesp = processDesp.Activities["NormalActivity"];

			normalActDesp.Properties.SetValue("IsDynamic", true);

			return processDesp;
		}

		private static void ValidateFirstCapTwoTransitionsProperties(WfCreateActivityParam cap)
		{
			Assert.AreEqual(cap.TransitionTemplates.Count, cap.CreatedDescriptor.ToTransitions.Count);
			Assert.AreEqual(cap.CreatedDescriptor.Process.InitialActivity.Key, cap.CreatedDescriptor.ToTransitions[1].ToActivity.Key);
			Assert.IsTrue(cap.CreatedDescriptor.ToTransitions[1].IsBackward);
			Assert.IsFalse(cap.CreatedDescriptor.ToTransitions[1].DefaultSelect);
		}

		private static void ValidateSecondCapTwoTransitionsProperties(WfCreateActivityParam cap)
		{
			Assert.AreEqual(cap.TransitionTemplates.Count, cap.CreatedDescriptor.ToTransitions.Count);
			Assert.AreEqual(cap.CreatedDescriptor.Process.InitialActivity.Key, cap.CreatedDescriptor.ToTransitions[1].ToActivity.Key);
			Assert.IsTrue(cap.CreatedDescriptor.ToTransitions[1].IsBackward);
			Assert.IsFalse(cap.CreatedDescriptor.ToTransitions[1].DefaultSelect);
		}
	}
}
