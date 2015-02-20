using System;
using System.Collections.Generic;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// ������ԱID�Ƚ���
    /// </summary>
    /// <typeparam name="T">����</typeparam>
    public class OguObjectIDEqualityComparer<T> : EqualityComparer<T> where T : class, IOguObject
    {
        private static OguObjectIDEqualityComparer<T> _S_Instance = new OguObjectIDEqualityComparer<T>();
        /// <summary>
        /// ʵ������
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
    ///  ������ԱFullPath�Ƚ���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OguObjectFullPathEqualityComparer<T> : EqualityComparer<T> where T : class, IOguObject
    {
        private static OguObjectFullPathEqualityComparer<T> _S_Instance = new OguObjectFullPathEqualityComparer<T>();
        /// <summary>
        /// ʵ������
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
