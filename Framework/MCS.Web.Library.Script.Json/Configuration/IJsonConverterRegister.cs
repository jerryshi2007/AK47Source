using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Web.Library.Script.Configuration
{
    /// <summary>
    /// 序列化器的注册器
    /// </summary>
    public interface IJsonConverterRegister
    {
        void RegisterConverters();
    }
}
