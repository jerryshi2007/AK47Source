using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects
{
	public interface IPropertyPersister<TItem> where TItem : IPropertyValueAccessor
	{
		void Read(TItem currentProperty, PersisterContext<TItem> context);

		void Write(TItem currentProperty, PersisterContext<TItem> context);
	}

	public abstract class PropertyPersisterBase<TItem> : IPropertyPersister<TItem> where TItem : IPropertyValueAccessor
	{
		public abstract void Read(TItem currentProperty, PersisterContext<TItem> context);

		public abstract void Write(TItem currentProperty, PersisterContext<TItem> context);

		protected void Register()
		{
			JSONSerializerExecute.RegisterConverter(typeof(EditorParamsDefineConverter));
			JSONSerializerExecute.RegisterConverter(typeof(ControlPropertyDefineConverter));
		}
	}

	public sealed class PersisterContext<TItem> : IDisposable where TItem : IPropertyValueAccessor
	{
		private object _Tag = null;

		private SerializableEditableKeyedDataObjectCollectionBase<string, TItem> _Properties = null;

		private PersisterContext(SerializableEditableKeyedDataObjectCollectionBase<string, TItem> properties, Object tag)
		{
			this._Properties = properties;
			this._Tag = tag;
		}

		public static PersisterContext<TItem> CreatePersisterContext(SerializableEditableKeyedDataObjectCollectionBase<string, TItem> properties, Object tag)
		{
			return new PersisterContext<TItem>(properties, tag);
		}

		public SerializableEditableKeyedDataObjectCollectionBase<string, TItem> Properties
		{
			get
			{
				return this._Properties;
			}
		}

		public object Tag
		{
			get
			{
				return this._Tag;
			}
		}

		public void Dispose()
		{
		}
	}
}
