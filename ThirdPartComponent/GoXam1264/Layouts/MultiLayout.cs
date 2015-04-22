
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
using System.Windows.Markup;
using Northwoods.GoXam;

namespace Northwoods.GoXam.Layout {
  /// <summary>
  /// This <see cref="IDiagramLayout"/> is a compound layout where
  /// each of the nested <see cref="Layouts"/> works on a subset of the
  /// nodes and links that are this <c>MultiLayout</c>'s responsibility.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The items in the <see cref="Layouts"/> collection must implement
  /// the <see cref="IDiagramLayout"/> interface, and are normally
  /// instances of a <see cref="DiagramLayout"/> class.
  /// Nesting of <see cref="MultiLayout"/>s is probably not useful.
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
  [ContentProperty("Layouts")]
  public class MultiLayout : FrameworkElement, IDiagramLayout {
    /// <summary>
    /// Construct an empty <see cref="MultiLayout"/>.
    /// </summary>
    /// <remarks>
    /// You will need to supply at least one <see cref="IDiagramLayout"/>
    /// in the <see cref="Layouts"/> collection before this <see cref="MultiLayout"/>
    /// can do anything useful.
    /// </remarks>
    public MultiLayout() {
      this.Layouts = new List<IDiagramLayout>();
    }

    static MultiLayout() {
      IdProperty = DependencyProperty.Register("Id", typeof(String), typeof(MultiLayout),
        new FrameworkPropertyMetadata("", OnPropertyChanged));
      ArrangementOriginProperty = DependencyProperty.Register("ArrangementOrigin", typeof(Point), typeof(MultiLayout),
        new FrameworkPropertyMetadata(new Point(0, 0), OnPropertyChanged));
      ArrangementProperty = DependencyProperty.Register("Arrangement", typeof(MultiArrangement), typeof(MultiLayout),
        new FrameworkPropertyMetadata(MultiArrangement.None, OnPropertyChanged));
      ArrangementSpacingProperty = DependencyProperty.Register("ArrangementSpacing", typeof(Size), typeof(MultiLayout),
        new FrameworkPropertyMetadata(new Size(20, 20), OnPropertyChanged));
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      MultiLayout layout = (MultiLayout)d;
      layout.InvalidateLayout();
    }

    /// <summary>
    /// Set the <see cref="ValidLayout"/> property to false, and
    /// ask the diagram's <see cref="LayoutManager"/> to perform layouts in the near future.
    /// </summary>
    public void InvalidateLayout() {
      this.ValidLayout = false;
      Diagram diagram = this.Diagram;
      if (diagram != null) diagram.UpdateDiagramLayout();
    }


    // IDiagramLayout members:

