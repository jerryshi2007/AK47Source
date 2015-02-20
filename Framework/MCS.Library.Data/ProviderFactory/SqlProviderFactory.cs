#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	SqlProviderFactory.cs
// Remark	��	SqlClientFactory Factory Class
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
// -------------------------------------------------
#endregion
using System.Data.Common;
using System.Data.SqlClient;


namespace ChinaCustoms.Framework.DeluxeWorks.Library.Data
{
    /// <summary>
    /// SqlClientFactory Factory Class
    /// </summary>
    internal class SqlProviderFactory : ProviderFactory
    {
        public override DbProviderFactory Create()
        {
            return SqlClientFactory.Instance;
        }
    }
}
