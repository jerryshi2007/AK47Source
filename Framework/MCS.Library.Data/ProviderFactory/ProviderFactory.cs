#region
// -------------------------------------------------
// Assembly	£º	DeluxeWorks.Library.Data
// FileName	£º	ProviderFactory.cs
// Remark	£º	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ÍõÏè	    20070430		´´½¨
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
