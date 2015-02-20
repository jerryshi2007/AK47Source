using CIIC.HSR.TSP.WebComponents.Extensions;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Proxies;
using Newtonsoft.Json;
using Kendo.Mvc.UI;
using System.Web;
using CIIC.HSR.TSP.DataAccess;
namespace CIIC.HSR.TSP.WF.UI.Control.DefaultActions
{
    /// <summary>
    /// 流程行为的默认实现
    /// </summary>
    public class WFDefaultOperationController : Controller
    {
        /// <summary>
        /// 启动流程
        /// </summary>
        /// <param name="paras">参数</param>
        /// <returns>流程信息</returns>
        [HttpPost]
        public ActionResult StartWorkflow(WFStartWorkflowParameter param)
        {
            return this.JsonExecute(() =>
            {
                return Json(param.Execute());
            });
        }

        /// <summary>
        /// 撤回一步
        /// </summary>
        /// <param name="param">参数</param>
        /// <returns>流程信息</returns>
        [HttpPost]
        public ActionResult Withdraw(WFWithdrawParameter param)
        {
            return this.JsonExecute(() =>
            {
                return Json(param.Execute());
            });
        }

        /// <summary>
        /// 废弃流程
        /// </summary>
        /// <param name="paras">参数</param>
        /// <returns>流程信息</returns>
        [HttpPost]
        public ActionResult Cancel(WFCancelParameter param)
        {
            return this.JsonExecute(() =>
            {
                return Json(param.Execute());
            });
        }

        [HttpPost]
        public ActionResult MoveToDefault(WFMoveToParameter param)
        {
            return this.JsonExecute(() =>
            {
                return Json(param.Execute());
            });
        }

        /// <summary>
        /// 暂停流程
        /// </summary>
        /// <param name="paras">参数</param>
        /// <returns>流程信息</returns>
        [HttpPost]
        public ActionResult Pause(WFPauseParameter param)
        {
            return this.JsonExecute(() =>
            {
                return Json(param.Execute());
            });
        }

        /// <summary>
        /// 恢复作废的流程
        /// </summary>
        /// <param name="paras">参数</param>
        /// <returns>流程信息</returns>
        [HttpPost]
        public ActionResult Restore(WFRestoreParameter param)
        {
            return this.JsonExecute(() =>
            {
                return Json(param.Execute());
            });
        }

        /// <summary>
        /// 继续暂停的流程
        /// </summary>
        /// <param name="paras">参数</param>
        /// <returns>流程信息</returns>
        [HttpPost]
        public ActionResult Resume(WFResumeParameter param)
        {
            return this.JsonExecute(() =>
            {
                return Json(param.Execute());
            });
        }

        /// <summary>
        /// 保存工作流程
        /// </summary>
        /// <param name="paras">参数</param>
        /// <returns>流程信息</returns>
        [HttpPost]
        public ActionResult Save(WFSaveParameter param)
        {
            return this.JsonExecute(() =>
            {
                return Json(param.Execute());
            });
        }

        /// <summary>
        /// 更新流程相关的参数
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateProcess(WFUpdateProcessParameter param)
        {
            return this.JsonExecute(() =>
            {
                return Json(param.Execute());
            });
        }

        /// <summary>
        /// 保存工作流程
        /// </summary>
        /// <param name="paras">参数</param>
        /// <returns>流程信息</returns>
        [HttpPost]
        public ActionResult OpinionGridList([DataSourceRequest]DataSourceRequest request, string queryConditionJsonData)
        {
            WFOpinionQueryCondition param = JsonConvert.DeserializeObject<WFOpinionQueryCondition>(HttpUtility.HtmlDecode(queryConditionJsonData));
            //查询意见
            List<WfClientOpinion> wfClientOpinionList = GetOpinionComments(param.ProcessID,
                                                                           param.ResourceID,
                                                                           param.ActivityID,
                                                                           param.InMoveToModel,
                                                                           param.IssueTime);

            int pageCnt = wfClientOpinionList.Count;
            int currentPage = request.Page;
            int pageSize=request.PageSize;
            //排序
            wfClientOpinionList = wfClientOpinionList.OrderByDescending(p => p.IssueTime).ToList();
            //分页实现
            if (pageSize <= wfClientOpinionList.Count)
            {
                //分页  
                wfClientOpinionList = wfClientOpinionList.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            }
            //获取审核结果
            foreach (var item in wfClientOpinionList)
            {
                if (item.Content == "{{**string.Empty**}}")
                {
                    item.Content = "";
                }
                item.ExtraData = item.GetNextSteps().GetSelectedStep().GetDescription();
            }
            PagedCollection<WfClientOpinion> dictList = new PagedCollection<WfClientOpinion>();
            dictList.Items = wfClientOpinionList;
            DataSourceResult dsResult = dictList.Items.ToDataSourceResultResetPage(request);
            dsResult.Total = pageCnt;

            return Json(dsResult, JsonRequestBehavior.AllowGet);
        }

        private List<WfClientOpinion> GetOpinionComments(string currentProcessId, string currentResourceId,
                                                          string currentActivityId, bool inMoveToModel, DateTime? IssueTime)
        {
            WfClientOpinionCollection opinionCollection = null;

            List<WfClientOpinion> wfClientOpinionList = new List<WfClientOpinion>();
            //根据ResourceId获取意见信息
            if (!string.IsNullOrEmpty(currentResourceId))
            {
                opinionCollection = WfClientProcessRuntimeServiceProxy.Instance.GetOpinionsByResourceID(currentResourceId);
            }
            else
            {
                //根据ProcessId获取意见信息
                opinionCollection = WfClientProcessRuntimeServiceProxy.Instance.GetOpinionsByProcessID(currentProcessId);
            }

            if (inMoveToModel && IssueTime!=null)
            {
                var searchResult = from q in opinionCollection select q;
                searchResult = searchResult.Where(p => p.IssueTime != IssueTime);
                wfClientOpinionList = searchResult.ToList();
            }
            else
            {
                wfClientOpinionList = opinionCollection.ToList();
            }

            return wfClientOpinionList;
        }

    }
}
