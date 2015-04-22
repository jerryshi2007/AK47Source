
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
using System.Windows.Input;
using System.Windows.Media.Animation;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Layout;

namespace Northwoods.GoXam {

  /// <summary>
  /// This class is responsible for automatically positioning all nodes in the diagram.
  /// </summary>
  /// <remarks>
  /// <para>
  /// You can explicitly re-layout the whole diagram by calling <see cref="LayoutDiagram()"/>.
  /// However, it is more common to let this <c>LayoutManager</c> decide when layouts should happen.
  /// You can control this by setting the <see cref="DiagramLayout.Conditions"/> property
  /// on each kind of layout.
  /// By default each layout has its <see cref="DiagramLayout.Conditions"/> set to invalidate
  /// its layout whenever a node or a link or a group membership is added or removed, but
  /// not when a node changes size.
  /// Once a <see cref="DiagramLayout"/> has been invalidated, this <c>LayoutManager</c>
  /// will eventually call its <see cref="IDiagramLayout.DoLayout"/> method.
  /// </para>
  /// <para>
  /// Each <see cref="Node"/> and <see cref="Link"/> has a single <see cref="IDiagramLayout"/> that
  /// may position the node and route the link.  The <see cref="IDiagramLayout"/> is found by
  /// proceeding up the chain of containing <see cref="Group"/>s until it finds one with a value
  /// for <see cref="Group.Layout"/>, or it ultimately uses the diagram's
  /// <see cref="Northwoods.GoXam.Diagram.Layout"/>.
  /// </para>
  /// <para>
  /// The <see cref="CanLayoutPart"/> predicate decides whether a particular part should be laid
  /// out by a given layout.  You can set the <see cref="Part.LayoutId"/> attached property to
  /// be the string "None" if you do not want it to be laid out.
  /// </para>
  /// <para>
  /// Each <see cref="Northwoods.GoXam.Diagram"/> has an instance of this class as its
  /// <see cref="Northwoods.GoXam.Diagram.LayoutManager"/> property.
  /// If you want to customize the standard behavior, you can easily override any of its methods
  /// and substitute an instance of your custom layout manager class for your diagram.
  /// <code>
  /// public class CustomLayoutManager : LayoutManager {
  ///   protected override bool CanLayoutPart(Part p, IDiagramLayout lay) {
  ///     return ...;  // decide dynamically which Nodes and Links should be considered for each layout
  ///   }
  /// }
  /// </code>
  /// and install it with either XAML:
  /// <code>
  ///   &lt;go:Diagram ...&gt;
  ///     &lt;go:Diagram.LayoutManager&gt;
  ///       &lt;local:CustomLayoutManager /&gt;
  ///     &lt;/go:Diagram.LayoutManager&gt;
  ///   &lt;/go:Diagram&gt;
  /// </code>
  /// or in the initialization of your Diagram control:
  /// <code>
  ///   myDiagram.LayoutManager = new CustomLayoutManager();
  /// </code>
  /// </para>
  /// <para>
  /// Automatic layout in GoXam includes support for animated movement of
  /// nodes to their intended destinations.
  /// Such support is not limited to the execution of <see cref="PerformLayout"/> --
  /// you can call <see cref="MoveAnimated"/> at any time.
  /// For example, the <see cref="Node"/> method <see cref="Node.Move"/> calls <see cref="MoveAnimated"/>.
  /// </para>
  /// <para>
  /// Setting the <see cref="Animated"/> property to false disables all animation.
  /// Set the <see cref="AnimationTime"/> property to control how quickly
  /// the animation completes.
  /// For nodes that do not have an initial location (because the value is <c>NaN,NaN</c>),
  /// you can specify the point from where they appear to come
  /// by setting the <see cref="DefaultLocation"/>.
  /// </para>
  /// <para>
  /// Although this class inherits from <c>FrameworkElement</c>
  /// in order to support data binding,
  /// it is not really a <c>FrameworkElement</c> or <c>UIElement</c>!
  /// Please ignore all of the properties, methods, and events defined by
  /// <c>FrameworkElement</c> and <c>UIElement</c>.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class LayoutManager : FrameworkElement {
    /// <summary>
    /// The constructor for the standard layout manager that is the initial value of
    /// <see cref="Northwoods.GoXam.Diagram.LayoutManager"/>.
    /// </summary>
    public LayoutManager() {
      this.AnimationDuration = new Duration(new TimeSpan(0, 0, 0, 0, this.AnimationTime));
    }

