using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Validators;
using MCS.Library.Validation;
using MCS.Library.SOA.DataObjects.Security.Logs;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	public abstract class SCMemberRelativeExecutorBase : SCObjectExecutor
	{
		private SchemaObjectBase _Container = null;
		private SCSimpleRelationBase _Relation = null;
		private bool _RelationExisted = false;
		private bool _NeedContainerStatusCheck = false;

		public SCMemberRelativeExecutorBase(SCOperationType opType, SchemaObjectBase container, SCBase member)
			: base(opType, member)
		{
			container.NullCheck("container");

			container.ClearRelativeData();

			if (member != null)
				member.ClearRelativeData();

			this._Container = container;
			this._Relation = PrepareRelationObject(container, member);
		}

		/// <summary>
		/// 是否需要检查容器的状态
		/// </summary>
		public bool NeedContainerStatusCheck
		{
			get
			{
				return this._NeedContainerStatusCheck;
			}
			set
			{
				this._NeedContainerStatusCheck = value;
			}
		}

		public SchemaObjectBase Parent
		{
			get
			{
				return this._Container;
			}
		}

		public SCSimpleRelationBase Relation
		{
			get
			{
				return this._Relation;
			}
		}

		/// <summary>
		/// 是否覆盖保存已经存在的关系
		/// </summary>
		public bool OverrideExistedRelation
		{
			get;
			set;
		}

		public bool RelationExisted
		{
			get
			{
				return this._RelationExisted;
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

		protected override object DoOperation(SchemaObjectOperationContext context)
		{
			if (OverrideExistedRelation || this.RelationExisted == false)
				SCMemberRelationAdapter.Instance.Update(this.Relation);

			if (SaveTargetData)
				SchemaObjectAdapter.Instance.Update(Data);

			return this.Relation;
		}

		protected override void Validate()
		{
			if (this.NeedValidation == true)
			{
				SchemaPropertyValidatorContext context = SchemaPropertyValidatorContext.Current;

				try
				{
					context.Target = this.Data;
					context.Container = this._Container;

					ValidationResults validationResult = this.Data.Validate();

					ExceptionHelper.TrueThrow(validationResult.ResultCount > 0, validationResult.ToString());

					CheckStatus();
				}
				finally
				{
					SchemaPropertyValidatorContext.Clear();
				}
			}
		}

		protected override void CheckStatus()
		{
			List<SchemaObjectBase> dataToBeChecked = new List<SchemaObjectBase>();

			if (this.NeedStatusCheck)
				dataToBeChecked.Add(this.Data);

			if (this.NeedContainerStatusCheck)
				dataToBeChecked.Add(this._Container);

			CheckObjectStatus(dataToBeChecked.ToArray());
		}

		protected override void PrepareOperationLog(SchemaObjectOperationContext context)
		{
			SCOperationLog log = SCOperationLog.CreateLogFromEnvironment();

			log.ResourceID = this.Data.ID;
			log.SchemaType = this.Data.SchemaType;
			log.OperationType = this.OperationType;
			log.Category = this.Data.Schema.Category;
			log.Subject = string.Format("{0}: {1} 于 {2}",
				EnumItemDescriptionAttribute.GetDescription(this.OperationType), this.Data.Name, ((SCBase)this._Container).Name);

			log.SearchContent = this.Data.ToFullTextString() + " " + this._Container.ToFullTextString();

			context.Logs.Add(log);
		}

		protected abstract SCSimpleRelationBase CreateRelation(SchemaObjectBase container, SchemaObjectBase member);

		private SCSimpleRelationBase PrepareRelationObject(SchemaObjectBase container, SchemaObjectBase member)
		{
			SCSimpleRelationBase relation = SCMemberRelationAdapter.Instance.Load(container.ID, member.ID);

			if (relation == null)
				relation = CreateRelation(container, member);
			else
			{
				if (relation.Status == SchemaObjectStatus.Normal)
					this._RelationExisted = true;
				else
					relation.Status = SchemaObjectStatus.Normal;
			}

			return relation;
		}
	}
}
