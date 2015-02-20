using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Caching;
using MCS.Library.Core;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 机构人员的对象缓存
	/// </summary>
	public abstract class OguObjectCacheBase : CacheQueue<string, List<IOguObject>>
	{
		/// <summary>
		/// 根据ID类型得到Cache实例
		/// </summary>
		/// <param name="idType"></param>
		/// <returns></returns>
		public static OguObjectCacheBase GetInstance(SearchOUIDType idType)
		{
			OguObjectCacheBase result = null;

			switch (idType)
			{
				case SearchOUIDType.Guid:
					result = OguObjectIDCache.Instance;
					break;
				case SearchOUIDType.FullPath:
					result = OguObjectFullPathCache.Instance;
					break;
				case SearchOUIDType.LogOnName:
					result = OguObjectLogOnNameCache.Instance;
					break;
			}

			return result;
		}

		/// <summary>
		/// 添加对象到Cache
		/// </summary>
		/// <param name="objs"></param>
		public void AddObjectsToCache<T>(IEnumerable<T> objs) where T : IOguObject
		{
			objs.ForEach(o => AddOneObjectToCache(o));
		}

		/// <summary>
		/// 收集在Cache已经存在的对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ids"></param>
		/// <param name="notInCacheIds"></param>
		/// <returns></returns>
		public IList<T> GetObjectsInCache<T>(string[] ids, out string[] notInCacheIds) where T : IOguObject
		{
			List<string> notInCacheList = new List<string>();

			Dictionary<string, string> idDict = new Dictionary<string, string>();

			foreach (string id in ids)
				if (idDict.ContainsKey(id) == false)
					idDict.Add(id, id);

			List<T> result = new List<T>();

			foreach (KeyValuePair<string, string> kp in idDict)
				FillObjectInCacheToList(kp.Key, result, notInCacheList);

			notInCacheIds = notInCacheList.ToArray();

			return result;
		}

		/// <summary>
		/// 添加单个对象到Cache
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		protected abstract void AddOneObjectToCache<T>(T obj) where T : IOguObject;

		/// <summary>
		/// 得到Cache项的Dependency
		/// </summary>
		/// <returns></returns>
		protected static DependencyBase CreateDependency()
		{
			return new MixedDependency(new UdpNotifierCacheDependency(), new MemoryMappedFileNotifierCacheDependency());
		}

		private void FillObjectInCacheToList<T>(string id, IList<T> list, IList<string> notInCacheList) where T : IOguObject
		{
			List<IOguObject> objsInCache = null;

			if (this.TryGetValue(id, out objsInCache))
			{
				objsInCache.ForEach(o =>
				{
					if (o is T)
						list.Add((T)o);
				});
			}
			else
				notInCacheList.Add(id);
		}
	}

	internal class OguObjectIDCache : OguObjectCacheBase
	{
		public static readonly OguObjectCacheBase Instance = CacheManager.GetInstance<OguObjectIDCache>();

		protected override void AddOneObjectToCache<T>(T obj)
		{
			List<IOguObject> existsList = null;

			if (this.TryGetValue(obj.ID, out existsList) == false)
			{
				existsList = new List<IOguObject>();
				Add(obj.ID, existsList, CreateDependency());
			}

			lock (existsList)
			{
				existsList.Add(obj);
			}
		}
	}

	internal class OguObjectFullPathCache : OguObjectCacheBase
	{
		public static readonly OguObjectCacheBase Instance = CacheManager.GetInstance<OguObjectFullPathCache>();

		protected override void AddOneObjectToCache<T>(T obj)
		{
			List<IOguObject> existsList = null;

			if (this.TryGetValue(obj.FullPath, out existsList) == false)
			{
				existsList = new List<IOguObject>();
				Add(obj.FullPath, existsList, CreateDependency());
			}

			lock (existsList)
			{
				existsList.Add(obj);
			}
		}
	}

	internal class OguObjectLogOnNameCache : OguObjectCacheBase
	{
		public static readonly OguObjectCacheBase Instance = CacheManager.GetInstance<OguObjectLogOnNameCache>();

		protected override void AddOneObjectToCache<T>(T obj)
		{
			ExceptionHelper.FalseThrow(obj is IUser, "对象{0}必须是用户对象才能添加到OguObjectLogOnNameCache中", obj.FullPath);

			List<IOguObject> existsList = null;
			string logonName = ((IUser)obj).LogOnName;

			if (this.TryGetValue(logonName, out existsList) == false)
			{
				existsList = new List<IOguObject>();
				Add(logonName, existsList, CreateDependency());
			}

			lock (existsList)
			{
				existsList.Add(obj);
			}
		}
	}

}
