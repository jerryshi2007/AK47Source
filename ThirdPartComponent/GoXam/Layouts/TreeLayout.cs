
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
using System.Linq;
using System.Windows;
using Northwoods.GoXam;

namespace Northwoods.GoXam.Layout {

  /// <summary>
  /// Position nodes in a tree-like arrangement.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Although this class inherits from <c>FrameworkElement</c>
  /// in order to support data binding,
  /// it is not really a <c>FrameworkElement</c> or <c>UIElement</c>!
  /// Please ignore all of the properties, methods, and events defined by
  /// <c>FrameworkElement</c> and <c>UIElement</c>.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class TreeLayout : DiagramLayout {
    /// <summary>
    /// Construct a layout with the default values.
    /// </summary>
    public TreeLayout() {
      this.Roots = new List<Node>();
      this.RootDefaults = new TreeVertex();
      this.AlternateDefaults = new TreeVertex();
    }

    /// <summary>
    /// Make a copy of a <see cref="TreeLayout"/>, copying most of the
    /// important properties except for the <see cref="Network"/> and <see cref="Roots"/>.
    /// </summary>
    /// <param name="layout"></param>
    /// <remarks>
    /// This copies the inheritable properties from the argument layout's
    /// <see cref="RootDefaults"/> and the <see cref="AlternateDefaults"/>.
    /// </remarks>
    public TreeLayout(TreeLayout layout) : base(layout) {
      if (layout != null) {
        this.Network = null;
        this.Roots = new List<Node>();
        this.Path = layout.Path;
        this.TreeStyle = layout.TreeStyle;
        this.Arrangement = layout.Arrangement;
        this.ArrangementSpacing = layout.ArrangementSpacing;

        this.RootDefaults = new TreeVertex();
        this.RootDefaults.CopyInheritedPropertiesFrom(layout.RootDefaults);
        this.AlternateDefaults = new TreeVertex();
        this.AlternateDefaults.CopyInheritedPropertiesFrom(layout.AlternateDefaults);
      }
    }

    static TreeLayout() {
      NetworkProperty = DependencyProperty.Register("Network", typeof(TreeNetwork), typeof(TreeLayout),
        new FrameworkPropertyMetadata(null, OnNetworkChanged));
      RootsProperty = DependencyProperty.Register("Roots", typeof(ICollection<Node>), typeof(TreeLayout),
        new FrameworkPropertyMetadata(null));

      PathProperty = DependencyProperty.Register("Path", typeof(TreePath), typeof(TreeLayout),
        new FrameworkPropertyMetadata(TreePath.Destination, OnPropertyChanged));
      TreeStyleProperty = DependencyProperty.Register("TreeStyle", typeof(TreeStyle), typeof(TreeLayout),
        new FrameworkPropertyMetadata(TreeStyle.Layered, OnPropertyChanged));
      ArrangementProperty = DependencyProperty.Register("Arrangement", typeof(TreeArrangement), typeof(TreeLayout),
        new FrameworkPropertyMetadata(TreeArrangement.Vertical, OnPropertyChanged));
      ArrangementSpacingProperty = DependencyProperty.Register("ArrangementSpacing", typeof(Size), typeof(TreeLayout),
        new FrameworkPropertyMetadata(new Size(10, 10), OnPropertyChanged));

      SortingProperty = DependencyProperty.Register("Sorting", typeof(TreeSorting), typeof(TreeLayout),
        new FrameworkPropertyMetadata(TreeSorting.Forwards, OnSortingChanged));
      ComparerProperty = DependencyProperty.Register("Comparer", typeof(IComparer<TreeVertex>), typeof(TreeLayout),
        new FrameworkPropertyMetadata(AlphabeticNodeTextComparer, OnComparerChanged));
      AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(0.0, OnAngleChanged));
      AlignmentProperty = DependencyProperty.Register("Alignment", typeof(TreeAlignment), typeof(TreeLayout),
        new FrameworkPropertyMetadata(TreeAlignment.CenterChildren, OnAlignmentChanged));
      NodeIndentProperty = DependencyProperty.Register("NodeIndent", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(0.0, OnNodeIndentChanged));
      NodeIndentPastParentProperty = DependencyProperty.Register("NodeIndentPastParent", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(0.0, OnNodeIndentPastParentChanged));
      NodeSpacingProperty = DependencyProperty.Register("NodeSpacing", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(20.0, OnNodeSpacingChanged));
      LayerSpacingProperty = DependencyProperty.Register("LayerSpacing", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(50.0, OnLayerSpacingChanged));
      LayerSpacingParentOverlapProperty = DependencyProperty.Register("LayerSpacingParentOverlap", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(0.0, OnLayerSpacingParentOverlapChanged));
      CompactionProperty = DependencyProperty.Register("Compaction", typeof(TreeCompaction), typeof(TreeLayout),
        new FrameworkPropertyMetadata(TreeCompaction.Block, OnCompactionChanged));
      BreadthLimitProperty = DependencyProperty.Register("BreadthLimit", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(0.0, OnBreadthLimitChanged));
      RowSpacingProperty = DependencyProperty.Register("RowSpacing", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(25.0, OnRowSpacingChanged));
      RowIndentProperty = DependencyProperty.Register("RowIndent", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(10.0, OnRowIndentChanged));
      CommentSpacingProperty = DependencyProperty.Register("CommentSpacing", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(10.0, OnCommentSpacingChanged));
      CommentMarginProperty = DependencyProperty.Register("CommentMargin", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(20.0, OnCommentMarginChanged));
      SetsPortSpotProperty = DependencyProperty.Register("SetsPortSpot", typeof(bool), typeof(TreeLayout),
        new FrameworkPropertyMetadata(true, OnSetsPortSpotChanged));
      PortSpotProperty = DependencyProperty.Register("PortSpot", typeof(Spot), typeof(TreeLayout),
        new FrameworkPropertyMetadata(Spot.Default, OnPortSpotChanged));
      SetsChildPortSpotProperty = DependencyProperty.Register("SetsChildPortSpot", typeof(bool), typeof(TreeLayout),
        new FrameworkPropertyMetadata(true, OnSetsChildPortSpotChanged));
      ChildPortSpotProperty = DependencyProperty.Register("ChildPortSpot", typeof(Spot), typeof(TreeLayout),
        new FrameworkPropertyMetadata(Spot.Default, OnChildPortSpotChanged));

