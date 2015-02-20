using System.DirectoryServices;
using MCS.Library.Core;

namespace MCS.Library
{
	/// <summary>
	/// 扩展方法类
	/// </summary>
	public static class ADExtension
	{
		/// <summary>
		/// 转换为AD的ObjectClass
		/// </summary>
		/// <param name="schemaType"></param>
		/// <returns></returns>
		public static string ToObjectClass(this ADSchemaType schemaType)
		{
			EnumItemDescriptionAttribute attr = EnumItemDescriptionAttribute.GetAttribute(schemaType);

			return attr.ShortName;
		}

		/// <summary>
		/// 将DirectoryEntry转换成ADSchemaType
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		public static ADSchemaType ToADSchemaType(this DirectoryEntry entry)
		{
			ADSchemaType result = ADSchemaType.Unspecified;

			EnumItemDescriptionList items = EnumItemDescriptionAttribute.GetDescriptionList(typeof(ADSchemaType));

			foreach (EnumItemDescription item in items)
			{
				if (string.Compare(item.ShortName, entry.SchemaClassName, true) == 0)
				{
					result = (ADSchemaType)item.EnumValue;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// 是否绑定到了远程对象
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		[System.Diagnostics.DebuggerNonUserCode]
		public static bool IsBounded(this DirectoryEntry entry)
		{
			bool result = false;

			if (entry != null)
			{
				try
				{
					entry.NativeGuid.CheckStringIsNullOrEmpty("NativeGuid");

					result = true;
				}
				catch (System.Runtime.InteropServices.COMException ex)
				{
					if (ex.ErrorCode != -2147463159)
						throw;
				}
			}

			return result;
		}

		/// <summary>
		/// 强制绑定远程对象
		/// </summary>
		/// <param name="entry"></param>
		[System.Diagnostics.DebuggerNonUserCode]
		public static void ForceBound(this DirectoryEntry entry)
		{
			if (entry != null)
			{
				try
				{
					entry.NativeGuid.CheckStringIsNullOrEmpty("NativeGuid");
				}
				catch (System.Runtime.InteropServices.COMException ex)
				{
					if (ex.ErrorCode != -2147463159)
						throw;
				}
			}
		}
	}
}
