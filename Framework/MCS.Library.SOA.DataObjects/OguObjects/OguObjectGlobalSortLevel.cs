using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// OguObject对象GlobalSort比较对象
    /// </summary>
    public class OguObjectGlobalSortLevel
    {
        private IOguObject oguObject = null;
        public OguObjectGlobalSortLevel(IOguObject ogu)
        {
            this.oguObject = ogu;
        }

        public static bool operator ==(OguObjectGlobalSortLevel a, OguObjectGlobalSortLevel b)
        {
            return string.Equals(a.oguObject.GlobalSortID, b.oguObject.GlobalSortID, StringComparison.Ordinal);
        }

        public static bool operator !=(OguObjectGlobalSortLevel a, OguObjectGlobalSortLevel b)
        {
            return !(a == b);
        }

        public static bool operator >(OguObjectGlobalSortLevel a, OguObjectGlobalSortLevel b)
        {
            return string.Compare(a.oguObject.GlobalSortID, b.oguObject.GlobalSortID, StringComparison.Ordinal) < 0;
        }

        public static bool operator >=(OguObjectGlobalSortLevel a, OguObjectGlobalSortLevel b)
        {
            return string.Compare(a.oguObject.GlobalSortID, b.oguObject.GlobalSortID, StringComparison.Ordinal) <= 0;
        }

        public static bool operator <(OguObjectGlobalSortLevel a, OguObjectGlobalSortLevel b)
        {
            return string.Compare(a.oguObject.GlobalSortID, b.oguObject.GlobalSortID, StringComparison.Ordinal) > 0;
        }

        public static bool operator <=(OguObjectGlobalSortLevel a, OguObjectGlobalSortLevel b)
        {
            return string.Compare(a.oguObject.GlobalSortID, b.oguObject.GlobalSortID, StringComparison.Ordinal) >= 0;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
