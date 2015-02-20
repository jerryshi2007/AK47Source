using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects
{
	public static class PropertiesPersisterHelper<T> where T : IPropertyValueAccessor
	{
		public static Dictionary<string, IPropertyPersister<T>> GetAllPropertiesPersisters()
		{
			return (Dictionary<string, IPropertyPersister<T>>)ObjectContextCache.Instance.GetOrAddNewValue("AllIPropertyPersister", (cache, key) =>
			{
				Dictionary<string, IPropertyPersister<T>> newItem = GetPersister();

				cache.Add(key, newItem);

				return newItem;
			});
		}

		private static Dictionary<string, IPropertyPersister<T>> GetPersister()
		{
			Dictionary<string, IPropertyPersister<T>> result = new Dictionary<string, IPropertyPersister<T>>();

			TypeConfigurationCollection persisterTypes = PropertyPersisterSettings.GetConfig().Persisters;
				//PropertyEditorSettings.GetConfig().Editors;

			foreach (TypeConfigurationElement typeElem in persisterTypes)
			{
				IPropertyPersister<T> editor = (IPropertyPersister<T>)typeElem.CreateInstance();

				if (result.ContainsKey(typeElem.Name) == false)
					result.Add(typeElem.Name, editor);
			}

			return result;
		}

	}
}
