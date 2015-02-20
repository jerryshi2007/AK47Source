using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using MCS.Library.Core;
using System.IO.Packaging;
using System.Globalization;

namespace MCS.Library.Office.OpenXml.Excel
{
    public abstract class ExcelXml
    {
       // internal delegate int ChangedEventHandler(StyleBaseWrapper sender, StyleChangeEventArgs e);

        internal ExcelXml(XmlNamespaceManager nameSpaceManager)
        {
            this.TopNode = null;
            this.NameSpaceManager = nameSpaceManager;
        }

        internal ExcelXml(XmlNamespaceManager nameSpaceManager, XmlNode topNode)
        {
            TopNode = topNode;
            NameSpaceManager = nameSpaceManager;
        }

        internal XmlNamespaceManager NameSpaceManager { get; set; }
        internal XmlNode TopNode { get; set; }

        private string[] _schemaNodeOrder = null;

        internal string[] SchemaNodeOrder
        {
            get
            {
                return _schemaNodeOrder;
            }
            set
            {
                _schemaNodeOrder = value;
            }

        }

        internal XmlNode CreateNode(string path)
        {
            if (path.IsNullOrEmpty())
            {
                return TopNode;
            }
            else
            {
                return CreateNode(path, false);
            }
        }

        internal XmlNode CreateNode(string path, bool insertFirst)
        {
            XmlNode node = TopNode;
            XmlNode prependNode = null;
            foreach (string subPath in path.Split('/'))
            {
                XmlNode subNode = node.SelectSingleNode(subPath, this.NameSpaceManager);
                if (subNode == null)
                {
                    string nodeName;
                    string nodePrefix;

                    string nameSpaceURI = string.Empty;
                    string[] nameSplit = subPath.Split(':');

                    if (SchemaNodeOrder != null && subPath[0] != '@')
                    {
                        insertFirst = false;
                        prependNode = GetPrependNode(subPath, node);
                    }

                    if (nameSplit.Length > 1)
                    {
                        nodePrefix = nameSplit[0];
                        if (nodePrefix[0] == '@')
                        {
                            nodePrefix = nodePrefix.Substring(1, nodePrefix.Length - 1);
                        }
                        nameSpaceURI = this.NameSpaceManager.LookupNamespace(nodePrefix);
                        nodeName = nameSplit[1];
                    }
                    else
                    {
                        nodePrefix = string.Empty;
                        nameSpaceURI = string.Empty;
                        nodeName = nameSplit[0];
                    }
                    if (subPath.StartsWith("@"))
                    {
                        XmlAttribute addedAtt = node.OwnerDocument.CreateAttribute(subPath.Substring(1, subPath.Length - 1), nameSpaceURI);
                        node.Attributes.Append(addedAtt);
                    }
                    else
                    {
                        if (nodePrefix.IsNullOrEmpty())
                        {
                            subNode = node.OwnerDocument.CreateElement(nodeName, nameSpaceURI);
                        }
                        else
                        {
                            if (nodePrefix.IsNullOrEmpty() || (node.OwnerDocument != null && node.OwnerDocument.DocumentElement != null && node.OwnerDocument.DocumentElement.NamespaceURI == nameSpaceURI &&
                                node.OwnerDocument.DocumentElement.Prefix.IsNullOrEmpty()))
                            {
                                subNode = node.OwnerDocument.CreateElement(nodeName, nameSpaceURI);
                            }
                            else
                            {
                                subNode = node.OwnerDocument.CreateElement(nodePrefix, nodeName, nameSpaceURI);
                            }
                        }
                        if (prependNode != null)
                        {
                            node.InsertBefore(subNode, prependNode);
                            prependNode = null;
                        }
                        else if (insertFirst)
                        {
                            node.PrependChild(subNode);
                        }
                        else
                        {
                            node.AppendChild(subNode);
                        }
                    }
                }
                node = subNode;
            }

            return node;
        }

        private XmlNode GetPrependNode(string nodeName, XmlNode node)
        {
            int pos = GetNodePos(nodeName);
            if (pos < 0)
            {
                return null;
            }
            XmlNode prependNode = null;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                int childPos = GetNodePos(childNode.Name);
                if (childPos > -1)
                {
                    if (childPos > pos)
                    {
                        prependNode = childNode;
                        break;
                    }
                }
            }

            return prependNode;
        }

        private int GetNodePos(string nodeName)
        {
            int ix = nodeName.IndexOf(":");
            if (ix > 0)
            {
                nodeName = nodeName.Substring(ix + 1, nodeName.Length - (ix + 1));
            }
            for (int i = 0; i < _schemaNodeOrder.Length; i++)
            {
                if (nodeName == _schemaNodeOrder[i])
                {
                    return i;
                }
            }

            return -1;
        }

