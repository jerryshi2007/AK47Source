using System;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Security
{
	[Serializable]
	public abstract class SCBase : SchemaObjectBase, ISCQualifiedNameObject
	{
		public SCBase(string schemaType) :
			base(schemaType)
		{
		}

		[NoMapping]
		public string Name
		{
			get
			{
				return this.Properties.GetValue("Name", string.Empty);
			}
			set
			{
				this.Properties.SetValue("Name", value);
			}
		}

		[NoMapping]
		public string DisplayName
		{
			get
			{
				return this.Properties.GetValue("DisplayName", string.Empty);
			}
			set
			{
				this.Properties.SetValue("DisplayName", value);
			}
		}

		[NoMapping]
		public string CodeName
		{
			get
			{
				return this.Properties.GetValue("CodeName", string.Empty);
			}
			set
			{
				this.Properties.SetValue("CodeName", value);
			}
		}

		public virtual string ToDescription()
		{
			string result = ToString();

			if (this.SchemaType.IsNotEmpty())
			{
				string name = this.Properties.GetValue("Name", string.Empty);

				if (name.IsNotEmpty())
					result = string.Format("{0}:{1}({2})", this.SchemaType, name, this.ID);
				else
					result = string.Format("{0}({1})", this.SchemaType, this.ID);
			}

			return result;
		}

		public string GetQualifiedName()
		{
			return this.Name;
		}
	}
}
