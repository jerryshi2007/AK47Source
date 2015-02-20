using System;
using System.Text;
using System.Collections.Generic;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 与流程相关的对话框的参数基类
    /// </summary>
    [Serializable]
    public class WfProcessDailogControlParams : DialogControlParamsBase
    {
        private string processID = string.Empty;
        private string resourceID = string.Empty;

        /// <summary>
        /// 构造方法
        /// </summary>
        public WfProcessDailogControlParams()
        {
        }

        /// <summary>
        /// 流程ID
        /// </summary>
        public string ProcessID
        {
            get
            {
                return this.processID;
            }
            set
            {
                this.processID = value;
            }
        }

        /// <summary>
        /// 资源ID
        /// </summary>
        public string ResourceID
        {
            get
            {
                return this.resourceID;
            }
            set
            {
                this.resourceID = value;
            }
        }

        /// <summary>
        /// 从url中提取resourceID和processID参数
        /// </summary>
        public override void LoadDataFromQueryString()
        {
            this.resourceID = WebUtility.GetRequestQueryValue("resourceID", GetRuntimeResourceID());
            this.processID = WebUtility.GetRequestQueryValue("processID", GetRuntimeProcessID());

            base.LoadDataFromQueryString();
        }

        /// <summary>
        /// 得到运行时的resourceID。如果不能从参数中获取，就通过ProcessContext来获取
        /// </summary>
        /// <returns></returns>
        public string GetRuntimeResourceID()
        {
            string result = this.resourceID;

            try
            {
                //if (string.IsNullOrEmpty(this.resourceID) && ProcessContext.Current.OriginalActivity != null)     //12-29
                //    result = ProcessContext.Current.OriginalActivity.Process.ResourceID;                          //12-29
                if (string.IsNullOrEmpty(this.resourceID) && WfClientContext.Current.OriginalActivity != null)
                    result = WfClientContext.Current.OriginalActivity.Process.ResourceID;
            }
            catch (System.Exception)
            {
            }

            return result;
        }

        /// <summary>
        /// 得到运行时的processID。如果不能从参数中获取，就通过ProcessContext来获取
        /// </summary>
        /// <returns></returns>
        public string GetRuntimeProcessID()
        {
            string result = this.processID;

            try
            {
                //if (string.IsNullOrEmpty(this.processID) && ProcessContext.Current.OriginalActivity != null)   //12-29
                //    result = ProcessContext.Current.OriginalActivity.Process.ID;                               //12-29
                if (string.IsNullOrEmpty(this.processID) && WfClientContext.Current.OriginalActivity != null)
                    result = WfClientContext.Current.OriginalActivity.Process.ID;
            }
            catch (System.Exception)
            {
            }

            return result;
        }

        /// <summary>
        /// 得到流程对象
        /// </summary>
        public IWfProcess Process
        {
            get
            {
                IWfProcess process = null;

                string processID = GetRuntimeProcessID();

                if (string.IsNullOrEmpty(processID))
                {
                    //if (ProcessContext.Current.OriginalActivity != null)            //12-29
                    //    process = ProcessContext.Current.OriginalActivity.Process;  //12-29
                    if (WfClientContext.Current.OriginalActivity != null)
                        process = WfClientContext.Current.OriginalActivity.Process;
                }
                else
                {
                    try
                    {
                        //process = WfRuntime.GetWfProcesses(processID)[processID]; //12-29
                        process = WfRuntime.GetProcessByProcessID(processID);
                    }
                    catch (System.Exception)
                    {
                    }
                }

                return process;
            }
        }

        /// <summary>
        /// 构造弹出对话框的参数
        /// </summary>
        /// <param name="strB"></param>
        protected override void BuildRequestParams(StringBuilder strB)
        {
            base.BuildRequestParams(strB);

            AppendNotNullStringParam(strB, "resourceID", GetRuntimeResourceID());
            AppendNotNullStringParam(strB, "processID", GetRuntimeProcessID());
        }
    }
}
