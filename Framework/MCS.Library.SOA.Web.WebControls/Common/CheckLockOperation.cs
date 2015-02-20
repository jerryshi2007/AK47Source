using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.OGUPermission;
using MCS.Web.WebControls.Properties;
using MCS.Library.SOA.DataObjects;

namespace MCS.Web.WebControls
{
    public class CheckLockOperation
    {
        /// <summary>
        /// 根据checkLock的结果显示不同的信息
        /// </summary>
        /// <param name="CheckLockResult">操作返回的对象</param>
        /// <returns></returns>
        public static string GetCheckLockStatusText(CheckLockResult chkResult)
        {
            string result;

            string displayName = string.Empty;

            if (chkResult.CurrentLock != null)
                displayName = GetDisplayName(chkResult.CurrentLock.PersonID);

            switch (chkResult.CurrentLockStatus)
            {
                case LockStatus.LockedByAnother:
                    result = string.Format(Resources.LockedByAnother, displayName);
                    break;
                case LockStatus.NotLocked:
                    result = Resources.NotLocked;
                    break;
                case LockStatus.LockByAnotherAndExpire:
                    result = string.Format(Resources.LockByAnotherAndExpire, displayName);
                    break;
                case LockStatus.LockedByRight:
                    result = Resources.LockedByRight;
                    break;
                case LockStatus.LockedByRightAndExpire:
                    result = Resources.LockedByRightAndExpire;
                    break;
                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 根据TrySetLock的结果显示不同的信息
        /// </summary>
        /// <param name="LockOperationResult">操作返回的对象</param>
        /// <returns></returns>
        public static string GetTrySetLockOperationResultDetail(SetLockResult lockResult)
        {
            string result;

            string displayName = GetDisplayName(lockResult.PersonID);

            switch (lockResult.OriginalLockStatus)
            {
                case LockStatus.LockedByAnother:
                    result = string.Format(Resources.TryLockedByAnother, displayName);
                    break;
                case LockStatus.NotLocked:
                    result = Resources.TryNotLocked;
                    break;
                case LockStatus.LockByAnotherAndExpire:
                    result = string.Format(Resources.TryLockByAnotherAndExpire, displayName);
                    break;
                case LockStatus.LockedByRight:
                    result = Resources.TryLockedByRight;
                    break;
                case LockStatus.LockedByRightAndExpire:
                    result = Resources.TryLockedByRightAndExpire;
                    break;
                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 获得占用锁的用户名
        /// </summary>
        /// <param name="lockOperationResult"></param>
        /// <returns></returns>
        private static string GetDisplayName(string userID)
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
}
