using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.OGUPermission;
using MCS.Library.Passport;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Converters;
using MCS.Library.WF.Contracts.Converters.DataObjects;
using MCS.Library.WF.Contracts.Converters.Descriptors;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;

namespace MCS.Library.WF.Contracts.Converters.Runtime
{
    public class WfClientProcessInfoBaseConverter
    {
        public static readonly WfClientProcessInfoBaseConverter Instance = new WfClientProcessInfoBaseConverter();
        private WfClientProcessInfoFilter _Filter = WfClientProcessInfoFilter.Default;

        private WfClientProcessInfoBaseConverter()
        {
        }

        public WfClientProcessInfoBaseConverter(WfClientProcessInfoFilter filter)
        {
            this._Filter = filter;
        }

        public WfClientProcessInfoFilter Filter
        {
            get
            {
                return this._Filter;
            }
            set
            {
                this._Filter = value;
            }
        }

        public WfClientAuthorizationInfo GetAuthorizationInfo(IWfProcess process, IWfActivity originalActivity, WfClientUser user)
        {
            string userID = user.IsNotNullOrEmpty() ? user.ID : string.Empty;
            string originalActivityID = originalActivity != null ? originalActivity.ID : string.Empty;

            return GetAuthorizationInfo(process, originalActivityID, userID);
        }

        /// <summary>
        /// 得到在当前活动的意见信息
        /// </summary>
        /// <param name="originalActivity"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public WfClientOpinion GetUserActivityOpinion(IWfActivity originalActivity, WfClientUser user)
        {
            WfClientOpinion opinion = null;

            if (originalActivity != null && user != null)
            {
                WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

                builder.AppendItem("ACTIVITY_ID", originalActivity.ID);
                builder.AppendItem("ISSUE_PERSON_ID", user.ID);

                GenericOpinion serverOpinion = GenericOpinionAdapter.Instance.LoadByBuilder(builder).FirstOrDefault();

                if (serverOpinion != null)
                    WfClientOpinionConverter.Instance.ServerToClient(serverOpinion, ref opinion);
            }

            return opinion;
        }

        /// <summary>
        /// 把当前环节的信息填充到意见当中
        /// </summary>
        /// <param name="opinion"></param>
        /// <param name="originalActivity"></param>
        /// <param name="user"></param>
        /// <param name="delegatedUser"></param>
        public void FillOpinionInfoByProcessByActivity(WfClientOpinion opinion, IWfActivity originalActivity, WfClientUser user, WfClientUser delegatedUser)
        {
            if (opinion != null && originalActivity != null)
            {
                if (opinion.ID.IsNullOrEmpty())
                    opinion.ID = UuidHelper.NewUuidString();

                opinion.ResourceID = originalActivity.Process.ResourceID;
                opinion.ProcessID = originalActivity.Process.ID;
                opinion.ActivityID = originalActivity.ID;

                IWfActivity rootActivity = originalActivity.OpinionRootActivity;

                if (rootActivity.Process.MainStream != null && rootActivity.MainStreamActivityKey.IsNotEmpty())
                {
                    opinion.LevelName = rootActivity.MainStreamActivityKey;
                }
                else
                {
                    if (string.IsNullOrEmpty(rootActivity.Descriptor.AssociatedActivityKey))
                        opinion.LevelName = rootActivity.Descriptor.Key;
                    else
                        opinion.LevelName = rootActivity.Descriptor.AssociatedActivityKey;
                }

                if (rootActivity.Process.MainStream != null)
                    opinion.LevelDesp = rootActivity.Process.MainStream.Activities[opinion.LevelName].Name;
                else
                    opinion.LevelDesp = rootActivity.Descriptor.Process.Activities[opinion.LevelName].Name;

                if (user != null)
                {
                    opinion.IssuePersonID = user.ID;
                    opinion.IssuePersonName = user.DisplayName;

                    if (originalActivity != null)
                    {
                        IUser delegator = originalActivity.Assignees.FindDelegator((IUser)user.ToOguObject());

                        opinion.IssuePersonID = delegator.ID;
                        opinion.IssuePersonName = delegator.DisplayName;
                    }

                    if (delegatedUser == null)
                        delegatedUser = user;

                    opinion.AppendPersonID = delegatedUser.ID;
                    opinion.AppendPersonName = delegatedUser.DisplayName;
                }
            }
        }

        public WfClientAuthorizationInfo GetAuthorizationInfo(IWfProcess process, string originalActivityID, string userID)
        {
            WfClientAuthorizationInfo result = new WfClientAuthorizationInfo();

            result.InMoveToMode = GetInMoveToMode(process, originalActivityID, userID);
            result.IsProcessAdmin = GetIsProcessAdmin(process, userID);
            result.IsProcessViewer = GetIsProcessViewer(process, userID);
            result.IsInAcl = process.IsUserInAcl(new OguUser(userID));
            result.OriginalActivityID = originalActivityID;
            result.UserID = userID;

            return result;
        }