        internal void DeleteAllNode(string path)
        {
            string[] split = path.Split('/');
            XmlNode node = TopNode;
            foreach (string s in split)
            {
                node = node.SelectSingleNode(s, NameSpaceManager);
                if (node != null)
                {
                    if (node is XmlAttribute)
                    {
                        (node as XmlAttribute).OwnerElement.Attributes.Remove(node as XmlAttribute);
                    }
                    else
                    {
                        node.ParentNode.RemoveChild(node);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        internal void DeleteNode(string path)
        {
            var node = TopNode.SelectSingleNode(path, NameSpaceManager);
            if (node != null)
            {
                if (node is XmlAttribute)
                {
                    XmlAttribute att = (XmlAttribute)node;
                    att.OwnerElement.Attributes.Remove(att);
                }
                else
                {
                    node.ParentNode.RemoveChild(node);
                }
            }
        }

        internal void SetXmlNodeString(string path, string value)
        {
            SetXmlNodeString(TopNode, path, value, false, false);
        }

        internal void SetXmlNodeString(string path, string value, bool removeIfBlank)
        {
            SetXmlNodeString(TopNode, path, value, removeIfBlank, false);
        }

        internal void SetXmlNodeString(XmlNode node, string path, string value)
        {
            SetXmlNodeString(node, path, value, false, false);
        }

        internal void SetXmlNodeString(XmlNode node, string path, string value, bool removeIfBlank)
        {
            SetXmlNodeString(node, path, value, removeIfBlank, false);
        }

        internal void SetXmlNodeString(XmlNode node, string path, string value, bool removeIfBlank, bool insertFirst)
        {
            if (node == null)
            {
                return;
            }
            if (value.IsNullOrEmpty() && removeIfBlank)
            {
                DeleteAllNode(path);
            }
            else
            {
                XmlNode nameNode = node.SelectSingleNode(path, NameSpaceManager);
                if (nameNode == null)
                {
                    CreateNode(path, insertFirst);
                    nameNode = node.SelectSingleNode(path, NameSpaceManager);
                }
                nameNode.InnerText = value;
            }
        }

        internal void SetXmlNodeBool(string path, bool value)
        {
            SetXmlNodeString(TopNode, path, value ? "1" : "0", false, false);
        }

        internal void SetXmlNodeBool(string path, bool value, bool removeIf)
        {
            if (value == removeIf)
            {
                var node = TopNode.SelectSingleNode(path, NameSpaceManager);
                if (node != null)
                {
                    if (node is XmlAttribute)
                    {
                        var elem = (node as XmlAttribute).OwnerElement;
                        elem.ParentNode.RemoveChild(elem);
                    }
                    else
                    {
                        TopNode.RemoveChild(node);
                    }
                }
            }
            else
            {
                SetXmlNodeString(TopNode, path, value ? "1" : "0", false, false);
            }
        }

        internal bool ExistNode(string path)
        {
            if (TopNode == null || TopNode.SelectSingleNode(path, NameSpaceManager) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        internal bool? GetXmlNodeBoolNullable(string path)
        {
            var value = GetXmlNodeString(path);
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            return GetXmlNodeBool(path);
        }

        internal bool GetXmlNodeBool(string path)
        {
            return GetXmlNodeBool(path, false);
        }

        internal bool GetXmlNodeBool(string path, bool blankValue)
        {
            string value = GetXmlNodeString(path);
            if (value == "1" || value == "-1" || value == "True")
            {
                return true;
            }
            else if (value.IsNullOrEmpty())
            {
                return blankValue;
            }
            else
            {
                return false;
            }
        }

        internal int GetXmlNodeInt(string path)
        {
            int i;
            if (int.TryParse(GetXmlNodeString(path), out i))
            {
                return i;
            }
            else
            {
                return int.MinValue;
            }
        }

        internal decimal GetXmlNodeDecimal(string path)
        {
            decimal d;
            if (decimal.TryParse(GetXmlNodeString(path), NumberStyles.Any, CultureInfo.InvariantCulture, out d))
            {
                return d;
            }
            else
            {
                return 0;
            }
        }

        internal double? GetXmlNodeDoubleNull(string path)
        {
            string s = GetXmlNodeString(path);
            if (s.IsNullOrEmpty())
            {
                return null;
            }
            else
            {
                double v;
                if (double.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out v))
                {
                    return v;
                }
                else
                {
                    return null;
                }
            }
        }

        internal string GetXmlNodeString(string path)
        {
            if (TopNode == null)
            {
                return string.Empty;
            }
            XmlNode nameNode = TopNode.SelectSingleNode(path, NameSpaceManager);
            if (nameNode != null)
            {
                if (nameNode.NodeType == XmlNodeType.Attribute)
                {
                    return nameNode.Value != null ? nameNode.Value : string.Empty;
                }
                else
                {
                    return nameNode.InnerText;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        internal static Uri GetNewUri(Package package, string sUri)
        {
            int id = 1;
            Uri uri;
            do
            {
                uri = new Uri(string.Format(sUri, id++), UriKind.Relative);
            }
            while (package.PartExists(uri));

            return uri;
        }

        internal void InserAfter(XmlNode parentNode, string beforeNodes, XmlNode newNode)
        {
            string[] nodePaths = beforeNodes.Split(',');

            foreach (string nodePath in nodePaths)
            {
                XmlNode node = parentNode.SelectSingleNode(nodePath, NameSpaceManager);
                if (node != null)
                {
                    parentNode.InsertAfter(newNode, node);
                    return;
                }
            }
            parentNode.InsertAfter(newNode, null);
        }
    }

    internal class ExcelXmlElementInstance : ExcelXml
    {
        internal ExcelXmlElementInstance(XmlNamespaceManager namespaceManager)
            : base(namespaceManager)
        { }

        internal ExcelXmlElementInstance(XmlNamespaceManager namespaceManager, XmlNode topNode)
            : base(namespaceManager, topNode)
        { }
    }

    internal static class ExcelXmlElementFactory
    {
        internal static ExcelXml Create(XmlNamespaceManager namespaceManager)
        {
            return new ExcelXmlElementInstance(namespaceManager);
        }

        internal static ExcelXml Create(XmlNamespaceManager namespaceManager, XmlNode topNode)
        {
            return new ExcelXmlElementInstance(namespaceManager, topNode);
        }
    }

}
