#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	EnumItemDescriptionAttribute.cs
// Remark	：	这些描述信息包括：描述信息，ID号，短名。
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MCS.Library.Properties;
using MCS.Library.Globalization;

namespace MCS.Library.Core
{
	/// <summary>
	/// 定义了枚举型中每个枚举项附加的属性，这个属性包含了对该枚举项的描述信息
	/// </summary>
	/// <remarks>
	/// 这些描述信息包括：描述信息，ID号，短名。
	/// </remarks>
	[AttributeUsageAttribute(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class EnumItemDescriptionAttribute : System.Attribute
	{
		internal string description = string.Empty;
		private int sortId = -1;
		private string shortName = string.Empty;
		private string category = string.Empty;

		private static LruDictionary<Type, EnumItemDescriptionList> innerDictionary =
			new LruDictionary<Type, EnumItemDescriptionList>();

		/// <summary>
		/// 缺省的构造方法
		/// </summary>
		/// <remarks>
		/// 缺省的构造方法
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" lang="cs" title="EnumItemDescriptionAttribute缺省的构造方法" />
		/// </remarks>
		public EnumItemDescriptionAttribute()
		{
		}

		/// <summary>
		/// 带描述信息参数的构造函数
		/// </summary>
		/// <param name="desp">对枚举项的描述信息</param>
		/// <remarks>
		/// 带描述信息参数的构造函数，同时设置枚举项附加属性的描述值等于desp。
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" lang="cs" title="获得带描述信息参数的构造方法" />
		/// </remarks>
		public EnumItemDescriptionAttribute(string desp)
		{
			this.description = desp;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="desp"></param>
		/// <param name="category"></param>
		public EnumItemDescriptionAttribute(string desp, string category)
		{
			this.description = desp;
			this.category = category;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="desp">枚举项的描述信息</param>
		/// <param name="sort">枚举项的排序号</param>
		public EnumItemDescriptionAttribute(string desp, int sort)
		{
			this.description = desp;
			this.sortId = sort;
		}

		/// <summary>
		/// 带描述信息和排序号参数的构造函数
		/// </summary>
		/// <param name="desp">枚举项的描述信息</param>
		/// <param name="sort">枚举项的排序号</param>
		/// <param name="category">多语言版的类别</param>
		/// <remarks>
		/// 带描述信息和排序号参数的构造函数，同时设置枚举项附加属性的描述信息和排序号
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" lang="cs" title="带描述信息和序号参数的构造方法" />
		/// </remarks>
		public EnumItemDescriptionAttribute(string desp, int sort, string category)
		{
			this.description = desp;
			this.sortId = sort;
			this.category = category;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="desp"></param>
		/// <param name="sName"></param>
		/// <param name="sort"></param>
		public EnumItemDescriptionAttribute(string desp, string sName, int sort)
		{
			this.description = desp;
			this.shortName = sName;
			this.sortId = sort;
		}

		/// <summary>
		/// 带描述信息、短名和排序号参数的构造函数
		/// </summary>
		/// <param name="desp">枚举项的描述信息</param>
		/// <param name="sName">枚举项的短名</param>
		/// <param name="sort">枚举项的排序号</param>
		/// <param name="category">多语言版的类别</param>
		/// <remarks>带描述信息、短名和排序号参数的构造函数，同时设置枚举项附加属性的描述信息、短名和排序号
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" lang="cs" title="带描述信息、短名和排序号参数的构造方法" />
		///</remarks>
		public EnumItemDescriptionAttribute(string desp, string sName, int sort, string category)
		{
			this.description = desp;
			this.shortName = sName;
			this.sortId = sort;
			this.category = category;
		}

		/// <summary>
		/// 枚举项的描述信息
		/// </summary>
		/// <remarks>该属性是可读写的
		/// </remarks>
		public string Description
		{
			get
			{
				string result = this.description;

				if (string.IsNullOrEmpty(this.category) == false)
					result = Translator.Translate(this.category, this.description);

				return result; 
			}
			set
			{ 
				this.description = value; 
			}
		}

		 /// <summary>
		 /// 枚举项的排序号
		 /// </summary>
		 /// <remarks>该属性是可读写的
		 /// </remarks>
		public int SortId
		{
			get { return this.sortId; }
			set { this.sortId = value; }
		}

		/// <summary>
		/// 枚举项的短名
		/// </summary>
		/// <remarks>该属性是可读写的
		/// </remarks>
		public string ShortName
		{
			get { return this.shortName; }
			set { this.shortName = value; }
		}

		/// <summary>
		/// 翻译的类别
		/// </summary>
		public string Category
		{
			get { return this.category; }
			set { this.category = value; }
		}

		/// <summary>
		/// 获得枚举项附加属性的描述信息属性
		/// </summary>
		/// <param name="enumItem">枚举项</param>
		/// <returns>描述信息属性，若该附加属性没有定义，则返回null</returns>
		/// <remarks>获得枚举项的附加属性，若该附加属性没有定义，则返回null
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" region = "GetAttributeTest" lang="cs" title="得到枚举项的描述信息属性" />
		/// </remarks>
		[System.Diagnostics.DebuggerNonUserCode]
		public static EnumItemDescriptionAttribute GetAttribute(System.Enum enumItem)
		{
			return AttributeHelper.GetCustomAttribute<EnumItemDescriptionAttribute>(enumItem.GetType().GetField(enumItem.ToString()));
		}

		/// <summary>
		/// 获得枚举项的描述信息值，若没有定义该附加属性，则返回空串
		/// </summary>
		/// <param name="enumItem">枚举项</param>
		/// <returns>枚举项的描述信息值，若没有定义该枚举项附加属性，则返回空串</returns>
		/// <remarks>获得枚举项的描述信息值，若没有定义该枚举项附加属性，则返回空串
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" region = "GetDescriptionTest" lang="cs" title="得到枚举项的描述信息属性" />
		/// </remarks>
		[System.Diagnostics.DebuggerNonUserCode]
		public static string GetDescription(System.Enum enumItem)
		{
			string result = string.Empty;

			EnumItemDescriptionAttribute attr = GetAttribute(enumItem);

			if (attr != null)
				result = attr.Description;

			return result;
		}

		/// <summary>
		/// 获得已排序的枚举型的描述信息表
		/// </summary>
		/// <param name="enumType">枚举型</param>
		/// <returns>已排序的枚举型的描述信息表</returns>
		/// <remarks>得到已排序的枚举型的描述信息表，该表是根据SortID属性排序的。
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" region = "GetEnumItemDescriptionListTest" lang="cs" title="获得已排序的枚举项的描述信息表" />
		/// </remarks>
		public static EnumItemDescriptionList GetDescriptionList(Type enumType)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(enumType != null, "enumType");
			ExceptionHelper.FalseThrow<ArgumentException>(enumType.IsEnum, Resource.TypeMustBeEnum, enumType.FullName);

			lock (EnumItemDescriptionAttribute.innerDictionary)
			{
				EnumItemDescriptionList result;

				if (EnumItemDescriptionAttribute.innerDictionary.TryGetValue(enumType, out result) == false)
				{
					result = GetDescriptionListFromEnumType(enumType);
					EnumItemDescriptionAttribute.innerDictionary.Add(enumType, result);
				}

				return result;
			}
		}

		private static EnumItemDescriptionList GetDescriptionListFromEnumType(Type enumType)
		{
			List<EnumItemDescription> eidList = new List<EnumItemDescription>();

			FieldInfo[] fileds = enumType.GetFields();

			for (int i = 0; i < fileds.Length; i++)
			{
				FieldInfo fi = fileds[i];

				if (fi.IsLiteral && fi.IsStatic)
					eidList.Add(EnumItemDescription.CreateFromFieldInfo(fi, enumType));
			}

			eidList.Sort(delegate(EnumItemDescription x, EnumItemDescription y)
					{
						return Math.Sign(x.SortId - y.SortId);
					}
				);

			return new EnumItemDescriptionList(eidList);
		}
	}

	/// <summary>
	/// 枚举项描述类
	/// </summary>
	/// <remarks>用于描述枚举项的类，其中描述信息包括：Name，Description，ShortName，EnumValue，SortID。
	/// </remarks>
	public sealed class EnumItemDescription : IComparer<EnumItemDescription>
	{
		private string name = string.Empty;
		private string description = string.Empty;
		private string shortName = string.Empty;
		private int enumValue;
		private int sortId = -1;
		private string category = string.Empty;

		/// <summary>
		/// 通过一个字段属性创建一个对枚举项描述的实例
		/// </summary>
		/// <param name="fi">字段属性实例</param>
		/// <param name="enumType">枚举型</param>
		/// <returns>描述枚举项的实例</returns>
		/// <remarks>通过一个字段属性值创建一个枚举项描述的实例
		/// <seealso cref="MCS.Library.Core.EnumItemDescriptionAttribute"/>
		/// </remarks>
		internal static EnumItemDescription CreateFromFieldInfo(FieldInfo fi, Type enumType)
		{
			EnumItemDescription eid = new EnumItemDescription();

			eid.Name = fi.Name;
			eid.EnumValue = (int)fi.GetValue(enumType);
			eid.SortId = eid.EnumValue;

			eid.FillDescriptionAttributeInfo(AttributeHelper.GetCustomAttribute<EnumItemDescriptionAttribute>(fi));

			return eid;
		}

		private void FillDescriptionAttributeInfo(EnumItemDescriptionAttribute attr)
		{
			if (attr != null)
			{
				this.Description = attr.description;
				this.ShortName = attr.ShortName;
				this.Category = attr.Category;

				if (attr.SortId != -1)
					this.sortId = attr.SortId;
			}
		}

		private EnumItemDescription()
		{
		}

		/// <summary>
		/// 枚举项排序的ID
		/// </summary>
		/// <remarks>枚举项排序的ID，当对枚举项进行排序时，使用该属性进行排序，该属性是只读的
		/// </remarks>
		public int SortId
		{
			get { return this.sortId; }
			internal set { this.sortId = value; }
		}

		/// <summary>
		/// 枚举项的值
		/// </summary>
		/// <remarks>该属性是只读的
		/// </remarks>
		public int EnumValue
		{
			get { return this.enumValue; }
			internal set { this.enumValue = value; }
		}

		/// <summary>
		/// 枚举项的短名
		/// </summary>
		/// <remarks>该属性是只读的
		/// </remarks>
		public string ShortName
		{
			get { return this.shortName; }
			internal set { this.shortName = value; }
		}

		/// <summary>
		/// 枚举项的描述信息
		/// </summary>
		/// <remarks>该属性是只读的
		/// </remarks> 
		public string Description
		{
			get
			{
				string result = this.description;

				if (string.IsNullOrEmpty(this.category) == false)
					result = Translator.Translate(this.category, this.description);

				return result; 
			}
			internal set
			{ 
				this.description = value; 
			}
		}

		/// <summary>
		/// 类别
		/// </summary>
		public string Category
		{
			get { return this.category; }
			internal set { this.category = value; }
		}

		/// <summary>
		/// 枚举项的名字
		/// </summary>
		/// <remarks>
		/// 该属性是只读的
		/// </remarks>
		public string Name
		{
			get { return this.name; }
			internal set { this.name = value; }
		}

		#region IComparer<EnumItemDescription> 成员

		/// <summary>
		/// 枚举项的比较方法
		/// </summary>
		/// <param name="x">需要比较的枚举项描述类的实例</param>
		/// <param name="y">需要比较的枚举项描述类的实例</param>
		/// <returns>比较结果</returns>
		/// <remarks>枚举项的比较方法,返回值是两个实例的排序号相减的结果
		/// </remarks>
		public int Compare(EnumItemDescription x, EnumItemDescription y)
		{
			return x.SortId - y.SortId;
		}

		#endregion
	}

	/// <summary>
	/// 强类型的枚举项描述信息集合
	/// </summary>
	/// <remarks>
	/// 强类型的枚举项描述信息集合,该信息是只读的
	/// </remarks>
	public sealed class EnumItemDescriptionList : ReadOnlyCollection<EnumItemDescription>
	{
		/// <summary>
		/// 带枚举项描述信息类参数的构造函数
		/// </summary>
		/// <param name="list">枚举项描述信息类的实例</param>
		/// <remarks>带枚举项描述信息类参数的构造函数
		/// </remarks>
		public EnumItemDescriptionList(IList<EnumItemDescription> list) : base(list)
		{
		}

		/// <summary>
		/// 获得指定位置元素的枚举项描述信息
		/// </summary>
		/// <param name="i">第i个元素</param>
		/// <returns>枚举项描述信息类的实例</returns>
		/// <remarks>该属性是只读的
		/// </remarks>
		public new EnumItemDescription this[int i]
		{
			get { return base[i]; }
		}
	}
}
