using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Locks;
using MCS.Library.SOA.DataObjects.Security.Logs;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	/// <summary>
	/// 递归替换Acl的操作
	/// </summary>
	public class SCReplaceAclRecursivelyExecutor : SCExecutorBase
	{
		private SchemaObjectCollection _Candidates = new SchemaObjectCollection();
		private ISCAclContainer _Parent = null;
		private bool _ForceReplace = true;

		public SCReplaceAclRecursivelyExecutor(SCOperationType opType, ISCAclContainer parent) :
			base(opType)
		{
			parent.NullCheck("parent");

			this._Parent = parent;
			this.AutoStartTransaction = false;
		}

		/// <summary>
		/// 是否强制覆盖不继承的对象
		/// </summary>
		public bool ForceReplace
		{
			get
			{
				return this._ForceReplace;
			}
			set
			{
				this._ForceReplace = value;
			}
		}

		protected override void PrepareOperationLog(SchemaObjectOperationContext context)
		{
			SCOperationLog log = SCOperationLog.CreateLogFromEnvironment();

			log.ResourceID = ((SchemaObjectBase)this._Parent).ID;
			log.SchemaType = ((SchemaObjectBase)this._Parent).SchemaType;
			log.OperationType = this.OperationType;
			log.Category = ((SchemaObjectBase)this._Parent).Schema.Category;
			log.Subject = string.Format("{0}: {1}",
				EnumItemDescriptionAttribute.GetDescription(this.OperationType), ((SCBase)this._Parent).Name);

			log.SearchContent = ((SchemaObjectBase)this._Parent).ToFullTextString();

			context.Logs.Add(log);
		}

		protected override void PrepareData(SchemaObjectOperationContext context)
		{
			PrepareCandidatesRecursively(this._Parent);

			if (this._Candidates.Count > 0)
			{
				ProcessProgress.Current.MinStep = 0;
				ProcessProgress.Current.MaxStep = this._Candidates.Count;
				ProcessProgress.Current.CurrentStep = 0;
			}
		}

		protected override object DoOperation(Adapters.SchemaObjectOperationContext context)
		{
			int replacedCount = 0;

			//取父级的权限定义
			var aclMembers = Adapters.SCAclAdapter.Instance.LoadByContainerID(((SchemaObjectBase)(this._Parent)).ID, DateTime.MinValue);

			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("ID");
			inBuilder.AppendItem((from acl in aclMembers select acl.MemberID).ToArray());

			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			if (inBuilder.IsEmpty)
				where.AppendItem("1", "2");

			var memberObjects = Adapters.SchemaObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(inBuilder, where), DateTime.MinValue);

			if (this._Candidates.Count > 0)
			{
				ProcessProgress.Current.MaxStep = this._Candidates.Count;
				ProcessProgress.Current.CurrentStep = 0;

				ProcessProgress.Current.Output.WriteLine("正在替换子对象ACL");

				foreach (SchemaObjectBase candidate in this._Candidates)
				{
					string objName = candidate.Properties.GetValue("Name", string.Empty);

					try
					{
						Debug.Assert(this._Parent != null, "容器对象为null");

						var oldItems = SCAclAdapter.Instance.LoadByContainerID(candidate.ID, DateTime.MinValue);

						var container = new SCAclContainer(candidate);

						foreach (var item in aclMembers)
							if (item.Status == SchemaObjectStatus.Normal)
								container.Members.Add(item.ContainerPermission, memberObjects[item.MemberID]);


						if (oldItems != null)
						{
							container.Members.MergeChangedItems(oldItems);
						}

						SCObjectOperations.Instance.UpdateObjectAcl(container);

						if (SCDataOperationLockContext.Current.Lock != null && (replacedCount) % 5 == 0)
							SCDataOperationLockContext.Current.ExtendLock();

						replacedCount++;

						ProcessProgress.Current.StatusText = string.Format("已替换\"{0}\"的Acl", objName);
						ProcessProgress.Current.Increment();
						ProcessProgress.Current.Response();
					}
					catch (System.Exception ex)
					{
						throw new ApplicationException(string.Format("替换对象{0}({1})的Acl出错: {2}", objName, candidate.ID, ex.Message));
					}
				}
			}
			else
			{
				ProcessProgress.Current.Output.WriteLine("当前对象没有子对象ACL");
				ProcessProgress.Current.CurrentStep = ProcessProgress.Current.MaxStep = 1;
			}

			ProcessProgress.Current.StatusText = string.Format("总共替换了{0:#,##0}个对象的Acl", replacedCount);
			ProcessProgress.Current.Response();

			return this._Parent;
		}

		/// <summary>
		/// 准备数据
		/// </summary>
		/// <param name="sourceObj"></param>
		private void PrepareCandidatesRecursively(ISCAclContainer sourceObj)
		{
			if (sourceObj is ISCRelationContainer)
			{
				SchemaObjectCollection objs = ((ISCRelationContainer)sourceObj).CurrentChildren;

				foreach (SchemaObjectBase obj in objs)
				{
					if (obj is ISCAclContainer && (this._ForceReplace || obj.Properties.GetValue("AllowAclInheritance", true)))
					{
						PrepareCandidatesRecursively((ISCAclContainer)obj);

						AddCandidates(obj);
					}
				}
			}
		}

		private void AddCandidates(SchemaObjectBase obj)
		{
			this._Candidates.Add(obj);
			obj.ClearRelativeData();

			ProcessProgress.Current.StatusText = string.Format("已经准备了{0:#,##0}个待替换Acl的对象", this._Candidates.Count);
			ProcessProgress.Current.Response();
		}
	}
}
