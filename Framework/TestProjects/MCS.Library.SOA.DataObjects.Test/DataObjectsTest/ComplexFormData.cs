using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Test.DataObjectsTest
{
	[Serializable]
	[XmlRootMapping("ComplexFormData")]
	public class ComplexFormData : GenericFormData
	{
		[XmlObjectMapping]
		[NoMapping]
		public string StringProperty
		{
			get;
			set;
		}

		private SubClassDataTypeACollection _SubDataA = null;

		public SubClassDataTypeACollection SubDataA
		{
			get
			{
				if (this._SubDataA == null)
				{
					if (this.Loaded)
						this._SubDataA = ComplexFormDataAdapter.Instance.LoadRelativeData<SubClassDataTypeA, SubClassDataTypeACollection>(this.ID, "SubDataA");
					else
						this._SubDataA = new SubClassDataTypeACollection();
				}

				return this._SubDataA;
			}
		}

		private SubClassDataTypeBCollection _SubDataB = null;

		public SubClassDataTypeBCollection SubDataB
		{
			get
			{
				if (this._SubDataB == null)
				{
					if (this.Loaded)
						this._SubDataB = ComplexFormDataAdapter.Instance.LoadRelativeData<SubClassDataTypeB, SubClassDataTypeBCollection>(this.ID, "SubDataB");
					else
						this._SubDataB = new SubClassDataTypeBCollection();
				}

				return this._SubDataB;
			}
		}
	}

	[Serializable]
	public class GenericFormRelativeDataCollection : EditableDataObjectCollectionBase<ComplexFormData>
	{
	}

	[Serializable]
	[XmlRootMapping("SubClassDataTypeA")]
	public class SubClassDataTypeA : GenericFormRelativeData
	{
		[XmlObjectMapping]
		[NoMapping]
		public string SubStringPropertyA
		{
			get;
			set;
		}
	}

	[Serializable]
	public class SubClassDataTypeACollection : EditableDataObjectCollectionBase<SubClassDataTypeA>
	{
	}

	[Serializable]
	[XmlRootMapping("SubClassDataTypeB")]
	public class SubClassDataTypeB : GenericFormRelativeData
	{
		[XmlObjectMapping]
		[NoMapping]
		public string SubStringPropertyB
		{
			get;
			set;
		}
	}

	[Serializable]
	public class SubClassDataTypeBCollection : EditableDataObjectCollectionBase<SubClassDataTypeB>
	{
	}
}
