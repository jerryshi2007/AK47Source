using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PermissionCenter
{
	/// <summary>
	/// 指定场景的使用方式，无法继承此类
	/// </summary>
	[Serializable]
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public sealed class SceneUsageAttribute : Attribute
	{
		// See the attribute guidelines at 
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		private readonly string sceneFileName;
		private readonly string actName;
		private readonly bool autoActName;

		/// <summary>
		/// 使用指定的文件名初始化<see cref="SceneUsageAttribute"/>，自动确定Act名称
		/// </summary>
		/// <param name="sceneFileVirtualPath">场景控制文件的虚拟路径</param>
		public SceneUsageAttribute(string sceneFileVirtualPath)
		{
			if (sceneFileVirtualPath == null)
				throw new ArgumentNullException("sceneFileName");

			this.sceneFileName = sceneFileVirtualPath;
			this.autoActName = true;
		}

		/// <summary>
		/// 使用指定的文件名和Act名称，初始化<see cref="SceneUsageAttribute"/>
		/// </summary>
		/// <param name="sceneFileVirtualPath">场景控制文件的虚拟路径</param>
		/// <param name="actName">act的名称</param>
		public SceneUsageAttribute(string sceneFileVirtualPath, string actName)
			: this(sceneFileVirtualPath)
		{
			if (actName == null)
				throw new ArgumentNullException("actName");

			this.autoActName = false;
			this.actName = actName;
		}

		/// <summary>
		/// 获取场景的名称
		/// </summary>
		public string SceneFileVirtualPath
		{
			get { return this.sceneFileName; }
		}

		/// <summary>
		/// 如果指定了场景名称，获取场景的名称，否则为 <see langword="null"/> 
		/// </summary>
		public string ActName
		{
			get { return this.actName; }
		}

		/// <summary>
		/// 获取一个值，表示是否自动确定Act名。
		/// </summary>
		/// <value>如果为<see langword="true"/>，表示自动确定场景名称</value>
		public bool AutoDeterminActName
		{
			get { return this.autoActName; }
		}

		/// <summary>
		/// 求Act名
		/// </summary>
		/// <param name="page">页面</param>
		/// <returns></returns>
		internal string ResolveActName(System.Web.UI.Page page)
		{
			if (this.autoActName)
			{
				return page.GetType().BaseType.Name;
			}
			else
			{
				return this.actName;
			}
		}

		/// <summary>
		/// 求场景名称
		/// </summary>
		/// <param name="page"></param>
		/// <param name="isCurrentTime"></param>
		/// <returns></returns>
		internal string ResolveSceneName(System.Web.UI.Page page, bool isCurrentTime)
		{
			if (page is ITimeSceneDescriptor)
			{
				ITimeSceneDescriptor dsc = (ITimeSceneDescriptor)page;
				return isCurrentTime ? dsc.NormalSceneName : dsc.ReadOnlySceneName;
			}
			else
			{
				return isCurrentTime ? "Normal" : "ReadOnly";
			}
		}
	}
}