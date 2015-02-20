using CIIC.HSR.TSP.WF.UI.Control.Controls.MoveTo;
using CIIC.HSR.TSP.WF.UI.Control.Controls.StartWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.Controls.CancelWorkflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CIIC.HSR.TSP.WF.UI.Control.Controls.PauseWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.Controls.ResumeWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Withdraw;
using CIIC.HSR.TSP.WF.UI.Control.Controls.SaveWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.Controls.FollowWorkFlow;
using CIIC.HSR.TSP.WF.UI.Control.Controls.RestoreWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Comments;
using CIIC.HSR.TSP.WF.UI.Control.Controls.OpinionGridList;
using CIIC.HSR.TSP.WF.UI.Control.Controls.UpdateProcess;
using CIIC.HSR.TSP.WF.UI.Control.Controls.ToolbarStartWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.Controls.ToolbarMoveToWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Graph;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls
{
	public class WFWidgetFactory<TModel>
	{
		#region prop
		public HtmlHelper<TModel> HtmlHelper { get; set; }

		private ViewContext ViewContext
		{
			get
			{
				return this.HtmlHelper.ViewContext;
			}
		}

		private ViewDataDictionary ViewData
		{
			get
			{
				return this.HtmlHelper.ViewData;
			}
		}

		#endregion

		public WFWidgetFactory(HtmlHelper<TModel> htmlHelper)
		{
			this.HtmlHelper = htmlHelper;
		}

		public WFStartWorkflowBuilder WFStartWorkflow()
		{
			return new WFStartWorkflowBuilder(this.HtmlHelper);
		}

		public WFMoveToBuilder WFMoveTo()
		{
			return new WFMoveToBuilder(this.HtmlHelper);
		}

		public WFCancelWorkflowBuilder WFCancelWorkflow()
		{
			return new WFCancelWorkflowBuilder(this.HtmlHelper);
		}

		public WFPauseWorkflowBuilder WFPauseWorkflow()
		{
			return new WFPauseWorkflowBuilder(this.HtmlHelper);
		}

		public WFResumeWorkflowBuilder WFResumeWorkflow()
		{
			return new WFResumeWorkflowBuilder(this.HtmlHelper);
		}

		public WFWithdrawWorkflowBuilder WFWithdrawWorkflow()
		{
			return new WFWithdrawWorkflowBuilder(this.HtmlHelper);
		}

		public WFSaveWorkflowBuilder WFSaveWorkflow()
		{
			return new WFSaveWorkflowBuilder(this.HtmlHelper);
		}

		public WFTrackWorkflowBuilder WFTrackWorkflow()
		{
			return new WFTrackWorkflowBuilder(this.HtmlHelper);
		}

		public WFRestoreWorkflowBuilder WFRestoreWorkflow()
		{
			return new WFRestoreWorkflowBuilder(this.HtmlHelper);
		}

		public WFUpdateProcessBuilder WFUpdateProcess()
		{
			return new WFUpdateProcessBuilder(this.HtmlHelper);
		}

		public WFCommentsBuilder<TModel> WFComments()
		{
			return new WFCommentsBuilder<TModel>(HtmlHelper);
		}

        public WFOpinionGridListBuilder<TModel> WFOpinionGridList()
		{
            return new WFOpinionGridListBuilder<TModel>(HtmlHelper);
		}

        public WFToolbarStartWorkflowBuilder WFToolbarStartWorkflow()
        {
            return new WFToolbarStartWorkflowBuilder(this.HtmlHelper);
        }

        public WFToolbarMoveToWorkflowBuilder WFToolbarMoveToWorkflow()
        {
            return new WFToolbarMoveToWorkflowBuilder(this.HtmlHelper);
        }

        public WFGraphBuilder WFGraph()
        {
            return new WFGraphBuilder(this.HtmlHelper);
        }
       
	}
}
