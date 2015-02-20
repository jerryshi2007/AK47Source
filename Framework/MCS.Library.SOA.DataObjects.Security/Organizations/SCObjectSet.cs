using System;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Permissions;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 表示导出对象的集合
	/// </summary>
	public class SCObjectSet
	{
		#region 字段

		private SchemaObjectCollection objects = null;

		private DateTime timeContext = default(DateTime);

		private SCMemberRelationCollection membership = null;

		private SCRelationObjectCollection relations = null;

		private SCAclContainerCollection acls = null;

		private MCS.Library.SOA.DataObjects.Security.Conditions.SCConditionCollection conditions = null;

		#endregion

		#region 构造函数

		public SCObjectSet()
		{
			this.timeContext = DateTime.Now;
		}

		#endregion

		#region 属性

		public string Scope { get; set; }

		public SchemaObjectCollection Objects
		{
			get
			{
				if (this.objects == null)
					this.objects = new SchemaObjectCollection();
				return this.objects;
			}

			set
			{
				this.objects = value;
			}
		}

		public bool HasObjects
		{
			get
			{
				return this.objects != null && this.objects.Count > 0;
			}
		}

		public bool HasMembership
		{
			get
			{
				return this.membership != null && this.membership.Count > 0;
			}
		}

		public DateTime TimeContext
		{
			get { return this.timeContext; }
			set { this.timeContext = value; }
		}

		public SCMemberRelationCollection Membership
		{
			get
			{
				if (this.membership == null)
					this.membership = new SCMemberRelationCollection();
				return this.membership;
			}

			set
			{
				this.membership = value;
			}
		}

		public MCS.Library.SOA.DataObjects.Security.Conditions.SCConditionCollection Conditions
		{
			get
			{
				if (this.conditions == null)
					this.conditions = new MCS.Library.SOA.DataObjects.Security.Conditions.SCConditionCollection();
				return this.conditions;
			}

			set
			{
				this.conditions = value;
			}
		}

		public bool HasConditions
		{
			get
			{
				return this.conditions != null && this.conditions.Count > 0;
			}
		}

		public SCRelationObjectCollection Relations
		{
			get
			{
				if (this.relations == null)
					this.relations = new SCRelationObjectCollection();
				return this.relations;
			}

			set
			{
				this.relations = value;
			}
		}

		public bool HasRelations
		{
			get
			{
				return this.relations != null && this.relations.Count > 0;
			}
		}

		public Permissions.SCAclContainerCollection Acls
		{
			get
			{
				if (this.acls == null)
					this.acls = new SCAclContainerCollection();

				return this.acls;
			}

			set
			{
				this.acls = value;
			}
		}

		public bool HasAcls
		{
			get
			{
				return this.acls != null && this.acls.Count > 0;
			}
		}

		#endregion

		#region 公开的方法
		public void Write(System.IO.TextWriter output)
		{
			System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(output);

			writer.WriteStartDocument();

			writer.WriteStartElement("sc");

			writer.WriteStartElement("context");
			writer.WriteStartAttribute("timeContext");
			writer.WriteValue(this.timeContext);
			writer.WriteEndAttribute();

			if (this.Scope != null)
			{
				writer.WriteStartAttribute("scope");
				writer.WriteValue(this.Scope);
				writer.WriteEndAttribute();
			}

			writer.WriteEndElement();
			if (this.HasObjects)
			{
				writer.WriteStartElement("objects");

				foreach (var obj in this.objects)
				{
					writer.WriteRaw(obj.ToString());
				}

				writer.WriteEndElement();
			}

			if (this.HasRelations)
			{
				writer.WriteStartElement("relations");

				foreach (var obj in this.relations)
				{
					writer.WriteRaw(obj.ToString());
				}

				writer.WriteEndElement();
			}

			if (this.HasMembership)
			{
				writer.WriteStartElement("membership");

				foreach (var obj in this.membership)
				{
					writer.WriteRaw(obj.ToString());
				}

				writer.WriteEndElement();
			}

			if (this.HasConditions)
			{
				writer.WriteStartElement("conditions");

				foreach (var obj in this.conditions)
				{
					this.WriteCondition(writer, obj);
				}

				writer.WriteEndElement();
			}

			if (this.HasAcls)
			{
				writer.WriteStartElement("acls");

				foreach (var acl in this.acls)
				{
					this.WriteAcls(writer, acl);
				}

				writer.WriteEndElement();
			}

			writer.WriteEndElement();
			writer.Close();
		}

		public void Load(System.IO.TextReader input)
		{
			int count;

			var xmlDoc = new System.Xml.XmlDocument();
			xmlDoc.Load(input);
			SchemaObjectXmlImporter importer = new SchemaObjectXmlImporter();

			var nodeContext = xmlDoc.SelectSingleNode("/sc/context");
			if (nodeContext != null)
			{
				var attrTime = nodeContext.Attributes["timeContext"];
				if (attrTime != null && attrTime.Specified)
				{
					this.timeContext = System.Xml.XmlConvert.ToDateTime(attrTime.Value, System.Xml.XmlDateTimeSerializationMode.Local);
				}

				var scope = nodeContext.Attributes["scope"];
				if (scope != null && scope.Specified)
				{
					this.Scope = scope.Value;
				}
			}

			var nodeObjects = xmlDoc.SelectNodes("/sc/objects/Object");
			this.objects = null;
			count = nodeObjects.Count;

			if (count > 0)
			{
				this.objects = new SchemaObjectCollection();

				for (int i = 0; i < count; i++)
				{
					var xml = nodeObjects[i].OuterXml;
					var schemaType = nodeObjects[i].Attributes["SchemaType"].Value;
					this.objects.Add(importer.XmlToObject(xml, schemaType));
				}
			}

			nodeObjects = xmlDoc.SelectNodes("/sc/relations/Object");
			this.relations = null;
			count = nodeObjects.Count;

			if (count > 0)
			{
				this.relations = new SCRelationObjectCollection();

				for (int i = 0; i < count; i++)
				{
					var xml = nodeObjects[i].OuterXml;
					var schemaType = nodeObjects[i].Attributes["SchemaType"].Value;
					this.relations.Add((SCRelationObject)importer.XmlToObject(xml, schemaType));
				}
			}

			nodeObjects = xmlDoc.SelectNodes("/sc/membership/Object");
			this.membership = null;
			count = nodeObjects.Count;

			if (count > 0)
			{
				this.membership = new SCMemberRelationCollection();

				for (int i = 0; i < count; i++)
				{
					var xml = nodeObjects[i].OuterXml;
					var schemaType = nodeObjects[i].Attributes["SchemaType"].Value;
					this.membership.Add((SCSimpleRelationBase)importer.XmlToObject(xml, schemaType));
				}
			}

			nodeObjects = xmlDoc.SelectNodes("/sc/conditions/condition");
			this.conditions = null;
			count = nodeObjects.Count;

			if (count > 0)
			{
				this.conditions = new MCS.Library.SOA.DataObjects.Security.Conditions.SCConditionCollection();

				for (int i = 0; i < count; i++)
				{
					var xml = nodeObjects[i];

					var condition = LoadCondition(xml);

					this.conditions.Add(condition);
				}
			}

			nodeObjects = xmlDoc.SelectNodes("/sc/acls/acl");
			this.acls = null;
			count = nodeObjects.Count;

			if (count > 0)
			{
				this.acls = new SCAclContainerCollection();

				for (int i = 0; i < count; i++)
				{
					var xml = nodeObjects[i];

					var acl = LoadAcl(xml);

					this.acls.Add(acl);
				}
			}
		}

		#endregion

		#region 私有的方法

		private static Conditions.SCCondition LoadCondition(System.Xml.XmlNode xml)
		{
			var condition = new Conditions.SCCondition();

			condition.Condition = xml.Attributes["Expression"].Value;

			condition.Description = xml.Attributes["Description"].Value;

			condition.OwnerID = xml.Attributes["OwnerID"].Value;

			condition.SortID = System.Xml.XmlConvert.ToInt32(xml.Attributes["SortID"].Value);

			condition.Type = xml.Attributes["Type"].Value;

			condition.VersionEndTime = System.Xml.XmlConvert.ToDateTime(xml.Attributes["VersionEndTime"].Value, System.Xml.XmlDateTimeSerializationMode.Local);
			condition.VersionStartTime = System.Xml.XmlConvert.ToDateTime(xml.Attributes["VersionStartTime"].Value, System.Xml.XmlDateTimeSerializationMode.Local);
			return condition;
		}

		private static Permissions.SCAclItem LoadAcl(System.Xml.XmlNode xml)
		{
			var condition = new Permissions.SCAclItem();

			condition.ContainerID = xml.Attributes["ContainerID"].Value;

			condition.ContainerPermission = xml.Attributes["ContainerPermission"].Value;

			condition.ContainerSchemaType = xml.Attributes["ContainerSchemaType"].Value;

			condition.SortID = System.Xml.XmlConvert.ToInt32(xml.Attributes["SortID"].Value);

			condition.MemberID = xml.Attributes["MemberID"].Value;

			condition.MemberSchemaType = xml.Attributes["MemberSchemaType"].Value;

			condition.VersionEndTime = System.Xml.XmlConvert.ToDateTime(xml.Attributes["VersionEndTime"].Value, System.Xml.XmlDateTimeSerializationMode.Local);

			condition.VersionStartTime = System.Xml.XmlConvert.ToDateTime(xml.Attributes["VersionStartTime"].Value, System.Xml.XmlDateTimeSerializationMode.Local);
			return condition;
		}

		private void WriteCondition(System.Xml.XmlWriter writer, Conditions.SCCondition obj)
		{
			writer.WriteStartElement("condition");

			writer.WriteStartAttribute("Expression");
			writer.WriteValue(obj.Condition);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("Description");
			writer.WriteValue(obj.Description);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("OwnerID");
			writer.WriteValue(obj.OwnerID);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("SortID");
			writer.WriteValue(obj.SortID);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("Type");
			writer.WriteValue(obj.Type);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("VersionEndTime");
			writer.WriteValue(obj.VersionEndTime);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("VersionStartTime");
			writer.WriteValue(obj.VersionStartTime);
			writer.WriteEndAttribute();

			writer.WriteEndElement();
		}

		private void WriteAcls(System.Xml.XmlWriter writer, SCAclItem acl)
		{
			writer.WriteStartElement("acl");

			writer.WriteStartAttribute("ContainerID");
			writer.WriteValue(acl.ContainerID);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("ContainerPermission");
			writer.WriteValue(acl.ContainerPermission);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("ContainerSchemaType");
			writer.WriteValue(acl.ContainerSchemaType);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("MemberID");
			writer.WriteValue(acl.MemberID);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("MemberSchemaType");
			writer.WriteValue(acl.MemberSchemaType);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("SortID");
			writer.WriteValue(acl.SortID);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("VersionEndTime");
			writer.WriteValue(acl.VersionEndTime);
			writer.WriteEndAttribute();

			writer.WriteStartAttribute("VersionStartTime");
			writer.WriteValue(acl.VersionStartTime);
			writer.WriteEndAttribute();

			writer.WriteEndElement();
		}

		#endregion
	}
}