using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using MCS.Library.SOA.DataObjects.Security.Logs;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Security.Tasks;
using MCS.Library.Validation;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Actions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Schemas.Actions;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	internal class AUObjectExecutor : AUExecutorBase
	{
		private SchemaObjectBase _Data = null;
		private bool _NeedValidation = true;
		private bool _NeedDeleteRelations = false;
		private bool _NeedDeleteMemberRelations = false;
		private bool _ObjectNameChanged = false;
		private bool _NeedGenerateFullPaths = false;
		private bool _NeedDeleteConditions = false;
		private bool _NeedStatusCheck = false;

		public AUObjectExecutor(AUOperationType opType, SchemaObjectBase data)
			: base(opType)
		{
			data.NullCheck("data");

			data.ClearRelativeData();
			this._Data = data;
		}

		/// <summary>
		/// 对象的Name属性是否改变
		/// </summary>
		public bool ObjectNameChanged
		{
			get
			{
				return this._ObjectNameChanged;
			}
		}

		/// <summary>
		/// 是否需要状态检查
		/// </summary>
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

		/// <summary>
		/// 是否删除相关的条件对象
		/// </summary>
		public bool NeedDeleteConditions
		{
			get
			{
				return this._NeedDeleteConditions;
			}
			set
			{
				this._NeedDeleteConditions = value;
			}
		}

		public bool NeedValidation
		{
			get
			{
				return this._NeedValidation;
			}
			set
			{
				this._NeedValidation = value;
			}
		}

		public bool NeedDeleteRelations
		{
			get
			{
				return this._NeedDeleteRelations;
			}
			set
			{
				this._NeedDeleteRelations = value;
			}
		}

		/// <summary>
		/// 是否连带删除成员关系
		/// </summary>
		public bool NeedDeleteMemberRelations
		{
			get
			{
				return this._NeedDeleteMemberRelations;
			}
			set
			{
				this._NeedDeleteMemberRelations = value;
			}
		}

		public SchemaObjectBase Data
		{
			get
			{
				return this._Data;
			}
		}

		protected override void PrepareData(AUObjectOperationContext context)
		{
			SCActionContext.Current.OriginalObject = SchemaObjectAdapter.Instance.Load(this.Data.ID);
			SCActionContext.Current.CurrentObject = this.Data;

			string originalName = SCActionContext.Current.OriginalObject != null ? SCActionContext.Current.OriginalObject.Properties.GetValue("Name", string.Empty) : string.Empty;
			string currentName = SCActionContext.Current.OriginalObject != null ? SCActionContext.Current.CurrentObject.Properties.GetValue("Name", string.Empty) : string.Empty;

			this._ObjectNameChanged = originalName != currentName;

			//是否是关系容器
			ISCRelationContainer rContainer = this.Data as ISCRelationContainer;

			if (this._ObjectNameChanged)
			{
				this._NeedGenerateFullPaths = (rContainer != null && rContainer.CurrentChildren.Count > 0);

				this.Data.CurrentParentRelations.ForEach(relation =>
				{
					if (relation.FullPath.IsNotEmpty())
					{
						int index = relation.FullPath.LastIndexOf(originalName);

						if (index >= 0)
							relation.FullPath = relation.FullPath.Substring(0, index) + currentName;
					}
				});

				context["parentRelations"] = this.Data.CurrentParentRelations;
			}

			this.Validate();

			if (this._NeedDeleteRelations)
			{
				//加载关系对象，然后为了统一删除和打标志。
				if (rContainer != null)
					context["childrenRelations"] = rContainer.CurrentChildrenRelations;
				context["parentRelations"] = this.Data.CurrentParentRelations;
			}

			if (this._NeedDeleteMemberRelations)
			{
				if (this.Data is ISCMemberObject)
					context["containersRelations"] = ((ISCMemberObject)this.Data).GetCurrentMemberOfRelations();

				if (this.Data is ISCContainerObject)
					context["membersRelations"] = ((ISCContainerObject)this.Data).GetCurrentMembersRelations();
			}

			if (this.Data.Status != SchemaObjectStatus.Normal)
			{
				if (this.Data is ISCAclContainer)
					context["aclMembers"] = ((ISCAclContainer)this.Data).GetAclMembers();

				if (this.Data is ISCAclMember)
					context["aclContainers"] = ((ISCAclMember)this.Data).GetAclContainers();
			}

			base.PrepareData(context);
		}

		/// <summary>
		/// 通常重载此方法来进行校验工作
		/// </summary>
		/// <param name="validationResults"></param>
		protected virtual void DoValidate(ValidationResults validationResults)
		{
			if (this.Data.Status == SchemaObjectStatus.Normal)
			{
				ValidationResults dataValidationResults = this.Data.Validate();

				foreach (ValidationResult result in dataValidationResults)
					validationResults.AddResult(result);

				foreach (SCRelationObject relation in this.Data.CurrentParentRelations)
				{
					ValidationResults relationValidationResults = relation.Validate();

					foreach (ValidationResult result in relationValidationResults)
						validationResults.AddResult(result);
				}
			}
		}

		/// <summary>
		/// 验证当前数类型
		/// </summary>
		protected virtual void Validate()
		{
			#region “验证”
			if (this._NeedValidation)
			{
				ValidationResults validationResults = new ValidationResults();

				DoValidate(validationResults);

				if (validationResults.ResultCount > 0)
					throw new AUObjectValidationException(validationResults.ToString(), this.Data);
			}

			CheckStatus();
			#endregion
		}

		/// <summary>
		/// 验证对象的状态
		/// </summary>
		protected virtual void CheckStatus()
		{
			if (this._NeedStatusCheck)
				CheckObjectStatus(this.Data);
		}

		protected override object DoOperation(AUObjectOperationContext context)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				SchemaObjectAdapter.Instance.Update(Data);

				DoRelativeDataOperation(context);

				scope.Complete();
			}

			return this.Data;
		}

		/// <summary>
		/// 在派生类中重写时，处理相关数据
		/// </summary>
		/// <param name="context"></param>
		protected virtual void DoRelativeDataOperation(AUObjectOperationContext context)
		{
			if (this._ObjectNameChanged)
			{
				if (context.ContainsKey("parentRelations"))
					UpdateRelations((IEnumerable<SCRelationObject>)context["parentRelations"]);
			}

			if (this._NeedDeleteRelations)
			{
				if (context.ContainsKey("childrenRelations"))
					UpdateRelationsStatus((IEnumerable<SCRelationObject>)context["childrenRelations"]);

				if (context.ContainsKey("parentRelations"))
					UpdateRelationsStatus((IEnumerable<SCRelationObject>)context["parentRelations"]);
			}

			if (context.ContainsKey("containersRelations"))
				UpdateMembersRelationsStatus((IEnumerable<SCSimpleRelationBase>)context["containersRelations"]);

			if (context.ContainsKey("membersRelations"))
				UpdateMembersRelationsStatus((IEnumerable<SCSimpleRelationBase>)context["membersRelations"]);

			if (context.ContainsKey("aclMembers"))
				SCAclAdapter.Instance.UpdateStatus(((SCAclContainerOrMemberCollectionBase)context["aclMembers"]), SchemaObjectStatus.Deleted);

			if (context.ContainsKey("aclContainers"))
				SCAclAdapter.Instance.UpdateStatus(((SCAclContainerOrMemberCollectionBase)context["aclContainers"]), SchemaObjectStatus.Deleted);

			if (this._NeedGenerateFullPaths)
				SCToDoJobListAdapter.Instance.Insert(SCToDoJob.CreateGenerateFullPathsJob(this.Data));

			if (this._NeedDeleteConditions && this.Data is ISCUserContainerWithConditionObject && this.Data.Status != SchemaObjectStatus.Normal)
				AUConditionAdapter.Instance.DeleteByOwner(this.Data.ID, "Default");
		}

		private static void UpdateRelations(IEnumerable<SCRelationObject> relations)
		{
			relations.ForEach(r => SchemaRelationObjectAdapter.Instance.Update(r));
		}

		private static void UpdateRelationsStatus(IEnumerable<SCRelationObject> relations)
		{
			relations.ForEach(r => SchemaRelationObjectAdapter.Instance.UpdateStatus(r, SchemaObjectStatus.Deleted));
		}

		private static void UpdateMembersRelationsStatus(IEnumerable<SCSimpleRelationBase> relations)
		{
			relations.ForEach(r => SCMemberRelationAdapter.Instance.UpdateStatus(r, SchemaObjectStatus.Deleted));
		}

		protected override void PrepareOperationLog(AUObjectOperationContext context)
		{
			context.Logs.Add(AUCommon.ToOperationLog(this.Data, this.OperationType));
		}
	}
}
