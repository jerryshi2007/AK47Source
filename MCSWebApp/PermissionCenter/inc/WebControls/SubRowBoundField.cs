using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;
using System.Globalization;
using System.Collections.Specialized;
using System.Runtime;
using System.Web.UI.HtmlControls;

namespace PermissionCenter.WebControls
{
	public class SubRowBoundField : BoundField
	{
		public SubRowBoundField()
		{
		}

		private ITemplate subTemplate;
		[Browsable(false), TemplateContainer(typeof(IDataItemContainer), BindingDirection.TwoWay), DefaultValue((string)null), Description("TemplateField_ItemTemplate"), PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ITemplate SubRowTemplate
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this.subTemplate;
			}
			set
			{
				this.subTemplate = value;
				this.OnFieldChanged();
			}
		}

		public virtual int SubColSpan
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				object o = this.ViewState["subColSpan"];
				if (o != null)
					return (int)o;
				else
					return 1;
			}
			set
			{
				if (value < 1)
					throw new ArgumentOutOfRangeException("value");

				this.ViewState["subColSpan"] = value;
				this.OnFieldChanged();
			}
		}

		protected override void CopyProperties(DataControlField newField)
		{
			//((BoundField)newField).ApplyFormatInEditMode = this.ApplyFormatInEditMode;
			//((BoundField)newField).ConvertEmptyStringToNull = this.ConvertEmptyStringToNull;
			//((BoundField)newField).DataField = this.DataField;
			//((BoundField)newField).DataFormatString = this.DataFormatString;
			//((BoundField)newField).HtmlEncode = this.HtmlEncode;
			//((BoundField)newField).HtmlEncodeFormatString = this.HtmlEncodeFormatString;
			//((BoundField)newField).NullDisplayText = this.NullDisplayText;
			//((BoundField)newField).ReadOnly = this.ReadOnly;
			base.CopyProperties(newField);
		}

		protected override DataControlField CreateField()
		{
			return new SubTemplateField();
		}

		public override void ExtractValuesFromCell(IOrderedDictionary dictionary, DataControlFieldCell cell, DataControlRowState rowState, bool includeReadOnly)
		{
			Control control = null;
			string dataField = this.DataField;
			object text = null;
			string nullDisplayText = this.NullDisplayText;
			if (((rowState & DataControlRowState.Insert) == DataControlRowState.Normal) || this.InsertVisible)
			{
				if (cell.Controls.Count > 0)
				{
					control = cell.Controls[0];
					TextBox box = control as TextBox;
					if (box != null)
					{
						text = box.Text;
					}
				}
				else if (includeReadOnly)
				{
					string s = cell.Text;
					if (s == "&nbsp;")
					{
						text = string.Empty;
					}
					else if (this.SupportsHtmlEncode && this.HtmlEncode)
					{
						text = HttpUtility.HtmlDecode(s);
					}
					else
					{
						text = s;
					}
				}
				if (text != null)
				{
					if (((text is string) && (((string)text).Length == 0)) && this.ConvertEmptyStringToNull)
					{
						text = null;
					}
					if (((text is string) && (((string)text) == nullDisplayText)) && (nullDisplayText.Length > 0))
					{
						text = null;
					}
					if (dictionary.Contains(dataField))
					{
						dictionary[dataField] = text;
					}
					else
					{
						dictionary.Add(dataField, text);
					}
				}
			}
		}

		public override bool Initialize(bool enableSorting, Control control)
		{
			base.Initialize(enableSorting, control);
			return false;
		}

		protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
		{
			Control bindingControl = null;

			if ((((rowState & DataControlRowState.Edit) != DataControlRowState.Normal) && !this.ReadOnly) || ((rowState & DataControlRowState.Insert) != DataControlRowState.Normal))
			{
				TextBox box = new TextBox
				{
					ToolTip = this.HeaderText
				};

				bindingControl = box;
			}
			else if (this.DataField.Length != 0)
			{
				bindingControl = new Label();
			}

			cell.Controls.Add(bindingControl);

			if (this.Visible)
			{
				bindingControl.DataBinding += new EventHandler(this.OnDataBindField);
			}

			cell.Controls.Add(new StrangeControl() { ColSpan = this.SubColSpan });
			HtmlGenericControl cont = new HtmlGenericControl("div");
			cell.Controls.Add(cont);
			if (this.subTemplate != null)
			{
				this.subTemplate.InstantiateIn(cont);
			}
		}

		protected override void LoadViewState(object state)
		{
			//this._dataField = null;
			//this._dataFormatString = null;
			//this._htmlEncodeSet = false;
			//this._htmlEncodeFormatStringSet = false;
			base.LoadViewState(state);
		}

		protected override void OnDataBindField(object sender, EventArgs e)
		{
			Control control = (Control)sender;
			Control namingContainer = control.NamingContainer;
			object dataValue = this.GetValue(namingContainer);
			bool encode = (this.SupportsHtmlEncode && this.HtmlEncode) && (control is Label);
			string str = this.FormatDataValue(dataValue, encode);
			if (control is Label)
			{
				if (str.Length == 0)
				{
					str = "&nbsp;";
				}
				((Label)control).Text = str;
			}
			else
			{
				if (!(control is TextBox))
				{
					throw new HttpException(string.Format("绑定控件，错误的控件类型 {0}", new object[] { this.DataField }));
				}
				if (this.ApplyFormatInEditMode)
				{
					((TextBox)control).Text = str;
				}
				else if (dataValue != null)
				{
					((TextBox)control).Text = dataValue.ToString();
				}
				if ((dataValue != null) && dataValue.GetType().IsPrimitive)
				{
					((TextBox)control).Columns = 5;
				}
			}
		}

		protected virtual void OnSubDataBindField(object sender, EventArgs e)
		{
			Control control = (Control)sender;
			Control namingContainer = control.NamingContainer;
			object dataValue = this.GetValue(namingContainer);
			bool encode = (this.SupportsHtmlEncode && this.HtmlEncode) && (control is Label);
			string str = this.FormatDataValue(dataValue, encode);
			if (control is ITemplate)
			{
				if (str.Length == 0)
				{
					str = "&nbsp;";
				}
			}
		}
	}

	public class StrangeControl : WebControl
	{
		int colSpan = 1;
		public StrangeControl()
			: base("td")
		{
			//base.PreventAutoID();
		}

		public int ColSpan
		{
			get { return this.colSpan; }
			set
			{
				if (value < 1)
					throw new ArgumentOutOfRangeException("value");
				this.colSpan = value;
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write("</td></tr><tr><td colspan=\"" + ColSpan + "\">");
		}
	}
}