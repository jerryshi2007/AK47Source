using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.OGUPermission.Commands
{
	internal static class QueryHelper
	{
		public static OguObjectCollection<IUser> QueryUser(string logonName)
		{
			string searchTerm = GetFullTextSeachTerm(logonName);
			OguObjectCollection<IUser> users = null;

			if (searchTerm == logonName)
			{
				users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, logonName);
			}
			else
			{
				IOrganization root = OguMechanismFactory.GetMechanism().GetRoot();
				users = root.QueryChildren<IUser>(searchTerm, true, SearchLevel.SubTree, 15);
			}

			ExceptionHelper.FalseThrow(users.Count != 0, "不能根据'{0}'找到用户", logonName);

			return users;
		}

		public static OguObjectCollection<IOguObject> QueryObjectByID(string objectID)
		{
			OguObjectCollection<IOguObject> objs = OguMechanismFactory.GetMechanism().GetObjects<IOguObject>(SearchOUIDType.Guid, objectID);

			ExceptionHelper.FalseThrow(objs.Count != 0, "不能根据ID'{0}'找到对象", objectID);

			return objs;
		}

		public static OguObjectCollection<IOguObject> QueryObjectByFullPath(string fullPath)
		{
			OguObjectCollection<IOguObject> objs = OguMechanismFactory.GetMechanism().GetObjects<IOguObject>(SearchOUIDType.FullPath, fullPath);

			ExceptionHelper.FalseThrow(objs.Count != 0, "不能根据FullPath'{0}'找到对象", fullPath);

			return objs;
		}

		private static string GetFullTextSeachTerm(string searchTerm)
		{
			return searchTerm.Trim('*');
		}
	}
}
