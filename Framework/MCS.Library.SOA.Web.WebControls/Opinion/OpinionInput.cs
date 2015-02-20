using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using MCS.Web.Library;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Principal;
using MCS.Library.Globalization;
using MCS.Library.Caching;
using MCS.Web.Library.MVC;
using MCS.Library.OGUPermission;

[assembly: WebResource("MCS.Web.WebControls.Opinion.pen.png", "image/png")]
[assembly: WebResource("MCS.Web.WebControls.Opinion.line.png", "image/png")]

namespace MCS.Web.WebControls
{

	[DefaultProperty("Opinion")]
	[ToolboxData("<{0}:OpinionInput runat=server></{0}:OpinionInput>")]
	public class OpinionInput : Control, INamingContainer
	{
		private HtmlSelect _PredefinedOpinion = new HtmlSelect();
		private RadioButtonList _RadioButtonOpinion = new RadioButtonList();

		private HtmlTableCell radioCell = new HtmlTableCell();

		//ydz 当时为了添加图片
		private HtmlButton _PredefinedOpinionEdit = new HtmlButton();
		//private HtmlInputButton _PredefinedOpinionEdit = new HtmlInputButton();
		private HtmlTextArea _TextArea = new HtmlTextArea();
		private HtmlInputHidden _OpinionType = new HtmlInputHidden();
		private HtmlGenericControl _TextAreaReadOnly = new HtmlGenericControl("div");
		private HtmlTable _Table = new HtmlTable();
		private HtmlTableRow _PredefinedOpinionRow = null;

		private PredefinedOpinionDialog _predefinedOpinionDlg = new PredefinedOpinionDialog() { InvokeWithoutViewState = true };

		private Label _InvisibleLabel = new Label();

		private GenericOpinion _Opinion = null;

		#region Properties
		[Bindable(false)]
		public string TextInputClientID
		{
			get
			{
				return _TextArea.ClientID;
			}
		}

		[Bindable(false)]
		public string OpinionTypeClientID
		{
			get
			{
				return _OpinionType.ClientID;
			}
		}

		[DefaultValue(typeof(Unit), "")]
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

		[DefaultValue(false)]
		[Bindable(true), Category("Appearance")]
		public bool ReadOnly
		{
			get
			{

				return WebControlUtility.GetViewStateValue(ViewState, "ReadOnly", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ReadOnly", value);
			}
		}

		/// <summary>
		/// 是否显示用户自定义的意见
		/// </summary>
		[DefaultValue(false)]
		[Bindable(true), Category("Appearance")]
		[Description("是否显示用户自定义的意见")]
		public bool ShowPredefinedOpinions
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "ShowPredefinedOpinions", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ShowPredefinedOpinions", value);
			}
		}

