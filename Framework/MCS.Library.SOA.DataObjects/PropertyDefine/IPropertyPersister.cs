using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	public interface IPropertyPersister
	{
		void Load(PropertyValue currentProperty);

		void Save(PropertyValue currentProperty);
	}

	public abstract class PropertyPersisterBase : IPropertyPersister
	{
		public abstract void Load(PropertyValue currentProperty);

		public abstract void Save(PropertyValue currentProperty);
	}

	/*
	public class PropertyPersisterManager
	{

	} */
}
