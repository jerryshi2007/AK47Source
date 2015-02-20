using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Schemas.Client.ServiceBroker
{
	public interface ISchemaQueryService
	{
		/// <summary>
		/// 获取对象的ACL
		/// </summary>
		/// <param name="id">对象ID</param>
		/// <returns></returns>
		ClientAclItem[] GetAcls(string id);
		/// <summary>
		/// 获取对象的子对象
		/// </summary>
		/// <param name="id">对象的ID</param>
		/// <param name="childSchemaTypes">用于子对象的schema类型，为空数组时表示不过滤</param>
		/// <param name="normalOnly">为true时，仅包含正常对象</param>
		/// <returns></returns>
		ClientSchemaObjectBase[] GetChildren(string id, string[] childSchemaTypes, bool normalOnly);
		/// <summary>
		/// 获取对象上定义的条件
		/// </summary>
		/// <param name="id">对象的ID</param>
		/// <returns></returns>
		ClientConditionItem[] GetConditions(string id);
		/// <summary>
		/// 获取对象的父级对象
		/// </summary>
		/// <param name="id">对象ID</param>
		/// <param name="containerSchemaTypes">父级对象的Schema类型 为空数组时表示不过滤。</param>
		/// <param name="normalOnly">为true时，仅包含正常对象</param>
		/// <returns></returns>
		ClientSchemaObjectBase[] GetContainers(string id, string[] containerSchemaTypes, bool normalOnly);
		/// <summary>
		/// 获取成员对象
		/// </summary>
		/// <param name="id">对象的iD</param>
		/// <param name="memberSchemaTypes">成员对象的schema类型 为空数组时表示不过滤</param>
		/// <param name="normalOnly">为true时，仅包含正常对象</param>
		/// <returns></returns>
		ClientSchemaObjectBase[] GetMembers(string id, string[] memberSchemaTypes, bool normalOnly);
		/// <summary>
		/// 获取对象的成员关系
		/// </summary>
		/// <param name="containerID">容器对象的ID</param>
		/// <param name="memberSchemaTypes">成员对象的schema类型 为空数组时表示不过滤。</param>
		/// <param name="normalOnly">为true时，仅包含正常对象</param>
		/// <returns></returns>
		ClientSchemaMember[] GetMemberships(string containerID, string[] memberSchemaTypes, bool normalOnly);
		/// <summary>
		/// 根据代码名称获取对象
		/// </summary>
		/// <param name="codeNames">代码名称</param>
		/// <param name="objectSchemaTypes">对象的schema类型 为空数组时表示不过滤</param>
		/// <param name="normalOnly">为true时，仅包含正常对象</param>
		/// <returns></returns>
		ClientSchemaObjectBase[] GetObjectsByCodeNames(string[] codeNames, string[] objectSchemaTypes, bool normalOnly);
		/// <summary>
		/// 根据对象ID获取对象
		/// </summary>
		/// <param name="ids">对象的ID</param>
		/// <param name="objectSchemaTypes">对象的schema类型 为空数组时表示不过滤。</param>
		/// <param name="normalOnly">为true时，仅包含正常对象</param>
		/// <returns></returns>
		ClientSchemaObjectBase[] GetObjectsByIDs(string[] ids, string[] objectSchemaTypes, bool normalOnly);
		/// <summary>
		/// 根据XPath的查询结果获取对象
		/// </summary>
		/// <param name="xQuery">用于查询的XPath表达式</param>
		/// <param name="objectSchemaTypes">对象的schema类型</param>
		/// <param name="normalOnly">为true时，仅包含正常对象</param>
		/// <returns></returns>
		ClientSchemaObjectBase[] GetObjectsByXQuery(string xQuery, string[] objectSchemaTypes, bool normalOnly);
		/// <summary>
		/// 获取对象的父级对象
		/// </summary>
		/// <param name="id">对象的ID</param>
		/// <param name="parentSchemaTypes">父级对象的schema类型</param>
		/// <param name="normalOnly">为true时，仅包含正常对象</param>
		/// <returns></returns>
		ClientSchemaObjectBase[] GetParents(string id, string[] parentSchemaTypes, bool normalOnly);
		/// <summary>
		/// 获取模式对象的属性定义
		/// </summary>
		/// <param name="schemaType">对象的schema类型</param>
		/// <returns></returns>
		ClientPropertyDefine[] GetSchemaPropertyDefinition(string schemaType);
	}
}
