using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 支持级联传递的分页控件。当某个分页控件的分页改变后，允许同步到别的实现了ICascadePagedControl控件
	/// </summary>
	public interface ICascadePagedControl
	{
		/// <summary>
		/// 设置页码
		/// </summary>
		/// <param name="source"></param>
		/// <param name="pageIndex"></param>
		void SetPageIndex(object source, int pageIndex);
	}
}
