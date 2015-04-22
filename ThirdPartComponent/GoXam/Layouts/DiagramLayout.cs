
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
using Northwoods.GoXam.Model;

namespace Northwoods.GoXam.Layout {
  /// <summary>
  /// <c>DiagramLayout</c> is the base class for all of the predefined specific layout implementations.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This provides a rudimentary default layout that will position all of the nodes that have no position
  /// (i.e. the <see cref="Node.Location"/> is <c>NaN, NaN</c>).  Nodes that already have a position are ignored.
  /// The layout behavior may be improved in the future.
  /// </para>
  /// <para>
  /// The <see cref="LayoutManager"/> will call <see cref="CanLayoutPart"/> to decide
  /// which <see cref="Node"/>s and <see cref="Link"/>s will be passed to <see cref="DoLayout"/>.
  /// <see cref="CanLayoutPart"/> looks at this layout's <see cref="Id"/> and the part's
  /// <see cref="Part.LayoutId"/> to see if they match, among other criteria.
  /// </para>
  /// <para>
  /// As changes occur to the diagram, such as the addition of a <see cref="Node"/>
  /// or the removal of a <see cref="Link"/> or the change in size of a <see cref="Group"/>,
  /// the <see cref="Invalidate"/> method will be called.
  /// Depending on the kind of change and on the value of <see cref="Conditions"/>,
  /// the <see cref="ValidLayout"/> property may be set to false.
  /// At a later time, such as at the end of a transaction, the <see cref="LayoutManager"/>
  /// will call <see cref="DoLayout"/> to make the layout valid again.
  /// </para>
  /// <para>
  /// To implement your own custom layouts, you can inherit from either this class or from
  /// one of the other predefined layout classes, overriding the <see cref="DoLayout"/> method.
  /// You can call the <see cref="Node.Move"/> method to re-position a node, including whole groups,
  /// possibly with animation.
  /// Install the layout as the value of <see cref="Northwoods.GoXam.Diagram.Layout"/>
  /// or <see cref="Northwoods.GoXam.Group.Layout"/>.
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
  public class DiagramLayout : FrameworkElement, IDiagramLayout {  //?? needs to be FrameworkElement to allow databinding properties, especially as Group.Layout
    /// <summary>
    /// Create a <see cref="DiagramLayout"/> that is invalidated
    /// only under the <see cref="LayoutChange.Standard"/> conditions.
    /// </summary>
    public DiagramLayout() {
      this.Conditions = LayoutChange.Standard;
    }

    /// <summary>
    /// Help making a copy of a <see cref="DiagramLayout"/>.
    /// </summary>
    /// <param name="layout"></param>
    /// <remarks>
    /// This is just called by the copy-constructors of classes that inherit from this one.
    /// </remarks>
    protected DiagramLayout(DiagramLayout layout) {
      if (layout != null) {
        this.Id = layout.Id;
        this.ArrangementOrigin = layout.ArrangementOrigin;
      }
    }

    static DiagramLayout() {
      IdProperty = DependencyProperty.Register("Id", typeof(String), typeof(DiagramLayout),
        new FrameworkPropertyMetadata(""));
      ArrangementOriginProperty = DependencyProperty.Register("ArrangementOrigin", typeof(Point), typeof(DiagramLayout),
        new FrameworkPropertyMetadata(new Point(0, 0)));
    }


    /// <summary>
    /// Throw an exception if the current thread does not have access to this <c>DependencyObject</c>.
    /// </summary>
    protected void VerifyAccess() {
      if (!CheckAccess()) Diagram.Error("No access to thread");
    }


    // IDiagramLayout members:

    /// <summary>
    /// Gets or sets a reference to the owner <see cref="Northwoods.GoXam.Diagram"/>.
    /// </summary>
    /// <value>
    /// The default value is null, but it is set automatically by the
    /// <see cref="Northwoods.GoXam.Diagram.Layout"/> and
    /// <see cref="Northwoods.GoXam.Group.Layout"/> property setters.
    /// You should not need to set this property.
    /// </value>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Diagram Diagram { get; set; }

    /// <summary>
    /// Gets or sets a reference to the owner <see cref="Northwoods.GoXam.Group"/>, if any.
    /// </summary>
    /// <value>
    /// The default value is null, but it is set automatically by the
    /// <see cref="Northwoods.GoXam.Diagram.Layout"/> and
    /// <see cref="Northwoods.GoXam.Group.Layout"/> property setters;
    /// the former sets this property to null.
    /// You should not need to set this property.
    /// </value>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Group Group { get; set; }

