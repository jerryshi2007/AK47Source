using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using MCS.Library;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Locks;
using MCS.Library.SOA.DataObjects.Security.Transfer;

namespace MCS.Library.SOA.Security.ADSyncUtilities.Entity
{
	public class SynchronizeContext
	{
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
		private DateTime _LastExtendLockTime = DateTime.MaxValue;

		private ModifiedItemgCollection _ModifiedOuAndUserItems = null;	//因为执行删除要放到执行修改之前，所以把删除项和修改项分开。
		private ModifiedItemgCollection _ModifiedGroupItems = null;
		private ModifiedItemgCollection _DeletedOuAndUserItems = null;
		private ModifiedItemgCollection _DeletedGroupItems = null;

		private DelayActionCollection _DelayActions = null; //延后 操作
		private KeyedOguObjectCollection _GroupsToTakeCare = null;
		private Dictionary<string, IUser> _UsersAsInMainJobs = null;
		private SCLock _SynchronizeLock = null;

		Dictionary<string, string> includeMappings = new Dictionary<string, string>(); // 保存映射

		private List<string> includePathsCache = new List<string>();//映射路径的缓存


		public Dictionary<string, string>.ValueCollection AdTargets
		{
			get
			{
				return includeMappings.Values;
			}
		}

		/// <summary>
		/// 被挑选作为主职的兼职用户字典
		/// </summary>
		public Dictionary<string, IUser> SidelineUsers
		{
			get
			{
				if (_UsersAsInMainJobs == null)
					_UsersAsInMainJobs = new Dictionary<string, IUser>();

				return _UsersAsInMainJobs;
			}
		}

		private IDMapper _IDMapper = null;
		private string targetOUPath = null;
		private string sourceRootPath = null;

		private int _AddingItemCount = 0;
		private int _AddedItemCount = 0;

		private int _ModifyingItemCount = 0;
		private int _ModifiedItemCount = 0;

		private int _DeletingItemCount = 0;
		private int _DeletedItemCount = 0;

		#region 属性
		public static SynchronizeContext Current
		{
			get
			{
				return (SynchronizeContext)ObjectContextCache.Instance.GetOrAddNewValue("SynchronizeContext", (cache, key) =>
				{
					SynchronizeContext context = new SynchronizeContext() { SynchronizeID = Guid.NewGuid().ToString() };
					cache.Add(key, context);

					return context;
				});
			}
		}

		public ADSynchronizeLog LogEntity { get; set; }
		public string SynchronizeID { get; set; }
		public string StartPath { get; set; }
		public string RecycleBinOU { get; set; }

		public string TargetRootOU
		{
			get
			{
				return targetOUPath;
			}

			set
			{
				this.targetOUPath = value;
			}
		}


		public string SourceRootPath
		{
			get { return this.sourceRootPath; }
			internal set { this.sourceRootPath = value; }
		}

		public KeyedOguObjectCollection GroupsToTakeCare
		{
			get
			{
				if (this._GroupsToTakeCare == null)
				{
					this._GroupsToTakeCare = new KeyedOguObjectCollection();
				}

				return this._GroupsToTakeCare;
			}
		}

		public DelayActionCollection DelayActions
		{
			get
			{
				if (_DelayActions == null)
					this._DelayActions = new DelayActionCollection();

				return _DelayActions;
			}
		}

		public string DefaultPassword { get; set; }
		public ADHelper ADHelper { get; set; }

		public int AddingItemCount
		{
			get
			{
				return this._AddingItemCount;
			}
		}

		public int AddedItemCount
		{
			get
			{
				return this._AddedItemCount;
			}
		}

		public int ModifyingItemCount
		{
			get
			{
				return this._ModifyingItemCount;
			}
		}

		public int ModifiedItemCount
		{
			get
			{
				return this._ModifiedItemCount;
			}
		}


		public int DeletingItemCount
		{
			get
			{
				return this._DeletingItemCount;
			}
		}

		public int DeletedItemCount
		{
			get
			{
				return this._DeletedItemCount;
			}
		}

		public int ExceptionCount { get; set; }
		public IOguObject CurrentOguObject { get; set; }

		public ADSynchronizeResult SynchronizeResult { get; set; }

		public IDMapper IDMapper
		{
			get
			{
				if (_IDMapper == null)
				{
					_IDMapper = new IDMapper();
				}

				return _IDMapper;
			}
		}

		public ModifiedItemgCollection ModifiedOuAndUserItems
		{
			get
			{
				if (this._ModifiedOuAndUserItems == null)
				{
					this._ModifiedOuAndUserItems = new ModifiedItemgCollection();
				}
				return this._ModifiedOuAndUserItems;
			}
		}

		public ModifiedItemgCollection ModifiedGroupItems
		{
			get
			{
				if (this._ModifiedGroupItems == null)
				{
					this._ModifiedGroupItems = new ModifiedItemgCollection();
				}

				return this._ModifiedGroupItems;
			}
		}

