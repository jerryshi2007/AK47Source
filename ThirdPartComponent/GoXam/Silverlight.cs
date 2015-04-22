


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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Northwoods.GoXam {














































  [System.Diagnostics.Conditional("UNUSED")]
  internal class BrowsableAttribute : Attribute {
    public BrowsableAttribute(bool b) { }
  }

  [System.Diagnostics.Conditional("UNUSED")]
  internal class DesignTimeVisibleAttribute : Attribute {
    public DesignTimeVisibleAttribute(bool b) { }
  }

  [System.Diagnostics.Conditional("UNUSED")]
  internal class DesignerSerializationVisibilityAttribute : Attribute {
    public DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility v) { }
  }
  internal enum DesignerSerializationVisibility {
    Hidden,
    Visible,
    Content
  }

  [System.Diagnostics.Conditional("UNUSED")]
  internal class ValueConversionAttribute : Attribute {
    public ValueConversionAttribute(Type from, Type to) { }
  }

  internal class FrameworkPropertyMetadata : PropertyMetadata {
    public FrameworkPropertyMetadata() : base(null) { }
    public FrameworkPropertyMetadata(Object defval) : base(defval) { }
    public FrameworkPropertyMetadata(Object defval, PropertyChangedCallback changed) : base(defval, changed) { }
  }


  internal static class SystemParameters {
    public static double MinimumHorizontalDragDistance {
      get { return 4; }
    }

    public static double MinimumVerticalDragDistance {
      get { return 4; }
    }

    public static int WheelScrollLines {
      get { return 3; }
    }
  }


  internal static class Clipboard {
    public static bool ContainsData(String format) {
      if (format == null) return false;
      lock (Global) {
        return Global.ContainsKey(format);
      }
    }

    public static Object GetData(String format) {
      if (format == null) return null;
      lock (Global) {
        Object data = null;
        Global.TryGetValue(format, out data);
        return data;
      }
    }

    public static void SetData(String format, Object data) {
      if (format == null) return;
      lock (Global) {
        if (data != null) {
          Global[format] = data;
        } else {
          Global.Remove(format);
        }
      }
    }

    private static Dictionary<String, Object> Global = new Dictionary<String, Object>();
  }


  internal class StreamGeometry {
    public StreamGeometryContext Open() {
      _Geo = new PathGeometry();
      StreamGeometryContext ctxt = new StreamGeometryContext(_Geo);
      return ctxt;
    }

    public FillRule FillRule { get; set; }

    internal PathGeometry _Geo;
  }

  internal class StreamGeometryContext : IDisposable {
    private PathGeometry _Geo;
    private PathFigure _Fig;

    private void CheckState() {
      if (_Geo == null) Diagram.Error("StreamGeometryContext has been closed");
      if (_Fig == null) Diagram.Error("Need to call StreamGeometryContext.BeginFigure first");
    }

    internal StreamGeometryContext(PathGeometry geo) {
      _Geo = geo;
    }

    public void Dispose() { }

    public void BeginFigure(Point startPoint, bool isFilled, bool isClosed) {
      if (_Geo == null) Diagram.Error("StreamGeometryContext has been closed");
      _Fig = new PathFigure();
      _Fig.StartPoint = startPoint;
      _Fig.IsFilled = isFilled;
      _Fig.IsClosed = isClosed;
      _Geo.Figures.Add(_Fig);
    }

    public void Close() {
      _Geo = null;
      _Fig = null;
    }

    public void LineTo(Point point, bool isStroked, bool isSmoothJoin) {
      CheckState();
      LineSegment seg = new LineSegment();
      seg.Point = point;
      _Fig.Segments.Add(seg);
    }

    public void PolyLineTo(IList<Point> points, bool isStroked, bool isSmoothJoin) {
      CheckState();
      PolyLineSegment seg = new PolyLineSegment();
      PointCollection pts = new PointCollection();
      foreach (Point p in points) pts.Add(p);
      seg.Points = pts;
      _Fig.Segments.Add(seg);
    }

    public void BezierTo(Point point1, Point point2, Point point3, bool isStroked, bool isSmoothJoin) {
      CheckState();
      BezierSegment seg = new BezierSegment();
      seg.Point1 = point1;
      seg.Point2 = point2;
      seg.Point3 = point3;
      _Fig.Segments.Add(seg);
    }

    public void PolyBezierTo(IList<Point> points, bool isStroked, bool isSmoothJoin) {
      CheckState();
      PolyBezierSegment seg = new PolyBezierSegment();
      PointCollection pts = new PointCollection();
      foreach (Point p in points) pts.Add(p);
      seg.Points = pts;
      _Fig.Segments.Add(seg);
    }

    public void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection, bool isStroked, bool isSmoothJoin) {
      CheckState();
      ArcSegment seg = new ArcSegment();
      seg.Point = point;
      seg.Size = size;
      seg.RotationAngle = rotationAngle;
      seg.IsLargeArc = isLargeArc;
      seg.SweepDirection = sweepDirection;
      _Fig.Segments.Add(seg);
    }

    public void QuadraticBezierTo(Point point1, Point point2, bool isStroked, bool isSmoothJoin) {
      CheckState();
      QuadraticBezierSegment seg = new QuadraticBezierSegment();
      seg.Point1 = point1;
      seg.Point2 = point2;
      _Fig.Segments.Add(seg);
    }

    public void PolyQuadraticBezierTo(IList<Point> points, bool isStroked, bool isSmoothJoin) {
      CheckState();
      PolyQuadraticBezierSegment seg = new PolyQuadraticBezierSegment();
      PointCollection pts = new PointCollection();
      foreach (Point p in points) pts.Add(p);
      seg.Points = pts;
      _Fig.Segments.Add(seg);
    }
  }

  internal class Vector {
    private double x;
    private double y;

    public Vector(double sx, double sy) {
      x = sx;
      y = sy;
    }

    public double X {
      get { return x; }
      set { x = value; }
    }
    public double Y {
      get { return y; }
      set { y = value; }
    }

    public double Length {
      get { return Math.Sqrt(x * x + y * y); }
    }

    public void Normalize() {
      double tempLength = Length;
      x = x / tempLength;
      y = y / tempLength;
    }

    public static Vector Subtract(Vector v1, Vector v2) {
      return new Vector(v1.X - v2.X, v1.Y - v2.Y);
    }

    public static Vector operator *(double a, Vector b) {
      return new Vector(a * b.X, a * b.Y);
    }

    public static Point operator +(Point a, Vector b) {
      return new Point(a.X + b.X, a.Y + b.Y);
    }
  }
}

