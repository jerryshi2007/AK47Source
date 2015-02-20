using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicPropertyValueAccessor : DynamicMemberValueAccessorBase
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DynamicPropertyValueAccessor Instance = new DynamicPropertyValueAccessor();

        private DynamicPropertyValueAccessor()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="genericType"></param>
        /// <returns></returns>
        protected override IMemberAccessor CreateDelegationClass(Type genericType)
        {
            return Activator.CreateInstance(typeof(DynamicPropertyDelegation<>).MakeGenericType(genericType)) as IMemberAccessor;
        }
    }
}
