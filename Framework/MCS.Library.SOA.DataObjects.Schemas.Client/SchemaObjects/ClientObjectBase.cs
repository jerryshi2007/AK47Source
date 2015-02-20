using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MCS.Library.SOA.DataObjects.Schemas.Client
{
	public abstract class ClientObjectBase
	{
		private ClientSchemaObjectStatus _Status = ClientSchemaObjectStatus.Normal;
		private DateTime _VersionStartTime = DateTime.MinValue;
		private DateTime _VersionEndTime = DateTime.MinValue;

		protected ClientObjectBase()
		{
		}

		/// <summary>
		/// 在派生类中重写时，获取模式对象状态
		/// </summary>
		/// <value><see cref="ClientSchemaObjectStatus"/>值之一</value>
		[XmlAttribute]
		public virtual ClientSchemaObjectStatus Status
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
		[XmlAttribute]
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
		[XmlAttribute]
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
	}
}
