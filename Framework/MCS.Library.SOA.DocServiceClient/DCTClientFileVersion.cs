using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DocServiceContract;
using MCS.Library.Core;

namespace MCS.Library.SOA.DocServiceClient
{
    public class DCTClientFileVersion : DCTFileVersion
    {
        private DCSClient client;

        public DCSClient Client
        {
            get { return client; }
            set { client = value; }
        }

        private DCTClientFile clientFile;

        public DCTClientFile ClientFile
        {
            get { return clientFile; }
            set { clientFile = value; }
        }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        public byte[] GetContent()
        {
            byte[] result = null;
            ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
            {
                result = action.DCMGetVersionFileContent(clientFile.ID, this.ID);
            });
            return result;
        }
    }
}
