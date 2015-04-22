
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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Northwoods.GoXam.Model;

namespace Northwoods.GoXam.Tool {

  /// <summary>
  /// This abstract class is the base class for <see cref="LinkingTool"/> and <see cref="RelinkingTool"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This class includes properties for defining and accessing any temporary nodes and temporary link
  /// that are used during any linking operation, as well as access to the existing diagram's
  /// nodes and link.
  /// </para>
  /// </remarks>
  public abstract class LinkingBaseTool : DiagramTool {
    /// <summary>
    /// This does common initialization for the linking tools.
    /// </summary>
    protected LinkingBaseTool() {
      this.Forwards = true;
      this.ValidPortsCache = new Dictionary<FrameworkElement, bool>();
    }

    static LinkingBaseTool() {
      PortGravityProperty = DependencyProperty.Register("PortGravity", typeof(double), typeof(LinkingBaseTool),
        new FrameworkPropertyMetadata(100.0));
      TemporaryNodeTemplateProperty = DependencyProperty.Register("TemporaryNodeTemplate", typeof(DataTemplate), typeof(LinkingBaseTool),
        new FrameworkPropertyMetadata(null));
      TemporaryLinkTemplateProperty = DependencyProperty.Register("TemporaryLinkTemplate", typeof(DataTemplate), typeof(LinkingBaseTool),
        new FrameworkPropertyMetadata(null));
    }

