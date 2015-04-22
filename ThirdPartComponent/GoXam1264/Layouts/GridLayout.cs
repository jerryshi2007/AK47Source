
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
using System.Globalization;
using System.Linq;
using System.Windows;
using Northwoods.GoXam.Model;

namespace Northwoods.GoXam.Layout {

  /// <summary>
  /// This simple layout places all of the nodes in a grid-like arrangement, ordered,
  /// and wrapping as needed.
  /// </summary>
  /// <remarks>
  /// <para>
  /// By default this layout will sort all of the nodes alphabetically (ignoring case)
  /// and position them left-to-right until they don't fit in the current row,
  /// at which time it starts a new row.
  /// There is a uniform cell size equal to the maximum node width (plus spacing width)
  /// and the maximum node height (plus spacing height).
  /// At least one node is placed in each row, even if the node by itself is wider
  /// than the wrapping width.
  /// The wrapping width is given by the width in model coordinates of the diagram's
  /// panel's viewport width.
  /// </para>
  /// <para>
  /// You can specify values for the <see cref="CellSize"/> <c>Width</c> and/or <c>Height</c>.
  /// If a node is wider than the cell size, it spans more than one cell in the row.
  /// You can also specify a value for the <see cref="WrappingWidth"/>,
  /// which will be used instead of the diagram's viewport width.
  /// </para>
  /// <para>
  /// This layout is sufficiently simple that it does not use a <see cref="GenericNetwork{V, E, Y}"/>.
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
  public class GridLayout : DiagramLayout {
    /// <summary>
    /// Create a <see cref="GridLayout"/>.
    /// </summary>
    public GridLayout() {}

    /// <summary>
    /// Make a copy of a <see cref="GridLayout"/>, copying most of the important properties.
    /// </summary>
    /// <param name="layout"></param>
    public GridLayout(GridLayout layout) : base(layout) {
      if (layout != null) {
        this.WrappingWidth = layout.WrappingWidth;
        this.WrappingColumn = layout.WrappingColumn;
        this.CellSize = layout.CellSize;
        this.Spacing = layout.Spacing;
        this.Alignment = layout.Alignment;
        this.Arrangement = layout.Arrangement;
        this.Comparer = layout.Comparer;
        this.Sorting = layout.Sorting;
      }
    }

