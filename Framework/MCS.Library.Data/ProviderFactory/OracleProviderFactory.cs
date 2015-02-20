#region
// -------------------------------------------------
// Assembly	£º	DeluxeWorks.Library.Data
// FileName	£º	OracleProviderFactory.cs
// Remark	£º	OracleClientFactory Factory Class
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ÍõÏè	    20070430		´´½¨
// -------------------------------------------------
#endregion
using System.Data.Common;
using System.Data.OracleClient;

namespace ChinaCustoms.Framework.DeluxeWorks.Library.Data
{
    /// <summary>
    /// OracleClientFactory Factory Class
    /// </summary>
    internal class OracleProviderFactory : ProviderFactory
    {
        public override DbProviderFactory Create()
        {
            return OracleClientFactory.Instance;
        }
    }
}
