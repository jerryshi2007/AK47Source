using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Caching
{
    /// <summary>
    /// Cache通知发送者的接口
    /// </summary>
    public interface ICacheNotifier
    {
        /// <summary>
        /// 同步发送Cache通知
        /// </summary>
        /// <param name="notifyDataArray"></param>
        void SendNotify(params CacheNotifyData[] notifyDataArray);

        /// <summary>
        /// 异步发送Cache通知
        /// </summary>
        /// <param name="notifyDataArray"></param>
        void SendNotifyAsync(params CacheNotifyData[] notifyDataArray);
    }
}
