using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// Activity集合
    /// </summary>
    [Serializable]
    public class WfActivityCollection : WfKeyedCollectionBase<string, IWfActivity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IWfActivity this[int index]
        {
            get
            {
				return this.InnerGet(index);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IWfActivity this[string key]
        {
            get
            {
                return this.InnerGet(key);
            }
        }

        /// <summary>
        /// 增加一个节点
        /// </summary>
        /// <param name="act"></param>
        public void Add(IWfActivity act)
        {
			this.InnerAdd(act);
        }

        /// <summary>
        /// 删除一个节点
        /// </summary>
        /// <param name="act"></param>
        public void Remove(IWfActivity act)
        {
			this.InnerRemove(act);
        }

        /// <summary>
        /// 将ActivityID作为Key
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override string GetKeyFromItem(IWfActivity data)
        {
            return data.ID;
        }
    }
}