    /// <summary>
    /// Identifies the <see cref="Id"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IdProperty;
    /// <summary>
    /// Gets or sets an identifier for a particular layout.
    /// </summary>
    /// <value>
    /// The default value is the empty string.
    /// </value>
    /// <remarks>
    /// This is used by <see cref="DiagramLayout.CanLayoutPart"/> to decide
    /// whether a part should participate in this layout.
    /// </remarks>
    /// <seealso cref="Part.LayoutId"/>
    public String Id {
      get { return (String)GetValue(IdProperty); }
      set { SetValue(IdProperty, value); }
    }

    /// <summary>
    /// Gets or sets whether this layout is valid; if not, the <see cref="Northwoods.GoXam.LayoutManager"/>
    /// will call <see cref="DoLayout"/> sometime in order to make sure the layout is valid.
    /// </summary>
    /// <remarks>
    /// This property simply holds the validity state --
    /// changing the value does not request the performance of the layout any time soon.
    /// </remarks>
    /// <seealso cref="InvalidateLayout"/>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ValidLayout {
      get { return _ValidLayout; }
      set {
        bool old = _ValidLayout;
        if (old != value) {
          _ValidLayout = value;
          if (value && (this.Conditions & LayoutChange.InitialOnly) == LayoutChange.InitialOnly) {
            this.InitialLayoutCompleted = true;
          }
          //if (!(this.Diagram is Palette)) Diagram.Debug("ValidLayout = " + value.ToString() + " " + (this.Group == null ? "(diagram)" : Diagram.Str(this.Group)));
        }
      }
    }
    private bool _ValidLayout;

