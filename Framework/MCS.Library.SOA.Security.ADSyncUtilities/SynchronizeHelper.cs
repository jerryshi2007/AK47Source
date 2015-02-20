using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using MCS.Library;
using MCS.Library.Caching;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;
using System.Collections;
using System.Diagnostics;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public static class SynchronizeHelper
	{
		public const long ACCOUNT_EXPIRES_MAX_VALUE = 9223372036854775807;
		internal const int ADQueryBatchSize = 500;

		private class NativeGuidConverter : IEnumerable<byte[]>
		{
			private IEnumerable<string> nativeIDs;

			public NativeGuidConverter(IEnumerable<string> nativeIDs)
			{
				this.nativeIDs = nativeIDs;
			}

			public IEnumerator<byte[]> GetEnumerator()
			{
				foreach (var item in this.nativeIDs)
				{
					yield return ConvertGuidToBinary(item);
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
		}

		#region AD对象属性名称
		private static readonly char[] hexDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
		private static readonly string[] ADOUObjNeededProperties = new string[]
												  {
													  "objectGUID",
													  "distinguishedName",
													  "objectCategory",
													  "objectClass",
													  "name",
													  "displayName",
													  "extensionName",
													  "adminDisplayName",
													  "address",
													  "mobile",
													  "telephoneNumber",
													  "mail",
													  "adminDescription",
												  };

		private static readonly string[] ADUserObjNeededProperties = new string[]
												  {
													  "objectGUID",
													  "distinguishedName",
													  "objectClass",
													  "objectCategory",
													  "name",
													  "displayName",
													  "extensionName",
													  "adminDisplayName",
													  "givenName",
													  "address",
													  "mobile",
													  "telephoneNumber",
													  "mail",
													  "sn",
													  "samAccountName",
													  "userAccountControl",
													  "accountExpires",
													  "otherMobile",
													  "company",
													  "department",
													  "title",
													  "pager"
												  };

		private static readonly string[] ADGroupObjNeededProperties = new string[]
												  {
													  "objectGUID",
													  "distinguishedName",
													  "objectClass",
													  "objectCategory",
													  "name",
													  "displayName",
													  "extensionName",
													  "adminDisplayName",
													  "address",
													  "mobile",
													  "telephoneNumber",
													  "mail",
													  "samAccountName",
													  "member",
													  "displayNamePrintable"
												  };

		private static readonly string[] ADObjNeededProperties = new string[]
												  {
													  "objectGUID",
													  "distinguishedName",
													  "objectCategory",
													  "objectClass",
													  "name",
													  "givenName",
													  "displayName",
													  "sn",
													  "samAccountName",
													  "extensionName",
													  "adminDisplayName",
													  "accountExpires",
													  "userAccountControl",
													   "otherMobile",
													  "company",
													  "department",
													  "title",
													  "member",
													  "address",
													  "mobile",
													  "telephoneNumber",
													  "mail",
													  "member",
													  "adminDescription",
													  "displayNamePrintable",
													  "pager"
												  };

		public static readonly string PermissionCenterInvolved = "{__pc_involved__}";

		#endregion

		#region SearchResult系列

		public static ADObjectWrapper ToADOjectWrapper(this SearchResult searchResult)
		{
			ADObjectWrapper adOjectWrapper = null;

			if (searchResult != null)
			{
				ADHelper adHelper = SynchronizeContext.Current.ADHelper;
				adOjectWrapper = new ADObjectWrapper();

				if (searchResult.IsOrgnization())
				{
					adOjectWrapper.ObjectType = ADSchemaType.Organizations;

					foreach (var adouObjNeededProperty in ADOUObjNeededProperties)
					{
						adOjectWrapper.Properties.Add(adouObjNeededProperty,
							adHelper.GetSearchResultPropertyStrValues(adouObjNeededProperty, searchResult));
					}
				}
				else if (searchResult.IsUser())
				{
					adOjectWrapper.ObjectType = ADSchemaType.Users;

					foreach (var adouObjNeededProperty in ADUserObjNeededProperties)
					{
						adOjectWrapper.Properties.Add(adouObjNeededProperty,
							adHelper.GetSearchResultPropertyStrValues(adouObjNeededProperty, searchResult));
					}
				}
				else if (searchResult.IsGroup())
				{
					adOjectWrapper.ObjectType = ADSchemaType.Groups;

					foreach (var adouObjNeededProperty in ADGroupObjNeededProperties)
					{
						adOjectWrapper.Properties.Add(adouObjNeededProperty,
							adHelper.GetSearchResultPropertyStrValues(adouObjNeededProperty, searchResult));
					}
				}
			}

			return adOjectWrapper;
		}

		public static bool IsOrgnization(this SearchResult searchResult)
		{
			var returnValue = false;

			if (searchResult != null)
			{
				//objectCategory=organizationalUnit
				var strValue = SynchronizeContext.Current.ADHelper.GetSearchResultPropertyStrValue("objectCategory", searchResult);
				if (strValue.StartsWith("CN=Organizational-Unit"))//if (strValue == "organizationalUnit")
				{
					returnValue = true;
				}
			}

			return returnValue;
		}

		public static bool IsGroup(this SearchResult searchResult)
		{
			var returnValue = false;

			if (searchResult != null)
			{
				//(objectCategory=group)(objectClass=group)
				foreach (var objClass in searchResult.Properties["objectClass"])
				{
					if (objClass.ToString() == "group")
					{
						returnValue = true;
						break;
					}
				}
			}

			return returnValue;
		}

		public static bool IsUser(this SearchResult searchResult)
		{
			var returnValue = false;

			if (searchResult != null)
			{
				//(objectCategory=person)(objectClass=user)
				foreach (var objClass in searchResult.Properties["objectClass"])
				{
					if (objClass.ToString() == "user")
					{
						returnValue = true;
						break;
					}
				}
			}

			return returnValue;
		}

		public static void WithDirectoryEntry(ADHelper adHelper, string nativeID, Action<DirectoryEntry> success, Action fail)
		{
			var result = SynchronizeHelper.GetSearchResultByID(adHelper, nativeID);
			if (result != null)
			{
				using (DirectoryEntry ent = result.GetDirectoryEntry())
				{
					if (ent != null)
					{
						if (success != null)
							success(ent);
					}
					else
					{
						if (fail != null)
							fail();
					}
				}
			}
			else
			{
				if (fail != null)
				{
					fail();
				}
			}
		}

		public static DirectoryEntry GetDirectoryEntryByID(ADHelper adHelper, string nativeID)
		{
			var result = SynchronizeHelper.GetSearchResultByID(adHelper, nativeID);
			if (result != null)
			{
				return result.GetDirectoryEntry();
			}
			else
			{
				return null;
			}
		}

		public static SearchResult GetSearchResultByDN(ADHelper adHelper, string DN, ADSchemaType schemaType)
		{
			ADSearchConditions condition = new ADSearchConditions(SearchScope.Subtree);
			condition.SizeLimit = 1;
			string filter = string.Empty;

			switch (schemaType)
			{
				case ADSchemaType.Organizations:
					filter = "(objectCategory=organizationalUnit)";
					break;
				case ADSchemaType.Users:
					filter = "(&(objectCategory=person)(objectClass=user))";
					break;
				case ADSchemaType.Groups:
					filter = "(&(objectCategory=group)(objectClass=group))";
					break;
			}

			filter = string.Format("(&(distinguishedName={0}){1})", AppendNamingContext(DN), filter);

			using (DirectoryEntry parentEntry = adHelper.GetRootEntry())
			{
				var searchList = adHelper.ExecuteSearch(parentEntry,
														filter,
														condition,
														ADObjNeededProperties);

				return searchList.FirstOrDefault();
			}
		}

		public static IEnumerable<SearchResult> GetSearchResultsByDNList(ADHelper adHelper, IEnumerable<string> rdnList, ADSchemaType schemaType, int sizeLimit)
		{
			return GetSearchResultsByDNList(adHelper, rdnList, schemaType, GetNeededProperties(schemaType), sizeLimit);
		}

		public static IEnumerable<SearchResult> GetSearchResultsByDNList(ADHelper adHelper, IEnumerable<string> rdnList, ADSchemaType schemaType, string[] properties, int sizeLimit)
		{
			ADSearchConditions condition = new ADSearchConditions(SearchScope.Subtree);
			condition.SizeLimit = sizeLimit;
			condition.PageSize = sizeLimit;
			var filter = ADSearchConditions.GetFilterByMask(schemaType);

			StringBuilder postfixBuilder = new StringBuilder();

			foreach (var val in rdnList)
			{
				postfixBuilder.Append("(").Append("distinguishedName=").Append(AppendNamingContext(val)).Append(")");
			}

			if (postfixBuilder.Length > 0)
			{
				var postExpression = postfixBuilder.Length > 0 ? "(|" + postfixBuilder.ToString() + ")" : string.Empty;
				filter = string.IsNullOrEmpty(filter) ? postExpression : "(&" + filter + postExpression + ")";
				using (DirectoryEntry parentEntry = adHelper.GetRootEntry())
				{
					var searchList = adHelper.ExecuteSearch(parentEntry,
															filter,
															condition,
															properties);

					return searchList;
				}
			}
			else
			{
				return new SearchResult[0];
			}
		}

		/// <summary>
		/// 基于单个属性的精确匹配多个结果
		/// </summary>
		/// <param name="adHelper"></param>
		/// <param name="propertyName">匹配的单一属性名称</param>
		/// <param name="propertyValues">属性值，注意如果是字符串，会替换其中的特殊字符，只能使用字节数组或基元类型。</param>
		/// <param name="schemaType"></param>
		/// <param name="properties">要提取的属性名</param>
		/// <param name="sizeLimit">返回结果的数量</param>
		/// <returns></returns>
		public static IEnumerable<SearchResult> GetSearchResultsByPropertyValues(ADHelper adHelper, string propertyName, IEnumerable propertyValues, ADSchemaType schemaType, string[] properties, int sizeLimit)
		{
			var filter = ADSearchConditions.GetFilterByMask(schemaType);
			return GetSearchResultsByPropertyValues(adHelper, propertyName, propertyValues, properties, sizeLimit, filter);
		}

		private static IEnumerable<SearchResult> GetSearchResultsByPropertyValues(ADHelper adHelper, string propertyName, IEnumerable propertyValues, string[] properties, int sizeLimit, string filter)
		{
			ADSearchConditions condition = new ADSearchConditions(SearchScope.Subtree);
			condition.SizeLimit = sizeLimit;
			condition.PageSize = sizeLimit;
			propertyName = ADHelper.EscapeString(propertyName);

			StringBuilder postfixBuilder = new StringBuilder();

			foreach (var val in propertyValues)
			{
				postfixBuilder.Append("(").Append(propertyName).Append("=").Append(ADHelper.EscapeValueForLdapQuery(val)).Append(")");
			}

			if (postfixBuilder.Length > 0)
			{
				var postExpression = postfixBuilder.Length > 0 ? "(|" + postfixBuilder.ToString() + ")" : string.Empty;
				filter = string.IsNullOrEmpty(filter) ? postExpression : "(&" + filter + postExpression + ")";
				using (DirectoryEntry parentEntry = adHelper.GetRootEntry())
				{
					var searchList = adHelper.ExecuteSearch(parentEntry,
															filter,
															condition,
															properties);

					return searchList;
				}
			}
			else
			{
				return new SearchResult[0];
			}
		}

		public static SearchResult GetSearchResultByID(ADHelper adHelper, string nativeID, ADSchemaType schemaType)
		{
			return GetSearchResultByID(adHelper, nativeID, GetNeededProperties(schemaType));
		}

		public static SearchResult GetSearchResultByID(ADHelper adHelper, string nativeID)
		{
			return GetSearchResultByID(adHelper, nativeID, ADObjNeededProperties);
		}

		public static SearchResult GetSearchResultByID(ADHelper adHelper, string nativeID, string[] neededProperties)
		{
			ADSearchConditions condition = new ADSearchConditions(SearchScope.Subtree);
			condition.SizeLimit = 1;
			string filter = string.Empty;

			filter = string.Format("(objectGuid={0})", ConvertGuidToOctectString(nativeID));

			using (DirectoryEntry parentEntry = adHelper.GetRootEntry())
			{
				var searchList = adHelper.ExecuteSearch(parentEntry,
														filter,
														condition,
														neededProperties);

				return searchList.FirstOrDefault();
			}
		}

		public static string[] GetNeededProperties(ADSchemaType schemaType)
		{
			string[] neededProperties = null;

			switch (schemaType)
			{
				case ADSchemaType.Organizations:
					neededProperties = ADOUObjNeededProperties;
					break;
				case ADSchemaType.Users:
					neededProperties = ADUserObjNeededProperties;
					break;
				case ADSchemaType.Groups:
					neededProperties = ADGroupObjNeededProperties;
					break;
				default:
					throw new NotSupportedException("不支持的类型：" + schemaType);
			}

			return neededProperties;
		}

		/// <summary>
		/// 获取搜索结果
		/// </summary>
		/// <param name="nativeIDs">一组NativeID</param>
		/// <param name="schemaType"></param>
		/// <param name="sizeLimit"></param>
		/// <returns></returns>
		public static IEnumerable<SearchResult> GetSearchResultsByIDs(ADHelper adHelper, IEnumerable<string> nativeIDs, ADSchemaType schemaType, int sizeLimit)
		{
			return GetSearchResultsByIDs(adHelper, nativeIDs, GetNeededProperties(schemaType), sizeLimit);
		}

		public static IEnumerable<SearchResult> GetSearchResultsByIDs(ADHelper adHelper, IEnumerable<string> nativeIDs, string[] properties, int sizeLimit)
		{
			return GetSearchResultsByPropertyValues(adHelper, "objectGuid", new NativeGuidConverter(nativeIDs), properties, sizeLimit, null);
		}

		private static string ConvertGuidToOctectString(Guid guid)
		{
			byte[] byteGuid = guid.ToByteArray();

			char[] buf = new char[byteGuid.Length * 3];

			int i = 0;
			for (int k = 0; k < byteGuid.Length; k++)
			{
				buf[i++] = '\\';
				byte b = byteGuid[k];
				buf[i++] = hexDigits[((b & 0xf0) >> 4)];
				buf[i++] = hexDigits[b & 0x0f];
			}

			return new string(buf);
		}

		private static string ConvertGuidToOctectString(string nativeId)
		{
			int size = nativeId.Length >> 1;
			char[] buf = new char[size * 3];

			int i = 0;
			for (int k = 0; k < size; k++)
			{
				buf[i++] = '\\';

				buf[i++] = nativeId[k << 1];
				buf[i++] = nativeId[(k << 1) + 1];
			}

			return new string(buf);
		}

		private static byte[] ConvertGuidToBinary(string nativeId)
		{
			int size = nativeId.Length >> 1;
			byte[] buf = new byte[size];

			for (int k = 0; k < size; k++)
			{
				buf[k] = (byte)((AttributeHelper.FromHex(nativeId[k << 1]) << 4) | (AttributeHelper.FromHex(nativeId[(k << 1) + 1])));
			}

			return buf;
		}

		public static SearchResult GetSearchResultByLogonName(string logonName, ADSchemaType schemaType)
		{
			ADHelper adHelper = SynchronizeContext.Current.ADHelper;
			ADSearchConditions condition = new ADSearchConditions(SearchScope.Subtree);
			condition.SizeLimit = 1;
			string filter = string.Empty;

			string[] neededProperties = GetNeededProperties(schemaType);

			filter = string.Format("(samAccountName={0})", ADHelper.EscapeValueForLdapQuery(logonName));

			using (DirectoryEntry parentEntry = adHelper.GetRootEntry())
			{
				var searchList = adHelper.ExecuteSearch(parentEntry,
														filter,
														condition,
														neededProperties);

				return searchList.FirstOrDefault();
			}
		}

		#endregion

		#region ADObjectWrapper系列

		public static List<ADObjectWrapper> SearchChildren(ADObjectWrapper adObject)
		{
			List<ADObjectWrapper> result = new List<ADObjectWrapper>();

			ADHelper adHelper = SynchronizeContext.Current.ADHelper;
			ADSearchConditions condition = new ADSearchConditions(SearchScope.OneLevel);

			using (DirectoryEntry parentEntry = adHelper.NewEntry(adObject.DN))
			{
				List<SearchResult> searchList = adHelper.ExecuteSearch(parentEntry,
														ADSearchConditions.GetFilterByMask(ADSchemaType.Users | ADSchemaType.Organizations | ADSchemaType.Groups),
														condition,
														ADObjNeededProperties);

				foreach (var searchResult in searchList)
				{
					result.Add(searchResult.ToADOjectWrapper());
				}
			}

			return result;
		}

		public static List<ADObjectWrapper> FindAllChildrenUser(ADObjectWrapper adObject)
		{
			List<ADObjectWrapper> result = new List<ADObjectWrapper>();

			ADHelper adHelper = SynchronizeContext.Current.ADHelper;
			ADSearchConditions condition = new ADSearchConditions(SearchScope.Subtree);
			condition.PageSize = 1000;

			using (DirectoryEntry parentEntry = adHelper.NewEntry(adObject.DN))
			{
				List<SearchResult> searchList = adHelper.ExecuteSearch(parentEntry,
														ADSearchConditions.GetFilterByMask(ADSchemaType.Users),
														condition,
														ADUserObjNeededProperties);

				foreach (SearchResult searchResult in searchList)
				{
					result.Add(searchResult.ToADOjectWrapper());
				}
			}

			return result;
		}

		public static List<ADObjectWrapper> FindAllChildrenUser(this DirectoryEntry entry)
		{
			List<ADObjectWrapper> result = new List<ADObjectWrapper>();

			ADHelper adHelper = SynchronizeContext.Current.ADHelper;
			ADSearchConditions condition = new ADSearchConditions(SearchScope.Subtree);
			condition.PageSize = 1000;

			List<SearchResult> searchList = adHelper.ExecuteSearch(entry,
														ADSearchConditions.GetFilterByMask(ADSchemaType.Users),
														condition,
														ADUserObjNeededProperties);

			foreach (SearchResult searchResult in searchList)
				result.Add(searchResult.ToADOjectWrapper());

			return result;
		}
		#endregion

		#region ADHelper 系列

		/// <summary>
		/// 需要同步上下文，获取缓存的命名上下文
		/// </summary>
		/// <returns></returns>
		private static string GetCachedNamingContext()
		{
			string myKey = "ADNamingContext";

			string result = ObjectCacheQueue.Instance.GetOrAddNewValue(myKey, (cache, key) =>
			{
				string namingContext = SynchronizeContext.Current.ADHelper.GetNamingContext();
				SlidingTimeDependency dependency = new SlidingTimeDependency(TimeSpan.FromMinutes(10));
				cache.Add(key, namingContext, dependency);
				return namingContext;
			}).ToString();

			return result;
		}

		public static string AppendNamingContext(string strDN)
		{
			string upperCaseDN = strDN.ToUpper();

			if (upperCaseDN.IndexOf("DC=") == -1)
			{
				if (strDN != string.Empty)
					strDN = strDN + "," + GetCachedNamingContext();
				else
					strDN = GetCachedNamingContext();
			}

			return strDN;
		}

		#endregion

		#region 延迟操作系列
		/// <summary>
		/// 要移动的目标位置存在类似对象，则先改名后再移动，并且延迟操作中再改回。
		/// </summary>
		/// <param stringValue="oguObject"></param>
		/// <param stringValue="targetObject"></param>
		/// <param stringValue="parentEntry"></param>
		public static void SolveConflictAboutMove(IOguObject oguObject, DirectoryEntry targetObject, DirectoryEntry parentEntry)
		{
			string magic = oguObject.Name + DateTime.Now.ToString("yyMMddHHmmss") + SynchronizeContext.Current.DelayActions.Count;

			targetObject.Rename(oguObject.ObjectType.SchemaTypeToPrefix() + "=" + ADHelper.EscapeString(magic));
			targetObject.CommitChanges();

			targetObject.MoveTo(parentEntry);

			SynchronizeContext.Current.DelayActions.Add(new DelayRenameAction(oguObject, targetObject.NativeGuid));
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oguObject"></param>
		/// <returns></returns>
		public static string GetRelativePath(IOguObject oguObject)
		{
			if (oguObject.FullPath == SynchronizeContext.Current.SourceRootPath)
			{
				return string.Empty;
			}
			else if (oguObject.FullPath.StartsWith(SynchronizeContext.Current.SourceRootPath, StringComparison.Ordinal) && oguObject.FullPath.Length > 0 && oguObject.FullPath[oguObject.FullPath.Length - 1] != '\\')
			{
				return oguObject.FullPath.Substring(SynchronizeContext.Current.SourceRootPath.Length + 1);
			}
			else
			{
				throw new ArgumentOutOfRangeException("oguObject", "不在同步范围的路径");
			}
		}

		public static string GetParentObjectDN(IOguObject oguObject)
		{
			string[] rdns = GetOguObjectRdns(oguObject);
			return string.Join(",", rdns, 1, rdns.Length - 1);
		}

		public static string GetOguObjectDN(IOguObject oguObject)
		{
			StringBuilder strResult = new StringBuilder(oguObject.FullPath.Length * 2);

			if (SynchronizeContext.Current.IsIncluded(oguObject) == false)
				throw new InvalidOperationException("对象不在同步范围中");

			string relativePath = SynchronizeHelper.GetRelativePath(oguObject);
			if (relativePath.Length > 0)
			{
				var paths = relativePath.Split('\\');

				if (paths.Length > 0)
				{
					paths[0] = SynchronizeContext.Current.GetMappedName(paths[0]); // 一级目录

					switch (oguObject.ObjectType)
					{
						case SchemaType.Organizations:

							for (int i = paths.Length - 1; i > 0; i--)
							{
								strResult.Append("OU=").Append(ADHelper.EscapeString(paths[i])).Append(",");
							}

							strResult.Append(paths[0]);

							break;
						case SchemaType.Users:
						case SchemaType.Groups:

							string name = "CN=";

							for (int i = paths.Length - 1; i > 0; i--)
							{
								strResult.Append(name).Append(ADHelper.EscapeString(paths[i])).Append(",");
								name = "OU=";
							}

							strResult.Append(paths[0]);

							break;
					}
				}
			}

			if (strResult.Length > 0 && string.IsNullOrEmpty(SynchronizeContext.Current.TargetRootOU) == false)
			{
				strResult.Append(",").Append(SynchronizeContext.Current.TargetRootOU);
			}

			return strResult.ToString();
		}

		/// <summary>
		/// 获取对象RDN的数组，数组最开始为对象RDN
		/// </summary>
		/// <param name="oguObject"></param>
		/// <returns></returns>
		public static string[] GetOguObjectRdns(IOguObject oguObject)
		{
			if (SynchronizeContext.Current.IsIncluded(oguObject) == false)
				throw new InvalidOperationException("对象不在同步范围中");

			string relativePath = SynchronizeHelper.GetRelativePath(oguObject);
			if (relativePath.Length > 0)
			{
				string[] paths = relativePath.Split('\\');

				if (paths.Length > 0)
				{
					paths[0] = SynchronizeContext.Current.GetMappedName(paths[0]); // 一级目录

					switch (oguObject.ObjectType)
					{
						case SchemaType.Organizations:

							for (int i = paths.Length - 1; i > 0; i--)
							{
								paths[i] = "OU=" + ADHelper.EscapeString(paths[i]);
							}

							break;
						case SchemaType.Users:
						case SchemaType.Groups:

							paths[paths.Length - 1] = "CN=" + ADHelper.EscapeString(paths[paths.Length - 1]);

							for (int i = paths.Length - 2; i > 0; i--)
							{
								paths[i] = "OU=" + ADHelper.EscapeString(paths[i]);
							}
							break;
					}
				}

				if (string.IsNullOrEmpty(SynchronizeContext.Current.TargetRootOU) == false)
				{
					string[] temp = new string[paths.Length + 1];
					Array.Copy(paths, 0, temp, 1, paths.Length);
					temp[0] = SynchronizeContext.Current.TargetRootOU;
					paths = temp;
				}

				Array.Reverse(paths);

				return paths;
			}
			else
			{
				return new string[0];
			}
		}

		/// <summary>
		/// 将组织的路径转换为DN，注意路径必须以\作为分隔符，路径中的特殊字符会自动转义，遇到两个\\作为斜线处理。
		/// </summary>
		/// <param stringValue="val"></param>
		/// <returns></returns>
		public static string OUPathToDN(string path)
		{
			string[] parts = new PathPartEnumerator(path).ToArray();

			return OUPathToDN(parts, parts.Length);
		}

		/// <summary>
		/// 仅当斜杠不是转义符，可以这样玩
		/// </summary>
		/// <param name="path1"></param>
		/// <param name="path2"></param>
		/// <returns></returns>
		public static string CombinePath(string path1, string path2)
		{
			string result = null;
			if (path2.Length == 0)
			{
				result = path1;
			}
			else if (path1.Length == 0)
			{
				result = path2;
			}
			else
			{
				bool path2Rooted = path2.StartsWith("\\", StringComparison.Ordinal);
				char ch = path1[path1.Length - 1];
				if ((ch == '\\' && path2Rooted == false) || (ch != '\\' && path2Rooted))
				{
					result = path1 + path2;
				}
				else if (ch != '\\' && path2Rooted == false)
				{
					result = path1 + '\\' + path2;
				}
				else
				{
					result = path1 + path2.Substring(1);
				}
			}

			return result;
		}

		public static string OUPathToDN(string[] parts, int level)
		{
			if (level < 1 || level > parts.Length)
				throw new ArgumentOutOfRangeException("level");

			StringBuilder strB = new StringBuilder();

			for (int i = level - 1; i >= 0; i--)
			{
				if (strB.Length > 0)
					strB.Append(",");

				strB.Append("OU=" + ADHelper.EscapeString(parts[i]));
			}

			return strB.ToString();
		}

		public static List<string> GetAllParentPaths(string fullPath)
		{
			List<string> list = new List<string>();
			string[] paths = fullPath.Split('\\');

			if (paths.Length > 1)
			{
				for (int i = 0; i < paths.Length - 1; i++)
				{
					var curPath = paths[0];

					for (int j = 1; j < i + 1; j++)
					{
						curPath += "\\" + paths[j];
					}

					list.Add(curPath);
				}
			}

			return list;
		}

		public static DateTime GetADAccountExpiresDate(object objDateTime)
		{
			DateTime defaultExpiredDate = new DateTime(1970, 1, 1);
			DateTime result = defaultExpiredDate;

			if (objDateTime != null)
			{
				DateTime dateTime = (DateTime)objDateTime;

				if (dateTime >= defaultExpiredDate)
				{
					result = dateTime;
				}
			}

			return result;
		}

		public static int GetADUserAccountControlValue(object curValue, bool accountDisabled, bool passwordNotRequired, bool dontExpirePassword)
		{
			if (curValue == null)
			{
				return (int)(ADS_USER_FLAG.ADS_UF_NORMAL_ACCOUNT | ADS_USER_FLAG.ADS_UF_PASSWD_NOTREQD);
			}

			int result = (int)curValue;

			result = SetAccountValueFlag(result, accountDisabled, ADS_USER_FLAG.ADS_UF_ACCOUNTDISABLE);
			result = SetAccountValueFlag(result, passwordNotRequired, ADS_USER_FLAG.ADS_UF_PASSWD_NOTREQD);
			result = SetAccountValueFlag(result, dontExpirePassword, ADS_USER_FLAG.ADS_UF_DONT_EXPIRE_PASSWD);

			return result;
		}

		private static int SetAccountValueFlag(int originalValue, bool flag, ADS_USER_FLAG relativeFlag)
		{
			int result = originalValue;

			if (flag)
				result |= (int)relativeFlag;
			else
				result &= ~(int)relativeFlag;

			return result;
		}

		/// <summary>
		/// 检查指定的路径是否，或者为此路径的子路径
		/// </summary>
		/// <param name="dn">子路径</param>
		/// <param name="item">父路径</param>
		/// <returns></returns>
		public static bool IsOrInPath(string path, string parentPath)
		{
			if (path.EndsWith("\\", StringComparison.Ordinal))
				throw new FormatException("路径不得以\\结尾");

			bool result = false;
			if (parentPath == path)
			{
				result = true;
			}
			else
			{
				if (path.StartsWith(parentPath, StringComparison.Ordinal) == false)
					result = false;
				else
					result = parentPath.Length < path.Length && path[parentPath.Length] == '\\';
			}

			return result;
		}
	}
}