using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.Office.OpenXml.Word
{
	/// <summary>
	/// 简单填充集合
	/// </summary>
	public sealed class SimplePropertyCollection : EditableKeyedDataObjectCollectionBase<string, DCTSimpleProperty>
	{


		protected override string GetKeyForItem(DCTSimpleProperty item)
		{
			return item.TagID;
		}
	}
}
