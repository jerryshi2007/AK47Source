using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;

namespace MCS.Library.Services
{
    /// <summary>
    /// 启动在另一个AppDomain中的Service代理
    /// </summary>
    public class ServiceHostProxy : MarshalByRefObject, IDisposable
    {
        private ServiceHost _host = null;
        private readonly string _typeDesp = string.Empty;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="typeDesp"></param>
        public ServiceHostProxy(string typeDesp)
        {
            typeDesp.CheckStringIsNullOrEmpty("typeDesp");

            this._typeDesp = typeDesp;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            Type type = TypeCreator.GetTypeInfo(_typeDesp);

            this._host = new ServiceHost(type);
            this._host.Open();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._host != null)
                    this._host.Close();
            }
        }
    }
}
