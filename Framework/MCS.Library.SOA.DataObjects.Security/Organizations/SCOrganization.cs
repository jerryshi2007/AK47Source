using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 表示组织机构
	/// </summary>
	[Serializable]
	public class SCOrganization : SCRoleMemberBase, ISCRelationContainer, ISCUserContainerObject, ISCAclContainer, IAllowAclInheritance
	{
		public const string RootOrganizationID = "e588c4c6-4097-4979-94c2-9e2429989932";
		public const string RootOrganizationName = "根组织";

		/// <summary>
		/// 初始化<see cref="SCOrganization"/>的新实例
		/// </summary>
		public SCOrganization() :
			base(StandardObjectSchemaType.Organizations.ToString())
		{
		}

		public SCOrganization(string schemaType)
			: base(schemaType)
		{
		}

		/// <summary>
		/// 获取一个值，表示当前组织是否代表全局组织（虚拟组织）。
		/// </summary>
		[NoMapping]
		public bool IsRoot
		{
			get
			{
				return string.Compare(this.ID, RootOrganizationID, true) == 0;
			}
		}

		/// <summary>
		/// 获取全局（根）组织
		/// </summary>
		/// <returns>一个表示根的<see cref="SCOrganization"/>的实例。</returns>
		public static SCOrganization GetRoot()
		{
			SCOrganization result = new SCOrganization();

			result.ID = RootOrganizationID;
			result.Name = RootOrganizationName;

			return result;
		}

		/// <summary>
		/// 获取根关系对象
		/// </summary>
		/// <returns>一个表示根关系的<see cref="SCRelationObject"/>对象</returns>
		public static SCRelationObject GetRootRelationObject()
		{
			SCRelationObject result = new SCRelationObject();

			result.ParentSchemaType = StandardObjectSchemaType.Organizations.ToString();
			result.ID = RootOrganizationID;
			result.InnerSort = 0;
			result.ChildSchemaType = StandardObjectSchemaType.Organizations.ToString();
			result.FullPath = string.Empty;
			result.GlobalSort = string.Empty;

			return result;
		}

		#region ISCRelationContainer Members

		[NonSerialized]
		private SCChildrenRelationObjectCollection _AllChildrenRelations = null;

		/// <summary>
		/// 获取一个<see cref="SCChildrenRelationObjectCollection"/>，表示所有子级关系
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCChildrenRelationObjectCollection AllChildrenRelations
		{
			get
			{
				if (this._AllChildrenRelations == null && this.ID.IsNotEmpty())
				{
					this._AllChildrenRelations = SchemaRelationObjectAdapter.Instance.LoadByParentID(this.ID);
				}

				return _AllChildrenRelations;
			}
		}

		/// <summary>
		/// 获取一个<see cref="SCChildrenRelationObjectCollection"/>，表示当前子级关系
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCChildrenRelationObjectCollection CurrentChildrenRelations
		{
			get
			{
				return (SCChildrenRelationObjectCollection)AllChildrenRelations.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
			}
		}

		[NonSerialized]
		private SchemaObjectCollection _AllChildren = null;

		/// <summary>
		/// 获取一个<see cref="SchemaObjectCollection"/>，表示所有的子级
		/// </summary>
		[NoMapping]
		[ScriptIgnore]
		public SchemaObjectCollection AllChildren
		{
			get
			{
				if (this._AllChildren == null && this.ID.IsNotEmpty())
					this._AllChildren = SchemaObjectAdapter.Instance.Load(AllChildrenRelations.ToChildrenIDsBuilder());

				return this._AllChildren;
			}
		}

		[NonSerialized]
		private SchemaObjectCollection _CurrentlChildren = null;

		/// <summary>
		/// 获取一个表示当前的子级的<see cref="SchemaObjectCollection"/>
		/// </summary>
		[NoMapping]
		[ScriptIgnore]
		public SchemaObjectCollection CurrentChildren
		{
			get
			{
				if (this._CurrentlChildren == null && this.ID.IsNotEmpty())
				{
					this._CurrentlChildren = SchemaObjectAdapter.Instance.Load(CurrentChildrenRelations.ToChildrenIDsBuilder());
					this._CurrentlChildren = this._CurrentlChildren.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
				}

				return this._CurrentlChildren;
			}
		}
		#endregion

		int ISCRelationContainer.GetCurrentChildrenCount()
		{
			return SchemaRelationObjectAdapter.Instance.GetChildrenCount(this.ID, null, DateTime.MinValue);
		}

		int ISCRelationContainer.GetCurrentMaxInnerSort()
		{
			return SchemaRelationObjectAdapter.Instance.GetMaxInnerSort(this.ID, null, DateTime.MinValue);
		}

		protected override void OnIDChanged()
		{
			base.OnIDChanged();

			this._AllChildrenRelations = null;
			this._AllChildren = null;
			this._CurrentlChildren = null;
		}

		#region ISCUserContainerObject Members

		public SchemaObjectCollection GetCurrentUsers()
		{
			SchemaObjectCollection result = null;

			SCRelationObject relation = this.CurrentParentRelations.FirstOrDefault();

			if (relation != null)
			{
				SCObjectAndRelationCollection objsAndRelations = SCSnapshotAdapter.Instance.QueryObjectAndRelationByParentFullPath(
					SchemaInfo.FilterByCategory("Users").ToSchemaNames(), new string[] { relation.FullPath }, true, true, false, DateTime.MinValue);

				result = objsAndRelations.ToSchemaObjects();
			}
			else
				result = new SchemaObjectCollection();

			return result;
		}

		#endregion

		#region ISCAclContainer Members

		public SCAclMemberCollection GetAclMembers()
		{
			return SCAclAdapter.Instance.LoadByContainerID(this.ID, SchemaObjectStatus.Normal, DateTime.MinValue);
		}

		#endregion
	}
}
