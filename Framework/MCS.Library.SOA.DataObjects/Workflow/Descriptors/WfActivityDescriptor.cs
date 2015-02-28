using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow.Builders;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 流程活动定义的描述
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public partial class WfActivityDescriptor : WfKeyedDescriptorBase, IWfActivityDescriptor, ISimpleXmlSerializer
	{
		private static readonly string[] ReservedVariableNames = new string[] { "IsReturnSkipped", "GeneratedByTemplate", "TemplateKey",
			WfHelper.ActivityGroupName, WfProcessBuilderBase.AutoBuiltActivityVariableName, WfHelper.SecretaryTemplateActivity, WfHelper.SecretaryActivity };

		private IWfProcessDescriptor _Process = null;

		private FromTransitionsDescriptorCollection _FromTransitions = null;

		private ToTransitionsDescriptorCollection _ToTransitions = null;

		private WfVariableDescriptorCollection _Variables = null;

		private WfResourceDescriptorCollection _Resources = null;
		private WfConditionDescriptor _Condition = null;

		private WfBranchProcessTemplateCollection _BranchProcessTemplates = null;

		private WfRelativeLinkDescriptorCollection _RelativeLinks = null;

		private WfResourceDescriptorCollection _EnterEventReceivers = null;

		private WfResourceDescriptorCollection _LeaveEventReceivers = null;

		private WfResourceDescriptorCollection _InternalRelativeUsers = null;

		private WfExternalUserCollection _ExternalUsers = null;

		private WfServiceOperationDefinitionCollection _EnterEventExecServices;

		private WfServiceOperationDefinitionCollection _LeaveEventExecServices;

		[XElementFieldSerialize(IgnoreDeserializeError = true)]
		private WfParameterNeedToBeCollected _ParametersNeedToBeCollected = null;

		[NonSerialized]
		private IWfActivity _Instance = null;

		public WfActivityDescriptor()
		{
		}

		public WfActivityDescriptor(string key, WfActivityType activityType)
		{
			ActivityType = activityType;
			//方法中用到activitytype
			InitProperties();
			Key = key;
		}

		internal WfActivityDescriptor(string key)
			: base(key)
		{
		}

		#region IWfActivityDescriptor Members
		public string CodeName
		{
			get { return Properties.GetValue("CodeName", string.Empty); }
			set { Properties.SetValue("CodeName", value); }
		}

		public string Url
		{
			get { return Properties.GetValue("Url", string.Empty); }
			set { Properties.SetValue("Url", value); }
		}

		public string LevelName
		{
			get { return Properties.GetValue("LevelName", string.Empty); }
			set { Properties.SetValue("LevelName", value); }
		}

		public string Scene
		{
			get { return Properties.GetValue("Scene", string.Empty); }
			set { Properties.SetValue("Scene", value); }
		}

		public string ReadOnlyScene
		{
			get { return Properties.GetValue("ReadOnlyScene", string.Empty); }
			set { Properties.SetValue("ReadOnlyScene", value); }
		}

		public string InheritedScene
		{
			get
			{
				string result = Scene;

				IWfProcess process = this.ProcessInstance;

				while (process != null && result.IsNullOrEmpty() &&
					process.Descriptor.Properties.GetValue("ProbeParentProcessParams", false) && process.HasParentProcess)
				{
					IWfProcess parentProcess = process.EntryInfo.OwnerActivity.Process;

					if (string.Compare(process.ResourceID, parentProcess.ResourceID, true) == 0)
						result = process.EntryInfo.OwnerActivity.Descriptor.Scene;
					else
						break;

					process = parentProcess;
				}

				return result;
			}
		}

		public string InheritedReadOnlyScene
		{
			get
			{
				string result = ReadOnlyScene;

				IWfProcess process = this.ProcessInstance;

				while (process != null && result.IsNullOrEmpty() &&
					process.Descriptor.Properties.GetValue("ProbeParentProcessParams", false) && process.HasParentProcess)
				{
					IWfProcess parentProcess = process.EntryInfo.OwnerActivity.Process;

					if (string.Compare(process.ResourceID, parentProcess.ResourceID, true) == 0)
						result = process.EntryInfo.OwnerActivity.Descriptor.ReadOnlyScene;
					else
						break;

					process = parentProcess;
				}

				return result;
			}
		}

		public DateTime EstimateStartTime
		{
			get { return Properties.GetValue("EstimateStartTime", DateTime.MinValue); }
			set { Properties.SetValue("EstimateStartTime", value); }
		}

		public DateTime EstimateEndTime
		{
			get { return Properties.GetValue("EstimateEndTime", DateTime.MinValue); }
			set { Properties.SetValue("EstimateEndTime", value); }
		}

		public Decimal EstimateDuration
		{
			get { return Properties.GetValue("EstimateDuration", (Decimal)0); }
			set { Properties.SetValue("EstimateDuration", value); }
		}

		[XElementFieldSerialize(AlternateFieldName = "_ActivityType")]
		public WfActivityType ActivityType
		{
			get;
			set;
		}

		public string TaskTitle
		{
			get { return Properties.GetValue("TaskTitle", string.Empty); }
			set { Properties.SetValue("TaskTitle", value); }
		}

		public string NotifyTaskTitle
		{
			get { return Properties.GetValue("NotifyTaskTitle", string.Empty); }
			set { Properties.SetValue("NotifyTaskTitle", value); }
		}

		public IWfActivity Instance
		{
			get
			{
				if (this._Instance == null && this.ProcessInstance != null)
					this._Instance = this.ProcessInstance.Activities.FindActivityByDescriptorKey(this.Key);

				return this._Instance;
			}
			internal set
			{
				_Instance = value;
			}
		}

		public WfResourceDescriptorCollection Resources
		{
			get
			{
				if (this._Resources == null)
					this._Resources = new WfResourceDescriptorCollection(this);

				return this._Resources;
			}
		}

		public FromTransitionsDescriptorCollection FromTransitions
		{
			get
			{
				if (this._FromTransitions == null)
					this._FromTransitions = new FromTransitionsDescriptorCollection(this);

				return this._FromTransitions;
			}
		}

		public ToTransitionsDescriptorCollection ToTransitions
		{
			get
			{
				if (this._ToTransitions == null)
					this._ToTransitions = new ToTransitionsDescriptorCollection(this);

				return this._ToTransitions;
			}
		}

		public WfVariableDescriptorCollection Variables
		{
			get
			{
				if (this._Variables == null)
					this._Variables = new WfVariableDescriptorCollection(this);

				return this._Variables;
			}
		}

		public IWfProcessDescriptor Process
		{
			get
			{
				return this._Process;
			}
			set
			{
				this._Process = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public WfConditionDescriptor Condition
		{
			get
			{
				if (this._Condition == null)
					this._Condition = new WfConditionDescriptor(this);

				return this._Condition;
			}
			set
			{
				this._Condition = value;
				this._Condition.Owner = this;
			}
		}

		/// <summary>
		/// 关联活动的Key，如果有此节点，表示此节点是被某个活动创建的。此属性不需要JSON序列化
		/// </summary>
		public string AssociatedActivityKey
		{
			get;
			set;
		}

		/// <summary>
		/// 当前活动是从哪里复制的
		/// </summary>
		public string ClonedKey
		{
			get;
			set;
		}

		public WfBranchProcessTemplateCollection BranchProcessTemplates
		{
			get
			{
				if (this._BranchProcessTemplates == null)
					this._BranchProcessTemplates = new WfBranchProcessTemplateCollection(this);

				return _BranchProcessTemplates;
			}
		}

		public WfRelativeLinkDescriptorCollection RelativeLinks
		{
			get
			{
				if (this._RelativeLinks == null)
					this._RelativeLinks = new WfRelativeLinkDescriptorCollection(this);

				return this._RelativeLinks;
			}
		}

		/// <summary>
		/// 进入活动时的被通知人
		/// </summary>
		public WfResourceDescriptorCollection EnterEventReceivers
		{
			get
			{
				if (this._EnterEventReceivers == null)
					this._EnterEventReceivers = new WfResourceDescriptorCollection(this);

				return this._EnterEventReceivers;
			}
		}

		/// <summary>
		/// 离开活动时的被通知人
		/// </summary>
		public WfResourceDescriptorCollection LeaveEventReceivers
		{
			get
			{
				if (this._LeaveEventReceivers == null)
					this._LeaveEventReceivers = new WfResourceDescriptorCollection(this);

				return this._LeaveEventReceivers;
			}
		}

		/// <summary>
		/// 内部相关人员
		/// </summary>
		public WfResourceDescriptorCollection InternalRelativeUsers
		{
			get
			{
				if (this._InternalRelativeUsers == null)
					this._InternalRelativeUsers = new WfResourceDescriptorCollection(this);

				return this._InternalRelativeUsers;
			}
		}

		/// <summary>
		/// 外部相关人员
		/// </summary>
		public WfExternalUserCollection ExternalUsers
		{
			get
			{
				if (this._ExternalUsers == null)
					this._ExternalUsers = new WfExternalUserCollection();

				return this._ExternalUsers;
			}
		}

		/// <summary>
		/// 进入活动时调用的WebService
		/// </summary>
		public WfServiceOperationDefinitionCollection EnterEventExecuteServices
		{
			get
			{
				if (this._EnterEventExecServices == null)
				{
					this._EnterEventExecServices = new WfServiceOperationDefinitionCollection();
				}

				return this._EnterEventExecServices;
			}
		}

		/// <summary>
		/// 离开活动时调用的WebService
		/// </summary>
		public WfServiceOperationDefinitionCollection LeaveEventExecuteServices
		{
			get
			{
				if (this._LeaveEventExecServices == null)
				{
					this._LeaveEventExecServices = new WfServiceOperationDefinitionCollection();
				}

				return this._LeaveEventExecServices;
			}
		}

		/// <summary>
		/// 流和自动收集参数集合
		/// </summary>
		public WfParameterNeedToBeCollected ParametersNeedToBeCollected
		{
			get
			{
				if (this._ParametersNeedToBeCollected == null)
					this._ParametersNeedToBeCollected = new WfParameterNeedToBeCollected();

				return this._ParametersNeedToBeCollected;
			}
		}

		/// <summary>
		/// 在导航栏上的显示方式
		/// </summary>
		public WfNavigatorDisplayMode NavigatorDisplayMode
		{
			get { return Properties.GetValue("NavigatorDisplayMode", WfNavigatorDisplayMode.DependsOnProcess); }
			set { Properties.SetValue("NavigatorDisplayMode", value); }
		}

		/// <summary>
		/// 是否是条件节点（有条件，且有开始时间）
		/// </summary>
		public bool IsConditionActivity
		{
			get
			{
				return EstimateStartTime != DateTime.MinValue ||
					this.Condition.IsEmpty == false;
			}
		}

		/// <summary>
		/// 是否是流程实例主线流程中的活动点
		/// </summary>
		public bool IsMainStreamActivity
		{
			get
			{
				bool result = false;

				if (this._Process != null)
					result = this._Process.IsMainStream;

				return result;
			}
		}

		/// <summary>
		/// 在退件时，是否需要跳过去的活动
		/// </summary>
		public bool IsReturnSkipped
		{
			get
			{
				return this.Variables.GetValue("IsReturnSkipped", false);
			}
			set
			{
				WfVariableDescriptor variable = null;

				if (this.Variables.ContainsKey("IsReturnSkipped"))
				{
					variable = this.Variables["IsReturnSkipped"];
					variable.OriginalValue = value.ToString();
				}
				else
				{
					variable = new WfVariableDescriptor("IsReturnSkipped", value.ToString(), DataType.Boolean);
					this.Variables.AddNotExistsItem(variable);
				}
			}
		}

		/// <summary>
		/// 由模板活动生成的活动标志
		/// </summary>
		public bool GeneratedByTemplate
		{
			get
			{
				return this.Variables.GetValue("GeneratedByTemplate", false);
			}
			set
			{
				this.Variables.SetValue("GeneratedByTemplate", value.ToString(), DataType.Boolean);
			}
		}

		/// <summary>
		/// 生成此活动的动态模板活动的Key
		/// </summary>
		public string TemplateKey
		{
			get
			{
				return this.Variables.GetValue("TemplateKey", string.Empty);
			}
			set
			{
				this.Variables.SetValue("TemplateKey", value);
			}
		}

		/// <summary>
		/// 相同组的活动。当Varialbes中的“ActivityGroupName”变量值相同的活动。如果ActivityGroupName为空，则以Key值代替(组里只有自己)
		/// </summary>
		public IEnumerable<IWfActivityDescriptor> GetSameGroupActivities()
		{
			string groupName = this.Variables.GetValue(WfHelper.ActivityGroupName, this.Key);

			List<IWfActivityDescriptor> result = new List<IWfActivityDescriptor>();

			if (this.Process != null)
			{
				foreach (IWfActivityDescriptor actDesp in this.Process.Activities)
				{
					if (actDesp.Variables.GetValue(WfHelper.ActivityGroupName, actDesp.Key) == groupName)
						result.Add(actDesp);
				}
			}

			return result;
		}

		/// <summary>
		/// 根据活动的出线查找后续第一个符合条件的活动
		/// </summary>
		/// <param name="predicate">条件回调</param>
		/// <returns></returns>
		public IWfActivityDescriptor FindSubsequentActivity(Func<IWfTransitionDescriptor, IWfActivityDescriptor, bool> predicate)
		{
			IWfActivityDescriptor result = null;

			if (predicate != null)
			{
				Dictionary<string, IWfTransitionDescriptor> elapsedTransitions = new Dictionary<string, IWfTransitionDescriptor>();

				result = InnerFindSubsequentActivity(this, elapsedTransitions, predicate);
			}

			return result;
		}

		private static IWfActivityDescriptor InnerFindSubsequentActivity(IWfActivityDescriptor startActivity,
			Dictionary<string, IWfTransitionDescriptor> elapsedTransitions,
			Func<IWfTransitionDescriptor, IWfActivityDescriptor, bool> predicate)
		{
			IWfActivityDescriptor result = null;

			foreach (IWfTransitionDescriptor transition in startActivity.ToTransitions)
			{
				if (elapsedTransitions.ContainsKey(transition.Key) == false)
				{
					elapsedTransitions.Add(transition.Key, transition);

					if (predicate(transition, transition.ToActivity))
						result = transition.ToActivity;
					else
						result = InnerFindSubsequentActivity(transition.ToActivity, elapsedTransitions, predicate);

					if (result != null)
						break;
				}
			}

			return result;
		}
		#endregion

		protected override PropertyDefineCollection GetPropertyDefineCollection()
		{
			return GetCachedPropertyDefineCollection("WfActivityDescriptor_" + ActivityType.ToString(),
				() =>
				{
					PropertyDefineCollection extraPdc = new PropertyDefineCollection();

					extraPdc.LoadPropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups["BasicActivityProperties"]);

					switch (this.ActivityType)
					{
						case WfActivityType.InitialActivity:
							extraPdc.AppendPropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups["InitialActivityProperties"]);
							break;
						case WfActivityType.CompletedActivity:
							extraPdc.AppendPropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups["CompletedActivityProperties"]);
							break;
						case WfActivityType.NormalActivity:
							extraPdc.AppendPropertiesFromConfiguration(WfActivitySettings.GetConfig().PropertyGroups["NormalActivityProperties"]);
							break;
					}

					return extraPdc;
				});
		}

		internal void SetProcessInstance(IWfProcess process)
		{
			this.ProcessInstance = process;

			this.Resources.ForEach(r => r.SetProcessInstance(process));
			//进入和离开活动点，资源未设置，于2011-9-6添加
			this.EnterEventReceivers.ForEach(r => r.SetProcessInstance(process));
			this.LeaveEventReceivers.ForEach(r => r.SetProcessInstance(process));

			this.ToTransitions.ForEach(t => ((WfTransitionDescriptor)t).ProcessInstance = process);
			//2011-9-1 徐磊修改:分支流程资源的实例问题
			this.BranchProcessTemplates.ForEach(b =>
			{
				((WfBranchProcessTemplateDescriptor)b).ProcessInstance = process;
				b.Resources.ForEach(r => r.SetProcessInstance(process));
			});
		}

		private bool InnerCanReachTo(string targetActivityDespKey, Dictionary<IWfActivityDescriptor, IWfActivityDescriptor> history)
		{
			bool result = false;

			if (history.ContainsKey(this) == false)
			{
				result = string.Compare(this.Key, targetActivityDespKey, true) == 0;

				if (result == false)
				{
					history.Add(this, this);

					foreach (IWfTransitionDescriptor transition in this.ToTransitions)
					{
						if (transition is IWfForwardTransitionDescriptor)
						{
							result = ((WfActivityDescriptor)transition.ToActivity).InnerCanReachTo(targetActivityDespKey, history);

							if (result)
								break;
						}
					}
				}
			}

			return result;
		}

		public bool CanReachTo(IWfActivityDescriptor targetAct)
		{
			bool result = false;

			if (targetAct != null)
			{
				Dictionary<IWfActivityDescriptor, IWfActivityDescriptor> history = new Dictionary<IWfActivityDescriptor, IWfActivityDescriptor>();

				result = InnerCanReachTo(targetAct.Key, history);
			}

			return result;
		}

		/// <summary>
		/// 得到关联的活动定义
		/// </summary>
		/// <returns></returns>
		public IWfActivityDescriptor GetAssociatedActivity()
		{
			IWfActivityDescriptor result = this;

			if (this.AssociatedActivityKey.IsNotEmpty())
				result = this.Process.Activities[this.AssociatedActivityKey];

			return result;
		}

		/// <summary>
		/// 将所有包含originalUser的WfUserResourceDescriptor的资源替换为包含replaceUsers的一系列资源。
		/// 如果replaceUsers为null或者空集合，则相当于删除原始用户
		/// </summary>
		/// <param name="originalUser"></param>
		/// <param name="replaceUsers"></param>
		/// <returns>被替换的个数</returns>
		public int ReplaceAllUserResourceDescriptors(IUser originalUser, IEnumerable<IUser> replaceUsers)
		{
			int result = 0;

			result += this.Resources.ReplaceAllUserResourceDescriptors(originalUser, replaceUsers);
			result += this.EnterEventReceivers.ReplaceAllUserResourceDescriptors(originalUser, replaceUsers);
			result += this.LeaveEventReceivers.ReplaceAllUserResourceDescriptors(originalUser, replaceUsers);

			result += this.BranchProcessTemplates.ReplaceAllUserResourceDescriptors(originalUser, replaceUsers);

			return result;
		}

		/// <summary>
		/// 在当前活动之前插入活动。当前活动不能是起始点。插入后，当前活动点的进线搬移到新活动点上，
		/// 新活动点到当前活动点之间，另创建一根出线（前进线）。
		/// </summary>
		/// <param name="newActDesp"></param>
		public void InsertBefore(IWfActivityDescriptor newActDesp)
		{
			newActDesp.NullCheck("newActDesp");
			//(ActivityType != WfActivityType.InitialActivity).FalseThrow("不能在起始活动前插入活动");

			newActDesp.FromTransitions.Clear();
			newActDesp.ToTransitions.Clear();

			newActDesp.AssociatedActivityKey = WfRuntime.ProcessContext.ActivityChangingContext.AssociatedActivityKey;

			if (this.ActivityType == WfActivityType.InitialActivity)
			{
				((WfActivityDescriptor)newActDesp).ActivityType = WfActivityType.InitialActivity;
				this.ActivityType = WfActivityType.NormalActivity;
				this.Process.Activities.InitialActivity = newActDesp;
			}

			this.Process.Activities.Add(newActDesp);

			IList<IWfTransitionDescriptor> fromReturnTrans = this.FromTransitions.FindAll(t => t.IsBackward == true);
			IList<IWfTransitionDescriptor> fromNotReturnTrans = this.FromTransitions.FindAll(t => t.IsBackward == false);

			List<IWfTransitionDescriptor> thisFromTransitions = new List<IWfTransitionDescriptor>(this.FromTransitions);

			//处理当前点所有的进线，清除原来的From点的出线
			foreach (IWfTransitionDescriptor thisFromTransition in thisFromTransitions)
			{
				thisFromTransition.FromActivity.ToTransitions.RemoveByToActivity(this);
			}

			this.FromTransitions.Clear();

			newActDesp.FromTransitions.CopyFrom(fromNotReturnTrans);
			newActDesp.FromTransitions.CopyFrom(fromReturnTrans);

			newActDesp.ToTransitions.AddForwardTransition(this);

			foreach (WfTransitionDescriptor newFromTransition in newActDesp.FromTransitions)
			{
				newFromTransition.JoinActivity(newFromTransition.FromActivity, newActDesp);
				newFromTransition.FromActivity.ToTransitions.Add(newFromTransition);
			}

			if (this.Instance != null)
			{
				if (WfRuntime.ProcessContext != null)
					WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this.Instance.Process);
			}
		}

		/// <summary>
		/// 在当前活动之后添加活动。当前活动不能是结束点。
		/// 自动创建一条当前点和新节点的连线，而当前点的向前的出线，将作为新点的出线。
		/// </summary>
		/// <param name="newActDesp"></param>
		public void Append(IWfActivityDescriptor newActDesp)
		{
			Append(newActDesp, false);
		}

		/// <summary>
		/// 在当前活动之后添加活动。当前活动不能是结束点。
		/// 自动创建一条当前点和新节点的连线，而当前点的向前的出线，将作为新点的出线。
		/// </summary>
		/// <param name="newActDesp">需要添加的新活动</param>
		/// <param name="moveReturnLine">是否将当前点的退回线也复制到新的活动之后</param>
		public void Append(IWfActivityDescriptor newActDesp, bool moveReturnLine)
		{
			newActDesp.NullCheck("newActDesp");
			(ActivityType != WfActivityType.CompletedActivity).FalseThrow("不能在结束活动后添加活动");

			newActDesp.FromTransitions.Clear();
			newActDesp.ToTransitions.Clear();

			newActDesp.AssociatedActivityKey = WfRuntime.ProcessContext.ActivityChangingContext.AssociatedActivityKey;

			this.Process.Activities.Add(newActDesp);

			IList<IWfTransitionDescriptor> notReturnTrans = this.ToTransitions.FindAll(t => t.IsBackward == false);
			IList<IWfTransitionDescriptor> returnTrans = this.ToTransitions.FindAll(t => t.IsBackward == true);

			newActDesp.ToTransitions.CopyFrom(notReturnTrans);

			if (moveReturnLine)
				newActDesp.ToTransitions.CopyFrom(returnTrans);

			this.ToTransitions.Clear();

			//是否将退回线移动到新活动后，如果不是，则保留到当前活动后
			if (moveReturnLine == false)
				this.ToTransitions.CopyFrom(returnTrans);

			this.ToTransitions.AddForwardTransition(newActDesp);

			foreach (WfTransitionDescriptor t in newActDesp.ToTransitions)
			{
				t.JoinActivity(newActDesp, t.ToActivity);
				t.ToActivity.FromTransitions.Add(t);
			}

			if (this.Instance != null)
			{
				if (WfRuntime.ProcessContext != null)
					WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this.Instance.Process);
			}
		}

		/// <summary>
		/// 添加本身连线
		/// </summary>
		/// <param name="cloneTranDes"></param>
		/// <param name="newActDesp"></param>
		private void AddSelfCloneTransition(WfTransitionDescriptor cloneTranDes, IWfActivityDescriptor newActDesp)
		{
			cloneTranDes.JoinActivity(this, newActDesp);
			this.ToTransitions.Add(cloneTranDes);
			newActDesp.FromTransitions.Add(cloneTranDes);
		}

		public override void MergeDefinedProperties()
		{
			base.MergeDefinedProperties();

			foreach (WfBranchProcessTemplateDescriptor template in this.BranchProcessTemplates)
				template.MergeDefinedProperties();

			foreach (WfTransitionDescriptor t in this.ToTransitions)
				t.MergeDefinedProperties();
		}

		public override void SyncPropertiesToFields()
		{
			this.Condition.SyncPropertiesToFields(this.Properties["Condition"]);
			this.Variables.SyncPropertiesToFields(this.Properties["Variables"], ReservedVariableNames);
			this.Resources.SyncPropertiesToFields(this.Properties["Resources"]);
			this.RelativeLinks.SyncPropertiesToFields(this.Properties["RelativeLinks"]);
			this.BranchProcessTemplates.SyncPropertiesToFields(this.Properties["BranchProcessTemplates"]);
			this.EnterEventReceivers.SyncPropertiesToFields(this.Properties["EnterEventReceivers"]);
			this.LeaveEventReceivers.SyncPropertiesToFields(this.Properties["LeaveEventReceivers"]);
			this.LeaveEventReceivers.SyncPropertiesToFields(this.Properties["LeaveEventReceivers"]);

			this.EnterEventExecuteServices.SyncPropertiesToFields(this.Properties["EnterEventExecuteServices"]);
			this.LeaveEventExecuteServices.SyncPropertiesToFields(this.Properties["LeaveEventExecuteServices"]);
			this.InternalRelativeUsers.SyncPropertiesToFields(this.Properties["InternalRelativeUsers"]);
			this.ExternalUsers.SyncPropertiesToFields(this.Properties["ExternalUsers"]);
			this.ParametersNeedToBeCollected.SyncPropertiesToFields(this.Properties["ParametersNeedToBeCollected"]);
		}

		internal void CopyPropertiesTo(WfKeyedDescriptorBase destObject)
		{
			WfActivityDescriptor actDesp = (WfActivityDescriptor)destObject;

			base.CloneProperties(destObject);

			this.CloneCollectionProperties(actDesp);
		}

		private void CloneCollectionProperties(WfActivityDescriptor targetActDesp)
		{
			targetActDesp.BranchProcessTemplates.Clear();
			targetActDesp.BranchProcessTemplates.CopyFrom(this.BranchProcessTemplates, t => t.Clone());

			targetActDesp.Condition = this.Condition.Clone();

			targetActDesp.EnterEventReceivers.Clear();
			targetActDesp.EnterEventReceivers.CopyFrom(this.EnterEventReceivers);

			targetActDesp.ExternalUsers.Clear();
			targetActDesp.ExternalUsers.CopyFrom(this.ExternalUsers);

			targetActDesp.InternalRelativeUsers.Clear();
			targetActDesp.InternalRelativeUsers.CopyFrom(this.InternalRelativeUsers);

			targetActDesp.LeaveEventReceivers.Clear();
			targetActDesp.LeaveEventReceivers.CopyFrom(this.LeaveEventReceivers);

			if (this.Process != null)
				targetActDesp.Process = this.Process;

			targetActDesp.RelativeLinks.Clear();
			targetActDesp.RelativeLinks.CopyFrom(this.RelativeLinks);

			targetActDesp.Resources.Clear();
			targetActDesp.Resources.CopyFrom(this.Resources);

			targetActDesp.Variables.Clear();
			targetActDesp.Variables.CopyFrom(this.Variables, v => v.Clone());

			targetActDesp.EnterEventExecuteServices.Clear();
			targetActDesp.EnterEventExecuteServices.CopyFrom(this.EnterEventExecuteServices, s => s.Clone());

			targetActDesp.LeaveEventExecuteServices.Clear();
			targetActDesp.LeaveEventExecuteServices.CopyFrom(this.LeaveEventExecuteServices, s => s.Clone());
		}

		internal protected override void CloneProperties(WfKeyedDescriptorBase destObject)
		{
			WfActivityDescriptor actDesp = (WfActivityDescriptor)destObject;

			actDesp.ActivityType = this.ActivityType;

			CopyPropertiesTo(actDesp);

			if (this.Process != null)
				actDesp.Key = this.Process.FindNotUsedActivityKey();
		}

		/// <summary>
		/// 克隆某个节点到当前节点下
		/// </summary>
		/// <returns>被克隆的节点</returns>
		public IWfActivityDescriptor Clone()
		{
			WfActivityDescriptor actDesp = null;

			//YDZ 2012/12/03 Clone 当Initial向Normal转换时，重新加载数据发生改变
			if (this.ActivityType == WfActivityType.InitialActivity)
				actDesp = this.InitialToNormalActivity();
			else
			{
				actDesp = new WfActivityDescriptor();
				CloneProperties(actDesp);
			}

			return actDesp;
		}

		/// <summary>
		/// 得到所有的出线所包含的活动（没有重复的）
		/// </summary>
		/// <returns></returns>
		public WfActivityDescriptorCollection GetToActivities()
		{
			WfActivityDescriptorCollection result = new WfActivityDescriptorCollection(this.Process);

			foreach (IWfTransitionDescriptor transition in this.ToTransitions)
			{
				if (result.ContainsKey(transition.ToActivity.Key) == false)
					result.Add(transition.ToActivity);
			}

			return result;
		}

		/// <summary>
		/// 得到所有的进线所包含的活动（没有重复的）
		/// </summary>
		/// <returns></returns>
		public WfActivityDescriptorCollection GetFromActivities()
		{
			WfActivityDescriptorCollection result = new WfActivityDescriptorCollection(this.Process);

			foreach (IWfTransitionDescriptor transition in this.FromTransitions)
			{
				if (result.ContainsKey(transition.FromActivity.Key) == false)
					result.Add(transition.FromActivity);
			}

			return result;
		}

		/// <summary>
		/// Clone传入的线，然后与现有的线合并。
		/// 合并的原则是，目标点如果在现有的集合中已经存在，则不合并。
		/// </summary>
		/// <returns>最后添加的线</returns>
		/// <param name="sourceTransitions"></param>
		public IList<IWfTransitionDescriptor> CloneAndMergeToTransitions(IEnumerable<IWfTransitionDescriptor> sourceTransitions)
		{
			this.Process.NullCheck("不能在没有流程定义的情况下执行此方法");

			List<IWfTransitionDescriptor> addedTransitions = new List<IWfTransitionDescriptor>();

			if (sourceTransitions != null)
			{
				foreach (IWfTransitionDescriptor transition in sourceTransitions)
				{
					string targetActivityKey = transition.ToActivity.Key;

					if (this.ToTransitions.Exists(t => t.ToActivity.Key == targetActivityKey) == false)
					{
						IWfTransitionDescriptor clonedTransition = ((WfTransitionDescriptor)transition).Clone();

						((WfTransitionDescriptor)clonedTransition).Key = this.Process.FindNotUsedTransitionKey();
						((WfTransitionDescriptor)clonedTransition).JoinActivity(transition.FromActivity, transition.ToActivity);

						addedTransitions.Add(clonedTransition);
					}
				}

				foreach (IWfTransitionDescriptor transition in addedTransitions)
					transition.ConnectActivities(this, transition.ToActivity);
			}

			return addedTransitions;
		}

		/// <summary>
		/// 将当前Initial节点转换成Normal节点
		/// </summary>
		/// <returns></returns>
		private WfActivityDescriptor InitialToNormalActivity()
		{
			string activityKey = this.Key;
			if (this.Process != null)
				activityKey = this.Process.FindNotUsedActivityKey();

			WfActivityDescriptor actDesp = new WfActivityDescriptor(activityKey, WfActivityType.NormalActivity);

			foreach (PropertyValue pv in this.Properties)
			{
				PropertyValue currentProperty = actDesp.Properties[pv.Definition.Name];
				if (currentProperty != null)
				{
					currentProperty.StringValue = string.IsNullOrEmpty(pv.StringValue) ? pv.Definition.DefaultValue : pv.StringValue;
				}
			}

			actDesp.Key = activityKey;

			if (this.ProcessInstance != null)
				actDesp.ProcessInstance = this.ProcessInstance;

			this.CloneCollectionProperties(actDesp);

			return actDesp;
		}

		/// <summary>
		/// 删除活动描述本身，前后点的所有线都被删除
		/// </summary>
		public void Remove()
		{
			this.FromTransitions.Clear();
			this.ToTransitions.Clear();

			//删除自己这个活动
			this.Process.Activities.Remove(act => act.Key == this.Key);

			if (this.Instance != null)
			{
				if (WfRuntime.ProcessContext != null)
					WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this.Instance.Process);
			}
		}

		/// <summary>
		/// 删除活动本身。在删除后，所有进线和出线所涉及到的活动会相互之间连接起来。
		/// 如果这些活动原本已经有连线了，为了避免两个活动间出线两条连线，因此应该舍弃到一些。
		/// 在线的模板方面，使用原有的出线作为线模板。如果前后两个点之间原本有线，则使用原来的线。
		/// 当然，如果前后两个点之间原来的线是退回线，则覆盖原来的线
		/// </summary>
		public void Delete()
		{
			(ActivityType != WfActivityType.InitialActivity).FalseThrow<WfDescriptorException>("不能删除初始活动描述");
			(ActivityType != WfActivityType.CompletedActivity).FalseThrow<WfDescriptorException>("不能删除结束活动描述");

			WfActivityDeleteInfoCollection adic = new WfActivityDeleteInfoCollection(this);

			this.Remove();

			adic.MergeOriginalActivitiesTranstions();
		}

		private static bool CanFromActivityAddTransition(IWfActivityDescriptor fromActDesp, WfTransitionDescriptor joinTransition)
		{
			bool result = true;

			fromActDesp.ToTransitions.ForEach(t =>
			{
				if (t.Name == joinTransition.Name
					&& t.ToActivity.Key == joinTransition.ToActivity.Key
					&& t.ToActivity.ActivityType == joinTransition.ToActivity.ActivityType
					&& t.DefaultSelect == joinTransition.DefaultSelect
					&& t.AffectedProcessReturnValue == joinTransition.AffectedProcessReturnValue
					&& t.AffectProcessReturnValue == joinTransition.AffectProcessReturnValue
					&& t.Condition.Expression == joinTransition.Condition.Expression
					&& t.Priority == joinTransition.Priority
					)
					result = false;
			});

			return result;
		}


		/// <summary>
		/// 得到活动描述的唯一进线，如果找不到，则返回null
		/// </summary>
		/// <param name="actDesp"></param>
		/// <returns></returns>
		private static IWfTransitionDescriptor FindExclusiveFromTransition(IWfActivityDescriptor actDesp)
		{
			IWfTransitionDescriptor result = null;

			if (actDesp.FromTransitions.Count == 1)
			{
				result = actDesp.FromTransitions[0];
			}
			else
			{
				foreach (IWfTransitionDescriptor transition in actDesp.FromTransitions)
				{
					IWfActivityDescriptor fromActDesp = transition.FromActivity;

					if (fromActDesp.Instance != null && fromActDesp.Instance.Status != WfActivityStatus.NotRunning)
					{
						result = transition;
						break;
					}
				}
			}

			return result;
		}

		internal IWfTransitionDescriptor FindExsitedDynamicToTransition(IWfActivityDescriptor templateActDesp)
		{
			IWfTransitionDescriptor result = null;

			foreach (IWfTransitionDescriptor transition in this.ToTransitions)
			{
				if (transition.Properties.GetValue("DynamicSource", string.Empty) == templateActDesp.Key)
				{
					result = transition;
					break;
				}
			}

			return result;
		}

		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			((ISimpleXmlSerializer)this.Properties).ToXElement(element, refNodeName);

			if (this.Resources.Count > 0)
				((ISimpleXmlSerializer)this.Resources).ToXElement(element, "Resources");

			if (this.EnterEventReceivers.Count > 0)
				((ISimpleXmlSerializer)this.EnterEventReceivers).ToXElement(element, "EnterEventReceivers");

			if (this.LeaveEventReceivers.Count > 0)
				((ISimpleXmlSerializer)this.LeaveEventReceivers).ToXElement(element, "LeaveEventReceivers");

			if (this.Condition.IsEmpty == false)
				((ISimpleXmlSerializer)this.Condition).ToXElement(element, "Condition");

			if (this.EnterEventExecuteServices.Count > 0)
				((ISimpleXmlSerializer)this.EnterEventExecuteServices).ToXElement(element, "EnterEventExecuteServices");

			if (this.LeaveEventExecuteServices.Count > 0)
				((ISimpleXmlSerializer)this.LeaveEventExecuteServices).ToXElement(element, "LeaveEventExecuteServices");

			if (this.BranchProcessTemplates.Count > 0)
				((ISimpleXmlSerializer)this.BranchProcessTemplates).ToXElement(element, "BranchProcessTemplates");

			if (this.RelativeLinks.Count > 0)
				((ISimpleXmlSerializer)this.RelativeLinks).ToXElement(element, "RelativeLinks");
		}

		#endregion
	}

	[Serializable]
	[XElementSerializable]
	public class WfActivityDescriptorCollection : WfKeyedDescriptorCollectionBase<IWfActivityDescriptor>, ISimpleXmlSerializer
	{
		private IWfProcessDescriptor _Process = null;
		private IWfActivityDescriptor _InitialActivity = null;
		private IWfActivityDescriptor _CompletedActivity = null;

		public WfActivityDescriptorCollection()
			: base(null)
		{
		}

		protected WfActivityDescriptorCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		public WfActivityDescriptorCollection(IWfProcessDescriptor processDesp)
			: base(processDesp)
		{
			(processDesp != null).FalseThrow<ArgumentNullException>("processDesp");

			this._Process = processDesp;
		}

		internal IWfActivityDescriptor InitialActivity
		{
			get { return this._InitialActivity; }
			set { this._InitialActivity = value; }
		}

		internal IWfActivityDescriptor CompletedActivity
		{
			get { return _CompletedActivity; }
		}

		public IWfProcessDescriptor Process
		{
			get { return _Process; }
		}

		/// <summary>
		/// 将所有包含originalUser的WfUserResourceDescriptor的资源替换为包含replaceUsers的一系列资源。
		/// 如果replaceUsers为null或者空集合，则相当于删除原始用户
		/// </summary>
		/// <param name="originalUser"></param>
		/// <param name="replaceUsers"></param>
		/// <returns>被替换的个数</returns>
		public int ReplaceAllUserResourceDescriptors(IUser originalUser, IEnumerable<IUser> replaceUsers)
		{
			int result = 0;

			foreach (IWfActivityDescriptor actDesp in this)
				result += actDesp.ReplaceAllUserResourceDescriptors(originalUser, replaceUsers);

			return result;
		}

		protected override void OnInsert(int index, object value)
		{
			if (XElementFormatter.FormattingStatus == XElementFormattingStatus.None)
			{
				WfActivityDescriptor item = (WfActivityDescriptor)value;
				ValidateActivity(item);

				item.Process = _Process;

				switch (item.ActivityType)
				{
					case WfActivityType.InitialActivity:
						this._InitialActivity = item;
						break;
					case WfActivityType.CompletedActivity:
						this._CompletedActivity = item;
						break;
				}
			}

			base.OnInsert(index, value);
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			base.OnRemoveComplete(index, value);

			if (XElementFormatter.FormattingStatus == XElementFormattingStatus.None)
			{
				WfActivityDescriptor item = (WfActivityDescriptor)value;

				switch (item.ActivityType)
				{
					case WfActivityType.InitialActivity:
						this._InitialActivity = null;
						break;
					case WfActivityType.CompletedActivity:
						this._CompletedActivity = null;
						break;
				}
			}
		}

		protected override void OnValidate(object value)
		{
			value.NullCheck("value");

			if (this.Owner != null && this.Owner.ProcessInstance != null)
				((WfDescriptorBase)value).ProcessInstance = this.Owner.ProcessInstance;

			base.OnValidate(value);
		}

		private void ValidateActivity(IWfActivityDescriptor item)
		{
			switch (item.ActivityType)
			{
				case WfActivityType.InitialActivity:
					CheckActivityType(WfActivityType.InitialActivity, "一个流程只能有一个开始活动");
					break;
				case WfActivityType.CompletedActivity:
					CheckActivityType(WfActivityType.CompletedActivity, "一个流程只能有一个结束活动");
					break;
			}
		}

		private void CheckActivityType(WfActivityType activityType, string errorMessage)
		{
			foreach (IWfActivityDescriptor act in base.List)
			{
				(act.ActivityType == activityType).TrueThrow(Translator.Translate(WfHelper.CultureCategory, errorMessage));
			}
		}

		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			foreach (IWfActivityDescriptor actDesp in this)
			{
				XElement actElem = element.AddChildElement("Activity");

				((ISimpleXmlSerializer)actDesp).ToXElement(actElem, refNodeName);
			}
		}

		#endregion
	}

	/// <summary>
	/// 活动上的条件计算所产生的异常
	/// </summary>
	[Serializable]
	public class WfActivityEvaluationException : WfEvaluationExceptionBase
	{
		/// <summary>
		/// 
		/// </summary>
		public WfActivityEvaluationException()
			: base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public WfActivityEvaluationException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		/// <param name="condition"></param>
		public WfActivityEvaluationException(string message, WfConditionDescriptor condition)
			: base(message, condition)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public WfActivityEvaluationException(string message, System.Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		/// <param name="condition"></param>
		public WfActivityEvaluationException(string message, System.Exception innerException, WfConditionDescriptor condition)
			: base(message, innerException, condition)
		{
		}
	}
}
