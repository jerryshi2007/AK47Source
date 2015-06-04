using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects.Workflow.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 
    /// </summary>
    public static class WfRuntime
    {
        /// <summary>
        /// 运行时的一些参数，上下文有效
        /// </summary>
        public static WfRuntimeParameters Parameters
        {
            get
            {
                object parameters = null;

                parameters = ObjectContextCache.Instance.GetOrAddNewValue("RuntimeParameters", (cache, key) =>
                {
                    parameters = new WfRuntimeParameters();
                    cache.Add("RuntimeParameters", parameters);

                    return parameters;
                });

                return (WfRuntimeParameters)parameters;
            }
        }

        public static WfProcessActionContext ProcessContext
        {
            get
            {
                object processContext = null;

                processContext = ObjectContextCache.Instance.GetOrAddNewValue("ProcessContext", (cache, key) =>
                {
                    processContext = new WfProcessActionContext();
                    cache.Add("ProcessContext", processContext);

                    return processContext;
                });

                return (WfProcessActionContext)processContext;
            }
        }

        public static IWfProcess StartWorkflow(WfProcessStartupParams startupParams)
        {
            startupParams.NullCheck<WfRuntimeException>("startupParams");

            WfProcess process = new WfProcess(startupParams.ProcessDescriptor);

            FillProcessDescriptorProperties(startupParams, process.Descriptor);
            FillProcessInstanceProperties(startupParams, process);

            WfProcessContextCache.Instance.Add(process.ID, process);

            if (process.Creator == null && DeluxePrincipal.IsAuthenticated)
                process.Creator = DeluxeIdentity.CurrentUser;

            if (process.InitialActivity != null)
            {
                if (process.InitialActivity.Descriptor.Properties.GetValue("AutoGenerateCadidates", true))
                {
                    process.InitialActivity.GenerateCandidatesFromResources();

                    WfMatrix matrix = process.GetMatrix();

                    if (matrix != null)
                        ((WfActivityBase)process.InitialActivity).GenerateCandidatesFromMatrix(matrix);
                }

                if (startupParams.CheckStartProcessUserPermission)
                    CheckStartProcessUserPermission(process);

                if (startupParams.Assignees.Count == 0)
                {
                    startupParams.Assignees.CopyFrom(process.InitialActivity.Candidates);
                }

                if (process.InitialActivity.Descriptor.Resources.Count == 0)
                {
                    startupParams.Assignees.ToUsers().ForEach(u => process.InitialActivity.Descriptor.Resources.Add(new WfUserResourceDescriptor(u)));
                }

                WfSimulator.WriteSimulationInfo(process, WfSimulationOperationType.Startup);

                //如果自动启动第一个活动且存在活动点，则流转到第一个点
                if (startupParams.AutoStartInitialActivity)
                {
                    WfAssigneeCollection assignees = startupParams.Assignees;
                    process.InitialActivity.Candidates.Clear();
                    process.InitialActivity.Candidates.CopyFrom(startupParams.Assignees);

                    IWfActivity originalInitial = process.InitialActivity;

                    WfRuntime.DecorateProcess(process);

                    //修饰流程后，如果起始点发生了变化...
                    if (originalInitial != process.InitialActivity)
                        assignees = process.InitialActivity.Candidates;

                    WfTransferParams transferParams = new WfTransferParams(process.InitialActivity.Descriptor);

                    //设置初始节点子流程参数
                    process.InitialActivity.Descriptor.BranchProcessTemplates.ForEach(branchTemplate =>
                    {
                        transferParams.BranchTransferParams.Add(new WfBranchProcessTransferParams(branchTemplate));
                    });

                    transferParams.Operator = startupParams.Creator;
                    transferParams.Assignees.CopyFrom(assignees);
                    process.MoveTo(transferParams);

                    WfRuntime.ProcessContext.NormalizeTaskTitles();
                }
            }

            WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(process);

            return process;
        }

        /// <summary>
        /// 调用修饰器，修饰流程。例如添加秘书环节
        /// </summary>
        /// <param name="process"></param>
        public static void DecorateProcess(IWfProcess process)
        {
            process.NullCheck("process");

            WfDecoratorSettings.GetConfig().GetDecorators().Decorate(process);
        }

        private static void FillProcessDescriptorProperties(WfProcessStartupParams startupParams, IWfProcessDescriptor processDesp)
        {
            if (startupParams.DefaultTaskTitle.IsNotEmpty())
                processDesp.DefaultTaskTitle = startupParams.DefaultTaskTitle;

            if (startupParams.DefaultUrl.IsNotEmpty())
                processDesp.Url = startupParams.DefaultUrl;

            if (startupParams.RuntimeProcessName.IsNotEmpty() && processDesp.Properties.ContainsKey("RuntimeProcessName"))
                processDesp.Properties.SetValue("RuntimeProcessName", startupParams.RuntimeProcessName);
        }

        private static void FillProcessInstanceProperties(WfProcessStartupParams startupParams, WfProcess process)
        {
            process.ResourceID = startupParams.ResourceID;
            process.RelativeParams.CopyFrom(startupParams.RelativeParams);
            process.RelativeID = startupParams.RelativeID;
            process.RelativeURL = startupParams.RelativeURL;
            process.BranchStartupParams = startupParams.BranchStartupParams;
            process.EntryInfo = startupParams.Group;
            process.OwnerActivityID = startupParams.OwnerActivityID;
            process.OwnerTemplateKey = startupParams.OwnerTemplateKey;
            process.Sequence = startupParams.Sequence;
            process.ApplicationRuntimeParameters.CopyFrom(startupParams.ApplicationRuntimeParameters);
            process.Committed = startupParams.AutoCommit;

            process.OwnerDepartment = startupParams.Department;
            process.Creator = startupParams.Creator;
        }

        private static void CheckStartProcessUserPermission(IWfProcess process)
        {
            if (process.EntryInfo == null &&
                WfRuntime.ProcessContext.EnableSimulation == false)
            {
                //看看是否使用起始点的指派人是否限为流程启动人
                if (process.Descriptor.Properties.GetValue("OnlyInitialResourcesCanStartProcess", false))
                {
                    bool allowed = DeluxePrincipal.IsAuthenticated;

                    if (allowed)
                        allowed = process.InitialActivity.Candidates.Contains(DeluxeIdentity.CurrentUser);

                    allowed.FalseThrow(Translator.Translate(Define.DefaultCulture, "只有流程起始活动的指派人才能启动流程"));
                }
                else
                {
                    if (process.Descriptor.ProcessStarters.Count > 0)
                    {
                        OguDataCollection<IUser> starters = process.Descriptor.ProcessStarters.ToUsers();

                        (starters.FindSingleObjectByID(DeluxeIdentity.CurrentUser.ID) != null).FalseThrow(
                            Translator.Translate(Define.DefaultCulture, "只有被允许启动流程的人才能启动流程"));
                    }
                }
            }
        }

        /// <summary>
        /// 根据流程ID得到流程
        /// </summary>
        /// <param name="processID"></param>
        /// <returns></returns>
        public static IWfProcess GetProcessByProcessID(string processID)
        {
            processID.NullCheck<WfRuntimeException>("processID");

            IWfProcess result = WfProcessContextCache.Instance.GetOrAddNewValue(processID, (cache, key) =>
            {
                IWfProcess process = null;

                PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(string.Format("Load Process({0})", processID), () =>
                    {
                        IWfProcessPersistManager persistManager = WorkflowSettings.GetConfig().GetPersistManager();

                        process = persistManager.LoadProcessByProcessID(key);

                        cache.Add(key, process);
                    });

                return process;
            });

            return result;
        }

        /// <summary>
        /// 得到某个活动的子流程。如果缓存中存在，则使用缓存中的流程
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="templateKey"></param>
        /// <returns></returns>
        public static WfProcessCollection GetProcessByOwnerActivityID(string activityID, string templateKey)
        {
            activityID.CheckStringIsNullOrEmpty("activityID");
            templateKey.CheckStringIsNullOrEmpty("templateKey");

            WfProcessCollection queryResult = null;

            PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(string.Format("LoadProcessByOwnerActivityID({0}-{1})", activityID, templateKey), () =>
                queryResult = WorkflowSettings.GetConfig().GetPersistManager().LoadProcessByOwnerActivityID(activityID, templateKey)
            );

            WfProcessCollection result = new WfProcessCollection();

            foreach (IWfProcess process in queryResult)
            {
                //如果缓存中存在，则使用缓存的数据，如果缓存中不存在，则使用查询到的结果
                IWfProcess processNeedToAdd = WfProcessContextCache.Instance.GetOrAddNewValue(process.ID, (cache, key) =>
                {
                    cache.Add(key, process);

                    return process;
                });

                result.Add(processNeedToAdd);
            }

            return result;
        }

        /// <summary>
        /// 得到某个活动的子流程的状态信息，没有流程实例，只是状态信息
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="templateKey"></param>
        /// <param name="includeAborted">是否包含已经作废的流程</param>
        /// <returns></returns>
        public static WfProcessCurrentInfoCollection GetProcessStatusByOwnerActivityID(string activityID, string templateKey, bool includeAborted)
        {
            activityID.CheckStringIsNullOrEmpty("activityID");

            WfProcessCurrentInfoCollection queryResult = null;

            PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(string.Format("LoadProcessByOwnerActivityID({0}-{1})", activityID, templateKey), () =>
                queryResult = WorkflowSettings.GetConfig().GetPersistManager().LoadProcessInfoOwnerActivityID(activityID, templateKey, includeAborted)
            );

            return queryResult;
        }

        public static void ProcessPendingActivity(WfPendingActivityInfo pendingActivityInfo)
        {
            pendingActivityInfo.NullCheck("pendingActivityInfo");

            IWfProcess process = GetProcessByActivityID(pendingActivityInfo.ActivityID);
            bool needToDeletePendingInfo = true;

            if (process != null)
            {
                IWfActivity currentActivity = process.CurrentActivity;

                process.ProcessPendingActivity();

                if (currentActivity != null)
                {
                    needToDeletePendingInfo = false;

                    //已经不存在Pending状态
                    if (currentActivity.Status != WfActivityStatus.Pending)
                    {
                        WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(process);

                        using (TransactionScope scope = TransactionScopeFactory.Create())
                        {
                            WfRuntime.PersistWorkflows();
                            WfPendingActivityInfoAdapter.Instance.Delete(pendingActivityInfo);

                            scope.Complete();
                        }
                    }
                }
            }

            if (needToDeletePendingInfo)
            {
                WfPendingActivityInfoAdapter.Instance.Delete(pendingActivityInfo);
            }
        }

        /// <summary>
        /// 根据ActivityID得到流程
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public static IWfProcess GetProcessByActivityID(string activityID)
        {
            activityID.NullCheck<WfRuntimeException>("activityID");

            IWfProcess process = null;

            foreach (KeyValuePair<string, IWfProcess> kp in WfProcessContextCache.Instance)
            {
                if (kp.Value.Activities.ContainsKey(activityID))
                {
                    process = kp.Value;
                    break;
                }
            }

            if (process == null)
            {
                PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(string.Format("LoadProcessByActivityID({0})", activityID), () =>
                    process = WorkflowSettings.GetConfig().GetPersistManager().LoadProcessByActivityID(activityID)
                );

                WfProcessContextCache.Instance.Add(process.ID, process);
            }

            return process;
        }

        /// <summary>
        /// 根据ResourceID得到流程
        /// </summary>
        /// <param name="resourceID"></param>
        /// <returns></returns>
        public static WfProcessCollection GetProcessByResourceID(string resourceID)
        {
            resourceID.NullCheck<WfRuntimeException>("resourceID");

            WfProcessCollection processes = new WfProcessCollection();

            foreach (KeyValuePair<string, IWfProcess> kp in WfProcessContextCache.Instance)
            {
                if (string.Compare(kp.Value.ResourceID, resourceID, true) == 0)
                {
                    processes.Add(kp.Value);
                    break;
                }
            }

            WfProcessCollection persistedProcesses = null;

            PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(string.Format("LoadProcessByResourceID({0})", resourceID), () =>
                persistedProcesses = WorkflowSettings.GetConfig().GetPersistManager().LoadProcessByResourceID(resourceID)
            );

            persistedProcesses.ForEach(p =>
            {
                if (processes.ContainsKey(p.ID) == false)
                    processes.Add(p);

                WfProcessContextCache.Instance[p.ID] = p;
            });

            return processes;
        }

        /// <summary>
        /// 根据流程ID删除流程
        /// </summary>
        /// <param name="processID"></param>
        public static void DeleteProcessByProcessID(string processID)
        {
            WorkflowSettings.GetConfig().GetPersistManager().DeleteProcessByProcessID(processID);
        }

        /// <summary>
        /// 根据ActivityID删除流程
        /// </summary>
        /// <param name="activityID"></param>
        public static void DeleteProcessByActivityID(string activityID)
        {
            WorkflowSettings.GetConfig().GetPersistManager().DeleteProcessByActivityID(activityID);
        }

        /// <summary>
        /// 根据ResourceID删除流程
        /// </summary>
        /// <param name="resourceID"></param>
        public static void DeleteProcessByResourceID(string resourceID)
        {
            WorkflowSettings.GetConfig().GetPersistManager().DeleteProcessByResourceID(resourceID);
        }

        /// <summary>
        /// 保存一个流程的数据，保存成功后，清除所有的缓存
        /// </summary>
        /// <param name="process"></param>
        public static void PersistWorkflows()
        {
            IWfProcessPersistManager persistManager = WorkflowSettings.GetConfig().GetPersistManager();

            WfActionParams actionParams = new WfActionParams(WfRuntime.ProcessContext);

            WfRuntime.ProcessContext.Acl.Distinct((a1, a2) => string.Compare(a1.ResourceID, a2.ResourceID, true) == 0 &&
                    string.Compare(a1.ObjectID, a2.ObjectID, true) == 0);

            PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("PersistWorkflows", () =>
                {
                    CommonInfoMappingCollection cimItems = new CommonInfoMappingCollection();

                    cimItems.FromProcesses(WfRuntime.ProcessContext.AffectedProcesses);

                    using (TransactionScope scope = TransactionScopeFactory.Create())
                    {
                        PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("DeletePendingActivities",
                            () => WfPendingActivityInfoAdapter.Instance.Delete(WfRuntime.ProcessContext.ReleasedPendingActivities));

                        PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("Update Acl",
                            () => WfAclAdapter.Instance.Update(WfRuntime.ProcessContext.Acl));

                        PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("Update CommonInfoMapping",
                            () => CommonInfoMappingAdapter.Instance.Update(cimItems));

                        //保存流程后再执行Actions
                        PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("PersistActions",
                            () => WfRuntime.ProcessContext.AffectedActions.PersistActions(actionParams));

                        ProcessProgress.Current.MaxStep += WfRuntime.ProcessContext.AffectedProcesses.Count;
                        ProcessProgress.Current.Response();

                        int i = 0;
                        int total = WfRuntime.ProcessContext.AffectedProcesses.Count;

                        WfRuntime.ProcessContext.AffectedProcesses.ForEach(process =>
                            {
                                PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(
                                    string.Format("PersistWorkflow: {0}", process.ID),
                                    () => persistManager.SaveProcess(process));

                                i++;
                                ProcessProgress.Current.Increment();

                                ProcessProgress.Current.StatusText = Translator.Translate(Define.DefaultCulture, "保存了{0}/{1}条流程数据", i, total);
                                ProcessProgress.Current.Response();
                            });

                        //保存流程后再执行Actions
                        PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("AfterWorkflowPersistAction",
                            () => WfRuntime.ProcessContext.AffectedActions.AfterWorkflowPersistAction(actionParams));

                        ProcessProgress.Current.StatusText = string.Empty;
                        ProcessProgress.Current.Response();

                        scope.Complete();
                    }
                });

            WfRuntime.ClearCache();
        }

        /// <summary>
        /// 清除流程上下文中的缓存
        /// </summary>
        public static void ClearCache()
        {
            WfRuntime.ProcessContext.Clear();
            WfProcessContextCache.Instance.Clear();
        }
    }
}
