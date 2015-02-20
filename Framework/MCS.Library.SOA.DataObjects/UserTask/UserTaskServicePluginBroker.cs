using MCS.Library.OGUPermission;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.UserTaskPlugin
{
    /// <summary>
    /// 任务同步服务调用代理
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.33440")]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "TaskPluginSoap", Namespace = "http://tempuri.org/")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(DictionaryEntry[]))]
    public class UserTaskServicePluginBroker : System.Web.Services.Protocols.SoapHttpClientProtocol
    {
        /// <summary>
        /// 创建实例
        /// </summary>
        public static UserTaskServicePluginBroker Instance
        {
            get
            {
                return new UserTaskServicePluginBroker();
            }
        }
        /// <summary>
        /// 构造，初始化服务地址
        /// </summary>
        private UserTaskServicePluginBroker()
        {
            ServiceBrokerContext.Current.InitWebClientProtocol(this);

            this.Url = ResourceUriSettings.GetConfig().Paths.CheckAndGet("userTaskPluginService").Uri.ToString();
        }

        /// <summary>
        /// 服务地址
        /// </summary>
        public new string Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                base.Url = value;
            }
        }

        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendUserTasks", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SendUserTasks(string josn, [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)] DictionaryEntry[] args)
        {
            this.Invoke("SendUserTasks", new object[] {
                        josn,
                        args});
        }

        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SetUserTasksAccomplished", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SetUserTasksAccomplished(string josn, [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)] DictionaryEntry[] args)
        {
            this.Invoke("SetUserTasksAccomplished", new object[] {
                        josn,
                        args});
        }

        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/DeleteUserAccomplishedTasks", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void DeleteUserAccomplishedTasks(string josn, [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)] DictionaryEntry[] args)
        {
            this.Invoke("DeleteUserAccomplishedTasks", new object[] {
                        josn,
                        args});
        }

        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/DeleteUserTasks", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void DeleteUserTasks(string josn, [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)] DictionaryEntry[] args)
        {
            this.Invoke("DeleteUserTasks", new object[] {
                        josn,
                        args});
        }

        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SyncProcess", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void SyncProcess(string json)
        {
            this.Invoke("SyncProcess", new object[] {
                        json});
        }

    }
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34230")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public partial class DictionaryEntry
    {

        private object keyField;

        private object valueField;

        /// <remarks/>
        public object Key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }

        /// <remarks/>
        public object Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
}