        private static bool GetInMoveToMode(IWfProcess process, string originalActivityID, string userID)
        {
            bool result = false;

            IWfActivity currentActivity = process.Activities[originalActivityID];

            if (currentActivity != null)
            {
                //锁判断
                //result = this.LockResult == null || this.LockResult.Succeed;
                result = process.Status == WfProcessStatus.Running
                                && currentActivity.Status == WfActivityStatus.Running;

                if (result)
                    result = IsUserInAssignees(userID, currentActivity.Assignees);
            }

            return result;
        }

        private static bool IsUserInAssignees(string userID, WfAssigneeCollection currentActivityAssignees)
        {
            return currentActivityAssignees.Exists(a => string.Compare(a.User.ID, userID, true) == 0);
            //bool result = false;

            //IList<WfAssignee> assignees = currentActivityAssignees.FindAll(a => a.User.ID == userID);

            //if (assignees.Count > 0)
            //{
            //    int urlEmptyCount = 0;

            //    foreach (WfAssignee assignee in assignees)
            //    {
            //        if (assignee.Url.IsNullOrEmpty())
            //        {
            //            urlEmptyCount++;
            //        }
            //        else
            //        { 
            //            if (AssigneeUriIsSameAsEntryUri(new Uri(assignee.Url, UriKind.RelativeOrAbsolute)))
            //            {
            //                result = true;
            //                break;
            //            }
            //        }
            //    }

            //    //如果所有assignees的url都是空，则不比较url了。
            //    if (result == false && urlEmptyCount == assignees.Count)
            //        result = true;
            //}

            //return result;
        }

        private static bool GetIsProcessAdmin(IWfProcess process, string userID)
        {
            bool result = false;

            if (userID.IsNotEmpty())
            {
                OguUser user = new OguUser(userID);

                result = RolesDefineConfig.GetConfig().IsCurrentUserInRoles(user, "ProcessAdmin");

                if (result == false)
                    result = WfApplicationAuthAdapter.Instance.GetUserApplicationAuthInfo(user).Contains(
                        process.Descriptor.ApplicationName, process.Descriptor.ProgramName, WfApplicationAuthType.FormAdmin);
            }

            return result;
        }

        /// <summary>
        /// 是否是流程的查看者。本方法仅返回流程分类授权的信息，即使是流程环节中的人，也可能返回为False
        /// </summary>
        /// <param name="process"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        private static bool GetIsProcessViewer(IWfProcess process, string userID)
        {
            bool result = false;

            if (userID.IsNotEmpty())
            {
                OguUser user = new OguUser(userID);

                result = WfApplicationAuthAdapter.Instance.GetUserApplicationAuthInfo(user).Contains(
                        process.Descriptor.ApplicationName, process.Descriptor.ProgramName, WfApplicationAuthType.FormViewer);
            }

            return result;
        }

        public void ServerToClient(IWfProcess process, WfClientProcessInfoBase client)
        {
            client.ID = process.ID;
            client.ProcessDescriptorKey = process.Descriptor.Key;

            if (process.CurrentActivity != null)
                client.CurrentActivityKey = process.CurrentActivity.Descriptor.Key;

            client.CanWithdraw = process.CanWithdraw;
            client.RelativeID = process.RelativeID;
            client.RelativeUrl = process.RelativeURL;
            client.ResourceID = process.ResourceID;
            client.SearchID = process.SearchID;
            client.StartTime = process.StartTime;
            client.EndTime = process.EndTime;
            client.Creator = (WfClientUser)process.Creator.ToClientOguObject();
            client.OwnerDepartment = (WfClientOrganization)process.OwnerDepartment.ToClientOguObject();
            client.Status = process.Status.ToClientProcessStatus();
            client.Committed = process.Committed;
            client.RuntimeProcessName = process.RuntimeProcessName;
            client.UpdateTag = process.UpdateTag;

            if (process.HasParentProcess)
            {
                client.OwnerActivityID = process.EntryInfo.OwnerActivity.ID;
                client.OwnerTemplateKey = process.EntryInfo.ProcessTemplate.Key;
            }

            if ((this.Filter & WfClientProcessInfoFilter.ProcessContext) != WfClientProcessInfoFilter.InstanceOnly)
            {
                Dictionary<string, object> processContext = client.ProcessContext;

                process.Context.ForEach(kp => processContext[kp.Key] = kp.Value);
            }

            if ((this.Filter & WfClientProcessInfoFilter.ApplicationRuntimeParameters) != WfClientProcessInfoFilter.InstanceOnly)
            {
                WfClientDictionaryConverter.Instance.ServerToClient(process.ApplicationRuntimeParameters, client.ApplicationRuntimeParameters);
            }

            //增加主线活动信息
            WfClientMainStreamActivityDescriptorCollectionConverter.Instance.ServerToClient(process.GetMainStreamActivities(false), client.MainStreamActivityDescriptors);
        }
    }
}
