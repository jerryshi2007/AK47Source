using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Test
{
	/// <summary>
	/// 用于序列化测试的Hashtable容器
	/// </summary>
	[Serializable]
	public class HashtableContainer : ISerializable
	{
		private Hashtable _Dictionary = null;
		public int SampleInt = 0;
		private string _SerializedDictionary = string.Empty;

		public HashtableContainer()
		{
		}

		public HashtableContainer(SerializationInfo info, StreamingContext context)
		{
			this.SampleInt = info.GetInt32("SampleInt");

			try
			{
				this._SerializedDictionary = info.GetString("_SerializedDictionary");

				if (this._SerializedDictionary.IsNotEmpty())
				{
					this._Dictionary = (Hashtable)SerializationHelper.DeserializeStringToObject(this._SerializedDictionary, SerializationFormatterType.Binary, new MappingBinder());
				}
			}
			catch (SerializationException)
			{
			}

			if (this._Dictionary == null)
				this._Dictionary = (Hashtable)info.GetValue("_Dictionary", typeof(Hashtable));
		}

		public Hashtable Dictionary
		{
			get
			{
				if (this._Dictionary == null)
					this._Dictionary = new Hashtable();

				return this._Dictionary;
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("SampleInt", this.SampleInt);
			info.AddValue("_Dictionary", this._Dictionary);

			if (IsInvalidDictionary(this._Dictionary))
				info.AddValue("_SerializedDictionary", this._SerializedDictionary);
			else
				info.AddValue("_SerializedDictionary", SerializationHelper.SerializeObjectToString(this._Dictionary, SerializationFormatterType.Binary));
		}

		private static bool IsInvalidDictionary(Hashtable table)
		{
			bool result = false;

			if (table != null)
			{
				foreach (DictionaryEntry entry in table)
				{
					if (entry.Value is UnknownSerializationType || entry.Value is TypeLoadException)
					{
						result = true;
						break;
					}
				}
			}

			return result;
		}
	}

	public class MappingBinder : SerializationBinder
	{
		public override Type BindToType(string assemblyName, string typeName)
		{
			Type result = null;

			try
			{
				Assembly assembly = Assembly.Load(assemblyName);

				result = assembly.GetType(typeName);

				if (result == null)
					result = typeof(UnknownTypeClass2);
			}
			catch (System.Exception)
			{
				result = typeof(UnknownSerializationType);
			}

			return result;
		}
	}

	//[Serializable]
	//public class UnknownTypeClass
	//{
	//    public string Name = "Jacob";
	//}

	[Serializable]
	public class UnknownTypeClass2
	{
		public string Name = "Jacob";
	}
}
