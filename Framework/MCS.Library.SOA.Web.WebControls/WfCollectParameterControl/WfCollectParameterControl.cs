using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;

namespace MCS.Web.WebControls
{
    [DefaultProperty("AutoCollectDataWhenPostBack")]
    [ToolboxData("<{0}:WfCollectParameterControl runat=server></{0}:WfCollectParameterControl>")]
    public class WfCollectParameterControl : Control, INamingContainer, IWfApplicationRuntimeParametersCollector
    {
        private WfParameterValidationControl innerPageValidator = new WfParameterValidationControl();

        public WfCollectParameterControl()
        {

        }

        #region properties
        [DefaultValue(true)]
        public bool Enabled
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "Enabled", true);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "Enabled", value);
            }
        }

        /// <summary>
        /// PostBack后，是否自动收集数据（不校验）。此操作发生在Load和控件事件(Button Click)之间
        /// </summary>
        [DefaultValue(true)]
        public bool AutoCollectDataWhenPostBack
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "AutoCollectDataWhenPostBack", true);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "AutoCollectDataWhenPostBack", value);
            }
        }

        private ProcessParameterEvalMode _ProcessParameterEvalMode = ProcessParameterEvalMode.ApprovalRootProcess;

        [DefaultValue(ProcessParameterEvalMode.ApprovalRootProcess)]
        public ProcessParameterEvalMode ProcessParameterEvalMode
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "ProcessParameterEvalMode", ProcessParameterEvalMode.ApprovalRootProcess);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "ProcessParameterEvalMode", value);
            }
        }
        #endregion

        /// <summary>
        /// 当前流程
        /// </summary>
        private IWfProcess CurrentProcess
        {
            get
            {
                IWfProcess process = null;

                if (WfClientContext.Current.OriginalActivity != null)
                {
                    switch (this._ProcessParameterEvalMode)
                    {
                        case ProcessParameterEvalMode.CurrentProcess:
                            process = WfClientContext.Current.OriginalActivity.Process;
                            break;
                        case ProcessParameterEvalMode.RootProcess:
                            process = WfClientContext.Current.OriginalActivity.Process.RootProcess;
                            break;
                        case ProcessParameterEvalMode.SameResourceRootProcess:
                            process = WfClientContext.Current.OriginalActivity.Process.SameResourceRootProcess;
                            break;
                        case ProcessParameterEvalMode.ApprovalRootProcess:
                        default:
                            process = WfClientContext.Current.OriginalActivity.Process.ApprovalRootProcess;
                            break;

                    }
                }

                return process;
            }
        }

        #region "override"
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            this.innerPageValidator.CollectParameterControl = this;
            this.Controls.Add(this.innerPageValidator);
        }
        #endregion

        public void CollectApplicationData(MCS.Library.SOA.DataObjects.Workflow.IWfProcess process)
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        /// 收集数据
        /// </summary>
        public void CollectData()
        {
            //WfParameterNeedToBeCollected items = new WfParameterNeedToBeCollected();
            ////ParameterType = MCS.Library.SOA.DataObjects.Workflow.DataType.String,
            ////ParameterType = MCS.Library.SOA.DataObjects.Workflow.DataType.DateTime,
            //items.Add(new WfParameterDescriptor() { AutoCollect = true, ParameterName = "Drtest", ControlID = "testControl", ControlPropertyName = "SelectedValue" });
            //items.Add(new WfParameterDescriptor() { AutoCollect = true, ParameterName = "Calendartest", ControlID = "DeluxeCalendar1", ControlPropertyName = "Value" });

            WfParameterNeedToBeCollected items = this.GetCurrentDescriptorParameters();

            foreach (WfParameterDescriptor wfpd in items)
            {
                if (wfpd.AutoCollect)
                {
                    CollectParameter(wfpd);
                }
            }
        }

        /// <summary>
        /// 获取当前流程所有定义自动收集参数集合
        /// </summary>
        /// <returns></returns>
        private WfParameterNeedToBeCollected GetCurrentDescriptorParameters()
        {
            WfParameterNeedToBeCollected items = new WfParameterNeedToBeCollected();
            items.MergeParameterItems(CurrentProcess.CurrentActivity.Descriptor.ParametersNeedToBeCollected);
            items.MergeParameterItems(CurrentProcess.Descriptor.ParametersNeedToBeCollected);

            return items;
        }

        /*  private void MergeParameterItems(WfParameterNeedToBeCollected items, WfParameterNeedToBeCollected wfParameters)
         {
             foreach (WfParameterDescriptor item in wfParameters)
             {
                 if (!items.ContainsKey(item.ParameterName))
                 {
                     items.Add(item);
                 }
             }
         } */

        private void CollectParameter(WfParameterDescriptor wfpd)
        {
            Control control = WebControlUtility.FindControlByID(this.Page, wfpd.ControlID, true);

            if (control != null)
            {
                //控件对象路径
                string targetPro = string.Empty;
                //控件对象属性
                string targetProName = wfpd.ControlPropertyName;

                DataBindingControl.SplitPath(control, wfpd.ControlPropertyName, out targetPro, out targetProName);

                //获取目标属性值
                object targetItem = DataBindingControl.FindObjectByPath(control, targetPro);

                if (targetItem != null)
                {
                    object targetValue = DataBindingControl.FindObjectByPath(targetItem, targetProName);

                    Type realType = typeof(string);

                    if (wfpd.ParameterType.TryToRealType(out realType))
                    {
						this.CurrentProcess.ApplicationRuntimeParameters[wfpd.ParameterName] = DataConverter.ChangeType(targetValue, realType);
                    }
                }
            }
        }
    }
}
