using System;
using System.Text;
using System.Collections.Generic;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// ��������صĶԻ���Ĳ�������
    /// </summary>
    [Serializable]
    public class WfProcessDailogControlParams : DialogControlParamsBase
    {
        private string processID = string.Empty;
        private string resourceID = string.Empty;

        /// <summary>
        /// ���췽��
        /// </summary>
        public WfProcessDailogControlParams()
        {
        }

        /// <summary>
        /// ����ID
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
        /// ��ԴID
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
        /// ��url����ȡresourceID��processID����
        /// </summary>
        public override void LoadDataFromQueryString()
        {
            this.resourceID = WebUtility.GetRequestQueryValue("resourceID", GetRuntimeResourceID());
            this.processID = WebUtility.GetRequestQueryValue("processID", GetRuntimeProcessID());

            base.LoadDataFromQueryString();
        }

        /// <summary>
        /// �õ�����ʱ��resourceID��������ܴӲ����л�ȡ����ͨ��ProcessContext����ȡ
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
        /// �õ�����ʱ��processID��������ܴӲ����л�ȡ����ͨ��ProcessContext����ȡ
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
        /// �õ����̶���
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
        /// ���쵯���Ի���Ĳ���
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
