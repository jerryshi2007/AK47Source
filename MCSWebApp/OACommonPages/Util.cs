using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Principal;
using MCS.Web.Library.Script;

namespace MCS.OA.CommonPages
{
	internal static class Util
	{
		internal static MCS.Library.SOA.DataObjects.UserCustomSearchConditionCollection LoadSearchCondition(string resourceKey, string conditionType)
		{
			return UserCustomSearchConditionAdapter.Instance.Load(c =>
			{
				c.AppendItem("RESOURCE_ID", resourceKey);
				c.AppendItem("CONDITION_TYPE", conditionType);
				c.AppendItem("USER_ID", DeluxeIdentity.CurrentUser.ID);
			});
		}

		internal static UserCustomSearchCondition NewSearchCondition(string resourceKey, string conditionType, string conditionName)
		{
			UserCustomSearchCondition condition = new UserCustomSearchCondition()
			{
				ID = Guid.NewGuid().ToString(),
				UserID = DeluxeIdentity.CurrentUser.ID,
				ResourceID = resourceKey,
				ConditionName = conditionName,
				ConditiontType = conditionType,
				CreateTime = DateTime.Now
			};

			return condition;
		}

		internal static void SaveSearchCondition(MCS.Web.WebControls.SearchEventArgs e, MCS.Web.WebControls.DeluxeSearch control, string pageKey, object dataToSave)
		{
			if (e.IsSaveCondition && string.IsNullOrEmpty(e.ConditionName) == false)
			{
				UserCustomSearchCondition condition = Util.NewSearchCondition(pageKey, "Default", e.ConditionName);

				condition.ConditionContent = JSONSerializerExecute.Serialize(dataToSave);

				UserCustomSearchConditionAdapter.Instance.Update(condition);

				control.UserCustomSearchConditions = Util.LoadSearchCondition(pageKey, "Default");
			}
		}
	}
}