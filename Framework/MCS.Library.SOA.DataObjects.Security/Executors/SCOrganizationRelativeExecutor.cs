using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.Validation;
using MCS.Library.SOA.DataObjects.Security.Logs;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	/// <summary>
	/// 和组织结构内对象相关的Executor的基类
	/// </summary>
	public class SCOrganizationRelativeExecutor : SCObjectExecutor
	{
		private SCOrganization _Parent = null;
		private SCRelationObject _Relation = null;
		private bool _RelationExisted = false;
		private bool _OverrideDefault = false;
		private SCAclContainer _AclContainer = null;
		private SCParentsRelationObjectCollection _TargetParentRelations = null;
		private bool _NeedParentStatusCheck = false;
		private bool _NeedDuplicateRelationCheck = false;

		public SCOrganizationRelativeExecutor(SCOperationType opType, SCOrganization parent, SCBase data)
			: base(opType, data)
		{
			parent.NullCheck("organization");

			data.ClearRelativeData();
			parent.ClearRelativeData();

			this._Parent = parent;
			this._Relation = PrepareRelationObject(parent, data);

			if (this.OperationType == SCOperationType.AddOrganization)
				this._AclContainer = PrepareAclContainer(parent, data);

			if (data is SCUser)
			{
				this._TargetParentRelations = data.CurrentParentRelations;

				if (this.OperationType == SCOperationType.AddUser && this._TargetParentRelations.Count == 0)
				{
					SCUser user = (SCUser)data;

					user.OwnerID = parent.ID;
					user.OwnerName = parent.Properties.GetValue("Name", string.Empty);
				}
			}
		}

		public bool OverrideDefault
		{
			get
			{
				return this._OverrideDefault;
			}
			set
			{
				this._OverrideDefault = value;
			}
		}

		public SCOrganization Parent
		{
			get
			{
				return this._Parent;
			}
		}

		public SCRelationObject Relation
		{
			get
			{
				return this._Relation;
			}
		}

		/// <summary>
		/// 是否同时保存目标数据
		/// </summary>
		public bool SaveTargetData
		{
			get;
			set;
		}

		/// <summary>
		/// 是否覆盖保存已经存在的关系
		/// </summary>
		public bool OverrideExistedRelation
		{
			get;
			set;
		}

		/// <summary>
		/// 是否需要校验对象间是否有重复的关系
		/// </summary>
		public bool NeedDuplicateRelationCheck
		{
			get
			{
				return this._NeedDuplicateRelationCheck;
			}
			set
			{
				this._NeedDuplicateRelationCheck = value;
			}
		}

		/// <summary>
		/// 是否需要检查父对象的状态
		/// </summary>
		public bool NeedParentStatusCheck
		{
			get
			{
				return this._NeedParentStatusCheck;
			}
			set
			{
				this._NeedParentStatusCheck = value;
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

			log.ResourceID = this.Data.ID;
			log.SchemaType = this.Data.SchemaType;
			log.OperationType = this.OperationType;
			log.Category = this.Data.Schema.Category;


			log.Subject = string.Format("{0}: 对象 {1}， 组织 {2}",
				EnumItemDescriptionAttribute.GetDescription(this.OperationType), this.Data.Name, this._Parent.Name);

			log.SearchContent = this.Data.ToFullTextString() +
				" " + this.Parent.ToFullTextString();

			context.Logs.Add(log);
		}

		private SCRelationObject PrepareRelationObject(SCOrganization parent, SCBase data)
		{
			SCRelationObject relation = SchemaRelationObjectAdapter.Instance.Load(parent.ID, data.ID);

			if (relation == null)
				relation = new SCRelationObject(parent, data);
			else
			{
				if (relation.Status == SchemaObjectStatus.Normal)
					this._RelationExisted = true;
				else
					relation.Status = SchemaObjectStatus.Normal;
			}

			return relation;
		}

		protected override void DoValidate(Validation.ValidationResults validationResults)
		{
			base.DoValidate(validationResults);

			if (this.Relation.Status == SchemaObjectStatus.Normal)
			{
				ValidationResults currentResults = this.Relation.Validate();

				foreach (ValidationResult result in currentResults)
					validationResults.AddResult(result);
			}
		}

		protected override void CheckStatus()
		{
			List<SchemaObjectBase> dataToBeChecked = new List<SchemaObjectBase>();

			if (this.NeedStatusCheck)
				dataToBeChecked.Add(this.Data);

			if (this.NeedParentStatusCheck)
				dataToBeChecked.Add(this.Parent);

			CheckObjectStatus(dataToBeChecked.ToArray());

			if (this.NeedDuplicateRelationCheck)
			{
				if (this.Data.CurrentParentRelations.Count > 0 && this.Data.CurrentParentRelations.Exists(r => r.ParentID != this.Parent.ID))
				{
					throw new SCStatusCheckException(string.Format("对象\"{0}\"已经属于组织\"{1}了，不能再添加到别的组织中",
						this.Data.ToSimpleObject().Name, this.Parent.ToSimpleObject().Name));
				}
			}
		}

		protected override object DoOperation(SchemaObjectOperationContext context)
		{
			//需要考虑默认关系的情况
			if (this._TargetParentRelations != null)
			{
				//如果是插入或更新
				if (this.Relation.Status == SchemaObjectStatus.Normal)
				{
					if (this._OverrideDefault)
					{
						this._TargetParentRelations.ForEach(r =>
							{
								if (r.Default == true && r.ParentID != this.Relation.ParentID)
								{
									r.Default = false;
									SchemaRelationObjectAdapter.Instance.Update(r);
								}
							}
							);
					}
					else
					{
						if (this._TargetParentRelations.Exists(r => r.Default == true && r.ParentID != this.Relation.ParentID))
							this.Relation.Default = false;
					}
				}
				else
				{
					if (this.Relation.Default == true)
					{
						SCRelationObject firstNotDefaultRelation = this._TargetParentRelations.Find(r => r.Default == false && r.ParentID != this.Relation.ParentID);

						if (firstNotDefaultRelation != null)
						{
							firstNotDefaultRelation.Default = true;
							SchemaRelationObjectAdapter.Instance.Update(firstNotDefaultRelation);
						}
					}
				}
			}

			//当不是删除操作，且需要修改关系时
			if (this.Data.Status == SchemaObjectStatus.Normal && (OverrideExistedRelation || this.RelationExisted == false))
				SchemaRelationObjectAdapter.Instance.Update(this.Relation);

			if (SaveTargetData)
			{
				SchemaObjectAdapter.Instance.Update(Data);

				this.DoRelativeDataOperation(context);
			}

			if (this._AclContainer != null)
			{
				SCAclAdapter.Instance.Update(this._AclContainer);
			}

			return this.Relation;
		}

		private SCAclContainer PrepareAclContainer(SCOrganization parent, SCBase currentData)
		{
			SCAclContainer result = null;

			if (currentData is ISCAclContainer)
			{
				result = new SCAclContainer(currentData);

				result.Members.CopyFrom(SCAclAdapter.Instance.LoadByContainerID(parent.ID, DateTime.MinValue));
			}

			return result;
		}
	}
}