    /// <summary>
    /// Gets or sets the <see cref="Northwoods.GoXam.Diagram"/> that owns this layout.
    /// </summary>
    /// <value>
    /// This may be null if there are no <see cref="Layouts"/>.
    /// You should not need to set this property.
    /// </value>
    public Diagram Diagram {
      get {
        foreach (IDiagramLayout layout in this.Layouts) {
          if (layout.Diagram != null) return layout.Diagram;
        }
        return null;
      }
      set {
        foreach (IDiagramLayout layout in this.Layouts) {
          layout.Diagram = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="Northwoods.GoXam.Group"/> that owns this layout,
    /// if the layout is the value of a <see cref="Northwoods.GoXam.Group.Layout"/>.
    /// </summary>
    /// <value>
    /// This returns null if this layout is declared on the <see cref="Northwoods.GoXam.Diagram"/>
    /// or if there are no <see cref="Layouts"/>.
    /// </value>
    public Group Group {
      get {
        foreach (IDiagramLayout layout in this.Layouts) {
          if (layout.Group != null) return layout.Group;
        }
        return null;
      }
      set {
        foreach (IDiagramLayout layout in this.Layouts) {
          layout.Group = value;
        }
      }
    }

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
    /// This property is ignored -- <see cref="CanLayoutPart"/> just calls
    /// <see cref="IDiagramLayout.CanLayoutPart"/> of the nested layouts.
    /// </remarks>
    public String Id {
      get { return (String)GetValue(IdProperty); }
      set { SetValue(IdProperty, value); }
    }


    /// <summary>
    /// This layout is valid only if all of its <see cref="Layouts"/> are valid.
    /// </summary>
    public virtual bool ValidLayout {
      get {
        foreach (IDiagramLayout layout in this.Layouts) {
          if (!layout.ValidLayout) return false;
        }
        return true;
      }
      set {
        //Diagram.Debug("Multi: " + this.Layouts.Count.ToString() + " ValidLayout = " + value.ToString() + " " + (this.Group == null ? "(diagram)" : Diagram.Str(this.Group)));
        foreach (IDiagramLayout layout in this.Layouts) {
          layout.ValidLayout = value;
        }
      }
    }

    /// <summary>
    /// Declare that this layout may be invalid, for a given reason.
    /// </summary>
    /// <param name="reason">a <see cref="LayoutChange"/> hint</param>
    /// <param name="part">the <see cref="Northwoods.GoXam.Part"/> that changed</param>
    /// <remarks>
    /// For each layout in <see cref="Layouts"/> in which the given part participates,
    /// call <see cref="IDiagramLayout.Invalidate"/> with the same arguments.
    /// </remarks>
    public virtual void Invalidate(LayoutChange reason, Part part) {
      foreach (IDiagramLayout layout in this.Layouts) {
        if (reason == LayoutChange.All || layout.CanLayoutPart(part)) {
          layout.Invalidate(reason, part);
        }
      }
    }

    /// <summary>
    /// This predicate is true if <see cref="IDiagramLayout.CanLayoutPart"/>
    /// is true for any of its <see cref="Layouts"/>.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public virtual bool CanLayoutPart(Part part) {
      foreach (IDiagramLayout layout in this.Layouts) {
        if (layout is MultiLayout) {
          Diagram.Error("MultiLayouts cannot be nested");
          return false;
        }
        DiagramLayout lay = layout as DiagramLayout;
        if (lay != null) lay.DataContext = this.DataContext;  //?? support databinding of Layout properties
        if (layout.CanLayoutPart(part)) return true;
      }
      return false;
    }

    /// <summary>
    /// Actually perform all of the <see cref="Layouts"/> for the given nodes and links.
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="links"></param>
    /// <remarks>
    /// This iterates over the layouts in <see cref="Layouts"/>.
    /// If the layout's <see cref="IDiagramLayout.ValidLayout"/> is false,
    /// it gets the subsets of nodes and links that apply to that particular layout
    /// and then calls <see cref="IDiagramLayout.DoLayout"/> on it.
    /// </remarks>
    public virtual void DoLayout(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      if (diagram.Panel == null) return;
      LayoutManager mgr = diagram.LayoutManager;
      Northwoods.GoXam.Tool.DraggingTool tool = diagram.DraggingTool;
      if (tool == null) tool = new Northwoods.GoXam.Tool.DraggingTool();
      Point pos = this.ArrangementOrigin;
      foreach (IDiagramLayout layout in this.Layouts) {
        bool invalidlayout = !layout.ValidLayout;
        if (invalidlayout || this.Arrangement != MultiArrangement.None) {  //??? need to move networks of valid layouts
          IEnumerable<Node> subnodes = (mgr != null ? nodes.Where(n => mgr.CanLayoutPart(n, layout)) : nodes);
          IEnumerable<Link> sublinks = (mgr != null ? links.Where(l => mgr.CanLayoutPart(l, layout)) : links);
          if (invalidlayout) {
            //Diagram.Debug("multilayout " + layout.Id + " of " + Diagram.Str(layout.Group) + ": INVALID " + subnodes.Count().ToString() + " nodes " + sublinks.Count().ToString() + " links " + Diagram.Str(subnodes.ToArray()) + " at " + Diagram.Str(pos));
            DiagramLayout dlay = layout as DiagramLayout;
            if (dlay != null) dlay.ArrangementOrigin = pos;
            layout.DoLayout(subnodes, sublinks);
          }
          //?? need to make sure all links are routed, so their Bounds aren't bogus
          Rect b = diagram.Panel.ComputeBounds(subnodes.OfType<Part>() /*.Concat<Part>(sublinks.OfType<Part>())*/ );
          if (!invalidlayout && !b.IsEmpty) {
            //Diagram.Debug("multilayout " + layout.Id + " of " + Diagram.Str(layout.Group) + ": MOVING " + subnodes.Count().ToString() + " nodes " + sublinks.Count().ToString() + " links " + Diagram.Str(new Point(pos.X-b.X, pos.Y-b.Y)) + "  computedBounds: " + Diagram.Str(b));
            var dict = tool.ComputeEffectiveCollection(subnodes.OfType<Part>());
            tool.MoveParts(dict, new Point(pos.X-b.X, pos.Y-b.Y));
          }
          switch (this.Arrangement) {
            case MultiArrangement.None: break;
            case MultiArrangement.Horizontal:
              if (b.Width > 0) {
                pos.X += b.Width;
                pos.X += this.ArrangementSpacing.Width;
              }
              break;
            case MultiArrangement.Vertical:
              if (b.Height > 0) {
                pos.Y += b.Height;
                pos.Y += this.ArrangementSpacing.Height;
              }
              break;
          }
        }
      }
    }

    /// <summary>
    /// Gets the collection of <see cref="IDiagramLayout"/> layouts that are
    /// managed by this <see cref="MultiLayout"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Because this is a "Content" property, one can initialize an instance
    /// of <see cref="MultiLayout"/> in XAML just by creating nested elements
    /// that are themselves layouts.
    /// </para>
    /// <para>
    /// Nesting of <see cref="MultiLayout"/>s is not supported,
    /// since it would probably not be useful.
    /// </para>
    /// </remarks>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public System.Collections.IList Layouts { get; private set; }


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


    /// <summary>
    /// Identifies the <see cref="Arrangement"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ArrangementProperty;
    /// <summary>
    /// Gets or sets how <see cref="DoLayout"/> should lay out the nested layouts.
    /// </summary>
    /// <value>
    /// The default value is <see cref="MultiArrangement.Vertical"/>.
    /// </value>
    [DefaultValue(MultiArrangement.None)]
    public MultiArrangement Arrangement {
      get { return (MultiArrangement)GetValue(ArrangementProperty); }
      set { SetValue(ArrangementProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="ArrangementSpacing"/> dependency property;
    /// </summary>
    public static readonly DependencyProperty ArrangementSpacingProperty;
    /// <summary>
    /// Gets or sets the space between which each nested layout will be positioned.
    /// </summary>
    /// <value>
    /// This defaults to the Size(20, 20).
    /// </value>
    public Size ArrangementSpacing {
      get { return (Size)GetValue(ArrangementSpacingProperty); }
      set { SetValue(ArrangementSpacingProperty, value); }
    }
  }

  /// <summary>
  /// This enumeration specifies how to position the results of nested layouts of a <see cref="MultiLayout"/>.
  /// </summary>
  public enum MultiArrangement {
    /// <summary>
    /// Just use the natural positioning of each layout in the <see cref="MultiLayout"/>.
    /// </summary>
    None,
    /// <summary>
    /// Position each sublayout in a non-overlapping fashion by increasing Y coordinates,
    /// starting at the <see cref="MultiLayout.ArrangementOrigin"/>.
    /// </summary>
    Vertical,
    /// <summary>
    /// Position each sublayout in a non-overlapping fashion by increasing X coordinates,
    /// starting at the <see cref="MultiLayout.ArrangementOrigin"/>.
    /// </summary>
    Horizontal
  }
}
