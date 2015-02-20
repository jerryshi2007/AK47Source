using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.TextBox;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.Globalization;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using Newtonsoft.Json;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Json.Converters;
using CIIC.HSR.TSP.WF.UI.Control.Controls.StartWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.DefaultActions;
using Kendo.Mvc.UI.Fluent;
using Kendo.Mvc.UI;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.OpinionGridList
{

    public class WFOpinionGridList<TModel> : WFControlBase
    {
        private int defaultHeight = 200;
        private int defaultPageSize = 5;
        private const string defaultOpinionGridListID = "opinionGridList";
        private readonly HtmlHelper<TModel> htmlHelper = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="htmlHelper"></param>
        public WFOpinionGridList(HtmlHelper<TModel> htmlHelper)
            : base(htmlHelper.ViewContext, htmlHelper.ViewData)
        {
            this.htmlHelper = htmlHelper;
        }

        ///// <summary>
        ///// 页数
        ///// </summary>
        public int PageSize
        {
            get
            {
                return defaultPageSize;
            }
            set
            {
                this.defaultPageSize = value;
            }
        }

        ///// <summary>
        ///// 标题
        ///// </summary>
        public string Title
        {
            get;
            set;
        }

        ///// <summary>
        ///// 高度
        ///// </summary>
        public int Height
        {
            get
            {
                return defaultHeight;
            }
            set
            {
                this.defaultHeight = value;
            }
        }

        public override void WriteHtml(System.IO.StringWriter stringWriter)
        {
            //获取默认ID
            if (string.IsNullOrEmpty(this.Name))
            {
                this.Name = defaultOpinionGridListID;
            }
            //查询条件
            string queryCondition = GetQueryConditionJsonData();
            StringBuilder mvcHtmlStr = new StringBuilder();
            //获取首页数据
            IEnumerable<WfClientOpinion> model = GetOpinionFristPageComments();
            //控件初始化
            var kendoGrid = htmlHelper.Kendo().Grid(model)
                            .Name(this.Name)
                            .ToolBar(toolbar => toolbar.Template(Translator.Translate(CultureDefine.DefaultCulture, this.Title)))
                            .Columns(columns =>
                            {
                                columns.Bound(p => p.Content).Title(Translator.Translate(CultureDefine.DefaultCulture, "意见")).Width(130);
                                columns.Bound(p => p.ExtraData).Title(Translator.Translate(CultureDefine.DefaultCulture, "结果")).Width(30);
                                columns.Bound(p => p.IssuePersonName).Title(Translator.Translate(CultureDefine.DefaultCulture, "处理人")).Width(30);
                                columns.Bound(p => p.IssueTime).Title(Translator.Translate(CultureDefine.DefaultCulture, "日期")).Width(30).Format("{0:yyyy-MM-dd HH:mm}");
                            })
                            .Pageable()
                            .Scrollable(scr => scr.Height(this.Height))
                            .Pageable(pageable => pageable
                            .Refresh(false)
                            .PageSizes(false)
                            .ButtonCount(5))
                            .DataSource(dataSource => dataSource
                                .Ajax()
                                .PageSize(this.PageSize)
                                .ServerOperation(true)
                                .Read(read => read.Action(WFDefaultActionUrl.OpinionGridList, "WFDefaultOperation", new { area = "", queryConditionJsonData = queryCondition }))
                             );

            mvcHtmlStr.Append(kendoGrid.ToString());
            stringWriter.Write(mvcHtmlStr);
            base.WriteHtml(stringWriter);
        }

        protected string GetQueryConditionJsonData()
        {
            WFOpinionQueryCondition param = new WFOpinionQueryCondition();
            WFUIRuntimeContext runtime = this.ViewContext.HttpContext.Request.GetWFContext();

            if (runtime != null && runtime.Process != null)
            {
                param.ProcessID = runtime.Process.ID;
                param.ActivityID = runtime.ActivityID;
                param.ResourceID = runtime.Process.ResourceID;
                param.InMoveToModel = runtime.Process.AuthorizationInfo.InMoveToMode;

                if (runtime.Process != null && runtime.Process.CurrentOpinion != null)
                {
                    param.IssueTime = runtime.Process.CurrentOpinion.IssueTime;
                }

            }

            string paramJson = JsonConvert.SerializeObject(param);
            string retResult = HttpUtility.HtmlEncode(paramJson);

            return retResult;
        }

        private IEnumerable<WfClientOpinion> GetOpinionFristPageComments()
        {
            WFUIRuntimeContext runtime = this.ViewContext.HttpContext.Request.GetWFContext();

            List<WfClientOpinion> wfClientOpinionList = new List<WfClientOpinion>();

            if (runtime == null || runtime.Process == null)
                return wfClientOpinionList;

            string currentProcessId = runtime.Process.ID;
            string currentResourceId = runtime.Process.ResourceID;
            string currentActivityId = runtime.ActivityID;
            bool inMoveToModel = runtime.Process.AuthorizationInfo.InMoveToMode;
            string userId = runtime.CurrentUser.ID;

            WfClientOpinionCollection opinionCollection = null;

            //根据ResourceId获取意见信息
            if (!string.IsNullOrEmpty(currentResourceId))
            {
                opinionCollection = WfClientProcessRuntimeServiceProxy.Instance.GetOpinionsByResourceID(currentResourceId);
            }
            else
            {
                opinionCollection = WfClientProcessRuntimeServiceProxy.Instance.GetOpinionsByProcessID(currentProcessId);
            }

            if (inMoveToModel && runtime.Process != null && runtime.Process.CurrentOpinion != null)
            {
                var searchResult = from q in opinionCollection select q;
                searchResult = searchResult.Where(p => p.IssueTime != runtime.Process.CurrentOpinion.IssueTime);

                wfClientOpinionList = searchResult.ToList();
            }
            else
            {
                wfClientOpinionList = opinionCollection.ToList();
            }

            //取得总页数
            int totalPageCount = wfClientOpinionList.Count;

            //排序
            wfClientOpinionList = wfClientOpinionList.OrderByDescending(p => p.IssueTime).ToList();

            ////获取审核结果
            if (totalPageCount > 0)
            {
                int rowIndexTotal = 0;

                if (totalPageCount <= this.PageSize)
                    rowIndexTotal = totalPageCount - 1;
                else
                    rowIndexTotal = this.PageSize;

                for (int rowIndex = 0; rowIndex <= rowIndexTotal; rowIndex++)
                {
                    WfClientOpinion opinion = wfClientOpinionList[rowIndex];

                    if (opinion.Content == "{{**string.Empty**}}")
                    {
                        opinion.Content = string.Empty;
                    }

                    opinion.ExtraData =
                        Translator.Translate(CultureDefine.DefaultCulture,
                            opinion.GetNextSteps().GetSelectedStep().GetDescription());
                }
            }

            if (totalPageCount > this.PageSize)
            {
                for (int rowIndex = this.PageSize; rowIndex < totalPageCount; rowIndex++)
                {
                    wfClientOpinionList[rowIndex] = new WfClientOpinion { };
                }
            }

            return wfClientOpinionList;
        }
    }
}
