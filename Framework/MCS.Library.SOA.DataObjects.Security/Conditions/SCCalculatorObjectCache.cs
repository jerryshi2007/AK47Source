using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.Conditions
{
	/// <summary>
	/// 计算过程中的对象缓存（非线程安全，此对象用于单线程的对象上下文中）
	/// </summary>
	internal class SCCalculatorObjectCache
	{
		private Dictionary<string, SCObjectAndRelation> _CacheByCodeName = new Dictionary<string, SCObjectAndRelation>(StringComparer.OrdinalIgnoreCase);
		private Dictionary<string, SCObjectAndRelation> _CacheByID = new Dictionary<string, SCObjectAndRelation>(StringComparer.OrdinalIgnoreCase);
		private Dictionary<string, SCObjectAndRelation> _CacheByFullPath = new Dictionary<string, SCObjectAndRelation>(StringComparer.OrdinalIgnoreCase);

		public Dictionary<string, SCObjectAndRelation> CacheByCodeName
		{
			get
			{
				return this._CacheByCodeName;
			}
		}

		public Dictionary<string, SCObjectAndRelation> CacheByID
		{
			get
			{
				return this._CacheByID;
			}
		}

		public Dictionary<string, SCObjectAndRelation> CacheByFullPath
		{
			get
			{
				return this._CacheByFullPath;
			}
		}

		public bool TryGetValue(BuiltInFunctionIDType idType, string objectID, out SCObjectAndRelation result)
		{
			Dictionary<string, SCObjectAndRelation> dictionary = IDTypeToDictionary(idType);

			return dictionary.TryGetValue(objectID, out result);
		}

		public void AddObject(BuiltInFunctionIDType idType, string objectID, SCObjectAndRelation relation)
		{
			if (relation != null)
			{
				AddObjectToDictionary(this._CacheByCodeName, relation.CodeName, relation);
				AddObjectToDictionary(this._CacheByID, relation.ID, relation);
				AddObjectToDictionary(this._CacheByFullPath, relation.FullPath, relation);
			}
			else
			{
				Dictionary<string, SCObjectAndRelation> dictionary = IDTypeToDictionary(idType);

				dictionary[objectID] = relation; //null
			}
		}

		private void AddObjectToDictionary(Dictionary<string, SCObjectAndRelation> dictionary, string key, SCObjectAndRelation relation)
		{
			if (dictionary.ContainsKey(key) == false)
				dictionary.Add(key, relation);
		}

		private Dictionary<string, SCObjectAndRelation> IDTypeToDictionary(BuiltInFunctionIDType idType)
		{
			Dictionary<string, SCObjectAndRelation> result = null;

			switch (idType)
			{
				case BuiltInFunctionIDType.CodeName:
					result = this._CacheByCodeName;
					break;
				case BuiltInFunctionIDType.Guid:
					result = this._CacheByID;
					break;
				case BuiltInFunctionIDType.FullPath:
					result = this._CacheByFullPath;
					break;
				default:
					throw new NotSupportedException(string.Format("不支持的BuiltInFunctionIDType类型{0}", idType));
			}

			return result;
		}
	}
}
