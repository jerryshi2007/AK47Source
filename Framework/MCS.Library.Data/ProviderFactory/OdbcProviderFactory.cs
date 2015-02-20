#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	OdbcProviderFactory.cs
// Remark	��	OdbcFactory Factory Class
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
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
