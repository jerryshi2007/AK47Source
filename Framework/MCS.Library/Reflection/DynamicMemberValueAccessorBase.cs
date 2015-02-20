using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DynamicMemberValueAccessorBase : IMemberAccessor
    {
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Type, IMemberAccessor> _classAccessors = new Dictionary<Type, IMemberAccessor>();

        /// <summary>
        /// 
        /// </summary>
        protected readonly object _SyncObject = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public object GetValue(object instance, string memberName)
        {
            return this.GetValue(instance.GetType(), instance, memberName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public object GetValue(Type type, object instance, string memberName)
        {
            return this.FindClassAccessor(type).GetValue(instance, memberName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <param name="newValue"></param>
        public void SetValue(object instance, string memberName, object newValue)
        {
            this.SetValue(instance.GetType(), instance, memberName, newValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="memberName"></param>
        /// <param name="newValue"></param>
        public void SetValue(Type type, object instance, string memberName, object newValue)
        {
            this.FindClassAccessor(type).SetValue(instance, memberName, newValue);
        }

        /// <summary>
        /// 创建委托类的实例
        /// </summary>
        /// <param name="genericType"></param>
        /// <returns></returns>
        protected abstract IMemberAccessor CreateDelegationClass(Type genericType);

        private IMemberAccessor FindClassAccessor(Type type)
        {
            IMemberAccessor classAccessor;

            lock (this._SyncObject)
            {
                if (_classAccessors.TryGetValue(type, out classAccessor) == false)
                {
                    classAccessor = this.CreateDelegationClass(type);
                    ((IMemberDelegation)classAccessor).InitDelegations();

                    _classAccessors.Add(type, classAccessor);
                }
            }

            return classAccessor;
        }
    }
}
