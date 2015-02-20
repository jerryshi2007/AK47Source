﻿using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MCS.Library.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class XmlObjectMappingItemCollection : KeyedCollection<string, XmlObjectMappingItem>
	{
		private string rootName = string.Empty;

		/// <summary>
		/// 
		/// </summary>
		public XmlObjectMappingItemCollection()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}

		/// <summary>
		/// 表名
		/// </summary>
		public string RootName
		{
			get { return this.rootName; }
			set { this.rootName = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected override string GetKeyForItem(XmlObjectMappingItem item)
		{
			return item.NodeName;
		}
	}
}
