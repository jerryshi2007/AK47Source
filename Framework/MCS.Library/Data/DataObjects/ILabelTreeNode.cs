using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// 带名称和描述的节点接口
    /// </summary>
    public interface INamedTreeNode
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 描述
        /// </summary>
        string Description
        {
            get;
        }
    }
}
