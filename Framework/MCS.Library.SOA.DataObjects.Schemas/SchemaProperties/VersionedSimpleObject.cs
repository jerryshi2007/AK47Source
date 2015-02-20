using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	[Serializable]
	public class VersionedSimpleObject
	{
		/// <summary>
		/// 在派生类中重写时，获取或设置表示对象的ID的字符串
		/// </summary>
		[ORFieldMapping("ID", PrimaryKey = true)]
		public virtual string ID
		{
			get;
			set;
		}

		/// <summary>
		/// 在派生类中重写时，获取或设置一个表示模式类型的字符串
		/// </summary>
		[ORFieldMapping("SchemaType")]
		public virtual string SchemaType
		{
			get;
			set;
		}

		/// <summary>
		/// 在派生类中重写时，获取或设置表示模式对象状态的<see cref="SchemaObjectStatus"/>值之一。
		/// </summary>
		[ORFieldMapping("Status")]
		public virtual SchemaObjectStatus Status
		{
			get;
			set;
		}

		/// <summary>
		/// 在派生类中重写时，获取或设置表示版本开始时间的<see cref="DateTime"/>
		/// </summary>
		[ORFieldMapping("VersionStartTime")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select)]
		public virtual DateTime VersionStartTime
		{
			get;
			set;
		}

		/// <summary>
		/// 在派生类中重写时，获取或设置表示版本结束时间的<see cref="DateTime"/>
		/// </summary>
		[ORFieldMapping("VersionEndTime")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select)]
		public virtual DateTime VersionEndTime
		{
			get;
			set;
		}

		/// <summary>
		/// 对象的一些标识型信息
		/// </summary>
		[NoMapping]
		public string Tag
		{
			get;
			set;
		}

		/// <summary>
		/// 获取或设置表示对象创建日期的<see cref="DateTime"/>
		/// </summary>
		[ORFieldMapping("CreateDate")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select)]
		public DateTime CreateDate
		{
			get;
			set;
		}

		private IUser _Creator = null;

		/// <summary>
		/// 获取或设置表示对象创建者的<see cref="IUser"/>的实现类的实例。
		/// </summary>
		[SubClassORFieldMapping("ID", "CreatorID")]
		[SubClassORFieldMapping("DisplayName", "CreatorName")]
		[SubClassType(typeof(OguUser))]
		[SubClassSqlBehavior(BindingFlags = ClauseBindingFlags.Select)]
		public IUser Creator
		{
			get
			{
				return this._Creator;
			}
			set
			{
				this._Creator = (IUser)OguUser.CreateWrapperObject(value);
			}
		}
	}
}
