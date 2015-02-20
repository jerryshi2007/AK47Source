using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 秘书的修饰器
	/// </summary>
	public class WfSecretaryDecorator : IWfProcessDecorator
	{
		private static string[] _TemplateActSavedProperties = new string[] { "AllowAddApprover", "AllowAddConsignApprover", "AllowToBeAppended", "AllowToBeDeleted", "AllowCirculate", "AllowReturn", "AllowToBeReturned" };
		private static string[] _PrevActForbidProperties = new string[] { "AllowAddApprover", "AllowToBeModified", "AllowAddConsignApprover", "AllowToBeAppended", "AllowToBeDeleted", "AllowCirculate" };
		private static string[] _PrevActAllowProperties = new string[] { "AllowReturn" };
		private static string[] _SucceedActForbidProperties = new string[] { "AllowToBeAppended", "AllowToBeModified", "AllowToBeDeleted", "AllowCirculate", "AllowToBeReturned" };

		/// <summary>
		/// 添加秘书的结果
		/// </summary>
		private class AddSecretaryResult
		{
			public IWfActivity PrevActivity
			{
				get;
				set;
			}

			public IWfActivity SucceedActivity
			{
				get;
				set;
			}
		}

		#region IWfProcessDecorator Members

		public void Decorate(IWfProcess process)
		{
			//包含秘书，且需要生成秘书的活动
			List<WfSecretaryOperation> secretaryOperations = new List<WfSecretaryOperation>();

			process.Descriptor.Activities.ForEach(actDesp =>
			{
				secretaryOperations.Add(GetSecretaryOperation(actDesp));
			});

			List<IWfActivity> generatedActivities = ProcessSecretaryActivities(secretaryOperations);

			generatedActivities.ForEach(act => act.GenerateCandidatesFromResources());
		}

		#endregion
		/// <summary>
		/// 得到秘书操作的类型，包括：增加，修改，清除秘书的操作类
		/// </summary>
		/// <param name="actDesp"></param>
		/// <returns></returns>
		private static WfSecretaryOperation GetSecretaryOperation(IWfActivityDescriptor actDesp)
		{
			WfSecretaryOperation result = new WfSecretaryOperation();

			if (actDesp.Variables.GetValue(WfHelper.SecretaryTemplateActivity, false))
			{
				List<IUser> secretaries = GetSecretariesFromCandidates(actDesp.Instance.Candidates);

				result.Secretaries = secretaries;

				if (secretaries.Count > 0)
				{
					if (CompareOriginalSecretaries(actDesp, secretaries) == false)
						result.OperationType = WfSecretaryOperationType.ChangeSecretaries;
				}
				else
					result.OperationType = WfSecretaryOperationType.ClearSecretaries;
			}
			else if (CanGenerateSecretaries(actDesp))
			{
				List<IUser> secretaries = GetSecretariesFromCandidates(actDesp.Instance.Candidates);

				result.Secretaries = secretaries;

				if (secretaries.Count > 0)
					result.OperationType = WfSecretaryOperationType.AddSecretaries;
			}
			else
				result.Secretaries = new List<IUser>();

			result.ActivityDescriptor = actDesp;

			return result;
		}

		private static bool CanGenerateSecretaries(IWfActivityDescriptor actDesp)
		{
			//自动添加秘书，且不是模板活动
			bool result = actDesp.Properties.GetValue("AutoAppendSecretary", false) &&
				actDesp.Properties.GetValue("IsDynamic", false) == false;

			if (result)
			{
				result = actDesp.Variables.GetValue(WfHelper.SecretaryActivity, false) == false;

				if (result)
					result = actDesp.Variables.GetValue(WfHelper.SecretaryTemplateActivity, false) == false;
			}

			return result;
		}

		private static List<IWfActivity> ProcessSecretaryActivities(IEnumerable<WfSecretaryOperation> secretaryOperations)
		{
			List<IWfActivity> generatedActivities = new List<IWfActivity>();

			foreach (WfSecretaryOperation operation in secretaryOperations)
			{
				switch (operation.OperationType)
				{
					case WfSecretaryOperationType.AddSecretaries:
						AddSecretaryActivities(operation, generatedActivities);
						break;
					case WfSecretaryOperationType.ChangeSecretaries:
						ChangeSecretaryActivities(operation, generatedActivities);
						break;
					case WfSecretaryOperationType.ClearSecretaries:
						ClearSecretaryActivities(operation, generatedActivities);
						break;
				}
			}

			return generatedActivities;
		}

		private static void SetActivityPropertiesBooleanValue(IWfActivityDescriptor actDesp, bool value, string[] properties)
		{
			foreach (string propertyName in properties)
				actDesp.Properties.TrySetValue(propertyName, value);
		}

		#region Operations
		private static void AddSecretaryActivities(WfSecretaryOperation operation, List<IWfActivity> generatedActivities)
		{
			AddSecretaryResult instanceResult = AddInstanceSecretaryActivities(operation, generatedActivities);

			if (operation.ActivityDescriptor.Instance.MainStreamActivityKey.IsNotEmpty())
				AddMainStreamSecretaryActivities(operation, operation.ActivityDescriptor.Instance.GetMainStreamActivityDescriptor(), instanceResult);
		}

		/// <summary>
		/// 添加实例活动的秘书环节
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="generatedActivities"></param>
		private static AddSecretaryResult AddInstanceSecretaryActivities(WfSecretaryOperation operation, List<IWfActivity> generatedActivities)
		{
			AddSecretaryResult result = new AddSecretaryResult();

			//保留住原来领导活动的出线
			List<IWfTransitionDescriptor> templateToTransitions = new List<IWfTransitionDescriptor>(operation.ActivityDescriptor.ToTransitions);

			IWfActivityDescriptor prevSecretaryActDesp = CreateSecretaryActivityDescriptor(operation.ActivityDescriptor, operation.Secretaries);

			SetActivityPropertiesBooleanValue(prevSecretaryActDesp, false, WfSecretaryDecorator._PrevActForbidProperties);
			SetActivityPropertiesBooleanValue(prevSecretaryActDesp, true, WfSecretaryDecorator._PrevActAllowProperties);

			//在领导活动之前插入秘书活动
			result.PrevActivity = operation.ActivityDescriptor.Instance.InsertBefore(prevSecretaryActDesp);

			generatedActivities.Add(result.PrevActivity);

			DecroateTemplateActivity(operation.ActivityDescriptor, operation.Secretaries);

			IWfActivityDescriptor succeedSecretaryActDesp = CreateSecretaryActivityDescriptor(operation.ActivityDescriptor, operation.Secretaries);

			SetActivityPropertiesBooleanValue(succeedSecretaryActDesp, false, WfSecretaryDecorator._SucceedActForbidProperties);

			result.SucceedActivity = operation.ActivityDescriptor.Instance.Append(succeedSecretaryActDesp, true);

			generatedActivities.Add(result.SucceedActivity);

			AdjustTemplateActivityToTransitions(operation.ActivityDescriptor, succeedSecretaryActDesp, templateToTransitions);

			return result;
		}

		/// <summary>
		/// 添加主线活动的秘书环节
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="msOperationActDesp"></param>
		private static void AddMainStreamSecretaryActivities(WfSecretaryOperation operation, IWfActivityDescriptor msOperationActDesp, AddSecretaryResult instanceResult)
		{
			AddSecretaryResult result = new AddSecretaryResult();

			//保留住原来领导活动的出线
			List<IWfTransitionDescriptor> templateToTransitions = new List<IWfTransitionDescriptor>(msOperationActDesp.ToTransitions);

			IWfActivityDescriptor prevSecretaryActDesp = CreateSecretaryActivityDescriptor(msOperationActDesp, operation.Secretaries);
			SetActivityPropertiesBooleanValue(prevSecretaryActDesp, false, WfSecretaryDecorator._PrevActForbidProperties);
			SetActivityPropertiesBooleanValue(prevSecretaryActDesp, true, WfSecretaryDecorator._PrevActAllowProperties);
			msOperationActDesp.InsertBefore(prevSecretaryActDesp);

			DecroateTemplateActivity(msOperationActDesp, operation.Secretaries);

			IWfActivityDescriptor succeedSecretaryActDesp = CreateSecretaryActivityDescriptor(msOperationActDesp, operation.Secretaries);

			SetActivityPropertiesBooleanValue(succeedSecretaryActDesp, false, WfSecretaryDecorator._SucceedActForbidProperties);
			msOperationActDesp.Append(succeedSecretaryActDesp, true);

			AdjustTemplateActivityToTransitions(msOperationActDesp, succeedSecretaryActDesp, templateToTransitions);

			((WfActivityBase)instanceResult.PrevActivity).MainStreamActivityKey = prevSecretaryActDesp.Key;
			((WfActivityBase)instanceResult.SucceedActivity).MainStreamActivityKey = succeedSecretaryActDesp.Key;
		}

		private static void ChangeSecretaryActivities(WfSecretaryOperation operation, List<IWfActivity> generatedActivities)
		{
			ChangeInstanceSecretaryActivities(operation, generatedActivities);

			if (operation.ActivityDescriptor.Instance.MainStreamActivityKey.IsNotEmpty())
				ChangeMainStreamSecretaryActivities(operation, operation.ActivityDescriptor.Instance.GetMainStreamActivityDescriptor());
		}

		private static void ChangeInstanceSecretaryActivities(WfSecretaryOperation operation, List<IWfActivity> generatedActivities)
		{
			IWfTransitionDescriptor prevTransition = operation.ActivityDescriptor.FromTransitions.Find(t => t.FromActivity.Variables.GetValue(WfHelper.SecretaryActivity, false));
			IWfTransitionDescriptor succeedTransition = operation.ActivityDescriptor.ToTransitions.Find(t => t.ToActivity.Variables.GetValue(WfHelper.SecretaryActivity, false));

			if (prevTransition != null)
			{
				ChangeActivityResources(prevTransition.FromActivity, operation.Secretaries);
				generatedActivities.Add(prevTransition.FromActivity.Instance);
			}

			if (succeedTransition != null)
			{
				ChangeActivityResources(succeedTransition.ToActivity, operation.Secretaries);
				generatedActivities.Add(succeedTransition.ToActivity.Instance);
			}

			DecroateTemplateActivity(operation.ActivityDescriptor, operation.Secretaries);
		}

		private static void ChangeMainStreamSecretaryActivities(WfSecretaryOperation operation, IWfActivityDescriptor msOperationActDesp)
		{
			IWfTransitionDescriptor prevTransition = msOperationActDesp.FromTransitions.Find(t => t.FromActivity.Variables.GetValue(WfHelper.SecretaryActivity, false));
			IWfTransitionDescriptor succeedTransition = msOperationActDesp.ToTransitions.Find(t => t.ToActivity.Variables.GetValue(WfHelper.SecretaryActivity, false));

			if (prevTransition != null)
			{
				ChangeActivityResources(prevTransition.FromActivity, operation.Secretaries);
			}

			if (succeedTransition != null)
			{
				ChangeActivityResources(succeedTransition.ToActivity, operation.Secretaries);
			}

			DecroateTemplateActivity(operation.ActivityDescriptor, operation.Secretaries);
		}

		/// <summary>
		/// 删除秘书活动
		/// </summary>
		/// <param name="operation"></param>
		/// <param name="generatedActivities"></param>
		private static void ClearSecretaryActivities(WfSecretaryOperation operation, List<IWfActivity> generatedActivities)
		{
			ClearInstanceSecretaryActivities(operation, generatedActivities);

			if (operation.ActivityDescriptor.Instance.MainStreamActivityKey.IsNotEmpty())
				ClearMainStreamSecretaryActivities(operation, operation.ActivityDescriptor.Instance.GetMainStreamActivityDescriptor());
		}

		private static void ClearInstanceSecretaryActivities(WfSecretaryOperation operation, List<IWfActivity> generatedActivities)
		{
			IWfTransitionDescriptor prevTransition = operation.ActivityDescriptor.FromTransitions.Find(t => t.FromActivity.Variables.GetValue(WfHelper.SecretaryActivity, false));
			IWfTransitionDescriptor succeedTransition = operation.ActivityDescriptor.ToTransitions.Find(t => t.ToActivity.Variables.GetValue(WfHelper.SecretaryActivity, false));

			bool needRemoveInitial = false;

			if (prevTransition != null)
			{
				if (prevTransition.FromActivity.ActivityType == WfActivityType.InitialActivity)
					needRemoveInitial = true;

				List<IWfTransitionDescriptor> fromTransitions = new List<IWfTransitionDescriptor>();
				prevTransition.FromActivity.FromTransitions.CopyTo(fromTransitions);
				((WfActivityBase)prevTransition.FromActivity.Instance).InternalRemove();

				foreach (IWfTransitionDescriptor fromTransition in fromTransitions)
					fromTransition.ConnectActivities(fromTransition.FromActivity, operation.ActivityDescriptor);
			}

			if (succeedTransition != null)
			{
				List<IWfTransitionDescriptor> toTransitions = new List<IWfTransitionDescriptor>();

				succeedTransition.ToActivity.ToTransitions.CopyTo(toTransitions);
				succeedTransition.ToActivity.Instance.Remove();

				foreach (IWfTransitionDescriptor toTransition in toTransitions)
					toTransition.ConnectActivities(operation.ActivityDescriptor, toTransition.ToActivity);
			}

			if (needRemoveInitial)
			{
				((WfActivityDescriptor)operation.ActivityDescriptor).ActivityType = WfActivityType.InitialActivity;
				operation.ActivityDescriptor.Process.Activities.InitialActivity = operation.ActivityDescriptor;
				((WfProcess)operation.ActivityDescriptor.Instance.Process).InitialActivity = operation.ActivityDescriptor.Instance;
			}

			DedecroateTemplateActivity(operation.ActivityDescriptor, operation.Secretaries);
		}

		private static void ClearMainStreamSecretaryActivities(WfSecretaryOperation operation, IWfActivityDescriptor msOperationActDesp)
		{
			IWfTransitionDescriptor prevTransition = msOperationActDesp.FromTransitions.Find(t => t.FromActivity.Variables.GetValue(WfHelper.SecretaryActivity, false));
			IWfTransitionDescriptor succeedTransition = msOperationActDesp.ToTransitions.Find(t => t.ToActivity.Variables.GetValue(WfHelper.SecretaryActivity, false));

			bool needRemoveInitial = false;

			if (prevTransition != null)
			{
				if (prevTransition.FromActivity.ActivityType == WfActivityType.InitialActivity)
					needRemoveInitial = true;

				List<IWfTransitionDescriptor> fromTransitions = new List<IWfTransitionDescriptor>();
				prevTransition.FromActivity.FromTransitions.CopyTo(fromTransitions);
				prevTransition.FromActivity.Remove();

				foreach (IWfTransitionDescriptor fromTransition in fromTransitions)
					fromTransition.ConnectActivities(fromTransition.FromActivity, msOperationActDesp);
			}

			if (succeedTransition != null)
			{
				List<IWfTransitionDescriptor> toTransitions = new List<IWfTransitionDescriptor>();

				succeedTransition.ToActivity.ToTransitions.CopyTo(toTransitions);
				succeedTransition.ToActivity.Remove();

				foreach (IWfTransitionDescriptor toTransition in toTransitions)
					toTransition.ConnectActivities(msOperationActDesp, toTransition.ToActivity);
			}

			if (needRemoveInitial)
			{
				((WfActivityDescriptor)msOperationActDesp).ActivityType = WfActivityType.InitialActivity;
				msOperationActDesp.Process.Activities.InitialActivity = msOperationActDesp;
			}

			DedecroateTemplateActivity(msOperationActDesp, operation.Secretaries);
		}
		#endregion Operations

		private static IWfActivityDescriptor CreateSecretaryActivityDescriptor(IWfActivityDescriptor templateDescriptor, IEnumerable<IUser> secretaries)
		{
			WfActivityDescriptor actDesp = new WfActivityDescriptor(templateDescriptor.Process.FindNotUsedActivityKey(), WfActivityType.NormalActivity);

			actDesp.Name = "秘书";

			ChangeActivityResources(actDesp, secretaries);

			actDesp.Variables.SetValue(WfHelper.SecretaryActivity, "True", DataType.Boolean);
			actDesp.Variables.SetValue(WfHelper.ActivityGroupName, templateDescriptor.Key);

			if (templateDescriptor.TemplateKey.IsNotEmpty())
				actDesp.SetDynamicActivityProperties(templateDescriptor.TemplateKey);

			return actDesp;
		}

		private static void ChangeActivityResources(IWfActivityDescriptor actDesp, IEnumerable<IUser> users)
		{
			actDesp.Resources.Clear();

			users.ForEach(u => actDesp.Resources.Add(new WfUserResourceDescriptor(u)));
		}

		private static List<IUser> GetSecretariesFromCandidates(WfAssigneeCollection candidates)
		{
			List<IUser> result = new List<IUser>();

			foreach (WfAssignee assignee in candidates)
			{
				if (assignee.User != null)
				{
					foreach (IUser secretary in assignee.User.Secretaries)
					{
						if (result.Exists(u => string.Compare(u.ID, secretary.ID, true) == 0) == false)
							result.Add(secretary);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 修饰需要生成秘书的的活动，添加必要的属性
		/// </summary>
		/// <param name="templateDescriptor"></param>
		private static void DecroateTemplateActivity(IWfActivityDescriptor templateDescriptor, IEnumerable<IUser> secretaries)
		{
			templateDescriptor.Variables.SetValue(WfHelper.SecretaryTemplateActivity, "True", DataType.Boolean);
			templateDescriptor.Variables.SetValue(WfHelper.ActivityGroupName, templateDescriptor.Key);

			string secretariesIDs = GetUsersIDs(secretaries);

			templateDescriptor.Variables.SetValue(WfHelper.SecretaryTemplateSecretaries, secretariesIDs);

			foreach (string propertyName in WfSecretaryDecorator._TemplateActSavedProperties)
			{
				templateDescriptor.Variables.SetValue(propertyName,
					templateDescriptor.Properties.GetValue(propertyName, false).ToString(),
					DataType.Boolean);

				templateDescriptor.Properties.SetValue(propertyName, false);
			}
		}

		/// <summary>
		/// 消除需要秘书模版的活动上的装饰。
		/// </summary>
		/// <param name="templateDescriptor"></param>
		/// <param name="secretaries"></param>
		private static void DedecroateTemplateActivity(IWfActivityDescriptor templateDescriptor, IEnumerable<IUser> secretaries)
		{
			templateDescriptor.Variables.SetValue(WfHelper.SecretaryTemplateActivity, "False", DataType.Boolean);
			templateDescriptor.Variables.SetValue(WfHelper.SecretaryTemplateSecretaries, string.Empty);
			templateDescriptor.Variables.SetValue(WfHelper.ActivityGroupName, string.Empty);

			foreach (string propertyName in _TemplateActSavedProperties)
			{
				templateDescriptor.Properties.SetValue(propertyName, templateDescriptor.Variables.GetValue(propertyName, false));
			}
		}

		private static bool CompareOriginalSecretaries(IWfActivityDescriptor templateDescriptor, IEnumerable<IUser> secretaries)
		{
			bool result = true;

			string[] originalIDs = templateDescriptor.Variables.GetValue(WfHelper.SecretaryTemplateSecretaries, string.Empty).Split(',');

			result = originalIDs.Length == secretaries.Count();

			if (result)
			{
				foreach (IUser user in secretaries)
				{
					if (originalIDs.Exists(id => string.Compare(user.ID, id, true) == 0) == false)
					{
						result = false;
						break;
					}
				}
			}

			return result;
		}

		private static string GetUsersIDs(IEnumerable<IUser> users)
		{
			StringBuilder strB = new StringBuilder();

			foreach (IUser user in users)
			{
				if (strB.Length > 0)
					strB.Append(",");

				strB.Append(user.ID);
			}

			return strB.ToString();
		}

		private static void AdjustTemplateActivityToTransitions(IWfActivityDescriptor templateActDesp,
			IWfActivityDescriptor secondSecretaryActDesp,
			IEnumerable<IWfTransitionDescriptor> originalToTransitions)
		{
			templateActDesp.ToTransitions.Clear();

			foreach (IWfTransitionDescriptor transition in originalToTransitions)
			{
				WfTransitionDescriptor clonedTransition = (WfTransitionDescriptor)((WfTransitionDescriptor)transition).Clone();

				clonedTransition.Key = templateActDesp.Process.FindNotUsedTransitionKey();
				((WfTransitionDescriptor)clonedTransition).ConnectActivities(templateActDesp, secondSecretaryActDesp);
			}
		}
	}
}