    static LayoutManager() {
      InitialProperty = DependencyProperty.Register("Initial", typeof(LayoutInitial), typeof(LayoutManager),
        new FrameworkPropertyMetadata(LayoutInitial.InvalidateIfNodesUnlocated));
      AnimatedProperty = DependencyProperty.Register("Animated", typeof(bool), typeof(LayoutManager),
        new FrameworkPropertyMetadata(true));
      AnimationTimeProperty = DependencyProperty.Register("AnimationTime", typeof(int), typeof(LayoutManager),
        new FrameworkPropertyMetadata(250, OnAnimationTimeChanged));
      DefaultLocationProperty = DependencyProperty.Register("DefaultLocation", typeof(Point), typeof(LayoutManager),
        new FrameworkPropertyMetadata(new Point(0, 0)));
    }


    /// <summary>
    /// Throw an exception if the current thread does not have access to this <c>DependencyObject</c>.
    /// </summary>
    protected void VerifyAccess() {
      if (!CheckAccess()) Diagram.Error("No access to thread");
    }


    /// <summary>
    /// Gets the <see cref="Northwoods.GoXam.Diagram"/> for which this <see cref="LayoutManager"/> performs automatic layouts.
    /// </summary>
    /// <value>
    /// This value is automatically set by the <see cref="Northwoods.GoXam.Diagram.LayoutManager"/> setter.
    /// </value>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Diagram Diagram { get; internal set; }


    /// <summary>
    /// Request a re-layout of all of the nodes and links in this diagram.
    /// </summary>
    /// <remarks>
    /// This first marks each <see cref="IDiagramLayout"/> in the diagram as invalid
    /// and then schedules a layout to occur soon.
    /// </remarks>
    public void LayoutDiagram() {
      LayoutDiagram(LayoutInitial.InvalidateAll, false);
    }

    /// <summary>
    /// Request a re-layout of all invalidated layouts in this diagram.
    /// </summary>
    /// <param name="init">
    /// If the value is <see cref="LayoutInitial.InvalidateAll"/>,
    /// all <see cref="IDiagramLayout"/>s will be declared invalid, and they will all be performed
    /// at the time given by the <paramref name="immediate"/> argument.
    /// If the value is <see cref="LayoutInitial.InvalidateIfNodesUnlocated"/>,
    /// <see cref="IDiagramLayout"/>s will be declared invalid only if they are responsible
    /// for <see cref="Node"/>s that do not have a <see cref="Node.Location"/>,
    /// because their X and/or Y values are <c>Double.NaN</c>.
    /// If the value is <see cref="LayoutInitial.ValidateAll"/>,
    /// all <see cref="IDiagramLayout"/>s will be declared valid, and no layout will occur
    /// until after something else happens to invalidate a layout.
    /// If the value is <see cref="LayoutInitial.None"/>,
    /// no layout validity is changed -- any invalid layouts will be performed at the specified time.
    /// </param>
    /// <param name="immediate">
    /// If true this method performs an immediate layout.
    /// If false this method schedules a layout to occur soon.
    /// </param>
    /// <remarks>
    /// The value for the <paramref name="init"/> parameter usually comes from the
    /// <see cref="Initial"/> property.
    /// This method changes the validity of layouts by setting the <see cref="IDiagramLayout.ValidLayout"/> property.
    /// </remarks>
    public virtual void LayoutDiagram(LayoutInitial init, bool immediate) {
      if (init == LayoutInitial.ValidateAll) {
        ResetAllLayouts(LayoutInitial.ValidateAll);
        // don't need to call DoLayoutDiagram!
      } else if (immediate) {
        ResetAllLayouts(init);
        DoLayoutDiagram();
      } else {
        // this is delayed for Silverlight to allow nodes to data-bind Location,
        // which occurs later than in WPF
        Diagram.InvokeLater(this, () => { ResetAllLayouts(init); });
        InvokeLayoutDiagram("UpdateLayout");
      }
    }


