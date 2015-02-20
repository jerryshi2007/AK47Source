using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.Accredit
{
    public class ADGroup2DBConvertion
    {
        public ADGroup2DBConvertion()
        {
        }

        public void DoConvert()
        {
            ServerInfo serverInfo = ServerInfoConfigSettings.GetConfig().ServerInfos["dc"].ToServerInfo();
            for (int i = 0; i < ADSyncGroupSettings.GetConfig().GroupConfigurations.Count; i++)
            {
                GroupConfigurationElement element = ADSyncGroupSettings.GetConfig().GroupConfigurations[i];
                AccreditHelper.SyncGroupMembers(element.SourceDN, element.DestinationPath, serverInfo);
            }
        }
    }
}
