
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
using System.Collections.Generic;

namespace Northwoods.GoXam {

  internal static class Geo {
    public readonly static Size Unlimited = new Size(Double.PositiveInfinity, Double.PositiveInfinity);

    public static void Inflate(ref Rect a, double w, double h) {
      a.X -= w;
      a.Width += w*2;
      a.Y -= h;
      a.Height += h*2;
    }

    public static Size Size(Rect r) {
      return new Size(r.Width, r.Height);
    }

    public static bool Contains(Rect a, Point b) {
      return a.X <= b.X && b.X <= a.X+a.Width && a.Y <= b.Y && b.Y <= a.Y+a.Height;
    }

    public static bool Contains(Rect a, Rect b) {
      return a.X <= b.X && b.X+b.Width <= a.X+a.Width && a.Y <= b.Y && b.Y+b.Height <= a.Y+a.Height && a.Width >= 0 && a.Height >= 0;
    }

    public static Rect Union(Rect a, Rect b) {
      double minx = Math.Min(a.X, b.X);
      double miny = Math.Min(a.Y, b.Y);
      double maxr = Math.Max(a.X+a.Width, b.X+b.Width);
      double maxb = Math.Max(a.Y+a.Height, b.Y+b.Height);
      return new Rect(minx, miny, maxr-minx, maxb-miny);
    }

    public static Rect Union(Rect a, Point b) {
      if (b.X < a.X) {
        a.Width = (a.X+a.Width-b.X);
        a.X = b.X;
      } else if (b.X > a.X+a.Width) {
        a.Width = (b.X-a.X);
      }
      if (b.Y < a.Y) {
        a.Height = (a.Y+a.Height-b.Y);
        a.Y = b.Y;
      } else if (b.Y > a.Y+a.Height) {
        a.Height = (b.Y-a.Y);
      }
      return a;
    }

    //public static Rect Union(Point a, Point b) { return LineBounds(a, b); }

    public static Rect Intersection(Rect a, Rect b) {
      double maxx = Math.Max(a.X, b.X);
      double maxy = Math.Max(a.Y, b.Y);
      double minr = Math.Min(a.X+a.Width, b.X+b.Width);
      double minb = Math.Min(a.Y+a.Height, b.Y+b.Height);
      return new Rect(maxx, maxy, Math.Max(0, minr-maxx), Math.Max(0, minb-maxy));
    }

    public static bool Intersects(Rect a, Rect b) {
      double tw = a.Width;
      double rw = b.Width;
      double tx = a.X;
      double rx = b.X;
      if (!Double.IsPositiveInfinity(tw) && !Double.IsPositiveInfinity(rw)) {
        tw += tx;
        rw += rx;
        if (Double.IsNaN(rw) || Double.IsNaN(tw) || tx > rw || rx > tw) return false;
      }

      double th = a.Height;
      double rh = b.Height;
      double ty = a.Y;
      double ry = b.Y;
      if (!Double.IsPositiveInfinity(th) && !Double.IsPositiveInfinity(rh)) {
        th += ty;
        rh += ry;
        if (Double.IsNaN(rh) || Double.IsNaN(th) || ty > rh || ry > th) return false;
      }
      return true;
    }


    public static bool IsApprox(double x, double y) {
      double d = x - y;
      return d < 0.5 && d > -0.5;
    }

    public static bool IsApproxEqual(double x, double y) {
      double d = x - y;
      return d < 0.00000005 && d > -0.00000005;
    }

    public static bool IsApproxEqual(Point a, Point b) {
      return IsApproxEqual(a.X, b.X) && IsApproxEqual(a.Y, b.Y);
    }

    public static bool IsApprox(Point a, Point b) {
      return IsApprox(a.X, b.X) && IsApprox(a.Y, b.Y);
    }

    public static bool IsApprox(Size a, Size b) {
      return IsApprox(a.Width, b.Width) && IsApprox(a.Height, b.Height);
    }

    public static bool IsApprox(Rect a, Rect b) {
      return IsApprox(a.X, b.X) && IsApprox(a.Y, b.Y) && IsApprox(a.Width, b.Width) && IsApprox(a.Height, b.Height);
    }


    public static Point Add(Point a, Point b) {
      return new Point(a.X+b.X, a.Y+b.Y);
    }

    public static Point Subtract(Point a, Point b) {
      return new Point(a.X-b.X, a.Y-b.Y);
    }

    public static Rect Bounds(Point[] a) {
      if (a == null || a.Length == 0) return new Rect(0, 0, 0, 0);
      double minx = Double.PositiveInfinity;
      double maxx = Double.NegativeInfinity;
      double miny = Double.PositiveInfinity;
      double maxy = Double.NegativeInfinity;
      foreach (Point p in a) {
        minx = Math.Min(minx, p.X);
        maxx = Math.Max(maxx, p.X);
        miny = Math.Min(miny, p.Y);
        maxy = Math.Max(maxy, p.Y);
      }
      return new Rect(minx, miny, maxx-minx, maxy-miny);
    }

    public static Rect Bounds(IEnumerable<Point> a) {
      if (a == null) return new Rect(0, 0, 0, 0);
      double minx = Double.PositiveInfinity;
      double maxx = Double.NegativeInfinity;
      double miny = Double.PositiveInfinity;
      double maxy = Double.NegativeInfinity;
      foreach (Point p in a) {
        minx = Math.Min(minx, p.X);
        maxx = Math.Max(maxx, p.X);
        miny = Math.Min(miny, p.Y);
        maxy = Math.Max(maxy, p.Y);
      }
      if (minx == Double.PositiveInfinity) return new Rect(0, 0, 0, 0);
      return new Rect(minx, miny, maxx-minx, maxy-miny);
    }

    /// <summary>
    /// Compute a <c>Size</c> that fits in <paramref name="target"/> while maintaining
    /// the aspect ratio given by <paramref name="aspect"/>.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="aspect">if both width and height are zero or negative, assume 1x1</param>
    /// <returns></returns>
    public static Size LargestSizeKeepingAspectRatio(Size target, Size aspect) {
      double ax = Math.Max(0, aspect.Width);
      double ay = Math.Max(0, aspect.Height);
      // if both are zero or negative, just assume aspect ratio of 1x1
      if (ax == 0 && ay == 0) {
        ax = 1;
        ay = 1;
      }
      double tx = Math.Max(0, target.Width);
      double ty = Math.Max(0, target.Height);
      if (ax == 0) {  // no width
        return new Size(0, ty);
      } else if (ay == 0) {  // no height
        return new Size(tx, 0);
      } else {
        if (tx == 0 || ty == 0) {
          return new Size(tx, ty);
        } else {
          double aratio = ay/ax;
          double tratio = ty/tx;
          if (aratio < tratio)
            return new Size(tx, aratio*tx);
          else
            return new Size(ty/aratio, ty);
        }
      }
    }

    /// <summary>
    /// Find the closest point of a rectangle to a given point that is on a line from that point.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="p1">
    /// the point we are looking to be closest to, on the line formed with <paramref name="p2"/>
    /// </param>
    /// <param name="p2">
    /// forms a line with <paramref name="p1"/>
    /// </param>
    /// <param name="result">
    /// the point of this object that is closest to <paramref name="p1"/> and that is on
    /// the infinite line from <paramref name="p1"/> to <paramref name="p2"/>
    /// </param>
    /// <returns>
    /// true if the infinite line does intersect with the rectangle; false otherwise
    /// </returns>
    public static bool GetNearestIntersectionPoint(Rect rect, Point p1, Point p2, out Point result) {
      Point topleft = new Point(rect.X, rect.Y);
      Point topright = new Point(rect.X + rect.Width, rect.Y);
      Point bottomleft = new Point(rect.X, rect.Y + rect.Height);
      Point bottomright = new Point(rect.X + rect.Width, rect.Y + rect.Height);

      // this is the point we want to be closest to
      double Cx = p1.X;
      double Cy = p1.Y;

      Point P;
      double closestdist = 10e20f;
      Point closestpoint = topleft;
      if (NearestIntersectionOnLine(topleft, topright, p1, p2, out P)) {
        // calculate the (non-square-rooted) distance from C to P
        double dist = (P.X-Cx) * (P.X-Cx) + (P.Y-Cy) * (P.Y-Cy);
        if (dist < closestdist) {
          closestdist = dist;
          closestpoint = P;
        }
      }

      if (NearestIntersectionOnLine(topright, bottomright, p1, p2, out P)) {
        double dist = (P.X-Cx) * (P.X-Cx) + (P.Y-Cy) * (P.Y-Cy);
        if (dist < closestdist) {
          closestdist = dist;
          closestpoint = P;
        }
      }

      if (NearestIntersectionOnLine(bottomright, bottomleft, p1, p2, out P)) {
        double dist = (P.X-Cx) * (P.X-Cx) + (P.Y-Cy) * (P.Y-Cy);
        if (dist < closestdist) {
          closestdist = dist;
          closestpoint = P;
        }
      }

      if (NearestIntersectionOnLine(bottomleft, topleft, p1, p2, out P)) {
        double dist = (P.X-Cx) * (P.X-Cx) + (P.Y-Cy) * (P.Y-Cy);
        if (dist < closestdist) {
          closestdist = dist;
          closestpoint = P;
        }
      }

      result = closestpoint;
      return (closestdist < 10e20f);
    }

