using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Logs;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.Validation;
using MCS.Library.SOA.DataObjects.Security.Tasks;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	public class MoveAUExecutor : AUExecutorBase
	{
		private bool _RelationExisted = false;
		private AdminUnit _Object = null;
		private SchemaObjectBase _SourceObject;
		private SchemaObjectBase _Target = null;
		private SCRelationObject _OriginalRelation = null;
		private SCRelationObject _TargetRelation = null;

		private bool _NeedGenerateFullPaths = false;
		private bool _NeedStatusCheck = false;
		private SchemaObjectBase _ActualTarget;

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

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="opType">操作类型</param>
		/// <param name="obj">要移动的管理单元</param>
		/// <param name="target">目标管理单元 或为 null 表示移动到Schema </param>
		public MoveAUExecutor(AUOperationType opType, AdminUnit obj, AdminUnit target)
			: base(opType)
		{
			(this._Object = obj).NullCheck("obj");
			this._Target = target;
		}

		private SCRelationObject PrepareTargetRelation(SchemaObjectBase targetOrg, AdminUnit obj)
		{
			SCRelationObject relation = SchemaRelationObjectAdapter.Instance.Load(targetOrg.ID, obj.ID);

			if (relation == null)
			{
				relation = new SCRelationObject(targetOrg, obj) { Default = true };
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

		protected override void PrepareOperationLog(AUObjectOperationContext context)
		{
			if (this.RelationExisted == false)
			{
				AUOperationLog log = AUOperationLog.CreateLogFromEnvironment();

				log.ResourceID = this._Object.ID;
				log.SchemaType = this._Object.SchemaType;
				log.OperationType = this.OperationType;
				log.Category = this._Object.Schema.Category;
				log.Subject = string.Format("{0}: {1} 从 {2} 至 {3}",
					EnumItemDescriptionAttribute.GetDescription(this.OperationType), this._Object.Name,
					AUCommon.DisplayNameFor(this._SourceObject), AUCommon.DisplayNameFor(this._ActualTarget));

				log.SearchContent = this._Object.ToFullTextString() +
					" " + AUCommon.FullTextFor(this._SourceObject) +
					" " + this._ActualTarget.ToFullTextString();

				context.Logs.Add(log);
			}
			else
			{
				AUOperationLog log = AUOperationLog.CreateLogFromEnvironment();

				log.ResourceID = this._Object.ID;
				log.SchemaType = this._Object.SchemaType;
				log.OperationType = this.OperationType;
				log.Category = this._Object.Schema.Category;
				log.Subject = string.Format("{0}: {1} 从 {2} 至 {3}（跳过已经存在的关系）",
					EnumItemDescriptionAttribute.GetDescription(this.OperationType), this._Object.Name,
					AUCommon.DisplayNameFor(this._SourceObject), AUCommon.DisplayNameFor(this._Target));

				log.SearchContent = this._Object.ToFullTextString() +
					" " + AUCommon.DisplayNameFor(this._SourceObject) +
					" " + this._ActualTarget.ToFullTextString();

				context.Logs.Add(log);
			}
		}

		protected override void PrepareData(AUObjectOperationContext context)
		{
			this._OriginalRelation = this._Object.GetCurrentVeryParentRelation();

			if (this._OriginalRelation == null || this._OriginalRelation.Status != SchemaObjectStatus.Normal)
				throw new AUObjectValidationException("未找到此管理单元对应的上级关系");

			AUCommon.DoDbAction(() =>
			{
				this._SourceObject = this._OriginalRelation.Parent;
				this._ActualTarget = this._Target ?? SchemaObjectAdapter.Instance.Load(this._Object.AUSchemaID);
			});

			if (this._SourceObject == null || this._SourceObject.Status != SchemaObjectStatus.Normal)
				throw new AUObjectValidationException("未找到此管理单元对应的上级对象");

			if (this._OriginalRelation.ParentID == this._ActualTarget.ID)
				throw new AUObjectValidationException("此管理单元已经属于目标管理单元，无需移动。");
			AUCommon.DoDbAction(() =>
			{
				this._TargetRelation = PrepareTargetRelation(this._ActualTarget, this._Object);
				this._NeedGenerateFullPaths = (this._Object is ISCRelationContainer) && (((ISCRelationContainer)this._Object).CurrentChildren.Count > 0);

				Validate();
			});
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
				CheckObjectStatus(this._Object, this._ActualTarget);
			#endregion
		}

		protected override object DoOperation(AUObjectOperationContext context)
		{
			if (this._RelationExisted == false)
			{
				if (this._OriginalRelation.Status == SchemaObjectStatus.Normal)
				{
					this._OriginalRelation.Status = SchemaObjectStatus.Deleted;
					AUCommon.DoDbAction(() =>
						SchemaRelationObjectAdapter.Instance.UpdateStatus(this._OriginalRelation, SchemaObjectStatus.Deleted));
				}

				AUCommon.DoDbAction(() =>
					SchemaRelationObjectAdapter.Instance.Update(this._TargetRelation));

				//if (this.NeedChangeOwner)
				//    SchemaObjectAdapter.Instance.Update(this._Object);

				if (this._NeedGenerateFullPaths)
					AUCommon.DoDbAction(() =>
						SCToDoJobListAdapter.Instance.Insert(SCToDoJob.CreateGenerateFullPathsJob(this._Object)));
			}

			return this._TargetRelation;
		}
	}
}
