using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Security.Logs;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	/// <summary>
	/// 修改对象权限的Executor
	/// </summary>
	public class SCUpdateObjectAclExecutor : SCExecutorBase
	{
		private SCAclContainer _Container = null;

		private SCBase _ContainerObject = null;

		public SCUpdateObjectAclExecutor(SCOperationType opType, SCAclContainer container)
			: base(opType)
		{
			container.NullCheck("container");

			this._Container = container;
		}

		public SCAclContainer Container
		{
			get
			{
				return this._Container;
			}
		}

		protected override void PrepareData(SchemaObjectOperationContext context)
		{
			base.PrepareData(context);
			this._ContainerObject = (SCBase)SchemaObjectAdapter.Instance.Load(this._Container.ContainerID);

			if (this._ContainerObject == null || this._ContainerObject.Status != SchemaObjectStatus.Normal)
				throw new SCStatusCheckException("ACL容器对象无效");
	
			if ((this._ContainerObject is ISCAclContainer) == false)
				throw new SCStatusCheckException(string.Format("ACL容器对象无效：{0}不实现ISCAclContainer接口", this._ContainerObject.ToDescription()));
		}

		protected override object DoOperation(SchemaObjectOperationContext context)
		{
			SCAclAdapter.Instance.Update(this.Container);

			return this.Container;
		}

		protected override void PrepareOperationLog(SchemaObjectOperationContext context)
		{
			SCOperationLog log = SCOperationLog.CreateLogFromEnvironment();

			var obj = (SCBase)this._ContainerObject;
			log.ResourceID = obj.ID;
			log.SchemaType = obj.SchemaType;
			log.OperationType = this.OperationType;
			log.Category = obj.Schema.Category;
			log.Subject = string.Format("{0}: {1}",
				EnumItemDescriptionAttribute.GetDescription(this.OperationType), this._ContainerObject.DisplayName);

			log.SearchContent = this._ContainerObject.ToFullTextString();

			context.Logs.Add(log);
		}
	}
}
