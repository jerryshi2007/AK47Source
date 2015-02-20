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
	public class WfClientRolePropertyDefinitionConverter
	{
		public static readonly WfClientRolePropertyDefinitionConverter Instance = new WfClientRolePropertyDefinitionConverter();

		private WfClientRolePropertyDefinitionConverter()
		{
		}

		public SOARolePropertyDefinition ClientToServer(WfClientRolePropertyDefinition client, ref SOARolePropertyDefinition server)
		{
			client.NullCheck("client");

			if (server == null)
				server = new SOARolePropertyDefinition();

			server.Name = client.Name;
			server.Caption = client.Caption;
			server.DataType = client.DataType;
			server.DefaultValue = client.DefaultValue;
			server.SortOrder = client.SortOrder;
			server.Description = client.Description;

			return server;
		}

		public WfClientRolePropertyDefinition ServerToClient(SOARolePropertyDefinition server, ref WfClientRolePropertyDefinition client)
		{
			server.NullCheck("server");

			if (client == null)
				client = new WfClientRolePropertyDefinition();

			client.Name = server.Name;
			client.Caption = server.Caption;
			client.DataType = server.DataType;
			client.DefaultValue = server.DefaultValue;
			client.SortOrder = server.SortOrder;
			client.Description = server.Description;

			return client;
		}
	}
}
