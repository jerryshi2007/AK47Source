using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.OGUPermission;
using MCS.Library.Core;

namespace WorkflowDesigner.ModalDialog
{
    static class QueryHelper
    {
        public static IUser QueryUser(string logonName)
        {
            OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, logonName);

            ExceptionHelper.FalseThrow(users.Count != 0, "不能根据登录名'{0}'找到用户", logonName);

            return users[0];
        }
    }
}