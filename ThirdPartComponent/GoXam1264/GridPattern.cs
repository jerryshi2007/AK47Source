
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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Northwoods.GoXam {
  /// <summary>
  /// This panel is used to draw regular grid patterns.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The typical <see cref="GridPattern"/> will have pairs of <c>Path</c>s as child elements.
  /// Each <c>Path</c> will have the attached property <see cref="FigureProperty"/> set to indicate
  /// whether the path should be a line or a bar, and whether the path has repeated horizontal
  /// or vertical segments.
  /// </para>
  /// <para>
  /// The <see cref="CellSize"/> property controls the size of each cell.
  /// </para>
  /// <para>
  /// A simple grid is defined by:
  /// <code>
  ///   &lt;go:GridPattern CellSize="10 10" Width="200" Height="200"&gt;
  ///     &lt;Path Stroke="LightGray" StrokeThickness="1" go:GridPattern.Figure="HorizontalLine" /&gt;
  ///     &lt;Path Stroke="LightGray" StrokeThickness="1" go:GridPattern.Figure="VerticalLine" /&gt;
  ///   &lt;/go:GridPattern&gt;
  /// </code>
  /// </para>
  /// <para>
  /// You can also have repeating variations of the lines by having multiple <c>Path</c>s,
  /// where those after the first one in each direction have the <see cref="IntervalProperty"/> attached
  /// property to set integer values larger than one.
  /// The interval describes how often that path should be drawn.
  /// Note that the interval is an integer not a double because it is a multiple.
  /// So in the follow example, once every five lines there is a Gray line instead of a LightGray line --
  /// in other words every 10 model units the line is LightGray,
  /// except that every 50 model units the line is Gray.
  /// <code>
  ///   &lt;go:GridPattern CellSize="10 10" Width="200" Height="200"&gt;
  ///     &lt;Path Stroke="LightGray" StrokeThickness="1" go:GridPattern.Figure="HorizontalLine" /&gt;
  ///     &lt;Path Stroke="LightGray" StrokeThickness="1" go:GridPattern.Figure="VerticalLine" /&gt;
  ///     &lt;Path Stroke="Gray" StrokeThickness="1" go:GridPattern.Figure="HorizontalLine" go:GridPattern.Interval="5" /&gt;
  ///     &lt;Path Stroke="Gray" StrokeThickness="1" go:GridPattern.Figure="VerticalLine" go:GridPattern.Interval="5" /&gt;
  ///   &lt;/go:GridPattern&gt;
  /// </code>
  /// </para>
  /// <para>
  /// You can have alternating green bars by:
  /// <code>
  ///   &lt;go:GridPattern CellSize="50 50" Width="200" Height="200"&gt;
  ///     &lt;Path Fill="LightGreen" go:GridPattern.Figure="HorizontalBar" go:GridPattern.Interval="2" /&gt;
  ///   &lt;/go:GridPattern&gt;
  /// </code>
  /// </para>
  /// <para>
  /// A <c>GridPattern</c> may be the value of a diagram's <see cref="Northwoods.GoXam.Diagram.GridPattern"/> property.
  /// You will need to set <see cref="Northwoods.GoXam.Diagram.GridVisible"/> to true.
  /// Such a <c>GridPattern</c> is automatically arranged to occupy the whole <see cref="DiagramPanel"/>'s viewport;
  /// so you should not set its <b>Width</b> or <b>Height</b>.
  /// The <c>GridPattern</c> may be supplied as a property element value in XAML or as a <c>DataTemplate</c>
  /// specified as the diagram's <see cref="Northwoods.GoXam.Diagram.GridPatternTemplate"/>.
  /// If no <see cref="Northwoods.GoXam.Diagram.GridPattern"/> property and no 
  /// <see cref="Northwoods.GoXam.Diagram.GridPatternTemplate"/> are supplied,
  /// it will use a default template.
  /// </para>
  /// <para>
  /// You should not apply any transformations to the <c>Path</c>s; that is reserved for GoXam's use.
  /// If the <c>GridPattern</c> is the whole diagram's <see cref="Northwoods.GoXam.Diagram.GridPattern"/>,
  /// you should not apply any transformations to it at all.
  /// </para>
  /// </remarks>
  public class GridPattern : Panel {
    /// <summary>
    /// Create an empty <see cref="GridPattern"/> panel.
    /// </summary>
    public GridPattern() {
      this.HorizontalAlignment = HorizontalAlignment.Left;
      this.VerticalAlignment = VerticalAlignment.Top;
    }

    static GridPattern() {
      // Dependency properties
      CellSizeProperty = DependencyProperty.Register("CellSize", typeof(Size), typeof(GridPattern),
        new FrameworkPropertyMetadata(new Size(10, 10), OnCellSizeChanged));
      OriginProperty = DependencyProperty.Register("Origin", typeof(Point), typeof(GridPattern),
        new FrameworkPropertyMetadata(new Point(0, 0), OnOriginChanged));

      // Attached dependency properties
      FigureProperty = DependencyProperty.RegisterAttached("Figure", typeof(GridFigure), typeof(GridPattern),
        new FrameworkPropertyMetadata(GridFigure.None, OnFigureChanged));
      IntervalProperty = DependencyProperty.RegisterAttached("Interval", typeof(int), typeof(GridPattern),
        new FrameworkPropertyMetadata(1, OnIntervalChanged));
      OffsetProperty = DependencyProperty.RegisterAttached("Offset", typeof(int), typeof(GridPattern),
        new FrameworkPropertyMetadata(0, OnOffsetChanged));
      BarThicknessProperty = DependencyProperty.RegisterAttached("BarThickness", typeof(int), typeof(GridPattern),
        new FrameworkPropertyMetadata(1, OnBarThicknessChanged));
      CrossLengthProperty = DependencyProperty.RegisterAttached("CrossLength", typeof(double), typeof(GridPattern),
        new FrameworkPropertyMetadata(6.0, OnCrossLengthChanged));
    }


    /// <summary>
    /// Identifies the <see cref="CellSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CellSizeProperty;
    /// <summary>
    /// Gets or sets the size of each grid cell.
    /// </summary>
    /// <value>
    /// The default value is 10x10.
    /// </value>
    public Size CellSize {
      get { return (Size)GetValue(CellSizeProperty); }
      set { SetValue(CellSizeProperty, value); }
    }
    private static void OnCellSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Size sz = (Size)e.NewValue;
      if (sz.Width <= 0 || sz.Height <= 0 || Double.IsNaN(sz.Width) || Double.IsNaN(sz.Height) || Double.IsInfinity(sz.Width) || Double.IsInfinity(sz.Height)) {
        Diagram.Error("New Size value for CellSize must have positive non-infinite dimensions, preferably greater than one, not: " + sz.ToString());
      }
      GridChanged(d);
    }

    /// <summary>
    /// Identifies the <see cref="Origin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OriginProperty;
    /// <summary>
    /// Gets or sets the snapping grid's coordinates starting point.
    /// </summary>
    /// <value>
    /// The default value is <c>Point(0, 0)</c>.
    /// Both X and Y values must be non-infinite numbers.
    /// </value>
    public Point Origin {
      get { return (Point)GetValue(OriginProperty); }
      set { SetValue(OriginProperty, value); }
    }
    private static void OnOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Point pt = (Point)e.NewValue;
      if (Double.IsNaN(pt.X) || Double.IsNaN(pt.Y) || Double.IsInfinity(pt.X) || Double.IsInfinity(pt.Y)) {
        Diagram.Error("New Point value for Origin must have non-infinite numbers, not: " + pt.ToString());
      }
      GridChanged(d);
    }


    /// <summary>
    /// Identifies the <c>Figure</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty FigureProperty;
    /// <summary>
    /// Gets the <see cref="GridFigure"/> of the <c>Path</c> that describes its appearance.
    /// </summary>
    /// <param name="d">a <c>Path</c></param>
    /// <returns>This defaults to <see cref="GridFigure.None"/></returns>
    public static GridFigure GetFigure(DependencyObject d) { return (GridFigure)d.GetValue(FigureProperty); }
    /// <summary>
    /// Sets the <see cref="GridFigure"/> of the <c>Path</c> to determine its appearance.
    /// </summary>
    /// <param name="d">a <c>Path</c></param>
    /// <param name="v">
    /// a <see cref="GridFigure"/>; the panel assumes <see cref="GridFigure.None"/> otherwise
    /// </param>
    public static void SetFigure(DependencyObject d, GridFigure v) { d.SetValue(FigureProperty, v); }
    private static void OnFigureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      GridChanged(d);
    }

    /// <summary>
    /// Identifies the <c>Interval</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty IntervalProperty;
    /// <summary>
    /// Gets how far apart the lines (or the starts of the bar edges) should be drawn, in multiples of the cell size.
    /// </summary>
    /// <param name="d">a <c>Path</c></param>
    /// <returns>This defaults to 1, a single cell width or height, depending on the value of <see cref="GetFigure"/>.</returns>
    public static int GetInterval(DependencyObject d) { return (int)d.GetValue(IntervalProperty); }
    /// <summary>
    /// Sets how far apart the lines (or the starts of the bar edges) should be drawn.
    /// </summary>
    /// <param name="d">a <c>Path</c></param>
    /// <param name="v">
    /// a positive integer, in units of the cell width or height, depending on the direction given by <see cref="GetFigure"/>
    /// </param>
    public static void SetInterval(DependencyObject d, int v) { d.SetValue(IntervalProperty, v); }
    private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      int intvl = (int)e.NewValue;
      if (intvl <= 0) {
        Diagram.Error("New integer value for Interval attached property must be positive, not: " + intvl.ToString(System.Globalization.CultureInfo.CurrentCulture));
      }
      GridChanged(d);
    }

    /// <summary>
    /// Identifies the <c>Offset</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty OffsetProperty;
    /// <summary>
    /// Gets the offset for starting the count at which <c>Path</c>s should be drawn, in multiples of the cell size.
    /// </summary>
    /// <param name="d">a <c>Path</c></param>
    /// <returns>This defaults to zero.</returns>
    public static int GetOffset(DependencyObject d) { return (int)d.GetValue(OffsetProperty); }
    /// <summary>
    /// Sets the starting offset at which the given <c>Path</c> is drawn, in multiples of the cell size.
    /// </summary>
    /// <param name="d">a <c>Path</c></param>
    /// <param name="v">
    /// a positive integer, in units of the cell width or height, depending on the direction given by <see cref="GetFigure"/>
    /// </param>
    public static void SetOffset(DependencyObject d, int v) { d.SetValue(OffsetProperty, v); }
    private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      int intvl = (int)e.NewValue;
      if (intvl <= 0) {
        Diagram.Error("New integer value for Offset attached property must be positive, not: " + intvl.ToString(System.Globalization.CultureInfo.CurrentCulture));
      }
      GridChanged(d);
    }

    /// <summary>
    /// Identifies the <c>BarThickness</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty BarThicknessProperty;
    /// <summary>
    /// Gets the thickness of horizontal or vertical bars, in multiples of the cell size.
    /// </summary>
    /// <param name="d">a <c>Path</c></param>
    /// <returns>This defaults to 1, a single cell width or height, depending on the <see cref="GetFigure"/>.</returns>
    public static int GetBarThickness(DependencyObject d) { return (int)d.GetValue(BarThicknessProperty); }
    /// <summary>
    /// Sets the thickness of bars, in multiples of the cell size.
    /// </summary>
    /// <param name="d">a <c>Path</c></param>
    /// <param name="v">
    /// a positive integer, in units of the cell width or height, depending on the direction given by <see cref="GetFigure"/>
    /// </param>
    public static void SetBarThickness(DependencyObject d, int v) { d.SetValue(BarThicknessProperty, v); }
    private static void OnBarThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      int intvl = (int)e.NewValue;
      if (intvl <= 0) {
        Diagram.Error("New integer value for BarThickness attached property must be positive, not: " + intvl.ToString(System.Globalization.CultureInfo.CurrentCulture));
      }
      GridChanged(d);
    }

    /// <summary>
    /// Identifies the <c>CrossLength</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty CrossLengthProperty;
    /// <summary>
    /// Gets the length of a <see cref="GridFigure.HorizontalCross"/> or <see cref="GridFigure.VerticalCross"/> line.
    /// </summary>
    /// <param name="d">a <c>Path</c></param>
    /// <returns>This defaults to 6.</returns>
    public static double GetCrossLength(DependencyObject d) { return (double)d.GetValue(CrossLengthProperty); }
    /// <summary>
    /// Sets the length of a <see cref="GridFigure.HorizontalCross"/> or <see cref="GridFigure.VerticalCross"/> line.
    /// </summary>
    /// <param name="d">a <c>Path</c></param>
    /// <param name="v">
    /// a positive number, in model coordinates; the actual length of the line is limited by the <see cref="CellSize"/>.
    /// </param>
    public static void SetCrossLength(DependencyObject d, double v) { d.SetValue(CrossLengthProperty, v); }
    private static void OnCrossLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      double len = (double)e.NewValue;
      if (len <= 0) {
        Diagram.Error("New double value for CrossLength attached property must be positive, not: " + len.ToString(System.Globalization.CultureInfo.CurrentCulture));
      }
      GridChanged(d);
    }

    private static void GridChanged(DependencyObject d) {
      GridPattern g = d as GridPattern;
      if (g == null) g = Diagram.FindParent<GridPattern>(d);
      if (g != null) {
        g.InvalidateGrid();
        Part part = Part.FindAncestor<Part>(g);
        if (part != null) {
          part.InvalidateVisual(g);
        } else {



          g.InvalidateMeasure();
          //Diagram.Debug("GridChanged (not in Part) -- InvalidateMeasure");
        }
      }
    }


    private class Interval {
      public int Offset { get; set; }
      public int Spacing { get; set; }
    }

    private List<List<Interval>> CollectIntervals() {
      List<Path> paths = new List<Path>();
      foreach (UIElement e in this.Children) {
        paths.Add(e as Path);  // include null when not a Path
      }
      List<List<Interval>> list = new List<List<Interval>>();
      for (int i = 0; i < paths.Count; i++) {
        Path p = paths[i];
        List<Interval> ints = new List<Interval>();
        list.Add(ints);
        if (p == null) continue;
        GridFigure fig = GetFigure(p);
        if (fig == GridFigure.None) continue;
        bool vert = IsVertical(fig);
        for (int j = i+1; j < paths.Count; j++) {
          Path q = paths[j];
          if (q == null) continue;
          GridFigure f = GetFigure(q);
          if (f == GridFigure.None || IsVertical(f) != vert) continue;
          int v = GetInterval(q);
          int off = GetOffset(q);
          if (v >= 2) ints.Add(new Interval() { Spacing=v, Offset=off });
        }
      }
      return list;
    }

    // see if the Nth line should be drawn, considering both its frequency
    // and whether any later (in Z order) lines would be drawn at this point
    private bool ShouldDraw(int i, int freq, int off, List<Interval> ints) {
      // negative numerators don't matter when comparing with zero
      // assumes FREQ > 0
      if ((i-off)%freq != 0) return false;
      // assumes fs.Intvl > 0
      return !ints.Any(fs => (i-fs.Offset)%fs.Spacing == 0);
    }

    private bool IsVertical(GridFigure fig) {
      return (fig != GridFigure.None && ((int)fig & 1) == 1);
    }

    private Geometry GetDefiningGeometry(GridFigure fig, double gw, double gh, int freq, int off, List<Interval> ints, int barmult) {
      double scale = this.RenderedScale;
      if (scale <= 0) scale = 1;
      Size csz = this.CellSize;
      double cw = csz.Width;
      double ch = csz.Height;
      int numx = (int)Math.Ceiling(gw/cw);
      int numy = (int)Math.Ceiling(gh/ch);
      Point cpt = this.Origin;
      GeoStream sg = new GeoStream();
      using (StreamGeometryContext context = sg.Open()) {
        if (fig == GridFigure.None) {
          context.BeginFigure(new Point(0, 0), true, true);
          return sg.Geometry;
        } else if (IsVertical(fig)) {  // vertical
          if (fig == GridFigure.VerticalBar) {
            // vertical bars
            int start = (int)Math.Floor(-cpt.X/cw);
            int i = start;
            for (; i < start+numx; i++) {
              double x = i*cw+cpt.X;
              double x2 = i*cw+cpt.X+barmult*cw;
              if (0 < x2 && x < gw && ShouldDraw(i, freq, off, ints)) {
                x = Math.Max(x, 0);
                x2 = Math.Min(x2, gw);
                context.BeginFigure(new Point(x, 0), true, true);
                context.LineTo(new Point(x, gh), true, false);
                context.LineTo(new Point(x2, gh), true, true);
                context.LineTo(new Point(x2, 0), true, true);
                if (cw*freq*scale < 2) break;
              }
            }
          } else {
            // vertical lines
            int start = (int)Math.Floor(-cpt.X/cw);
            for (int i = start; i <= start+numx; i++) {
              double x = i*cw+cpt.X;
              if (0 <= x && x <= gw && ShouldDraw(i, freq, off, ints)) {
                context.BeginFigure(new Point(x, 0), false, false);
                context.LineTo(new Point(x, gh), true, false);
                if (cw*freq*scale < 2) break;
              }
            }
          }
        } else {
          if (fig == GridFigure.HorizontalBar) {
            // horizontal bars
            int start = (int)Math.Floor(-cpt.Y/ch);
            int i = start;
            for (; i < start+numy; i++) {
              double y = i*ch+cpt.Y;
              double y2 = i*ch+cpt.Y+barmult*ch;
              if (0 < y2 && y < gh && ShouldDraw(i, freq, off, ints)) {
                y = Math.Max(y, 0);
                y2 = Math.Min(y2, gh);
                context.BeginFigure(new Point(0, y), true, true);
                context.LineTo(new Point(gw, y), true, false);
                context.LineTo(new Point(gw, y2), true, false);
                context.LineTo(new Point(0, y2), true, false);
                if (ch*freq*scale < 2) break;
              }
            }
          } else {
            // horizontal lines
            int start = (int)Math.Floor(-cpt.Y/ch);
            for (int i = start; i <= start+numy; i++) {
              double y = i*ch+cpt.Y;
              if (0 <= y && y <= gh && ShouldDraw(i, freq, off, ints)) {
                context.BeginFigure(new Point(0, y), false, false);
                context.LineTo(new Point(gw, y), true, false);
                if (ch*freq*scale < 2) break;  //??? NYI automatically calling InvalidateVisual on each GridPattern when the DiagramPanel.Scale changes
              }
            }
          }
        }
        return sg.Geometry;
      }
    }

    /// <summary>
    /// Supply the geometries for child <c>Path</c> elements that have a
    /// <see cref="GetFigure"/> that is not <see cref="GridFigure.None"/>,
    /// and measure all children.
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns></returns>
    protected override Size MeasureOverride(Size availableSize) {
      //DateTime before = DateTime.Now;
      double w = availableSize.Width;
      if (Double.IsPositiveInfinity(w) || Double.IsNaN(w)) w = this.Width;
      if (Double.IsPositiveInfinity(w) || Double.IsNaN(w)) w = this.MinWidth;
      if (Double.IsPositiveInfinity(w) || Double.IsNaN(w)) w = this.MaxWidth;
      if (Double.IsPositiveInfinity(w) || Double.IsNaN(w)) w = 0;
      double h = availableSize.Height;
      if (Double.IsPositiveInfinity(h) || Double.IsNaN(h)) h = this.Height;
      if (Double.IsPositiveInfinity(h) || Double.IsNaN(h)) h = this.MinHeight;
      if (Double.IsPositiveInfinity(h) || Double.IsNaN(h)) h = this.MaxHeight;
      if (Double.IsPositiveInfinity(h) || Double.IsNaN(h)) h = 0;
      //Diagram.Debug("GridPattern.Measure: " + Diagram.Str(availableSize) + Diagram.Str(w) + "x" + Diagram.Str(h));

      List<List<Interval>> ints = this.GridIntervals;
      bool recompute = (ints == null || this.GeometrySize.Width != w || this.GeometrySize.Height != h);
      if (recompute) ints = CollectIntervals();  // if no Intervals were cached, or if we need newly sized PathGeometries, compute them
      for (int i = 0; i < this.Children.Count; i++) {
        UIElement e = this.Children[i];
        // if there is a cached GridIntervals, assume the PathGeometries are up-to-date
        if (recompute) {
          Path path = e as Path;
          if (path != null) {
            GridFigure fig = GetFigure(path);
            int freq = GetInterval(path);
            int off = GetOffset(path);
            int bar = GetBarThickness(path);
            if (fig != GridFigure.None && freq >= 1 && bar >= 1) {
              path.Visibility = Visibility.Visible;
              path.Data = GetDefiningGeometry(fig, w, h, freq, off, ints[i], bar);
              if (fig == GridFigure.HorizontalDot || fig == GridFigure.VerticalDot) {
                path.StrokeDashCap = PenLineCap.Round;
                Size csz = this.CellSize;
                Point orig = this.Origin;
                double st = path.StrokeThickness;
                if (st <= 0) {
                  st = 1;
                  path.StrokeThickness = st;
                }
                if (fig == GridFigure.HorizontalDot) {
                  path.StrokeDashArray = new DoubleCollection() { 0, csz.Width/st };
                  path.StrokeDashOffset = -orig.X/st;
                } else if (fig == GridFigure.VerticalDot) {
                  path.StrokeDashArray = new DoubleCollection() { 0, csz.Height/st };
                  path.StrokeDashOffset = -orig.Y/st;
                }
              } else if (fig == GridFigure.HorizontalCross || fig == GridFigure.VerticalCross) {
                  Size csz = this.CellSize;
                  Point orig = this.Origin;
                  double st = path.StrokeThickness;
                  if (st <= 0) {
                    st = 1;
                    path.StrokeThickness = st;
                  }
                  double len = GetCrossLength(path);
                  double halfdash = len/(st*2);
                  if (fig == GridFigure.HorizontalCross) {
                    if (csz.Width > len) {
                      path.StrokeDashArray = new DoubleCollection() { halfdash, (csz.Width-len)/st, halfdash, 0 };
                      path.StrokeDashOffset = -orig.X/st;
                    }
                  } else if (fig == GridFigure.VerticalCross) {
                    if (csz.Height > len) {
                      path.StrokeDashArray = new DoubleCollection() { halfdash, (csz.Height-len)/st, halfdash, 0 };
                      path.StrokeDashOffset = -orig.Y/st;
                    }
                  }
              }
            } else {
              path.Visibility = Visibility.Collapsed;
            }
          }
        }
        e.Measure(availableSize);
      }
      if (recompute) this.GridIntervals = ints;  // if no Intervals cached, save them
      this.GeometrySize = new Size(w, h);

      //Diagram.Debug("GridPattern measure: " + (DateTime.Now-before).TotalMilliseconds.ToString());
      return new Size(w, h);
    }

    /// <summary>
    /// Arrange all of the child elements.
    /// </summary>
    /// <param name="finalSize"></param>
    /// <returns></returns>
    protected override Size ArrangeOverride(Size finalSize) {
      //Diagram.Debug("GridPattern.Arrange " + Diagram.Str(finalSize));
      foreach (UIElement e in this.Children) {
        e.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
      }
      return finalSize;
    }







































    // when used as the Diagram's GridPattern background

    internal void InvokeBackgroundGridUpdate(DiagramPanel panel) {
      if (this.Timer != null) {
        this.Timer.Stop();
        this.Timer = null;
      }
      if (!this.UpdateBackgroundGridNeeded) {
        this.UpdateBackgroundGridNeeded = true;
        Diagram.InvokeLater(this, () => QueueBackgroundGridUpdate(panel));
      }
    }

    private void QueueBackgroundGridUpdate(DiagramPanel panel) {
      this.UpdateBackgroundGridNeeded = false;
      if (ShouldCacheBitmap(panel)) {
        // stop any current timer
        if (this.Timer != null) this.Timer.Stop();
        // start a new timer
        this.Timer = new System.Windows.Threading.DispatcherTimer();
        this.Timer.Tick += (s, e) => {
          var tim = s as System.Windows.Threading.DispatcherTimer;
          if (tim != null) tim.Stop();  // don't repeat
          this.Timer = null;
          DoUpdateBackgroundGrid(panel, panel.ViewportBounds, panel.Scale, true);
        };
        // wait half second before calling DoUpdateBackgroundGrid
        this.Timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
        this.Timer.Start();
      } else {  // just call DoUpdateBackgroundGrid right now
        DoUpdateBackgroundGrid(panel, panel.ViewportBounds, panel.Scale, true);
      }
    }

    //?? ought to decide based on complexity of drawing the grid, not the size
    // currently just checks for > 1 megapixel
    private bool ShouldCacheBitmap(DiagramPanel panel) {
      return panel.CachesBitmap && panel.ViewportWidth*panel.ViewportHeight > 1000*1000;
    }

    internal void CancelUpdate() {
      if (this.Timer != null) {
        this.Timer.Stop();
        this.Timer = null;
      }
    }

    private bool UpdateBackgroundGridNeeded { get; set; }
    private System.Windows.Threading.DispatcherTimer Timer { get; set; }

    private Size GeometrySize { get; set; }
    private List<List<Interval>> GridIntervals { get; set; }
    private Rect RenderedViewport { get; set; }
    private double RenderedScale { get; set; }

    internal void InvalidateGrid() {
      this.GeometrySize = Size.Empty;
      this.GridIntervals = null;
      this.RenderedViewport = Rect.Empty;
      this.RenderedScale = 0;





    }

    internal void DoUpdateBackgroundGrid(DiagramPanel panel, Rect bounds, double scale, bool delayed) {
      Diagram diagram = panel.Diagram;
      if (diagram == null) return;
      if (!diagram.GridVisible || diagram.GridPattern != this) return;

      if (this.RenderedViewport == bounds && this.RenderedScale == scale) return;
      // pretend to scroll the grid appropriately
      Point origin = diagram.GridSnapOrigin;
      this.Origin = new Point(origin.X-bounds.X, origin.Y-bounds.Y);

      // remember to avoid duplicate renderings
      this.RenderedViewport = bounds;
      this.RenderedScale = scale;  // also used by GetDefiningGeometry

      // size it to fill the viewport
      Size gridsize = new Size(bounds.Width, bounds.Height);
      this.Width = gridsize.Width;
      this.Height = gridsize.Height;
      //Diagram.Debug("Grid: " + Diagram.Str(bounds) + scale.ToString() + " " + Diagram.Str(gridsize));

      // scale this GridPattern
      this.RenderTransformOrigin = new Point(0, 0);
      this.RenderTransform = new ScaleTransform() { CenterX=0, CenterY=0, ScaleX=scale, ScaleY=scale };

      // get it to update
      InvalidateMeasure();
      //panel.InvalidateMeasure();
      if (!delayed || !panel.CachesBitmap) {
        panel.ClearBackground();
        // before rendering, make sure the grid lines have been updated
        Measure(gridsize);
        Arrange(new Rect(0, 0, gridsize.Width, gridsize.Height));
        //Diagram.Debug("Grid arranged: " + Diagram.Str(bounds) + scale.ToString() + " " + Diagram.Str(gridsize) + "actual: " + Diagram.Str(this.ActualWidth) + "x" + Diagram.Str(this.ActualHeight));
      } else {
        //Diagram.Debug("Grid invalidated measure");
      }



































    }

    internal void TransformBrush(DiagramPanel panel) {

















    }

    internal void PrepareForPrinting(DiagramPanel panel, Rect viewb, double scale) {
      this.Visibility = Visibility.Visible;
      DoUpdateBackgroundGrid(panel, viewb, scale, false);  // false: no bitmap cache update needed
    }






  }

  /// <summary>
  /// This enumeration describes how <c>Path</c> elements that are children
  /// of a <see cref="GridPattern"/> may be used to draw a regular grid.
  /// </summary>
  public enum GridFigure {
    /// <summary>
    /// The default value -- not used by <see cref="GridPattern"/> as a regular grid line or bar.
    /// </summary>
    None = 0,
    /// <summary>
    /// Draw regular horizontal lines; supply "Stroke..." properties to the <c>Path</c>.
    /// </summary>
    HorizontalLine = 2,
    /// <summary>
    /// Draw regular vertical lines; supply "Stroke..." properties to the <c>Path</c>.
    /// </summary>
    VerticalLine = 3,
    /// <summary>
    /// Draw regular horizontal bars; supply the "Fill" property to the <c>Path</c>.
    /// The brush fill applies to the whole <see cref="GridPattern"/>, not to each bar.
    /// Control the breadth of each bar in units of the cell size
    /// by setting the attached <see cref="GridPattern.BarThicknessProperty"/>.
    /// </summary>
    HorizontalBar = 4,
    /// <summary>
    /// Draw regular vertical bars; supply the "Fill" property to the <c>Path</c>.
    /// The brush fill applies to the whole <see cref="GridPattern"/>, not to each bar.
    /// Control the breadth of each bar in units of the cell size
    /// by setting the attached <see cref="GridPattern.BarThicknessProperty"/>.
    /// </summary>
    VerticalBar = 5,
    /// <summary>
    /// Draw regular dots along horizontal lines at the grid points.
    /// You need to supply the <b>Stroke</b> brush property to give each dot a color.
    /// The <b>StrokeThickness</b> specifies the size of each dot.
    /// If you do not specify a non-zero <b>StrokeThickness</b>, it will be set to 1.
    /// <see cref="GridPattern"/> will automatically set the other "Stroke..." properties.
    /// </summary>
    HorizontalDot = 6,
    /// <summary>
    /// Draw regular dots along vertical lines at the grid points.
    /// You need to supply the <b>Stroke</b> brush property to give each dot a color.
    /// The <b>StrokeThickness</b> specifies the size of each dot.
    /// If you do not specify a non-zero <b>StrokeThickness</b>, it will be set to 1.
    /// <see cref="GridPattern"/> will automatically set the other "Stroke..." properties.
    /// </summary>
    VerticalDot = 7,
    /// <summary>
    /// Draw horizontal lines for crosses at the grid points.
    /// You need to supply the <b>Stroke</b> brush property to give each line a color.
    /// If you do not specify a non-zero <b>StrokeThickness</b>, it will be set to 1.
    /// <see cref="GridPattern"/> will automatically set the other "Stroke..." properties.
    /// Control the length of each dash by setting the <see cref="GridPattern.CrossLengthProperty"/>.
    /// </summary>
    HorizontalCross=8,
    /// <summary>
    /// Draw vertical lines for crosses at the grid points.
    /// You need to supply the <b>Stroke</b> brush property to give each line a color.
    /// If you do not specify a non-zero <b>StrokeThickness</b>, it will be set to 1.
    /// <see cref="GridPattern"/> will automatically set the other "Stroke..." properties.
    /// Control the length of each dash by setting the <see cref="GridPattern.CrossLengthProperty"/>.
    /// </summary>
    VerticalCross=9,
  }
}
