using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace MCS.Web.WebControls
{
	public interface ICutomerServiceExecutiveQuery
	{
		IEnumerable<IUser> QueryCutomerServiceExecutive(params string[] categories);
	}

	public class DefaultCutomerServiceExecutiveQuery : ICutomerServiceExecutiveQuery
	{
		public IEnumerable<IUser> QueryCutomerServiceExecutive(params string[] categories)
		{
			var result = new List<IUser>();

			if (categories.Any())
			{
				var executives = CutomerServiceExecutiveSetting.GetConfig()
					.CutomerServiceExecutives.Cast<CutomerServiceExecutiveElement>()
					.Where(q => categories.Contains(q.Category));

				if (executives.Any())
				{
					var logOnNames = executives.Select(r => r.LogOnName).ToArray();

					result =
						OguMechanismFactory.GetMechanism()
							.GetObjects<IUser>(SearchOUIDType.LogOnName, logOnNames)
							.Distinct((x, y) => string.Compare(x.ID, y.ID, true) == 0)
							.ToList();
				}
			}

			return result;
		}
	}
}
