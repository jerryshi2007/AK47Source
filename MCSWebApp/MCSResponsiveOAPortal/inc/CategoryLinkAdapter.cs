using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace MCSResponsiveOAPortal
{
    public sealed class CategoryLinkAdapter
    {
        private static string ns = "CategorySchema.xsd";

        public static readonly CategoryLinkAdapter Instance = new CategoryLinkAdapter();

        /// <summary>
        /// 提供缓存，以便在应用程序作用域中保持此实例
        /// </summary>
        public static XmlReaderSettings Settings { get; set; }

        CategoryLinkAdapter()
        {

        }

        public void DoSearch(string[] parts, ArrayList result, System.Xml.XmlReader reader)
        {
            DoSearch(parts, result, reader, true);
        }

        public void DoSearch(string[] parts, ArrayList result, System.Xml.XmlReader reader, bool securityTrimmingEnabled)
        {
            if (reader.ReadToFollowing("category", ns) && reader.MoveToAttribute("name") && reader.Value == "Root" && reader.MoveToElement())
            {
                int i = 1;
                if (i < parts.Length)
                {
                    ReadSubElements(reader, parts, i, result, securityTrimmingEnabled);
                }
                else
                {
                    ReadSubItems(reader, result, securityTrimmingEnabled);
                }
            }
        }

        private void ReadSubItems(System.Xml.XmlReader reader, ArrayList result, bool securityTrimmingEnabled)
        {
            int depth = reader.Depth + 1;
            while (reader.Read())
            {
                if (reader.Depth == depth)
                {
                    if (reader.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        var aResult = ReadResult(reader, securityTrimmingEnabled);
                        if (aResult != null)
                            result.Add(aResult);
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                    }
                }
                else if (reader.Depth > depth)
                {
                    reader.Skip();
                }
                else if (reader.Depth < depth)
                {
                    break;
                }
            }
        }

        private CategoryDirectory ReadResult(System.Xml.XmlReader reader, bool securityTrimmingEnabled)
        {
            CategoryDirectory result = null;
            if (reader.NodeType == System.Xml.XmlNodeType.Element)
            {
                if (reader.LocalName == "category" && reader.NamespaceURI == ns)
                {
                    if (DeterminVisible(reader) && DeterminSecure(securityTrimmingEnabled, reader))
                    {
                        result = new CategoryDirectory();
                        if (reader.MoveToAttribute("name"))
                            result.Name = reader.Value;

                        if (reader.MoveToAttribute("title"))
                            result.Title = reader.Value;

                        reader.MoveToElement();
                    }

                }
                else if (reader.LocalName == "item" && reader.NamespaceURI == ns)
                {
                    if (DeterminVisible(reader) && DeterminSecure(securityTrimmingEnabled, reader))
                    {
                        result = new CategoryLink();
                        if (reader.MoveToAttribute("name"))
                            result.Name = reader.Value;
                        if (reader.MoveToAttribute("title"))
                            result.Title = reader.Value;
                        if (reader.MoveToAttribute("group"))
                            ((CategoryLink)result).Group = reader.ReadContentAsInt();
                        if (reader.MoveToAttribute("href"))
                            ((CategoryLink)result).Url = reader.Value;
                        if (reader.MoveToAttribute("role"))
                            ((CategoryLink)result).Role = reader.Value;
                        if (reader.MoveToAttribute("feature"))
                            ((CategoryLink)result).Feature = reader.Value;
                    }
                }
                else if (reader.LocalName == "group" && reader.NamespaceURI == ns)
                {
                    if (DeterminVisible(reader) && DeterminSecure(securityTrimmingEnabled, reader))
                    {
                        result = new CategoryGroup();
                        if (reader.MoveToAttribute("name"))
                            result.Name = reader.Value;
                        if (reader.MoveToAttribute("title"))
                            result.Title = reader.Value;
                        reader.MoveToElement();
                        ArrayList list = new ArrayList();

                        ReadSubItems(reader, list, securityTrimmingEnabled);
                        ((CategoryGroup)result).Categories = (CategoryDirectory[])list.ToArray(typeof(CategoryDirectory));
                    }
                    else
                    {
                        var level = reader.Depth;
                        while (reader.Read() && reader.Depth > level)
                        {
                            reader.Skip(); //跳过其任何子节点
                        }
                    }
                }
            }

            reader.MoveToElement();

            return result;
        }


        private void ReadSubElements(System.Xml.XmlReader reader, string[] parts, int i, ArrayList result, bool securityTrimmingEnabled)
        {
            string nextName = parts[i];
            int depth = reader.Depth + 1;
            while (reader.Read())
            {
                if (reader.Depth == depth)
                {
                    switch (reader.NodeType)
                    {
                        case System.Xml.XmlNodeType.Attribute:
                        case System.Xml.XmlNodeType.CDATA:
                        case System.Xml.XmlNodeType.Comment:
                        case System.Xml.XmlNodeType.Whitespace:
                            continue;
                        case System.Xml.XmlNodeType.EndElement:
                            break;
                        case System.Xml.XmlNodeType.Element:
                            break;
                        default:
                            throw new System.Xml.XmlException("读取到错误的节点");
                    }

                    if (reader.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        bool hit = false;
                        if (reader.MoveToAttribute("name"))
                        {
                            hit = reader.Value == nextName;

                            if (hit)
                                hit = DeterminVisible(reader);
                            reader.MoveToElement();
                        }

                        if (hit)
                        {
                            i++;
                            if (i < parts.Length)
                            {
                                ReadSubElements(reader, parts, i, result, securityTrimmingEnabled);
                            }
                            else
                            {
                                ReadSubItems(reader, result, securityTrimmingEnabled);
                            }

                            break;
                        }
                        else
                        {
                            //reader.Skip();
                        }
                    }
                }
                else if (reader.Depth > depth)
                {
                    reader.Skip();
                }
                else
                {
                    break;
                }
            }
        }

        private bool DeterminVisible(System.Xml.XmlReader reader)
        {
            bool result = true;
            if (reader.MoveToAttribute("visible"))
            {
                if (reader.HasValue)
                    result = reader.ReadContentAsBoolean();
            }

            return result;
        }

        private bool DeterminSecure(bool securityTrimmingEnabled, XmlReader reader)
        {
            bool secure = true;

            if (securityTrimmingEnabled == true)
            {
                if (reader.MoveToAttribute("role"))
                {
                    string r = reader.ReadContentAsString();
                    if (r == "")
                        secure = false;
                    else if (r == "*")
                        secure = true;
                    else
                        secure = HttpContext.Current.User.IsInRole(r);
                }
            }

            return secure;
        }
    }
}