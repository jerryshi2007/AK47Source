#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	EnumItemDescriptionAttribute.cs
// Remark	��	��Щ������Ϣ������������Ϣ��ID�ţ�������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
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
	/// ������ö������ÿ��ö����ӵ����ԣ�������԰����˶Ը�ö�����������Ϣ
	/// </summary>
	/// <remarks>
	/// ��Щ������Ϣ������������Ϣ��ID�ţ�������
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
		/// ȱʡ�Ĺ��췽��
		/// </summary>
		/// <remarks>
		/// ȱʡ�Ĺ��췽��
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" lang="cs" title="EnumItemDescriptionAttributeȱʡ�Ĺ��췽��" />
		/// </remarks>
		public EnumItemDescriptionAttribute()
		{
		}

		/// <summary>
		/// ��������Ϣ�����Ĺ��캯��
		/// </summary>
		/// <param name="desp">��ö�����������Ϣ</param>
		/// <remarks>
		/// ��������Ϣ�����Ĺ��캯����ͬʱ����ö��������Ե�����ֵ����desp��
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" lang="cs" title="��ô�������Ϣ�����Ĺ��췽��" />
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
		/// <param name="desp">ö�����������Ϣ</param>
		/// <param name="sort">ö����������</param>
		public EnumItemDescriptionAttribute(string desp, int sort)
		{
			this.description = desp;
			this.sortId = sort;
		}

		/// <summary>
		/// ��������Ϣ������Ų����Ĺ��캯��
		/// </summary>
		/// <param name="desp">ö�����������Ϣ</param>
		/// <param name="sort">ö����������</param>
		/// <param name="category">�����԰�����</param>
		/// <remarks>
		/// ��������Ϣ������Ų����Ĺ��캯����ͬʱ����ö��������Ե�������Ϣ�������
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" lang="cs" title="��������Ϣ����Ų����Ĺ��췽��" />
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
		/// ��������Ϣ������������Ų����Ĺ��캯��
		/// </summary>
		/// <param name="desp">ö�����������Ϣ</param>
		/// <param name="sName">ö����Ķ���</param>
		/// <param name="sort">ö����������</param>
		/// <param name="category">�����԰�����</param>
		/// <remarks>��������Ϣ������������Ų����Ĺ��캯����ͬʱ����ö��������Ե�������Ϣ�������������
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" lang="cs" title="��������Ϣ������������Ų����Ĺ��췽��" />
		///</remarks>
		public EnumItemDescriptionAttribute(string desp, string sName, int sort, string category)
		{
			this.description = desp;
			this.shortName = sName;
			this.sortId = sort;
			this.category = category;
		}

		/// <summary>
		/// ö�����������Ϣ
		/// </summary>
		/// <remarks>�������ǿɶ�д��
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
		 /// ö����������
		 /// </summary>
		 /// <remarks>�������ǿɶ�д��
		 /// </remarks>
		public int SortId
		{
			get { return this.sortId; }
			set { this.sortId = value; }
		}

		/// <summary>
		/// ö����Ķ���
		/// </summary>
		/// <remarks>�������ǿɶ�д��
		/// </remarks>
		public string ShortName
		{
			get { return this.shortName; }
			set { this.shortName = value; }
		}

		/// <summary>
		/// ��������
		/// </summary>
		public string Category
		{
			get { return this.category; }
			set { this.category = value; }
		}

		/// <summary>
		/// ���ö��������Ե�������Ϣ����
		/// </summary>
		/// <param name="enumItem">ö����</param>
		/// <returns>������Ϣ���ԣ����ø�������û�ж��壬�򷵻�null</returns>
		/// <remarks>���ö����ĸ������ԣ����ø�������û�ж��壬�򷵻�null
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" region = "GetAttributeTest" lang="cs" title="�õ�ö�����������Ϣ����" />
		/// </remarks>
		[System.Diagnostics.DebuggerNonUserCode]
		public static EnumItemDescriptionAttribute GetAttribute(System.Enum enumItem)
		{
			return AttributeHelper.GetCustomAttribute<EnumItemDescriptionAttribute>(enumItem.GetType().GetField(enumItem.ToString()));
		}

		/// <summary>
		/// ���ö�����������Ϣֵ����û�ж���ø������ԣ��򷵻ؿմ�
		/// </summary>
		/// <param name="enumItem">ö����</param>
		/// <returns>ö�����������Ϣֵ����û�ж����ö��������ԣ��򷵻ؿմ�</returns>
		/// <remarks>���ö�����������Ϣֵ����û�ж����ö��������ԣ��򷵻ؿմ�
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" region = "GetDescriptionTest" lang="cs" title="�õ�ö�����������Ϣ����" />
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
		/// ����������ö���͵�������Ϣ��
		/// </summary>
		/// <param name="enumType">ö����</param>
		/// <returns>�������ö���͵�������Ϣ��</returns>
		/// <remarks>�õ��������ö���͵�������Ϣ���ñ��Ǹ���SortID��������ġ�
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\EnumItemDescriptionAttributeTest.cs" region = "GetEnumItemDescriptionListTest" lang="cs" title="����������ö�����������Ϣ��" />
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
	/// ö����������
	/// </summary>
	/// <remarks>��������ö������࣬����������Ϣ������Name��Description��ShortName��EnumValue��SortID��
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
		/// ͨ��һ���ֶ����Դ���һ����ö����������ʵ��
		/// </summary>
		/// <param name="fi">�ֶ�����ʵ��</param>
		/// <param name="enumType">ö����</param>
		/// <returns>����ö�����ʵ��</returns>
		/// <remarks>ͨ��һ���ֶ�����ֵ����һ��ö����������ʵ��
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
		/// ö���������ID
		/// </summary>
		/// <remarks>ö���������ID������ö�����������ʱ��ʹ�ø����Խ������򣬸�������ֻ����
		/// </remarks>
		public int SortId
		{
			get { return this.sortId; }
			internal set { this.sortId = value; }
		}

		/// <summary>
		/// ö�����ֵ
		/// </summary>
		/// <remarks>��������ֻ����
		/// </remarks>
		public int EnumValue
		{
			get { return this.enumValue; }
			internal set { this.enumValue = value; }
		}

		/// <summary>
		/// ö����Ķ���
		/// </summary>
		/// <remarks>��������ֻ����
		/// </remarks>
		public string ShortName
		{
			get { return this.shortName; }
			internal set { this.shortName = value; }
		}

		/// <summary>
		/// ö�����������Ϣ
		/// </summary>
		/// <remarks>��������ֻ����
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
		/// ���
		/// </summary>
		public string Category
		{
			get { return this.category; }
			internal set { this.category = value; }
		}

		/// <summary>
		/// ö���������
		/// </summary>
		/// <remarks>
		/// ��������ֻ����
		/// </remarks>
		public string Name
		{
			get { return this.name; }
			internal set { this.name = value; }
		}

		#region IComparer<EnumItemDescription> ��Ա

		/// <summary>
		/// ö����ıȽϷ���
		/// </summary>
		/// <param name="x">��Ҫ�Ƚϵ�ö�����������ʵ��</param>
		/// <param name="y">��Ҫ�Ƚϵ�ö�����������ʵ��</param>
		/// <returns>�ȽϽ��</returns>
		/// <remarks>ö����ıȽϷ���,����ֵ������ʵ�������������Ľ��
		/// </remarks>
		public int Compare(EnumItemDescription x, EnumItemDescription y)
		{
			return x.SortId - y.SortId;
		}

		#endregion
	}

	/// <summary>
	/// ǿ���͵�ö����������Ϣ����
	/// </summary>
	/// <remarks>
	/// ǿ���͵�ö����������Ϣ����,����Ϣ��ֻ����
	/// </remarks>
	public sealed class EnumItemDescriptionList : ReadOnlyCollection<EnumItemDescription>
	{
		/// <summary>
		/// ��ö����������Ϣ������Ĺ��캯��
		/// </summary>
		/// <param name="list">ö����������Ϣ���ʵ��</param>
		/// <remarks>��ö����������Ϣ������Ĺ��캯��
		/// </remarks>
		public EnumItemDescriptionList(IList<EnumItemDescription> list) : base(list)
		{
		}

		/// <summary>
		/// ���ָ��λ��Ԫ�ص�ö����������Ϣ
		/// </summary>
		/// <param name="i">��i��Ԫ��</param>
		/// <returns>ö����������Ϣ���ʵ��</returns>
		/// <remarks>��������ֻ����
		/// </remarks>
		public new EnumItemDescription this[int i]
		{
			get { return base[i]; }
		}
	}
}
