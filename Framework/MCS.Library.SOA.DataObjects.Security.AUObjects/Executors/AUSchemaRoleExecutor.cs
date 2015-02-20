using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Logs;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	/// <summary>
	/// 管理单元架构角色定义的执行器
	/// </summary>
	internal class AUSchemaRoleExecutor : AUMemberRelativeExecutor
	{
		private static readonly string[] emptyStringArray = { };
		private SchemaObjectCollection adminUnits; //所有相关的管理单元
		private PendingActionCollection pendingActions = new PendingActionCollection();

		public AUSchemaRoleExecutor(AUOperationType type, AUSchema schema, AUSchemaRole data)
			: base(type, schema, data)
		{
			switch (type)
			{
				case AUOperationType.AddSchemaRole:
				case AUOperationType.RemoveSchemaRole:
					break;
				default:
					throw new ArgumentOutOfRangeException("type", string.Format("AUSchemaRoleExecutor不支持{0}操作", type));
			}
		}

		protected override SCSimpleRelationBase CreateRelation(SchemaObjectBase container, SchemaObjectBase member)
		{
			return new SCMemberRelation(container, member);
		}

		protected override void PrepareData(AUObjectOperationContext context)
		{
			base.PrepareData(context);
			this.pendingActions.Clear();

			if (this.Data.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal)
			{
				//添加
				this.adminUnits = Adapters.AUSnapshotAdapter.Instance.LoadAUBySchemaID(this.Container.ID, true, DateTime.MinValue);
				foreach (AdminUnit unit in this.adminUnits)
				{
					var existedRole = Adapters.AUSnapshotAdapter.Instance.LoadAURole(this.Data.ID, unit.ID, false, DateTime.MinValue);
					if (existedRole == null)
					{
						pendingActions.Add(new AddMemberAction(unit, new AURole() { ID = UuidHelper.NewUuidString(), SchemaRoleID = this.Data.ID }));
					}
					else if (existedRole.Status != Schemas.SchemaProperties.SchemaObjectStatus.Normal)
					{
						pendingActions.Add(new EnableMemberAction(unit, existedRole));
					}
				}
			}
			else
			{
				//只是删掉角色
				var existedRoles = Adapters.AUSnapshotAdapter.Instance.LoadAURoles(this.Data.ID, true, DateTime.MinValue);

				foreach (var item in existedRoles)
				{
					RelationHelper.Instance.ClearContainer(item);
					pendingActions.Add(new DeleteSelfAction(item));
				}
			}
		}

		protected override object DoOperation(AUObjectOperationContext context)
		{
			object result = base.DoOperation(context);

			this.pendingActions.DoActions();

			this.DoRelativeDataOperation(context);

			return result;
		}

		protected override void DoValidate(Validation.ValidationResults validationResults)
		{
			base.DoValidate(validationResults);
		}

		protected override void PrepareOperationLog(AUObjectOperationContext context)
		{
			AUOperationLog log = AUOperationLog.CreateLogFromEnvironment();

			log.ResourceID = this.Data.ID;
			log.SchemaType = this.Data.SchemaType;
			log.OperationType = this.OperationType;
			log.Category = this.Data.Schema.Category;
			log.Subject = string.Format("{0}: {1} 于 {2}",
				EnumItemDescriptionAttribute.GetDescription(this.OperationType), AUCommon.DisplayNameFor(this.Data), AUCommon.DisplayNameFor(this.Container));

			//log.SearchContent = this.Data.ToFullTextString() + " " + this.Container.ToFullTextString();

			context.Logs.Add(log);
		}
	}
}
