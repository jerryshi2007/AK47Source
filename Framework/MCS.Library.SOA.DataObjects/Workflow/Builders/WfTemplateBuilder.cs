using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 分支流程模板构造器
	/// </summary>
	public static class WfTemplateBuilder
	{
		/// <summary>
		/// 自动启动的子流程的模板的Key
		/// </summary>
		public const string AutoStartSubProcessTemplateKey = "WfBranchProcessTemplateDescriptorKey";

		/// <summary>
		/// 构造缺省的会签模板
		/// </summary>
		/// <param name="key"></param>
		/// <param name="blockingType"></param>
		/// <returns></returns>
		public static IWfBranchProcessTemplateDescriptor CreateDefaultConsignTemplate(
			string key, 
			WfBranchProcessExecuteSequence execSequence,
			WfBranchProcessBlockingType blockingType,
			IEnumerable<IUser> users)
		{
			key.CheckStringIsNullOrEmpty("key");
			users.NullCheck("users");

			WfBranchProcessTemplateDescriptor template = new WfBranchProcessTemplateDescriptor(key);

			template.BranchProcessKey = WfProcessDescriptorManager.DefaultConsignProcessKey;
			template.ExecuteSequence = execSequence;
			template.BlockingType = blockingType;

			users = users.Distinct(new OguObjectIDEqualityComparer<IUser>());

			foreach (IUser user in users)
				template.Resources.Add(new WfUserResourceDescriptor(user));
			
			return template;
		}

		/// <summary>
		/// 创建一个缺省的审批流模板
		/// </summary>
		/// <param name="key"></param>
		/// <param name="processDespKey">流程描述的Key</param>
		/// <param name="execSequence"></param>
		/// <param name="blockingType"></param>
		/// <param name="users"></param>
		/// <returns></returns>
		public static IWfBranchProcessTemplateDescriptor CreateDefaultApprovalTemplate(
			string key,
			string processDespKey,
			WfBranchProcessExecuteSequence execSequence,
			WfBranchProcessBlockingType blockingType,
			IEnumerable<IUser> users)
		{
			key.CheckStringIsNullOrEmpty("key");
			users.NullCheck("users");

			WfBranchProcessTemplateDescriptor template = new WfBranchProcessTemplateDescriptor(key);

			template.BranchProcessKey = WfProcessDescriptorManager.DefaultApprovalProcessKey;
			template.ExecuteSequence = execSequence;
			template.BlockingType = blockingType;

			users = users.Distinct(new OguObjectIDEqualityComparer<IUser>());

			foreach (IUser user in users)
				template.Resources.Add(new WfUserResourceDescriptor(user));

			return template;
		}

		/// <summary>
		/// 构造缺省送阅流程模板
		/// </summary>
		/// <param name="key"></param>
		/// <param name="users"></param>
		/// <returns></returns>
		public static IWfBranchProcessTemplateDescriptor CreateDefaultCirculationTemplate(
			string key,
			IEnumerable<IUser> users)
		{
			key.CheckStringIsNullOrEmpty("key");
			users.NullCheck("users");

			WfBranchProcessTemplateDescriptor template = new WfBranchProcessTemplateDescriptor(key);

			template.BranchProcessKey = WfProcessDescriptorManager.DefaultCirculationProcessKey;
			template.ExecuteSequence = WfBranchProcessExecuteSequence.Parallel;
			template.BlockingType = WfBranchProcessBlockingType.WaitNoneOfBranchProcessComplete;

			users = users.Distinct(new OguObjectIDEqualityComparer<IUser>());

			foreach (IUser user in users)
				template.Resources.Add(new WfUserResourceDescriptor(user));

			return template;
		}
	}
}