    /// <summary>
    /// Identifies the <see cref="Initial"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InitialProperty;
    /// <summary>
    /// Gets or sets under what conditions an "initialization" or "reset" of the model should cause layouts to be invalidated.
    /// </summary>
    /// <value>
    /// The default value is <see cref="LayoutInitial.InvalidateIfNodesUnlocated"/>.
    /// </value>
    /// <remarks>
    /// The diagram will call <see cref="LayoutDiagram(LayoutInitial, bool)"/> with this value
    /// when there has been a significant change to the whole diagram, such as:
    /// <list>
    /// <item>the diagram's model's <see cref="Northwoods.GoXam.Model.IDiagramModel.NodesSource"/> (or other source) has been replaced</item>
    /// <item>one of the diagram's model's "Path..." properties has changed</item>
    /// <item>the diagram's <see cref="Northwoods.GoXam.Diagram.Model"/> has been replaced</item>
    /// <item>one or more of the diagram's node <c>DataTemplate</c>s have been replaced</item>
    /// </list>
    /// </remarks>
    public LayoutInitial Initial {
      get { return (LayoutInitial)GetValue(InitialProperty); }
      set { SetValue(InitialProperty, value); }
    }

    private void ResetAllLayouts(LayoutInitial reason) {
      if (reason == LayoutInitial.None) return;
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      if (reason == LayoutInitial.InvalidateAll || reason == LayoutInitial.ValidateAll) {
        // invalidate or validate all layouts
        foreach (Node n in diagram.Nodes) {
          Group g = n as Group;
          if (g != null) ResetOneLayout(reason, g.Layout);
        }
        ResetOneLayout(reason, diagram.Layout);
      } else if (reason == LayoutInitial.InvalidateIfNodesUnlocated) {
        // validate all layouts ...
        foreach (Node n in diagram.Nodes) {
          Group g = n as Group;
          if (g != null) ResetOneLayout(LayoutInitial.ValidateAll, g.Layout);
        }
        ResetOneLayout(LayoutInitial.ValidateAll, diagram.Layout);
        // ... unless they have unlocated nodes
        foreach (Node n in diagram.Nodes) {
          Point loc = n.Location;
          if (Double.IsNaN(loc.X) || Double.IsNaN(loc.Y)) {
            IDiagramLayout layout = GetLayoutBy(n);
            if (layout != null && layout.ValidLayout) layout.ValidLayout = false;
          }
        }
      }
    }

    private void ResetOneLayout(LayoutInitial reason, IDiagramLayout layout) {
      if (layout == null) return;
      switch (reason) {
        case LayoutInitial.InvalidateAll:
          layout.ValidLayout = false;
          break;
        case LayoutInitial.ValidateAll:
          layout.ValidLayout = true;
          break;
        default: break;
      }
    }


    // this is just called from LayoutDiagram (above) & PartManager.OnModelChanged upon CommittedTransaction
    internal void InvokeLayoutDiagram(String why) {
      VerifyAccess();
      if (this.LayoutDiagramReason == null) {
        if (this.IsLayingOut) return;
        Diagram diagram = this.Diagram;
        if (diagram == null) return;
        if (diagram.Model == null) return;
        if (diagram.Model.NodesSource == null) return;
        this.LayoutDiagramReason = why;
        Diagram.InvokeLater(this, DoLayoutDiagram);
      }
    }

    private String LayoutDiagramReason { get; set; }  // first reason for layout request
    private bool IsLayingOut { get; set; }  // true during DoLayoutDiagram, but not during any animation
    internal bool IsAnimating {  // true during animation, not during layout
      get { return _IsAnimating; }
      set {
        if (_IsAnimating != value) {
          bool old = _IsAnimating;
          //Diagram.Debug("  isanimating: " + old.ToString() + " --> " + value.ToString());
          _IsAnimating = value;
          //Diagram diagram = this.Diagram;
          //if (diagram != null) {
          //  DiagramPanel panel = diagram.Panel;
          //  if (panel != null) {
          //    panel.RaiseAnimatingChanged(this, value);
          //  }
          //}
        }
      }
    }
    private bool _IsAnimating;
    private bool WasEnabled { get; set; }
    private bool HadFocus { get; set; }
    private Storyboard Story { get; set; }  // collects animations during layout
    private Storyboard StoryToCancel { get; set; }  // during animation, save it for possible cancellation
    private Dictionary<Node, PointAnimation> Animations = new Dictionary<Node, PointAnimation>();

