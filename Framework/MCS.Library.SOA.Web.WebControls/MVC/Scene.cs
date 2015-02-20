using System;
using System.Web;
using System.Xml;
using System.Text;
using System.Web.UI;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MCS.Library.Core;
using MCS.Library.Caching;

namespace MCS.Web.Library.MVC
{
	/// <summary>
	/// 场景
	/// </summary>
	[Serializable]
	public class Scene
	{
		private const string CurrentSceneKey = "CurrentScene";
		private string sceneID = string.Empty;
		private string sceneName = string.Empty;
		private string sceneDesc = string.Empty;
		private SceneItemCollection items = null;

		/// <summary>
		/// 控件重绘策略集合
		/// </summary>
		private static Dictionary<Type, SceneRenderBase> regRenderList = new Dictionary<Type, SceneRenderBase>();

		internal Scene(XmlNode node)
		{
			this.sceneID = XmlHelper.GetAttributeText(node, "sceneID");
			this.sceneName = XmlHelper.GetAttributeText(node, "sceneName");
			this.sceneDesc = XmlHelper.GetAttributeText(node, "sceneDesc");
			this.items = new SceneItemCollection();

			foreach (XmlElement elem in node.SelectNodes("Item"))
				this.items.Add(new SceneItem(elem));
		}

		/// <summary>
		/// 场景ID
		/// </summary>
		public string SceneID
		{
			get
			{
				return this.sceneID;
			}
			set
			{
				this.sceneID = value;
			}
		}

		/// <summary>
		/// 场景名称
		/// </summary>
		public string SceneName
		{
			get { return this.sceneName; }
			set { this.sceneName = value; }
		}

		/// <summary>
		/// 场景描述
		/// </summary>
		public string SceneDesc
		{
			get { return this.sceneDesc; }
			set { this.sceneDesc = value; }
		}

		/// <summary>
		/// 场景设置集合
		/// </summary>
		public SceneItemCollection Items
		{
			get
			{
				if (this.items == null)
					this.items = new SceneItemCollection();

				return this.items;
			}
		}

		/// <summary>
		/// 注册特殊控件重绘方案
		/// </summary>
		/// <param name="type">特殊控件类型</param>
		/// <param name="render">重写SceneRenderBase后的重绘方案</param>
		public static void RegisterRender(System.Type type, SceneRenderBase render)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");
			ExceptionHelper.FalseThrow<ArgumentNullException>(render != null, "render");

			lock (Scene.regRenderList)
			{
				if (Scene.regRenderList.ContainsKey(type) == false)
					Scene.regRenderList.Add(type, render);
			}
		}

		/// <summary>
		/// 重绘当前场景
		/// </summary>
		/// <param name="rootControl"></param>
		public void RenderToControl(Control rootControl)
		{
			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("Scene-RenderToControl",
				() => InnerRenderToControl(rootControl));
		}

		private void InnerRenderToControl(Control rootControl)
		{
			foreach (SceneItem item in this.Items)
			{
				Control ctrl = this.FindControlByPath(rootControl, item.ControlID);

				if (ctrl != null)
				{
					ApplySceneItemToControl(item, ctrl);
				}
			}
		}

		private static void ApplySceneItemToControl(SceneItem item, Control ctrl)
		{
			Type type = ctrl.GetType();

			SceneRenderBase render;

			//检测是否是特殊控件，并已经注册
			if (Scene.regRenderList.TryGetValue(type, out render) == false)
				render = new SceneRenderBase(ctrl.GetType());

			render.RenderEnabled(ctrl, item.Enabled);
			render.RenderReadOnly(ctrl, item.ReadOnly);
			render.RenderVisible(ctrl, item.Visible);
			render.RenderHtmlAttributes(ctrl, item.Attributes);
			render.RenderHtmlStyles(ctrl, item.Styles);
            render.RenderSubItems(ctrl,item.SubItems);

			if (item.Recursive)
			{
				foreach (Control child in ctrl.Controls)
				{
					ApplySceneItemToControl(item, child);
				}
			}
		}

		/// <summary>
		/// 根据控件路径查找控件
		/// </summary>
		/// <param name="rootControl">父控件</param>
		/// <param name="idPath">控件路径</param>
		/// <returns>所查找的控件</returns>
		private Control FindControlByPath(Control rootControl, string idPath)
		{
			int depth = 0;
			string[] idPaths = idPath.Split('.');
			while (idPaths.Length > depth)
			{
				rootControl = rootControl.FindControl(idPaths[depth]);
				depth++;
			}
			return rootControl;
		}

		/// <summary>
		/// 将Scene对象设置为当前的场景，之后使用Scene.Current就可以返回此场景对象。
		/// </summary>
		public void SetCurrent()
		{
			ObjectContextCache.Instance[CurrentSceneKey] = this;
		}

		public void SaveViewState()
		{
			WebUtility.SaveViewStateToCurrentHandler(CurrentSceneKey, this);
		}

		/// <summary>
		/// 当前的场景对象
		/// </summary>
		public static Scene Current
		{
			get
			{
				object scene = null;

				if (ObjectContextCache.Instance.TryGetValue(CurrentSceneKey, out scene) == false)
				{
					if (HttpContext.Current.CurrentHandler is Page)
						if (((Page)HttpContext.Current.CurrentHandler).IsPostBack)
							scene = WebUtility.LoadViewStateFromCurrentHandler(CurrentSceneKey);

					ObjectContextCache.Instance.Add(CurrentSceneKey, scene);
				}

				return (Scene)scene;
			}
		}
	}

	/// <summary>
	/// 场景集合
	/// </summary>
	[Serializable]
	public class SceneCollection : KeyedCollection<string, Scene>
	{
		internal SceneCollection()
		{
		}

		protected override string GetKeyForItem(Scene item)
		{
			return item.SceneID;
		}
	}
}
