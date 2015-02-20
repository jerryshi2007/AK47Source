using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MCS.Library.SOA.ESB.Policies;
using ConfigurationService.ClientComponent;
using System.ServiceModel;
using MCS.Library.SOA.ESB.Proxy;
using System.ServiceModel.Channels;
using WfDemoService;

namespace WfDemoServiceTest
{
	class Program
	{
		static void Main(string[] args)
		{
            MseTest();
		}
        private static void MseTest()
        {
            //定义请求消息头
            RequestMessageHeader header = new RequestMessageHeader("0002", "190.1.1.1");
            header.UserCode = "0002";
            header.AddHeaderItem("ClientHostName", "clientNamexxxx");

            ConfigurationServiceClientSource clientSource = new ConfigurationServiceClientSource();
            //使用配置文件中的默认配置源的程序名称
            string bindingString = clientSource.GetSectionAsRawXml("MyApplication", "MseOrderBinding");
            string endpointString = clientSource.GetSectionAsRawXml("MyApplication", "MseOrderEndpoint");

            string configString = bindingString + endpointString;
            EndpointAddress eptAddress = ConfigurationHelper.BuilderEndpointAddress(configString);
            Binding binding = ConfigurationHelper.BuilderBinding(configString);

            MCS.Library.SOA.ESB.Proxy.ClientProxy<IDemoOrderService> proxy = new MCS.Library.SOA.ESB.Proxy.ClientProxy<IDemoOrderService>(header, binding, eptAddress);
            proxy.Open();
            DemoOrderData orderData =  proxy.ClientChannel.LoadByOrderID("OID2011021020");

            if (orderData != null)
            {
                Console.WriteLine(orderData.Subject);
            }

            //MSEOrder.svc_OrderServiceClient client = new MSEOrder.svc_OrderServiceClient();

            //MSEOrder.LoadByOrderID id = new MSEOrder.LoadByOrderID();
            //id.demoOrderID = "OID2011021020";

            //MSEOrder.LoadByOrderIDResponse response = client.VirLoadByOrderID(id);

            //if (response != null)
            //    Console.WriteLine(response.LoadByOrderIDResult.Subject);

            //client.Close();

            //Console.ReadLine();
        }
        private static void DirectTest()
        {
            //DemoOrderServiceClient client = new DemoOrderServiceClient();
            //DemoOrderData order = client.LoadByOrderID("OID2011021020");

            //if (order != null)
            //    Console.WriteLine(order.ExtensionData.ToString());

            //client.Close();

            //Console.ReadLine();
        }
	}
}
