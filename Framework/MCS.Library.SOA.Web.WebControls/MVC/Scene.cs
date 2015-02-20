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
	/// ����
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
		/// �ؼ��ػ���Լ���
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
		/// ����ID
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
		/// ��������
		/// </summary>
		public string SceneName
		{
			get { return this.sceneName; }
			set { this.sceneName = value; }
		}

		/// <summary>
		/// ��������
		/// </summary>
		public string SceneDesc
		{
			get { return this.sceneDesc; }
			set { this.sceneDesc = value; }
		}

		/// <summary>
		/// �������ü���
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
		/// ע������ؼ��ػ淽��
		/// </summary>
		/// <param name="type">����ؼ�����</param>
		/// <param name="render">��дSceneRenderBase����ػ淽��</param>
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
		/// �ػ浱ǰ����
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

			//����Ƿ�������ؼ������Ѿ�ע��
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
		/// ���ݿؼ�·�����ҿؼ�
		/// </summary>
		/// <param name="rootControl">���ؼ�</param>
		/// <param name="idPath">�ؼ�·��</param>
		/// <returns>�����ҵĿؼ�</returns>
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
		/// ��Scene��������Ϊ��ǰ�ĳ�����֮��ʹ��Scene.Current�Ϳ��Է��ش˳�������
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
		/// ��ǰ�ĳ�������
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
	/// ��������
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
