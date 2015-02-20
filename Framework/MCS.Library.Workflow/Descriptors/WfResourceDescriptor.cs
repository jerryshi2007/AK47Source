using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Properties;
using System.Xml;

namespace MCS.Library.Workflow.Descriptors
{
    /// <summary>
    /// 资源定义类的基类
    /// </summary>
    [Serializable]
    public abstract class WfResourceDescriptor : WfDescriptorBase
    {
		public virtual List<IUser> GetUserList()
		{
			ExceptionHelper.FalseThrow(false, Resource.ResourceCanNotGetUserList, this.GetType().Name);

			return null;
		}
    }

    /// <summary>
    /// 资源集合类
    /// </summary>
    [Serializable]
	public class WfResourceDescriptorCollection : WfCollectionBase<WfResourceDescriptor>
    {
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>返回按索引访问的类型</returns>
        public WfResourceDescriptor this[int index]
        {
            get
            {
				return InnerGet(index);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(WfResourceDescriptor item)
        {
			InnerAdd(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resDesp"></param>
        public void Remove(WfResourceDescriptor resDesp)
        {
			InnerRemove(resDesp);
        }
    }
}

