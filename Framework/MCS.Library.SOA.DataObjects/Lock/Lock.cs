using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 锁类型
	/// </summary>
	public enum LockType
	{
		/// <summary>
		/// 管理员锁
		/// </summary>
		[EnumItemDescription("管理员锁", 1)]
		AdminLock = 1,

		/// <summary>
		/// 表单锁
		/// </summary>
		[EnumItemDescription("表单锁", 2)]
		FormLock = 2,

		/// <summary>
		/// 流转锁
		/// </summary>
		[EnumItemDescription("流转锁", 3)]
		ActivityLock = 3
	}

	/// <summary>
	/// 锁状态
	/// </summary>
	public enum LockStatus
	{
		/// <summary>
		/// 资源上有其他人加的锁，并没过期
		/// </summary>
		LockedByAnother = 0,

		/// <summary>
		/// 资源上没有锁
		/// </summary>
		NotLocked,

		/// <summary>
		/// 资源上有其他人加的锁，并已过期
		/// </summary>
		LockByAnotherAndExpire,

		/// <summary>
		///资源上有自己加的锁，并没过期
		/// </summary>
		LockedByRight,

		/// <summary>
		/// 资源上有自己加的锁，并已过期
		/// </summary>
		//该状态只用于锁控件提醒过期的判断
		LockedByRightAndExpire
	}

	/// <summary>
	/// 锁实体
	/// </summary>
	[Serializable]
	public class Lock
	{
		#region 私有变量

		private string lockID = string.Empty;
		private string resourceID = string.Empty;
		private string personID = string.Empty;
		private DateTime lockTime;
		private TimeSpan effectiveTime = LockConfigSetting.GetConfig().DefaultEffectiveTime;
		private LockType lockType = LockType.FormLock;

		#endregion

		#region 构造函数
		public Lock()
		{
			LockTime = DateTime.Now;
		}

		public Lock(string lockID, string personID)
			: this(lockID, lockID, personID)
		{
		}

		public Lock(string lockID, string resID, string personID)
			: this(lockID, resID, personID, LockConfigSetting.GetConfig().DefaultEffectiveTime, LockType.FormLock)
		{
		}

		public Lock(string lockID, string resID, string personID, TimeSpan effectiveTime, LockType lockType)
		{
			this.lockID = lockID;
			this.resourceID = resID;
			this.personID = personID;
			this.lockTime = DateTime.Now;
			this.effectiveTime = effectiveTime;
			this.lockType = lockType;
		}

		[Obsolete]
		public Lock(string lockID, string personID, DateTime lockTime, TimeSpan effectiveTime, LockType lockType)
		{
			LockID = lockID;
			PersonID = personID;
			LockTime = lockTime;
			EffectiveTime = effectiveTime;
			LockType = lockType;
		}
		#endregion

		#region 属性
		/// <summary>
		/// 锁ID
		/// </summary>
		[ORFieldMapping("LOCK_ID")]
		public string LockID
		{
			get
			{
				return this.lockID;
			}
			set
			{
				ExceptionHelper.TrueThrow<ArgumentNullException>(value == null, "value");
				this.lockID = value;
			}
		}

		/// <summary>
		/// 锁所对应的资源ID
		/// </summary>
		[ORFieldMapping("RESOURCE_ID")]
		public string ResourceID
		{
			get
			{
				return this.resourceID;
			}
			set
			{
				this.resourceID = value;
			}
		}

		/// <summary>
		/// 加锁人ID
		/// </summary>
		[ORFieldMapping("LOCK_PERSON_ID")]
		public string PersonID
		{
			get
			{
				return this.personID;
			}
			set
			{
				ExceptionHelper.TrueThrow<ArgumentNullException>(value == null, "value");
				this.personID = value;
			}
		}

		/// <summary>
		/// 锁创建时间
		/// </summary>
		[ORFieldMapping("LOCK_TIME")]
		public DateTime LockTime
		{
			get
			{
				return this.lockTime;
			}
			set
			{
				this.lockTime = value;
			}
		}

		/// <summary>
		/// 锁有效时间
		/// </summary>
		[ORFieldMapping("EFFECTIVE_TIME")]
		public TimeSpan EffectiveTime
		{
			get
			{
				return this.effectiveTime;
			}
			set
			{
				this.effectiveTime = value;
			}
		}

		/// <summary>
		/// 锁类型
		/// </summary>
		[ORFieldMapping("LOCK_TYPE")]
		public LockType LockType
		{
			get
			{
				return this.lockType;
			}
			set
			{
				this.lockType = value;
			}
		}
		#endregion
	}

	[Serializable]
	public abstract class LockResultBase
	{
		private string personID = string.Empty;

		/// <summary>
		/// 用户ID
		/// </summary>
		public string PersonID
		{
			get
			{
				return this.personID;
			}
			protected set
			{
				this.personID = value;
			}
		}

		protected LockResultBase(string userID)
		{
			this.personID = userID;
		}

		protected static string GetStatusPromptText(LockStatus status, string lockUserName)
		{
			string result = string.Empty;

			switch (status)
			{
				case LockStatus.LockedByAnother:
					result = string.Format("该资源已被{0}加锁", lockUserName);
					break;
				case LockStatus.NotLocked:
					result = "原资源锁已被破坏";
					break;
				case LockStatus.LockByAnotherAndExpire:
					result = string.Format("该资源已被{0}加锁，并已过期，验证失败", lockUserName);
					break;
				case LockStatus.LockedByRight:
					result = "锁未过期";
					break;
				case LockStatus.LockedByRightAndExpire:
					result = "锁已过期，请重新加锁";
					break;
			}

			return result;
		}

		/// <summary>
		/// 获得占用锁的用户名
		/// </summary>
		/// <param name="lockOperationResult"></param>
		/// <returns></returns>
		protected static string GetDisplayName(string userID)
		{
			IOrganizationMechanism operation = OguMechanismFactory.GetMechanism();

			string displayName = "其他用户";

			if (string.IsNullOrEmpty(userID) == false)
			{
				OguObjectCollection<IUser> user
					= operation.GetObjects<IUser>(SearchOUIDType.Guid, userID);

				if (user.Count != 0)
					displayName = user[0].DisplayName;
			}

			return displayName;
		}
	}

	public class CheckLockResult : LockResultBase
	{
		private Lock currentLock = null;
		private LockStatus currentLockStatus = LockStatus.NotLocked;

		internal CheckLockResult(string personID, DataTable table)
			: base(personID)
		{
			if (table.Rows.Count > 0)
			{
				this.currentLock = new Lock();

				ORMapping.DataRowToObject(table.Rows[0], this.currentLock);

				this.currentLockStatus = CalculateLockStatus(personID, (DateTime)table.Rows[0]["CURRENT_TIME"], this.currentLock);
			}
		}

		/// <summary>
		/// 当前锁对象
		/// </summary>
		public Lock CurrentLock
		{
			get
			{
				return this.currentLock;
			}
		}

		/// <summary>
		/// 当前锁状态
		/// </summary>
		public LockStatus CurrentLockStatus
		{
			get
			{
				return this.currentLockStatus;
			}
		}

		public string GetCheckLockStatusText()
		{
			string displayName = string.Empty;

			if (this.CurrentLock != null)
				displayName = GetDisplayName(this.CurrentLock.PersonID);

			return GetStatusPromptText(this.CurrentLockStatus, displayName);
		}

		private LockStatus CalculateLockStatus(string personID, DateTime currentTime, Lock currentLock)
		{
			LockStatus status = LockStatus.NotLocked;

			if (currentLock != null)
			{
				if (string.Compare(personID, currentLock.PersonID, true) == 0)
				{
					if (currentLock.LockTime.Add(currentLock.EffectiveTime) < currentTime)
						status = LockStatus.LockedByRightAndExpire;
					else
						status = LockStatus.LockedByRight;
				}
				else
				{
					if (currentLock.LockTime.Add(currentLock.EffectiveTime) < currentTime)
						status = LockStatus.LockByAnotherAndExpire;
					else
						status = LockStatus.LockedByAnother;
				}
			}

			return status;
		}
	}

	[Serializable]
	public class SetLockResult : LockResultBase
	{
		private Lock newLock = null;
		private Lock originalLock = null;
		private LockStatus originalLockStatus = LockStatus.NotLocked;

		internal SetLockResult(string personID, DataTable table)
			: base(personID)
		{
			ExceptionHelper.FalseThrow(table.Rows.Count == 2,
				"加锁后返回的结果集的行数必须为2，现在是{1}", table.Rows.Count);

			if (table.Rows[0]["LOCK_ID"] != DBNull.Value)
			{
				this.newLock = new Lock();

				ORMapping.DataRowToObject(table.Rows[0], this.newLock);
			}

			if (table.Rows[1]["LOCK_ID"] != DBNull.Value)
			{
				this.originalLock = new Lock();

				ORMapping.DataRowToObject(table.Rows[1], this.originalLock);
			}

			this.originalLockStatus = CalculateLockStatus(this.newLock, this.originalLock);
		}

		/// <summary>
		/// 是否成功
		/// </summary>
		public bool Succeed
		{
			get
			{
				return this.newLock != null;
			}
		}

		/// <summary>
		/// 原始锁
		/// </summary>
		public Lock OriginalLock
		{
			get
			{
				return this.originalLock;
			}
		}

		/// <summary>
		/// 新锁
		/// </summary>
		public Lock NewLock
		{
			get
			{
				return this.newLock;
			}
		}

		/// <summary>
		/// 原始锁的状态
		/// </summary>
		public LockStatus OriginalLockStatus
		{
			get
			{
				return this.originalLockStatus;
			}
		}

		/// <summary>
		/// 根据TrySetLock的结果显示不同的信息
		/// </summary>
		/// <param name="LockOperationResult">操作返回的对象</param>
		/// <returns></returns>
		public string GetTrySetLockOperationResultDetail()
		{
			string displayName = GetDisplayName(this.PersonID);

			return GetStatusPromptText(this.OriginalLockStatus, displayName);
		}

		private LockStatus CalculateLockStatus(Lock nLock, Lock oLock)
		{
			LockStatus result = LockStatus.NotLocked;

			if (oLock == null)
				result = LockStatus.NotLocked;
			else
			{
				if (nLock == null)
					result = LockStatus.LockedByAnother;
				else
				{
					if (string.Compare(nLock.PersonID, oLock.PersonID, true) == 0)
					{
						if (oLock.LockTime.Add(oLock.EffectiveTime) < nLock.LockTime)
							result = LockStatus.LockedByRightAndExpire;
						else
							result = LockStatus.LockedByRight;
					}
					else
					{
						if (oLock.LockTime.Add(oLock.EffectiveTime) < nLock.LockTime)
							result = LockStatus.LockByAnotherAndExpire;
					}
				}
			}

			return result;
		}
	}

}
