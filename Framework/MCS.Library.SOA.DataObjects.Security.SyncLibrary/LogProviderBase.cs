using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;
using System.Runtime;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    /// <summary>
    /// 日志提供程序
    /// </summary>
    public abstract class LogProviderBase : ProviderBase
    {
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        protected LogProviderBase()
        {
        }

        /// <summary>
        /// 写入已更新的对象日志
        /// </summary>
        /// <param name="syncSession"></param>
        /// <param name="scObj"></param>
        public abstract void WriteLog(SyncSession syncSession, SchemaObjectBase scObj);

        /// <summary>
        /// 写入更新失败的日志
        /// </summary>
        /// <param name="syncSession"></param>
        /// <param name="scObj"></param>
        /// <param name="ex"></param>
        public abstract void WriteErrorLog(SyncSession syncSession, SchemaObjectBase scObj, Exception ex);

        /// <summary>
        /// 写入开始更新的日志
        /// </summary>
        /// <param name="session"></param>
        public abstract void WriteStartLog(SyncSession session);

        /// <summary>
        /// 写入结束日志
        /// </summary>
        /// <param name="session"></param>
        /// <param name="success">表示是否成功结束</param>
        public abstract void WriteEndLog(SyncSession session, bool success);
    }
}
