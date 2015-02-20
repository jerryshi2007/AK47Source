using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library;
using MCS.Library.Core;
using MCS.Library.Logging;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class ADObjectModifier
	{
		public static void ApplyModify()
		{
			SynchronizeContext context = SynchronizeContext.Current;

			context.DeletedOuAndUserItems.ForEach(item => { SynchronizeContext.Current.ExtendLockTime(); ADObjectModifier.ApplyModify(item); });
			context.ModifiedOuAndUserItems.ForEach(item => { SynchronizeContext.Current.ExtendLockTime(); ADObjectModifier.ApplyModify(item); });
			context.DeletedGroupItems.ForEach(item => { SynchronizeContext.Current.ExtendLockTime(); ADObjectModifier.ApplyModify(item); });
			context.ModifiedGroupItems.ForEach(item => { SynchronizeContext.Current.ExtendLockTime(); ADObjectModifier.ApplyModify(item); });

			// 执行后续操作
			context.DelayActions.DoActions(context);

			ModifyGroups(context.GroupsToTakeCare, context);
		}

		private static void ModifyGroups(KeyedOguObjectCollection groups, SynchronizeContext context)
		{
			for (int i = 0; i < groups.Count / SynchronizeHelper.ADQueryBatchSize; i++)
				ModifyBatchGroups(groups, (i * SynchronizeHelper.ADQueryBatchSize), SynchronizeHelper.ADQueryBatchSize, context);

			ModifyBatchGroups(groups, groups.Count / SynchronizeHelper.ADQueryBatchSize, groups.Count % SynchronizeHelper.ADQueryBatchSize, context);
		}

		private static void ModifyBatchGroups(KeyedOguObjectCollection groups, int startIndex, int batchSize, SynchronizeContext context)
		{
			//Key是AD Guid，Value是Ogu ID
			Dictionary<string, string> adToOguMapping = GetADToOguMappingFromGroups(groups, startIndex, batchSize, context);

			string[] propertiesToGet = { "objectguid", "member" };
			IEnumerable<SearchResult> adGroups = SynchronizeHelper.GetSearchResultsByIDs(context.ADHelper, adToOguMapping.Keys, propertiesToGet, batchSize);

			foreach (SearchResult adGroup in adGroups)
			{
				SynchronizeContext.Current.ExtendLockTime();

				string guid = AttributeHelper.Hex((byte[])adGroup.Properties["objectguid"][0]);

				ADGroupMemberCollection adGroupMembers = new ADGroupMemberCollection(adGroup.Properties["member"], new string[] { "objectGUID" });

				Dictionary<string, ADObjectWrapper> adMemberDict = adGroupMembers.ToDictionary(o => o.NativeGuid);
				IGroup oguGroup = (IGroup)groups[adToOguMapping[guid]];

				if (IsSameMembers(oguGroup.Members, adMemberDict) == false)
					SyncGroupMembers(oguGroup, adGroup, context);
			}
		}

		private static void SyncGroupMembers(IGroup oguGroup, SearchResult adGroup, SynchronizeContext context)
		{
			using (DirectoryEntry adEntry = adGroup.GetDirectoryEntry())
			{
				try
				{
					var members = adEntry.Properties["member"];
					members.Clear();

					AddGroupMembers(oguGroup.Members.ToIDList(), members, context);

					adEntry.CommitChanges();
				}
				catch (Exception ex)
				{
					context.ExceptionCount++;

					LogHelper.WriteSynchronizeDBLogDetail(SynchronizeContext.Current.SynchronizeID, "修改群组成员", oguGroup.ID, oguGroup.Name,
									 context.ADHelper.GetPropertyStrValue("objectGuid", adEntry),
									 context.ADHelper.GetPropertyStrValue("distinguishedName", adEntry),
									 string.Format("修改群组成员时出错:" + ex.Message));
				}
			}
		}

		//可考虑扩展方法
		private static bool IsSameMembers(OguObjectCollection<IUser> oguGroupMembers, Dictionary<string, ADObjectWrapper> adGroupMembers)
		{
			bool result = true;

			if (oguGroupMembers.Count == adGroupMembers.Count)
			{
				foreach (IUser user in oguGroupMembers)
				{
					if (adGroupMembers.ContainsKey(user.ID) == false)
					{
						result = false;
						break;
					}
				}
			}
			else
				result = false;

			return result;
		}

		private static Dictionary<string, string> GetADToOguMappingFromGroups(KeyedOguObjectCollection groups, int startIndex, int batchSize, SynchronizeContext context)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();

			for (int i = startIndex; i < startIndex + batchSize; i++)
			{
				SynchronizeContext.Current.ExtendLockTime();

				IDMapping mapping = context.IDMapper.GetAdObjectMapping(groups[i].ID);

				result.Add(mapping.ADObjectGuid, mapping.SCObjectID);
			}

			return result;
		}

		private static void AddGroupMembers(IList<string> oguMemberIDs, PropertyValueCollection adGroupMembers, SynchronizeContext context)
		{
			for (int i = 0; i < oguMemberIDs.Count / SynchronizeHelper.ADQueryBatchSize; i++)
			{
				SynchronizeContext.Current.ExtendLockTime();

				FillGroupMembersByOguIDs(CopyPartOfList(oguMemberIDs, i * SynchronizeHelper.ADQueryBatchSize, SynchronizeHelper.ADQueryBatchSize), adGroupMembers, context);
			}

			FillGroupMembersByOguIDs(CopyPartOfList(oguMemberIDs,
				(oguMemberIDs.Count / SynchronizeHelper.ADQueryBatchSize) * SynchronizeHelper.ADQueryBatchSize,
				oguMemberIDs.Count % SynchronizeHelper.ADQueryBatchSize),
				adGroupMembers, context);
		}

		private static void FillGroupMembersByOguIDs(IEnumerable<string> oguObjectIDs, PropertyValueCollection adGroupMembers, SynchronizeContext context)
		{
			IEnumerable<SearchResult> result = SynchronizeHelper.GetSearchResultsByIDs(context.ADHelper, context.IDMapper.GetMappedADObjectIDs(oguObjectIDs),
				new string[] { "distinguishedName" }, oguObjectIDs.Count());

			foreach (SearchResult sr in result)
			{
				SynchronizeContext.Current.ExtendLockTime();

				adGroupMembers.Add(sr.Properties["distinguishedName"][0]);
			}
		}

		private static List<string> CopyPartOfList(IList<string> source, int startIndex, int count)
		{
			List<string> result = new List<string>();

			for (int i = startIndex; i < startIndex + count; i++)
				result.Add(source[i]);

			return result;
		}

		private static void ApplyModify(ModifiedItem item)
		{
			try
			{
				if ((item.ModifyType & ObjectModifyType.Delete) == ObjectModifyType.Delete)
				{
					// 只要有删除动作，则不做其它动作
					SynchronizeContext.Current.IncreaseDeleteItemCount(() => DeleteADObject(ObjectModifyType.Delete, item.OguObjectData, item.ADObjectData));
				}
				else if ((item.ModifyType & ObjectModifyType.Add) == ObjectModifyType.Add)
				{
					SynchronizeContext.Current.IncreaseAddItemCount(() => AddADObject(item));

					if ((item.ModifyType & ObjectModifyType.PropertyModified) == ObjectModifyType.PropertyModified)
						SynchronizeContext.Current.IncreaseModifyItemCount(() => ModifyADObjectProperties(ObjectModifyType.PropertyModified, item));
				}
				else
				{
					if ((item.ModifyType & ObjectModifyType.PropertyModified) == ObjectModifyType.PropertyModified)
						SynchronizeContext.Current.IncreaseModifyItemCount(() => ModifyADObjectProperties(ObjectModifyType.PropertyModified, item));

					if ((item.ModifyType & ObjectModifyType.MissingMarker) == ObjectModifyType.MissingMarker)
						ModifyADObjectProperties(ObjectModifyType.MissingMarker, item);

					// if ((item.ModifyType & ObjectModifyType.ChildrenModified) == ObjectModifyType.ChildrenModified)
					//     SynchronizeContext.Current.IncreaseModifyItemCount(() => ModifyADGroupChildren(ObjectModifyType.ChildrenModified, item));
				}
			}
			catch (Exception ex)
			{
				SynchronizeContext.Current.ExceptionCount++;

				LogHelper.WriteSynchronizeDBLogDetail(SynchronizeContext.Current.SynchronizeID, "应用变更:" + item.ModifyType,
					item.OguObjectData == null ? string.Empty : item.OguObjectData.ID,
					item.OguObjectData == null ? string.Empty : item.OguObjectData.Name,
					item.ADObjectData == null ? string.Empty : item.ADObjectData.NativeGuid.ToString(),
					item.ADObjectData == null ? string.Empty : item.ADObjectData.DN, ex.GetRealException().ToString());
			}
		}

		private static void AddADObject(ModifiedItem item)
		{
			var oguObject = item.OguObjectData;
			string[] rdns = SynchronizeHelper.GetOguObjectRdns(oguObject);

			string parentDN = string.Join(",", rdns, 1, rdns.Length - 1);

			ADHelper adHelper = SynchronizeContext.Current.ADHelper;

			using (DirectoryEntry parentEntry = parentDN.IsNullOrEmpty() ? adHelper.GetRootEntry() : adHelper.NewEntry(parentDN))
			{
				parentEntry.ForceBound();
				using (DirectoryEntry newEntry = WhatEverNewEntry(oguObject, parentEntry))
				{
					ObjectSetterHelper.Convert(ObjectModifyType.Add, oguObject, newEntry);

					//item.ADObjectData = SynchronizeHelper.GetSearchResultByID(newEntry.NativeGuid, newEntry.ToADSchemaType()).ToADOjectWrapper();

					if (SynchronizeContext.Current.IDMapper.NewIDMappingDictionary.ContainsKey(oguObject.ID) &&
						SynchronizeContext.Current.IDMapper.NewIDMappingDictionary[oguObject.ID] == null)
					{
						SynchronizeContext.Current.IDMapper.NewIDMappingDictionary[oguObject.ID] = new IDMapping()
						{
							SCObjectID = oguObject.ID,
							ADObjectGuid = newEntry.NativeGuid,
							LastSynchronizedVersionTime = (DateTime)oguObject.Properties["VERSION_START_TIME"]
						};
					}
				}

			}
		}

		private static DirectoryEntry WhatEverNewEntry(IOguObject oguObject, DirectoryEntry parentEntry)
		{
			try
			{
				return parentEntry.Children.Add(GetADTargetName(oguObject), ((ADSchemaType)oguObject.ObjectType).ToObjectClass());
			}
			catch (DirectoryServicesCOMException cex)
			{
				if (cex.ErrorCode == -2147019886)
				{
					string demoName = oguObject.Name + DateTime.Now.ToString("yyyyMMddHHmmss") + SynchronizeContext.Current.DelayActions.Count;
					var demo = parentEntry.Children.Add(oguObject.ObjectType.SchemaTypeToPrefix() + "=" + ADHelper.EscapeString(demoName), ((ADSchemaType)oguObject.ObjectType).ToObjectClass());
					// 重名
					SynchronizeContext.Current.DelayActions.Add(new DelayRenameAction(oguObject, demo.NativeGuid));

					return demo;
				}
				else
				{
					throw;
				}
			}

		}

		private static string GetADTargetName(IOguObject oguObject)
		{
			SchemaMappingConfigurationElement mappingElement = PermissionCenterToADSynchronizeSettings.GetConfig().SchemaMappings[oguObject.ObjectType.ToString()];

			string prefix = mappingElement.Prefix;
			string targetName = SynchronizeContext.Current.GetMappedName(oguObject, prefix);

			//string targetOU = SynchronizeContext.Current.TargetRootOU;

			//if (targetOU.IsNotEmpty())
			//{
			//    //???
			//    if (SynchronizeHelper.GetOguObjectDN(oguObject) == targetOU)
			//        targetName = new PathPartEnumerator(SynchronizeContext.Current.TargetRootOU).Last();
			//}

			return targetName; //prefix + "=" + ADHelper.EscapeString(targetName);
		}

		private static void DeleteADObject(ObjectModifyType modifyType, IOguObject oguObject, ADObjectWrapper adObject)
		{
			ADHelper adHelper = SynchronizeContext.Current.ADHelper;

			using (DirectoryEntry entry = SynchronizeHelper.GetSearchResultByID(adHelper, adObject.NativeGuid).GetDirectoryEntry())
			{
				entry.ForceBound();
				ObjectSetterHelper.Convert(modifyType, oguObject, entry);
			}
		}

		private static void ModifyADObjectProperties(ObjectModifyType modifyType, ModifiedItem item)
		{
			IOguObject oguObject = item.OguObjectData;
			ADObjectWrapper adObject = item.ADObjectData;
			using (DirectoryEntry entry = SynchronizeHelper.GetSearchResultByID(SynchronizeContext.Current.ADHelper, adObject.NativeGuid, (ADSchemaType)oguObject.ObjectType).GetDirectoryEntry())
			{
				entry.ForceBound();
				ObjectSetterHelper.Convert(modifyType, oguObject, entry);
			}
		}

		private static void ModifyADGroupChildren(ObjectModifyType modifyType, ModifiedItem item)
		{
			ADHelper adHelper = SynchronizeContext.Current.ADHelper;
			IOguObject oguObject = item.OguObjectData;
			ADObjectWrapper adObject = item.ADObjectData;

			using (DirectoryEntry entry = SynchronizeHelper.GetSearchResultByID(adHelper, adObject.NativeGuid, ADSchemaType.Groups).GetDirectoryEntry())
			{
				entry.ForceBound();
				ObjectSetterHelper.Convert(modifyType, oguObject, entry);
			}
		}
	}
}