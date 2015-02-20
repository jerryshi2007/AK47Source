using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Web.WebControls
{
	[Serializable]
	public class RelativeLinkCategory
	{
		private RelativeLinkItemCollection _Links = new RelativeLinkItemCollection();

		[Browsable(true)]
		[DefaultValue("")]
		public string Title { get; set; }
		
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[MergableProperty(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue((string)null)]
		[Browsable(false)]
		public RelativeLinkItemCollection Links
		{
			get
			{
				if (this._Links == null)
					this._Links = new RelativeLinkItemCollection();

				return this._Links;
			}
		}
	}

	[Serializable]
	public class RelativeLinkCategoryCollection : DataObjectCollectionBase<RelativeLinkCategory>
	{
		public void Add(RelativeLinkCategory item)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

			InnerAdd(item);
		}

		public RelativeLinkCategory this[int index]
		{
			get
			{
				return (RelativeLinkCategory)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
	}
}
