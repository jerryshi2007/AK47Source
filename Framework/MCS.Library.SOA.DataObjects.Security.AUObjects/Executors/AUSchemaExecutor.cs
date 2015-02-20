using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Actions;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	internal class AUSchemaExecutor : AUObjectExecutor
	{
		private SchemaObjectCollection allCurrentAUs;
		private PendingActionCollection pendingActions = new PendingActionCollection();

		public AUSchemaExecutor(AUOperationType type, SchemaObjectBase schema)
			: base(type, schema)
		{
			if (type != AUOperationType.AddAdminSchema && type != AUOperationType.RemoveAdminSchema && type != AUOperationType.UpdateAdminSchema)
				throw new NotSupportedException("不支持在此Executor中使用操作" + type);
		}

		protected override void PrepareData(AUObjectOperationContext context)
		{
			base.PrepareData(context);
			var originalSchema = SCActionContext.Current.OriginalObject as AUSchema;
			var targetSchema = (AUSchema)this.Data;

			pendingActions.Clear();

			PrepareScopeItems(context, originalSchema, targetSchema);
		}

		private void PrepareScopeItems(AUObjectOperationContext context, AUSchema originalSchema, AUSchema targetSchema)
		{
			var srcArray = originalSchema != null ? originalSchema.Scopes.Split(AUCommon.Spliter, StringSplitOptions.RemoveEmptyEntries) : AUCommon.ZeroLengthStringArray;
			var targetArray = targetSchema.Scopes.Split(AUCommon.Spliter, StringSplitOptions.RemoveEmptyEntries);

			Array.Sort(srcArray);
			Array.Sort(targetArray);

			var toBeAddedScopes = GetToBeAddedScopes(srcArray, targetArray);
			var toBeDeletedScopes = GetToBeRemovedScopes(srcArray, targetArray);
			//角色不在此修改

			this.allCurrentAUs = Adapters.AUSnapshotAdapter.Instance.LoadAUBySchemaID(targetSchema.ID, true, DateTime.MinValue);
			if (allCurrentAUs.Any() && this.Data.Status == SchemaObjectStatus.Deleted)
				throw new InvalidOperationException("必须先删除所有与此管理架构相关的管理单元才可以进行Schema删除。");

			if (toBeAddedScopes.Length > 0 || toBeDeletedScopes.Length > 0)
			{
				if (this.Data.Status == SchemaObjectStatus.Deleted)
					throw new InvalidOperationException("进行删除操作时，不应修改管理范围。");

				PrepareToBeAddedScopes(toBeAddedScopes, pendingActions);

				PrepareToBeDeletedScopes(toBeDeletedScopes, pendingActions);
			}
		}

		private void PrepareToBeDeletedScopes(string[] items, PendingActionCollection pendingActions)
		{
			foreach (string scope in items)
			{
				foreach (AdminUnit au in allCurrentAUs)
				{
					var auScope = Adapters.AUSnapshotAdapter.Instance.LoadAUScope(au.ID, scope, true, DateTime.MinValue).GetUniqueNormalObject<AUAdminScope>();
					if (auScope != null)
					{
						pendingActions.Add(new ClearConditionAction(auScope));
						pendingActions.Add(new ClearContainerAction(auScope));
						pendingActions.Add(new RemoveMemberAction(au, auScope));
					}
				}
			}
		}

		private void PrepareToBeAddedScopes(string[] items, PendingActionCollection pendingActions)
		{
			foreach (string scope in items)
			{
				foreach (AdminUnit au in allCurrentAUs)
				{
					var existed = Adapters.AUSnapshotAdapter.Instance.LoadAUScope(au.ID, scope, false, false, DateTime.MinValue);
					if (existed.Count == 0)
						pendingActions.Add(new AddMemberAction(au, new AUAdminScope() { ID = UuidHelper.NewUuidString(), ScopeSchemaType = scope }));
					else
					{
						// 尽量恢复已经删除的Scope，而不是重新创建一个
						if (existed.GetUniqueNormalObject<AUAdminScope>() == null)
						{
							AUAdminScope onlyScope = (AUAdminScope)existed.First();
							onlyScope.Status = SchemaObjectStatus.Normal;
							pendingActions.Add(new EnableMemberAction(au, onlyScope));
						}

						// 否则，这个Scope已经是正常状态了？
					}
				}
			}
		}

		/// <summary>
		/// 从已经排序的数组中获取差集
		/// </summary>
		/// <param name="srcArray"></param>
		/// <param name="targetArray"></param>
		/// <returns></returns>
		private string[] GetToBeRemovedScopes(string[] srcArray, string[] targetArray)
		{
			System.Collections.ArrayList tempArray = new System.Collections.ArrayList();
			for (int i = srcArray.Length - 1; i >= 0; i--)
			{
				string a = srcArray[i];
				int index = Array.BinarySearch<string>(targetArray, a);
				if (index < 0)
					tempArray.Add(a);
			}

			return tempArray.Count > 0 ? (string[])tempArray.ToArray(typeof(string)) : AUCommon.ZeroLengthStringArray;
		}

		/// <summary>
		/// 从已经排序的数组中获取补集
		/// </summary>
		/// <param name="srcArray"></param>
		/// <param name="targetArray"></param>
		/// <returns></returns>
		private string[] GetToBeAddedScopes(string[] srcArray, string[] targetArray)
		{
			System.Collections.ArrayList tempArray = new System.Collections.ArrayList();
			for (int i = targetArray.Length - 1; i >= 0; i--)
			{
				string a = targetArray[i];
				int index = Array.BinarySearch<string>(srcArray, a);
				if (index < 0)
					tempArray.Add(a);
			}

			return tempArray.Count > 0 ? (string[])tempArray.ToArray(typeof(string)) : AUCommon.ZeroLengthStringArray;
		}

		protected override void DoRelativeDataOperation(AUObjectOperationContext context)
		{
			base.DoRelativeDataOperation(context);

			this.pendingActions.DoActions();
		}

		private SCSimpleRelationBase CreateRelation(SchemaObjectBase container, SchemaObjectBase member)
		{
			return new SCMemberRelation(container, member);
		}

		protected override void PrepareOperationLog(AUObjectOperationContext context)
		{
			base.PrepareOperationLog(context);
		}
	}
}
