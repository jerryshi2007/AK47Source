using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using MCS.Web.Responsive.Library.Script;
using MCS.Web.Responsive.Library.Resources;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects;
using System.ComponentModel;

[assembly: WebResource("MCS.Web.Responsive.WebControls.PropertyForm.PropertyForm.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.PropertyForm.PropertyForm.css", "text/css", PerformSubstitution = true)]

namespace MCS.Web.Responsive.WebControls
{
	[PersistChildren(false)]
	[ParseChildren(true)]
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(ClientMsgResources), 2)]
	[RequiredScript(typeof(ClientPropertyEditorControlBaseResources), 3)]
	[ClientCssResource("MCS.Web.Responsive.WebControls.PropertyForm.PropertyForm.css")]
    [ClientScriptResource("MCS.Web.WebControls.WebControls.PropertyForm", "MCS.Web.Responsive.WebControls.PropertyForm.PropertyForm.js")]
	public class PropertyForm : PropertyEditorControlBase
	{
		private PropertyLayoutCollection _Layouts = new PropertyLayoutCollection();

		public PropertyForm()
			: base()
		{
			JSONSerializerExecute.RegisterConverter(typeof(PropertyLayoutConverter));
		}

		[PersistenceMode(PersistenceMode.InnerProperty), Description("布局属性定义")]
		[MergableProperty(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue((string)null)]
		[Browsable(false)]
		[ScriptControlProperty, ClientPropertyName("formsections")]
		public PropertyLayoutCollection Layouts
		{
			get
			{
				return this._Layouts;
			}
		}

		protected override void LoadClientState(string clientState)
		{
			object[] state = JSONSerializerExecute.Deserialize<object[]>(clientState);

			if (state[0] != null)
				this.Properties.CopyFrom(JSONSerializerExecute.Deserialize<PropertyValueCollection>(state[0]));

			if (state[1] != null)
				this._Layouts.CopyFrom(JSONSerializerExecute.Deserialize<PropertyLayoutCollection>(state[1]));

		}
	}
}