		public ModifiedItemgCollection DeletedOuAndUserItems
		{
			get
			{
				if (this._DeletedOuAndUserItems == null)
				{
					this._DeletedOuAndUserItems = new ModifiedItemgCollection();
				}

				return _DeletedOuAndUserItems;
			}
		}

		public ModifiedItemgCollection DeletedGroupItems
		{
			get
			{
				if (this._DeletedGroupItems == null)
				{
					this._DeletedGroupItems = new ModifiedItemgCollection();
				}

				return _DeletedGroupItems;
			}
		}
		#endregion

		private SynchronizeContext()
		{
		}

		public static void Clear()
		{
			ObjectContextCache.Instance.Remove("SynchronizeContext");
		}

		/// <summary>
		/// 判断是不是需要同步的对象
		/// </summary>
		/// <param name="oguObject"></param>
		/// <returns></returns>
		public bool IsRealObject(IOguObject oguObject)
		{
			bool result = false;

			string path = oguObject.FullPath;
			int len = path.Length;
			int sLen = this.SourceRootPath.Length;

			if (path.EndsWith("\\"))
				throw new FormatException("路径不得以\\结束");
			else if (len > sLen && path.StartsWith(this.SourceRootPath, StringComparison.Ordinal) && path[sLen] == '\\')
			{
				// 如果路径至少有一段
				int ind = path.IndexOf("\\", sLen + 1);

				if (ind > 0)
				{
					string name = path.Substring(sLen + 1, ind - sLen - 1);

					if (this.includeMappings.ContainsKey(name))
						result = true;
				}
			}

			if (result)
			{
				if (oguObject.ObjectType == SchemaType.Users)
				{
					IUser user = (IUser)oguObject;

					if (user.IsSideline)
					{
						if (this.SidelineUsers.ContainsKey(oguObject.ID) == false)
						{
							//所有兼职信息中，是否在同步范围中没有主职
							result = user.AllRelativeUserInfo.NotExists(u => u.IsSideline == false && u.FullPath.StartsWith(this.StartPath));

							if (result)
								this.SidelineUsers.Add(oguObject.ID, (IUser)oguObject);	//将兼职视为主职
						}
						else
						{
							result = this.SidelineUsers[user.ID].FullPath == user.FullPath;
						}
					}
				}
			}

			return result;
		}

		public void IncreaseAddItemCount(Action action)
		{
			IncreaseItemCount(action, ref this._AddingItemCount, ref this._AddedItemCount);
		}

		public void IncreaseModifyItemCount(Action action)
		{
			IncreaseItemCount(action, ref this._ModifyingItemCount, ref this._ModifiedItemCount);
		}

		public void IncreaseDeleteItemCount(Action action)
		{
			IncreaseItemCount(action, ref this._DeletingItemCount, ref this._DeletedItemCount);
		}

		private static void IncreaseItemCount(Action action, ref int doingCounter, ref int doneCounter)
		{
			if (action != null)
			{
				doingCounter++;
				action();
				doneCounter++;
			}
		}

		public void PrepareAndAddDeletedItem(IOguObject oguObject, ADObjectWrapper adObject, ObjectModifyType objectModifyType)
		{
			ModifiedItem item = new ModifiedItem() { OguObjectData = oguObject, ADObjectData = adObject, ModifyType = objectModifyType };

			if (adObject.ObjectType == ADSchemaType.Groups)
			{
				this.DeletedGroupItems.Add(item);
			}
			else
			{
				this.DeletedOuAndUserItems.Add(item);
			}
		}

		public void PrepareAndAddModifiedItem(IOguObject oguObject, ADObjectWrapper adObject, ObjectModifyType objectModifyType)
		{
			ModifiedItem item = new ModifiedItem() { OguObjectData = oguObject, ADObjectData = adObject, ModifyType = objectModifyType };

			if (oguObject.ObjectType == SchemaType.Groups)
			{
				this.ModifiedGroupItems.Add(item);
			}
			else
			{
				this.ModifiedOuAndUserItems.Add(item);
			}
		}

		public void PrepareGroupToTakeCare(IOguObject oguObject)
		{
			this.GroupsToTakeCare.AddNotExistsItem(oguObject);
		}

		public void NormalizeModifiedItems()
		{
			NormalizeUsers();
			RemoveModifiedItemsInDeletedItems(this.DeletedOuAndUserItems, this.ModifiedOuAndUserItems);
			RemoveModifiedItemsInDeletedItems(this.DeletedGroupItems, this.ModifiedGroupItems);
		}

