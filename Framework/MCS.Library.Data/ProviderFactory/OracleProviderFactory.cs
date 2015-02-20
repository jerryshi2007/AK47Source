#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	OracleProviderFactory.cs
// Remark	��	OracleClientFactory Factory Class
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
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
