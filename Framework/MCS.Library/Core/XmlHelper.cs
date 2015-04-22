#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	XmlHelper.cs
// Remark	��	�������ṩ��XML������ݴ������غ�����������þ�̬��������ʽ�ṩ���������ݶ�����XML����֮�������ת���� 
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections.Generic;

using MCS.Library.Properties;
using System.Reflection;
using System.Collections;
using System.Xml.Linq;

namespace MCS.Library.Core
{
    /// <summary>
    /// �������ṩ��XML������ݴ������غ�����������������з���������XML�ĵ����������أ�Ŀ�ľ���Ϊ�˷������Ա�ĳ��򿪷����ṩ��Ч����ݵĹ�
    /// �ú���
    /// </summary>
    /// <remarks>
    /// �������ṩ��XML������ݴ������غ�����������þ�̬��������ʽ�ṩ���������ݶ�����XML����֮�������ת����
    /// <list type="bullet">
    /// <item>���ݼ���XML�ĵ������ת������DataSet, DataReader�ȵ�����ת����XML�ĵ�����ķ���</item>
    /// <item>XML�ĵ�����Ľڵ�����ӽڵ�����ԵĲ�ͬ����</item>
    /// <item>XML�ĵ������нڵ��滻�����ڡ������Ĵ���</item>
    /// </list>
    /// </remarks>
    public static class XmlHelper
    {
        /// <summary>
        /// ��ö��Xml NodeList��ʱ��ص��ĺ����ӿ�
        /// </summary>
        /// <param name="nodeRoot">Xml�ĵ��Ľڵ�</param>
        /// <param name="oParam">��Xml�ĵ��Ľڵ������Ĳ���</param>
        /// <remarks>ͨ���ص�������ʵ�ָ���xml�ڵ����Ҫ�����в�ͬ�Ĳ������ܡ�
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" lang="cs" title="ö��Xml NodeListʱ��Ļص������ӿ�" />
        /// </remarks>
        public delegate void DoNodeList(XmlNode nodeRoot, object oParam);

        #region ����Exception����Xml�ĵ�����

        /// <summary>
        /// ���쳣��������Ϊһ��XML�ĵ������൱�ڽ�Exception���л�ΪXml
        /// </summary>
        /// <param name="ex">�쳣����</param>
        /// <returns>��ʾ�쳣�����Xml�ĵ�</returns>
        /// <remarks>�൱�ڽ�Exception�������л�ΪXml�ĵ�����������Exception����������û�
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region ="GetExceptionXmlTest" lang="cs" title="��Exception���л�ΪXml" />
        /// </remarks>
        public static XmlDocument GetExceptionXmlDoc(System.Exception ex)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(ex != null, "ex");

            XmlDocument xmlDoc = CreateDomDocument("<ResponseError/>");
            XmlElement root = xmlDoc.DocumentElement;

            AppendExceptionNode(root, ex);

            return xmlDoc;
        }

        private static void AppendExceptionNode(this XmlNode root, System.Exception ex)
        {
            AppendNode(root, "Value", ex.Message);
            AppendNode(root, "Stack", ex.StackTrace);

            if (ex.InnerException != null)
            {
                XmlNode nodeInnerEx = AppendNode(root, "InnerException");

                AppendExceptionNode(nodeInnerEx, ex.InnerException);
            }
        }

        /// <summary>
        /// ��XmlWriter�����Exception����Ϣ
        /// </summary>
        /// <param name="writer">XmlWriter����</param>
        /// <param name="ex">�쳣����</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region ="AppendExceptionInfoTest" lang="cs" title="��Exception���л�ΪXml" />
        /// </remarks>
        public static void AppendExceptionInfo(this XmlWriter writer, System.Exception ex)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(ex != null, "ex");

