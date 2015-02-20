using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 实现客户端校验的校验器须实现此接口
    /// </summary>
    public interface IClientValidatable
    {
        /// <summary>
        /// 客户端校验函数名称
        /// </summary>
        string ClientValidateMethodName { get; }

        /// <summary>
        /// 获取客户端校验方法脚本
        /// </summary>
        /// <returns>script</returns>
        string GetClientValidateScript();

        /// <summary>
        /// 获取客户端校验附加数据，比如正则表达式，范围值，等等
        /// </summary>
        Dictionary<string, object> GetClientValidateAdditionalData(object info);

    }
}
