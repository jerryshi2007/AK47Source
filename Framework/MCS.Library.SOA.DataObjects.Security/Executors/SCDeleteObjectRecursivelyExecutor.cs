using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Locks;
using MCS.Library.SOA.DataObjects.Security.Logs;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	/// <summary>
	/// 递归删除所有对象
	/// </summary>
	public class SCDeleteObjectsRecursivelyExecutor : SCExecutorBase
	{
		private SCDeleteCandidateCollection _Candidates = new SCDeleteCandidateCollection();
		private SchemaObjectCollection _OriginalObjectsToDelete = null;
		private SCOrganization _Parent = null;
		private bool _NeedStatusCheck = false;

		public SCDeleteObjectsRecursivelyExecutor(SCOrganization parent, SchemaObjectCollection objectsToDelete) :
			base(SCOperationType.DeleteObjectsRecursively)
		{
			parent.NullCheck("parent");
			objectsToDelete.NullCheck("objectsToDelete");

			this._Parent = parent;
			this._OriginalObjectsToDelete = objectsToDelete;
			this.AutoStartTransaction = false;
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
				this._NeedStatusCheck = value;
			}
		}

		protected override void PrepareData(SchemaObjectOperationContext context)
		{
			CheckObjectStatus(this._Parent);

			foreach (SchemaObjectBase sourceObj in this._OriginalObjectsToDelete)
			{
				PrepareCandidatesRecursively(sourceObj);

				AddCandidates(this._Parent, false, sourceObj);
			}

			ProcessProgress.Current.MinStep = 0;
			ProcessProgress.Current.MaxStep = this._Candidates.Count;
			ProcessProgress.Current.CurrentStep = 0;

			ProcessProgress.Current.Response();
		}

		protected override void PrepareOperationLog(SchemaObjectOperationContext context)
		{
		}

		private void PrepareCandidatesRecursively(SchemaObjectBase sourceObj)
		{
			if (sourceObj is ISCRelationContainer)
			{
				SchemaObjectCollection objs = ((ISCRelationContainer)sourceObj).CurrentChildren;

				foreach (SchemaObjectBase obj in objs)
				{
					PrepareCandidatesRecursively(obj);
					AddCandidates(sourceObj, true, obj);
				}
			}
		}

		private void AddCandidates(SchemaObjectBase parent, bool deletedByContainer, SchemaObjectBase obj)
		{
			this._Candidates.Add(new SCDeleteCandidate(parent, deletedByContainer, obj));
			obj.ClearRelativeData();

			ProcessProgress.Current.StatusText = string.Format("已经准备了{0:#,##0}个待删除数据", this._Candidates.Count);
			ProcessProgress.Current.Response();
		}

		protected override object DoOperation(SchemaObjectOperationContext context)
		{
			int deletedCount = 0;

			foreach (SCDeleteCandidate candidate in this._Candidates)
			{
				string objName = candidate.Object.Properties.GetValue("Name", string.Empty);

				try
				{
					SCObjectOperations.Instance.DoOperation(SCObjectOperationMode.Delete, candidate.Object, candidate.Parent, candidate.DeletedByContainer);

					ProcessProgress.Current.StatusText = string.Format("已删除\"{0}\"", objName);
					ProcessProgress.Current.Increment();
					ProcessProgress.Current.Response();

					if (SCDataOperationLockContext.Current.Lock != null && (deletedCount) % 5 == 0)
						SCDataOperationLockContext.Current.ExtendLock();

					deletedCount++;
				}
				catch (System.Exception ex)
				{
					throw new ApplicationException(string.Format("删除对象{0}({1})出错: {2}", objName, candidate.Object.ID, ex.Message));
				}
			}

			SCOperationLog log = SCOperationLog.CreateLogFromEnvironment();

			log.ResourceID = this._Parent.ID;
			log.SchemaType = this._Parent.SchemaType;
			log.OperationType = this.OperationType;
			log.Category = this._Parent.Schema.Category;
			log.Subject = string.Format("{0}: 总计应删除{1}中的 {2:#,##0} 个对象，实际删除{3:#,##0}个对象",
				EnumItemDescriptionAttribute.GetDescription(this.OperationType), this._Parent.Name,
				this._Candidates.Count.ToString(), deletedCount);
			log.SearchContent = this._Parent.ToFullTextString();

			context.Logs.Add(log);

			ProcessProgress.Current.StatusText = string.Format("总共删除了{0:#,##0}个对象", deletedCount);
			ProcessProgress.Current.Response();

			return this._Parent;
		}
	}

	/// <summary>
	/// 待删除的对象
	/// </summary>
	internal class SCDeleteCandidate
	{
		public SCDeleteCandidate()
		{
		}

		public SCDeleteCandidate(SchemaObjectBase parent, bool deletedByContainer, SchemaObjectBase obj)
		{
			this.Parent = parent;
			DeletedByContainer = deletedByContainer;
			this.Object = obj;
		}

		public SchemaObjectBase Parent
		{
			get;
			set;
		}

		public bool DeletedByContainer
		{
			get;
			set;
		}

		public SchemaObjectBase Object
		{
			get;
			set;
		}
	}

	internal class SCDeleteCandidateCollection : EditableDataObjectCollectionBase<SCDeleteCandidate>
	{
	}
}