    /// <summary>
    /// Identifies the <see cref="PortGravity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PortGravityProperty;
    /// <summary>
    /// Gets or sets the distance at which link snapping occurs.
    /// </summary>
    /// <value>
    /// The default value is 100.0.
    /// </value>
    public double PortGravity {
      get { return (double)GetValue(PortGravityProperty); }
      set { SetValue(PortGravityProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TemporaryNodeTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TemporaryNodeTemplateProperty;
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used to render the <see cref="TemporaryFromNode"/>
    /// and <see cref="TemporaryToNode"/>.
    /// </summary>
    /// <value>
    /// If the value is null, the linking tools use a default template.
    /// </value>
    public DataTemplate TemporaryNodeTemplate {
      get { return (DataTemplate)GetValue(TemporaryNodeTemplateProperty); }
      set { SetValue(TemporaryNodeTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TemporaryLinkTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TemporaryLinkTemplateProperty;
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used to render the <see cref="TemporaryLink"/>.
    /// </summary>
    /// <value>
    /// If the value is null, the linking tools use a default template.
    /// </value>
    public DataTemplate TemporaryLinkTemplate {
      get { return (DataTemplate)GetValue(TemporaryLinkTemplateProperty); }
      set { SetValue(TemporaryLinkTemplateProperty, value); }
    }


    /// <summary>
    /// Gets or sets the original <see cref="Link"/> being reconnected by the <see cref="RelinkingTool"/>.
    /// </summary>
    /// <value>
    /// This should be non-null only when this tool is a <see cref="RelinkingTool"/> and it has been activated.
    /// </value>
    protected Link OriginalLink { get; set; }
    /// <summary>
    /// Gets or sets the original node from which the link was connected.
    /// </summary>
    /// <value>
    /// For the <see cref="LinkingTool"/> this will be the starting port if drawing
    /// a new link in the forwards direction.
    /// For the <see cref="RelinkingTool"/> this will be the "from" node of the <see cref="OriginalLink"/>.
    /// </value>
    protected Node OriginalFromNode { get; set; }
    /// <summary>
    /// Gets or sets the <c>FrameworkElement</c> that is the port from which the link was connected.
    /// </summary>
    /// <value>
    /// For the <see cref="LinkingTool"/> this will be the starting port if drawing
    /// a new link in the forwards direction.
    /// For the <see cref="RelinkingTool"/> this will be the "from" port of the <see cref="OriginalLink"/>.
    /// </value>
    protected FrameworkElement OriginalFromPort { get; set; }
    /// <summary>
    /// Gets or sets the original node to which the link was connected.
    /// </summary>
    /// <value>
    /// For the <see cref="LinkingTool"/> this will be the starting node if drawing
    /// a new link in the backwards direction.
    /// For the <see cref="RelinkingTool"/> this will be the "to" node of the <see cref="OriginalLink"/>.
    /// </value>
    protected Node OriginalToNode { get; set; }
    /// <summary>
    /// Gets or sets the <c>FrameworkElement</c> that is the port to which the link was connected.
    /// </summary>
    /// <value>
    /// For the <see cref="LinkingTool"/> this will be the starting port if drawing
    /// a new link in the backwards direction.
    /// For the <see cref="RelinkingTool"/> this will be the "to" port of the <see cref="OriginalLink"/>.
    /// </value>
    protected FrameworkElement OriginalToPort { get; set; }

    /// <summary>
    /// Gets or sets the temporary <see cref="Link"/> that is shown while the user is drawing or reconnecting a link.
    /// </summary>
    /// <value>
    /// This value is constructed by the linking tools using the <see cref="TemporaryLinkTemplateProperty"/>.
    /// </value>
    public Link TemporaryLink { get; set; }
    /// <summary>
    /// Gets or sets the temporary <see cref="Node"/> at the "from" end of the link.
    /// </summary>
    /// <value>
    /// This value is constructed by the linking tools using the <see cref="TemporaryNodeTemplateProperty"/>.
    /// </value>
    public Node TemporaryFromNode { get; set; }
    /// <summary>
    /// Gets the element representing the port for the <see cref="TemporaryFromNode"/>.
    /// </summary>
    /// <value>
    /// This assumes it is the node's <see cref="Part.VisualElement"/>.
    /// </value>
    protected FrameworkElement TemporaryFromPort {
      get { if (this.TemporaryFromNode != null) return this.TemporaryFromNode.VisualElement; else return null; }
    }
    /// <summary>
    /// Gets or sets the temporary <see cref="Node"/> at the "to" end of the link.
    /// </summary>
    /// <value>
    /// This value is constructed by the linking tools using the <see cref="TemporaryNodeTemplateProperty"/>.
    /// </value>
    public Node TemporaryToNode { get; set; }
    /// <summary>
    /// Gets the element representing the port for the <see cref="TemporaryToNode"/>.
    /// </summary>
    /// <value>
    /// This assumes it is the node's <see cref="Part.VisualElement"/>.
    /// </value>
    protected FrameworkElement TemporaryToPort {
      get { if (this.TemporaryToNode != null) return this.TemporaryToNode.VisualElement; else return null; }
    }

    /// <summary>
    /// Gets or sets whether the linking operation is in the forwards direction.
    /// </summary>
    protected bool Forwards { get; set; }

    /// <summary>
    /// Gets or sets the dictionary used to keep track of ports for which a link may be valid.
    /// </summary>
    /// <remarks>
    /// This dictionary remembers the results of calls to <see cref="IsValidLink"/>.
    /// </remarks>
    protected Dictionary<FrameworkElement, bool> ValidPortsCache { get; set; }

    /// <summary>
    /// Gets or sets a proposed <c>FrameworkElement</c> port for connecting a link.
    /// </summary>
    /// <value>
    /// Whether this is a "to" port or a "from" port depends on the direction
    /// in which the link is being drawn or reconnected.
    /// </value>
    protected FrameworkElement TargetPort { get; set; }

    internal PartsModel TemporaryModel {
      get { return this.Diagram.PartsModel; }
    }


    /// <summary>
    /// If needed, call <see cref="CreateTemporaryNode"/> and <see cref="CreateTemporaryLink"/>,
    /// and capture the mouse.
    /// </summary>
    public override void DoStart() {
      if (this.TemporaryFromNode == null) {
        this.TemporaryFromNode = CreateTemporaryNode(false);
      }
      if (this.TemporaryToNode == null) {
        this.TemporaryToNode = CreateTemporaryNode(true);
      }
      if (this.TemporaryLink == null) {
        this.TemporaryLink = CreateTemporaryLink();
      }
      CaptureMouse();
    }

    /// <summary>
    /// Release the mouse capture and clear out any temporary state.
    /// </summary>
    public override void DoStop() {
      ReleaseMouse();
      this.TemporaryModel.SkipsUndoManager = true;
      this.TemporaryModel.RemoveLink(this.TemporaryLink);
      this.TemporaryModel.RemoveNode(this.TemporaryFromNode);
      this.TemporaryModel.RemoveNode(this.TemporaryToNode);
      this.TemporaryModel.SkipsUndoManager = false;

      this.OriginalLink = null;
      this.OriginalFromNode = null;
      this.OriginalFromPort = null;
      this.OriginalToNode = null;
      this.OriginalToPort = null;

      this.ValidPortsCache.Clear();
    }


    /// <summary>
    /// Construct a <see cref="Node"/> to act as a temporary node.
    /// </summary>
    /// <param name="toend"></param>
    /// <returns>
    /// an unbound <see cref="Node"/> defined by the <see cref="TemporaryNodeTemplate"/>,
    /// or if that is null, by the default template named "DefaultTemporaryNodeTemplate".
    /// </returns>
    /// <remarks>
    /// The node should be put in the "Tool" layer.
    /// </remarks>
    protected virtual Node CreateTemporaryNode(bool toend) {
      Node node = new Node();  // for LinkingBaseTool.CreateTemporaryNode
      node.Id = (toend ? "TemporaryToNode" : "TemporaryFromNode");
      DataTemplate template = this.TemporaryNodeTemplate;
      if (template == null) template = Diagram.FindDefault<DataTemplate>("DefaultTemporaryNodeTemplate");
      node.ContentTemplate = template;  // but not bound to data, so no .Content
      return node;
    }

    /// <summary>
    /// Construct a <see cref="Link"/> to act as a temporary link.
    /// </summary>
    /// <returns></returns>
    /// <returns>
    /// an unbound <see cref="Link"/> defined by the <see cref="TemporaryLinkTemplate"/>,
    /// or if that is null, by the default template named "DefaultTemporaryLinkTemplate".
    /// </returns>
    /// <remarks>
    /// The link should be put in the "Tool" layer.
    /// </remarks>
    protected virtual Link CreateTemporaryLink() {
      Link link = new Link();  // for LinkingBaseTool.CreateTemporaryLink
      DataTemplate template = this.TemporaryLinkTemplate;
      if (template == null) template = Diagram.FindDefault<DataTemplate>("DefaultTemporaryLinkTemplate");
      link.ContentTemplate = template;  // but not bound to data, so no .Content
      return link;
    }

    /// <summary>
    /// Make a temporary port look and act like a real one.
    /// </summary>
    /// <param name="realnode"></param>
    /// <param name="realport"></param>
    /// <param name="tempnode"></param>
    /// <param name="tempport"></param>
    /// <param name="toend"></param>
    protected virtual void CopyPortProperties(Node realnode, FrameworkElement realport, Node tempnode, FrameworkElement tempport, bool toend) {
      if (realnode == null || realport == null || tempnode == null || tempport == null) return;
      Size portsz = realnode.GetEffectiveSize(realport);
      tempport.Width = portsz.Width;
      tempport.Height = portsz.Height;
      if (toend) {
        Node.SetToSpot(tempport, Node.GetToSpot(realport));
        Node.SetToEndSegmentLength(tempport, Node.GetToEndSegmentLength(realport));
      } else {
        Node.SetFromSpot(tempport, Node.GetFromSpot(realport));
        Node.SetFromEndSegmentLength(tempport, Node.GetFromEndSegmentLength(realport));
      }
      tempnode.LocationSpot = Spot.Center;
      tempnode.Location = realnode.GetElementPoint(realport, Spot.Center);
      tempnode.SetAngle(tempport, realnode.GetAngle(realport));
    }

    /// <summary>
    /// Reset temporary port properties to neutral values when there is no target port.
    /// </summary>
    /// <param name="tempnode"></param>
    /// <param name="tempport"></param>
    protected virtual void SetNoTargetPortProperties(Node tempnode, FrameworkElement tempport) {
      if (tempport != null) {
        tempport.Width = 1;
        tempport.Height = 1;
        Node.SetFromSpot(tempport, Spot.None);
        Node.SetToSpot(tempport, Spot.None);
      }
      if (tempnode != null) {
        tempnode.Location = this.Diagram.LastMousePointInModel;
      }
    }


    /// <summary>
    /// Mouse movement results in the temporary node moving to where the valid <see cref="LinkingBaseTool.TargetPort"/> is located,
    /// or to where the mouse is if there is no valid target port nearby.
    /// </summary>
    /// <remarks>
    /// This calls <see cref="LinkingBaseTool.FindTargetPort"/> to update the <see cref="LinkingBaseTool.TargetPort"/>
    /// given the new mouse point.
    /// If a valid target port is found, this calls <see cref="LinkingBaseTool.CopyPortProperties"/> to move the
    /// temporary node/port and make them appear like the target node/port.
    /// If no valid target port is found, this calls <set cref="LinkingBaseTool.SetNoTargetPortProperties"/>
    /// to move the temporary node to where the mouse currently is and to remove any node/port appearance.
    /// </remarks>
    public override void DoMouseMove() {
      if (this.Active) {
        Diagram diagram = this.Diagram;
        this.TargetPort = FindTargetPort(this.Forwards);
        if (this.TargetPort != null) {
          Node targetnode = Diagram.FindAncestor<Node>(this.TargetPort);
          if (targetnode != null) {
            if (this.Forwards) {
              CopyPortProperties(targetnode, this.TargetPort, this.TemporaryToNode, this.TemporaryToPort, true);
              return;
            } else {
              CopyPortProperties(targetnode, this.TargetPort, this.TemporaryFromNode, this.TemporaryFromPort, false);
              return;
            }
          }
        }
        // found no potential port
        if (this.Forwards) {
          SetNoTargetPortProperties(this.TemporaryToNode, this.TemporaryToPort);
        } else {
          SetNoTargetPortProperties(this.TemporaryFromNode, this.TemporaryFromPort);
        }
      }
    }


    // search up the chain of parent elements to find one that has Node.GetLinkableTo == true and IsValidTo
    internal FrameworkElement FindValidToPort(DependencyObject d) {
      FrameworkElement elt = Diagram.FindAncestorOrSelf<FrameworkElement>(d);
      if (elt == null) return null;
      Node node = Diagram.FindAncestor<Node>(elt);
      if (node == null) return null;
      return FindElementUpFrom(elt, Node.GetLinkableTo, x => IsValidTo(node, x));
    }

    // search up the chain of parent elements to find one that has Node.GetLinkableFrom == true and IsValidFrom
    internal FrameworkElement FindValidFromPort(DependencyObject d) {
      FrameworkElement elt = Diagram.FindAncestorOrSelf<FrameworkElement>(d);
      if (elt == null) return null;
      Node node = Diagram.FindAncestor<Node>(elt);
      if (node == null) return null;
      return FindElementUpFrom(elt, Node.GetLinkableFrom, x => IsValidFrom(node, x));
    }

    /// <summary>
    /// Find a port with which the user could complete a valid link.
    /// </summary>
    /// <param name="toend">true if looking for a "to" port</param>
    /// <returns>
    /// a <c>FrameworkElement</c> representing a valid port,
    /// or null if no such port is near the current mouse point (within <see cref="PortGravity"/> distance)
    /// </returns>
    /// <remarks>
    /// <para>
    /// This finds elements near to the current mouse point for which a valid link connection is possible.
    /// For example, when <paramref name="toend"/> is true, this looks for elements (i.e. "ports") in nodes that
    /// have <see cref="Node.GetLinkableTo"/> return true and for which <see cref="IsValidTo"/> is true.
    /// </para>
    /// <para>
    /// For each port element found, this calls <see cref="IsValidLink"/> to find out if a link between
    /// the original node/port and the found node/port would be valid.
    /// The result is saved in the <see cref="ValidPortsCache"/> for faster decisions later during
    /// the operation of this tool.
    /// The closest valid port is returned.
    /// </para>
    /// </remarks>
    protected virtual FrameworkElement FindTargetPort(bool toend) {
      Diagram diagram = this.Diagram;
      Point p = diagram.LastMousePointInModel;
      double gravity = this.PortGravity;
      IEnumerable<FrameworkElement> nearports;
      if (toend)
        nearports = diagram.Panel.FindElementsNear<FrameworkElement>(p, gravity, FindValidToPort, x => true, SearchLayers.Nodes);
      else
        nearports = diagram.Panel.FindElementsNear<FrameworkElement>(p, gravity, FindValidFromPort, x => true, SearchLayers.Nodes);
      double bestDist = gravity*gravity;  // square here so don't need to sqrt later
      FrameworkElement bestPort = null;
      foreach (FrameworkElement port in nearports) {
        if (port == null) continue;
        Node node = Diagram.FindAncestor<Node>(port);
        if (node == null) continue;
        Point toPoint = node.GetElementPoint(port, Spot.Center);  //?? assumes center point of port
        double dx = p.X - toPoint.X;
        double dy = p.Y - toPoint.Y;
        double dist = dx*dx + dy*dy;  // don't bother taking sqrt
        if (dist < bestDist) {  // closest so far
          bool valid = true;
          // check cache of IsValidLink calls
          if (this.ValidPortsCache.TryGetValue(port, out valid)) {
            // known to be either valid or invalid
            if (valid) { // known to be a valid port for a link
              bestPort = port;
              bestDist = dist;
            } // else known not valid: don't need to call IsValidLink again
          } else {  // but if not cached, try IsValidLink in the appropriate direction
            if ((toend && IsValidLink(this.OriginalFromNode, this.OriginalFromPort, node, port)) ||
                (!toend && IsValidLink(node, port, this.OriginalToNode, this.OriginalToPort))) {
              // now known valid, remember in cache
              this.ValidPortsCache[port] = true;
              bestPort = port;
              bestDist = dist;
            } else {
              // now known not valid, remember in cache
              this.ValidPortsCache[port] = false;
            }
          }
        }
      }
      if (bestPort != null) {
        Node targetnode = Diagram.FindAncestor<Node>(bestPort);
        if (targetnode != null && (targetnode.Layer == null || targetnode.Layer.AllowLink)) {
          return bestPort;
        }
      }
      return null;
    }


    /// <summary>
    /// This predicate is true if it is permissible to connect a link from a given node/port.
    /// </summary>
    /// <param name="fromnode"></param>
    /// <param name="fromport"></param>
    /// <returns>
    /// False if the <paramref name="fromnode"/> is in a <see cref="Layer"/> that does not <see cref="Layer.AllowLink"/>.
    /// False if <see cref="Node.GetLinkableFrom"/> for the <paramref name="fromport"/> is either false or null.
    /// False if the number of links connected to the <paramref name="fromport"/> would exceed the <see cref="Node.GetLinkableMaximum"/> value.
    /// Otherwise true.
    /// </returns>
    public virtual bool IsValidFrom(Node fromnode, FrameworkElement fromport) {
      if (fromnode == null || fromport == null) {
        var lmodel = this.Diagram.Model as ILinksModel;
        return (lmodel != null && lmodel.ValidUnconnectedLinks == ValidUnconnectedLinks.Allowed);
      }
      if (fromnode.Layer != null && !fromnode.Layer.AllowLink) return false;
      if (Node.GetLinkableFrom(fromport) != true) return false;  // false or null value means not linkable
      int maxlinks = Node.GetLinkableMaximum(fromport);
      if (maxlinks < int.MaxValue) {
        //??? wrong number, because RelinkingTool doesn't temporarily disconnect?
        if (this.OriginalLink != null && fromnode == this.OriginalFromNode && fromport == this.OriginalFromPort) return true;
        if (fromnode.FindLinksConnectedWithPort(fromnode.GetPortName(fromport)).Count() >= maxlinks) return false;
      }
      return true;
    }

    /// <summary>
    /// This predicate is true if it is permissible to connect a link to a given node/port.
    /// </summary>
    /// <param name="tonode"></param>
    /// <param name="toport"></param>
    /// <returns>
    /// False if the <paramref name="tonode"/> is in a <see cref="Layer"/> that does not <see cref="Layer.AllowLink"/>.
    /// False if <see cref="Node.GetLinkableTo"/> for the <paramref name="toport"/> is either false or null.
    /// False if the number of links connected to the <paramref name="toport"/> would exceed the <see cref="Node.GetLinkableMaximum"/> value.
    /// Otherwise true.
    /// </returns>
    public virtual bool IsValidTo(Node tonode, FrameworkElement toport) {
      if (tonode == null || toport == null) {
        var lmodel = this.Diagram.Model as ILinksModel;
        return (lmodel != null && lmodel.ValidUnconnectedLinks == ValidUnconnectedLinks.Allowed);
      }
      if (tonode.Layer != null && !tonode.Layer.AllowLink) return false;
      if (Node.GetLinkableTo(toport) != true) return false;  // false or null value means not linkable
      int maxlinks = Node.GetLinkableMaximum(toport);
      if (maxlinks < int.MaxValue) {
        //??? wrong number, because RelinkingTool doesn't temporarily disconnect?
        if (this.OriginalLink != null && tonode == this.OriginalToNode && toport == this.OriginalToPort) return true;
        if (tonode.FindLinksConnectedWithPort(tonode.GetPortName(toport)).Count() >= maxlinks) return false;
      }
      return true;
    }

    /// <summary>
    /// This predicate is true if both argument ports are in the same <see cref="Node"/>.
    /// </summary>
    /// <param name="fromport"></param>
    /// <param name="toport"></param>
    /// <returns></returns>
    public virtual bool IsInSameNode(FrameworkElement fromport, FrameworkElement toport) {
      if (fromport == null || toport == null) return false;
      if (fromport == toport) return true;
      Node fromnode = Diagram.FindAncestor<Node>(fromport);
      Node tonode = Diagram.FindAncestor<Node>(toport);
      return (fromnode != null && fromnode == tonode);
    }

    /// <summary>
    /// This predicate is true if there is a link in the model going from <paramref name="fromport"/> to <paramref name="toport"/>.
    /// </summary>
    /// <param name="fromport">a <c>FrameworkElement</c> representing the "from" port</param>
    /// <param name="toport">a <c>FrameworkElement</c> representing the "to" port</param>
    /// <returns></returns>
    public virtual bool IsLinked(FrameworkElement fromport, FrameworkElement toport) {
      if (this.Diagram == null || this.Diagram.PartManager == null) return false;
      if (fromport == null || toport == null) return false;
      Node fromnode = Diagram.FindAncestor<Node>(fromport);
      Node tonode = Diagram.FindAncestor<Node>(toport);

      Object fromdata = fromnode.Data;
      Object fromid = fromnode.GetPortName(fromport);
      Object todata = tonode.Data;
      Object toid = tonode.GetPortName(toport);
      IDiagramModel model = this.Diagram.PartManager.FindCommonDataModel(fromdata, todata);
      return model != null && model.IsLinked(fromdata, fromid, todata, toid);
    }

    /// <summary>
    /// This predicate should be true when it is logically valid to connect a link from
    /// one node/port to another node/port.
    /// </summary>
    /// <param name="fromnode">the "from" <see cref="Node"/></param>
    /// <param name="fromport">the "from" <c>FrameworkElement</c></param>
    /// <param name="tonode">the "to" <see cref="Node"/> (perhaps the same as <paramref name="fromnode"/>)</param>
    /// <param name="toport">the "to" <c>FrameworkElement</c> (perhaps the same as <paramref name="fromport"/>)</param>
    /// <returns>
    /// False if <see cref="IsValidFrom"/> is false for the <paramref name="fromnode"/> and <paramref name="fromport"/>.
    /// False if <see cref="IsValidTo"/> is false for the <paramref name="tonode"/> and <paramref name="toport"/>.
    /// False if <see cref="IsInSameNode"/> is true unless <see cref="Node.GetLinkableSelfNode"/> is true for both ports.
    /// False if <see cref="IsLinked"/> is true unless <see cref="Node.GetLinkableDuplicates"/> is true for both ports.
    /// False if trying to link to the link's own label node(s).
    /// False if <see cref="IDiagramModel.IsLinkValid"/> or one of the model-specific <c>IsRelinkValid</c> methods is false,
    /// depending on whether <see cref="OriginalLink"/> is null (a new link) or non-null (a relink).
    /// Otherwise true.
    /// </returns>
    public virtual bool IsValidLink(Node fromnode, FrameworkElement fromport, Node tonode, FrameworkElement toport) {
      if (this.Diagram == null || this.Diagram.PartManager == null) return false;
      //?? allow partly connected or unconnected links
      if (!IsValidFrom(fromnode, fromport)) return false;
      if (!IsValidTo(tonode, toport)) return false;

      if (fromport != null && toport != null) {
        if (!(Node.GetLinkableSelfNode(fromport) && Node.GetLinkableSelfNode(toport)) && IsInSameNode(fromport, toport)) return false;
        if (!(Node.GetLinkableDuplicates(fromport) && Node.GetLinkableDuplicates(toport)) && IsLinked(fromport, toport)) return false;
      }
      
      // disallow linking to the link's own label(s)
      if (this.OriginalLink != null) {
        if (fromnode != null && fromnode.IsContainedBy(this.OriginalLink)) return false;
        if (tonode != null && tonode.IsContainedBy(this.OriginalLink)) return false;
      }

      Object fromdata = (fromnode != null ? fromnode.Data : null);
      Object fromid = (fromnode != null ? fromnode.GetPortName(fromport) : null);
      Object todata = (tonode != null ? tonode.Data : null);
      Object toid = (tonode != null ? tonode.GetPortName(toport) : null);
      IDiagramModel model = this.Diagram.PartManager.FindCommonDataModel(fromdata, todata);
      if (model == null) {
        if (fromnode != null) model = fromnode.Model;
        else if (tonode != null) model = tonode.Model;
        var lmodel = model as ILinksModel;
        if (lmodel == null || lmodel.ValidUnconnectedLinks == ValidUnconnectedLinks.None) return false;
        // if the model allows unconnected links, it's OK
      }
      if (this.OriginalLink != null) {
        ILinksModel lmodel = model as ILinksModel;
        if (lmodel != null) return lmodel.IsRelinkValid(fromdata, fromid, todata, toid, this.OriginalLink.Data);
        IConnectedModel cmodel = model as IConnectedModel;
        if (cmodel != null) return cmodel.IsRelinkValid(fromdata, todata, this.OriginalLink.FromData, this.OriginalLink.ToData);
        ITreeModel tmodel = model as ITreeModel;
        if (tmodel != null) return tmodel.IsRelinkValid(fromdata, todata, this.OriginalLink.FromData, this.OriginalLink.ToData);
        return false;
      } else {
        return model.IsLinkValid(fromdata, fromid, todata, toid);
      }
    }
  }


  /// <summary>
  /// This enumeration lists the possible directions that the user may draw a new link
  /// using the <see cref="LinkingTool"/>.
  /// </summary>
  public enum LinkingDirection {
    /// <summary>
    /// Links may be drawn either forwards or backwards.
    /// </summary>
    Either,
    /// <summary>
    /// Links may be drawn forwards only (i.e. from "from" node to "to" node).
    /// </summary>
    ForwardsOnly,
    /// <summary>
    /// Links may be drawn backwards only (i.e. from "to" node to "from" node).
    /// </summary>
    BackwardsOnly
  }


  /// <summary>
  /// The <c>LinkingTool</c> lets a user draw a new <see cref="Link"/> between two ports, using a mouse-drag operation.
  /// </summary>
  /// <remarks>
  /// <para>
  /// By default an instance of this tool is installed as a mouse-move tool in each <see cref="Northwoods.GoXam.Diagram"/>,
  /// as the value of <see cref="Northwoods.GoXam.Diagram.LinkingTool"/>.
  /// <see cref="CanStart"/> calls <see cref="FindLinkablePort"/> to find a valid "port" element
  /// from which (or to which) the user may interactively draw a new link.
  /// <see cref="DoActivate"/> sets up a temporary link and two temporary nodes, one at the start port and one following the mouse.
  /// </para>
  /// <para>
  /// This tool does not utilize any <see cref="Adornment"/>s or tool handles.
  /// </para>
  /// <para>
  /// This tool conducts a model edit (<see cref="DiagramTool.StartTransaction"/> and <see cref="DiagramTool.StopTransaction"/>)
  /// while the tool is <see cref="DiagramTool.Active"/>.
  /// </para>
  /// <para>
  /// If you want to programmatically start a new user mouse-gesture to draw a new link
  /// from a given <c>FrameworkElement</c> that may be a "port" element or may be within
  /// the visual tree of a "port" element, set the <see cref="StartElement"/> property
  /// to let <see cref="FindLinkablePort"/> find the real "port" element.
  /// Then start and activate this tool:
  /// <code>
  ///   myDiagram.LinkingTool.StartElement = ...;
  ///   myDiagram.CurrentTool = myDiagram.LinkingTool;
  ///   myDiagram.CurrentTool.DoActivate();
  /// </code>
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class LinkingTool : LinkingBaseTool {

    static LinkingTool() {
      DirectionProperty = DependencyProperty.Register("Direction", typeof(LinkingDirection), typeof(LinkingTool),
        new FrameworkPropertyMetadata(LinkingDirection.Either));
    }

    /// <summary>
    /// Identifies the <see cref="Direction"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DirectionProperty;
    /// <summary>
    /// Gets or sets the direction in which new links may be drawn.
    /// </summary>
    /// <value>
    /// This defaults to <see cref="LinkingDirection.Either"/>.
    /// </value>
    public LinkingDirection Direction {
      get { return (LinkingDirection)GetValue(DirectionProperty); }
      set { SetValue(DirectionProperty, value); }
    }

    /// <summary>
    /// Gets or sets the port element at which the linking operation started.
    /// </summary>
    /// <value>
    /// The default value is null.
    /// This is set by <see cref="DoActivate"/>.
    /// </value>
    protected FrameworkElement StartPort { get; set; }

    /// <summary>
    /// Gets or sets the <c>FrameworkElement</c> at which <see cref="FindLinkablePort"/> should start its search.
    /// </summary>
    /// <value>
    /// The default value is null.
    /// </value>
    /// <remarks>
    /// If you want to explicitly start a new user mouse-gesture to draw a new link
    /// from a given <c>FrameworkElement</c> that may be a "port" element or may be within
    /// the visual tree of a "port" element, set this property to that element
    /// to let <see cref="FindLinkablePort"/> find the real "port" element.
    /// Then start and activate this tool:
    /// <code>
    ///   myDiagram.LinkingTool.StartElement = ...;
    ///   myDiagram.CurrentTool = myDiagram.LinkingTool;
    ///   myDiagram.CurrentTool.DoActivate();
    /// </code>
    /// </remarks>
    public FrameworkElement StartElement { get; set; }

    /// <summary>
    /// This tool can run when the diagram allows linking, the model is modifiable,
    /// the left-button mouse drag has moved far enough away to not be click, and
    /// when <see cref="FindLinkablePort"/> has returned a valid port.
    /// </summary>
    /// <returns></returns>
    public override bool CanStart() {
      if (!base.CanStart()) return false;

      Diagram diagram = this.Diagram;
      if (diagram == null || diagram.IsReadOnly) return false;
      if (!diagram.AllowLink) return false;
      IDiagramModel model = diagram.Model;
      if (model == null || !model.Modifiable) return false;
      // require left button & that it has moved far enough away from the mouse down point, so it isn't a click
      if (!IsLeftButtonDown()) return false;
      // don't include the following check when this tool is running modally
      if (diagram.CurrentTool != this) {
        if (!IsBeyondDragSize()) return false;
      }
      FrameworkElement port = FindLinkablePort();
      return (port != null);
    }

    /// <summary>
    /// Return the element at the mouse-down point, if it is part of a node and if it is valid to link with it.
    /// </summary>
    /// <returns>
    /// If the <see cref="Direction"/> is <see cref="LinkingDirection.Either"/> or <see cref="LinkingDirection.ForwardsOnly"/>,
    /// this checks the element and its parent <see cref="Node"/> by calling <see cref="LinkingBaseTool.IsValidFrom"/>.
    /// If the <see cref="Direction"/> is <see cref="LinkingDirection.Either"/> or <see cref="LinkingDirection.BackwardsOnly"/>,
    /// this checks the element and its parent <see cref="Node"/> by calling <see cref="LinkingBaseTool.IsValidTo"/>.
    /// In either case finding a matching port will return that port and set <see cref="LinkingBaseTool.Forwards"/> in the appropriate direction.
    /// Otherwise this will return null.
    /// </returns>
    /// <remarks>
    /// <para>
    /// If <see cref="StartElement"/> is non-null, it starts it search for a linkable "port" element at that element.
    /// The element may itself be a valid element to draw from or to.
    /// If <see cref="StartElement"/> is null, it finds the <c>FrameworkElement</c> at the
    /// <see cref="Northwoods.GoXam.Diagram.FirstMousePointInModel"/>.
    /// </para>
    /// <para>
    /// Both <see cref="CanStart"/> and <see cref="DoActivate"/> call this method,
    /// although the latter only does so if <see cref="StartPort"/> is null.
    /// </para>
    /// </remarks>
    protected virtual FrameworkElement FindLinkablePort() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      FrameworkElement elt = this.StartElement;
      if (elt == null) elt = diagram.Panel.FindElementAt<FrameworkElement>(diagram.FirstMousePointInModel,
                                 Diagram.FindAncestorOrSelf<FrameworkElement>, x => true, SearchLayers.All);
      Node node = Diagram.FindAncestor<Node>(elt);
      if (node == null) return null;
      // don't search for valid ports "underneath" the object at the current mouse point;
      // only search up the parent tree of elements for one that IsValidFrom
      FrameworkElement port;
      LinkingDirection dir = this.Direction;
      if (dir == LinkingDirection.Either || dir == LinkingDirection.ForwardsOnly) {
        port = FindElementUpFrom(elt, Node.GetLinkableFrom, x => IsValidFrom(node, x));
        if (port != null) {
          this.Forwards = true;
          return port;
        }
      }
      if (dir == LinkingDirection.Either || dir == LinkingDirection.BackwardsOnly) {
        port = FindElementUpFrom(elt, Node.GetLinkableTo, x => IsValidTo(node, x));
        if (port != null) {
          this.Forwards = false;
          return port;
        }
      }
      return null;
    }

    /// <summary>
    /// Start the linking operation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If <see cref="StartPort"/> is already set, it uses that element as the starting port.
    /// If it is not set, this calls <see cref="FindLinkablePort"/> and remembers the port as <see cref="StartPort"/>.
    /// </para>
    /// <para>
    /// It then starts a model edit and changes the cursor.
    /// Next it initializes and adds the <see cref="LinkingBaseTool.TemporaryFromNode"/>, <see cref="LinkingBaseTool.TemporaryToNode"/>,
    /// and <see cref="LinkingBaseTool.TemporaryLink"/> to the diagram's <see cref="Northwoods.GoXam.Diagram.PartsModel"/>.
    /// The temporary nodes that are positioned and sized to be like the real (<see cref="LinkingBaseTool.OriginalFromPort"/>
    /// and <see cref="LinkingBaseTool.OriginalToPort"/>) ports by calling <see cref="LinkingBaseTool.CopyPortProperties"/>.
    /// The temporary link connects the two temporary nodes, of course.
    /// </para>
    /// <para>
    /// The base <see cref="LinkingBaseTool.DoStart"/> method starts mouse capture.
    /// </para>
    /// </remarks>
    public override void DoActivate() {
      if (this.StartPort == null) this.StartPort = FindLinkablePort();
      if (this.StartPort == null) return;

      StartTransaction("NewLink");
      Diagram diagram = this.Diagram;
      diagram.Cursor = Cursors.Hand;

      this.TemporaryModel.SkipsUndoManager = true;
      this.TemporaryModel.AddNode(this.TemporaryFromNode);
      this.TemporaryModel.AddNode(this.TemporaryToNode);

      if (this.Forwards) {
        this.OriginalFromPort = this.StartPort;
        this.OriginalFromNode = Diagram.FindAncestor<Node>(this.OriginalFromPort);
        CopyPortProperties(this.OriginalFromNode, this.OriginalFromPort, this.TemporaryFromNode, this.TemporaryFromPort, false);
      } else {
        this.OriginalToPort = this.StartPort;
        this.OriginalToNode = Diagram.FindAncestor<Node>(this.OriginalToPort);
        CopyPortProperties(this.OriginalToNode, this.OriginalToPort, this.TemporaryToNode, this.TemporaryToPort, true);
      }

      if (this.TemporaryLink != null) {
        if (this.TemporaryFromNode != null) {
          this.TemporaryLink.PartsModelFromNode = this.TemporaryFromNode.Id;
        }
        if (this.TemporaryToNode != null) {
          this.TemporaryLink.PartsModelToNode = this.TemporaryToNode.Id;
        }
        this.TemporaryModel.AddLink(this.TemporaryLink);
      }
      this.TemporaryModel.SkipsUndoManager = false;

      this.Active = true;
    }

    /// <summary>
    /// Finishing the linking operation stops the model edit and resets the cursor.
    /// </summary>
    /// <remarks>
    /// The base <see cref="LinkingBaseTool.DoStop"/> method cleans up the temporary nodes and links
    /// and releases the mouse capture.
    /// </remarks>
    public override void DoDeactivate() {
      this.Active = false;
      StopTransaction();
      Diagram diagram = this.Diagram;
      diagram.Cursor = null;
    }

    /// <summary>
    /// Clean up tool state.
    /// </summary>
    public override void DoStop() {
      this.StartPort = null;
      this.StartElement = null;
      base.DoStop();
    }


    /// <summary>
    /// A mouse-up ends the linking operation; if there is a valid <see cref="LinkingBaseTool.TargetPort"/> nearby,
    /// this calls <see cref="IDiagramModel.AddLink"/> to create a new link.
    /// </summary>
    /// <remarks>
    /// If a new link is created in the model, the corresponding <see cref="Link"/> is selected in the diagram
    /// and the "link drawn" event is raised.
    /// In any case this stops the tool.
    /// </remarks>
    public override void DoMouseUp() {
      if (this.Active) {
        Diagram diagram = this.Diagram;
        if (diagram == null) return;
        PartManager mgr = diagram.PartManager;
        if (mgr == null) return;
        this.TransactionResult = null;

        Object fromdata = null;
        Object fromid = null;
        Object todata = null;
        Object toid = null;

        IDiagramModel model = null;
        this.TargetPort = FindTargetPort(this.Forwards);
        Node targetnode = null;
        if (this.TargetPort != null) {
          targetnode = Diagram.FindAncestor<Node>(this.TargetPort);
          if (targetnode != null) {
            if (this.Forwards) {
              if (this.OriginalFromNode != null) {
                fromdata = this.OriginalFromNode.Data;
                fromid = this.OriginalFromNode.GetPortName(this.OriginalFromPort);
              }
              todata = targetnode.Data;
              toid = targetnode.GetPortName(this.TargetPort);
            } else {
              fromdata = targetnode.Data;
              fromid = targetnode.GetPortName(this.TargetPort);
              if (this.OriginalToNode != null) {
                todata = this.OriginalToNode.Data;
                toid = this.OriginalToNode.GetPortName(this.OriginalToPort);
              }
            }
            model = mgr.FindCommonDataModel(fromdata, todata);
          }
        } else {  // not connecting to a port; set FROMDATA or TODATA, but not both
          if (this.Forwards) {
            if (this.OriginalFromNode != null) {
              ILinksModel lmodel = this.OriginalFromNode.Model as ILinksModel;
              if (lmodel != null && lmodel.ValidUnconnectedLinks == ValidUnconnectedLinks.Allowed) {
                fromdata = this.OriginalFromNode.Data;
                fromid = this.OriginalFromNode.GetPortName(this.OriginalFromPort);
                model = lmodel;
              }
            }
          } else {
            if (this.OriginalToNode != null) {
              ILinksModel lmodel = this.OriginalToNode.Model as ILinksModel;
              if (lmodel != null && lmodel.ValidUnconnectedLinks == ValidUnconnectedLinks.Allowed) {
                todata = this.OriginalToNode.Data;
                toid = this.OriginalToNode.GetPortName(this.OriginalToPort);
                model = lmodel;
              }
            }
          }
        }
        if (model != null) {
          Object linkdata = model.AddLink(fromdata, fromid, todata, toid);
          Link link = null;
          if (linkdata != null) link = mgr.FindLinkForData(linkdata, model);
          if (link == null) link = mgr.FindLinkForData(fromdata, todata, model);
          if (link != null) {
            if (this.TargetPort == null) {
              // need to tell Route that it ought to end at Diagram.LastMousePointInModel
              if (this.Forwards) {
                link.Route.DefaultToPoint = this.Diagram.LastMousePointInModel;
              } else {
                link.Route.DefaultFromPoint = this.Diagram.LastMousePointInModel;
              }
            }
            if (diagram.AllowSelect) {
              diagram.Select(link);
            }
            // set the TransactionResult before raising event, in case it changes the result or cancels the tool
            this.TransactionResult = "NewLink";
            RaiseEvent(Diagram.LinkDrawnEvent, new DiagramEventArgs(link));
          } else {
            model.ClearUnresolvedReferences();
          }
        }
      }
      StopTool();
    }

  }  // end LinkingTool


  /// <summary>
  /// The <c>RelinkingTool</c> allows the user to reconnect an existing <see cref="Link"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// By default an instance of this tool is installed as a mouse-down tool in each <see cref="Northwoods.GoXam.Diagram"/>,
  /// as the value of <see cref="Northwoods.GoXam.Diagram.RelinkingTool"/>.
  /// </para>
  /// <para>
  /// This tool makes use of two <see cref="Adornment"/>s,
  /// each including a relink handle (one for each end of the link),
  /// shown when a link is selected.
  /// </para>
  /// <para>
  /// This tool conducts a model edit (<see cref="DiagramTool.StartTransaction"/> and <see cref="DiagramTool.StopTransaction"/>)
  /// while the tool is <see cref="DiagramTool.Active"/>.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class RelinkingTool : LinkingBaseTool {

    /// <summary>
    /// Gets or sets the relinking tool handle that the user is using (dragging).
    /// </summary>
    protected FrameworkElement Handle { get; set; }

    /// <summary>
    /// Show an <see cref="Adornment"/> for each end of the <see cref="Link"/> that
    /// the user may reconnect.
    /// </summary>
    /// <param name="part"></param>
    /// <remarks>
    /// <para>
    /// If the link and layer and diagram support relinking
    /// (<see cref="Link.CanRelinkFrom"/> and/or <see cref="Link.CanRelinkTo"/>),
    /// this creates <see cref="Adornment"/>s
    /// using the <see cref="Link.RelinkFromAdornmentTemplate"/> and/or
    /// <see cref="Link.RelinkToAdornmentTemplate"/>.
    /// If the <c>DataTemplate</c> is null, a default template is used instead,
    /// "DefaultRelinkFromAdornmentTemplate" or "DefaultRelinkToAdornmentTemplate".
    /// </para>
    /// </remarks>
    public override void UpdateAdornments(Part part) {
      Link link = part as Link;
      if (link == null) return;

      // show handles if link is selected, remove them if no longer selected
      bool isselected = link.IsSelected;

      Adornment adornment = null;
      String ToolCategory = "RelinkFrom";
      if (isselected) {
        FrameworkElement selelt = link.VisualElement;
        if (selelt != null && Part.IsVisibleElement(selelt) && link.CanRelinkFrom()) {
          adornment = link.GetAdornment(ToolCategory);
          if (adornment == null) {
            DataTemplate template = link.RelinkFromAdornmentTemplate;
            if (template == null) template = Diagram.FindDefault<DataTemplate>("DefaultRelinkFromAdornmentTemplate");
            adornment = link.MakeAdornment(selelt, template);
            if (adornment != null) adornment.Category = ToolCategory;
          }
          if (adornment != null) {
            Rect r = link.GetElementBounds(selelt);
            adornment.Position = new Point(r.X, r.Y);
            adornment.RotationAngle = link.GetAngle(selelt);
            adornment.Remeasure();
          }
        }
      }
      link.SetAdornment(ToolCategory, adornment);

      adornment = null;
      ToolCategory = "RelinkTo";
      if (isselected) {
        FrameworkElement selelt = link.VisualElement;
        if (selelt != null && Part.IsVisibleElement(selelt) && link.CanRelinkTo()) {
          adornment = link.GetAdornment(ToolCategory);
          if (adornment == null) {
            DataTemplate template = link.RelinkToAdornmentTemplate;
            if (template == null) template = Diagram.FindDefault<DataTemplate>("DefaultRelinkToAdornmentTemplate");
            adornment = link.MakeAdornment(selelt, template);
            if (adornment != null) adornment.Category = ToolCategory;
          }
          if (adornment != null) {
            Rect r = link.GetElementBounds(selelt);
            adornment.Position = new Point(r.X, r.Y);
            adornment.RotationAngle = link.GetAngle(selelt);
            adornment.Remeasure();
          }
        }
      }
      link.SetAdornment(ToolCategory, adornment);
    }


    /// <summary>
    /// This tool can run when the diagram allows relinking, the model is modifiable,
    /// and there is a relink handle at the mouse-down point.
    /// </summary>
    /// <returns></returns>
    public override bool CanStart() {
      if (!base.CanStart()) return false;

      Diagram diagram = this.Diagram;
      if (diagram == null || diagram.IsReadOnly) return false;
      if (!diagram.AllowRelink) return false;
      IDiagramModel model = diagram.Model;
      if (model == null || !model.Modifiable) return false;
      if (!IsLeftButtonDown()) return false;
      FrameworkElement h = FindToolHandleAt(diagram.FirstMousePointInModel, "RelinkFrom");
      if (h == null) h = FindToolHandleAt(diagram.FirstMousePointInModel, "RelinkTo");
      return h != null;
    }


    /// <summary>
    /// Start the relinking operation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Find the relink handle by calling <see cref="DiagramTool.FindToolHandleAt"/>
    /// looking for either the "RelinkFrom" handle or the "RelinkTo" handle,
    /// saving the result in <see cref="Handle"/>.
    /// </para>
    /// <para>
    /// This starts a model edit (<see cref="DiagramTool.StartTransaction"/>)
    /// and sets the cursor.
    /// </para>
    /// <para>
    /// The value of <see cref="LinkingBaseTool.Forwards"/> is set
    /// depending on the category of relink handle found.
    /// The <see cref="LinkingBaseTool.OriginalLink"/> property and
    /// various "Original..." port and node properties are set too.
    /// The temporary nodes and temporary link are also initialized.
    /// </para>
    /// <para>
    /// The base <see cref="LinkingBaseTool.DoStart"/> method starts mouse capture.
    /// </para>
    /// </remarks>
    public override void DoActivate() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      this.Handle = FindToolHandleAt(diagram.FirstMousePointInModel, "RelinkFrom");
      if (this.Handle == null) this.Handle = FindToolHandleAt(diagram.FirstMousePointInModel, "RelinkTo");
      if (this.Handle == null) return;

      StartTransaction("Relink");
      diagram.Cursor = Cursors.Hand;

      // "forwards" means the user started dragging the RelinkHandle at the "To" end of the link
      Adornment ad = FindAdornment(this.Handle);
      this.Forwards = (ad == null || ad.Category == "RelinkTo");

      this.OriginalLink = FindAdornedPart(this.Handle) as Link;
      this.OriginalFromPort = this.OriginalLink.FromPort;
      this.OriginalFromNode = this.OriginalLink.FromNode;
      this.OriginalToPort = this.OriginalLink.ToPort;
      this.OriginalToNode = this.OriginalLink.ToNode;
      this.OriginalLinkBounds = this.OriginalLink.Bounds;

      this.TemporaryModel.AddNode(this.TemporaryFromNode);

      this.TemporaryModel.AddNode(this.TemporaryToNode);

      // initialize temporary node/port for the end that is not being relinked
      if (this.Forwards) {
        CopyPortProperties(this.OriginalFromNode, this.OriginalFromPort, this.TemporaryFromNode, this.TemporaryFromPort, true);
      } else {
        CopyPortProperties(this.OriginalToNode, this.OriginalToPort, this.TemporaryToNode, this.TemporaryToPort, false);
      }
      // in case either end is not connected to a node/port, initialize the temporary node/port
      if (this.OriginalLink != null && this.OriginalLink.Route.PointsCount > 0) {
        if (this.OriginalFromNode == null) {
          if (this.TemporaryFromPort != null) {
            this.TemporaryFromPort.Width = 0;
            this.TemporaryFromPort.Height = 0;
          }
          if (this.TemporaryFromNode != null) {
            this.TemporaryFromNode.Location = this.OriginalLink.Route.GetPoint(0);
          }
        }
        if (this.OriginalToNode == null) {
          if (this.TemporaryToPort != null) {
            this.TemporaryToPort.Width = 0;
            this.TemporaryToPort.Height = 0;
          }
          if (this.TemporaryToNode != null) {
            this.TemporaryToNode.Location = this.OriginalLink.Route.GetPoint(this.OriginalLink.Route.PointsCount-1);
          }
        }
      }

      if (this.TemporaryLink != null) {
        if (this.TemporaryFromNode != null) {
          this.TemporaryLink.PartsModelFromNode = this.TemporaryFromNode.Id;
        }
        if (this.TemporaryToNode != null) {
          this.TemporaryLink.PartsModelToNode = this.TemporaryToNode.Id;
        }
        this.TemporaryModel.AddLink(this.TemporaryLink);
        this.TemporaryLink.Route.InvalidateRoute();
      }

      this.Active = true;
    }

    private Rect OriginalLinkBounds { get; set; }

    /// <summary>
    /// Finishing the relinking operation stops the model edit and resets the cursor.
    /// </summary>
    /// <remarks>
    /// The base <see cref="LinkingBaseTool.DoStop"/> method cleans up the temporary nodes and links
    /// and releases the mouse capture.
    /// </remarks>
    public override void DoDeactivate() {
      this.Active = false;
      StopTransaction();
      this.Handle = null;
      Diagram diagram = this.Diagram;
      diagram.Cursor = null;
    }

    /// <summary>
    /// Clean up tool state.
    /// </summary>
    public override void DoStop() {
      this.Handle = null;
      base.DoStop();
    }


    // used by DraggingTool for dragging a (perhaps partly or fully unconnected) Link
    internal void DoDraggingMouseMove(Node fromnode, FrameworkElement fromport, Node tonode, FrameworkElement toport) {
      if (this.TemporaryFromNode == null) this.TemporaryFromNode = CreateTemporaryNode(false);
      this.TemporaryModel.AddNode(this.TemporaryFromNode);
      if (this.TemporaryToNode == null) this.TemporaryToNode = CreateTemporaryNode(true);
      this.TemporaryModel.AddNode(this.TemporaryToNode);

      if (fromnode != null) {
        CopyPortProperties(fromnode, fromport, this.TemporaryFromNode, this.TemporaryFromPort, false);
      } else {
        SetNoTargetPortProperties(this.TemporaryFromNode, this.TemporaryFromPort);
      }
      if (tonode != null) {
        CopyPortProperties(tonode, toport, this.TemporaryToNode, this.TemporaryToPort, true);
      } else {
        SetNoTargetPortProperties(this.TemporaryToNode, this.TemporaryToPort);
      }
    }

    internal void StopDraggingMouseMove() {
      this.TemporaryModel.RemoveNode(this.TemporaryFromNode);
      this.TemporaryModel.RemoveNode(this.TemporaryToNode);
    }


    /// <summary>
    /// A mouse-up ends the relinking operation; if there is a valid <see cref="LinkingBaseTool.TargetPort"/> nearby,
    /// this calls <see cref="IDiagramModel.RemoveLink"/> to delete the old link, and
    /// <see cref="IDiagramModel.AddLink"/> to create the replacement link.
    /// </summary>
    /// <remarks>
    /// The corresponding new <see cref="Link"/> is selected in the diagram
    /// and the "link relinked" event is raised.
    /// In any case this stops the tool.
    /// </remarks>
    public override void DoMouseUp() {
      if (this.Active) {
        Diagram diagram = this.Diagram;
        if (diagram == null) return;
        PartManager mgr = diagram.PartManager;
        if (mgr == null) return;
        this.TransactionResult = null;

        IDiagramModel oldmodel = null;
        if (this.OriginalLink != null) oldmodel = this.OriginalLink.Model;
        Object oldfromdata = null;
        Object oldfromid = null;
        if (this.OriginalFromNode != null) {
          if (oldmodel == null) oldmodel = this.OriginalFromNode.Model;
          oldfromdata = this.OriginalFromNode.Data;
          oldfromid = this.OriginalFromNode.GetPortName(this.OriginalFromPort);
        }
        Object oldtodata = null;
        Object oldtoid = null;
        if (this.OriginalToNode != null) {
          if (oldmodel == null) oldmodel = this.OriginalToNode.Model;
          oldtodata = this.OriginalToNode.Data;
          oldtoid = this.OriginalToNode.GetPortName(this.OriginalToPort);
        }

        IDiagramModel newmodel = oldmodel;
        Object newfromdata = oldfromdata;
        Object newfromid = oldfromid;
        Object newtodata = oldtodata;
        Object newtoid = oldtoid;

        this.TargetPort = FindTargetPort(this.Forwards);
        if (this.TargetPort != null) {
          Node targetnode = Diagram.FindAncestor<Node>(this.TargetPort);
          if (targetnode != null) {
            if (this.Forwards) {
              newtodata = targetnode.Data;
              newtoid = targetnode.GetPortName(this.TargetPort);
              newmodel = mgr.FindCommonDataModel(oldfromdata, newtodata);
            } else {
              newfromdata = targetnode.Data;
              newfromid = targetnode.GetPortName(this.TargetPort);
              newmodel = mgr.FindCommonDataModel(newfromdata, oldtodata);
            }
          }
        } else {  // no target port found, disconnect if allowed by the model
          ILinksModel lmodel = oldmodel as ILinksModel;
          if (lmodel != null && lmodel.ValidUnconnectedLinks == ValidUnconnectedLinks.Allowed) {
            if (this.Forwards) {
              newtodata = null;
              newtoid = null;
            } else {
              newfromdata = null;
              newfromid = null;
            }
          }
        }

        // for ILinksModel, change link data's ToNode & ToPort (or FromNode & FromPort if !Forwards);
        // otherwise, call RemoveLink and AddLink
        if (oldmodel != null && newmodel != null) {
          ILinksModel oldlmodel = oldmodel as ILinksModel;
          ILinksModel newlmodel = newmodel as ILinksModel;

          Link newlink = null;
          if (oldlmodel != null) {  // an ILinksModel
            Object oldlinkdata = this.OriginalLink.Data;
            if (newlmodel == oldlmodel) {  // modify the link data
              // setting any of the from/to node/portparam data should cause
              // the PartManager to remove the old Link and create a new one
              if (this.Forwards) {
                newlmodel.SetLinkToPort(oldlinkdata, newtodata, newtoid);
              } else {
                newlmodel.SetLinkFromPort(oldlinkdata, newfromdata, newfromid);
              }
              newlink = mgr.FindLinkForData(oldlinkdata, newlmodel);
            } else {  // just remove the old link data and add a new one
              oldlmodel.RemoveLink(oldlinkdata);
              Object newlinkdata = newmodel.AddLink(newfromdata, newfromid, newtodata, newtoid);
              if (newlmodel != null) {
                newlink = mgr.FindLinkForData(newlinkdata, newlmodel);
              } else {
                newlink = mgr.FindLinkForData(newfromdata, newtodata, newmodel);
              }
            }
          } else  {  // an ITreeModel or IConnectedModel
            // always remove the old link and add a new one
            oldmodel.RemoveLink(oldfromdata, oldfromid, oldtodata, oldtoid);
            Object newlinkdata = newmodel.AddLink(newfromdata, newfromid, newtodata, newtoid);
            if (newlmodel != null) {
              newlink = mgr.FindLinkForData(newlinkdata, newlmodel);
            } else {
              newlink = mgr.FindLinkForData(newfromdata, newtodata, newmodel);
            }
          }

          if (newlink != null) {
            if (this.TargetPort == null) {
              // need to tell Route that it ought to end at Diagram.LastMousePointInModel
              if (this.Forwards) {
                newlink.Route.DefaultToPoint = this.Diagram.LastMousePointInModel;
              } else {
                newlink.Route.DefaultFromPoint = this.Diagram.LastMousePointInModel;
              }
              newlink.Route.InvalidateRoute();
            }
            // select the link
            if (diagram.AllowSelect) newlink.IsSelected = true;
            // set the TransactionResult before raising event, in case it changes the result or cancels the tool
            this.TransactionResult = "Relink";
            RaiseEvent(Diagram.LinkRelinkedEvent, new DiagramEventArgs(newlink));
          } else {
            newmodel.ClearUnresolvedReferences();
          }

          // fix any jump-overs
          diagram.Panel.InvalidateJumpOverLinks(this.OriginalLinkBounds);
        }  // else if no OLDMODEL, don't do anything
      }
      StopTool();
    }

  }  // end RelinkingTool
}
