using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects.Security.Locks
{
	/// <summary>
	/// 操作锁对象
	/// </summary>
	[Serializable]
	[ORTableMapping("SC.Locks")]
	public class SCLock
	{
		public const string DefaultDataOperationLockID = "29208710-7d24-9c32-42e0-0b05972fd96c";
		//public const string SynchronizationLockID = "0b3c3556-cef0-41a4-b84c-7f4ea2c4cffb";

		public SCLock()
		{
			this.EffectiveTime = SCLockSettings.GetConfig().DefaultEffectiveTime;
		}

		/// <summary>
		/// 锁ID
		/// </summary>
		[ORFieldMapping("LockID", PrimaryKey = true)]
		public string LockID { get; set; }

		/// <summary>
		/// 锁对应的资源ID
		/// </summary>
		[ORFieldMapping("ResourceID")]
		public string ResourceID { get; set; }

		private IUser _LockPerson = null;

		/// <summary>
		/// 上锁人
		/// </summary>
		[SubClassType(typeof(OguUser))]
		[SubClassORFieldMapping("ID", "LockPersonID")]
		[SubClassORFieldMapping("DisplayName", "LockPersonName")]
		public IUser LockPerson
		{
			get
			{
				return this._LockPerson;
			}
			set
			{
				this._LockPerson = (IUser)OguUser.CreateWrapperObject(value);
			}
		}

		/// <summary>
		/// 上锁时间
		/// </summary>
		[ORFieldMapping("LockTime")]
		[SqlBehavior(DefaultExpression = "GETDATE()")]
		public DateTime LockTime { get; set; }

		/// <summary>
		/// 锁有效时间
		/// </summary>
		[ORFieldMapping("EffectiveTime")]
		public TimeSpan EffectiveTime { get; set; }

		/// <summary>
		/// 锁的类型
		/// </summary>
		[ORFieldMapping("LockType")]
		public SCLockType LockType { get; set; }

		/// <summary>
		/// 锁的描述信息
		/// </summary>
		[ORFieldMapping("Description")]
		public string Description { get; set; }

		public static SCLock CreateDefaultDataOperationLock()
		{
			SCLock result = new SCLock();

			result.LockID = DefaultDataOperationLockID;
			result.LockType = SCLockType.DataOperation;

			if (DeluxePrincipal.IsAuthenticated)
				result.LockPerson = DeluxeIdentity.CurrentUser;

			return result;
		}
	}
}
