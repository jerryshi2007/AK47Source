using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Web.UI;
using System.Threading;
using MCS.Library.Core;

namespace MCS.Web.Library.MVC
{
	/// <summary>
	/// MVC模式中，页面的抽象基类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ViewBase<T> : System.Web.UI.Page where T : CommandStateBase
	{
		private T viewData = default(T);

		/// <summary>
		/// 视图数据
		/// </summary>
		protected T ViewData
		{
			get
			{
				return this.viewData;
			}
		}

		/// <summary>
		/// CommandState初始化后，不是对话框状态，也不是CallBack状态才调用此方法。用于使用数据绑定控件
		/// </summary>
		protected virtual void CommandStateInitialized()
		{
		}

		protected override void InitializeCulture()
		{
			this.Culture = Thread.CurrentThread.CurrentUICulture.Name;
		}

		protected override void CreateChildControls()
		{
			this.viewData = (T)CommandStateHelper.GetCommandState(typeof(T));
			base.CreateChildControls();
		}

		protected override void OnInit(EventArgs e)
		{
			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("ViewBase-Init",
				() => base.OnInit(e));
		}

		protected override void OnInitComplete(EventArgs e)
		{
			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("ViewBase-InitComplete",
					() => base.OnInitComplete(e));
		}

		protected override void  OnLoadComplete(EventArgs e)
		{
			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("ViewBase-LoadComplete",
					() => base.OnLoadComplete(e));
		}

		protected override void OnLoad(EventArgs e)
		{
			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("ViewBase-Load", () =>
				{
					base.OnLoad(e);

					if (this.viewData != null)
					{
						if (this.IsCallback == false)
						{
							PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("CommandStateInitialized",
								() => CommandStateInitialized());
						}
					}
				});
		}

		/// <summary>
		/// 重载OnPreRender
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			if (Scene.Current != null)
				Scene.Current.RenderToControl(this);

			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("ViewBase-PreRender",
				() => base.OnPreRender(e));
		}

		/// <summary>
		/// 重载Render，输出性能信息
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("ViewBase-Render",
				() => base.Render(writer));
		}

		/// <summary>
		/// PreRender之后，准备保存ViewState
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRenderComplete(EventArgs e)
		{
			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("ViewBase-PreRenderComplete", () =>
				{
					CommandStateHelper.SaveViewState();

					if (Scene.Current != null)
						Scene.Current.SaveViewState();

					base.OnPreRenderComplete(e);
				});
		}

		/// <summary>
		/// 重载基类加载ViewState的过程
		/// </summary>
		/// <returns></returns>
		protected override object LoadPageStateFromPersistenceMedium()
		{
			object result = null;

			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("LoadPageStateFromPersistenceMedium",
				() => result = InnerLoadPageStateFromPersistenceMedium());

			return result;
		}

		private object InnerLoadPageStateFromPersistenceMedium()
		{
			object result = null;

			ViewStatePersistSettings settings = ViewStatePersistSettings.GetConfig();

			try
			{
				if (settings.Enabled)
				{
					PageStatePersister persister = settings.Persister;

					this.PageStatePersister.Load();
					persister.Load();

					result = new Pair(persister.ControlState, persister.ViewState);
				}
				else
					result = base.LoadPageStateFromPersistenceMedium();
			}
			catch (System.Web.HttpException)
			{
				if (Page.IsCallback == false)
					throw;
			}

			return result;
		}

		/// <summary>
		/// 重载基类保存ViewState的过程
		/// </summary>
		/// <param name="state"></param>
		protected override void SavePageStateToPersistenceMedium(object state)
		{
			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("SavePageStateToPersistenceMedium",
				() => InnerSavePageStateToPersistenceMedium(state));
		}

		private void InnerSavePageStateToPersistenceMedium(object state)
		{
			ViewStatePersistSettings settings = ViewStatePersistSettings.GetConfig();

			if (settings.Enabled)
			{
				PageStatePersister persister = settings.Persister;

				if (state is Pair)
				{
					Pair statePair = (Pair)state;

					persister.ControlState = statePair.First;
					persister.ViewState = statePair.Second;
				}
				else
					persister.ViewState = state;

				persister.Save();

				this.PageStatePersister.ViewState = persister.ViewState;
				this.PageStatePersister.ControlState = persister.ControlState;

				this.PageStatePersister.Save();
			}
			else
				base.SavePageStateToPersistenceMedium(state);
		}
	}
}