    static GridLayout() {
      WrappingWidthProperty = DependencyProperty.Register("WrappingWidth", typeof(double), typeof(GridLayout),
        new FrameworkPropertyMetadata(Double.NaN, OnPropertyChanged));
      WrappingColumnProperty = DependencyProperty.Register("WrappingColumn", typeof(int), typeof(GridLayout),
        new FrameworkPropertyMetadata(0, OnPropertyChanged));
      CellSizeProperty = DependencyProperty.Register("CellSize", typeof(Size), typeof(GridLayout),
        new FrameworkPropertyMetadata(new Size(Double.NaN, Double.NaN), OnPropertyChanged));
      SpacingProperty = DependencyProperty.Register("Spacing", typeof(Size), typeof(GridLayout),
        new FrameworkPropertyMetadata(new Size(10, 10), OnPropertyChanged));
      AlignmentProperty = DependencyProperty.Register("Alignment", typeof(GridAlignment), typeof(GridLayout),
        new FrameworkPropertyMetadata(GridAlignment.Location, OnPropertyChanged));
      ArrangementProperty = DependencyProperty.Register("Arrangement", typeof(GridArrangement), typeof(GridLayout),
        new FrameworkPropertyMetadata(GridArrangement.LeftToRight, OnPropertyChanged));
      ComparerProperty = DependencyProperty.Register("Comparer", typeof(IComparer<Node>), typeof(GridLayout),
        new FrameworkPropertyMetadata(new AlphaComparer(), OnPropertyChanged));
      SortingProperty = DependencyProperty.Register("Sorting", typeof(GridSorting), typeof(GridLayout),
        new FrameworkPropertyMetadata(GridSorting.Ascending, OnPropertyChanged));
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      GridLayout layout = (GridLayout)d;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Position all nodes in the manner of a simple rectangular array.
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="links">these are ignored</param>
    public override void DoLayout(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;

      this.ArrangementOrigin = InitialOrigin(this.ArrangementOrigin);

      HashSet<Node> allnodes = new HashSet<Node>(nodes);  // also gets rid of any duplicates
      foreach (Node n in allnodes.ToList()) {  // use a copy of ALLNODES, because we'll be removing from it
        // make sure all Nodes have already been measured
        n.MaybeRemeasureNow();
        Group g = n as Group;
        if (g != null) {
          List<Node> contained = null;
          foreach (Node m in allnodes) {
            if (m.IsContainedBy(g)) {  // get rid of nodes contained by any other nodes
              if (contained == null) contained = new List<Node>();
              contained.Add(m);
            }
          }
          if (contained != null) foreach (Node c in contained) allnodes.Remove(c);
        }
      }

      // layout unconnected Links
      foreach (Link link in links) {
        if (link.FromNode != null || link.ToNode != null) continue;  // ignore connected links
        Node dummy = new Node();  // use a dummy node
        dummy.Tag = link;  // reference to the Link being laid out
        var shape = new System.Windows.Shapes.Rectangle();
        shape.Width = link.ActualWidth;
        shape.Height = link.ActualHeight;
        dummy.Content = shape;
        dummy.Measure(Geo.Unlimited);
        dummy.Location = new Point(link.Bounds.X, link.Bounds.Y);
        allnodes.Add(dummy);
      }

      // order the nodes, perhaps by sorting
      Node[] nodearray = allnodes.ToArray();
      if (nodearray.Length == 0) return;  // all done!
      switch (this.Sorting) {
        case GridSorting.Forward: break;
        case GridSorting.Reverse: Array.Reverse(nodearray, 0, nodearray.Length); break;
        case GridSorting.Ascending: Array.Sort<Node>(nodearray, this.Comparer); break;
        case GridSorting.Descending: Array.Sort<Node>(nodearray, this.Comparer); Array.Reverse(nodearray, 0, nodearray.Length); break;
      }

      int climit = this.WrappingColumn;
      // calculate the wrapping width, which may depend on the panel's ViewportBounds.Width
      double xlimit = this.WrappingWidth;
      if (Double.IsNaN(xlimit)) {
        Rect viewb = panel.ViewportBounds;
        xlimit = Math.Max(viewb.Width - panel.Padding.Left - panel.Padding.Right, 0);
        //Diagram.Debug(viewb.ToString() + " XLIMIT: " + xlimit.ToString() + " actual: " + panel.ActualWidth.ToString() + " pad: " + panel.Padding.ToString());
      } else {
        xlimit = Math.Max(this.WrappingWidth, 0);
      }
      // gotta have some kind of limit
      if (climit <= 0 && xlimit <= 0) {
        climit = 1;
      }

      // get the minimum space between each node
      double spacex = this.Spacing.Width;
      if (Double.IsNaN(spacex) || Double.IsInfinity(spacex)) spacex = 0;
      double spacey = this.Spacing.Height;
      if (Double.IsNaN(spacey) || Double.IsInfinity(spacey)) spacey = 0;

      switch (this.Alignment) {
        case GridAlignment.Position:
          DoPositionLayout(nodearray, xlimit, climit, spacex, spacey);
          break;
        case GridAlignment.Location:
          DoLocationLayout(nodearray, xlimit, climit, spacex, spacey);
          break;
      }
    }

    private void DoPositionLayout(Node[] nodearray, double xlimit, int climit, double spacex, double spacey) {
      // calculate how wide each cell should be
      double cellW = Math.Max(this.CellSize.Width, 1);
      if (Double.IsNaN(cellW) || Double.IsInfinity(cellW)) {  // unknown -- calculate maximums
        cellW = 0;
        foreach (Node n in nodearray) {
          Size sz = n.GetEffectiveSize(null);
          cellW = Math.Max(cellW, sz.Width);
        }
      }
      cellW += spacex;
      cellW = Math.Max(cellW, 1);

      // calculate how tall each cell should be
      double cellH = Math.Max(this.CellSize.Height, 1);
      if (Double.IsNaN(cellH) || Double.IsInfinity(cellH)) {
        cellH = 0;
        foreach (Node n in nodearray) {
          Size sz = n.GetEffectiveSize(null);
          cellH = Math.Max(cellH, sz.Height);
        }
      }
      cellH += spacey;
      cellH = Math.Max(cellH, 1);

      this.ActualCellSize = new Size(cellW, cellH);

      // iterate through the nodes, placing each one
      GridArrangement arr = this.Arrangement;
      double originx = this.ArrangementOrigin.X;
      double originy = this.ArrangementOrigin.Y;
      double x = originx;
      double y = originy;
      int rowcount = 0;
      double rowheight = 0;
      foreach (Node n in nodearray) {
        // figure out how many cells the node needs
        Size sz = n.GetEffectiveSize(null);
        double w = Math.Ceiling((sz.Width+spacex)/cellW);
        double h = Math.Ceiling((sz.Height+spacey)/cellH);
        Size csz = new Size(w*cellW, h*cellH);

        // maybe need to wrap to next row
        double needed;
        switch (arr) {
          case GridArrangement.RightToLeft: needed = Math.Abs(x-sz.Width) + originx; break;
          default:                          needed = x+sz.Width - originx; break;
        }
        // changed to account for location of origin
        if ((climit > 0 && rowcount > climit - 1) || (xlimit > 0 && rowcount > 0 && needed > xlimit)) {
          rowcount = 0;
          x = originx;
          y += rowheight;
          rowheight = 0;
        }

        rowheight = Math.Max(rowheight, csz.Height);

        // assign new node Position
        double coff;
        switch (arr) {
          case GridArrangement.RightToLeft: coff = -sz.Width; break;
          default:                          coff = 0; break;
        }
        Move(n, new Point(x+coff, y));

        // advance to next column
        switch (arr) {
          case GridArrangement.RightToLeft: x -= csz.Width; break;
          default:                          x += csz.Width; break;
        }
        rowcount++;
      }
    }

    private void DoLocationLayout(Node[] nodearray, double xlimit, int climit, double spacex, double spacey) {
      // calculate how wide each cell should be
      double cellW = Math.Max(this.CellSize.Width, 1);
      double cellL = 0;
      double cellR = 0;
      foreach (Node n in nodearray) {
        Size sz = n.GetEffectiveSize(null);
        // when AlignsLocation, accumulate separate maximums for left of the Location, and for right
        Point relloc = n.GetRelativeElementPoint(n.LocationElement, n.LocationSpot);
        cellL = Math.Max(cellL, relloc.X);
        cellR = Math.Max(cellR, sz.Width-relloc.X);
      }
      // account for space; which side depends on Arrangement
      GridArrangement arr = this.Arrangement;
      switch (arr) {
        case GridArrangement.RightToLeft: cellL += spacex; break;
        default:                          cellR += spacex; break;
      }
      if (Double.IsNaN(cellW) || Double.IsInfinity(cellW)) {  // unknown -- calculate maximums
        cellW = Math.Max(cellL+cellR, 1);
      } else {
        cellW = Math.Max(cellW+spacex, 1);
      }

      // calculate how tall each cell should be
      double cellH = Math.Max(this.CellSize.Height, 1);
      double cellT = 0;
      double cellB = 0;
      foreach (Node n in nodearray) {
        Size sz = n.GetEffectiveSize(null);
        // when AlignsLocation, accumulate separate maximums for above the Location, and for below
        Point relloc = n.GetRelativeElementPoint(n.LocationElement, n.LocationSpot);
        cellT = Math.Max(cellT, relloc.Y);
        cellB = Math.Max(cellB, sz.Height-relloc.Y);
      }
      // assume all space on bottom
      cellB += spacey;
      if (Double.IsNaN(cellH) || Double.IsInfinity(cellH)) {  // unknown -- calculate maximums
        cellH = Math.Max(cellT+cellB, 1);
      } else {
        cellH = Math.Max(cellH+spacey, 1);
      }

      this.ActualCellSize = new Size(cellW, cellH);

      // iterate through the nodes, placing each node's Location at (X,Y)
      double originx = this.ArrangementOrigin.X;
      double originy = this.ArrangementOrigin.Y;
      double x = originx;
      double y = originy;
      int rowcount = 0;
      double rowheight = 0;

      // changed to fix problems with distance betwen rows
      double maxhei = 0;
      foreach (Node n in nodearray) {
        maxhei = Math.Max(maxhei, n.GetEffectiveSize(null).Height);
      }
      double rowhei = Math.Max(maxhei + spacey, cellH);
      //Diagram.Debug("GivenCellSize: " + Diagram.Str(this.CellSize) + "  ActualCellSize: " + Diagram.Str(this.ActualCellSize) + "  rowhei: " + Diagram.Str(rowhei));
      double xpos = 0;
      foreach (Node n in nodearray) {
        // figure out how many cells the node needs
        Size sz = n.GetEffectiveSize(null);
        Point relloc = n.GetRelativeElementPoint(n.LocationElement, n.LocationSpot);
        // assign new node Position
        if (rowcount > 0) {
          switch (arr) {
            case GridArrangement.RightToLeft: x = Math.Floor((x-originx-(sz.Width-relloc.X))/cellW)*cellW+originx; break;
            default: x = Math.Ceiling((x-originx+relloc.X)/cellW)*cellW+originx; break;
          }
        } else {  // rowcount == 0
          switch (arr) {
            case GridArrangement.RightToLeft: xpos = n.Position.X+sz.Width; break;
            default: xpos = n.Position.X; break;
          }
        }
        // maybe need to wrap to next row
        double needed;
        // subtract the stuff sticking out beyond the end of the grid from the XLIMIT
        // changed fix problems with premature wrapping with RTL
        switch (arr) {
          case GridArrangement.RightToLeft: needed = -(x + relloc.X) + originx+xpos; break;
          default: needed = x + sz.Width - relloc.X - originx-xpos; break;
        }

        // check the column limit or the width limit or both
        //Diagram.Debug(xlimit.ToString() + " " + needed.ToString() + " x: " + x.ToString() + " " + Diagram.Str(sz) + " " + relloc.X.ToString() + " " + n.ToString());
        if ((climit > 0 && rowcount > climit-1) || (xlimit > 0 && rowcount > 0 && needed > xlimit)) {
          rowcount = 0;
          x = originx;
          
          // changed to fix problems with row spacing
          y += rowhei;

          rowheight = 0;  // minimum rowheight is the maximum amount above location point
        }
        // consider the height of the node below the location point
        rowheight = Math.Max(rowheight, Math.Ceiling(((sz.Height-relloc.Y)+spacey)/cellH)*cellH);
        // the Position is just the Location minus the RelativeElementPoint
        //Diagram.Debug("gridlayout: " + Diagram.Str(new Point(x-relloc.X, y-relloc.Y)) + n.ToString());
        Move(n, new Point(x-relloc.X, y-relloc.Y));

        // changed to account for space
        // advance to next column
        switch (arr) {
          case GridArrangement.RightToLeft: x -= relloc.X+spacex; break;
          default:                          x += sz.Width-relloc.X+spacex; break;
        }
        rowcount++;
      }
    }


    // handle unconnected Links that use dummy Nodes as placeholders
    private void Move(Node n, Point pos) {
      if (n.Tag is Link) {
        Link l = (Link)n.Tag;
        Rect lb = Geo.Bounds(l.Route.Points);
        l.Route.MovePoints(new Point(pos.X-lb.X, pos.Y-lb.Y));
        l.Remeasure();
      } else {
        n.Move(pos, true);
      }
    }


    sealed internal class AlphaComparer : IComparer<Node> {  // nested class
      public int Compare(Node a, Node b) {
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
      }
    }



    /// <summary>
    /// Identifies the <see cref="WrappingWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WrappingWidthProperty;
    /// <summary>
    /// Gets or sets the wrapping width.
    /// </summary>
    /// <value>
    /// The default value is <c>Double.NaN</c>, meaning to use the
    /// width of the diagram's panel's viewport.
    /// Any real value must be larger than zero.
    /// </value>
    public double WrappingWidth {
      get { return (double)GetValue(WrappingWidthProperty); }
      set { SetValue(WrappingWidthProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="WrappingColumn"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WrappingColumnProperty;
    /// <summary>
    /// Gets or sets the maximum number of columns.
    /// </summary>
    /// <value>
    /// The default value is zero, meaning not to limit the number of columns.
    /// 1 is a common value.
    /// </value>
    public int WrappingColumn {
      get { return (int)GetValue(WrappingColumnProperty); }
      set { SetValue(WrappingColumnProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="CellSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CellSizeProperty;
    /// <summary>
    /// Gets or sets the minimum node size by which each node is positioned in the grid.
    /// </summary>
    /// <value>
    /// The default value is NaN x NaN.
    /// The units are in model coordinates.
    /// When the width is <c>Double.NaN</c>,
    /// the <see cref="ActualCellSize"/> uses the maximum of all node widths
    /// plus the <see cref="Spacing"/> width.
    /// When the height is <c>Double.NaN</c>,
    /// the <see cref="ActualCellSize"/> uses the maximum of all node heights,
    /// plus the <see cref="Spacing"/> height.
    /// </value>
    /// <remarks>
    /// When the cell size is smaller than a node, the node will occupy more than one cell.
    /// This allows nodes to be positioned closer to each other, but then variations in node
    /// sizes may cause them not to be aligned in perfect rows or columns.
    /// </remarks>

    [TypeConverter(typeof(Northwoods.GoXam.Route.SizeConverter))]

    public Size CellSize {
      get { return (Size)GetValue(CellSizeProperty); }
      set { SetValue(CellSizeProperty, value); }
    }


    /// <summary>
    /// Gets the actual cell size used by the grid.
    /// </summary>
    /// <value>
    /// This is set by each layout, as computed by taking the value of
    /// <see cref="CellSize"/> and perhaps considering the nodes being laid out,
    /// and adding the <see cref="Spacing"/>.
    /// </value>
    public Size ActualCellSize { get; protected set; }


    /// <summary>
    /// Identifies the <see cref="Spacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SpacingProperty;
    /// <summary>
    /// Gets or sets the minimum horizontal and vertical space between nodes.
    /// </summary>
    /// <value>
    /// The default value is 10x10.
    /// The units are in model coordinates.
    /// </value>
    public Size Spacing {
      get { return (Size)GetValue(SpacingProperty); }
      set { SetValue(SpacingProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="Alignment"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AlignmentProperty;
    /// <summary>
    /// Gets or sets whether the <see cref="Node.Location"/> or the <see cref="Node.Position"/> should be used
    /// to arrange each node.
    /// </summary>
    /// <value>
    /// The default value is <see cref="GridAlignment.Location"/> -- the <see cref="Node.Location"/>s will be aligned in a grid.
    /// </value>
    public GridAlignment Alignment {
      get { return (GridAlignment)GetValue(AlignmentProperty); }
      set { SetValue(AlignmentProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="Arrangement"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ArrangementProperty;
    /// <summary>
    /// Gets or sets how to arrange the nodes.
    /// </summary>
    /// <value>
    /// The default value is <see cref="GridArrangement.LeftToRight"/>.
    /// </value>
    public GridArrangement Arrangement {
      get { return (GridArrangement)GetValue(ArrangementProperty); }
      set { SetValue(ArrangementProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="Comparer"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ComparerProperty;
    /// <summary>
    /// Gets or sets the comparison function used to sort the nodes.
    /// </summary>
    /// <value>
    /// The default value is a case-insensitive alphabetic comparison
    /// using the <see cref="Part.Text"/> (attached) property of each node.
    /// </value>
    public IComparer<Node> Comparer {
      get { return (IComparer<Node>)GetValue(ComparerProperty); }
      set { SetValue(ComparerProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="Sorting"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SortingProperty;
    /// <summary>
    /// Gets or sets what order to place the nodes.
    /// </summary>
    /// <value>
    /// The default value is <see cref="GridSorting.Ascending"/>.
    /// </value>
    public GridSorting Sorting {
      get { return (GridSorting)GetValue(SortingProperty); }
      set { SetValue(SortingProperty, value); }
    }
  }

  /// <summary>
  /// This enumeration specifies whether to position each node by
  /// its <see cref="Node.Position"/> or by its <see cref="Node.Location"/>.
  /// </summary>
  public enum GridAlignment {
    /// <summary>
    /// Position the top-left corner of each node at a grid point.
    /// </summary>
    Position,
    /// <summary>
    /// Position the node's <see cref="Node.Location"/> at a grid point.
    /// </summary>
    Location
  }

  /// <summary>
  /// This enumeration specifies how to fill each row.
  /// </summary>
  public enum GridArrangement {
    /// <summary>
    /// Fill each row from left to right.
    /// </summary>
    LeftToRight,
    /// <summary>
    /// Fill each row from right to left.
    /// </summary>
    RightToLeft

    //?? TopToBottom, BottomToTop
  }

  /// <summary>
  /// This enumeration specifies whether to sort all of the nodes,
  /// and in what order to position them.
  /// </summary>
  /// <remarks>
  /// The default <see cref="GridLayout.Comparer"/> does a case-insensitive comparison
  /// of the value of each node's <see cref="Part.Text"/> property.
  /// </remarks>
  public enum GridSorting {
    /// <summary>
    /// Lay out each item in the order in which the nodes were given.
    /// </summary>
    Forward,
    /// <summary>
    /// Lay out each item in reverse order from which the nodes were given.
    /// </summary>
    Reverse,
    /// <summary>
    /// Lay out each item according to the sort order given by <see cref="GridLayout.Comparer"/>.
    /// </summary>
    Ascending,
    /// <summary>
    /// Lay out each item in reverse sort order given by <see cref="GridLayout.Comparer"/>.
    /// </summary>
    Descending
  }
}
