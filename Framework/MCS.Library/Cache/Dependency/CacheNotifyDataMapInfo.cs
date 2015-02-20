using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Caching
{
	/// <summary>
	/// 
	/// </summary>
	public struct CacheNotifyDataMapInfo
	{
		/// <summary>
		/// 
		/// </summary>
		public const int CacheDataBlockSize = 1024;

		/// <summary>
		/// 
		/// </summary>
		public const int CacheDataItemCount = 100;

		/// <summary>
		/// 
		/// </summary>
		public const int TotalMapSize = CacheDataBlockSize * CacheDataItemCount;

		/// <summary>
		/// 内存标记
		/// </summary>
		public const long Tag = 197204262332;

		/// <summary>
		/// 
		/// </summary>
		public Int64 Mark;

		/// <summary>
		/// 
		/// </summary>
		public int Pointer;

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public CacheNotifyDataMapInfo Clone()
		{
			CacheNotifyDataMapInfo result = new CacheNotifyDataMapInfo();

			result.Mark = this.Mark;
			result.Pointer = this.Pointer;

			return result;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public struct CacheNotifyDataMapItem
	{
		/// <summary>
		/// 
		/// </summary>
		public long Ticks;

		/// <summary>
		/// 
		/// </summary>
		public long Size;
	}
}
