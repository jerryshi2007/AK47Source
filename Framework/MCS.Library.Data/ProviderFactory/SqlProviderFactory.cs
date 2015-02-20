#region
// -------------------------------------------------
// Assembly	£º	DeluxeWorks.Library.Data
// FileName	£º	SqlProviderFactory.cs
// Remark	£º	SqlClientFactory Factory Class
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ÍõÏè	    20070430		´´½¨
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