		[Conditional("PC_TRACE")]
		private void NormalizeUsers()
		{
			for (int i = this.DeletedOuAndUserItems.Count - 1; i >= 0; i--)
			{
				SynchronizeContext.Current.ExtendLockTime();

				var u = this.DeletedOuAndUserItems[i];
				{
					var mapping = this.IDMapper.ADIDMappingDictionary[u.ADObjectData.NativeGuid];
					if (mapping != null)
					{
						Debug.Assert(SynchronizeContext.Current.SidelineUsers.ContainsKey(mapping.SCObjectID) == false, "不应该被删除");
					}
				}
			}
		}

		public void WriteExceptionDBLogIfError(string actionName, Action action)
		{
			WriteExceptionDBLogIfError(actionName, string.Empty, string.Empty, action);
		}

		public void WriteExceptionDBLogIfError(string actionName, string adObjectID, string adObjectName, Action action)
		{
			if (action != null)
			{
				try
				{
					action();
				}
				catch (System.Exception ex)
				{
					string currentOguObjectID = string.Empty;
					string currentOguObjectName = string.Empty;

					if (this.CurrentOguObject != null)
					{
						currentOguObjectID = this.CurrentOguObject.ID;
						currentOguObjectName = this.CurrentOguObject.Name;
					}

					this.ExceptionCount++;
					LogHelper.WriteSynchronizeDBLogDetail(this.SynchronizeID, actionName,
						currentOguObjectID, currentOguObjectName, adObjectID, adObjectName, ex.GetRealException().ToString());

					throw;
				}
			}
		}

		#region Lock Operation
		public void AddLock()
		{
			SCLock syncLock = new SCLock() { LockID = SCLock.DefaultDataOperationLockID, LockType = SCLockType.DataOperation };

			syncLock.EffectiveTime = SynchronizationEffectiveTime;
			syncLock.Description = "权限中心同步到AD";

			SCCheckLockResult checkResult = SCLockAdapter.Instance.AddLock(syncLock);

			if (checkResult.Available == false)
				throw new SCCheckLockException(SCCheckLockException.CheckLockResultToMessage(checkResult));

			this._SynchronizeLock = syncLock;
			this._LastExtendLockTime = DateTime.Now;
		}

		public void DeleteLock()
		{
			if (this._SynchronizeLock != null)
			{
				SCLockAdapter.Instance.DeleteLock(this._SynchronizeLock);
				this._SynchronizeLock = null;
				this._LastExtendLockTime = DateTime.MinValue;
			}
		}

		public void ExtendLockTime()
		{
			if (this._SynchronizeLock != null)
			{
				if (DateTime.Now.Subtract(this._LastExtendLockTime) > ExtendLockInterval)
				{
					SCLockAdapter.Instance.ExtendLockTime(this._SynchronizeLock);
					this._LastExtendLockTime = DateTime.Now;
				}
			}
		}
		#endregion

		private static void RemoveModifiedItemsInDeletedItems(ModifiedItemgCollection deletedItems, ModifiedItemgCollection modifiedItems)
		{
			int i = 0;
			while (i < deletedItems.Count)
			{
				SynchronizeContext.Current.ExtendLockTime();

				if (modifiedItems.ExistedADObject(deletedItems[i].ADObjectData.NativeGuid))
					deletedItems.RemoveAt(i);
				else
					i++;
			}
		}

		internal void ClearIncludeMappings()
		{
			this.includeMappings.Clear();
			this.includePathsCache.Clear();
		}

		internal void AddIncludeMapping(string pcName, string adName)
		{
			this.includeMappings.Add(pcName, adName);
			this.includePathsCache.Add(this.SourceRootPath + "\\" + pcName);
		}

		internal string GetMappedName(string pcName)
		{
			return this.includeMappings[pcName];
		}

		/// <summary>
		/// 查找对象的名称，名称将包含前缀
		/// </summary>
		/// <param name="oguObject"></param>
		/// <param name="alternatePrifix"></param>
		/// <returns></returns>
		internal string GetMappedName(IOguObject oguObject, string alternatePrifix)
		{
			if (oguObject.FullPath == SynchronizeContext.Current.SourceRootPath)
			{
				Debugger.Break(); //什么时候会把同步根弄进来
				return SynchronizeContext.Current.TargetRootOU;
			}
			else
			{
				for (int i = 0; i < this.includePathsCache.Count; i++)
				{
					if (this.includePathsCache[i] == oguObject.FullPath)
					{
						return this.GetMappedName(oguObject.Name);
					}
				}
			}

			return alternatePrifix + "=" + ADHelper.EscapeString(oguObject.Name);
		}

		internal bool ContainsMapping(string p)
		{
			return includeMappings.ContainsKey(p);
		}

		internal bool IsIncluded(IOguObject obj)
		{
			if (obj.FullPath == this.StartPath)
				return true;
			else
			{
				bool result = false;
				foreach (string item in this.includePathsCache)
				{
					if (SynchronizeHelper.IsOrInPath(obj.FullPath, item))
					{
						result = true;
						break;
					}
				}

				return result;
			}
		}
	}
}
