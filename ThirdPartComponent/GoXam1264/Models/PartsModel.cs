
/*
 *  Copyright © Northwoods Software Corporation, 1998-2011. All Rights Reserved.
 *
 *  Restricted Rights: Use, duplication, or disclosure by the U.S.
 *  Government is subject to restrictions as set forth in subparagraph
 *  (c) (1) (ii) of DFARS 252.227-7013, or in FAR 52.227-19, or in FAR
 *  52.227-14 Alt. III, as applicable.
 *
 *  This software is proprietary to and embodies the confidential
 *  technology of Northwoods Software Corporation. Possession, use, or
 *  copying of this software and media is authorized only pursuant to a
 *  valid written license from Northwoods or an authorized sublicensor.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Northwoods.GoXam;

namespace Northwoods.GoXam.Model {

  /// <summary>
  /// A model where the data are FrameworkElements:
  /// <see cref="Northwoods.GoXam.Node"/> and <see cref="Northwoods.GoXam.Link"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.NodesSource"/> is an <see cref="ObservableCollection{Node}"/>.
  /// <see cref="GraphLinksModel{NodeType, NodeKey, PortKey, LinkType}.LinksSource"/> is an <see cref="ObservableCollection{Link}"/>.
  /// References to nodes are strings.
  /// They are stored on each <see cref="Northwoods.GoXam.Node"/> by the
  /// <see cref="Northwoods.GoXam.Node.Id"/> property.
  /// Links refer to nodes by the <see cref="Northwoods.GoXam.Link.PartsModelFromNode"/>
  /// and <see cref="Northwoods.GoXam.Link.PartsModelToNode"/> properties,
  /// and by the <see cref="Northwoods.GoXam.Link.PartsModelLabelNode"/> property.
  /// Nodes also refer to their containing subgraph nodes by the
  /// <see cref="Northwoods.GoXam.Node.PartsModelContainingSubGraph"/> property.
  /// </para>
  /// <para>
  /// This class is not Serializable, because the data are FrameworkElements.
  /// </para>
  /// </remarks>
  public class PartsModel : GraphLinksModel<Node, String, String, Link> {
    /// <summary>
    /// Create an empty model of Parts.
    /// </summary>
    public PartsModel() {
      this.Initializing = true;
      this.Name = "PartsModel";
      this.NodeKeyPath = "Id";
      this.GroupNodePath = "PartsModelContainingSubGraph";
      this.LinkFromPath = "PartsModelFromNode";
      this.LinkFromParameterPath = "PartsModelFromPortId";
      this.LinkToPath = "PartsModelToNode";
      this.LinkToParameterPath = "PartsModelToPortId";
      this.LinkLabelNodePath = "PartsModelLabelNode";
      this.NodesSource = new ObservableCollection<Node>();
      this.LinksSource = new ObservableCollection<Link>();
      this.Modifiable = true;
      this.Initializing = false;
    }

    /// <summary>
    /// Unlike most models, the <see cref="PartsModel"/> knows about the <see cref="Northwoods.GoXam.Diagram"/>
    /// that owns the <see cref="Northwoods.GoXam.Part"/>s that are this model's node data and link data.
    /// </summary>
    /// <value>
    /// This property is set by the <see cref="Northwoods.GoXam.Diagram.PartsModel"/> setter.
    /// You should not need to set this property.
    /// </value>
    public Diagram Diagram { get; set; }

    /// <summary>
    /// A node data is a group if it is a <see cref="Northwoods.GoXam.Group"/>.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns></returns>
    protected override bool FindIsGroupForNode(Node nodedata) {
      return nodedata is Group;
    }

    /// <summary>
    /// A node data is a link label if <see cref="Northwoods.GoXam.Node.IsLinkLabel"/> is true.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns></returns>
    protected override bool FindIsLinkLabelForNode(Node nodedata) {
      if (nodedata == null) return false;
      return nodedata.IsLinkLabel;
    }

    /// <summary>
    /// This makes the node's <see cref="Northwoods.GoXam.Node.Id"/> string
    /// unique by suffixing an integer.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <returns></returns>
    protected override bool MakeNodeKeyUnique(Node nodedata) {
      if (nodedata == null) return false;
      String id = nodedata.Id;
      if (id == null) id = "Id";
      int c = 2;
      while (FindNodeByKey(id) != null) {
        id = nodedata.Id + c.ToString(System.Globalization.CultureInfo.InvariantCulture);
        c++;
      }
      nodedata.Id = id;
      return true;
    }

    /// <summary>
    /// This copies a node by copying its elements.
    /// </summary>
    /// <param name="nodedata"></param>
    /// <param name="env"></param>
    /// <returns></returns>
    protected override Node CopyNode1(Node nodedata, CopyDictionary env) {
      if (nodedata == null) return null;
      UIElement ve = CopyVisual(nodedata.EnsuredVisualElement);
      Node node = (Node)Activator.CreateInstance(nodedata.GetType());  // for PartsModel.CopyNode1
      node.IsLinkLabel = nodedata.IsLinkLabel;
      node.Id = nodedata.Id;
      node.PartsModelContainingSubGraph = nodedata.PartsModelContainingSubGraph;
      node.IsExpandedTree = nodedata.IsExpandedTree;
      Group groupdata = nodedata as Group;
      Group group = node as Group;
      if (groupdata != null && group != null) {
        group.IsExpandedSubGraph = groupdata.IsExpandedSubGraph;
      }
      node.Content = ve;  // for PartsModel.CopyNode1
      return node;
    }

    private static UIElement CopyVisual(UIElement elt) {
      if (elt == null) return null;
      String xaml = "";
      //using (System.IO.StringWriter sw = new System.IO.StringWriter()) {
      //  using (System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(sw)) {



      //  }
      //}







      return System.Windows.Markup.XamlReader.Load(xaml) as UIElement;

    }

    /// <summary>
    /// This finishes the copying of a node by updating any reference to a containing group node.
    /// </summary>
    /// <param name="oldnodedata">the original <c>Node</c></param>
    /// <param name="env"></param>
    /// <param name="newnodedata">the copied <c>Node</c></param>
    /// <param name="newgroup">the node whose <c>Id</c> is the new value for the <c>PartsModelContainingSubGraph</c> property</param>
    /// <param name="newmembers">unused</param>
    protected override void CopyNode2(Node oldnodedata, CopyDictionary env, Node newnodedata, Node newgroup, IEnumerable<Node> newmembers) {
      if (newnodedata == null) return;
      if (newgroup != null) newnodedata.PartsModelContainingSubGraph = newgroup.Id;
    }

    /// <summary>
    /// This copies a link by copying its elements.
    /// </summary>
    /// <param name="linkdata"></param>
    /// <param name="env"></param>
    /// <returns></returns>
    protected override Link CopyLink1(Link linkdata, CopyDictionary env) {
      if (linkdata == null) return null;
      UIElement ve = CopyVisual(linkdata.EnsuredVisualElement);
      Link link = new Link();  // for PartsModel.CopyLink1
      link.Content = ve;  // for PartsModel.CopyLink1
      link.PartsModelFromNode = linkdata.PartsModelFromNode;
      link.PartsModelFromPortId = linkdata.PartsModelFromPortId;
      link.PartsModelToNode = linkdata.PartsModelToNode;
      link.PartsModelToPortId = linkdata.PartsModelToPortId;
      link.PartsModelLabelNode = linkdata.PartsModelLabelNode;
      return link;
    }

    /// <summary>
    /// This finishes the copying of a link by updating any references to the "from" node,
    /// the "to" node, and any "label" node.
    /// </summary>
    /// <param name="oldlinkdata"></param>
    /// <param name="env"></param>
    /// <param name="newlinkdata"></param>
    /// <param name="newfromnodedata"></param>
    /// <param name="newtonodedata"></param>
    /// <param name="newlinklabel"></param>
    protected override void CopyLink2(Link oldlinkdata, CopyDictionary env, Link newlinkdata, Node newfromnodedata, Node newtonodedata, Node newlinklabel) {
      if (newlinkdata == null) return;
      if (newfromnodedata != null) newlinkdata.PartsModelFromNode = newfromnodedata.Id;
      newlinkdata.PartsModelFromPortId = newlinkdata.PartsModelFromPortId;
      if (newtonodedata != null) newlinkdata.PartsModelToNode = newtonodedata.Id;
      newlinkdata.PartsModelToPortId = newlinkdata.PartsModelToPortId;
      if (newlinklabel != null) newlinkdata.PartsModelLabelNode = newlinklabel.Id;
    }

    /// <summary>
    /// Create and insert a new link by constructing a <see cref="Northwoods.GoXam.Link"/>.
    /// </summary>
    /// <param name="fromdata">a non-null <see cref="Node"/></param>
    /// <param name="fromparam"></param>
    /// <param name="todata">a non-null <see cref="Node"/></param>
    /// <param name="toparam"></param>
    /// <returns></returns>
    protected override Link InsertLink(Node fromdata, String fromparam, Node todata, String toparam) {
      if (fromdata == null || todata == null) return null;
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      Link link = new Link();  // for PartsModel.InsertLink
      //??? copy a prototype
      link.PartsModelFromNode = fromdata.Id;
      link.PartsModelFromPortId = fromparam;
      link.PartsModelToNode = todata.Id;
      link.PartsModelToPortId = toparam;
      DataTemplate template = this.LinkTemplate;
      if (template == null) template = Diagram.FindDefault<DataTemplate>("DefaultLinkTemplate");
      link.ContentTemplate = template;  // but not bound to data, so no .Content
      link.ApplyTemplate();
      InsertLink(link);
      return link;
    }

    /// <summary>
    /// Gets or sets a <c>DataTemplate</c> that implements the appearance of each link that
    /// is created by <see cref="InsertLink"/>.
    /// </summary>
    /// <value>
    /// By default this is null, which results in a very boring looking link.
    /// </value>
    public DataTemplate LinkTemplate { get; set; }
  }

}

