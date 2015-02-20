using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.UI.Control.DefaultActions
{
    /// <summary>
    ///  弹出框HTML
    /// </summary>
    public class WFUIControlCommon
    {
        public static string WriteMvcHtmlString(string id,bool isDropBtn)
        {
            string mvcHtmlStr = @"
                           <div id='{0}' style='display:none'>
                            <div class='panel panel-default'>
                              <div class='panel-heading clearfix panel-title'>
                                  <div class='pull-right'>
                                         <button type='button' class='btn btn-default' id='bthSelect{2}'  ><b class='glyphicon glyphicon-ok'></b>确定</button>
                                         <button type='button' class='btn btn-default' onclick='closeWindowMoveTo{2}()'><b class='glyphicon glyphicon-remove'></b>取消</button>
                                  </div>
                              </div>
                               <div  class='panel-body' id='{1}' style='margin-left:5px'>
                               </div>
                             </div>
                            </div><script>
	                        jQuery(function(){{jQuery('#{0}').kendoWindow({{'modal':false,'iframe':false,'draggable':true,'pinned':false,'title':'','resizable':false,'content':null,'width':350,'height':170,'actions':['Close']}});
                            $('#bthSelect{2}').click(function(){{
                                 $.fn.HSR.Controls.WFOpenWindow.WFCandidatesSelect('{3}','{1}','{0}');
                            }});      
                            }});
                            function closeWindowMoveTo{2}() {{parent.$('#{0}').data('kendoWindow').close();}}
                        </script>";

            return string.Format(mvcHtmlStr, id + "MoveToWindow", id + "InnerMoveToWindow", id, isDropBtn.ToString());
        }

        public static void SetSelectCandidates(WFStartWorkflowParameter param, WFUIRuntimeContext runtime, string templateKey, Dictionary<string, List<WfClientUser>> DictionaryWfClientUser)
        {

            WfClientProcessStartupParams clientStartupParams = new WfClientProcessStartupParams();

            clientStartupParams.ResourceID = System.Guid.NewGuid().ToString();
            clientStartupParams.ProcessDescriptorKey = templateKey;
            clientStartupParams.Creator = runtime.CurrentUser;
            clientStartupParams.AutoPersist = false;
            WfClientProcessInfo processInfo = WfClientProcessRuntimeServiceProxy.Instance.StartWorkflow(clientStartupParams);

            bool IsAllow = processInfo.CurrentActivity.Descriptor.Properties.GetValue("AllowSelectCandidates", false);
            bool IsMulti = processInfo.CurrentActivity.Descriptor.Properties.GetValue("AllowAssignToMultiUsers", true);
            int tempCandidatesCnt = 0;
            var nextActivity = processInfo.NextActivities.FirstOrDefault();

            if (IsAllow && null != nextActivity)
            {
                string codeName = nextActivity.Transition.Properties.GetValue("CodeName", nextActivity.Transition.Name);
                string compareName = string.IsNullOrEmpty(codeName) ? nextActivity.Transition.Key : codeName;

                nextActivity.Activity.Candidates.ForEach(assignee =>
                {
                    param.Target.Candidates.Add(assignee);
                    tempCandidatesCnt = tempCandidatesCnt + 1;
                });

                if (DictionaryWfClientUser != null)
                {
                    foreach (var key in DictionaryWfClientUser.Keys)
                    {
                        if (compareName == key)
                        {
                            List<WfClientUser> wfUserList = DictionaryWfClientUser[key];
                            foreach (var item in wfUserList)
                            {
                                WfClientAssignee assignee = new WfClientAssignee();
                                assignee.User = new WfClientUser();
                                assignee.User.ID = item.ID;
                                assignee.User.Name = item.Name;
                                assignee.User.DisplayName = item.DisplayName;
                                assignee.Selected = true;
                                param.Target.Candidates.Add(assignee);
                                tempCandidatesCnt = tempCandidatesCnt + 1;
                            }
                        }
                    }
                }
            }
            if (tempCandidatesCnt > 1 && IsAllow)
            {
                param.IsSelectCandidates = true;
                //单选还是多选
                if (IsMulti)
                    param.IsAssignToMultiUsers = true;
            }

        }

        public static int AddWfClientUser<T>(Dictionary<string, List<WfClientUser>> DictionaryWfClientUser, T param, int tempCandidatesCnt) where T : WFMoveToTargetParameter
        {
            string compareName = param.ActionResult;

            if (DictionaryWfClientUser != null)
            {
                foreach (var key in DictionaryWfClientUser.Keys)
                {
                    if (compareName == key)
                    {
                        List<WfClientUser> wfUserList = DictionaryWfClientUser[key];
                        foreach (var item in wfUserList)
                        {
                            WfClientAssignee assignee = new WfClientAssignee();
                            assignee.User = new WfClientUser();
                            assignee.User.ID = item.ID;
                            assignee.User.Name = item.Name;
                            assignee.User.DisplayName = item.DisplayName;
                            assignee.Selected = true;
                            param.Candidates.Add(assignee);
                            tempCandidatesCnt = tempCandidatesCnt + 1;
                        }
                    }
                }
            }
            return tempCandidatesCnt;
        }

        public static string GetCurrentOpinionId(WFUIRuntimeContext runtime)
        {
            WfClientOpinion clientOpinion = null;
            string clientOpinionId = "";

            if (runtime != null && runtime.Process != null && runtime.Process.CurrentOpinion != null)
            {
                clientOpinion = runtime.Process.CurrentOpinion;
                clientOpinionId = clientOpinion.ID;
            }

            return clientOpinionId;
        }

    }
}
