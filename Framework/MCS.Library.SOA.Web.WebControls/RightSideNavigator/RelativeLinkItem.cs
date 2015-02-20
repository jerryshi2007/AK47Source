using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Web.WebControls
{
	[Serializable]
	[ParseChildren(true)]
	public class RelativeLinkItem
	{
		public string Title { get; set; }

		public string CategoryName { get; set; }

		public string MoreCategoryName { get; set; }
	}

	[Serializable]
	public class RelativeLinkItemCollection : DataObjectCollectionBase<RelativeLinkItem>
	{
		public void Add(RelativeLinkItem item)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

			InnerAdd(item);
		}

		public RelativeLinkItem this[int index]
		{
			get
			{
				return (RelativeLinkItem)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
	}
}
