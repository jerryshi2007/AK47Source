using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.Descriptors
{
	public class WfClientResourceDescriptorConverterFactory
	{
		private static Dictionary<Type, WfClientResourceDescriptorConverterBase> _ClientToServerMap = new Dictionary<Type, WfClientResourceDescriptorConverterBase>();
		private static Dictionary<Type, WfClientResourceDescriptorConverterBase> _ServerToClientMap = new Dictionary<Type, WfClientResourceDescriptorConverterBase>();

		public static readonly WfClientResourceDescriptorConverterFactory Instance = new WfClientResourceDescriptorConverterFactory();

		static WfClientResourceDescriptorConverterFactory()
		{
			WfClientResourceDescriptorConverterFactory.RegisterClientToServer(typeof(WfClientUserResourceDescriptor), WfClientUserResourceDescriptorConverter.Instance);
			WfClientResourceDescriptorConverterFactory.RegisterServerToClient(typeof(WfUserResourceDescriptor), WfClientUserResourceDescriptorConverter.Instance);

			WfClientResourceDescriptorConverterFactory.RegisterClientToServer(typeof(WfClientDepartmentResourceDescriptor), WfClientOrganizationResourceDescriptorConverter.Instance);
			WfClientResourceDescriptorConverterFactory.RegisterServerToClient(typeof(WfDepartmentResourceDescriptor), WfClientOrganizationResourceDescriptorConverter.Instance);

			WfClientResourceDescriptorConverterFactory.RegisterClientToServer(typeof(WfClientGroupResourceDescriptor), WfClientGroupResourceDescriptorConverter.Instance);
			WfClientResourceDescriptorConverterFactory.RegisterServerToClient(typeof(WfGroupResourceDescriptor), WfClientGroupResourceDescriptorConverter.Instance);

			WfClientResourceDescriptorConverterFactory.RegisterClientToServer(typeof(WfClientRoleResourceDescriptor), WfClientRoleResourceDescriptorConverter.Instance);
			WfClientResourceDescriptorConverterFactory.RegisterServerToClient(typeof(WfRoleResourceDescriptor), WfClientRoleResourceDescriptorConverter.Instance);

			WfClientResourceDescriptorConverterFactory.RegisterClientToServer(typeof(WfClientActivityAssigneesResourceDescriptor), WfClientActivityAssigneesResourceDescriptorConverter.Instance);
			WfClientResourceDescriptorConverterFactory.RegisterServerToClient(typeof(WfActivityAssigneesResourceDescriptor), WfClientActivityAssigneesResourceDescriptorConverter.Instance);

			WfClientResourceDescriptorConverterFactory.RegisterClientToServer(typeof(WfClientActivityOperatorResourceDescriptor), WfClientActivityOperatorResourceDescriptorConverter.Instance);
			WfClientResourceDescriptorConverterFactory.RegisterServerToClient(typeof(WfActivityOperatorResourceDescriptor), WfClientActivityOperatorResourceDescriptorConverter.Instance);

			WfClientResourceDescriptorConverterFactory.RegisterClientToServer(typeof(WfClientDynamicResourceDescriptor), WfClientDynamicResourceDescriptorConverter.Instance);
			WfClientResourceDescriptorConverterFactory.RegisterServerToClient(typeof(WfDynamicResourceDescriptor), WfClientDynamicResourceDescriptorConverter.Instance);

			WfClientResourceDescriptorConverterFactory.RegisterClientToServer(typeof(WfClientActivityMatrixResourceDescriptor), WfClientActivityMatrixResourceDescriptorConverter.Instance);
			WfClientResourceDescriptorConverterFactory.RegisterServerToClient(typeof(WfActivityMatrixResourceDescriptor), WfClientActivityMatrixResourceDescriptorConverter.Instance);
		}

		private WfClientResourceDescriptorConverterFactory()
		{
		}

		public static void RegisterClientToServer(Type clientType, WfClientResourceDescriptorConverterBase converter)
		{
			_ClientToServerMap[clientType] = converter;
		}

		public static void RegisterServerToClient(Type serverType, WfClientResourceDescriptorConverterBase converter)
		{
			_ServerToClientMap[serverType] = converter;
		}

		public WfClientResourceDescriptorConverterBase GetConverterByClientType(Type clientType)
		{
			WfClientResourceDescriptorConverterBase result = null;

			_ClientToServerMap.TryGetValue(clientType, out result);

			return result;
		}

		public WfClientResourceDescriptorConverterBase GetConverterByServerType(Type serverType)
		{
			WfClientResourceDescriptorConverterBase result = null;

			_ServerToClientMap.TryGetValue(serverType, out result);

			return result;
		}
	}
}
