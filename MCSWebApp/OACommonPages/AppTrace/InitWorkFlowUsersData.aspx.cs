using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;

namespace MCS.OA.CommonPages.AppTrace
{
	public partial class InitWorkFlowUsersData : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void InitWorkFlowUserData_Click(object sender, EventArgs e)
		{
			DbHelper.RunSqlWithTransaction(@"  TRUNCATE TABLE  WF.ACTIVE_USERS ;  
    INSERT INTO WF.ACTIVE_USERS (ACTIVE_USER_ID)
    SELECT DISTINCT USER_GUID
    FROM SinoOceanOgus.dbo.OU_USERS 
    AS OUU	INNER JOIN SinoOceanOgus.dbo.USERS AS U ON OUU.USER_GUID = U.GUID
    WHERE OUU.STATUS = 1 AND U.POSTURAL <>1 ;
EXEC [WF].[ImportInvalidAssignees]");
		}
	}
}