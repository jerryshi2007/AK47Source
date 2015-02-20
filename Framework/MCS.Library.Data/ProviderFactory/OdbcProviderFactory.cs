#region
// -------------------------------------------------
// Assembly	£º	DeluxeWorks.Library.Data
// FileName	£º	OdbcProviderFactory.cs
// Remark	£º	OdbcFactory Factory Class
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ÍõÏè	    20070430		´´½¨
// -------------------------------------------------
#endregion
using System.Data.Common;
using System.Data.Odbc;

namespace ChinaCustoms.Framework.DeluxeWorks.Library.Data
{   
    /// <summary>
    /// OdbcFactory Factory Calass
    /// </summary>
    internal class OdbcProviderFactory : ProviderFactory
    {
        public override DbProviderFactory Create()
        {
            return OdbcFactory.Instance;
        }
    }
}
