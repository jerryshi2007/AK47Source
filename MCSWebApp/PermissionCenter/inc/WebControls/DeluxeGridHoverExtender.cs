using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;

namespace PermissionCenter.WebControls
{
	/// <summary>
	/// 为GridView添加客户端Hover行为（仅限权限中心内部使用）
	/// </summary>
	[Description("提供当鼠标移动到Grid行时自动添加或移除CssClass的功能")]
	[TargetControlType(typeof(System.Web.UI.WebControls.GridView))]
	public class DeluxeGridHoverExtender : ExtenderControl
	{
		public DeluxeGridHoverExtender()
		{
			this.HoverCssClass = "hover";
			this.RowItemCssClass = "item aitem";
		}

		/// <summary>
		/// 获取或设置当鼠标移入行时要在tr上添加或当鼠标离开行时要在tr上移除的样式类（如果需要添加/移除多个，则以空格分隔）
		/// </summary>
		[Description("鼠标悬停时应在tr上添加或移除的CssClass，以空格分隔")]
		[Category("Behavior")]
		[DefaultValue("hover")]
		public string HoverCssClass { get; set; }

		/// <summary>
		/// 获取或设置需要应用Hover行为的tr元素的CssClass。（如果有多个，则以空格分隔）
		/// </summary>
		[Description("应用样式行为的tr元素的CssClass，以空格分隔，关系为或")]
		[Category("Behavior")]
		[DefaultValue("item aitem")]
		public string RowItemCssClass { get; set; }

		protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors(Control targetControl)
		{
			ScriptBehaviorDescriptor descriptor = new ScriptBehaviorDescriptor("PermissionCenter.GridHoverBehavior", targetControl.ClientID);
			descriptor.AddProperty("hoverCssClass", this.HoverCssClass);
			descriptor.AddProperty("rowItemCssClass", this.RowItemCssClass);
			return new ScriptDescriptor[] { descriptor };
		}

		protected override IEnumerable<ScriptReference> GetScriptReferences()
		{
			ScriptReference reference = new ScriptReference();
			reference.Assembly = "PermissionCenter";
			reference.Name = "PermissionCenter.PcGridHoverBehavior.js";

			return new ScriptReference[] { reference };
		}
	}
}