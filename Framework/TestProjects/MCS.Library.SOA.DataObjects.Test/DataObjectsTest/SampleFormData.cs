using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Test.DataObjectsTest
{
	[Serializable]
	[XmlRootMapping("SampleFormData")]
	public class SampleFormData : GenericFormData
	{
		[XmlObjectMapping]
		[NoMapping]
		public string StringProperty
		{
			get;
			set;
		}

		private SampleSubFormDataCollection _SubData = null;

		[XmlObjectMapping]
		public SampleSubFormDataCollection SubData
		{
			get
			{
				if (this._SubData == null)
					this._SubData = new SampleSubFormDataCollection();

				return this._SubData;
			}
		}
	}

	[Serializable]
	public class SampleFormDataCollection : EditableDataObjectCollectionBase<SampleFormData>
	{
	}

	[Serializable]
	[XmlRootMapping("SampleSubFormData")]
	public class SampleSubFormData
	{
		[XmlObjectMapping]
		public int SubItemID
		{
			get;
			set;
		}

		[XmlObjectMapping]
		public string Name
		{
			get;
			set;
		}
	}

	[Serializable]
	public class SampleSubFormDataCollection : EditableDataObjectCollectionBase<SampleSubFormData>
	{
	}
}
