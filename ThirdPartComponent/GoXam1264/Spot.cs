
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
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace Northwoods.GoXam {

  /// <summary>
  /// A spot represents a relative point from (0,0) to (1,1) within the bounds of an element, plus an absolute offset.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A spot can represent a particular point (such as <see cref="TopLeft"/> or <c>new Spot(0.25, 0.25)</c>),
  /// or a particular side (such as <see cref="RightSide"/>)
  /// or multiple sides (such as <see cref="TopBottomSides"/>),
  /// or when equal to <see cref="None"/> no particular point at all.
  /// </para>
  /// <para>
  /// The <see cref="X"/> and <see cref="Y"/> values denote the fractional distance
  /// of the spot point along the dimensions of the element's width and height.
  /// Zero is at the left or the top of the element;
  /// one is at the right or the bottom of the element.
  /// The values must be between 0 and 1.
  /// Values that are <c>NaN</c> are reserved for special spots such as
  /// <see cref="None"/> or the spots that represent sides.
  /// </para>
  /// <para>
  /// Once the point is determined by the <see cref="X"/> and <see cref="Y"/>
  /// values within a particular bounding rectangle, the <see cref="OffsetX"/>
  /// and <see cref="OffsetY"/> values are added to determine the final point.
  /// The offset values may be negative.
  /// Negative offset values when the <c>(X,Y)</c> are near the left and/or top
  /// may result in a point that is outside of the bounds of the element.
  /// Positive offset values when the <c>(X,Y)</c> are near the right and/or bottom
  /// may also result in a point that is outside of the bounds of the element.
  /// </para>
  /// </remarks>




  [TypeConverter(typeof(Northwoods.GoXam.Spot.SpotConverter))]
  public struct Spot : IFormattable {
    private double _X;
    private double _Y;
    private double _OffsetX;
    private double _OffsetY;

    /// <summary>
    /// Use this <see cref="Spot"/> value to indicate no particular spot --
    /// code looking for a particular point on an element will need to do their
    /// own calculations to determine the desired point depending on the circumstances.
    /// </summary>
    public readonly static Spot None = new Spot(Double.NaN, Double.NaN, 0, 0);

    /// <summary>
    /// Use this <see cref="Spot"/> value to indicate that the real spot value is elsewhere.
    /// </summary>
    public readonly static Spot Default = new Spot(Double.NaN, Double.NaN, -1, 0);

    /// <summary>
    /// The specific point at the top-left corner of the bounding rectangle.
    /// </summary>
    public readonly static Spot TopLeft =       new Spot(0.0, 0.0, 0, 0);

    /// <summary>
    /// The specific point at the center of the top side of the bounding rectangle.
    /// </summary>
    public readonly static Spot TopCenter =     new Spot(0.5, 0.0, 0, 0);

    /// <summary>
    /// A synonym for <see cref="Spot.TopCenter"/>.
    /// </summary>
    public readonly static Spot MiddleTop = TopCenter;

    /// <summary>
    /// The specific point at the top-right corner of the bounding rectangle.
    /// </summary>
    public readonly static Spot TopRight =      new Spot(1.0, 0.0, 0, 0);

    /// <summary>
    /// The specific point at the middle of the left side of the bounding rectangle.
    /// </summary>
    public readonly static Spot MiddleLeft =    new Spot(0.0, 0.5, 0, 0);

    /// <summary>
    /// The specific point at the very center of the bounding rectangle.
    /// </summary>
    public readonly static Spot Center =        new Spot(0.5, 0.5, 0, 0);

    /// <summary>
    /// The specific point at the middle of the right side of the bounding rectangle.
    /// </summary>
    public readonly static Spot MiddleRight =   new Spot(1.0, 0.5, 0, 0);

    /// <summary>
    /// The specific point at the bottom-left corner of the bounding rectangle.
    /// </summary>
    public readonly static Spot BottomLeft =    new Spot(0.0, 1.0, 0, 0);

    /// <summary>
    /// The specific point at the middle of the bottom side of the bounding rectangle.
    /// </summary>
    public readonly static Spot BottomCenter =  new Spot(0.5, 1.0, 0, 0);

    /// <summary>
    /// A synonym for <see cref="Spot.BottomCenter"/>.
    /// </summary>
    public readonly static Spot MiddleBottom = BottomCenter;

    /// <summary>
    /// The specific point at the bottom-right corner of the bounding rectangle.
    /// </summary>
    public readonly static Spot BottomRight =   new Spot(1.0, 1.0, 0, 0);

    internal const int MTop = 1;
    internal const int MLeft = 2;
    internal const int MRight = 4;
    internal const int MBottom = 8;

    /// <summary>
    /// The set of points at the top side of the bounding rectangle.
    /// </summary>
    public readonly static Spot TopSide =          new Spot(Double.NaN, Double.NaN, 1, MTop);

    /// <summary>
    /// The set of points at the left side of the bounding rectangle.
    /// </summary>
    public readonly static Spot LeftSide =         new Spot(Double.NaN, Double.NaN, 1, MLeft);

    /// <summary>
    /// The set of points at the right side of the bounding rectangle.
    /// </summary>
    public readonly static Spot RightSide =        new Spot(Double.NaN, Double.NaN, 1, MRight);

    /// <summary>
    /// The set of points at the bottom side of the bounding rectangle.
    /// </summary>
    public readonly static Spot BottomSide =       new Spot(Double.NaN, Double.NaN, 1, MBottom);


    /// <summary>
    /// The set of points at the top or bottom sides of the bounding rectangle.
    /// </summary>
    public readonly static Spot TopBottomSides =   new Spot(Double.NaN, Double.NaN, 1, MTop | MBottom);

    /// <summary>
    /// The set of points at the left or right sides of the bounding rectangle.
    /// </summary>
    public readonly static Spot LeftRightSides =   new Spot(Double.NaN, Double.NaN, 1, MLeft | MRight);


    /// <summary>
    /// The set of points at the top or left sides of the bounding rectangle.
    /// </summary>
    public readonly static Spot TopLeftSides =     new Spot(Double.NaN, Double.NaN, 1, MTop | MLeft);

    /// <summary>
    /// The set of points at the top or right sides of the bounding rectangle.
    /// </summary>
    public readonly static Spot TopRightSides =    new Spot(Double.NaN, Double.NaN, 1, MTop | MRight);

    /// <summary>
    /// The set of points at the left or bottom sides of the bounding rectangle.
    /// </summary>
    public readonly static Spot BottomLeftSides =  new Spot(Double.NaN, Double.NaN, 1, MBottom | MLeft);

    /// <summary>
    /// The set of points at the right or bottom sides of the bounding rectangle.
    /// </summary>
    public readonly static Spot BottomRightSides = new Spot(Double.NaN, Double.NaN, 1, MBottom | MRight);


    /// <summary>
    /// The set of points on all sides of the bounding rectangle except the top side.
    /// </summary>
    public readonly static Spot NotTopSide =       new Spot(Double.NaN, Double.NaN, 1, MLeft | MRight | MBottom);

    /// <summary>
    /// The set of points on all sides of the bounding rectangle except the left side.
    /// </summary>
    public readonly static Spot NotLeftSide =      new Spot(Double.NaN, Double.NaN, 1, MTop | MRight | MBottom);

    /// <summary>
    /// The set of points on all sides of the bounding rectangle except the right side.
    /// </summary>
    public readonly static Spot NotRightSide =     new Spot(Double.NaN, Double.NaN, 1, MTop | MLeft | MBottom);

    /// <summary>
    /// The set of points on all sides of the bounding rectangle except the bottom side.
    /// </summary>
    public readonly static Spot NotBottomSide =    new Spot(Double.NaN, Double.NaN, 1, MTop | MLeft | MRight);


    /// <summary>
    /// The set of points on all sides of the bounding rectangle.
    /// </summary>
    public readonly static Spot AllSides =         new Spot(Double.NaN, Double.NaN, 1, MTop | MLeft | MRight | MBottom);


    /// <summary>
    /// Create a spot defining a particular relative point in a rectangle.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Spot(double x, double y) {
      CheckLocation(x);
      CheckLocation(y);
      _X = x;
      _Y = y;
      _OffsetX = 0;
      _OffsetY = 0;
    }

    /// <summary>
    /// Create a spot defining a particular relative point in a rectangle.
    /// </summary>
    /// <param name="location"></param>
    public Spot(Point location) : this(location.X, location.Y) { }

    /// <summary>
    /// Create a spot defining a particular relative point in a rectangle plus an offset.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="offx"></param>
    /// <param name="offy"></param>
    public Spot(double x, double y, double offx, double offy) {
      CheckLocation(x);
      CheckLocation(y);
      CheckOffset(offx);
      CheckOffset(offy);
      _X = x;
      _Y = y;
      _OffsetX = offx;
      _OffsetY = offy;
    }

    /// <summary>
    /// Create a spot defining a particular relative point in a rectangle plus an offset.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="offset"></param>
    public Spot(Point location, Point offset) : this(location.X, location.Y, offset.X, offset.Y) { }

    private static void CheckLocation(double v) {
      if (v > 1 || v < 0) {
        Diagram.Error("Large Spot values are not permitted; value should be in the range from zero to one: " + v.ToString(System.Globalization.CultureInfo.InvariantCulture));
      }
    }

    private static void CheckOffset(double v) {
      if (Double.IsNaN(v) || Double.IsInfinity(v)) {
        Diagram.Error("Not-a-Number or Infinite Spot offset values are not permitted: " + v.ToString(System.Globalization.CultureInfo.InvariantCulture));
      }
    }


    /// <summary>
    /// Two spots are equal if all four property values are the same
    /// (<see cref="X"/>, <see cref="Y"/>, <see cref="OffsetX"/>, <see cref="OffsetY"/>).
    /// </summary>
    /// <param name="spot1"></param>
    /// <param name="spot2"></param>
    /// <returns>true if the two spots are equal</returns>
    /// <remarks>
    /// If one spot's <see cref="X"/> or <see cref="Y"/> is <c>NaN</c>,
    /// the other spot's corresponding property must also be <c>NaN</c>.
    /// </remarks>
    /// <seealso cref="operator!="/>
    /// <seealso cref="Equals(Spot, Spot)"/>
    public static bool operator==(Spot spot1, Spot spot2) {
      return
        (spot1.X == spot2.X || (Double.IsNaN(spot1.X) && Double.IsNaN(spot2.X))) &&
        (spot1.Y == spot2.Y || (Double.IsNaN(spot1.Y) && Double.IsNaN(spot2.Y))) &&
        spot1.OffsetX == spot2.OffsetX &&
        spot1.OffsetY == spot2.OffsetY;
    }

    /// <summary>
    /// Inequality of spots.
    /// </summary>
    /// <param name="spot1"></param>
    /// <param name="spot2"></param>
    /// <returns>true if the two spots are unequal</returns>
    /// <seealso cref="operator=="/>
    /// <seealso cref="Equals(Spot, Spot)"/>
    public static bool operator!=(Spot spot1, Spot spot2) {
      return
        !(spot1.X == spot2.X || (Double.IsNaN(spot1.X) && Double.IsNaN(spot2.X))) ||
        !(spot1.Y == spot2.Y || (Double.IsNaN(spot1.Y) && Double.IsNaN(spot2.Y))) ||
        spot1.OffsetX != spot2.OffsetX ||
        spot1.OffsetY != spot2.OffsetY;
    }

    /// <summary>
    /// Two spots are equal if all four property values are the same
    /// (<see cref="X"/>, <see cref="Y"/>, <see cref="OffsetX"/>, <see cref="OffsetY"/>).
    /// </summary>
    /// <param name="spot1"></param>
    /// <param name="spot2"></param>
    /// <returns>true if the two spots are equal</returns>
    /// <remarks>
    /// If one spot's <see cref="X"/> or <see cref="Y"/> is <c>NaN</c>,
    /// the other spot's corresponding property must also be <c>NaN</c>.
    /// </remarks>
    public static bool Equals(Spot spot1, Spot spot2) {
      return
        (spot1.X == spot2.X || (Double.IsNaN(spot1.X) && Double.IsNaN(spot2.X))) &&
        (spot1.Y == spot2.Y || (Double.IsNaN(spot1.Y) && Double.IsNaN(spot2.Y))) &&
        spot1.OffsetX == spot2.OffsetX &&
        spot1.OffsetY == spot2.OffsetY;
    }

    /// <summary>
    /// Two spots are equal if all four property values are the same
    /// (<see cref="X"/>, <see cref="Y"/>, <see cref="OffsetX"/>, <see cref="OffsetY"/>).
    /// </summary>
    /// <param name="obj">any <c>Object</c></param>
    /// <returns>true if the other object is a <see cref="Spot"/> and they are equal</returns>
    /// <remarks>
    /// If one spot's <see cref="X"/> or <see cref="Y"/> is <c>NaN</c>,
    /// the other spot's corresponding property must also be <c>NaN</c>.
    /// </remarks>
    public override bool Equals(object obj) {
      if ((obj == null) || !(obj is Spot)) return false;
      return Equals(this, (Spot)obj);
    }

    /// <summary>
    /// Two spots are equal if all four property values are the same
    /// (<see cref="X"/>, <see cref="Y"/>, <see cref="OffsetX"/>, <see cref="OffsetY"/>).
    /// </summary>
    /// <param name="value">a <see cref="Spot"/></param>
    /// <returns>true if the two spots are equal</returns>
    /// <remarks>
    /// If one spot's <see cref="X"/> or <see cref="Y"/> is <c>NaN</c>,
    /// the other spot's corresponding property must also be <c>NaN</c>.
    /// </remarks>
    public bool Equals(Spot value) {
      return Equals(this, value);
    }

    /// <summary>
    /// The hash code is a combination of all four property values.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() {
      return (((_X.GetHashCode() ^ _Y.GetHashCode()) ^ _OffsetX.GetHashCode()) ^ _OffsetY.GetHashCode());
    }


    /// <summary>
    /// Convert a string into a <see cref="Spot"/> value.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// The string can be up to four numbers separated by spaces.
    /// The first number is <see cref="X"/>,
    /// the second number is <see cref="Y"/>,
    /// the third number is <see cref="OffsetX"/>,
    /// the fourth number is <see cref="OffsetY"/>.
    /// Missing numbers default to zero.
    /// </para>
    /// <para>
    /// The string can also be any of the named special spot values,
    /// such as "None", "Center", "TopLeft", "TopSide", "LeftRightSides",
    /// "AllSides", or any other of the predefined field names that are spots.
    /// </para>
    /// </remarks>
    public static Spot Parse(string source) {
      return ConvertFromString(source, null);
    }

    /// <summary>
    /// Convert a string into a <see cref="Spot"/> value.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// The string can be up to four numbers separated by spaces.
    /// The first number is <see cref="X"/>,
    /// the second number is <see cref="Y"/>,
    /// the third number is <see cref="OffsetX"/>,
    /// the fourth number is <see cref="OffsetY"/>.
    /// Missing numbers default to zero.
    /// </para>
    /// <para>
    /// The string can also be any of the named special spot values,
    /// such as "None", "Center", "TopLeft", "TopSide", "LeftRightSides",
    /// "AllSides", or any other of the predefined field names that are spots.
    /// </para>
    /// </remarks>
    public static Spot Parse(string source, IFormatProvider provider) {
      return ConvertFromString(source, provider);
    }

    internal static Spot ConvertFromString(string source, IFormatProvider provider) {
      if (source == null || source.Length == 0) return new Spot();
      if (provider == null) provider = CultureInfo.InvariantCulture;
      String trimmed = source.Trim();
      if (trimmed.Length == 0) return new Spot();
      if (Char.IsLetter(trimmed[0])) {
        // handle both predefined Spot values and side values
        if (trimmed == "None") return None;
        if (trimmed == "Center") return Center;
        if (trimmed == "TopLeft") return TopLeft;
        if (trimmed == "TopCenter") return TopCenter;
        if (trimmed == "MiddleTop") return TopCenter;
        if (trimmed == "TopRight") return TopRight;
        if (trimmed == "MiddleLeft") return MiddleLeft;
        if (trimmed == "MiddleRight") return MiddleRight;
        if (trimmed == "BottomLeft") return BottomLeft;
        if (trimmed == "BottomCenter") return BottomCenter;
        if (trimmed == "MiddleBottom") return BottomCenter;
        if (trimmed == "BottomRight") return BottomRight;
        if (trimmed == "TopSide") return TopSide;
        if (trimmed == "LeftSide") return LeftSide;
        if (trimmed == "RightSide") return RightSide;
        if (trimmed == "BottomSide") return BottomSide;
        if (trimmed == "TopBottomSides") return TopBottomSides;
        if (trimmed == "LeftRightSides") return LeftRightSides;
        if (trimmed == "TopLeftSides") return TopLeftSides;
        if (trimmed == "TopRightSides") return TopRightSides;
        if (trimmed == "BottomLeftSides") return BottomLeftSides;
        if (trimmed == "BottomRightSides") return BottomRightSides;
        if (trimmed == "NotTopSide") return NotTopSide;
        if (trimmed == "NotLeftSide") return NotLeftSide;
        if (trimmed == "NotRightSide") return NotRightSide;
        if (trimmed == "NotBottomSide") return NotBottomSide;
        if (trimmed == "AllSides") return AllSides;
        if (trimmed == "Default") return Default;
      }
      double x = 0;
      double y = 0;
      double offx = 0;
      double offy = 0;
      string[] subs = trimmed.Split(' ');
      int i = 0;
      while (i < subs.Length && subs[i].Length == 0) i++;
      if (i < subs.Length) x = System.Convert.ToDouble(subs[i++], provider);
      while (i < subs.Length && subs[i].Length == 0) i++;
      if (i < subs.Length) y = System.Convert.ToDouble(subs[i++], provider);
      while (i < subs.Length && subs[i].Length == 0) i++;
      if (i < subs.Length) offx = System.Convert.ToDouble(subs[i++], provider);
      while (i < subs.Length && subs[i].Length == 0) i++;
      if (i < subs.Length) offy = System.Convert.ToDouble(subs[i++], provider);
      return new Spot(x, y, offx, offy);
    }


    /// <summary>
    /// Produce a string representation of a <see cref="Spot"/>.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// For special spot values, this results in the name of the spot,
    /// such as "None", "Center", "TopLeft", "TopSide", "LeftRightSides",
    /// "AllSides", or any other of the predefined field names that are spots.
    /// </para>
    /// <para>
    /// For regular specific spots, this results in a string consisting of
    /// four numbers separated by spaces.  The values of <see cref="X"/>,
    /// <see cref="Y"/>, <see cref="OffsetX"/>, and <see cref="OffsetY"/>,
    /// are written in that order.
    /// </para>
    /// </remarks>
    public override string ToString() {
      return ConvertToString(null, null);
    }

    /// <summary>
    /// Produce a string representation of a <see cref="Spot"/>.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// For special spot values, this results in the name of the spot,
    /// such as "None", "Center", "TopLeft", "TopSide", "LeftRightSides",
    /// "AllSides", or any other of the predefined field names that are spots.
    /// </para>
    /// <para>
    /// For regular specific spots, this results in a string consisting of
    /// four numbers separated by spaces.  The values of <see cref="X"/>,
    /// <see cref="Y"/>, <see cref="OffsetX"/>, and <see cref="OffsetY"/>,
    /// are written in that order.
    /// </para>
    /// </remarks>
    public string ToString(IFormatProvider provider) {
      return ConvertToString(null, provider);
    }

    string IFormattable.ToString(string format, IFormatProvider provider) {
      return ConvertToString(format, provider);
    }

    internal string ConvertToString(string format, IFormatProvider provider) {
      if (provider == null) provider = CultureInfo.InvariantCulture;
      if (this.IsSpot) {
        if (format == null) {
          return string.Format(provider, "{0} {1} {2} {3}", new object[] { _X, _Y, _OffsetX, _OffsetY });
        } else {
          return string.Format(provider, "{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "}", new object[] { _X, _Y, _OffsetX, _OffsetY });
        }
      } else {
        // just generate no-spot values
        if (this == None) return "None";
        if (this == TopSide) return "TopSide";
        if (this == LeftSide) return "LeftSide";
        if (this == RightSide) return "RightSide";
        if (this == BottomSide) return "BottomSide";
        if (this == TopBottomSides) return "TopBottomSides";
        if (this == LeftRightSides) return "LeftRightSides";
        if (this == TopLeftSides) return "TopLeftSides";
        if (this == TopRightSides) return "TopRightSides";
        if (this == BottomLeftSides) return "BottomLeftSides";
        if (this == BottomRightSides) return "BottomRightSides";
        if (this == NotTopSide) return "NotTopSide";
        if (this == NotLeftSide) return "NotLeftSide";
        if (this == NotRightSide) return "NotRightSide";
        if (this == NotBottomSide) return "NotBottomSide";
        if (this == AllSides) return "AllSides";
        if (this == Default) return "Default";
        return "None";
      }
    }


    /// <summary>
    /// The fractional point along the X-axis.
    /// </summary>
    /// <value>must be between zero and one, inclusive</value>
    public double X {
      get { return _X; }
      set {
        CheckLocation(value);
        _X = value;
      }
    }

    /// <summary>
    /// The fractional point along the Y-axis.
    /// </summary>
    /// <value>must be between zero and one, inclusive</value>
    public double Y {
      get { return _Y; }
      set {
        CheckLocation(value);
        _Y = value;
      }
    }

    /// <summary>
    /// The final additional offset along the X-axis.
    /// </summary>
    /// <value>may be negative or positive of any reasonable size, but must be a number and not infinity</value>
    public double OffsetX {
      get { return _OffsetX; }
      set {
        CheckOffset(value);
        _OffsetX = value;
      }
    }

    /// <summary>
    /// The final additional offset along the Y-axis.
    /// </summary>
    /// <value>may be negative or positive of any reasonable size, but must be a number and not infinity</value>
    public double OffsetY {
      get { return _OffsetY; }
      set {
        CheckOffset(value);
        _OffsetY = value;
      }
    }


    /// <summary>
    /// True if this spot represents a specific spot, not a side nor <see cref="None"/>.
    /// </summary>
    public bool IsSpot {  // represents a spot, not None nor a side
      get { return !Double.IsNaN(_X) && !Double.IsNaN(_Y); }
    }

    /// <summary>
    /// True if this is an unspecific special spot, such as <see cref="None"/> or one of the sides.
    /// </summary>
    public bool IsNoSpot {  // could be == None or could be a side
      get { return Double.IsNaN(_X) || Double.IsNaN(_Y); }
    }

    /// <summary>
    /// True if and only if this spot is equal to <see cref="None"/>.
    /// </summary>
    public bool IsNone {
      get { return this == None; }
    }

    /// <summary>
    /// True if and only if this spot is different than <see cref="None"/>.
    /// </summary>
    public bool IsNotNone {
      get { return this != None; }
    }

    /// <summary>
    /// True if and only if this spot is equal to <see cref="Default"/>.
    /// </summary>
    public bool IsDefault {
      get { return this == Default; }
    }

    /// <summary>
    /// True if this is a special spot referring to one (or more) of the sides.
    /// </summary>
    /// <remarks>
    /// This is false if the spot is <see cref="None"/>.
    /// </remarks>
    public bool IsSide {
      get { return this.IsNoSpot && _OffsetX == 1 && _OffsetY != 0; }
    }

    /// <summary>
    /// Return a new spot with the same <see cref="X"/> and <see cref="Y"/> values,
    /// but with offsets of zero.
    /// </summary>
    /// <remarks>
    /// The result is meaningless if <see cref="IsNoSpot"/> is true.
    /// </remarks>
    public Spot WithoutOffset {
      get { return new Spot(_X, _Y, 0, 0); }
    }

    /// <summary>
    /// Return a new spot that is opposite this spot.
    /// </summary>
    /// <remarks>
    /// The <see cref="X"/> and <see cref="Y"/> values will be an equal distance
    /// away from the center on the other side of the center.
    /// The <see cref="OffsetX"/> and <see cref="OffsetY"/> values are also negated.
    /// </remarks>
    /// <remarks>
    /// The result is meaningless if <see cref="IsNoSpot"/> is true.
    /// </remarks>
    public Spot Opposite {
      get { return new Spot(0.5-(_X-0.5), 0.5-(_Y-0.5), -_OffsetX, -_OffsetY); }
    }

    /// <summary>
    /// Given a rectangle, return the specific point in or near the rectangle that this spot is at.
    /// </summary>
    /// <param name="r">a <c>Rect</c></param>
    /// <returns>a <c>Point</c></returns>
    /// <remarks>
    /// The result is meaningless if <see cref="IsNoSpot"/> is true.
    /// </remarks>
    public Point PointInRect(Rect r) {
      return new Point(r.X + _X*r.Width + _OffsetX, r.Y + _Y*r.Height + _OffsetY);
    }

    /// <summary>
    /// Given a point and the size of the desired rectangle, return the rectangle
    /// for which this spot is at that point.
    /// </summary>
    /// <param name="p">a <c>Point</c></param>
    /// <param name="sz">a <c>Size</c></param>
    /// <returns>a <c>Rect</c></returns>
    /// <remarks>
    /// The result is meaningless if <see cref="IsNoSpot"/> is true.
    /// </remarks>
    public Rect RectForPoint(Point p, Size sz) {
      return new Rect(p.X - _OffsetX - _X*sz.Width, p.Y - _OffsetY - _Y*sz.Height, sz.Width, sz.Height);
    }

    /// <summary>
    /// This predicate is true if this <see cref="Spot"/> is a side that
    /// includes the side(s) given by <paramref name="side"/>.
    /// </summary>
    /// <param name="side">a <see cref="Spot"/> that <see cref="IsSide"/></param>
    /// <returns></returns>
    public bool IncludesSide(Spot side) {
      if (!this.IsSide) return false;
      if (!side.IsSide) return false;
      int sides = (int)_OffsetY;
      int s = (int)side._OffsetY;
      return ((sides & s) == s);
    }

    /// <summary>
    /// Produce <see cref="Spot"/> that denotes one or more sides,
    /// based on which sides should be included.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="top"></param>
    /// <param name="right"></param>
    /// <param name="bottom"></param>
    /// <returns>
    /// If all of the arguments are false, this returns <see cref="Spot.None"/>;
    /// otherwise the resulting spot will be <see cref="IsSide"/>.
    /// </returns>
    public static Spot GetSide(bool left, bool top, bool right, bool bottom) {
      int bits = 0;
      if (top) bits += MTop;
      if (left) bits += MLeft;
      if (right) bits += MRight;
      if (bottom) bits += MBottom;
      if (bits == 0) return Spot.None;
      return new Spot(Double.NaN, Double.NaN, 1, bits);
    }


#pragma warning disable 1591




























    [EditorBrowsableAttribute(EditorBrowsableState.Never)]
    public sealed class SpotConverter : TypeConverter {  // nested class, must be public for Silverlight
      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
      }

      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
      }

      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
        if (value == null) {
          Diagram.Error("cannot convert a Spot from a null value");
        }
        string source = value as string;
        if (source != null) {
          return Spot.Parse(source, CultureInfo.InvariantCulture);
        }
        return base.ConvertFrom(context, culture, value);
      }

      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
        if (destinationType != null && value is Spot) {
          Spot spot = (Spot)value;
          if (destinationType == typeof(string)) {
            return spot.ConvertToString(null, CultureInfo.InvariantCulture);
          }
        }
        return base.ConvertTo(context, culture, value, destinationType);
      }
    }
#pragma warning restore 1591

  }
}
