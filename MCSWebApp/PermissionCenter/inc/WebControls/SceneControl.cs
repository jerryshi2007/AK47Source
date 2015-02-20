using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Web.Library.MVC;

namespace PermissionCenter.WebControls
{
	[ParseChildren(false)]
	[ToolboxData(@"<{0}:SceneControl runat=""server"" />")]
	[Designer("MCS.Web.WebControls.GenericControlDesigner, MCS.Web.WebControls")]
	[NonVisualControl]
	public class SceneControl : Control
	{
		private bool enableScene = true;

		/// <summary>
		/// 获取或设置一个值，表示是否启用场景文件
		/// </summary>
		public bool SceneEnabled
		{
			get { return this.enableScene; }
			set { this.enableScene = value; }
		}

		public static SceneControl GetCurrent(Page page)
		{
			if (page == null)
				throw new ArgumentNullException("page");
			return page.Items[typeof(SceneControl)] as SceneControl;
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (!this.DesignMode)
			{
				Page iPage = this.Page;
				if (GetCurrent(this.Page) != null)
					throw new InvalidOperationException("页面上存在多个SceneControl");
				iPage.Items[typeof(SceneControl)] = this;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (this.SceneEnabled)
			{
				SceneUsageAttribute sceneControlAttr = TypeDescriptor.GetAttributes(this.Page)[typeof(SceneUsageAttribute)] as SceneUsageAttribute;

				if (sceneControlAttr == null)
					throw new HttpException("Page必须使用SceneControlAttribute特性进行标注，才可以使用SceneControl");

				var scenes = ActHelper.GetActs(sceneControlAttr.SceneFileVirtualPath)[sceneControlAttr.ResolveActName(this.Page)].Scenes;

				if (scenes.Count > 0)
				{
					bool isCurrentTime = TimePointContext.Current.UseCurrentTime;
					string sceneName = sceneControlAttr.ResolveSceneName(Page, isCurrentTime);

					scenes[sceneName].RenderToControl(this.Page);

					if (isCurrentTime && this.Page is INormalSceneDescriptor)
					{
						((INormalSceneDescriptor)this.Page).AfterNormalSceneApplied();
					}
				}
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.DesignMode)
			{
				writer.RenderBeginTag(HtmlTextWriterTag.Div);

				base.Render(writer);

				writer.RenderEndTag();
			}
			else
			{
				base.Render(writer);
			}
		}
	}
}