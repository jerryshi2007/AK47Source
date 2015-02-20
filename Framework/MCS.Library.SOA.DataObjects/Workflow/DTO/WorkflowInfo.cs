using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow.DTO
{
	public class WorkflowInfo
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public string ApplicationName { get; set; }
		public string ProgramName { get; set; }
		public string GraphDescription { get; set; }

		public string ResourceID { get; set; }
		public string Status { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public string Creator { get; set; }
		public string OwnerDepartment { get; set; }
		public string CurrentActivityKey { get; set; }
		public int UpdateTag { get; set; }

		private List<ActivityInfo> _activities = new List<ActivityInfo>();
		public List<ActivityInfo> Activities
		{
			get
			{
				return _activities;
			}
			set
			{
				this._activities = value;
			}
		}

		private List<TransitionInfo> _transitions = new List<TransitionInfo>();
		public List<TransitionInfo> Transitions
		{
			get
			{
				return _transitions;
			}
			set
			{
				this._transitions = value;
			}
		}

		public static WorkflowInfo ProcessAdapter(IWfProcess procInstance)
		{
			return ProcessAdapter(procInstance, false);
		}

		public static WorkflowInfo ProcessAdapter(IWfProcess procInstance, bool isMainStream)
		{
			WorkflowInfo result = new WorkflowInfo();

			if (procInstance == null)
				return result;

			AdaptProcessProperties(procInstance, result, isMainStream);

			if (isMainStream == false || procInstance.MainStream == null)
			{
				foreach (IWfActivity instanceAct in procInstance.Activities)
				{
					result.Activities.Add(AdaptActivityProperties(instanceAct.Descriptor, isMainStream));
					result.Transitions.AddRange(AdaptTransitionProperties(procInstance, instanceAct));
				}
			}
			else
			{
				foreach (IWfActivityDescriptor instanceActDesc in procInstance.MainStream.Activities)
				{
					result.Activities.Add(AdaptActivityProperties(instanceActDesc, isMainStream));
					result.Transitions.AddRange(AdaptMainStreamTransitionProperties(instanceActDesc));
				}
			}

			return result;
		}

		private static List<TransitionInfo> AdaptMainStreamTransitionProperties(IWfActivityDescriptor instanceActDesc)
		{
			List<TransitionInfo> result = new List<TransitionInfo>();

			foreach (var tran in instanceActDesc.ToTransitions)
			{
				var link = new TransitionInfo()
				{
					Key = tran.Key,
					Name = tran.Name,
					Enabled = tran.Enabled,
					FromActivityKey = instanceActDesc.Key,
					ToActivityKey = tran.ToActivity.Key,
					WfReturnLine = tran.IsBackward,
					IsPassed = false
				};

				result.Add(link);
			}

			return result;
		}

		private static List<TransitionInfo> AdaptTransitionProperties(IWfProcess procInstance, IWfActivity instanceAct)
		{
			List<TransitionInfo> result = new List<TransitionInfo>();
			IWfActivityDescriptor instanceActDesc = instanceAct.Descriptor;

			foreach (var tran in instanceActDesc.ToTransitions)
			{
				var link = new TransitionInfo()
				{
					Key = tran.Key,
					Name = tran.Name,
					Enabled = tran.Enabled,
					FromActivityKey = instanceActDesc.Key,
					ToActivityKey = tran.ToActivity.Key,
					WfReturnLine = tran.IsBackward,
					IsPassed = false
				};

				var fromAct = procInstance.Activities.FindActivityByDescriptorKey(link.FromActivityKey);
				var toAct = procInstance.Activities.FindActivityByDescriptorKey(link.ToActivityKey);

				if (fromAct != null && toAct != null)
				{
					if ((fromAct.Status == WfActivityStatus.Completed || fromAct.Status == WfActivityStatus.Running || fromAct.Status == WfActivityStatus.Pending) &&
						(toAct.Status == WfActivityStatus.Completed || toAct.Status == WfActivityStatus.Running || toAct.Status == WfActivityStatus.Pending))
					{
						link.IsPassed = true;
					}
				}

				result.Add(link);
			}

			return result;
		}

		private static ActivityInfo AdaptActivityProperties(IWfActivityDescriptor instanceActDesc, bool isMainStream)
		{
			ActivityInfo result = new ActivityInfo()
			{
				Key = instanceActDesc.Key,
				CloneKey = instanceActDesc.ClonedKey,
				Name = instanceActDesc.Name,
				Description = instanceActDesc.Description,
				Enabled = instanceActDesc.Enabled,
				ActivityType = instanceActDesc.ActivityType,
				IsDynamic = instanceActDesc.Properties.GetValue("IsDynamic", false),
				HasBranchProcess = false,
				ID = UuidHelper.NewUuidString(),
				Status = EnumItemDescriptionAttribute.GetDescription(WfActivityStatus.NotRunning)
			};

			if (isMainStream == false)
			{
				result.ID = instanceActDesc.Instance.ID;
				result.Status = EnumItemDescriptionAttribute.GetDescription(instanceActDesc.Instance.Status);
				result.StartTime = instanceActDesc.Instance.StartTime;
				result.EndTime = instanceActDesc.Instance.EndTime;

				if (instanceActDesc.Instance.BranchProcessGroups.Count > 0)
					result.HasBranchProcess = true;

				if (instanceActDesc.Instance.Operator != null)
				{
					result.Operator = instanceActDesc.Instance.Operator.DisplayName;
				}
				else
				{
					StringBuilder strBuilder = new StringBuilder();

					if (instanceActDesc.Instance.Status == WfActivityStatus.Running && instanceActDesc.Instance.Assignees.Count > 0)
					{
						instanceActDesc.Instance.Assignees.ForEach(p => strBuilder.Append(p.User.Name + " "));
					}
					else
					{
						instanceActDesc.Instance.Candidates.GetSelectedAssignees().ForEach(p => strBuilder.Append(p.User.Name + " "));
					}

					result.Operator = strBuilder.ToString();
				}
			}

			return result;
		}

		private static void AdaptProcessProperties(IWfProcess procInstance, WorkflowInfo targetDTO, bool isMainStream)
		{
			targetDTO.Key = procInstance.ID;

			targetDTO.Name = procInstance.Context.GetValue("RuntimeProcessName", string.Empty);
			targetDTO.UpdateTag = procInstance.UpdateTag;

			IWfProcessDescriptor instanceDesc = GetProcessDescriptor(procInstance, isMainStream);

			if (targetDTO.Name.IsNullOrEmpty())
				targetDTO.Name = instanceDesc.Name;

			targetDTO.GraphDescription = instanceDesc.GraphDescription;
			targetDTO.ApplicationName = instanceDesc.ApplicationName;
			targetDTO.ProgramName = instanceDesc.ProgramName;
			targetDTO.ResourceID = procInstance.ResourceID;
			targetDTO.Status = EnumItemDescriptionAttribute.GetDescription(procInstance.Status);
			targetDTO.StartTime = procInstance.StartTime;
			targetDTO.EndTime = procInstance.EndTime;
			targetDTO.Creator = (procInstance.Creator == null ? "" : procInstance.Creator.Name);
			targetDTO.OwnerDepartment = (procInstance.OwnerDepartment == null ? string.Empty : procInstance.OwnerDepartment.Name);
			targetDTO.CurrentActivityKey = (procInstance.CurrentActivity == null ? string.Empty : procInstance.CurrentActivity.Descriptor.Key);
		}

		private static IWfProcessDescriptor GetProcessDescriptor(IWfProcess procInstance, bool isMainStream)
		{
			IWfProcessDescriptor instanceDesc = procInstance.Descriptor;

			if (isMainStream && procInstance.MainStream != null)
				instanceDesc = procInstance.MainStream;

			return instanceDesc;
		}
	}
}
