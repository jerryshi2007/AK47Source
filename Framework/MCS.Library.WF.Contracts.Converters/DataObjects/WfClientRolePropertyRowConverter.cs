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
	public class WfClientRolePropertyRowConverter
	{
		public static readonly WfClientRolePropertyRowConverter Instance = new WfClientRolePropertyRowConverter();

		private WfClientRolePropertyRowConverter()
		{
		}

		public SOARolePropertyRow ClientToServer(WfClientRolePropertyRow client, SOARolePropertyDefinitionCollection serverColumns, ref SOARolePropertyRow server)
		{
			client.NullCheck("client");

			if (server == null)
				server = new SOARolePropertyRow();

			server.RowNumber = client.RowNumber;
			server.Operator = client.Operator;
			server.OperatorType = client.OperatorType.ToRoleOperatorType();

			foreach (WfClientRolePropertyValue cpv in client.Values)
			{
				SOARolePropertyValue spv = null;

				WfClientRolePropertyValueConverter.Instance.ClientToServer(cpv, serverColumns, ref spv);

				server.Values.Add(spv);
			}

			return server;
		}

		public WfClientRolePropertyRow ServerToClient(SOARolePropertyRow server, WfClientRolePropertyDefinitionCollection clientColumns, ref WfClientRolePropertyRow client)
		{
			server.NullCheck("server");

			if (client == null)
				client = new WfClientRolePropertyRow();

			client.RowNumber = server.RowNumber;
			client.Operator = server.Operator;
			client.OperatorType = server.OperatorType.ToClientRoleOperatorType();

			foreach (SOARolePropertyValue spv in server.Values)
			{
				WfClientRolePropertyValue cpv = null;

				WfClientRolePropertyValueConverter.Instance.ServerToClient(spv, clientColumns, ref cpv);

				client.Values.Add(cpv);
			}

			return client;
		}
	}
}
