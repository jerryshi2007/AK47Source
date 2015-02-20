using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Data.Mapping;
using MCS.Library.Data;
using System.Web.Script.Serialization;
using MCS.Library.SOA.DataObjects.Security.Conditions;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 表示管理单元中的一个管理范围
	/// </summary>
	[Serializable]
	public class AUAdminScope : SchemaObjectBase, IAdminScopeItemContainer
	{
		[NonSerialized]
		AdminUnit owner = null;

		public AUAdminScope()
			: base(AUCommon.SchemaAUAdminScope)
		{
		}

		public AUAdminScope(string schemaType)
			: base(schemaType)
		{
		}

		protected override void OnIDChanged()
		{
			base.OnIDChanged();
			owner = null;
		}

		[NoMapping]
		[ScriptIgnore]
		[System.Xml.Serialization.XmlIgnore]
		public AdminUnit OwnerAdminUnit
		{
			get
			{
				if (this.owner == null)
					owner = GetOwnerUnit();
				return owner;
			}
		}

		/// <summary>
		/// 获取或设置此类型定义的管理范围类别
		/// </summary>
		[NoMapping]
		public string ScopeSchemaType
		{
			get { return this.Properties.GetValue<string>("ScopeSchemaType", string.Empty); }

			set { this.Properties.SetValue<string>("ScopeSchemaType", value); }
		}

		/// <summary>
		/// 获取此管理范围类别的所有固定成员
		/// </summary>
		/// <returns></returns>
		public PC.SCObjectMemberRelationCollection GetScopeConstRelations()
		{
			SCObjectMemberRelationCollection result = null;

			result = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(this.ID);

			return result;
		}

		/// <summary>
		/// 获取此管理单元管理范围中定义的条件表达式
		/// </summary>
		/// <returns></returns>
		public SCCondition GetCondition()
		{
			return Adapters.AUConditionAdapter.Instance.Load(this.ID, AUCommon.ConditionType).FirstOrDefault();
		}

		/// <summary>
		/// 获取此管理范围所属管理单元
		/// </summary>
		/// <returns></returns>
		public AdminUnit GetOwnerUnit()
		{
			return (AdminUnit)Adapters.AUSnapshotAdapter.Instance.LoadContainers(this.ID, this.SchemaType, AUCommon.SchemaAdminUnit, false, DateTime.MinValue).FirstOrDefault();
		}

		public SchemaObjectCollection GetCurrentObjects()
		{
			SCMemberRelationCollectionBase members = null;
			AUCommon.DoDbAction(() =>
			{
				members = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(this.ID, this.ScopeSchemaType).FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
			});
			if (members.Count > 0)
			{
				return Adapters.AUSnapshotAdapter.Instance.LoadScopeItems(members.ToIDArray(), this.ScopeSchemaType, true, DateTime.MinValue);
			}
			else
			{
				return new SchemaObjectCollection();
			}
		}
	}
}
