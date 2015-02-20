#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	OleProviderFactory.cs
// Remark	��	OracleClientFactory Factory Class
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
// -------------------------------------------------
#endregion
using System.Data.Common;
using System.Data.OleDb;

namespace ChinaCustoms.Framework.DeluxeWorks.Library.Data
{
    /// <summary>
    /// OracleClientFactory Factory Class
    /// </summary>
    internal class OleProviderFactory : ProviderFactory
    {
        public override DbProviderFactory Create()
        {
            return OleDbFactory.Instance;
        }
    }
}
