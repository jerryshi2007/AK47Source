using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Globalization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 编辑流程、活动、线的属性的执行器的基类
	/// </summary>
	public abstract class WfEditPropertiesExecutorBase<T> : WfExecutorBase where T : IWfKeyedDescriptor
	{
		private IWfProcess _Process = null;
		private T _Descriptor = default(T);
		private bool _SyncMainStreamObject = false;

		public WfEditPropertiesExecutorBase(IWfActivity operatorActivity, IWfProcess process, T descriptor, bool syncMSObject, WfControlOperationType operationType)
			: base(operatorActivity, operationType)
		{
			process.NullCheck("process");
			descriptor.NullCheck("descriptor");

			this._Process = process;
			this._Descriptor = descriptor;
			this._SyncMainStreamObject = syncMSObject;

			descriptor.SyncPropertiesToFields();
		}

		public IWfProcess Process
		{
			get
			{
				return this._Process;
			}
		}

		public T Descriptor
		{
			get
			{
				return this._Descriptor;
			}
		}

		public bool SyncMainStreamObject
		{
			get
			{
				return this._SyncMainStreamObject;
			}
		}

		protected override IWfProcess OnGetCurrentProcess()
		{
			return this.Process;
		}

		protected override void OnModifyWorkflow(WfExecutorDataContext dataContext)
		{
			if (this.SyncMainStreamObject && this.Process.MainStream != null)
				SyncMainStreamObjectProperties();

			WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this.Process);
		}

		/// <summary>
		/// 找到主线活动中对应的对象
		/// </summary>
		/// <returns></returns>
		protected virtual T FindMainStreamObject()
		{
			return default(T);
		}

		protected virtual void MergeMainStreamProperties(T targetDescriptor)
		{
			MergePropertyValues(targetDescriptor, this.Descriptor.Properties, targetDescriptor.Properties);
		}

		/// <summary>
		/// 初始化日志的标题
		/// </summary>
		/// <param name="dataContext"></param>
		/// <param name="log"></param>
		protected override void OnPrepareUserOperationLogDescription(WfExecutorDataContext dataContext, UserOperationLog log)
		{
			base.OnPrepareUserOperationLogDescription(dataContext, log);

			log.Subject = Translator.Translate(Define.DefaultCulture, "修改了对象{0}的属性", this.Descriptor.Key);
		}

		private void SyncMainStreamObjectProperties()
		{
			T msDescriptor = FindMainStreamObject();

			if (msDescriptor != null)
			{
				MergeMainStreamProperties(msDescriptor);
				msDescriptor.SyncPropertiesToFields();
			}
		}

		/// <summary>
		/// 将sourceProperties同步到targetProperties中
		/// </summary>targetProperties
		/// <param name="sourceProperties"></param>
		/// <param name="targetProperties"></param>
		private void MergePropertyValues(T targetDescriptor, PropertyValueCollection sourceProperties, PropertyValueCollection targetProperties)
		{
			string originalKey = targetProperties.GetValue("Key", string.Empty);

			targetProperties.ReplaceExistedPropertyValues(sourceProperties);

			targetProperties.SetValue("Key", originalKey);

			targetDescriptor.SyncPropertiesToFields();
		}
	}
}
