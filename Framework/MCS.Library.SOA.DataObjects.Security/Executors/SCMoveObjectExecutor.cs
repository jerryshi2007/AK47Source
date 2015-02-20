using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Logs;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Tasks;
using MCS.Library.Validation;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	/// <summary>
	/// </summary>
	public class SCMoveObjectExecutor : SCExecutorBase
	{
		private bool _RelationExisted = false;
		private SCBase _Object = null;
		private SCOrganization _OriginalOrganization = null;
		private SCOrganization _TargetOrganization = null;

		private SCRelationObject _OriginalRelation = null;
		private SCRelationObject _TargetRelation = null;

		private bool _NeedGenerateFullPaths = false;
		private bool _NeedChangeOwner = false;
		private bool _NeedStatusCheck = false;

		public SCMoveObjectExecutor(SCOperationType opType, SCOrganization originalOrg, SCBase obj, SCOrganization targetOrg)
			: base(opType)
		{
			obj.NullCheck("obj");
			targetOrg.NullCheck("targetOrg");

			obj.ClearRelativeData();
			targetOrg.ClearRelativeData();

			if (originalOrg != null)
				originalOrg.ClearRelativeData();

			this._Object = obj;
			this._OriginalOrganization = originalOrg;
			this._TargetOrganization = targetOrg;

			this._NeedChangeOwner = PrepareNeedChangeOwner(originalOrg, targetOrg, obj);

			if (originalOrg != null && originalOrg.ID == targetOrg.ID)
			{
				this._RelationExisted = true;
			}
			else
			{
				this._OriginalRelation = PreprareOriginalRelation(originalOrg, obj);
				this._TargetRelation = PrepareTargetRelation(targetOrg, obj);
				this._NeedGenerateFullPaths = (obj is ISCRelationContainer) && (((ISCRelationContainer)obj).GetCurrentChildrenCount() > 0);
			}
		}

		public bool NeedStatusCheck
		{
			get
			{
				return this._NeedStatusCheck;
			}
			set
			{
				this._NeedStatusCheck = value;
			}
		}

		/// <summary>
		/// 是否需要改变Owner
		/// </summary>
		public bool NeedChangeOwner
		{
			get
			{
				return this._NeedChangeOwner;
			}
		}

		/// <summary>
		/// 是否需要全局生成FullPath
		/// </summary>
		public bool NeedGenerateFullPaths
		{
			get
			{
				return this._NeedGenerateFullPaths;
			}
		}

		public SCRelationObject OriginalRelation
		{
			get
			{
				return this._OriginalRelation;
			}
		}

		public SCRelationObject TargetRelation
		{
			get
			{
				return this._TargetRelation;
			}
		}

		public bool RelationExisted
		{
			get
			{
				return this._RelationExisted;
			}
		}

		private SCRelationObject PreprareOriginalRelation(SCOrganization originalOrg, SCBase obj)
		{
			SCRelationObject relation = null;

			if (originalOrg != null)
			{
				relation = SchemaRelationObjectAdapter.Instance.Load(originalOrg.ID, obj.ID);
			}
			else
			{
				relation = obj.CurrentParentRelations.FirstOrDefault();
				this._OriginalOrganization = (SCOrganization)obj.CurrentParents.FirstOrDefault();
			}

			return relation;
		}

		private static bool PrepareNeedChangeOwner(SCOrganization originalOrg, SCOrganization targetOrg, SCBase obj)
		{
			bool result = false;

			if (originalOrg != null && targetOrg != null)
			{
				string originalOwnerID = obj.Properties.GetValue("OwnerID", string.Empty);

				if (originalOwnerID.IsNotEmpty())
				{
					if (originalOrg.ID == originalOwnerID && originalOwnerID != targetOrg.ID)
						result = true;
				}
			}

			return result;
		}

		private SCRelationObject PrepareTargetRelation(SCOrganization targetOrg, SCBase obj)
		{
			SCRelationObject relation = SchemaRelationObjectAdapter.Instance.Load(targetOrg.ID, obj.ID);

			if (relation == null)
			{
				relation = new SCRelationObject(targetOrg, obj);

				if (this._OriginalRelation != null)
				{
					relation.Default = _OriginalRelation.Default;
				}
			}
			else
			{
				if (relation.Status == SchemaObjectStatus.Normal)
					this._RelationExisted = true;
				else
				{
					relation.CalculateFullPathAndGlobalSort(targetOrg, obj);
					relation.Status = SchemaObjectStatus.Normal;
				}
			}

			return relation;
		}

		protected override void PrepareOperationLog(SchemaObjectOperationContext context)
		{
			if (this.RelationExisted == false)
			{
				SCOperationLog log = SCOperationLog.CreateLogFromEnvironment();

				log.ResourceID = this._Object.ID;
				log.SchemaType = this._Object.SchemaType;
				log.OperationType = this.OperationType;
				log.Category = this._Object.Schema.Category;
				log.Subject = string.Format("{0}: {1} 从 {2} 至 {3}",
					EnumItemDescriptionAttribute.GetDescription(this.OperationType), this._Object.Name,
					this._OriginalOrganization.Name, this._TargetOrganization.Name);

				log.SearchContent = this._Object.ToFullTextString() +
					" " + this._OriginalOrganization.ToFullTextString() +
					" " + this._TargetOrganization.ToFullTextString();

				context.Logs.Add(log);
			}
			else
			{
				SCOperationLog log = SCOperationLog.CreateLogFromEnvironment();

				log.ResourceID = this._Object.ID;
				log.SchemaType = this._Object.SchemaType;
				log.OperationType = this.OperationType;
				log.Category = this._Object.Schema.Category;
				log.Subject = string.Format("{0}: {1} 从 {2} 至 {3}（跳过已经存在的关系）",
					EnumItemDescriptionAttribute.GetDescription(this.OperationType), this._Object.Name,
					this._OriginalOrganization.Name, this._TargetOrganization.Name);

				log.SearchContent = this._Object.ToFullTextString() +
					" " + this._OriginalOrganization.ToFullTextString() +
					" " + this._TargetOrganization.ToFullTextString();

				context.Logs.Add(log);
			}
		}

		protected override void PrepareData(SchemaObjectOperationContext context)
		{
			if (this.NeedChangeOwner)
			{
				this._Object.Properties.SetValue("OwnerID", this._TargetOrganization.ID);
				this._Object.Properties.SetValue("OwnerName", this._TargetOrganization.Properties.GetValue("Name", string.Empty));
			}

			Validate();
		}

		protected virtual void Validate()
		{
			#region “验证”
			this._RelationExisted.TrueThrow("无法进行移动，因为对象在目标位置已经存在。");

			ValidationResults validationResults = new ValidationResults();

			ValidationResults relationValidationResults = this._TargetRelation.Validate();

			foreach (ValidationResult result in relationValidationResults)
				validationResults.AddResult(result);

			ExceptionHelper.TrueThrow(validationResults.ResultCount > 0, validationResults.ToString());

			if (this._NeedStatusCheck)
				CheckObjectStatus(this._Object, this._TargetOrganization);
			#endregion
		}

		protected override object DoOperation(SchemaObjectOperationContext context)
		{
			if (this._RelationExisted == false)
			{
				if (this._OriginalRelation.Status == SchemaObjectStatus.Normal)
				{
					this._OriginalRelation.Status = SchemaObjectStatus.Deleted;
					SchemaRelationObjectAdapter.Instance.Update(this._OriginalRelation);
				}

				SchemaRelationObjectAdapter.Instance.Update(this._TargetRelation);

				if (this.NeedChangeOwner)
					SchemaObjectAdapter.Instance.Update(this._Object);

				if (this._NeedGenerateFullPaths)
					SCToDoJobListAdapter.Instance.Insert(SCToDoJob.CreateGenerateFullPathsJob(this._Object));
			}

			return this._TargetRelation;
		}
	}
}
