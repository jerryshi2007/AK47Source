using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Runtime;
using System.Configuration.Provider;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    /// <summary>
    /// 数据提供程序
    /// </summary>
    public abstract class DataProviderBase : ProviderBase
    {
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        protected DataProviderBase()
        {
        }

        /// <summary>
        /// 重置，以便可以从头开始查询数据
        /// </summary>
        public abstract void Reset();
        /// <summary>
        /// 移动到下一个记录
        /// </summary>
        /// <returns></returns>
        public abstract bool MoveNext();
        /// <summary>
        /// 关闭
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// 获取当前对象的属性集字典
        /// </summary>
        public abstract NameObjectCollection CurrentData { get; }

    }
}
