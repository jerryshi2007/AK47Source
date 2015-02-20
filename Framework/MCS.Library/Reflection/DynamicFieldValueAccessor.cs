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
    public class DynamicFieldValueAccessor : DynamicMemberValueAccessorBase
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DynamicFieldValueAccessor Instance = new DynamicFieldValueAccessor();

        private DynamicFieldValueAccessor()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="genericType"></param>
        /// <returns></returns>
        protected override IMemberAccessor CreateDelegationClass(Type genericType)
        {
            return Activator.CreateInstance(typeof(DynamicFieldDelegation<>).MakeGenericType(genericType)) as IMemberAccessor;
        }
    }
}
