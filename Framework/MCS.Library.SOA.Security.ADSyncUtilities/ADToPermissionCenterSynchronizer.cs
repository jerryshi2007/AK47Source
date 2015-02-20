using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.Locks;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class ADToPermissionCenterSynchronizer
	{
		private static string SynchronizationLockID = "29134792-17F5-418E-8359-9AE3AE9FB459";

		/// <summary>
		/// 同步操作的锁的超时时间
		/// </summary>
		private static readonly TimeSpan SynchronizationEffectiveTime = new TimeSpan(0, 1, 0);

		/// <summary>
		/// 延长锁生命期的检查间隔（10秒）
		/// </summary>
		private static readonly TimeSpan ExtendLockInterval = new TimeSpan(0, 0, 10);

		/// <summary>
		/// 最后一次延长锁生命期的时间
		/// </summary>
		private DateTime lastExtendLockTime = DateTime.MaxValue;
		private SCLock synchronizeLock;

		public Entity.ADReverseSynchronizeLog Log { get; private set; }

		private void Lock()
		{
			SCLock syncLock = new SCLock() { LockID = SynchronizationLockID, LockType = SCLockType.DataOperation };

			syncLock.EffectiveTime = SynchronizationEffectiveTime;
			syncLock.Description = "AD反向同步到权限中心";

			SCCheckLockResult checkResult = SCLockAdapter.Instance.AddLock(syncLock);

			if (checkResult.Available == false)
				throw new SCCheckLockException(SCCheckLockException.CheckLockResultToMessage(checkResult));

			this.synchronizeLock = syncLock;
			this.lastExtendLockTime = DateTime.Now;
		}

		private void Unlock()
		{
			if (this.synchronizeLock != null)
			{
				SCLockAdapter.Instance.DeleteLock(this.synchronizeLock);
				this.synchronizeLock = null;
				this.lastExtendLockTime = DateTime.MinValue;
			}
		}

		private void ExtendLockTime()
		{
			if (this.synchronizeLock != null)
			{
				if (DateTime.Now.Subtract(this.lastExtendLockTime) > ExtendLockInterval)
				{
					SCLockAdapter.Instance.ExtendLockTime(this.synchronizeLock);
					this.lastExtendLockTime = DateTime.Now;
				}
			}
		}

		private ADToPermissionCenterSynchronizer(string taskID)
		{
			DateTime now = DateTime.Now;

			this.Log = new ADReverseSynchronizeLog()
			{
				CreateTime = now,
				StartTime = now,
				EndTime = now,
				LogID = taskID,
				OperatorID = "PermissionCenterServices",
				OperatorName = "PermissionCenterServices",
				Status = ADSynchronizeResult.Running,
			};
		}

		public static ADReverseSynchronizeLog Start()
		{
			return Start(UuidHelper.NewUuidString());
		}

		public static ADReverseSynchronizeLog Start(string taskID)
		{
			taskID = string.IsNullOrEmpty(taskID) ? UuidHelper.NewUuidString() : taskID;
			ADToPermissionCenterSynchronizer context = new ADToPermissionCenterSynchronizer(taskID);
			context.Lock();
			try
			{
				Adapters.ADReverseSynchronizeLogAdapter.Instance.Update(context.Log);

				int batchSize = 32;
				EntityMappingCollection mappings = LoadMappingData();

				ADHelper adHelper = CreateADHelper();

				for (int i = 0; i < mappings.Count / batchSize; i++)
				{
					ProcessBatch(context, mappings, adHelper, i * batchSize, batchSize);
				}

				ProcessBatch(context, mappings, adHelper, mappings.Count / batchSize * batchSize, mappings.Count % batchSize);

				if (context.Log.Status == ADSynchronizeResult.Running)
					context.Log.Status = ADSynchronizeResult.Correct;
			}
			catch (Exception ex)
			{
				context.Log.Status = ADSynchronizeResult.Interrupted;
				LogHelper.WriteEventLog("反向同步时出现问题", ex.ToString(), Logging.LogPriority.Lowest, TraceEventType.Stop);
				Trace.TraceError(ex.ToString());
			}
			finally
			{
				context.Log.EndTime = DateTime.Now;
				Adapters.ADReverseSynchronizeLogAdapter.Instance.Update(context.Log);
				context.Unlock();
			}

			return context.Log;
		}

		private static void ProcessBatch(ADToPermissionCenterSynchronizer context, EntityMappingCollection mappings, ADHelper adHelper, int startIndex, int size)
		{
			string[] propertiesToGet = { "sAMAccountName", "mail", "msRTCSIP-PrimaryUserAddress" };

			IEnumerable<SearchResult> adResults = SynchronizeHelper.GetSearchResultsByPropertyValues(adHelper, "sAMAccountName", mappings.ToKeyArray("CodeName", startIndex, size), ADSchemaType.Users, propertiesToGet, size);

			Dictionary<string, SimpleUser> codeNameDict = mappings.ToCodeNameDictionary(startIndex, size);

			List<SimpleUser> changes = DiscoverChanges(codeNameDict, adResults);

			ApplyChanges(changes, context);
		}

		private static List<SimpleUser> DiscoverChanges(Dictionary<string, SimpleUser> codeNameDict, IEnumerable<SearchResult> adResults)
		{
			List<SimpleUser> changes = new List<SimpleUser>();

			foreach (SearchResult item in adResults)
			{
				SimpleUser entity = codeNameDict[(string)item.Properties["sAMAccountName"][0]];

				if (IsDifferent(item, entity))
				{
					entity.Tag = item;
					changes.Add(entity);
				}
			}

			return changes;
		}

		private static void ApplyChanges(List<SimpleUser> changes, ADToPermissionCenterSynchronizer context)
		{
			string[] changesIdArray = changes.Select<SimpleUser, string>(m => m.SCObjectID).ToArray();
			PC.SchemaObjectCollection pcObjects = LoadSCObjects(changesIdArray);

			foreach (SimpleUser item in changes)
			{
				if (pcObjects.ContainsKey(item.SCObjectID))
				{
					try
					{
						PC.SchemaObjectBase scObj = MeargeChanges(item.Tag, pcObjects[item.SCObjectID]);

						PC.Adapters.SchemaObjectAdapter.Instance.Update(scObj);
						context.Log.NumberOfModifiedItems++;
					}
					catch (Exception ex)
					{
						context.Log.NumberOfExceptions++;
						context.Log.Status = ADSynchronizeResult.HasError;
						LogHelper.WriteReverseSynchronizeDBLogDetail(context.Log.LogID, item.SCObjectID, AttributeHelper.Hex((byte[])item.Tag.Properties["objectguid"][0]), item.CodeName, ex.Message, ex.ToString());
						Trace.TraceError("未成功更新," + ex.ToString());
					}
				}
				else
				{
					LogHelper.WriteReverseSynchronizeDBLogDetail(context.Log.LogID, item.SCObjectID, AttributeHelper.Hex((byte[])item.Tag.Properties["objectguid"][0]), item.CodeName, "未找到AD对象对应的权限中心对象。", null);
				}
			}
		}

		private static PC.SchemaObjectBase MeargeChanges(System.DirectoryServices.SearchResult searchResult, PC.SchemaObjectBase schemaObjectBase)
		{
			PC.SCUser user = (PC.SCUser)schemaObjectBase;

			if (searchResult.Properties.Contains("msRTCSIP-PrimaryUserAddress"))
				user.Properties.SetValue("Sip", searchResult.Properties["msRTCSIP-PrimaryUserAddress"][0].ToString());

			if (searchResult.Properties.Contains("mail"))
				user.Properties.SetValue("Mail", searchResult.Properties["mail"][0].ToString());

			return schemaObjectBase;
		}

		private static PC.SchemaObjectCollection LoadSCObjects(string[] changesIdArray)
		{
			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("ID");
			inBuilder.AppendItem(changesIdArray);

			return PC.Adapters.SchemaObjectAdapter.Instance.Load(inBuilder, DateTime.MinValue);
		}

		private static bool IsDifferent(System.DirectoryServices.SearchResult item, SimpleUser entity)
		{
			bool same = true;

			if (item.Properties.Contains("mail") && item.Properties["mail"][0].ToString() != entity.Mail)
				same = false;
			else
			{
				if (item.Properties.Contains("msRTCSIP-PrimaryUserAddress") && item.Properties["msRTCSIP-PrimaryUserAddress"][0].ToString() != entity.Sip)
					same = false;
			}

			return !same;
		}

		private static EntityMappingCollection LoadMappingData()
		{
			return Adapters.SimpleUserAdapter.Instance.LoadAllMappings();
		}

		private static ADHelper CreateADHelper()
		{
			//初始化域控制器的配置信息
			ServerInfoConfigureElement serverSetting = ServerInfoConfigSettings.GetConfig().ServerInfos["dc"];
			ServerInfo serverInfo = serverSetting == null ? null : serverSetting.ToServerInfo();

			PermissionCenterToADSynchronizeSettings config = PermissionCenterToADSynchronizeSettings.GetConfig();

			return ADHelper.GetInstance(serverInfo);
		}
	}
}
