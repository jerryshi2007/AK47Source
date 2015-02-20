using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Web.UI;
using System.ComponentModel;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Web.WebControls
{
	[Serializable]
	public class OpinionReasonItem
	{
		private string key = string.Empty;
		private string description = string.Empty;
		private bool autoFill = true;

		[DefaultValue("")]
		public string Key 
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		[DefaultValue("")]
		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		[DefaultValue(true)]
		public bool AutoFill
		{
			get
			{
				return this.autoFill;
			}
			set
			{
				this.autoFill = value;
			}
		}
	}

	[Serializable]
	public class OpinionReasonItemCollection : DataObjectCollectionBase<OpinionReasonItem>
	{
		public void Add(OpinionReasonItem item)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

			InnerAdd(item);
		}

		public OpinionReasonItem this[int index]
		{
			get
			{
				return (OpinionReasonItem)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
	}
}
