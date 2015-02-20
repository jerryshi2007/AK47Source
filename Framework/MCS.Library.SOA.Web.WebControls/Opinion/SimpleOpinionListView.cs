using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Globalization;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 简单意见列表显示控件
	/// </summary>
	[DefaultProperty("Opinions")]
	[ToolboxData("<{0}:SimpleOpinionListView runat=server></{0}:SimpleOpinionListView>")]
	public class SimpleOpinionListView : Control, INamingContainer
	{
		private GenericOpinionCollection _Opinions = null;
		private HtmlTable _Table = new HtmlTable();

		#region Events
		/// <summary>
		/// 意见绑定事件定义
		/// </summary>
		public event EventHandler<OpinionListViewBindEventArgs> OpinionBind;
		#endregion Events

		#region Properties
		/// <summary>
		/// 意见集合
		/// </summary>
		[Browsable(false)]
		public GenericOpinionCollection Opinions
		{
			get
			{
				return _Opinions;
			}
			set
			{
				_Opinions = value;
			}
		}

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(true)]
		[Localizable(true)]
		public bool EnableUserPresence
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "EnableUserPresence", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "EnableUserPresence", value);
			}
		}

		[DefaultValue(typeof(Unit), "100%")]
		[Bindable(true), Category("Appearance")]
		public Unit Width
		{
			get
			{

				return WebControlUtility.GetViewStateValue(ViewState, "Width", Unit.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "Width", value);
			}
		}

		[Description("意见为空时显示的文本")]
		[DefaultValue("（无）")]
		[Bindable(true), Category("Appearance")]
		public string EmptyOpinionText
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "EmptyOpinionText", Translator.Translate(Define.DefaultCulture, "（无）"));
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "EmptyOpinionText", value);
			}
		}

		[DefaultValue(typeof(Unit), "")]
		[Bindable(true), Category("Appearance")]
		public Unit Height
		{
			get
			{

				return WebControlUtility.GetViewStateValue(ViewState, "Height", Unit.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "Height", value);
			}
		}
		#endregion Properties

		#region Protected
		protected override void OnPreRender(EventArgs e)
		{
			InitRootTable(this._Table, Width, Height);
			this.Controls.Add(this._Table);

			if (this._Opinions != null)
			{
				OpinionListControlHelper.PrepareSignImages(this._Opinions);
				RenderOpinions(this._Opinions, this._Table);
			}

			base.OnPreRender(e);
		}

		/// <summary>
		/// 触发意见绑定事件
		/// </summary>
		/// <param name="e">意见绑定事件参数</param>
		protected virtual void OnOpinionBind(OpinionListViewBindEventArgs e)
		{
			if (OpinionBind != null)
				OpinionBind(this, e);
		}
		#endregion Protected

		#region Private
		private void RenderOpinions(GenericOpinionCollection opinions, HtmlTable table)
		{
			int i = 0;

			foreach (GenericOpinion opinion in opinions)
			{
				HtmlTableRow row = new HtmlTableRow();

				row.ID = "OpinionRow" + i.ToString();
				table.Controls.Add(row);

				RenderOneOpinion(opinion, row, i);

				i++;
			}
		}

		private void RenderOneOpinion(GenericOpinion opinion, HtmlTableRow row, int index)
		{
			HtmlTableCell cell = new HtmlTableCell();

			cell.Attributes["class"] = "opinions";

			row.Controls.Add(cell);

			RenderOneOpinionContent(opinion, cell, index);
		}

		private void RenderOneOpinionContent(GenericOpinion opinion, Control opinionContainer, int index)
		{		
			HtmlGenericControl opDiv = new HtmlGenericControl("div");
			opDiv.Attributes["class"] = "opinion";
			opDiv.Style["padding"] = "0px";

			if (index > 0)
				opDiv.Style["border-top"] = "1px dotted silver";

			object nextStepsString = string.Empty;

			if (opinion.ExtData.TryGetValue("NextSteps", out nextStepsString))
			{
				OpinionListView.RenderOriginalOpinionSelector(opinionContainer, (string)nextStepsString);
			}

			OpinionListViewNamingContainer container = new OpinionListViewNamingContainer();
			opDiv.Controls.Add(container);
	
			container.ID = "Opinion" + opinion.ID;

			OnOpinionBind(new OpinionListViewBindEventArgs(opinion, null, container, true));

			string opText = EmptyOpinionText;

			if (string.IsNullOrEmpty(opinion.Content) == false)
				opText = opinion.Content;

			HtmlGenericControl div = new HtmlGenericControl("div");
			div.Style["padding"] = "6px 8px";

			if (EnableUserPresence == false)
			{
				RenderOneOpinionWithoutPrecense(opinion, opText, div);
			}
			else
			{
				RenderOneOpinionWithPrecense(opinion, opText, div);
			}

			opDiv.Controls.Add(div);
			opinionContainer.Controls.Add(opDiv);
		}

		private void RenderOneOpinionWithPrecense(GenericOpinion opinion, string opText, Control container)
		{
			HtmlGenericControl opTextContainer = new HtmlGenericControl("div");

			opTextContainer.Attributes["class"] = "text";
			opTextContainer.InnerHtml = HttpUtility.HtmlEncode(opText).Replace("\r\n", "<br/>");
			container.Controls.Add(opTextContainer);

			HtmlGenericControl signName = new HtmlGenericControl("div");
			signName.Attributes["class"] = "signName";
			container.Controls.Add(signName);

			UserPresence presence = new UserPresence();

			if (OpinionListControlHelper.UserSignatures.ContainsKey(opinion.IssuePerson.ID))
			{
				HtmlImage sigImage = new HtmlImage();

				sigImage.Src = OpinionListControlHelper.UserSignatures[opinion.IssuePerson.ID];
				sigImage.Alt = opinion.IssuePerson.DisplayName;

				signName.Controls.Add(sigImage);

				presence.ShowUserDisplayName = false;
			}

			signName.Controls.Add(presence);

			presence.UserID = opinion.IssuePerson.ID;
			presence.UserDisplayName = opinion.IssuePerson.DisplayName;	//防止人员离职
			presence.EnsureInUserList();

			if (opinion.IssuePerson.ID != opinion.AppendPerson.ID)
			{
				HtmlGenericControl sp1 = new HtmlGenericControl("span");
				sp1.InnerText = "(";
				signName.Controls.Add(sp1);

				UserPresence presence2 = new UserPresence();
				signName.Controls.Add(presence2);
				presence2.UserID = opinion.AppendPerson.ID;
				presence2.UserDisplayName = opinion.AppendPerson.DisplayName;	//防止人员离职
				presence2.EnsureInUserList();

				HtmlGenericControl sp2 = new HtmlGenericControl("span");
				sp2.InnerText = string.Format(" {0})", Translator.Translate(Define.DefaultCulture, "代写"));
				signName.Controls.Add(sp2);
			}

			HtmlGenericControl dateContainer = new HtmlGenericControl("div");

			dateContainer.Attributes["class"] = "signDate";
			dateContainer.InnerText = opinion.AppendDatetime.ToString("yyyy-MM-dd HH:mm:ss");
			container.Controls.Add(dateContainer);
		}

		private void RenderOneOpinionWithoutPrecense(GenericOpinion opinion, string opText, Control container)
		{
			string signature = HttpUtility.HtmlEncode(opinion.IssuePerson.DisplayName);

			if (opinion.IssuePerson.ID != opinion.AppendPerson.ID)
			{
				signature = string.Format("{0}({1} {2})",
					HttpUtility.HtmlEncode(opinion.IssuePerson.DisplayName),
					HttpUtility.HtmlEncode(opinion.AppendPerson.DisplayName),
					Translator.Translate(Define.DefaultCulture, "代写"));
			}

			if (OpinionListControlHelper.UserSignatures.ContainsKey(opinion.IssuePerson.ID))
			{
				signature = string.Format("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />",
					OpinionListControlHelper.UserSignatures[opinion.IssuePerson.ID],
					signature);
			}

			HtmlGenericControl divOpinion = new HtmlGenericControl("div");
			divOpinion.Attributes["class"] = "text";
			divOpinion.InnerHtml = HttpUtility.HtmlEncode(opText).Replace("\r\n", "<br/>");

			container.Controls.Add(divOpinion);

			HtmlGenericControl divSignName = new HtmlGenericControl("div");
			divSignName.Attributes["class"] = "signName";

			divSignName.Controls.Add(new LiteralControl(signature));

			container.Controls.Add(divSignName);

			HtmlGenericControl divSignDate = new HtmlGenericControl("div");
			divSignDate.Attributes["class"] = "signDate";
			divSignDate.InnerText = opinion.AppendDatetime.ToString("yyyy-MM-dd HH:mm:ss");

			container.Controls.Add(divSignDate);
		}

		private static void InitRootTable(HtmlTable table, Unit width, Unit height)
		{
			table.CellPadding = 4;
			table.CellSpacing = 0;
			table.Style["width"] = width.ToString();
			table.Style["height"] = height.ToString();
			table.ID = "ContainerTable";
			table.Attributes["class"] = "opinionListView";
		}
		#endregion Private
	}
}