      AlternateSortingProperty = DependencyProperty.Register("AlternateSorting", typeof(TreeSorting), typeof(TreeLayout),
        new FrameworkPropertyMetadata(TreeSorting.Forwards, OnAlternateSortingChanged));
      AlternateComparerProperty = DependencyProperty.Register("AlternateComparer", typeof(IComparer<TreeVertex>), typeof(TreeLayout),
        new FrameworkPropertyMetadata(AlphabeticNodeTextComparer, OnAlternateComparerChanged));
      AlternateAngleProperty = DependencyProperty.Register("AlternateAngle", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(0.0, OnAlternateAngleChanged));
      AlternateAlignmentProperty = DependencyProperty.Register("AlternateAlignment", typeof(TreeAlignment), typeof(TreeLayout),
        new FrameworkPropertyMetadata(TreeAlignment.CenterChildren, OnAlternateAlignmentChanged));
      AlternateNodeIndentProperty = DependencyProperty.Register("AlternateNodeIndent", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(0.0, OnAlternateNodeIndentChanged));
      AlternateNodeIndentPastParentProperty = DependencyProperty.Register("AlternateNodeIndentPastParent", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(0.0, OnAlternateNodeIndentPastParentChanged));
      AlternateNodeSpacingProperty = DependencyProperty.Register("AlternateNodeSpacing", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(20.0, OnAlternateNodeSpacingChanged));
      AlternateLayerSpacingProperty = DependencyProperty.Register("AlternateLayerSpacing", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(50.0, OnAlternateLayerSpacingChanged));
      AlternateLayerSpacingParentOverlapProperty = DependencyProperty.Register("AlternateLayerSpacingParentOverlap", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(0.0, OnAlternateLayerSpacingParentOverlapChanged));
      AlternateCompactionProperty = DependencyProperty.Register("AlternateCompaction", typeof(TreeCompaction), typeof(TreeLayout),
        new FrameworkPropertyMetadata(TreeCompaction.Block, OnAlternateCompactionChanged));
      AlternateBreadthLimitProperty = DependencyProperty.Register("AlternateBreadthLimit", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(0.0, OnAlternateBreadthLimitChanged));
      AlternateRowSpacingProperty = DependencyProperty.Register("AlternateRowSpacing", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(25.0, OnAlternateRowSpacingChanged));
      AlternateRowIndentProperty = DependencyProperty.Register("AlternateRowIndent", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(10.0, OnAlternateRowIndentChanged));
      AlternateCommentSpacingProperty = DependencyProperty.Register("AlternateCommentSpacing", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(10.0, OnAlternateCommentSpacingChanged));
      AlternateCommentMarginProperty = DependencyProperty.Register("AlternateCommentMargin", typeof(double), typeof(TreeLayout),
        new FrameworkPropertyMetadata(20.0, OnAlternateCommentMarginChanged));
      AlternateSetsPortSpotProperty = DependencyProperty.Register("AlternateSetsPortSpot", typeof(bool), typeof(TreeLayout),
        new FrameworkPropertyMetadata(true, OnAlternateSetsPortSpotChanged));
      AlternatePortSpotProperty = DependencyProperty.Register("AlternatePortSpot", typeof(Spot), typeof(TreeLayout),
        new FrameworkPropertyMetadata(Spot.Default, OnAlternatePortSpotChanged));
      AlternateSetsChildPortSpotProperty = DependencyProperty.Register("AlternateSetsChildPortSpot", typeof(bool), typeof(TreeLayout),
        new FrameworkPropertyMetadata(true, OnAlternateSetsChildPortSpotChanged));
      AlternateChildPortSpotProperty = DependencyProperty.Register("AlternateChildPortSpot", typeof(Spot), typeof(TreeLayout),
        new FrameworkPropertyMetadata(Spot.Default, OnAlternateChildPortSpotChanged));
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="Network"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NetworkProperty;
    /// <summary>
    /// Gets or sets the <see cref="TreeNetwork"/> that the layout will be performed on.
    /// </summary>
    /// <value>
    /// The initial value is null.
    /// </value>
    public TreeNetwork Network {
      get { return (TreeNetwork)GetValue(NetworkProperty); }
      set { SetValue(NetworkProperty, value); }
    }
    private static void OnNetworkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      TreeNetwork net = (TreeNetwork)e.NewValue;
      if (net != null) net.Layout = layout;
    }

    /// <summary>
    /// Allocate a <see cref="TreeNetwork"/>.
    /// </summary>
    /// <returns></returns>
    public virtual TreeNetwork CreateNetwork() {
      TreeNetwork n = new TreeNetwork();
      n.Layout = this;
      return n;
    }

    /// <summary>
    /// Create and initialize a <see cref="TreeNetwork"/> with the given nodes and links.
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="links"></param>
    /// <returns>a <see cref="TreeNetwork"/></returns>
    /// <remarks>
    /// This does not include any nodes of category "Comment".
    /// Comment nodes are added by the <see cref="AddComments"/> method.
    /// </remarks>
    public virtual TreeNetwork MakeNetwork(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      TreeNetwork net = CreateNetwork();
      net.AddNodesAndLinks(nodes.Where(n => n.Category != "Comment"), links);
      return net;
    }

    /// <summary>
    /// Do a tree layout.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If you are re-using this <c>TreeLayout</c> with the same diagram,
    /// you should set <see cref="Network"/> to null or a new <see cref="TreeNetwork"/>.
    /// </para>
    /// <para>
    /// If you are re-using the same <see cref="TreeNetwork"/> too,
    /// you should set the <see cref="Roots"/> properties, either
    /// by clearing it and letting <see cref="FindRoots"/> do its job,
    /// or by specifying all the <see cref="Node"/>s that
    /// are the roots of your trees.
    /// </para>
    /// </remarks>
    public override void DoLayout(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      if (this.Network == null) {
        this.Network = MakeNetwork(nodes, links);
      }
      if (this.Arrangement != TreeArrangement.FixedRoots) {
        this.ArrangementOrigin = InitialOrigin(this.ArrangementOrigin);
      }

      // Progress update.
      RaiseProgress(0);

      if (this.Network.VertexCount > 0) {
        CreateTrees();  // protected non-recursive method calls protected recursive WalkTree
        RaiseProgress(0.1);
        InitializeAll();  // internal recursive method calls protected non-recursive InitializeTreeVertexValues
        RaiseProgress(0.15);
        AssignAll();  // internal recursive method calls protected non-recursive AssignTreeVertexValues
        RaiseProgress(0.2);
        SortAll();  // internal recursive method calls protected non-recursive SortTreeVertexChildren
        RaiseProgress(0.3);
        AnnotateAll();  // protected non-recursive method calls protected non-recursive method AddComments
        RaiseProgress(0.4);
        LayoutAllTrees();  // internal non-recursive method calls protected recursive method LayoutTree
        RaiseProgress(0.6);
        ArrangeTrees();  // protected non-recursive method
        RaiseProgress(0.7);

        // Update the "physical" positions of the nodes and links.
        if (this.Diagram != null && !this.Diagram.CheckAccess()) {
          Diagram.InvokeLater(this.Diagram, UpdateParts);
        } else {
          UpdateParts();
        }
      }
      // Progress update.
      RaiseProgress(1);

      this.Network = null;
      this.Roots = new List<Node>();
    }

    private void UpdateParts() {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram != null) diagram.StartTransaction("TreeLayout");
      LayoutNodesAndLinks();
      if (diagram != null) diagram.CommitTransaction("TreeLayout");
      if (diagram != null && diagram.LayoutManager != null) {
        TreeNetwork net = this.Network;
        diagram.LayoutManager.AddUpdateLinks(this, () => {
          this.Network = net;
          LayoutLinks();
          this.Network = null;
        });
      }
    }


    /// <summary>
    /// Identifies the <see cref="Roots"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RootsProperty;
    /// <summary>
    /// Gets or sets a <see cref="List{Node}"/> of root <see cref="Northwoods.GoXam.Node"/>s.
    /// </summary>
    /// <value>
    /// Initially this will be an empty collection.
    /// If the <see cref="Path"/> is either <see cref="TreePath.Destination"/> or
    /// <see cref="TreePath.Source"/>, <see cref="FindRoots"/> can easily
    /// determine all of the tree roots by searching the whole network.
    /// Otherwise, you should explicitly initialize this collection.
    /// </value>
    public ICollection<Node> Roots {
      get { return (ICollection<Node>)GetValue(RootsProperty); }
      set { SetValue(RootsProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="Path"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PathProperty;
    /// <summary>
    /// Gets or sets how the tree should be constructed from the
    /// <see cref="TreeEdge"/>s connecting <see cref="TreeVertex"/>s.
    /// </summary>
    /// <value>
    /// The default value is <see cref="TreePath.Destination"/>.
    /// </value>
    [DefaultValue(TreePath.Destination)]
    public TreePath Path {
      get { return (TreePath)GetValue(PathProperty); }
      set { SetValue(PathProperty, value); }
    }

    /// <summary>
    /// This method is responsible for initializing all of the <see cref="TreeVertex"/>s,
    /// setting <see cref="TreeVertex.Initialized"/>, <see cref="TreeVertex.Level"/>,
    /// <see cref="TreeVertex.Parent"/>, and <see cref="TreeVertex.Children"/>,
    /// and making sure <see cref="Roots"/>
    /// has at least one suitable <see cref="Northwoods.GoXam.Node"/> in it.
    /// </summary>
    /// <remarks>
    /// This will call <see cref="FindRoots"/> if <see cref="Roots"/> is empty.
    /// Then it will iterate over the roots, calling <see cref="WalkTree"/> on each one.
    /// </remarks>
    protected virtual void CreateTrees() {
      //??? support incremental layouts
      this.Network.DeleteSelfEdges();
      foreach (TreeVertex v in this.Network.Vertexes) {
        v.Initialized = false;
        v.Level = 0;
        v.Parent = null;
        v.Children = TreeVertex.NoChildren;
      }
      // make sure all Roots Nodes have corresponding TreeVertexs
      foreach (Node r in this.Roots.ToArray()) {
        TreeVertex v = this.Network.FindVertex(r);
        if (v == null) {
          this.Roots.Remove(r);
        }
      }
      if (this.Roots.Count == 0) {
        FindRoots();
      }
      foreach (Node r in this.Roots.ToArray()) {
        TreeVertex v = this.Network.FindVertex(r);
        if (v != null && !v.Initialized) {
          // WalkTree will remove objects in the Roots collection that are child nodes
          v.Initialized = true;
          WalkTree(v);
        }
      }
    }

    /// <summary>
    /// This method is responsible for finding all of the root nodes.
    /// </summary>
    /// <remarks>
    /// When you have not already added the roots to the <see cref="Roots"/> collection, this will choose a root node.
    /// The choice might not be what you want, so we recommend that you specify the <see cref="Roots"/>.
    /// </remarks>
    protected virtual void FindRoots() {
      foreach (TreeVertex v in this.Network.Vertexes) {
        switch (this.Path) {
          case TreePath.Destination:
            if (v.SourceEdgesCount == 0) this.Roots.Add(v.Node);
            break;
          case TreePath.Source:
            if (v.DestinationEdgesCount == 0) this.Roots.Add(v.Node);
            break;
          default:
            throw new InvalidOperationException("Unhandled Path value " + this.Path.ToString());
        }
      }
      if (this.Roots.Count == 0) {
        ChooseRoot();
      }
    }

    private void ChooseRoot() {
      int mincount = 999999;
      TreeVertex bestroot = null;
      foreach (TreeVertex v in this.Network.Vertexes) {
        switch (this.Path) {
          case TreePath.Destination:
            if (v.SourceEdgesCount < mincount) { mincount = v.SourceEdgesCount; bestroot = v; }
            break;
          case TreePath.Source:
            if (v.DestinationEdgesCount < mincount) { mincount = v.DestinationEdgesCount; bestroot = v; }
            break;
          default:
            throw new InvalidOperationException("Unhandled Path value " + this.Path.ToString());
        }
      }
      if (bestroot != null)
        this.Roots.Add(bestroot.Node);
    }

    /// <summary>
    /// Traverse the <see cref="Network"/> and assign the <see cref="TreeVertex.Parent"/>,
    /// <see cref="TreeVertex.Level"/>, and <see cref="TreeVertex.Children"/> properties.
    /// </summary>
    /// <param name="v"></param>
    /// <remarks>
    /// This method should walk the tree recursively.
    /// The standard implementation gracefully handles shared nodes, including cyclical references.
    /// However, there can only be one <see cref="TreeVertex.Parent"/> per node.
    /// If a node is declared to be the child of multiple nodes, it is not guaranteed which
    /// node wins as the parent node.
    /// If any of the children of this node are in the <see cref="Roots"/> collection,
    /// they are removed from that collection.
    /// </remarks>
    protected virtual void WalkTree(TreeVertex v) {  //?? this could be optimized
      if (v == null) return;
      // handle DAGs gracefully, and ignore cycles
      switch (this.Path) {
        case TreePath.Destination:
          if (v.DestinationEdgesCount > 0) {
            TreeNetwork.VertexList list = new TreeNetwork.VertexList();
            foreach (TreeVertex c in v.DestinationVertexes) { if (WalkOK(v, c)) list.Add(c);
            }
            if (list.Count > 0) v.Children = list.ToArray();
          }
          break;
        case TreePath.Source:
          if (v.SourceEdgesCount > 0) {
            TreeNetwork.VertexList list = new TreeNetwork.VertexList();
            foreach (TreeVertex c in v.SourceVertexes) { if (WalkOK(v, c)) list.Add(c); }
            if (list.Count > 0) v.Children = list.ToArray();
          }
          break;
        default:
          throw new InvalidOperationException("Unhandled Path value " + this.Path.ToString());
      }
      // set Initialized before calling WalkTree
      foreach (TreeVertex c in v.Children) {
        c.Initialized = true;
        c.Level = v.Level+1;
        c.Parent = v;
        this.Roots.Remove(c.Node);
      }
      foreach (TreeVertex c in v.Children) {
        WalkTree(c);
      }
    }

    private bool WalkOK(TreeVertex v, TreeVertex c) {
      if (c.Initialized) {
        if (IsAncestor(c, v)) return false;
        if (c.Level > v.Level) return false;
        RemoveChild(c.Parent, c);
        // new Level will be assigned just before calling WalkTree on C
        return true;
      } else {
        return true;
      }
    }

    private bool IsAncestor(TreeVertex a, TreeVertex b) {
      if (b == null) return false;
      TreeVertex x = b.Parent;
      while (x != null && x != a) x = x.Parent;
      return (x == a);
    }

    private void RemoveChild(TreeVertex p, TreeVertex c) {
      if (p == null) return;
      if (c == null) return;
      TreeVertex[] v = p.Children;
      int num = 0;
      for (int i = 0; i < v.Length; i++) if (v[i] == c) num++;
      if (num > 0) {
        TreeVertex[] r = new TreeVertex[v.Length-num];
        int j = 0;
        for (int i = 0; i < v.Length; i++) if (v[i] != c) r[j++] = v[i];
        p.Children = r;
      }
    }


    /// <summary>
    /// Identifies the <see cref="TreeStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TreeStyleProperty;
    /// <summary>
    /// Gets or sets the <see cref="TreeStyle"/> for the resulting trees.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Northwoods.GoXam.Layout.TreeStyle.Layered"/>.
    /// </value>
    [DefaultValue(TreeStyle.Layered)]
    public TreeStyle TreeStyle {
      get { return (TreeStyle)GetValue(TreeStyleProperty); }
      set { SetValue(TreeStyleProperty, value); }
    }


    private void InitializeAll() {
      foreach (Node r in this.Roots) {
        TreeVertex v = this.Network.FindVertex(r);
        InitializeTree(v);
      }
    }

    private void InitializeTree(TreeVertex v) {
      if (v == null) return;
      InitializeTreeVertexValues(v);
      int count = 0;
      int maxchildren = v.ChildrenCount;
      int maxgeneration = 0;
      foreach (TreeVertex c in v.Children) {
        InitializeTree(c);
        count += c.DescendentCount+1;
        maxchildren = Math.Max(maxchildren, c.MaxChildrenCount);
        maxgeneration = Math.Max(maxgeneration, c.MaxGenerationCount);
      }
      v.DescendentCount = count;
      v.MaxChildrenCount = maxchildren;
      v.MaxGenerationCount = (maxchildren > 0 ? maxgeneration+1 : 0);
    }

    internal TreeVertex Mom(TreeVertex v) {
      switch (this.TreeStyle) {
        default:
        case TreeStyle.Layered:
          if (v.Parent != null) return v.Parent;
          return this.RootDefaults;
        case TreeStyle.RootOnly:
          if (v.Parent == null) return this.RootDefaults;
          if (v.Parent.Parent == null) return this.AlternateDefaults;
          return v.Parent;
        case TreeStyle.Alternating:
          if (v.Parent != null) {
            if (v.Parent.Parent != null) return v.Parent.Parent;
            return this.AlternateDefaults;
          }
          return this.RootDefaults;
        case TreeStyle.LastParents: {
          bool lastparent = true;
          if (v.ChildrenCount == 0) {
            lastparent = false;
          } else {
            foreach (TreeVertex c in v.Children) {
              if (c.ChildrenCount > 0) {
                lastparent = false;
                break;
              }
            }
          }
          if (lastparent && v.Parent != null) return this.AlternateDefaults;
          if (v.Parent != null) return v.Parent;
          return this.RootDefaults;
        }
      }
    }

    /// <summary>
    /// Assign initial property values for a <see cref="TreeVertex"/>.
    /// </summary>
    /// <param name="v"></param>
    /// <remarks>
    /// The values may be inherited, so this method is called while
    /// propagating values from the root nodes.
    /// This method should not walk the tree, since it is called for each
    /// <see cref="TreeVertex"/> in a depth-first manner starting at a root.
    /// You probably do not need to override this method,
    /// but if you do you should call first either the base method
    /// or <see cref="TreeVertex.CopyInheritedPropertiesFrom"/>, since they
    /// assign most of the <see cref="TreeVertex"/> property values
    /// used to influence the layout.
    /// Informational properties such as <see cref="TreeVertex.DescendentCount"/>
    /// and <see cref="TreeVertex.MaxGenerationCount"/> will not yet have been initialized
    /// by the time this method is called.
    /// It is more common to override <see cref="AssignTreeVertexValues"/> in order to
    /// modify a property or two to customize the layout at that node.
    /// </remarks>
    /// <seealso cref="AssignTreeVertexValues"/>
    protected virtual void InitializeTreeVertexValues(TreeVertex v) {
      TreeVertex mom = Mom(v);
      v.CopyInheritedPropertiesFrom(mom);
      v.Initialized = true;  // should already be true, but make sure
    }

    private void AssignAll() {
      foreach (Node r in this.Roots) {
        TreeVertex v = this.Network.FindVertex(r);
        AssignTree(v);
      }
    }

    private void AssignTree(TreeVertex v) {
      if (v == null) return;
      AssignTreeVertexValues(v);
      foreach (TreeVertex c in v.Children) {
        AssignTree(c);
      }
    }

    /// <summary>
    /// Assign final property values for a <see cref="TreeVertex"/>.
    /// </summary>
    /// <param name="v"></param>
    /// <remarks>
    /// This method is commonly overidden in order to provide
    /// tree layout properties for particular nodes.
    /// This method is called after values have been inherited from other
    /// <see cref="TreeVertex"/>s, so you can examine and modify the
    /// values of related tree nodes.
    /// This method should not walk the tree, since it is called for each
    /// <see cref="TreeVertex"/> in a depth-first manner starting at a root.
    /// </remarks>
    /// <seealso cref="InitializeTreeVertexValues"/>
    protected virtual void AssignTreeVertexValues(TreeVertex v) {
      //??? try to make layouts "square"
      // only when limited breadth and when there are children
      //if (n.BreadthLimit > 0 && n.DescendentCount > 0) {
      //  if (n.MaxGenerationCount == 1) {  // just leaf nodes as children
      //    if (n.DescendentCount > 10) {
      //      double angle = OrthoAngle(n);
      //      double b = (angle == 90 || angle == 270) ? n.Width : n.Height;
      //      n.BreadthLimit = (b+n.NodeSpacing)*((double)Math.Ceiling(Math.Sqrt(n.DescendentCount)));
      //    }
      //  } else if (n.DescendentCount > 1) {  // apportion the breadth
      //    foreach (TreeVertex c in n.Children) {
      //      if (c.DescendentCount > 10) {
      //        double s = (double)Math.Sqrt((double)(c.DescendentCount+1)/(double)n.DescendentCount);
      //        c.BreadthLimit = n.BreadthLimit * s;
      //      }
      //    }
      //  }
      //}
    }


    private void SortAll() {
      foreach (Node r in this.Roots) {
        SortTree(this.Network.FindVertex(r));
      }
    }

    private void SortTree(TreeVertex v) {
      if (v == null) return;
      SortTreeVertexChildren(v);
      foreach (TreeVertex c in v.Children) {
        SortTree(c);
      }
    }

    /// <summary>
    /// Sort the <see cref="TreeVertex.Children"/> of a node.
    /// </summary>
    /// <param name="v"></param>
    /// <remarks>
    /// This method should not walk the tree, since it is called for each
    /// <see cref="TreeVertex"/> in a breadth-first manner starting at a root.
    /// </remarks>
    protected virtual void SortTreeVertexChildren(TreeVertex v) {
      switch (v.Sorting) {
        case TreeSorting.Forwards:  // nothing needed
          break;
        case TreeSorting.Reverse:  // just reverse the array
          Array.Reverse(v.Children, 0, v.Children.Length);
          break;
        case TreeSorting.Ascending:  // sort using Comparer
          Array.Sort(v.Children, 0, v.Children.Length, v.Comparer);
          break;
        case TreeSorting.Descending:  // sort, then reverse
          Array.Sort(v.Children, 0, v.Children.Length, v.Comparer);
          Array.Reverse(v.Children, 0, v.Children.Length);
          break;
        default:
          throw new InvalidOperationException("Unhandled Sorting value " + v.Sorting.ToString());
      }
    }


    private void AnnotateAll() {
      foreach (TreeVertex v in this.Network.Vertexes) {
        AddComments(v);
      }
    }

    /// <summary>
    /// Find associated objects to be positioned along with the
    /// <see cref="GenericNetwork{N, L, Y}"/>.<see cref="GenericNetwork{N, L, Y}.Vertex"/>.
    /// </summary>
    /// <param name="v"></param>
    /// <remarks>
    /// This looks for <see cref="Northwoods.GoXam.Node"/> objects
    /// whose category is "Comment" and that refer to
    /// the tree vertex's <see cref="GenericNetwork{N, L, Y}.Vertex.Node"/>.
    /// You may want to override this method in order to customize how
    /// any associated objects are found and how
    /// the node's <see cref="GenericNetwork{N, L, Y}.Vertex.Bounds"/>
    /// are set to reserve space for those associated objects.
    /// This method should not walk the tree, since it is called for each
    /// <see cref="TreeVertex"/> in an indeterminate order.
    /// </remarks>
    /// <seealso cref="LayoutComments"/>
    protected virtual void AddComments(TreeVertex v) {
      double a = v.Angle;
      bool vertical = (a == 90 || a == 270);
      bool leaf = (v.ChildrenCount == 0);
      Rect extra = new Rect();
      double spacing = 0;
      if (v.Node != null) {
        IEnumerable<Node> comments = v.Node.NodesConnected.Where(n => n.Category == "Comment");
        foreach (Node comment in comments) {
          if (v.Comments == null) v.Comments = new List<Node>();
          v.Comments.Add(comment);
          Rect cb = comment.Bounds;
          if ((vertical && !leaf) || (!vertical && leaf)) {  // if vertical tree, leave room for comments on the side, unless it's a leaf
            extra.Width = Math.Max(extra.Width, cb.Width);
            extra.Height += cb.Height + spacing;
          } else {
            extra.Width += cb.Width + spacing;
            extra.Height = Math.Max(extra.Height, cb.Height);
          }
          spacing = v.CommentSpacing;
        }
      }
      if (v.Comments != null && v.Comments.Count > 0) {
        if ((vertical && !leaf) || (!vertical && leaf)) {
          extra.Width += v.CommentMargin;
          if (a > 135 && leaf) {
            extra.X = -extra.Width;
            v.Focus = new Point(v.Focus.X + extra.Width, v.Focus.Y);
          }
          extra.Height = Math.Max(0, extra.Height - v.Height);
        } else {
          extra.Height += v.CommentMargin;
          if (a > 135 && leaf) {
            extra.Y = -extra.Height;
            v.Focus = new Point(v.Focus.X, v.Focus.Y+ extra.Height);
          }
          extra.Width = Math.Max(0, extra.Width - v.Width);
        }
        v.Bounds = new Rect(v.Bounds.X + extra.X, v.Bounds.Y + extra.Y, v.Bounds.Width + extra.Width, v.Bounds.Height + extra.Height);
      }
    }

    /// <summary>
    /// Position any <see cref="TreeVertex.Comments"/> around the vertex.
    /// </summary>
    /// <param name="v"></param>
    /// <remarks>
    /// This method should not walk the tree, since it is called for each
    /// <see cref="TreeVertex"/> in an indeterminate order.
    /// </remarks>
    /// <seealso cref="AddComments"/>
    protected virtual void LayoutComments(TreeVertex v) {
      if (v.Comments != null) {
        Node node = v.Node;
        Rect nodebounds = node.Bounds;
        double a = v.Angle;
        bool vertical = (a == 90 || a == 270);
        bool leaf = (v.ChildrenCount == 0);
        double f = 0;
        foreach (Node c in v.Comments) {
          if ((vertical && !leaf) || (!vertical && leaf)) {
            if (a > 135 && leaf)
              c.Location = new Point(nodebounds.Left - v.CommentMargin - c.Width, nodebounds.Top + f);  //??? assumes LocationSpot is TopLeft for each comment
            else
              c.Location = new Point(nodebounds.Right + v.CommentMargin, nodebounds.Top + f);
            f += c.Height + v.CommentSpacing;
          } else {
            if (a > 135 && leaf)
              c.Location = new Point(nodebounds.Left + f, nodebounds.Top - v.CommentMargin - c.Height);
            else
              c.Location = new Point(nodebounds.Left + f, nodebounds.Bottom + v.CommentMargin);
            f += c.Width + v.CommentSpacing;
          }
          //IGoRoutable rout = c as IGoRoutable;
          //if (rout != null) rout.UpdateRoute();
        }
      }
    }


    private void LayoutAllTrees() {
      foreach (Node r in this.Roots) {
        LayoutTree(this.Network.FindVertex(r));  // protected recursive method
      }
    }

    /// <summary>
    /// Recursively lay out a subtree starting with the given parent node.
    /// </summary>
    /// <param name="v"></param>
    /// <remarks>
    /// This method should walk the tree recursively.
    /// </remarks>
    protected virtual void LayoutTree(TreeVertex v) {
      if (v == null) return;
      switch (v.Compaction) {
        case TreeCompaction.None:
          LayoutTreeNone(v);
          break;
        case TreeCompaction.Block:
          LayoutTreeBlock(v);
          break;
        default:
          throw new InvalidOperationException("Unhandled Compaction value " + v.Compaction.ToString());
      }
    }

    // if TreeCompaction.None
    private void LayoutTreeNone(TreeVertex v) {
      if (v == null) return;
      if (v.ChildrenCount == 0) {
        v.RelativePosition = new Point(0, 0);
        v.SubtreeSize = v.Size;
        v.SubtreeOffset = new Point(0, 0);
        return;
      }
      double angle = OrthoAngle(v);
      bool vertical = (angle == 90 || angle == 270);
      double maxsubbreadth = 0;
      foreach (TreeVertex child in v.Children) {
        LayoutTree(child);  // layout children first
        maxsubbreadth = Math.Max(maxsubbreadth, (vertical ? child.SubtreeSize.Width : child.SubtreeSize.Height));
      }
      TreeAlignment align = v.Alignment;
      bool alignstart = (align == TreeAlignment.Start);
      bool alignend = (align == TreeAlignment.End);
      double limit = Math.Max(0, v.BreadthLimit);
      double layerspacing = ComputeLayerSpacing(v);  // limit layerspacing to be no more negative than the depth of the node
      double nodespacing = v.NodeSpacing;
      double nodeindent = ComputeNodeIndent(v);
      double initialspacing = (alignstart ? nodeindent : (alignend ? 0 : nodeindent/2));
      double rowspacing = v.RowSpacing;
      double rowindent = 0;
      if (alignstart || alignend || v.RouteAroundCentered || (v.RouteAroundLastParent && v.MaxGenerationCount == 1)) {
        rowindent = Math.Max(0, v.RowIndent);
      }
      double subwidth = 0;
      double subheight = 0;
      double olddepth = 0;
      double x = 0;
      double y = 0;
      int row = 0;
      int col = 0;
      int firstidx = 0;
      TreeVertex[] children = v.Children;
      for (int idx = 0; idx < children.Length; idx++) {
        TreeVertex child = children[idx];
        if (vertical) {
          if (limit > 0 && col > 0 && x + nodespacing + child.SubtreeSize.Width > limit) {  // need new row
            if (x < maxsubbreadth) {  // align the previous row
              ShiftRelPosAlign(v, align, new Size(maxsubbreadth-x, 0), firstidx, idx-1);
            }
            row++;
            col = 0;  // start new row
            firstidx = idx;  // remember index of first child in this row
            olddepth = subheight;  // remember current depth for this row
            x = 0;
            y = (angle > 135 ? -subheight-rowspacing : subheight+rowspacing);
          }
          double nspace = (col == 0 ? initialspacing : nodespacing);
          RecordMidPoints(child, new Point(0, y));  // for links being routed around rows
          child.RelativePosition = new Point(x + nspace, y);
          subwidth = Math.Max(subwidth, x + nspace + child.SubtreeSize.Width);
          subheight = Math.Max(subheight, olddepth + (row == 0 ? 0 : rowspacing) + child.SubtreeSize.Height);
          x += nspace + child.SubtreeSize.Width;
        } else {  // horizontal growth direction
          if (limit > 0 && col > 0 && y + nodespacing + child.SubtreeSize.Height > limit) {  // need new row
            if (y < maxsubbreadth) {  // align the previous row
              ShiftRelPosAlign(v, align, new Size(0, maxsubbreadth-y), firstidx, idx-1);
            }
            row++;
            col = 0;  // start new row
            firstidx = idx;
            olddepth = subwidth;  // remember current depth for this row
            y = 0;
            x = (angle > 135 ? -subwidth-rowspacing : subwidth+rowspacing);
          }
          double nspace = (col == 0 ? initialspacing : nodespacing);
          RecordMidPoints(child, new Point(x, 0));
          child.RelativePosition = new Point(x, y + nspace);
          subheight = Math.Max(subheight, y + nspace + child.SubtreeSize.Height);
          subwidth = Math.Max(subwidth, olddepth + (row == 0 ? 0 : rowspacing) + child.SubtreeSize.Width);
          y += nspace + child.SubtreeSize.Height;
        }
        col++;
      }
      if (row > 0) {  // if there were multiple rows...
        if (vertical) {
          subheight += Math.Max(0, layerspacing);  // leave extra room past last row
          if (x < subwidth) {  // align the last row
            ShiftRelPosAlign(v, align, new Size(subwidth-x, 0), firstidx, children.Length-1);
          }
          if (rowindent > 0) {
            if (!alignend) ShiftRelPos(v, new Size(rowindent, 0), 0, children.Length-1);
            subwidth += rowindent;
          }
        } else {
          subwidth += Math.Max(0, layerspacing);  // leave extra room past last row
          if (y < subheight) {  // align the last row
            ShiftRelPosAlign(v, align, new Size(0, subheight-y), firstidx, children.Length-1);
          }
          if (rowindent > 0) {
            if (!alignend) ShiftRelPos(v, new Size(0, rowindent), 0, children.Length-1);
            subheight += rowindent;
          }
        }
      }
      // align this parent node relative to all of its children/descendents
      Point offset = new Point(0, 0);
      switch (align) {
        case TreeAlignment.CenterSubtrees:
          if (vertical) {
            offset.X += (subwidth-v.Width)/2;
          } else {
            offset.Y += (subheight-v.Height)/2;
          }
          break;
        case TreeAlignment.CenterChildren:
          if (row > 0) {  // same as CenterSubtrees when there's multiple rows
            if (vertical) {
              offset.X += (subwidth-v.Width)/2;
            } else {
              offset.Y += (subheight-v.Height)/2;
            }
          } else {
            int num = v.ChildrenCount;
            if (vertical) {
              double first = v.Children[0].SubtreeOffset.X;
              double last = v.Children[num-1].RelativePosition.X + v.Children[num-1].SubtreeOffset.X + v.Children[num-1].Width;
              offset.X += first + (last-first-v.Width)/2;
            } else {
              double first = v.Children[0].SubtreeOffset.Y;
              double last = v.Children[num-1].RelativePosition.Y + v.Children[num-1].SubtreeOffset.Y + v.Children[num-1].Height;
              offset.Y += first + (last-first-v.Height)/2;
            }
          }
          break;
        case TreeAlignment.Start:
          break;
        case TreeAlignment.End:
          if (vertical) {
            subwidth += nodeindent;
            offset.X += (subwidth-v.Width);
          } else {
            subheight += nodeindent;
            offset.Y += (subheight-v.Height);
          }
          break;
        default:
          throw new InvalidOperationException("Unhandled Alignment value " + align.ToString());
      }
      // fix up all children's RelativePositions relative to the desired position of this parent node
      foreach (TreeVertex child in v.Children) {
        if (vertical) {
          child.RelativePosition = new Point(
            child.RelativePosition.X + child.SubtreeOffset.X - offset.X,
            child.RelativePosition.Y + ((angle > 135) ?
              (-child.SubtreeSize.Height + child.SubtreeOffset.Y - layerspacing) :
              (v.Height + layerspacing + child.SubtreeOffset.Y)));
        } else {
          child.RelativePosition = new Point(
            child.RelativePosition.X + ((angle > 135) ?
              (-child.SubtreeSize.Width + child.SubtreeOffset.X - layerspacing) :
              (v.Width + layerspacing + child.SubtreeOffset.X)),
            child.RelativePosition.Y + child.SubtreeOffset.Y - offset.Y);
        }
      }
      // include the NODE itself in the subtree size, and adjust the offset if the direction is negative
      if (vertical) {
        if (v.Width > subwidth) {
          subwidth = v.Width;
          offset.X = 0;
        }
        if (angle > 135) offset.Y += subheight+layerspacing;
        subheight = Math.Max(Math.Max(subheight, v.Height), subheight+v.Height+layerspacing);
      } else {
        if (angle > 135) offset.X += subwidth+layerspacing;
        subwidth = Math.Max(Math.Max(subwidth, v.Width), subwidth+v.Width+layerspacing);
        if (v.Height > subheight) {
          subheight = v.Height;
          offset.Y = 0;
        }
      }
      // save results
      v.SubtreeOffset = offset;
      v.SubtreeSize = new Size(subwidth, subheight);
    }


    private static Size AlignOffset(TreeAlignment align, Size off) {
      switch (align) {
        case TreeAlignment.CenterSubtrees: off.Width /= 2; off.Height /= 2; break;
        case TreeAlignment.CenterChildren: off.Width /= 2; off.Height /= 2; break;
        case TreeAlignment.Start: off.Width = 0; off.Height = 0; break;
        case TreeAlignment.End: break;
        default: throw new InvalidOperationException("Unhandled Alignment value " + align.ToString());
      }
      return off;
    }

    private void ShiftRelPosAlign(TreeVertex v, TreeAlignment align, Size off, int first, int last) {
      Size shift = AlignOffset(align, off);
      ShiftRelPos(v, shift, first, last);
    }

    private void ShiftRelPos(TreeVertex v, Size off, int first, int last) {
      if (off.Width == 0 && off.Height == 0) return;
      TreeVertex[] children = v.Children;
      for (int i = first; i <= last; i++) {
        TreeVertex c = children[i];
        Point p = c.RelativePosition;
        p.X += off.Width;
        p.Y += off.Height;
        c.RelativePosition = p;
      }
    }

    // tell TreeEdges their parent node relative coordinates
    private void RecordMidPoints(TreeVertex v, Point p) {
      TreeVertex parent = v.Parent;
      switch (this.Path) {
        case TreePath.Destination:
          foreach (TreeEdge e in v.SourceEdges) {
            if (e.FromVertex == parent) {
              e.RelativePoint = p;
            }
          }
          break;
        case TreePath.Source:
          foreach (TreeEdge e in v.DestinationEdges) {
            if (e.ToVertex == parent) {
              e.RelativePoint = p;
            }
          }
          break;
        default:
          throw new InvalidOperationException("Unhandled Path value " + this.Path.ToString());
      }
    }


    // if TreeCompaction.Block
    private void LayoutTreeBlock(TreeVertex vertex) {
      if (vertex == null) return;
      if (vertex.ChildrenCount == 0) {
        vertex.RelativePosition = new Point(0, 0);
        vertex.SubtreeSize = vertex.Size;
        vertex.SubtreeOffset = new Point(0, 0);
        vertex.LeftFringe = null;
        vertex.RightFringe = null;
        return;
      }
      double angle = OrthoAngle(vertex);
      bool vertical = (angle == 90 || angle == 270);
      double maxsubbreadth = 0;
      foreach (TreeVertex child in vertex.Children) {
        LayoutTree(child);  // layout children first
        maxsubbreadth = Math.Max(maxsubbreadth, (vertical ? child.SubtreeSize.Width : child.SubtreeSize.Height));
      }
      TreeAlignment align = vertex.Alignment;
      bool alignstart = (align == TreeAlignment.Start);
      bool alignend = (align == TreeAlignment.End);
      double limit = Math.Max(0, vertex.BreadthLimit);
      double layerspacing = ComputeLayerSpacing(vertex);  // limit layerspacing to be no more negative than the depth of the node
      double nodespacing = vertex.NodeSpacing;
      double nodeindent = ComputeNodeIndent(vertex);
      double rowspacing = vertex.RowSpacing;
      double rowindent = 0;
      if (alignstart || alignend || vertex.RouteAroundCentered || (vertex.RouteAroundLastParent && vertex.MaxGenerationCount == 1)) {
        rowindent = Math.Max(0, vertex.RowIndent);
      }
      double subwidth = 0;
      double subheight = 0;
      double olddepth = 0;
      Point[] lfringe = null;
      Point[] rfringe = null;
      Size lsize = new Size(0, 0);
      double x = 0;
      double y = 0;
      int row = 0;
      int col = 0;
      int firstidx = 0;
      TreeVertex[] children = vertex.Children;
      for (int idx = 0; idx < children.Length; idx++) {
        TreeVertex child = children[idx];
        if (vertical) {
          if (limit > 0 && col > 0 && x + nodespacing + child.SubtreeSize.Width > limit) {  // need new row
            if (x < maxsubbreadth) {  // align the previous row
              ShiftRelPosAlign(vertex, align, new Size(maxsubbreadth-x, 0), firstidx, idx-1);
            }
            row++;  // start new row
            col = 0;  // number of nodes in this row
            firstidx = idx;  // remember index of first child in this row
            olddepth = subheight;  // remember current depth for this row
            x = 0;  // total width so far, excluding any indents
            y = (angle > 135 ? -subheight-rowspacing : subheight+rowspacing);
          }
          RecordMidPoints(child, new Point(0, y));  // for links being routed around rows
          double cpos = 0;
          if (col == 0) {  // get or create first set of fringes for these children
            lfringe = child.LeftFringe;
            rfringe = child.RightFringe;
            lsize = child.SubtreeSize;
            // if CHILD doesn't have fringes, or we can't use them, create them
            if (lfringe == null || rfringe == null || angle != OrthoAngle(child)) {
              lfringe = AllocTempPointArray(2);
              rfringe = AllocTempPointArray(2);
              lfringe[0] = new Point(0, 0);
              lfringe[1] = new Point(0, lsize.Height);
              rfringe[0] = new Point(lsize.Width, 0);
              rfringe[1] = new Point(lsize.Width, lsize.Height);
            }
          } else {
            cpos = MergeFringes(vertex, child, ref lfringe, ref rfringe, ref lsize);
            // if CHILD's subtree after merge extends "left" beyond original fringe,
            // we need to adjust all of the previous children's RelativePositions (starting at FIRSTIDX)
            // and normalize the fringes
            if (x < child.SubtreeSize.Width && cpos < 0) {
              ShiftRelPos(vertex, new Size(-cpos, 0), firstidx, idx-1);
              ShiftFringe(lfringe, new Size(-cpos, 0));
              ShiftFringe(rfringe, new Size(-cpos, 0));
              cpos = 0;
            }
          }
          child.RelativePosition = new Point(cpos, y);
          subwidth = Math.Max(subwidth, lsize.Width);
          subheight = Math.Max(subheight, olddepth + (row == 0 ? 0 : rowspacing) + child.SubtreeSize.Height);
          x = lsize.Width;
        } else {  // horizontal growth direction
          if (limit > 0 && col > 0 && y + nodespacing + child.SubtreeSize.Height > limit) {  // need new row
            if (y < maxsubbreadth) {  // align the previous row
              ShiftRelPosAlign(vertex, align, new Size(0, maxsubbreadth-y), firstidx, idx-1);
            }
            row++;  // start new row
            col = 0;  // number of nodes in this row
            firstidx = idx;  // remember index of first child in this row
            olddepth = subwidth;  // remember current depth for this row
            y = 0;  // total height so far, excluding any indents
            x = (angle > 135 ? -subwidth-rowspacing : subwidth+rowspacing);
          }
          RecordMidPoints(child, new Point(x, 0));  // for links being routed around rows
          double cpos = 0;
          if (col == 0) {  // get or create first set of fringes for these children
            lfringe = child.LeftFringe;
            rfringe = child.RightFringe;
            lsize = child.SubtreeSize;
            // if CHILD doesn't have fringes, or we can't use them, create them
            if (lfringe == null || rfringe == null || angle != OrthoAngle(child)) {
              lfringe = AllocTempPointArray(2);
              rfringe = AllocTempPointArray(2);
              lfringe[0] = new Point(0, 0);
              lfringe[1] = new Point(lsize.Width, 0);
              rfringe[0] = new Point(0, lsize.Height);
              rfringe[1] = new Point(lsize.Width, lsize.Height);
            }
          } else {
            cpos = MergeFringes(vertex, child, ref lfringe, ref rfringe, ref lsize);
            // if CHILD's subtree after merge extends "above" beyond original fringe,
            // we need to adjust all of the previous children's RelativePositions (starting at FIRSTIDX)
            // and normalize the fringes
            if (y < child.SubtreeSize.Height && cpos < 0) {
              ShiftRelPos(vertex, new Size(0, -cpos), firstidx, idx-1);
              ShiftFringe(lfringe, new Size(0, -cpos));
              ShiftFringe(rfringe, new Size(0, -cpos));
              cpos = 0;
            }
          }
          child.RelativePosition = new Point(x, cpos);
          subheight = Math.Max(subheight, lsize.Height);
          subwidth = Math.Max(subwidth, olddepth + (row == 0 ? 0 : rowspacing) + child.SubtreeSize.Width);
          y = lsize.Height;
        }
        col++;
      }
      if (row > 0) {  // if there were multiple rows...
        if (vertical) {
          subheight += Math.Max(0, layerspacing);  // leave extra room past last row
          if (x < subwidth) {  // align the last row
            ShiftRelPosAlign(vertex, align, new Size(subwidth-x, 0), firstidx, children.Length-1);
          }
          if (rowindent > 0) {
            if (!alignend) ShiftRelPos(vertex, new Size(rowindent, 0), 0, children.Length-1);
            subwidth += rowindent;
          }
        } else {
          subwidth += Math.Max(0, layerspacing);  // leave extra room past last row
          if (y < subheight) {  // align the last row
            ShiftRelPosAlign(vertex, align, new Size(0, subheight-y), firstidx, children.Length-1);
          }
          if (rowindent > 0) {
            if (!alignend) ShiftRelPos(vertex, new Size(0, rowindent), 0, children.Length-1);
            subheight += rowindent;
          }
        }
      }
      // align this parent node relative to all of its children/descendents
      Point offset = new Point(0, 0);
      switch (align) {
        case TreeAlignment.CenterSubtrees:
          if (vertical) {
            offset.X += (subwidth-vertex.Width-nodeindent)/2;
          } else {
            offset.Y += (subheight-vertex.Height-nodeindent)/2;
          }
          break;
        case TreeAlignment.CenterChildren:
          if (row > 0) {  // same as CenterSubtrees when there's multiple rows
            if (vertical) {
              offset.X += (subwidth-vertex.Width-nodeindent)/2;
            } else {
              offset.Y += (subheight-vertex.Height-nodeindent)/2;
            }
          } else {
            int num = vertex.ChildrenCount;
            if (vertical) {
              double first = vertex.Children[0].RelativePosition.X + vertex.Children[0].SubtreeOffset.X;
              double last = vertex.Children[num-1].RelativePosition.X + vertex.Children[num-1].SubtreeOffset.X + vertex.Children[num-1].Width;
              offset.X += first + (last-first-vertex.Width-nodeindent)/2;
            } else {
              double first = vertex.Children[0].RelativePosition.Y + vertex.Children[0].SubtreeOffset.Y;
              double last = vertex.Children[num-1].RelativePosition.Y + vertex.Children[num-1].SubtreeOffset.Y + vertex.Children[num-1].Height;
              offset.Y += first + (last-first-vertex.Height-nodeindent)/2;
            }
          }
          break;
        case TreeAlignment.Start:
          if (vertical) {
            offset.X -= nodeindent;
            subwidth += nodeindent;
          } else {
            offset.Y -= nodeindent;
            subheight += nodeindent;
          }
          break;
        case TreeAlignment.End:
          if (vertical) {
            offset.X += (subwidth-vertex.Width)+nodeindent;
            subwidth += nodeindent;
          } else {
            offset.Y += (subheight-vertex.Height)+nodeindent;
            subheight += nodeindent;
          }
          break;
        default:
          throw new InvalidOperationException("Unhandled Alignment value " + align.ToString());
      }
      // fix up all children's RelativePositions relative to the desired position of this parent node
      foreach (TreeVertex child in vertex.Children) {
        if (vertical) {
          child.RelativePosition = new Point(
            child.RelativePosition.X + child.SubtreeOffset.X - offset.X,
            child.RelativePosition.Y + ((angle > 135) ?
              (-child.SubtreeSize.Height + child.SubtreeOffset.Y - layerspacing) :
              (vertex.Height + layerspacing + child.SubtreeOffset.Y)));
        } else {
          child.RelativePosition = new Point(
            child.RelativePosition.X + ((angle > 135) ?
              (-child.SubtreeSize.Width + child.SubtreeOffset.X - layerspacing) :
              (vertex.Width + layerspacing + child.SubtreeOffset.X)),
            child.RelativePosition.Y + child.SubtreeOffset.Y - offset.Y);
        }
      }
      // include the NODE itself in the subtree size, and adjust the offset if the direction is negative
      Size fringeoff = new Size(0, 0);
      if (vertical) {
        if (vertex.Width > subwidth) {
          fringeoff = AlignOffset(align, new Size(vertex.Width-subwidth, 0));
          subwidth = vertex.Width;
          offset.X = 0;
        }
        if (offset.X < 0) {
          fringeoff.Width -= offset.X;  // increase fringe offset
          offset.X = 0;
        }
        if (angle > 135) offset.Y += subheight+layerspacing;
        subheight = Math.Max(Math.Max(subheight, vertex.Height), subheight+vertex.Height+layerspacing);
        fringeoff.Height += vertex.Height + layerspacing;
      } else {
        if (angle > 135) offset.X += subwidth+layerspacing;
        subwidth = Math.Max(Math.Max(subwidth, vertex.Width), subwidth+vertex.Width+layerspacing);
        if (vertex.Height > subheight) {
          fringeoff = AlignOffset(align, new Size(0, vertex.Height-subheight));
          subheight = vertex.Height;
          offset.Y = 0;
        }
        if (offset.Y < 0) {
          fringeoff.Height -= offset.Y;  // increase fringe offset
          offset.Y = 0;
        }
        fringeoff.Width += vertex.Width + layerspacing;
      }
      // create new fringes for the whole subtree
      Point[] newleft;
      Point[] newright;
      if (row > 0) {  // when there are multiple rows, just assume everything's a big block
        newleft = AllocTempPointArray(4);
        newright = AllocTempPointArray(4);
        if (vertical) {
          newleft[2]  = new Point(0, vertex.Height+layerspacing);
          newleft[3]  = new Point(newleft[2].X, subheight);
          newright[2] = new Point(subwidth, newleft[2].Y);
          newright[3] = new Point(newright[2].X, newleft[3].Y);
        } else {
          newleft[2]  = new Point(vertex.Width+layerspacing, 0);
          newleft[3]  = new Point(subwidth, newleft[2].Y);
          newright[2] = new Point(newleft[2].X, subheight);
          newright[3] = new Point(newleft[3].X, newright[2].Y);
        }
      } else {
        newleft = AllocTempPointArray(lfringe.Length + 2);
        newright = AllocTempPointArray(rfringe.Length + 2);
        for (int i = 0; i < lfringe.Length; i++) {
          Point p = lfringe[i];
          newleft[i + 2] = new Point(p.X + fringeoff.Width, p.Y + fringeoff.Height);
        }
        for (int i = 0; i < rfringe.Length; i++) {
          Point p = rfringe[i];
          newright[i + 2] = new Point(p.X + fringeoff.Width, p.Y + fringeoff.Height);
        }
      }
      // add the parent node to the fringe
      if (vertical) {
        newleft[0]  = new Point(offset.X, 0);
        newleft[1]  = new Point(newleft[0].X, vertex.Height);
        // handle negative LayerSpacing causing overlap in breadth
        if (newleft[2].Y < newleft[1].Y) {
          if (newleft[2].X > newleft[0].X)
            newleft[2] = newleft[1];  // inside parent node: use parent corner
          else
            newleft[1] = newleft[2];  // outside parent node: extend exterior
        }
        if (newleft[3].Y < newleft[2].Y) {
          if (newleft[3].X > newleft[0].X)
            newleft[3] = newleft[2];
          else
            newleft[2] = newleft[3];
        }
        newright[0] = new Point(offset.X + vertex.Width, 0);
        newright[1] = new Point(newright[0].X, vertex.Height);
        // handle negative LayerSpacing causing overlap in breadth
        if (newright[2].Y < newright[1].Y) {
          if (newright[2].X < newright[0].X)
            newright[2] = newright[1];  // inside parent node: use parent corner
          else
            newright[1] = newright[2];  // outside parent node: extend exterior
        }
        if (newright[3].Y < newright[2].Y) {
          if (newright[3].X < newright[0].X)
            newright[3] = newright[2];
          else
            newright[2] = newright[3];
        }
        // extend the fringe vertically so that orthogonal links don't extend outside
        newleft[2].Y -= layerspacing/2;
        newright[2].Y -= layerspacing/2;
      } else {
        newleft[0]  = new Point(0, offset.Y);
        newleft[1]  = new Point(vertex.Width, newleft[0].Y);
        // handle negative LayerSpacing causing overlap in breadth
        if (newleft[2].X < newleft[1].X) {
          if (newleft[2].Y > newleft[0].Y)
            newleft[2] = newleft[1];  // inside parent node: use parent corner
          else
            newleft[1] = newleft[2];  // outside parent node: extend exterior
        }
        if (newleft[3].X < newleft[2].X) {
          if (newleft[3].Y > newleft[0].Y)
            newleft[3] = newleft[2];
          else
            newleft[2] = newleft[3];
        }
        newright[0] = new Point(0, offset.Y + vertex.Height);
        newright[1] = new Point(vertex.Width, newright[0].Y);
        // handle negative LayerSpacing causing overlap in breadth
        if (newright[2].X < newright[1].X) {
          if (newright[2].Y < newright[0].Y)
            newright[2] = newright[1];  // inside parent node: use parent corner
          else
            newright[1] = newright[2];  // outside parent node: extend exterior
        }
        if (newright[3].X < newright[2].X) {
          if (newright[3].Y < newright[0].Y)
            newright[3] = newright[2];
          else
            newright[2] = newright[3];
        }
        // extend the fringe horizontally so that orthogonal links don't extend outside
        newleft[2].X -= layerspacing/2;
        newright[2].X -= layerspacing/2;
      }
      FreeTempPointArray(lfringe);
      FreeTempPointArray(rfringe);
      // save results
      vertex.LeftFringe = newleft;
      vertex.RightFringe = newright;
      vertex.SubtreeOffset = offset;
      vertex.SubtreeSize = new Size(subwidth, subheight);
    }

    static void ShiftFringe(Point[] fringe, Size offset) {
      for (int i = 0; i < fringe.Length; i++) {
        Point p = fringe[i];
        p.X += offset.Width;
        p.Y += offset.Height;
        fringe[i] = p;
      }
    }

    private double MergeFringes(TreeVertex parent, TreeVertex child, ref Point[] lfringe, ref Point[] rfringe, ref Size lsize) {
      double angle = OrthoAngle(parent);
      bool vertical = (angle == 90 || angle == 270);
      double nodespacing = parent.NodeSpacing;
      Point[] al = lfringe;
      Point[] ar = rfringe;
      Size asz = lsize;
      Point[] bl = child.LeftFringe;
      Point[] br = child.RightFringe;
      Size bsz = child.SubtreeSize;
      //double b = (vertical ? asz.Width + nodespacing + bsz.Width : asz.Height + nodespacing + bsz.Height);
      double d = (vertical ? Math.Max(asz.Height, bsz.Height) : Math.Max(asz.Width, bsz.Width));
      if (bl == null || angle != OrthoAngle(child)) {
        bl = AllocTempPointArray(2);
        br = AllocTempPointArray(2);
        if (vertical) {
          bl[0] = new Point(0, 0);
          bl[1] = new Point(0, bsz.Height);
          br[0] = new Point(bsz.Width, 0);
          br[1] = new Point(br[0].X, bl[1].Y);
        } else {
          bl[0] = new Point(0, 0);
          bl[1] = new Point(bsz.Width, 0);
          br[0] = new Point(0, bsz.Height);
          br[1] = new Point(bl[1].X, br[0].Y);
        }
      }
      // handle general case
      if (vertical) {
        double abreadth = asz.Width;
        double m = abreadth - FringeDistanceX(ar, bl, abreadth);
        m += nodespacing;
        lfringe = FringeUnionLeftX(al, bl, m);
        rfringe = FringeUnionRightX(ar, br, m);
        lsize = new Size(Math.Max(0, m) + bsz.Width, d);
        FreeTempPointArray(al);
        FreeTempPointArray(bl);
        FreeTempPointArray(ar);
        FreeTempPointArray(br);
        return m;
      } else {
        double abreadth = asz.Height;
        double m = abreadth - FringeDistanceY(ar, bl, abreadth);
        m += nodespacing;
        lfringe = FringeUnionLeftY(al, bl, m);
        rfringe = FringeUnionRightY(ar, br, m);
        lsize = new Size(d, Math.Max(0, m) + bsz.Height);
        FreeTempPointArray(al);
        FreeTempPointArray(bl);
        FreeTempPointArray(ar);
        FreeTempPointArray(br);
        return m;
      }
    }

    Point[] FringeUnionLeftY(Point[] av, Point[] bv, double offset) {
      if (av == null || av.Length < 2 || bv == null || bv.Length < 2) return null;  // not enough points!
      Point[] cv = AllocTempPointArray(av.Length + bv.Length);
      int i = 0;
      int j = 0;
      int k = 0;
      Point c;
      // copy any leading points of BV, shifted by OFFSET
      while (j < bv.Length && bv[j].X < av[0].X) {
        c = bv[j++];
        c.Y += offset;
        cv[k++] = c;
      }
      // copy all of AV
      while (i < av.Length) {
        c = av[i++];
        cv[k++] = c;
      }
      // skip the part of BV that is covered by AV
      double lastX = av[av.Length-1].X;
      while (j < bv.Length && bv[j].X <= lastX) {
        j++;
      }
      // copy the trailing points of BV, shifted by OFFSET
      while (j < bv.Length && bv[j].X > lastX) {
        c = bv[j++];
        c.Y += offset;
        cv[k++] = c;
      }
      // minimize size of result array
      Point[] r = AllocTempPointArray(k);
      Array.Copy(cv, 0, r, 0, k);
      FreeTempPointArray(cv);
      return r;
    }

    Point[] FringeUnionLeftX(Point[] av, Point[] bv, double offset) {
      if (av == null || av.Length < 2 || bv == null || bv.Length < 2) return null;  // not enough points!
      Point[] cv = AllocTempPointArray(av.Length + bv.Length);
      int i = 0;
      int j = 0;
      int k = 0;
      Point c;
      // copy any leading points of BV, shifted by OFFSET
      while (j < bv.Length && bv[j].Y < av[0].Y) {
        c = bv[j++];
        c.X += offset;
        cv[k++] = c;
      }
      // copy all of AV
      while (i < av.Length) {
        c = av[i++];
        cv[k++] = c;
      }
      // skip the part of BV that is covered by AV
      double lastY = av[av.Length-1].Y;
      while (j < bv.Length && bv[j].Y <= lastY) {
        j++;
      }
      // copy the trailing points of BV, shifted by OFFSET
      while (j < bv.Length && bv[j].Y > lastY) {
        c = bv[j++];
        c.X += offset;
        cv[k++] = c;
      }
      // minimize size of result array
      Point[] r = AllocTempPointArray(k);
      Array.Copy(cv, 0, r, 0, k);
      FreeTempPointArray(cv);
      return r;
    }

    Point[] FringeUnionRightY(Point[] av, Point[] bv, double offset) {
      if (av == null || av.Length < 2 || bv == null || bv.Length < 2) return null;  // not enough points!
      Point[] cv = AllocTempPointArray(av.Length + bv.Length);
      int i = 0;
      int j = 0;
      int k = 0;
      Point c;
      // copy any leading points of AV
      while (i < av.Length && av[i].X < bv[0].X) {
        c = av[i++];
        cv[k++] = c;
      }
      // copy all of BV, shifted by OFFSET
      while (j < bv.Length) {
        c = bv[j++];
        c.Y += offset;
        cv[k++] = c;
      }
      // skip the part of AV that is covered by BV
      double lastX = bv[bv.Length-1].X;
      while (i < av.Length && av[i].X <= lastX) {
        i++;
      }
      // copy the trailing points of AV
      while (i < av.Length && av[i].X > lastX) {
        c = av[i++];
        cv[k++] = c;
      }
      // minimize size of result array
      Point[] r = AllocTempPointArray(k);
      Array.Copy(cv, 0, r, 0, k);
      FreeTempPointArray(cv);
      return r;
    }

    Point[] FringeUnionRightX(Point[] av, Point[] bv, double offset) {
      if (av == null || av.Length < 2 || bv == null || bv.Length < 2) return null;  // not enough points!
      Point[] cv = AllocTempPointArray(av.Length + bv.Length);
      int i = 0;
      int j = 0;
      int k = 0;
      Point c;
      // copy any leading points of AV
      while (i < av.Length && av[i].Y < bv[0].Y) {
        c = av[i++];
        cv[k++] = c;
      }
      // copy all of BV, shifted by OFFSET
      while (j < bv.Length) {
        c = bv[j++];
        c.X += offset;
        cv[k++] = c;
      }
      // skip the part of AV that is covered by BV
      double lastY = bv[bv.Length-1].Y;
      while (i < av.Length && av[i].Y <= lastY) {
        i++;
      }
      // copy the trailing points of AV
      while (i < av.Length && av[i].Y > lastY) {
        c = av[i++];
        cv[k++] = c;
      }
      // minimize size of result array
      Point[] r = AllocTempPointArray(k);
      Array.Copy(cv, 0, r, 0, k);
      FreeTempPointArray(cv);
      return r;
    }

    static double FringeDistanceY(Point[] av, Point[] bv, double offset) {
      double min = 9999999;
      if (av == null || av.Length < 2 || bv == null || bv.Length < 2) return min;  // not enough points!
      int i = 0;
      int j = 0;
      while (i < av.Length && j < bv.Length) {
        Point ai = av[i];
        Point bj = bv[j];
        bj.Y += offset;
        Point an = ai;
        if (i+1 < av.Length) {
          an = av[i+1];
        }
        Point bn = bj;
        if (j+1 < bv.Length) {
          bn = bv[j+1];
          bn.Y += offset;
        }
        double diff = min;
        if (ai.X == bj.X) {  // same X's, so just compare Y's
          diff = bj.Y-ai.Y;
        } else if (ai.X > bj.X && ai.X < bn.X) {  // ai is between bj and bn
          diff = (bj.Y + ((ai.X-bj.X)/(bn.X-bj.X))*(bn.Y-bj.Y)) - ai.Y;
        } else if (bj.X > ai.X && bj.X < an.X) {  // bj is between ai and an
          diff = bj.Y - (ai.Y + ((bj.X-ai.X)/(an.X-ai.X))*(an.Y-ai.Y));
        }
        if (diff < min) {
          min = diff;
        }
        // pick next point
        if (an.X <= ai.X) {
          i++;
        } else if (bn.X <= bj.X) {
          j++;
        } else {
          // both indices increment if both next points have the same X
          if (an.X <= bn.X) i++;
          if (bn.X <= an.X) j++;
        }
      }
      return min;
    }

    static double FringeDistanceX(Point[] av, Point[] bv, double offset) {
      double min = 9999999;
      if (av == null || av.Length < 2 || bv == null || bv.Length < 2) return min;  // not enough points!
      int i = 0;
      int j = 0;
      while (i < av.Length && j < bv.Length) {
        Point ai = av[i];
        Point bj = bv[j];
        bj.X += offset;
        Point an = ai;
        if (i+1 < av.Length) {
          an = av[i+1];
        }
        Point bn = bj;
        if (j+1 < bv.Length) {
          bn = bv[j+1];
          bn.X += offset;
        }
        double diff = min;
        if (ai.Y == bj.Y) {  // same Y's, so just compare X's
          diff = bj.X-ai.X;
        } else if (ai.Y > bj.Y && ai.Y < bn.Y) {  // ai is between bj and bn
          diff = (bj.X + ((ai.Y-bj.Y)/(bn.Y-bj.Y))*(bn.X-bj.X)) - ai.X;
        } else if (bj.Y > ai.Y && bj.Y < an.Y) {  // bj is between ai and an
          diff = bj.X - (ai.X + ((bj.Y-ai.Y)/(an.Y-ai.Y))*(an.X-ai.X));
        }
        if (diff < min) {
          min = diff;
        }
        // pick next point
        if (an.Y <= ai.Y) {
          i++;
        } else if (bn.Y <= bj.Y) {
          j++;
        } else {
          // both indices increment if both next points have the same Y
          if (an.Y <= bn.Y) i++;
          if (bn.Y <= an.Y) j++;
        }
      }
      return min;
    }

    internal Point[] AllocTempPointArray(int len) {
      if (myTempArrays == null) myTempArrays = new Point[10][][];

      for (int i = 0; i < myTempArrays.Length; i++) {
        Point[][] cache = myTempArrays[i];
        if (cache == null || len >= cache.Length) {
          cache = new Point[Math.Max(len+1, 40)][];
          myTempArrays[i] = cache;
        }
        Point[] val = cache[len];
        if (val != null) {
          cache[len] = null;
          return val;
        }
      }

      return new Point[len];
    }

    internal void FreeTempPointArray(Point[] a) {
      if (myTempArrays == null) return;

      int len = a.Length;
      for (int i = 0; i < myTempArrays.Length; i++) {
        Point[][] cache = myTempArrays[i];
        if (cache != null && len < cache.Length && cache[len] == null) {
          cache[len] = a;
          return;
        }
      }

    }


    /// <summary>
    /// Identifies the <see cref="Arrangement"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ArrangementProperty;
    /// <summary>
    /// Gets or sets how <see cref="ArrangeTrees"/> should lay out the separate trees.
    /// </summary>
    /// <value>
    /// The default value is <see cref="TreeArrangement.Vertical"/>.
    /// </value>
    [DefaultValue(TreeArrangement.Vertical)]
    public TreeArrangement Arrangement {
      get { return (TreeArrangement)GetValue(ArrangementProperty); }
      set { SetValue(ArrangementProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="ArrangementSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ArrangementSpacingProperty;
    /// <summary>
    /// Gets or sets the space between which <see cref="ArrangeTrees"/> will position the trees.
    /// </summary>
    /// <value>
    /// This defaults to the Size(10, 10).
    /// </value>
    /// <remarks>
    /// This property is ignored if <see cref="Arrangement"/> is <see cref="TreeArrangement.FixedRoots"/>.
    /// </remarks>
    public Size ArrangementSpacing {
      get { return (Size)GetValue(ArrangementSpacingProperty); }
      set { SetValue(ArrangementSpacingProperty, value); }
    }

    /// <summary>
    /// Position each separate tree.
    /// </summary>
    /// <remarks>
    /// This is called after each tree has been laid out and thus each subtree bounds are known.
    /// </remarks>
    protected virtual void ArrangeTrees() {
      if (this.Arrangement == TreeArrangement.FixedRoots) {
        foreach (Node r in this.Roots) {
          TreeVertex root = this.Network.FindVertex(r);
          Rect b = r.Bounds;
          if (Double.IsNaN(b.X) || Double.IsInfinity(b.X)) b.X = 0;
          if (Double.IsNaN(b.Y) || Double.IsInfinity(b.Y)) b.Y = 0;
          AssignAbsolutePositions(root, new Point(b.X, b.Y));
        }
      } else {
        Point p = this.ArrangementOrigin;
        foreach (Node r in this.Roots) {
          TreeVertex root = this.Network.FindVertex(r);
          if (root == null) continue;
          //double angle = OrthoAngle(root);
          Point q = p;
          q.X += root.SubtreeOffset.X;
          q.Y += root.SubtreeOffset.Y;
          AssignAbsolutePositions(root, q);
          switch (this.Arrangement) {
            case TreeArrangement.Vertical:
              p.Y += root.SubtreeSize.Height + this.ArrangementSpacing.Height;
              break;
            case TreeArrangement.Horizontal:
              p.X += root.SubtreeSize.Width + this.ArrangementSpacing.Width;
              break;
            default:
              throw new InvalidOperationException("Unhandled Arrangement value " + this.Arrangement.ToString());
          }
        }
      }
    }

    internal void AssignAbsolutePositions(TreeVertex v, Point p) {
      if (v == null) return;
      v.Position = p;
      foreach (TreeVertex c in v.Children) {
        AssignAbsolutePositions(c, new Point(p.X + c.RelativePosition.X, p.Y + c.RelativePosition.Y));
      }
    }

    /// <summary>
    /// Call <see cref="GenericNetwork{N, L, Y}.Vertex.CommitPosition"/> to position each node,
    /// call <see cref="LayoutComments"/>, and then call
    /// <see cref="TreeEdge.CommitPosition"/> to route the links.
    /// </summary>
    /// <remarks>
    /// This sets any port spots, as directed by <see cref="SetPortSpots"/>,
    /// and then calls <see cref="LayoutNodes"/> and <see cref="LayoutLinks"/>.
    /// </remarks>
    protected virtual void LayoutNodesAndLinks() {
      SetPortSpotsAll();
      LayoutNodes();
      LayoutLinks();
    }

    /// <summary>
    /// Commit the position of all of the vertex nodes.
    /// </summary>
    protected virtual void LayoutNodes() {
      foreach (TreeVertex v in this.Network.Vertexes) {
        v.CommitPosition();
      }
      foreach (TreeVertex v in this.Network.Vertexes) {
        LayoutComments(v);
      }
    }

    /// <summary>
    /// Commit the position and routing of all of the edge links.
    /// </summary>
    protected virtual void LayoutLinks() {
      foreach (TreeEdge edge in this.Network.Edges) {
        edge.CommitPosition();
      }
    }

    private void SetPortSpotsAll() {
      foreach (Node r in this.Roots) {
        TreeVertex v = this.Network.FindVertex(r);
        SetPortSpotsTree(v);
      }
    }

    private void SetPortSpotsTree(TreeVertex v) {
      if (v == null) return;
      SetPortSpots(v);
      foreach (TreeVertex c in v.Children) {
        SetPortSpotsTree(c);
      }
    }

    /// <summary>
    /// Assign port spots for single-port nodes,
    /// if <see cref="TreeVertex.SetsPortSpot"/> or <see cref="TreeVertex.SetsChildPortSpot"/>
    /// is true, according to the value of <see cref="PortSpot"/> or <see cref="ChildPortSpot"/>.
    /// </summary>
    /// <param name="v"></param>
    /// <remarks>
    /// This iterates over all of the <see cref="TreeEdge"/>s in the network,
    /// finds the corresponding <see cref="FrameworkElement"/>, and assigns their FromSpot and/or
    /// ToSpot property if the <see cref="TreeVertex.SetsPortSpot"/> and/or
    /// <see cref="TreeVertex.SetsChildPortSpot"/> properties are true,
    /// and if the port's node only has a single port.
    /// The spot values are given by <see cref="PortSpot"/>
    /// and <see cref="ChildPortSpot"/> respectively, unless the value is <see cref="Spot.Default"/>.
    /// In the latter case the actual spot is determined by the <see cref="TreeVertex.Angle"/>
    /// of the parent node (the <see cref="GenericNetwork{N, L, Y}.Edge.FromVertex"/>).
    /// For example, when the angle is zero, the tree grows deeper toward the right.
    /// So the FromSpot will be set to <c>Spot.MiddleRight</c>, and the
    /// ToSpot will be set to <c>Spot.MiddleLeft</c>.
    /// </remarks>
    protected virtual void SetPortSpots(TreeVertex v) {
      double angle = OrthoAngle(v);
      if (this.Path == TreePath.Destination) {
        foreach (TreeEdge edge in v.DestinationEdges) {
          Link link = edge.Link;
          if (link != null) {
            if (v.SetsPortSpot) {
              if (v.PortSpot.IsDefault) {
                if (angle == 0) {
                  link.Route.FromSpot = Spot.MiddleRight;
                } else if (angle == 90) {
                  link.Route.FromSpot = Spot.MiddleBottom;
                } else if (angle == 180) {
                  link.Route.FromSpot = Spot.MiddleLeft;
                } else {
                  link.Route.FromSpot = Spot.MiddleTop;
                }
              } else {
                link.Route.FromSpot = v.PortSpot;
              }
            }
            if (v.SetsChildPortSpot) {
              if (v.ChildPortSpot.IsDefault) {
                if (angle == 0) {
                  link.Route.ToSpot = Spot.MiddleLeft;
                } else if (angle == 90) {
                  link.Route.ToSpot = Spot.MiddleTop;
                } else if (angle == 180) {
                  link.Route.ToSpot = Spot.MiddleRight;
                } else {
                  link.Route.ToSpot = Spot.MiddleBottom;
                }
              } else {
                link.Route.ToSpot = v.ChildPortSpot;
              }
            }
          }
        }
      } else {  // tree path == source
        foreach (TreeEdge edge in v.SourceEdges) {
          Link link = edge.Link;
          if (link != null) {
            if (v.SetsPortSpot) {
              if (v.PortSpot.IsDefault) {
                if (angle == 0) {
                  link.Route.ToSpot = Spot.MiddleRight;
                } else if (angle == 90) {
                  link.Route.ToSpot = Spot.MiddleBottom;
                } else if (angle == 180) {
                  link.Route.ToSpot = Spot.MiddleLeft;
                } else {
                  link.Route.ToSpot = Spot.MiddleTop;
                }
              } else {
                link.Route.ToSpot = v.PortSpot;
              }
            }
            if (v.SetsChildPortSpot) {
              if (v.ChildPortSpot.IsDefault) {
                if (angle == 0) {
                  link.Route.FromSpot = Spot.MiddleLeft;
                } else if (angle == 90) {
                  link.Route.FromSpot = Spot.MiddleTop;
                } else if (angle == 180) {
                  link.Route.FromSpot = Spot.MiddleRight;
                } else {
                  link.Route.FromSpot = Spot.MiddleBottom;
                }
              } else {
                link.Route.FromSpot = v.ChildPortSpot;
              }
            }
          }
        }
      }
    }

    internal static double OrthoAngle(TreeVertex v) {
      if (v.Angle <= 45)
        return 0;
      else if (v.Angle <= 135)
        return 90;
      else if (v.Angle <= 225)
        return 180;
      else if (v.Angle <= 315)
        return 270;
      else
        return 0;
    }

    internal static double ComputeLayerSpacing(TreeVertex v) {
      double angle = OrthoAngle(v);
      bool vertical = (angle == 90 || angle == 270);
      double layerspacing = v.LayerSpacing;  // limit layerspacing to be no more negative than the depth of the node
      if (v.LayerSpacingParentOverlap > 0) {
        double overlap = Math.Min(1, v.LayerSpacingParentOverlap);
        layerspacing -= (vertical ? v.Height*overlap : v.Width*overlap);
      }
      if (layerspacing < (vertical ? -v.Height : -v.Width))
        layerspacing = (vertical ? -v.Height : -v.Width);
      return layerspacing;
    }

    internal static double ComputeNodeIndent(TreeVertex v) {
      double angle = OrthoAngle(v);
      bool vertical = (angle == 90 || angle == 270);
      double nodeindent = v.NodeIndent;
      if (v.NodeIndentPastParent > 0) {
        double ind = Math.Min(1, v.NodeIndentPastParent);
        nodeindent += (vertical ? v.Width*ind : v.Height*ind);
      }
      nodeindent = Math.Max(0, nodeindent);
      return nodeindent;
    }

/*
    internal double RelativeBreadth(TreeVertex vertex, double angle) {
      if (vertex == null) return 0;
      if (angle == 0 || angle == 180) return vertex.Height;
      if (angle == 90 || angle == 270) return vertex.Width;
      double a = angle * Math.PI / 180;
      return (double)(Math.Abs(Math.Cos(a) * vertex.Width) + Math.Abs(Math.Sin(a) * vertex.Height));
    }

    internal double RelativeDepth(TreeVertex vertex, double angle) {
      if (vertex == null) return 0;
      if (angle == 0 || angle == 180) return vertex.Width;
      if (angle == 90 || angle == 270) return vertex.Height;
      double a = angle * Math.PI / 180;
      return (double)(Math.Abs(Math.Cos(a) * vertex.Height) + Math.Abs(Math.Sin(a) * vertex.Width));
    }

    internal Point ComputePoint(Point p, double dist, double angle) {
      double a = angle * Math.PI / 180;
      return new Point(p.X + (double)(dist*Math.Cos(a)), p.Y + (double)(dist*Math.Sin(a)));
    }
*/

    internal static readonly IComparer<TreeVertex> AlphabeticNodeTextComparer = new AlphaComparer();

    // private variables to store information about tree configuration

    private Point[][][] myTempArrays;


    /// <summary>
    /// Gets or sets the object holding the default values for root <see cref="TreeVertex"/>s.
    /// </summary>
    /// <remarks>
    /// The values for the following inheritable properties are actually stored in this object:
    /// <see cref="Sorting"/>, <see cref="Comparer"/>, <see cref="Angle"/>,
    /// <see cref="Alignment"/>, <see cref="NodeIndent"/>, <see cref="NodeIndentPastParent"/>,
    /// <see cref="NodeSpacing"/>, <see cref="LayerSpacing"/>, <see cref="LayerSpacingParentOverlap"/>,
    /// <see cref="Compaction"/>, <see cref="BreadthLimit"/>, <see cref="RowSpacing"/>, <see cref="RowIndent"/>,
    /// <see cref="CommentSpacing"/>, <see cref="CommentMargin"/>,
    /// <see cref="SetsPortSpot"/>, <see cref="PortSpot"/>, <see cref="SetsChildPortSpot"/>, <see cref="ChildPortSpot"/>.
    /// The other properties of this <see cref="TreeVertex"/> are ignored.
    /// </remarks>
    public TreeVertex RootDefaults { get; set; }

    /// <summary>
    /// Gets or sets the object holding the default values for alternate layer <see cref="TreeVertex"/>s,
    /// used when the <see cref="Style"/> is <see cref="Northwoods.GoXam.Layout.TreeStyle.Alternating"/>
    /// or <see cref="Northwoods.GoXam.Layout.TreeStyle.LastParents"/>.
    /// </summary>
    /// <remarks>
    /// See the list of inheritable properties in the remarks for <see cref="RootDefaults"/>.
    /// The other properties of this <see cref="TreeVertex"/> are ignored.
    /// </remarks>
    public TreeVertex AlternateDefaults { get; set; }


    // for convenience, access RootDefaults properties as if directly on this Tree


    /// <summary>
    /// Identifies the <see cref="Sorting"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SortingProperty;
    /// <summary>
    /// Gets or sets the default <see cref="TreeSorting"/> policy.
    /// </summary>
    /// <value>
    /// The default is <see cref="TreeSorting.Forwards"/>.
    /// </value>
    /// <remarks>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </remarks>
    [DefaultValue(TreeSorting.Forwards)]
    public TreeSorting Sorting {
      get { return (TreeSorting)GetValue(SortingProperty); }
      set { SetValue(SortingProperty, value); }
    }
    private static void OnSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.Sorting = (TreeSorting)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="Comparer"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ComparerProperty;
    /// <summary>
    /// Gets or sets the default <c>IComparer</c> used for sorting.
    /// </summary>
    /// <value>
    /// The default comparer compares the <see cref="GenericNetwork{N, L, Y}.Vertex.Node"/> Text values.
    /// </value>
    /// <remarks>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </remarks>
    public IComparer<TreeVertex> Comparer {
      get { return (IComparer<TreeVertex>)GetValue(ComparerProperty); }
      set { SetValue(ComparerProperty, value); }
    }
    private static void OnComparerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.Comparer = (IComparer<TreeVertex>)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="Angle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AngleProperty;
    /// <summary>
    /// Gets or sets the default direction for tree growth.
    /// </summary>
    /// <value>
    /// The default value is 0; the value must be one of: 0, 90, 180, 270.
    /// These values are in degrees, where 0 is along the positive X axis,
    /// and where 90 is along the positive Y axis.
    /// </value>
    /// <remarks>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </remarks>
    [DefaultValue(0.0)]
    public double Angle {
      get { return (double)GetValue(AngleProperty); }
      set { SetValue(AngleProperty, value); }
    }
    private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.Angle = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="Alignment"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlignmentProperty;
    /// <summary>
    /// Gets or sets the default alignment of parents relative to their children.
    /// </summary>
    /// <value>
    /// The default value is <see cref="TreeAlignment.CenterChildren"/>.
    /// </value>
    /// <remarks>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </remarks>
    [DefaultValue(TreeAlignment.CenterChildren)]
    public TreeAlignment Alignment {
      get { return (TreeAlignment)GetValue(AlignmentProperty); }
      set { SetValue(AlignmentProperty, value); }
    }
    private static void OnAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.Alignment = (TreeAlignment)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="NodeIndent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NodeIndentProperty;
    /// <summary>
    /// Gets or sets the default indentation of the first child.
    /// </summary>
    /// <value>
    /// The default value is zero.  The value should be non-negative.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property is only sensible when the <see cref="Alignment"/>
    /// is <see cref="TreeAlignment.Start"/> or <see cref="TreeAlignment.End"/>.
    /// Having a positive value is useful if you want to reserve space
    /// at the start of the row of children for some reason.
    /// For example, if you want to pretend the parent node is infinitely deep,
    /// you can set this to be the breadth of the parent node.
    /// </para>
    /// <para>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    /// <seealso cref="NodeIndentPastParent"/>
    /// <seealso cref="RowIndent"/>
    [DefaultValue(0.0)]
    public double NodeIndent {
      get { return (double)GetValue(NodeIndentProperty); }
      set { SetValue(NodeIndentProperty, value); }
    }
    private static void OnNodeIndentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.NodeIndent = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="NodeIndentPastParent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NodeIndentPastParentProperty;
    /// <summary>
    /// Gets or sets the fraction of this node's breadth is added to <see cref="NodeIndent"/>
    /// to determine any spacing at the start of the children.
    /// </summary>
    /// <value>
    /// The default value is 0.0 -- the only indentation is specified by <see cref="NodeIndent"/>.
    /// When the value is 1.0, the children will be indented past the breadth of the parent node.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property is only sensible when the <see cref="Alignment"/>
    /// is <see cref="TreeAlignment.Start"/> or <see cref="TreeAlignment.End"/>.
    /// </para>
    /// <para>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    /// <seealso cref="NodeIndent"/>
    [DefaultValue(0.0)]
    public double NodeIndentPastParent {
      get { return (double)GetValue(NodeIndentPastParentProperty); }
      set { SetValue(NodeIndentPastParentProperty, value); }
    }
    private static void OnNodeIndentPastParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.NodeIndentPastParent = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="NodeSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NodeSpacingProperty;
    /// <summary>
    /// Gets or sets the distance between child nodes.
    /// </summary>
    /// <value>
    /// The default value is 20.
    /// A negative value causes sibling nodes to overlap.
    /// </value>
    /// <remarks>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </remarks>
    [DefaultValue(20.0)]
    public double NodeSpacing {
      get { return (double)GetValue(NodeSpacingProperty); }
      set { SetValue(NodeSpacingProperty, value); }
    }
    private static void OnNodeSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.NodeSpacing = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="LayerSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LayerSpacingProperty;
    /// <summary>
    /// Gets or sets the distance between a parent node and its children.
    /// </summary>
    /// <value>
    /// The default value is 50.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is the distance between a parent node and its first row
    /// of children, in case there are multiple rows of its children.
    /// The <see cref="RowSpacing"/> property determines the distance
    /// between rows of children.
    /// Negative values may cause children to overlap with the parent.
    /// </para>
    /// <para>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    /// <seealso cref="LayerSpacingParentOverlap"/>
    [DefaultValue(50.0)]
    public double LayerSpacing {
      get { return (double)GetValue(LayerSpacingProperty); }
      set { SetValue(LayerSpacingProperty, value); }
    }
    private static void OnLayerSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.LayerSpacing = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="LayerSpacingParentOverlap"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LayerSpacingParentOverlapProperty;
    /// <summary>
    /// Gets or sets the fraction of the node's depth for which the children's layer starts overlapped with the parent's layer.
    /// </summary>
    /// <value>
    /// The default value is 0.0 -- there is overlap between layers only if <see cref="LayerSpacing"/> is negative.
    /// A value of 1.0 and a zero <see cref="LayerSpacing"/> will cause child nodes to completely overlap the parent.
    /// </value>
    /// <remarks>
    /// <para>
    /// A value greater than zero may still cause overlap between layers,
    /// unless the value of <see cref="LayerSpacing"/> is large enough.
    /// A value of zero might still allow overlap between layers,
    /// if <see cref="LayerSpacing"/> is negative.
    /// </para>
    /// <para>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    [DefaultValue(0.0)]
    public double LayerSpacingParentOverlap {
      get { return (double)GetValue(LayerSpacingParentOverlapProperty); }
      set { SetValue(LayerSpacingParentOverlapProperty, value); }
    }
    private static void OnLayerSpacingParentOverlapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.LayerSpacingParentOverlap = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="Compaction"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CompactionProperty;
    /// <summary>
    /// Gets or sets how closely to pack the child nodes of a subtree.
    /// </summary>
    /// <value>
    /// The default value is <see cref="TreeCompaction.Block"/>.
    /// </value>
    /// <remarks>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </remarks>
    [DefaultValue(TreeCompaction.Block)]
    public TreeCompaction Compaction {
      get { return (TreeCompaction)GetValue(CompactionProperty); }
      set { SetValue(CompactionProperty, value); }
    }
    private static void OnCompactionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.Compaction = (TreeCompaction)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="BreadthLimit"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BreadthLimitProperty;
    /// <summary>
    /// Gets or sets a limit on how broad a tree should be.
    /// </summary>
    /// <value>
    /// A value of zero (the default) means there is no limit;
    /// a positive value specifies a limit.
    /// The default value is zero.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property is just a suggested constraint on how
    /// broadly the tree will be laid out.
    /// When there isn't enough breadth for all of the children of a node,
    /// the children are placed in as many rows as needed to try to stay
    /// within the given breadth limit.
    /// If the value is too small, since this layout algorithm
    /// does not modify the size or shape of any node, the nodes will
    /// just be laid out in a line, one per row, and the breadth is
    /// determined by the broadest node.
    /// The distance between rows is specified by <see cref="RowSpacing"/>.
    /// To make room for the links that go around earlier rows to get to
    /// later rows, when the alignment is not a "center" alignment, the
    /// <see cref="RowIndent"/> property specifies that space at the
    /// start of each row.
    /// </para>
    /// <para>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    [DefaultValue(0.0)]
    public double BreadthLimit {
      get { return (double)GetValue(BreadthLimitProperty); }
      set { SetValue(BreadthLimitProperty, value); }
    }
    private static void OnBreadthLimitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.BreadthLimit = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="RowSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RowSpacingProperty;
    /// <summary>
    /// Gets or sets the distance between rows of children.
    /// </summary>
    /// <value>
    /// The default value is 25.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property is only used when there is more than one
    /// row of children for a given parent node.
    /// <see cref="LayerSpacing"/> determines the distance between
    /// the parent node and its first row of child nodes.
    /// </para>
    /// <para>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    /// <seealso cref="BreadthLimit"/>
    /// <seealso cref="RowIndent"/>
    [DefaultValue(25.0)]
    public double RowSpacing {
      get { return (double)GetValue(RowSpacingProperty); }
      set { SetValue(RowSpacingProperty, value); }
    }
    private static void OnRowSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.RowSpacing = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="RowIndent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RowIndentProperty;
    /// <summary>
    /// Gets or sets the default indentation of the first child of each row,
    /// if the <see cref="Alignment"/> is not a "Center" alignment.
    /// </summary>
    /// <value>
    /// The default value is 10.  The value should be non-negative.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used to leave room for the links that connect a parent node
    /// with the child nodes that are in additional rows.
    /// </para>
    /// <para>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    /// <seealso cref="BreadthLimit"/>
    /// <seealso cref="RowIndent"/>
    [DefaultValue(10.0)]
    public double RowIndent {
      get { return (double)GetValue(RowIndentProperty); }
      set { SetValue(RowIndentProperty, value); }
    }
    private static void OnRowIndentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.RowIndent = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="CommentSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommentSpacingProperty;
    /// <summary>
    /// Gets or sets the distance between comments.
    /// </summary>
    /// <value>
    /// The default value is 10.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used by <see cref="AddComments"/> and <see cref="LayoutComments"/>.
    /// </para>
    /// <para>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    /// <seealso cref="CommentMargin"/>
    [DefaultValue(10.0)]
    public double CommentSpacing {
      get { return (double)GetValue(CommentSpacingProperty); }
      set { SetValue(CommentSpacingProperty, value); }
    }
    private static void OnCommentSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.CommentSpacing = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="CommentMargin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommentMarginProperty;
    /// <summary>
    /// Gets or sets the distance between a node and its comments.
    /// </summary>
    /// <value>
    /// The default value is 20.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used by <see cref="AddComments"/> and <see cref="LayoutComments"/>.
    /// </para>
    /// <para>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    /// <seealso cref="CommentSpacing"/>
    [DefaultValue(20.0)]
    public double CommentMargin {
      get { return (double)GetValue(CommentMarginProperty); }
      set { SetValue(CommentMarginProperty, value); }
    }
    private static void OnCommentMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.CommentMargin = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="SetsPortSpot"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SetsPortSpotProperty;
    /// <summary>
    /// Gets or sets whether <see cref="TreeLayout.SetPortSpots"/> should set the
    /// FromSpot for this parent node port.
    /// </summary>
    /// <value>
    /// The default value is true -- this may modify the spot of the port of this node, the parent,
    /// if the node has only a single port.
    /// </value>
    /// <remarks>
    /// <para>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// The spot used depends on the value of <see cref="PortSpot"/>.
    /// </para>
    /// <para>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    [DefaultValue(true)]
    public bool SetsPortSpot {
      get { return (bool)GetValue(SetsPortSpotProperty); }
      set { SetValue(SetsPortSpotProperty, value); }
    }
    private static void OnSetsPortSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.SetsPortSpot = (bool)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="PortSpot"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PortSpotProperty;
    /// <summary>
    /// Gets or sets the spot that this node's port gets as its FromSpot.
    /// </summary>
    /// <value>The default value is <see cref="Spot.Default"/>.</value>
    /// <remarks>
    /// <para>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// A value of <c>Spot.Default</c> will cause <see cref="TreeLayout.SetPortSpots"/>
    /// to assign a FromSpot based on the parent node's
    /// <see cref="TreeVertex.Angle"/>.
    /// If the value is other than <c>NoSpot</c>, it is just assigned.
    /// When <see cref="TreeLayout.Path"/> is <see cref="TreePath.Source"/>,
    /// the port's ToSpot is set instead of the FromSpot.
    /// </para>
    /// <para>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    public Spot PortSpot {
      get { return (Spot)GetValue(PortSpotProperty); }
      set { SetValue(PortSpotProperty, value); }
    }
    private static void OnPortSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.PortSpot = (Spot)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="SetsChildPortSpot"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SetsChildPortSpotProperty;
    /// <summary>
    /// Gets or sets whether <see cref="TreeLayout.SetPortSpots"/> should set the
    /// ToSpot for each child node port.
    /// </summary>
    /// <value>
    /// The default value is true -- this may modify the spots of the ports of the children nodes,
    /// if the node has only a single port.
    /// </value>
    /// <remarks>
    /// <para>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// The spot used depends on the value of <see cref="ChildPortSpot"/>.
    /// </para>
    /// <para>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    [DefaultValue(true)]
    public bool SetsChildPortSpot {
      get { return (bool)GetValue(SetsChildPortSpotProperty); }
      set { SetValue(SetsChildPortSpotProperty, value); }
    }
    private static void OnSetsChildPortSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.SetsChildPortSpot = (bool)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="ChildPortSpot"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ChildPortSpotProperty;
    /// <summary>
    /// Gets or sets the spot that children nodes' ports get as their ToSpot.
    /// </summary>
    /// <value>The default value is <see cref="Spot.Default"/>.</value>
    /// <remarks>
    /// <para>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// A value of <c>Spot.Default</c> will cause <see cref="TreeLayout.SetPortSpots"/>
    /// to assign a ToSpot based on the parent node's
    /// <see cref="TreeVertex.Angle"/>.
    /// If the value is other than <c>NoSpot</c>, it is just assigned.
    /// When <see cref="TreeLayout.Path"/> is <see cref="TreePath.Source"/>,
    /// the port's FromSpot is set instead of the ToSpot.
    /// </para>
    /// <para>
    /// This sets the <see cref="RootDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    public Spot ChildPortSpot {
      get { return (Spot)GetValue(ChildPortSpotProperty); }
      set { SetValue(ChildPortSpotProperty, value); }
    }
    private static void OnChildPortSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.RootDefaults.ChildPortSpot = (Spot)e.NewValue;
      layout.InvalidateLayout();
    }


    // for convenience, access AlternateDefaults properties as if directly on this Tree

    /// <summary>
    /// Identifies the <see cref="AlternateSorting"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateSortingProperty;
    /// <summary>
    /// Gets or sets the default <see cref="TreeSorting"/> policy.
    /// </summary>
    /// <value>
    /// The default is <see cref="TreeSorting.Forwards"/>.
    /// </value>
    /// <remarks>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </remarks>
    [DefaultValue(TreeSorting.Forwards)]
    public TreeSorting AlternateSorting {
      get { return (TreeSorting)GetValue(AlternateSortingProperty); }
      set { SetValue(AlternateSortingProperty, value); }
    }
    private static void OnAlternateSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.Sorting = (TreeSorting)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateComparer"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateComparerProperty;
    /// <summary>
    /// Gets or sets the default <c>IComparer</c> used for sorting.
    /// </summary>
    /// <value>
    /// The default comparer compares the <see cref="GenericNetwork{N, L, Y}.Vertex.Node"/> Text values.
    /// </value>
    /// <remarks>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </remarks>
    public IComparer<TreeVertex> AlternateComparer {
      get { return (IComparer<TreeVertex>)GetValue(AlternateComparerProperty); }
      set { SetValue(AlternateComparerProperty, value); }
    }
    private static void OnAlternateComparerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.Comparer = (IComparer<TreeVertex>)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateAngle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateAngleProperty;
    /// <summary>
    /// Gets or sets the default direction for tree growth.
    /// </summary>
    /// <value>
    /// The default value is 0; the value must be one of: 0, 90, 180, 270.
    /// These values are in degrees, where 0 is along the positive X axis,
    /// and where 90 is along the positive Y axis.
    /// </value>
    /// <remarks>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </remarks>
    [DefaultValue(0.0)]
    public double AlternateAngle {
      get { return (double)GetValue(AlternateAngleProperty); }
      set { SetValue(AlternateAngleProperty, value); }
    }
    private static void OnAlternateAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.Angle = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateAlignment"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateAlignmentProperty;
    /// <summary>
    /// Gets or sets the default alignment of parents relative to their children.
    /// </summary>
    /// <value>
    /// The default value is <see cref="TreeAlignment.CenterChildren"/>.
    /// </value>
    /// <remarks>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </remarks>
    [DefaultValue(TreeAlignment.CenterChildren)]
    public TreeAlignment AlternateAlignment {
      get { return (TreeAlignment)GetValue(AlternateAlignmentProperty); }
      set { SetValue(AlternateAlignmentProperty, value); }
    }
    private static void OnAlternateAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.Alignment = (TreeAlignment)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateNodeIndent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateNodeIndentProperty;
    /// <summary>
    /// Gets or sets the default indentation of the first child.
    /// </summary>
    /// <value>
    /// The default value is zero.  The value should be non-negative.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property is only sensible when the <see cref="Alignment"/>
    /// is <see cref="TreeAlignment.Start"/> or <see cref="TreeAlignment.End"/>.
    /// Having a positive value is useful if you want to reserve space
    /// at the start of the row of children for some reason.
    /// For example, if you want to pretend the parent node is infinitely deep,
    /// you can set this to be the breadth of the parent node.
    /// </para>
    /// <para>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    /// <seealso cref="AlternateNodeIndentPastParent"/>
    /// <seealso cref="AlternateRowIndent"/>
    [DefaultValue(0.0)]
    public double AlternateNodeIndent {
      get { return (double)GetValue(AlternateNodeIndentProperty); }
      set { SetValue(AlternateNodeIndentProperty, value); }
    }
    private static void OnAlternateNodeIndentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.NodeIndent = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateNodeIndentPastParent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateNodeIndentPastParentProperty;
    /// <summary>
    /// Gets or sets the fraction of this node's breadth is added to <see cref="NodeIndent"/>
    /// to determine any spacing at the start of the children.
    /// </summary>
    /// <value>
    /// The default value is 0.0 -- the only indentation is specified by <see cref="NodeIndent"/>.
    /// When the value is 1.0, the children will be indented past the breadth of the parent node.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property is only sensible when the <see cref="Alignment"/>
    /// is <see cref="TreeAlignment.Start"/> or <see cref="TreeAlignment.End"/>.
    /// </para>
    /// <para>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    /// <seealso cref="AlternateNodeIndent"/>
    [DefaultValue(0.0)]
    public double AlternateNodeIndentPastParent {
      get { return (double)GetValue(AlternateNodeIndentPastParentProperty); }
      set { SetValue(AlternateNodeIndentPastParentProperty, value); }
    }
    private static void OnAlternateNodeIndentPastParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.NodeIndentPastParent = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateNodeSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateNodeSpacingProperty;
    /// <summary>
    /// Gets or sets the distance between child nodes.
    /// </summary>
    /// <value>
    /// The default value is 20.
    /// A negative value causes sibling nodes to overlap.
    /// </value>
    /// <remarks>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </remarks>
    [DefaultValue(20.0)]
    public double AlternateNodeSpacing {
      get { return (double)GetValue(AlternateNodeSpacingProperty); }
      set { SetValue(AlternateNodeSpacingProperty, value); }
    }
    private static void OnAlternateNodeSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.NodeSpacing = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateLayerSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateLayerSpacingProperty;
    /// <summary>
    /// Gets or sets the distance between a parent node and its children.
    /// </summary>
    /// <value>
    /// The default value is 50.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is the distance between a parent node and its first row
    /// of children, in case there are multiple rows of its children.
    /// The <see cref="RowSpacing"/> property determines the distance
    /// between rows of children.
    /// Negative values may cause children to overlap with the parent.
    /// </para>
    /// <para>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    /// <seealso cref="AlternateLayerSpacingParentOverlap"/>
    [DefaultValue(50.0)]
    public double AlternateLayerSpacing {
      get { return (double)GetValue(AlternateLayerSpacingProperty); }
      set { SetValue(AlternateLayerSpacingProperty, value); }
    }
    private static void OnAlternateLayerSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.LayerSpacing = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateLayerSpacingParentOverlap"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateLayerSpacingParentOverlapProperty;
    /// <summary>
    /// Gets or sets the fraction of the node's depth for which the children's layer starts overlapped with the parent's layer.
    /// </summary>
    /// <value>
    /// The default value is 0.0 -- there is overlap between layers only if <see cref="LayerSpacing"/> is negative.
    /// A value of 1.0 and a zero <see cref="LayerSpacing"/> will cause child nodes to completely overlap the parent.
    /// </value>
    /// <remarks>
    /// <para>
    /// A value greater than zero may still cause overlap between layers,
    /// unless the value of <see cref="LayerSpacing"/> is large enough.
    /// A value of zero might still allow overlap between layers,
    /// if <see cref="LayerSpacing"/> is negative.
    /// </para>
    /// <para>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    [DefaultValue(0.0)]
    public double AlternateLayerSpacingParentOverlap {
      get { return (double)GetValue(AlternateLayerSpacingParentOverlapProperty); }
      set { SetValue(AlternateLayerSpacingParentOverlapProperty, value); }
    }
    private static void OnAlternateLayerSpacingParentOverlapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.LayerSpacingParentOverlap = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateCompaction"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateCompactionProperty;
    /// <summary>
    /// Gets or sets how closely to pack the child nodes of a subtree.
    /// </summary>
    /// <value>
    /// The default value is <see cref="TreeCompaction.Block"/>.
    /// </value>
    /// <remarks>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </remarks>
    [DefaultValue(TreeCompaction.Block)]
    public TreeCompaction AlternateCompaction {
      get { return (TreeCompaction)GetValue(AlternateCompactionProperty); }
      set { SetValue(AlternateCompactionProperty, value); }
    }
    private static void OnAlternateCompactionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.Compaction = (TreeCompaction)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateBreadthLimit"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateBreadthLimitProperty;
    /// <summary>
    /// Gets or sets a limit on how broad a tree should be.
    /// </summary>
    /// <value>
    /// A value of zero (the default) means there is no limit;
    /// a positive value specifies a limit.
    /// The default value is zero.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property is just a suggested constraint on how
    /// broadly the tree will be laid out.
    /// When there isn't enough breadth for all of the children of a node,
    /// the children are placed in as many rows as needed to try to stay
    /// within the given breadth limit.
    /// If the value is too small, since this layout algorithm
    /// does not modify the size or shape of any node, the nodes will
    /// just be laid out in a line, one per row, and the breadth is
    /// determined by the broadest node.
    /// The distance between rows is specified by <see cref="RowSpacing"/>.
    /// To make room for the links that go around earlier rows to get to
    /// later rows, when the alignment is not a "center" alignment, the
    /// <see cref="RowIndent"/> property specifies that space at the
    /// start of each row.
    /// </para>
    /// <para>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    [DefaultValue(0.0)]
    public double AlternateBreadthLimit {
      get { return (double)GetValue(AlternateBreadthLimitProperty); }
      set { SetValue(AlternateBreadthLimitProperty, value); }
    }
    private static void OnAlternateBreadthLimitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.BreadthLimit = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateRowSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateRowSpacingProperty;
    /// <summary>
    /// Gets or sets the distance between rows of children.
    /// </summary>
    /// <value>
    /// The default value is 25.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property is only used when there is more than one
    /// row of children for a given parent node.
    /// <see cref="LayerSpacing"/> determines the distance between
    /// the parent node and its first row of child nodes.
    /// </para>
    /// <para>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    /// <seealso cref="AlternateBreadthLimit"/>
    /// <seealso cref="AlternateRowIndent"/>
    [DefaultValue(25.0)]
    public double AlternateRowSpacing {
      get { return (double)GetValue(AlternateRowSpacingProperty); }
      set { SetValue(AlternateRowSpacingProperty, value); }
    }
    private static void OnAlternateRowSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.RowSpacing = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateRowIndent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateRowIndentProperty;
    /// <summary>
    /// Gets or sets the default indentation of the first child of each row,
    /// if the <see cref="Alignment"/> is not a "Center" alignment.
    /// </summary>
    /// <value>
    /// The default value is 10.  The value should be non-negative.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used to leave room for the links that connect a parent node
    /// with the child nodes that are in additional rows.
    /// </para>
    /// <para>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    /// <seealso cref="AlternateBreadthLimit"/>
    /// <seealso cref="AlternateRowIndent"/>
    [DefaultValue(10.0)]
    public double AlternateRowIndent {
      get { return (double)GetValue(AlternateRowIndentProperty); }
      set { SetValue(AlternateRowIndentProperty, value); }
    }
    private static void OnAlternateRowIndentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.RowIndent = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateCommentSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateCommentSpacingProperty;
    /// <summary>
    /// Gets or sets the distance between comments.
    /// </summary>
    /// <value>
    /// The default value is 10.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used by <see cref="AddComments"/> and <see cref="LayoutComments"/>.
    /// </para>
    /// <para>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    /// <seealso cref="AlternateCommentMargin"/>
    [DefaultValue(10.0)]
    public double AlternateCommentSpacing {
      get { return (double)GetValue(AlternateCommentSpacingProperty); }
      set { SetValue(AlternateCommentSpacingProperty, value); }
    }
    private static void OnAlternateCommentSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.CommentSpacing = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateCommentMargin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateCommentMarginProperty;
    /// <summary>
    /// Gets or sets the distance between a node and its comments.
    /// </summary>
    /// <value>
    /// The default value is 20.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used by <see cref="AddComments"/> and <see cref="LayoutComments"/>.
    /// </para>
    /// <para>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    /// <seealso cref="AlternateCommentSpacing"/>
    [DefaultValue(20.0)]
    public double AlternateCommentMargin {
      get { return (double)GetValue(AlternateCommentMarginProperty); }
      set { SetValue(AlternateCommentMarginProperty, value); }
    }
    private static void OnAlternateCommentMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.CommentMargin = (double)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateSetsPortSpot"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateSetsPortSpotProperty;
    /// <summary>
    /// Gets or sets whether <see cref="TreeLayout.SetPortSpots"/> should set the
    /// FromSpot for this parent node port.
    /// </summary>
    /// <value>
    /// The default value is true -- this may modify the spot of the port of this node, the parent,
    /// if the node has only a single port.
    /// </value>
    /// <remarks>
    /// <para>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// The spot used depends on the value of <see cref="PortSpot"/>.
    /// </para>
    /// <para>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    [DefaultValue(true)]
    public bool AlternateSetsPortSpot {
      get { return (bool)GetValue(AlternateSetsPortSpotProperty); }
      set { SetValue(AlternateSetsPortSpotProperty, value); }
    }
    private static void OnAlternateSetsPortSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.SetsPortSpot = (bool)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternatePortSpot"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternatePortSpotProperty;
    /// <summary>
    /// Gets or sets the spot that this node's port gets as its FromSpot.
    /// </summary>
    /// <value>The default value is <see cref="Spot.Default"/>.</value>
    /// <remarks>
    /// <para>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// A value of <c>Spot.Default</c> will cause <see cref="TreeLayout.SetPortSpots"/>
    /// to assign a FromSpot based on the parent node's
    /// <see cref="TreeVertex.Angle"/>.
    /// If the value is other than <c>NoSpot</c>, it is just assigned.
    /// When <see cref="TreeLayout.Path"/> is <see cref="TreePath.Source"/>,
    /// the port's ToSpot is set instead of the FromSpot.
    /// </para>
    /// <para>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    public Spot AlternatePortSpot {
      get { return (Spot)GetValue(AlternatePortSpotProperty); }
      set { SetValue(AlternatePortSpotProperty, value); }
    }
    private static void OnAlternatePortSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.PortSpot = (Spot)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateSetsChildPortSpot"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateSetsChildPortSpotProperty;
    /// <summary>
    /// Gets or sets whether <see cref="TreeLayout.SetPortSpots"/> should set the
    /// ToSpot for each child node port.
    /// </summary>
    /// <value>
    /// The default value is true -- this may modify the spots of the ports of the children nodes,
    /// if the node has only a single port.
    /// </value>
    /// <remarks>
    /// <para>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// The spot used depends on the value of <see cref="ChildPortSpot"/>.
    /// </para>
    /// <para>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    [DefaultValue(true)]
    public bool AlternateSetsChildPortSpot {
      get { return (bool)GetValue(AlternateSetsChildPortSpotProperty); }
      set { SetValue(AlternateSetsChildPortSpotProperty, value); }
    }
    private static void OnAlternateSetsChildPortSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.SetsChildPortSpot = (bool)e.NewValue;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AlternateChildPortSpot"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlternateChildPortSpotProperty;
    /// <summary>
    /// Gets or sets the spot that children nodes' ports get as their ToSpot.
    /// </summary>
    /// <value>The default value is <see cref="Spot.Default"/>.</value>
    /// <remarks>
    /// <para>
    /// This inherited property is initialized in the <see cref="TreeLayout.InitializeTreeVertexValues"/> method.
    /// A value of <c>Spot.Default</c> will cause <see cref="TreeLayout.SetPortSpots"/>
    /// to assign a ToSpot based on the parent node's
    /// <see cref="TreeVertex.Angle"/>.
    /// If the value is other than <c>NoSpot</c>, it is just assigned.
    /// When <see cref="TreeLayout.Path"/> is <see cref="TreePath.Source"/>,
    /// the port's FromSpot is set instead of the ToSpot.
    /// </para>
    /// <para>
    /// This sets the <see cref="AlternateDefaults"/>'s property of the same name.
    /// </para>
    /// </remarks>
    public Spot AlternateChildPortSpot {
      get { return (Spot)GetValue(AlternateChildPortSpotProperty); }
      set { SetValue(AlternateChildPortSpotProperty, value); }
    }
    private static void OnAlternateChildPortSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      TreeLayout layout = (TreeLayout)d;
      layout.AlternateDefaults.ChildPortSpot = (Spot)e.NewValue;
      layout.InvalidateLayout();
    }


    sealed internal class AlphaComparer : IComparer<TreeVertex> {  // nested class
      public int Compare(TreeVertex m, TreeVertex n) {
        if (m != null) {
          if (n != null) {
            Node a = m.Node;
            Node b = n.Node;
            if (a != null) {
              if (b != null)
                return String.Compare(a.Text, b.Text, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.CompareOptions.IgnoreCase);
              else
                return 1;
            } else {
              if (b != null)
                return -1;
              else
                return 0;
            }
          } else {
            return 1;
          }
        } else {
          if (n != null)
            return -1;
          else
            return 0;
        }
      }
    }
  }


  /// <summary>
  /// This enumeration specifies how to build a tree from the <see cref="TreeNetwork"/>.
  /// </summary>
  public enum TreePath {
    /// <summary>
    /// The children of a <see cref="TreeVertex"/> are its <see cref="Northwoods.GoXam.Layout.GenericNetwork{V,E,Y}.Vertex.DestinationVertexes"/>,
    /// the collection of connected <see cref="TreeEdge"/>.<see cref="Northwoods.GoXam.Layout.GenericNetwork{V,E,Y}.Edge.ToVertex"/>s.
    /// </summary>
    /// <remarks>
    /// The tree roots are those <see cref="TreeVertex"/>s that have a zero <see cref="Northwoods.GoXam.Layout.GenericNetwork{V,E,Y}.Vertex.SourceEdgesCount"/>.
    /// </remarks>
    Destination,
    /// <summary>
    /// The children of a <see cref="TreeVertex"/> are its <see cref="Northwoods.GoXam.Layout.GenericNetwork{V,E,Y}.Vertex.SourceVertexes"/>,
    /// the collection of connected <see cref="TreeEdge"/>.<see cref="Northwoods.GoXam.Layout.GenericNetwork{V,E,Y}.Edge.FromVertex"/>s.
    /// </summary>
    /// <remarks>
    /// The tree roots are those <see cref="TreeVertex"/>s that have a zero <see cref="Northwoods.GoXam.Layout.GenericNetwork{V,E,Y}.Vertex.DestinationEdgesCount"/>.
    /// </remarks>
    Source
  }
  //?? future: different ways of handling shared children and cycles

  /// <summary>
  /// This enumeration specifies whether to sort the children of a node,
  /// and in what order to position them.
  /// </summary>
  public enum TreeSorting {
    /// <summary>
    /// Lay out each child in the order in which they were found.
    /// </summary>
    Forwards,
    /// <summary>
    /// Lay out each child in reverse order from which they were found.
    /// </summary>
    Reverse,
    /// <summary>
    /// Lay out each child according to the sort order given by <see cref="TreeVertex.Comparer"/>.
    /// </summary>
    Ascending,
    /// <summary>
    /// Lay out each child in reverse sort order given by <see cref="TreeVertex.Comparer"/>.
    /// </summary>
    Descending
  }
  //?? future: None, to allow reordering to increase compaction
  //??? need "LeftToRight" for determining order in rows

  /// <summary>
  /// This enumeration specifies how to position a parent <see cref="TreeVertex"/>
  /// relative to its children.
  /// </summary>
  public enum TreeAlignment {
    /// <summary>
    /// The parent is centered at the middle of the range of its child subtrees.
    /// </summary>
    /// <remarks>
    /// When there is a breadth limit that causes there to be multiple rows,
    /// the links that extend from the parent to those children in rows past
    /// the first one may cross over the nodes that are in earlier rows.
    /// </remarks>
    CenterSubtrees,
    /// <summary>
    /// The parent is centered at the middle of the range of its immediate child nodes.
    /// </summary>
    /// <remarks>
    /// When there is a breadth limit that causes there to be multiple rows,
    /// the links that extend from the parent to those children in rows past
    /// the first one may cross over the nodes that are in earlier rows.
    /// </remarks>
    CenterChildren,
    /// <summary>
    /// The parent is positioned near the first of its children.
    /// </summary>
    Start,
    /// <summary>
    /// The parent is positioned near the last of its children.
    /// </summary>
    End
  }

  //?? future enum, plus add TreeVertex.NodeSpacingStyle (default Relative),
  //       .LayerSpacingStyle (default Relative), and .NodeIndentStyle (default Absolute)
  //public enum TreeSpacingStyle {
  //  Relative,
  //  Absolute
  //}

  /// <summary>
  /// This enumeration specifies how closely packed the children of a node should be.
  /// </summary>
  public enum TreeCompaction {
    /// <summary>
    /// Only simple placement of children next to each other, as determined by their subtree breadth.
    /// </summary>
    /// <remarks>
    /// For any node, there will not be another node at any depth occupying the same breadth position,
    /// unless there are multiple rows.  In other words, if there is no breadth limit resulting in
    /// multiple rows, with this compaction mode it is as if every node were infinitely deep.
    /// </remarks>
    None,
    /// <summary>
    /// A simple fitting of subtrees.
    /// </summary>
    /// <remarks>
    /// This mode produces more compact trees -- often nicer looking too.
    /// Nodes will not overlap each other, unless you have negative values
    /// for some of the spacing properties.
    /// However it is possible when the links are orthogonally styled that
    /// occasionally the subtrees will be placed so close together that some
    /// links may overlap the links or even the nodes of other subtrees.
    /// </remarks>
    Block
  }
  //?? future: better fitting in 2D

  /// <summary>
  /// This enumeration identifies the general style in which the nodes are laid out.
  /// </summary>
  public enum TreeStyle {
    /// <summary>
    /// The normal tree style, where all of the children of each <see cref="TreeVertex"/> are lined up
    /// horizontally or vertically.
    /// </summary>
    /// <remarks>
    /// Each <see cref="TreeVertex"/> gets its properties from its parent node.
    /// <see cref="TreeLayout.RootDefaults"/> is used for all default <see cref="TreeVertex"/> property values;
    /// <see cref="TreeLayout.AlternateDefaults"/> is ignored.
    /// </remarks>
    Layered,
    /// <summary>
    /// Just like the standard layered style, except that the nodes with children but no grandchildren
    /// have alternate properties.
    /// </summary>
    /// <remarks>
    /// Each <see cref="TreeVertex"/> gets its properties from its parent node.
    /// However, for those nodes whose <see cref="TreeVertex.MaxGenerationCount"/> is 1,
    /// in other words when it has children but no grandchildren,
    /// the properties are copied from the <see cref="TreeLayout.AlternateDefaults"/>.
    /// If the tree only has two levels, the root node gets the <see cref="TreeLayout.RootDefaults"/>.
    /// </remarks>
    LastParents,
    /// <summary>
    /// Alternate layers of the tree have different properties, typically including the angle.
    /// </summary>
    /// <remarks>
    /// Each <see cref="TreeVertex"/> gets its properties from its "grand-parent" node.
    /// The root nodes get their defaults from <see cref="TreeLayout.RootDefaults"/>;
    /// the immediate children of root nodes get their defaults from <see cref="TreeLayout.AlternateDefaults"/>.
    /// </remarks>
    Alternating,
    /// <summary>
    /// All of the nodes get the alternate properties, except the root node gets the default properties.
    /// </summary>
    /// <remarks>
    /// The root node gets the <see cref="TreeLayout.RootDefaults"/> properties,
    /// the root node's children get the <see cref="TreeLayout.AlternateDefaults"/> properties,
    /// and all of the rest of the <see cref="TreeVertex"/>s get their properties from their parent node.
    /// </remarks>
    RootOnly
  }
  //?? future: SingleBus, DoubleBus, Radial

  /// <summary>
  /// This enumeration specifies how to position the resulting trees in the document.
  /// </summary>
  public enum TreeArrangement {
    /// <summary>
    /// Position each tree in a non-overlapping fashion by increasing Y coordinates,
    /// starting at the <see cref="DiagramLayout.ArrangementOrigin"/>.
    /// </summary>
    Vertical,
    /// <summary>
    /// Position each tree in a non-overlapping fashion by increasing X coordinates,
    /// starting at the <see cref="DiagramLayout.ArrangementOrigin"/>.
    /// </summary>
    Horizontal,
    /// <summary>
    /// Do not move each root node, but position all of their descendents relative to their root.
    /// </summary>
    FixedRoots
  }
  //?? future: Square, AlignRootsVertically, AlignRootsHorizontally
}