    public static bool IntersectsLineSegment(Rect rect, Point p1, Point p2) {
      if (p1.X == p2.X)
        return (rect.Left <= p1.X && p1.X <= rect.Right && Math.Min(p1.Y, p2.Y) <= rect.Bottom && Math.Max(p1.Y, p2.Y) >= rect.Top);
      if (p1.Y == p2.Y)
        return (rect.Top <= p1.Y && p1.Y <= rect.Bottom && Math.Min(p1.X, p2.X) <= rect.Right && Math.Max(p1.X, p2.X) >= rect.Left);
      if (Contains(rect, p1))
        return true;
      if (Contains(rect, p2))
        return true;
      if (IntersectingLines(new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top), p1, p2))
        return true;
      if (IntersectingLines(new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Bottom), p1, p2))
        return true;
      if (IntersectingLines(new Point(rect.Right, rect.Bottom), new Point(rect.Left, rect.Bottom), p1, p2))
        return true;
      if (IntersectingLines(new Point(rect.Left, rect.Bottom), new Point(rect.Left, rect.Top), p1, p2))
        return true;
      return false;
    }

    // ?? ContainsLineSegment, ContainsRectangle, IntersectsLineSegment, IntersectsRectangle

    public static bool LineContainsPoint(Point a, Point b, double fuzz, Point p) {
      if (fuzz <= 0) fuzz = .000001;
      double maxx, minx, maxy, miny;
      if (a.X < b.X) {
        minx = a.X;
        maxx = b.X;
      } else {
        minx = b.X;
        maxx = a.X;
      }
      if (a.Y < b.Y) {
        miny = a.Y;
        maxy = b.Y;
      } else {
        miny = b.Y;
        maxy = a.Y;
      }

      // do simple vertical and horizontal cases first, without fuzz beyond the endpoints
      if (a.X == b.X) return (miny <= p.Y && p.Y <= maxy && a.X-fuzz <= p.X && p.X <= a.X+fuzz);
      if (a.Y == b.Y) return (minx <= p.X && p.X <= maxx && a.Y-fuzz <= p.Y && p.Y <= a.Y+fuzz);

      // if we're in the x-range, including fuzz,
      double xrange_high = maxx + fuzz;
      double xrange_low = minx - fuzz;
      if ((xrange_low <= p.X) && (p.X <= xrange_high)) {

        // and if we're in the y-range
        double yrange_high = maxy + fuzz;
        double yrange_low = miny - fuzz;
        if ((yrange_low <= p.Y) && (p.Y <= yrange_high)) {

          // see if we should compute the X coordinate or Y coordinate
          if (xrange_high - xrange_low > yrange_high - yrange_low) {
            if (a.X - b.X > fuzz || b.X - a.X > fuzz) {
              double slope = (b.Y - a.Y) / (b.X - a.X);
              double guess_y = (slope * (p.X - a.X) + a.Y);

              if ((guess_y - fuzz <= p.Y) && (p.Y <= guess_y + fuzz)) {
                return true;
              }
            } else {
              return true;
            }
          } else {
            if (a.Y - b.Y > fuzz || b.Y - a.Y > fuzz) {
              double slope = (b.X - a.X) / (b.Y - a.Y);
              double guess_x = (slope * (p.Y - a.Y) + a.X);

              if ((guess_x - fuzz <= p.X) && (p.X <= guess_x + fuzz)) {
                return true;
              }
            } else {
              return true;
            }
          }
        }
      }
      return false;
    }

    public static bool BezierContainsPoint(Point s, Point c1, Point c2, Point e, double epsilon, Point p) {
      if (!LineContainsPoint(s, e, epsilon, c1) || !LineContainsPoint(s, e, epsilon, c2)) {
        Point a1 = new Point((s.X+c1.X)/2, (s.Y+c1.Y)/2);
        Point a2 = new Point((c1.X+c2.X)/2, (c1.Y+c2.Y)/2);
        Point a3 = new Point((c2.X+e.X)/2, (c2.Y+e.Y)/2);

        Point v = new Point((a1.X+a2.X)/2, (a1.Y+a2.Y)/2);
        Point w = new Point((a2.X+a3.X)/2, (a2.Y+a3.Y)/2);
        Point m = new Point((v.X+w.X)/2, (v.Y+w.Y)/2);
        return BezierContainsPoint(s, a1, v, m, epsilon, p) || BezierContainsPoint(m, w, a3, e, epsilon, p);
      } else {
        return LineContainsPoint(s, e, epsilon, p);
      }
    }

    public static void BezierMidPoint(Point b0, Point b1, Point b2, Point b3, out Point v, out Point w) {
      Point c1 = new Point((b0.X+b1.X)/2, (b0.Y+b1.Y)/2);
      Point c2 = new Point((b1.X+b2.X)/2, (b1.Y+b2.Y)/2);
      Point c3 = new Point((b2.X+b3.X)/2, (b2.Y+b3.Y)/2);

      v = new Point((c1.X+c2.X)/2, (c1.Y+c2.Y)/2);
      w = new Point((c2.X+c3.X)/2, (c2.Y+c3.Y)/2);
    }

    public static Rect LineBounds(Point a, Point b) {
      double minx = Math.Min(a.X, b.X);
      double miny = Math.Min(a.Y, b.Y);
      double maxx = Math.Max(a.X, b.X);
      double maxy = Math.Max(a.Y, b.Y);
      return new Rect(minx, miny, maxx-minx, maxy-miny);
    }

    public static Rect BezierBounds(Point s, Point c1, Point c2, Point e, double epsilon) {
      if (!LineContainsPoint(s, e, epsilon, c1) || !LineContainsPoint(s, e, epsilon, c2)) {
        Point a1 = new Point((s.X+c1.X)/2, (s.Y+c1.Y)/2);
        Point a2 = new Point((c1.X+c2.X)/2, (c1.Y+c2.Y)/2);
        Point a3 = new Point((c2.X+e.X)/2, (c2.Y+e.Y)/2);

        Point v = new Point((a1.X+a2.X)/2, (a1.Y+a2.Y)/2);
        Point w = new Point((a2.X+a3.X)/2, (a2.Y+a3.Y)/2);
        Point m = new Point((v.X+w.X)/2, (v.Y+w.Y)/2);
        return Union(BezierBounds(s, a1, v, m, epsilon), BezierBounds(m, w, a3, e, epsilon));
      } else {
        return LineBounds(s, e);
      }
    }

    public static bool BezierNearestIntersectionOnLine(Point s, Point c1, Point c2, Point e, Point p1, Point p2, double epsilon, out Point result) {
      if (epsilon <= 0) epsilon = .000001;

      Point R;
      double closestdist = 10e20f;
      Point closestpoint = s;
      if (!LineContainsPoint(s, e, epsilon, c1) || !LineContainsPoint(s, e, epsilon, c2)) {
        Point a1 = new Point((s.X+c1.X)/2, (s.Y+c1.Y)/2);
        Point a2 = new Point((c1.X+c2.X)/2, (c1.Y+c2.Y)/2);
        Point a3 = new Point((c2.X+e.X)/2, (c2.Y+e.Y)/2);
        Point v = new Point((a1.X+a2.X)/2, (a1.Y+a2.Y)/2);
        Point w = new Point((a2.X+a3.X)/2, (a2.Y+a3.Y)/2);
        Point m = new Point((v.X+w.X)/2, (v.Y+w.Y)/2);

        if (BezierNearestIntersectionOnLine(s, a1, v, m, p1, p2, epsilon, out R)) {
          double dist = (R.X-p1.X) * (R.X-p1.X) + (R.Y-p1.Y) * (R.Y-p1.Y);
          if (dist < closestdist) {
            closestdist = dist;
            closestpoint = R;
          }
        }

        if (BezierNearestIntersectionOnLine(m, w, a3, e, p1, p2, epsilon, out R)) {
          double dist = (R.X-p1.X) * (R.X-p1.X) + (R.Y-p1.Y) * (R.Y-p1.Y);
          if (dist < closestdist) {
            closestdist = dist;
            closestpoint = R;
          }
        }
      } else {
        if (NearestIntersectionOnLine(s, e, p1, p2, out R)) {
          // calculate the (non-square-rooted) distance from P1 to R
          double dist = (R.X-p1.X) * (R.X-p1.X) + (R.Y-p1.Y) * (R.Y-p1.Y);
          if (dist < closestdist) {
            closestdist = dist;
            closestpoint = R;
          }
        }
      }
      result = closestpoint;
      return (closestdist < 10e20f);
    }

    /// <summary>
    /// Return a point on a straight line segment that is closest to a given point.
    /// </summary>
    /// <param name="a">One end of the line.</param>
    /// <param name="b">The other end of the line.</param>
    /// <param name="p">The point to be closest to.</param>
    /// <param name="result">
    /// A <c>Point</c> that is on the finite length straight line segment from
    /// <paramref name="a"/> to <paramref name="b"/>
    /// </param>
    /// <returns>
    /// true if the point <paramref name="p"/> is on a perpendicular line to the line segment;
    /// false if the point <paramref name="p"/> is beyond either end of the line segment.
    /// When this returns false, the <paramref name="result"/> will be either
    /// <paramref name="a"/> or <paramref name="b"/>.
    /// </returns>
    public static bool NearestPointOnLine(Point a, Point b, Point p, out Point result) {
      // A and B are the endpoints of this segment
      double Ax = a.X;
      double Ay = a.Y;
      double Bx = b.X;
      double By = b.Y;
      // P is the point we want to get closest to
      double Px = p.X;
      double Py = p.Y;

      // handle vertical and horizontal lines specially
      if (Ax == Bx) {
        double min, max;
        if (Ay < By) {
          min = Ay;
          max = By;
        } else {
          min = By;
          max = Ay;
        }
        double newp = Py;
        if (newp < min) {
          result = new Point(Ax, min);
          return false;
        }
        if (newp > max) {
          result = new Point(Ax, max);
          return false;
        }
        result = new Point(Ax, newp);
        return true;
      } else if (Ay == By) {
        double min, max;
        if (Ax < Bx) {
          min = Ax;
          max = Bx;
        } else {
          min = Bx;
          max = Ax;
        }
        double newp = Px;
        if (newp < min) {
          result = new Point(min, Ay);
          return false;
        }
        if (newp > max) {
          result = new Point(max, Ay);
          return false;
        }
        result = new Point(newp, Ay);
        return true;
      } else {
        // ought to take sqrt to get real length, but don't bother...
        double L = (Bx-Ax) * (Bx-Ax) + (By-Ay) * (By-Ay);
        // ought to be dividing By L^2, but didn't bother to sqrt!
        double Q = ((Ax-Px) * (Ax-Bx) + (Ay-Py) * (Ay-By)) / L;

        if (Q < 0) {
          result = a;
          return false;
        } else if (Q > 1) {
          result = b;
          return false;
        } else {
          // OK to use point on line between A and B
          double x = Ax + Q * (Bx-Ax);
          double y = Ay + Q * (By-Ay);
          result = new Point(x, y);
          return true;
        }
      }
    }

    /// <summary>
    /// Find the intersection point of the finite line segment A-B and the infinite line P-Q
    /// that is closest to point P.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="p"></param>
    /// <param name="q"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool NearestIntersectionOnLine(Point a, Point b, Point p, Point q, out Point result) {
      if (a == b) {
        result = a;
        return false;
      }
      // A and B are the endpoints of this segment
      double Ax = a.X;
      double Ay = a.Y;
      double Bx = b.X;
      double By = b.Y;
      // P-Q form an infinite line that we're looking to intersect with the A-B finite line segment.
      // P is the point we want to get closest to
      double Px = p.X;
      double Py = p.Y;
      double Qx = q.X;
      double Qy = q.Y;

      if (Px == Qx) {  // P-Q line is vertical
        if (Ax == Bx) {  // parallel vertical lines--easiest to just return closest point
          NearestPointOnLine(a, b, p, out result);
          return false;
        }
        else {  // line segment A-B is not vertical, but P-Q is
          double M = (By - Ay) / (Bx - Ax);
          double y = M * (Px - Ax) + Ay;
          return NearestPointOnLine(a, b, new Point(Px, y), out result);
        }
      }
      else {
        double S = (Qy - Py) / (Qx - Px);
        if (Ax == Bx) {  // line segment A-B is vertical, but P-Q is not
          double y = S * (Ax - Px) + Py;
          if (y < Math.Min(Ay, By)) {
            result = new Point(Ax, Math.Min(Ay, By));
            return false;
          }
          if (y > Math.Max(Ay, By)) {
            result = new Point(Ax, Math.Max(Ay, By));
            return false;
          }
          result = new Point(Ax, y);
          return true;
        }
        else {  // neither line is vertical
          double M = (By - Ay) / (Bx - Ax);
          if (S == M) {  // parallel lines--easiest to just return closest point
            NearestPointOnLine(a, b, p, out result);
            return false;
          }
          else {
            double x = (M * Ax - S * Px + Py - Ay) / (M - S);
            if (M == 0) {  // line segment A-B is horizontal
              if (x < Math.Min(Ax, Bx)) {
                result = new Point(Math.Min(Ax, Bx), Ay);
                return false;
              }
              if (x > Math.Max(Ax, Bx)) {
                result = new Point(Math.Max(Ax, Bx), Ay);
                return false;
              }
              result = new Point(x, Ay);
              return true;
            }
            else {
              double y = M * (x - Ax) + Ay;
              return NearestPointOnLine(a, b, new Point(x, y), out result);
            }
          }
        }
      }
    }

    /// <summary>
    /// Return the angle of the line going from the origin to a point.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>
    /// an angle in degrees, with <c>0</c> along the positive X axis, and
    /// with <c>90.0</c> along the positive Y axis.
    /// </returns>
    public static double GetAngle(double x, double y) {
      double A;
      if (x == 0) {
        if (y > 0)
          A = 90;
        else
          A = 270;
      } else if (y == 0) {
        if (x > 0)
          A = 0;
        else
          A = 180;
      } else {
        if (Double.IsNaN(x) || Double.IsNaN(y)) return 0;
        A = Math.Atan(Math.Abs(y/x))*180/Math.PI;
        if (x < 0) {
          if (y < 0)
            A += 180;
          else
            A = 180-A;
        } else if (y < 0) {
          A = 360-A;
        }
      }
      return A;
    }

    public static double GetAngle(Point a, Point b) {
      return GetAngle(b.X-a.X, b.Y-a.Y);
    }

    public static double NormalizeAngle(double a) {
      if (a < 0) a += 360;
      if (a >= 360) a -= 360;
      return a;
    }


    // This predicate returns true if two finite straight line segments intersect.
    public static bool IntersectingLines(Point a1, Point a2, Point b1, Point b2) {
      return ((ComparePointWithLine(a1, a2, b1) * ComparePointWithLine(a1, a2, b2) <= 0) && (ComparePointWithLine(b1, b2, a1) * ComparePointWithLine(b1, b2, a2) <= 0));
    }

    public static int ComparePointWithLine(Point a1, Point a2, Point p) {
      double x2 = a2.X - a1.X;
      double y2 = a2.Y - a1.Y;
      double px = p.X - a1.X;
      double py = p.Y - a1.Y;
      double ccw = px*y2 - py*x2;
      if (ccw == 0) {
        ccw = px*x2 + py*y2;
        if (ccw > 0) {
          px -= x2;
          py -= y2;
          ccw = px*x2 + py*y2;
          if (ccw < 0) ccw = 0;
        }
      }
      return (ccw < 0) ? -1 : ((ccw > 0) ? 1 : 0);
    }


    private static double GetRadians(double x, double y) {
      double A;
      if (x == 0) {
        if (y > 0)
          A = Math.PI / 2;
        else
          A = -Math.PI / 2;
      } else if (y == 0) {
        if (x > 0)
          A = 0;
        else
          A = Math.PI;
      } else {
        A = Math.Atan(Math.Abs(y / x));
        if (x < 0) {
          if (y < 0)
            A += Math.PI;
          else
            A = Math.PI - A;
        } else if (y < 0) {
          A = 2 * Math.PI - A;
        }
      }
      return A;
    }

    public static Matrix Multiply(Matrix matrix1, Matrix matrix2) {
      if (matrix2.IsIdentity) return matrix1;
      return new Matrix((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21),
        (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22),
        (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21),
        (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22),
        ((matrix1.OffsetX * matrix2.M11) + (matrix1.OffsetY * matrix2.M21)) + matrix2.OffsetX,
        ((matrix1.OffsetX * matrix2.M12) + (matrix1.OffsetY * matrix2.M22)) + matrix2.OffsetY);
    }

    /// <summary>
    /// Find the intersection point of the elliptical path defined by rectangle rect and an infinite
    /// line p1-p2 that is closest to point p1.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool NearestIntersectionOnEllipse(Rect rect, Point p1, Point p2, out Point result) {
      if (rect.Width == 0)
        return NearestIntersectionOnLine(new Point(rect.X, rect.Y), new Point(rect.X, rect.Y + rect.Height), p1, p2, out result);
      else if (rect.Height == 0)
        return NearestIntersectionOnLine(new Point(rect.X, rect.Y), new Point(rect.X + rect.Width, rect.Y), p1, p2, out result);
      else {

        double Rx = rect.Width/2;
        double Ry = rect.Height/2;

        double Cx = rect.X + Rx;
        double Cy = rect.Y + Ry;

        // see if the slope of P1-P2 is vertical or close to it (1 : 10000)
        double m = 9999;
        if (p1.X > p2.X)
          m = (p1.Y - p2.Y)/(p1.X - p2.X);
        else if (p1.X < p2.X)
          m = (p2.Y - p1.Y)/(p2.X - p1.X);

        if (Math.Abs(m) < 9999) {
          double b = (p1.Y - Cy) - m*(p1.X - Cx);

          if ((Rx*Rx)*(m*m) + (Ry*Ry) - (b*b) < 0) {
            result = new Point();  //?? should this return the closest point, even if there's no intersection
            return false;
          }

          double sqrt = Math.Sqrt((Rx*Rx)*(m*m) + (Ry*Ry) - (b*b));

          double xplus = ((-((Rx*Rx) * m * b) + (Rx*Ry*sqrt)) / ((Ry*Ry) + ((Rx*Rx) * (m*m)))) + Cx;
          double xminus = ((-((Rx*Rx) * m * b) - (Rx*Ry*sqrt)) / ((Ry*Ry) + ((Rx*Rx) * (m*m)))) + Cx;

          double yplus = m*(xplus - Cx) + b + Cy;
          double yminus = m*(xminus - Cx) + b + Cy;

          double distplus = Math.Abs((p1.X - xplus)*(p1.X - xplus)) + Math.Abs((p1.Y - yplus)*(p1.Y - yplus));
          double distminus = Math.Abs((p1.X - xminus)*(p1.X - xminus)) + Math.Abs((p1.Y - yminus)*(p1.Y - yminus));

          if (distplus < distminus)
            result = new Point(xplus, yplus);
          else
            result = new Point(xminus, yminus);

        } else {
          double Ry2 = Ry*Ry;
          double Rx2 = Rx*Rx;
          double diff = p1.X-Cx;
          double sqr = Ry2 - (Ry2/Rx2)*(diff*diff);
          if (sqr < 0) {
            result = new Point();
            return false;
          }

          double sqrt = Math.Sqrt(sqr);

          double yplus = Cy + sqrt;
          double yminus = Cy - sqrt;

          double distplus = Math.Abs(yplus - p1.Y);
          double distminus = Math.Abs(yminus - p1.Y);

          if (distplus < distminus)
            result = new Point(p1.X, yplus);
          else
            result = new Point(p1.X, yminus);

        }
        return true;
      }
    }

    /// <summary>
    /// Find the intersection point of the elliptical path defined by rectangle rect and an infinite
    /// line p1-p2 that is closest to point p1 within the area from startAngle through the sweepAngle.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="result"></param>
    /// <param name="startAngle"></param>
    /// <param name="sweepAngle"></param>
    /// <returns></returns>
    public static bool NearestIntersectionOnArc(Rect rect, Point p1, Point p2, out Point result, double startAngle, double sweepAngle) {

      double Rx = rect.Width/2;
      double Ry = rect.Height/2;

      double Cx = rect.X + Rx;
      double Cy = rect.Y + Ry;

      double sa;
      double sw;

      if (sweepAngle < 0) {
        sa = startAngle + sweepAngle;
        sw = -sweepAngle;
      } else {
        sa = startAngle;
        sw = sweepAngle;
      }

      if (p1.X != p2.X) {

        double m;
        if (p1.X > p2.X)
          m = (p1.Y - p2.Y)/(p1.X - p2.X);
        else
          m = (p2.Y - p1.Y)/(p2.X - p1.X);

        double b = (p1.Y - Cy) - m*(p1.X - Cx);

        double sqrt = Math.Sqrt((Rx*Rx)*(m*m) + (Ry*Ry) - (b*b));

        double xplus = ((-((Rx*Rx) * m * b) + (Rx*Ry*sqrt)) / ((Ry*Ry) + ((Rx*Rx) * (m*m)))) + Cx;
        double xminus = ((-((Rx*Rx) * m * b) - (Rx*Ry*sqrt)) / ((Ry*Ry) + ((Rx*Rx) * (m*m)))) + Cx;

        double yplus = m*(xplus - Cx) + b + Cy;
        double yminus = m*(xminus - Cx) + b + Cy;

        double angleplus = GetAngle(xplus - Cx, yplus - Cy);
        double angleminus = GetAngle(xminus - Cx, yminus - Cy);

        if (angleplus < sa)
          angleplus += 360;
        if (angleminus < sa)
          angleminus += 360;
        if (angleplus > sa + sw)
          angleplus -= 360;
        if (angleminus > sa + sw)
          angleminus -= 360;

        bool bplus = ((angleplus >= sa) && (angleplus <= sa + sw));
        bool bminus = ((angleminus >= sa) && (angleminus <= sa + sw));

        if (bplus && bminus) {

          double distplus = Math.Abs((p1.X - xplus)*(p1.X - xplus)) + Math.Abs((p1.Y - yplus)*(p1.Y - yplus));
          double distminus = Math.Abs((p1.X - xminus)*(p1.X - xminus)) + Math.Abs((p1.Y - yminus)*(p1.Y - yminus));

          if (distplus < distminus)
            result = new Point(xplus, yplus);
          else
            result = new Point(xminus, yminus);

          return true;

        } else if (bplus && !bminus) {
          result = new Point(xplus, yplus);
          return true;

        } else if (!bplus && bminus) {
          result = new Point(xminus, yminus);
          return true;

        } else // (!bplus && !bminus)
          result = new Point();
        return false;

      } else {

        double sqrt = Math.Sqrt((Ry*Ry) - ((Ry*Ry)/(Rx*Rx))*((p1.X-Cx)*(p1.X-Cx)));

        double yplus = Cy + sqrt;
        double yminus = Cy - sqrt;

        double angleplus = GetAngle(p1.X - Cx, yplus - Cy);
        double angleminus = GetAngle(p1.X - Cx, yminus - Cy);

        if (angleplus < sa)
          angleplus += 360;
        if (angleminus < sa)
          angleminus += 360;
        if (angleplus > sa + sw)
          angleplus -= 360;
        if (angleminus > sa + sw)
          angleminus -= 360;

        bool bplus = ((angleplus >= sa) && (angleplus <= sa + sw));
        bool bminus = ((angleminus >= sa) && (angleminus <= sa + sw));

        if (bplus && bminus) {

          double distplus = Math.Abs(yplus - p1.Y);
          double distminus = Math.Abs(yminus - p1.Y);

          if (distplus < distminus)
            result = new Point(p1.X, yplus);
          else
            result = new Point(p1.X, yminus);
          return true;

        } else if (bplus && !bminus) {
          result = new Point(p1.X, yplus);
          return true;

        } else if (!bplus && bminus) {
          result = new Point(p1.X, yminus);
          return true;

        } else  //(!bplus && !bminus)
          result = new Point();
        return false;
      }
    }

    public static Point PointOnEllipse(Rect rect, double angle) {
      double a = rect.Width/2;
      double b = rect.Height/2;

      if (angle == 0)
        return new Point(rect.Right, rect.Top+b);
      if (angle == 90)
        return new Point(rect.Left+a, rect.Bottom);
      if (angle == 180)
        return new Point(rect.Left, rect.Top+b);
      if (angle == 270)
        return new Point(rect.Left+a, rect.Top);

      double theta = angle*Math.PI/180;
      double sin = Math.Sin(theta);
      double cos = Math.Cos(theta);

      double e2 = 1 - (b*b)/(a*a);
      double r = a*Math.Sqrt((1- e2) / (1 - e2*cos*cos));

      return new Point(cos*r + rect.X+a, sin*r + rect.Y+b);
    }


    public readonly static double MagicBezierFactor = (4 * (1 - Math.Cos(Math.PI/4))) / (3 * Math.Sin(Math.PI/4));


    public static Point RotatePoint(Point p, Point c, double angle) {
      if (angle == 0 || p == c)
        return p;
      double rad = angle * Math.PI / 180;
      double cosine = Math.Cos(rad);
      double sine = Math.Sin(rad);
      double dx = p.X - c.X;
      double dy = p.Y - c.Y;
      return new Point((c.X + cosine * dx - sine * dy),
                       (c.Y + sine * dx + cosine * dy));
    }
    
    public static Point RotatePoint(Point p, Point c, double cosine, double sine) {
      if (p == c)
        return p;
      double dx = p.X - c.X;
      double dy = p.Y - c.Y;
      return new Point((c.X + cosine * dx - sine * dy),
                       (c.Y + sine * dx + cosine * dy));
    }

    public static void RotatePoints(Point[] u, Point[] r, Point rotatePoint, double angle) {
      if (angle == 0) {
        int len = Math.Min(u.Length, r.Length);
        Array.Copy(u, r, len);
      } else {
        double rad = angle * Math.PI / 180;
        double cosine = Math.Cos(rad);
        double sine = Math.Sin(rad);
        RotatePoints(u, r, rotatePoint, cosine, sine);
      }
    }

    public static void RotatePoints(Point[] u, Point[] r, Point rotatePoint, double cosine, double sine) {
      int len = Math.Min(u.Length, r.Length);
      if (cosine == 1 && sine == 0) {
        Array.Copy(u, r, len);
      } else {
        for (int i = 0; i < len; i++) {
          r[i] = RotatePoint(u[i], rotatePoint, cosine, sine);
        }
      }
    }

    public static Point RescalePoint(Rect oldr, Rect newr, Point rp, double angle) {
      Point rp2 = rp;
      double cosine = 1;
      double sine = 0;
      if (angle != 0) {
        double rad = angle * Math.PI / 180;
        cosine = Math.Cos(rad);
        sine = Math.Sin(rad);
        rp2 = RotatePoint(rp, new Point(oldr.X + oldr.Width / 2, oldr.Y + oldr.Height / 2), cosine, -sine);
      }
      double scaleFactorX = (oldr.Width <= 0 ? 1 : (newr.Width / oldr.Width));
      double scaleFactorY = (oldr.Height <= 0 ? 1 : (newr.Height / oldr.Height));
      double newX = newr.X + ((rp2.X - oldr.X) * scaleFactorX);
      double newY = newr.Y + ((rp2.Y - oldr.Y) * scaleFactorY);
      Point rp3 = new Point(newX, newY);
      if (angle != 0)
        rp3 = RotatePoint(rp3, new Point(newr.X + newr.Width / 2, newr.Y + newr.Height / 2), cosine, sine);
      return rp3;
    }

    public static Rect GetRotatedBounds(Rect r, double angle) {
      if (angle == 0)
        return r;

      double rad = angle * Math.PI / 180;
      double cosine = Math.Cos(rad);
      double sine = Math.Sin(rad);
      Point center = new Point(r.X + r.Width / 2, r.Y + r.Height / 2);

      Point otl = new Point(r.X, r.Y);
      Point ntl = RotatePoint(otl, center, cosine, sine);

      Point otr = new Point(r.X + r.Width, r.Y);
      Point ntr = RotatePoint(otr, center, cosine, sine);

      Point obr = new Point(r.X + r.Width, r.Y + r.Height);
      Point nbr = RotatePoint(obr, center, cosine, sine);

      Point obl = new Point(r.X, r.Y + r.Height);
      Point nbl = RotatePoint(obl, center, cosine, sine);

      double minx = Math.Min(ntl.X, Math.Min(ntr.X, Math.Min(nbr.X, nbl.X)));
      double maxx = Math.Max(ntl.X, Math.Max(ntr.X, Math.Max(nbr.X, nbl.X)));
      double miny = Math.Min(ntl.Y, Math.Min(ntr.Y, Math.Min(nbr.Y, nbl.Y)));
      double maxy = Math.Max(ntl.Y, Math.Max(ntr.Y, Math.Max(nbr.Y, nbl.Y)));

      return new Rect(minx, miny, maxx - minx, maxy - miny);
    }

    /// <summary>
    /// Returns the closest point to <paramref name="source"/> in <paramref name="points"/>.
    /// </summary>
    /// <param name="points">The list of points checking for distance to source.</param>
    /// <param name="source">The point we want to be closest to.</param>
    /// <returns>The closest point to <paramref name="source"/>.</returns>
    public static Point FindClosest(List<Point> points, Point source) {
      Point closest = points[0];
      for (int i = 1; i < points.Count; i++) {
        Point p1 = points[i];
        Point p2 = closest;
        if (((source.X - p1.X) * (source.X - p1.X) + (source.Y - p1.Y) * (source.Y - p1.Y)) < ((source.X - p2.X) * (source.X - p2.X) + (source.Y - p2.Y) * (source.Y - p2.Y)))
          closest = p1;
      }
      return closest;
    }

    /// <summary>
    /// Returns true if line(line1, line2) intersects with line segment(seg1, seg2) and sets [intersection] to the point
    /// </summary>
    /// <param name="line1"></param>
    /// <param name="line2"></param>
    /// <param name="seg1"></param>
    /// <param name="seg2"></param>
    /// <param name="intersection"></param>
    /// <returns></returns>
    public static bool LineIntersectsWithSegment(Point line1, Point line2, Point seg1, Point seg2, out List<Point> intersection) {
      intersection = new List<Point>();
      Point p;
      if (Geo.NearestIntersectionOnLine(seg1, seg2, line1, line2, out p)) {
        intersection.Add(p);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Gets the positive angle with repect to center's x axis of [point]
    /// </summary>
    /// <param name="point"></param>
    /// <param name="center"></param>
    /// <returns></returns>
    private static double GetAngleFromX(Point point, Point center) {
      double x = Math.Abs(point.X - center.X);
      double y = Math.Abs(point.Y - center.Y);
      // Check for common cases first:
      if (y == 0) { // If we're on the X axis
        if (point.X < center.X)
          return Math.PI;
        else
          return 0;
      }
      if (x == 0) { // If we're on the Y axis
        if (point.Y < center.Y)
          return Math.PI / 2;
        else
          return 3 * Math.PI / 2;
      }

      // Find quadrant:
      double angle = 0;
      if (point.X >= center.X && point.Y <= center.Y)   // 1st Quad
        angle = Math.Atan(y / x);
      else if (point.X < center.X && point.Y < center.Y)   // 2nd Quad
        angle = Math.Atan(x / y) + Math.PI / 2;
      else if (point.X < center.X && point.Y > center.Y)   // 3rd Quad
        angle = Math.Atan(y / x) + Math.PI;
      else if (point.X > center.X && point.Y > center.Y)  // 4th Quad      
        angle = Math.Atan(x / y) + 3 * Math.PI / 2;
      return angle;
    }

    /// <summary>
    /// Rotates the point [p] about (0,0)
    /// </summary>
    /// <param name="p">The point to be rotated</param>
    /// <param name="angle">The angle to rotate the point in degrees (Counter-Clockwise is positive)</param>
    /// <returns></returns>
    public static Point RotatePoint(Point p, double angle) {
      if (angle == 0 || (p.X == 0 && p.Y == 0))
        return p;
      double rad = angle * Math.PI / 180;
      double cosine = Math.Cos(rad);
      double sine = Math.Sin(rad);
      return new Point((cosine * p.X - sine * p.Y),
                       (sine * p.X + cosine * p.Y));
    }

    /// <summary>
    /// Scales the given point by scaleX, scaleY
    /// </summary>
    /// <param name="pt">The point to be scaled</param>
    /// <param name="scaleX">The amount to scale the point in the x direction</param>
    /// <param name="scaleY">The amount to scale the point in the y direction</param>
    /// <returns>The scaled point</returns>
    private static Point ScalePoint(Point pt, double scaleX, double scaleY) {
      return new Point(pt.X * scaleX, pt.Y * scaleY);
    }

    /// <summary>
    /// Translates the point by transX, transY
    /// </summary>
    /// <param name="pt">The point to be translated</param>
    /// <param name="transX">The amount to translate in the x direction</param>
    /// <param name="transY">The amount to translate in the y direction</param>
    /// <returns>The translated point</returns>
    public static Point TranslatePoint(Point pt, double transX, double transY) {
      return new Point(pt.X + transX, pt.Y + transY);
    }

    /// <summary>
    /// Translates a collection of points by transX, transY
    /// </summary>
    /// <param name="pts">The collection to be translated</param>
    /// <param name="transX">The amount to translate in the x direction</param>
    /// <param name="transY">The amount to translate in the y direction</param>
    /// <returns>The translated collection</returns>
    public static IEnumerable<Point> TranslatePoints(IEnumerable<Point> pts, double transX, double transY) {
      List<Point> translatedPoints = new List<Point>();
      foreach (Point pt in pts) {
        translatedPoints.Add(TranslatePoint(pt, transX, transY));
      }
      return translatedPoints;
    }

    /// <summary>
    /// Returns all intersections between infinite line [line1-line2] and [ellipse] and returns them
    /// </summary>
    /// <param name="line1">First point on the infinite line</param>
    /// <param name="line2">Second point on the infinite line</param>
    /// <param name="ellipse">The ellipse we are checking for intersections with</param>
    /// <returns>A list of intersection points</returns>
    public static List<Point> GetLineEllipseIntersections(Point line1, Point line2, EllipseGeometry ellipse) {
      List<Point> result = new List<Point>();

      // Now, check for the actual intersections
      Point center = ellipse.Center;
      double ax = ellipse.RadiusX;
      double by = ellipse.RadiusY;
      if (ax == 0 || by == 0) return result; // No space is occupied if radii are 0
      Point int1 = new Point();
      Point int2 = new Point();
      if (Math.Abs(line2.X - line1.X) < 0.5) { // If it's a vertical line
        double disc = 1 - (line2.X - center.X) * (line2.X - center.X) / (ax * ax);
        if (disc < 0) return result;
        double y1 = by * Math.Sqrt(disc) + center.Y;
        double y2 = -1 * by * Math.Sqrt(disc) + center.Y;
        int1 = new Point(line2.X, y1);
        int2 = new Point(line2.X, y2);

        result.Add(int1);
        result.Add(int2);
      }
      else // Else we need to quadratic formula
      {
        double slope = (line2.Y - line1.Y) / (line2.X - line1.X);
        double a = 1 / (ax * ax) + (slope * slope) / (by * by);
        double b = 2 * slope * (line1.Y - slope * line1.X) / (by * by) - 2 * slope * center.Y / (by * by) - 2 * center.X / (ax * ax);
        double c = 2 * slope * line1.X * center.Y / (by * by) - 2 * line1.Y * center.Y / (by * by)
          + center.Y * center.Y / (by * by) + center.X * center.X / (ax * ax) - 1 + (line1.Y - slope * line1.X)
          * (line1.Y - slope * line1.X) / (by * by);

        // Quadratic Formula to find intersections:
        double discriminant = b * b - 4 * a * c;
        if (discriminant < 0)
          return result; // No intersections
        double intX1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
        int1 = new Point(intX1, slope * intX1 - slope * line1.X + line1.Y);
        double intX2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
        int2 = new Point(intX2, slope * intX2 - slope * line1.X + line1.Y);

        result.Add(int1);
        result.Add(int2);
      }

      return result;
    }

    /// <summary>
    /// Returns all intersections between infinite line [line1-line2] and the ArcSegment and puts them in [result]
    /// </summary>
    /// <param name="line1">First point on the infinite line</param>
    /// <param name="line2">Second point on the infinite line</param>
    /// <param name="ellipseStartPoint">The start point of the arc segment</param>
    /// <param name="arcSeg">The arc segment to find intersections with</param>
    /// <param name="intersection">The list to return the intersections with</param>
    /// <returns>Returns true if their are any intersections; false otherwise</returns>
    public static bool GetLineArcSegmentIntersections(Point line1, Point line2, Point ellipseStartPoint, ArcSegment arcSeg, out List<Point> intersection) {
      if (arcSeg.Size.Width == 0 || arcSeg.Size.Height == 0)
        return LineIntersectsWithSegment(line1, line2, ellipseStartPoint, arcSeg.Point, out intersection);//LineIntersectsWithSegment(line1, line2, ellipseStartPoint, arcSeg.Point, out intersection);
      
      intersection = new List<Point>();
      Point center;
      Point startPoint;
      Point endPoint;
      Size size = arcSeg.Size;

      // Now set up the startPoint and endPoint
      double half = (new Vector(arcSeg.Point.X - ellipseStartPoint.X, arcSeg.Point.Y - ellipseStartPoint.Y)).Length;

      startPoint = RotatePoint(ellipseStartPoint, -1 * arcSeg.RotationAngle);
      startPoint = ScalePoint(startPoint, (arcSeg.Size.Height) / (arcSeg.Size.Width), 1);
      endPoint = RotatePoint(arcSeg.Point, -1 * arcSeg.RotationAngle);
      endPoint = ScalePoint(endPoint, (arcSeg.Size.Height) / (arcSeg.Size.Width), 1);

      Vector unitVec = new Vector(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
      if (unitVec.X == 0 && unitVec.Y == 0) return false;
      double halfDistance = unitVec.Length / 2;
      unitVec.Normalize(); // Now make it a unit vector

      // What arc do we use?
      bool rotateCCW = (arcSeg.SweepDirection == SweepDirection.Counterclockwise) == arcSeg.IsLargeArc;
      if (rotateCCW) // Rotate it
        unitVec = new Vector(-1 * unitVec.Y, unitVec.X);
      else
        unitVec = new Vector(unitVec.Y, -1 * unitVec.X);

      Point mid = new Point((endPoint.X + startPoint.X) / 2, (endPoint.Y + startPoint.Y) / 2);
      double distToCent = Math.Sqrt(arcSeg.Size.Height * arcSeg.Size.Height - halfDistance * halfDistance);
      if (double.IsNaN(distToCent)) {
        endPoint = arcSeg.Point;
        startPoint = ellipseStartPoint;
        endPoint = RotatePoint(endPoint, -arcSeg.RotationAngle);
        startPoint = RotatePoint(startPoint, -arcSeg.RotationAngle);
        center = new Point((endPoint.X + startPoint.X) / 2, (endPoint.Y + startPoint.Y) / 2);

        double ratio = arcSeg.Size.Width / arcSeg.Size.Height;
        size.Height = Math.Sqrt((startPoint.X - center.X) * (startPoint.X - center.X) / (ratio * ratio) + (startPoint.Y - center.Y) * (startPoint.Y - center.Y));
        size.Width = size.Height * ratio;
        center = RotatePoint(center, arcSeg.RotationAngle);
      }
      else // The adjusted center of the ellipse
        {
        center = mid + distToCent * unitVec;

        center = ScalePoint(center, arcSeg.Size.Width / arcSeg.Size.Height, 1);
        center = RotatePoint(center, arcSeg.RotationAngle);
      }

      center = RotatePoint(center, -1 * arcSeg.RotationAngle);
      startPoint = RotatePoint(line1, -1 * arcSeg.RotationAngle);
      endPoint = RotatePoint(line2, -1 * arcSeg.RotationAngle);

      // Now, check for the actual intersections
      double ax = size.Width;
      double by = size.Height;
      Point int1 = new Point();
      Point int2 = new Point();
      if (Math.Abs(endPoint.X - startPoint.X) < 0.5) { // If it's a vertical line
        if (endPoint.Y - startPoint.Y == 0) return false;
        double disc = 1 - (endPoint.X - center.X) * (endPoint.X - center.X) / (ax * ax);
        if (disc < 0) return false;
        double y1 = by * Math.Sqrt(disc) + center.Y;
        double y2 = -1 * by * Math.Sqrt(disc) + center.Y;
        int1 = new Point(endPoint.X, y1);
        int2 = new Point(endPoint.X, y2);
      }
      else // Else we need to quadratic formula
      {
        double slope = (endPoint.Y - startPoint.Y) / (endPoint.X - startPoint.X);
        double a = 1 / (ax * ax) + (slope * slope) / (by * by);
        double b = 2 * slope * (startPoint.Y - slope * startPoint.X) / (by * by) - 2 * slope * center.Y / (by * by) - 2 * center.X / (ax * ax);
        double c = 2 * slope * startPoint.X * center.Y / (by * by) - 2 * startPoint.Y * center.Y / (by * by) + center.Y * center.Y / (by * by) + center.X * center.X / (ax * ax) - 1 + (startPoint.Y - slope * startPoint.X) * (startPoint.Y - slope * startPoint.X) / (by * by);

        // Quadratic Formula to find intersections:
        double discriminant = b * b - 4 * a * c;
        if (discriminant < 0 || a == 0)
          return false; // No intersections
        double intX1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
        int1 = new Point(intX1, slope * intX1 - slope * startPoint.X + startPoint.Y);
        double intX2 = (-b - Math.Sqrt(discriminant)) / (2 * a);
        int2 = new Point(intX2, slope * intX2 - slope * startPoint.X + startPoint.Y);
      }

      // Tranform the center back to where it was:
      center = RotatePoint(center, arcSeg.RotationAngle);
      int1 = RotatePoint(int1, arcSeg.RotationAngle);
      int2 = RotatePoint(int2, arcSeg.RotationAngle);

      // Finally, just check to make sure these intersection points are actually on the curve:
      double sAngle = GetAngleFromX(ellipseStartPoint, center);
      double eAngle = GetAngleFromX(arcSeg.Point, center);
      double a1 = GetAngleFromX(int1, center);
      bool a1Valid = false;
      double a2 = GetAngleFromX(int2, center);
      bool a2Valid = false;
      bool isCCW = arcSeg.SweepDirection == SweepDirection.Counterclockwise;

      if (sAngle > eAngle) {
        // For int1
        if (a1 < sAngle && a1 > eAngle)
          a1Valid = !isCCW; // It's in if clockwise, not in otherwise
        else
          a1Valid = isCCW;
        // For int2
        if (a2 < sAngle && a2 > eAngle)
          a2Valid = !isCCW;
        else
          a2Valid = isCCW;
      }
      else {
        // For int1
        if (a1 > sAngle && a1 < eAngle)
          a1Valid = isCCW;
        else
          a1Valid = !isCCW;
        // For int2
        if (a2 > sAngle && a2 < eAngle)
          a2Valid = isCCW;
        else
          a2Valid = !isCCW;
      }

      if (a1Valid)
        intersection.Add(int1);
      if (a2Valid)
        intersection.Add(int2);
      if (intersection.Count > 0)
        return true;
      return false;
    }

    /// <summary>
    /// Returns all intersections between infinite line [l1-l2] and the QuadBezier and puts them in [result]
    /// </summary>
    /// <param name="s">Start point of the bezier curve</param>
    /// <param name="c1">First control point of the bezier curve</param>
    /// <param name="e">End point of the bezier curve</param>
    /// <param name="l1">First point of the infinite line</param>
    /// <param name="l2">Second point of the infinite line</param>
    /// <param name="epsilon">Accuracy tolerance</param>
    /// <param name="result">List of the intersections</param>
    /// <returns>True if intersections are found; false otherwise</returns>
    public static bool GetQuadBezierIntersectionsOnLine(Point s, Point c1, Point e, Point l1, Point l2, double epsilon, out List<Point> result) {
      // Elevate the quad and use the cubic alg. to find
      Point p0 = s;
      Point p1 = new Point(p0.X / 3 + 2 * c1.X / 3, p0.Y / 3 + 2 * c1.Y / 3);
      Point p2 = new Point(2 * c1.X / 3 + e.X / 3, 2 * c1.Y / 3 + e.Y / 3);
      Point p3 = e;
      Point point;
      bool res = Geo.BezierNearestIntersectionOnLine(p0, p1, p2, p3, l1, l2, epsilon, out point);
      result = new List<Point>();
      result.Add(point);
      return res;
    }

    /// <summary>
    /// Returns all intersections of infinite line [line1-line2] and the polyline defined by [start] and [points]
    /// </summary>
    /// <param name="line1">First point of the infinite line</param>
    /// <param name="line2">Second point of the infinite line</param>
    /// <param name="start">First point of the polyline</param>
    /// <param name="points">List of line segments</param>
    /// <param name="result">The list of intersections</param>
    /// <returns>True if intersections are found; false otherwise</returns>
    public static bool GetPolyLineSegIntersections(Point line1, Point line2, Point start, PointCollection points, out List<Point> result) {
      Point lastPoint = start;
      List<Point> next;
      result = new List<Point>();
      foreach (Point p in points) {
        if (LineIntersectsWithSegment(line1, line2, lastPoint, p, out next)) {
          result.Add(next[0]);
          next.Clear();
        }
        lastPoint = p;
      }
      return result.Count > 0;
    }

    /// <summary>
    /// Returns all intersections between infinite line [line1-line2] and the PolyBezier and puts them in [result]
    /// </summary>
    /// <param name="line1">First point of the infinite line</param>
    /// <param name="line2">Second point of the infinite line</param>
    /// <param name="start">First point of the PolyBezier</param>
    /// <param name="points">List of points in the PolyBezier</param>
    /// <param name="epsilon">Accuracy tolerance</param>
    /// <param name="result">The list of intersections</param>
    /// <returns>True if intersections are found; false otherwise</returns>
    public static bool GetPolyBezierIntersections(Point line1, Point line2, Point start, PointCollection points, double epsilon, out List<Point> result) {
      Point s = start;
      Point c1 = new Point();
      Point c2 = new Point();
      Point e;
      Point next;
      result = new List<Point>();
      int count = 1;
      foreach (Point p in points) {
        if (count == 1) {
          c1 = p;
          count++;
        }
        else if (count == 2) {
          c2 = p;
          count++;
        }
        else {
          e = p;
          if (Geo.BezierNearestIntersectionOnLine(s, c1, c2, e, line1, line2, epsilon, out next)) {
            result.Add(next);
          }
          count = 1;
        }
      }

      return result.Count > 0;
    }

    /// <summary>
    /// Returns all intersections between infinite line [line1-line2] and the PolyBezier and puts them in [result]
    /// </summary>
    /// <param name="line1">First point of the infinite line</param>
    /// <param name="line2">Second point of the infinite line</param>
    /// <param name="start">First point of the PolyQuadBezier</param>
    /// <param name="points">List of points in the PolyQuadBezier</param>
    /// <param name="epsilon">Accuracy tolerance</param>
    /// <param name="result">The list of intersections</param>
    /// <returns>True if intersections are found; false otherwise</returns>
    public static bool GetPolyQuadBezierIntersection(Point line1, Point line2, Point start, PointCollection points, double epsilon, out List<Point> result) {
      Point s = start;
      Point c1 = new Point();
      Point e;
      List<Point> next;
      result = new List<Point>();
      bool count = true;
      foreach (Point p in points) {
        if (count) {
          c1 = p;
          count = !count;
        }
        else {
          e = p;
          if (GetQuadBezierIntersectionsOnLine(s, c1, e, line1, line2, epsilon, out next)) {
            result.Add(next[0]);
          }
          count = !count;
        }
      }

      return result.Count > 0;
    }

    /// <summary>
    /// Returns true if [point] is within [fuzz] pixels of the infinite line passing through [p1] and [p2]
    /// </summary>
    /// <param name="p1">Point 1 on the infinite line</param>
    /// <param name="p2">Point 2 on the infinite line</param>
    /// <param name="point">The point we're checking to be on the line</param>
    /// <param name="fuzz">The number of pixels away [point] can be from the line</param>
    /// <returns>True if [point] is on or within [fuzz] pixels of the line, false otherwise</returns>
    public static bool InfiniteLineContainsPoint(Point p1, Point p2, Point point, double fuzz) {
      if (p1 == p2) return false;
      if (p2.X - p1.X == 0) { // Vertical Line
        if (point.X <= p2.X + fuzz && point.X >= p2.X - fuzz)
          return true;
        return false;
      }
      if (p2.Y - p1.Y == 0) { // Horizontal Line
        if (point.Y <= p2.Y + fuzz && point.Y >= p2.Y - fuzz)
          return true;
        return false;
      }

      // We know they intersect, so no need to check for zeroes
      double m = (p2.Y - p1.Y) / (p2.X - p1.X);
      double m2 = -1 / m;
      double intx = (m2 * point.X - m * p1.X + p1.Y - point.Y) / (m2 - m);
      Point point2 = new Point(intx, m * (intx - p1.X) + p1.Y);

      if ((point2.X - point.X) * (point2.X - point.X) + (point2.Y - point.Y) * (point2.Y - point.Y) <= fuzz * fuzz)
        return true;
      return false;
    }

    // This is needed in Silverlight because of a Silverlight bug
    // that causes a Path with a PathGeometry that was created using
    // <Path ... Data="M0 0 L10 20 ..." />
    // to have ZERO Bounds after trying to access the PathGeometry.Figures property.
    //
    // In both WPF and Silverlight Paths constructed using the Path markup syntax
    // using the StreamGeometry mini-language are supposed to be "black boxes".
    // There is no way to walk the PathGeometry.Figures to determine its actual geometry.
    // One can only get the PathGeometry.Bounds.
    // We have to treat it as a rectangular shape.
    // Note: Silverlight does not support the PathFigureCollection mini-language.
    // (Example: <PathGeometry Figures="M0 0 L10 20 ..." />.)
    // WPF supports both the StreamGeometry mini-language and the PathFigureCollection mini-language.
    //
    // So if the programmer wants to use a Node containing Paths for which Links want to
    // connect at the edge of the Paths (when Spot.None, of course),
    // they have to construct a PathFigureCollection containing PathFigures.
    //
    // But in Silverlight there is a bug causing the PathGeometry to have zero Bounds
    // once our code tries to get the value of PathGeometry.Figures,
    // if the Path was defined in XAML.
    // This causes that Path to disappear, to the consternation of our customers.
    // Typically that happenened (before version 1.2.6f) when routing a link to a node
    // containing a Path defined in XAML, when the Spot was None and no surrounding
    // Panel with a Background hid the actual shape of the Path.
    //
    // Our work-around requires our code to construct each PathGeometry with an
    // explicitly created PathFigureCollection containing PathFigures.
    // Explicitly setting PathGeometry.Figures gives it a local value which
    // tells this predicate that it's OK to try to get the PathGeometry.Figures property.
    // If it hasn't been set, it won't have a local value, which means
    // (most likely) that it was created in XAML using Data="...".
    public static bool PathGeometryHasFigures(PathGeometry path) {
      object val = path.ReadLocalValue(PathGeometry.FiguresProperty);
      var pfc = val as PathFigureCollection;
      return pfc != null && pfc.Count > 0;
    }

    /// <summary>
    /// Returns a point on [path] that is the closest intersection of the path and the line formed by p1/p2
    /// </summary>
    /// <param name="path">The Path Geometry to find intersections with</param>
    /// <param name="p1">First point on the infinite line</param>
    /// <param name="p2">Second point on the infinite line</param>
    /// <param name="result">The list of intersection points</param>
    /// <param name="mustBeFilled">True if each figure must be filled to be included</param>
    /// <returns>True if intersections are found; false otherwise</returns>
    public static bool GetIntersectionsOnPathGeometry(PathGeometry path, Point p1, Point p2, out List<Point> result, bool mustBeFilled) {
      result = new List<Point>();
      List<Point> next = new List<Point>();

      if (!PathGeometryHasFigures(path)) {
        Rect bounds = path.Bounds;
        return GetIntersectionsOnRect(bounds, p1, p2, out result);
      }

      foreach (PathFigure fig in path.Figures) {
        if (mustBeFilled && !fig.IsFilled) continue;
        Point start = fig.StartPoint;

        // Snap to points very close to the line
        if (InfiniteLineContainsPoint(p1, p2, start,1)) result.Add(start);

        foreach (PathSegment seg in fig.Segments) {
          // Find an intersection with this segment:
          // Options:
          //    ArcSegment -- LineIntersectsWithArcSegment
          //    LineSegment -- LineIntersectsWithSegment
          //    BezierSegment -- Geo.BezierNearestIntersectionOnLine
          //    QuadraticBezierSegment -- NearestQuadBezierIntersectionOnLine
          //    PolyLineSegment -- NearestPolyLineSegIntersection
          //    PolyBezierSegment -- NearestPolyBezierIntersection
          //    PolyQuadraticBezierSegment -- NearestPolyQuadBezierIntersection
          Point tempstart = start;
          if (seg is LineSegment) {
            LineSegment ls = seg as LineSegment;
            start = ls.Point;
            if (!LineIntersectsWithSegment(p1, p2, tempstart, ls.Point, out next))
              continue;   // Keep going if no intersection was found         
          }
          else if (seg is ArcSegment) {
            ArcSegment aseg = seg as ArcSegment;
            start = aseg.Point;
            if (!GetLineArcSegmentIntersections(p1, p2, tempstart, aseg, out next))
              continue;
          }
          else if (seg is BezierSegment) {
            BezierSegment bs = seg as BezierSegment;
            start = bs.Point3;
            if (!GetIntersectionsOnBezier(p1, p2, tempstart, bs.Point1, bs.Point2, bs.Point3, out next))
              continue;
          }
          else if (seg is QuadraticBezierSegment) {
            QuadraticBezierSegment qbs = seg as QuadraticBezierSegment;
            start = qbs.Point2;
            if (!GetQuadBezierIntersectionsOnLine(tempstart, qbs.Point1, qbs.Point2, p1, p2, .05, out next))
              continue;
          }
          else if (seg is PolyLineSegment) {
            PolyLineSegment pls = seg as PolyLineSegment;
            for (int i = 1; i < pls.Points.Count - 1; i++)
              if (InfiniteLineContainsPoint(p1, p2, pls.Points[i], 1)) result.Add(pls.Points[i]);
            start = pls.Points[pls.Points.Count - 1];
            if (!GetPolyLineSegIntersections(p1, p2, tempstart, pls.Points, out next))
              continue;
          }
          else if (seg is PolyBezierSegment) {
            PolyBezierSegment pbs = seg as PolyBezierSegment;
            start = pbs.Points[pbs.Points.Count - 1];
            if (!GetPolyBezierIntersections(p1, p2, tempstart, pbs.Points, .05, out next))
              continue;
          }
          else { // seg is PolyQuadraticBezierSegment
            PolyQuadraticBezierSegment pqbs = seg as PolyQuadraticBezierSegment;
            start = pqbs.Points[pqbs.Points.Count - 1];
            if (!GetPolyQuadBezierIntersection(p1, p2, tempstart, pqbs.Points, .05, out next))
              continue;
          }

          // Snap to points very close to the line
          if (InfiniteLineContainsPoint(p1, p2, start, 1)) result.Add(start);

          // If it gets here, next should be set to the intersections of seg (not null)
          foreach (Point p in next)
            result.Add(p);
        }

        if (fig.IsFilled || fig.IsClosed) {
          List<Point> temp;
          if (LineIntersectsWithSegment(p1, p2, start, fig.StartPoint, out temp))
            result.Add(temp[0]);     
        }
      }
      return result.Count > 0;
    }

    /// <summary>
    /// Returns all intersections between infinite line [p1-p2] and [geom] and puts them in [result]
    /// </summary>
    /// <param name="geom">The geometry to find intersections with</param>
    /// <param name="p1">First point on the infinite line</param>
    /// <param name="p2">Second point on the infinite line</param>
    /// <param name="result">The list of intersections</param>
    /// <returns>True if intersections are found; false otherwise</returns>
    public static bool GetIntersectionsOnGeometry(Geometry geom, Point p1, Point p2, out List<Point> result) {
      return GetIntersectionsOnGeometry(geom, p1, p2, out result, false);
    }

    /// <summary>
    /// Returns all intersections between infinite line [p1-p2] and [geom] and puts them in [result]
    /// </summary>
    /// <param name="geom">The geometry to find intersections with</param>
    /// <param name="p1">First point on the infinite line</param>
    /// <param name="p2">Second point on the infinite line</param>
    /// <param name="result">The list of intersections</param>
    /// <param name="mustBeFilled">True if each figure must be filled to be included</param>
    /// <returns>True if intersections are found; false otherwise</returns>
    public static bool GetIntersectionsOnGeometry(Geometry geom, Point p1, Point p2, out List<Point> result, bool mustBeFilled) {
      result = new List<Point>();
      if (geom == null) return false;
      // Apply transforms to the points:      
      if (geom.Transform != null) {
        Matrix mat;
        TransformGroup tg = new TransformGroup();
        tg.Children = new TransformCollection();
        tg.Children.Add(geom.Transform);
        mat = tg.Value;
        Geo.Invert(ref mat);
        p1 = mat.Transform(p1);
        p2 = mat.Transform(p2);
      }

      // What kind of geometry?
      if (geom is LineGeometry) {
        LineGeometry linegeom = geom as LineGeometry;
        LineIntersectsWithSegment(p1, p2, linegeom.StartPoint, linegeom.EndPoint, out result);
      }
      else if (geom is RectangleGeometry) {
        RectangleGeometry rectgeom = geom as RectangleGeometry;
        GetIntersectionsOnRect(rectgeom.Rect, p1, p2, out result);
      }
      else if (geom is EllipseGeometry) {
        EllipseGeometry eg = geom as EllipseGeometry;
        result = GetLineEllipseIntersections(p1, p2, eg);
      }
      else if (geom is PathGeometry) {
        PathGeometry pg = geom as PathGeometry;
        GetIntersectionsOnPathGeometry(pg, p1, p2, out result, mustBeFilled);
      }





















      else { // geom is GroupGeometry, recursively call this method
        // Compute the first one manually
        List<Point> next;
        foreach (Geometry geo in ((GeometryGroup)geom).Children) {
          if (!GetIntersectionsOnGeometry(geo, p1, p2, out next, mustBeFilled))
            continue; // Keep going if we didn't find anything
          foreach (Point p in next)
            result.Add(p);
        }
      }

      if (geom.Transform != null) {
        TransformGroup tg = new TransformGroup();
        tg.Children = new TransformCollection();
        tg.Children.Add(geom.Transform);
        for (int i = 0; i < result.Count; i++)
          result[i] = tg.Value.Transform(result[i]);
      }
      return result.Count > 0;
    }

    /// <summary>
    /// Returns all intersections between infinite line [p1-p2] and [rect] and puts them in [result]
    /// </summary>
    /// <param name="rect">The Rect to check for intersections</param>
    /// <param name="p1">First point on the infinite line</param>
    /// <param name="p2">Second point on the infinite line</param>
    /// <param name="result">The list of intersections</param>
    /// <returns>True if intersections are found; false otherwise</returns>
    public static bool GetIntersectionsOnRect(Rect rect, Point p1, Point p2, out List<Point> result) {
      result = new List<Point>();
      Point temp;
      Point s1 = new Point(rect.Left, rect.Top);
      Point s2 = new Point(rect.Right, rect.Top);
      Point s3 = new Point(rect.Right, rect.Bottom);
      Point s4 = new Point(rect.Left, rect.Bottom);
      // Get intersections with individual segments on the rect
      if (Geo.NearestIntersectionOnLine(s1, s2, p1, p2, out temp))
        result.Add(temp);
      if (Geo.NearestIntersectionOnLine(s2, s3, p1, p2, out temp) && !result.Contains(temp))
        result.Add(temp);
      if (Geo.NearestIntersectionOnLine(s3, s4, p1, p2, out temp) && !result.Contains(temp))
        result.Add(temp);
      if (Geo.NearestIntersectionOnLine(s4, s1, p1, p2, out temp) && !result.Contains(temp))
        result.Add(temp);

      return result.Count > 0;
    }

    private const double ONETHIRD = 0.333333333333333;
    private const double SQRTTHREE = 1.73205080756888;

    /// <summary>
    /// Gets the cubed root of [x]
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private static double GetCubedRoot(double x) {
      if (x < 0)
        return -Math.Pow(-x, ONETHIRD);
      else
        return Math.Pow(x, ONETHIRD);
    }

    /// <summary>
    /// Returns a list of the 3 roots of a cubic equation of the form at^3 + bt^2 + ct + d
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <returns>A list of the roots</returns>
    private static List<double> GetRootsOfCubic(double a, double b, double c, double d) {
      List<double> roots = new List<double>();

      // Use Cardano's method to find the roots
      double f, g, h;

      f = (3 * c / a - Math.Pow(b, 2) / Math.Pow(a, 2)) / 3;
      g = (2 * Math.Pow(b, 3) / Math.Pow(a, 3) - 9 * b * c / Math.Pow(a, 2) + 27 * d / a) / 27;
      h = Math.Pow(g, 2) / 4 + Math.Pow(f, 3) / 27;

      if (f == 0 && g == 0 && h == 0) // 3 equal roots
      {
        // Just take cubed root for this case
        roots.Add(-GetCubedRoot(d / a));
      }
      else if (h <= 0) // 3 real roots
      {
        double i, j, k, m, n, p;

        i = Math.Pow(Math.Pow(g, 2) / 4 - h, 0.5);
        j = GetCubedRoot(i);
        k = Math.Acos(-(g / (2 * i)));
        m = Math.Cos(k / 3);
        n = SQRTTHREE * Math.Sin(k / 3);

        p = -(b / (3 * a));

        // Add the solutions:
        roots.Add(2 * j * m + p);
        roots.Add(-j * (m + n) + p);
        roots.Add(-j * (m - n) + p);
      }
      else if (h > 0) // 1 real root and 2 complex roots
      {
        double r, s, t, u, p;

        r = -(g / 2) + Math.Pow(h, 0.5);
        s = GetCubedRoot(r);
        t = -(g / 2) - Math.Pow(h, 0.5);
        u = GetCubedRoot(t);
        p = -(b / (3 * a));

        // Add the only root
        roots.Add((s + u) + p);
      }

      return roots;
    }

    public static List<double> GetRootsOfQuadratic(double a, double b, double c) {
      double dd = Math.Sqrt(b * b - 4 * a * c);
      List<double> result = new List<double>();
      result.Add((-b + dd) / (2 * a));
      result.Add((-b - dd) / (2 * a));
      return result;
    }

    /// <summary>
    /// Calculates all intersections between infinite line [p1,p2] and the cubic bezier curve [s,c1,c2,e]
    /// </summary>
    /// <param name="p1">First point on the infinite line</param>
    /// <param name="p2">Second point on the infinite line</param>
    /// <param name="s">The start point of the Bezier Curve</param>
    /// <param name="c1">The first control point of the Bezier Curve</param>
    /// <param name="c2">The second control point of the Bezier Curve</param>
    /// <param name="e">The end of the Bezier Curve</param>
    /// <param name="intersections">The list of intersections</param>
    /// <returns>True if there are any intersections; false otherwise</returns>
    public static bool GetIntersectionsOnBezier(Point p2, Point p1, Point s, Point c1, Point c2, Point e, out List<Point> intersections) {
      // Make sure p1 is on the left and switch it if it's not
      if (p1.X > p2.X) {
        Point temp = p1;
        p1 = p2;
        p2 = temp;
      }

      // First find out transformations required to make line [p1,p2] lie on the x axis
      double offX = p1.X;
      double offY = p1.Y;
      double angle = 0; // degrees
      if (p2.X - offX == 0) {
        double f = e.X - s.X + 3 * c1.X - 3 * c2.X;
        double g = 3 * s.X - 6 * c1.X + 3 * c2.X;
        double h = 3 * c1.X - 3 * s.X;
        double i = s.X - p1.X;
        List<double> vertRoots;
        if (f != 0)
          vertRoots = GetRootsOfCubic(f, g, h, i);
        else
          vertRoots = GetRootsOfQuadratic(g, h, i);
        
        intersections = new List<Point>();
        foreach (double t in vertRoots) {
          Point intercept = new Point((1 - t) * (1 - t) * (1 - t) * s.X + 3 * (1 - t) * (1 - t) * t * c1.X + 3 * (1 - t) * t * t * c2.X + t * t * t * e.X, 
            (1 - t) * (1 - t) * (1 - t) * s.Y + 3 * (1 - t) * (1 - t) * t * c1.Y + 3 * (1 - t) * t * t * c2.Y + t * t * t * e.Y);
          if (double.IsNaN(intercept.X) || double.IsNaN(intercept.Y) || t < 0 || t > 1) continue; // Make sure the point is actually on the bezier
          intersections.Add(intercept);
        }
        return intersections.Count > 0;
      }
      else
        angle = 180 * Math.Atan((p2.Y - offY) / (p2.X - offX)) / Math.PI; // guarenteed to be between -90 and 90 since p2 is to the right of p1

      // Now transform all of the points by the opposite, translation first
      p1 = TranslatePoint(p1, -offX, -offY);
      p2 = TranslatePoint(p2, -offX, -offY);
      s = TranslatePoint(s, -offX, -offY);
      c1 = TranslatePoint(c1, -offX, -offY);
      c2 = TranslatePoint(c2, -offX, -offY);
      e = TranslatePoint(e, -offX, -offY);
      // p1 is at 0,0, no need to rotate
      p2 = RotatePoint(p2, -angle);
      s = RotatePoint(s, -angle);
      c1 = RotatePoint(c1, -angle);
      c2 = RotatePoint(c2, -angle);
      e = RotatePoint(e, -angle);

      // Now we calculate intersections by finding the zeroes of the equation at^3 + bt^2 + ct + d = 0
      intersections = new List<Point>();
      double a = -s.Y + 3 * c1.Y - 3 * c2.Y + e.Y;
      double b = 3 * s.Y - 6 * c1.Y + 3 * c2.Y;
      double c = -3 * s.Y + 3 * c1.Y;
      double d = s.Y;

      List<double> roots = GetRootsOfCubic(a, b, c, d);
      intersections = new List<Point>();

      // Now find x for each of the t's:
      foreach (double t in roots) {
        Point intercept = new Point((1 - t) * (1 - t) * (1 - t) * s.X + 3 * (1 - t) * (1 - t) * t * c1.X + 3 * (1 - t) * t * t * c2.X + t * t * t * e.X, 0);
        intercept = RotatePoint(intercept, angle);
        intercept = TranslatePoint(intercept, offX, offY);
        if (double.IsNaN(intercept.X) || double.IsNaN(intercept.Y) || t<0 || t>1) continue; // Make sure the point is actually on the bezier
        intersections.Add(intercept);
      }

      return intersections.Count > 0;
    }

    /// <summary>
    /// Returns true if [p1] lies inside [geom], false otherwise
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="geom"></param>
    /// <returns></returns>
    public static bool GeometryContainsPoint(Point p1, Geometry geom) {
      return GeometryContainsPoint(p1, geom, false);
    }


    /// <summary>
    /// Returns true if [fe] contains [p]. [fe] must be onscreen.
    /// </summary>
    /// <param name="p">Point in [fe]'s coordinates</param>
    /// <param name="fe">The framework element to test</param>
    /// <returns>True if [fe] contains [p] else false</returns>
    /// <remarks>Silverlight's gives false negatives SOMETIMES so it is conclusive if true, not if false</remarks>
    public static bool OnscreenElementContainsPoint(Point p, FrameworkElement fe) {

      GeneralTransform global = DiagramPanel.CoordinatesTransform(Application.Current.RootVisual, fe);
      if (global != null) p = global.Transform(p);
      IEnumerable<UIElement> result = VisualTreeHelper.FindElementsInHostCoordinates(p, fe);
      foreach (UIElement el in result) 
        if (el == fe) 
          return true;
      return false;









    }

 
    /// <summary>
    /// Count intersections on the ray from outside bounding box to p1. If odd, it's in, else it's out (Raycasting)
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="geom"></param>
    /// <param name="overrideNonZero"></param>
    /// <returns>True if p1 is in the geometry</returns>
    public static bool GeometryContainsPoint(Point p1, Geometry geom, bool overrideNonZero) {
      if (geom.Transform != null) {
        Matrix tm = new Matrix();
        TransformGroup tg = new TransformGroup();
        tg.Children = new TransformCollection();
        tg.Children.Add(geom.Transform);
        tm = tg.Value;

        Geo.Invert(ref tm);
        p1 = tm.Transform(p1);
      }
      FillRule fillrule = FillRule.Nonzero;
      List<Point> curFigure = new List<Point>();
      Rect bounds = geom.Bounds;
      // What kind of geometry?
      if (geom is LineGeometry) {
        LineGeometry linegeom = geom as LineGeometry;
        curFigure.Add(linegeom.StartPoint);
        curFigure.Add(linegeom.EndPoint);
      }
      else if (geom is RectangleGeometry) {
        RectangleGeometry rectgeom = geom as RectangleGeometry;
        Rect rect = rectgeom.Bounds;
        Inflate(ref rect, .25, .25); // If it's witin a half pixel it's in
        return rect.Contains(p1);
      }
      else if (geom is EllipseGeometry) {
        EllipseGeometry eg = geom as EllipseGeometry;
        // Just use raycasting here instead of trying to approximate the ellipse (It doesn't matter since this ellipse has no children):
        return GeometryContainsPointRaycasting(p1, geom);
      }
      else if (geom is PathGeometry) {
        PathGeometry pg = geom as PathGeometry;
        // see if a PathFigureCollection is available
        if (!PathGeometryHasFigures(pg)) {
          Rect rect = pg.Bounds;
          Inflate(ref rect, .25, .25); // If it's witin a half pixel it's in
          return rect.Contains(p1);
        }
        if (!overrideNonZero) fillrule = pg.FillRule;
        if (fillrule == FillRule.EvenOdd)
          return GeometryContainsPointRaycasting(p1, geom);

        curFigure = GetKeyPointsInPathGeometry(pg);
      }
      else { // geom is GroupGeometry, recursively call this method
        if (!overrideNonZero) fillrule = ((GeometryGroup)geom).FillRule;
        if (fillrule == FillRule.EvenOdd)
          return GeometryContainsPointRaycasting(p1, geom);
        foreach (Geometry geo in ((GeometryGroup)geom).Children) {
          // Override any fillrules of the children since they don't matter
          if (GeometryContainsPoint(p1, geo, true))
            return true;
        }
      }

      return FigureContainsPointWinding(p1, curFigure.ToArray(), bounds);
    }

    /// <summary>
    /// Determines using raycasting whether or not [p1] lies inside [geom]
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="geom"></param>
    /// <returns>True if p1 is in the geometry</returns>
    public static bool GeometryContainsPointRaycasting(Point p1, Geometry geom) {
      List<Point> result = new List<Point>();
      int invalid = 0;

      Rect bounds = geom.Bounds;
      Point p2 = new Point(bounds.Left - 50, bounds.Top - 25); // gets bounds AFTER Transforms

      // Make sure it's inside the bounding box first
      if (p1.X < p2.X || p1.Y < p2.Y || p1.X > bounds.Right || p1.Y > bounds.Bottom) return false;

      // If it's inside the bounding box, count intersections on the ray from P2 to P1
      GetIntersectionsOnGeometry(geom, p1, p2, out result, true);
      Point[] points = result.ToArray();
      result.Clear();
      bool add = true;
      // Add points back unless they're duplicates
      for (int i = 0; i < points.Length; i++) {
        add = true;
        for (int j = i + 1; j < points.Length; j++)
          if (IsApproxEqual(points[j],points[i])) // If it's the same point
            add = false;
        if (add)
          result.Add(points[i]);
      }
      foreach (Point p in result) {
        if (IsApproxEqual(p, p1)) return true;
        if (p.X > p1.X || p.Y > p1.Y) invalid++;
      }

      return (result.Count - invalid) % 2 != 0;
    }

    /// <summary>
    /// Elevate the given Bezier Curve up to degree [epsilon]
    /// </summary>
    /// <param name="controlPoints">An array of control points that define a Bezier Curve</param>
    /// <param name="epsilon">The degree to raise the bezier to</param>
    /// <returns>A list of points that define an elevated Bezier Curve</returns>
    public static Point[] ApproximateBezierElevation(Point[] controlPoints, double epsilon) {
      Point[] nextApprox = new Point[controlPoints.Length + 1];
      nextApprox[0] = controlPoints[0];
      for (int i = 1; i < controlPoints.Length; i++) {
        double a = i / (double)controlPoints.Length;
        nextApprox[i] = new Point(a * controlPoints[i - 1].X + (1 - a) * controlPoints[i].X, a * controlPoints[i - 1].Y + (1 - a) * controlPoints[i].Y);
      }
      nextApprox[controlPoints.Length] = controlPoints[controlPoints.Length - 1];
      if (nextApprox.Length > epsilon) // If we're close enough
        return nextApprox;
      return ApproximateBezierElevation(nextApprox, epsilon); // If we're not within epsilon, keep going
    }

    /// <summary>
    /// Breaks the bezier up into line segments that are a maximum of sqrt(epsilon) away from the original curve
    /// </summary>
    /// <param name="p1">Start point of the bezier curve</param>
    /// <param name="p2">First control point of the bezier curve</param>
    /// <param name="p3">Second control point of the bezier curve</param>
    /// <param name="p4">End point of the bezier curve</param>
    /// <param name="result">The list of line segments</param>
    /// <param name="epsilon">The square of the maximum amount of deviation from the original curve</param>
    public static void SubdivideBezier(Point p1, Point p2, Point p3, Point p4, List<Point> result, double epsilon) {

      // Get all of the standard points in bezier subdivision
      double x12 = (p1.X + p2.X) / 2;
      double y12 = (p1.Y + p2.Y) / 2;
      double x23 = (p2.X + p3.X) / 2;
      double y23 = (p2.Y + p3.Y) / 2;
      double x34 = (p3.X + p4.X) / 2;
      double y34 = (p3.Y + p4.Y) / 2;
      double x123 = (x12 + x23) / 2;
      double y123 = (y12 + y23) / 2;
      double x234 = (x23 + x34) / 2;
      double y234 = (y23 + y34) / 2;
      double x1234 = (x123 + x234) / 2;
      double y1234 = (y123 + y234) / 2;

      // See how flat the curve is
      double dx = p4.X - p1.X;
      double dy = p4.Y - p1.Y;

      double d2 = Math.Abs(((p2.X - p4.X) * dy - (p2.Y - p4.Y) * dx));
      double d3 = Math.Abs(((p3.X - p4.X) * dy - (p3.Y - p4.Y) * dx));

      // Make sure distance is acceptable before adding
      if ((d2 + d3) * (d2 + d3) < epsilon * (dx * dx + dy * dy)) {
        result.Add(new Point(x1234, y1234));
        return;
      }

      // Recursively subdivide
      SubdivideBezier(p1, new Point(x12, y12), new Point(x123, y123), new Point(x1234, y1234), result, epsilon);
      SubdivideBezier(new Point(x1234, y1234), new Point(x234, y234), new Point(x34, y34), p4, result, epsilon);
    }

    /// <summary>
    /// Returns a list of points that approximate bezier [p1-p4] with maximum error of epsilon
    /// </summary>
    /// <param name="p1">Start point of the bezier curve</param>
    /// <param name="p2">First control point of the bezier curve</param>
    /// <param name="p3">Second control point of the bezier curve</param>
    /// <param name="p4">End point of the bezier curve</param>
    /// <param name="epsilon">The maximum amount of error</param>
    /// <returns>A list of points that approximate the bezier curve</returns>
    public static List<Point> ApproximateBezier(Point p1, Point p2, Point p3, Point p4, double epsilon) {
      List<Point> result = new List<Point>();
      result.Add(p1);
      SubdivideBezier(p1, p2, p3, p4, result, epsilon * epsilon);
      result.Add(p4);
      return result;
    }

    /// <summary>
    /// Returns a piecewise linear approximation of the given arcsegment, stepping by epsilon
    /// </summary>
    /// <param name="arcSeg">The arc segment to approximate</param>
    /// <param name="ellipseStartPoint">The point to start the arc segment at</param>
    /// <param name="epsilon">The x distance between each point</param>
    /// <returns>The list of points that approximate the arc segment</returns>
    public static List<Point> ApproximateArcSegment(ArcSegment arcSeg, Point ellipseStartPoint, double epsilon) {
      if (arcSeg.Point == ellipseStartPoint) return new List<Point>();
      // Get the center
      Point center;
      Point startPoint;
      Point endPoint;
      Size size = arcSeg.Size;

      // Now set up the startPoint and endPoint
      double half = (new Vector(arcSeg.Point.X - ellipseStartPoint.X, arcSeg.Point.Y - ellipseStartPoint.Y)).Length;

      startPoint = RotatePoint(ellipseStartPoint, -1 * arcSeg.RotationAngle);
      startPoint = ScalePoint(startPoint, (arcSeg.Size.Height) / (arcSeg.Size.Width), 1);
      endPoint = RotatePoint(arcSeg.Point, -1 * arcSeg.RotationAngle);
      endPoint = ScalePoint(endPoint, (arcSeg.Size.Height) / (arcSeg.Size.Width), 1);

      Vector unitVec = new Vector(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
      double halfDistance = unitVec.Length / 2;
      unitVec.Normalize(); // Now make it a unit vector

      // What arc do we use?
      bool rotateCCW = (arcSeg.SweepDirection == SweepDirection.Counterclockwise) == arcSeg.IsLargeArc;
      if (rotateCCW) // Rotate it
        unitVec = new Vector(-1 * unitVec.Y, unitVec.X);
      else
        unitVec = new Vector(unitVec.Y, -1 * unitVec.X);

      Point mid = new Point((endPoint.X + startPoint.X) / 2, (endPoint.Y + startPoint.Y) / 2);
      double distToCent = Math.Sqrt(arcSeg.Size.Height * arcSeg.Size.Height - halfDistance * halfDistance);
      if (double.IsNaN(distToCent)) {
        endPoint = arcSeg.Point;
        startPoint = ellipseStartPoint;
        endPoint = RotatePoint(endPoint, -arcSeg.RotationAngle);
        startPoint = RotatePoint(startPoint, -arcSeg.RotationAngle);
        center = new Point((endPoint.X + startPoint.X) / 2, (endPoint.Y + startPoint.Y) / 2);

        double ratio = arcSeg.Size.Width / arcSeg.Size.Height;
        size.Height = Math.Sqrt((startPoint.X - center.X) * (startPoint.X - center.X) / (ratio * ratio) + (startPoint.Y - center.Y) * (startPoint.Y - center.Y));
        size.Width = size.Height * ratio;
        center = RotatePoint(center, arcSeg.RotationAngle);
      }
      else // The adjusted center of the ellipse
      {
        center = mid + distToCent * unitVec;

        center = ScalePoint(center, arcSeg.Size.Width / arcSeg.Size.Height, 1);
        center = RotatePoint(center, arcSeg.RotationAngle);
      }

      double transX = center.X;
      double transY = center.Y;
      // Rotate back
      center = RotatePoint(center, -arcSeg.RotationAngle);
      startPoint = RotatePoint(ellipseStartPoint, -arcSeg.RotationAngle);
      endPoint = RotatePoint(arcSeg.Point, -arcSeg.RotationAngle);
      // Translate to origin
      center = TranslatePoint(center, -transX, -transY);
      startPoint = TranslatePoint(startPoint, -transX, -transY);
      endPoint = TranslatePoint(endPoint, -transX, -transY);
      double a = size.Width;
      double b = size.Height;
      double sign = 1;
      double dir = 1;
      double x = startPoint.X;
      double nextX;
      double nextY;
      double endSign = 1;
      if (endPoint.Y < 0) endSign = -1;
      bool done = false;
      int count = 0;
      Rect stopBounds = new Rect();
      Point p;
      List<Point> points = new List<Point>();
      p = startPoint;
      p = TranslatePoint(p, transX, transY);
      p = RotatePoint(p, arcSeg.RotationAngle);
      points.Add(p);
      if (startPoint.Y < 0)
        sign = -1;
      if (arcSeg.SweepDirection == SweepDirection.Clockwise)
        dir = -sign;
      else
        dir = sign;

      // Figure out where to stop:
      if (endPoint.X + epsilon <= a) { // Go right
        nextX = endPoint.X + epsilon;
        stopBounds = new Rect(new Point(endPoint.X, endPoint.Y), new Point(endPoint.X + epsilon, endSign * b * Math.Sqrt(1 - (nextX * nextX) / (a * a))));
      }
      else { // Go left
        nextX = endPoint.X - epsilon;
        stopBounds = new Rect(new Point(endPoint.X, endPoint.Y), new Point(endPoint.X - epsilon, endSign * b * Math.Sqrt(1 - (nextX * nextX) / (a * a))));
      }

      nextX = x + dir * epsilon;
      if (1 - (nextX * nextX) / (a * a) < 0) {
        p = new Point(dir * a, 0);
        if (Geo.Contains(stopBounds, p))
          done = true;
        p = TranslatePoint(p, transX, transY);
        p = RotatePoint(p, arcSeg.RotationAngle);
        points.Add(p);
        // Do the next half of the ellipse
        sign = -sign;
        dir = -dir;
        nextX = nextX + dir * epsilon;
        nextX = nextX + dir * epsilon;
        nextY = sign * b * Math.Sqrt(1 - (nextX * nextX) / (a * a));
      }
      else
        nextY = sign * b * Math.Sqrt(1 - (nextX * nextX) / (a * a));
      while (!done) {
        while (nextX > -a && nextX < a) {
          p = new Point(nextX, nextY);
          if (Geo.Contains(stopBounds, p)) {
            done = true;
            break;
          }
          p = TranslatePoint(p, transX, transY);
          p = RotatePoint(p, arcSeg.RotationAngle);
          points.Add(p);
          nextX = nextX + dir * epsilon;
          nextY = sign * b * Math.Sqrt(1 - (nextX * nextX) / (a * a));
        }
        if (!done) {
          p = new Point(dir * a, 0);
          if (Geo.Contains(stopBounds, p)) {
            done = true;
            break;
          }
          p = TranslatePoint(p, transX, transY);
          p = RotatePoint(p, arcSeg.RotationAngle);
          points.Add(p);
          // Do the next half of the ellipse
          sign = -sign;
          dir = -dir;
          nextX = nextX + dir * epsilon;
          nextY = sign * b * Math.Sqrt(1 - (nextX * nextX) / (a * a));

          if (count++ > 3) { done = true; break; } // Makes sure this never gets into an infinite loop (Should throw error here?)
        }
      }

      // Finally, add the end point
      endPoint = TranslatePoint(endPoint, transX, transY);
      endPoint = RotatePoint(endPoint, arcSeg.RotationAngle);
      points.Add(endPoint);

      return points;
    }

    /// <summary>
    /// Returns a list of points that approximate the pathgeometry [pg]
    /// </summary>
    /// <param name="pg">The path geometry to approximate</param>
    /// <returns>A list of points that approximate the path geometry</returns>
    public static List<Point> GetKeyPointsInPathGeometry(PathGeometry pg) {
      // ought to check Geo.PathGeometryHasFigures,
      // but the only caller of GetKeyPointsInPathGeometry does that instead, for speed
      List<Point> result = new List<Point>();
      Point lastPoint;
      foreach (PathFigure fig in pg.Figures) {
        if (!fig.IsFilled) continue;
        result.Add(fig.StartPoint);
        lastPoint = fig.StartPoint;
        foreach (PathSegment seg in fig.Segments) {
          if (seg is LineSegment) {
            LineSegment ls = seg as LineSegment;
            result.Add(ls.Point);
          }
          else if (seg is ArcSegment) {
            ArcSegment aseg = seg as ArcSegment;
            result.AddRange(ApproximateArcSegment(aseg, lastPoint, 5));
          }
          else if (seg is BezierSegment) {
            BezierSegment bs = seg as BezierSegment;
            result.AddRange(ApproximateBezier(lastPoint, bs.Point1, bs.Point2, bs.Point3, .5));
          }
          else if (seg is QuadraticBezierSegment) {
            QuadraticBezierSegment qbs = seg as QuadraticBezierSegment;
            Point[] points = new Point[3] { lastPoint, qbs.Point1, qbs.Point2 };
            points = ApproximateBezierElevation(points, 4); // elevate by 1 degree
            result.AddRange(ApproximateBezier(points[0], points[1], points[2], points[3], .5));
          }
          else if (seg is PolyLineSegment) {
            PolyLineSegment pls = seg as PolyLineSegment;
            foreach (Point p in pls.Points)
              result.Add(p);
          }
          else if (seg is PolyBezierSegment) {
            PolyBezierSegment pbs = seg as PolyBezierSegment;
            Point[] points = new Point[4];
            points[0] = lastPoint;
            int count = 1;
            foreach (Point p in pbs.Points) {
              points[count++] = p;
              if (count == 4) {
                count = 0;
                result.AddRange(ApproximateBezier(points[0], points[1], points[2], points[3], .01));
              }
            }
          }
          else { // seg is PolyQuadraticBezierSegment
            PolyQuadraticBezierSegment pqbs = seg as PolyQuadraticBezierSegment;
            Point[] points = new Point[3];
            points[0] = lastPoint;
            int count = 1;
            foreach (Point p in pqbs.Points) {
              points[count++] = p;
              if (count == 3) {
                count = 0;
                points = ApproximateBezierElevation(points, 4); // elevate by 1 degree
                result.AddRange(ApproximateBezier(points[0], points[1], points[2], points[3], .01));
              }
            }
          }
          lastPoint = result[result.Count - 1];
        }
      }
      return result;
    }

    // returns the square of the distance between A and B
    public static double DistanceSquared(Point a, Point b) {
      return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
    }

    // returns the square of the distance between P and the finite line segment A-B
    public static double DistanceToLineSegmentSquared(Point a, Point b, Point p) {
      double ux = b.X-a.X;
      double uy = b.Y-a.Y;
      double length = ux*ux + uy*uy;
      double vx = a.X-p.X;
      double vy = a.Y-p.Y;
      double dot = - vx*ux - vy*uy;
      if (dot <= 0 || dot >= length) {  // beyond the segment
        ux = b.X-p.X;
        uy = b.Y-p.Y;
        return Math.Min(vx*vx + vy*vy, ux*ux + uy*uy);
      } else {
        double det = ux*vy - uy*vx;
        return (det*det) / length;
      }
    }

    /// <summary>
    /// Determines using the Winding Number method whether [p1] is in the figure defined by [points]
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="points"></param>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public static bool FigureContainsPointWinding(Point p1, Point[] points, Rect bounds) {
      if (points.Length < 3) {
        if (points.Length == 1 && p1 == points[0]) return true;
        if (points.Length == 2) return LineContainsPoint(points[0], points[1], .001, p1);
        return false;
      }
      // First make sure the point [p1] is even in [bounds]
      if (p1.X < bounds.X || p1.Y < bounds.Y || p1.X > bounds.Right || p1.Y > bounds.Bottom) return false;
      if (DistanceSquared(points[0], p1) <= .25) return true;

      double yoff = -p1.Y;
      double xoff = -p1.X;
      p1 = new Point(p1.X + xoff, p1.Y + yoff); // make p1 the origin;
      Point p2 = new Point(1, 0); // Make a line representing the x axis

      int windingNumber = 0;
      Point last = new Point(points[0].X + xoff, points[0].Y + yoff);
      for (int i = 1; i < points.Length; i++) {
        Point p = new Point(points[i].X + xoff, points[i].Y + yoff);
        if (DistanceSquared(points[i], p1) <= .25) return true;
        //if (LineContainsPoint(last, points[i], .01, p1)) return true; // If it's on an edge, it's in the figure
        if (p.Y == 0 && last.Y == 0) continue;
        Point intersect;
        if (Geo.NearestIntersectionOnLine(last, p, p1, p2, out intersect) && intersect.X >= 0) {
          if (p.Y > last.Y) // p is on top
          {
            if (p.Y == 0 || last.Y == 0) // But not both
              windingNumber += 5;
            else
              windingNumber += 10;
          }
          else { // last is on top
            if (p.Y == 0 || last.Y == 0) // But not both
              windingNumber -= 5;
            else
              windingNumber -= 10;
          }
        }

        last = p;
      }

      return windingNumber != 0;
    }

    /// <summary>
    /// Inverts the square Matrix [matrix]
    /// </summary>
    /// <param name="matrix">The matrix to invert</param>
    public static void Invert(ref Matrix matrix) {
      double det = matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21;
      if (det == 0) return; // No inverse exists
      double temp11 = matrix.M11;
      double temp21 = matrix.M21;
      double temp12 = matrix.M12;
      double temp22 = matrix.M22;
      double tempX = matrix.OffsetX;
      matrix.M11 = matrix.M22 / det;
      matrix.M12 = -1 * matrix.M12 / det;
      matrix.M21 = -1 * matrix.M21 / det;
      matrix.M22 = temp11 / det;
      matrix.OffsetX = (temp21 * matrix.OffsetY - matrix.OffsetX * temp22) / det;
      matrix.OffsetY = -1 * (temp11 * matrix.OffsetY - tempX * temp12) / det;
    }


    public static Point[] ApproximateEllipse(EllipseGeometry ellipse) {
      Point cnt = new Point(Math.Ceiling(ellipse.Center.X), Math.Ceiling(ellipse.Center.Y));
      int radX = (int)Math.Ceiling(ellipse.RadiusX);
      int radY = (int)Math.Ceiling(ellipse.RadiusY);
      int width = 2 * radX;
      int height = 2 * radY;

      if (width == 0 || height == 0) return new Point[0];
      Point[] points = new Point[2 * (int)Math.Round(cnt.X + radX - (cnt.X - radX + 1)) + 2];

      points[0] = new Point(cnt.X - radX, cnt.Y);
      points[points.Length - 1] = new Point(cnt.X + radX, cnt.Y);
      int count = 1;
      for (int x = (int)cnt.X - radX + 1; x < cnt.X + radX; x++) {
        double y = radY * Math.Sqrt(1 - ((x - cnt.X) * (x - cnt.X)) / (radX * radX)) + cnt.Y;
        points[count++] = new Point(x, y);
        points[count++] = new Point(x, 2 * cnt.Y - y);
      }

      return points;
    }

    public static bool EllipseOverlapsPart(Point[] ellipse, Rect ellipseBounds, Part part) {
      DiagramPanel dp = part.Panel as DiagramPanel;

      Rect r = part.Bounds;
      if (Geo.Contains(ellipseBounds, r) || Geo.Contains(r, ellipseBounds)) return true;

      Rect intersection = Intersection(ellipseBounds, r);
      Point topleft = dp.TransformModelToGlobal(new Point(intersection.X, intersection.Y));
      Point botright = dp.TransformModelToGlobal(new Point(intersection.X + intersection.Width, intersection.Y + intersection.Height));
      intersection = new Rect(topleft, botright);
      if (intersection.Width == 0 || intersection.Height == 0) return false; // Bounding boxes don't intersect

      Point t;
      foreach (Point p in ellipse) {
        t = dp.TransformModelToGlobal(p);
        if (intersection.Contains(t)) {
          IEnumerable<UIElement> search = VisualTreeHelper.FindElementsInHostCoordinates(t, dp);
          foreach (UIElement next in search)
            if (next == part) return true;
        }
      }
      return false;
    }

    public static bool EllipseOverlapsElement(Point[] ellipse, Rect ellipseBounds, UIElement elt) {
      Part part = Diagram.FindAncestor<Part>(elt);
      if (part == null || !(elt is FrameworkElement)) return false;
     
      DiagramPanel dp = part.Panel as DiagramPanel;
      Rect r = part.GetElementBounds(elt as FrameworkElement);
      if (Geo.Contains(ellipseBounds, r) || Geo.Contains(r, ellipseBounds)) return true;

      Rect intersection = Intersection(ellipseBounds, r);
      Point topleft = dp.TransformModelToGlobal(new Point(intersection.X, intersection.Y));
      Point botright = dp.TransformModelToGlobal(new Point(intersection.X + intersection.Width, intersection.Y + intersection.Height));
      intersection = new Rect(topleft, botright);
      if (intersection.Width == 0 || intersection.Height == 0) return false; // Bounding boxes don't intersect

      Point t;
      foreach (Point p in ellipse) {
        t = dp.TransformModelToGlobal(p);
        if (intersection.Contains(t)) {
          IEnumerable<UIElement> search = VisualTreeHelper.FindElementsInHostCoordinates(t, dp);
          foreach (UIElement next in search)
            if (next == elt) return true;
        }
      }
      return false;
    }

  }
}
