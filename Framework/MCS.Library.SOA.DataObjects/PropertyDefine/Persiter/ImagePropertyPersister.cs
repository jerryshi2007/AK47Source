using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Library.Script;
using MCS.Library.Caching;
using System.Reflection;

namespace MCS.Library.SOA.DataObjects
{
	public abstract class ImagePropertyPersisterBase<T> : PropertyPersisterBase<T> where T : IPropertyValueAccessor
	{
		public override void Read(T currentProperty, PersisterContext<T> context)
		{

		}

		public override void Write(T currentProperty, PersisterContext<T> context)
		{
			string value = currentProperty.StringValue;

			if (value.IsNotEmpty())
			{
				var img = JSONSerializerExecute.Deserialize<ImageProperty>(value);
				if (img != null)
				{
					ImagePropertyAdapter.Instance.UpdateContent(img);
					currentProperty.StringValue = JSONSerializerExecute.Serialize(img);
				}
			}
		}
	}

	public sealed class ImagePropertyPersister : ImagePropertyPersisterBase<PropertyValue>
	{
	}
}
