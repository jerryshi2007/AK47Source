using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Services;
using MCS.Library.Accredit;
using MCS.Library.OGUPermission;

namespace MCS.Services.AD2Accredit
{
	/// <summary>
	/// 同步AD的组织机构到安全中心
	/// </summary>
	public class ADToAccreditService : ThreadTaskBase
	{
		public override void OnThreadTaskStart()
		{
			using (AD2DBInitialParams initParams = AD2DBInitialParams.GetParams())
			{
				initParams.Log = this.Params.Log;

				AD2DBConvertion converter = new AD2DBConvertion(initParams);

				converter.DoConvert();

				OguMechanismFactory.GetMechanism().RemoveAllCache();
				PermissionMechanismFactory.GetMechanism().RemoveAllCache();
			}
		}
	}
}