    private void DoLayoutDiagram() {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      // postpone this layout request?
      if (this.IsLayingOut || this.IsAnimating) {
        Diagram.InvokeLater(this, DoLayoutDiagram);
        return;
      }
      try {
        //?? generalize the UI for indicating that a layout is ongoing, if it's going to take more than a second or so
        diagram.Cursor = Cursors.Wait;
        this.IsLayingOut = true;
        // this.IsAnimating is false now, due to above check
        this.HadFocus = diagram.IsKeyboardFocused;
        //DateTime before = DateTime.Now;
        this.Story = new Storyboard();  // collect any animations
        diagram.StartTransaction("Layout");
        PerformLayout();
        IDiagramModel model = diagram.Model;
        if (model != null) model.SkipsUndoManager = true;
        StartStoryboard(this.Story);
        //Diagram.Debug("total layout time: " + Diagram.Str((DateTime.Now-before).TotalMilliseconds)
        //  + "  nodes: " + this.Diagram.PartManager.NodesCount.ToString()
        //  + "  since Created: " + (DateTime.Now-this.Diagram.DebugCreated).TotalMilliseconds.ToString());
      } finally {
        // to avoid infinite loop, PartManager checks for "Layout" to avoid calling InvokeLayoutDiagram again
        diagram.CommitTransaction("Layout");
        if (!this.IsAnimating) FinishedLayout();
        this.LayoutDiagramReason = null;
        this.IsLayingOut = false;
      }
    }

    private void FinishedLayout() {  // no animation
      this.SkipsInvalidate = false;
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      //Diagram.Debug(diagram.Name + ".IsEnabled finished " + diagram.IsEnabled.ToString() + "  no animation");
      diagram.Cursor = null;
      IDiagramModel model = diagram.Model;
      if (model != null) model.SkipsUndoManager = false;
      DiagramPanel panel = diagram.Panel;
      if (panel != null) panel.InvokeUpdateDiagramBounds("Layout");
    }

    /// <summary>
    /// This method is called asynchronously in order to layout a <see cref="Diagram"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This calls <see cref="IDiagramLayout.DoLayout"/> on each low-level <see cref="Group"/>
    /// that has a <see cref="Group.Layout"/>, and proceeds up the chains of containing
    /// groups until it finally does a layout on the <see cref="Diagram"/>'s <see cref="Northwoods.GoXam.Diagram.Layout"/>.
    /// For a <see cref="Group"/> that does not have a <see cref="Group.Layout"/>, a layout
    /// ignores the group itself but includes the group's members as if the group did not exist.
    /// If <see cref="IDiagramLayout.ValidLayout"/> is true for a layout, this does not call its
    /// <see cref="IDiagramLayout.DoLayout"/> method.
    /// </para>
    /// <para>
    /// The layout ignores invisible elements.
    /// A <see cref="Node"/> or <see cref="Link"/> is included in a layout only if <see cref="CanLayoutPart"/>
    /// returns true.
    /// </para>
    /// </remarks>
    protected virtual void PerformLayout() {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      IDiagramLayout layout = diagram.Layout;
      // even if there's no Diagram.Layout, we still want to perform layouts of group nodes
      // recurse to do any nested layouts of groups, and to find nested nodes without layouts
      foreach (Group g in diagram.Nodes.OfType<Group>().Where(x => Part.IsVisibleElement(x) && x.IsTopLevel)) {
        PerformLayout1(g, layout, null, null);
      }
      if (layout != null && !layout.ValidLayout) {
        layout.Diagram = diagram;
        layout.Group = null;
        //if (!(diagram is Palette)) Diagram.Debug("top layout #" + diagram.PartManager.NodesCount.ToString() + " " + diagram.Nodes.Where(n => CanLayoutPart(n, layout)).Count().ToString() + " nodes " + Diagram.Str(diagram.Nodes.Where(n => CanLayoutPart(n, layout)).Cast<Part>().Take(20)));
        layout.DoLayout(diagram.Nodes.Where(n => CanLayoutPart(n, layout)), diagram.Links.Where(l => CanLayoutPart(l, layout)));
        layout.ValidLayout = true;
      }
      //if (!(diagram is Palette)) Diagram.Debug("  PerformLayout finished");
    }

