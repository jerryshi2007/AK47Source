using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 用户关于委托待办的实现
	/// </summary>
	public sealed class WfDelegationAdapter : UpdatableAndLoadableAdapterBase<WfDelegation, WfDelegationCollection>, IWfDelegationReader
	{
		/// <summary>
		/// 得到实例
		/// </summary>
		public static readonly WfDelegationAdapter Instance = new WfDelegationAdapter();

		private WfDelegationAdapter()
		{
           
		}

		public WfDelegationCollection GetUserActiveDelegations(IUser user, IWfProcess process)
		{
			user.NullCheck("user");

			return GetUserActiveDelegations(user.ID);
		}

		/// <summary>
		/// 得到委托人的所有委托信息
		/// </summary>
		/// <param name="sourceUserID"></param>
		/// <returns></returns>
		public WfDelegationCollection Load(string sourceUserID)
		{
			sourceUserID.CheckStringIsNullOrEmpty("sourceUserID");

			return Load(builder => builder.AppendItem("SOURCE_USER_ID", sourceUserID));
		}

		/// <summary>
		/// 得到委托人的所有在有效期内的委托信息
		/// </summary>
		/// <param name="sourceUserID"></param>
		/// <returns></returns>
		public WfDelegationCollection GetUserActiveDelegations(string sourceUserID)
		{
			sourceUserID.CheckStringIsNullOrEmpty("sourceUserID");

            WfDelegationCollection delegationsInCache = WfDelegationCache.Instance.GetOrAddNewValue(CalculateCacheKey(sourceUserID), (cache, key) =>
			{
				WfDelegationCollection delegations = Load(builder => builder.AppendItem("SOURCE_USER_ID", sourceUserID));

                MixedDependency dependency = new MixedDependency(new UdpNotifierCacheDependency(), new MemoryMappedFileNotifierCacheDependency());

				cache.Add(key, delegations, dependency);

				return delegations;
			});

			WfDelegationCollection result = new WfDelegationCollection();

			DateTime now = DateTime.Now;

			foreach (WfDelegation delegation in delegationsInCache)
			{
				if (now >= delegation.StartTime && now < delegation.EndTime)
					result.Add(delegation);
			}

			return result;
		}

		/// <summary>
		/// 重载对象更新后的代码，发送UDP通知，更新各服务器中的缓存
		/// </summary>
		/// <param name="data"></param>
		/// <param name="context"></param>
		protected override void AfterInnerUpdate(WfDelegation data, Dictionary<string, object> context)
		{
			base.AfterInnerUpdate(data, context);
			UpdateCache(data);
		}

		private static void UpdateCache(WfDelegation data)
		{
			CacheNotifyData notifyData = new CacheNotifyData(typeof(WfDelegationCache), data.SourceUserID, CacheNotifyType.Invalid);

			UdpCacheNotifier.Instance.SendNotifyAsync(notifyData);
            MmfCacheNotifier.Instance.SendNotify(notifyData);
		}

		protected override void AfterInnerDelete(WfDelegation data, Dictionary<string, object> context)
		{
			base.AfterInnerDelete(data, context);
			UpdateCache(data);
		}

		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}

        private static string CalculateCacheKey(string sourceUserID)
        {
            string result = sourceUserID;

            if (TenantContext.Current.Enabled)
                result = string.Format("{0}-{1}", TenantContext.Current.TenantCode, sourceUserID);

            return result;
        }
	}
}
