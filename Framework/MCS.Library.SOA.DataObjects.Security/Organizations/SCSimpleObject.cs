using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 简单的Schema对象，不包含Property信息，主要用于从数据库中获取状态
	/// </summary>
	[Serializable]
	public class SCSimpleObject : VersionedSimpleObject
	{
		/// <summary>
		/// 获取或设置表示对象名称的字符串
		/// </summary>
		[ORFieldMapping("Name")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select)]
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置表示对象显示名称的字符串
		/// </summary>
		[ORFieldMapping("DisplayName")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select)]
		public string DisplayName
		{
			get;
			set;
		}

		[NoMapping]
		public string VisibleName
		{
			get
			{
				return string.IsNullOrEmpty(this.DisplayName) ? (string.IsNullOrEmpty(this.Name) ? this.ID.ToString() : this.Name) : this.DisplayName;
			}
		}

		/// <summary>
		/// 代码名称
		/// </summary>
		[NoMapping]
		public string CodeName
		{
			get;
			set;
		}

		///// <summary>
		///// 获取或设置表示对象创建日期的<see cref="DateTime"/>
		///// </summary>
		//[ORFieldMapping("CreateDate")]
		//[SqlBehavior(BindingFlags = ClauseBindingFlags.Select)]
		//public DateTime CreateDate
		//{
		//    get;
		//    set;
		//}

		//private IUser _Creator = null;

		///// <summary>
		///// 获取或设置表示对象创建者的<see cref="IUser"/>的实现类的实例。
		///// </summary>
		//[SubClassORFieldMapping("ID", "CreatorID")]
		//[SubClassORFieldMapping("DisplayName", "CreatorName")]
		//[SubClassType(typeof(OguUser))]
		//[SubClassSqlBehavior(BindingFlags = ClauseBindingFlags.Select)]
		//public IUser Creator
		//{
		//    get
		//    {
		//        return this._Creator;
		//    }
		//    set
		//    {
		//        this._Creator = (IUser)OguUser.CreateWrapperObject(value);
		//    }
		//}
	}

	/// <summary>
	/// 表示简单对象的集合
	/// </summary>
	[Serializable]
	public class SCSimpleObjectCollection : EditableDataObjectCollectionBase<SCSimpleObject>
	{
		public IEnumerable<T> ToOguObjects<T>() where T : IOguObject
		{
			List<T> result = new List<T>();

			foreach (SCSimpleObject item in this)
			{
				IOguObject obj = item.ToOguObject();

				if (obj != null && obj is T)
					result.Add((T)obj);
			}

			return result;
		}

		/// <summary>
		/// 将Name拼接成FullPath
		/// </summary>
		/// <returns></returns>
		public string JoinNameToFullPath()
		{
			StringBuilder strB = new StringBuilder();

			foreach (SCSimpleObject so in this)
			{
				if (strB.Length > 0)
					strB.Append("\\");

				strB.Append(so.Name);
			}

			return strB.ToString();
		}

		/// <summary>
		/// 将此集合的元素次序反转
		/// </summary>
		public void Reverse()
		{
			this.InnerList.Reverse();
		}

		public string[] ToIDArray()
		{
			string[] result = new string[this.Count];
			for (int i = this.Count - 1; i >= 0; i--)
				result[i] = this[i].ID;

			return result;
		}

		public Dictionary<string, SCSimpleObject> ToDictionary()
		{
			Dictionary<string, SCSimpleObject> result = new Dictionary<string, SCSimpleObject>();

			foreach (SCSimpleObject obj in this)
			{
				if (result.ContainsKey(obj.ID) == false)
					result.Add(obj.ID, obj);
			}

			return result;
		}
	}
}