    private void PerformLayout1(Group group, IDiagramLayout parentlayout, HashSet<Node> nodes, HashSet<Link> links) {
      IDiagramLayout layout = group.Layout;
      if (layout != null) {  // a group with a Layout
        layout.Diagram = this.Diagram;
        layout.Group = group;
        // collect new sets of nodes and links to be laid out
        HashSet<Node> memnodes = new HashSet<Node>();
        HashSet<Link> memlinks = new HashSet<Link>();
        // recurse to find any nodes inside groups without Layouts
        foreach (Node m in group.MemberNodes.Where(x => CanLayoutPart(x, layout))) {
          Group g = m as Group;
          if (g != null) {  // recurse; but if G has no Layout, use this GROUP's Layout
            PerformLayout1(g, layout, memnodes, memlinks);
          }
          if (GetLayoutBy(m) == layout) {
            memnodes.Add(m);
          }
        }
        if (memnodes.Count > 0) {  // don't bother collecting links or calling DoLayout if there aren't any nodes
          foreach (Link l in group.MemberLinks.Where(x => CanLayoutPart(x, layout))) {
            if (GetLayoutBy(l) == layout) {
              memlinks.Add(l);
            }
          }
          // if there are some nodes and the layout is invalid, call DoLayout
          if (!layout.ValidLayout) {
            layout.DoLayout(memnodes, memlinks);
            layout.ValidLayout = true;
            foreach (Link l in memlinks) {
              if (!Part.IsVisibleElement(l)) continue;
              Route route = l.Route;
              if (route != null) {
                Rect b = route.RouteBounds;  // make sure the route points are up-to-date
              }
            }
            //if (!(this.Diagram is Palette)) Diagram.Debug("nested layout " + Diagram.Str(group) + ": " + memnodes.Count.ToString() + "/" + group.MemberNodes.Count().ToString() + " nodes " + memlinks.Count.ToString() + "/" + group.MemberLinks.Count().ToString() + " links " + Diagram.Str(memnodes.ToArray()) + " bounds: " + Diagram.Str(group.Bounds));
            // Now layout.ValidLayout should be true
            // and add to the parent's collection of nodes to be laid out
            if (nodes != null && CanLayoutPart(group, parentlayout)) {
              nodes.Add(group);
              //Diagram.Debug("  invalidating: " + (parentlayout.Group != null ? Diagram.Str(parentlayout.Group) : "top"));
              if (parentlayout != null) parentlayout.Invalidate(LayoutChange.GroupSizeChanged, group);
            }
          }
        }
      } else {  // no Layout for this group node
        // recurse into this group to find more nodes to be laid out by the parent Layout
        foreach (Node m in group.MemberNodes.Where(x => CanLayoutPart(x, parentlayout))) {
          Group g = m as Group;
          if (g != null) {
            PerformLayout1(g, parentlayout, nodes, links);
          } else {  // add any simple member nodes to the parent Layout
            if (nodes != null && GetLayoutBy(m) == parentlayout) {
              nodes.Add(m);
            }
          }
        }
        // and add any member links to the parent Layout
        if (links != null) {
          foreach (Link l in group.MemberLinks.Where(x => CanLayoutPart(x, parentlayout))) {
            if (GetLayoutBy(l) == parentlayout) {
              links.Add(l);
            }
          }
        }
        // but don't add this group to the NODES collection
      }
    }

    /// <summary>
    /// This predicate decides whether a <see cref="Part"/> should participate in an
    /// <see cref="IDiagramLayout"/>'s layout.
    /// </summary>
    /// <param name="p">a <see cref="Node"/> or <see cref="Link"/></param>
    /// <param name="lay">the layout that might be responsible for positioning the part</param>
    /// <returns>
    /// True if the part is visible and if the part's <see cref="Part.LayoutId"/> is not "None" and is
    /// either "All" or the same value as the layout's <see cref="IDiagramLayout.Id"/>.
    /// False for all <see cref="Adornment"/>s and for all <see cref="Node"/>s that are
    /// <see cref="Node.IsLinkLabel"/>, and for all <see cref="Group"/>s that have no
    /// <see cref="Group.Layout"/>.
    /// </returns>
    public virtual bool CanLayoutPart(Part p, IDiagramLayout lay) {
      if (p == null || lay == null) return false;
      Node n = p as Node;
      if (n != null) {
        if (n.IsLinkLabel && n.LabeledLink != null) return false;  // expect that label nodes get laid out by their LinkPanel
        if (n is Adornment) return false;
      }
      IDiagramLayout layout = GetLayoutBy(p);
      if (layout != lay) {
        MultiLayout multi = layout as MultiLayout;
        if (multi != null) {
          if (!multi.Layouts.Contains(lay)) return false;
        } else {
          return false;
        }
      }
      return lay.CanLayoutPart(p);
    }

    // does NOT observe CanLayoutPart -- just looks for nearest group.Layout
    private IDiagramLayout GetLayoutBy(Part p) {  //?? slow!
      foreach (Group g in p.ContainingGroups) {
        IDiagramLayout lay = g.Layout;
        if (lay == null) lay = GetLayoutBy(g);
        if (lay != null) return lay;
      }
      Diagram diagram = p.Diagram;
      if (diagram != null) return diagram.Layout;
      return null;
    }

