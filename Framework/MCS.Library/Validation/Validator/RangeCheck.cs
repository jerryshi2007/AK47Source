using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RangeChecker<T> where T : IComparable
    {
        private T lowerBound;
        private T upperBound;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        public RangeChecker(T lowerBound, T upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool IsInRange(T target)
        {
            return target.CompareTo(this.lowerBound) >= 0 && target.CompareTo(this.upperBound) <= 0;
        }
    }
}
