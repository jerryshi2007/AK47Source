using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Descriptors;

namespace MCS.Library.Workflow.Engine
{
    public interface IWorkflowReader
    {
        WfProcessCollection LoadProcesses(params string[] processIDs);

        string GetProcessIDByActivityID(string activityID);

        List<string> GetBranchProcessIDsByOperationID(string operationID);

        List<string> GetProcessIDsByResourceID(string resourceID);

		IWfOperation LoadOperationByBranchProcessID(string processID);

		WfOperationCollection LoadOperations(IWfAnchorActivity activity);

        Dictionary<string, string> GetWfContext(params string[] IDs);

		string GetBranchContext(string operationID, string processID);

		Dictionary<string, WfAssigneeCollection> LoadAssignees(params string[] activityIDs);

        List<string> GetUserRelativeProcessIDsByResourceID(string resourceID, IUser user);

		WfActivityCollection LoadProcessActivities(IWfProcess process);

		IWfProcessDescriptor LoadProcessDescriptor(string processID, string processKey);
    }
}