    /// <summary>
    /// Gets or sets whether <see cref="InvalidateLayout"/> should do nothing when a part is added/removed/changed.
    /// </summary>
    /// <value>
    /// By default this is false.
    /// </value>
    /// <remarks>
    /// This property does not affect the behavior of <see cref="LayoutDiagram(LayoutInitial, bool)"/>
    /// in determining the value of <see cref="IDiagramLayout.ValidLayout"/> for all layouts in this diagram.
    /// </remarks>
    public bool SkipsInvalidate { get; set; }

    /// <summary>
    /// Declare that the <see cref="IDiagramLayout"/> including a given <see cref="Part"/>
    /// is now invalid and may need to be performed again soon.
    /// </summary>
    /// <param name="p">must be a <see cref="Part"/></param>
    /// <param name="change">
    /// a hint on the reason for the need for a new layout;
    /// this is used to decide whether a layout is really needed.
    /// </param>
    /// <remarks>
    /// <para>
    /// This is called by the diagram's <see cref="PartManager"/> each time
    /// a node or link or group-membership is added or removed.
    /// It calls <see cref="IDiagramLayout.Invalidate"/> to let the part's
    /// responsible <see cref="IDiagramLayout"/> know that there's been a change
    /// and that the layout may need to be performed again, depending on
    /// the layout's <see cref="DiagramLayout.Conditions"/>.
    /// </para>
    /// <para>
    /// This method ignores parts in temporary layers.
    /// This method also does nothing if <see cref="SkipsInvalidate"/> is true
    /// or if the <see cref="Northwoods.GoXam.Diagram.Model"/>'s
    /// <see cref="IDiagramModel.SkipsUndoManager"/> property is true.
    /// </para>
    /// </remarks>
    public virtual void InvalidateLayout(Part p, LayoutChange change) {
      // ignore any invalidations during a layout
      if (this.IsLayingOut) return;
      // ignore any invalidations during a rebuild (replaced NodesSource or replaced Model or changed templates)
      if (this.SkipsInvalidate) return;
      // ignore any position changes during animation
      if (change == LayoutChange.NodeLocationChanged && this.IsAnimating) return;
      // ignore temporary objects
      if (p != null && (p.Layer == null || p.Layer.IsTemporary)) return;
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      // invalidate the layout for the part
      IDiagramLayout layout;
      if (p != null) {
        if ((change == LayoutChange.MemberAdded || change == LayoutChange.MemberRemoved) && p is Group) {
          layout = ((Group)p).Layout;
        } else {
          layout = GetLayoutBy(p);
        }
      } else {
        layout = diagram.Layout;
      }
      //if (change != LayoutChange.NodeLocationChanged && change != LayoutChange.NodeSizeChanged && change != LayoutChange.GroupSizeChanged && !(diagram is Palette))
      //  Diagram.Debug("LayoutManager.InvalidateLayout: " + Diagram.Str(p) + " " + change.ToString() + " in " + (layout == null ? "(no layout)" : (layout.Group == null ? "(diagram)" : Diagram.Str(layout.Group))));
      if (layout != null) layout.Invalidate(change, p);
    }


    // Animation control

