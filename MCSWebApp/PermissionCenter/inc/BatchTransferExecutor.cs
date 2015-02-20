using System;
using System.Linq;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter
{
	internal abstract class BatchTransferExecutor : BatchExecutor
	{
		private int totalSteps;
		private int passedSteps;

		public BatchTransferExecutor(string[] src, string[] target)
			: base(src, target)
		{
			this.totalSteps = src.Length * target.Length;
			this.passedSteps = 0;
		}

		protected override bool DoExecute(SchemaObjectCollection srcObjects, SchemaObjectCollection targetObjects)
		{
			bool result = base.DoExecute(srcObjects, targetObjects);
			this.ReportProgress("操作结束", true);
			return result;
		}

		protected void ReportProgress(string description, bool increatment)
		{
			this.ReportProgress(description, description, increatment);
		}

		protected void ReportProgress(string description, string log, bool increatment)
		{
			var pg = ProcessProgress.Current;
			if (pg != null)
			{
				pg.StatusText = description;

				if (log != null)
					pg.Output.WriteLine(log);

				if (increatment)
				{
					pg.MinStep = 0;
					pg.MaxStep = this.totalSteps;
					if (pg.CurrentStep < this.totalSteps)
						pg.CurrentStep = this.passedSteps;

					this.passedSteps++;
				}

				pg.Response();
			}
		}

		protected override bool HandleError(Exception ex)
		{
			ProcessProgress.Current.Error.WriteLine(ex.Message);
			ProcessProgress.Current.Output.WriteLine(ex.Message);
			return true;
		}

		protected override bool HandleItemError(SchemaObjectBase src, SchemaObjectBase target, Exception ex)
		{
			ProcessProgress.Current.Error.WriteLine(ex.Message);
			ProcessProgress.Current.Output.WriteLine(ex.Message);
			return true;
		}
	}

	internal abstract class OUTransferExecutor : BatchTransferExecutor
	{
		private SCOrganization parent;

		public OUTransferExecutor(string orgKey, string[] src, string[] target)
			: base(src, target)
		{
			orgKey.NullCheck("orgKey");
			this.parent = (SCOrganization)DbUtil.GetEffectiveObject(orgKey, null);
		}

		public SCOrganization ParentOrg
		{
			get { return this.parent; }
		}
	}

	internal class MoveObjectsToOrgTransfer : OUTransferExecutor
	{
		public MoveObjectsToOrgTransfer(string orgKey, string[] src, string[] target)
			: base(orgKey, src, target)
		{
		}

		protected override BatchExecuteCardinality CardinalityOfDestination
		{
			get
			{
				return BatchExecuteCardinality.Mandatory | BatchExecuteCardinality.One;
			}
		}

		protected override string DescriptionForSource
		{
			get { return "对象"; }
		}

		protected override string DescriptionForTarget
		{
			get { return "组织"; }
		}

		protected override void DoExecuteItem(MCS.Library.SOA.DataObjects.Security.SchemaObjectBase src, MCS.Library.SOA.DataObjects.Security.SchemaObjectBase target)
		{
			this.ReportProgress(string.Format("正在移动{0}", ((SCBase)src).DisplayName), true);
			SCObjectOperations.InstanceWithPermissions.MoveObjectToOrganization(this.ParentOrg, (SCBase)src, (SCOrganization)target);
		}
	}

	internal class MoveUsersToOrgsTransfer : MoveObjectsToOrgTransfer
	{
		public MoveUsersToOrgsTransfer(string orgKey, string[] src, string[] target)
			: base(orgKey, src, target)
		{
		}

		protected override string DescriptionForSource
		{
			get
			{
				return "人员";
			}
		}

		protected override BatchExecuteCardinality CardinalityOfDestination
		{
			get
			{
				return BatchExecuteCardinality.Mandatory | BatchExecuteCardinality.Many;
			}
		}
	}

	internal class CopyUsersToGroupsTransfer : OUTransferExecutor
	{
		public CopyUsersToGroupsTransfer(string orgKey, string[] src, string[] target)
			: base(orgKey, src, target)
		{
		}

		protected override string DescriptionForSource
		{
			get { return "人员"; }
		}

		protected override string DescriptionForTarget
		{
			get { return "群组"; }
		}

		protected override void DoExecuteItem(MCS.Library.SOA.DataObjects.Security.SchemaObjectBase src, MCS.Library.SOA.DataObjects.Security.SchemaObjectBase target)
		{
			this.ReportProgress(string.Format("正在添加向群组 {0} 添加人员 {1}", ((SCBase)target).DisplayName, ((SCBase)src).DisplayName), true);
			SCObjectOperations.InstanceWithPermissions.AddUserToGroup((SCUser)src, (SCGroup)target);
		}
	}

	internal class CopyUsersToOrgsTransfer : OUTransferExecutor
	{
		public CopyUsersToOrgsTransfer(string orgKey, string[] src, string[] target)
			: base(orgKey, src, target)
		{
		}

		protected override string DescriptionForSource
		{
			get { return "人员"; }
		}

		protected override string DescriptionForTarget
		{
			get { return "组织"; }
		}

		protected override void DoExecuteItem(MCS.Library.SOA.DataObjects.Security.SchemaObjectBase src, MCS.Library.SOA.DataObjects.Security.SchemaObjectBase target)
		{
			this.ReportProgress(string.Format("正在添加向组织 {0} 添加人员 {1}", ((SCBase)target).DisplayName, ((SCBase)src).DisplayName), true);
			SCObjectOperations.InstanceWithPermissions.AddUserToOrganization((SCUser)src, (SCOrganization)target);
		}
	}

	internal class BatchGroupTransferExecutor : BatchTransferExecutor
	{
		private SCRelationObjectCollection relations;
		private SchemaObjectCollection parents;

		public BatchGroupTransferExecutor(string[] srcGrpKeys, string[] targetOrgKeys)
			: base(srcGrpKeys, targetOrgKeys)
		{
		}

		protected override string DescriptionForSource
		{
			get { return "群组"; }
		}

		protected override string DescriptionForTarget
		{
			get { return "组织"; }
		}

		protected override BatchExecuteCardinality CardinalityOfDestination
		{
			get
			{
				return BatchExecuteCardinality.One | BatchExecuteCardinality.Mandatory;
			}
		}

		protected override void ValidateObjects()
		{
			WhereSqlClauseBuilder bd = new WhereSqlClauseBuilder();
			bd.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			InSqlClauseBuilder inB = new InSqlClauseBuilder("ObjectID");
			inB.AppendItem(this.SourceKeys);

			InSqlClauseBuilder inC = new InSqlClauseBuilder("ParentSchemaType");
			inC.AppendItem(SchemaInfo.FilterByCategory("Organizations").ToSchemaNames());

			InSqlClauseBuilder inD = new InSqlClauseBuilder("Groups");
			inC.AppendItem(SchemaInfo.FilterByCategory("Groups").ToSchemaNames());

			this.relations = SchemaRelationObjectAdapter.Instance.Load(new ConnectiveSqlClauseCollection(bd, inB, inC, inD, VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder()), DateTime.MinValue);
			this.parents = DbUtil.LoadObjects(this.relations.ToParentIDArray());

			base.ValidateObjects();
		}

		protected override void DoExecuteItem(SchemaObjectBase src, SchemaObjectBase target)
		{
			this.ReportProgress(string.Format("正在移动 {0} ", ((SCBase)src).DisplayName), true);
			var parentId = (from r in this.relations where r.ID == src.ID select r.ParentID).FirstOrDefault();

			SCOrganization parent = parentId != null ? (from o in this.parents where o.ID == parentId select (SCOrganization)o).FirstOrDefault() : null;

			SCObjectOperations.InstanceWithPermissions.MoveObjectToOrganization(parent, (SCGroup)src, (SCOrganization)target);
		}
	}
}