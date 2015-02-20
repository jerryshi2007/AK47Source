using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	/// <summary>
	/// 带版本的Schema对象的基类
	/// </summary>
	[Serializable]
	public abstract class VersionedSchemaObjectBase : IVersionDataObject, IDefinitionProperties<SchemaPropertyValue>, IMemberAccessor, ISCStatusObject
	{
		private string _SchemaType = string.Empty;
		private SchemaObjectStatus _Status = SchemaObjectStatus.Normal;
		private DateTime _VersionStartTime = DateTime.MinValue;
		private DateTime _VersionEndTime = DateTime.MinValue;
		private string _Tag = string.Empty;

		[NonSerialized]
		private SchemaDefineBase _Schema = null;

		protected VersionedSchemaObjectBase()
		{
		}

		/// <summary>
		/// 使用指定的模式名初始化<see cref="SchemaObjectBase"/>成员
		/// </summary>
		/// <param name="schemaType">模式的名称</param>
		protected VersionedSchemaObjectBase(string schemaType)
		{
			schemaType.CheckStringIsNullOrEmpty("schemaType");

			this._SchemaType = schemaType;
		}

		/// <summary>
		/// 获取模式定义
		/// </summary>
		/// <value>表示模式定义的<see cref="SchemaDefine"/>对象</value>
		[NoMapping]
		[ScriptIgnore]
		public SchemaDefineBase Schema
		{
			get
			{
				if (this._Schema == null && this.SchemaType.IsNotEmpty())
					this._Schema = this.GetSchema(this.SchemaType);

				return this._Schema;
			}
		}

		public SchemaPropertyValueCollection Properties
		{
			get
			{
				return this.GetProperties();
			}
		}

		/// <summary>
		/// 在派生类中重写时，获取或设置ID
		/// </summary>
		[ORFieldMapping("ID", PrimaryKey = true)]
		public virtual string ID
		{
			get
			{
				return this.Properties.GetValue("ID", string.Empty);
			}
			set
			{
				string originalValue = this.Properties.GetValue("ID", string.Empty);

				this.Properties.SetValue("ID", value);

				if (originalValue != value)
				{
					OnIDChanged();
				}
			}
		}

		/// <summary>
		/// 在派生类中重写时，获取模式的类型
		/// </summary>
		[ORFieldMapping("SchemaType")]
		public virtual string SchemaType
		{
			get
			{
				return this._SchemaType;
			}
		}

		/// <summary>
		/// 在派生类中重写时，获取模式对象状态
		/// </summary>
		/// <value><see cref="SchemaObjectStatus"/>值之一</value>
		[ORFieldMapping("Status")]
		public virtual SchemaObjectStatus Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
			}
		}

		/// <summary>
		/// 在派生类中重写时，获取版本开始时间
		/// </summary>
		[ORFieldMapping("VersionStartTime")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select)]
		public virtual DateTime VersionStartTime
		{
			get
			{
				return this._VersionStartTime;
			}
			set
			{
				this._VersionStartTime = value;
			}
		}

		/// <summary>
		/// 在派生类中重写时，获取版本结束时间
		/// </summary>
		[ORFieldMapping("VersionEndTime")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select)]
		public virtual DateTime VersionEndTime
		{
			get
			{
				return this._VersionEndTime;
			}
			set
			{
				this._VersionEndTime = value;
			}
		}

		/// <summary>
		/// 对象的一些标识型信息
		/// </summary>
		[NoMapping]
		public virtual string Tag
		{
			get
			{
				return this._Tag;
			}
			set
			{
				this._Tag = value;
			}
		}

		/// <summary>
		/// 在派生类中重写时，获取创建日期
		/// </summary>
		[ORFieldMapping("CreateDate")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Insert, DefaultExpression = "GETDATE()")]
		public virtual DateTime CreateDate
		{
			get;
			set;
		}

		private IUser _Creator = null;

		/// <summary>
		/// 获取或设置<see cref="SchemaObjectBase"/>对象的创建者
		/// </summary>
		/// <value>表示创建者的<see cref="IUser"/>的实例或<see langword="null"/></value>
		[SubClassORFieldMapping("ID", "CreatorID")]
		[SubClassORFieldMapping("DisplayName", "CreatorName")]
		[SubClassType(typeof(OguUser))]
		[SubClassSqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Insert)]
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

		/// <summary>
		/// 得到属性集合
		/// </summary>
		/// <returns></returns>
		protected abstract SchemaPropertyValueCollection GetProperties();

		/// <summary>
		/// 得到Schema的定义信息
		/// </summary>
		/// <returns></returns>
		protected abstract SchemaDefineBase GetSchema(string schemaType);

		/// <summary>
		/// 当ID改变时
		/// </summary>
		protected virtual void OnIDChanged()
		{
		}

		SerializableEditableKeyedDataObjectCollectionBase<string, SchemaPropertyValue> IDefinitionProperties<SchemaPropertyValue>.Properties
		{
			get
			{
				return this.Properties;
			}
		}

		public virtual object GetValue(object instance, string memberName)
		{
			object result = null;

			switch (memberName)
			{
				case "ID":
					result = this.ID;
					break;
				case "VersionStartTime":
					result = this.VersionStartTime;
					break;
				case "VersionEndTime":
					result = this.VersionEndTime;
					break;
				case "Status":
					result = this.Status;
					break;
				case "SchemaType":
					result = this.SchemaType;
					break;
				case "CreateDate":
					result = this.CreateDate;
					break;
				case "Creator":
					result = this.Creator;
					break;
				default:
					{
						PropertyInfo pi = (PropertyInfo)TypePropertiesCacheQueue.Instance.GetPropertyInfo(instance.GetType(), memberName);

						if (pi != null && pi.CanRead)
							result = pi.GetValue(instance, null);

						break;
					}
			}

			return result;
		}

		public virtual void SetValue(object instance, string memberName, object newValue)
		{
			switch (memberName)
			{
				case "ID":
					this.ID = (string)newValue;
					break;
				case "VersionStartTime":
					this.VersionStartTime = (DateTime)newValue;
					break;
				case "VersionEndTime":
					this.VersionEndTime = (DateTime)newValue;
					break;
				case "Status":
					this.Status = (SchemaObjectStatus)newValue;
					break;
				case "SchemaType":
					this._SchemaType = (string)newValue;
					break;
				case "CreateDate":
					this.CreateDate = (DateTime)newValue;
					break;
				case "Creator":
					this.Creator = (IUser)newValue;
					break;
				default:
					{
						PropertyInfo pi = (PropertyInfo)TypePropertiesCacheQueue.Instance.GetPropertyInfo(instance.GetType(), memberName);

						if (pi != null && pi.CanWrite)
							pi.SetValue(instance, newValue, null);

						break;
					}
			}
		}

		/// <summary>
		/// 在派生类中重写时，将对象属性附加到<see cref="XElement"/>中。
		/// </summary>
		/// <param name="element">将在其中进行附加的<see cref="XElement"/></param>
		public virtual void ToXElement(XElement element)
		{
			element.NullCheck("element");

			element.SetAttributeValue("SchemaType", this.SchemaType);

			this.Properties.ForEach(pv => pv.ToXElement(element));
		}

		/// <summary>
		/// 获取表示当前对象的<see cref="string"/>
		/// </summary>
		/// <returns>一个XML的字符串表示</returns>
		public override string ToString()
		{
			XElement element = new XElement("Object");

			ToXElement(element);

			return element.ToString();
		}

		/// <summary>
		/// 在派生类中重写时，从<see cref="XElement"/>恢复属性数据
		/// </summary>
		/// <param name="element">要从其中恢复数据的<see cref="XElement"/></param>
		public virtual void FromXElement(XElement element)
		{
			element.NullCheck("element");

			this.Properties.ForEach(pv => pv.FromXElement(element));
		}

		/// <summary>
		/// 在派生类中重写时，从XML字符串恢复数据
		/// </summary>
		/// <param name="data">一个XML格式的字符串</param>
		public virtual void FromString(string data)
		{
			if (data.IsNotEmpty())
			{
				XElement element = XElement.Parse(data);

				FromXElement(element);
			}
		}

		/// <summary>
		/// 获取用于全文索引的字符串
		/// </summary>
		/// <returns></returns>
		public string ToFullTextString()
		{
			StringBuilder searchContent = new StringBuilder(256);

			Schema.Properties.ForEach(pd =>
				pd.SnapshotMode.IfInFullTextIndex(() => searchContent.AppendWithSplitChars(this.Properties[pd.Name].StringValue)));

			return searchContent.ToString();
		}
	}
}
