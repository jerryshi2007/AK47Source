using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Logs;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	/// <summary>
	/// 角色和权限
	/// </summary>
	public class SCJoinRoleAndPermissionExecutor : SCExecutorBase
	{
		private SCRelationObject _Relation = null;
		private bool _RelationExisted = false;
		private SCRole _Role = null;
		private SCPermission _Permission = null;
		private bool _NeedStatusCheck = false;

		public SCJoinRoleAndPermissionExecutor(SCOperationType opType, SCRole role, SCPermission permission)
			: base(opType)
		{
			role.NullCheck("role");
			permission.NullCheck("permission");

			role.ClearRelativeData();
			permission.ClearRelativeData();

			this._Relation = PrepareRelationObject(role, permission);

			this._Role = role;
			this._Permission = permission;
		}

		/// <summary>
		/// 是否进行状态检查
		/// </summary>
		public bool NeedStatusCheck
		{
			get
			{
				return this._NeedStatusCheck;
			}
			set
			{
				this._NeedStatusCheck = false;
			}
		}

		public SCRelationObject Relation
		{
			get
			{
				return this._Relation;
			}
		}

		public bool RelationExisted
		{
			get
			{
				return this._RelationExisted;
			}
		}

		protected override void PrepareOperationLog(SchemaObjectOperationContext context)
		{
			SCOperationLog log = SCOperationLog.CreateLogFromEnvironment();

			log.ResourceID = this._Role.ID;
			log.SchemaType = this._Role.SchemaType;
			log.OperationType = this.OperationType;
			log.Category = this._Role.Schema.Category;


			log.Subject = string.Format("{0}: 替角色 {1} {2} 权限 {3}",
				EnumItemDescriptionAttribute.GetDescription(this.OperationType), this._Role.Name, (this._Relation.Status == SchemaObjectStatus.Normal ? "赋予" : "解除"),
				this._Permission.Name);

			log.SearchContent = this._Role.ToFullTextString() +
				" " + this._Permission.ToFullTextString();

			context.Logs.Add(log);
		}

		/// <summary>
		/// 是否覆盖保存已经存在的关系
		/// </summary>
		public bool OverrideExistedRelation
		{
			get;
			set;
		}

		private SCRelationObject PrepareRelationObject(SCRole role, SCPermission permission)
		{
			SCRelationObject relation = SchemaRelationObjectAdapter.Instance.Load(role.ID, permission.ID);

			if (relation == null)
				relation = new SCRelationObject(role, permission);
			else
			{
				if (relation.Status == SchemaObjectStatus.Normal)
					this._RelationExisted = true;
				else
					relation.Status = SchemaObjectStatus.Normal;
			}

			return relation;
		}

		protected override void PrepareData(SchemaObjectOperationContext context)
		{
			base.PrepareData(context);

			if (NeedStatusCheck)
				CheckObjectStatus(this._Role, this._Permission);
		}

		protected override object DoOperation(SchemaObjectOperationContext context)
		{
			if (OverrideExistedRelation || this.RelationExisted == false)
				SchemaRelationObjectAdapter.Instance.Update(this.Relation);

			return this.Relation;
		}
	}
}
