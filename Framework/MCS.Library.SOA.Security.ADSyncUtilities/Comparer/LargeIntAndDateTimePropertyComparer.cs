using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class LargeIntAndDateTimePropertyComparer : OguAndADObjectPropertyComparerBase
	{
		protected override bool ComparePropertyValue(IOguObject srcOguObject, string srcPropertyName, ADObjectWrapper adObject, string targetPropertyName, string context)
		{
			DateTime dtResult = DateTime.MinValue;
			long adAccountExpiresValue = Convert.ToInt64(adObject.Properties[targetPropertyName]);

			try
			{
				if (adAccountExpiresValue != SynchronizeHelper.ACCOUNT_EXPIRES_MAX_VALUE)
				{
					if (adAccountExpiresValue != 0)
					{
						DateTime dt = DateTime.FromFileTime(adAccountExpiresValue);

						//舍弃掉毫秒
						dtResult = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
					}
				}
			}
			catch (System.ArgumentOutOfRangeException)
			{
				dtResult = DateTime.MaxValue;
			}

			dtResult = SynchronizeContext.Current.ADHelper.GetUserAccountExpirationDate(dtResult);
			var accountExpiresDate = Convert.ToDateTime(srcOguObject.Properties[srcPropertyName]);

			return accountExpiresDate == dtResult;
		}
	}
}