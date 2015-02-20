using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.WF.Contracts.Workflow.DataObjects;

namespace MCS.Library.WF.Contracts.Converters.DataObjects
{
	public class WfClientRolePropertyValueConverter
	{
		public static readonly WfClientRolePropertyValueConverter Instance = new WfClientRolePropertyValueConverter();

		private WfClientRolePropertyValueConverter()
		{
		}

		public SOARolePropertyValue ClientToServer(WfClientRolePropertyValue client, SOARolePropertyDefinitionCollection serverColumns, ref SOARolePropertyValue server)
		{
			client.NullCheck("client");
			serverColumns.NullCheck("serverColumns");

			if (server == null)
			{
				SOARolePropertyDefinition serverColumn = serverColumns[client.Column.Name];

				serverColumn.NullCheck("serverColumn");

				server = new SOARolePropertyValue(serverColumn);
			}

			server.Value = client.Value;

			return server;
		}

		public WfClientRolePropertyValue ServerToClient(SOARolePropertyValue server, WfClientRolePropertyDefinitionCollection clientColumns, ref WfClientRolePropertyValue client)
		{
			server.NullCheck("server");
			clientColumns.NullCheck("clientColumns");

			if (client == null)
			{
				WfClientRolePropertyDefinition clientColumn = clientColumns[server.Column.Name];

				clientColumn.NullCheck("clientColumn");

				client = new WfClientRolePropertyValue(clientColumn);
			}

			client.Value = server.Value;

			return client;
		}
	}
}
