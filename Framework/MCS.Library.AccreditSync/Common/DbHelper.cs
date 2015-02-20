using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;

namespace MCS.Library.Accredit
{
	internal static class DbHelper
	{
		public const string AccreditAdminConnName = "AccreditAdmin";

		public static void ExecSql(Action<Database> action)
		{
			using (DbContext context = DbContext.GetContext(AccreditAdminConnName))
			{
				Database db = DatabaseFactory.Create(AccreditAdminConnName);

				action(db);
			}
		}
	}
}
