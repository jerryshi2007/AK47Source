
/*
 *  Copyright © Northwoods Software Corporation, 1998-2010. All Rights Reserved.
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
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Northwoods.GoXam.Model;

//?? Silverlight or Xbap: improving dragging between Diagrams
//?? DragsRealtime (assumed to be true)

namespace Northwoods.GoXam.Tool {

  /// <summary>
  /// The <c>DraggingTool</c> is used to move or copy selected parts with the mouse.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This tool does not utilize any <see cref="Adornment"/>s or tool handles.
  /// </para>
  /// <para>
  /// This tool conducts a model edit (<see cref="DiagramTool.StartTransaction"/> and <see cref="DiagramTool.StopTransaction"/>)
  /// while the tool is <see cref="DiagramTool.Active"/>.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class DraggingTool : DiagramTool {

    static DraggingTool() {
      CopiesEffectiveCollectionProperty = DependencyProperty.Register("CopiesEffectiveCollection", typeof(bool), typeof(DraggingTool),
        new FrameworkPropertyMetadata(false));
      InclusionsProperty = DependencyProperty.Register("Inclusions", typeof(EffectiveCollectionInclusions), typeof(DraggingTool),
        new FrameworkPropertyMetadata(EffectiveCollectionInclusions.Standard));
      DragOverSnapAreaProperty = DependencyProperty.Register("DragOverSnapArea", typeof(DragOverSnapArea), typeof(DraggingTool),
        new FrameworkPropertyMetadata(DragOverSnapArea.Diagram));
      DropOntoEnabledProperty = DependencyProperty.Register("DropOntoEnabled", typeof(bool), typeof(DraggingTool),
        new FrameworkPropertyMetadata(false));

      // these two are for DropOnto when behavior is AddsLinkFromNode, AddsLinkToNode, or SplicesIntoLink:
      FromPortIdProperty = DependencyProperty.Register("FromPortId", typeof(String), typeof(DraggingTool),
        new FrameworkPropertyMetadata(null));
      ToPortIdProperty = DependencyProperty.Register("ToPortId", typeof(String), typeof(DraggingTool),
        new FrameworkPropertyMetadata(null));
    }

    /// <summary>
    /// Identifies the <see cref="CopiesEffectiveCollection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CopiesEffectiveCollectionProperty;
    /// <summary>
    /// Gets or sets whether for a copying operation the extended selection
    /// is copied or only selected parts.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    /// <remarks>
    /// Basically this controls whether <see cref="PartManager.CopyParts"/> is
    /// called with the <see cref="Diagram.SelectedParts"/> or with the possibly
    /// augmented collection returned by <see cref="ComputeEffectiveCollection"/>.
    /// The latter collection typically will include all of the <see cref="Link"/>s
    /// that connect selected nodes, even if they are not <see cref="Part.IsSelected"/>.
    /// </remarks>
    public bool CopiesEffectiveCollection {
      get { return (bool)GetValue(CopiesEffectiveCollectionProperty); }
      set { SetValue(CopiesEffectiveCollectionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Inclusions"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InclusionsProperty;
    /// <summary>
    /// Gets or sets whether moving or copying a node also includes all of the
    /// node's children and their descendants, along with the links to those additional nodes.
    /// </summary>
    /// <value>
    /// The default value is <see cref="EffectiveCollectionInclusions.Standard"/>.
    /// </value>
    /// <remarks>
    /// When set to <see cref="EffectiveCollectionInclusions.SubTree"/>,
    /// <see cref="ComputeEffectiveCollection"/> will augment the collection of
    /// selected nodes to include their tree children nodes.
    /// The resulting collection often will include many <see cref="Node"/>s and
    /// <see cref="Link"/>s that are not <see cref="Part.IsSelected"/>.
    /// </remarks>
    public EffectiveCollectionInclusions Inclusions {
      get { return (EffectiveCollectionInclusions)GetValue(InclusionsProperty); }
      set { SetValue(InclusionsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DragOverSnapArea"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DragOverSnapAreaProperty;
    /// <summary>
    /// Gets or sets whether dragging any parts over the diagram or over any parts causes their position to be snapped to grid points.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Northwoods.GoXam.Tool.DragOverSnapArea.Diagram"/>.
    /// </value>
    /// <remarks>
    /// This property is independent of <see cref="DropOntoEnabled"/>
    /// </remarks>
    public DragOverSnapArea DragOverSnapArea {
      get { return (DragOverSnapArea)GetValue(DragOverSnapAreaProperty); }
      set { SetValue(DragOverSnapAreaProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DropOntoEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DropOntoEnabledProperty;
    /// <summary>
    /// Gets or sets whether stationary parts get their <see cref="Part.IsDropOntoAccepted"/>
    /// property temporarily set to true during a drag.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    /// <remarks>
    /// <para>
    /// Basically this controls whether <see cref="DropOnto"/>
    /// performs additional side effects upon a drop,
    /// and whether <see cref="DragOver"/> maintains the <see cref="DragOverPart"/> property and
    /// sets the <see cref="Part.IsDropOntoAccepted"/> property to support drag-over highlighting.
    /// </para>
    /// <para>
    /// This property is independent of <see cref="DragOverSnapArea"/>
    /// </para>
    /// </remarks>
    public bool DropOntoEnabled {
      get { return (bool)GetValue(DropOntoEnabledProperty); }
      set { SetValue(DropOntoEnabledProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="FromPortId"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FromPortIdProperty;
    /// <summary>
    /// Gets or sets the port identifier used when <see cref="DropOnto"/>
    /// creates a new link due to <see cref="Part.DropOntoBehavior"/> is
    /// <see cref="DropOntoBehavior.AddsLinkFromNode"/>, <see cref="DropOntoBehavior.AddsLinkToNode"/>,
    /// or <see cref="DropOntoBehavior.SplicesIntoLink"/>.
    /// </summary>
    /// <value>
    /// By default the value is null. 
    /// </value>
    public String FromPortId {
      get { return (String)GetValue(FromPortIdProperty); }
      set { SetValue(FromPortIdProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ToPortId"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ToPortIdProperty;
    /// <summary>
    /// Gets or sets the port identifier used when <see cref="DropOnto"/>
    /// creates a new link due to <see cref="Part.DropOntoBehavior"/> is
    /// <see cref="DropOntoBehavior.AddsLinkFromNode"/>, <see cref="DropOntoBehavior.AddsLinkToNode"/>,
    /// or <see cref="DropOntoBehavior.SplicesIntoLink"/>.
    /// </summary>
    /// <value>
    /// By default the value is null.
    /// </value>
    public String ToPortId {
      get { return (String)GetValue(ToPortIdProperty); }
      set { SetValue(ToPortIdProperty, value); }
    }


    /// <summary>
    /// This simple class provides temporary information about each dragged or copied <see cref="Part"/>,
    /// as values in the <see cref="DraggedParts"/> and <see cref="CopiedParts"/> dictionaries.
    /// </summary>
    public sealed class Info {  // nested class
      /// <summary>
      /// Gets or sets the original <see cref="Node.Location"/> of the <see cref="Part"/>.
      /// </summary>
      /// <value>
      /// This property is used by <see cref="MoveParts"/>.
      /// </value>
      /// <remarks>
      /// This tool needs to remember the original location of each dragged part so that
      /// independent movement of each node, particularly in the presence of drag snapping
      /// (for example, over grids), can occur while trying to maintain the same relative
      /// positioning of the parts.
      /// </remarks>
      public Point Point { get; set; }

      internal Point Shifted { get; set; }
    }  // end DraggingTool.Info

    private class NodeInfoPair {
      public Node Node { get; set; }
      public Info Info { get; set; }
      public Info GroupInfo { get; set; }
    }


    /// <summary>
    /// Gets or sets the <see cref="Part"/> found at the mouse point by <see cref="StandardMouseSelect"/>.
    /// </summary>
    /// <value>
    /// This property is set during <see cref="DoActivate"/> by <see cref="StandardMouseSelect"/>.
    /// </value>
    protected Part CurrentPart { get; set; }

    /// <summary>
    /// Gets or sets the collection of <see cref="Part"/>s being dragged.
    /// </summary>
    /// <value>
    /// The value is a <c>Dictionary</c> mapping <see cref="Part"/>s to <see cref="Info"/>s
    /// holding dragging information about each part.
    /// This property is set by <see cref="DoActivate"/> with the result from calling <see cref="ComputeEffectiveCollection"/>.
    /// </value>
    public Dictionary<Part, Info> DraggedParts { get; set; }

    /// <summary>
    /// Gets or sets whether a potentially external drag-and-drop has been started by this tool.
    /// </summary>
    /// <value>
    /// This property is set to true when starting a drag-and-drop in <c>DoDragOut</c>.
    /// </value>
    protected bool DragOutStarted { get; set; }

    /// <summary>
    /// Gets or sets whether a drop has occurred after a drag-out had started.
    /// </summary>
    /// <value>
    /// This property is set to false by <see cref="DoDragOut"/>, and is set to true upon a drop.
    /// </value>
    internal bool Dropped { get; set; }

    internal Point StartPanelPoint { get; set; }

    /// <summary>
    /// Gets or sets the mouse point from which parts start to move.
    /// </summary>
    /// <value>
    /// The value is a <c>Point</c> in model coordinates.
    /// This property is normally set to the diagram's mouse-down point,
    /// but may be set to a different point if parts are being copied
    /// from a different control.
    /// </value>
    protected Point StartPoint { get; set; }


    /// <summary>
    /// Gets or sets the collection of parts that this tool has copied.
    /// </summary>
    /// <value>
    /// The value is a <c>Dictionary</c> mapping <see cref="Part"/>s to <see cref="Info"/>s
    /// holding dragging information about each part.
    /// This property is set when there is a control-drag from within the diagram
    /// (<see cref="DoMouseMove"/> or <see cref="DoMouseUp"/>).
    /// </value>













    public Dictionary<Part, Info> CopiedParts { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="Part"/>, not being dragged, that the mouse is over,
    /// and sets its <see cref="Part.IsDropOntoAccepted"/> property appropriately.
    /// </summary>
    /// <value>
    /// This property is both used and set by <see cref="DragOver"/> and <see cref="DropOnto"/>,
    /// as long as <see cref="DropOntoEnabled"/> is true.
    /// </value>
    protected Part DragOverPart {
      get { return _DragOverPart; }
      set {
        Part old = _DragOverPart;
        if (old != value) {
          if (old != null) {
            old.IsDropOntoAccepted = false;
            old.Remeasure();
          }
          _DragOverPart = value;
          if (value != null) {
            value.IsDropOntoAccepted = true;
            value.Remeasure();
          }
        }
      }
    }
    private Part _DragOverPart;

    private bool SuspendedRouting { get; set; }

    private Node CurrentSnapper { get; set; }


    /// <summary>
    /// This tool can run if this diagram allows selection and moves/copies/dragging-out,
    /// if the mouse has moved far enough away to be a drag and not a click,
    /// and if <see cref="FindDraggablePart"/> has found a selectable part at the mouse-down point.
    /// </summary>
    /// <returns></returns>
    public override bool CanStart() {
      if (!base.CanStart()) return false;

      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      if (!diagram.AllowMove && !diagram.AllowCopy && !diagram.AllowDragOut) return false;
      if (!diagram.AllowSelect) return false;

      // require left button & that it has moved far enough away from the mouse down point, so it isn't a click
      if (!IsLeftButtonDown()) return false;
      // don't include the following check when this tool is running modally
      if (diagram.CurrentTool != this) {
        if (!IsBeyondDragSize()) return false;
      }

      // need to be over a part that is movable or copyable
      Part part = FindDraggablePart();
      return (part != null);
    }

    /// <summary>
    /// Return the selectable and movable/copyable <see cref="Part"/> at the mouse-down point.
    /// </summary>
    /// <returns>
    /// null if there is no selectable <see cref="Part"/> at the point,
    /// or if <see cref="Part.CanMove"/> and <see cref="Part.CanCopy"/> are both false.
    /// </returns>
    protected virtual Part FindDraggablePart() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      Part part = FindPartAt(diagram.FirstMousePointInModel, true);
      if (part != null && (part.CanMove() || part.CanCopy())) return part;
      return null;
    }

    /// <summary>
    /// Don't have the Control modifier unselect an already selected part.
    /// </summary>
    /// <remarks>
    /// This also remembers the selectable <see cref="CurrentPart"/> at the
    /// current mouse point.
    /// </remarks>
    protected override void StandardMouseSelect() {
      Diagram diagram = this.Diagram;
      if (diagram == null || !diagram.AllowSelect) return;
      this.CurrentPart = FindPartAt(diagram.FirstMousePointInModel, true);
      if (this.CurrentPart == null) return;

      // change behavior of StandardMouseSelect because we don't want the Control modifier to unselect
      // an already selected object
      if (!this.CurrentPart.IsSelected) {
        if (!IsControlKeyDown() && !IsShiftKeyDown()) {
          diagram.ClearSelection();
        }
        this.CurrentPart.IsSelected = true;
      }
    }

    /// <summary>
    /// Start the dragging operation.
    /// </summary>
    /// <remarks>
    /// Select the <see cref="CurrentPart"/>,
    /// set <see cref="DraggedParts"/> to be the result of <see cref="ComputeEffectiveCollection"/>,
    /// start a model edit (<see cref="DiagramTool.StartTransaction"/>),
    /// and either capture the mouse, or in WPF if <see cref="Diagram.AllowDragOut"/> is true,
    /// start a drag-and-drop by calling <see cref="DoDragOut"/>.
    /// </remarks>
    public override void DoActivate() {
      StandardMouseSelect();
      Part mainnode = this.CurrentPart;
      if (mainnode == null || (!mainnode.CanMove() && !mainnode.CanCopy())) return;
      Diagram diagram = this.Diagram;
      if (diagram == null) return;

      this.Active = true;
      if (diagram.Panel != null) this.StartPanelPoint = diagram.Panel.Position;
      this.DraggedParts = ComputeEffectiveCollection(diagram.SelectedParts);
      SuspendRouting(this.DraggedParts);
      StartTransaction("Drag");
      this.StartPoint = diagram.FirstMousePointInModel;
      if (diagram.AllowDragOut) {
        DoDragOut();
      } else {
        CaptureMouse();
      }
    }


    /// <summary>
    /// Find the actual collection of nodes and links to be moved or copied,
    /// given an initial collection.
    /// </summary>
    /// <param name="parts"></param>
    /// <returns>
    /// a <c>Dictionary</c> of <see cref="Part"/>s,
    /// mapped to <see cref="Info"/>s holding their original <see cref="Node.Location"/>s
    /// </returns>
    /// <remarks>
    /// <para>
    /// Besides the <see cref="Part"/>s in the <paramref name="parts"/> collection,
    /// the result collection will include all member nodes and links,
    /// links whose connected nodes are both in the effective collection,
    /// and any link labels.
    /// This means that sometimes some or many of the parts that are dragged
    /// are not actually selected.
    /// </para>
    /// <para>
    /// You may want to override this method to include additional parts
    /// that are related logically in some manner particular to your application.
    /// To handle the common case of wanting to move all of the tree-structure children
    /// of selected nodes, you can just set <see cref="Inclusions"/> to
    /// <see cref="EffectiveCollectionInclusions.SubTree"/>.
    /// </para>
    /// </remarks>
    public virtual Dictionary<Part, Info> ComputeEffectiveCollection(IEnumerable<Part> parts) {
      bool dragging = (this.Diagram != null && this.Diagram.CurrentTool == this);
      Dictionary<Part, Info> map = new Dictionary<Part, Info>();
      if (parts == null) return map;
      foreach (Part p in parts) {
        GatherMembers(map, p, dragging);
      }
      // ignore any links that are connected to nodes not in the effective collection
      foreach (Part p in parts) {
        Link l = p as Link;
        if (l != null) {
          Node from = l.FromNode;
          if (from != null && !map.ContainsKey(from)) {
            map.Remove(l);
          } else {
            Node to = l.ToNode;
            if (to != null && !map.ContainsKey(to)) {
              map.Remove(l);
            }
          }
        }
      }
      return map;
    }

    private ICopyDictionary CopyParts(IDiagramModel model) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      PartManager mgr = diagram.PartManager;
      if (mgr == null) return null;
      ICopyDictionary dict;
      if (this.CopiesEffectiveCollection) {
        dict = mgr.CopyParts(this.DraggedParts.Keys, model);
      } else {
        dict = mgr.CopyParts(diagram.SelectedParts, model);
      }
      return dict;
    }

    private void GatherMembers(Dictionary<Part, Info> map, Part p, bool dragging) {
      if (map.ContainsKey(p)) return;
      // only check CanMove or CanCopy when dragging interactively, because this is the Diagram.CurrentTool
      if (dragging && !p.CanMove() && !p.CanCopy()) return;
      Node n = p as Node;
      if (n != null) {
        map[n] = new Info() { Point = n.Location };
        if (((int)this.Inclusions & 1 /*?? EffectiveCollectionInclusions.Members */) != 0) {
          Group g = n as Group;
          if (g != null) {
            foreach (Node c in g.MemberNodes) {
              GatherMembers(map, c, dragging);
            }
            foreach (Link c in g.MemberLinks) {
              GatherMembers(map, c, dragging);
            }
          }
        }
        if (((int)this.Inclusions & 2 /*?? EffectiveCollectionInclusions.ConnectingLinks */) != 0) {
          foreach (Link l in n.LinksConnected) {
            if (map.ContainsKey(l)) continue;
            Node from = l.FromNode;
            Node to = l.ToNode;
            if (from != null && map.ContainsKey(from) && to != null && map.ContainsKey(to)) {
              GatherMembers(map, l, dragging);
            }
          }
        }
        if ((this.Inclusions & EffectiveCollectionInclusions.TreeChildren) != 0) {
          foreach (Node c in n.NodesOutOf) {
            GatherMembers(map, c, dragging);
          }
        }
      } else {
        Link l = p as Link;
        if (l != null) {
          map[p] = new Info();
          if (((int)this.Inclusions & 4 /*?? EffectiveCollectionInclusions.LinkLabelNodes */) != 0) {
            Node lab = l.LabelNode;
            if (lab != null) {
              GatherMembers(map, lab, dragging);
            }
          }
        }
      }
    }

    /// <summary>
    /// Finish and clean-up after a dragging operation.
    /// </summary>
    /// <remarks>
    /// This calls <see cref="DiagramPanel.StopAutoScroll"/>,
    /// makes sure any <see cref="DragOverPart"/> is no longer dragged over,
    /// removes any copied parts that are no longer needed,
    /// releases the mouse if it was captured,
    /// and stops the transaction on the model.
    /// </remarks>
    public override void DoStop() {
      Diagram diagram = this.Diagram;
      if (diagram != null && diagram.Panel != null) diagram.Panel.StopAutoScroll();
      this.CurrentSnapper = null;
      this.DragOverPart = null;
      ResumeRouting(this.DraggedParts);
      this.DraggedParts = null;
      this.DragOutStarted = false;
      this.Dropped = false;

      DraggingTool.CleanUpDraggingTool();

      this.StartPanelPoint = new Point(Double.NaN, Double.NaN);
      DraggingTool.Root = null;
      DraggingTool.CurrentDiagram = null;
      DraggingTool.Source = null;
      RemoveCopies();
      ReleaseMouse();
      StopTransaction();
    }

    /// <summary>
    /// Abort any dragging operation.
    /// </summary>
    /// <remarks>
    /// Remove any copied parts,
    /// move dragged parts to their original locations,
    /// and stop this tool.
    /// </remarks>
    public override void DoCancel() {
      RemoveCopies();
      RestoreOriginalLocations();
      Diagram diagram = this.Diagram;
      if (diagram != null && diagram.Panel != null && !Double.IsNaN(this.StartPanelPoint.X)) {
        diagram.Panel.Position = this.StartPanelPoint;
      }
      StopTool();
    }


    private void SuspendRouting(Dictionary<Part, Info> parts) {
      if (parts == null) return;
      this.SuspendedRouting = true;
      foreach (Link link in parts.Keys.OfType<Link>()) {
        Route route = link.Route;
        if (route != null) {
          route.SuspendsRouting = true;
        }
      }
    }

    private void ResumeRouting(Dictionary<Part, Info> parts) {
      if (parts == null) return;
      if (!this.SuspendedRouting) return;
      foreach (Link link in parts.Keys.OfType<Link>()) {
        Route route = link.Route;
        if (route != null) {
          route.SuspendsRouting = false;
          // only need to re-route if the routing style is AvoidsNodes
          if (route.Routing == LinkRouting.AvoidsNodes) {
            route.InvalidateRoute();
          } else {
            link.Remeasure();
          }
        }
      }
      this.SuspendedRouting = false;
    }


    // DURING A DRAG, either moving or copying:

    /// <summary>
    /// Handle switching between copying and moving modes as the Control key
    /// is pressed or released.
    /// </summary>
    /// <param name="e"></param>
    public override void DoKeyDown(KeyEventArgs e) {
      if (e == null) return;
      if (!this.Active) return;
      if (e.Key == Key.Escape) {
        DoCancel();
      } else {
        MoveOnKey(e);
      }
    }

    /// <summary>
    /// Handle switching between copying and moving modes as the Control key
    /// is pressed or released.
    /// </summary>
    /// <param name="e"></param>
    public override void DoKeyUp(KeyEventArgs e) {
      if (e == null) return;
      if (!this.Active) return;
      MoveOnKey(e);
    }

    private void MoveOnKey(KeyEventArgs e) {

      if (e.Key == Key.Ctrl) DoMouseMove();



    }


    // DURING A DRAG: copying parts

    private bool CanCopyNode(Node n) {
      if (n == null) return false;
      if (!n.CanCopy()) return false;
      if (n.IsBoundToData) return true;
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      return (diagram.Model == diagram.PartsModel);
    }

    private bool CanCopyLink(Link l) {
      if (l == null) return false;
      if (!l.CanCopy()) return false;
      if (l.IsBoundToData) return true;
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      return (diagram.Model == diagram.PartsModel);
    }

    private void AddCopies(bool undoable) {
      if (this.CopiedParts == null) {
        Diagram diagram = this.Diagram;
        if (diagram == null || diagram.IsReadOnly /* || !diagram.AllowInsert */) return;
        IDiagramModel model = diagram.Model;  // this model must support the data
        if (model == null) return;
        PartManager mgr = diagram.PartManager;
        if (mgr == null) return;
        if (this.DraggedParts == null) return;
        RestoreOriginalLocations();
        mgr.TemporaryParts = !undoable;
        model.SkipsUndoManager = !undoable;

        this.StartPoint = diagram.FirstMousePointInModel;
        ICopyDictionary dict = CopyParts(model);
        IDataCollection copieddata = dict.Copies;

        Dictionary<Part, Info> copiedparts = new Dictionary<Part, Info>();
        foreach (Node n in this.DraggedParts.Keys.OfType<Node>().Where(CanCopyNode)) {
          Node c = mgr.FindNodeForData(dict.FindCopiedNode(n.Data), model);
          if (c != null) {
            copiedparts[c] = new Info() { Point = n.Location };
          }
        }
        foreach (Link l in mgr.FindLinksForData(copieddata)) {
          copiedparts[l] = new Info();
        }
        this.CopiedParts = copiedparts;

        mgr.TemporaryParts = false;
      }
    }

    private void RemoveCopies() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      IDiagramModel model = diagram.Model;
      if (diagram.PartManager != null && model != null && this.CopiedParts != null) {
        bool oldremove = model.Modifiable;
        model.Modifiable = true;
        diagram.PartManager.DeleteParts(this.CopiedParts.Keys);
        model.Modifiable = oldremove;
        this.CopiedParts = null;
      }
      if (model != null) {
        model.SkipsUndoManager = false;
      }
      this.StartPoint = diagram.FirstMousePointInModel;
    }


    // DURING A DRAG: moving parts (perhaps copied parts)

    private void MoveCollection(Dictionary<Part, Info> parts) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      Point oldpos = this.StartPoint;
      Point newpos = diagram.LastMousePointInModel;
      if ((this.DragOverSnapArea&DragOverSnapArea.Nodes) != 0 && diagram.Panel != null) {
        this.CurrentSnapper = diagram.Panel.FindElementAt<Node>(newpos, Diagram.FindAncestor<Node>,
                                 n => n.DragOverSnapEnabled && !parts.ContainsKey(n), SearchLayers.Nodes);
      } else {
        this.CurrentSnapper = null;
      }
      MoveParts(parts, Geo.Subtract(newpos, oldpos));
    }

    /// <summary>
    /// Move a collection of <see cref="Part"/>s by a given offset.
    /// </summary>
    /// <param name="parts">
    /// a <c>Dictionary</c> of parts,
    /// mapped to <see cref="Info"/> values that include the nodes' original locations
    /// </param>
    /// <param name="offset">a <c>Point</c> value in model coordinates</param>
    /// <remarks>
    /// This respects the <see cref="Part.CanMove"/> predicate for nodes
    /// when this is the <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>.
    /// This tries to avoid routing the links that are being moved.
    /// </remarks>
    public virtual void MoveParts(Dictionary<Part, Info> parts, Point offset) {
      if (parts == null || parts.Count == 0) return;
      if (Double.IsNaN(offset.X)) offset.X = 0;
      if (Double.IsNaN(offset.Y)) offset.Y = 0;
      //?? can't optimize a zero offset move, due to possible need to snap the nodes
      //if (offset.X == 0 && offset.Y == 0) return;

      bool dragging = (this.Diagram != null && this.Diagram.CurrentTool == this);
      bool wassuspended = this.SuspendedRouting;
      if (!wassuspended) SuspendRouting(parts);

      List<NodeInfoPair> nestednodes = new List<NodeInfoPair>();
      List<KeyValuePair<Part, Info>> linkkvps = new List<KeyValuePair<Part, Info>>();
      foreach (KeyValuePair<Part, Info> kvp in parts) {
        Node n = kvp.Key as Node;
        if (n != null) {
          Info ginfo = FindContainingInfo(n, parts);
          if (ginfo != null) {
            nestednodes.Add(new NodeInfoPair() { Node=n, Info=kvp.Value, GroupInfo=ginfo });
          } else if (!dragging || n.CanMove()) {  // only check CanMove() if the user is dragging
            Point oldloc = kvp.Value.Point;
            Point newloc = ComputeMove(n, new Point(oldloc.X + offset.X, oldloc.Y + offset.Y), parts);
            //Diagram.Debug("  " + Diagram.Str(n) + " from: " + Diagram.Str(oldloc) + " to: " + Diagram.Str(newloc));
            n.Location = newloc;
            kvp.Value.Shifted = Geo.Subtract(newloc, oldloc);
          }
        } else {
          Link l = kvp.Key as Link;
          if (l != null) linkkvps.Add(kvp);
        }
      }

      foreach (NodeInfoPair nip in nestednodes) {
        Point oldloc = nip.Info.Point;
        Point newloc = Geo.Add(oldloc, nip.GroupInfo.Shifted);
        nip.Node.Location = newloc;
      }

      foreach (KeyValuePair<Part, Info> kvp in linkkvps) {
        Link link = kvp.Key as Link;
        if (link == null) continue;
        Route route = link.Route;
        if (route != null && route.SuspendsRouting) {  // need to move the route explicitly
          Node from = link.FromNode;
          Node to = link.ToNode;
          if (from != null && to != null) {
            Info info;
            Point oldfromloc = new Point();
            if (parts.TryGetValue(from, out info)) oldfromloc = info.Point;
            Point newfromloc = from.Location;
            Point fromoff = Geo.Subtract(newfromloc, oldfromloc);
            Point oldtoloc = new Point();
            if (parts.TryGetValue(to, out info)) oldtoloc = info.Point;
            Point newtoloc = to.Location;
            Point tooff = Geo.Subtract(newtoloc, oldtoloc);
            // if both ends moved the same amount, can keep the original route
            if (Geo.IsApprox(fromoff, tooff)) {
              Point oldlinkoff = kvp.Value.Point;
              Point linkoff = Geo.Subtract(fromoff, oldlinkoff);
              parts[link] = new Info() { Point = fromoff };
              route.MovePoints(linkoff);
            } else {  // don't try to maintain shape of path
              route.SuspendsRouting = false;
              route.InvalidateRoute();
            }
          }
        }
      }

      if (!wassuspended) ResumeRouting(parts);
    }

    private Info FindContainingInfo(Node n, Dictionary<Part, Info> parts) {
      foreach (Group g in n.ContainingGroups) {
        Info info = FindContainingInfo(g, parts);
        if (info != null) return info;
        if (parts.TryGetValue(g, out info)) return info;
      }
      return null;
    }

    // move everything back to their original locations, respecting no rerouting of links in DraggedParts
    private void RestoreOriginalLocations() {
      if (this.DraggedParts != null) {
        List<KeyValuePair<Part, Info>> linkkvps = new List<KeyValuePair<Part, Info>>();
        foreach (KeyValuePair<Part, Info> kvp in this.DraggedParts) {
          Node n = kvp.Key as Node;
          if (n != null) {
            n.Location = kvp.Value.Point;
          } else {
            Link l = kvp.Key as Link;
            if (l != null) linkkvps.Add(kvp);
          }
        }
        foreach (KeyValuePair<Part, Info> kvp in linkkvps) {
          Link l = kvp.Key as Link;
          if (l != null) {
            Route route = l.Route;
            if (route.SuspendsRouting) {
              Point oldlinkoff = kvp.Value.Point;
              Point linkoff = Geo.Subtract(new Point(0, 0), oldlinkoff);
              this.DraggedParts[l] = new Info();
              route.MovePoints(linkoff);
            }
          }
        }
        Diagram diagram = this.Diagram;
        if (diagram != null && diagram.Panel != null) diagram.Panel.UpdateDiagramBounds();
      }
    }


    // DURING A DRAG: while moving parts, snapping to either nodes or the diagram's grid

    /// <summary>
    /// This predicate is true if the given <paramref name="snapper"/> node can control the movement
    /// of the given <paramref name="moving"/> node.
    /// </summary>
    /// <param name="moving">the <see cref="Northwoods.GoXam.Node"/> being dragged</param>
    /// <param name="pt">a <c>Point</c> in model coordinates, typically the current mouse point</param>
    /// <param name="snapper">the <see cref="Northwoods.GoXam.Node"/> that might control the <paramref name="moving"/> node's movement</param>
    /// <param name="draggedparts">a <c>Dictionary</c> of <see cref="Part"/>s being dragged</param>
    /// <returns>true if the <paramref name="snapper"/> is not in the <paramref name="draggedparts"/> and is <see cref="Part.DragOverSnapEnabled"/></returns>
    /// <remarks>
    /// This is called by <see cref="ComputeMove"/>.
    /// </remarks>
    protected virtual bool ConsiderSnapTo(Node moving, Point pt, Node snapper, Dictionary<Part, Info> draggedparts) {
      if (snapper == null) return false;
      if (!snapper.DragOverSnapEnabled) return false;
      return draggedparts != null && !draggedparts.ContainsKey(snapper);
    }

    /// <summary>
    /// Compute the new location for a node, given another node that is controlling its movement.
    /// </summary>
    /// <param name="moving">the <see cref="Northwoods.GoXam.Node"/> being dragged</param>
    /// <param name="pt">the <c>Point</c> in model coordinates to which the node is being dragged</param>
    /// <param name="snapper">the <see cref="Northwoods.GoXam.Node"/>, typically a grid, which is controlling the drag snapping</param>
    /// <param name="draggedparts">a <c>Dictionary</c> of <see cref="Part"/>s being dragged</param>
    /// <returns>a new node location, in model coordinates</returns>
    /// <remarks>
    /// <para>
    /// This is called by <see cref="ComputeMove"/>.
    /// </para>
    /// <para>
    /// When there is a <paramref name="snapper"/>, 
    /// if the <paramref name="snapper"/>'s <see cref="Part.DragOverSnapEnabled"/> property is false,
    /// this method just returns the unmodified <paramref name="pt"/> value.
    /// Otherwise this uses the <see cref="Part.DragOverSnapCellSize"/> and
    /// <see cref="Part.DragOverSnapCellSpot"/>
    /// to compute the nearest grid point that should be the location of the
    /// <paramref name="moving"/> node.
    /// </para>
    /// <para>
    /// If there is no <paramref name="snapper"/>, this uses the <see cref="Northwoods.GoXam.Diagram"/>'s
    /// <see cref="Northwoods.GoXam.Diagram.GridSnapCellSize"/>,
    /// <see cref="Northwoods.GoXam.Diagram.GridSnapCellSpot"/>, and
    /// <see cref="Northwoods.GoXam.Diagram.GridSnapOrigin"/>
    /// properties to calculate and return the nearest grid point.
    /// </para>
    /// </remarks>
    protected virtual Point SnapTo(Node moving, Point pt, Node snapper, Dictionary<Part, Info> draggedparts) {
      if (moving == null) return pt;
      if (snapper == null) {
        Diagram diagram = this.Diagram;
        if (diagram == null || !diagram.GridSnapEnabled) return pt;
        Size cell = diagram.GridSnapCellSize;
        Point origin = diagram.GridSnapOrigin;
        Point s = diagram.GridSnapCellSpot.PointInRect(new Rect(0, 0, cell.Width, cell.Height));
        Point q = FindNearestInfiniteGridPoint(pt, s, origin, cell);
        return q;
      } else {
        if (!snapper.DragOverSnapEnabled) return pt;

        Rect r = snapper.GetElementBounds(snapper.LocationElement);
        if (Double.IsNaN(r.X) || Double.IsNaN(r.Y)) return pt;

        Rect mb = moving.Bounds;
        Point oldloc = moving.Location;
        //if (ds.KeepsOutside) {  //???
        //  Point p = oldloc;
        //  if (oldloc.X <= r.Left) {
        //    p.X = r.Left - mb.Width + (oldloc.X-mb.X);
        //  } else if (oldloc.X >= r.Right) {
        //    p.X = r.Right + (oldloc.X-mb.X);
        //  }
        //  if (oldloc.Y <= r.Top) {
        //    p.Y = r.Top - mb.Height + (oldloc.Y-mb.Y);
        //  } else if (oldloc.Y >= r.Bottom) {
        //    p.Y = r.Bottom + (oldloc.Y-mb.Y);
        //  }
        //  return p;
        //}

        double left = Math.Max(0, oldloc.X - mb.Left);
        double right = Math.Max(0, mb.Right - oldloc.X);
        double top = Math.Max(0, oldloc.Y - mb.Top);
        double bottom = Math.Max(0, mb.Bottom - oldloc.Y);

        Size cell = snapper.DragOverSnapCellSize;
        Point origin = snapper.DragOverSnapOrigin;
        r.X += origin.X;
        r.Y += origin.Y;
        Point s = snapper.DragOverSnapCellSpot.PointInRect(new Rect(0, 0, cell.Width, cell.Height));
        Point q = FindNearestInfiniteGridPoint(pt, s, new Point(r.X, r.Y), cell);

        if (q.X - left < r.Left) {
          q.X += cell.Width * (int)Math.Ceiling((r.Left-(q.X-left))/cell.Width);
        }
        if (q.X + right > r.Right) {
          q.X -= cell.Width * (int)Math.Ceiling(((q.X+right)-r.Right)/cell.Width);
        }
        if (right > 0 && q.X - left < r.Left) {
          q.X += cell.Width * (int)Math.Ceiling((r.Left-(q.X-left))/cell.Width);
        }

        if (q.Y - top < r.Top) {
          q.Y += cell.Height * (int)Math.Ceiling((r.Top-(q.Y-top))/cell.Height);
        }
        if (q.Y + bottom > r.Bottom) {
          q.Y -= cell.Height * (int)Math.Ceiling(((q.Y+bottom)-r.Bottom)/cell.Height);
        }
        if (bottom > 0 && q.Y - top < r.Top) {
          q.Y += cell.Height * (int)Math.Ceiling((r.Top-(q.Y-top))/cell.Height);
        }
        return q;
      }
    }

    internal static Point FindNearestInfiniteGridPoint(Point p, Point offset, Point origin, Size cellsize) {
      // the grid space looks like the figure below.
      //  +               +
      //   A(ax, ay)
      //
      //        *p(x,y)
      //  +               +

      double x = p.X;
      double y = p.Y;
      double originX = origin.X + offset.X;
      double originY = origin.Y + offset.Y;

      double gridW = cellsize.Width;
      double gridH = cellsize.Height;

      double ax = (Math.Floor((x-originX)/gridW))*gridW + originX;
      double ay = (Math.Floor((y-originY)/gridH))*gridH + originY;

      double bestx = ax;
      if ((ax+gridW - x) < gridW/2)
        bestx = ax+gridW;
      double besty = ay;
      if ((ay+gridH - y) < gridH/2)
        besty = ay+gridH;

      return new Point(bestx, besty);
    }

    /// <summary>
    /// This method computes the new location for a node, given a new desired location
    /// and a dictionary of dragged parts.
    /// </summary>
    /// <param name="n">the <see cref="Node"/> being moved</param>
    /// <param name="newloc">the proposed new <see cref="Node.Location"/> for the node <paramref name="n"/>, in model coordinates</param>
    /// <param name="draggedparts">a <c>Dictionary</c> of <see cref="Part"/>s being dragged</param>
    /// <returns>a new location for the node, in model coordinates</returns>
    /// <remarks>
    /// <para>
    /// If <see cref="DragOverSnapArea"/> includes nodes,
    /// this finds a <see cref="Node"/> that acts as a drag-snapping controller, typically a grid-like node,
    /// at the <paramref name="newloc"/> point, using <see cref="ConsiderSnapTo"/> as the predicate to find
    /// a qualified drag-snapping node.
    /// If it finds such a drag-snapper node, it calls <see cref="SnapTo"/> to get the adjusted new location.
    /// </para>
    /// <para>
    /// If <see cref="DragOverSnapArea"/> includes the diagram,
    /// and if the diagram's <see cref="Northwoods.GoXam.Diagram.GridSnapEnabled"/> property is true,
    /// this calls <see cref="SnapTo"/> with a null value for the moving node,
    /// allowing that method to consider the "GridSnap..." properties of the <see cref="Diagram"/>
    /// in computing the new location for the node <paramref name="n"/>.
    /// </para>
    /// <para>
    /// It then limits the new location using the <see cref="Node.MinLocation"/> and <see cref="Node.MaxLocation"/> properties.
    /// </para>
    /// </remarks>
    protected virtual Point ComputeMove(Node n, Point newloc, Dictionary<Part, Info> draggedparts) {
      if (n == null) return newloc;
      Point pt = newloc;
      Node snapper = this.CurrentSnapper;
      // only consider snapping to nodes if DragOverSnapArea includes Nodes
      if (snapper != null && (this.DragOverSnapArea&DragOverSnapArea.Nodes) != 0 && ConsiderSnapTo(n, newloc, snapper, draggedparts)) {
        pt = SnapTo(n, newloc, snapper, draggedparts);
      }
      // only consider diagram snapping if there's no SNAPPER node and DragOverSnapArea includes Diagram
      if (snapper == null && (this.DragOverSnapArea&DragOverSnapArea.Diagram) != 0) {
        pt = SnapTo(n, newloc, null, draggedparts);
      }
      Point min = n.MinLocation;
      Point max = n.MaxLocation;
      return new Point(Math.Max(min.X, Math.Min(pt.X, max.X)), Math.Max(min.Y, Math.Min(pt.Y, max.Y)));
    }


    // DURING A DRAG: affecting stationary parts

    /// <summary>
    /// Determine if the currently dragged selection (<see cref="DraggedParts"/>
    /// or <see cref="CopiedParts"/>) would be valid to be dropped onto the target part <paramref name="p"/>,
    /// depending on its <see cref="Part.DropOntoBehavior"/>.
    /// </summary>
    /// <param name="pt">normally the current mouse point, in model coordinates</param>
    /// <param name="p">the <see cref="Part"/> under the mouse point that the selection might be dropped onto</param>
    /// <returns></returns>
    /// <remarks>
    /// This predicate is false if the target part <paramref name="p"/> is a selected part or
    /// if it is one of the parts in the <see cref="DraggedParts"/> or <see cref="CopiedParts"/> collections.
    /// It is also false if the target part is an <see cref="Adornment"/>.
    /// </remarks>
    protected virtual bool ConsiderDragOver(Point pt, Part p) {
      if (p == null) return false;
      DropOntoBehavior action = p.DropOntoBehavior;
      if (action == DropOntoBehavior.None) return false;
      if (p.IsSelected) return false;
      Dictionary<Part, Info> parts = this.CopiedParts ?? this.DraggedParts;
      if (parts == null) return false;
      if (parts.ContainsKey(p)) return false;
      if (p is Adornment) return false;  // not likely to be an Adornment, since we're not searching Temporary layers

      if ((action&DropOntoBehavior.AddsToGroup) != 0) {
        Group grp = p as Group;  // dropped-onto part must be a Group
        if (grp == null) return false;
        if (!parts.Keys.OfType<Node>().Any(n => IsValidMember(grp, n))) return false;
      } else if ((action&DropOntoBehavior.AddsLinkFromNode) != 0) {
        Node nd = p as Node;  // dropped-onto part must be a Node
        if (nd == null) return false;
        if (!parts.Keys.OfType<Node>().Any(n => LinkableFromStationaryNode(n, parts, nd))) return false;
      } else if ((action&DropOntoBehavior.AddsLinkToNode) != 0) {
        Node nd = p as Node;  // dropped-onto part must be a Node
        if (nd == null) return false;
        if (!parts.Keys.OfType<Node>().Any(n => LinkableToStationaryNode(n, parts, nd))) return false;
      } else if ((action&DropOntoBehavior.SplicesIntoLink) != 0) {
        Link lk = p as Link;  // dropped-onto part must be a Link
        if (lk == null) return false;
        // also disallow splicing into a link that connects a dragged/copied node
        if (!parts.Keys.OfType<Node>().Any(n => LinkableFromStationaryNode(n, parts, lk.FromNode))) return false;
        if (!parts.Keys.OfType<Node>().Any(n => LinkableToStationaryNode(n, parts, lk.ToNode))) return false;
      }
      return true;
    }

    /// <summary>
    /// Affect some stationary unselected objects that are under a given point,
    /// and consider auto-scrolling.
    /// </summary>
    /// <param name="pt">normally the current mouse point, in model coordinates</param>
    /// <param name="moving"></param>
    /// <param name="copying"></param>
    /// <remarks>
    /// <para>
    /// If <see cref="DropOntoEnabled"/> is true, this looks for a <see cref="Part"/>
    /// at the mouse point for which <see cref="ConsiderDragOver"/> is true;
    /// if successful it sets <see cref="DragOverPart"/> to that part.
    /// </para>
    /// <para>
    /// This method also performs auto-scrolling, if <see cref="Northwoods.GoXam.Diagram.AllowScroll"/> is true.
    /// </para>
    /// </remarks>

    new

    protected virtual void DragOver(Point pt, bool moving, bool copying) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;

      if (this.DropOntoEnabled && diagram.Model.Modifiable) {
        Part overpart = panel.FindElementAt<Part>(pt, Diagram.FindAncestor<Part>, p => ConsiderDragOver(pt, p), SearchLayers.Parts);
        this.DragOverPart = overpart;
      }

      if (diagram.AllowScroll && (moving || copying)) panel.DoAutoScroll(pt);
    }


    /// <summary>
    /// This predicate is called to determine whether a <see cref="Node"/>
    /// may be added as a member of the <see cref="Group"/> <paramref name="group"/>
    /// by <see cref="DropOnto"/>.
    /// </summary>
    /// <param name="group">this may be null if the node is being added as a top-level node</param>
    /// <param name="node">a <see cref="Node"/>, possibly another <see cref="Group"/></param>
    /// <returns>the result of calling <see cref="Northwoods.GoXam.Model.IGroupsModel.IsMemberValid"/></returns>
    /// <remarks>
    /// <see cref="ConsiderDragOver"/> also calls this predicate,
    /// to determine if side-effects on stationary parts should occur.
    /// </remarks>
    public virtual bool IsValidMember(Group group, Node node) {
      if (node == null) return false;
      if (group == null) return true;  // always allow nodes to be top-level
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      Object gdata = group.Data;
      Object ndata = node.Data;
      PartManager partmgr = diagram.PartManager;
      if (partmgr == null) return true;
      IGroupsModel gmodel = partmgr.FindCommonDataModel(gdata, ndata) as IGroupsModel;
      return gmodel != null && gmodel.IsMemberValid(gdata, ndata);
    }

    /// <summary>
    /// This predicate is called to determine whether a <see cref="Node"/>
    /// may be reconnected by <see cref="DropOnto"/>.
    /// </summary>
    /// <param name="fromnode"></param>
    /// <param name="tonode"></param>
    /// <param name="relink">the <see cref="Link"/> to relink to connect the <paramref name="fromnode"/> and the <paramref name="tonode"/></param>
    /// <returns>
    /// the result of calling <see cref="Northwoods.GoXam.Model.IDiagramModel.IsLinkValid"/> if <paramref name="relink"/> is null,
    /// or if <paramref name="relink"/> is supplied,
    /// the result of calling <see cref="Northwoods.GoXam.Model.ILinksModel.IsRelinkValid"/>,
    /// <see cref="Northwoods.GoXam.Model.IConnectedModel.IsRelinkValid"/>, or
    /// <see cref="Northwoods.GoXam.Model.ITreeModel.IsRelinkValid"/>.
    /// </returns>
    /// <remarks>
    /// <see cref="ConsiderDragOver"/> also calls this predicate,
    /// to determine if side-effects on stationary parts should occur.
    /// </remarks>
    public virtual bool IsValidLink(Node fromnode, Node tonode, Link relink) {
      if (fromnode == null || tonode == null) return false;  // can't have a link without nodes at both ends
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      PartManager partmgr = diagram.PartManager;
      if (partmgr == null) return true;
      Object fromdata = fromnode.Data;
      Object fromid = this.FromPortId;
      Object todata = tonode.Data;
      Object toid = this.ToPortId;
      IDiagramModel model = partmgr.FindCommonDataModel(fromdata, todata);
      if (model == null) return false;
      if (relink != null) {
        ILinksModel lmodel = model as ILinksModel;
        if (lmodel != null) return lmodel.IsRelinkValid(fromdata, fromid, todata, toid, relink.Data);
        IConnectedModel cmodel = model as IConnectedModel;
        if (cmodel != null) return cmodel.IsRelinkValid(fromdata, todata, relink.FromData, relink.ToData);
        ITreeModel tmodel = model as ITreeModel;
        if (tmodel != null) return tmodel.IsRelinkValid(fromdata, todata, relink.FromData, relink.ToData);
        return false;
      } else {
        return model.IsLinkValid(fromdata, fromid, todata, toid);
      }
    }

    private bool LinkableFromStationaryNode(Node n, Dictionary<Part, Info> dragged, Node stationary) {
      if (n == null) return false;
      if (n == stationary) return false;  // shouldn't happen, but might as well check anyway
      var links = n.LinksInto.ToList();
      if (links.Count > 1) return false;  //?? multiple links: disallow
      Link relink = null;
      if (links.Count == 1) {  // one link: relink if not connected with a dragged node
        relink = links.First();
        Node other = relink.FromNode;  // assume IsValidLink will check for relink valid
        if (other != null && dragged.ContainsKey(other)) return false;
      }  // zero links: new link, only need to check IsValidLink
      // ignore all nodes that are members of a dragged Group
      if (dragged.OfType<Group>().Any(g => n.IsContainedBy(g))) return false;
      return IsValidLink(stationary, n, relink);
    }

    private bool LinkableToStationaryNode(Node n, Dictionary<Part, Info> dragged, Node stationary) {
      if (n == null) return false;
      if (n == stationary) return false;  // shouldn't happen, but might as well check anyway
      var links = n.LinksOutOf.ToList();
      if (links.Count > 1) return false;  //?? multiple links: disallow
      Link relink = null;
      if (links.Count == 1) {  // one link: relink if not connected with a dragged node
        relink = links.First();
        Node other = relink.ToNode;  // assume IsValidLink will check for relink valid
        if (other != null && dragged.ContainsKey(other)) return false;
      }  // zero links: new link, assume IsValidLink will check for new link valid
      // ignore all nodes that are members of a dragged Group
      if (dragged.OfType<Group>().Any(g => n.IsContainedBy(g))) return false;
      return IsValidLink(n, stationary, relink);
    }


    // UPON A DROP:

    private bool LinkableFromStationaryNode(Node n, Node stationary) {
      if (n == null) return false;
      if (n == stationary) return false;  // shouldn't happen, but might as well check anyway
      var links = n.LinksInto.ToList();
      if (links.Count > 1) return false;  //?? multiple links: disallow
      Link relink = null;
      if (links.Count == 1) {  // one link: relink if not connected with a dragged node
        relink = links.First();
        Node other = relink.FromNode;  // assume IsValidLink will check for relink valid
        if (other != null && other.IsSelected) return false;
      }  // zero links: new link, only need to check IsValidLink
      // ignore all nodes that are members of a selected Group
      if (this.Diagram.SelectedParts.OfType<Group>().Any(g => n.IsContainedBy(g))) return false;
      return IsValidLink(stationary, n, relink);
    }

    private bool LinkableToStationaryNode(Node n, Node stationary) {
      if (n == null) return false;
      if (n == stationary) return false;  // shouldn't happen, but might as well check anyway
      var links = n.LinksOutOf.ToList();
      if (links.Count > 1) return false;  //?? multiple links: disallow
      Link relink = null;
      if (links.Count == 1) {  // one link: relink if not connected with a dragged node
        relink = links.First();
        Node other = relink.ToNode;  // assume IsValidLink will check for relink valid
        if (other != null && other.IsSelected) return false;
      }  // zero links: new link, assume IsValidLink will check for new link valid
      // ignore all nodes that are members of a selected Group
      if (this.Diagram.SelectedParts.OfType<Group>().Any(g => n.IsContainedBy(g))) return false;
      return IsValidLink(n, stationary, relink);
    }

    private HashSet<Node> FindSelectedUnnestedNodes() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return new HashSet<Node>();
      // find selected nodes
      IEnumerable<Node> selnodes = diagram.SelectedParts.OfType<Node>().ToList();
      // filter out nodes that are members (perhaps indirectly) of other selected nodes
      return new HashSet<Node>(selnodes.Where(n => !selnodes.Any(m => n.IsContainedBy(m))));
    }

    /// <summary>
    /// Perform any additional side-effects after a drop.
    /// </summary>
    /// <param name="pt"></param>
    /// <remarks>
    /// <para>
    /// If <see cref="DropOntoEnabled"/> is true, and if the selection
    /// is dropped onto a <see cref="Group"/> that has 
    /// <see cref="Part.DropOntoBehavior"/> set to <see cref="DropOntoBehavior.AddsToGroup"/>,
    /// this will support re"parent"ing of nodes and links by adding them to the group.
    /// This calls <see cref="IsValidMember"/> to decide if the additional group membership is valid.
    /// If the selection is dropped onto the background,
    /// the selected nodes and links are removed from any groups of which they were a member
    /// and for which <see cref="Part.DropOntoBehavior"/> is also set to
    /// <see cref="DropOntoBehavior.AddsToGroup"/>, and they are made into top-level nodes and links.
    /// </para>
    /// <para>
    /// If the target <see cref="Node"/> has a <see cref="Part.DropOntoBehavior"/> set to
    /// <see cref="DropOntoBehavior.AddsLinkFromNode"/> or <see cref="DropOntoBehavior.AddsLinkToNode"/>,
    /// this will support automatically adding a link from or to the dropped-on node with each
    /// of the dropped nodes.
    /// This calls <see cref="IsValidLink"/> to decide if the additional link relationship is valid.
    /// </para>
    /// <para>
    /// If the target <see cref="Link"/> has a <see cref="Part.DropOntoBehavior"/> set to
    /// <see cref="DropOntoBehavior.SplicesIntoLink"/>,
    /// this will support automatically removing that link and adding two links,
    /// one to and one from each of the dropped nodes, connecting with the two nodes
    /// that the original link had been connected with.
    /// This calls <see cref="IsValidLink"/> twice to decide if the additional link relationships are valid.
    /// </para>
    /// </remarks>
    protected virtual void DropOnto(Point pt) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;
      IDiagramModel model = diagram.Model;
      if (model == null) return;

      if (this.DropOntoEnabled && model.Modifiable) {
        Part overpart = panel.FindElementAt<Part>(pt, Diagram.FindAncestor<Part>, p => ConsiderDragOver(pt, p), SearchLayers.Parts);
        this.DragOverPart = overpart; 
        if (overpart != null) {
          DropOntoBehavior action = overpart.DropOntoBehavior;
          if ((action & DropOntoBehavior.AddsToGroup) != 0) {  // add to Group that the selection was dropped on
            Group group = overpart as Group;
            if (group != null && model is ISubGraphModel) {
              HashSet<Node> droppeds = FindSelectedUnnestedNodes();
              MaybeAddToSubGraph(group, droppeds);
            }
          } else if ((action & DropOntoBehavior.AddsLinkFromNode) != 0) {
            Node node = overpart as Node;
            if (node != null) {  // the node that the selection was dropped on
              HashSet<Node> droppeds = FindSelectedUnnestedNodes();
              // maybe the node that was dropped upon is part of a Group with DropOntoBehavior.AddsToGroup
              MaybeAddToSubGraph(node.ContainingSubGraph, droppeds);
              // link from node to all valid unnested nodes not already connected to dropped nodes
              foreach (Node n in droppeds.Where(n => LinkableFromStationaryNode(n, node))) {
                Link existing = n.LinksInto.FirstOrDefault();
                if (existing != null) {  // must connect with an unselected (i.e. not dropped) node
                  ILinksModel lmodel = model as ILinksModel;
                  if (lmodel != null) {  // reconnect the existing link data
                    lmodel.SetLinkFromPort(existing.Data, node.Data, this.FromPortId);
                  } else {  // remove the existing link and add the new one
                    model.RemoveLink(existing.FromNode.Data, null, existing.ToNode.Data, null);
                    model.AddLink(node.Data, this.FromPortId, n.Data, this.ToPortId);
                  }
                } else {  // add link from node NODE to node N
                  model.AddLink(node.Data, this.FromPortId, n.Data, this.ToPortId);
                }
              }
            }
          } else if ((action & DropOntoBehavior.AddsLinkToNode) != 0) {
            Node node = overpart as Node;
            if (node != null) {  // the node that the selection was dropped on
              HashSet<Node> droppeds = FindSelectedUnnestedNodes();
              // maybe the node that was dropped upon is part of a Group with DropOntoBehavior.AddsToGroup
              MaybeAddToSubGraph(node.ContainingSubGraph, droppeds);
              // link to node from all valid unnested nodes not already connected to dropped nodes
              foreach (Node n in droppeds.Where(n => LinkableToStationaryNode(n, node))) {
                Link existing = n.LinksOutOf.FirstOrDefault();
                if (existing != null) {  // must connect with an unselected (i.e. not dropped) node
                  ILinksModel lmodel = model as ILinksModel;
                  if (lmodel != null) {  // reconnect the existing link data
                    lmodel.SetLinkToPort(existing.Data, node.Data, this.ToPortId);
                  } else {  // remove the existing link and add the new one
                    model.RemoveLink(existing.FromNode.Data, null, existing.ToNode.Data, null);
                    model.AddLink(n.Data, this.FromPortId, node.Data, this.ToPortId);
                  }
                } else {  // add link from node N to node NODE
                  model.AddLink(n.Data, this.FromPortId, node.Data, this.ToPortId);
                }
              }
            }
          } else if ((action & DropOntoBehavior.SplicesIntoLink) != 0) {
            Link link = overpart as Link;
            if (link != null) {  // the link that the selection was dropped on
              Node fromnode = link.FromNode;
              Object fromdata = link.FromData;
              Object fromport = link.FromPortId;
              Node tonode = link.ToNode;
              Object todata = link.ToData;
              Object toport = link.ToPortId;
              HashSet<Node> droppeds = FindSelectedUnnestedNodes();
              // maybe the link that was dropped upon is part of a Group with DropOntoBehavior.AddsToGroup
              MaybeAddToSubGraph(link.ContainingSubGraph, droppeds);
              // instead of modifying the existing link and adding one new link,
              // just remove the existing link and add two new links
              model.RemoveLink(fromdata, fromport, todata, toport);
              // link from FromNode to all valid unnested nodes not already connected to dropped nodes
              foreach (Node n in droppeds.Where(n => LinkableFromStationaryNode(n, fromnode))) {
                // add link from node NODE to node N
                model.AddLink(fromdata, fromport, n.Data, this.ToPortId);
              }
              // link to ToNode from all valid unnested nodes not already connected to dropped nodes
              foreach (Node n in droppeds.Where(n => LinkableToStationaryNode(n, tonode))) {
                // add link from node N to node NODE
                model.AddLink(n.Data, this.FromPortId, todata, toport);
              }
            }
          }
        } else {  // not dropped onto a part
          ISubGraphModel sgmodel = diagram.Model as ISubGraphModel;  //?? IGroupsModel
          if (sgmodel != null && sgmodel.Modifiable) {
            // maybe remove selection from existing container group, if the old group had DropOntoBehavior.AddsToGroup
            HashSet<Node> nodes = FindSelectedUnnestedNodes();
            IEnumerable<Node> removable = nodes.Where(n => n.ContainingGroups.Any(g => (g.DropOntoBehavior&DropOntoBehavior.AddsToGroup) != 0));
            foreach (Node n in removable) {
              if (IsValidMember(null, n)) {
                sgmodel.SetGroupNode(n.Data, null);
              }
            }
          }
        }
      }

      // after dropping a Node, see if it overlaps with any AvoidsNodes link that might need to be re-routed
      foreach (Node node in diagram.SelectedParts.OfType<Node>()) {
        panel.InvalidateAvoidsNodesLinks(node.Bounds);
      }

      this.DragOverPart = null;
    }

    private void MaybeAddToSubGraph(Group group, HashSet<Node> nodes) {
      if (group != null && (group.DropOntoBehavior&DropOntoBehavior.AddsToGroup) != 0) {
        Diagram diagram = this.Diagram;
        if (diagram != null) {
          ISubGraphModel sgmodel = diagram.Model as ISubGraphModel;  //?? IGroupsModel
          if (sgmodel != null) {
            foreach (Node n in nodes) {
              if (IsValidMember(group, n)) {
                sgmodel.SetGroupNode(n.Data, group.Data);
              }
            }
          }
        }
      }
    }


    // Internal mouse drag and drop, not involving Windows drag-and-drop (WPF)
    // nor simulated drag-and-drop between diagrams (Silverlight or XBAP)
    
    /// <summary>
    /// As the user moves the mouse, move the collection of <see cref="CopiedParts"/>
    /// or <see cref="DraggedParts"/>, depending on whether <see cref="MayCopy"/>
    /// or <see cref="MayMove"/> is true.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When <see cref="MayCopy"/> is true but <see cref="CopiedParts"/> is still null,
    /// it will restore the original locations of the dragged parts and
    /// make a copy by calling <see cref="PartManager.CopyParts"/>.
    /// When <see cref="MayCopy"/> becomes false because the Control-key modifier no
    /// longer applies, this will remove the copied parts by calling
    /// <see cref="PartManager.DeleteParts"/> and move the <see cref="DraggedParts"/>.
    /// </para>
    /// <para>
    /// This also calls <see cref="DragOver"/>.
    /// </para>
    /// </remarks>
    public override void DoMouseMove() {
      if (!this.Active) return;

      if (!IsLeftButtonDown()) {  // this might happen in Silverlight
        StopTool();
        return;
      }

      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      Part mainnode = this.CurrentPart;
      if (mainnode != null && this.DraggedParts != null) {
        bool copying = false;
        bool moving = false;

        // handle copying by making copy immediately, and dragging it
        if (MayCopy()) {
          copying = true;
          AddCopies(false);  // false means it doesn't get recorded in UndoManager
          MoveCollection(this.CopiedParts);
        } else if (MayMove()) {
          moving = true;
          RemoveCopies();  // get rid of any copies, in case COPYING had been true before
          MoveCollection(this.DraggedParts);
        } else {
          RemoveCopies();
        }

        DragOver(diagram.LastMousePointInModel, moving, copying);
      }
    }

    /// <summary>
    /// On a mouse-up finish moving or copying the effective selection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This also calls <see cref="DropOnto"/>,
    /// updates the diagram's bounds,
    /// raises a "selection copied" or "selection moved" event,
    /// and stops this tool.
    /// </para>
    /// </remarks>
    public override void DoMouseUp() {
      if (this.Active) {
        this.Dropped = true;
        Diagram diagram = this.Diagram;
        if (diagram == null) return;
        bool copying = MayCopy();
        if (copying && this.CopiedParts != null) {
          // get rid of any temporary objects before turning on any UndoManager
          RestoreOriginalLocations();
          RemoveCopies();
          AddCopies(true);
          MoveCollection(this.CopiedParts);
          if (this.CopiedParts != null) diagram.Select(this.CopiedParts.Keys);
          this.CopiedParts = null;
          this.CurrentSnapper = null;
        } else {
          MaybeUpdateContainingSubgraphs();
        }
        // consider side-effects on the "target" object, if any
        DropOnto(diagram.LastMousePointInModel);

        diagram.Panel.UpdateDiagramBounds();
        ResumeRouting(this.DraggedParts);
        // set the EditResult before raising event, in case it changes the result or cancels the tool
        this.TransactionResult = (copying ? "CopySelection" : "MoveSelection");
        RaiseEvent((copying ? Diagram.SelectionCopiedEvent : Diagram.SelectionMovedEvent), new DiagramEventArgs());
      }
      StopTool();
    }

    private void MaybeUpdateContainingSubgraphs() {
      foreach (Node n in this.DraggedParts.Keys.OfType<Node>()) {
        Group sg = n.ContainingSubGraph;
        if (sg != null && !this.DraggedParts.ContainsKey(sg)) {
          GroupPanel gp = sg.GroupPanel;
          if (gp != null && gp.SurroundsMembersAfterDrop) {
            sg.Remeasure();
          }
        }
      }
    }

    /// <summary>
    /// This predicate is true when the view allows objects to be copied and inserted,
    /// and some object in the diagram's selection is copyable, and the user is holding down the Control key.
    /// </summary>
    /// <returns></returns>
    protected virtual bool MayCopy() {
      Diagram diagram = this.Diagram;  // this diagram must allow insert
      if (diagram == null || diagram.IsReadOnly || !diagram.AllowInsert || !diagram.AllowCopy) return false;
      if (!IsControlKeyDown()) return false;
      IDiagramModel model = diagram.Model;  // this model must support the data
      if (model == null || !model.Modifiable) return false;
      foreach (Part part in diagram.SelectedParts) {
        Node n = part as Node;
        if (n != null && n.CanCopy()) {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// This predicate is true when the view allows objects to be moved,
    /// and some object in the Selection is movable.
    /// </summary>
    /// <returns></returns>
    protected virtual bool MayMove() {
      Diagram diagram = this.Diagram;  // this diagram must allow insert
      if (diagram == null || diagram.IsReadOnly || !diagram.AllowMove) return false;
      IDiagramModel model = diagram.Model;  // this model must support the data
      if (model == null) return false;
      foreach (Part part in diagram.SelectedParts) {
        Node n = part as Node;
        if (n != null && n.CanMove()) {
          return true;
        }
      }
      return false;
    }


    private static List<DraggingTool> DraggedOverTools = new List<DraggingTool>();

    private void RememberDraggingTool() {
      lock (DraggingTool.DraggedOverTools) {
        if (!DraggingTool.DraggedOverTools.Contains(this)) {
          DraggingTool.DraggedOverTools.Add(this);
        }
      }
    }

    internal static void CleanUpDraggingTool() {
      // hack to clean up temporary nodes created by DragOver in other Diagrams
      lock (DraggingTool.DraggedOverTools) {
        if (DraggingTool.DraggedOverTools.Count > 0) {
          foreach (DraggingTool tool in DraggingTool.DraggedOverTools) {
            tool.RemoveCopies();
            if (tool.Diagram != null && tool.Diagram.Panel != null) {
              tool.Diagram.Panel.StopAutoScroll();  // make sure there isn't any timer going
            }
          }
          DraggingTool.DraggedOverTools.Clear();
        }
      }
      // don't set DraggingTool.Source = null
    }


    // Simulated drag-and-drop, for Silverlight and for WPF XBAP.
    // In WPF, used when Diagram.SimulatesDragDrop is true.

    //??? support moving between diagrams
    //??? feedback: e.g. No Drop cursor
    //??? drop effects: e.g. Copy Allowed

    /// <summary>
    /// This is an experimental property that gets the <see cref="DraggingTool"/> that
    /// is the source of a simulated drag-and-drop in Silverlight or in WPF XBAP.
    /// </summary>
    public static DraggingTool Source { get; internal set; }

    internal static UIElement Root { get; set; }
    internal static Diagram CurrentDiagram { get; set; }

    // in the target DraggingTool:

    internal /*?? protected virtual */ bool MayCopySimulated() {
      Diagram diagram = this.Diagram;  // this diagram must allow insert
      if (diagram == null || !diagram.AllowDrop || diagram.IsReadOnly || !diagram.AllowInsert) return false;
      IDiagramModel model = diagram.Model;  // this model must support the data
      if (model == null || !model.Modifiable) return false;
      return true;
    }

    internal /*?? public virtual */ void DoSimulatedDragEnter() {
      //Diagram.Debug("DragEnter " + this.Diagram.Name);
      if (!MayCopySimulated()) return;
      RememberDraggingTool();
    }

    internal /*?? public virtual */ void DoSimulatedDragLeave() {
      //Diagram.Debug("DragLeave " + this.Diagram.Name);
      RemoveCopies();
      Diagram diagram = this.Diagram;
      if (diagram != null && diagram.Panel != null && diagram.CurrentTool != this && !Double.IsNaN(this.StartPanelPoint.X)) {
        diagram.Panel.Position = this.StartPanelPoint;
      }
    }

    internal /*?? public virtual */ void DoSimulatedDragOver() {
      //Diagram.Debug("DragOver " + this.Diagram.Name);
      if (!MayCopySimulated()) return;
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DraggingTool source = DraggingTool.Source;
      if (source != null && source.DraggedParts != null) {
        AddCopies(source.DraggedParts.Keys, false);
        MoveCollection(this.CopiedParts);

        DragOver(diagram.LastMousePointInModel, false, true); //???
      }
    }

    internal /*?? public virtual */ void DoSimulatedDrop() {
      //Diagram.Debug("Drop onto " + this.Diagram.Name);
      DraggingTool source = DraggingTool.Source;
      if (source != null) {
        // always tell source Diagram/DraggingTool that the drop happened, even if disallowed
        source.Dropped = true;

        // get rid of any temporary objects before turning on any UndoManager
        RemoveCopies();

        if (!MayCopySimulated()) return;

        Diagram diagram = this.Diagram;
        if (diagram == null) return;
        StartTransaction("Drop");
        AddCopies(source.DraggedParts.Keys, true);
        MoveCollection(this.CopiedParts);
        if (this.CopiedParts != null) diagram.Select(this.CopiedParts.Keys);
        this.TransactionResult = "ExternalCopy";
        // consider side-effects on the "target" object, if any
        DropOnto(diagram.LastMousePointInModel);

        // cleanup
        this.CopiedParts = null;
        this.CurrentSnapper = null;

        RaiseEvent(Diagram.ExternalObjectsDroppedEvent, new DiagramEventArgs());
        StopTransaction();

        diagram.Panel.UpdateDiagramBounds();
      }
    }

    private void AddCopies(IEnumerable<Part> originals, bool undoable) {
      if (this.CopiedParts == null) {
        Diagram diagram = this.Diagram;
        if (diagram == null || diagram.IsReadOnly /* || !diagram.AllowInsert */) return;
        IDiagramModel model = diagram.Model;
        if (model == null) return;
        PartManager mgr = diagram.PartManager;
        if (mgr == null) return;
        mgr.TemporaryParts = !undoable;
        model.SkipsUndoManager = !undoable;

        this.StartPoint = diagram.LastMousePointInModel;
        ICopyDictionary dict = mgr.CopyParts(originals, model);
        IDataCollection copieddata = dict.Copies;

        Rect bounds = DiagramPanel.ComputeLocations(originals.OfType<Node>());
        Point mid = new Point(bounds.X+bounds.Width/2, bounds.Y+bounds.Height/2);
        Point loc = this.StartPoint;

        Dictionary<Part, Info> copiedparts = new Dictionary<Part, Info>();
        foreach (Node n in originals.OfType<Node>().Where(CanCopyNode)) {
          Node c = mgr.FindNodeForData(dict.FindCopiedNode(n.Data), model);
          if (c != null) {
            Point l = n.Location;
            c.Location = new Point(loc.X - (mid.X - l.X), loc.Y - (mid.Y - l.Y));
            copiedparts[c] = new Info() { Point = c.Location };
          }
        }
        foreach (Link l in mgr.FindLinksForData(copieddata)) {
          copiedparts[l] = new Info();
        }
        this.CopiedParts = copiedparts;

        mgr.TemporaryParts = false;
      }
    }




    // in the source DraggingTool:

    internal /*?? protected virtual */ void DoDragOut() {
      //Diagram.Debug("Start DragOut from " + this.Diagram.Name);
      this.DragOutStarted = true;
      this.Dropped = false;
      DraggingTool.Source = this;
      UIElement ve = this.Diagram;
      while (ve != null) {
        UIElement pve = VisualTreeHelper.GetParent(ve) as UIElement;
        if (pve == null) break;
        ve = pve;
      }
      DraggingTool.Root = ve;
      CaptureMouse();
    }


































































































































































































































































































































































































































































































































  }  // end DraggingTool


  /// <summary>
  /// This enumeration controls where the <see cref="DraggingTool"/> might
  /// snap the selection location during dragging.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Drag snapping may happen for the whole diagram and/or for individual nodes,
  /// or not at all.
  /// Snapping behavior for the whole diagram is governed by the various
  /// "GridSnap..." properties on <see cref="Northwoods.GoXam.Diagram"/>.
  /// Snapping behavior over a particular node is governed by the various
  /// "DragOverSnap..." properties on the node.
  /// </para>
  /// <para>
  /// Snap movement during a drag is implemented by <see cref="DraggingTool.SnapTo"/>.
  /// </para>
  /// </remarks>
  public enum DragOverSnapArea {
    /// <summary>
    /// Perform no location snapping during a drag.
    /// </summary>
    None = 0,
    /// <summary>
    /// Perform location snapping for the whole diagram.
    /// </summary>
    /// <remarks>
    /// This is the default value for <see cref="DraggingTool.DragOverSnapArea"/>.
    /// <see cref="Northwoods.GoXam.Diagram.GridSnapEnabled"/> must also be true.
    /// </remarks>
    Diagram = 1,
    /// <summary>
    /// Perform location snapping when the mouse is over a node
    /// that has <see cref="Part.DragOverSnapEnabled"/> set to true.
    /// </summary>
    Nodes = 2,
    /// <summary>
    /// Perform location snapping when the mouse is over a node
    /// that has <see cref="Part.DragOverSnapEnabled"/> set to true,
    /// or if the mouse is not over such a node, perform location
    /// snapping for the whole diagram, if
    /// <see cref="Northwoods.GoXam.Diagram.GridSnapEnabled"/> is also true.
    /// </summary>
    DiagramAndNodes = Diagram | Nodes
  }

  /// <summary>
  /// This enumeration controls the behavior of <see cref="DraggingTool.ComputeEffectiveCollection"/>.
  /// </summary>
  [Flags]
  public enum EffectiveCollectionInclusions {
    /// <summary>
    /// Don't add any parts but the selected ones.
    /// </summary>
    None = 0,
    //?? Members = 1,
    //?? ConnectingLinks = 2,
    //?? LinkLabelNodes = 4,
    /// <summary>
    /// Just include the selected nodes, their members if they are groups, and the links connecting them.
    /// </summary>
    Standard = 7,
    /// <summary>
    /// Include tree-structure descendant nodes (children, grandchildren, etc.)
    /// and the links connecting to them.
    /// </summary>
    TreeChildren = 8,
    /// <summary>
    /// Include all standard connected parts and also tree-structure descendant
    /// nodes (children, grandchildren, etc.) and the links connecting to them.
    /// </summary>
    SubTree = Standard | TreeChildren
  }
}
