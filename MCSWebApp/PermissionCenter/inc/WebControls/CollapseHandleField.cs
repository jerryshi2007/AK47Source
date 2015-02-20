using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI.HtmlControls;
using System.Web.UI;

namespace PermissionCenter.WebControls
{
	public class CollapseHandleField : DataControlField
	{
		protected override DataControlField CreateField()
		{
			return new CollapseHandleField();
		}

		protected override void CopyProperties(DataControlField newField)
		{
			base.CopyProperties(newField);
			((CollapseHandleField)(newField)).OpenCssClass = this.OpenCssClass;
			((CollapseHandleField)(newField)).CollapseCssClass = this.CollapseCssClass;
		}

		[System.Web.UI.CssClassProperty]
		[Description("折叠时的Css样式")]
		public string CollapseCssClass
		{
			get
			{
				return ((string)this.ViewState["collapseCssClass"]) ?? string.Empty;
			}

			set
			{
				if (object.Equals(value, this.ViewState["collapseCssClass"]) == false)
				{
					this.ViewState["collapseCssClass"] = value ?? string.Empty;
					this.OnFieldChanged();
				}
			}
		}

		[System.Web.UI.CssClassProperty]
		[Description("展开时的Css样式")]
		public string OpenCssClass
		{
			get
			{
				return ((string)this.ViewState["openCssClass"]) ?? string.Empty;
			}

			set
			{
				if (object.Equals(value, this.ViewState["openCssClass"]) == false)
				{
					this.ViewState["openCssClass"] = value ?? string.Empty;
					this.OnFieldChanged();
				}
			}
		}

		public string OnClientCollapse
		{
			get
			{
				return ((string)this.ViewState["clientCollapse"]) ?? string.Empty;
			}

			set
			{
				if (object.Equals(value, this.ViewState["clientCollapse"]) == false)
				{
					this.ViewState["clientCollapse"] = value ?? string.Empty;
					this.OnFieldChanged();
				}
			}
		}

		public string OnClientOpen
		{
			get
			{
				return ((string)this.ViewState["clientOpen"]) ?? string.Empty;
			}

			set
			{
				if (object.Equals(value, this.ViewState["clientOpen"]) == false)
				{
					this.ViewState["clientOpen"] = value ?? string.Empty;
					this.OnFieldChanged();
				}
			}
		}

		public override void ValidateSupportsCallback()
		{
			//base.ValidateSupportsCallback();
		}

		public override bool Initialize(bool sortingEnabled, System.Web.UI.Control control)
		{
			if (control.Page != null)
			{
				ClientScriptManager clientScript = control.Page.ClientScript;
				string script = @"
function CollapseHandleField_getAttribute(elem,name){
	if(elem.getAttribute)
		return elem.getAttribute(name);
	else
		return elem[name];
}
function CollapseHandleField_toggleVisible(obj,open){
	var callBackName = null;
	if(open){
		obj.className = CollapseHandleField_getAttribute(obj,'data-openCssClass');
		callBackName = CollapseHandleField_getAttribute(obj,'data-clientOpen');		
	}else{
		obj.className = CollapseHandleField_getAttribute(obj,'data-collapseCssClass');
		callBackName = CollapseHandleField_getAttribute(obj,'data-clientCollapse');		
	}

	obj.onclick = function(){ CollapseHandleField_toggleVisible(obj,!open);};
	if(callBackName){
		var fun = window[callBackName];
		if(typeof(fun) === 'function'){
			fun();
		}
	}
	
}


";
				clientScript.RegisterClientScriptBlock(typeof(CollapseHandleField), "", script, true);
			}

			return base.Initialize(sortingEnabled, control);
		}

		public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
		{
			base.InitializeCell(cell, cellType, rowState, rowIndex);
			if (cellType == DataControlCellType.DataCell)
			{
				if (((rowState & DataControlRowState.Insert) == DataControlRowState.Normal) && base.Visible)
				{
					HtmlGenericControl lnk = new HtmlGenericControl("span");
					lnk.EnableViewState = false;
					lnk.Attributes["class"] = this.CollapseCssClass;
					lnk.Attributes["data-collapseCssClass"] = this.CollapseCssClass;
					lnk.Attributes["data-openCssClass"] = this.OpenCssClass;
					lnk.Attributes["onclick"] = "CollapseHandleField_toggleVisible(this,1)";
					lnk.Attributes["data-clientCollapse"] = this.OnClientCollapse;
					lnk.Attributes["data-clientOpen"] = this.OnClientOpen;
					cell.Controls.Add(lnk);
				}
			}
		}
	}
}