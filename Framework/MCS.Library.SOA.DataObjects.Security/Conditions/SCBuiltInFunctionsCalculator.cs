using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Expression;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.Conditions
{
	/// <summary>
	/// 内置的计算函数
	/// </summary>
	internal class SCBuiltInFunctionsCalculator
	{
		public static readonly SCBuiltInFunctionsCalculator Instance = new SCBuiltInFunctionsCalculator();
		private static readonly string[] _AllSchemaTypes = new string[0];

		private SCBuiltInFunctionsCalculator()
		{
		}

		/// <summary>
		/// 是否是内置函数
		/// </summary>
		/// <param name="funcName"></param>
		/// <returns></returns>
		public bool IsFunction(string funcName)
		{
			funcName.CheckStringIsNullOrEmpty("funcName");

			BuiltInFunctionInfoCollection funcsInfo = BuiltInFunctionHelper.GetBuiltInFunctionsInfo(this.GetType());

			return funcsInfo.Contains(funcName);
		}

		/// <summary>
		/// 计算内置函数
		/// </summary>
		/// <param name="funcName"></param>
		/// <param name="arrParams"></param>
		/// <param name="callerContext"></param>
		/// <returns></returns>
		public object Calculate(string funcName, ParamObjectCollection arrParams, object callerContext)
		{
			return BuiltInFunctionHelper.ExecuteFunction(funcName, this, arrParams, callerContext);
		}

		#region BuiltInFunctions
		[BuiltInFunction("PropStartWith", "属性值是否以某前缀开头，大小写无关")]
		private bool PropStartWithFunction(string propName, string prefix, SCConditionCalculatingContext callerContext)
		{
			string propValue = callerContext.CurrentObject.Properties.GetValue(propName, string.Empty);

			return propValue.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
		}

		[BuiltInFunction("PropEndWith", "属性值是否以某后缀结尾，大小写无关")]
		private bool PropEndWithFunction(string propName, string suffix, SCConditionCalculatingContext callerContext)
		{
			string propValue = callerContext.CurrentObject.Properties.GetValue(propName, string.Empty);

			return propValue.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
		}

		[BuiltInFunction("IsChildAll", "用户所有的兼职信息都是同一个组织的直接子对象")]
		private bool IsChildAllFunction(BuiltInFunctionIDType idType, string parentID, SCConditionCalculatingContext callerContext)
		{
			parentID.CheckStringIsNullOrEmpty("parentID");

			bool result = false;
			SCUser currentUser = (SCUser)callerContext.CurrentObject;

			switch (idType)
			{
				case BuiltInFunctionIDType.CodeName:
					result = currentUser.CurrentParents.AllAndNotEmpty(p => string.Compare(p.Properties.GetValue("CodeName", string.Empty), parentID, true) == 0);
					break;
				case BuiltInFunctionIDType.Guid:
					result = currentUser.CurrentParentRelations.AllAndNotEmpty(r => string.Compare(r.ParentID, parentID, true) == 0);
					break;
				case BuiltInFunctionIDType.FullPath:
					{
						SCObjectAndRelation parent = GetObjectByID(idType, parentID, callerContext);

						if (parent != null)
							result = currentUser.CurrentParentRelations.AllAndNotEmpty(r => string.Compare(r.ParentID, parent.ID, true) == 0);
						break;
					}
				default:
					throw new NotSupportedException(string.Format("不支持的BuiltInFunctionIDType类型{0}", idType));
			}

			return result;
		}

		[BuiltInFunction("IsChild", "是否是直接的子对象")]
		private bool IsChildFunction(BuiltInFunctionIDType idType, string parentID, SCConditionCalculatingContext callerContext)
		{
			parentID.CheckStringIsNullOrEmpty("parentID");

			bool result = false;
			SCUser currentUser = (SCUser)callerContext.CurrentObject;

			switch (idType)
			{
				case BuiltInFunctionIDType.CodeName:
					result = currentUser.CurrentParents.Exists(p => string.Compare(p.Properties.GetValue("CodeName", string.Empty), parentID, true) == 0);
					break;
				case BuiltInFunctionIDType.Guid:
					result = currentUser.CurrentParentRelations.Exists(r => string.Compare(r.ParentID, parentID, true) == 0);
					break;
				case BuiltInFunctionIDType.FullPath:
					{
						SCObjectAndRelation parent = GetObjectByID(idType, parentID, callerContext);

						if (parent != null)
							result = currentUser.CurrentParentRelations.Exists(r => string.Compare(r.ParentID, parent.ID, true) == 0);
						break;
					}
				default:
					throw new NotSupportedException(string.Format("不支持的BuiltInFunctionIDType类型{0}", idType));
			}

			return result;
		}

		[BuiltInFunction("IsDescendantAll", "用户所有的兼职信息都是同一个组织的子孙")]
		private bool IsDescendantAllFunction(BuiltInFunctionIDType idType, string ancestorID, SCConditionCalculatingContext callerContext)
		{
			ancestorID.CheckStringIsNullOrEmpty("ancestorID");

			bool result = false;

			SCObjectAndRelation ancestor = GetObjectByID(idType, ancestorID, callerContext);

			if (ancestor != null)
				result = callerContext.CurrentObject.CurrentParentRelations.AllAndNotEmpty(r =>
					r.FullPath.IndexOf(ancestor.FullPath, StringComparison.OrdinalIgnoreCase) == 0);

			return result;
		}

		[BuiltInFunction("IsDescendant", "是否是子孙对象")]
		private bool IsDescendantFunction(BuiltInFunctionIDType idType, string ancestorID, SCConditionCalculatingContext callerContext)
		{
			ancestorID.CheckStringIsNullOrEmpty("ancestorID");

			bool result = false;

			SCObjectAndRelation ancestor = GetObjectByID(idType, ancestorID, callerContext);

			if (ancestor != null)
				result = callerContext.CurrentObject.CurrentParentRelations.Exists(r =>
					r.FullPath.IndexOf(ancestor.FullPath, StringComparison.OrdinalIgnoreCase) == 0);

			return result;
		}
		#endregion BuiltInFunctions

		private SCObjectAndRelation GetObjectByID(BuiltInFunctionIDType idType, string objectID, SCConditionCalculatingContext callerContext)
		{
			SCCalculatorObjectCache cache = null;

			if (callerContext.ExtendedData.ContainsKey("SCCalculatorObjectCache"))
				cache = (SCCalculatorObjectCache)callerContext.ExtendedData["SCCalculatorObjectCache"];

			if (cache == null)
			{
				cache = new SCCalculatorObjectCache();
				callerContext.ExtendedData.Add("SCCalculatorObjectCache", cache);
			}

			SCObjectAndRelation result = null;

			if (cache.TryGetValue(idType, objectID, out result) == false)
			{
				result = QueryObjectByID(idType, objectID);

				cache.AddObject(idType, objectID, result);
			}

			return result;
		}

		private static SCObjectAndRelation QueryObjectByID(BuiltInFunctionIDType idType, string objectID)
		{
			SCObjectAndRelation result = null;

			switch (idType)
			{
				case BuiltInFunctionIDType.Guid:
					result = SCSnapshotAdapter.Instance.QueryObjectAndRelationByIDs(_AllSchemaTypes, new string[] { objectID }, false, DateTime.MinValue).FirstOrDefault();
					break;
				case BuiltInFunctionIDType.CodeName:
					result = SCSnapshotAdapter.Instance.QueryObjectAndRelationByCodeNames(_AllSchemaTypes, new string[] { objectID }, false, DateTime.MinValue).FirstOrDefault();
					break;
				case BuiltInFunctionIDType.FullPath:
					result = SCSnapshotAdapter.Instance.QueryObjectAndRelationByFullPaths(_AllSchemaTypes, new string[] { objectID }, false, DateTime.MinValue).FirstOrDefault();
					break;
				default:
					throw new NotSupportedException(string.Format("不支持的BuiltInFunctionIDType类型{0}", idType));
			}

			return result;
		}
	}
}
