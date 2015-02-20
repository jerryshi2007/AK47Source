using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using MCS.Web.Responsive.Library.Script;
using MCS.Web.Responsive.Library.Resources;
using System.ComponentModel;
using MCS.Web.Library.Script;
using System.Web;
using MCS.Library.SOA.DataObjects;

[assembly: WebResource("MCS.Web.Responsive.WebControls.PropertyGrid.PropertyGrid.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.PropertyGrid.PropertyGrid.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.Responsive.WebControls.PropertyGrid.Sprites.gif", "image/gif")]

namespace MCS.Web.Responsive.WebControls
{
    [PersistChildren(false)]
    [ParseChildren(true)]
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(ClientMsgResources), 2)]
    [RequiredScript(typeof(ClientPropertyEditorControlBaseResources), 3)]
    [ClientCssResource("MCS.Web.Responsive.WebControls.PropertyGrid.PropertyGrid.css")]
    [ClientScriptResource("MCS.Web.WebControls.PropertyGrid", "MCS.Web.Responsive.WebControls.PropertyGrid.PropertyGrid.js")]
    public class PropertyGrid : PropertyEditorControlBase
    {
        public PropertyGrid()
            : base()
        {
        }

        [DefaultValue("")]
        [ClientPropertyName("caption")]
        [ScriptControlProperty]
        public string Caption
        {
            get
            {
                return GetPropertyValue("Caption", string.Empty);
            }
            set
            {
                SetPropertyValue("Caption", value);
            }
        }

        [DefaultValue(PropertyGridDisplayOrder.ByCategory)]
        [ClientPropertyName("displayOrder")]
        [ScriptControlProperty]
        public PropertyGridDisplayOrder DisplayOrder
        {
            get
            {
                return GetPropertyValue("DisplayOrder", PropertyGridDisplayOrder.ByCategory);
            }
            set
            {
                SetPropertyValue("DisplayOrder", value);
            }
        }

        [Browsable(false)]
        public string SpriteImageUrl
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "MCS.Web.Responsive.WebControls.PropertyGrid.Sprites.gif");
            }
        }

        protected override void LoadClientState(string clientState)
        {
            if (string.IsNullOrEmpty(clientState) == false)
                this.Properties.CopyFrom(JSONSerializerExecute.Deserialize<PropertyValueCollection>(clientState));
        }

    }
}
