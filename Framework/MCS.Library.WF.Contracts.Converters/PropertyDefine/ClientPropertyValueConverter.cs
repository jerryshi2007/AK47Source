using MCS.Library.SOA.DataObjects;
using MCS.Library.WF.Contracts.PropertyDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;

namespace MCS.Library.WF.Contracts.Converters.PropertyDefine
{
    public class ClientPropertyValueConverter
    {
        public static readonly ClientPropertyValueConverter Instance = new ClientPropertyValueConverter();

        private ClientPropertyValueConverter()
        {
        }

        /// <summary>
        /// 客户端的属性值传递到服务器端，不会去改变服务器端的属性定义
        /// </summary>
        /// <param name="cpv"></param>
        /// <param name="pv"></param>
        public void ClientToServer(ClientPropertyValue cpv, PropertyValue pv)
        {
            cpv.NullCheck("cpv");

            if (pv != null)
                pv.StringValue = cpv.StringValue;
        }

        public void ServerToClient(PropertyValue pv, ClientPropertyValue cpv)
        {
            pv.NullCheck("pv");
            cpv.NullCheck("cpv");

            cpv.DataType = pv.Definition.DataType.ToClientPropertyDataType();
            cpv.Key = pv.Definition.Name;

            if (pv.StringValue.IsNullOrEmpty())
                cpv.StringValue = pv.Definition.DefaultValue;
            else
                cpv.StringValue = pv.StringValue;
        }
    }

    public class ClientPropertyValueCollectionConverter
    {
        public static readonly ClientPropertyValueCollectionConverter Instance = new ClientPropertyValueCollectionConverter();

        private ClientPropertyValueCollectionConverter()
        {
        }

        /// <summary>
        /// 仅复制目标集合中已有的属性
        /// </summary>
        /// <param name="cpvc"></param>
        /// <param name="pvc"></param>
        public void ClientToServer(IEnumerable<ClientPropertyValue> cpvc, PropertyValueCollection pvc)
        {
            cpvc.NullCheck("cpvc");
            pvc.NullCheck("pvc");

            foreach (ClientPropertyValue cpv in cpvc)
            {
                if (pvc.ContainsKey(cpv.Key))
                    ClientPropertyValueConverter.Instance.ClientToServer(cpv, pvc[cpv.Key]);
            }
        }

        /// <summary>
        /// 如果目标集合中不存在，则添加一项
        /// </summary>
        /// <param name="pvc"></param>
        /// <param name="cpvc"></param>
        public void ServerToClient(IEnumerable<PropertyValue> pvc, ClientPropertyValueCollection cpvc)
        {
            pvc.NullCheck("pvc");
            cpvc.NullCheck("cpvc");

            foreach (PropertyValue pv in pvc)
            {
                ClientPropertyValue cpv = cpvc[pv.Definition.Name];

                if (cpv == null)
                {
                    cpv = new ClientPropertyValue(pv.Definition.Name);
                    cpvc.Add(cpv);
                }
                ClientPropertyValueConverter.Instance.ServerToClient(pv, cpv);
            }
        }
    }
}
