using System;
using System.ComponentModel;
using System.Web.UI;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace PermissionCenter.WebControls
{
	[Flags]
	public enum IconSize
	{
		Size16 = 1,
		Size32 = 2,
		Both = 3
	}

	[DefaultProperty("Text")]
	[ToolboxData("<{0}:HeaderControl runat=server></{0}:HeaderControl>")]
	[NonVisualControl]
	public class HeaderControl : Control
	{
		private IconSize size = IconSize.Both;

		[DefaultValue((IconSize)IconSize.Both)]
		public IconSize Size
		{
			get { return this.size; }
			set { this.size = value; }
		}

		protected override void Render(HtmlTextWriter writer)
		{
			this.RenderInlineStyleSheet(writer);
			base.Render(writer);
		}

		protected virtual void RenderInlineStyleSheet(HtmlTextWriter output)
		{
			output.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
			output.RenderBeginTag(HtmlTextWriterTag.Style);
			if ((this.Size & IconSize.Size16) == IconSize.Size16)
			{
				output.Write(".pc-icon16{ display:inline-block; width:16px; height:16px; border:0; padding:0; vertical-align: middle; }");
				foreach (ObjectSchemaConfigurationElement schema in ObjectSchemaSettings.GetConfig().Schemas)
				{
					string key = schema.Name;
					string img = MCS.Web.WebControls.ControlResources.GetResourceByKey(schema.LogoImage + "16");
					output.Write(".pc-icon16." + key + "{");
					output.Write("background: transparent url('{0}') scroll 0 0 ;", img);
					output.Write("}\r\n");
				}
			}

			if ((this.Size & IconSize.Size32) == IconSize.Size32)
			{
				output.Write(".pc-icon32{ display:inline-block; width:32px; height:32px; border:0; padding:0; vertical-align: middle; }");
				foreach (ObjectSchemaConfigurationElement schema in ObjectSchemaSettings.GetConfig().Schemas)
				{
					string key = schema.Name;
					string img = MCS.Web.WebControls.ControlResources.GetResourceByKey(schema.LogoImage + "32");
					output.Write(".pc-icon32." + key + "{");
					output.Write("background: transparent url('{0}') scroll 0 0 ;", img);
					output.Write("}\r\n");
				}
			}

			output.RenderEndTag();
		}
	}
}
