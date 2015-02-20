using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.UI.Control.Controls;
using CIIC.HSR.TSP.WF.UI.Control.ModelBinding;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.Interfaces
{
	/// <summary>
	/// 流转参数上下文
	/// </summary>
	[ModelBinder(typeof(WFMoveToParameterBinder))]
	public class WFMoveToParameter : WFParameterWithResponseBase
	{
		private WFMoveToTargetParameter _Target = null;

		/// <summary>
		/// 是否按默认节点流转
		/// </summary>
		public bool IsDefault { get; set; }

		/// <summary>
		/// 业务表单地址
		/// </summary>
		public string BusinessUrl { get; set; }

		/// <summary>
		/// 待办标题
		/// </summary>
		public string TaskTitle { get; set; }

		public WFMoveToTargetParameter Target
		{
			get
			{
				if (this._Target == null)
					this._Target = new WFMoveToTargetParameter();

				return this._Target;
			}
			set
			{
				this._Target = value;
			}
		}

		/// <summary>
		/// 设置用户上下文
		/// </summary>
		/// <param name="id">用户Id</param>
		/// <param name="name">用户名</param>
		public void SetOperator(string id, string name)
		{
			this.RuntimeContext.Operator.ID = id;
			this.RuntimeContext.Operator.Name = name;
			this.TransferParameter.Operator.ID = id;
			this.TransferParameter.Operator.Name = name;
		}

		/// <summary>
		/// 向流程中加入上下文参数
		/// </summary>
		/// <param name="key">关键字</param>
		/// <param name="value">值</param>
		public void SetParameter(string key, string value)
		{
			this.RuntimeContext.ApplicationRuntimeParameters.Add(key, value);
		}

		/// <summary>
		/// 流转相关的参数，如开始节点、结束节点等
		/// </summary>
		public WfClientTransferParams TransferParameter
		{
			get;
			set;
		}

        /// <summary>
        /// 设置审批人
        /// </summary>
        /// <param name="name">用户名</param>
        public void AddWfClientAssignee(List<WfClientUser> wfClientUserList)
        {
            foreach (var item in wfClientUserList)
            {
                WfClientAssignee wfClientAssigneeTemp = new WfClientAssignee();
                wfClientAssigneeTemp.User = new WfClientUser();
                wfClientAssigneeTemp.User.ID = item.ID;
                wfClientAssigneeTemp.User.Name = item.Name;
                wfClientAssigneeTemp.User.DisplayName = item.DisplayName;

                var userIdList = this.Target.Candidates.Select(x => x.User.ID).ToList();

                if (!userIdList.Contains(wfClientAssigneeTemp.User.ID))
                {
                    this.Target.Candidates.Add(wfClientAssigneeTemp);
                }
            }
        }

		protected override void InternalExecute(ResponseData response)
		{
			this.TransferParameter.FromTransitionDescriptorKey = this.Target.TransitionKey;
			this.TransferParameter.NextActivityDescriptorKey = this.Target.ActivityKey;
			this.TransferParameter.Assignees.CopyFrom(this.Target.Candidates);

            base.DefaultFillEmailCollector();

			response.ProcessInfo =
				WfClientProcessRuntimeServiceProxy.Instance.MoveTo(this.ProcessId, TransferParameter, this.RuntimeContext);
		}
	}
}
