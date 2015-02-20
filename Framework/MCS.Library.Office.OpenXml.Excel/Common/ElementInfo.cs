using System;
using System.Collections.Generic;
using System.Xml;
using MCS.Library.Core;
using System.Xml.Linq;
using System.Linq;

namespace MCS.Library.Office.OpenXml.Excel
{
	public abstract class ElementInfo : IDisposable
	{
		internal readonly Dictionary<string, string> Attributes = new Dictionary<string, string>();

		public virtual void Dispose()
		{
			this.Attributes.Clear();
		}

		public string GetAttribute(string key)
		{
			if (this.Attributes.ContainsKey(key))
			{
				return this.Attributes[key];
			}
			return null;
		}

		public void SetAttribute(string key, string value)
		{
			this.Attributes[key] = value;
		}

		public bool GetBooleanAttribute(string key)
		{
			if (this.Attributes.ContainsKey(key))
			{
				string value = this.Attributes[key];
				if (string.IsNullOrEmpty(value))
				{
					return false;
				}

				return value.Equals("1") ? true : false;
			}
			return false;
		}

		public void SetBooleanAttribute(string key, bool value)
		{
			if (value)
			{
				this.Attributes[key] = "1";
			}
			else
			{
				this.Attributes.Remove(key);
			}
		}

		/// <summary>
		/// 当不存在是返回"int.MinValue"
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public int GetIntAttribute(string key)
		{
			int result = int.MinValue;
			if (this.Attributes.ContainsKey(key) == true)
			{
				int.TryParse(this.Attributes[key], out result);
			}

			return result;
		}

		public void SetIntAttribute(string key, int value)
		{
			this.Attributes[key] = value.ToString();
		}

		protected internal abstract string NodeName { get; }
	}
}
