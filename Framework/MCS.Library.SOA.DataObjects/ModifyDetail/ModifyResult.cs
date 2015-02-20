#region
// -------------------------------------------------
// Assembly	��	HB.DataObjects
// FileName	��	CompareResult.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    2008-04-21		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Properties;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	#region IModifyResult

	public interface IModifyResult
	{
		XmlNode ToXmlNode(XmlDocument xmlDoc);

		void FromXmlNode(XmlNode node);

		/// <summary>
		/// ����
		/// </summary>
		string Description
		{
			get;
			set;
		}

		/// <summary>
		/// ��������
		/// </summary>
		string PropertyName
		{
			get;
		}
	}

	#endregion

	#region ModifyResultBase
	public abstract class ModifyResultBase : IModifyResult
	{
		#region IModifyResult Members

		public virtual XmlNode ToXmlNode(XmlDocument xmlDoc)
		{
			XmlNode node = xmlDoc.CreateElement("Item");

			XmlHelper.AppendAttr(node, "propertyName", this.PropertyName);
			XmlHelper.AppendAttr(node, "type", this.GetType().Name);
			XmlHelper.AppendNotDefaultAttr<string>(node, "description", this.Description);

			return node;
		}

		public virtual void FromXmlNode(XmlNode node)
		{
			this.PropertyName = XmlHelper.GetAttributeText(node, "propertyName");
			this.Description = XmlHelper.GetAttributeText(node, "description");
		}

		public virtual string Description
		{
			get;
			set;
		}

		public virtual string PropertyName
		{
			get;
			set;
		}

		#endregion
	}

	#endregion

	#region ModifyResultHelper
	public static class ModifyResultHelper
	{
		private static Dictionary<string, Type> typeDict = new Dictionary<string, Type>();

		static ModifyResultHelper()
		{
			typeDict.Add("SimpleModifyResult", typeof(SimpleModifyResult));
			typeDict.Add("ListModifyResult", typeof(ListModifyResult));
		}

		public static IModifyResult GetModifyResultByType(string type)
		{
			Type resultType = null;

			ExceptionHelper.FalseThrow(typeDict.TryGetValue(type, out resultType), 
				"Can not find ModifyResult type: {0}", type);

			return (IModifyResult)TypeCreator.CreateInstance(resultType);
		}
	}
	#endregion ModifyResultHelper
	#region SimpleModifyResult

	public class SimpleModifyResult : ModifyResultBase
	{
		#region ����

		/// <summary>
		/// ����
		/// </summary>
		private Type objectType;
		
		/// <summary>
		/// չʾ�޸���Ϣʱ������
		/// </summary>
		private int sortID;
		
		/// <summary>
		/// �����޸�ǰ��ֵ
		/// </summary>
		private object oldValue;

		/// <summary>
		/// �����޸ĺ��ֵ
		/// </summary>
		private object newValue;
		
		/// <summary>
		/// ����
		/// </summary>
		public Type ObjectType
		{
			get { return this.objectType; }
			set { this.objectType = value; }
		}

		/// <summary>
		/// չʾ�޸���Ϣʱ������
		/// </summary>
		public int SortID
		{
			get { return this.sortID; }
			set { this.sortID = value; }
		}

		/// <summary>
		/// �����޸�ǰ��ֵ
		/// </summary>
		public object OldValue
		{
			get { return this.oldValue; }
			set { this.oldValue = value; }
		}

		/// <summary>
		/// �����޸ĺ��ֵ
		/// </summary>
		public object NewValue
		{
			get { return this.newValue; }
			set { this.newValue = value; }
		}

		#endregion

		/// <summary>
		/// ת��ΪXML�ڵ�
		/// </summary>
		/// <param name="xmlDoc">XML�ĵ�����</param>
		/// <returns>XML�ڵ�</returns>
		public override XmlNode ToXmlNode(XmlDocument xmlDoc)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(xmlDoc == null, "xmlDoc");

			XmlNode node = base.ToXmlNode(xmlDoc);

			XmlHelper.AppendAttr<int>(node, "sortID", this.sortID);
			XmlHelper.AppendAttr(node, "objectType", this.objectType.AssemblyQualifiedName);
			XmlHelper.AppendAttr(node, "oldValue", this.oldValue);
			XmlHelper.AppendAttr(node, "newValue", this.newValue);

			return node;
		}

		/// <summary>
		/// ��XML��ԭ����
		/// </summary>
		/// <param name="node">XML�ڵ�</param>
		public override void FromXmlNode(XmlNode node)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(node == null, "node");

			base.FromXmlNode(node);

			this.sortID = XmlHelper.GetAttributeValue<int>(node, "sortID", 0);

			this.objectType = typeof(object);

			string typeName = XmlHelper.GetAttributeValue<string>(node, "objectType", string.Empty);

			if (!string.IsNullOrEmpty(typeName))
				this.objectType = TypeCreator.GetTypeInfo(typeName);

			this.oldValue = XmlHelper.GetAttributeValue<string>(node, "oldValue", string.Empty);
			this.newValue = XmlHelper.GetAttributeValue<string>(node, "newValue", string.Empty);
		}
	}

	#endregion

	#region ListModifyResult

	public class ListModifyResult : ModifyResultBase
	{
		private ModifyResultCollection added = new ModifyResultCollection();
		private ModifyResultCollection updated = new ModifyResultCollection();
		private ModifyResultCollection deleted = new ModifyResultCollection();

		#region IModifyResult Members

		public override XmlNode ToXmlNode(XmlDocument xmlDoc)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(xmlDoc == null, "xmlDoc");

			XmlNode node = base.ToXmlNode(xmlDoc);

			XmlNode addedNode = XmlHelper.AppendNode(node, "Added");
			this.Added.ForEach(mr => addedNode.AppendChild(mr.ToXmlNode(xmlDoc)));

			XmlNode updatedNode = XmlHelper.AppendNode(node, "Updated");
			this.Updated.ForEach(mr => updatedNode.AppendChild(mr.ToXmlNode(xmlDoc)));

			XmlNode deletedNode = XmlHelper.AppendNode(node, "Deleted");
			this.Deleted.ForEach(mr => deletedNode.AppendChild(mr.ToXmlNode(xmlDoc)));

			return node;
		}

		public override void FromXmlNode(XmlNode node)
		{
			base.FromXmlNode(node);

			XmlNodeListToCollection(node.SelectNodes("Added/Item"), this.Added);
			XmlNodeListToCollection(node.SelectNodes("Updated/Item"), this.Updated);
			XmlNodeListToCollection(node.SelectNodes("Deleted/Item"), this.Deleted);
		}

		private static void XmlNodeListToCollection(XmlNodeList nodeList, ModifyResultCollection collection)
		{
			foreach (XmlNode node in nodeList)
			{
				IModifyResult mr = ModifyResultHelper.GetModifyResultByType(XmlHelper.GetAttributeText(node, "type"));

				mr.FromXmlNode(node);
				collection.Add(mr);
			}
		}

		public ModifyResultCollection Added
		{
			get { return this.added; }
		}

		public ModifyResultCollection Updated
		{
			get { return this.updated; }
		}

		public ModifyResultCollection Deleted
		{
			get { return this.deleted; }
		}
		#endregion
	}

	#endregion
	#region ComplexModifyResult

	/// <summary>
	/// ���Ӷ���Ƚϵ�����
	/// </summary>
	public enum ComplexModifyResultType
	{
		/// <summary>
		/// �ַ�����
		/// </summary>
		DeltaDistribution,
		/// <summary>
		/// �ļ�����
		/// </summary>
		DeltaResourceMapping
	}

	/// <summary>
	/// ���Ӷ���ȽϵĽ��
	/// </summary>
	public class ComplexModifyResult : DeltaDataCollectionBase<List<string>>, IModifyResult
	{
		private const string DeltaDistributions = "deltaDistributions";
		private const string DeltaResourceMappings = "deltaResourceMappings";
		private string description = string.Empty;

		/// <summary>
		/// ��������
		/// </summary>
		public string PropertyName
		{
			get
			{
				return this.ModifyResultType == ComplexModifyResultType.DeltaDistribution ? "DeltaDistribution" : "DeltaResourceMapping";
			}
		}

		/// <summary>
		/// ����
		/// </summary>
		public string Description
		{
			get
			{
				if (string.IsNullOrEmpty(this.description))
					return this.ModifyResultType == ComplexModifyResultType.DeltaDistribution ? "�ַ�����" : "�����ļ�";
				else
					return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		protected override DeltaDataCollectionBase CreateNewInstance()
		{
			return new ComplexModifyResult();
		}

		private ComplexModifyResultType modifyResultType = ComplexModifyResultType.DeltaDistribution;

		/// <summary>
		/// �ȽϽ��������
		/// </summary>
		public ComplexModifyResultType ModifyResultType
		{
			get { return this.modifyResultType; }
			set { this.modifyResultType = value; }
		}

		/// <summary>
		/// ת��ΪXML
		/// </summary>
		/// <param name="xmlDoc">XML �ĵ�����</param>
		/// <returns>XML�ڵ�</returns>
		public XmlNode ToXmlNode(XmlDocument xmlDoc)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(xmlDoc == null, "xmlDoc");

			if (this.IsEmpty())
				return null;

			XmlNode root;

			if (this.modifyResultType == ComplexModifyResultType.DeltaDistribution)
				root = xmlDoc.CreateElement(DeltaDistributions);
			else
				root = xmlDoc.CreateElement(DeltaResourceMappings);

			XmlNode insertedNode = XmlHelper.AppendNode<string>(root, "inserted", string.Empty);

			foreach (string str in this.Inserted)
				XmlHelper.AppendNode<string>(insertedNode, "name", str);

			XmlNode updatedNode = XmlHelper.AppendNode<string>(root, "updated", string.Empty);

			foreach (string str in this.Updated)
				XmlHelper.AppendNode<string>(updatedNode, "name", str);

			XmlNode deletedNode = XmlHelper.AppendNode<string>(root, "deleted", string.Empty);

			foreach (string str in this.Deleted)
				XmlHelper.AppendNode<string>(deletedNode, "name", str);

			return root;
		}

		/// <summary>
		/// ��XML��ԭ
		/// </summary>
		/// <param name="node">xml�ڵ�</param>
		public void FromXmlNode(XmlNode node)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(node == null, "node");

			if (node.Name == DeltaDistributions)
				this.ModifyResultType = ComplexModifyResultType.DeltaDistribution;
			else
				this.ModifyResultType = ComplexModifyResultType.DeltaResourceMapping;

			foreach (XmlNode listNode in node.ChildNodes)
			{
				if (listNode.Name == "inserted")
					this.Inserted = GenerateListFromXmlNode(listNode);

				if (listNode.Name == "updated")
					this.Updated = GenerateListFromXmlNode(listNode);

				if (listNode.Name == "deleted")
					this.Deleted = GenerateListFromXmlNode(listNode);
			}
		}

		private List<string> GenerateListFromXmlNode(XmlNode node)
		{
			List<string> list = new List<string>();

			foreach (XmlNode strNode in node.ChildNodes)
			{
				if (strNode.NodeType == XmlNodeType.Element)
					list.Add(strNode.FirstChild.Value);
			}

			return list;
		}

		public override bool IsEmpty()
		{
			return this.Inserted.Count == 0 && this.Deleted.Count == 0 && this.Updated.Count == 0;
		}
	}

	#endregion

	#region MaterialModifyResult
	public class MaterialModifyResult : DeltaDataCollectionBase<MaterialModifyObjectCollection>, IModifyResult
	{
		private const string DeltaMaterials = "deltaMaterials";
		private string description = "��ز���";

		/// <summary>
		/// ��������
		/// </summary>
		public string PropertyName
		{
			get
			{
				return "DeltaMaterial";
			}
		}

		/// <summary>
		/// ����
		/// </summary>
		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		protected override DeltaDataCollectionBase CreateNewInstance()
		{
			return new DeltaMaterialList();
		}

		/// <summary>
		/// ת��ΪXML�ڵ�
		/// </summary>
		/// <param name="xmlDoc">XML�ĵ�����</param>
		/// <returns>XML�ڵ�</returns>
		public XmlNode ToXmlNode(XmlDocument xmlDoc)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(xmlDoc == null, "xmlDoc");

			if (this.IsEmpty())
				return null;

			XmlNode root = xmlDoc.CreateElement(DeltaMaterials);

			XmlHelper.AppendAttr(root, "description", this.description);

			XmlNode insertedNode = XmlHelper.AppendNode<string>(root, "inserted", string.Empty);

			foreach (MaterialModifyObject m in this.Inserted)
				this.GenerateXmlNodeForMaterial(insertedNode, m);

			XmlNode updatedNode = XmlHelper.AppendNode<string>(root, "updated", string.Empty);

			foreach (MaterialModifyObject m in this.Updated)
				this.GenerateXmlNodeForMaterial(updatedNode, m);

			XmlNode deletedNode = XmlHelper.AppendNode<string>(root, "deleted", string.Empty);

			foreach (MaterialModifyObject m in this.Deleted)
				this.GenerateXmlNodeForMaterial(deletedNode, m);

			return root;
		}

		private void GenerateXmlNodeForMaterial(XmlNode root, MaterialModifyObject m)
		{
			XmlNode materialNode = XmlHelper.AppendNode<string>(root, "material", string.Empty);

			XmlHelper.AppendAttr(materialNode, "id", m.ID);
			XmlHelper.AppendAttr(materialNode, "relativeFilePath", m.RelativeFilePath);
			XmlHelper.AppendAttr(materialNode, "originalName", m.OriginalName);
			XmlHelper.AppendAttr(materialNode, "title", m.Title);
			XmlHelper.AppendAttr(materialNode, "sortID", m.SortID);
			XmlHelper.AppendAttr(materialNode, "wfActivityID", m.WfActivityID);
			XmlHelper.AppendAttr(materialNode, "wfProcessID", m.WfProcessID);
			XmlHelper.AppendAttr(materialNode, "creatorFullPath", m.CreatorFullPath);
		}

		/// <summary>
		/// ��XML��ԭ����
		/// </summary>
		/// <param name="root">XML���ڵ�</param>
		public void FromXmlNode(XmlNode root)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(root == null, "root");

			this.description = XmlHelper.GetAttributeValue<string>(root, "description", string.Empty);

			foreach (XmlNode node in root.ChildNodes)
			{
				if (node.Name == "inserted")
					this.Inserted = GenerateMaterialModifyObjectCollectionFromXmlNode(node);

				if (node.Name == "updated")
					this.Updated = GenerateMaterialModifyObjectCollectionFromXmlNode(node);

				if (node.Name == "deleted")
					this.Deleted = GenerateMaterialModifyObjectCollectionFromXmlNode(node);
			}
		}

		private MaterialModifyObjectCollection GenerateMaterialModifyObjectCollectionFromXmlNode(XmlNode node)
		{
			MaterialModifyObjectCollection materialModifyObjects = new MaterialModifyObjectCollection();

			foreach (XmlNode materialNode in node.ChildNodes)
			{
				if (materialNode.NodeType == XmlNodeType.Element)
					materialModifyObjects.Add(this.GenerateMaterialModifyObjectFromXmlNode(materialNode));
			}

			return materialModifyObjects;
		}

		private MaterialModifyObject GenerateMaterialModifyObjectFromXmlNode(XmlNode node)
		{
			MaterialModifyObject materialModifyObject = new MaterialModifyObject();

			materialModifyObject.ID = XmlHelper.GetAttributeValue<string>(node, "id", string.Empty);
			materialModifyObject.RelativeFilePath = XmlHelper.GetAttributeValue<string>(node, "relativeFilePath", string.Empty);
			materialModifyObject.OriginalName = XmlHelper.GetAttributeValue<string>(node, "originalName", string.Empty);
			materialModifyObject.Title = XmlHelper.GetAttributeValue<string>(node, "title", string.Empty);
			materialModifyObject.SortID = XmlHelper.GetAttributeValue<int>(node, "sortID", 0);
			materialModifyObject.WfActivityID = XmlHelper.GetAttributeValue<string>(node, "wfActivityID", string.Empty);
			materialModifyObject.WfProcessID = XmlHelper.GetAttributeValue<string>(node, "wfProcessID", string.Empty);
			materialModifyObject.CreatorFullPath = XmlHelper.GetAttributeValue<string>(node, "creatorFullPath", string.Empty);

			return materialModifyObject;
		}

		/// <summary>
		/// ת��ΪDeltaMaterialList
		/// </summary>
		/// <returns>DeltaMaterial����</returns>
		public DeltaMaterialList ConvertToDelatMaterialList()
		{
			DeltaMaterialList deltaMaterials = new DeltaMaterialList();

			if (this.Inserted != null)
				deltaMaterials.Inserted = this.Inserted.ConvertToMaterialList();

			if (this.Updated != null)
				deltaMaterials.Updated = this.Updated.ConvertToMaterialList();

			if (this.Deleted != null)
				deltaMaterials.Deleted = this.Deleted.ConvertToMaterialList();

			return deltaMaterials;
		}

		public override bool IsEmpty()
		{
			return this.Inserted.Count == 0 && this.Deleted.Count == 0 && this.Updated.Count == 0;
		}
	}

	/// <summary>
	/// ��������ıȽϽ��
	/// </summary>
	public class MaterialModifyObject
	{
		#region ����

		string id = string.Empty;
		/// <summary>
		/// ID
		/// </summary>
		public string ID
		{
			get { return id; }
			set { id = value; }
		}

		string relativeFilePath = string.Empty;
		/// <summary>
		/// ���·��
		/// </summary>
		public string RelativeFilePath
		{
			get { return this.relativeFilePath; }
			set { this.relativeFilePath = value; }
		}

		string originalName = string.Empty;
		/// <summary>
		/// ԭʼ�ļ���
		/// </summary>
		public string OriginalName
		{
			get { return this.originalName; }
			set { this.originalName = value; }
		}

		string title = string.Empty;
		/// <summary>
		/// ����
		/// </summary>
		public string Title
		{
			get { return this.title; }
			set { this.title = value; }
		}

		int sortID = 0;
		/// <summary>
		/// �����
		/// </summary>
		public int SortID
		{
			get { return this.sortID; }
			set { this.sortID = value; }
		}

		string wfActivityID = string.Empty;
		/// <summary>
		/// �������ڵ�ID
		/// </summary>
		public string WfActivityID
		{
			get { return this.wfActivityID; }
			set { this.wfActivityID = value; }
		}

		string wfProcessID = string.Empty;
		/// <summary>
		/// ����������ID
		/// </summary>
		public string WfProcessID
		{
			get { return this.wfProcessID; }
			set { this.wfProcessID = value; }
		}

		string creatorFullPath = string.Empty;
		/// <summary>
		/// ������fullPath
		/// </summary>
		public string CreatorFullPath
		{
			get { return this.creatorFullPath; }
			set { this.creatorFullPath = value; }
		}

		#endregion

		/// <summary>
		/// ת��Ϊ��������
		/// </summary>
		/// <returns>��������</returns>
		public Material ConvertToMaterial()
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(this == null, "MaterialModifyObject");

			Material m = new Material();

			m.ID = this.id;
			m.RelativeFilePath = this.relativeFilePath;
			m.OriginalName = this.originalName;
			m.Title = this.title;
			m.SortID = this.sortID;
			m.WfActivityID = this.wfActivityID;
			m.WfProcessID = this.wfProcessID;
			m.Creator = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.FullPath, this.creatorFullPath)[0];

			return m;
		}
	}

	public class MaterialModifyObjectCollection : CollectionBase
	{
		#region ���캯��

		public MaterialModifyObjectCollection()
		{

		}

		#endregion

		#region ���ϲ���

		public MaterialModifyObject this[int index]
		{
			get
			{
				return (MaterialModifyObject)InnerList[index];
			}
			set
			{
				InnerList[index] = value;
			}
		}

		public int IndexOf(MaterialModifyObject materialModifyObject)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(materialModifyObject != null, "materialModifyObject");

			return List.IndexOf(materialModifyObject);
		}

		public int Add(MaterialModifyObject materialModifyObject)
		{
			return List.Add(materialModifyObject);
		}

		public void Insert(int index, MaterialModifyObject materialModifyObject)
		{
			List.Insert(index, materialModifyObject);
		}

		public void Remove(MaterialModifyObject materialModifyObject)
		{
			List.Remove(materialModifyObject);
		}

		public bool Contains(MaterialModifyObject materialModifyObject)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(materialModifyObject != null, "materialModifyObject");

			return List.Contains(materialModifyObject);
		}

		#endregion

		#region ���ػ���ķ���

		protected override void OnValidate(object value)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");
		}

		#endregion

		/// <summary>
		/// ת��Ϊ��������
		/// </summary>
		/// <returns>��������</returns>
		public MaterialList ConvertToMaterialList()
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(this == null, "MaterialModifyObjectCollection");

			MaterialList materials = new MaterialList();

			foreach (MaterialModifyObject materialModifyObject in this)
			{
				if (materialModifyObject != null)
				{
					Material m = materialModifyObject.ConvertToMaterial();
					materials.Add(m);
				}
			}

			return materials;
		}
	}

	#endregion

	#region ModifyResultCollection

	public class ModifyResultCollection : DataObjectCollectionBase<IModifyResult>
	{
		#region ���캯��

		public ModifyResultCollection()
		{

		}

		#endregion

		#region ���ϲ���

		public IModifyResult this[int index]
		{
			get
			{
				return (IModifyResult)InnerList[index];
			}
			set
			{
				InnerList[index] = value;
			}
		}

		public int Add(IModifyResult resultBase)
		{
			return List.Add(resultBase);
		}

		public void Insert(int index, IModifyResult resultBase)
		{
			List.Insert(index, resultBase);
		}

		public void Remove(IModifyResult resultBase)
		{
			List.Remove(resultBase);
		}

		#endregion

		#region ���ػ���ķ���

		protected override void OnValidate(object value)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");
		}

		#endregion

		/// <summary>
		/// ת��ΪXML
		/// </summary>
		/// <returns>XML��</returns>
		public XmlDocument ToXml()
		{
			XmlDocument doc = XmlHelper.CreateDomDocument("<ModifyResult/>");

			XmlNode root = doc.DocumentElement;

			foreach (IModifyResult modifyResult in this)
			{
				XmlNode node = modifyResult.ToXmlNode(doc);

				if (node != null)
					root.AppendChild(node);
			}

			return doc;
		}

		public void FromXml(XmlDocument doc)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(doc != null, "doc");

			this.Clear();

			XmlNodeList itemNodes = doc.DocumentElement.SelectNodes("Item");

			FromXmlNodeList(itemNodes);
		}

		private void FromXmlNodeList(XmlNodeList nodes)
		{
			foreach (XmlNode node in nodes)
			{
				IModifyResult mr = ModifyResultHelper.GetModifyResultByType(XmlHelper.GetAttributeText(node, "type"));

				mr.FromXmlNode(node);
				this.Add(mr);
			}
		}
	}

	#endregion
}