using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Principal;
using MCS.Library.Globalization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    public abstract class WfTransferParamsBase
    {
        private IWfActivityDescriptor _NextActivityDescriptor = null;
        private IUser _Operator = null;
        private WfAssigneeCollection _Assignees = null;
        private IWfTransitionDescriptor _FromTransitionDescriptor = null;

        private Dictionary<string, object> _Context = null;

        public WfTransferParamsBase()
        {
        }

        public WfTransferParamsBase(IWfActivityDescriptor nextActivityDescriptor)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(nextActivityDescriptor != null, "nextActivityDescriptor");

            _NextActivityDescriptor = nextActivityDescriptor;
        }

        public IWfActivityDescriptor NextActivityDescriptor
        {
            get
            {
                return this._NextActivityDescriptor;
            }
            set
            {
                this._NextActivityDescriptor = value;
            }
        }

        public IUser Operator
        {
            get
            {
                return this._Operator;
            }
            set
            {
                this._Operator = (IUser)OguBase.CreateWrapperObject(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public WfAssigneeCollection Assignees
        {
            get
            {
                if (this._Assignees == null)
                    this._Assignees = new WfAssigneeCollection();

                return _Assignees;
            }
        }

        /// <summary>
        /// 来自于哪里的线的定义
        /// </summary>
        public IWfTransitionDescriptor FromTransitionDescriptor
        {
            get
            {
                return this._FromTransitionDescriptor;
            }
            set
            {
                this._FromTransitionDescriptor = value;
            }
        }

        /// <summary>
        /// 根据流程和上下文
        /// </summary>
        /// <param name="process"></param>
        internal void FillActivityAndTransitionInfoFromContext(IWfProcess process)
        {
            process.NullCheck("process");

            if (this._NextActivityDescriptor == null)
            {
                string nextActivityDescriptorKey = this.Context.GetValue("NextActivityDescriptorKey", string.Empty);

                if (nextActivityDescriptorKey.IsNotEmpty())
                {
                    IWfActivity nextActivity = process.Activities.FindActivityByDescriptorKey(nextActivityDescriptorKey);

                    if (nextActivity != null)
                        this.NextActivityDescriptor = nextActivity.Descriptor;
                }
            }

            if (this._FromTransitionDescriptor == null)
            {
                string fromTransitionDescriptorKey = this.Context.GetValue("FromTransitionDescriptorKey", string.Empty);

                if (fromTransitionDescriptorKey.IsNotEmpty())
                {
                    IWfTransitionDescriptor fromTransition = process.Descriptor.FindTransitionByKey(fromTransitionDescriptorKey);

                    this.FromTransitionDescriptor = fromTransition;
                }
            }
        }

        /// <summary>
        /// 上下文信息，用于存放一些传递过来的上下文信息
        /// </summary>
        public Dictionary<string, object> Context
        {
            get
            {
                if (this._Context == null)
                    this._Context = new Dictionary<string, object>();

                return this._Context;
            }
        }
    }

    /// <summary>
    /// 普通流程的流转参数
    /// </summary>
    public class WfTransferParams : WfTransferParamsBase
    {
        private WfBranchProcessTransferParamsCollection _BranchTransferParams = new WfBranchProcessTransferParamsCollection();

        public WfBranchProcessTransferParamsCollection BranchTransferParams
        {
            get
            {
                return this._BranchTransferParams;
            }
        }

        public WfTransferParams()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextActivityDescriptor"></param>
        public WfTransferParams(IWfActivityDescriptor nextActivityDescriptor)
            : base(nextActivityDescriptor)
        {
        }

        /// <summary>
        /// 根据默认的下一环节生成流转参数
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static WfTransferParams FromNextDefaultActivity(IWfProcess process)
        {
            process.NullCheck("process");

            WfTransferParams transferParams = null;

            if (process.CurrentActivity != null)
                transferParams = FromNextDefaultActivity(process.CurrentActivity.Descriptor);

            (transferParams != null).FalseThrow(Translator.Translate(Define.DefaultCulture, "不能根据流程当前活动找到默认的下一步环节"));

            return transferParams;
        }

        /// <summary>
        /// 根据当前活动定义找到默认的下一环节，生成流转参数
        /// </summary>
        /// <param name="currentActDesp"></param>
        /// <returns></returns>
        public static WfTransferParams FromNextDefaultActivity(IWfActivityDescriptor currentActDesp)
        {
            return FromNextActivity(currentActDesp, (toTransitions) => toTransitions.FindDefaultSelectTransition());
        }

        public static WfTransferParams FromNextActivity(IWfActivityDescriptor currentActDesp, Func<WfTransitionDescriptorCollection, IWfTransitionDescriptor> predicate)
        {
            currentActDesp.NullCheck("actDesp");
            predicate.NullCheck("predicate");

            WfTransferParams transferParams = null;

            WfTransitionDescriptorCollection toTransitions =
                                currentActDesp.ToTransitions.GetAllCanTransitTransitions();

            //找到匹配的线
            IWfTransitionDescriptor transition = predicate(toTransitions);

            if (transition != null)
            {
                transferParams = new WfTransferParams(transition.ToActivity);

                transferParams.FromTransitionDescriptor = transition;

                if (transition.ToActivity.Instance != null)
                    transferParams.Assignees.CopyFrom(transition.ToActivity.Instance.Candidates);

                if (DeluxePrincipal.IsAuthenticated)
                    transferParams.Operator = DeluxeIdentity.CurrentUser;
            }

            (transferParams != null).FalseThrow(Translator.Translate(Define.DefaultCulture, "不能根据活动定义{0}找到符合条件的下一步环节", currentActDesp.Key));

            return transferParams;
        }
    }

    /// <summary>
    /// 分支流程的启动参数
    /// </summary>
    [Serializable]
    [XElementSerializable]
    [XElementFieldSerialize(IgnoreDeserializeError = true)]
    public class WfBranchProcessTransferParams
    {
        private IWfBranchProcessTemplateDescriptor _Template = null;
        private WfBranchProcessStartupParamsCollection _BranchParams = new WfBranchProcessStartupParamsCollection();

        /// <summary>
        /// 构造方法
        /// </summary>
        public WfBranchProcessTransferParams()
        {
        }

        /// <summary>
        /// 构造方法。根据分支流程模板以及其中的资源初始化BranchParams
        /// </summary>
        /// <param name="template"></param>
        public WfBranchProcessTransferParams(IWfBranchProcessTemplateDescriptor template)
        {
            template.NullCheck("template");

            this._Template = template;

            OguDataCollection<IUser> users = PrepareBranchTransferUsers(template);

            if (template.ExecuteSequence == WfBranchProcessExecuteSequence.SerialInSameProcess)
            {
                //串行流程中，仅仅启动一个流程
                this.BranchParams.Add(new WfBranchProcessStartupParams(users));
            }
            else
            {
                this.BranchParams.Add(users);
            }

            if (template.DefaultTaskTitle.IsNotEmpty())
                this.BranchParams.ForEach(b => b.DefaultTaskTitle = template.DefaultTaskTitle);
        }

        /// <summary>
        /// 构造方法。根据分支流程模板以及传递进来的人员初始化BranchParams
        /// </summary>
        /// <param name="template"></param>
        /// <param name="users"></param>
        public WfBranchProcessTransferParams(IWfBranchProcessTemplateDescriptor template, IEnumerable<IUser> users)
        {
            template.NullCheck("template");
            users.NullCheck("users");

            this._Template = template;
            this.BranchParams.Add(users);

            if (template.DefaultTaskTitle.IsNotEmpty())
                this.BranchParams.ForEach(b => b.DefaultTaskTitle = template.DefaultTaskTitle);
        }

        private static OguDataCollection<IUser> PrepareBranchTransferUsers(IWfBranchProcessTemplateDescriptor template)
        {
            OguDataCollection<IUser> users = null;

            bool fetchFirstSubInitUsers = false;

            //如果是串行流程或者Templete的Resource中没有人，则从第一个点取人
            if (template.ExecuteSequence == WfBranchProcessExecuteSequence.SerialInSameProcess)
            {
                fetchFirstSubInitUsers = true;
            }
            else
            {
                users = template.Resources.ToUsers();

                if (template.OperationDefinition == null && users.Count == 0)
                    fetchFirstSubInitUsers = true;
            }

            if (fetchFirstSubInitUsers)
            {
                IWfProcessDescriptor processDesp = template.GetBranchProcessDescriptor();
                users = processDesp.InitialActivity.Resources.ToUsers();
            }

            return users;
        }

        /// <summary>
        /// 分支流程的具体参数
        /// </summary>
        public WfBranchProcessStartupParamsCollection BranchParams
        {
            get { return this._BranchParams; }
        }

        public IWfBranchProcessTemplateDescriptor Template
        {
            get { return this._Template; }
            set { this._Template = value; }
        }
    }

    [Serializable]
    public class WfBranchProcessTransferParamsCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfBranchProcessTransferParams>
    {
        protected override string GetKeyForItem(WfBranchProcessTransferParams branchProcessTransferParams)
        {
            branchProcessTransferParams.NullCheck("branchProcessTransferParams");
            branchProcessTransferParams.Template.NullCheck("branchProcessTransferParams.Template");

            return branchProcessTransferParams.Template.Key;
        }

        public void InitFromTemplates(IEnumerable<IWfBranchProcessTemplateDescriptor> templates)
        {
            this.Clear();

            foreach (IWfBranchProcessTemplateDescriptor template in templates)
            {
                this.Add(new WfBranchProcessTransferParams(template));
            }
        }
    }

}
