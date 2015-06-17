using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[ORTableMapping("WF.GENERIC_OPINIONS")]
	[XmlRootMapping("GenericOpinion", OnlyMapMarkedProperties = false)]
    [TenantRelativeObject]
	public class GenericOpinion
	{
		private string id = String.Empty;
		private string resourceID = String.Empty;
		private string content = String.Empty;
		private IUser issuePerson = null;
		private DateTime issueDatetime;
		protected IUser appendPerson = null;
		private DateTime appendDatetime;
		private string processID = String.Empty;
		private string activityID = String.Empty;
		private string levelName = String.Empty;
		private string levelDesp = String.Empty;
		private string opinionType = string.Empty;
		private ApprovalScore evalue;
		private ApprovalResult result;
		private IDictionary<string, object> extraData = null;
		private string rawExtData = string.Empty;

		public GenericOpinion()
		{
		}

		/// <summary>
		/// 关键字
		/// </summary>
		[ORFieldMapping("ID", PrimaryKey = true)]
		public string ID
		{
			get { return this.id; }
			set { this.id = value; }
		}

		/// <summary>
		/// 对应的文件ID
		/// </summary>
		[ORFieldMapping("RESOURCE_ID")]
		public string ResourceID
		{
			get { return this.resourceID; }
			set { this.resourceID = value; }
		}

		/// <summary>
		/// 意见的内容
		/// </summary>
		[ORFieldMapping("CONTENT")]
		public string Content
		{
			get { return this.content; }
			set { this.content = value; }
		}

		/// <summary>
		/// 发布人对象
		/// </summary>
		/// <remark>
		/// 发布人对象
		/// </remark>
		[SubClassORFieldMapping("ID", "ISSUE_PERSON_ID")]
		[SubClassORFieldMapping("DisplayName", "ISSUE_PERSON_NAME")]
		[SubClassORFieldMapping("Levels", "ISSUE_PERSON_LEVEL")]
		[SubClassType(typeof(OguUser))]
		public IUser IssuePerson
		{
			get
			{
				return this.issuePerson;
			}
			set
			{
				this.issuePerson = (IUser)OguBase.CreateWrapperObject(value);
			}
		}

		/// <summary>
		/// 填写人对象
		/// </summary>
		/// <remark>
		/// 填写人对象
		/// </remark>
		[SubClassORFieldMapping("ID", "APPEND_PERSON_ID")]
		[SubClassORFieldMapping("DisplayName", "APPEND_PERSON_NAME")]
		[SubClassType(typeof(OguUser))]
		public IUser AppendPerson
		{
			get
			{
				return this.appendPerson;
			}
			set
			{
				this.appendPerson = (IUser)OguBase.CreateWrapperObject(value);
			}
		}

		/// <summary>
		/// 创建意见的时间
		/// </summary>
		[ORFieldMapping("ISSUE_DATETIME")]
		[SqlBehavior(ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
		public DateTime IssueDatetime
		{
			get { return this.issueDatetime; }
			set { this.issueDatetime = value; }
		}


		/// <summary>
		/// 修改意见的时间
		/// </summary>
		[ORFieldMapping("APPEND_DATETIME")]
		[SqlBehavior(ClauseBindingFlags.All, DefaultExpression = "getdate()")]
		public DateTime AppendDatetime
		{
			get { return this.appendDatetime; }
			set { this.appendDatetime = value; }
		}

		/// <summary>
		/// 对应流程的ID
		/// </summary>
		[ORFieldMapping("PROCESS_ID")]
		public string ProcessID
		{
			get { return this.processID; }
			set { this.processID = value; }
		}

		/// <summary>
		/// 对应流程环节的ID
		/// </summary>
		[ORFieldMapping("ACTIVITY_ID")]
		public string ActivityID
		{
			get { return this.activityID; }
			set { this.activityID = value; }
		}

		/// <summary>
		/// 对应业务环节的名称
		/// </summary>
		[ORFieldMapping("LEVEL_NAME")]
		public string LevelName
		{
			get { return this.levelName; }
			set { this.levelName = value; }
		}

		/// <summary>
		/// 对应业务环节的描述
		/// </summary>
		[ORFieldMapping("LEVEL_DESP")]
		public string LevelDesp
		{
			get { return this.levelDesp; }
			set { this.levelDesp = value; }
		}

		/// <summary>
		/// 意见的类型
		/// </summary>
		[ORFieldMapping("OPINION_TYPE")]
		public string OpinionType
		{
			get { return this.opinionType; }
			set { this.opinionType = value; }
		}

		/// <summary>
		/// 打分和评价
		/// </summary>
		[ORFieldMapping("EVALUE")]
		public ApprovalScore Evalue
		{
			get { return this.evalue; }
			set { this.evalue = value; }
		}

		/// <summary>
		/// 结果
		/// </summary>
		[ORFieldMapping("RESULT")]
		public ApprovalResult Result
		{
			get { return this.result; }
			set { this.result = value; }
		}

		[NoMapping]
		public IDictionary<string, object> ExtData
		{
			get
			{
				if (this.extraData == null)
					this.extraData = LoadExtDataFromRawExtData(RawExtData);

				return this.extraData;
			}
		}

		/// <summary>
		/// 扩展数据
		/// </summary>
		[ORFieldMapping("EXT_DATA")]
		public string RawExtData
		{
			get
			{
				return this.rawExtData;
			}
			set
			{
				this.rawExtData = value;
			}
		}

		internal static string ConvertExtraDataToXmlString(IDictionary<string, object> data)
		{
			XmlDocument xmlDoc = XmlHelper.CreateDomDocument("<ExtraData/>");

			foreach (KeyValuePair<string, object> kp in data)
			{
				XmlNode nodeItem = XmlHelper.AppendNode(xmlDoc.DocumentElement, "Item");

				XmlHelper.AppendAttr(nodeItem, "key", kp.Key);
				XmlHelper.AppendAttr(nodeItem, "value", kp.Value);

				if (kp.Value != null)
					XmlHelper.AppendAttr(nodeItem, "type", kp.Value.GetType().AssemblyQualifiedName);
			}

			return xmlDoc.OuterXml;
		}

		private static IDictionary<string, object> LoadExtDataFromRawExtData(string raw)
		{
			IDictionary<string, object> result = new Dictionary<string, object>();

			if (string.IsNullOrEmpty(raw) == false)
			{
				XmlDocument xmlDoc = XmlHelper.CreateDomDocument(raw);

				foreach (XmlNode elem in xmlDoc.DocumentElement)
				{
					string typeDesc = XmlHelper.GetAttributeValue(elem, "type", "System.String");

					object defaultValue = TypeCreator.GetTypeDefaultValue(TypeCreator.GetTypeInfo(typeDesc));

					result.Add(XmlHelper.GetAttributeText(elem, "key"),
						XmlHelper.GetAttributeValue(elem, "value", defaultValue));

				}
			}

			return result;
		}
	}

	/// <summary>
	/// 通用意见集合
	/// </summary>
	[Serializable]
	public class GenericOpinionCollection : EditableDataObjectCollectionBase<GenericOpinion>
	{
		internal void LoadFromDataView(DataView dv)
		{
			this.Clear();

			ORMapping.DataViewToCollection(this, dv);
		}

		public GenericOpinionCollection GetOpinions(string levelName)
		{
			GenericOpinionCollection opinions = new GenericOpinionCollection();

			foreach (GenericOpinion o in this)
				if (o.LevelName == levelName)
					opinions.Add(o);

			return opinions;
		}
	}
}