    // finished layout, when InitialOnly -- don't layout any more, unless unconditionally
    internal bool InitialLayoutCompleted { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="LayoutChange"/> conditions for which
    /// <see cref="Invalidate"/> will set <see cref="ValidLayout"/> to false.
    /// </summary>
    /// <value>
    /// The default value is <see cref="LayoutChange.Standard"/>,
    /// which includes all <see cref="LayoutChange.Added"/> and <see cref="LayoutChange.Removed"/> flags
    /// and <see cref="LayoutChange.DiagramLayoutChanged"/> and <see cref="LayoutChange.GroupLayoutChanged"/>.
    /// </value>
    /// <remarks>
    /// <para>
    /// In WPF XAML, you can combine <see cref="LayoutChange"/> enum values:
    /// <code>&lt;go:TreeLayout Conditions="NodeAdded LinkAdded" ... /&gt;</code>
    /// </para>
    /// <para>
    /// In Silverlight XAML, use the <c>ConditionFlags</c> property instead:
    /// <code>&lt;golayout:TreeLayout ConditionFlags="NodeAdded LinkAdded" ... /&gt;</code>
    /// </para>
    /// <para>
    /// Changing this value does not invalidate this layout,
    /// but will cause the next layout to be considered an "initial" one.
    /// </para>
    /// </remarks>
    public LayoutChange Conditions {
      get { return _Conditions; }
      set {
        LayoutChange old = _Conditions;
        if (old != value) {
          _Conditions = value;
          this.InitialLayoutCompleted = false;
        }
      }
    }
    private LayoutChange _Conditions;


    /// <summary>
    /// Gets or sets the <see cref="LayoutChange"/> conditions for which
    /// <see cref="Invalidate"/> will set <see cref="ValidLayout"/> to false. [Silverlight only.]
    /// </summary>
    /// <value>
    /// By default this includes all <see cref="LayoutChange"/> flags except
    /// for <see cref="LayoutChange.NodeSizeChanged"/>.
    /// </value>
    /// <remarks>
    /// This is the same property as <see cref="Conditions"/>, except that the
    /// type is <see cref="LayoutChangeValue"/>, which can be parsed by XAML.
    /// In Silverlight XAML:
    /// <code>&lt;golayout:TreeLayout ConditionFlags="NodeAdded LinkAdded" ... /&gt;</code>
    /// </remarks>
    public LayoutChangeValue ConditionFlags {
      get { return new LayoutChangeValue() { Value=this.Conditions }; }
      set { this.Conditions = value.Value; }
    }


    /// <summary>
    /// Set the <see cref="ValidLayout"/> property to false, and
    /// ask the diagram's <see cref="LayoutManager"/> to perform layouts in the near future.
    /// </summary>
    /// <remarks>
    /// This is typically called when a layout property value has changed.
    /// </remarks>
    /// <seealso cref="Invalidate"/>
    public void InvalidateLayout() {
      this.ValidLayout = false;
      Diagram diagram = this.Diagram;
      if (diagram != null) {
        LayoutManager mgr = diagram.LayoutManager;
        if (mgr != null) {
          mgr.LayoutDiagram(LayoutInitial.None, false);
        }
      }
    }

    /// <summary>
    /// Declare that this layout might no longer be valid, depending on the change and the <see cref="Conditions"/>.
    /// </summary>
    /// <param name="reason">
    /// A <see cref="LayoutChange"/> hint describing what change may have made the layout invalid.
    /// If the value is <see cref="LayoutChange.All"/>, <see cref="ValidLayout"/> will be set to false unconditionally.
    /// </param>
    /// <param name="part">
    /// the <see cref="Northwoods.GoXam.Part"/> that has changed
    /// </param>
    /// <remarks>
    /// This method may set <see cref="ValidLayout"/> to false, if the <see cref="Conditions"/> are met.
    /// This method does not request a new layout -- call <see cref="InvalidateLayout"/> if
    /// you want to unconditionally set <see cref="ValidLayout"/> to false and ask the
    /// <see cref="LayoutManager"/> to schedule perform layouts that are invalid.
    /// </remarks>
    /// <seealso cref="InvalidateLayout"/>
    public virtual void Invalidate(LayoutChange reason, Part part) {
      if (!this.ValidLayout) return;
      // no Part needed for these two cases:
      if (reason == LayoutChange.All) {
        this.ValidLayout = false;
        return;
      }
      // don't care any more?
      if (this.InitialLayoutCompleted) return;
      if ((reason&this.Conditions&LayoutChange.DiagramLayoutChanged) == LayoutChange.DiagramLayoutChanged) {
        this.ValidLayout = false;
        return;
      }
      // now require a Part
      if (part == null) return;
      // the rest of the LayoutChanges involve a particular Part
      Node node = part as Node;
      if (node != null) {
        Layer layer = node.Layer;
        if (layer != null && !layer.IsTemporary) {
          if ((reason&this.Conditions&LayoutChange.NodeLocationChanged) == LayoutChange.NodeLocationChanged) {
            if (!node.IsLinkLabel) {
              this.ValidLayout = false;
            }
          } else if ((reason&this.Conditions&LayoutChange.NodeSizeChanged) == LayoutChange.NodeSizeChanged) {
            if (!node.IsLinkLabel) {
              this.ValidLayout = false;
            }
          } else if ((reason&this.Conditions&LayoutChange.GroupSizeChanged) == LayoutChange.GroupSizeChanged) {
            if (node is Group) {
              this.ValidLayout = false;
            }
          } else if ((reason&this.Conditions&LayoutChange.NodeAdded) == LayoutChange.NodeAdded) {
            this.ValidLayout = false;
          } else if ((reason&this.Conditions&LayoutChange.NodeRemoved) == LayoutChange.NodeRemoved) {
            this.ValidLayout = false;
          } else if ((reason&this.Conditions&LayoutChange.MemberAdded) == LayoutChange.MemberAdded) {
            this.ValidLayout = false;
          } else if ((reason&this.Conditions&LayoutChange.MemberRemoved) == LayoutChange.MemberRemoved) {
            this.ValidLayout = false;
          } else if ((reason&this.Conditions&LayoutChange.GroupLayoutChanged) == LayoutChange.GroupLayoutChanged) {
            this.ValidLayout = false;
          } else if ((reason&this.Conditions&LayoutChange.VisibleChanged) == LayoutChange.VisibleChanged) {
            this.ValidLayout = false;
          }
        }
      } else {
        Link link = part as Link;
        if (link != null) {
          Layer layer = link.Layer;
          if (layer != null && !layer.IsTemporary) {
            if ((reason&this.Conditions&LayoutChange.LinkAdded) == LayoutChange.LinkAdded) {
              this.ValidLayout = false;
            } else if ((reason&this.Conditions&LayoutChange.LinkRemoved) == LayoutChange.LinkRemoved) {
              this.ValidLayout = false;
            } else if ((reason&this.Conditions&LayoutChange.VisibleChanged) == LayoutChange.VisibleChanged) {
              this.ValidLayout = false;
            }
          }
        }
      }
    }

    /// <summary>
    /// Decide whether a given <see cref="Northwoods.GoXam.Part"/> should participate in this layout.
    /// </summary>
    /// <param name="part"></param>
    /// <returns>
    /// Typically this should return false if the part is not visible,
    /// if the part is in a temporary <see cref="Northwoods.GoXam.Layer"/>,
    /// or if the part's <see cref="Northwoods.GoXam.Part.LayoutId"/> is "None".
    /// This should return true if the part's <c>LayoutId</c> is "All" or
    /// if it matches the <see cref="Id"/> of this layout.
    /// </returns>
    public virtual bool CanLayoutPart(Part part) {
      if (part == null) return false;
      if (part.LayoutId == "None" || this.Id == "None") return false;
      if (!Part.IsVisibleElement(part)) return false;
      if (part.Layer != null && part.Layer.IsTemporary) return false;
      if (part.LayoutId == this.Id) return true;
      if (part.LayoutId == "All" || this.Id == "All") return true;
      // consider Links with default LayoutId if connected to a compatible Node
      Link link = part as Link;
      if (link != null && part.LayoutId == "") {
        if (CanLayoutPart(link.FromNode) || CanLayoutPart(link.ToNode)) return true;
      }
      return false;
    }

    //??? stupid layout of nodes without an assigned Location, ignoring any links or groups

    /// <summary>
    /// Position all nodes that do not have an assigned <see cref="Node.Location"/>
    /// in the manner of a simple rectangular array.
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="links"></param>
    /// <remarks>
    /// You can override this method to do whatever node positioning and link routing that you wish.
    /// </remarks>
    public virtual void DoLayout(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      LayoutManager mgr = diagram.LayoutManager;
      IEnumerable<Node> unlocated = nodes.Where(n => {
        Point loc = n.Location;
        return Double.IsNaN(loc.X) || Double.IsNaN(loc.Y);
      }).ToList();
      int num = unlocated.Count();
      if (num > 0) {
        DiagramPanel panel = diagram.Panel;
        if (panel == null) return;
        int sqrt = (int)Math.Floor(Math.Sqrt(num-1)) + 1;
        double originx = 0;
        double originy = 0;
        double x = originx;
        double y = originy;
        int i = 0;
        double maxh = 0;
        foreach (Node n in unlocated) {
          Size sz = n.GetEffectiveSize(null);
          // skip any parts that are not in NODES or that are UNLOCATED
          //while (!panel.IsUnoccupied(new Rect(x, y, sz.Width, sz.Height),
          //    part => {
          //      Node node = part as Node;
          //      return node != null && (!nodes.Contains(node) || unlocated.Contains(node));
          //    } )) {
            //x += Math.Max(sz.Width, 50)+20;
            //maxh = Math.Max(maxh, Math.Max(sz.Height, 50));
            //if (i >= sqrt-1) {
            //  i = 0;
            //  x = originx;
            //  y += maxh+20;
            //  maxh = 0;
            //} else {
            //  i++;
            //}
          //}
          if (mgr != null) {
            mgr.MoveAnimated(n, new Point(x, y));  // assign new node Position
          } else {
            n.Position = new Point(x, y);
          }
          x += Math.Max(sz.Width, 50)+20;
          maxh = Math.Max(maxh, Math.Max(sz.Height, 50));
          if (i >= sqrt-1) {
            i = 0;
            x = originx;
            y += maxh+20;
            maxh = 0;
          } else {
            i++;
          }
        }
      }
    }

    /// <summary>
    /// Identifies the <see cref="ArrangementOrigin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ArrangementOriginProperty;
    /// <summary>
    /// Gets or sets the point of the top-left node.
    /// </summary>
    /// <value>
    /// The default value is the Point(0,0).
    /// </value>
    /// <remarks>
    /// Some kinds of layout may ignore this property.
    /// </remarks>
    public Point ArrangementOrigin {
      get { return (Point)GetValue(ArrangementOriginProperty); }
      set { SetValue(ArrangementOriginProperty, value); }
    }

    internal Point InitialOrigin(Point origin) {
      if (this.Group != null) {
        if (this.Group.Layout is MultiLayout) return origin;
        Point loc = this.Group.Location;
        if (Double.IsNaN(loc.X) || Double.IsNaN(loc.Y)) return origin;
        GroupPanel gp = this.Group.GroupPanel;
        if (gp != null) {
          loc.X += gp.Padding.Left;
          loc.Y += gp.Padding.Top;
        }
        return loc;
      }
      return origin;
    }

    ///// <summary>
    ///// The Progress event is raised at various times during the <see cref="IDiagramLayout.DoLayout"/>
    ///// procedure to indicate progress.
    ///// </summary>
    ///// <remarks>
    ///// In particular, a Progress event should be raised at the start of the layout 
    ///// with a progress of 0 and at the end of the layout with a progress of 1.
    ///// Other events with progress values should be layout routine specific.
    ///// </remarks>
    //public event EventHandler<ProgressEventArgs> Progress;

    ///// <summary>
    ///// Raise the <see cref="Progress"/> event.
    ///// </summary>
    ///// <param name="e"></param>
    //protected virtual void OnProgress(ProgressEventArgs e) {
    //  if (this.Progress != null) this.Progress(this, e);
    //}

    ///// <summary>
    ///// Raise the <see cref="Progress"/> event indicating fractional completion.
    ///// </summary>
    ///// <param name="frac">a double value from zero to one: zero means starting and one means finished</param>
    //protected void RaiseProgress(double frac) {
    //  OnProgress(new ProgressEventArgs() { Progress = frac });
    //}

    internal void RaiseProgress(double frac) { }
  }


