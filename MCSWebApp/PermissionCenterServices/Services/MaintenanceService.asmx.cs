using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Conditions;

namespace PermissionCenter.Services
{
	/// <summary>
	/// 用于后台运行的服务
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class MaintenanceService : System.Web.Services.WebService
	{
		/// <summary>
		/// 重新计算所有角色和群组中的成员
		/// </summary>
		[WebMethod]
		public void RecalculateAllConditions()
		{
			SCConditionCalculator calculator = new SCConditionCalculator();
			SCCacheHelper.InvalidateAllCache();
			calculator.GenerateAllUserAndContainerSnapshot();
		}
	}
}
