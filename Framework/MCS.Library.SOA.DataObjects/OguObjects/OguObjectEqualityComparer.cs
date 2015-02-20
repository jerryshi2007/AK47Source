using System;
using System.Collections.Generic;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 机构人员ID比较类
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class OguObjectIDEqualityComparer<T> : EqualityComparer<T> where T : class, IOguObject
    {
        private static OguObjectIDEqualityComparer<T> _S_Instance = new OguObjectIDEqualityComparer<T>();
        /// <summary>
        /// 实例对象
        /// </summary>
        public static new OguObjectIDEqualityComparer<T> Default
        {
            get
            {
                return _S_Instance;
            }
        }
   
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool Equals(T x, T y)
        {
            bool result = (x == y);
            if (!result)
            {
                if (x != null && y != null)
                    result = string.Equals(x.ID, y.ID, StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }

        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int GetHashCode(T obj)
        {
            return obj == null ? 0 : obj.ID.ToLower().GetHashCode();
        }
    }

    /// <summary>
    ///  机构人员FullPath比较类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OguObjectFullPathEqualityComparer<T> : EqualityComparer<T> where T : class, IOguObject
    {
        private static OguObjectFullPathEqualityComparer<T> _S_Instance = new OguObjectFullPathEqualityComparer<T>();
        /// <summary>
        /// 实例对象
        /// </summary>
        public static new OguObjectFullPathEqualityComparer<T> Default
        {
            get
            {
                return _S_Instance;
            }
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool Equals(T x, T y)
        {
            bool result = (x == y);
            if (!result)
            {
                if (x != null && y != null)
                    result = string.Equals(x.FullPath, y.FullPath, StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }

        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int GetHashCode(T obj)
        {
            return obj == null ? 0 : obj.FullPath.ToLower().GetHashCode();
        }
    }
}