  ///// <summary>
  ///// Holds information for the <see cref="DiagramLayout.Progress"/> event.
  ///// </summary>
  ///// <remarks>
  ///// This class passes the fraction done to <see cref="DiagramLayout.Progress"/>
  ///// event handlers.
  ///// </remarks>
  //public class ProgressEventArgs : RoutedEventArgs {
  //  /// <summary>
  //  /// Gets or sets the fraction that <see cref="IDiagramLayout.DoLayout"/> is done.
  //  /// </summary>
  //  /// <value>
  //  /// This is a double between zero and one.
  //  /// The value is zero initially.
  //  /// The value is one when it is finished.
  //  /// </value>
  //  public double Progress { get; set; }
  //}


  /// <summary>
  /// This enumerates the reasons that a layout may no longer be valid
  /// when a particular <see cref="Part"/> has been added or removed.
  /// </summary>
  /// <remarks>
  /// <para>
  /// These flags values may be combined to provide the <see cref="DiagramLayout.Conditions"/>
  /// under which a <see cref="DiagramLayout"/> may be invalidated.
  /// </para>
  /// <para>
  /// You can combine these values in XAML too.
  /// In WPF, just say:
  /// <code>&lt;golayout:TreeLayout Conditions="NodeAdded LinkAdded" ... /&gt;</code>
  /// But due to a restriction with Silverlight type converters,
  /// if you want to specify the value for <see cref="DiagramLayout.Conditions"/>
  /// in XAML, you will need to set the <c>ConditionFlags</c> attribute instead:
  /// <code>&lt;golayout:TreeLayout ConditionFlags="NodeAdded LinkAdded" ... /&gt;</code>
  /// </para>
  /// </remarks>
  [TypeConverter(typeof(LayoutChangeConverter))]
  [Flags]
  public enum LayoutChange {
    /// <summary>
    /// No particular reason for an layout invalidation.
    /// </summary>
    None=0,
    /// <summary>
    /// A node has been added to the diagram.
    /// </summary>
    NodeAdded=0x0001,
    /// <summary>
    /// A node has been removed from the diagram.
    /// </summary>
    NodeRemoved=0x0002,
    /// <summary>
    /// A link has been added to the diagram.
    /// </summary>
    LinkAdded=0x0004,
    /// <summary>
    /// A link has been removed from the diagram.
    /// </summary>
    LinkRemoved=0x0008,
    /// <summary>
    /// A member node has been added to a group.
    /// </summary>
    MemberAdded=0x0010,
    /// <summary>
    /// A member node has been removed from a group.
    /// </summary>
    MemberRemoved=0x0020,
    /// <summary>
    /// The <see cref="Northwoods.GoXam.Diagram.Layout"/> property value has been replaced.
    /// </summary>
    /// <remarks>
    /// There is no <see cref="Northwoods.GoXam.Part"/> associated with this change.
    /// </remarks>
    DiagramLayoutChanged=0x0040,
    /// <summary>
    /// The <see cref="Northwoods.GoXam.Group.Layout"/> property value has been replaced on a group.
    /// </summary>
    GroupLayoutChanged=0x0080,
    /// <summary>
    /// The <see cref="Northwoods.GoXam.Part.Visible"/> property has changed on a node or a link.
    /// </summary>
    VisibleChanged=0x0100,
    /// <summary>
    /// The size of a node (that is not a group) has changed.
    /// </summary>
    /// <remarks>
    /// Caution: this is uncommonly used, due to the likelihood of frequent layouts.
    /// </remarks>
    NodeSizeChanged=0x0200,
    /// <summary>
    /// The location of a node or group has changed.
    /// </summary>
    /// <remarks>
    /// Caution: this is rarely used, due to the likelihood of frequent layouts.
    /// </remarks>
    NodeLocationChanged=0x0400,
    /// <summary>
    /// The size of a group has changed.
    /// </summary>
    GroupSizeChanged=0x0800,
    /// <summary>
    /// A node, link, or group membership has been added.
    /// </summary>
    Added=NodeAdded | LinkAdded | MemberAdded,
    /// <summary>
    /// A node, link, or group membership has been removed.
    /// </summary>
    Removed=NodeRemoved | LinkRemoved | MemberRemoved,
    /// <summary>
    /// All of the <see cref="Added"/> and <see cref="Removed"/> flags,
    /// plus <see cref="DiagramLayoutChanged"/> and <see cref="GroupLayoutChanged"/>,
    /// but not <see cref="NodeSizeChanged"/> and <see cref="NodeLocationChanged"/> and <see cref="InitialOnly"/>.
    /// </summary>
    /// <remarks>
    /// This is the default value of <see cref="DiagramLayout.Conditions"/>.
    /// It corresponds to all of the graph-structural changes and layout replacements.
    /// </remarks>
    Standard=Added | Removed | DiagramLayoutChanged | GroupLayoutChanged,
    /// <summary>
    /// All <see cref="LayoutChange"/> flags combined together, except for <see cref="InitialOnly"/>.
    /// </summary>
    /// <remarks>
    /// Caution: due to the likelihood of frequent layouts, this is basically only used
    /// to indicate to <see cref="IDiagramLayout.Invalidate"/> that a new layout should
    /// be performed unconditionally.
    /// </remarks>
    All=Standard | VisibleChanged | GroupSizeChanged | NodeSizeChanged | NodeLocationChanged,
    /// <summary>
    /// When this flag is included in the <see cref="DiagramLayout.Conditions"/>,
    /// no layouts will occur after the initial one, unless the layout is invalidated unconditionally.
    /// </summary>
    InitialOnly=0x800000,
  }

#pragma warning disable 1591
  [EditorBrowsableAttribute(EditorBrowsableState.Never)]
  public sealed class LayoutChangeConverter : TypeConverter {  // nested class, must be public for Silverlight
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
      return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
      return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
      String source = value as String;
      if (source != null) {
        String[] split = source.Split(new char[] { '|', '+', ' ', ',' });
        int result = 0;
        foreach (String s in split) {
          if (s.Length == 0) continue;
          int v = (int)Enum.Parse(typeof(LayoutChange), s, true);
          result |= v;
        }

        return new LayoutChangeValue() { Value = (LayoutChange)result };



      }
      return base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {

      if (destinationType == typeof(String) && value is LayoutChangeValue) {
        int v = (int)(((LayoutChangeValue)value).Value);
        String s = "";
        var a = Enumerable.Range(0, 31).Select(i => (LayoutChange)(1 ^ i)).ToArray();






        foreach (Object x in a) {
          int i = (int)x;
          if ((v & i) == i) {
            if (s.Length > 0) s += " ";
            s += x.ToString();
          }
        }
        return s;
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }


  /// <summary>
  /// This trivial wrapper structure exists to allow the XAML parser in Silverlight
  /// to use a TypeConverter to convert a string into a LayoutChange value.
  /// [Silverlight only.]
  /// </summary>
  [TypeConverter(typeof(LayoutChangeConverter))]
  public struct LayoutChangeValue {
    /// <summary>
    /// Gets or sets the <see cref="LayoutChange"/> value.
    /// </summary>
    public LayoutChange Value { get; set; }
  }

#pragma warning restore 1591
}
