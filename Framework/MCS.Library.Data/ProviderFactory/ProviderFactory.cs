#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	ProviderFactory.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace ChinaCustoms.Framework.DeluxeWorks.Library.Data
{
    abstract internal class ProviderFactory
    {
        public abstract DbProviderFactory Create();
    }
}
