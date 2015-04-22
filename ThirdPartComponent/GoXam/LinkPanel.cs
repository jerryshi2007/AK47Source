
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Northwoods.GoXam {

  /// <summary>
  /// A <c>LinkPanel</c> is a <c>Panel</c> used to position and orient elements along
  /// the route of a <see cref="Link"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The simplest <see cref="Link"/> is implemented with a <c>DataTemplate</c> that
  /// consists of a <c>Shape</c> that is the stroke (a line) connecting one node with another.
  /// In WPF that is normally an instance of <c>LinkShape</c>;
  /// in Silverlight that is normally an instance of <c>System.Windows.Shapes.Path</c>.
  /// Such a link cannot have any arrowheads or text labels or other decorations.
  /// Furthermore you cannot connect any link to or from the link itself,
  /// nor can any link labels be selectable or manipulable as separate objects.
  /// </para>
  /// <para>
  /// However, most <see cref="Diagram.LinkTemplate"/>s are implemented as
  /// <c>DataTemplate</c>s consisting of <see cref="LinkPanel"/>s.
  /// The principal child element of a <c>LinkPanel</c> is normally a <c>LinkShape</c>
  /// (WPF) or <c>Path</c> (Silverlight) that has the <c>x:Name</c> of "Path"
  /// and that has the attached property <see cref="LinkPanel.IsLinkShapeProperty"/> set to true.
  /// The panel arranges the other child elements along the <see cref="Link"/>'s
  /// <see cref="Link.Route"/> according to various attached properties.
  /// </para>
  /// <para>
  /// At the current time a <c>LinkPanel</c> or <c>LinkShape</c>/<c>Path</c>
  /// must be the root visual element of a <see cref="Link"/>.
  /// </para>
  /// <para>
  /// Here is a simple <c>DataTemplate</c> for a <see cref="Link"/> that has an arrowhead:
  /// <code>
  ///   &lt;DataTemplate x:Key="ExampleLinkTemplate"&gt;
  ///     &lt;go:LinkPanel go:Link.SelectionElementName="Path" go:Link.SelectionAdorned="True" &gt;
  ///       &lt;go:LinkShape x:Name="Path" go:LinkPanel.IsLinkShape="True" Stroke="Black" StrokeThickness="1" /&gt;
  ///       &lt;Polygon Fill="Black" Points="8 4  0 8  2 4  0 0" go:LinkPanel.Alignment="1 0.5" go:LinkPanel.Index="-1" go:LinkPanel.Orientation="Along" /&gt;
  ///     &lt;/go:LinkPanel&gt;
  ///   &lt;/DataTemplate&gt;
  /// </code>
  /// </para>
  /// </remarks>
  public class LinkPanel : Panel {
    /// <summary>
    /// Construct an empty <see cref="LinkPanel"/>.
    /// </summary>
    /// <remarks>
    /// This sets the <see cref="Part.SelectionElementNameProperty"/> to "Path".
    /// </remarks>
    public LinkPanel() {
      Part.SetSelectionElementName(this, "Path");
    }

    static LinkPanel() {
      ImplementationProperty = DependencyProperty.Register("Implementation", typeof(LinkPanelImplementation), typeof(LinkPanel),
        new FrameworkPropertyMetadata(LinkPanelImplementation.Path));

      IsLinkShapeProperty = DependencyProperty.RegisterAttached("IsLinkShape", typeof(bool), typeof(LinkPanel),
        new FrameworkPropertyMetadata(false));
      IndexProperty = DependencyProperty.RegisterAttached("Index", typeof(int), typeof(LinkPanel),
        new FrameworkPropertyMetadata(Int32.MinValue, OnChangedAttachedProperty));
      FractionProperty = DependencyProperty.RegisterAttached("Fraction", typeof(double), typeof(LinkPanel),
        new FrameworkPropertyMetadata(0.0, OnChangedAttachedProperty));
      OffsetProperty = DependencyProperty.RegisterAttached("Offset", typeof(Point), typeof(LinkPanel),
        new FrameworkPropertyMetadata(new Point(0, 0), OnChangedAttachedProperty));
      AlignmentProperty = DependencyProperty.RegisterAttached("Alignment", typeof(Spot), typeof(LinkPanel),
        new FrameworkPropertyMetadata(Spot.Center, OnChangedAttachedProperty));
      OrientationProperty = DependencyProperty.RegisterAttached("Orientation", typeof(LabelOrientation), typeof(LinkPanel),
        new FrameworkPropertyMetadata(LabelOrientation.None, OnChangedAttachedProperty));
    }

    internal static void OnChangedAttachedProperty(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Part part = Diagram.FindAncestor<Part>(d);
      Link link = part as Link;
      if (link == null) {
        Node node = part as Node;
        if (node != null) {
          link = node.LabeledLink;
          if (link == null) {
            node = node.ContainingSubGraph;
            if (node != null) link = node.LabeledLink;
          }
        }
      }
      if (link != null) link.Remeasure();
    }


    /// <summary>
    /// Gets the <c>LinkShape</c> (WPF) or <c>Path</c> (Silverlight) that represents the visual path of the <see cref="Route"/> of the <see cref="Link"/>.
    /// </summary>
    /// <remarks>
    /// Although a <see cref="LinkPanel"/> is normally a <see cref="Link"/>'s <see cref="Part.VisualElement"/>,
    /// it may be the root element of a <see cref="Node"/> in two cases.
    /// If the panel is the element of an <see cref="Adornment"/>, and the adornment's <see cref="Adornment.AdornedPart"/>
    /// is a <see cref="Link"/>, this returns that link's <see cref="Link.Path"/>.
    /// If the panel is the element of a node that is a label of a link (i.e. <see cref="Node.LabeledLink"/> is non-null),
    /// then this returns that link's path.
    /// </remarks>
    public Shape Path {
      get {
        if (_Path == null) {
          // if this LinkPanel is used in a Node or Group that is the label of a Link, use that link's Path
          // if this LinkPanel is used in an Adornment, if the AdornedPart is actually a Link, use that link's Path
          Node node = Diagram.FindAncestor<Node>(this);
          if (node != null) {
            Adornment ad = node as Adornment;
            if (ad != null) {
              Link adornedlink = ad.AdornedPart as Link;
              if (adornedlink != null) {
                _Path = adornedlink.Path;
                return _Path;
              }
            } else {
              Link labeledlink = node.LabeledLink;
              if (labeledlink != null) {
                _Path = labeledlink.Path;
                return _Path;
              }
            }
          }

          if (this.Implementation != LinkPanelImplementation.Path) return null;

          // search for the link shape defined in templates with the attached property LinkPanel.IsLinkShape

          if (_Path == null) {
            _Path = Part.FindElementDownFrom(this, x => x is Path && GetIsLinkShape(x) && x.Name == "Path") as Path;
          }
          if (_Path == null) {
            _Path = Part.FindElementDownFrom(this, x => x is Path && GetIsLinkShape(x)) as Path;
          }








        }
        return _Path;
      }
    }
    private Shape _Path;  //?? no way to clear these cached references


    /// <summary>
    /// Identifies the <see cref="Implementation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ImplementationProperty;
    /// <summary>
    /// Gets or sets the nature of the link implementation.
    /// </summary>
    /// <value>
    /// This defaults to <see cref="Northwoods.GoXam.LinkPanelImplementation.Path"/>.
    /// A different value will result in <see cref="Path"/> always return null.
    /// </value>
    public LinkPanelImplementation Implementation {
      get { return (LinkPanelImplementation)GetValue(ImplementationProperty); }
      set { SetValue(ImplementationProperty, value); }
    }

    /// <summary>
    /// Identifies the <c>IsLinkShape</c> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsLinkShapeProperty;
    /// <summary>
    /// Gets whether a <c>LinkShape</c> (WPF) or <c>Path</c> (Silverlight)
    /// is the primary stroke for the link.
    /// </summary>
    /// <value>
    /// This defaults to false.
    /// </value>
    public static bool GetIsLinkShape(DependencyObject d) { return (bool)d.GetValue(IsLinkShapeProperty); }

    /// <summary>
    /// Sets whether a <c>LinkShape</c> (WPF) or <c>Path</c> (Silverlight)
    /// is the primary stroke for the link.
    /// </summary>
    public static void SetIsLinkShape(DependencyObject d, bool v) { d.SetValue(IsLinkShapeProperty, v); }

    /// <summary>
    /// Identifies the <c>Index</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty IndexProperty;
    /// <summary>
    /// Gets an element's segment index along the link's route.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <returns>
    /// <para>
    /// Zero identifies the segment closest to the "from" end of the route.
    /// Positive values identify segments further along the route.
    /// Negative values identify segments starting at the "to" end of the route;
    /// -1 is the last segment, -2 is the next to last, etc.
    /// </para>
    /// <para>
    /// If the index is not specified for an element, the <see cref="LinkPanel"/>
    /// will arrange it to be at the <see cref="Route"/>'s <see cref="Route.MidPoint"/>.
    /// </para>
    /// </returns>
    public static int GetIndex(DependencyObject d) { return (int)d.GetValue(IndexProperty); }
    /// <summary>
    /// Sets an element's segment index along the link's route.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <param name="v">
    /// <para>
    /// Zero identifies the segment closest to the "from" end of the route.
    /// Positive values identify segments further along the route.
    /// Negative values identify segments starting at the "to" end of the route;
    /// -1 is the last segment, -2 is the next to last, etc.
    /// </para>
    /// <para>
    /// If the index is not specified for an element, the <see cref="LinkPanel"/>
    /// will arrange it to be at the <see cref="Route"/>'s <see cref="Route.MidPoint"/>.
    /// </para>
    /// </param>
    public static void SetIndex(DependencyObject d, int v) { d.SetValue(IndexProperty, v); }

    /// <summary>
    /// Identifies the <c>Fraction</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty FractionProperty;
    /// <summary>
    /// Gets the fractional distance along a segment at which the element should be positioned.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <returns>
    /// The default value is zero.
    /// For negative indexes, the fraction increases from zero to one as the point moves
    /// from the "to" end towards the "from" end.
    /// </returns>
    /// <remarks>
    /// The fraction is not used when the <see cref="GetIndex"/> value is not a valid point/segment in the route.
    /// </remarks>
    public static double GetFraction(DependencyObject d) { return (double)d.GetValue(FractionProperty); }
    /// <summary>
    /// Sets the fractional distance along a segment at which the element should be positioned.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <param name="v">
    /// The fraction should be between zero and one, inclusive.
    /// For non-negative indexes, the fraction increases from zero to one as the point moves
    /// from the "from" end toward the "to" end.
    /// For negative indexes, the fraction increases from zero to one as the point moves
    /// from the "to" end towards the "from" end.
    /// </param>
    /// <remarks>
    /// The fraction is not used when the <see cref="GetIndex"/> value is not a valid point/segment in the route.
    /// </remarks>
    public static void SetFraction(DependencyObject d, double v) { d.SetValue(FractionProperty, v); }

    /// <summary>
    /// Identifies the <c>Offset</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty OffsetProperty;
    /// <summary>
    /// Gets the distances from the fractional point of a link segment at which the element should be positioned.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <returns>
    /// The default value is (0, 0).
    /// The offset distances are in model coordinates.
    /// The offset is rotated to the angle of the link segment.
    /// Positive X values result in the element being positioned farther along the link segment;
    /// negative values are closer.
    /// Positive Y values result in the element being positioned away from the link segment
    /// on the right side of the path; negative values go toward the left side.
    /// </returns>

    [TypeConverter(typeof(Route.PointConverter))]

    public static Point GetOffset(DependencyObject d) { return (Point)d.GetValue(OffsetProperty); }
    /// <summary>
    /// Sets the distances from the fractional point of a link segment at which the element should be positioned.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <param name="v">
    /// The offset distances are in model coordinates.
    /// The offset is rotated to the angle of the link segment.
    /// Positive X values result in the element being positioned farther along the link segment;
    /// negative values are closer.
    /// Positive Y values result in the element being positioned away from the link segment
    /// on the right side of the path; negative values go toward the left side.
    /// </param>

    [TypeConverter(typeof(Route.PointConverter))]

    public static void SetOffset(DependencyObject d, Point v) { d.SetValue(OffsetProperty, v); }

    /// <summary>
    /// Identifies the <c>Alignment</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty AlignmentProperty;
    /// <summary>
    /// Gets an element's alignment spot, which controls the point of the element that
    /// is positioned at a distance of the way along a particular segment of the route.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <returns>
    /// The default value is <see cref="Spot.Center"/>.
    /// </returns>
    public static Spot GetAlignment(DependencyObject d) { return (Spot)d.GetValue(AlignmentProperty); }
    /// <summary>
    /// Sets an element's alignment spot, which controls the point of the element that
    /// is positioned at a distance of the way along a particular segment of the route.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <param name="v"></param>
    public static void SetAlignment(DependencyObject d, Spot v) { d.SetValue(AlignmentProperty, v); }

    /// <summary>
    /// Identifies the <c>Orientation</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty;
    /// <summary>
    /// Gets an element's intended rotation policy.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <returns>
    /// The default value is <see cref="LabelOrientation.None"/>, meaning the element is not to be rotated.
    /// </returns>
    public static LabelOrientation GetOrientation(DependencyObject d) { return (LabelOrientation)d.GetValue(OrientationProperty); }
    /// <summary>
    /// Sets an element's intended rotation policy.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <param name="v"></param>
    public static void SetOrientation(DependencyObject d, LabelOrientation v) { d.SetValue(OrientationProperty, v); }


    /// <summary>
    /// Dynamically compute the desired angle of an element along a segment of the route.
    /// </summary>
    /// <param name="elt">the <c>UIElement</c> being rotated</param>
    /// <param name="orient">the <see cref="LabelOrientation"/> declared for the element</param>
    /// <param name="angle">the angle of the segment of the route where the element is attached</param>
    /// <returns>the intended angle for the element</returns>
    /// <remarks>
    /// This method is not called unless the <see cref="GetOrientation"/> attached property value is
    /// not <see cref="LabelOrientation.None"/>.
    /// </remarks>
    protected virtual double ComputeAngle(UIElement elt, LabelOrientation orient, double angle) {
      double a;
      switch (orient) {
        default:
        case LabelOrientation.None: a = 0; break;
        case LabelOrientation.Along: a = angle; break;
        case LabelOrientation.Plus90: a = angle + 90; break;
        case LabelOrientation.Minus90: a = angle - 90; break;
        case LabelOrientation.Opposite: a = angle + 180; break;
        case LabelOrientation.Upright:  // like Along
          a = Geo.NormalizeAngle(angle);
          if (a > 90 && a < 270) a -= 180;  // make sure never upside-down
          break;
        case LabelOrientation.Plus90Upright:  // like Plus90
          a = Geo.NormalizeAngle(angle + 90);
          if (a > 90 && a < 270) a -= 180;  // make sure never upside-down
          break;
        case LabelOrientation.Minus90Upright:  // like Minus90
          a = Geo.NormalizeAngle(angle - 90);
          if (a > 90 && a < 270) a -= 180;  // make sure never upside-down
          break;
        case LabelOrientation.Upright45:  // like Along
          a = Geo.NormalizeAngle(angle);
          if (a > 45 && a < 135) return 0;  // make sure never angled too much
          if (a > 225 && a < 315) return  0;
          if (a > 90 && a < 270) a -= 180;  // make sure never upside-down
          break;
      }
      return Geo.NormalizeAngle(a);
    }

    /// <summary>
    /// Compute the offset to use in determining the position of a label.
    /// </summary>
    /// <param name="elt">the label being positioned, either a child element of the <see cref="LinkPanel"/>
    /// or the <see cref="Part.VisualElement"/> of a label <see cref="Node"/></param>
    /// <param name="index">the segment of the link, or a negative value if the label should be at the <see cref="Route.MidPoint"/></param>
    /// <param name="segangle">the angle of the <paramref name="index"/>th segment, or the <see cref="Route.MidAngle"/></param>
    /// <param name="sz">the size of the label</param>
    /// <param name="labelangle">the angle at which the label is rotated, from calling <see cref="ComputeAngle"/></param>
    /// <returns>an offset that is rotated according to the angle of the segment;
    /// this will be added to the midpoint of the link or to the fractional point along the segment</returns>
    protected virtual Point ComputeOffset(UIElement elt, int index, double segangle, Size sz, double labelangle) {
      Point off = GetOffset(elt);
      Spot align = GetAlignment(elt);
      if (align.IsNoSpot) align = Spot.Center;
      Point p = align.PointInRect(new Rect(0, 0, sz.Width, sz.Height));
      if (Double.IsNaN(off.X)) {
        if (index >= 0) {
          off.X = p.X + 3;
        } else {
          off.X = -(p.X + 3);
        }
      }
      if (Double.IsNaN(off.Y)) {
        //?? make this smarter to handle short end segments, especially if Orthogonal
        off.Y = -(p.Y + 3);
      }
      //?? make this better for larger non-square/circular labels that are not rotated
      return Geo.RotatePoint(off, segangle);
    }

    private double ComputeFraction(double f) {
      if (Double.IsNaN(f)) return 0;
      if (f < 0) return 0;
      if (f > 1) return 1;
      return f;
    }

    /// <summary>
    /// Determine the size of the union of the bounds of the positioned and rotated child elements.
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns></returns>
    protected override Size MeasureOverride(Size availableSize) {
      Part part = Diagram.FindAncestor<Part>(this);
      if (part == null) return new Size();

      if (!part.IsMeasuringArranging) return new Size();

      Link link = part as Link;
      if (link == null) {
        Adornment ad = part as Adornment;
        if (ad != null) link = ad.AdornedPart as Link;
        if (link == null) return new Size();
      }

      //Diagram.Debug(" LinkPanelM- " + (link.Data != null ? link.Data.ToString() : ""));

      Shape stroke = this.Path;  // may be null
      link.Path = stroke;  // the Link caches what the Path really is

      Route route = link.Route;
      Rect routeBounds = route.RouteBounds;  // in model coordinates
      Rect linkBounds = routeBounds;    // includes all labels
      childrenBounds = new List<Rect>();  // in local coordinates
      if (stroke != null) {
        stroke.Measure(Geo.Unlimited);
        Size sz = stroke.DesiredSize;
        linkBounds.Width = Math.Max(linkBounds.Width, sz.Width);
        linkBounds.Height = Math.Max(linkBounds.Height, sz.Height);
        childrenBounds.Add(new Rect(0, 0, linkBounds.Width, linkBounds.Height));
      }

      IList<Point> pts = (List<Point>)route.Points;
      int nPoints = pts.Count;
      foreach (UIElement e in this.Children) {
        if (e == stroke) continue;  // already measured the stroke, above
        e.Measure(Geo.Unlimited);
        //if (e.GetType().Name.Contains("Expander")) Diagram.Debug(e.ToString() + " measured: " + Diagram.Str(e.DesiredSize));
        if (nPoints < 2) continue;
        Size sz = e.DesiredSize;
        int index = GetIndex(e);
        double frac = ComputeFraction(GetFraction(e));
        Spot align = GetAlignment(e);
        if (align.IsNoSpot) align = Spot.Center;
        LabelOrientation orient = GetOrientation(e);
        Point eltpt;  // local coordinates
        if (index < -nPoints || index >= nPoints) {  // beyond range? assume at the MidPoint, with the MidAngle
          Point mid = route.MidPoint;
          if (this.Implementation == LinkPanelImplementation.Stretch) {
            Point p0 = pts[0];
            Point pn = pts[nPoints-1];
            sz.Width = Math.Sqrt(Geo.DistanceSquared(pn, p0));
          }
          double segangle = route.MidAngle;
          // maybe rotate the label
          double labelangle = 0;
          if (orient != LabelOrientation.None) {
            labelangle = ComputeAngle(e, orient, segangle);
            link.SetAngle(e, labelangle, align);
          }
          // maybe the alignment point is away from the line
          eltpt = new Point(mid.X - routeBounds.X, mid.Y - routeBounds.Y);  // local coordinates
          Point offset = ComputeOffset(e, index, segangle, sz, labelangle);
          eltpt = Geo.Add(eltpt, offset);
        } else {  // on a particular segment, given by Index, at a point given by Fraction
          // negative index means start from last point, going "backwards"
          Point a, b;
          if (index >= 0) {
            a = pts[index];
            b = (index < nPoints-1) ? pts[index+1] : a;
          } else {
            int i = nPoints + index;  // remember that index is negative here
            a = pts[i];
            b = (i > 0) ? pts[i-1] : a;
          }
          // compute the fractional point along the line, in local coordinates
          eltpt = new Point(a.X + (b.X-a.X)*frac - routeBounds.X, a.Y + (b.Y-a.Y)*frac - routeBounds.Y);
          double segangle = (index >= 0 ? Geo.GetAngle(a, b) : Geo.GetAngle(b, a));
          // maybe rotate the label
          double labelangle = 0;
          if (orient != LabelOrientation.None) {
            labelangle = ComputeAngle(e, orient, segangle);
            link.SetAngle(e, labelangle, align);
          }
          // maybe the alignment point is away from the line
          Point offset = ComputeOffset(e, index, segangle, sz, labelangle);
          eltpt = Geo.Add(eltpt, offset);
        }
        Rect cb = align.RectForPoint(eltpt, sz);
        childrenBounds.Add(cb);  // local coordinates
        linkBounds.Union(new Rect(cb.X+routeBounds.X, cb.Y+routeBounds.Y, cb.Width, cb.Height));  // model coordinates
      }

      // if this panel is the "whole" link, update the link's Bounds
      if (link.VisualElement == this) {
        //Diagram.Debug(" LinkPanelM+ " + (link.Data != null ? link.Data.ToString() : "") + " " + Diagram.Str(routeBounds) + Diagram.Str(linkBounds));
        link.Bounds = new Rect(routeBounds.X, routeBounds.Y, linkBounds.Width, linkBounds.Height);
      }

      return new Size(routeBounds.Width, routeBounds.Height);
    }

    // to avoid recomputing the intended bounds for Arrange, remember them here during Measure
    private List<Rect> childrenBounds = new List<Rect>();

    /// <summary>
    /// Arrange the child elements of this panel and of any label nodes.
    /// </summary>
    /// <param name="finalSize"></param>
    /// <returns></returns>
    /// <remarks>
    /// This positions each child element based on the attached property values of
    /// <see cref="GetIndex"/>, <see cref="GetFraction"/>, <see cref="GetAlignment"/>,
    /// and <see cref="GetOrientation"/>.
    /// This also positions any <see cref="Link.LabelNode"/>,
    /// using the <see cref="LinkPanel"/> attached properties on the node's <see cref="Part.VisualElement"/>,
    /// even though such a node is not within the visual tree of this panel.
    /// If the label node is a <see cref="Group"/>, this will instead position all of the
    /// group's <see cref="Group.MemberNodes"/>.
    /// </remarks>
    protected override Size ArrangeOverride(Size finalSize) {
      Part part = Diagram.FindAncestor<Part>(this);
      if (part == null) return new Size();

      Link link = part as Link;
      if (link == null) {
        Adornment node = part as Adornment;
        if (node != null) link = node.AdornedPart as Link;
      }

      if (link != null) {
        //Diagram.Debug(" LinkPanelA- " + (link.Data != null ? link.Data.ToString() : ""));

        Shape stroke = this.Path;  // may be null
        Route route = link.Route;
        Rect routeBounds = route.RouteBounds;
        IList<Point> pts = (List<Point>)route.Points;
        int nPoints = pts.Count;
        int childidx = 0;
        if (stroke != null) {
          if (childidx < childrenBounds.Count) {
            stroke.Arrange(childrenBounds[childidx++]);
          }
        }
        foreach (UIElement e in this.Children) {
          if (e == stroke) continue;
          if (childidx < childrenBounds.Count) {
            //if (e.GetType().Name.Contains("Expander")) Diagram.Debug(e.ToString() + " arranged: " + Diagram.Str(childrenBounds[childidx]));
            e.Arrange(childrenBounds[childidx++]);
          }
        }

        //Diagram.Debug(" LinkPanelA+ " + (link.Data != null ? link.Data.ToString() : ""));

        Node label = link.LabelNode;
        if (label != null) {
          Group labelgroup = label as Group;
          foreach (Node m in (labelgroup != null ? labelgroup.MemberNodes : new Node[1]{ label })) {
            if (nPoints < 2) continue;
            UIElement e = m.VisualElement;
            Size sz = new Size(m.Bounds.Width, m.Bounds.Height);
            int index = GetIndex(e);
            double frac = ComputeFraction(GetFraction(e));
            Spot align = GetAlignment(e);
            if (align.IsNoSpot) align = Spot.Center;
            LabelOrientation orient = GetOrientation(e);
            Point nodept;  // model coordinates
            if (index < -nPoints || index >= nPoints) {  // beyond range? assume at the MidPoint, with the MidAngle
              Point mid = route.MidPoint;
              double segangle = route.MidAngle;
              // maybe rotate the label
              double labelangle = 0;
              if (orient != LabelOrientation.None) {
                labelangle = ComputeAngle(e, orient, segangle);
                m.SetAngle(e, labelangle, align);
              }
              // maybe the alignment point is away from the line
              nodept = mid;  // model coordinates
              Point offset = ComputeOffset(e, index, segangle, sz, labelangle);
              nodept = Geo.Add(nodept, offset);
            } else {  // on a particular segment, given by Index, at a point given by Fraction
              // negative index means start from last point, going "backwards"
              Point a, b;
              if (index >= 0) {
                a = pts[index];
                b = (index < nPoints-1) ? pts[index+1] : a;
              } else {
                int i = nPoints + index;  // remember that index is negative here
                a = pts[i];
                b = (i > 0) ? pts[i-1] : a;
              }
              // compute the fractional point along the line, in model coordinates
              nodept = new Point(a.X + (b.X-a.X)*frac, a.Y + (b.Y-a.Y)*frac);
              double segangle = (index >= 0 ? Geo.GetAngle(a, b) : Geo.GetAngle(b, a));
              // maybe rotate the label
              double labelangle = 0;
              if (orient != LabelOrientation.None) {
                labelangle = ComputeAngle(e, orient, segangle);
                m.SetAngle(e, labelangle, align);
              }
              // maybe the alignment point is away from the line
              Point offset = ComputeOffset(e, index, segangle, sz, labelangle);
              nodept = Geo.Add(nodept, offset);
            }
            Rect hb = align.RectForPoint(nodept, sz);
            m.Position = new Point(hb.X, hb.Y);
          }
        }
      }

      return finalSize;
    }
  }


  /// <summary>
  /// This enumeration governs if and how a <see cref="LinkPanel"/>'s child element
  /// or label node is rotated to be oriented along the <see cref="Route"/> of a <see cref="Link"/>.
  /// </summary>
  /// <remarks>
  /// These values are supplied by the <see cref="LinkPanel.GetOrientation"/> attached
  /// property on each link panel child element or label node.
  /// You can also override <see cref="LinkPanel.ComputeAngle"/> for more flexibility
  /// in controlling the angle of an element.
  /// </remarks>
  public enum LabelOrientation {
    /// <summary>
    /// The element is never rotated -- the angle is always zero.
    /// </summary>
    None,
    /// <summary>
    /// The element's angle is always the same as the angle of the link's route
    /// at the segment where the element is attached.
    /// </summary>
    /// <remarks>
    /// The element is turned to have the same angle as the route.
    /// Use this orientation for arrow heads.
    /// </remarks>
    Along,
    /// <summary>
    /// The element's angle is always 90 degrees more than the angle of the link's route
    /// at the segment where the element is attached.
    /// </summary>
    /// <remarks>
    /// The element is turned clockwise to be perpendicular to the route.
    /// </remarks>
    Plus90,
    /// <summary>
    /// The element's angle is always 90 degrees less than the angle of the link's route
    /// at the segment where the element is attached.
    /// </summary>
    /// <remarks>
    /// The element is turned counter-clockwise to be perpendicular to the route.
    /// </remarks>
    Minus90,
    /// <summary>
    /// The element's angle is always 180 degrees opposite from the angle of the link's route
    /// at the segment where the element is attached.
    /// </summary>
    Opposite,
    /// <summary>
    /// The element's angle always follows the angle of the link's route
    /// at the segment where the element is attached, but is never upside down.
    /// </summary>
    /// <remarks>
    /// The element is turned to have the same angle as the route, just like <see cref="Along"/>.
    /// This is typically only used for elements that contain text.
    /// </remarks>
    Upright,
    /// <summary>
    /// The element's angle is always 90 degrees more than the angle of the link's route
    /// at the segment where the element is attached, but is never upside down.
    /// </summary>
    /// <remarks>
    /// The element is turned clockwise to be perpendicular to the route, just like <see cref="Plus90"/>.
    /// This is typically only used for elements that contain text.
    /// </remarks>
    Plus90Upright,
    /// <summary>
    /// The element's angle is always 90 degrees less than the angle of the link's route
    /// at the segment where the element is attached, but is never upside down.
    /// </summary>
    /// <remarks>
    /// The element is turned counter-clockwise to be perpendicular to the route, just like <see cref="Minus90"/>.
    /// This is typically only used for elements that contain text.
    /// </remarks>
    Minus90Upright,
    /// <summary>
    /// The element's angle always follows the angle of the link's route
    /// at the segment where the element is attached, but is never upside down
    /// and is never angled more than +/- 45 degrees.
    /// </summary>
    /// <remarks>
    /// When the route's angle is within 45 degrees of vertical (90 or 270 degrees),
    /// the element's angle is set to zero.
    /// This is typically only used for elements that contain text.
    /// </remarks>
    Upright45,
  }


  /// <summary>
  /// This enumeration describes the kinds of principal elements a
  /// <see cref="LinkPanel"/> uses to render the route of a <see cref="Link"/>.
  /// </summary>
  /// <remarks>
  /// This is the type of the <see cref="LinkPanel.Implementation"/> property.
  /// </remarks>
  public enum LinkPanelImplementation {
    /// <summary>
    /// The normal case where the route is shown as a stroked open <c>Shape</c> that
    /// is a possibly multi-segment or curved line.
    /// </summary>
    /// <remarks>
    /// For this case the value of <see cref="LinkPanel.Path"/>
    /// will be a <c>LinkShape</c> (WPF) or a <c>Path</c> (Silverlight).
    /// </remarks>
    Path,
    /// <summary>
    /// The link is shown as any rotated fixed-size <c>UIElement</c>.
    /// </summary>
    Fixed,
    /// <summary>
    /// The link is shown as any rotated <c>UIElement</c>, but its width is arranged
    /// to be stretched to reach the distance between the end points of the route.
    /// </summary>
    Stretch,
    //?? Scale  // stretches width and scales height to maintain aspect ratio
  }
}