            writer.WriteStartElement("ResponseError");
            AppendExceptionNode(writer, ex);
            writer.WriteEndElement();
        }

        private static void AppendExceptionNode(this XmlWriter writer, System.Exception ex)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(writer != null, "writer");
            ExceptionHelper.FalseThrow<ArgumentNullException>(ex != null, "ex");

            XmlHelper.AppendNode(writer, "Value", ex.Message);
            XmlHelper.AppendNode(writer, "Stack", ex.StackTrace);

            if (ex.InnerException != null)
            {
                writer.WriteStartElement("InnerException");
                AppendExceptionNode(writer, ex.InnerException);
                writer.WriteEndElement();
            }
        }
        #endregion

        #region ���غͽ���Xml�ĵ�����
        /// <summary>
        /// ��ĳ�������ļ�����XML�ĵ�����
        /// </summary>
        /// <param name="path">�ļ�·��</param>
        /// <returns>XML�ĵ�����</returns>
        /// <remarks>��ĳ�������ļ�����XML�ĵ����󣬸÷�����XmlDocument.Load�Ĳ�ͬ��������֧�ֹ��������ʹ�ڱ�ĳ�������д����ļ�
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region ="LoadDocumentTest" lang="cs" title="�Ӵ��̼���Xml�ĵ�����" />
        ///</remarks>
        public static XmlDocument LoadDocument(string path)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(path, "path");

            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(fs);

                return xmlDoc;
            }
        }

        /// <summary>
        /// ��ĳ�������ļ��첽����XML�ĵ�����
        /// </summary>
        /// <param name="path">�ļ�·��</param>
        /// <returns>XML�ĵ�����</returns>
        /// <remarks>��ĳ�������ļ�����XML�ĵ����󣬸÷�����XmlDocument.Load�Ĳ�ͬ��������֧�ֹ��������ʹ�ڱ�ĳ�������д����ļ�
        ///</remarks>
        public static async Task<XmlDocument> LoadDocumentAsync(string path)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(path, "path");

            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                StreamReader reader = new StreamReader(fs);

                string xml = await reader.ReadToEndAsync();

                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(xml);

                return xmlDoc;
            }
        }

        /// <summary>
        /// �Թ������ʽ���ļ�����XElement
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XElement LoadElement(string path)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(path, "path");

            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return XElement.Load(fs);
            }
        }

        /// <summary>
        /// �Թ������ʽ�첽���ļ�����XElement
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<XElement> LoadElementAsync(string path)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(path, "path");

            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                StreamReader reader = new StreamReader(fs);

                string xml = await reader.ReadToEndAsync();

                return XElement.Parse(xml);
            }
        }

        /// <summary>
        /// ����һ��XML�ĵ����󣬲���ʼ���ĵ��������ĵ�����Ĵ����ͳ�ʼ����
        /// </summary>
        /// <param name="xml">XML�ĵ������ڳ�ʼ����ʱ��Ҫ�������ֵ</param>
        /// <returns>�����Ժ��ұ���ʼ����XmlDocument����</returns>
        /// <remarks>
        /// ����һ��XML�ĵ����󣬲��ҿ��Գ�ʼ���ĵ��������ĵ�����Ĵ����ͳ�ʼ�������ú���������XML�ĵ�����
        /// �����ͳ�ʼ�����������������������˱����Ա�Ĵ���XML�ĵ�����ͳ�ʼ���Ĺ�����
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="CreateDocumentTest" lang="cs" title="����һ��Xml�ĵ�����" />
        /// </remarks>
        /// <example>
        /// <code>
        /// XmlDocument xmlDoc = XMLHelper.CreateDomDocument("&lt;DataSet/&gt;");
        /// </code>
        /// </example>
        public static XmlDocument CreateDomDocument(string xml)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(xml, "xml");

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(xml);

            return xmlDoc;
        }
        #endregion ���غͽ���Xml�ĵ�����

        #region AppendNode��Attributeϵ��
        /// <summary>
        /// ��xml�ĵ���������rootָ���Ľڵ��������һ������ΪnodeNameָ�����ӽڵ�
        /// </summary>
        /// <param name="root">�ĵ������е�ָ��Ҫ����ӽڵ�ĸ��ڵ�</param>
        /// <param name="nodeName">Ҫ����ӵ��ӽڵ�����</param>
        /// <returns>����ӵ��ӽڵ����</returns>
        /// <remarks>
        /// ��xml�ĵ���������rootָ���Ľڵ��������һ������ΪnodeNameָ�����ӽڵ㡣���ӽڵ��������
        /// nodeNameָ�����������ݣ�InnerXml��Ϊ�ա�
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region ="AppendNodeTest" lang="cs" title="��xml�ĵ���������ָ���Ľڵ��������ָ�����ӽڵ�" />
        /// </remarks>
        /// <example>
        /// <code>
        /// XmlDocument xmlDoc = CreateDomDocument("&lt;FIRST&gt;&lt;SECOND&gt;&lt;THIRD/&gt;&lt;/SECOND&gt;&lt;/FIRST&gt;");
        /// XmlNode root = xmlDoc.DocumentElement.FirstChild;
        /// XmlNode node = XMLHelper.AppendNode(root, "FOURTH");
        /// </code>
        /// ���Ϊ��
        /// <code>
        /// &lt;FIRST&gt;
        ///		&lt;SECOND&gt;
        ///			&lt;THIRD/&gt;
        ///			&lt;FOURTH/&gt;
        ///		&lt;/SECOND&gt;
        /// &lt;/FIRST&gt;
        /// </code>
        /// </example>
        public static XmlNode AppendNode(this XmlNode root, string nodeName)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            XmlNode node = root.OwnerDocument.CreateNode(XmlNodeType.Element, nodeName, root.NamespaceURI);

            root.AppendChild(node);

            return node;
        }

        /// <summary>
        /// ��xml�ĵ���������rootָ���Ľڵ��������һ������ΪnodeNameָ�����ӽڵ�
        /// </summary>
        /// <param name="root"></param>
        /// <param name="nodeName"></param>
        /// <param name="namespaceURI"></param>
        /// <returns></returns>
        public static XmlNode AppendNodeWithNamespace(this XmlNode root, string nodeName, string namespaceURI)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");
            ExceptionHelper.FalseThrow<ArgumentNullException>(namespaceURI != null, "namespaceURI");

            XmlNode node = root.OwnerDocument.CreateNode(XmlNodeType.Element, nodeName, namespaceURI);

            root.AppendChild(node);

            return node;
        }

        /// <summary>
        /// ��xml�ĵ���������rootָ���Ľڵ��������һ������ΪnodeNameָ�����ӽڵ㣬���иýڵ������ΪnodeValue
        /// </summary>
        /// <typeparam name="T">�ڵ�ֵ������</typeparam>
        /// <param name="root"></param>
        /// <param name="nodeName"></param>
        /// <param name="namespaceURI"></param>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        public static XmlNode AppendNodeWithNamespace<T>(this XmlNode root, string nodeName, string namespaceURI, T nodeValue)
        {
            XmlNode node = AppendNodeWithNamespace(root, nodeName, namespaceURI);

            if (nodeValue != null)
                node.InnerText = ChangeDataToString(nodeValue);

            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="nodeName"></param>
        /// <param name="namespaceURI"></param>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        public static XmlNode AppendCDataNodeWithNamespace<T>(this XmlNode root, string nodeName, string namespaceURI, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");
            ExceptionHelper.FalseThrow<ArgumentNullException>(namespaceURI != null, "namespaceURI");

            XmlNode node = root.OwnerDocument.CreateNode(XmlNodeType.Element, nodeName, namespaceURI);

            root.AppendChild(node);

            if (nodeValue != null)
            {
                XmlCDataSection cdata = root.OwnerDocument.CreateCDataSection(ChangeDataToString(nodeValue));

                node.AppendChild(cdata);
            }

            return node;
        }

        /// <summary>
        /// ���nodeValue��Ϊ��(null��string.Empty)����xml�ĵ���������rootָ���Ľڵ��������һ������ΪnodeNameָ�����ӽڵ㣬���иýڵ������ΪnodeValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="nodeName"></param>
        /// <param name="namespaceURI"></param>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        public static XmlNode AppendNotNullNodeWithNamespace<T>(this XmlNode root, string nodeName, string namespaceURI, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            string nodeText = string.Empty;

            if (nodeValue != null)
                nodeText = ChangeDataToString(nodeValue);

            XmlNode node = null;

            if (string.IsNullOrEmpty(nodeText) == false)
            {
                node = AppendNodeWithNamespace(root, nodeName, namespaceURI);

                if (nodeValue != null)
                    node.InnerText = ChangeDataToString(nodeValue);
            }

            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="nodeName"></param>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        public static XmlNode AppendNotDefaultNode<T>(this XmlNode root, string nodeName, T nodeValue)
        {
            return AppendNotDefaultNodeWithNamespace(root, nodeName, root.NamespaceURI, nodeValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root"></param>
        /// <param name="nodeName"></param>
        /// <param name="namespaceURI"></param>
        /// <param name="nodeValue"></param>
        /// <returns></returns>
        public static XmlNode AppendNotDefaultNodeWithNamespace<T>(this XmlNode root, string nodeName, string namespaceURI, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            string nodeText = string.Empty;

            T defaultValue = default(T);

            if (nodeValue.Equals(defaultValue) == false)
                nodeText = ChangeDataToString(nodeValue);

            XmlNode node = null;

            if (string.IsNullOrEmpty(nodeText) == false)
            {
                node = AppendNodeWithNamespace(root, nodeName, namespaceURI);

                if (nodeValue != null)
                    node.InnerText = ChangeDataToString(nodeValue);
            }

            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <param name="namespaceURI"></param>
        /// <returns></returns>
        public static XmlAttribute AppendAttrWithNamespace(this XmlNode node, string attrName, string namespaceURI)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            XmlAttribute attr = node.OwnerDocument.CreateAttribute(attrName, namespaceURI);

            node.Attributes.SetNamedItem(attr);

            return attr;
        }

        /// <summary>
        /// Ϊxml�ĵ������nodeָ���ڵ��ϴ���һ�����ԣ���������ΪattrName�����Ե�ֵΪattrValue
        /// </summary>
        /// <param name="node">Ҫ������ԵĽڵ�</param>
        /// <param name="attrName">Ҫ��ӵ���������</param>
        /// <param name="attrValue">Ҫ��ӵ���������</param>
        /// <param name="namespaceURI"></param>
        /// <typeparam name="T">����ֵ������</typeparam>
        /// <returns>��ӵ����Զ���</returns>
        /// <remarks>
        /// Ϊxml�ĵ������nodeָ���ڵ��ϴ���һ�����ԣ���������ΪattrName��Ĭ�ϸ����Ե�ֵΪstrValueָ����ע�⣬�ú���������
        /// AppendAttr(XmlDocument xmlDoc, XmlNode node, string attrName)������һ��Ĭ��ֵΪ�յ����ԣ�Ȼ���ٰ����������޸ĳ�
        /// attrValueָ��������
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="AppendAttrTest" lang="cs" title="��xml�ĵ������nodeָ���ڵ��ϴ���һ������" />
        /// </remarks>
        public static XmlAttribute AppendAttrWithNamespace<T>(this XmlNode node, string attrName, string namespaceURI, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            XmlAttribute attr = AppendAttrWithNamespace(node, attrName, namespaceURI);

            if (attrValue != null)
                attr.Value = ChangeDataToString(attrValue);

            return attr;
        }

        /// <summary>
        /// ��attrValueֵ�ǿ�ʱ(����null��string.Empty)��Ϊxml�ĵ������nodeָ���ڵ��ϴ���һ�����ԣ���������ΪattrName�����Ե�ֵΪattrValue
        /// </summary>
        /// <typeparam name="T">����ֵ������</typeparam>
        /// <param name="node">Ҫ������ԵĽڵ�</param>
        /// <param name="attrName">Ҫ��ӵ���������</param>
        /// <param name="attrValue">Ҫ��ӵ���������</param>
        /// <param name="namespaceURI"></param>
        /// <returns>��ӵ����Զ���</returns>
        public static XmlAttribute AppendNotNullAttrWithNamespace<T>(this XmlNode node, string attrName, string namespaceURI, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            string attrText = string.Empty;

            if (attrValue != null)
                attrText = ChangeDataToString(attrValue);

            XmlAttribute attr = null;

            if (string.IsNullOrEmpty(attrText) == false)
            {
                attr = AppendAttrWithNamespace(node, attrName, namespaceURI);

                if (attrValue != null)
                    attr.Value = attrText;
            }

            return attr;
        }

        /// <summary>
        /// ���attrValue�����ڸ����͵�ȱʡֵ������Ӵ�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        /// <param name="namespaceURI"></param>
        /// <returns></returns>
        public static XmlAttribute AppendNotDefaultAttrWithNamespace<T>(this XmlNode node, string attrName, string namespaceURI, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            string attrText = string.Empty;

            T defaultValue = default(T);

            if (attrValue.Equals(defaultValue) == false)
                attrText = ChangeDataToString(attrValue);

            XmlAttribute attr = null;

            if (string.IsNullOrEmpty(attrText) == false)
            {
                attr = AppendAttrWithNamespace(node, attrName, namespaceURI);

                if (attrValue != null)
                    attr.Value = attrText;
            }

            return attr;
        }

        /// <summary>
        /// ��xml�ĵ���������rootָ���Ľڵ��������һ������ΪnodeNameָ�����ӽڵ㣬���иýڵ������ΪnodeValue
        /// </summary>
        /// <typeparam name="T">�ڵ�ֵ������</typeparam>
        /// <param name="root">�ĵ������е�ָ��Ҫ����ӽڵ�ĸ��ڵ�</param>
        /// <param name="nodeName">Ҫ����ӵ��ӽڵ�����</param>
        /// <param name="nodeValue">Ҫ����ӵ��ӽڵ������</param>
        /// <returns>����ӵ��ӽڵ����</returns>
        /// <remarks>
        /// ��xml�ĵ���������rootָ���Ľڵ��������һ������ΪnodeNameָ�����ӽڵ㡣���ӽڵ��������
        /// nodeNameָ������������ΪnodeValue��
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="AppendNodeTest" lang="cs" title="��xml�ĵ���������ָ���Ľڵ��������ָ�����ӽڵ�" />
        /// </remarks>
        public static XmlNode AppendNode<T>(this XmlNode root, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            XmlNode node = AppendNode(root, nodeName);

            if (nodeValue != null)
                node.InnerText = ChangeDataToString(nodeValue);

            return node;
        }

        /// <summary>
        /// ���nodeValue��Ϊ��(null��string.Empty)����xml�ĵ���������rootָ���Ľڵ��������һ������ΪnodeNameָ�����ӽڵ㣬���иýڵ������ΪnodeValue
        /// </summary>
        /// <typeparam name="T">�ڵ�ֵ������</typeparam>
        /// <param name="root">�ĵ������е�ָ��Ҫ����ӽڵ�ĸ��ڵ�</param>
        /// <param name="nodeName">Ҫ����ӵ��ӽڵ�����</param>
        /// <param name="nodeValue">Ҫ����ӵ��ӽڵ������</param>
        /// <returns>����ӵ��ӽڵ����</returns>
        public static XmlNode AppendNotNullNode<T>(this XmlNode root, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            string nodeText = string.Empty;

            if (nodeValue != null)
                nodeText = ChangeDataToString(nodeValue);

            XmlNode node = null;

            if (string.IsNullOrEmpty(nodeText) == false)
            {
                node = AppendNode(root, nodeName);

                if (nodeValue != null)
                    node.InnerText = ChangeDataToString(nodeValue);
            }

            return node;
        }

        /// <summary>
        /// ��XmlWriter���������һ������ΪnodeNameָ�����ӽڵ㣬���иýڵ������ΪnodeValue
        /// </summary>
        /// <typeparam name="T">�ڵ�ֵ������</typeparam>
        /// <param name="writer">XmlWriter����</param>
        /// <param name="nodeName">Ҫ����ӵ��ӽڵ�����</param>
        /// <param name="nodeValue">Ҫ����ӵ��ӽڵ������</param>
        /// <remarks>��xml�ĵ���������rootָ���Ľڵ��������һ������ΪnodeNameָ�����ӽڵ㡣���ӽڵ��������
        /// nodeNameָ������������ΪnodeValue��
        /// /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="XmlWriterAppendNodeTest" lang="cs" title="��Xml Writer��������ָ���Ľڵ��������ָ�����ӽڵ�" />
        /// </remarks>
        public static void AppendNode<T>(this XmlWriter writer, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(writer != null, "writer");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            string nodeText = string.Empty;

            if (nodeValue != null)
                nodeText = ChangeDataToString(nodeValue);

            writer.WriteElementString(nodeName, nodeText);
        }

        /// <summary>
        /// ���nodeValue��Ϊ��(null��string.Empty)����XmlWriter���������һ������ΪnodeNameָ�����ӽڵ㣬���иýڵ������ΪnodeValue
        /// </summary>
        /// <typeparam name="T">�ڵ�ֵ������</typeparam>
        /// <param name="writer">XmlWriter����</param>
        /// <param name="nodeName">Ҫ����ӵ��ӽڵ�����</param>
        /// <param name="nodeValue">Ҫ����ӵ��ӽڵ������</param>
        public static void AppendNotNullNode<T>(this XmlWriter writer, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(writer != null, "writer");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            string nodeText = string.Empty;

            if (nodeValue != null)
                nodeText = ChangeDataToString(nodeValue);

            if (string.IsNullOrEmpty(nodeText) == false)
                writer.WriteElementString(nodeName, nodeText);
        }

        /// <summary>
        /// ��XmlWriter���������һ������ΪnodeNameָ�����ӽڵ㣬���иýڵ������CData�У�����ΪnodeValue
        /// </summary>
        /// <typeparam name="T">�ڵ�ֵ������</typeparam>
        /// <param name="writer">XmlWriter����</param>
        /// <param name="nodeName">Ҫ����ӵ��ӽڵ�����</param>
        /// <param name="nodeValue">Ҫ����ӵ��ӽڵ������</param>
        /// <remarks>��xml�ĵ���������rootָ���Ľڵ��������һ������ΪnodeNameָ�����ӽڵ㡣���ӽڵ��������
        /// nodeNameָ�������иýڵ������CData�У�����ΪnodeValue��
        /// /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="XmlWriterAppendNodeTest" lang="cs" title="��Xml Writer��������ָ���Ľڵ��������ָ�����ӽڵ�" />
        /// </remarks>
        public static void AppendCDataNode<T>(this XmlWriter writer, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(writer != null, "writer");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            string nodeText = string.Empty;

            if (nodeValue != null)
                nodeText = ChangeDataToString(nodeValue);

            writer.WriteStartElement(nodeName);
            writer.WriteCData(nodeText);
            writer.WriteEndElement();
        }

        /// <summary>
        /// ��xml�ĵ���������rootָ���Ľڵ��������һ������ΪnodeNameָ�����ӽڵ㣬���иýڵ������ΪnodeValueָ����
        /// </summary>
        /// <param name="root">�ĵ������е�ָ��Ҫ����ӽڵ�ĸ��ڵ�</param>
        /// <param name="nodeName">Ҫ����ӵ��ӽڵ�����</param>
        /// <param name="nodeValue">Ҫ����ӵ��ӽڵ������</param>
        /// <typeparam name="T">�ڵ�ֵ������</typeparam>
        /// <returns>����ӵ��ӽڵ����</returns>
        /// <remarks>��xml�ĵ���������rootָ���Ľڵ��������һ������ΪnodeNameָ�����ӽڵ㣬���иýڵ������ΪnodeValueָ�������ǽڵ�����ݻᱻCData Section��Χ����
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="AppendCDataNodeTest" lang="cs" title="��xml�ĵ���������ָ���Ľڵ��������ָ�����ӽڵ�" />
        /// </remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// XmlDocument xmlDoc = CreateDomDocument("<Document/>");
        /// 
        /// XmlNode root = xmlDoc.DocumentElement;
        /// 
        /// XmlNode node = AppendCDataNode(root, "Data", "  Hello world !");
        ///
        /// ���Ϊ��
        /// <Document>
        ///		<Data>
        ///			<![CDATA[  Hello world !]]/>
        ///		</Data>
        ///	</Document>
        /// ]]>
        /// </code>
        /// </example>
        public static XmlNode AppendCDataNode<T>(this XmlNode root, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            XmlNode node = root.OwnerDocument.CreateNode(XmlNodeType.Element, nodeName, root.NamespaceURI);

            root.AppendChild(node);

            if (nodeValue != null)
            {
                XmlCDataSection cdata = root.OwnerDocument.CreateCDataSection(ChangeDataToString(nodeValue));

                node.AppendChild(cdata);
            }

            return node;
        }

        /// <summary>
        /// ��XML�ĵ�����xmlDoc�е�root�ڵ������һ��������nodeNameָ��������nodeValueָ���Ľڵ㡣�����rootָ��
        /// �ĸ��ڵ����Ѿ������нڵ�����ΪnodeName�������Ǿͽ�������������Ϊ�յ�ʱ���ֵ�޸�ΪnodeValue
        /// </summary>
        /// <typeparam name="T">nodeValue������</typeparam>
        /// <param name="root">�ĵ������е�ָ��Ҫ����ӽڵ�ĸ��ڵ�</param>
        /// <param name="nodeName">Ҫ����ӵ��ӽڵ����ƣ���xPath��ʽ��</param>
        /// <param name="nodeValue">Ҫ����ӵ��ӽڵ������</param>
        /// <returns>����ӵ��ӽڵ����</returns>
        /// <remarks>
        /// ��XML�ĵ�����xmlDoc�е�root�ڵ������һ��������nodeNameָ��������nodeValueָ���Ľڵ㡣�����rootָ��
        /// �ĸ��ڵ����Ѿ������нڵ�����ΪnodeName�������Ǿͽ�������������Ϊ�յ�ʱ���ֵ�޸�ΪnodeValue��
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region = "AppendNotExistsNodeTest" lang="cs" title="��xml�ĵ���������ָ���Ľڵ��������ָ�����ӽڵ�" />
        /// </remarks>
        /// <example>
        /// �������ǿ��Կ�һ�¸ú����Ĵ���ԭ�����������ݵĶԱȣ�����������Խڵ�FOURTH, nodeValue�趨Ϊ��1234����
        /// ���ڵ��趨ΪSECONDָ���ڵ㣩��
        /// <table align="center">
        ///		<tr>
        ///			<td align="center">XML�ĵ�����ԭ����</td>
        ///			<td align="center">XML�ĵ�����������</td>
        ///		</tr>
        ///		<tr>
        ///			<td>
        ///				<code>
        ///	&lt;FIRST&gt;
        ///		&lt;SECOND&gt;
        ///			&lt;THIRD/&gt;
        ///		&lt;/SECOND&gt;
        ///	&lt;/FIRST&gt;
        ///				</code>
        ///			</td>
        ///			<td>
        ///				<code>
        ///	&lt;FIRST&gt;
        ///		&lt;SECOND&gt;
        ///			&lt;THIRD/&gt;
        ///			&lt;FOURTH&gt;1234&lt;/FOURTH&gt;
        ///		&lt;/SECOND&gt;
        ///	&lt;/FIRST&gt;
        ///				</code>
        ///			</td>
        ///		</tr>
        ///		<tr>
        ///			<td>
        ///				<code>
        ///	&lt;FIRST&gt;
        ///		&lt;SECOND&gt;
        ///			&lt;THIRD/&gt;
        ///			&lt;FOURTH/&gt;
        ///		&lt;/SECOND&gt;
        ///	&lt;/FIRST&gt;
        ///				</code>
        ///			</td>
        ///			<td>
        ///				<code>
        ///	&lt;FIRST&gt;
        ///		&lt;SECOND&gt;
        ///			&lt;THIRD/&gt;
        ///			&lt;FOURTH&gt;1234&lt;/FOURTH&gt;
        ///		&lt;/SECOND&gt;
        ///	&lt;/FIRST&gt;
        ///				</code>
        ///			</td>
        ///		</tr>
        ///		<tr>
        ///			<td>
        ///				<code>
        ///	&lt;FIRST&gt;
        ///		&lt;SECOND&gt;
        ///			&lt;THIRD/&gt;
        ///			&lt;FOURTH&gt;6789&lt;/FOURTH&gt;
        ///		&lt;/SECOND&gt;
        ///	&lt;/FIRST&gt;
        ///				</code>
        ///			</td>
        ///			<td>
        ///				<code>
        ///	&lt;FIRST&gt;
        ///		&lt;SECOND&gt;
        ///			&lt;THIRD/&gt;
        ///			&lt;FOURTH&gt;6789&lt;/FOURTH&gt;
        ///		&lt;/SECOND&gt;
        ///	&lt;/FIRST&gt;
        ///				</code>
        ///			</td>
        ///		</tr>
        /// </table>
        /// </example>
        public static XmlNode AppendNotExistsNode<T>(this XmlNode root, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            XmlNode node = root.SelectSingleNode(nodeName);

            if (node == null)
                node = AppendNode(root, nodeName);

            if (nodeValue != null && string.IsNullOrEmpty(node.InnerText))
                node.InnerText = ChangeDataToString(nodeValue);

            return node;
        }

        /// <summary>
        /// ��XML�ĵ�����xmlDoc�е�ָ�����ڵ�root�£����������ΪnodeName�Ľڵ�ͰѸýڵ�������޸�ΪnodeValueָ�������ݡ�
        /// �������root�´���һ������ΪnodeName����ΪnodeValueָ�����½ڵ�
        /// </summary>
        /// <param name="root">���滻�ڵ�ĵĸ��ڵ�</param>
        /// <param name="nodeName">���滻�ӽڵ������(��xpath)</param>
        /// <param name="nodeValue">���滻�ӽڵ������</param>
        /// <typeparam name="T">�ڵ�ֵ������</typeparam>
        /// <returns>���滻�ڵ�Ľڵ����</returns>
        /// <remarks>ReplaceNodeTest
        /// ��XML�ĵ�����xmlDoc�е�ָ�����ڵ�root�£����������ΪnodeName�Ľڵ�ͰѸýڵ�������޸�ΪnodeValueָ�������ݡ�
        /// �������root�´���һ������ΪnodeName����ΪnodeValueָ�����½ڵ�
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region = "ReplaceNodeTest" lang="cs" title="��xml�ĵ����������һ���ڵ㣬���ýڵ���xml�ĵ������д��ڣ���Ѹýڵ��ֵ�滻ΪnodeValue" />
        /// </remarks>
        public static XmlNode ReplaceNode<T>(this XmlNode root, string nodeName, T nodeValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(root != null, "root");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, "nodeName");

            XmlNode node = root.SelectSingleNode(nodeName);

            if (node == null)
                node = AppendNode<T>(root, nodeName, nodeValue);
            else
                node.InnerText = ChangeDataToString(nodeValue);

            return node;
        }

        /// <summary>
        /// ��XML�ĵ�����xmlDoc�е�ָ�����ڵ�root�£����������ΪnodeName�Ľڵ�ͰѸýڵ�������޸�ΪnodeValueָ�������ݡ�
        /// �������root�´���һ������ΪnodeName����ΪnodeValueָ�����½ڵ�
        /// </summary>
        /// <param name="root">���滻�ڵ�ĵĸ��ڵ�</param>
        /// <param name="nodeName">���滻�ӽڵ������(��xpath)</param>
        /// <returns>���滻�ڵ�Ľڵ����</returns>
        /// <remarks> 
        /// ��XML�ĵ�����xmlDoc�е�ָ�����ڵ�root�£����������ΪnodeName�Ľڵ�ͰѸýڵ�������޸�ΪnodeValueָ�������ݡ�
        /// �������root�´���һ������ΪnodeName����ΪnodeValueָ�����½ڵ�
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region = "ReplaceNodeTest" lang="cs" title="��xml�ĵ����������һ���ڵ㣬���ýڵ���xml�ĵ������д��ڣ���Ѹýڵ��ֵ�滻ΪnodeValue" />
        /// </remarks>
        public static XmlNode ReplaceNode(this XmlNode root, string nodeName)
        {
            return ReplaceNode(root, nodeName, string.Empty);
        }

        /// <summary>
        /// Ϊxml�ĵ������nodeָ���ڵ��ϴ���һ�����ԣ���������ΪattrName��Ĭ�ϸ����Ե�ֵΪ��
        /// </summary>
        /// <param name="node">Ҫ������ԵĽڵ�</param>
        /// <param name="attrName">Ҫ��ӵ���������</param>
        /// <returns>��ӵ����Զ��󣨴�ʱ���Ե����ݻ��ǿյģ�</returns>
        /// <remarks>
        /// Ϊxml�ĵ������nodeָ���ڵ��ϴ���һ�����ԣ���������ΪattrName��Ĭ�ϸ����Ե�ֵΪ�ա��������Ҫ��Ӿ������������
        /// ���Բ��øú��������غ���AppendAttr(XmlNode node, string attrName, string strValue)
        ///  <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="AppendAttrTest" lang="cs" title="Ϊxml�ĵ������nodeָ���ڵ��ϴ���һ������" />
        /// </remarks>
        /// <example>
        /// <code>
        /// XmlDocument xmlDoc = XMLHelper.CreateDomDocument("&lt;FIRST&gt;&lt;SECOND&gt;&lt;THIRD/&gt;&lt;/SECOND&gt;&lt;/FIRST&gt;");
        /// XmlAttribute attr = XMLHelperr.AppendAttr(xmlDoc.DocumentElement.FirstChild, "WEIGHT");
        /// attr.Value = "65kg";
        /// </code>
        /// </example>
        public static XmlAttribute AppendAttr(this XmlNode node, string attrName)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            XmlAttribute attr = node.OwnerDocument.CreateAttribute(attrName, node.NamespaceURI);

            node.Attributes.SetNamedItem(attr);

            return attr;
        }

        /// <summary>
        /// Ϊxml�ĵ������nodeָ���ڵ��ϴ���һ�����ԣ���������ΪattrName�����Ե�ֵΪattrValue
        /// </summary>
        /// <param name="node">Ҫ������ԵĽڵ�</param>
        /// <param name="attrName">Ҫ��ӵ���������</param>
        /// <param name="attrValue">Ҫ��ӵ���������</param>
        /// <typeparam name="T">����ֵ������</typeparam>
        /// <returns>��ӵ����Զ���</returns>
        /// <remarks>
        /// Ϊxml�ĵ������nodeָ���ڵ��ϴ���һ�����ԣ���������ΪattrName��Ĭ�ϸ����Ե�ֵΪstrValueָ����ע�⣬�ú���������
        /// AppendAttr(XmlDocument xmlDoc, XmlNode node, string attrName)������һ��Ĭ��ֵΪ�յ����ԣ�Ȼ���ٰ����������޸ĳ�
        /// attrValueָ��������
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="AppendAttrTest" lang="cs" title="��xml�ĵ������nodeָ���ڵ��ϴ���һ������" />
        /// </remarks>
        public static XmlAttribute AppendAttr<T>(this XmlNode node, string attrName, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            XmlAttribute attr = AppendAttr(node, attrName);

            if (attrValue != null)
                attr.Value = ChangeDataToString(attrValue);

            return attr;
        }

        /// <summary>
        /// ��attrValueֵ�ǿ�ʱ(����null��string.Empty)��Ϊxml�ĵ������nodeָ���ڵ��ϴ���һ�����ԣ���������ΪattrName�����Ե�ֵΪattrValue
        /// </summary>
        /// <typeparam name="T">����ֵ������</typeparam>
        /// <param name="node">Ҫ������ԵĽڵ�</param>
        /// <param name="attrName">Ҫ��ӵ���������</param>
        /// <param name="attrValue">Ҫ��ӵ���������</param>
        /// <returns>��ӵ����Զ���</returns>
        public static XmlAttribute AppendNotNullAttr<T>(this XmlNode node, string attrName, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            string attrText = string.Empty;

            if (attrValue != null)
                attrText = ChangeDataToString(attrValue);

            XmlAttribute attr = null;

            if (string.IsNullOrEmpty(attrText) == false)
            {
                attr = AppendAttr(node, attrName);

                if (attrValue != null)
                    attr.Value = attrText;
            }

            return attr;
        }

        /// <summary>
        /// ���attrValue�����ڸ����͵�ȱʡֵ������Ӵ�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        public static XmlAttribute AppendNotDefaultAttr<T>(this XmlNode node, string attrName, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(node != null, "node");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            string attrText = string.Empty;

            T defaultValue = default(T);

            if (attrValue.Equals(defaultValue) == false)
                attrText = ChangeDataToString(attrValue);

            XmlAttribute attr = null;

            if (string.IsNullOrEmpty(attrText) == false)
            {
                attr = AppendAttr(node, attrName);

                if (attrValue != null)
                    attr.Value = attrText;
            }

            return attr;
        }

        /// <summary>
        /// ΪXml Writer����һ�����ԣ���������ΪattrName�����Ե�ֵΪattrValue
        /// </summary>
        /// <typeparam name="T">����ֵ������</typeparam>
        /// <param name="writer">XmlWriter</param>
        /// <param name="attrName">Ҫ��ӵ���������</param>
        /// <param name="attrValue">Ҫ��ӵ���������</param>
        /// <remarks>
        /// Ϊxml�ĵ������nodeָ���ڵ��ϴ���һ�����ԣ���������ΪattrName�����Ե�ֵΪattrValue��
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="XmlWriterAppendAttrTest" lang="cs" title="ΪXml Writer��ָ���ڵ��ϴ���һ������" />
        /// </remarks>
        public static void AppendAttr<T>(this XmlWriter writer, string attrName, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(writer != null, "writer");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            string attrText = string.Empty;

            if (attrValue != null)
                attrText = ChangeDataToString(attrValue);

            writer.WriteAttributeString(attrName, attrText);
        }

        /// <summary>
        /// ��attrValueֵ�ǿ�ʱ(����null��string.Empty)��ΪXml Writer����һ�����ԣ���������ΪattrName�����Ե�ֵΪattrValue
        /// </summary>
        /// <typeparam name="T">����ֵ������</typeparam>
        /// <param name="writer">XmlWriter</param>
        /// <param name="attrName">Ҫ��ӵ���������</param>
        /// <param name="attrValue">Ҫ��ӵ���������</param>
        public static void AppendNotNullAttr<T>(this XmlWriter writer, string attrName, T attrValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(writer != null, "writer");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            string attrText = string.Empty;

            if (attrValue != null)
                attrText = ChangeDataToString(attrValue);

            if (string.IsNullOrEmpty(attrText) == false)
                writer.WriteAttributeString(attrName, attrText);
        }

        #endregion AppendNode��Attributeϵ��

        /// <summary>
        /// �Խڵ��б�nodeList�е�ÿһ���ڵ����nodeOPָ���Ĳ���������oParam�ǲ���ʹ�õ��Ĳ���
        /// </summary>
        /// <param name="nodeList">�ڵ��б�</param>
        /// <param name="nodeOP">ÿ��ö��һ���ڵ������Ĳ���</param>
        /// <param name="oParam">ÿ�β���������Ĳ���</param>
        /// <remarks>
        /// �Խڵ��б�nodeList�е�ÿһ���ڵ����nodeOPָ���Ĳ���������oParam�ǲ���ʹ�õ��Ĳ���
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="EnumNodeListTest" lang="cs" title="�Խڵ��б�nodeList�е�ÿһ���ڵ����nodeOPָ���Ĳ���" />
        /// </remarks>
        /// <example>
        /// <code>
        /// public static void myMethod(XmlNode nodeRoot, object oParam)
        /// {
        /// }
        ///
        /// public static void Invoke(XmlNodeList nodeList)
        /// {
        /// 	XMLHelper.EnumNodeList(nodeList, new XMLHelper.DoNodeList(myMethod), new object());
        /// }
        /// </code>
        /// </example>
        public static void EnumNodeList(this XmlNodeList nodeList, DoNodeList nodeOP, object oParam)
        {
            if (nodeList != null && nodeOP != null)
                foreach (XmlNode node in nodeList)
                    nodeOP(node, oParam);
        }

        #region Get Node��Attribute��ֵϵ��
        /// <summary>
        /// ��ѯһ���ڵ㣬�õ��ýڵ�����ģ�����ýڵ㲻���ڣ��򷵻ؿմ�
        /// </summary>
        /// <param name="nodeRoot">ִ�в�ѯ�ĸ��ڵ�</param>
        /// <param name="path">��ѯ��XPath</param>
        /// <returns>��ѯnodeRoot�£�����path��ѯ���Ľڵ�����ģ����nodeRootΪ�գ���ڵ㲻���ڣ��򷵻ؿմ�</returns>
        /// <remarks>��ѯһ���ڵ㣬�õ��ýڵ�����ģ�����ýڵ㲻���ڣ��򷵻ؿմ�
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region ="GetSingleNodeExceptionTest" lang="cs" title="��ѯһ���ڵ㣬�õ��ýڵ������" />
        /// </remarks>
        public static string GetSingleNodeText(this XmlNode nodeRoot, string path)
        {
            return GetSingleNodeValue<string>(nodeRoot, path, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeRoot"></param>
        /// <param name="path"></param>
        /// <param name="nsMgr"></param>
        /// <returns></returns>
        public static string GetSingleNodeText(this XmlNode nodeRoot, string path, XmlNamespaceManager nsMgr)
        {
            return GetSingleNodeValue<string>(nodeRoot, path, nsMgr, string.Empty);
        }

        /// <summary>
        /// ��ѯһ���ڵ㣬�õ��ýڵ�����ģ�����ýڵ㲻���ڣ��򷵻ؿմ�
        /// </summary>
        /// <param name="nodeRoot">ִ�в�ѯ�ĸ��ڵ�</param>
        /// <param name="path">��ѯ��XPath</param>
        /// <param name="defaultValue">����ڵ㲻���ڣ����ص�ȱʡֵ</param>
        /// <typeparam name="T">����ֵ������</typeparam>
        /// <returns>��ѯnodeRoot�£�����path��ѯ���Ľڵ�����ģ����nodeRootΪ�գ���ڵ�򲻴��ڣ��򷵻�ȱʡֵ</returns>
        /// <remarks>��ѯһ���ڵ㣬�õ��ýڵ�����ģ�����ýڵ㲻���ڣ��򷵻�ȱʡֵ
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region ="GetSingleNodeValueTest" lang="cs" title="��ѯһ���ڵ㣬�õ��ýڵ������" />
        /// </remarks>
        public static T GetSingleNodeValue<T>(this XmlNode nodeRoot, string path, T defaultValue)
        {
            return GetSingleNodeValue(nodeRoot, path, null, defaultValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodeRoot"></param>
        /// <param name="path"></param>
        /// <param name="nsMgr"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetSingleNodeValue<T>(this XmlNode nodeRoot, string path, XmlNamespaceManager nsMgr, T defaultValue)
        {
            T result = defaultValue;

            if (nodeRoot != null)
            {
                ExceptionHelper.CheckStringIsNullOrEmpty(path, "path");

                XmlNode node = null;

                if (nsMgr != null)
                    node = nodeRoot.SelectSingleNode(path, nsMgr);
                else
                    node = nodeRoot.SelectSingleNode(path);

                if (node != null)
                {
                    if (defaultValue == null)
                        result = (T)DataConverter.ChangeType<string, T>(node.InnerText);
                    else
                        result = (T)DataConverter.ChangeType(node.InnerText, defaultValue.GetType());
                }
            }

            return result;
        }

        /// <summary>
        /// ��ѯһ���ڵ㣬����ýڵ㲻���ڣ����׳��쳣
        /// </summary>
        /// <param name="nodeRoot">ִ�в�ѯ�ĸ��ڵ�</param>
        /// <param name="path">��ѯ��XPath</param>
        /// <returns>��ѯnodeRoot�£�����path��ѯ���Ľڵ㣬���nodeRootΪ�գ���ڵ�򲻴��ڣ����׳��쳣</returns>
        /// <remarks>��ѯһ���ڵ㣬����ýڵ㲻���ڣ����׳��쳣
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="GetSingleNodeExceptionTestWithException" lang="cs" title="��ѯһ���ڵ㣬����ýڵ㲻���ڣ����׳��쳣" />
        /// </remarks>
        public static XmlNode GetSingleNodeException(this XmlNode nodeRoot, string path)
        {
            return GetSingleNodeException(nodeRoot, path, string.Empty);
        }

        /// <summary>
        /// ��ѯһ���ڵ㣬����ýڵ㲻���ڣ����׳��쳣
        /// </summary>
        /// <param name="nodeRoot">ִ�в�ѯ�ĸ��ڵ�</param>
        /// <param name="path">��ѯ��XPath</param>
        /// <param name="message">�쳣�ı�</param>
        /// <returns>��ѯnodeRoot�£�����path��ѯ���Ľڵ㣬���nodeRootΪ�գ���ڵ�򲻴��ڣ����׳��쳣</returns>
        /// <remarks>��ѯһ���ڵ㣬����ýڵ㲻���ڣ����׳��쳣
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="GetSingleNodeExceptionTestWithException" lang="cs" title="��ѯһ���ڵ㣬����ýڵ㲻���ڣ����׳��쳣" />
        /// </remarks>
        public static XmlNode GetSingleNodeException(this XmlNode nodeRoot, string path, string message)
        {
            return GetSingleNodeException(nodeRoot, path, null, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeRoot"></param>
        /// <param name="path"></param>
        /// <param name="nsMgr"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static XmlNode GetSingleNodeException(this XmlNode nodeRoot, string path, XmlNamespaceManager nsMgr, string message)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(nodeRoot != null, "nodeRoot");
            ExceptionHelper.CheckStringIsNullOrEmpty(path, "path");

            XmlNode node = null;

            if (nsMgr != null)
                node = nodeRoot.SelectSingleNode(path, nsMgr);
            else
                node = nodeRoot.SelectSingleNode(path);

            if (string.IsNullOrEmpty(message))
                message = string.Format(Resource.CanNotFindXmlNode, nodeRoot.Name, path);

            ExceptionHelper.FalseThrow<ArgumentException>(node != null, message);

            return node;
        }

        /// <summary>
        /// ��XmlReader�У��õ��ڵ���ַ���ֵ
        /// </summary>
        /// <param name="reader">XmlReader</param>
        /// <param name="nodeName">�ڵ�����</param>
        /// <returns>�ڵ���ַ���ֵ</returns>
        /// <remarks>��XmlReader�У��õ��ڵ���ַ���ֵ
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="XmlReaderGetNodeValueTest" lang="cs" title="��XmlReader�еõ���ǰ�ڵ���ַ���ֵ" />
        /// </remarks>
        public static string GetNodeText(this XmlReader reader, string nodeName)
        {
            return GetNodeValue(reader, nodeName, string.Empty);
        }

        /// <summary>
        /// ��XmlReader�У��õ��ڵ��ֵ
        /// </summary>
        /// <typeparam name="T">�ڵ��ֵ����</typeparam>
        /// <param name="reader">XmlReader</param>
        /// <param name="nodeName">�ڵ������</param>
        /// <param name="defaultValue">ȱʡֵ</param>
        /// <returns>�ڵ��ֵ</returns>
        /// <remarks>��XmlReader�У��õ��ڵ��ֵ
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="XmlReaderGetNodeValueTest" lang="cs" title="��XmlReader�еõ���ǰ�ڵ��ֵ" />
        /// </remarks>
        public static T GetNodeValue<T>(this XmlReader reader, string nodeName, T defaultValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(reader != null, "reader");
            ExceptionHelper.CheckStringIsNullOrEmpty(nodeName, nodeName);

            T result = defaultValue;

            if (reader.IsEmptyElement == false)
            {
                try
                {
                    reader.ReadStartElement(nodeName);

                    if (defaultValue == null)
                        result = (T)DataConverter.ChangeType<string, T>(reader.ReadString());
                    else
                        result = (T)DataConverter.ChangeType<string>(reader.ReadString(), defaultValue.GetType());

                    reader.ReadEndElement();
                }
                catch (System.Xml.XmlException)
                {
                }
            }
            else
            {
                try
                {
                    reader.ReadStartElement(nodeName);
                }
                catch (System.Xml.XmlException)
                {
                }
            }

            return result;
        }

        /// <summary>
        /// ��XmlReader�еõ���ǰ�ڵ�������ַ���ֵ
        /// </summary>
        /// <param name="reader">XmlReader</param>
        /// <param name="attrName">��������</param>
        /// <returns>��ǰ�ڵ�������ַ���ֵ</returns>
        /// <remarks>��XmlReader�еõ���ǰ�ڵ�������ַ���ֵ��������Բ����ڣ��򷵻ؿմ���
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="XmlReaderGetAttrValueTest" lang="cs" title="��XmlReader�еõ���ǰ�ڵ�������ַ���ֵ" />
        /// </remarks>
        public static string GetAttributeText(this XmlReader reader, string attrName)
        {
            return GetAttributeValue(reader, attrName, string.Empty);
        }

        /// <summary>
        /// ��XmlReader�еõ���ǰ�ڵ������ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ������</typeparam>
        /// <param name="reader">XmlReader</param>
        /// <param name="attrName">��������</param>
        /// <param name="defaultValue">������Բ����ڣ�ȱʡֵ</param>
        /// <returns>��ǰ�ڵ������ֵ</returns>
        /// <remarks>��XmlReader�еõ���ǰ�ڵ������ֵ��������Բ����ڣ��򷵻�ȱʡֵ
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="XmlReaderGetAttrValueTest" lang="cs" title="��XmlReader�еõ���ǰ�ڵ������ֵ" />
        /// </remarks>
        public static T GetAttributeValue<T>(this XmlReader reader, string attrName, T defaultValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(reader != null, "reader");
            ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

            T result = defaultValue;

            string attrText = reader.GetAttribute(attrName);

            if (attrText != null)
            {
                if (defaultValue == null)
                    result = (T)DataConverter.ChangeType<string, T>(attrText);
                else
                    result = (T)DataConverter.ChangeType(attrText, defaultValue.GetType());
            }

            return result;
        }

        /// <summary>
        /// ���һ���ڵ�����Ե�ֵ����������Բ����ڣ��򷵻ؿմ�
        /// </summary>
        /// <param name="nodeRoot">ִ�в�ѯ�Ľڵ�</param>
        /// <param name="attrName">��������</param>
        /// <returns>nodeRoot��ĳ�����Ե�ֵ����������Բ����ڻ�nodeRootΪ�գ��򷵻ؿմ�</returns>
        /// <remarks>���һ�����Ե�ֵ����������Բ����ڣ��򷵻ؿմ�
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region="GetAttributeValueTest" lang="cs" title="���һ���ڵ�����Ե�ֵ" />
        /// </remarks>
        public static string GetAttributeText(this XmlNode nodeRoot, string attrName)
        {
            return GetAttributeValue<string>(nodeRoot, attrName, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeRoot"></param>
        /// <param name="attrName"></param>
        /// <param name="namespaceURI"></param>
        /// <returns></returns>
        public static string GetAttributeText(this XmlNode nodeRoot, string attrName, string namespaceURI)
        {
            return GetAttributeValue<string>(nodeRoot, attrName, namespaceURI, string.Empty);
        }

        /// <summary>
        /// ���һ���ڵ�����Ե�ֵ����������Բ����ڣ��򷵻�ȱʡֵ
        /// </summary>
        /// <typeparam name="T">����ֵ������</typeparam>
        /// <param name="nodeRoot">ִ�в�ѯ�Ľڵ�</param>
        /// <param name="attrName">��������</param>
        /// <param name="defaultValue">ȱʡֵ</param>
        /// <returns>nodeRoot��ĳ�����Ե�ֵ����������Բ����ڻ�nodeRootΪ�գ��򷵻ؿմ�</returns>
        /// <remarks>���һ���ڵ�����Ե�ֵ����������Բ����ڣ��򷵻�ȱʡֵ
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\XmlHelperTest.cs" region = "GetAttributeValueTest" lang="cs" title="���һ���ڵ�����Ե�ֵ" />
        /// </remarks>
        public static T GetAttributeValue<T>(this XmlNode nodeRoot, string attrName, T defaultValue)
        {
            return GetAttributeValue(nodeRoot, attrName, nodeRoot.NamespaceURI, defaultValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodeRoot"></param>
        /// <param name="attrName"></param>
        /// <param name="namespaceURI"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetAttributeValue<T>(this XmlNode nodeRoot, string attrName, string namespaceURI, T defaultValue)
        {
            T result = defaultValue;

            if (nodeRoot != null)
            {
                ExceptionHelper.CheckStringIsNullOrEmpty(attrName, "attrName");

                XmlNode attr = nodeRoot.Attributes.GetNamedItem(attrName, namespaceURI);

                if (attr != null)
                {
                    if (defaultValue == null)
                        result = (T)DataConverter.ChangeType<string, T>(attr.Value);
                    else
                        result = (T)DataConverter.ChangeType(attr.Value, defaultValue.GetType());
                }
            }

            return result;
        }
        #endregion  Get Node��Attribute��ֵϵ��

        #region Xml�Ͷ���֮���ת��
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="ignorProperties"></param>
        /// <returns></returns>
        public static XmlDocument SerializeObjectToXml<T>(T graph, params string[] ignorProperties)
        {
            XmlObjectMappingItemCollection mappings = XmlObjectMapping.GetMappingInfoByObject(graph);

            XmlDocument xmlDoc = CreateDomDocument(string.Format("<{0}/>", mappings.RootName));

            SerializePropertiesToNodes(xmlDoc.DocumentElement, mappings, graph, ignorProperties);

            return xmlDoc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlDoc"></param>
        /// <param name="graph"></param>
        /// <param name="ignorProperties"></param>
        public static void DeserializeToObject<T>(this XmlDocument xmlDoc, T graph, params string[] ignorProperties)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

            XmlObjectMappingItemCollection mappings = XmlObjectMapping.GetMappingInfoByObject(graph);

            DeserializeNodesToProperties(xmlDoc.DocumentElement, mappings, graph, ignorProperties);
        }

        private static string ChangeDataToString<T>(T data)
        {
            return (string)DataConverter.ChangeType(data, typeof(string));
        }

        private static object ConvertData(XmlObjectMappingItem item, object data)
        {
            try
            {
                System.Type realType = XmlObjectMapping.GetRealType(item.MemberInfo);

                return DataConverter.ChangeType(data, realType);
            }
            catch (System.Exception ex)
            {
                throw new SystemSupportException(
                    string.Format(Resource.ConvertXmlNodeToPropertyError,
                        item.NodeName, item.PropertyName, ex.Message),
                        ex
                    );
            }
        }

        private static void DeserializeNodesToObject<T>(XmlNode currentNode, T graph)
        {
            XmlObjectMappingItemCollection subMappings = XmlObjectMapping.GetMappingInfoByObject(graph);

            DeserializeNodesToProperties(currentNode, subMappings, graph);
        }

        private static void DeserializeNodesToProperties<T>(XmlNode parentNode, XmlObjectMappingItemCollection mappings, T graph, params string[] ignorProperties)
        {
            foreach (XmlObjectMappingItem item in mappings)
            {
                if (Array.Exists<string>(ignorProperties,
                    target => (string.Compare(target, item.PropertyName, true) == 0)) == false)
                {
                    System.Type realType = XmlObjectMapping.GetRealType(item.MemberInfo);

                    object data = null;

                    XmlElement subclassNode = (XmlElement)parentNode.SelectSingleNode(string.Format("Subclass[@propertyName=\"{0}\"]", item.PropertyName));

                    if (subclassNode != null)
                    {
                        data = DeserializeSubClassNodeToObject(subclassNode, item, graph);

                        SetValueToObject(item, graph, data);
                    }
                    else
                    {
                        if (item.MappingType == XmlNodeMappingType.Attribute)
                            data = XmlHelper.GetAttributeValue(parentNode, item.NodeName, TypeCreator.GetTypeDefaultValue(realType));
                        else
                            data = XmlHelper.GetSingleNodeValue(parentNode, item.NodeName, TypeCreator.GetTypeDefaultValue(realType));

                        if (Convertible(realType, data))
                            SetValueToObject(item, graph, ConvertData(item, data));
                    }
                }
            }
        }

        private delegate void DeserializeCollectionAction(int index, object itemDate);

        private static object DeserializeSubClassNodeToObject<T>(XmlNode subclassNode, XmlObjectMappingItem item, T graph)
        {
            string subClassTypeName = XmlHelper.GetAttributeText(subclassNode, "subClassTypeName");

            object data = GetValueFromObject(item, graph);

            bool isArray = XmlHelper.GetAttributeValue(subclassNode, "isArray", false);
            XmlNodeList valueNodes = subclassNode.SelectNodes("Items/Item");

            if (isArray)
                data = TypeCreator.CreateInstance(subClassTypeName,
                        valueNodes.Count);
            else
                if (data == null)
                    data = TypeCreator.CreateInstance(subClassTypeName);

            if (isArray)
            {
                DeserializeNodeToCollection(valueNodes, (i, itemData) => ((Array)data).SetValue(itemData, i));
            }
            else
            {
                if (data is IList)
                {
                    IList list = (IList)data;

                    list.Clear();

                    DeserializeNodeToCollection(valueNodes, (i, itemData) => list.Add(itemData));
                }
                else
                    DeserializeNodesToObject(subclassNode, data);
            }

            return data;
        }

        private static void DeserializeNodeToCollection(XmlNodeList valueNodes, DeserializeCollectionAction action)
        {
            for (int i = 0; i < valueNodes.Count; i++)
            {
                System.Type elementType = TypeCreator.GetTypeInfo(XmlHelper.GetAttributeText(valueNodes[i], "type"));

                XmlElement subclassNode = (XmlElement)valueNodes[i].SelectSingleNode("Subclass");

                object itemData = null;

                if (Type.GetTypeCode(elementType) == TypeCode.Object)
                {
                    itemData = TypeCreator.CreateInstance(elementType);
                    DeserializeNodesToObject(valueNodes[i], itemData);
                }
                else
                    itemData = DataConverter.ChangeType(XmlHelper.GetAttributeText(valueNodes[i], "value"), elementType);

                action(i, itemData);
            }
        }

        private static void SetMemberValueToObject(MemberInfo mi, object graph, object data)
        {
            data = DecorateDate(data);

            switch (mi.MemberType)
            {
                case MemberTypes.Property:
                    PropertyInfo pi = (PropertyInfo)mi;
                    if (pi.CanWrite)
                        pi.SetValue(graph, data, null);
                    break;
                case MemberTypes.Field:
                    FieldInfo fi = (FieldInfo)mi;
                    fi.SetValue(graph, data);
                    break;
                default:
                    XmlObjectMapping.ThrowInvalidMemberInfoTypeException(mi);
                    break;
            }
        }

        /// <summary>
        /// �����ݽ����������Σ�������������͵����Լӹ�
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static object DecorateDate(object data)
        {
            object result = data;

            if (data is DateTime)
            {
                DateTime dt = (DateTime)data;

                if (dt.Kind == DateTimeKind.Unspecified)
                    result = DateTime.SpecifyKind(dt, DateTimeKind.Local);
            }

            return result;
        }

        private static void SetValueToObject(XmlObjectMappingItem item, object graph, object data)
        {
            if (string.IsNullOrEmpty(item.SubClassPropertyName))
                SetMemberValueToObject(item.MemberInfo, graph, data);
            else
            {
                if (graph != null)
                {
                    MemberInfo mi = graph.GetType().GetProperty(item.PropertyName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (mi == null)
                        mi = graph.GetType().GetField(item.PropertyName,
                            BindingFlags.Instance | BindingFlags.Public);

                    if (mi != null)
                    {
                        object subGraph = GetMemberValueFromObject(mi, graph);

                        if (subGraph == null)
                        {
                            if (string.IsNullOrEmpty(item.SubClassTypeDescription) == false)
                                subGraph = TypeCreator.CreateInstance(item.SubClassTypeDescription);
                            else
                                subGraph = Activator.CreateInstance(XmlObjectMapping.GetRealType(mi), true);

                            SetMemberValueToObject(mi, graph, subGraph);
                        }

                        SetMemberValueToObject(item.MemberInfo, subGraph, data);
                    }
                }
            }
        }

        private static bool Convertible(System.Type targetType, object data)
        {
            bool result = true;

            if (data == null && targetType.IsValueType)
                result = false;
            else
            {
                if (data == DBNull.Value)
                {
                    if (targetType != typeof(DBNull) && targetType != typeof(string))
                        result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parentNode"></param>
        /// <param name="item"></param>
        /// <param name="graph"></param>
        /// <param name="ignorProperties"></param>
        /// <returns></returns>
        private static XmlNode SerializeObjectToNode<T>(XmlNode parentNode, XmlObjectMappingItem item, T graph, params string[] ignorProperties)
        {
            XmlObjectMappingItemCollection mappings = XmlObjectMapping.GetMappingInfoByObject(graph);

            XmlNode node = XmlHelper.AppendNode(parentNode, "Subclass");

            System.Type subClassType = graph.GetType();
            XmlHelper.AppendAttr(node, "subClassTypeName", subClassType.AssemblyQualifiedName);
            XmlHelper.AppendAttr(node, "propertyName", item.PropertyName);

            if (subClassType.IsArray)
                XmlHelper.AppendAttr(node, "isArray", true);

            SerializePropertiesToNodes(node, mappings, graph, ignorProperties);

            if (graph is IEnumerable)
                SerializeIEnumerableObject(node, (IEnumerable)graph);

            return node;
        }

        private static void SerializeIEnumerableObject(XmlNode parentNode, IEnumerable graph)
        {
            XmlNode itemsNode = XmlHelper.AppendNode(parentNode, "Items");

            foreach (object data in graph)
            {
                XmlNode itemNode = XmlHelper.AppendNode(itemsNode, "Item");

                if (data != null)
                {
                    XmlHelper.AppendAttr(itemNode, "type", data.GetType().AssemblyQualifiedName);
                    if (Type.GetTypeCode(data.GetType()) == TypeCode.Object)
                    {
                        XmlObjectMappingItemCollection mappings = XmlObjectMapping.GetMappingInfoByObject(data);
                        SerializePropertiesToNodes(itemNode, mappings, data);
                    }
                    else
                        XmlHelper.AppendAttr(itemNode, "value", data);
                }
            }
        }

        private static void SerializePropertiesToNodes<T>(XmlNode parentNode, XmlObjectMappingItemCollection mappings, T graph, params string[] ignorProperties)
        {
            foreach (XmlObjectMappingItem item in mappings)
            {
                if (Array.Exists<string>(ignorProperties,
                    target => (string.Compare(target, item.PropertyName, true) == 0)) == false)
                {
                    object data = GetValueFromObject(item, graph);

                    if ((data == null || data == DBNull.Value || (data != null && data.Equals(TypeCreator.GetTypeDefaultValue(data.GetType())))) == false)
                    {
                        if (Type.GetTypeCode(data.GetType()) == TypeCode.Object)
                        {
                            //�Ӷ���
                            SerializeObjectToNode(parentNode, item, data);
                        }
                        else
                        {
                            if (item.MappingType == XmlNodeMappingType.Attribute)
                                XmlHelper.AppendAttr(parentNode, item.NodeName, data);
                            else
                                XmlHelper.AppendNode(parentNode, item.NodeName, data);
                        }
                    }
                }
            }
        }

        private static object GetValueFromObject(XmlObjectMappingItem item, object graph)
        {
            object data = null;

            if (string.IsNullOrEmpty(item.SubClassPropertyName))
                data = GetValueFromObjectDirectly(item, graph);
            else
            {
                if (graph != null)
                {
                    MemberInfo mi = graph.GetType().GetProperty(item.PropertyName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (mi == null)
                        mi = graph.GetType().GetField(item.PropertyName,
                            BindingFlags.Instance | BindingFlags.Public);

                    if (mi != null)
                    {
                        object subGraph = GetMemberValueFromObject(mi, graph);

                        if (subGraph != null)
                            data = GetValueFromObjectDirectly(item, subGraph);
                    }
                }
            }

            return data;
        }

        private static object GetValueFromObjectDirectly(XmlObjectMappingItem item, object graph)
        {
            object data = GetMemberValueFromObject(item.MemberInfo, graph);

            if (data != null)
            {
                System.Type dataType = data.GetType();
                if (dataType.IsEnum)
                {
                    data = data.ToString();
                }
                else
                    if (dataType == typeof(TimeSpan))
                        data = ((TimeSpan)data).TotalSeconds;
            }

            return data;
        }

        private static object GetMemberValueFromObject(MemberInfo mi, object graph)
        {
            object data = null;

            switch (mi.MemberType)
            {
                case MemberTypes.Property:
                    PropertyInfo pi = (PropertyInfo)mi;
                    if (pi.CanRead)
                        data = pi.GetValue(graph, null);
                    break;
                case MemberTypes.Field:
                    FieldInfo fi = (FieldInfo)mi;
                    data = fi.GetValue(graph);
                    break;
                default:
                    XmlObjectMapping.ThrowInvalidMemberInfoTypeException(mi);
                    break;
            }

            return data;
        }
        #endregion Xml�Ͷ���֮���ת��
    }
}
