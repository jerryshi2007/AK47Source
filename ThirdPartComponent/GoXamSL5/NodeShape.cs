
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
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Northwoods.GoXam {


  /// <summary>
  /// This <c>Shape</c> can be used in a <see cref="NodePanel"/>
  /// as the background for text and/or other elements.
  /// </summary>
  /// <remarks>
  /// <para>
  /// You will need to set the <c>NodePanel.Figure</c> attached property (see cref="NodePanel.SetFigure"/>)
  /// to give the shape a predefined geometry.
  /// The geometry of some figures can be customized by setting parameters
  /// (also <see cref="NodePanel"/> attached properties).
  /// </para>
  /// <para>
  /// In addition to setting the usual properties of the <c>Shape</c> such as <c>Fill</c>,
  /// <c>Stroke</c>, <c>StrokeThickness</c>, et al., remember to set the <c>Width</c> and <c>Height</c>,
  /// unless this shape is the first child of a <see cref="NodePanel"/> with a <see cref="NodePanel.Sizing"/>
  /// other than <see cref="NodePanelSizing.Fixed"/>, in which case the <see cref="NodePanel"/>
  /// will size this shape appropriately.
  /// </para>
  /// <para>
  /// The <see cref="NodePanel"/> uses the <c>NodePanel.Spot1</c> and <c>NodePanel.Spot2</c>
  /// attached properties to control where in the shape the text element(s) should be placed.
  /// </para>
  /// <para>
  /// A typical usage:
  /// <code>
  ///   &lt;DataTemplate x:Key="ExampleNodeTemplate"&gt;
  ///     &lt;go:NodePanel go:Node.Location="{Binding Path=Data.XY, Mode=TwoWay}"
  ///                   go:Node.SelectionElementName="Shape" go:Node.Resizable="True"&gt;
  ///       &lt;go:NodeShape x:Name="Shape" go:NodePanel.Figure="RoundedRectangle" Width="50" Height="20"
  ///                     Stroke="Gray" StrokeThickness="1" Fill="LightYellow" /&gt;
  ///       &lt;TextBlock Text="{Binding Path=Data.Name}" TextWrapping="Wrap" /&gt;
  ///     &lt;/go:NodePanel&gt;
  ///   &lt;/DataTemplate&gt;
  /// </code>
  /// In WPF, you must use the <c>&lt;go:NodeShape&gt;</c> element instead of a <c>&lt;Path&gt;</c> element.
  /// In Silverlight 3, you must use the <c>&lt;Path&gt;</c> element instead of a <c>&lt;go:NodeShape&gt;</c> element.
  /// In Silverlight 4, you may use either the <c>&lt;Path&gt;</c> element or a <c>&lt;go:NodeShape&gt;</c> element.
  /// In Silverlight, the element must be a child of a <see cref="NodePanel"/>.
  /// </para>
  /// </remarks>
  public class NodeShape :

    Path



  {
    /// <summary>
    /// Construct a <see cref="NodeShape"/> with no particular figure or size or fill or outline.
    /// </summary>
    public NodeShape() {
      this.Stretch = Stretch.Fill;
    }


    //???
    //protected override Size ArrangeOverride(Size finalSize) {
    //  this.Data = new NodeGeometry(this).GetGeometry(finalSize);
    //  base.ArrangeOverride(finalSize);
    //}





































  }



  internal class GeoStream {
    public GeoStream() {
      _StreamGeometry = new StreamGeometry();
    }

    private StreamGeometry _StreamGeometry;

    public StreamGeometryContext Open() {
      return _StreamGeometry.Open();
    }

    public Geometry Geometry {
      get {

        return _StreamGeometry._Geo;




      }
    }

    public FillRule FillRule {
      get { return _StreamGeometry.FillRule; }
      set { _StreamGeometry.FillRule = value; }
    }
  }


  internal class NodeGeometry {
    internal NodeGeometry(Shape shape) {
      this.Shape = shape;
    }

    private Shape Shape { get; set; }

    // get these parameters from the Shape:
    private NodeFigure Figure {
      get { return NodePanel.GetFigure(this.Shape); }
      //set { NodePanel.SetFigure(this.Shape, value); }
    }

    private double FigureParameter1 {
      get { return NodePanel.GetFigureParameter1(this.Shape); }
      set { NodePanel.SetFigureParameter1(this.Shape, value); }
    }

    private double FigureParameter2 {
      get { return NodePanel.GetFigureParameter2(this.Shape); }
      set { NodePanel.SetFigureParameter2(this.Shape, value); }
    }

    // get/set these parameters from the Shape:
    private Spot Spot1 {
      get { return NodePanel.GetSpot1(this.Shape); }
      set { NodePanel.SetSpot1(this.Shape, value); }
    }

    private Spot Spot2 {
      get { return NodePanel.GetSpot2(this.Shape); }
      set { NodePanel.SetSpot2(this.Shape, value); }
    }


    internal Geometry GetGeometry() {
      return GetGeometry(new Size(this.Shape.Width, this.Shape.Height));
    }

    //??? doesn't always check/limit values for Parameter1/Parameter2
    internal Geometry GetGeometry(Size size) {
      double w = size.Width;
      if (Double.IsNaN(w) || Double.IsPositiveInfinity(w) || w < 0.5) w = 100;

      double h = size.Height;
      if (Double.IsNaN(h) || Double.IsPositiveInfinity(h) || h < 0.5) h = 100;

      Geometry geo = null;
      switch (this.Figure) {  // most common cases first

        case NodeFigure.None: // none a rectangle too, to make sure we don't return null
        case NodeFigure.Square:
        //?? s.Reshapable = false;    // maintain aspect ratio
        case NodeFigure.Rectangle: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 0), true, true);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
              context.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }

        case NodeFigure.Circle:
        //?? s.Reshapable = false;    // maintain aspect ratio
        case NodeFigure.Ellipse:
        case NodeFigure.Connector: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0.156, 0.156);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(0.844, 0.844);
            geo = new EllipseGeometry() { Center = new Point(.5 * w, .5 * h), RadiusX = .5 * w, RadiusY = .5 * h };
            break;
          }

        case NodeFigure.TriangleRight:
          if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .25);
          if (this.Spot2.IsDefault) this.Spot2 = new Spot(.5, .75);

          return new PathGeometry() {
            Figures = new PathFigureCollection() {
                new PathFigure() {
                  StartPoint = new Point(0, 0),
                    Segments = new PathSegmentCollection() {
                      new LineSegment() { Point = new Point(w, 0.5*h) },
                      new LineSegment() { Point = new Point(0, h) }
                    },
                  IsClosed = true
                }
              }
          };
        case NodeFigure.TriangleDown:
          if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25, 0);
          if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, .5);
          return new PathGeometry() {
            Figures = new PathFigureCollection() {
                new PathFigure() {
                  StartPoint = new Point(0, 0),
                    Segments = new PathSegmentCollection() {
                      new LineSegment() { Point = new Point(w, 0) },
                      new LineSegment() { Point = new Point(0.5*w, h) }
                    },
                  IsClosed = true
                }
              }
          };
        case NodeFigure.TriangleLeft:
          if (this.Spot1.IsDefault) this.Spot1 = new Spot(.5, .25);
          if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, .75);
          return new PathGeometry() {
            Figures = new PathFigureCollection() {
                new PathFigure() {
                  StartPoint = new Point(w, h),
                    Segments = new PathSegmentCollection() {
                      new LineSegment() { Point = new Point(0, 0.5*h) },
                      new LineSegment() { Point = new Point(w, 0) }
                    },
                  IsClosed = true
                }
              }
          };
        case NodeFigure.TriangleUp:
          if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25, .50);
          if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, 1);
          return new PathGeometry() {
            Figures = new PathFigureCollection() {
                new PathFigure() {
                  StartPoint = new Point(w, h),
                    Segments = new PathSegmentCollection() {
                      new LineSegment() { Point = new Point(0, h) },
                      new LineSegment() { Point = new Point(0.5*w, 0) }
                    },
                  IsClosed = true
                }
              }
          };

        // Lines
        case NodeFigure.Line1:
          geo = new LineGeometry() { StartPoint = new Point(0, 0), EndPoint = new Point(w, h) };
          break;
        case NodeFigure.Line2:
          geo = new LineGeometry() { StartPoint = new Point(w, 0), EndPoint = new Point(0, h) };
          break;
        case NodeFigure.LineH:
          geo = new LineGeometry() { StartPoint = new Point(0, 0), EndPoint = new Point(w, 0) };
          break;
        case NodeFigure.LineV:
          geo = new LineGeometry() { StartPoint = new Point(0, 0), EndPoint = new Point(0, h) };
          break;
        // Curves
        case NodeFigure.Curve1: {
            double factor = Geo.MagicBezierFactor;
            GeoStream sg1 = new GeoStream();
            using (StreamGeometryContext context = sg1.Open()) {
              context.BeginFigure(new Point(0, 0), false, false);
              context.BezierTo(new Point(factor * w, 0), new Point(1 * w, (1 - factor) * h), new Point(w, h), true, true);
            }
            geo = sg1.Geometry;
            break;
          }
        case NodeFigure.Curve2: {
            double factor = Geo.MagicBezierFactor;
            GeoStream sg2 = new GeoStream();
            using (StreamGeometryContext context = sg2.Open()) {
              context.BeginFigure(new Point(0, 0), false, false);
              context.BezierTo(new Point(0, factor * h), new Point((1 - factor) * w, h), new Point(w, h), true, true);
            }
            geo = sg2.Geometry;
            break;
          }
        case NodeFigure.Curve3: {
            double factor = Geo.MagicBezierFactor;
            GeoStream sg3 = new GeoStream();
            using (StreamGeometryContext context = sg3.Open()) {
              context.BeginFigure(new Point(1 * w, 0), false, false);
              context.BezierTo(new Point(1 * w, factor * h), new Point(factor * w, 1 * h), new Point(0, 1 * h), true, true);
            }
            geo = sg3.Geometry;
            break;
          }
        case NodeFigure.Curve4: {
            double factor = Geo.MagicBezierFactor;
            GeoStream sg4 = new GeoStream();
            using (StreamGeometryContext context = sg4.Open()) {
              context.BeginFigure(new Point(1 * w, 0), false, false);
              context.BezierTo(new Point((1 - factor) * w, 0), new Point(0, (1 - factor) * h), new Point(0, 1 * h), true, true);
            }
            geo = sg4.Geometry;
            break;
          }

        // Polygons
        case NodeFigure.Triangle:
        case NodeFigure.Alternative:
        case NodeFigure.Merge:
          if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25, .5);
          if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, 1);

          GeoStream sg5 = new GeoStream();
          using (StreamGeometryContext context = sg5.Open()) {
            context.BeginFigure(new Point(.5 * w, 0 * h), true, true);
            context.LineTo(new Point(0 * w, 1 * h), true, false);
            context.LineTo(new Point(1 * w, 1 * h), true, false);
          }
          geo = sg5.Geometry;
          break;
        case NodeFigure.Diamond:
        case NodeFigure.Decision:
          if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25, .25);
          if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, .75);

          GeoStream sg6 = new GeoStream();
          using (StreamGeometryContext context = sg6.Open()) {
            context.BeginFigure(new Point(.5 * w, 0), true, true);
            context.LineTo(new Point(0, .5 * h), true, false);
            context.LineTo(new Point(.5 * w, 1 * h), true, false);
            context.LineTo(new Point(1 * w, .5 * h), true, false);
          }
          geo = sg6.Geometry;
          break;
        case NodeFigure.Pentagon: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.2, .22);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.8, .9);

            Point[] points = CreatePolygon(5);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(points[0].X * w, points[0].Y * h), true, true);
              for (int i = 1; i < 5; i++)
                context.LineTo(new Point(points[i].X * w, points[i].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Hexagon:
        case NodeFigure.DataTransmission: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.07, .25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.93, .75);

            Point[] points = CreatePolygon(6);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(points[0].X * w, points[0].Y * h), true, true);
              for (int i = 1; i < 6; i++)
                context.LineTo(new Point(points[i].X * w, points[i].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Heptagon: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.2, .15);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.8, .85);

            Point[] points = CreatePolygon(7);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(points[0].X * w, points[0].Y * h), true, true);
              for (int i = 1; i < 7; i++)
                context.LineTo(new Point(points[i].X * w, points[i].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Octagon: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.15, .15);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.85, .85);

            Point[] points = CreatePolygon(8);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(points[0].X * w, points[0].Y * h), true, true);
              for (int i = 1; i < 8; i++)
                context.LineTo(new Point(points[i].X * w, points[i].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Nonagon: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.17, .13);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.82, .82);

            Point[] points = CreatePolygon(9);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(points[0].X * w, points[0].Y * h), true, true);
              for (int i = 1; i < 9; i++)
                context.LineTo(new Point(points[i].X * w, points[i].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Decagon: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.16, .16);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.84, .84);

            Point[] points = CreatePolygon(10);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(points[0].X * w, points[0].Y * h), true, true);
              for (int i = 1; i < 10; i++)
                context.LineTo(new Point(points[i].X * w, points[i].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Dodecagon: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.16, .16);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.84, .84);

            Point[] points = CreatePolygon(12);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(points[0].X * w, points[0].Y * h), true, true);
              for (int i = 1; i < 12; i++)
                context.LineTo(new Point(points[i].X * w, points[i].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }

        // Stars
        case NodeFigure.FivePointedStar: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.312, .383);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.693, .765);

            Point[] points = CreateStar(5);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(points[0].X * w, points[0].Y * h), true, true);
              for (int i = 1; i < 10; i++)
                context.LineTo(new Point(points[i].X * w, points[i].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.SixPointedStar: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.170, .251);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.833, .755);

            Point[] starPoints = CreateStar(6);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(starPoints[0].X * w, starPoints[0].Y * h), true, true);
              for (int i = 1; i < 12; i++)
                context.LineTo(new Point(starPoints[i].X * w, starPoints[i].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.SevenPointedStar: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.363, .361);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.641, .709);

            Point[] starPoints = CreateStar(7);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(starPoints[0].X * w, starPoints[0].Y * h), true, true);
              for (int i = 1; i < 14; i++)
                context.LineTo(new Point(starPoints[i].X * w, starPoints[i].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.EightPointedStar: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.252, .255);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, .75);

            Point[] starPoints = CreateStar(8);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(starPoints[0].X * w, starPoints[0].Y * h), true, true);
              for (int i = 1; i < 16; i++)
                context.LineTo(new Point(starPoints[i].X * w, starPoints[i].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.NinePointedStar: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.355, .361);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.645, .651);

            Point[] starPoints = CreateStar(9);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(starPoints[0].X * w, starPoints[0].Y * h), true, true);
              for (int i = 1; i < 18; i++)
                context.LineTo(new Point(starPoints[i].X * w, starPoints[i].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.TenPointedStar: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.281, .261);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.723, .748);

            Point[] starPoints = CreateStar(10);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(starPoints[0].X * w, starPoints[0].Y * h), true, true);
              for (int i = 1; i < 20; i++)
                context.LineTo(new Point(starPoints[i].X * w, starPoints[i].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }

        // Bursts
        case NodeFigure.FivePointedBurst: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.312, .383);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.693, .765);

            Point[] burstPoints = CreateBurst(5);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(burstPoints[0].X * w, burstPoints[0].Y * h), true, true);
              for (int i = 1; i < burstPoints.Length; i += 3)
                context.BezierTo(new Point(burstPoints[i].X * w, burstPoints[i].Y * h), new Point(burstPoints[i + 1].X * w, burstPoints[i + 1].Y * h), new Point(burstPoints[i + 2].X * w, burstPoints[i + 2].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.SixPointedBurst: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.170, .251);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.833, .755);

            Point[] burstPoints = CreateBurst(6);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(burstPoints[0].X * w, burstPoints[0].Y * h), true, true);
              for (int i = 1; i < burstPoints.Length; i += 3)
                context.BezierTo(new Point(burstPoints[i].X * w, burstPoints[i].Y * h), new Point(burstPoints[i + 1].X * w, burstPoints[i + 1].Y * h), new Point(burstPoints[i + 2].X * w, burstPoints[i + 2].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.SevenPointedBurst: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.363, .361);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.641, .709);

            Point[] burstPoints = CreateBurst(7);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(burstPoints[0].X * w, burstPoints[0].Y * h), true, true);
              for (int i = 1; i < burstPoints.Length; i += 3)
                context.BezierTo(new Point(burstPoints[i].X * w, burstPoints[i].Y * h), new Point(burstPoints[i + 1].X * w, burstPoints[i + 1].Y * h), new Point(burstPoints[i + 2].X * w, burstPoints[i + 2].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.EightPointedBurst: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.252, .255);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, .75);

            Point[] burstPoints = CreateBurst(8);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(burstPoints[0].X * w, burstPoints[0].Y * h), true, true);
              for (int i = 1; i < burstPoints.Length; i += 3)
                context.BezierTo(new Point(burstPoints[i].X * w, burstPoints[i].Y * h), new Point(burstPoints[i + 1].X * w, burstPoints[i + 1].Y * h), new Point(burstPoints[i + 2].X * w, burstPoints[i + 2].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.NinePointedBurst: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.355, .361);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.645, .651);

            Point[] burstPoints = CreateBurst(9);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(burstPoints[0].X * w, burstPoints[0].Y * h), true, true);
              for (int i = 1; i < burstPoints.Length; i += 3)
                context.BezierTo(new Point(burstPoints[i].X * w, burstPoints[i].Y * h), new Point(burstPoints[i + 1].X * w, burstPoints[i + 1].Y * h), new Point(burstPoints[i + 2].X * w, burstPoints[i + 2].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.TenPointedBurst: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.281, .261);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.723, .748);

            Point[] burstPoints = CreateBurst(10);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(burstPoints[0].X * w, burstPoints[0].Y * h), true, true);
              for (int i = 1; i < burstPoints.Length; i += 3)
                context.BezierTo(new Point(burstPoints[i].X * w, burstPoints[i].Y * h), new Point(burstPoints[i + 1].X * w, burstPoints[i + 1].Y * h), new Point(burstPoints[i + 2].X * w, burstPoints[i + 2].Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }

        // Basic Shapes
        case NodeFigure.Cloud:
          if (this.Spot1.IsDefault) this.Spot1 = new Spot(.1, .1);
          if (this.Spot2.IsDefault) this.Spot2 = new Spot(.9, .9);

          GeoStream sg26 = new GeoStream();
          using (StreamGeometryContext context = sg26.Open()) {
            context.BeginFigure(new Point(.08034461 * w, .1944299 * h), true, true);
            context.BezierTo(new Point(-.09239631 * w, .07836421 * h), new Point(.1406031 * w, -.0542823 * h), new Point(.2008615 * w, .05349299 * h), true, false);
            context.BezierTo(new Point(.2450511 * w, -.01697547 * h), new Point(.3776197 * w, -.02112067 * h), new Point(.4338609 * w, .074219 * h), true, false);
            context.BezierTo(new Point(.4539471 * w, 0), new Point(.6066018 * w, -.02526587 * h), new Point(.6558228 * w, .07004196 * h), true, false);
            context.BezierTo(new Point(.6914277 * w, -.02904177 * h), new Point(.8921095 * w, -.02220843 * h), new Point(.8921095 * w, .08370865 * h), true, false);
            context.BezierTo(new Point(1.036446 * w, .04105738 * h), new Point(1.020377 * w, .3022052 * h), new Point(.9147671 * w, .3194596 * h), true, false);
            context.BezierTo(new Point(1.04448 * w, .360238 * h), new Point(.992256 * w, .5219009 * h), new Point(.9082935 * w, .562044 * h), true, false);
            context.BezierTo(new Point(1.042337 * w, .5771781 * h), new Point(1.028411 * w, .8120651 * h), new Point(.9212406 * w, .8217117 * h), true, false);
            context.BezierTo(new Point(1.028411 * w, .9571472 * h), new Point(.8556702 * w, 1.052487 * h), new Point(.7592566 * w, .9156953 * h), true, false);
            context.BezierTo(new Point(.7431877 * w, 1.019325 * h), new Point(.5624123 * w, 1.031761 * h), new Point(.5101666 * w, .9310455 * h), true, false);
            context.BezierTo(new Point(.4820677 * w, 1.031761 * h), new Point(.3030112 * w, 1.002796 * h), new Point(.2609328 * w, .9344623 * h), true, false);
            context.BezierTo(new Point(.2329994 * w, 1.01518 * h), new Point(.03213784 * w, 1.01518 * h), new Point(.08034461 * w, .870098 * h), true, false);
            context.BezierTo(new Point(-.02812061 * w, .9032597 * h), new Point(-.01205169 * w, .6835638 * h), new Point(.06829292 * w, .6545475 * h), true, false);
            context.BezierTo(new Point(-.02812061 * w, .6089503 * h), new Point(-.01606892 * w, .4555777 * h), new Point(.06427569 * w, .4265613 * h), true, false);
            context.BezierTo(new Point(-.01606892 * w, .3892545 * h), new Point(-.01205169 * w, .1944299 * h), new Point(.08034461 * w, .1944299 * h), true, false);
          }
          geo = sg26.Geometry;
          break;
        case NodeFigure.Crescent:
        case NodeFigure.Gate: {              // Flowchart
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.511, .19);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.776, .76);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 0), true, true);
              context.BezierTo(new Point(1 * w, 0), new Point(1 * w, 1 * h), new Point(0, 1 * h), true, false);
              context.BezierTo(new Point(.5 * w, .75 * h), new Point(.5 * w, .25 * h), new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.FramedRectangle: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              double param1 = this.FigureParameter1;
              double param2 = this.FigureParameter2;
              // Set defaults for parameters if they aren't set already
              if (param1 == 0) param1 = .1; // Distance between left and inner rect
              if (param2 == 0) param2 = .1; // Distance between top and inner rect
              // Set valid text area
              if (this.Spot1.IsDefault) this.Spot1 = new Spot(param1, param2);
              if (this.Spot2.IsDefault) this.Spot2 = new Spot(1 - param1, 1 - param2);

              // Outside rectangle
              context.BeginFigure(new Point(0, 0), true, true);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
              context.LineTo(new Point(0, 0), true, false);

              // Inside Rectangle
              context.BeginFigure(new Point(param1 * w, param2 * h), false, true);
              context.LineTo(new Point((1 - param1) * w, param2 * h), true, false);
              context.LineTo(new Point((1 - param1) * w, (1 - param2) * h), true, false);
              context.LineTo(new Point(param1 * w, (1 - param2) * h), true, false);
              context.LineTo(new Point(param1 * w, param2 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.HalfEllipse:
        case NodeFigure.Delay: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .20);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, .80);

            double factor = Geo.MagicBezierFactor;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 0), true, true);
              context.BezierTo(new Point(factor * w, 0), new Point(1 * w, (.5 - factor / 2) * h), new Point(1 * w, .5 * h), true, false);
              context.BezierTo(new Point(1 * w, (.5 + factor / 2) * h), new Point(factor * w, 1 * h), new Point(0, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Heart: {
            // Set default params
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .25; // y value of the middle
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.15, .29);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.86, .68);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(.5 * w, 1 * h), true, true);
              context.BezierTo(new Point(.1 * w,.8 * h), new Point(0, .5 * h), new Point(0 * w, .3 * h), true, false);
              context.BezierTo(new Point(0 * w, 0), new Point(.45 * w, 0), new Point(.5 * w, .3 * h), true, false);
              context.BezierTo(new Point(.55 * w, 0), new Point(1 * w, 0), new Point(1 * w, .3 * h), true, false);
              context.BezierTo(new Point(w, .5 * h), new Point(.9 * w, .8 * h), new Point(.5 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Spade: {
            // Set default params
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .7; // y value of the center

            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.19, .26);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.80, .68);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              // Begin at the top middle
              context.BeginFigure(new Point(.5 * w, 0), true, true);
              context.LineTo(new Point(.51 * w, .01 * h), true, false);
              context.BezierTo(new Point(.6 * w, .2 * h), new Point(w, .25 * h), new Point(w, .5 * h), true, false);
              context.BezierTo(new Point(w, .8 * h), new Point(.6 * w, .8 * h), new Point(.55 * w, .7 * h), true, false);

              // start the base
              context.BezierTo(new Point(.5 * w, .75 * h), new Point(.55 * w, .95 * h), new Point(.75 * w, h), true, false);
              context.LineTo(new Point(.25 * w, h), true, false);
              context.BezierTo(new Point(.45 * w, .95 * h), new Point(.5 * w, .75 * h), new Point(.45 * w, .7 * h), true, false);

              // Finish the other side of the top
              context.BezierTo(new Point(.4 * w, .8 * h), new Point(0, .8 * h), new Point(0, .5 * h), true, false);
              context.BezierTo(new Point(0, .25 * h), new Point(.4 * w, .2 * h), new Point(.49 * w, .01 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Club: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.06, .39);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.93, .58);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              // start the base
              context.BeginFigure(new Point(.4 * w, .6 * h), true, true);
              context.BezierTo(new Point(.5 * w, .75 * h), new Point(.45 * w, .95 * h), new Point(.15 * w, 1 * h), true, false);              
              context.LineTo(new Point(.85 * w, h), true, false);
              context.BezierTo(new Point(.55 * w, .95 * h), new Point(.5 * w, .75 * h), new Point(.6 * w, .6 * h), true, false);
              // First circle:
              double r = .2; // radius
              double cx = .3; // offset from Center x
              double cy = 0; // offset from Center y
              double d = r * (4 * (Math.Sqrt(2) - 1) / 3);
              context.BezierTo(new Point((.5 - r + cx) * w, (.5 + d + cy) * h), new Point((.5 - d + cx) * w, (.5 + r + cy) * h), new Point((.5 + cx) * w, (.5 + r + cy) * h), true, false);
              context.BezierTo(new Point((.5 + d + cx) * w, (.5 + r + cy) * h), new Point((.5 + r + cx) * w, (.5 + d + cy) * h), new Point((1 - .5 + r + cx) * w, (.5 + cy) * h), true, false);
              context.BezierTo(new Point((1 - .5 + r + cx) * w, (.5 - d + cy) * h), new Point((.5 + d + cx) * w, (.5 - r + cy) * h), new Point((.5 + cx) * w, (.5 - r + cy) * h), true, false);
              context.BezierTo(new Point((.5 - d + cx) * w, (.5 - r + cy) * h), new Point((.5 - r + cx + .05) * w, (.5 - d + cy - .02) * h), new Point((.65) * w, (0.36771243) * h), true, false);
              r = .2; // radius
              cx = 0; // offset from Center x
              cy = -.3; // offset from Center y
              d = r * (4 * (Math.Sqrt(2) - 1) / 3);
              context.BezierTo(new Point((.5 + d + cx) * w, (.5 + r + cy) * h), new Point((.5 + r + cx) * w, (.5 + d + cy) * h), new Point((1 - .5 + r + cx) * w, (.5 + cy) * h), true, false);
              context.BezierTo(new Point((1 - .5 + r + cx) * w, (.5 - d + cy) * h), new Point((.5 + d + cx) * w, (.5 - r + cy) * h), new Point((.5 + cx) * w, (.5 - r + cy) * h), true, false);
              context.BezierTo(new Point((.5 - d + cx) * w, (.5 - r + cy) * h), new Point((.5 - r + cx) * w, (.5 - d + cy) * h), new Point((.5 - r + cx) * w, (.5 + cy) * h), true, false);
              context.BezierTo(new Point((.5 - r + cx) * w, (.5 + d + cy) * h), new Point((.5 - d + cx) * w, (.5 + r + cy) * h), new Point((.35) * w, (0.36771243) * h), true, false);
              r = .2; // radius
              cx = -.3; // offset from Center x
              cy = 0; // offset from Center y
              d = r * (4 * (Math.Sqrt(2) - 1) / 3);
              context.BezierTo(new Point((1 - .5 + r + cx - .05) * w, (.5 - d + cy - .02) * h), new Point((.5 + d + cx) * w, (.5 - r + cy) * h), new Point((.5 + cx) * w, (.5 - r + cy) * h), true, false);
              context.BezierTo(new Point((.5 - d + cx) * w, (.5 - r + cy) * h), new Point((.5 - r + cx) * w, (.5 - d + cy) * h), new Point((.5 - r + cx) * w, (.5 + cy) * h), true, false);
              context.BezierTo(new Point((.5 - r + cx) * w, (.5 + d + cy) * h), new Point((.5 - d + cx) * w, (.5 + r + cy) * h), new Point((.5 + cx) * w, (.5 + r + cy) * h), true, false);
              context.BezierTo(new Point((.5 + d + cx) * w, (.5 + r + cy) * h), new Point((.5 + r + cx) * w, (.5 + d + cy) * h), new Point(.4 * w, .6 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.YinYang: {
            double factor = Geo.MagicBezierFactor;
            double cpOffset = factor * .5;
            double radius = .5;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              // Right side
              context.BeginFigure(new Point(radius * w, 0), true, true);
              context.BezierTo(new Point((radius + cpOffset) * w, 0), new Point(1 * w, (radius - cpOffset) * h), new Point(1 * w, radius * h), true, false);
              context.BezierTo(new Point(1 * w, (radius + cpOffset) * h), new Point((radius + cpOffset) * w, 1 * h), new Point(radius * w, 1 * h), true, false);
              context.BezierTo(new Point(1 * w, radius * h), new Point(0, radius * h), new Point(radius * w, 0), true, false);

              // left side
              context.BeginFigure(new Point(radius * w, 1 * h), false, false);
              context.BezierTo(new Point((radius - cpOffset) * w, 1 * h), new Point(0, (radius + cpOffset) * h), new Point(0, radius * h), true, false);
              context.BezierTo(new Point(0, (radius - cpOffset) * h), new Point((radius - cpOffset) * w, 0), new Point(radius * w, 0), true, false);

              Point center = new Point(.5, .75);
              radius = .1;
              cpOffset = factor * .1;
              // bottom circle
              context.BeginFigure(new Point(center.X * w, (center.Y - radius) * h), true, true);
              context.BezierTo(new Point((center.X - cpOffset) * w, (center.Y - radius) * h), new Point((center.X - radius) * w, (center.Y - cpOffset) * h), new Point((center.X - radius) * w, center.Y * h), true, false); //UL TL
              context.BezierTo(new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false); //BL TB
              context.BezierTo(new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + radius) * w, center.Y * h), true, false); //BR TR
              context.BezierTo(new Point((center.X + radius) * w, (center.Y - cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y - radius) * h), new Point(center.X * w, (center.Y - radius) * h), true, false); //UR TT

              center = new Point(.5, .25);

              // top circle
              context.BeginFigure(new Point(center.X * w, (center.Y - radius) * h), true, true);
              context.BezierTo(new Point((center.X - cpOffset) * w, (center.Y - radius) * h), new Point((center.X - radius) * w, (center.Y - cpOffset) * h), new Point((center.X - radius) * w, center.Y * h), true, false); //UL TL
              context.BezierTo(new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false); //BL TB
              context.BezierTo(new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + radius) * w, center.Y * h), true, false); //BR TR
              context.BezierTo(new Point((center.X + radius) * w, (center.Y - cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y - radius) * h), new Point(center.X * w, (center.Y - radius) * h), true, false); //UR TT
              context.LineTo(new Point(center.X * w, (center.Y - radius) * h), false, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Peace: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.146, .146);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.853, .853);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              double d1 = .5 * (4 * (Math.Sqrt(2) - 1) / 3);

              // First circle:
              context.BeginFigure(new Point(w, .5 * h), true, true);
              context.BezierTo(new Point(w, (.5 - d1) * h), new Point((.5 + d1) * w, 0), new Point(.5 * w, 0), true, false);
              context.BezierTo(new Point((.5 - d1) * w, 0), new Point(0, (.5 - d1) * h), new Point(0, .5 * h), true, false);
              context.BezierTo(new Point(0, (.5 + d1) * h), new Point((.5 - d1) * w, h), new Point(.5 * w, h), true, false);
              context.BezierTo(new Point((.5 + d1) * w, h), new Point(w, (.5 + d1) * h), new Point(w, .5 * h), true, false);
              // Second Circle
              double d2 = .4 * (4 * (Math.Sqrt(2) - 1) / 3);
              context.BeginFigure(new Point(.9 * w, .5 * h), true, true);
              context.BezierTo(new Point(.9 * w, (.5 - d2) * h), new Point((.5 + d2) * w, .1 * h), new Point(.5 * w, .1 * h), true, false);
              context.BezierTo(new Point((.5 - d2) * w, .1 * h), new Point(.1 * w, (.5 - d2) * h), new Point(.1 * w, .5 * h), true, false);
              context.BezierTo(new Point(.1 * w, (.5 + d2) * h), new Point((.5 - d2) * w, .9 * h), new Point(.5 * w, .9 * h), true, false);
              context.BezierTo(new Point((.5 + d2) * w, .9 * h), new Point(.9 * w, (.5 + d2) * h), new Point(.9 * w, .5 * h), true, false);
              // Inner Circles
              { // #1
                double r = .07; // radius
                double cx = 0; // offset from Center x
                double cy = -.707 * .11; // offset from Center y
                double d = r * (4 * (Math.Sqrt(2) - 1) / 3);
                context.BeginFigure(new Point((1 - .5 + r + cx) * w, (.5 + cy) * h), true, true);
                context.BezierTo(new Point((1 - .5 + r + cx) * w, (.5 - d + cy) * h), new Point((.5 + d + cx) * w, (.5 - r + cy) * h), new Point((.5 + cx) * w, (.5 - r + cy) * h), true, false);
                context.BezierTo(new Point((.5 - d + cx) * w, (.5 - r + cy) * h), new Point((.5 - r + cx) * w, (.5 - d + cy) * h), new Point((.5 - r + cx) * w, (.5 + cy) * h), true, false);
                context.BezierTo(new Point((.5 - r + cx) * w, (.5 + d + cy) * h), new Point((.5 - d + cx) * w, (.5 + r + cy) * h), new Point((.5 + cx) * w, (.5 + r + cy) * h), true, false);
                context.BezierTo(new Point((.5 + d + cx) * w, (.5 + r + cy) * h), new Point((.5 + r + cx) * w, (.5 + d + cy) * h), new Point((1 - .5 + r + cx) * w, (.5 + cy) * h), true, false);
              }
              { // #2
                double r = .07; // radius
                double cx = -0.707 * .11; // offset from Center x
                double cy = 0.707 * .11; // offset from Center y
                double d = r * (4 * (Math.Sqrt(2) - 1) / 3);
                context.BeginFigure(new Point((1 - .5 + r + cx) * w, (.5 + cy) * h), true, true);
                context.BezierTo(new Point((1 - .5 + r + cx) * w, (.5 - d + cy) * h), new Point((.5 + d + cx) * w, (.5 - r + cy) * h), new Point((.5 + cx) * w, (.5 - r + cy) * h), true, false);
                context.BezierTo(new Point((.5 - d + cx) * w, (.5 - r + cy) * h), new Point((.5 - r + cx) * w, (.5 - d + cy) * h), new Point((.5 - r + cx) * w, (.5 + cy) * h), true, false);
                context.BezierTo(new Point((.5 - r + cx) * w, (.5 + d + cy) * h), new Point((.5 - d + cx) * w, (.5 + r + cy) * h), new Point((.5 + cx) * w, (.5 + r + cy) * h), true, false);
                context.BezierTo(new Point((.5 + d + cx) * w, (.5 + r + cy) * h), new Point((.5 + r + cx) * w, (.5 + d + cy) * h), new Point((1 - .5 + r + cx) * w, (.5 + cy) * h), true, false);
              }
              { // #3
                double r = .07; // radius
                double cx = 0.707 * .11; // offset from Center x
                double cy = 0.707 * .11; // offset from Center y
                double d = r * (4 * (Math.Sqrt(2) - 1) / 3);
                context.BeginFigure(new Point((1 - .5 + r + cx) * w, (.5 + cy) * h), true, true);
                context.BezierTo(new Point((1 - .5 + r + cx) * w, (.5 - d + cy) * h), new Point((.5 + d + cx) * w, (.5 - r + cy) * h), new Point((.5 + cx) * w, (.5 - r + cy) * h), true, false);
                context.BezierTo(new Point((.5 - d + cx) * w, (.5 - r + cy) * h), new Point((.5 - r + cx) * w, (.5 - d + cy) * h), new Point((.5 - r + cx) * w, (.5 + cy) * h), true, false);
                context.BezierTo(new Point((.5 - r + cx) * w, (.5 + d + cy) * h), new Point((.5 - d + cx) * w, (.5 + r + cy) * h), new Point((.5 + cx) * w, (.5 + r + cy) * h), true, false);
                context.BezierTo(new Point((.5 + d + cx) * w, (.5 + r + cy) * h), new Point((.5 + r + cx) * w, (.5 + d + cy) * h), new Point((1 - .5 + r + cx) * w, (.5 + cy) * h), true, false);
              }
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.NotAllowed: {
            double factor = Geo.MagicBezierFactor;
            double cpOffset = factor * .5;
            double radius = .5;
            Point center = new Point(.5, .5);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // Outer circle
              s.BeginFigure(new Point(center.X * w, (center.Y - radius) * h), true, true);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y - radius) * h), new Point((center.X - radius) * w, (center.Y - cpOffset) * h), new Point((center.X - radius) * w, center.Y * h), true, false); //UL TL
              s.BezierTo(new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false); //BL TB
              s.BezierTo(new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + radius) * w, center.Y * h), true, false); //BR TR
              s.BezierTo(new Point((center.X + radius) * w, (center.Y - cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y - radius) * h), new Point(center.X * w, (center.Y - radius) * h), true, false); //UR TT

              // Inner circle, composed of two parts, separated by
              // a beam across going from top-right to bottom-left.
              radius = .40;
              cpOffset = factor * .40;
              // First we cut up the top right 90 degree curve into two smaller curves.
              // Since its clockwise, StartOfArrow is the first of the two points on the circle. EndOfArrow is the other one.
              Point startOfArrowc1;
              Point startOfArrowc2;
              Point startOfArrow;
              Point unused;
              BreakUpBezier(new Point(center.X, center.Y - radius), new Point(center.X + cpOffset, center.Y - radius),
                new Point(center.X + radius, center.Y - cpOffset), new Point(center.X + radius, center.Y), .42, out startOfArrowc1,
                out startOfArrowc2, out startOfArrow, out unused, out unused);

              Point endOfArrowc1;
              Point endOfArrowc2;
              Point endOfArrow;
              BreakUpBezier(new Point(center.X, center.Y - radius), new Point(center.X + cpOffset, center.Y - radius),
                new Point(center.X + radius, center.Y - cpOffset), new Point(center.X + radius, center.Y), .58, out unused,
                out unused, out endOfArrow, out endOfArrowc1, out endOfArrowc2);

              // Cut up the bottom left 90 degree curve into two smaller curves.
              Point startOfArrow2c1;
              Point startOfArrow2c2;
              Point startOfArrow2;
              BreakUpBezier(new Point(center.X, center.Y + radius), new Point(center.X - cpOffset, center.Y + radius),
                new Point(center.X - radius, center.Y + cpOffset), new Point(center.X - radius, center.Y), .42, out startOfArrow2c1,
                out startOfArrow2c2, out startOfArrow2, out unused, out unused);
              Point endOfArrow2c1;
              Point endOfArrow2c2;
              Point endOfArrow2;
              BreakUpBezier(new Point(center.X, center.Y + radius), new Point(center.X - cpOffset, center.Y + radius),
                new Point(center.X - radius, center.Y + cpOffset), new Point(center.X - radius, center.Y), .58, out unused,
                out unused, out endOfArrow2, out endOfArrow2c1, out endOfArrow2c2);

              // Take all the parts made and make the two separate inner circle parts.
              s.BeginFigure(new Point(endOfArrow2.X * w, endOfArrow2.Y * h), true, true);
              s.BezierTo(new Point(endOfArrow2c1.X * w, endOfArrow2c1.Y * h), new Point(endOfArrow2c2.X * w, endOfArrow2c2.Y * h), new Point((center.X - radius) * w, center.Y * h), true, false);
              s.BezierTo(new Point((center.X - radius) * w, (center.Y - cpOffset) * h), new Point((center.X - cpOffset) * w, (center.Y - radius) * h), new Point(center.X * w, (center.Y - radius) * h), true, false);
              s.BezierTo(new Point(startOfArrowc1.X * w, startOfArrowc1.Y * h), new Point(startOfArrowc2.X * w, startOfArrowc2.Y * h), new Point(startOfArrow.X * w, startOfArrow.Y * h), true, false);
              s.LineTo(new Point(endOfArrow2.X * w, endOfArrow2.Y * h), true, false);

              s.BeginFigure(new Point(startOfArrow2.X * w, startOfArrow2.Y * h), true, true);
              s.LineTo(new Point(endOfArrow.X * w, endOfArrow.Y * h), true, false);
              s.BezierTo(new Point(endOfArrowc1.X * w, endOfArrowc1.Y * h), new Point(endOfArrowc2.X * w, endOfArrowc2.Y * h), new Point((center.X + radius) * w, center.Y * h), true, false);
              s.BezierTo(new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false);
              s.BezierTo(new Point(startOfArrow2c1.X * w, startOfArrow2c1.Y * h), new Point(startOfArrow2c2.X * w, startOfArrow2c2.Y * h), new Point(startOfArrow2.X * w, startOfArrow2.Y * h), true, false);

              sg.FillRule = FillRule.EvenOdd;
            }

            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Fragile: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, .4);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              //Top and crack
              context.BeginFigure(new Point(0, 0), true, true);
              context.LineTo(new Point(.25 * w, 0), true, false);
              context.LineTo(new Point(.2 * w, .15 * h), true, false);
              context.LineTo(new Point(.3 * w, .25 * h), true, false);
              context.LineTo(new Point(.29 * w, .33 * h), true, false);
              context.LineTo(new Point(.35 * w, .25 * h), true, false);
              context.LineTo(new Point(.3 * w, .15 * h), true, false);
              context.LineTo(new Point(.4 * w, 0), true, false);
              context.LineTo(new Point(1 * w, 0), true, false);

              // Left side
              context.BezierTo(new Point(1 * w, .25 * h), new Point(.75 * w, .5 * h), new Point(.55 * w, .5 * h), true, false);
              context.LineTo(new Point(.55 * w, .9 * h), true, false);

              // The base
              context.LineTo(new Point(.7 * w, .9 * h), true, false);
              context.LineTo(new Point(.7 * w, 1 * h), true, false);
              context.LineTo(new Point(.3 * w, 1 * h), true, false);
              context.LineTo(new Point(.3 * w, .9 * h), true, false);

              // Right side
              context.LineTo(new Point(.45 * w, .9 * h), true, false);
              context.LineTo(new Point(.45 * w, .5 * h), true, false);
              context.BezierTo(new Point(.25 * w, .5 * h), new Point(0, .25 * h), new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }

        case NodeFigure.HourGlass: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(.65 * w, .5 * h), true, true);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
              context.LineTo(new Point(.35 * w, .5 * h), true, false);
              context.LineTo(new Point(0, 0), true, false);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.LineTo(new Point(.65 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Lightning: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0 * w, .55 * h), true, true);
              context.LineTo(new Point(.38 * w, 0), true, false);
              context.LineTo(new Point(.75 * w, 0), true, false);
              context.LineTo(new Point(.25 * w, .45 * h), true, false);
              context.LineTo(new Point(.9 * w, .48 * h), true, false);
              context.LineTo(new Point(.4 * w, 1 * h), true, false);
              context.LineTo(new Point(.65 * w, .55 * h), true, false);
              context.LineTo(new Point(0, .55 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Parallelogram1: {
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .1; // Topleft corner's x distance from leftmost point
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(param1, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1 - param1, 1);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(param1 * w, 0), true, true);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.LineTo(new Point((1 - param1) * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Input:            // Flowchart
        case NodeFigure.Output: {
            GeoStream sg = new GeoStream();
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.1, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.9, 1);
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 1 * h), true, true);
              context.LineTo(new Point(.1 * w, 0), true, false);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.LineTo(new Point(.9 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Parallelogram2: {
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .25; // Topleft corner's x distance from leftmost point
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(param1, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1 - param1, 1);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(param1 * w, 0), true, true);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.LineTo(new Point((1 - param1) * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.ThickCross: {
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .25; // Thickness of the cross
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.5 - param1 / 2, .5 - param1 / 2);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.5 + param1 / 2, .5 + param1 / 2);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point((.5 - param1 / 2) * w, 0), true, true);
              context.LineTo(new Point((.5 + param1 / 2) * w, 0), true, false);
              context.LineTo(new Point((.5 + param1 / 2) * w, (.5 - param1 / 2) * h), true, false);
              context.LineTo(new Point(1 * w, (.5 - param1 / 2) * h), true, false);
              context.LineTo(new Point(1 * w, (.5 + param1 / 2) * h), true, false);
              context.LineTo(new Point((.5 + param1 / 2) * w, (.5 + param1 / 2) * h), true, false);
              context.LineTo(new Point((.5 + param1 / 2) * w, 1 * h), true, false);
              context.LineTo(new Point((.5 - param1 / 2) * w, 1 * h), true, false);
              context.LineTo(new Point((.5 - param1 / 2) * w, (.5 + param1 / 2) * h), true, false);
              context.LineTo(new Point(0, (.5 + param1 / 2) * h), true, false);
              context.LineTo(new Point(0, (.5 - param1 / 2) * h), true, false);
              context.LineTo(new Point((.5 - param1 / 2) * w, (.5 - param1 / 2) * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.ThickX: {
            double spotPlace = .25 / Math.Sqrt(2);
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.5 - spotPlace, .5 - spotPlace);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.5 + spotPlace, .5 + spotPlace);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(.3 * w, 0), true, true);
              s.LineTo(new Point(.5 * w, .2 * h), true, false);
              s.LineTo(new Point(.7 * w, 0), true, false);
              s.LineTo(new Point(1 * w, .3 * h), true, false);
              s.LineTo(new Point(.8 * w, .5 * h), true, false);
              s.LineTo(new Point(1 * w, .7 * h), true, false);
              s.LineTo(new Point(.7 * w, 1 * h), true, false);
              s.LineTo(new Point(.5 * w, .8 * h), true, false);
              s.LineTo(new Point(.3 * w, 1 * h), true, false);
              s.LineTo(new Point(0, .7 * h), true, false);
              s.LineTo(new Point(.2 * w, .5 * h), true, false);
              s.LineTo(new Point(0, .3 * h), true, false);
              s.LineTo(new Point(.3 * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.ThinCross: {
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .1; // Thickness of the cross
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point((.5 - param1 / 2) * w, 0), true, true);
              context.LineTo(new Point((.5 + param1 / 2) * w, 0), true, false);
              context.LineTo(new Point((.5 + param1 / 2) * w, (.5 - param1 / 2) * h), true, false);
              context.LineTo(new Point(1 * w, (.5 - param1 / 2) * h), true, false);
              context.LineTo(new Point(1 * w, (.5 + param1 / 2) * h), true, false);
              context.LineTo(new Point((.5 + param1 / 2) * w, (.5 + param1 / 2) * h), true, false);
              context.LineTo(new Point((.5 + param1 / 2) * w, 1 * h), true, false);
              context.LineTo(new Point((.5 - param1 / 2) * w, 1 * h), true, false);
              context.LineTo(new Point((.5 - param1 / 2) * w, (.5 + param1 / 2) * h), true, false);
              context.LineTo(new Point(0, (.5 + param1 / 2) * h), true, false);
              context.LineTo(new Point(0, (.5 - param1 / 2) * h), true, false);
              context.LineTo(new Point((.5 - param1 / 2) * w, (.5 - param1 / 2) * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.ThinX: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(.1 * w, 0), true, true);
              s.LineTo(new Point(.5 * w, .4 * h), true, false);
              s.LineTo(new Point(.9 * w, 0), true, false);
              s.LineTo(new Point(1 * w, .1 * h), true, false);
              s.LineTo(new Point(.6 * w, .5 * h), true, false);
              s.LineTo(new Point(1 * w, .9 * h), true, false);
              s.LineTo(new Point(.9 * w, 1 * h), true, false);
              s.LineTo(new Point(.5 * w, .6 * h), true, false);
              s.LineTo(new Point(.1 * w, 1 * h), true, false);
              s.LineTo(new Point(0, .9 * h), true, false);
              s.LineTo(new Point(.4 * w, .5 * h), true, false);
              s.LineTo(new Point(0, .1 * h), true, false);
              s.LineTo(new Point(.1 * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.RightTriangle: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .5);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.5, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 0), true, true);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.RoundedIBeam: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 0), true, true);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.BezierTo(new Point(.5 * w, .25 * h), new Point(.5 * w, .75 * h), new Point(1 * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
              context.BezierTo(new Point(.5 * w, .75 * h), new Point(.5 * w, .25 * h), new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.RoundedRectangle: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.1, .1);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.9, .9);

            double factor = Geo.MagicBezierFactor;
            double cpOffset = factor * .3;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(.3 * w, 0), true, true);
              context.LineTo(new Point(.7 * w, 0), true, false);
              context.BezierTo(new Point((.7 + cpOffset) * w, 0), new Point(1 * w, (.3 - cpOffset) * h), new Point(1 * w, .3 * h), true, false);
              context.LineTo(new Point(1 * w, .7 * h), true, false);
              context.BezierTo(new Point(1 * w, (.7 + cpOffset) * h), new Point((.7 + cpOffset) * w, 1 * h), new Point(.7 * w, 1 * h), true, false);
              context.LineTo(new Point(.3 * w, 1 * h), true, false);
              context.BezierTo(new Point((.3 - cpOffset) * w, 1 * h), new Point(0, (.7 + cpOffset) * h), new Point(0, .7 * h), true, false);
              context.LineTo(new Point(0, .3 * h), true, false);
              context.BezierTo(new Point(0, (.3 - cpOffset) * h), new Point((.3 - cpOffset) * w, 0), new Point(.3 * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.SquareIBeam: {
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .2; // Width of the ibeam in % of the total width
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 0), true, true);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.LineTo(new Point(1 * w, param1 * h), true, false);
              context.LineTo(new Point((.5 + param1 / 2) * w, param1 * h), true, false);
              context.LineTo(new Point((.5 + param1 / 2) * w, (1 - param1) * h), true, false);
              context.LineTo(new Point(1 * w, (1 - param1) * h), true, false);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
              context.LineTo(new Point(0, (1 - param1) * h), true, false);
              context.LineTo(new Point((.5 - param1 / 2) * w, (1 - param1) * h), true, false);
              context.LineTo(new Point((.5 - param1 / 2) * w, param1 * h), true, false);
              context.LineTo(new Point(0, param1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Trapezoid: {
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .2; // Distance from topleft of bounding rectangle, in % of the total width, of the topleft corner
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(param1, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1 - param1, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(param1 * w, 0), true, true);
              context.LineTo(new Point((1 - param1) * w, 0), true, false);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
              context.LineTo(new Point(param1 * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.ManualLoop:
        case NodeFigure.ManualOperation: {
            double param1 = this.FigureParameter1;
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.1, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.9, 1);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(param1, 0), true, true);
              context.LineTo(new Point(0, 0), true, false);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.LineTo(new Point(.9 * w, 1 * h), true, false);
              context.LineTo(new Point(.1 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.GenderMale: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.202, .257);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.692, .839);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              double factor = Geo.MagicBezierFactor;
              double cpOffset = factor * .4;
              double radius = .4;
              Point center = new Point(.5, .5);
              Point unused;
              Point mid;
              Point c1;
              Point c2;

              // Outer circle
              s.BeginFigure(new Point((center.X - radius) * w, center.Y * h), true, true);
              s.BezierTo(new Point((center.X - radius) * w, (center.Y - cpOffset) * h), new Point((center.X - cpOffset) * w, (center.Y - radius) * h), new Point(center.X * w, (center.Y - radius) * h), true, false);
              BreakUpBezier(new Point(center.X, center.Y - radius), new Point(center.X + cpOffset, center.Y - radius),
                 new Point(center.X + radius, center.Y - cpOffset), new Point(center.X + radius, center.Y), .44, out c1,
                 out c2, out mid, out unused, out unused);
              s.BezierTo(new Point(c1.X * w, c1.Y * h), new Point(c2.X * w, c2.Y * h), new Point(mid.X * w, mid.Y * h), true, false);
              Point startOfArrow = new Point(mid.X, mid.Y);
              BreakUpBezier(new Point(center.X, center.Y - radius), new Point(center.X + cpOffset, center.Y - radius),
                new Point(center.X + radius, center.Y - cpOffset), new Point(center.X + radius, center.Y), .56, out unused,
                out unused, out mid, out c1, out c2);
              Point endOfArrow = new Point(mid.X, mid.Y);

              s.LineTo(new Point((startOfArrow.X * .1 + .95 * .9) * w, (startOfArrow.Y * .1) * h), true, false);
              s.LineTo(new Point(.85 * w, (startOfArrow.Y * .1) * h), true, false);
              s.LineTo(new Point(.85 * w, 0), true, false);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point(1 * w, .15 * h), true, false);
              s.LineTo(new Point((endOfArrow.X * .1 + .9) * w, .15 * h), true, false);
              s.LineTo(new Point((endOfArrow.X * .1 + .9) * w, (endOfArrow.Y * .1 + .05 * .9) * h), true, false);
              s.LineTo(new Point(endOfArrow.X * w, endOfArrow.Y * h), true, false);

              s.BezierTo(new Point(c1.X * w, c1.Y * h), new Point(c2.X * w, c2.Y * h), new Point((center.X + radius) * w, center.Y * h), true, false);
              s.BezierTo(new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X - radius) * w, center.Y * h), true, false);

              // Inner circle
              radius = .35;
              cpOffset = factor * .35;
              s.BeginFigure(new Point(center.X * w, (center.Y - radius) * h), true, true);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y - radius) * h), new Point((center.X - radius) * w, (center.Y - cpOffset) * h), new Point((center.X - radius) * w, center.Y * h), true, false); //UL TL
              s.BezierTo(new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false); //BL TB
              s.BezierTo(new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + radius) * w, center.Y * h), true, false); //BR TR
              s.BezierTo(new Point((center.X + radius) * w, (center.Y - cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y - radius) * h), new Point(center.X * w, (center.Y - radius) * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.GenderFemale: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.232, .136);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.782, .611);
            // First, the circle:
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              // Head:
               // Outer Circle
              double r = .375; // radius
              double cx = 0; // offset from Center x
              double cy = -.125; // offset from Center y
              double d = r * (4 * (Math.Sqrt(2) - 1) / 3);
              context.BeginFigure(new Point((.525 + cx) * w, (.5 + r + cy) * h), true, true);
              context.BezierTo(new Point((.5 + d + cx) * w, (.5 + r + cy) * h), new Point((.5 + r + cx) * w, (.5 + d + cy) * h), new Point((1 - .5 + r + cx) * w, (.5 + cy) * h), true, false);
              context.BezierTo(new Point((1 - .5 + r + cx) * w, (.5 - d + cy) * h), new Point((.5 + d + cx) * w, (.5 - r + cy) * h), new Point((.5 + cx) * w, (.5 - r + cy) * h), true, false);
              context.BezierTo(new Point((.5 - d + cx) * w, (.5 - r + cy) * h), new Point((.5 - r + cx) * w, (.5 - d + cy) * h), new Point((.5 - r + cx) * w, (.5 + cy) * h), true, false);
              context.BezierTo(new Point((.5 - r + cx) * w, (.5 + d + cy) * h), new Point((.5 - d + cx) * w, (.5 + r + cy) * h), new Point((.475 + cx) * w, (.5 + r + cy) * h), true, false);
              // Legs
              context.LineTo(new Point(.475 * w, .85 * h), true, false);
              context.LineTo(new Point(.425 * w, .85 * h), true, false);
              context.LineTo(new Point(.425 * w, .9 * h), true, false);
              context.LineTo(new Point(.475 * w, .9 * h), true, false);
              context.LineTo(new Point(.475 * w, 1 * h), true, false);
              context.LineTo(new Point(.525 * w, 1 * h), true, false);
              context.LineTo(new Point(.525 * w, .9 * h), true, false);
              context.LineTo(new Point(.575 * w, .9 * h), true, false);
              context.LineTo(new Point(.575 * w, .85 * h), true, false);
              context.LineTo(new Point(.525 * w, .85 * h), true, false);

              // Inner circle
              r = .325; // radius
              cx = 0; // offset from Center x
              cy = -.125; // offset from Center y
              d = r * (4 * (Math.Sqrt(2) - 1) / 3);
              context.BeginFigure(new Point((1 - .5 + r + cx) * w, (.5 + cy) * h), true, true);
              context.BezierTo(new Point((1 - .5 + r + cx) * w, (.5 - d + cy) * h), new Point((.5 + d + cx) * w, (.5 - r + cy) * h), new Point((.5 + cx) * w, (.5 - r + cy) * h), true, false);
              context.BezierTo(new Point((.5 - d + cx) * w, (.5 - r + cy) * h), new Point((.5 - r + cx) * w, (.5 - d + cy) * h), new Point((.5 - r + cx) * w, (.5 + cy) * h), true, false);
              context.BezierTo(new Point((.5 - r + cx) * w, (.5 + d + cy) * h), new Point((.5 - d + cx) * w, (.5 + r + cy) * h), new Point((.5 + cx) * w, (.5 + r + cy) * h), true, false);
              context.BezierTo(new Point((.5 + d + cx) * w, (.5 + r + cy) * h), new Point((.5 + r + cx) * w, (.5 + d + cy) * h), new Point((1 - .5 + r + cx) * w, (.5 + cy) * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.PlusLine: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, .5 * h), false, false);
              context.LineTo(new Point(1 * w, .5 * h), true, false);

              context.BeginFigure(new Point(.5 * w, 0), false, false);
              context.LineTo(new Point(.5 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.XLine: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 1 * h), false, false);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.BeginFigure(new Point(0, 0), false, false);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.AsteriskLine: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              double offset = .2 / Math.Sqrt(2);
              context.BeginFigure(new Point(offset * w, (1 - offset) * h), false, false);
              context.LineTo(new Point((1 - offset) * w, offset * h), true, false);
              context.BeginFigure(new Point(offset * w, offset * h), false, false);
              context.LineTo(new Point((1 - offset) * w, (1 - offset) * h), true, false);
              context.BeginFigure(new Point(0 * w, .5 * h), false, false);
              context.LineTo(new Point(1 * w, .5 * h), true, false);
              context.BeginFigure(new Point(.5 * w, 0 * h), false, false);
              context.LineTo(new Point(.5 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.CircleLine: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.146, .146);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.853, .853);

            double factor = Geo.MagicBezierFactor;
            double cpOffset = factor * .5;
            double radius = .5;
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(1 * w, radius * h), false, false);
              context.BezierTo(new Point(1 * w, (radius + cpOffset) * h), new Point((radius + cpOffset) * w, 1 * h), new Point(radius * w, 1 * h), true, false);
              context.BezierTo(new Point((radius - cpOffset) * w, 1 * h), new Point(0, (radius + cpOffset) * h), new Point(0, radius * h), true, false);
              context.BezierTo(new Point(0, (radius - cpOffset) * h), new Point((radius - cpOffset) * w, 0), new Point(radius * w, 0), true, false);
              context.BezierTo(new Point((radius + cpOffset) * w, 0), new Point(1 * w, (radius - cpOffset) * h), new Point(1 * w, radius * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Pie: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              double r = .5;
              double cx = 0; // offset from Center x
              double cy = 0; // offset from Center y
              double d = r * (4 * (Math.Sqrt(2) - 1) / 3);

              double x1 = (.5 * Math.Sqrt(2) / 2 + .5);
              double y1 = (.5 - .5 * Math.Sqrt(2) / 2);
              double x4 = .5;
              double y4 = 0;
              double x2 = .7;
              double y2 = 0;
              double x3 = .5;
              double y3 = 0;

              context.BeginFigure(new Point(x1 * w, (y1) * h), true, true);
              context.BezierTo(new Point(x2 * w, y2 * h), new Point(x3 * w, y3 * h), new Point(x4 * w, y4 * h), true, false);
              context.BezierTo(new Point((.5 - d + cx) * w, (.5 - r + cy) * h), new Point((.5 - r + cx) * w, (.5 - d + cy) * h), new Point((.5 - r + cx) * w, (.5 + cy) * h), true, false);
              context.BezierTo(new Point((.5 - r + cx) * w, (.5 + d + cy) * h), new Point((.5 - d + cx) * w, (.5 + r + cy) * h), new Point((.5 + cx) * w, (.5 + r + cy) * h), true, false);
              context.BezierTo(new Point((.5 + d + cx) * w, (.5 + r + cy) * h), new Point((.5 + r + cx) * w, (.5 + d + cy) * h), new Point((1 - .5 + r + cx) * w, (.5 + cy) * h), true, false);
              context.LineTo(new Point(.5 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.PiePiece: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(w, h), true, true);
              s.BezierTo(new Point(.99 * w, .75 * h), new Point(.94 * w, .25 * h), new Point(w * (.5 + Math.Sqrt(2) / 4), 0), true, false);
              s.LineTo(new Point(0, h), true, false);
              s.LineTo(new Point(w, h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.StopSign: {
            double part = 1 / (Math.Sqrt(2) + 2);
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(part / 2, part / 2);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1 - part / 2, 1 - part / 2);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(part * w, 0), true, true);
              context.LineTo(new Point((1 - part) * w, 0), true, false);
              context.LineTo(new Point(1 * w, part * h), true, false);
              context.LineTo(new Point(1 * w, (1 - part) * h), true, false);
              context.LineTo(new Point((1 - part) * w, 1 * h), true, false);
              context.LineTo(new Point(part * w, 1 * h), true, false);
              context.LineTo(new Point(0, (1 - part) * h), true, false);
              context.LineTo(new Point(0, part * h), true, false);
              context.LineTo(new Point(part * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LogicImplies: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0,0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.8,.5);

            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .2; // Distance the arrow folds from the right
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point((1 - param1) * w, 0 * h), false, false);
              context.LineTo(new Point(1 * w, .5 * h), true, false);
              context.LineTo(new Point((1 - param1) * w, h), true, false);

              context.BeginFigure(new Point(0, .5 * h), false, false);
              context.LineTo(new Point(w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LogicIff: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.2, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.8, .5);

            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .2; // Distance the arrow folds from the right
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point((1 - param1) * w, 0 * h), false, false);
              context.LineTo(new Point(1 * w, .5 * h), true, false);
              context.LineTo(new Point((1 - param1) * w, h), true, false);

              context.BeginFigure(new Point(0, .5 * h), false, false);
              context.LineTo(new Point(w, .5 * h), true, false);

              context.BeginFigure(new Point(param1 * w, 0), false, false);
              context.LineTo(new Point(0, .5 * h), true, false);
              context.LineTo(new Point(param1 * w, h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LogicNot: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 0), false, false);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LogicAnd: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25,.5);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75,1);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 1 * h), false, false);
              context.LineTo(new Point(.5 * w, 0), true, false);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LogicOr: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.219, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.78, .409);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 0), false, false);
              context.LineTo(new Point(.5 * w, 1 * h), true, false);
              context.LineTo(new Point(1 * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LogicXor: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(.5 * w, 0), false, false);
              context.LineTo(new Point(.5 * w, 1 * h), true, false);
              context.BeginFigure(new Point(0, .5 * h), false, false);
              context.LineTo(new Point(1 * w, .5 * h), true, false);

              double factor = Geo.MagicBezierFactor;
              double cpOffset = factor * .5;
              double radius = .5;
              context.BezierTo(new Point(1 * w, (radius + cpOffset) * h), new Point((radius + cpOffset) * w, 1 * h), new Point(radius * w, 1 * h), true, false);
              context.BezierTo(new Point((radius - cpOffset) * w, 1 * h), new Point(0, (radius + cpOffset) * h), new Point(0, radius * h), true, false);
              context.BezierTo(new Point(0, (radius - cpOffset) * h), new Point((radius - cpOffset) * w, 0), new Point(radius * w, 0), true, false);
              context.BezierTo(new Point((radius + cpOffset) * w, 0), new Point(1 * w, (radius - cpOffset) * h), new Point(1 * w, radius * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LogicTruth: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 0), false, false);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.BeginFigure(new Point(.5 * w, 0), false, false);
              context.LineTo(new Point(.5 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LogicFalsity: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 1 * h), false, false);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
              context.BeginFigure(new Point(.5 * w, 1 * h), false, false);
              context.LineTo(new Point(.5 * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LogicThereExists: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 0), false, false);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.LineTo(new Point(1 * w, .5 * h), true, false);
              context.LineTo(new Point(0, .5 * h), true, false);
              context.BeginFigure(new Point(1 * w, .5 * h), false, false);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LogicForAll: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, .5);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 0), false, false);
              context.LineTo(new Point(.5 * w, 1 * h), true, false);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.BeginFigure(new Point(.25 * w, .5 * h), false, false);
              context.LineTo(new Point(.75 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LogicIsDefinedAs: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.01, .01);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.99, .49);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 0), false, false);
              context.LineTo(new Point(w, 0), true, false);

              context.BeginFigure(new Point(0, .5 * h), false, false);
              context.LineTo(new Point(w, .5 * h), true, false);

              context.BeginFigure(new Point(0, h), false, false);
              context.LineTo(new Point(w, h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LogicIntersect: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0,.5);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1,1);

            // U shape
            double factor = Geo.MagicBezierFactor;
            double cpOffset = factor * .5;
            double radius = .5;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 1 * h), false, false);
              context.LineTo(new Point(0, radius * h), true, false);
              context.BezierTo(new Point(0, (radius - cpOffset) * h), new Point((radius - cpOffset) * w, 0), new Point(radius * w, 0), true, false);
              context.BezierTo(new Point((radius + cpOffset) * w, 0), new Point(1 * w, (radius - cpOffset) * h), new Point(1 * w, radius * h), true, false);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LogicUnion: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0,0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1,.5);

            // upside down U
            double factor = Geo.MagicBezierFactor;
            double cpOffset = factor * .5;
            double radius = .5;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(1 * w, 0), false, false);
              context.LineTo(new Point(1 * w, radius * h), true, false);
              context.BezierTo(new Point(1 * w, (radius + cpOffset) * h), new Point((radius + cpOffset) * w, 1 * h), new Point(radius * w, 1 * h), true, false);
              context.BezierTo(new Point((radius - cpOffset) * w, 1 * h), new Point(0, (radius + cpOffset) * h), new Point(0, radius * h), true, false);
              context.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Arrow: {
            double param1 = this.FigureParameter1;
            double param2 = this.FigureParameter2;
            if (param1 == 0) param1 = .3; // % from the edge the ends of the arrow are
            if (param2 == 0) param2 = .3; // Arrow width
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .5 - param2 / 2);
            Point temp = GetIntersection(new Point(0, .5 + param2 / 2), new Point(1, .5 + param2 / 2), new Point(1 - param1, 1), new Point(1, .5));
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(temp.X, temp.Y);
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, (.5 - param2 / 2) * h), true, true);
              context.LineTo(new Point((1 - param1) * w, (.5 - param2 / 2) * h), true, false);
              context.LineTo(new Point((1 - param1) * w, 0), true, false);
              context.LineTo(new Point(1 * w, .5 * h), true, false);
              context.LineTo(new Point((1 - param1) * w, 1 * h), true, false);
              context.LineTo(new Point((1 - param1) * w, (.5 + param2 / 2) * h), true, false);
              context.LineTo(new Point(0, (.5 + param2 / 2) * h), true, false);
              context.LineTo(new Point(0, (.5 - param2 / 2) * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Chevron:
        case NodeFigure.ISOProcess: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 0), true, true);
              context.LineTo(new Point(.5 * w, 0), true, false);
              context.LineTo(new Point(1 * w, .5 * h), true, false);
              context.LineTo(new Point(.5 * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
              context.LineTo(new Point(.5 * w, .5 * h), true, false);
              context.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.DoubleArrow: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, 0), true, true);
              context.LineTo(new Point(.3 * w, 0.214 * h), true, false);
              context.LineTo(new Point(.3 * w, 0), true, false);
              context.LineTo(new Point(1.0 * w, .5 * h), true, false);
              context.LineTo(new Point(.3 * w, 1.0 * h), true, false);
              context.LineTo(new Point(.3 * w, 0.786 * h), true, false);
              context.LineTo(new Point(0, 1.0 * h), true, false);

              context.BeginFigure(new Point(.3 * w, .214 * h), false, false);
              context.LineTo(new Point(.3 * w, .786 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.DoubleEndArrow: {
            Point temp1 = GetIntersection(new Point(0, .5), new Point(.3, 0), new Point(0, .3), new Point(.3, .3));
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(temp1.X, temp1.Y);
            Point temp2 = GetIntersection(new Point(.7, 1), new Point(1, .5), new Point(.7, .7), new Point(1, .7));
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(temp2.X, temp2.Y);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(1 * w, .5 * h), true, true);
              context.LineTo(new Point(.7 * w, 1 * h), true, false);
              context.LineTo(new Point(.7 * w, .7 * h), true, false);
              context.LineTo(new Point(.3 * w, .7 * h), true, false);
              context.LineTo(new Point(.3 * w, 1 * h), true, false);
              context.LineTo(new Point(0, .5 * h), true, false);
              context.LineTo(new Point(.3 * w, 0), true, false);
              context.LineTo(new Point(.3 * w, .3 * h), true, false);
              context.LineTo(new Point(.7 * w, .3 * h), true, false);
              context.LineTo(new Point(.7 * w, 0), true, false);
              context.LineTo(new Point(1 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.IBeamArrow: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .3);
            Point temp = GetIntersection(new Point(.7, 1), new Point(1, .5), new Point(.7, .7), new Point(1, .7));
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(temp.X, temp.Y);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(1 * w, .5 * h), true, true);
              context.LineTo(new Point(.7 * w, 1 * h), true, false);
              context.LineTo(new Point(.7 * w, .7 * h), true, false);
              context.LineTo(new Point(.2 * w, .7 * h), true, false);
              context.LineTo(new Point(.2 * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
              context.LineTo(new Point(0, 0), true, false);
              context.LineTo(new Point(.2 * w, 0), true, false);
              context.LineTo(new Point(.2 * w, .3 * h), true, false);
              context.LineTo(new Point(.7 * w, .3 * h), true, false);
              context.LineTo(new Point(.7 * w, 0), true, false);
              context.LineTo(new Point(1 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Pointer: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.2, .35);
            Point temp = GetIntersection(new Point(.2, .65), new Point(1, .65), new Point(0, 1), new Point(1, .5));
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(temp.X, temp.Y);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(1 * w, .5 * h), true, true);
              context.LineTo(new Point(0, 1 * h), true, false);
              context.LineTo(new Point(.2 * w, .5 * h), true, false);
              context.LineTo(new Point(0, 0), true, false);
              context.LineTo(new Point(1 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.RoundedPointer: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.4, .35);
            Point temp = GetIntersection(new Point(.2, .65), new Point(1, .65), new Point(0, 1), new Point(1, .5));
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(temp.X, temp.Y);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(1 * w, .5 * h), true, true);
              context.LineTo(new Point(0, 1 * h), true, false);
              context.BezierTo(new Point(.5 * w, .75 * h), new Point(.5 * w, .25 * h), new Point(0, 0), true, false);
              context.LineTo(new Point(1 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.SplitEndArrow: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.2, .3);
            Point temp = GetIntersection(new Point(.7, 1), new Point(1, .5), new Point(.7, .7), new Point(1, .7));
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(temp.X, temp.Y);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(1 * w, .5 * h), true, true);
              context.LineTo(new Point(.7 * w, 1 * h), true, false);
              context.LineTo(new Point(.7 * w, .7 * h), true, false);
              context.LineTo(new Point(0, .7 * h), true, false);
              context.LineTo(new Point(.2 * w, .5 * h), true, false);
              context.LineTo(new Point(0, .3 * h), true, false);
              context.LineTo(new Point(.7 * w, .3 * h), true, false);
              context.LineTo(new Point(.7 * w, 0), true, false);
              context.LineTo(new Point(1 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.SquareArrow:
        case NodeFigure.MessageToUser: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.7, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(1 * w, .5 * h), true, true);
              context.LineTo(new Point(.7 * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
              context.LineTo(new Point(0, 0), true, false);
              context.LineTo(new Point(.7 * w, 0), true, false);
              context.LineTo(new Point(1 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Cone1: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25, .5);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, .97);

            double factor = Geo.MagicBezierFactor;
            double cpxOffset = factor * .5;
            double cpyOffset = factor * .1;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, .9 * h), true, true);
              context.LineTo(new Point(.5 * w, 0), true, false);
              context.LineTo(new Point(1 * w, .9 * h), true, false);
              context.BezierTo(new Point(1 * w, (.9 + cpyOffset) * h), new Point((.5 + cpxOffset) * w, 1 * h), new Point(.5 * w, 1 * h), true, false);
              context.BezierTo(new Point((.5 - cpxOffset) * w, 1 * h), new Point(0, (.9 + cpyOffset) * h), new Point(0, .9 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Cone2: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25, .5);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, .82);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, .9 * h), true, true);
              context.BezierTo(new Point((1 - .85 / .9) * w, 1 * h), new Point((.85 / .9) * w, 1 * h), new Point(1 * w, .9 * h), true, false);

              context.LineTo(new Point(.5 * w, 0), true, false);
              context.LineTo(new Point(0, .9 * h), true, false);

              context.BeginFigure(new Point(0, .9 * h), false, false);
              context.BezierTo(new Point((1 - .85 / .9) * w, .8 * h), new Point((.85 / .9) * w, .8 * h), new Point(1 * w, .9 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Cube1: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0,.3);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.5,.85);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(.5 * w, 1 * h), true, true);
              context.LineTo(new Point(1 * w, .85 * h), true, false);
              context.LineTo(new Point(1 * w, .15 * h), true, false);
              context.LineTo(new Point(.5 * w, 0 * h), true, false);
              context.LineTo(new Point(0 * w, .15 * h), true, false);
              context.LineTo(new Point(0 * w, .85 * h), true, false);

              context.BeginFigure(new Point(.5 * w, 1 * h), false, false);
              context.LineTo(new Point(.5 * w, .3 * h), true, false);
              context.LineTo(new Point(0, .15 * h), true, false);

              context.BeginFigure(new Point(.5 * w, .3 * h), false, false);
              context.LineTo(new Point(1 * w, .15 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Cube2: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0,.3);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.7,1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, .3 * h), true, true);
              context.LineTo(new Point(0 * w, 1 * h), true, false);
              context.LineTo(new Point(.7 * w, h), true, false);
              context.LineTo(new Point(1 * w, .7 * h), true, false);
              context.LineTo(new Point(1 * w, 0 * h), true, false);
              context.LineTo(new Point(.3 * w, 0 * h), true, false);

              context.BeginFigure(new Point(0, .3 * h), false, false);
              context.LineTo(new Point(.7 * w, .3 * h), true, false);
              context.LineTo(new Point(1 * w, 0 * h), true, false);
              context.BeginFigure(new Point(.7 * w, .3 * h), false, false);
              context.LineTo(new Point(.7 * w, 1 * h), true, false);


            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Cylinder1:
        case NodeFigure.MagneticData: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0,.2);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1,.9);

            double factor = Geo.MagicBezierFactor;
            double cpxOffset = factor * .5;
            double cpyOffset = factor * .1;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              // The base (top)
              context.BeginFigure(new Point(0, .1 * h), true, true);
              context.BezierTo(new Point(0, (.1 - cpyOffset) * h), new Point((.5 - cpxOffset) * w, 0), new Point(.5 * w, 0), true, true);
              context.BezierTo(new Point((.5 + cpxOffset) * w, 0), new Point(1.0 * w, (.1 - cpyOffset) * h), new Point(1.0 * w, .1 * h), true, true);
              context.LineTo(new Point(w, .9 * h), true, false);

              //// Bottom curve
              context.BezierTo(new Point(1.0 * w, (.9 + cpyOffset) * h), new Point((.5 + cpxOffset) * w, 1.0 * h), new Point(.5 * w, 1.0 * h), true, true);
              context.BezierTo(new Point((.5 - cpxOffset) * w, 1.0 * h), new Point(0, (.9 + cpyOffset) * h), new Point(0, .9 * h), true, true);
              context.LineTo(new Point(0, .1 * h), true, false);

              context.BeginFigure(new Point(0, .1 * h), false, false);
              context.BezierTo(new Point(0, (.1 + cpyOffset) * h), new Point((.5 - cpxOffset) * w, .2 * h), new Point(.5 * w, .2 * h), true, true);
              context.BezierTo(new Point((.5 + cpxOffset) * w, .2 * h), new Point(1.0 * w, (.1 + cpyOffset) * h), new Point(1.0 * w, .1 * h), true, true);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Cylinder2: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0,.1);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1,.8);

            double factor = Geo.MagicBezierFactor;
            double cpxOffset = factor * .5;
            double cpyOffset = factor * .1;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // The base (bottom)
              s.BeginFigure(new Point(0, .9 * h), false, false);
              s.BezierTo(new Point(0, (.9 - cpyOffset) * h), new Point((.5 - cpxOffset) * w, .8 * h), new Point(.5 * w, .8 * h), true, false);
              s.BezierTo(new Point((.5 + cpxOffset) * w, .8 * h), new Point(1 * w, (.9 - cpyOffset) * h), new Point(1 * w, .9 * h), true, false);

              // The body, starting and ending bottom left
              s.BeginFigure(new Point(0, .9 * h), true, true);
              s.LineTo(new Point(0, .1 * h), true, false);
              s.BezierTo(new Point(0, (.1 - cpyOffset) * h), new Point((.5 - cpxOffset) * w, 0), new Point(.5 * w, 0), true, false);
              s.BezierTo(new Point((.5 + cpxOffset) * w, 0), new Point(1 * w, (.1 - cpyOffset) * h), new Point(1 * w, .1 * h), true, false);
              s.LineTo(new Point(1 * w, .9 * h), true, false);
              s.BezierTo(new Point(1 * w, (.9 + cpyOffset) * h), new Point((.5 + cpxOffset) * w, 1 * h), new Point(.5 * w, 1 * h), true, false);
              s.BezierTo(new Point((.5 - cpxOffset) * w, 1 * h), new Point(0, (.9 + cpyOffset) * h), new Point(0, .9 * h), true, false);

              sg.FillRule = FillRule.Nonzero;
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Cylinder3: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.2,0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.9,1);

            double factor = Geo.MagicBezierFactor;
            double cpxOffset = factor * .1;
            double cpyOffset = factor * .5;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              //cylinder line (left)
              s.BeginFigure(new Point(.1 * w, 0), false, false);
              s.BezierTo(new Point((.1 + cpxOffset) * w, 0), new Point(.2 * w, (.5 - cpyOffset) * h), new Point(.2 * w, .5 * h), true, false);
              s.BezierTo(new Point(.2 * w, (.5 + cpyOffset) * h), new Point((.1 + cpxOffset) * w, 1 * h), new Point(.1 * w, 1 * h), true, false);

              // The body, starting and ending top left
              s.BeginFigure(new Point(.1 * w, 0), true, true);
              s.LineTo(new Point(.9 * w, 0), true, false);
              s.BezierTo(new Point((.9 + cpxOffset) * w, 0), new Point(1 * w, (.5 - cpyOffset) * h), new Point(1 * w, .5 * h), true, false);
              s.BezierTo(new Point(1 * w, (.5 + cpyOffset) * h), new Point((.9 + cpxOffset) * w, 1 * h), new Point(.9 * w, 1 * h), true, false); //other side down
              s.LineTo(new Point(.1 * w, 1 * h), true, false);
              s.BezierTo(new Point((.1 - cpxOffset) * w, 1 * h), new Point(0, (.5 + cpyOffset) * h), new Point(0, .5 * h), true, false);
              s.BezierTo(new Point(0, (.5 - cpyOffset) * h), new Point((.1 - cpxOffset) * w, 0), new Point(.1 * w, 0), true, false);

              sg.FillRule = FillRule.Nonzero;
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Cylinder4:
        case NodeFigure.DirectData: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.1,0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.8,1);

            double factor = Geo.MagicBezierFactor;
            double cpxOffset = factor * .1;
            double cpyOffset = factor * .5;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // Cylinder line (right)
              s.BeginFigure(new Point(.9 * w, 0), false, false);
              s.BezierTo(new Point((.9 - cpxOffset) * w, 0), new Point(.8 * w, (.5 - cpyOffset) * h), new Point(.8 * w, .5 * h), true, false);
              s.BezierTo(new Point(.8 * w, (.5 + cpyOffset) * h), new Point((.9 - cpxOffset) * w, 1 * h), new Point(.9 * w, 1 * h), true, false);

              // The body, starting and ending top right
              s.BeginFigure(new Point(.9 * w, 0), true, false);
              s.BezierTo(new Point((.9 + cpxOffset) * w, 0), new Point(1 * w, (.5 - cpyOffset) * h), new Point(1 * w, .5 * h), true, false);
              s.BezierTo(new Point(1 * w, (.5 + cpyOffset) * h), new Point((.9 + cpxOffset) * w, 1 * h), new Point(.9 * w, 1 * h), true, false);
              s.LineTo(new Point(.1 * w, 1 * h), true, false);
              s.BezierTo(new Point((.1 - cpxOffset) * w, 1 * h), new Point(0, (.5 + cpyOffset) * h), new Point(0, .5 * h), true, false);
              s.BezierTo(new Point(0, (.5 - cpyOffset) * h), new Point((.1 - cpxOffset) * w, 0), new Point(.1 * w, 0), true, false);
              s.LineTo(new Point(.9 * w, 0), true, false);

              sg.FillRule = FillRule.Nonzero;
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Prism1: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.408,.172);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.833,.662);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(.25 * w, .25 * h), true, true);
              context.LineTo(new Point(.75 * w, 0), true, false);
              context.LineTo(new Point(w, .5 * h), true, false);
              context.LineTo(new Point(.5 * w, h), true, false);
              context.LineTo(new Point(0, h), true, false);

              context.BeginFigure(new Point(.25 * w, .25 * h), false, false);
              context.LineTo(new Point(.5 * w, h), true, false);
            }
            geo = sg.Geometry;
            break;
          }

        case NodeFigure.Prism2: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25,.5);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75,.75);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(0, .25 * h), true, true);
              context.LineTo(new Point(.75 * w, 0), true, false);
              context.LineTo(new Point(1 * w, .25 * h), true, false);
              context.LineTo(new Point(.75 * w, .75 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);

              context.BeginFigure(new Point(0, h), false, false);
              context.LineTo(new Point(.25 * w, .5 * h), true, false);
              context.LineTo(new Point(w, .25 * h), true, false);

              context.BeginFigure(new Point(0, .25 * h), false, false);
              context.LineTo(new Point(.25 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Pyramid1: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25,.367);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75,.875);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(.5 * w, 0), true, true);
              context.LineTo(new Point(w, .75 * h), true, false);
              context.LineTo(new Point(.5 * w, 1 * h), true, false);
              context.LineTo(new Point(0, .75 * h), true, false);

              context.BeginFigure(new Point(.5 * w, 0), false, false);
              context.LineTo(new Point(.5 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Pyramid2: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25, .367);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, .875);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(.5 * w, 0), true, true);
              context.LineTo(new Point(w, .85 * h), true, false);
              context.LineTo(new Point(.5 * w, 1 * h), true, false);
              context.LineTo(new Point(0, .85 * h), true, false);

              context.BeginFigure(new Point(.5 * w, 0), false, false);
              context.LineTo(new Point(.5 * w, .7 * h), true, false);
              context.LineTo(new Point(0, .85 * h), true, false);

              context.BeginFigure(new Point(.5 * w, .7 * h), false, false);
              context.LineTo(new Point(1 * w, .85 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Actor: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.2, .2);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.8, .65);

            double factor = Geo.MagicBezierFactor;
            Size radius = new Size(.2, .1);
            Size offset = new Size(factor * radius.Width, factor * radius.Height);
            Point center = new Point(.5, .1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // Head
              s.BeginFigure(new Point(center.X * w, (center.Y + radius.Height) * h), true, true);
              s.BezierTo(new Point((center.X - offset.Width) * w, (center.Y + radius.Height) * h),
                new Point((center.X - radius.Width) * w, (center.Y + offset.Height) * h), new Point((center.X - radius.Width) * w, center.Y * h), true, false);
              s.BezierTo(new Point((center.X - radius.Width) * w, (center.Y - offset.Height) * h),
                new Point((center.X - offset.Width) * w, (center.Y - radius.Height) * h), new Point(center.X * w, (center.Y - radius.Height) * h), true, false);
              s.BezierTo(new Point((center.X + offset.Width) * w, (center.Y - radius.Height) * h),
                new Point((center.X + radius.Width) * w, (center.Y - offset.Height) * h), new Point((center.X + radius.Width) * w, center.Y * h), true, false);
              s.BezierTo(new Point((center.X + radius.Width) * w, (center.Y + offset.Height) * h),
                new Point((center.X + offset.Width) * w, (center.Y + radius.Height) * h), new Point(center.X * w, (center.Y + radius.Height) * h), true, false);

              // Body
              double r = .05;
              double cpOffset = factor * r;
              center = new Point(.05, .25);

              s.BeginFigure(new Point(.5 * w, .2 * h), true, false);
              s.LineTo(new Point(.95 * w, .2 * h), true, false);
              center = new Point(.95, .25);
              // Right shoulder
              s.BezierTo(new Point((center.X + cpOffset) * w, (center.Y - r) * h), new Point((center.X + r) * w, (center.Y - cpOffset) * h), new Point((center.X + r) * w, center.Y * h), true, false);
              // Right arm
              s.LineTo(new Point(1 * w, .6 * h), true, false);
              s.LineTo(new Point(.85 * w, .6 * h), true, false);
              s.LineTo(new Point(.85 * w, .35 * h), true, false);

              // Under right arm
              r = .025;
              cpOffset = factor * r;
              center = new Point(.825, .35);
              s.BezierTo(new Point((center.X + r) * w, (center.Y - cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y - r) * h), new Point(center.X * w, (center.Y - r) * h), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y - r) * h), new Point((center.X - r) * w, (center.Y - cpOffset) * h), new Point((center.X - r) * w, center.Y * h), true, false);

              // Right side/leg
              s.LineTo(new Point(.8 * w, 1 * h), true, false);
              s.LineTo(new Point(.55 * w, 1 * h), true, false);
              s.LineTo(new Point(.55 * w, .7 * h), true, false);

              // Right in between
              r = .05;
              cpOffset = factor * r;
              center = new Point(.5, .7);
              s.BezierTo(new Point((center.X + r) * w, (center.Y - cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y - r) * h), new Point(center.X * w, (center.Y - r) * h), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y - r) * h), new Point((center.X - r) * w, (center.Y - cpOffset) * h), new Point((center.X - r) * w, center.Y * h), true, false);

              // Left side/leg
              s.LineTo(new Point(.45 * w, 1 * h), true, false);
              s.LineTo(new Point(.2 * w, 1 * h), true, false);
              s.LineTo(new Point(.2 * w, .35 * h), true, false);

              // Left under arm
              r = .025;
              cpOffset = factor * r;
              center = new Point(.175, .35);
              s.BezierTo(new Point((center.X + r) * w, (center.Y - cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y - r) * h), new Point(center.X * w, (center.Y - r) * h), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y - r) * h), new Point((center.X - r) * w, (center.Y - cpOffset) * h), new Point((center.X - r) * w, center.Y * h), true, false);

              // Left arm
              s.LineTo(new Point(.15 * w, .6 * h), true, false);
              s.LineTo(new Point(0 * w, .6 * h), true, false);
              s.LineTo(new Point(0 * w, .25 * h), true, false);

              r = .05;
              cpOffset = factor * r;
              center = new Point(.05, .25);
              // Left shoulder
              s.BezierTo(new Point((center.X - r) * w, (center.Y - cpOffset) * h), new Point((center.X - cpOffset) * w, (center.Y - r) * h), new Point(center.X * w, (center.Y - r) * h), true, false);
              s.LineTo(new Point(.5 * w, .2 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Card: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .2);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(1 * w, 0 * h), true, true);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
              context.LineTo(new Point(0 * w, 1 * h), true, false);
              context.LineTo(new Point(0 * w, .2 * h), true, false);
              context.LineTo(new Point(.2 * w, 0 * h), true, false);
              context.LineTo(new Point(1 * w, 0 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Collate: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25,0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75,.25);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              context.BeginFigure(new Point(.5 * w, .5 * h), true, true);
              context.LineTo(new Point(0, 0), true, false);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.LineTo(new Point(.5 * w, .5 * h), true, false);

              context.BeginFigure(new Point(.5 * w, .5 * h), true, true);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
              context.LineTo(new Point(.5 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.CreateRequest: {
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .1;
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, param1);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, 1 - param1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext context = sg.Open()) {
              // Inside lines
              context.BeginFigure(new Point(0, param1 * h), false, false);
              context.LineTo(new Point(1 * w, param1 * h), true, false);
              context.BeginFigure(new Point(0, (1 - param1) * h), false, false);
              context.LineTo(new Point(1 * w, (1 - param1) * h), true, false);

              // Body
              context.BeginFigure(new Point(0, 0), true, true);
              context.LineTo(new Point(1 * w, 0), true, false);
              context.LineTo(new Point(1 * w, 1 * h), true, false);
              context.LineTo(new Point(0, 1 * h), true, false);
              context.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Database: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .4);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, .9);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              double factor = Geo.MagicBezierFactor;
              double cpxOffset = factor * .5;
              double cpyOffset = factor * .1;
              // Rings
              s.BeginFigure(new Point(1 * w, .1 * h), false, false);
              s.BezierTo(new Point(1 * w, (.1 + cpyOffset) * h), new Point((.5 + cpxOffset) * w, .2 * h), new Point(.5 * w, .2 * h), true, false);
              s.BezierTo(new Point((.5 - cpxOffset) * w, .2 * h), new Point(0, (.1 + cpyOffset) * h), new Point(0, .1 * h), true, false);
              s.BeginFigure(new Point(1 * w, .2 * h), false, false);
              s.BezierTo(new Point(1 * w, (.2 + cpyOffset) * h), new Point((.5 + cpxOffset) * w, .3 * h), new Point(.5 * w, .3 * h), true, false);
              s.BezierTo(new Point((.5 - cpxOffset) * w, .3 * h), new Point(0, (.2 + cpyOffset) * h), new Point(0, .2 * h), true, false);
              s.BeginFigure(new Point(1 * w, .3 * h), false, false);
              s.BezierTo(new Point(1 * w, (.3 + cpyOffset) * h), new Point((.5 + cpxOffset) * w, .4 * h), new Point(.5 * w, .4 * h), true, false);
              s.BezierTo(new Point((.5 - cpxOffset) * w, .4 * h), new Point(0, (.3 + cpyOffset) * h), new Point(0, .3 * h), true, false);

              // Body
              s.BeginFigure(new Point(1 * w, .1 * h), true, false);
              s.LineTo(new Point(1 * w, .9 * h), true, false);
              s.BezierTo(new Point(1 * w, (.9 + cpyOffset) * h), new Point((.5 + cpxOffset) * w, 1 * h), new Point(.5 * w, 1 * h), true, false);
              s.BezierTo(new Point((.5 - cpxOffset) * w, 1 * h), new Point(0, (.9 + cpyOffset) * h), new Point(0, .9 * h), true, false);
              s.LineTo(new Point(0, .1 * h), true, false);
              s.BezierTo(new Point(0, (.1 - cpyOffset) * h), new Point((.5 - cpxOffset) * w, 0), new Point(.5 * w, 0), true, false);
              s.BezierTo(new Point((.5 + cpxOffset) * w, 0), new Point(1 * w, (.1 - cpyOffset) * h), new Point(1 * w, .1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.DataStorage: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.226, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.9, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(.75 * w, 0), true, false);
              s.BezierTo(new Point(1 * w, 0), new Point(1 * w, 1 * h), new Point(.75 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.BezierTo(new Point(.25 * w, .9 * h), new Point(.25 * w, .1 * h), new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.DiskStorage: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .3);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, .9);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              double factor = Geo.MagicBezierFactor;
              double cpxOffset = factor * .5;
              double cpyOffset = factor * .1;
              // Rings
              s.BeginFigure(new Point(1 * w, .1 * h), false, false);
              s.BezierTo(new Point(1 * w, (.1 + cpyOffset) * h), new Point((.5 + cpxOffset) * w, .2 * h), new Point(.5 * w, .2 * h), true, false);
              s.BezierTo(new Point((.5 - cpxOffset) * w, .2 * h), new Point(0, (.1 + cpyOffset) * h), new Point(0, .1 * h), true, false);
              s.BeginFigure(new Point(1 * w, .2 * h), false, false);
              s.BezierTo(new Point(1 * w, (.2 + cpyOffset) * h), new Point((.5 + cpxOffset) * w, .3 * h), new Point(.5 * w, .3 * h), true, false);
              s.BezierTo(new Point((.5 - cpxOffset) * w, .3 * h), new Point(0, (.2 + cpyOffset) * h), new Point(0, .2 * h), true, false);

              // Body
              s.BeginFigure(new Point(1 * w, .1 * h), true, false);
              s.LineTo(new Point(1 * w, .9 * h), true, false);
              s.BezierTo(new Point(1 * w, (.9 + cpyOffset) * h), new Point((.5 + cpxOffset) * w, 1 * h), new Point(.5 * w, 1 * h), true, false);
              s.BezierTo(new Point((.5 - cpxOffset) * w, 1 * h), new Point(0, (.9 + cpyOffset) * h), new Point(0, .9 * h), true, false);
              s.LineTo(new Point(0, .1 * h), true, false);
              s.BezierTo(new Point(0, (.1 - cpyOffset) * h), new Point((.5 - cpxOffset) * w, 0), new Point(.5 * w, 0), true, false);
              s.BezierTo(new Point((.5 + cpxOffset) * w, 0), new Point(1 * w, (.1 - cpyOffset) * h), new Point(1 * w, .1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Display: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(.25 * w, 0), true, true);
              s.LineTo(new Point(.75 * w, 0), true, false);
              s.BezierTo(new Point(1 * w, 0), new Point(1 * w, 1 * h), new Point(.75 * w, 1 * h), true, false);
              s.LineTo(new Point(.25 * w, 1 * h), true, false);
              s.LineTo(new Point(0, .5 * h), true, false);
              s.LineTo(new Point(.25 * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.DividedEvent: {
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .2;
            else if (param1 < .15) param1 = .15; // Minimum
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, param1);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, 1 - param1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              double factor = Geo.MagicBezierFactor;
              double cpOffset = factor * .2;
              s.BeginFigure(new Point(0, param1 * h), false, false);
              s.LineTo(new Point(1 * w, param1 * h), true, false);

              s.BeginFigure(new Point(0, .2 * h), true, true);
              s.BezierTo(new Point(0, (.2 - cpOffset) * h), new Point((.2 - cpOffset) * w, 0), new Point(.2 * w, 0), true, false);
              s.LineTo(new Point(.8 * w, 0), true, false);
              s.BezierTo(new Point((.8 + cpOffset) * w, 0), new Point(1 * w, (.2 - cpOffset) * h), new Point(1 * w, .2 * h), true, false);
              s.LineTo(new Point(1 * w, .8 * h), true, false);
              s.BezierTo(new Point(1 * w, (.8 + cpOffset) * h), new Point((.8 + cpOffset) * w, 1 * h), new Point(.8 * w, 1 * h), true, false);
              s.LineTo(new Point(.2 * w, 1 * h), true, false);
              s.BezierTo(new Point((.2 - cpOffset) * w, 1 * h), new Point(0, (.8 + cpOffset) * h), new Point(0, .8 * h), true, false);
              s.LineTo(new Point(0, .2 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.DividedProcess: {
            double param1 = this.FigureParameter1;
            if (param1 < .1) param1 = .1; // Minimum
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, param1);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, param1 * h), false, false);
              s.LineTo(new Point(1 * w, param1 * h), true, false);

              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Document: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, .6);

            h = h / .8;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, .7 * h), true, true);
              s.LineTo(new Point(0, 0), true, false);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point(1 * w, .7 * h), true, false);
              s.BezierTo(new Point(.5 * w, .4 * h), new Point(.5 * w, 1 * h), new Point(0, .7 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.ExternalOrganization: {
            double param1 = this.FigureParameter1;
            if (param1 < .2) param1 = .2; // Minimum

            if (this.Spot1.IsDefault) this.Spot1 = new Spot(param1 / 2, param1 / 2);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1 - param1 / 2, 1 - param1 / 2);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // Top left triangle
              s.BeginFigure(new Point(param1 * w, 0), false, false);
              s.LineTo(new Point(0, param1 * h), true, false);
              // Top right triangle
              s.BeginFigure(new Point(1 * w, param1 * h), false, false);
              s.LineTo(new Point((1 - param1) * w, 0), true, false);
              // Bottom left triangle
              s.BeginFigure(new Point(0, (1 - param1) * h), false, false);
              s.LineTo(new Point(param1 * w, 1 * h), true, false);
              // Bottom Right triangle
              s.BeginFigure(new Point((1 - param1) * w, 1 * h), false, false);
              s.LineTo(new Point(1 * w, (1 - param1) * h), true, false);
              // Body
              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.ExternalProcess: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25, .25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, .75);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // Top left triangle
              s.BeginFigure(new Point(.1 * w, .4 * h), false, false);
              s.LineTo(new Point(.1 * w, .6 * h), true, false);
              // Top right triangle
              s.BeginFigure(new Point(.9 * w, .6 * h), false, false);
              s.LineTo(new Point(.9 * w, .4 * h), true, false);
              // Bottom left triangle
              s.BeginFigure(new Point(.6 * w, .1 * h), false, false);
              s.LineTo(new Point(.4 * w, .1 * h), true, false);
              // Bottom Right triangle
              s.BeginFigure(new Point(.4 * w, .9 * h), false, false);
              s.LineTo(new Point(.6 * w, .9 * h), true, false);
              // Body
              s.BeginFigure(new Point(.5 * w, 0), true, true);
              s.LineTo(new Point(1 * w, .5 * h), true, false);
              s.LineTo(new Point(.5 * w, 1 * h), true, false);
              s.LineTo(new Point(0, .5 * h), true, false);
              s.LineTo(new Point(.5 * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.File: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // The fold
              s.BeginFigure(new Point(.75 * w, 0), false, false);
              s.LineTo(new Point(.75 * w, .25 * h), true, false);
              s.LineTo(new Point(1 * w, .25 * h), true, false);

              // Body
              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(.75 * w, 0), true, false);
              s.LineTo(new Point(1 * w, .25 * h), true, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Interupt: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.5, .75);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(1 * w, .5 * h), false, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.BeginFigure(new Point(1 * w, .5 * h), false, false);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.BeginFigure(new Point(1 * w, .5 * h), true, true);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
              s.LineTo(new Point(1 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.InternalStorage: {
            double param1 = this.FigureParameter1;
            double param2 = this.FigureParameter2;
            if (param1 == 0) param1 = .1; // Distance from left
            if (param2 == 0) param2 = .1; // Distance from top
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(param1, param2);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // Two lines
              s.BeginFigure(new Point(param1 * w, 0), false, false);
              s.LineTo(new Point(param1 * w, 1 * h), true, false);
              s.BeginFigure(new Point(0, param2 * h), false, false);
              s.LineTo(new Point(1 * w, param2 * h), true, false);

              // The main body
              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Junction: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // X in circle
              double factor = Geo.MagicBezierFactor;
              double dist = (double)(1 / Math.Sqrt(2));
              double small = (double)((1 - 1 / Math.Sqrt(2)) / 2);
              double cpOffset = factor * .5;
              double radius = .5;
              // X
              s.BeginFigure(new Point((small + dist) * w, (small + dist) * h), false, false);
              s.LineTo(new Point(small * w, small * h), true, false);
              s.BeginFigure(new Point(small * w, (small + dist) * h), false, false);
              s.LineTo(new Point((small + dist) * w, small * h), true, false);

              // Circle
              s.BeginFigure(new Point(1 * w, radius * h), true, true);
              s.BezierTo(new Point(1 * w, (radius + cpOffset) * h), new Point((radius + cpOffset) * w, 1 * h), new Point(radius * w, 1 * h), true, false);
              s.BezierTo(new Point((radius - cpOffset) * w, 1 * h), new Point(0, (radius + cpOffset) * h), new Point(0, radius * h), true, false);
              s.BezierTo(new Point(0, (radius - cpOffset) * h), new Point((radius - cpOffset) * w, 0), new Point(radius * w, 0), true, false);
              s.BezierTo(new Point((radius + cpOffset) * w, 0), new Point(1 * w, (radius - cpOffset) * h), new Point(1 * w, radius * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LinedDocument: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.1, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, .6);

            h = h / .8;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(.1 * w, 0), false, false);
              s.LineTo(new Point(.1 * w, .75 * h), true, false);

              s.BeginFigure(new Point(0, .7 * h), true, true);
              s.LineTo(new Point(0, 0), true, false);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point(1 * w, .7 * h), true, false);
              s.BezierTo(new Point(.5 * w, .4 * h), new Point(.5 * w, 1 * h), new Point(0, .7 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.LoopLimit: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, 1 * h), true, true);
              s.LineTo(new Point(0, .25 * h), true, false);
              s.LineTo(new Point(.25 * w, 0), true, false);
              s.LineTo(new Point(.75 * w, 0), true, false);
              s.LineTo(new Point(1 * w, .25 * h), true, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.MagneticTape:
        case NodeFigure.SequentialData: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.15, .15);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.85, .8);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              double factor = Geo.MagicBezierFactor;
              double cpOffset = factor * .5;
              double radius = .5;

              s.BeginFigure(new Point(.5 * w, 1 * h), true, true);
              s.BezierTo(new Point((radius - cpOffset) * w, 1 * h), new Point(0, (radius + cpOffset) * h), new Point(0, radius * h), true, false);
              s.BezierTo(new Point(0, (radius - cpOffset) * h), new Point((radius - cpOffset) * w, 0), new Point(radius * w, 0), true, false);
              s.BezierTo(new Point((radius + cpOffset) * w, 0), new Point(1 * w, (radius - cpOffset) * h), new Point(1 * w, radius * h), true, false);
              s.BezierTo(new Point(1 * w, (radius + cpOffset) * h), new Point((radius + cpOffset) * w, .9 * h), new Point((radius + .1) * w, .9 * h), true, false);
              s.LineTo(new Point(1 * w, .9 * h), true, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.LineTo(new Point(.5 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.ManualInput: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(1 * w, 0), true, true);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, .25 * h), true, false);
              s.LineTo(new Point(1 * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.MessageFromUser: {
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .7; // How far from the right the point is
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(param1, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point(param1 * w, .5 * h), true, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.MicroformProcessing: {
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .25; // How far from the top/bottom the points are
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, param1);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, 1 - param1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(.5 * w, param1 * h), true, false);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.LineTo(new Point(.5 * w, (1 - param1) * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.MicroformRecording: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, .75);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(.75 * w, .25 * h), true, false);
              s.LineTo(new Point(1 * w, .15 * h), true, false);
              s.LineTo(new Point(1 * w, .85 * h), true, false);
              s.LineTo(new Point(.75 * w, .75 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.MultiDocument: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.8, .77);

            h = h / .8;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // Outline
              s.BeginFigure(new Point(w, 0), true, true);
              s.LineTo(new Point(w, .5 * h), true, false);
              s.BezierTo(new Point(.96 * w, .47 * h), new Point(.93 * w, .45 * h), new Point(.9 * w, .44 * h), true, false);
              s.LineTo(new Point(.9 * w, .6 * h), true, false);
              s.BezierTo(new Point(.86 * w, .57 * h), new Point(.83 * w, .55 * h), new Point(.8 * w, .54 * h), true, false);
              s.LineTo(new Point(.8 * w, .7 * h), true, false);
              s.BezierTo(new Point(.4 * w, .4 * h), new Point(.4 * w, 1 * h), new Point(0, .7 * h), true, false);
              s.LineTo(new Point(0, .2 * h), true, false);
              s.LineTo(new Point(.1 * w, .2 * h), true, false);
              s.LineTo(new Point(.1 * w, .1 * h), true, false);
              s.LineTo(new Point(.2 * w, .1 * h), true, false);
              s.LineTo(new Point(.2 * w, 0), true, false);
              s.LineTo(new Point(w, .0), true, false);

              // Inside lines
              s.BeginFigure(new Point(.1 * w, .2 * h), false, false);
              s.LineTo(new Point(.8 * w, .2 * h), true, false);
              s.LineTo(new Point(.8 * w, .54 * h), true, false);

              s.BeginFigure(new Point(.2 * w, .1 * h), false, false);
              s.LineTo(new Point(.9 * w, .1 * h), true, false);
              s.LineTo(new Point(.9 * w, .44 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.MultiProcess: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .2);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.8, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(.2 * w, .1 * h), false, false);
              s.LineTo(new Point(.9 * w, .1 * h), true, false);
              s.LineTo(new Point(.9 * w, .8 * h), true, false);
              s.BeginFigure(new Point(.1 * w, .2 * h), false, false);
              s.LineTo(new Point(.8 * w, .2 * h), true, false);
              s.LineTo(new Point(.8 * w, .9 * h), true, false);

              s.BeginFigure(new Point(.1 * w, .1 * h), true, true);
              s.LineTo(new Point(.2 * w, .1 * h), true, false);
              s.LineTo(new Point(.2 * w, 0), true, false);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point(1 * w, .8 * h), true, false);
              s.LineTo(new Point(.9 * w, .8 * h), true, false);
              s.LineTo(new Point(.9 * w, .9 * h), true, false);
              s.LineTo(new Point(.8 * w, .9 * h), true, false);
              s.LineTo(new Point(.8 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, .2 * h), true, false);
              s.LineTo(new Point(.1 * w, .2 * h), true, false);
              s.LineTo(new Point(.1 * w, .1 * h), true, false);

              sg.FillRule = FillRule.Nonzero;
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.OfflineStorage: {
            if (FigureParameter1 == 0) FigureParameter1 = .1; // Distance between 2 top lines
            double l = 1 - FigureParameter1; // Length of the top line
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(l / 4 + .5 * FigureParameter1, FigureParameter1);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(3 * l / 4 + .5 * FigureParameter1, FigureParameter1 + .5 * l);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(.5 * FigureParameter1 * w, FigureParameter1 * h), false, false);
              s.LineTo(new Point((1 - .5 * FigureParameter1) * w, FigureParameter1 * h), true, false);

              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point(.5 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.OffPageConnector: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(.75 * w, 0), true, false);
              s.LineTo(new Point(1 * w, .5 * h), true, false);
              s.LineTo(new Point(.75 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Or: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // + in circle
              double factor = Geo.MagicBezierFactor;
              double cpOffset = factor * .5;
              double radius = .5;
              // +
              s.BeginFigure(new Point(1 * w, .5 * h), false, false);
              s.LineTo(new Point(0, .5 * h), true, false);
              s.BeginFigure(new Point(.5 * w, 1 * h), false, false);
              s.LineTo(new Point(.5 * w, 0), true, false);

              // Circle
              s.BeginFigure(new Point(1 * w, radius * h), true, true);
              s.BezierTo(new Point(1 * w, (radius + cpOffset) * h), new Point((radius + cpOffset) * w, 1 * h), new Point(radius * w, 1 * h), true, false);
              s.BezierTo(new Point((radius - cpOffset) * w, 1 * h), new Point(0, (radius + cpOffset) * h), new Point(0, radius * h), true, false);
              s.BezierTo(new Point(0, (radius - cpOffset) * h), new Point((radius - cpOffset) * w, 0), new Point(radius * w, 0), true, false);
              s.BezierTo(new Point((radius + cpOffset) * w, 0), new Point(1 * w, (radius - cpOffset) * h), new Point(1 * w, radius * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.PaperTape: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .49);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, .75);

            h = h / .8;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, .7 * h), true, true);
              s.LineTo(new Point(0, .3 * h), true, false);
              s.BezierTo(new Point(.5 * w, .6 * h), new Point(.5 * w, 0), new Point(1 * w, .3), true, false);
              s.LineTo(new Point(1 * w, .7 * h), true, false);
              s.BezierTo(new Point(.5 * w, .4 * h), new Point(.5 * w, 1 * h), new Point(0, .7 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.PrimitiveFromCall: {
            double param1 = this.FigureParameter1;
            double param2 = this.FigureParameter2;
            if (param1 == 0) param1 = .1; // Distance of left line from left
            if (param2 == 0) param2 = .3; // Distance of point from right
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(param1, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1 - param2, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // Left Rectangle
              s.BeginFigure(new Point(param1 * w, 0), false, false);
              s.LineTo(new Point(param1 * w, 1 * h), true, false);

              // Body
              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point((1 - param2) * w, .5 * h), true, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.PrimitiveToCall: {
            double param1 = this.FigureParameter1;
            double param2 = this.FigureParameter2;
            if (param1 == 0) param1 = .1; // Distance of left line from left
            if (param2 == 0) param2 = .3; // Distance of top and bottom right corners from right
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(param1, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1 - param2, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // Left Rectangle
              s.BeginFigure(new Point(param1 * w, 0), false, false);
              s.LineTo(new Point(param1 * w, 1 * h), true, false);

              // Body
              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point((1 - param2) * w, 0), true, false);
              s.LineTo(new Point(1 * w, .5 * h), true, false);
              s.LineTo(new Point((1 - param2) * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Procedure:
        case NodeFigure.Subroutine: {
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .1; // Distance of left  and right lines from edge
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(param1, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1 - param1, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point((1 - param1) * w, 0), false, false);
              s.LineTo(new Point((1 - param1) * w, 1 * h), true, false);
              s.BeginFigure(new Point(param1 * w, 0), false, false);
              s.LineTo(new Point(param1 * w, 1 * h), true, false);

              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Process: {
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .1; // Distance of left  line from left edge
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(param1, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(param1 * w, 0), false, false);
              s.LineTo(new Point(param1 * w, 1 * h), true, false);

              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Sort: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25,.25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75,.5);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, .5 * h), false, false);
              s.LineTo(new Point(1 * w, .5 * h), true, false);

              s.BeginFigure(new Point(.5 * w, 0), true, true);
              s.LineTo(new Point(1 * w, .5 * h), true, false);
              s.LineTo(new Point(.5 * w, 1 * h), true, false);
              s.LineTo(new Point(0, .5 * h), true, false);
              s.LineTo(new Point(.5 * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Start: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(.25 * w, 0), false, false);
              s.LineTo(new Point(.25 * w, 1 * h), true, false);
              s.BeginFigure(new Point(.75 * w, 0), false, false);
              s.LineTo(new Point(.75 * w, 1 * h), true, false);

              s.BeginFigure(new Point(.25 * w, 0), true, true);
              s.LineTo(new Point(.75 * w, 0), true, false);
              s.BezierTo(new Point(1 * w, 0), new Point(1 * w, 1 * h), new Point(.75 * w, 1 * h), true, false);
              s.LineTo(new Point(.25 * w, 1 * h), true, false);
              s.BezierTo(new Point(0, 1 * h), new Point(0, 0), new Point(.25 * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.StoredData: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.226, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.81, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(.75 * w, 0), true, false);
              s.BezierTo(new Point(1 * w, 0), new Point(1 * w, 1 * h), new Point(.75 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.BezierTo(new Point(.25 * w, .9 * h), new Point(.25 * w, .1 * h), new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Terminator: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.23, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.77, 1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(.25 * w, 0), true, false);
              s.LineTo(new Point(.75 * w, 0), true, false);
              s.BezierTo(new Point(1 * w, 0), new Point(1 * w, 1 * h), new Point(.75 * w, 1 * h), true, false);
              s.LineTo(new Point(.25 * w, 1 * h), true, false);
              s.BezierTo(new Point(0, 1 * h), new Point(0, 0), new Point(.25 * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.TransmittalTape: {
            double param1 = this.FigureParameter1;
            if (param1 == 0) param1 = .1; // Bottom line's distance from the point on the triangle
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, 0);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(1, 1 - param1);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.LineTo(new Point(.75 * w, (1 - param1) * h), true, false);
              s.LineTo(new Point(0, (1 - param1) * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }

        // Digital Circuits
        case NodeFigure.AndGate: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, 0.05);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(0.8, 0.95);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              double factor = Geo.MagicBezierFactor;
              double cpOffset = factor * .5;

              // The gate body
              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(.5 * w, 0), true, false);
              s.BezierTo(new Point((.5 + cpOffset) * w, 0), new Point(1 * w, (.5 - cpOffset) * h), new Point(1 * w, .5 * h), true, false);
              s.BezierTo(new Point(1 * w, (.5 + cpOffset) * h), new Point((.5 + cpOffset) * w, 1 * h), new Point(.5 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Buffer: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.5, .75);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(1 * w, .5 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Clock: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              double factor = Geo.MagicBezierFactor;
              double cpOffset = factor * .5;
              double radius = .5;
              // Inside clock
              // This first line solves a GDI+ graphical error with
              // more complex gradient brushes.
              s.BeginFigure(new Point(1 * w, radius * h), false, false);
              s.LineTo(new Point(1 * w, radius * h), true, false);

              s.BeginFigure(new Point(.8 * w, .75 * h), false, false);
              s.LineTo(new Point(.8 * w, .25 * h), true, false);
              s.LineTo(new Point(.6 * w, .25 * h), true, false);
              s.LineTo(new Point(.6 * w, .75 * h), true, false);
              s.LineTo(new Point(.4 * w, .75 * h), true, false);
              s.LineTo(new Point(.4 * w, .25 * h), true, false);
              s.LineTo(new Point(.2 * w, .25 * h), true, false);
              s.LineTo(new Point(.2 * w, .75 * h), true, false);

              // Ellipse
              s.BeginFigure(new Point(1 * w, radius * h), true, true);
              s.BezierTo(new Point(1 * w, (radius + cpOffset) * h), new Point((radius + cpOffset) * w, 1 * h), new Point(radius * w, 1 * h), true, false);
              s.BezierTo(new Point((radius - cpOffset) * w, 1 * h), new Point(0, (radius + cpOffset) * h), new Point(0, radius * h), true, false);
              s.BezierTo(new Point(0, (radius - cpOffset) * h), new Point((radius - cpOffset) * w, 0), new Point(radius * w, 0), true, false);
              s.BezierTo(new Point((radius + cpOffset) * w, 0), new Point(1 * w, (radius - cpOffset) * h), new Point(1 * w, radius * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Ground: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(.5 * w, 0), false, false);
              s.LineTo(new Point(.5 * w, .4 * h), true, false);
              s.BeginFigure(new Point(.2 * w, .6 * h), false, false);
              s.LineTo(new Point(.8 * w, .6 * h), true, false);
              s.BeginFigure(new Point(.3 * w, .8 * h), false, false);
              s.LineTo(new Point(.7 * w, .8 * h), true, false);
              s.BeginFigure(new Point(.4 * w, 1 * h), false, false);
              s.LineTo(new Point(.6 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Inverter: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.4, .75);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // Inversion
              double factor = Geo.MagicBezierFactor;
              double cpOffset = factor * .1;
              double radius = .1;
              Point center = new Point(.9, .5);

              s.BeginFigure(new Point((center.X + radius) * w, center.Y * h), true, true);
              s.BezierTo(new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X - radius) * w, center.Y * h), true, false);
              s.BezierTo(new Point((center.X - radius) * w, (center.Y - cpOffset) * h), new Point((center.X - cpOffset) * w, (center.Y - radius) * h), new Point(center.X * w, (center.Y - radius) * h), true, false);
              s.BezierTo(new Point((center.X + cpOffset) * w, (center.Y - radius) * h), new Point((center.X + radius) * w, (center.Y - cpOffset) * h), new Point((center.X + radius) * w, center.Y * h), true, false);

              // Triangle
              s.BeginFigure(new Point(.8 * w, .5 * h), true, true);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
              s.LineTo(new Point(.8 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.NandGate: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, 0.05);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(0.8, 0.95);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              double factor = Geo.MagicBezierFactor;
              double cpxOffset = factor * .5;
              double cpyOffset = factor * .4;

              // Inversion
              double cpOffset = factor * .1;
              double radius = .1;
              Point center = new Point(.9, .5);

              s.BeginFigure(new Point((center.X + radius) * w, center.Y * h), true, true);
              s.BezierTo(new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X - radius) * w, center.Y * h), true, false);
              s.BezierTo(new Point((center.X - radius) * w, (center.Y - cpOffset) * h), new Point((center.X - cpOffset) * w, (center.Y - radius) * h), new Point(center.X * w, (center.Y - radius) * h), true, false);
              s.BezierTo(new Point((center.X + cpOffset) * w, (center.Y - radius) * h), new Point((center.X + radius) * w, (center.Y - cpOffset) * h), new Point((center.X + radius) * w, (center.Y) * h), true, false);

              // The gate body
              s.BeginFigure(new Point(.8 * w, .5 * h), true, true);
              s.BezierTo(new Point(.8 * w, (.5 + cpyOffset) * h), new Point((.4 + cpxOffset) * w, 1 * h), new Point(.4 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
              s.LineTo(new Point(.4 * w, 0), true, false);
              s.BezierTo(new Point((.4 + cpxOffset) * w, 0), new Point(.8 * w, (.5 - cpyOffset) * h), new Point(.8 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.NorGate: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.2, .25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.6, .75);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              double factor = Geo.MagicBezierFactor;

              // Inversion
              double cpOffset = factor * .1;
              double radius = .1;
              Point center = new Point(.9, .5);

              s.BeginFigure(new Point((center.X - radius) * w, center.Y * h), true, true);
              s.BezierTo(new Point((center.X - radius) * w, (center.Y - cpOffset) * h), new Point((center.X - cpOffset) * w, (center.Y - radius) * h), new Point(center.X * w, (center.Y - radius) * h), true, false);
              s.BezierTo(new Point((center.X + cpOffset) * w, (center.Y - radius) * h), new Point((center.X + radius) * w, (center.Y - cpOffset) * h), new Point((center.X + radius) * w, center.Y * h), true, false);
              s.BezierTo(new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X - radius) * w, center.Y * h), true, false);

              // Normal
              radius = .5;
              cpOffset = factor * radius;
              center = new Point(0, .5);
              s.BeginFigure(new Point(.8 * w, .5 * h), true, true);

              s.BezierTo(new Point(.7 * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(0, 1 * h), true, false);
              s.BezierTo(new Point(.25 * w, .75 * h), new Point(.25 * w, .25 * h), new Point(0, 0), true, false);
              s.BezierTo(new Point((center.X + cpOffset) * w, (center.Y - radius) * h), new Point(.7 * w, (center.Y - cpOffset) * h), new Point(.8 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.OrGate: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.2, .25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75, .75);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              double factor = Geo.MagicBezierFactor;
              double radius = .5;
              double cpOffset = factor * radius;
              Point center = new Point(0, .5);

              s.BeginFigure(new Point(0, 0), true, true);
              s.BezierTo(new Point((center.X + cpOffset + cpOffset) * w, (center.Y - radius) * h), new Point(.8 * w, (center.Y - cpOffset) * h), new Point(1 * w, .5 * h), true, false);
              s.BezierTo(new Point(.8 * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset + cpOffset) * w, (center.Y + radius) * h), new Point(0, 1 * h), true, false);
              s.BezierTo(new Point(.25 * w, .75 * h), new Point(.25 * w, .25 * h), new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.XnorGate: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              if (this.Spot1.IsDefault) this.Spot1 = new Spot(.4, .25);
              if (this.Spot2.IsDefault) this.Spot2 = new Spot(.65, .75);

              double factor = Geo.MagicBezierFactor;

              // Inversion
              double cpOffset = factor * .1;
              double radius = .1;
              Point center = new Point(.9, .5);
              s.BeginFigure(new Point((center.X - radius) * w, center.Y * h), true, true);
              s.BezierTo(new Point((center.X - radius) * w, (center.Y - cpOffset) * h), new Point((center.X - cpOffset) * w, (center.Y - radius) * h), new Point(center.X * w, (center.Y - radius) * h), true, false);
              s.BezierTo(new Point((center.X + cpOffset) * w, (center.Y - radius) * h), new Point((center.X + radius) * w, (center.Y - cpOffset) * h), new Point((center.X + radius) * w, center.Y * h), true, false);
              s.BezierTo(new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X - radius) * w, center.Y * h), true, false);

              // Normal
              radius = .5;
              cpOffset = factor * radius;
              center = new Point(.2, .5);
              s.BeginFigure(new Point(.1 * w, 0), false, false);
              s.BezierTo(new Point(.35 * w, .25 * h), new Point(.35 * w, .75 * h), new Point(.1 * w, 1 * h), true, false);

              s.BeginFigure(new Point(.8 * w, .5 * h), true, true);
              s.BezierTo(new Point(.7 * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(.2 * w, 1 * h), true, false);
              s.BezierTo(new Point(.45 * w, .75 * h), new Point(.45 * w, .25 * h), new Point(.2 * w, 0), true, false);
              s.BezierTo(new Point((center.X + cpOffset) * w, (center.Y - radius) * h), new Point(.7 * w, (center.Y - cpOffset) * h), new Point(.8 * w, .5 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.XorGate: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.4, .25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.8, .75);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              double factor = Geo.MagicBezierFactor;
              double radius = .5;
              double cpOffset = factor * radius;
              Point center = new Point(.2, .5);

              s.BeginFigure(new Point(.1 * w, 0), false, false);
              s.BezierTo(new Point(.35 * w, .25 * h), new Point(.35 * w, .75 * h), new Point(.1 * w, 1 * h), true, false);

              s.BeginFigure(new Point(.2 * w, 0), true, true);
              s.BezierTo(new Point((center.X + cpOffset) * w, (center.Y - radius) * h), new Point(.9 * w, (center.Y - cpOffset) * h), new Point(1 * w, .5 * h), true, false);
              s.BezierTo(new Point(.9 * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(.2 * w, 1 * h), true, false);
              s.BezierTo(new Point(.45 * w, .75 * h), new Point(.45 * w, .25 * h), new Point(.2 * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Capacitor: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // Two vertical Lines
              s.BeginFigure(new Point(0, 0), false, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.BeginFigure(new Point(1 * w, 0), false, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Resistor: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, .5 * h), false, false);
              s.LineTo(new Point(.1 * w, 0), true, false);
              s.LineTo(new Point(.2 * w, 1 * h), true, false);
              s.LineTo(new Point(.3 * w, 0), true, false);
              s.LineTo(new Point(.4 * w, 1 * h), true, false);
              s.LineTo(new Point(.5 * w, 0), true, false);
              s.LineTo(new Point(.6 * w, 1 * h), true, false);
              s.LineTo(new Point(.7 * w, .5 * h), true, false); // I only go from 0 to .7, letting bounds resize it accordingly
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Inductor: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // 3 loops and two ends
              double factor = Geo.MagicBezierFactor;
              double cpOffset = factor * .1;
              double radius = .1;
              Point center = new Point(.1, .5);
              s.BeginFigure(new Point((center.X - cpOffset * .5) * w, (center.Y + radius) * h), false, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X + radius) * w, (center.Y + cpOffset) * h), true, false);

              center = new Point(.3, .5);
              s.BezierTo(new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X + radius) * w, (center.Y + cpOffset) * h), true, false);

              center = new Point(.5, .5);
              s.BezierTo(new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X + radius) * w, (center.Y + cpOffset) * h), true, false);

              center = new Point(.7, .5);
              s.BezierTo(new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X + radius) * w, (center.Y + cpOffset) * h), true, false);

              center = new Point(.9, .5);
              s.BezierTo(new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point((center.X + cpOffset * .5) * w, (center.Y + radius) * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.ACvoltageSource: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // Circle with curve in middle
              double factor = Geo.MagicBezierFactor;
              double cpOffset = factor * .5;
              double radius = .5;
              Point center = new Point(.5, .5);

              s.BeginFigure(new Point((center.X - radius) * w, center.Y * h), false, true);
              s.BezierTo(new Point((center.X - radius) * w, (center.Y - cpOffset) * h), new Point((center.X - cpOffset) * w, (center.Y - radius) * h), new Point(center.X * w, (center.Y - radius) * h), true, false);
              s.BezierTo(new Point((center.X + cpOffset) * w, (center.Y - radius) * h), new Point((center.X + radius) * w, (center.Y - cpOffset) * h), new Point((center.X + radius) * w, center.Y * h), true, false);
              s.BezierTo(new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X - radius) * w, center.Y * h), true, false);

              s.BeginFigure(new Point((center.X - radius + .1) * w, center.Y * h), false, false);
              s.BezierTo(new Point(center.X * w, (center.Y - radius) * h), new Point(center.X * w, (center.Y + radius) * h), new Point((center.X + radius - .1) * w, center.Y * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.DCvoltageSource: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // Small vertical line and large vertical line
              s.BeginFigure(new Point(0, .75 * h), false, false);
              s.LineTo(new Point(0, .25 * h), true, false);
              s.BeginFigure(new Point(1 * w, 0), false, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Diode: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(0, .25);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.5,.75);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              // Small vertical line and large vertical line
              s.BeginFigure(new Point(0, 0), false, true);
              s.LineTo(new Point(1 * w, .5 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);

              s.BeginFigure(new Point(1 * w, 0), false, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }

        // Computer Shapes
        case NodeFigure.Wifi: {
            double origw = w;
            double origh = h;
            w = w * .38;
            h = h * .6;

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              double factor = Geo.MagicBezierFactor;
              double cpOffset = factor * .2;
              double radius = .2;
              Point center = new Point(.5, .5);
              double xOffset = (origw - w) / 2;
              double yOffset = (origh - h) / 2;

              s.BeginFigure(new Point((center.X - radius) * w /*xoffset*/+ xOffset, center.Y * h /*yoffset*/+ yOffset), true, true);
              s.BezierTo(new Point((center.X - radius) * w /*xoffset*/+ xOffset, (center.Y - cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X - cpOffset) * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), new Point(center.X * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point((center.X + cpOffset) * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), new Point((center.X + radius) * w /*xoffset*/+ xOffset, (center.Y - cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X + radius) * w /*xoffset*/+ xOffset, center.Y * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point((center.X + radius) * w /*xoffset*/+ xOffset, (center.Y + cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X + cpOffset) * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), new Point(center.X * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), new Point((center.X - radius) * w /*xoffset*/+ xOffset, (center.Y + cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X - radius) * w /*xoffset*/+ xOffset, center.Y * h /*yoffset*/+ yOffset), true, false);

              //Right curves
              cpOffset = factor * .4;
              radius = .4;
              center = new Point(.8, .5);
              s.BeginFigure(new Point(center.X * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), true, true);
              s.BezierTo(new Point((center.X + cpOffset) * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), new Point((center.X + radius) * w /*xoffset*/+ xOffset, (center.Y - cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X + radius) * w /*xoffset*/+ xOffset, center.Y * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point((center.X + radius) * w /*xoffset*/+ xOffset, (center.Y + cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X + cpOffset) * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), new Point(center.X * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point(center.X * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), new Point((center.X + radius - cpOffset * .5) * w /*xoffset*/+ xOffset, (center.Y + cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X + radius - cpOffset * .5) * w /*xoffset*/+ xOffset, center.Y * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point((center.X + radius - cpOffset * .5) * w /*xoffset*/+ xOffset, (center.Y - cpOffset) * h /*yoffset*/+ yOffset), new Point(center.X * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), new Point(center.X * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), true, false);

              cpOffset = factor * .8;
              radius = .8;
              center = new Point(1, .5);
              s.BeginFigure(new Point(center.X * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), true, true);
              s.BezierTo(new Point((center.X + cpOffset) * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), new Point((center.X + radius) * w /*xoffset*/+ xOffset, (center.Y - cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X + radius) * w /*xoffset*/+ xOffset, center.Y * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point((center.X + radius) * w /*xoffset*/+ xOffset, (center.Y + cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X + cpOffset) * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), new Point(center.X * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point(center.X * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), new Point((center.X + radius - cpOffset * .5) * w /*xoffset*/+ xOffset, (center.Y + cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X + radius - cpOffset * .5) * w /*xoffset*/+ xOffset, center.Y * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point((center.X + radius - cpOffset * .5) * w /*xoffset*/+ xOffset, (center.Y - cpOffset) * h /*yoffset*/+ yOffset), new Point(center.X * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), new Point(center.X * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), true, false);

              //Left curves
              cpOffset = factor * .4;
              radius = .4;
              center = new Point(.2, .5);
              s.BeginFigure(new Point(center.X * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), true, true);
              s.BezierTo(new Point((center.X - cpOffset) * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), new Point((center.X - radius) * w /*xoffset*/+ xOffset, (center.Y + cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X - radius) * w /*xoffset*/+ xOffset, center.Y * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point((center.X - radius) * w /*xoffset*/+ xOffset, (center.Y - cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X - cpOffset) * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), new Point(center.X * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point(center.X * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), new Point((center.X - radius + cpOffset * .5) * w /*xoffset*/+ xOffset, (center.Y - cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X - radius + cpOffset * .5) * w /*xoffset*/+ xOffset, center.Y * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point((center.X - radius + cpOffset * .5) * w /*xoffset*/+ xOffset, (center.Y + cpOffset) * h /*yoffset*/+ yOffset), new Point(center.X * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), new Point(center.X * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), true, false);

              cpOffset = factor * .8;
              radius = .8;
              center = new Point(0, .5);
              s.BeginFigure(new Point(center.X * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), true, true);
              s.BezierTo(new Point((center.X - cpOffset) * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), new Point((center.X - radius) * w /*xoffset*/+ xOffset, (center.Y + cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X - radius) * w /*xoffset*/+ xOffset, center.Y * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point((center.X - radius) * w /*xoffset*/+ xOffset, (center.Y - cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X - cpOffset) * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), new Point(center.X * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point(center.X * w /*xoffset*/+ xOffset, (center.Y - radius) * h /*yoffset*/+ yOffset), new Point((center.X - radius + cpOffset * .5) * w /*xoffset*/+ xOffset, (center.Y - cpOffset) * h /*yoffset*/+ yOffset), new Point((center.X - radius + cpOffset * .5) * w /*xoffset*/+ xOffset, center.Y * h /*yoffset*/+ yOffset), true, false);
              s.BezierTo(new Point((center.X - radius + cpOffset * .5) * w /*xoffset*/+ xOffset, (center.Y + cpOffset) * h /*yoffset*/+ yOffset), new Point(center.X * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), new Point(center.X * w /*xoffset*/+ xOffset, (center.Y + radius) * h /*yoffset*/+ yOffset), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Email: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, 0), false, true);
              s.LineTo(new Point(.5 * w, .6 * h), true, false);
              s.LineTo(new Point(w, 0), true, false);
              s.BeginFigure(new Point(1 * w, 0), false, false);
              s.LineTo(new Point(.5 * w, .6 * h), true, false);
              s.BeginFigure(new Point(0, 1 * h), false, false);
              s.LineTo(new Point(.45 * w, .54 * h), true, false);
              s.BeginFigure(new Point(1 * w, 1 * h), false, false);
              s.LineTo(new Point(.55 * w, .54 * h), true, false);

              s.BeginFigure(new Point(0, 0), true, true);
              s.LineTo(new Point(1 * w, 0), true, false);
              s.LineTo(new Point(1 * w, 1 * h), true, false);
              s.LineTo(new Point(0, 1 * h), true, false);
              s.LineTo(new Point(0, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Ethernet: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(0, .5 * h), false, false);
              s.LineTo(new Point(1 * w, .5 * h), true, false);
              s.BeginFigure(new Point(.5 * w, .5 * h), false, false);
              s.LineTo(new Point(.5 * w, .4 * h), true, false);
              s.BeginFigure(new Point(.75 * w, .5 * h), false, false);
              s.LineTo(new Point(.75 * w, .6 * h), true, false);
              s.BeginFigure(new Point(.25 * w, .5 * h), false, false);
              s.LineTo(new Point(.25 * w, .6 * h), true, false);

              // Boxes at the end of the wires
              s.BeginFigure(new Point(.35 * w, 0), true, true);
              s.LineTo(new Point(.65 * w, 0), true, false);
              s.LineTo(new Point(.65 * w, .4 * h), true, false);
              s.LineTo(new Point(.35 * w, .4 * h), true, false);
              s.LineTo(new Point(.35 * w, 0), true, false);

              s.BeginFigure(new Point(.10 * w, 1 * h), true, true);
              s.LineTo(new Point(.40 * w, 1 * h), true, false);
              s.LineTo(new Point(.40 * w, .6 * h), true, false);
              s.LineTo(new Point(.10 * w, .6 * h), true, false);
              s.LineTo(new Point(.10 * w, 1 * h), true, false);

              s.BeginFigure(new Point(.60 * w, 1 * h), true, true);
              s.LineTo(new Point(.90 * w, 1 * h), true, false);
              s.LineTo(new Point(.90 * w, .6 * h), true, false);
              s.LineTo(new Point(.60 * w, .6 * h), true, false);
              s.LineTo(new Point(.60 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Power: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.25,.55);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.75,.8);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              double factor = Geo.MagicBezierFactor;
              double cpOffset = factor * .4;
              double radius = .4;
              Point center = new Point(.5, .5);
              Point unused;
              Point mid;
              Point c1;
              Point c2;
              Point start;
              // Find the 45 degree midpoint for the first bezier
              BreakUpBezier(new Point(center.X, center.Y - radius), new Point(center.X + cpOffset, center.Y - radius),
                new Point(center.X + radius, center.Y - cpOffset), new Point(center.X + radius, center.Y), .5, out unused,
                out unused, out mid, out c1, out c2);
              start = new Point(mid.X, mid.Y);
              s.BeginFigure(new Point(mid.X * w, mid.Y * h), true, true);
              s.BezierTo(new Point(c1.X * w, c1.Y * h), new Point(c2.X * w, c2.Y * h), new Point((center.X + radius) * w, center.Y * h), true, false);
              s.BezierTo(new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X - radius) * w, center.Y * h), true, false);

              // Find the 45 degree midpoint of for the fourth bezier
              BreakUpBezier(new Point(center.X - radius, center.Y), new Point(center.X - radius, center.Y - cpOffset),
                new Point(center.X - cpOffset, center.Y - radius), new Point(center.X, center.Y - radius), .5, out c1,
                out c2, out mid, out unused, out unused);
              s.BezierTo(new Point(c1.X * w, c1.Y * h), new Point(c2.X * w, c2.Y * h), new Point(mid.X * w, mid.Y * h), true, false);

              //now make a smaller circle
              cpOffset = factor * .3;
              radius = .3;

              // Find the 45 degree midpoint for the first bezier
              BreakUpBezier(new Point(center.X - radius, center.Y), new Point(center.X - radius, center.Y - cpOffset),
                new Point(center.X - cpOffset, center.Y - radius), new Point(center.X, center.Y - radius), .5, out c1,
                out c2, out mid, out unused, out unused);
              s.LineTo(new Point(mid.X * w, mid.Y * h), true, false);
              s.BezierTo(new Point(c2.X * w, c2.Y * h), new Point(c1.X * w, c1.Y * h), new Point((center.X - radius) * w, center.Y * h), true, false);
              s.BezierTo(new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false);
              s.BezierTo(new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + radius) * w, center.Y * h), true, false);

              // Find the 45 degree midpoint for the fourth bezier
              BreakUpBezier(new Point(center.X, center.Y - radius), new Point(center.X + cpOffset, center.Y - radius),
                new Point(center.X + radius, center.Y - cpOffset), new Point(center.X + radius, center.Y), .5, out unused,
                out unused, out mid, out c1, out c2);
              s.BezierTo(new Point(c2.X * w, c2.Y * h), new Point(c1.X * w, c1.Y * h), new Point(mid.X * w, mid.Y * h), true, false);
              s.LineTo(new Point(start.X * w, start.Y * h), true, false);

              // The line
              s.BeginFigure(new Point(.45 * w, 0), true, true);
              s.LineTo(new Point(.45 * w, .5 * h), true, false);
              s.LineTo(new Point(.55 * w, .5 * h), true, false);
              s.LineTo(new Point(.55 * w, 0), true, false);
              s.LineTo(new Point(.45 * w, 0), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.Fallout: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              double factor = Geo.MagicBezierFactor;
              double cpOffset = factor * .5;
              double radius = .5;
              Point center = new Point(.5, .5);

              //Containing circle
              s.BeginFigure(new Point((center.X - radius) * w, center.Y * h), true, true);
              s.BezierTo(new Point((center.X - radius) * w, (center.Y - cpOffset) * h), new Point((center.X - cpOffset) * w, (center.Y - radius) * h), new Point(center.X * w, (center.Y - radius) * h), true, false);
              s.BezierTo(new Point((center.X + cpOffset) * w, (center.Y - radius) * h), new Point((center.X + radius) * w, (center.Y - cpOffset) * h), new Point((center.X + radius) * w, center.Y * h), true, false);
              s.BezierTo(new Point((center.X + radius) * w, (center.Y + cpOffset) * h), new Point((center.X + cpOffset) * w, (center.Y + radius) * h), new Point(center.X * w, (center.Y + radius) * h), true, false);
              s.BezierTo(new Point((center.X - cpOffset) * w, (center.Y + radius) * h), new Point((center.X - radius) * w, (center.Y + cpOffset) * h), new Point((center.X - radius) * w, center.Y * h), true, false);

              double offsetx = 0;
              double offsety = 0;
              //Triangles
              s.BeginFigure(new Point((.3 + offsetx) * w, (.8 + offsety) * h), true, true);
              s.LineTo(new Point((.5 + offsetx) * w, (.5 + offsety) * h), true, false);
              s.LineTo(new Point((.1 + offsetx) * w, (.5 + offsety) * h), true, false);
              s.LineTo(new Point((.3 + offsetx) * w, (.8 + offsety) * h), true, false);
              offsetx = .4;
              offsety = 0;
              //Triangles
              s.BeginFigure(new Point((.3 + offsetx) * w, (.8 + offsety) * h), true, true);
              s.LineTo(new Point((.5 + offsetx) * w, (.5 + offsety) * h), true, false);
              s.LineTo(new Point((.1 + offsetx) * w, (.5 + offsety) * h), true, false);
              s.LineTo(new Point((.3 + offsetx) * w, (.8 + offsety) * h), true, false);
              offsetx = .2;
              offsety = -.3;
              //Triangles
              s.BeginFigure(new Point((.3 + offsetx) * w, (.8 + offsety) * h), true, true);
              s.LineTo(new Point((.5 + offsetx) * w, (.5 + offsety) * h), true, false);
              s.LineTo(new Point((.1 + offsetx) * w, (.5 + offsety) * h), true, false);
              s.LineTo(new Point((.3 + offsetx) * w, (.8 + offsety) * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.IrritationHazard: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.3,.3);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.7,.7);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(.2 * w, 0 * h), true, true);
              s.LineTo(new Point(.5 * w, .3 * h), true, false);
              s.LineTo(new Point(.8 * w, 0 * h), true, false);
              s.LineTo(new Point(1 * w, .2 * h), true, false);
              s.LineTo(new Point(.7 * w, .5 * h), true, false);
              s.LineTo(new Point(1 * w, .8 * h), true, false);
              s.LineTo(new Point(.8 * w, 1 * h), true, false);
              s.LineTo(new Point(.5 * w, .7 * h), true, false);
              s.LineTo(new Point(.2 * w, 1 * h), true, false);
              s.LineTo(new Point(0 * w, .8 * h), true, false);
              s.LineTo(new Point(.3 * w, .5 * h), true, false);
              s.LineTo(new Point(0 * w, .2 * h), true, false);
              s.LineTo(new Point(.2 * w, 0 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.ElectricalHazard: {
            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(.37 * w, 0 * h), true, true);
              s.LineTo(new Point(.5 * w, .11 * h), true, false);
              s.LineTo(new Point(.77 * w, .04 * h), true, false);
              s.LineTo(new Point(.33 * w, .49 * h), true, false);
              s.LineTo(new Point(1 * w, .37 * h), true, false);
              s.LineTo(new Point(.63 * w, .86 * h), true, false);
              s.LineTo(new Point(.77 * w, .91 * h), true, false);
              s.LineTo(new Point(.34 * w, 1 * h), true, false);
              s.LineTo(new Point(.34 * w, .78 * h), true, false);
              s.LineTo(new Point(.44 * w, .8 * h), true, false);
              s.LineTo(new Point(.65 * w, .56 * h), true, false);
              s.LineTo(new Point(0 * w, .68 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.FireHazard: {
            if (this.Spot1.IsDefault) this.Spot1 = new Spot(.05, .645);
            if (this.Spot2.IsDefault) this.Spot2 = new Spot(.884, .908);

            GeoStream sg = new GeoStream();
            using (StreamGeometryContext s = sg.Open()) {
              s.BeginFigure(new Point(.1 * w, 1 * h), true, true);
              s.BezierTo(new Point(-.26 * w, .63 * h), new Point(.45 * w, .44 * h), new Point(.29 * w, 0 * h), true, false);
              s.BezierTo(new Point(.48 * w, .17 * h), new Point(.54 * w, .35 * h), new Point(.51 * w, .42 * h), true, false);
              s.BezierTo(new Point(.59 * w, .29 * h), new Point(.58 * w, .28 * h), new Point(.59 * w, .18 * h), true, false);
              s.BezierTo(new Point(.8 * w, .34 * h), new Point(.88 * w, .43 * h), new Point(.75 * w, .6 * h), true, false);
              s.BezierTo(new Point(.87 * w, .48 * h), new Point(.88 * w, .43 * h), new Point(.88 * w, .31 * h), true, false);
              s.BezierTo(new Point(1.18 * w, .76 * h), new Point(.82 * w, .8 * h), new Point(.9 * w, 1 * h), true, false);
              s.LineTo(new Point(.1 * w, 1 * h), true, false);
            }
            geo = sg.Geometry;
            break;
          }
        case NodeFigure.BpmnActivityLoop: {
          GeoStream sg = new GeoStream();
          using (StreamGeometryContext context = sg.Open()) {
            double r = .5;
            double cx = 0; // offset from Center x
            double cy = 0; // offset from Center y
            double d = r * (4 * (Math.Sqrt(2) - 1) / 3);
            double mx1 = (.5 * Math.Sqrt(2) / 2 + .5);
            double my1 = (.5 - .5 * Math.Sqrt(2) / 2);
            double x1 = 1;
            double y1 = .5;
            double x2 = .5;
            double y2 = 0;
            context.BeginFigure(new Point(mx1 * w, (1 - my1) * h), false, false);
            context.BezierTo(new Point(x1 * w, .7 * h), new Point(x1 * w, y1 * h), new Point(x1 * w, y1 * h), true, false);
            context.BezierTo(new Point((.5 + r + cx) * w, (.5 - d + cx) * h), new Point((.5 + d + cx) * w, (.5 - r + cx) * h), new Point((x2  + cx) * w, (y2 + cx) * h), true, false);
            context.BezierTo(new Point((.5 - d + cx) * w, (.5 - r + cy) * h), new Point((.5 - r + cx) * w, (.5 - d + cy) * h), new Point((.5 - r + cx) * w, (.5 + cy) * h), true, false); // ccw, topmiddle to middleleft
            context.BezierTo(new Point((.5 - r + cx) * w, (.5 + d + cy) * h), new Point((.5 - d + cx) * w, (.5 + r + cy) * h), new Point((.35 + cx) * w, (.5 + r -.02 + cy) * h), true, false);
            // draw arrowhead
            context.BeginFigure(new Point((.35 + cx) * w, (.5 + r + cy) * h), false, false);
            context.LineTo(new Point((.35 + cx) * w, (.5 + r - .2 + cy) * h), true, false);
            context.BeginFigure(new Point((.35 + cx) * w, (.5 + r + cy) * h), false, false);
            context.LineTo(new Point((.15 + cx) * w, (.5 + r + cy) * h), true, false);
          }
          geo = sg.Geometry;
          break;
        }
        case NodeFigure.BpmnActivityParallel: {
          GeoStream sg = new GeoStream();
          using (StreamGeometryContext context = sg.Open()) {
            context.BeginFigure(new Point(0, 0), false, false);
            context.LineTo(new Point(0, 1 * h), true, false);
            context.BeginFigure(new Point(.5 * w, 0), false, false);
            context.LineTo(new Point(.5 * w, 1 * h), true, false);
            context.BeginFigure(new Point(1 * w, 0), false, false);
            context.LineTo(new Point(1 * w, 1 * h), true, false);
          }
          geo = sg.Geometry;
          break;
        }
        case NodeFigure.BpmnActivitySequential: {
          GeoStream sg = new GeoStream();
          using (StreamGeometryContext context = sg.Open()) {
            context.BeginFigure(new Point(0, 0), false, false);
            context.LineTo(new Point(1 * w, 0), true, false);
            context.BeginFigure(new Point(0, .5 * h), false, false);
            context.LineTo(new Point(1 * w, .5 * h), true, false);
            context.BeginFigure(new Point(0, 1 * h), false, false);
            context.LineTo(new Point(1 * w, 1 * h), true, false);
          }
          geo = sg.Geometry;
          break;
        }
        case NodeFigure.BpmnActivityAdHoc: {
          GeoStream sg = new GeoStream();
          using (StreamGeometryContext context = sg.Open()) {
            context.BeginFigure(new Point(0, 0), false, false);
            context.BeginFigure(new Point(1 * w, 1 * h), false, false);
            context.BeginFigure(new Point(0, .5 * h), false, false);
            context.BezierTo(new Point(.2 * w, .35 * h), new Point(.3 * w, .35 * h), new Point(.5 * w, .5 * h), true, false);
            context.BezierTo(new Point(.7 * w, .65 * h), new Point(.8 * w, .65 * h), new Point(1 * w, .5 * h), true, false);
          }
          geo = sg.Geometry;
          break;
        }
        case NodeFigure.BpmnActivityCompensation: {
          GeoStream sg = new GeoStream();
          using (StreamGeometryContext context = sg.Open()) {
            context.BeginFigure(new Point(0, .5 * h), true, true);
            context.LineTo(new Point(.5 * w, 0), true, true);
            context.LineTo(new Point(.5 * w, 1 * h), true, true);
            context.LineTo(new Point(0, .5 * h), true, true);
            context.BeginFigure(new Point(.5 * w, .5 * h), true, true);
            context.LineTo(new Point(1 * w, 0), true, true);
            context.LineTo(new Point(1 * w, 1 * h), true, true);
            context.LineTo(new Point(.5 * w, .5 * h), true, true);
          }
          geo = sg.Geometry;
          break;
        }
        case NodeFigure.BpmnTaskMessage: {
          GeoStream sg = new GeoStream();
          using (StreamGeometryContext context = sg.Open()) {
            context.BeginFigure(new Point(0, 0), false, false);
            context.BeginFigure(new Point(1 * w, 1 * h), false, false);
            context.BeginFigure(new Point(0, .2 * h), false, false);
            context.LineTo(new Point(.5 * w, .5 * h), true, false);
            context.LineTo(new Point(1 * w, .2 * h), true, true);
            context.BeginFigure(new Point(0, .2 * h), true, true);
            context.LineTo(new Point(1 * w, .2 * h), true, false);
            context.LineTo(new Point(1 * w, .8 * h), true, false);
            context.LineTo(new Point(0, .8 * h), true, false);
            context.LineTo(new Point(0, .8 * h), true, false);
          }
          geo = sg.Geometry;
          break;
        }
        case NodeFigure.BpmnTaskScript: {
          GeoStream sg = new GeoStream();
          using (StreamGeometryContext context = sg.Open()) {
            context.BeginFigure(new Point(.7 * w, 1 * h), true, true);
            context.LineTo(new Point(.3 * w, 1 * h), true, false);
            context.BezierTo(new Point(.6 * w, .5 * h), new Point(0, .5 * h), new Point(0.3 * w, 0), true, false);
            context.LineTo(new Point(.7 * w, 0), true, false);
            context.BezierTo(new Point(.4 * w, .5 * h), new Point(1 * w, .5 * h), new Point(.7 * w, 1 * h), true, false);
            context.BeginFigure(new Point(.45 * w, .73 * h), false, false);
            context.LineTo(new Point(.7 * w, .73 * h), true, false);
            context.BeginFigure(new Point(.38 * w, .5 * h), false, false);
            context.LineTo(new Point(.63 * w, .5 * h), true, false);
            context.BeginFigure(new Point(.31 * w, .27 * h), false, false);
            context.LineTo(new Point(.56 * w, .27 * h), true, false);
          }   
          geo = sg.Geometry;
          break;
        }
        case NodeFigure.BpmnTaskUser: {
          GeoStream sg = new GeoStream();
          using (StreamGeometryContext context = sg.Open()) {
            //shirt
            context.BeginFigure(new Point(.335 * w, (1 - .555) * h), true, true);
            context.LineTo(new Point(.335 * w, (1 - .405) * h), true, true);
            context.LineTo(new Point((1 - .335) * w, (1 - .405) * h), true, true);
            context.LineTo(new Point((1 - .335) * w, (1 - .555) * h), true, true);
            context.BezierTo(new Point((1 - .12) * w, .46 * h), new Point((1 - .02) * w, .54 * h), new Point(1 * w, .68 * h), true, true);
            context.LineTo(new Point(1 * w, 1 * h), true, true);
            context.LineTo(new Point(0, 1 * h), true, true);
            context.LineTo(new Point(0, .68 * h), true, true);
            context.BezierTo(new Point(.02 * w, .54 * h), new Point(.12 * w, .46 * h), new Point(.335 * w, (1 - .555) * h), true, true);
            //start of neck
            context.BeginFigure(new Point(.365 * w, (1 - .595) * h), true, true);
            double radiushead = .5 - .285;
            Point center = new Point(.5, radiushead);
            double factor = Geo.MagicBezierFactor;
            double cpOffset = factor * .5;
            Size radius = new Size(radiushead, radiushead);
            Size offset = new Size(factor * radius.Width, factor * radius.Height);
            // Circle (head)
            context.BezierTo(new Point((center.X - ((offset.Width + radius.Width) / 2)) * w, (center.Y + ((radius.Height + offset.Height) / 2)) * h), new Point((center.X - radius.Width) * w, (center.Y + offset.Height) * h), new Point((center.X - radius.Width) * w, center.Y * h), true, true);
            context.BezierTo(new Point((center.X - radius.Width) * w, (center.Y - offset.Height) * h), new Point((center.X - offset.Width) * w, (center.Y - radius.Height) * h), new Point(center.X * w, (center.Y - radius.Height) * h), true, true);
            context.BezierTo(new Point((center.X + offset.Width) * w, (center.Y - radius.Height) * h), new Point((center.X + radius.Width) * w, (center.Y - offset.Height) * h), new Point((center.X + radius.Width) * w, center.Y * h), true, true);
            context.BezierTo(new Point((center.X + radius.Width) * w, (center.Y + offset.Height) * h), new Point((center.X + ((offset.Width + radius.Width) / 2)) * w, (center.Y + ((radius.Height + offset.Height) / 2)) * h), new Point((1 - .365) * w, (1 - .595) * h), true, true);
            context.LineTo(new Point((1 - .365) * w, (1 - .595) * h), true, true);
            //neckline
            context.LineTo(new Point((1 - .335) * w, (1 - .555) * h), true, true);
            context.LineTo(new Point((1 - .335) * w, (1 - .405) * h), true, true);
            context.LineTo(new Point(.335 * w, (1 - .405) * h), true, true);
            context.LineTo(new Point(.335 * w, (1 - .555) * h), true, true);
            //arm lines
            context.BeginFigure(new Point(.2 * w, 1 * h), false, false);
            context.LineTo(new Point(.2 * w, .8 * h), true, true);
            context.BeginFigure(new Point(.8 * w, 1 * h), false, false);
            context.LineTo(new Point(.8 * w, .8 * h), true, true);
          }
          geo = sg.Geometry;
          break;
        }
        case NodeFigure.BpmnEventConditional: {
          GeoStream sg = new GeoStream();
          using (StreamGeometryContext context = sg.Open()) {
            // Inside lines
            context.BeginFigure(new Point(.2 * w, .2 * h), false, false);
            context.LineTo(new Point(.8 * w, .2 * h), true, false);
            context.BeginFigure(new Point(.2 * w, .4 * h), false, false);
            context.LineTo(new Point(.8 * w, .4 * h), true, false);
            context.BeginFigure(new Point(.2 * w, .6 * h), false, false);
            context.LineTo(new Point(.8 * w, .6 * h), true, false);
            context.BeginFigure(new Point(.2 * w, .8 * h), false, false);
            context.LineTo(new Point(.8 * w, .8 * h), true, false);
            // Body
            context.BeginFigure(new Point(0 * w, 0 * h), true, true);
            context.LineTo(new Point(1 * w, 0 * h), true, false);
            context.LineTo(new Point(1 * w, 1 * h), true, false);
            context.LineTo(new Point(0 * w, 1 * h), true, false);
            context.LineTo(new Point(0 * w, 0 * h), true, false);
          }
          geo = sg.Geometry;
          break;
        }
        case NodeFigure.BpmnEventError: {
          GeoStream sg = new GeoStream();
          using (StreamGeometryContext context = sg.Open()) {
            context.BeginFigure(new Point(0, 1 * h), true, true);
            context.LineTo(new Point(.33 * w, 0), true, false);
            context.LineTo(new Point(.66 * w, .50 * h), true, false);
            context.LineTo(new Point(1 * w, 0), true, false);
            context.LineTo(new Point(.66 * w, 1 * h), true, false);
            context.LineTo(new Point(.33 * w, .50 * h), true, false);
          }
          geo = sg.Geometry;
          break;
        }
        case NodeFigure.BpmnEventEscalation: {
          GeoStream sg = new GeoStream();
          using (StreamGeometryContext context = sg.Open()) {
            context.BeginFigure(new Point(0, 0), false, false);
            context.BeginFigure(new Point(1 * w, 1 * h), false, false);  // set dimensions
            context.BeginFigure(new Point(.1 * w, 1 * h), true, true);
            context.LineTo(new Point(.5 * w, 0), true, false);
            context.LineTo(new Point(.9 * w, 1 * h), true, false);
            context.LineTo(new Point(.5 * w, .5 * h), true, false);
            context.LineTo(new Point(.1 * w, 1 * h), true, false);
          }
          geo = sg.Geometry;
          break;
        }
        case NodeFigure.BpmnEventTimer: {
          GeoStream sg = new GeoStream();
          using (StreamGeometryContext context = sg.Open()) {
            double radius = .5;

            // now, draw the hour lines
            context.BeginFigure(new Point(radius * w, 0), false, false);
            context.LineTo(new Point(radius * w, .15 * h), true, false);
            context.BeginFigure(new Point(radius * w, 1 * h), false, false);
            context.LineTo(new Point(radius * w, .85 * h), true, false);
            context.BeginFigure(new Point(0, radius * h), false, false);
            context.LineTo(new Point(.15 * w, radius * h), true, false);
            context.BeginFigure(new Point(1 * w, radius * h), false, false);
            context.LineTo(new Point(.85 * w, radius * h), true, false);

            // now draw the hands
            context.BeginFigure(new Point(radius * w, radius * h), false, false);
            context.LineTo(new Point(.58 * w, 0.1 * h), true, false);
            context.BeginFigure(new Point(radius * w, radius * h), false, false);
            context.LineTo(new Point(.78 * w, .54 * h), true, false);

            double factor = Geo.MagicBezierFactor;
            double cpOffset = factor * .5;
            context.BeginFigure(new Point(1 * w, radius * h), true, true);
            context.BezierTo(new Point(1 * w, (radius + cpOffset) * h), new Point((radius + cpOffset) * w, 1 * h), new Point(radius * w, 1 * h), true, false);
            context.BezierTo(new Point((radius - cpOffset) * w, 1 * h), new Point(0, (radius + cpOffset) * h), new Point(0, radius * h), true, false);
            context.BezierTo(new Point(0, (radius - cpOffset) * h), new Point((radius - cpOffset) * w, 0), new Point(radius * w, 0), true, false);
            context.BezierTo(new Point((radius + cpOffset) * w, 0), new Point(1 * w, (radius - cpOffset) * h), new Point(1 * w, radius * h), true, false);
          }
          geo = sg.Geometry;
          break;
        }
      }  // End switch

      return geo;
    }


    private static void BreakUpBezier(Point start, Point c1, Point c2, Point end, double fraction,
          out Point curve1cp1, out Point curve1cp2, out Point midpoint, out Point curve2cp1, out Point curve2cp2) {
      double fo = 1 - fraction;
      double so = fraction;
      Point m1 = new Point((start.X * fo + c1.X * so), (start.Y * fo + c1.Y * so));
      Point m2 = new Point((c1.X * fo + c2.X * so), (c1.Y * fo + c2.Y * so));
      Point m3 = new Point((c2.X * fo + end.X * so), (c2.Y * fo + end.Y * so));
      Point m12 = new Point((m1.X * fo + m2.X * so), (m1.Y * fo + m2.Y * so));
      Point m23 = new Point((m2.X * fo + m3.X * so), (m2.Y * fo + m3.Y * so));
      Point m123 = new Point((m12.X * fo + m23.X * so), (m12.Y * fo + m23.Y * so));
      curve1cp1 = m1;
      curve1cp2 = m12;
      midpoint = m123;
      curve2cp1 = m23;
      curve2cp2 = m3;
    }

    private static Point[] CreateBurst(int points) {
      Point[] star = CreateStar(points);
      Point[] pts = new Point[points * 3 + 1];

      pts[0] = star[0];
      for (int i = 1, count = 1; i < star.Length; i += 2, count += 3) {
        pts[count] = star[i];
        pts[count + 1] = star[i];
        pts[count + 2] = star[i + 1];
      }

      return pts;
    }

    private static Point[] CreateStar(int points) {
      // First, create a regular polygon
      Point[] polygon = CreatePolygon(points);
      // Calculate the points inbetween
      Point[] pts = new Point[points * 2 + 1];

      int half = polygon.Length / 2;
      int count = polygon.Length - 1;
      int offset = (points % 2 == 0) ? 2 : 1;

      for (int i = 0; i < count; i++) {
        // Get the intersection of two lines
        pts[i * 2] = polygon[i];
        pts[i * 2 + 1] = GetIntersection(polygon[i], polygon[(half + i - 1) % count], polygon[i + 1], polygon[(half + i + offset) % count]);
      }

      pts[pts.Length - 1] = pts[0];

      return pts;
    }

    private static Point GetIntersection(Point l1p1, Point l1p2, Point l2p1, Point l2p2) {
      double dx1 = l1p1.X - l1p2.X;
      double dx2 = l2p1.X - l2p2.X;
      double x;
      double y;

      if (dx1 == 0 || dx2 == 0) {
        if (dx1 == 0) {
          double m2 = (l2p1.Y - l2p2.Y) / dx2;
          double b2 = l2p1.Y - m2 * l2p1.X;
          x = l1p1.X;
          y = m2 * x + b2;
        }
        else {
          double m1 = (l1p1.Y - l1p2.Y) / dx1;
          double b1 = l1p1.Y - m1 * l1p1.X;
          x = l2p1.X;
          y = m1 * x + b1;
        }
      }
      else {
        double m1 = (l1p1.Y - l1p2.Y) / dx1;
        double m2 = (l2p1.Y - l2p2.Y) / dx2;
        double b1 = l1p1.Y - m1 * l1p1.X;
        double b2 = l2p1.Y - m2 * l2p1.X;

        x = (b2 - b1) / (m1 - m2);
        y = m1 * x + b1;
      }

      return new Point(x, y);
    }

    private static Point[] CreatePolygon(int sides) {
      Point[] points = new Point[sides + 1];
      double radius = .5;
      double center = .5;
      double offsetAngle = Math.PI * 1.5;
      double angle = 0;

      // Loop through each side of the polygon
      for (int i = 0; i < sides; i++) {
        angle = 2 * Math.PI / sides * i + offsetAngle;
        points[i] = new Point((double)(center + radius * Math.Cos(angle)), (double)(center + radius * Math.Sin(angle)));
      }

      // Add the last line
      points[points.Length - 1] = points[0];
      return points;
    }
  }


  /// <summary>
  /// Predefined shapes for <c>NodeShape</c> (WPF) or <c>Path</c> inside a <see cref="NodePanel"/>.
  /// </summary>
  /// <remarks>
  /// Set the <c>NodePanel.Figure</c> attached property (or call <see cref="NodePanel.SetFigure"/>) on the shape.
  /// </remarks>
  public enum NodeFigure {
    /// <summary>
    /// Used when there is no particular figure.
    /// </summary>
    None = 0,
    // Lines and Curves
    /// <summary>
    /// Represents a line.
    /// </summary>
    Line1,
    /// <summary>
    /// Represents a line.
    /// </summary>
    Line2,
    /// <summary>
    /// Represents a horizontal line.
    /// </summary>
    LineH,
    /// <summary>
    /// Represents a vertical line.
    /// </summary>
    LineV,
    /// <summary>
    /// Represents a curve.
    /// </summary>
    Curve1,
    /// <summary>
    /// Represents a curve.
    /// </summary>
    Curve2,
    /// <summary>
    /// Represents a curve.
    /// </summary>
    Curve3,
    /// <summary>
    /// Represents a curve.
    /// </summary>
    Curve4,


    // Regular Polygons
    /// <summary>
    /// Represents a three-sided figure (a triangle). This is the same shape
    /// represented by NodeFigure.Alternative and NodeFigure.Merge.
    /// </summary>
    Triangle,
    /// <summary>
    /// Represents a three-sided figure (a triangle) pointing towards the right.
    /// </summary>
    TriangleRight,
    /// <summary>
    /// Represents a three-sided figure (a triangle) pointing downwards.
    /// </summary>
    TriangleDown,
    /// <summary>
    /// Represents a three-sided figure (a triangle) pointing towards the left.
    /// </summary>
    TriangleLeft,
    /// <summary>
    /// Represents a three-sided figure (a triangle) pointing upwards.
    /// </summary>
    TriangleUp,
    /// <summary>
    /// Represents a four-sided figure (a diamond). This is the same shape
    /// represented by NodeFigure.Decision.
    /// </summary>
    Diamond,
    /// <summary>
    /// Represents a five-sided figure (a pentagon).
    /// </summary>
    Pentagon,
    /// <summary>
    /// Represents a six-sided figure (a hexagon). This is the same shape
    /// represented by NodeFigure.DataTransmission.
    /// </summary>
    Hexagon,
    /// <summary>
    /// Represents a seven-sided figure (a heptagon).
    /// </summary>
    Heptagon,
    /// <summary>
    /// Represents an eight-sided figure (an octagon).
    /// </summary>
    Octagon,
    /// <summary>
    /// Represents a nine-sided figure (a nonagon).
    /// </summary>
    Nonagon,
    /// <summary>
    /// Represents a ten-sided figure (a decagon).
    /// </summary>
    Decagon,
    /// <summary>
    /// Represents a twelve-sided figure (a dodecagon).
    /// </summary>
    Dodecagon,
    // #endregion

    // Regular Stars
    /// <summary>
    /// Represents a five-pointed star.
    /// </summary>
    FivePointedStar,
    /// <summary>
    /// Represents a six-pointed star.
    /// </summary>
    SixPointedStar,
    /// <summary>
    /// Represents a seven-pointed star.
    /// </summary>
    SevenPointedStar,
    /// <summary>
    /// Represents an eight-pointed star.
    /// </summary>
    EightPointedStar,
    /// <summary>
    /// Represents a nine-pointed star.
    /// </summary>
    NinePointedStar,
    /// <summary>
    /// Represents a ten-pointed star.
    /// </summary>
    TenPointedStar,
    /// <summary>
    /// Represents a five-pointed burst.
    /// </summary>
    FivePointedBurst,
    /// <summary>
    /// Represents a six-pointed burst.
    /// </summary>
    SixPointedBurst,
    /// <summary>
    /// Represents a seven-pointed burst.
    /// </summary>
    SevenPointedBurst,
    /// <summary>
    /// Represents an eight-pointed burst.
    /// </summary>
    EightPointedBurst,
    /// <summary>
    /// Represents a nine-pointed burst.
    /// </summary>
    NinePointedBurst,
    /// <summary>
    /// Represents a ten-pointed burst.
    /// </summary>
    TenPointedBurst,


    // Basic Shapes
    /// <summary>
    /// 
    /// </summary>
    Circle,
    /// <summary>
    /// 
    /// </summary>
    Cloud,
    /// <summary>
    /// 
    /// </summary>
    Crescent,
    /// <summary>
    /// 
    /// </summary>
    Ellipse,
    /// <summary>
    /// Represents a shape containing a rectangle within another regtangle.
    /// </summary>
    FramedRectangle,
    /// <summary>
    /// 
    /// </summary>
    HalfEllipse,
    /// <summary>
    /// Represents a shape resembling a heart.
    /// </summary>
    Heart,
    /// <summary>
    /// Represents a shape resembling a spade.
    /// </summary>
    Spade,
    /// <summary>
    /// Represents a shape resembling a club.
    /// </summary>
    Club,
    /// <summary>
    /// Represents an hour glass shape.
    /// </summary>
    HourGlass,
    /// <summary>
    /// Represents a shape resembling a bolt of lightning.
    /// </summary>
    Lightning,
    /// <summary>
    /// Represents a four-sided figure containing two acute opposite angles,
    /// and two obtuse opposite angles.
    /// </summary>
    Parallelogram1,
    /// <summary>
    /// Represents a four-sided figure containing two acute opposite angles,
    /// and two obtuse opposite angles.
    /// </summary>
    Parallelogram2,
    /// <summary>
    /// Represents a four-sided figure containing four ninety degree angles.
    /// </summary>
    Rectangle,
    /// <summary>
    /// Represents a three-sided figure containing one ninety degree angle.
    /// </summary>
    RightTriangle,
    /// <summary>
    /// 
    /// </summary>
    RoundedIBeam,
    /// <summary>
    /// 
    /// </summary>
    RoundedRectangle,
    /// <summary>
    /// 
    /// </summary>
    Square,
    /// <summary>
    /// Represents a figure in the shape of an 'I'.
    /// </summary>
    SquareIBeam,
    /// <summary>
    /// Represents a figure in the shape of a '+'.
    /// </summary>
    ThickCross,
    /// <summary>
    /// Represents a figure in the shape of a 'X'.
    /// </summary>
    ThickX,
    /// <summary>
    /// Represents a figure in the shape of a '+'.
    /// </summary>
    ThinCross,
    /// <summary>
    /// Represents a figure in the shape of a 'X'.
    /// </summary>
    ThinX,
    /// <summary>
    /// Represents a four-sided figure containing two acute adjacent angles,
    /// and two obtuse adjacent angles. This is the same shape represented by
    /// NodeFigure.ManualLoop and NodeFigure.ManualOperation
    /// </summary>
    Trapezoid,
    /// <summary>
    /// Represents the Yin-Yang symbol.
    /// </summary>
    YinYang,
    /// <summary>
    /// Represents the universal peace symbol.
    /// </summary>
    Peace,
    /// <summary>
    /// Represents a figure used to mean "Not Allowed." In the shape of a circle with a line through.
    /// </summary>
    NotAllowed,
    /// <summary>
    /// Represents a figure used to mean "Fragile." In the shape of a broken glass.
    /// </summary>
    Fragile,
    /// <summary>
    /// Represents the male gender in biology. In the shape of the astronomical Mars symbol.
    /// </summary>
    GenderMale,
    /// <summary>
    /// Represents the female gender in biology. In the shape of the astronomical Venus symbol.
    /// </summary>
    GenderFemale,
    /// <summary>
    /// Represents the "+" symbol using unfilled lines.
    /// </summary>
    PlusLine,
    /// <summary>
    /// Represents an "X" symbol using unfilled lines.
    /// </summary>
    XLine,
    /// <summary>
    /// Represents the "*" symbol using unfilled lines.
    /// </summary>
    AsteriskLine,
    /// <summary>
    /// Represents an unfilled circle.
    /// </summary>
    CircleLine,
    /// <summary>
    /// Represents a Pie with a piece taken out.
    /// </summary>
    Pie,
    /// <summary>
    /// Represents a piece of a pie.
    /// </summary>
    PiePiece,
    /// <summary>
    /// Represents an octagonal Stop Sign.
    /// </summary>
    StopSign,


    // Logical Symbols
    /// <summary>
    /// Logical symbol, represented by an arrow.
    /// </summary>
    LogicImplies,
    /// <summary>
    /// Logical symbol for If and Only If, represented by a double-headed arrow.
    /// </summary>
    LogicIff,
    /// <summary>
    /// Logical symbol for Not or Negation, represented by a horizontal line with a small vertical bar at the end.
    /// </summary>
    LogicNot,
    /// <summary>
    /// Logical symbol for And, represented by a vertically flipped "V".
    /// </summary>
    LogicAnd,
    /// <summary>
    /// Logical symbol for Or, represented by a "V".
    /// </summary>
    LogicOr,
    /// <summary>
    /// Logical symbol for Exclusive Or, represented by a circle with a "+" inscribed inside.
    /// </summary>
    LogicXor,
    /// <summary>
    /// Logical symbol for an unconditional Truth, represented by a "T".
    /// </summary>
    LogicTruth,
    /// <summary>
    /// Logical symbol for an unconditional Falsity, represented by a vertically flipped "T".
    /// </summary>
    LogicFalsity,
    /// <summary>
    /// Logical symbol for existential quantification, represented by a horizontally flipped "E".
    /// </summary>
    LogicThereExists,
    /// <summary>
    /// Logical symbol for universal quantification, represented by a vertically flipped "A".
    /// </summary>
    LogicForAll,
    /// <summary>
    /// Logical symbol for Definition, represented by three horizontal bars.
    /// </summary>
    LogicIsDefinedAs,
    /// <summary>
    /// Logical symbol for Intersection, represented by a vertically flipped "U".
    /// </summary>
    LogicIntersect,
    /// <summary>
    /// Logical symbol for Union, represented by a "U".
    /// </summary>
    LogicUnion,


    // Arrows
    /// <summary>
    /// Represents a basic arrow shape with a square end.
    /// </summary>
    Arrow,
    /// <summary>
    /// A chevron type arrow. This is the same shape represented by
    /// NodeFigure.ISOProcess.
    /// </summary>
    Chevron,
    /// <summary>
    /// Represents a shape consisting of two arrows.
    /// </summary>
    DoubleArrow,
    /// <summary>
    /// Represents an arrow with directional points on each end.
    /// </summary>
    DoubleEndArrow,
    /// <summary>
    /// Represents an arrow with an I-Beam end.
    /// </summary>
    IBeamArrow,
    /// <summary>
    /// 
    /// </summary>
    Pointer,
    /// <summary>
    /// 
    /// </summary>
    RoundedPointer,
    /// <summary>
    /// Represents an arrow with a triangle shaped split at the end.
    /// </summary>
    SplitEndArrow,
    /// <summary>
    /// 
    /// </summary>
    SquareArrow,


    // 3D shapes
    /// <summary>
    /// 
    /// </summary>
    Cone1,
    /// <summary>
    /// 
    /// </summary>
    Cone2,
    /// <summary>
    /// A two dimensional representation of a cube.
    /// </summary>
    Cube1,
    /// <summary>
    /// A two dimensional representation of a cube.
    /// </summary>
    Cube2,
    /// <summary>
    /// 
    /// </summary>
    Cylinder1,
    /// <summary>
    /// 
    /// </summary>
    Cylinder2,
    /// <summary>
    /// 
    /// </summary>
    Cylinder3,
    /// <summary>
    /// 
    /// </summary>
    Cylinder4,
    /// <summary>
    /// 
    /// </summary>
    Prism1,
    /// <summary>
    /// 
    /// </summary>
    Prism2,
    /// <summary>
    /// 
    /// </summary>
    Pyramid1,
    /// <summary>
    /// 
    /// </summary>
    Pyramid2,


    // Flowchart Shapes
    /// <summary>
    /// 
    /// </summary>
    Actor,
    /// <summary>
    /// Flowchart 'alternative' symbol. This is the same shape represented by
    /// NodeFigure.Triangle.
    /// </summary>
    Alternative,
    /// <summary>
    /// Flowchart 'card' symbol.
    /// </summary>
    Card,
    /// <summary>
    /// Flowchart 'collate' symbol.
    /// </summary>
    Collate,
    /// <summary>
    /// Flowchart 'connector' symbol. This is the same shape represented by
    /// NodeFigure.Ellipse.
    /// </summary>
    Connector,
    /// <summary>
    /// Flowchart 'create request' symbol.
    /// </summary>
    CreateRequest,
    /// <summary>
    /// Flowchart 'database' symbol.
    /// </summary>
    Database,
    /// <summary>
    /// Flowchart 'data storage' symbol.
    /// </summary>
    DataStorage,
    /// <summary>
    /// Flowchart 'data transmission' symbol. This is the same shape
    /// represented by NodeFigure.Hexagon.
    /// </summary>
    DataTransmission,
    /// <summary>
    /// Flowchart 'decision' symbol. This is the same shape represented by
    /// NodeFigure.Diamond.
    /// </summary>
    Decision,
    /// <summary>
    /// Flowchart 'delay' symbol. This is the same shape represented by
    /// NodeFigure.HalfEllipse.
    /// </summary>
    Delay,
    /// <summary>
    /// Flowchart 'direct data' symbol. This is the same shape represented by
    /// NodeFigure.Cylinder4.
    /// </summary>
    DirectData,
    /// <summary>
    /// Flowchart 'disk storage' symbol.
    /// </summary>
    DiskStorage,
    /// <summary>
    /// Flowchart 'display' symbol.
    /// </summary>
    Display,
    /// <summary>
    /// Flowchart 'divided event' symbol.
    /// </summary>
    DividedEvent,
    /// <summary>
    /// Flowchart 'divided process' symbol.
    /// </summary>
    DividedProcess,
    /// <summary>
    /// Flowchart 'document' symbol.
    /// </summary>
    Document,
    /// <summary>
    /// Flowchart 'external organization' symbol.
    /// </summary>
    ExternalOrganization,
    /// <summary>
    /// Flowchart 'external process' symbol.
    /// </summary>
    ExternalProcess,
    /// <summary>
    /// Flowchart 'file' symbol.
    /// </summary>
    File,
    /// <summary>
    /// Flowchart 'gate' symbol. This is the same shape represented by
    /// NodeFigure.Crescent.
    /// </summary>
    Gate,
    /// <summary>
    /// Flowchart 'input' symbol. This is the same shape represented by
    /// NodeFigure.Parallelogram1.
    /// </summary>
    Input,
    /// <summary>
    /// Flowchart 'interupt' symbol.
    /// </summary>
    Interupt,
    /// <summary>
    /// Flowchart 'internal storage' symbol.
    /// </summary>
    InternalStorage,
    /// <summary>
    /// Flowchart 'ISO Process' symbol. This is the same shape represented by
    /// NodeFigure.Chevron
    /// </summary>
    ISOProcess,
    /// <summary>
    /// Flowchart 'junction' symbol.
    /// </summary>
    Junction,
    /// <summary>
    /// 
    /// </summary>
    LinedDocument,
    /// <summary>
    /// Flowchart 'loop limit' symbol.
    /// </summary>
    LoopLimit,
    /// <summary>
    /// Flowchart 'magnetic data' symbol. This is the same shape represented by
    /// NodeFigure.Cylinder1.
    /// </summary>
    MagneticData,
    /// <summary>
    /// Flowchart 'magetic tape' symbol.
    /// </summary>
    MagneticTape,
    /// <summary>
    /// Flowchart 'manual input' symbol.
    /// </summary>
    ManualInput,
    /// <summary>
    /// Flowchart 'manual loop' symbol. This is the same shape represented by
    /// NodeFigure.Trapezoid.
    /// </summary>
    ManualLoop,
    /// <summary>
    /// Flowchart 'manual operation' symbol. This is the same shape represented
    /// by NodeFigure.Trapezoid and NodeFigure.ManualInput.
    /// </summary>
    ManualOperation,
    /// <summary>
    /// Flowchart 'merge' symbol. This is the same shape represented by
    /// NodeFigure.Triangle.
    /// </summary>
    Merge,
    /// <summary>
    /// Flowchart 'message from user' symbol.
    /// </summary>
    MessageFromUser,
    /// <summary>
    /// Flowchart 'message to user' symbol.
    /// </summary>
    MessageToUser,
    /// <summary>
    /// 
    /// </summary>
    MicroformProcessing,
    /// <summary>
    /// 
    /// </summary>
    MicroformRecording,
    /// <summary>
    /// Flowchart 'multiple document' symbol.
    /// </summary>
    MultiDocument,
    /// <summary>
    /// Flowchart 'multiple process' symbol.
    /// </summary>
    MultiProcess,
    /// <summary>
    /// 
    /// </summary>
    OfflineStorage,
    /// <summary>
    /// Flowchart 'off page connector' symbol.
    /// </summary>
    OffPageConnector,
    /// <summary>
    /// Flowchart 'or' symbol
    /// </summary>
    Or,
    /// <summary>
    /// Flowchart 'output' symbol. This is the same shape represented by
    /// NodeFigure.Parallelogram1.
    /// </summary>
    Output,
    /// <summary>
    /// 
    /// </summary>
    PaperTape,
    /// <summary>
    /// 
    /// </summary>
    PrimitiveFromCall,
    /// <summary>
    /// 
    /// </summary>
    PrimitiveToCall,
    /// <summary>
    /// Flowchart 'procedure' symbol.
    /// </summary>
    Procedure,
    /// <summary>
    /// Flowchart 'process' symbol.
    /// </summary>
    Process,
    /// <summary>
    /// Flowchart 'sequential data' symbol.
    /// </summary>
    SequentialData,
    /// <summary>
    /// Flowchart 'sort' symbol.
    /// </summary>
    Sort,
    /// <summary>
    /// Flowchart 'start' symbol.
    /// </summary>
    Start,
    /// <summary>
    /// Flowchart 'stored data' symbol.
    /// </summary>
    StoredData,
    /// <summary>
    /// Flowchart 'subroutine' symbol.
    /// </summary>
    Subroutine,
    /// <summary>
    /// Flowchart 'terminator' symbol.
    /// </summary>
    Terminator,
    /// <summary>
    /// Flowchart 'transmittal tape' symbol.
    /// </summary>
    TransmittalTape,


    // Digital Circuits
    /// <summary>
    /// Represents an and gate logic circuit.
    /// </summary>
    AndGate,
    /// <summary>
    /// Represents a buffer logic circuit.
    /// </summary>
    Buffer,
    /// <summary>
    /// Represents a system clock.
    /// </summary>
    Clock,
    /// <summary>
    /// Represents ground.
    /// </summary>
    Ground,
    /// <summary>
    /// Represents an inverter logic circuit.
    /// </summary>
    Inverter,
    /// <summary>
    /// Represents a nand gate logic circuit.
    /// </summary>
    NandGate,
    /// <summary>
    /// Represents a nor gate logic circuit.
    /// </summary>
    NorGate,
    /// <summary>
    /// Represents an or gate logic circuit.
    /// </summary>
    OrGate,
    /// <summary>
    /// Represents an xnor gate logic circuit.
    /// </summary>
    XnorGate,
    /// <summary>
    /// Represents an xor gate logic circuit.
    /// </summary>
    XorGate,
    /// <summary>
    /// Represents a capacitor.
    /// </summary>
    Capacitor,
    /// <summary>
    /// Represents a resistor.
    /// </summary>
    Resistor,
    /// <summary>
    /// Represents an inductor.
    /// </summary>
    Inductor,
    /// <summary>
    /// Represents an AC voltage source.
    /// </summary>
    ACvoltageSource,
    /// <summary>
    /// Represents a DC voltage source.
    /// </summary>
    DCvoltageSource,
    /// <summary>
    /// Represents a diode.
    /// </summary>
    Diode,


    // Computer Shapes
    /// <summary>
    /// Represents a wifi symbol.
    /// </summary>
    Wifi,
    /// <summary>
    /// Represents an email symbol.
    /// </summary>
    Email,
    /// <summary>
    /// Represents an ethernet jack symbol.
    /// </summary>
    Ethernet,
    /// <summary>
    /// Represents the power symbol.
    /// </summary>
    Power,


    // Hazard Shapes
    /// <summary>
    /// Represents the Fallout Shelter symbol.
    /// </summary>
    Fallout,
    /// <summary>
    /// Represents the Irritation Hazard symbol, in the shape of an 'X'.
    /// </summary>
    IrritationHazard,
    /// <summary>
    /// Represents an Electrical Hazard symbol, in the shape of a lightning bolt.
    /// </summary>
    ElectricalHazard,
    /// <summary>
    /// Represents a Fire Hazard symbol, in the shape of a fire.
    /// </summary>
    FireHazard,

    // Business Process Management Notation (BPMN) symbols
    /// <summary>
    /// BPMN Symbol for Activity loop marker
    /// </summary>
    BpmnActivityLoop,
    /// <summary>
    /// BPMN Symbol for Activity Parallel Multi-Instance marker
    /// </summary>
    BpmnActivityParallel,
    /// <summary>
    /// BPMN Symbol for Activity Sequential Multi-Instance marker
    /// </summary>
    BpmnActivitySequential,
    /// <summary>
    /// BPMN Symbol for Activity Ad Hoc marker
    /// </summary>
    BpmnActivityAdHoc,
    /// <summary>
    /// BPMN Symbol for Activity Compensation marker (also use for Compensation Event)
    /// </summary>
    BpmnActivityCompensation,
    /// <summary>
    /// BPMN Symbol for Task Type Send/Receive
    /// </summary>
    BpmnTaskMessage,
    /// <summary>
    /// BPMN Symbol for Task Type Script
    /// </summary>
    BpmnTaskScript,
    /// <summary>
    /// BPMN Symbol for Task Type User
    /// </summary>
    BpmnTaskUser,
    /// <summary>
    /// BPMN Symbol for Event Type Condition
    /// </summary>
    BpmnEventConditional,
    /// <summary>
    /// BPMN Symbol for Event Type Error
    /// </summary>
    BpmnEventError,
    /// <summary>
    /// BPMN Symbol for Event Type Escalation
    /// </summary>
    BpmnEventEscalation,
    /// <summary>
    /// BPMN Symbol for Event Type Timer
    /// </summary>
    BpmnEventTimer,
  };
}
