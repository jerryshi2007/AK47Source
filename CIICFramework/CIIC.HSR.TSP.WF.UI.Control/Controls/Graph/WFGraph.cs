#undef myDEBUG
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WF.UI.Control.Controls.StartWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using CIIC.HSR.TSP.WebComponents.Extensions;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Web.Library.Script;
using System.Web;
using MCS.Library.Core;
using System.Reflection;
using MCS.Library.Globalization;
using System.Diagnostics;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.Graph
{
    /// <summary>
    /// 流程图
    /// </summary>  
    public class WFGraph : WFControlBase
    {
        private const int Default_Page_Size = 10;
        private const string defaultControlId = "wfGraph";

        public WFGraph(ViewContext vc, ViewDataDictionary vdd)
            : base(vc, vdd)
        {
            Operations = new GraphOps();
            BranchOps = new BranchPageOps();
            InitActionUrl();
        }

        /// <summary>
        /// 分支列表获取地址
        /// </summary>
        public string BranchUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 显示相关设置
        /// </summary>
        public GraphOps Operations { private set; get; }

        /// <summary>
        /// 分支数据加载设置
        /// </summary>
        public BranchPageOps BranchOps { private set; get; }

        public override void WriteHtml(System.IO.StringWriter stringWriter)
        {
            //获取默认ID
            if (string.IsNullOrEmpty(this.Name))
            {
                this.Name = defaultControlId;
                this.Id = defaultControlId;
            }
            base.WriteHtml(stringWriter);

            //注册JS          
            this.ViewContext.HttpContext.RegisterWebResource(stringWriter, "WFControlsJS", this.GetType(), "CIIC.HSR.TSP.WF.UI.Control.JS.WFControl.js");

            StringBuilder mvcHtmlStr = new StringBuilder();
#if myDEBUG
            GetMvcHtmlStrDebug(mvcHtmlStr);
#else
            WFUIRuntimeContext runtime = this.ViewContext.HttpContext.Request.GetWFContext();
            mvcHtmlStr.Append(this.GetHtml(this, runtime));//html代码 
#endif


            stringWriter.Write(mvcHtmlStr);
        }

        #region 获取HTML

        [Conditional("myDEBUG")]
        private void GetMvcHtmlStrDebug(StringBuilder mvcHtmlStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetStyle());
            WfClientProcessInfo process = new WfClientProcessInfo();
            process.ID = "p1";

            string rowHTML = GetProcessHtml(process, this.Operations);
            sb.Append(string.Format(_container, this.Name, rowHTML, _modalTemplates, _progressbar));
            sb.Append(GetInitScript());


            mvcHtmlStr.Append(sb.ToString());//html代码            
        }

        internal string GetHtml(WFGraph component, WFUIRuntimeContext runtime)
        {
            string rowHTML = string.Empty;
            if (runtime != null && runtime.Process != null)
            {
                rowHTML = GetProcessHtml(runtime.Process, this.Operations);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(GetStyle());

            sb.Append(string.Format(_container, component.Name, rowHTML, _modalTemplates, _progressbar));
            sb.Append(GetInitScript());

            return sb.ToString();
        }

        internal static string GetProcessHtml(WfClientProcessInfo processInfo, GraphOps ops)
        {
            List<GraphActivityVM> activitys = null;
#if myDEBUG
            activitys = new List<GraphActivityVM>();
            PrepareDataDebug(activitys);
#else
            activitys = PrepareData(processInfo);
#endif

            StringBuilder sb = new StringBuilder();

            foreach (GraphActivityVM item in activitys)
            {
                sb.Append(item.ToHTML(ops));
            }
            string wrap = string.Empty;
            if (!ops.IsBranch)
            {
                //非子流程需要显示标题
                wrap = string.Format(_panelTitle, CIIC.HSR.TSP.WebComponents.Resource.Message.WFNavigation, _processWrap);
            }
            else
            {
                wrap = _processWrap;
            }

            return string.Format(wrap, processInfo.ID, sb.ToString());
        }

        /// <summary>
        /// 获取分支流程列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ops"></param>
        /// <returns></returns>
        internal static string GetBranchListHtml(string id, BranchPageOps ops)
        {

            List<GraphSubProcessVM> activitys = null;
#if myDEBUG
            activitys = new List<GraphSubProcessVM>();
            PrepareBranchDataDebug(activitys);
#else
            activitys = PrepareBranchData(id, ops);
#endif

            StringBuilder sb = new StringBuilder();

            foreach (GraphSubProcessVM item in activitys)
            {
                sb.Append(item.ToHTML());
            }

            //增加分页信息
            string page = string.Empty;
            if (ops.TotalRows > 0)
            {
                int mPage = ops.TotalRows % ops.PageRows;
                int preStartIndex = ops.StartRowIndex - ops.PageRows;
                int nexStartIndex = ops.StartRowIndex + ops.PageRows;
                int endStartIndex = ops.TotalRows - mPage;

                if (preStartIndex < 0)
                {
                    preStartIndex = 0;//只有一页
                }
                if (mPage == 0)
                {
                    endStartIndex = endStartIndex - ops.PageRows;//尾页
                }
                if (nexStartIndex > endStartIndex)
                {
                    nexStartIndex = endStartIndex;//下页是为尾页
                }
                int pagemax = ops.StartRowIndex + ops.PageRows;
                if (pagemax > ops.TotalRows)
                {
                    pagemax = ops.TotalRows;
                }
                string pageTotal = string.Format(CIIC.HSR.TSP.WebComponents.Resource.Message.GraphListPageTotal, ops.StartRowIndex.ToString()
                    , pagemax.ToString()
                    , ops.TotalRows.ToString());


                page = string.Format(_subProcessPage, string.Format(_pageTotal, pageTotal), ops.TotalRows.ToString()
                    , preStartIndex.ToString()
                    , nexStartIndex.ToString()
                    , endStartIndex.ToString());

            }

            return string.Format(_subProcessTemplates, page, sb.ToString());
        }

        #endregion

        #region template

        private static readonly string _panelTitle = @"<div class=""panel panel-default""><div class=""panel-heading""><h3 class=""panel-title"">{0}</h3></div><div class=""panel-body"">{1}</div></div>";

        private static readonly string _container = @"<div class=""wfGraph"" id=""{0}"" name=""{0}"" ><div class=""wfGraphBody"">{1}</div>{2}<div class=""wfGraphProgress"" style=""display:none"">{3}</div></div>";

        private static readonly string _processWrap = @"<div class=""eve-navigation"" processID=""{0}"" ><div class=""eve-nav"" style=""left:0px""><div class=""eve-content"">{1}</div></div></div>";

        private static readonly string _activityWrap = @"<div class=""item {0}"">{1}</div>";
        private static readonly string _activityLine = @"<div class=""eve-line""></div>";
        private static readonly string _activityStartIco = "glyphicon-inbox";
        private static readonly string _activityStepFIco = "glyphicon-user";
        private static readonly string _activityStepIco = "glyphicon-random";
        private static readonly string _activityEndIco = "glyphicon-ok";
        private static readonly string _activityIco = @"<div class=""eve-ico""><span class=""glyphicon {0}""></span></div>";
        private static readonly string _activityChildFlag = @"<div class=""childFlag""><span class=""glyphicon glyphicon-plus""  activityID=""{0}""></span></div>";
        private static readonly string _activityTemplate = @"<div class=""eve-obj""  title=""{3}"">    
                        {2}
                        <div class=""eve-title"">{0}</div>
                        <div class=""eve-body"">{1}</div>
                        </div>";
        private static readonly string _subProcessTemplates = @"<div class=""list-group childrenProcess"" >{0}{1}</div>";
        private static readonly string _subProcessTemplate = @"<div><div   class=""list-group-item"" processID=""{0}"" >
                        <div class=""branch-ico""><span class=""glyphicon {1}""></span></div>
                        <div class=""branch-title"">{2}</div>
                        <div class=""branch-body"">{3}</div>                        
                  </div><div class=""branch-graph wfGraph"" style=""display:none""></div></div>";

        //private static readonly string _pageNumbers =@"<ul class=""k-pager-numbers k-reset"">{0}</ul>";
        //private static readonly string _pageNumber = @"<li><span class=""k-state-selected"">{0}</span></li>";
        private static readonly string _pageTotal = @"<span class=""k-pager-info k-label"">{0}</span>";
        private static readonly string _progressbar = @"<div class=""progress progress-bar progress-striped active"" style=""margin: auto; left: 0px; top: 0px; width: 100px; height: 20px; right: 0px; bottom: 0px; position: fixed; z-index: 100001;""><div class=""progress-bar progress-striped active"" role=""progressbar"" aria-valuenow=""100"" aria-valuemin=""0"" aria-valuemax=""100"" style=""width: 100%;""><span class=""sr-only"">100% Complete</span></div></div>";
        private static readonly string _subProcessPage = @"<div class=""k-pager-wrap k-grid-pager k-widget"" data-role=""pager"" rowtotal=""{1}"">
        <a    class=""k-link k-pager-nav k-state-disabled k-pager-first "" href=""javascript:void (0)"" ops=""0""><span class=""k-icon k-i-seek-w"">seek-w</span></a>
        <a  class=""k-link k-pager-nav k-state-disabled"" href=""javascript:void (0)"" ops=""{2}""><span class=""k-icon k-i-arrow-w"">arrow-w</span></a>      
        <a   class=""k-link k-pager-nav k-state-disabled"" href=""javascript:void (0)"" ops=""{3}""><span class=""k-icon k-i-arrow-e"">arrow-e</span></a>
        <a  class=""k-link k-pager-nav k-state-disabled k-pager-last"" href=""javascript:void (0)"" ops=""{4}""><span class=""k-icon k-i-seek-e"">seek-e</span></a>
        {0}</div>";

        private static readonly string _modalTemplates = @"<div id=""childProcessDiv"" ></div>";

        private static readonly string _style = @"<style>
        .wfGraph .eve-navigation {{   
             position:relative;  
             width:100%;
             height:100px;    
             overflow :hidden;  
             cursor:move;  
        }}     
       .wfGraph .eve-nav{{
            position:absolute;
            top:0px;
            left:0px;
            z-index:1;
        }}
        .wfGraph .eve-content {{
            position: relative;
        }}
          .wfGraph  .eve-content .item {{
                position: absolute;
                top: 0px;
                z-index: 2;
            }} 

       .wfGraph .eve-obj, #childProcessDiv .branch-ico{{ 
         text-align:center;
         position:relative;    
         width:120px;         
        }}    
        #childProcessDiv .branch-ico{{
        float:left;
        }}    
      .wfGraph  .eve-line{{ 
             position:absolute; 
             width:50px;
             top:30px;
             right:-25px;           
             border-top-width:3px;
             border-top-style: solid;
             border-top-color:#606574;           
        }} 
       .wfGraph .eve-ico,#childProcessDiv .branch-ico{{ 
            font-size:20px;                               
            line-height:60px;          
            background-image:url({0});
            background-position:center top;
            background-repeat:no-repeat;
        }}
       .wfGraph .Running .eve-ico, #childProcessDiv .Running .branch-ico{{           
            background-image:url({1});
        }}
      .wfGraph  .Running .eve-line {{
             border-top-color:#ff6a00;  
        }}

     #childProcessDiv .list-group-item {{
            border-top:1px solid #CBD2D5;
        }}
       #childProcessDiv .branch-title{{
        line-height:2em;
        }}
        #childProcessDiv .branch-graph{{
        clear:both;
       }}
       .wfGraph .eve-title,.wfGraph .eve-body{{  
            overflow: hidden;
            white-space: nowrap;           
            -o-text-overflow: ellipsis; 
            text-overflow: ellipsis; 
        }} 
       .wfGraph .childFlag {{
             font-size:12px;
             position:absolute;
             cursor: default;
             padding:5px;
             right:0px;
             top:0px;
         }}
       .wfGraph .Completed{{

        }}
       .wfGraph .Running {{
            color:#ff6a00;
        }}
       .wfGraph  .NotRunning   {{          
            opacity:0.4;
            filter:alpha(opacity=40); 
        }}          
    </style>";

        private static readonly string _initJs = @"<script type=""text/javascript"">
                $(function(){{        
                     var ops = {1};                
                      $.fn.HSR.Controls.WFGraph('#{0}').Init(ops); 
                }});</script>";

        #endregion

        #region class

        /// <summary>
        /// 显示参数
        /// </summary>
        public class GraphOps
        {
            public GraphOps()
            {

                this.EnableDefaultUserName = true;
                this.DefaultUserName = CIIC.HSR.TSP.WebComponents.Resource.Message.DefaultUserName;
                this.ShowBranchRows = Default_Page_Size;
                this.BranchWinTitle = CIIC.HSR.TSP.WebComponents.Resource.Message.BranchWinTitle;
            }

            /// <summary>
            /// 激活流程节点上用户为空时候，默认值显示的信息
            /// </summary>
            public string DefaultUserName { set; get; }

            /// <summary>
            /// 激活流程节点上用户为空时候，是否显示默认值
            /// </summary>
            public bool EnableDefaultUserName { set; get; }

            /// <summary>
            /// 每页显示分支流程行数
            /// </summary>
            public int ShowBranchRows { set; get; }

            /// <summary>
            /// 分支流程窗体标题
            /// </summary>
            public string BranchWinTitle { set; get; }

            /// <summary>
            /// 是否为分支流程
            /// </summary>
            public bool IsBranch { set; get; }

        }

        public class BranchPageOps
        {
            public BranchPageOps()
            {
                PageRows = Default_Page_Size;
                TotalRows = -1;
                StartRowIndex = 0;
            }
            public int StartRowIndex { set; get; }
            public int TotalRows { set; get; }

            public int PageRows { set; get; }
        }
        internal enum GraphActivityStated
        {
            /// <summary>
            /// 未运行
            /// </summary>
            NotRunning,
            /// <summary>
            /// 运行中
            /// </summary>
            Running,
            /// <summary>
            /// 等待中
            /// </summary>
            Pending,

            /// <summary>
            /// 已完成
            /// </summary>
            Completed,

            /// <summary>
            /// 被终止
            /// </summary>
            Aborted
        }


        internal enum GraphActivityType
        {
            //开始
            Start = 0,
            //过程
            Step = 1,
            //结束
            End = 2
        }
        internal class GraphActivityVM
        {
            public string ID { set; get; }

            public string Name { set; get; }
            public string ResourseName { set; get; }

            public GraphActivityStated Stated { set; get; }

            public GraphActivityType ActivityType { set; get; }


            private List<GraphSubProcessVM> _items = new List<GraphSubProcessVM>();
            public List<GraphSubProcessVM> ChildrenProcess
            {
                get
                {
                    return _items;
                }
            }

            public bool HaveChild { internal set; get; }

            /// <summary>
            /// 获取连接线HTML
            /// </summary>
            /// <returns></returns>
            private string GetLine()
            {
                if (GraphActivityType.End != ActivityType)
                {
                    return _activityLine;
                }
                return string.Empty;
            }

            /// <summary>
            /// 获取图标 HTML
            /// </summary>
            /// <returns></returns>
            private string GetIco()
            {
                string str = string.Empty;
                switch (ActivityType)
                {
                    case GraphActivityType.Start:
                        str = _activityStartIco;
                        break;
                    case GraphActivityType.Step:
                        if (GraphActivityStated.Completed == Stated)
                        {
                            str = _activityStepFIco;
                        }
                        else
                        {
                            str = _activityStepIco;
                        }
                        break;
                    case GraphActivityType.End:
                        str = _activityEndIco;
                        break;
                }
                return string.Format(_activityIco, str);
            }

            public string ToHTML(GraphOps ops)
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder strB = new StringBuilder();

                string activityCss = Stated.ToString();


                strB.Append(GetLine());
                strB.Append(GetIco());

                if (HaveChild && !ops.IsBranch)//加载子流程相关样式，只支持1层子流程
                {
                    strB.Append(string.Format(_activityChildFlag, ID));
                }

                string title = HttpUtility.HtmlEncode(Name);
                string body = HttpUtility.HtmlEncode(ResourseName);

                string tooltrip = title;
                if (!string.IsNullOrEmpty(body))
                {
                    tooltrip = string.Format("{0}:{1}", title, body);
                }
                else if (ops.EnableDefaultUserName && this.ActivityType == GraphActivityType.Step)
                {
                    body = ops.DefaultUserName;
                    tooltrip = string.Format("{0}:{1}", title, body);
                }

                sb.Append(string.Format(_activityTemplate, title, body, strB.ToString(), tooltrip));

                return string.Format(_activityWrap, activityCss, sb.ToString());
            }
        }

        internal class GraphSubProcessVM
        {
            /// <summary>
            /// 关联主流程节点
            /// </summary>
            public string OWNER_ACTIVITY_ID { set; get; }
            /// <summary>
            /// 子流程实例ID
            /// </summary>
            public string ID { set; get; }

            /// <summary>
            /// 子流程实例名称
            /// </summary>
            public string Name { set; get; }

            public DateTime? EndTime { set; get; }

            /// <summary>
            /// 流程状态
            /// </summary>
            public GraphActivityStated Stated { set; get; }


            /// <summary>
            /// 获取图标 HTML
            /// </summary>
            /// <returns></returns>
            private string GetIco()
            {
                string str = string.Empty;
                switch (Stated)
                {
                    case GraphActivityStated.Completed:
                        str = _activityStepFIco;
                        break;
                    default:
                        str = _activityStepIco;
                        break;
                }
                return str;
            }
            public string ToHTML()
            {
                StringBuilder sb = new StringBuilder();
                string body = EndTime.HasValue ? EndTime.ToString() : string.Empty;
                sb.Append(string.Format(_subProcessTemplate, ID, GetIco(), HttpUtility.HtmlEncode(Name), body));
                return sb.ToString();
            }

        }


        #endregion

        #region 数据准备

        private string GetStyle()
        {
            return string.Format(_style, ClientScriptExtension.GetWebResourceUrl(this.GetType(), "CIIC.HSR.TSP.WF.UI.Control.JS.Image.step_done.png")
                , ClientScriptExtension.GetWebResourceUrl(this.GetType(), "CIIC.HSR.TSP.WF.UI.Control.JS.Image.step_cur.png")
             );
        }

        private string GetInitScript()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("url", this.ActionUrl);
            dic.Add("branchUrl", this.BranchUrl);
            dic.Add("errorMsg", string.Format(CIIC.HSR.TSP.WebComponents.Resource.Message.AjaxError, string.Empty));
            dic.Add("graphOps", this.Operations);
            dic.Add("branchOps", this.BranchOps);

            WfClientJsonConverterHelper.Instance.RegisterConverters();
            string str = JSONSerializerExecute.Serialize(dic);

            return string.Format(_initJs, this.Name, str);
        }

        /// <summary>
        /// 控制器
        /// </summary>
        private void InitActionUrl()
        {
            //配置Action的Url地址
            Controller controller = (Controller)this.ViewContext.Controller;
            this.ActionUrl = controller.Url.Action("WFGraphLoad", "WFGraph", new { @area = "" });
            this.BranchUrl = controller.Url.Action("WFGraphLoadBranch", "WFGraph", new { @area = "" });
        }

        [Conditional("myDEBUG")]
        private static void PrepareDataDebug(List<GraphActivityVM> items)
        {

            GraphActivityVM activity = new GraphActivityVM();
            activity.ID = "a1";
            activity.Name = "开始";
            activity.Stated = GraphActivityStated.Completed;
            activity.ResourseName = "";
            activity.ActivityType = GraphActivityType.Start;
            activity.HaveChild = false;
            items.Add(activity);

            activity = new GraphActivityVM();
            activity.ID = "a21";
            activity.Name = "审批节点";
            activity.Stated = GraphActivityStated.Completed;
            activity.ResourseName = "李四";
            activity.ActivityType = GraphActivityType.Step;
            activity.HaveChild = false;
            items.Add(activity);

            activity = new GraphActivityVM();
            activity.ID = "a2";
            activity.Name = "审批节点";
            activity.Stated = GraphActivityStated.Pending;
            activity.ResourseName = "张山";
            activity.ActivityType = GraphActivityType.Step;
            activity.HaveChild = true;
            items.Add(activity);

            activity = new GraphActivityVM();
            activity.ID = "a31";
            activity.Name = "审批节点";
            activity.Stated = GraphActivityStated.NotRunning;
            activity.ResourseName = "张三";
            activity.ActivityType = GraphActivityType.Step;
            activity.HaveChild = true;
            items.Add(activity);

            activity = new GraphActivityVM();
            activity.ID = "a3";
            activity.Name = "结束";
            activity.Stated = GraphActivityStated.NotRunning;
            activity.ResourseName = "";
            activity.ActivityType = GraphActivityType.End;
            activity.HaveChild = false;
            items.Add(activity);

        }

        private static List<GraphActivityVM> PrepareData(WfClientProcessInfo processInfo)
        {
            List<GraphActivityVM> items = new List<GraphActivityVM>();

            if (processInfo == null || processInfo.MainStreamActivityDescriptors == null)
            {
                return items;
            }
            GraphActivityVM activity;

            foreach (WfClientMainStreamActivityDescriptor item in processInfo.MainStreamActivityDescriptors)
            {
                activity = new GraphActivityVM();
                items.Add(activity);

                activity.ID = item.ActivityInstanceID;
                activity.Name = item.Activity.Name;

                activity.Stated = ToGraphStated(item);
                activity.ResourseName = GetUserNames(item);

                activity.ActivityType = ToGraphType(item.Activity.ActivityType);
                activity.HaveChild = false;

                if (item.BranchProcessGroupsCount > 0)
                {
                    activity.HaveChild = true;
                }


            }
            return items;
        }

        [Conditional("myDEBUG")]
        private static void PrepareBranchDataDebug(List<GraphSubProcessVM> list)
        {
            GraphSubProcessVM vm = new GraphSubProcessVM()
            {
                ID = "sub1",
                Name = "子流程1",
                Stated = GraphActivityStated.Completed,
                EndTime = DateTime.Now,
                OWNER_ACTIVITY_ID = "a2"
            };
            list.Add(vm);

            vm = new GraphSubProcessVM()
            {
                ID = "sub2",
                Name = "子流程2",
                Stated = GraphActivityStated.Running,
                EndTime = DateTime.Now,
                OWNER_ACTIVITY_ID = "a2"
            };
            list.Add(vm);

            vm = new GraphSubProcessVM()
            {
                ID = "sub3",
                Name = "子流程3",
                Stated = GraphActivityStated.Pending,
                EndTime = DateTime.Now,
                OWNER_ACTIVITY_ID = "a2"
            };
            list.Add(vm);
        }

        /// <summary>
        /// 准备分支流程
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ops">分页信息</param>
        /// <returns></returns>
        private static List<GraphSubProcessVM> PrepareBranchData(string id, BranchPageOps ops)
        {
            List<GraphSubProcessVM> list = new List<GraphSubProcessVM>();

            WfClientProcessCurrentInfoPageQueryResult result = WfClientProcessRuntimeServiceProxy.Instance.QueryBranchProcesses(id, string.Empty, ops.StartRowIndex, ops.PageRows, string.Empty, ops.TotalRows);

            foreach (var item in result.QueryResult)
            {
                GraphSubProcessVM vm = new GraphSubProcessVM()
                {
                    ID = item.InstanceID
                    ,
                    Name = item.ProcessName
                    ,
                    Stated = ToGraphStated(item)
                    ,
                    EndTime = item.EndTime
                    ,
                    OWNER_ACTIVITY_ID = id
                };
                list.Add(vm);
            }

            ops.TotalRows = result.TotalCount;
            return list;
        }


        private static string GetUserNames(WfClientMainStreamActivityDescriptor item)
        {
            if (item.Status == WfClientActivityStatus.Completed)
            {
                if (item.Operator == null)
                {
                    return string.Empty;
                }
                return item.Operator.Name;
            }
            WfClientAssigneeCollection assignees = item.Assignees;

            StringBuilder sb = new StringBuilder();
            string splitter = ",";

            for (int i = 0; i < assignees.Count; i++)
            {
                if (assignees[i].User == null)
                {
                    continue;
                }
                if (i > 0)
                {
                    sb.Append(splitter);
                }
                sb.Append(assignees[i].User.Name);
            }

            return sb.ToString();
        }
        private static GraphActivityStated ToGraphStated(WfClientProcessCurrentInfo item)
        {
            WfClientProcessStatus processStatus = item.Status;

            GraphActivityStated stated = GraphActivityStated.NotRunning;

            if (processStatus == WfClientProcessStatus.Completed)
            {
                stated = GraphActivityStated.Completed;
            }
            else if (processStatus == WfClientProcessStatus.Paused)
            {
                stated = GraphActivityStated.Pending;
            }
            else if (processStatus == WfClientProcessStatus.Running)
            {
                stated = GraphActivityStated.Running;
            }
            else if (processStatus == WfClientProcessStatus.NotRunning)
            {
                stated = GraphActivityStated.NotRunning;
            }
            else if (processStatus == WfClientProcessStatus.Aborted)
            {
                stated = GraphActivityStated.Aborted;
            }
            else if (processStatus == WfClientProcessStatus.Maintaining)
            {
                stated = GraphActivityStated.Pending;
            }
            return stated;
        }

        private static GraphActivityStated ToGraphStated(WfClientMainStreamActivityDescriptor item)
        {
            WfClientActivityStatus wfActivityStatus = item.Status;

            GraphActivityStated stated = GraphActivityStated.NotRunning;

            if (wfActivityStatus == WfClientActivityStatus.Completed)
            {
                if (item.Activity.ActivityType == WfClientActivityType.CompletedActivity)
                {
                    //起始节点需要高亮
                    stated = GraphActivityStated.Running;
                }
                else
                {
                    stated = GraphActivityStated.Completed;
                }

            }
            else if (wfActivityStatus == WfClientActivityStatus.Pending)
            {
                stated = GraphActivityStated.Running;
            }
            else if (wfActivityStatus == WfClientActivityStatus.Running)
            {
                stated = GraphActivityStated.Running;
            }
            else if (wfActivityStatus == WfClientActivityStatus.NotRunning)
            {
                stated = GraphActivityStated.NotRunning;
            }
            else if (wfActivityStatus == WfClientActivityStatus.Aborted)
            {
                stated = GraphActivityStated.NotRunning;
            }
            return stated;
        }

        private static GraphActivityType ToGraphType(WfClientActivityType activityType)
        {
            GraphActivityType type = GraphActivityType.Step;
            if (activityType == WfClientActivityType.InitialActivity)
            {
                type = GraphActivityType.Start;
            }
            else if (activityType == WfClientActivityType.CompletedActivity)
            {
                type = GraphActivityType.End;
            }
            return type;
        }
        #endregion

    }
}
