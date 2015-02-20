using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.WF.Contracts.Ogu;
using System.Collections;

namespace MCS.Library.WF.Contracts.Converters.Runtime
{
    /// <summary>
    /// 字典项的客户端到服务器端的转换
    /// </summary>
    public class WfClientDictionaryConverter
    {
        public static WfClientDictionaryConverter Instance = new WfClientDictionaryConverter();

        private static readonly Dictionary<Type, Type> _ClientToServer = new Dictionary<Type, Type>()
        {
            {typeof(WfClientUser), typeof(OguUser)},
            {typeof(WfClientOrganization), typeof(OguOrganization)},
            {typeof(WfClientGroup), typeof(OguGroup)},
        };

        private static readonly Dictionary<Type, Type> _ServerToClient = new Dictionary<Type, Type>()
        {
            {typeof(OguUser), typeof(WfClientUser)},
            {typeof(OguOrganization), typeof(WfClientOrganization)},
            {typeof(OguGroup), typeof(WfClientGroup)},
        };

        private WfClientDictionaryConverter()
        {
        }

        public void ClientToServer(IEnumerable<KeyValuePair<string, object>> client, IDictionary<string, object> server)
        {
            client.NullCheck("client");
            server.NullCheck("server");

            foreach (KeyValuePair<string, object> kp in client)
                server[kp.Key] = ClientObjectToServer(kp.Value);
        }

        public void ServerToClient(IEnumerable<KeyValuePair<string, object>> server, IDictionary<string, object> client)
        {
            client.NullCheck("client");
            server.NullCheck("server");

            foreach (KeyValuePair<string, object> kp in server)
                client[kp.Key] = ServerObjectToClient(kp.Value);
        }

        private static object ClientObjectToServer(object client)
        {
            object server = client;

            if (client != null)
            {
                Type clientType = client.GetType();
                bool serverIsList = false;

                if (clientType != typeof(string))
                {
                    if (client is IEnumerable)
                    {
                        IList serverItems = CreateEnumerableList(clientType, ClientTypeToServerType);

                        foreach (object clientItem in ((IEnumerable)client))
                            serverItems.Add(ClientObjectToServer(clientItem));

                        server = serverItems;

                        serverIsList = true;
                    }
                }

                if (serverIsList == false)
                {
                    if (client is WfClientOguObjectBase)
                        server = ((WfClientOguObjectBase)client).ToOguObject();
                }
            }

            return server;
        }

        private static Type ClientTypeToServerType(Type clientType)
        {
            Type serverType = null;

            if (_ClientToServer.TryGetValue(clientType, out serverType) == false)
                serverType = clientType;

            return serverType;
        }

        private static Type ServerTypeToClientType(Type serverType)
        {
            Type clientType = null;

            if (_ServerToClient.TryGetValue(serverType, out clientType) == false)
                clientType = serverType;

            return clientType;
        }

        private static IList CreateEnumerableList(Type listType, Func<Type, Type> typeConverter)
        {
            IList result = null;

            Type[] typeArgs = listType.GenericTypeArguments;

            if (listType.IsGenericType && typeArgs.Length == 1)
            {
                Type resultType = typeof(List<>);
                Type genericArgumentType = typeArgs[0];

                if (typeConverter != null)
                    genericArgumentType = typeConverter(genericArgumentType);

                Type genericListType = resultType.MakeGenericType(genericArgumentType);

                result = (IList)Activator.CreateInstance(genericListType);
            }
            else
            {
                result = new List<object>();
            }

            return result;
        }

        private static object ServerObjectToClient(object server)
        {
            object client = server;
            bool clientIsList = false;

            if (server != null)
            {
                Type serverType = server.GetType();

                if (serverType != typeof(string))
                {
                    if (server is IEnumerable)
                    {
                        IList clientItems = CreateEnumerableList(serverType, ServerTypeToClientType);

                        foreach (object serverItem in ((IEnumerable)server))
                            clientItems.Add(ServerObjectToClient(serverItem));

                        client = clientItems;
                        clientIsList = true;
                    }
                }

                if (clientIsList == false)
                {
                    if (server is IOguObject)
                        client = ((IOguObject)server).ToClientOguObject();
                }
            }

            return client;
        }
    }
}
