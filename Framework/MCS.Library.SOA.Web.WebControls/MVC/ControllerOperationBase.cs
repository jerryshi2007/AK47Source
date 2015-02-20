using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;

namespace MCS.Web.Library.MVC
{
	public abstract class ControllerOperationBase
	{
		/// <summary>
		/// 当流程对象已经加载（初始化）完成
		/// </summary>
		public ProcessReadyHandler ProcessReady;

		/// <summary>
		/// 流程对象加载（初始化）完成后，准备CommandState
		/// </summary>
		public PrepareCommandStateHandler PrepareCommandState;

		public IRequestCommand NavigationCommand { get; set; }
		public SceneInfoAttribute SceneInfo { get; set; }
		internal protected abstract void DoOperation();

		#region Protected
		protected void TransferToTargetView()
		{
			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("TransferToTargetView", () =>
				{
					if (NavigationCommand != null)
						NavigationCommand.Execute();
				});
		}

		protected virtual void OnProcessReady(IWfProcess process)
		{
			if (ProcessReady != null)
			{
				PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("ProcessReady",
					() => ProcessReady(process));
			}
		}

		protected virtual void OnPrepareCommandState(IWfProcess process)
		{
			if (PrepareCommandState != null)
			{
				PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("PrepareCommandState", () =>
					{
						CommandStateBase state = PrepareCommandState(process);

						if (state != null)
							CommandStateHelper.RegisterState(state);
					});
			}
		}

		protected void SetSceneByActivity(IWfActivity activity)
		{
			activity.NullCheck("activity");

			string scene = activity.Descriptor.InheritedScene;

			if (scene.IsNullOrEmpty())
			{
				if (SceneInfo != null)
					scene = SceneInfo.DefaultWorkflowSceneID;
			}

			if (scene.IsNotEmpty())
				OnSetScene(scene);
		}

		protected void SetReadOnlyScene(IWfActivity activity)
		{
			string scene = activity.Descriptor.InheritedReadOnlyScene;

			if (scene.IsNullOrEmpty())
			{
				if (SceneInfo != null)
					scene = SceneInfo.ReadOnlySceneID;
			}

			if (scene.IsNotEmpty())
				OnSetScene(scene);
		}

		protected virtual void OnSetScene(string sceneKey)
		{
			if (SceneInfo != null && sceneKey.IsNotEmpty())
			{
				Act act = ActHelper.GetActs(SceneInfo.SceneFileVirtualPath)[SceneInfo.ActID];

				(act != null).FalseThrow("不能在{0}场景文件中找到actID为{1}的Act", SceneInfo.SceneFileVirtualPath, SceneInfo.ActID);

				if (act.Scenes.Contains(sceneKey))
				{
					Scene scene = act.Scenes[sceneKey];

					scene.SetCurrent();
				}
			}
		}
		#endregion
	}
}