    /// <summary>
    /// Identifies the <see cref="Animated"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AnimatedProperty;
    /// <summary>
    /// Gets or sets whether there should be animated motion moving the nodes
    /// from their current positions to their laid out positions.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <seealso cref="AnimationTime"/>
    public bool Animated {
      get { return (bool)GetValue(AnimatedProperty); }
      set { SetValue(AnimatedProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AnimationTime"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AnimationTimeProperty;
    /// <summary>
    /// Gets or sets the time in milliseconds that any animation should take.
    /// </summary>
    /// <value>
    /// The default value is 250 (a quarter second).
    /// </value>
    /// <seealso cref="Animated"/>
    public int AnimationTime {  // in milliseconds
      get { return (int)GetValue(AnimationTimeProperty); }
      set { SetValue(AnimationTimeProperty, value); }
    }
    private static void OnAnimationTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      LayoutManager mgr = (LayoutManager)d;
      mgr.AnimationDuration = new Duration(new TimeSpan(0, 0, 0, 0, mgr.AnimationTime));
    }
    private Duration AnimationDuration { get; set; }


    /// <summary>
    /// Identifies the <see cref="DefaultLocation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultLocationProperty;
    /// <summary>
    /// Gets or sets the location from which unpositioned nodes appear
    /// at the start of the animation.
    /// </summary>
    /// <value>
    /// The default value is the origin <c>Point</c>(0, 0).
    /// </value>
    /// <remarks>
    /// Animated movement is fine for nodes that have a location before the layout,
    /// but for those nodes that did not have a location, pretend that they started
    /// at the <c>Point</c> given by this property.
    /// </remarks>
    public Point DefaultLocation {
      get { return (Point)GetValue(DefaultLocationProperty); }
      set { SetValue(DefaultLocationProperty, value); }
    }

    /// <summary>
    /// This basically just sets the value of <see cref="Northwoods.GoXam.Node.Position"/>,
    /// but with animated movement.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="newpos">a new position in model coordinates</param>
    /// <remarks>
    /// There is no animation if <see cref="Animated"/> is false,
    /// if the <see cref="AnimationTime"/> is very small, or
    /// if the original position and the new position are very near to each other
    /// or offscreen.
    /// </remarks>
    public void MoveAnimated(Node node, Point newpos) {
      if (node == null) return;
      VerifyAccess();
      Point oldloc = node.Location;
      Rect oldb = node.Bounds;

      //Diagram.Debug("LayoutManager.MoveAnimated: " + Diagram.Str(node) + "  " + Diagram.Str(node.Position) + " -- > " + Diagram.Str(newpos));
      // always move the node
      node.Position = newpos;
      if (this.Story != null) {
        // if MoveAnimated is called more than once for a Node, remove any previous saved animation
        PointAnimation anim = null;
        if (this.Animations.TryGetValue(node, out anim)) {
          //Diagram.Debug("  removed animation for " + node.ToString());
          this.Story.Children.Remove(anim);
        }
      }

      // try not to bother with animations that won't be visibly useful
      if (!this.Animated) return;
      if (this.AnimationTime <= 10) return;
      // two pixels or more (but that's in model coordinates)
      if (Math.Abs(newpos.X-oldb.X) < 2 && Math.Abs(newpos.Y-oldb.Y) < 2) return;
      FrameworkElement elt = node.VisualElement;
      if (elt == null) return;

      // only do animation for nodes whose new or old bounds are visible in the view
      //?? this doesn't work reliably:
      Diagram diagram = this.Diagram;
      if (diagram == null || diagram.Panel == null) return;
      Rect nearRect = diagram.Panel.InflatedViewportBounds;
      Rect newb = new Rect(newpos.X, newpos.Y, oldb.Width, oldb.Height);
      if (!Geo.Intersects(nearRect, oldb) && !Geo.Intersects(nearRect, newb)) return;

      // start an animation
      //Diagram.Debug("  MoveAnimated " + Diagram.Str(new Point(oldb.X, oldb.Y)) + Diagram.Str(node.Position));
      if (Double.IsNaN(oldloc.X)) oldloc.X = this.DefaultLocation.X;
      if (Double.IsNaN(oldloc.Y)) oldloc.Y = this.DefaultLocation.Y;
      PointAnimation a = new PointAnimation();
      a.From = oldloc;
      a.Duration = this.AnimationDuration;

      Storyboard.SetTarget(a, elt);
      Storyboard.SetTargetProperty(a, new PropertyPath(Node.LocationProperty));
      Storyboard story = this.Story;
      if (story == null) {  // when called from outside of DoLayoutDiagram
        Storyboard singlestory = new Storyboard();
        singlestory.Children.Add(a);
        StartStoryboard(singlestory);
      } else {
        StartingLayoutAnimation();
        story.Children.Add(a);  // collect animations for later action, all at once
        this.Animations[node] = a;
      }
    }

    private void StartingLayoutAnimation() {
      if (this.IsAnimating) return;
      this.IsAnimating = true;
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      //Diagram.Debug("Starting animation: " + diagram.Name);
      this.WasEnabled = diagram.IsEnabled;
      if (this.WasEnabled) {
        diagram.IsEnabled = false;
      }
    }

    private void FinishedLayoutAnimation() {
      this.SkipsInvalidate = false;
      if (!this.IsAnimating) return;
      this.IsAnimating = false;
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      //Diagram.Debug("Finished animation: " + diagram.Name);
      if (this.WasEnabled) {
        this.WasEnabled = false;
        diagram.ClearValue(System.Windows.Controls.Control.IsEnabledProperty);
      }
      if (this.HadFocus) diagram.Focus();
      diagram.Cursor = null;
      IDiagramModel model = diagram.Model;
      if (model != null) model.SkipsUndoManager = false;

      // combine layout edits into previous transaction
      if (model != null) {
        UndoManager undomgr = model.UndoManager;
        if (undomgr != null) undomgr.CoalesceLastTransaction("Layout");
      }

      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;
      panel.InvokeUpdateDiagramBounds("Layout");
    }

    private void StartStoryboard(Storyboard story) {
      if (story == null) return;
      story.Completed += (s, e) => {
        StopStoryboard(story);
        UpdateDiagramBounds();
      };
      story.Duration = this.AnimationDuration;
      //Diagram.Debug("Storyboard.Begin " + this.Diagram.ToString());



      story.Begin();
      if (this.Story == story) {
        this.Story = null;  // don't collect any more animations
        this.StoryToCancel = story; // remember for cancellation
        this.Animations.Clear();  // don't need this hashtable
      }
    }

    private void StopStoryboard(Storyboard story) {
      if (story == null) return;
      //Diagram.Debug("Storyboard.Stop " + this.Diagram.ToString());
      if (this.Story == story || this.StoryToCancel == story) {
        this.Story = null;
        this.StoryToCancel = null;
      }
      story.Stop();



    }

    internal void CancelAnimatedLayout() {
      //Diagram.Debug("cancelling animated layout " + this.Diagram.ToString() + " " + (this.StoryToCancel != null ? this.StoryToCancel.ToString() : "none"));
      StopStoryboard(this.StoryToCancel);
      this.IsLayingOut = false;
      this.IsAnimating = false;
      this.Story = null;
      this.StoryToCancel = null;
      this.Animations.Clear();
      this.UpdateLinksActions = null;
    }

    internal void AddUpdateLinks(DiagramLayout layout, Action update) {
      if (!this.Animated) return;
      // just for top-level layout, not for groups
      if (layout.Group == null) {
        if (this.UpdateLinksActions == null) this.UpdateLinksActions = new List<Action>();
        this.UpdateLinksActions.Add(update);
      }
    }

    private List<Action> UpdateLinksActions { get; set; }

    // called after animation has completed
    private void UpdateDiagramBounds() {
      // still have to update links?  delay the DiagramBounds update even more!
      if (this.UpdateLinksActions != null) {
        InvokeUpdateLinks("AnimationRerouting");
      } else {
        FinishedLayoutAnimation();
      }
    }

    private String InvokeUpdateLinksReason { get; set; }

    private void InvokeUpdateLinks(String why) {
      VerifyAccess();
      if (this.InvokeUpdateLinksReason == null) {
        if (this.UpdateLinksActions == null) return;
        this.InvokeUpdateLinksReason = why;
        Diagram.InvokeLater(this, DoUpdateLinks);
      }
    }

    private void DoUpdateLinks() {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      if (this.UpdateLinksActions != null) {
        bool oldskip = false;
        IDiagramModel model = diagram.Model;
        if (model != null) {
          oldskip = model.SkipsUndoManager;
          model.SkipsUndoManager = true;
        }
        foreach (Action act in this.UpdateLinksActions) act();
        if (model != null) {
          model.SkipsUndoManager = oldskip;
        }
        this.UpdateLinksActions = null;
      }
      FinishedLayoutAnimation();
      this.InvokeUpdateLinksReason = null;
    }
  }

  /// <summary>
  /// This enumeration controls the initial validity of <see cref="IDiagramLayout"/>s
  /// managed by <see cref="LayoutManager"/>, as the value of <see cref="LayoutManager.Initial"/>.
  /// </summary>
  /// <remarks>
  /// <see cref="LayoutManager.LayoutDiagram(LayoutInitial, bool)"/> describes some of the
  /// circumstances in which a diagram has had changes which would suggest invalidating (or validating)
  /// all of the layouts in a diagram.
  /// </remarks>
  public enum LayoutInitial {
    /// <summary>
    /// Do not change the value of <see cref="IDiagramLayout.ValidLayout"/> when the diagram is "reset".
    /// </summary>
    None = 0,
    /// <summary>
    /// Set all layouts to be invalid when the diagram is "reset"; all layouts will be performed soon.
    /// </summary>
    InvalidateAll,
    /// <summary>
    /// Set all layouts to be valid when the diagram is "reset", except declare invalid those layouts that have
    /// nodes without a <see cref="Node.Location"/> (i.e. one or both of the X and Y values are <c>NaN</c>).
    /// </summary>
    InvalidateIfNodesUnlocated,
    /// <summary>
    /// Set all layouts to be valid when the diagram is "reset"; no layout is performed until
    /// some other changes cause a layout to become invalid.
    /// </summary>
    ValidateAll,
  }
}
