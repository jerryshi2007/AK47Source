using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// 成员关系对象的适配器。这里的成员关系主要是群组和人员、应用和角色、应用和权限、角色和被授权对象之间的关系
	/// </summary>
	public class SCMemberRelationAdapter : SchemaObjectAdapterBase<SCSimpleRelationBase>
	{
		/// <summary>
		/// <see cref="SCMemberRelationAdapter"/>的实例，此字段为只读
		/// </summary>
		public static readonly SCMemberRelationAdapter Instance = new SCMemberRelationAdapter();

		private SCMemberRelationAdapter()
		{
		}

		/// <summary>
		/// 根据成员ID载入对象
		/// </summary>
		/// <param name="memberID">成员ID</param>
		/// <returns></returns>
		public SCObjectContainerRelationCollection LoadByMemberID(string memberID)
		{
			return LoadByMemberID(memberID, DateTime.MinValue);
		}

		/// <summary>
		/// 根据成员ID和容器模式类型载入对象
		/// </summary>
		/// <param name="memberID">成员ID</param>
		/// <param name="containerSchemaType">容器模式类型，如果空则忽略此参数</param>
		/// <returns></returns>
		public SCObjectContainerRelationCollection LoadByMemberID(string memberID, string containerSchemaType)
		{
			return LoadByMemberID(memberID, containerSchemaType, DateTime.MinValue);
		}

		/// <summary>
		/// 根据成员ID和时间点载入对象
		/// </summary>
		/// <param name="memberID">成员ID</param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SCObjectContainerRelationCollection LoadByMemberID(string memberID, DateTime timePoint)
		{
			return LoadByMemberID(memberID, string.Empty, timePoint);
		}

		/// <summary>
		/// 根据成员ID，容器模式类型和时间点载入对象
		/// </summary>
		/// <param name="memberID">成员ID</param>
		/// <param name="containerSchemaType">容器模式类型</param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SCObjectContainerRelationCollection LoadByMemberID(string memberID, string containerSchemaType, DateTime timePoint)
		{
			return LoadByMemberID(memberID, containerSchemaType, false, timePoint);
		}

		/// <summary>
		/// 根据成员ID，容器模式类型和时间点载入对象
		/// </summary>
		/// <param name="memberID">成员ID</param>
		/// <param name="containerSchemaType">容器模式类型</param>
		/// <param name="normalOnly">为true时仅含正常对象</param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SCObjectContainerRelationCollection LoadByMemberID(string memberID, string containerSchemaType, bool normalOnly, DateTime timePoint)
		{
			memberID.CheckStringIsNullOrEmpty("memberID");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("MemberID", memberID);

			if (containerSchemaType.IsNotEmpty())
				builder.AppendItem("ContainerSchemaType", containerSchemaType);

			if (normalOnly)
				builder.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			return Load<SCObjectContainerRelationCollection>(builder, timePoint);
		}

		/// <summary>
		/// 根据成员ID，容器模式类型和时间点载入对象
		/// </summary>
		/// <param name="memberID">成员ID</param>
		/// <param name="containerSchemaType">容器模式类型</param>
		/// <param name="normalOnly">为true时仅含正常对象</param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SCObjectContainerRelationCollection LoadByMemberID(string memberID, string[] containerSchemaTypes, bool normalOnly, DateTime timePoint)
		{
			memberID.CheckStringIsNullOrEmpty("memberID");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("MemberID", memberID);

			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("ContainerSchemaType");
			inBuilder.AppendItem(containerSchemaTypes);

			if (normalOnly)
				builder.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			return Load<SCObjectContainerRelationCollection>(new ConnectiveSqlClauseCollection(builder, inBuilder), timePoint);
		}

		/// <summary>
		/// 根据容器ID载入对象
		/// </summary>
		/// <param name="containerID">容器ID</param>
		/// <returns></returns>
		public SCObjectMemberRelationCollection LoadByContainerID(string containerID)
		{
			return LoadByContainerID(containerID, DateTime.MinValue);
		}

		/// <summary>
		/// 根据容器ID和成员模式类型载入对象
		/// </summary>
		/// <param name="containerID">容器ID</param>
		/// <param name="memberSchemaType">成员模式类型，为空则忽略此参数</param>
		/// <returns></returns>
		public SCObjectMemberRelationCollection LoadByContainerID(string containerID, string memberSchemaType)
		{
			return LoadByContainerID(containerID, memberSchemaType, DateTime.MinValue);
		}

		/// <summary>
		/// 根据容器ID，时间点载入对象(含删除的)
		/// </summary>
		/// <param name="containerID">容器ID</param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SCObjectMemberRelationCollection LoadByContainerID(string containerID, DateTime timePoint)
		{
			return LoadByContainerID(containerID, string.Empty, timePoint);
		}

		/// <summary>
		/// 根据容器ID，成员模式类型和时间点载入对象
		/// </summary>
		/// <param name="containerID">容器ID</param>
		/// <param name="memberSchemaType">成员模式类型，如果为空则忽略此参数</param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SCObjectMemberRelationCollection LoadByContainerID(string containerID, string memberSchemaType, DateTime timePoint)
		{
			return LoadByContainerID(containerID, memberSchemaType, false, timePoint);
		}

		public SCObjectMemberRelationCollection LoadByContainerID(string containerID, string memberSchemaType, bool normalOnly, DateTime timePoint)
		{
			containerID.CheckStringIsNullOrEmpty("containerID");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("ContainerID", containerID);

			if (memberSchemaType.IsNotEmpty())
				builder.AppendItem("MemberSchemaType", memberSchemaType);

			if (normalOnly)
				builder.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			return Load<SCObjectMemberRelationCollection>(builder, timePoint);
		}

		public SCObjectMemberRelationCollection LoadByContainerID(string containerID, string[] memberSchemaType, bool normalOnly, DateTime timePoint)
		{
			containerID.CheckStringIsNullOrEmpty("containerID");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("ContainerID", containerID);


			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("MemberSchemaType");
			inBuilder.AppendItem(memberSchemaType);

			if (normalOnly)
				builder.AppendItem("Status", (int)SchemaObjectStatus.Normal);

			return Load<SCObjectMemberRelationCollection>(new ConnectiveSqlClauseCollection(builder, inBuilder), timePoint);
		}

		/// <summary>
		/// 根据指定条件和时间点，载入对象
		/// </summary>
		/// <typeparam name="T">表示返回结果的<see cref="SCMemberRelationCollectionBase"/>的派生类型</typeparam>
		/// <param name="builder">包含条件的<see cref="IConnectiveSqlClause"/></param>
		/// <param name="timePoint">时间点</param>
		/// <returns></returns>
		public T Load<T>(IConnectiveSqlClause builder, DateTime timePoint) where T : SCMemberRelationCollectionBase, new()
		{
			var timeBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);

			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, timeBuilder);

			T result = null;

			VersionedObjectAdapterHelper.Instance.FillData(GetMappingInfo().TableName, connectiveBuilder, this.GetConnectionName(),
				view =>
				{
					result = new T();

					result.LoadFromDataView(view);
				});

			return result;
		}

		/// <summary>
		/// 根据容器ID，成员ID载入对象
		/// </summary>
		/// <param name="containerID">容器ID</param>
		/// <param name="memberID">成员ID</param>
		/// <returns></returns>
		public SCSimpleRelationBase Load(string containerID, string memberID)
		{
			return Load(containerID, memberID, DateTime.MinValue);
		}

		/// <summary>
		/// 根据容器ID，成员ID和时间点载入对象
		/// </summary>
		/// <param name="containerID">容器ID</param>
		/// <param name="memberID">成员ID</param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SCSimpleRelationBase Load(string containerID, string memberID, DateTime timePoint)
		{
			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("ContainerID", containerID);
			builder.AppendItem("MemberID", memberID);

			SCMemberRelationCollection relations = Load(builder, timePoint);

			return relations.FirstOrDefault();
		}

		/// <summary>
		/// 根据条件和时间点载入对象
		/// </summary>
		/// <param name="builder">包含条件的<see cref="IConnectiveSqlClause"/></param>
		/// <param name="timePoint"></param>
		/// <returns></returns>
		public SCMemberRelationCollection Load(IConnectiveSqlClause builder, DateTime timePoint)
		{
			var timeBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);

			ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(builder, timeBuilder);

			SCMemberRelationCollection result = null;

			VersionedObjectAdapterHelper.Instance.FillData(GetMappingInfo().TableName, connectiveBuilder, this.GetConnectionName(),
				view =>
				{
					result = new SCMemberRelationCollection();

					result.LoadFromDataView(view);
				});

			return result;
		}
	}
}
