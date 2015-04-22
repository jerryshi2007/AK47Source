
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
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Northwoods.GoXam {


  /// <summary>
  /// This <c>Shape</c> can be used only in the <c>DataTemplate</c> for an <see cref="Adornment"/>
  /// indicating that another part is selected.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A <see cref="SelectionHandle"/> takes the shape of the <see cref="Adornment.AdornedElement"/> that it adorns,
  /// but its <c>Stroke</c>, <c>StrokeThickness</c>, <c>Fill</c>, et al. must still be set.
  /// You should not set its <c>Width</c> or <c>Height</c>, because those will be determined
  /// by the size and shape of the adorned element.
  /// </para>
  /// <para>
  /// A simple example definition:
  /// <code>
  ///   &lt;DataTemplate&gt;
  ///     &lt;go:SelectionHandle Stroke="Red" StrokeThickness="2" /&gt;
  ///   &lt;/DataTemplate&gt;
  /// </code>
  /// </para>
  /// <para>
  /// If a <c>SelectionHandle</c> is not the only element of the selection adornment template,
  /// it should be named as the <see cref="Node.LocationElementName"/> for the whole adornment.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class SelectionHandle :

    Path



  {
    /// <summary>
    /// A <see cref="SelectionHandle"/> takes the shape of the <see cref="Adornment.AdornedElement"/> that it adorns,
    /// but its <c>Stroke</c>, <c>StrokeThickness</c>, <c>Fill</c>, et al. must still be set.
    /// </summary>
    public SelectionHandle() { }













  }



  /// <summary>
  /// This <c>Shape</c> can be used only in the <c>DataTemplate</c> for an <see cref="Adornment"/>
  /// used by <see cref="Northwoods.GoXam.Tool.DiagramTool"/>s to allow the adorned part to be manipulated.
  /// </summary>
  /// <remarks>
  /// <para>
  /// You should set its <c>Stroke</c>, <c>StrokeThickness</c>, <c>Fill</c>, et al.
  /// as you would for any <c>Shape</c>, as well as its <c>Width</c> and <c>Height</c>.
  /// </para>
  /// <para>
  /// A <c>ToolHandle</c> is rectangular by default, but you can set or bind its <see cref="NodeFigure"/>
  /// attached property to get different figures.
  /// You can specify a limited number of figure shapes for these handles:
  /// <see cref="NodeFigure.Rectangle"/> (the default figure),
  /// <see cref="NodeFigure.Ellipse"/>,
  /// <see cref="NodeFigure.Diamond"/>,
  /// <see cref="NodeFigure.TriangleRight"/>,
  /// <see cref="NodeFigure.TriangleDown"/>,
  /// <see cref="NodeFigure.TriangleLeft"/>,
  /// <see cref="NodeFigure.TriangleUp"/>,
  /// <see cref="NodeFigure.PlusLine"/>,
  /// <see cref="NodeFigure.XLine"/>,
  /// <see cref="NodeFigure.AsteriskLine"/>,
  /// </para>
  /// <para>
  /// A simple example definition:
  /// <code>
  ///   &lt;DataTemplate&gt;
  ///     &lt;go:SpotPanel&gt;
  ///       &lt;go:ToolHandle go:SpotPanel.Spot="Center" go:NodePanel.Figure="Ellipse"
  ///               Width="6" Height="6" Fill="Yellow" Stroke="Black" StrokeThickness="1" /&gt;
  ///     &lt;/go:SpotPanel&gt;
  ///   &lt;/DataTemplate&gt;
  /// </code>
  /// </para>
  /// <para>
  /// You can have many tool handles in a single <see cref="Adornment"/>.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class ToolHandle :

    Path



  {
    /// <summary>
    /// You still need to set its <c>Stroke</c>, <c>StrokeThickness</c>, <c>Fill</c>, et al, as with any <c>Shape</c>.
    /// </summary>
    public ToolHandle() {

      NodePanel.SetFigure(this, NodeFigure.Rectangle);

      this.Stretch = Stretch.Fill;  // because GetFigureGeometry always returns a Geometry with unscaled points from 0,0 to 1,1
    }















  }



  /// <summary>
  /// An adornment is a special kind of <see cref="Part"/> that is associated with another part,
  /// the <see cref="AdornedPart"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Adornments can be associated with a particular element in the adorned part --
  /// that is the value of <see cref="AdornedElement"/>.
  /// </para>
  /// <para>
  /// Adornments can be distinguished by their <see cref="Part.Category"/>.
  /// This property can be an arbitrary string value determined by the code creating the adornment,
  /// typically a tool that wants to be able to tell various adornments apart from each other.
  /// Use the <see cref="Part"/> method <see cref="Part.GetAdornment"/> to find an adornment
  /// for a part of a given category.
  /// </para>
  /// <para>
  /// For example, adornments created by <see cref="Part.UpdateSelectionAdornment"/> have the
  /// <see cref="Part.Category"/> of "Selection".
  /// Those created by <see cref="Northwoods.GoXam.Tool.ResizingTool.UpdateAdornments"/>
  /// have a category of "Resize".
  /// </para>
  /// <para>
  /// Adornments are always unbound parts -- but if the <see cref="AdornedPart"/> is bound to data,
  /// the adornment's data bindings can refer to the same data.
  /// </para>
  /// <para>
  /// There cannot be any links connected to an adornment, nor can an adornment have members or be a member of a group.
  /// </para>
  /// <para>
  /// An adornment cannot have its own adornments.
  /// </para>
  /// <para>
  /// The template you use for a selection adornment will normally consist of either
  /// a Path (Silverlight) or SelectionHandle (WPF),
  /// or for nodes a <see cref="SpotPanel"/>, which is treated specially within an <c>Adornment</c>,
  /// or for links a <see cref="LinkPanel"/>.
  /// Although this element is normally the root <see cref="Part.VisualElement"/>,
  /// if it is surrounded by a panel or container or decorator, you need to make sure that it is the <see cref="Node.LocationElement"/>
  /// for the <c>Adornment</c> by naming it and supplying that name as the <see cref="Node.LocationElementName"/>.
  /// </para>
  /// <para>
  /// The template you use for a tool adornment will normally consist of either
  /// a Path (Silverlight) or ToolHandle (WPF),
  /// or for nodes a <see cref="SpotPanel"/> containing Paths (Silverlight) or ToolHandles (WPF),
  /// or for links a <see cref="LinkPanel"/> containing Paths (Silverlight) or ToolHandles (WPF).
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class Adornment : Node {
    /// <summary>
    /// Each <see cref="Adornment"/> has a <see cref="Part.LayerName"/> of "Adornment".
    /// </summary>
    public Adornment() {
      this.LayerName = DiagramPanel.AdornmentLayerName;  // not "", as it is for regular Nodes
    }

    static Adornment() {
      AdornedElementProperty = DependencyProperty.Register("AdornedElement", typeof(FrameworkElement), typeof(Adornment),
        new FrameworkPropertyMetadata(null));
    }

    /// <summary>
    /// Identifies the <see cref="AdornedElement"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AdornedElementProperty;
    /// <summary>
    /// Gets or sets the <c>FrameworkElement</c> that this adornment is associated with.
    /// </summary>
    public FrameworkElement AdornedElement {
      get { return (FrameworkElement)GetValue(AdornedElementProperty); }
      set { SetValue(AdornedElementProperty, value); }
    }

    /// <summary>
    /// Gets the <see cref="Part"/> that this adornment is associated with.
    /// </summary>
    public Part AdornedPart {
      get { return Diagram.FindAncestor<Part>(this.AdornedElement); }
    }

    /// <summary>
    /// Adornments are always unbound, so this property is always null
    /// and cannot be set.
    /// </summary>
    public override Object Data {
      // by making sure Data is always null,
      // we assume an Adornment has no member nodes, isn't member of any group node,
      // doesn't have any connected links, and doesn't have any link label node
      get { return null; }
      set { Diagram.Error("Cannot set Part.Data on an Adornment"); }
    }

    /// <summary>
    /// Adornments are never bound to data, so this property is always false.
    /// </summary>
    public override bool IsBoundToData {
      get { return false; }
    }

    internal override void InvalidateOtherPartRelationships(bool arrangeonly) { }

    internal override bool IsReadyToMeasureArrange() {
      Part adpart = this.AdornedPart;
      return (adpart == null || adpart.ValidArrange);
    }

    /// <summary>
    /// An Adornment is never a member of any group.
    /// </summary>
    public override IEnumerable<Group> ContainingGroups {
      get { return NoGroups; }
    }

    /// <summary>
    /// An Adornment is never a member of any group.
    /// </summary>
    public override Group ContainingSubGraph {
      get { return null; }
    }


    /// <summary>
    /// Assume adornments can't have their own adornments.
    /// </summary>
    /// <param name="selelt"></param>
    /// <param name="templ"></param>
    /// <returns></returns>
    public override Adornment MakeAdornment(FrameworkElement selelt, DataTemplate templ) { return null; }

    /// <summary>
    /// Assume adornments don't have any adornments that need to be updated.
    /// </summary>
    public override void UpdateAdornments() { }




    /// <summary>
    /// Before measuring this adornment, make sure we have rendered any handles
    /// by setting the <c>Path.Data</c> of any path that has a <c>NodePanel.Figure</c> attached property.
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns></returns>
    protected override Size MeasureOverride(Size availableSize) {
      RenderHandles(this.VisualElement);  // specify Path.Data according to NodeFigure
      FrameworkElement locelt = this.LocationElement;
      if (locelt is SpotPanel) locelt.InvalidateMeasure();
      Size sz = base.MeasureOverride(availableSize);
      return sz;
    }

    // walk the visual tree of adornment elements
    private void RenderHandles(FrameworkElement elt) {
      if (elt == null) return;
      Path path = elt as Path;
      if (path != null) {  // if it's a Path
        // and if NodePanel.Figure is specified and is None, it's a SelectionHandle equivalent
        NodeFigure fig = NodePanel.GetFigure(elt);
        if (fig == NodeFigure.None) {  // must explicitly set go:NodePanel.Figure="None", to distinguish this selection handle from random Path shapes
          Shape adornedshape = this.AdornedElement as Shape;  // hack: because GetSelectionGeometry doesn't always return a Geometry of the AdornedElement's size
          if (adornedshape is Path && this.AdornedPart is Node &&
              NodePanel.GetFigure(adornedshape) != NodeFigure.None)  { // when that Path is a NodeShape equivalent
            path.Stretch = Stretch.Fill;
            Size sz = GetEffectiveSize(adornedshape);  // can't just use ActualWidth/Height, because measuring has reset them to zero
            path.Width = sz.Width;
            path.Height = sz.Height;
          }
          path.Data = GetSelectionGeometry(path, this);
        } else {  // otherwise assume it's a ToolHandle equivalent
          if (path.Data == null) {
            path.Stretch = Stretch.Fill;
            path.Data = GetFigureGeometry(path);  // returns a Geometry with points from 0,0 to 1,1
          }
        }
      } else {
        int count = VisualTreeHelper.GetChildrenCount(elt);
        for (int i = 0; i < count; i++) {
          RenderHandles(VisualTreeHelper.GetChild(elt, i) as FrameworkElement);
        }
      }
    }

    internal static Geometry Copy(Geometry g) {
      if (g == null) return null;

      RectangleGeometry rg = g as RectangleGeometry;
      if (rg != null) {
        return new RectangleGeometry() { Rect = rg.Bounds, RadiusX = rg.RadiusX, RadiusY = rg.RadiusY, Transform = rg.Transform };
      }
      EllipseGeometry eg = g as EllipseGeometry;
      if (eg != null) {
        return new EllipseGeometry() { Center = eg.Center, RadiusX = eg.RadiusX, RadiusY = eg.RadiusY, Transform = eg.Transform };
      }
      LineGeometry lg = g as LineGeometry;
      if (lg != null) {
        return new LineGeometry() { StartPoint = lg.StartPoint, EndPoint = lg.EndPoint, Transform = lg.Transform };
      }
      PathGeometry pg = g as PathGeometry;
      if (pg != null) {
        if (!Geo.PathGeometryHasFigures(pg)) {
          return new RectangleGeometry() { Rect = pg.Bounds, RadiusX = 0, RadiusY = 0, Transform = pg.Transform };
        } else {
          PathGeometry pg2 = new PathGeometry();
          pg2.FillRule = pg.FillRule;
          pg2.Figures = new PathFigureCollection();  // required for Geo.PathGeometryHasFigures to work
          foreach (PathFigure pf in pg.Figures) {
            PathFigure pf2 = new PathFigure();
            pf2.StartPoint = pf.StartPoint;
            foreach (PathSegment ps in pf.Segments) {
              pf2.Segments.Add(Copy(ps));
            }
            pf2.IsClosed = pf.IsClosed;
            pf2.IsFilled = pf.IsFilled;
            pg2.Figures.Add(pf2);
          }
          pg2.Transform = pg.Transform;
          return pg2;
        }
      }
      GeometryGroup gg = g as GeometryGroup;
      if (gg != null) {
        GeometryGroup gg2 = new GeometryGroup();
        gg2.FillRule = gg.FillRule;
        foreach (Geometry x in gg.Children) {
          gg2.Children.Add(Copy(x));
        }
        gg2.Transform = gg.Transform;
        return gg2;
      }
      Diagram.Error("Copying an unknown kind of Geometry: " + g.ToString());
      return null;
    }

    private static PathSegment Copy(PathSegment s) {
      if (s == null) return null;

      LineSegment ls = s as LineSegment;
      if (ls != null) {
        return new LineSegment() { Point = ls.Point };
      }
      BezierSegment bs = s as BezierSegment;
      if (bs != null) {
        return new BezierSegment() { Point1 = bs.Point1, Point2 = bs.Point2, Point3 = bs.Point3 };
      }
      QuadraticBezierSegment qbs = s as QuadraticBezierSegment;
      if (qbs != null) {
        return new QuadraticBezierSegment() { Point1 = qbs.Point1, Point2 = qbs.Point2 };
      }
      ArcSegment arc = s as ArcSegment;
      if (arc != null) {
        return new ArcSegment() { Point = arc.Point, Size = arc.Size, SweepDirection = arc.SweepDirection, IsLargeArc = arc.IsLargeArc, RotationAngle = arc.RotationAngle };
      }
      PolyLineSegment pls = s as PolyLineSegment;
      if (pls != null) {
        return new PolyLineSegment() { Points = Copy(pls.Points) };
      }
      PolyBezierSegment pbs = s as PolyBezierSegment;
      if (pbs != null) {
        return new PolyBezierSegment() { Points = Copy(pbs.Points) };
      }
      PolyQuadraticBezierSegment pqbs = s as PolyQuadraticBezierSegment;
      if (pqbs != null) {
        return new PolyQuadraticBezierSegment() { Points = Copy(pqbs.Points) };
      }
      Diagram.Error("Copying an unknown kind of PathSegment: " + s.ToString());
      return null;
    }

    private static PointCollection Copy(PointCollection coll) {
      PointCollection newcoll = new PointCollection();
      foreach (Point p in coll) newcoll.Add(p);
      return newcoll;
    }

    private static Geometry GetRenderedGeometry(Shape shape, Adornment ad) {
      Rectangle rectangle = shape as Rectangle;
      if (rectangle != null) {
        Size sz = ad.GetEffectiveSize(rectangle);
        return new RectangleGeometry() { Rect = new Rect(0, 0, sz.Width, sz.Height), RadiusX = rectangle.RadiusX, RadiusY = rectangle.RadiusY };
      }
      Ellipse ellipse = shape as Ellipse;
      if (ellipse != null) {
        Size sz = ad.GetEffectiveSize(ellipse);
        return new EllipseGeometry() { Center = new Point(sz.Width/2, sz.Height/2), RadiusX = sz.Width/2, RadiusY = sz.Height/2 };
      }
      Line line = shape as Line;
      if (line != null) return new LineGeometry() { StartPoint = new Point(line.X1, line.Y1), EndPoint = new Point(line.X2, line.Y2) };
      Polyline polyline = shape as Polyline;
      if (polyline != null) {
        PathGeometry g = new PathGeometry();
        PathFigure pf = new PathFigure();
        PolyLineSegment pls = new PolyLineSegment();
        pls.Points = Copy(polyline.Points);  //?? not returning a Geometry with the actual points
        if (polyline.Stretch != Stretch.None) {
          Size sz = ad.GetEffectiveSize(polyline);
          pls.Points = ScalePoints(pls.Points, sz, polyline.Stretch);
        }
        pf.Segments.Add(pls);
        if (pls.Points.Count > 0) pf.StartPoint = pls.Points[0];
        pf.IsClosed = false;
        pf.IsFilled = false;
        g.Figures.Add(pf);
        return g;
      }
      Polygon polygon = shape as Polygon;
      if (polygon != null) {
        PathGeometry g = new PathGeometry();
        PathFigure pf = new PathFigure();
        PolyLineSegment pls = new PolyLineSegment();
        pls.Points = Copy(polygon.Points);  //?? not returning a Geometry with the actual points
        if (polygon.Stretch != Stretch.None) {
          Size sz = ad.GetEffectiveSize(polygon);
          pls.Points = ScalePoints(pls.Points, sz, polygon.Stretch);
        }
        pf.Segments.Add(pls);
        if (pls.Points.Count > 0) pf.StartPoint = pls.Points[0];
        pf.IsClosed = true;
        pf.IsFilled = true;
        g.Figures.Add(pf);
        return g;
      }
      Path path = shape as Path;
      if (path != null) return Copy(path.Data);  //?? not returning a Geometry with the actual points
      return null;
    }

    private static PointCollection ScalePoints(PointCollection coll, Size sz, Stretch stretch) {
      if (stretch == Stretch.None) return coll;
      double minx = Double.MaxValue;
      double maxx = Double.MinValue;
      double miny = Double.MaxValue;
      double maxy = Double.MinValue;
      foreach (Point p in coll) {
        minx = Math.Min(minx, p.X);
        maxx = Math.Max(maxx, p.X);
        miny = Math.Min(miny, p.Y);
        maxy = Math.Max(maxy, p.Y);
      }
      double w = maxx-minx;
      double h = maxy-miny;
      if (w < 0.01) w = 0.01;
      if (h < 0.01) h = 0.01;
      double xr = sz.Width/w;
      double yr = sz.Height/h;
      switch (stretch) {
        case Stretch.Fill:
          break;
        case Stretch.Uniform:
          xr = yr = Math.Min(xr, yr);
          break;
        case Stretch.UniformToFill:
          xr = yr = Math.Max(xr, yr);
          break;
      }
      PointCollection pts = new PointCollection();
      foreach (Point p in coll) {
        double dx = (p.X-minx)*xr + minx;
        double dy = (p.Y-miny)*yr + miny;
        pts.Add(new Point(dx, dy));
      }
      return pts;
    }



















    // Return a Geometry that has points that are scaled up to the size of the ADORNED element.
    //?? hack: EXCEPT in Silverlight, where if the AdornedElement is a Path/Polygon/Polyline, the Geometry might not have absolute points,
    // but will need to be stretched by the ADORNMENT Shape.
    internal static Geometry GetSelectionGeometry(Shape adorning, Adornment ad) {
      FrameworkElement adorned = ad.AdornedElement;
      Part adornedpart = ad.AdornedPart;

      Shape shape = adorned as Shape;
      if (shape != null) return GetRenderedGeometry(shape, ad);

      // otherwise just assume it's rectangular
      // Try to surround the adorned element by accounting for the thickness of the adornment's stroke
      // as well as the actual size of the adorned element.
      double t = (adorning != null ? adorning.StrokeThickness : 0);
      Size sz = new Size(0, 0);
      if (adornedpart != null) {
        sz = adornedpart.GetEffectiveSize(adorned);  // can't just use ActualWidth/Height, because measuring has reset them to zero
      }
      return new RectangleGeometry() { Rect = new Rect(-t/2, -t/2, sz.Width+t, sz.Height+t) };
    }

    // return a Geometry that is unscaled (all points are from 0,0 to 1,1)
    //?? need to use NodeShape some day; promote this stuff to NodePanel also, to implement NodeShape equivalent
    internal static Geometry GetFigureGeometry(Shape elt) {
      switch (NodePanel.GetFigure(elt)) {
        default:
        case NodeFigure.Rectangle:

          Geometry



          _RectangleFigure = new RectangleGeometry() {
            Rect = new Rect(0, 0, 1, 1)
          };
          return _RectangleFigure;
        case NodeFigure.Ellipse:

          Geometry



          _EllipseFigure = new EllipseGeometry() {
            Center = new Point(0.5, 0.5),
            RadiusX = 0.5,
            RadiusY = 0.5
          };
          return _EllipseFigure;
        case NodeFigure.Diamond:

          Geometry



          _DiamondFigure = new PathGeometry() {
            Figures = new PathFigureCollection() {
              new PathFigure() {
                StartPoint = new Point(0.5, 0),
                Segments = new PathSegmentCollection() {
                  new LineSegment() { Point = new Point(1, 0.5) },
                  new LineSegment() { Point = new Point(0.5, 1) },
                  new LineSegment() { Point = new Point(0, 0.5) }
                },
                IsClosed = true
              }
            }
          };
          return _DiamondFigure;
        case NodeFigure.TriangleRight:

          Geometry



          _TriangleRightFigure = new PathGeometry() {
            Figures = new PathFigureCollection() {
              new PathFigure() {
                StartPoint = new Point(0, 0),
                  Segments = new PathSegmentCollection() {
                    new LineSegment() { Point = new Point(1, 0.5) },
                    new LineSegment() { Point = new Point(0, 1) }
                  },
                IsClosed = true
              }
            }
          };
          return _TriangleRightFigure;
        case NodeFigure.TriangleDown:

          Geometry



          _TriangleDownFigure = new PathGeometry() {
            Figures = new PathFigureCollection() {
              new PathFigure() {
                StartPoint = new Point(0, 0),
                  Segments = new PathSegmentCollection() {
                    new LineSegment() { Point = new Point(1, 0) },
                    new LineSegment() { Point = new Point(0.5, 1) }
                  },
                IsClosed = true
              }
            }
          };
          return _TriangleDownFigure;
        case NodeFigure.TriangleLeft:

          Geometry



          _TriangleLeftFigure = new PathGeometry() {
            Figures = new PathFigureCollection() {
              new PathFigure() {
                StartPoint = new Point(1, 1),
                  Segments = new PathSegmentCollection() {
                    new LineSegment() { Point = new Point(0, 0.5) },
                    new LineSegment() { Point = new Point(1, 0) }
                  },
                IsClosed = true
              }
            }
          };
          return _TriangleLeftFigure;
        case NodeFigure.TriangleUp:

          Geometry



          _TriangleUpFigure = new PathGeometry() {
            Figures = new PathFigureCollection() {
              new PathFigure() {
                StartPoint = new Point(1, 1),
                  Segments = new PathSegmentCollection() {
                    new LineSegment() { Point = new Point(0, 1) },
                    new LineSegment() { Point = new Point(0.5, 0) }
                  },
                IsClosed = true
              }
            }
          };
          return _TriangleUpFigure;
        case NodeFigure.PlusLine:

          Geometry



          _PlusLineFigure = new PathGeometry() {
            Figures = new PathFigureCollection() {
              new PathFigure() {
                StartPoint = new Point(0.5, 0),
                Segments = new PathSegmentCollection() {
                  new LineSegment() { Point = new Point(0.5, 1) }
                },
                IsClosed = false
              },
              new PathFigure() {
                StartPoint = new Point(0, 0.5),
                Segments = new PathSegmentCollection() {
                  new LineSegment() { Point = new Point(1, 0.5) }
                },
                IsClosed = false
              }
            }
          };
          return _PlusLineFigure;
        case NodeFigure.XLine:

          Geometry



          _XLineFigure = new PathGeometry() {
            Figures = new PathFigureCollection() {
              new PathFigure() {
                StartPoint = new Point(0, 0),
                Segments = new PathSegmentCollection() {
                  new LineSegment() { Point = new Point(1, 1) }
                },
                IsClosed = false
              },
              new PathFigure() {
                StartPoint = new Point(1, 0),
                Segments = new PathSegmentCollection() {
                  new LineSegment() { Point = new Point(0, 1) }
                },
                IsClosed = false
              }
            }
          };
          return _XLineFigure;
        case NodeFigure.AsteriskLine:

          Geometry



          _AsteriskLineFigure = new PathGeometry() {
            Figures = new PathFigureCollection() {
              new PathFigure() {
                StartPoint = new Point(0.5, 0),
                Segments = new PathSegmentCollection() {
                  new LineSegment() { Point = new Point(0.5, 1) }
                },
                IsClosed = false
              },
              new PathFigure() {
                StartPoint = new Point(0, 0.5),
                Segments = new PathSegmentCollection() {
                  new LineSegment() { Point = new Point(1, 0.5) }
                },
                IsClosed = false
              },
              new PathFigure() {
                StartPoint = new Point(0, 0),
                Segments = new PathSegmentCollection() {
                  new LineSegment() { Point = new Point(1, 1) }
                },
                IsClosed = false
              },
              new PathFigure() {
                StartPoint = new Point(1, 0),
                Segments = new PathSegmentCollection() {
                  new LineSegment() { Point = new Point(0, 1) }
                },
                IsClosed = false
              }
            }
          };
          return _AsteriskLineFigure;
      }
    }













  }
}
