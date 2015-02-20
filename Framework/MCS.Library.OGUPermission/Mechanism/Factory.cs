#region
// -------------------------------------------------
// Assembly	£º	DeluxeWorks.Library.OGUPermission
// FileName	£º	OguMechanismFactory.cs
// Remark	£º	»ú¹¹ÈËÔ±¹¤³§Àà
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    Éòá¿	    20070430		´´½¨
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.OGUPermission
{
    /// <summary>
    /// »ú¹¹ÈËÔ±¹¤³§Àà
    /// </summary>
    public static class OguMechanismFactory
    {
        /// <summary>
        /// µÃµ½»ú¹¹ÈËÔ±µÄÊµÏÖÀà
        /// </summary>
        /// <returns>IOrganizationMechanismµÄÊµÏÖÀà</returns>
        public static IOrganizationMechanism GetMechanism()
        {
			OguPermissionSettings oguPermissionSettings = OguPermissionSettings.GetConfig();
			return oguPermissionSettings.OguFactory;
			//return OguPermissionSettings.GetConfig().OguFactory;
        }
    }

    /// <summary>
    /// ÊÚÈ¨¹¤³§Àà
    /// </summary>
    public static class PermissionMechanismFactory
    {
        /// <summary>
        /// µÃµ½ÊÚÈ¨ÊµÏÖÀà
        /// </summary>
        /// <returns>IPermissionMechanismµÄÊµÏÖÀà</returns>
        public static IPermissionMechanism GetMechanism()
        {
            return OguPermissionSettings.GetConfig().PermissionFactory;
        }
    }
}