		[DefaultValue(true)]
		[Bindable(true), Category("Appearance")]
		[Description("是否在客户端显示(设置Style的Display)")]
		public bool ClientVisible
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "ClientVisible", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ClientVisible", value);
			}
		}

		[
		NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.Attribute),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue((string)null),
		Category("Appearance"),
		Description("字体")
		]
		public FontInfo OpinionFont
		{
			get
			{
				return ControlStyle.Font;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public Style ControlStyle
		{
			get
			{
				return _InvisibleLabel.ControlStyle;
			}
		}

		/// <summary>
		/// 意见对象
		/// </summary>
		[Browsable(false)]
		public GenericOpinion Opinion
		{
			get
			{
				if (Page != null && Page.IsPostBack)
				{
					this._Opinion = LoadOpinionState();

					if (_Opinion != null)
						_Opinion.Content = _TextArea.Value;
				}

				if (_Opinion == null)
					_Opinion = CreateDefaultOpinion();

				return _Opinion;
			}
			set
			{
				_Opinion = value;
			}
		}

		[Browsable(false)]
		internal HtmlTableCell OpinionContainer
		{
			get
			{
				return this.radioCell;
			}
		}

		#endregion Properties

		#region Protected
		protected override void OnInit(EventArgs e)
		{
			this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
			this.Page.PreLoad += new EventHandler(Page_PreLoad);

			base.OnInit(e);

			if (this.Page.IsCallback)
				EnsureChildControls();
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			Controls.Add(_Table);

			HtmlTableRow row = new HtmlTableRow();


			//radioCell.Style["height"] = "16px";
			radioCell.Style["width"] = "49%";
			//_RadioButtonOpinion.ID = "RadioButtonOpinion";
			//_RadioButtonOpinion.AutoPostBack = false;
			//radioCell.Controls.Add(_RadioButtonOpinion);
			row.Cells.Add(radioCell);

			HtmlTableCell btCell = new HtmlTableCell();
			//btCell.Style["height"] = "16px";
			btCell.Style["width"] = "50%";
			btCell.Style["text-align"] = "right";

			HtmlImage lineImage = new HtmlImage();
			lineImage.Src = Page.ClientScript.GetWebResourceUrl(typeof(OpinionInput),
						"MCS.Web.WebControls.Opinion.line.png");
			lineImage.Style[HtmlTextWriterStyle.MarginRight] = "3px";
			lineImage.Style[HtmlTextWriterStyle.MarginTop] = "5px";
			lineImage.Style[HtmlTextWriterStyle.MarginBottom] = "10px";
			lineImage.Style[HtmlTextWriterStyle.Height] = "20px";
			//lineImage.Style[HtmlTextWriterStyle.Width] = "2px";
			btCell.Controls.Add(lineImage);

			_PredefinedOpinion.ID = "PredefinedOpinion";
			_PredefinedOpinion.Style[HtmlTextWriterStyle.MarginBottom] = "10px";
			_PredefinedOpinion.Style[HtmlTextWriterStyle.MarginTop] = "5px";
			_PredefinedOpinion.Style[HtmlTextWriterStyle.Height] = "22px";
			_PredefinedOpinion.EnableViewState = false;
			_PredefinedOpinion.Attributes["onmousewheel"] = "return false;";
			btCell.Controls.Add(_PredefinedOpinion);

			HtmlImage img = new HtmlImage();
			img.Src = Page.ClientScript.GetWebResourceUrl(typeof(OpinionInput),
						"MCS.Web.WebControls.Opinion.pen.png");
			img.Align = "left";
			img.Style[HtmlTextWriterStyle.VerticalAlign] = "middle";
			img.Style[HtmlTextWriterStyle.TextAlign] = "left";
			//img.Style[HtmlTextWriterStyle.PaddingLeft] = "4px";
			img.Style[HtmlTextWriterStyle.PaddingRight] = "4px";
			_PredefinedOpinionEdit.Controls.Add(img);

			HtmlGenericControl wrapper = new HtmlGenericControl("span");
			wrapper.Style[HtmlTextWriterStyle.MarginLeft] = "4px";
			wrapper.Style[HtmlTextWriterStyle.FontFamily] = "'宋体',Arial,Helvetica,sans-serif;";
			wrapper.Style[HtmlTextWriterStyle.FontSize] = "12px";
			wrapper.InnerText = Translator.Translate(Define.DefaultCulture, "编辑");
			_PredefinedOpinionEdit.Controls.Add(wrapper);

			//font-family:'宋体',Arial,Helvetica,sans-serif;margin-right:4px;font-size: 12px
			//_PredefinedOpinionEdit.Value = Translator.Translate(Define.DefaultCulture, "编辑");
			_PredefinedOpinionEdit.Style["margin-left"] = "7px";
			_PredefinedOpinionEdit.Style[HtmlTextWriterStyle.BackgroundColor] = "#FDFCFC";
			_PredefinedOpinionEdit.Style[HtmlTextWriterStyle.BorderColor] = "#A7A7A7";
			_PredefinedOpinionEdit.Style[HtmlTextWriterStyle.BorderWidth] = "1px";
			_PredefinedOpinionEdit.Style[HtmlTextWriterStyle.Height] = "22px";
			_PredefinedOpinionEdit.Style[HtmlTextWriterStyle.MarginTop] = "5px";
			_PredefinedOpinionEdit.Style[HtmlTextWriterStyle.MarginBottom] = "10px";
			//_PredefinedOpinionEdit.Style[HtmlTextWriterStyle.Height] = "26px";
			//_PredefinedOpinionEdit.Style[HtmlTextWriterStyle.Width] = "62px";
			//_PredefinedOpinionEdit.Style[HtmlTextWriterStyle.BackgroundImage] = string.Format("url('{0}')", Page.ClientScript.GetWebResourceUrl(typeof(OpinionInput),
			//            "MCS.Web.WebControls.Opinion.btn-bg.png"));
			//_PredefinedOpinionEdit.Attributes["class"] = "formButton";
			btCell.Controls.Add(_PredefinedOpinionEdit);

			row.Cells.Add(btCell);
			row.Style["display"] = "none";

			HtmlTableCell emptyCell = new HtmlTableCell();
			emptyCell.Style["height"] = "16px";
			emptyCell.Style["width"] = "1%";
			row.Cells.Add(emptyCell);
			/*
			HtmlTableCell cell = new HtmlTableCell();
			row.Controls.Add(cell);
			*/
			_PredefinedOpinionRow = row;

			_Table.Controls.Add(row);

			HtmlTableRow row2 = new HtmlTableRow();
			HtmlTableCell cell2 = new HtmlTableCell() { ColSpan = 3 };

			cell2.Style["vertical-align"] = "top";

			_Table.Controls.Add(row2);
			row2.Controls.Add(cell2);

			SetTextAreaAttributes(_TextArea, "OpinionText");
			_TextArea.Attributes["onpropertychange"] = "onOpinionInputPropertyChange(this);";
			cell2.Controls.Add(_TextArea);

			SetTextAreaAttributes(_TextAreaReadOnly, "OpinionReadOnlyText");
			cell2.Controls.Add(_TextAreaReadOnly);

			_InvisibleLabel.Visible = false;
			Controls.Add(_InvisibleLabel);

			_predefinedOpinionDlg.ID = "PredefinedOpinionDialog";
			Controls.Add(_predefinedOpinionDlg);

			_OpinionType.ID = "OpinionType";
			Controls.Add(_OpinionType);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			_Table.Style["width"] = Width.ToString();
			_Table.Style["height"] = Height.ToString();

			_Table.CellPadding = 0;
			_Table.CellSpacing = 0;

			InitPredefinedOpinion();

			StringBuilder strB = new StringBuilder();

			if (OpinionFont.Overline)
				AppendFontStyle("overline", strB);

			if (OpinionFont.Strikeout)
				AppendFontStyle("line-through", strB);

			if (OpinionFont.Strikeout)
				AppendFontStyle("underline", strB);

			_TextArea.Visible = !ReadOnly;
			_TextAreaReadOnly.Visible = ReadOnly;

			if (ReadOnly)
			{
				SetTextAreaFontStyle(_TextAreaReadOnly, strB.ToString());
				_TextAreaReadOnly.InnerHtml = HttpUtility.HtmlEncode(Opinion.Content).Replace("\r\n", "<br />");
			}
			else
			{
				SetTextAreaFontStyle(_TextArea, strB.ToString());
				RegisterScripts();
			}

			string activityID = string.Empty;

			if (WfClientContext.Current.CurrentActivity != null)
				activityID = WfClientContext.Current.CurrentActivity.ID;

			_PredefinedOpinionEdit.Attributes["onclick"] = string.Format(
				"onEditPredefinedOpinionClick(\"{0}\", \"{1}\", \"{2}\", \"{3}\")",
				_predefinedOpinionDlg.ClientID,
				DeluxeIdentity.CurrentUser.ID,
				_PredefinedOpinion.ClientID,
				activityID);

			if (this.ReadOnly == false)
				SaveOpinionState(this._Opinion);

			if (ClientVisible)
				this._Table.Style[HtmlTextWriterStyle.Display] = "inline";
			else
				this._Table.Style[HtmlTextWriterStyle.Display] = "none";
		}
		#endregion

		#region Private
		private void SaveOpinionState(GenericOpinion opinion)
		{
			string data = string.Empty;

			if (opinion != null)
				data = SerializationHelper.SerializeObjectToString(opinion, SerializationFormatterType.Binary);

			Page.ClientScript.RegisterHiddenField(this.UniqueID, data);
		}

		private GenericOpinion LoadOpinionState()
		{
			GenericOpinion opinion = null;

			string data = HttpContext.Current.Request.Form[this.UniqueID];

			if (data.IsNotEmpty())
				opinion = (GenericOpinion)SerializationHelper.DeserializeStringToObject(data, SerializationFormatterType.Binary);

			return opinion;
		}

		private void Page_PreRenderComplete(object sender, EventArgs e)
		{
			if (DesignMode == false)
				_TextArea.InnerText = Opinion.Content;
		}

		private void RegisterScripts()
		{

			string clientScript =
				ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), this.GetType().Namespace + ".Opinion.OpinionInputClient.js");

			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpinionInput", clientScript, true);

			Page.ClientScript.RegisterStartupScript(this.GetType(), "OpinionInputStartup" + ID,
				string.Format("adjustInputHeight(document.getElementById(\"{0}\"));", _TextArea.ClientID),
				true);
		}

		private void AppendFontStyle(string style, StringBuilder strB)
		{
			if (strB.Length > 0)
				strB.Append(" ");

			strB.Append(style);
		}

		private void InitPredefinedOpinion()
		{
			if (ReadOnly || ShowPredefinedOpinions == false)
				_PredefinedOpinionRow.Style["display"] = "none";
			else
			{
				_PredefinedOpinionRow.Style["display"] = "inline";

				_PredefinedOpinion.Items.Clear();

				string text = GetPridefinedOpinions();
				string[] textArray = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

				_PredefinedOpinion.Style["width"] = "120px";
				_PredefinedOpinion.Items.Add(new ListItem(Translator.Translate(Define.DefaultCulture, "常用意见..."), ""));

				foreach (string t in textArray)
					_PredefinedOpinion.Items.Add(new ListItem(t, t));

				_PredefinedOpinion.Attributes["onchange"] = "document.all('" + _TextArea.ClientID + "').innerText = document.all('" + _PredefinedOpinion.ClientID + "').value";
			}
		}

		private static string GetPridefinedOpinions()
		{
			StringBuilder strB = new StringBuilder();

			if (WfClientContext.Current.OriginalActivity != null)
				strB.Append(WfClientContext.Current.OriginalActivity.Descriptor.Properties.GetValue("PredefinedOpinions", string.Empty));

			string text = UserSettings.LoadSettings(DeluxeIdentity.CurrentUser.ID).GetPropertyValue(
						PredefinedOpinionDialog.CategoryName, PredefinedOpinionDialog.SettingName, string.Empty);

			if (text.IsNotEmpty())
			{
				if (strB.Length > 0)
					strB.Append(" ");

				strB.Append(text);
			}

			return strB.ToString();
		}

		public static void FillOpinionInfoByProcess(GenericOpinion opinion)
		{
			IWfActivity originalActivity = WfClientContext.Current.OriginalCurrentActivity;

			FillOpinionInfoByProcess(opinion, originalActivity);
		}

		public static void FillOpinionInfoByProcess(GenericOpinion opinion, IWfActivity originalActivity)
		{
			if (originalActivity != null)
			{
				opinion.ResourceID = originalActivity.Process.ResourceID;
				opinion.ProcessID = originalActivity.Process.ID;
				opinion.ActivityID = originalActivity.ID;

				IWfActivity rootActivity = originalActivity.OpinionRootActivity;

				if (rootActivity.Process.MainStream != null && rootActivity.MainStreamActivityKey.IsNotEmpty())
				{
					opinion.LevelName = rootActivity.MainStreamActivityKey;
				}
				else
				{
					if (string.IsNullOrEmpty(rootActivity.Descriptor.AssociatedActivityKey))
						opinion.LevelName = rootActivity.Descriptor.Key;
					else
						opinion.LevelName = rootActivity.Descriptor.AssociatedActivityKey;
				}

				if (rootActivity.Process.MainStream != null)
					opinion.LevelDesp = rootActivity.Process.MainStream.Activities[opinion.LevelName].Name;
				else
					opinion.LevelDesp = rootActivity.Descriptor.Process.Activities[opinion.LevelName].Name;
			}

			opinion.FillPersonInfo();
		}

		internal static void SetOpinionType(string opinionType)
		{
			ObjectContextCache.Instance["OpinionType"] = opinionType;
		}

		internal static string GetOpinionType()
		{
			string opinionType = string.Empty;

			if (ObjectContextCache.Instance.ContainsKey("OpinionType"))
			{
				opinionType = (string)ObjectContextCache.Instance["OpinionType"];
			}

			return opinionType;
		}

		private GenericOpinion CreateDefaultOpinion()
		{
			GenericOpinion opinion = new GenericOpinion();

			opinion.ID = UuidHelper.NewUuidString();

			FillOpinionInfoByProcess(opinion);

			opinion.Content = _TextArea.Value;

			return opinion;
		}

		private void SetTextAreaAttributes(HtmlControl control, string id)
		{
			control.ID = id;
			control.Style["width"] = "99%";
			// control.Style["height"] = "99%";
			control.Style["min-height"] = "60px";
			control.Style["word-break"] = "break-all";
			control.Attributes["class"] = "flatInput";
		}

		private void SetTextAreaFontStyle(HtmlControl control, string textDecorations)
		{
			control.Style["font-family"] = OpinionFont.Name;
			control.Style["font-size"] = OpinionFont.Size.Unit.ToString();
			control.Style["font-weight"] = OpinionFont.Bold ? "bold" : string.Empty;
			control.Style["font-style"] = OpinionFont.Italic ? "italic" : string.Empty;

			control.Style["text-decoration"] = textDecorations;
		}

		private void Page_PreLoad(object sender, EventArgs e)
		{
			EnsureChildControls();
		}
		#endregion Private

	}
}
