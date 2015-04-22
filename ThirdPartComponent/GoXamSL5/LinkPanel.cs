
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
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

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
  /// To make it easy to implement common arrowheads, you can set the <see cref="LinkPanel.ToArrowProperty"/>
  /// and/or <see cref="LinkPanel.FromArrowProperty"/> attached properties on a <c>Path</c> element.
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
  ///       &lt;go:LinkShape x:Name="Path" Stroke="Black" StrokeThickness="1" /&gt;
  ///       &lt;Path Fill="Black" go:LinkPanel.ToArrow="Standard" /&gt;
  ///     &lt;/go:LinkPanel&gt;
  ///   &lt;/DataTemplate&gt;
  /// </code>
  /// In Silverlight:
  /// <code>
  ///   &lt;DataTemplate x:Key="ExampleLinkTemplate"&gt;
  ///     &lt;go:LinkPanel go:Link.SelectionElementName="Path" go:Link.SelectionAdorned="True" &gt;
  ///       &lt;Path go:LinkPanel.IsLinkShape="True" x:Name="Path" Stroke="Black" StrokeThickness="1" /&gt;
  ///       &lt;Path Fill="Black" go:LinkPanel.ToArrow="Standard" /&gt;
  ///     &lt;/go:LinkPanel&gt;
  ///   &lt;/DataTemplate&gt;
  /// </code>
  /// </para>
  /// <para>
  /// A <c>LinkPanel</c> may have multiple children (<c>LinkShape</c> (WPF) or <c>Path</c> (Silverlight))
  /// for which <c>go:LinkPanel.IsLinkShape="True"</c>.
  /// One of them must be the primary link path, with the <c>x:Name</c> of "Path";
  /// this is what will be returned by the <see cref="Path"/> property.
  /// All of rest will get the same route geometry as the primary <see cref="Path"/>.
  /// This makes it easy to implement gradient-like effects by using multiple link shapes
  /// of different colors:
  /// <code>
  ///   &lt;go:LinkPanel . . .&gt;
  ///     &lt;go:LinkShape StrokeThickness="7" Stroke="DarkBlue" /&gt;
  ///     &lt;go:LinkShape StrokeThickness="5" Stroke="Blue" /&gt;
  ///     &lt;go:LinkShape StrokeThickness="3" Stroke="LightBlue" /&gt;
  ///     &lt;go:LinkShape x:Name="Path" StrokeThickness="1" Stroke="White" /&gt;
  ///     . . .
  ///   &lt;/go:LinkPanel&gt;
  /// </code>
  /// In Silverlight:
  /// <code>
  ///   &lt;go:LinkPanel . . .&gt;
  ///     &lt;Path go:LinkPanel.IsLinkShape="True" StrokeThickness="7" Stroke="DarkBlue" /&gt;
  ///     &lt;Path go:LinkPanel.IsLinkShape="True" StrokeThickness="5" Stroke="Blue" /&gt;
  ///     &lt;Path go:LinkPanel.IsLinkShape="True" StrokeThickness="3" Stroke="LightBlue" /&gt;
  ///     &lt;Path go:LinkPanel.IsLinkShape="True" x:Name="Path" StrokeThickness="1" Stroke="White" /&gt;
  ///     . . .
  ///   &lt;/go:LinkPanel&gt;
  /// </code>
  /// </para>
  /// <para>
  /// By default each LinkPanel sets <c>UseLayoutRounding</c> to false,
  /// except that it is set to true when used inside an <see cref="Adornment"/>.
  /// </para>
  /// </remarks>
  public class LinkPanel : Panel {
    /// <summary>
    /// Construct an empty <see cref="LinkPanel"/>.
    /// </summary>
    /// <remarks>
    /// This sets the <see cref="Part.SelectionElementNameProperty"/> to "Path"
    /// and <c>UseLayoutRounding</c> to false.
    /// </remarks>
    public LinkPanel() {
      Part.SetSelectionElementName(this, "Path");

      this.UseLayoutRounding = false;

    }

    static LinkPanel() {
      ImplementationProperty = DependencyProperty.Register("Implementation", typeof(LinkPanelImplementation), typeof(LinkPanel),
        new FrameworkPropertyMetadata(LinkPanelImplementation.Path));

      IsLinkShapeProperty = DependencyProperty.RegisterAttached("IsLinkShape", typeof(bool), typeof(LinkPanel),
        new FrameworkPropertyMetadata(false));
      IndexProperty = DependencyProperty.RegisterAttached("Index", typeof(int), typeof(LinkPanel),
        new FrameworkPropertyMetadata(int.MinValue, OnChangedAttachedProperty));
      FractionProperty = DependencyProperty.RegisterAttached("Fraction", typeof(double), typeof(LinkPanel),
        new FrameworkPropertyMetadata(0.0, OnChangedAttachedProperty));
      OffsetProperty = DependencyProperty.RegisterAttached("Offset", typeof(Point), typeof(LinkPanel),
        new FrameworkPropertyMetadata(new Point(0, 0), OnChangedAttachedProperty));
      AlignmentProperty = DependencyProperty.RegisterAttached("Alignment", typeof(Spot), typeof(LinkPanel),
        new FrameworkPropertyMetadata(Spot.Center, OnChangedAttachedProperty));
      OrientationProperty = DependencyProperty.RegisterAttached("Orientation", typeof(LabelOrientation), typeof(LinkPanel),
        new FrameworkPropertyMetadata(LabelOrientation.None, OnChangedAttachedProperty));

      ToArrowProperty = DependencyProperty.RegisterAttached("ToArrow", typeof(Arrowhead), typeof(LinkPanel),
        new FrameworkPropertyMetadata(Arrowhead.None, OnToArrowChanged));
      ToArrowScaleProperty = DependencyProperty.RegisterAttached("ToArrowScale", typeof(double), typeof(LinkPanel),
        new FrameworkPropertyMetadata(1.0, OnToArrowScaleChanged));
      FromArrowProperty = DependencyProperty.RegisterAttached("FromArrow", typeof(Arrowhead), typeof(LinkPanel),
        new FrameworkPropertyMetadata(Arrowhead.None, OnFromArrowChanged));
      FromArrowScaleProperty = DependencyProperty.RegisterAttached("FromArrowScale", typeof(double), typeof(LinkPanel),
        new FrameworkPropertyMetadata(1.0, OnFromArrowScaleChanged));
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

              this.UseLayoutRounding = true;

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
    /// Identifies the <c>ToArrow</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty ToArrowProperty;
    /// <summary>
    /// Gets the Arrowhead on a Path, at the ToNode end.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <returns>
    /// The default value is <see cref="Arrowhead.None"/>, meaning there is no Arrowhead on the element.
    /// </returns>
    public static Arrowhead GetToArrow(DependencyObject d) { return (Arrowhead)d.GetValue(ToArrowProperty); }
    /// <summary>
    /// Sets the Arrowhead on a Path, at the ToNode end.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <param name="v">the constant of the Arrowhead enum to which ToArrowProperty is being set.</param>
    /// <remarks>
    /// The ToArrowProperty can only be set on a <c>Path</c> which is a child of a <see cref="LinkPanel"/>.
    /// If Silverlight, the attached propery <see cref="LinkPanel.IsLinkShapeProperty"/> must be false, as it is by default.
    /// Setting the ToArrowProperty will set the <c>Path.Data</c>, <see cref="LinkPanel.OrientationProperty"/>,
    /// <see cref="LinkPanel.AlignmentProperty"/>, and <see cref="LinkPanel.IndexProperty"/> attached properties.
    /// Consequently, setting both the ToArrowProperty and FromArrowProperty on the same path will result
    /// in the appearance of only the second property that is set.
    /// Setting the ToArrowProperty on multiple Paths that are children of the same LinkPanel will result in overlap of the
    /// Path.Data geometries.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Throws an Exception if ToArrowProperty is set on an element 
    /// other than a <c>Path</c>.
    /// </exception>
    public static void SetToArrow(DependencyObject d, Arrowhead v) { d.SetValue(ToArrowProperty, v); }
    private static void OnToArrowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Path path = d as Path;
      if (path == null) {
        Diagram.Error("ToArrow can only be set on a Path.");
      } else {
        SetArrowheadPath(path, (Arrowhead)e.NewValue, true);
      }
    }

    /// <summary>
    /// Identifies the <c>ToArrowScale</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty ToArrowScaleProperty;
    /// <summary>
    /// Gets the size of the Arrowhead on an element, at the ToNode end, as a scale multiple of its base size.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <returns>
    /// The default value is 1.0, indicating the base size defined in the xaml.
    /// </returns>
    public static double GetToArrowScale(DependencyObject d) { return (double)d.GetValue(ToArrowScaleProperty); }
    /// <summary>
    /// Sets the size of the <see cref="Arrowhead"/> on an element, at the ToNode end, as a scale multiple of its base size.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <param name="v">the scale factor. Acceptable values are real numbers between zero and infinity.</param>
    /// <remarks>
    /// Setting this property will have no effect if set on a <c>Path</c> without the <see cref="ToArrowProperty"/> set.
    /// It will also have no effect if the <see cref="LinkPanel.IndexProperty"/> is out of range.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Throws an exception if ToArrowScaleProperty is set to a negative number.
    /// </exception>
    public static void SetToArrowScale(DependencyObject d, double v) { d.SetValue(ToArrowScaleProperty, v); }
    private static void OnToArrowScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Path path = d as Path;
      if (path == null)
        Diagram.Error("ToArrowScale should only be set on a Path.");
      double newvalue = (double)e.NewValue;
      if (newvalue < 0 || Double.IsInfinity(newvalue) || Double.IsNaN(newvalue)) {
        d.SetValue(ToArrowScaleProperty, 1.0);
      } else {
        Part part = Part.FindAncestor<Part>(path);
        if (part != null)
          part.InvalidateVisual(path);
      }
    }

    /// <summary>
    /// Identifies the <c>FromArrow</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty FromArrowProperty;
    /// <summary>
    /// Gets the Arrowhead on a Path, at the FromNode end.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <returns>
    /// The default value is <see cref="Arrowhead.None"/>, meaning there is no Arrowhead on the element.
    /// </returns>
    public static Arrowhead GetFromArrow(DependencyObject d) { return (Arrowhead)d.GetValue(FromArrowProperty); }
    /// <summary>
    /// Sets the Arrowhead on a Path, at the FromNode end.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <param name="v">the constant of the Arrowhead enum to which FromArrowProperty is being set.</param>
    /// <remarks>
    /// The FromArrowProperty can only be set on a <c>Path</c> which is a child of a <see cref="LinkPanel"/>.
    /// If Silverlight, the attached propery <see cref="LinkPanel.IsLinkShapeProperty"/> must be false, as it is by default.
    /// Setting the FromArrowProperty will set the <c>Path.Data</c>, <see cref="LinkPanel.OrientationProperty"/>, 
    /// <see cref="LinkPanel.AlignmentProperty"/>, and <see cref="LinkPanel.IndexProperty"/> attached properties.
    /// Consequently, setting both the ToArrowProperty and FromArrowProperty on the same path will result
    /// in the appearance of only the second set property.
    /// Setting the FromArrowProperty on multiple Paths that are children of the same LinkPanel will result in overlap of the
    /// Path.Data geometries.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Throws an Exception if FromArrowProperty is set on an element 
    /// other than a <c>Path</c>.
    /// </exception>
    public static void SetFromArrow(DependencyObject d, Arrowhead v) { d.SetValue(FromArrowProperty, v); }
    private static void OnFromArrowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Path path = d as Path;
      if (path == null) {
        Diagram.Error("FromArrow can only be set on a Path.");
      } else {
        SetArrowheadPath(path, (Arrowhead)e.NewValue, false);
      }
    }

    /// <summary>
    /// Identifies the <c>FromArrowScale</c> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty FromArrowScaleProperty;
    /// <summary>
    /// Gets the size of the Arrowhead on an element, at the FromNode end, as a scale multiple of its base size.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <returns>
    /// The default value is 1.0, indicating the base size defined in Generic.xaml.
    /// </returns>
    public static double GetFromArrowScale(DependencyObject d) { return (double)d.GetValue(FromArrowScaleProperty); }
    /// <summary>
    /// Sets the size of the <see cref="Arrowhead"/> on an element at the FromNode end as a scale multiple of its base size.
    /// </summary>
    /// <param name="d">a child element of a <see cref="LinkPanel"/></param>
    /// <param name="v">the scale factor. Acceptable values are real numbers between zero and infinity.</param>
    /// <remarks>
    /// Setting this property will have no effect if set on a <c>Path</c> without the <see cref="FromArrowProperty"/> set.
    /// It will also have no effect if the <see cref="LinkPanel.IndexProperty"/> is out of range.
    /// </remarks>
    /// /// <exception cref="InvalidOperationException">
    /// Throws an exception if FromArrowScaleProperty is set to a negative number.
    /// </exception>
    public static void SetFromArrowScale(DependencyObject d, double v) { d.SetValue(FromArrowScaleProperty, v); }
    private static void OnFromArrowScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Path path = d as Path;
      if (path == null)
        Diagram.Error("FromArrowScale should only be set on a Path.");
      double newvalue = (double)e.NewValue;
      if (newvalue < 0 || Double.IsInfinity(newvalue) || Double.IsNaN(newvalue)) {
        d.SetValue(FromArrowScaleProperty, 1.0);
      } else {
        Part part = Part.FindAncestor<Part>(path);
        if (part != null)
          part.InvalidateVisual(path);
      }
    }


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
          if (a > 225 && a < 315) return 0;
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

      //Shift offset to account for Arrowhead
      if (elt is Path) {
        Path path = elt as Path;
        double strokeThickness = path.StrokeThickness;
        double arrowScale = 0;
        if (strokeThickness != 0) {

          if (GetToArrow(path) != Arrowhead.None) {
            arrowScale = GetToArrowScale(path);
          } else {
            if (GetFromArrow(path) != Arrowhead.None) {
              arrowScale = GetFromArrowScale(path);
            }
          }

          if (arrowScale != 0) {
            //these offsets are not exact, only approximations
            off.Y += 0.5 * arrowScale * ((strokeThickness >= 1) ? 1 : 0);
            off.X += 0.5 * arrowScale * strokeThickness;
          }
        }
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
      Rect linkBounds = new Rect(0, 0, routeBounds.Width, routeBounds.Height);  // will include all decorations
      childrenBounds = new List<Rect>();  // in local coordinates
      if (stroke != null) {
        stroke.Measure(Geo.Unlimited);
        Size sz = stroke.DesiredSize;
        double thick = stroke.StrokeThickness;
        linkBounds.Union(new Rect(-thick/2, -thick/2, sz.Width, sz.Height));  // local coordinates
        childrenBounds.Add(new Rect(0, 0, linkBounds.Width, linkBounds.Height));
      }

      IList<Point> pts = (List<Point>)route.Points;
      int nPoints = pts.Count;
      foreach (UIElement e in this.Children) {
        if (e == stroke) continue;  // already measured the stroke, above

        Path otherpath = e as Path;
        if (otherpath != null && !GetIsLinkShape(e)) otherpath = null;
        if (otherpath != null) {  // IsLinkShape="True"
          otherpath.Data = Adornment.Copy(route.DefiningGeometry);
        }






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
            Arrowhead toArrow = GetToArrow(e);
            Arrowhead fromArrow = GetFromArrow(e);
            if (toArrow == Arrowhead.None && fromArrow == Arrowhead.None) {
              link.SetAngle(e, labelangle, align);
            } else {
              double arrowScale = 0.0;
              if (toArrow != Arrowhead.None)
                arrowScale = GetToArrowScale(e);
              else
                arrowScale = GetFromArrowScale(e);
              link.SetAngleAndScale(e, labelangle, align, new Size(arrowScale, arrowScale));
            }
          }
          // maybe the alignment point is away from the line
          Point offset = ComputeOffset(e, index, segangle, sz, labelangle);
          eltpt = Geo.Add(eltpt, offset);
        }
        if (otherpath != null) {  // IsLinkShape="True"
          double thick = otherpath.StrokeThickness;
          linkBounds.Union(new Rect(-thick/2, -thick/2, sz.Width, sz.Height));  // local coordinates
          childrenBounds.Add(new Rect(0, 0, linkBounds.Width, linkBounds.Height));
        } else {
          Rect cb = align.RectForPoint(eltpt, sz);
          childrenBounds.Add(cb);  // local coordinates
          linkBounds.Union(new Rect(cb.X, cb.Y, cb.Width, cb.Height));  // local coordinates
        }
      }

      // if this panel is the "whole" link, update the link's Bounds
      if (link.VisualElement == this) {
        link.Bounds = new Rect(routeBounds.X, routeBounds.Y, linkBounds.Width, linkBounds.Height);  // convert to model coordinates
      }

      //Diagram.Debug(" LinkPanelM+ " + (link.Data != null ? link.Data.ToString() : "") + " " + Diagram.Str(routeBounds) + Diagram.Str(linkBounds));
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
            Rect cb = childrenBounds[childidx++];
            //if (e.GetType().Name.Contains("Expander")) Diagram.Debug(e.ToString() + " arranged: " + Diagram.Str(cb));
            e.Arrange(cb);
          }
        }

        //Diagram.Debug(" LinkPanelA+ " + (link.Data != null ? link.Data.ToString() : ""));

        Node label = link.LabelNode;
        if (label != null) {
          Group labelgroup = label as Group;
          foreach (Node m in (labelgroup != null ? labelgroup.MemberNodes : new Node[1] { label })) {
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


    internal static void SetArrowheadPath(Path path, Arrowhead currentArrow, bool to) {
      DataTemplate arrowheadTemplate;
      Path path1 = null;

      arrowheadTemplate = Diagram.FindDefault<DataTemplate>(currentArrow.ToString());
      if (arrowheadTemplate != null)
        path1 = arrowheadTemplate.LoadContent() as Path;

      if (path1 != null) {
        Geometry geom = path1.Data as Geometry;
        path1.Data = null; //Silverlight does not allow the sharing of Geometries
        if (geom != null) {
          path.Data = geom;
          LinkPanel.SetOrientation(path, LabelOrientation.Along);
          Spot alignmentSpot = LinkPanel.GetToArrowAlignmentSpot(currentArrow);
          if (to) {
            LinkPanel.SetIndex(path, -1);
            LinkPanel.SetAlignment(path, alignmentSpot);
          } else {
            LinkPanel.SetIndex(path, 0);
            LinkPanel.SetAlignment(path, new Spot(1-alignmentSpot.X, alignmentSpot.Y));  //Opposite horizontal alignment in Spot
          }
          Part part = Part.FindAncestor<Part>(path);
          if (part != null)
            part.InvalidateVisual(path);
        }
      }
    }

    private static Spot GetToArrowAlignmentSpot(Arrowhead arrow) {
      switch (arrow) {
        case Arrowhead.HalfArrowTop: return Spot.BottomRight;
        case Arrowhead.HalfTriangleTop: return Spot.BottomRight;
        case Arrowhead.OpenRightTriangleTop: return Spot.BottomRight;
        case Arrowhead.OpenTriangleTop: return Spot.BottomRight;
        default: return Spot.MiddleRight;
      }
    }

    /// <summary>
    /// Returns whether an Arrowhead should be filled.
    /// </summary>
    /// <param name="arrow">The Arrowhead which should or should not be filled.</param>
    /// <returns>True if the Arrowhead should be filled.</returns>
    public static bool IsFilled(Arrowhead arrow) {
      int arrowInt = (int)arrow;
      if (arrowInt < 0  ||  arrowInt > IsFilledArray.Length) return false;
      return IsFilledArray[arrowInt];
    }

    private static bool[] IsFilledArray  = new bool[]{    
      false,  //Arrowhead.None
      true,   //Arrowhead.Triangle
      true,   //DoubleTriangle
      true,   //RoundedTriangle
      true,   //PartialDoubleTriangle
      true,   //Boomerang
      true,   //BackwardTriangle
      true,   //HalfTriangleTop
      true,   //HalfTriangleBottom
      false,  //OpenTriangle
      false,  //BackwardOpenTriangle
      false,  //DoubleFeathers
      false,  //TripleFeathers
      false,  //OpenTriangleTop
      false,  //OpenTriangleBottom
      false,  //OpenTriangleLine
      false,  //Line
      false,  //DoubleLine
      false,  //TripleLine
      false,  //ForwardSlash
      false,  //BackSlash
      false,  //DoubleForwardSlash
      false,  //DoubleBackSlash
      false,  //TripleForwardSlash
      false,  //TripleBackSlash
      false,  //X
      false,  //Fork
      false,  //OpenRightTriangleTop
      false,  //OpenRightTriangleBottom
      true,   //Block
      true,   //Kite
      true,   //Diamond
      true,   //StretchedDiamond
      true,   //Standard
      true,   //FastForward
      true,   //AccelerationArrow
      true,   //BoxArrow
      true,   //StretchedChevron
      true,   //SimpleArrow
      true,   //BigEndArrow
      true,   //SidewaysV
      true,   //HalfArrowTop
      true,   //HalfArrowBottom
      true,   //Chevron
      true,   //OpposingDirectionDoubleArrow
      false,  //EquilibriumArrow
      true,   //CircleEndedArrow
      true,   //ConcaveTailArrow
      true,   //DynamicWidthArrow
      true,   //TailedNormalArrow
      true,   //PentagonArrow
      true,   //NormalArrow
      true,   //Circle
      false,  //ForwardSemiCircle
      false,  //BackwardSemiCircle
      true,   //PlusCircle
      true,   //LineCircle
      true,   //DoubleLineCircle
      true,   //TripleLineCircle
      true,   //CircleFork
      false,  //LineFork
      true,   //CircleLine
      true,   //CircleLineFork
      true,   //DiamondCircle
      true,   //TriangleLine
      true,   //BackwardCircleFork
      true,   //BackwardCircleLineFork
    };
  }


  /// <summary>
  /// This enumeration governs if and how a <see cref="LinkPanel"/>'s child element
  /// or label node is rotated to be oriented along the <see cref="Route"/> of a <see cref="Link"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// These values are supplied by the <see cref="LinkPanel.GetOrientation"/> attached
  /// property on each link panel child element or label node.
  /// You can also override <see cref="LinkPanel.ComputeAngle"/> for more flexibility
  /// in controlling the angle of an element.
  /// </para>
  /// <para>
  /// The "Upright" values are typically only used when the element contains text.
  /// </para>
  /// </remarks>
  public enum LabelOrientation {
    /// <summary>
    /// The element is never rotated -- the angle is always zero.
    /// </summary>
    None,
    /// <summary>
    /// The element's angle is always the same as the angle of the link's route
    /// at the segment where the element is attached.
    /// The element is turned to have the same angle as the route.
    /// Use this orientation for arrow heads.
    /// </summary>
    Along,
    /// <summary>
    /// The element's angle is always 90 degrees more than the angle of the link's route
    /// at the segment where the element is attached.
    /// The element is turned clockwise to be perpendicular to the route.
    /// </summary>
    Plus90,
    /// <summary>
    /// The element's angle is always 90 degrees less than the angle of the link's route
    /// at the segment where the element is attached.
    /// The element is turned counter-clockwise to be perpendicular to the route.
    /// </summary>
    Minus90,
    /// <summary>
    /// The element's angle is always 180 degrees opposite from the angle of the link's route
    /// at the segment where the element is attached.
    /// </summary>
    Opposite,
    /// <summary>
    /// The element's angle always follows the angle of the link's route
    /// at the segment where the element is attached, but is never upside down.
    /// The element is turned to have the same angle as the route, just like <see cref="Along"/>.
    /// This is typically only used for elements that contain text.
    /// </summary>
    Upright,
    /// <summary>
    /// The element's angle is always 90 degrees more than the angle of the link's route
    /// at the segment where the element is attached, but is never upside down.
    /// The element is turned clockwise to be perpendicular to the route, just like <see cref="Plus90"/>.
    /// This is typically only used for elements that contain text.
    /// </summary>
    Plus90Upright,
    /// <summary>
    /// The element's angle is always 90 degrees less than the angle of the link's route
    /// at the segment where the element is attached, but is never upside down.
    /// The element is turned counter-clockwise to be perpendicular to the route, just like <see cref="Minus90"/>.
    /// This is typically only used for elements that contain text.
    /// </summary>
    Minus90Upright,
    /// <summary>
    /// The element's angle always follows the angle of the link's route
    /// at the segment where the element is attached, but is never upside down
    /// and is never angled more than +/- 45 degrees.
    /// When the route's angle is within 45 degrees of vertical (90 or 270 degrees),
    /// the element's angle is set to zero.
    /// This is typically only used for elements that contain text.
    /// </summary>
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
    /// For this case the value of <see cref="LinkPanel.Path"/>
    /// will be a <c>LinkShape</c> (WPF) or a <c>Path</c> (Silverlight).
    /// </summary>
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





  /// <summary>
  /// Predefined shapes for <c>LinkShape</c> (WPF) or <c>Path</c> inside a <see cref="LinkPanel"/>.
  /// </summary>
  /// <remarks>
  /// Set the <c>LinkPanel.ToArrow</c> or <c>LinkPanel.FromArrow</c> attached properties (or call <c>LinkPanel.SetValue()</c>) providing either dependency property as a parameter.
  /// </remarks>
  public enum Arrowhead {

    /// <summary>
    /// Represents the default Arrowhead DataTemplate, which is no Arrowhead.
    /// </summary>
    None = 0,

    //Closed Triangles
    /// <summary>
    /// Represents an equilateral triangle pointing to the right.
    /// </summary>
    Triangle,
    /// <summary>
    /// Represents two triangles pointing to the right, one after another.
    /// </summary>
    DoubleTriangle,
    /// <summary>
    /// Represents a triangle pointing to the right with rounded corners.
    /// </summary>
    RoundedTriangle,
    /// <summary>
    /// Represents two consecutive triangles with the former being partially obscured by the latter.
    /// </summary>
    PartialDoubleTriangle,
    /// <summary>
    /// Represents a boomerang with sharp vertices.
    /// </summary>
    Boomerang,
    /// <summary>
    /// Represents a triangle pointing backwards. 
    /// </summary>
    BackwardTriangle,
    /// <summary>
    /// Represents the top half of a forward pointing triangle.
    /// </summary>
    HalfTriangleTop,
    /// <summary>
    /// Represents the bottom half of a forward pointing triangle.
    /// </summary>
    HalfTriangleBottom,

    //Open Triangles
    /// <summary>
    /// Represents a triangle pointing to the right with an open back end.
    /// </summary>
    OpenTriangle,
    /// <summary>
    /// Represents a triangle pointing to the left with an open back end.
    /// </summary>
    BackwardOpenTriangle,
    /// <summary>
    /// Represents two triangles pointing to the right, in a line, with an open back ends.
    /// </summary>
    DoubleFeathers,
    /// <summary>
    /// Represents three triangles pointing to the right, in a line, with an open back ends.
    /// </summary>
    TripleFeathers,
    /// <summary>
    /// Represents the top half of an open triangle.
    /// </summary>
    OpenTriangleTop,
    /// <summary>
    /// Represents the bottom half of an open triangle.
    /// </summary>
    OpenTriangleBottom,
    /// <summary>
    /// Represents an open triangle followed by a vertical line.
    /// </summary>
    OpenTriangleLine,

    //Lines
    /// <summary>
    /// Represents a vertical line.
    /// </summary>
    Line,
    /// <summary>
    /// Represents two vertical lines.
    /// </summary>
    DoubleLine,
    /// <summary>
    /// Represents three vertical lines.
    /// </summary>
    TripleLine,
    /// <summary>
    /// Represents a forward slash.
    /// </summary>
    ForwardSlash,
    /// <summary>
    /// Represents a backwards slash.
    /// </summary>
    BackSlash,
    /// <summary>
    /// Represents two forward slashes.
    /// </summary>
    DoubleForwardSlash,
    /// <summary>
    /// Represents two backwards slashes.
    /// </summary>
    DoubleBackSlash,
    /// <summary>
    /// Represents three forward slashes.
    /// </summary>
    TripleForwardSlash,
    /// <summary>
    /// Represents three backwards slashes.
    /// </summary>
    TripleBackSlash,
    /// <summary>
    /// Represents to crossed diagonal lines.
    /// </summary>
    X,
    /// <summary>
    /// Represents a horizontal line splitting into three directions.
    /// </summary>
    Fork,
    /// <summary>
    /// Represents the top half of a vertical line.
    /// </summary>
    OpenRightTriangleTop,
    /// <summary>
    /// Represents the bottom half of a vertical line.
    /// </summary>
    OpenRightTriangleBottom,

    //Blocks
    /// <summary>
    /// Represents a square.
    /// </summary>
    Block,
    /// <summary>
    /// Represents a quadrilateral with the long axis parallel to the direction of the link and the longer side to the right.
    /// </summary>
    Kite,
    /// <summary>
    /// Represents a rotated square.
    /// </summary>
    Diamond,
    /// <summary>
    /// Represents a parallelogram with the long axis parallel to the direction of the link.
    /// </summary>
    StretchedDiamond,

    //Arrows
    /// <summary>
    /// Represents a standard arrow.
    /// </summary>
    Standard,
    /// <summary>
    /// Represents two consecutive triangls followed by a vertical line.
    /// </summary>
    FastForward,
    /// <summary>
    /// Represents a line, followed by a thin rectangle, followed by a PentagonArrow.
    /// </summary>
    AccelerationArrow,
    /// <summary>
    /// Represents a rectangle affixed to a perpendicular rectangle, affixed to a triangle pointing to the right.
    /// </summary>
    BoxArrow,
    /// <summary>
    /// Represents a triangle appended to a tall rectangle with a slight triangular indentation on the end.
    /// </summary>
    StretchedChevron,
    /// <summary>
    /// Represents a PentagonArrow connected to a right pointing triangle by a line.
    /// </summary>
    SimpleArrow,
    /// <summary>
    /// Represents a small trianlge appended to and partially covering a large triangle.
    /// </summary>
    BigEndArrow,
    /// <summary>
    /// Represents the letter V rotated 90 degrees counter-clockwise.
    /// </summary>
    SidewaysV,
    /// <summary>
    /// Represents the top half of a Bommerang.
    /// </summary>
    HalfArrowTop,
    /// <summary>
    /// Represents the bottom half of a Boomerang.
    /// </summary>
    HalfArrowBottom,
    /// <summary>
    /// Represents a chevron shape pointing to the right and with straight sides.
    /// </summary>
    Chevron,
    /// <summary>
    /// Represents an arrow with two heads pointing in opposite directions.
    /// </summary>
    OpposingDirectionDoubleArrow,
    /// <summary>
    /// Represents the chemical symbol for an equilibrium reaction.
    /// </summary>
    EquilibriumArrow,
    /// <summary>
    /// Represents an arrow appended to an ellipse.
    /// </summary>
    CircleEndedArrow,
    /// <summary>
    /// Represents an arrow with an indented tail.
    /// </summary>
    ConcaveTailArrow,
    /// <summary>
    /// Represent an arrow with different slopes at different sections.
    /// </summary>
    DynamicWidthArrow,
    /// <summary>
    /// Represents a triangle appended to a long rectangle with a triangular indentation on the end.
    /// </summary>
    TailedNormalArrow,
    /// <summary>
    /// Represents a triangle appended to a tall rectangle.
    /// </summary>
    PentagonArrow,
    /// <summary>
    /// Represents a triangle appended to a long rectangle.
    /// </summary>
    NormalArrow,


    //Circles
    /// <summary>
    /// Represents a circle.
    /// </summary>
    Circle,
    /// <summary>
    /// Represents the forward half of a circle.
    /// </summary>
    ForwardSemiCircle,
    /// <summary>
    /// Represents the backward half of a circle.
    /// </summary>
    BackwardSemiCircle,
    /// <summary>
    /// Represents a circle with a plus in its center.
    /// Will only display properly if Stroke and Fill are different brushes.
    /// </summary>
    PlusCircle,


    //Miscellaneous and Mixed
    /// <summary>
    /// Represents a vertical line followed by a circle.
    /// </summary>
    LineCircle,
    /// <summary>
    /// Represents two vertical lines followed by a circle.
    /// </summary>
    DoubleLineCircle,
    /// <summary>
    /// Represents three vertical lines followed by a circle.
    /// </summary>
    TripleLineCircle,
    /// <summary>
    /// Represents a circle followed by a three-way fork.
    /// </summary>
    CircleFork,
    /// <summary>
    /// Represents a vertical line followed by a three-way fork.
    /// </summary>
    LineFork,
    /// <summary>
    /// Represents a circle followed by a vertical line.
    /// </summary>
    CircleLine,
    /// <summary>
    /// Represents a circle followed by a vertical line followed by a three-way fork.
    /// </summary>
    CircleLineFork,
    /// <summary>
    /// Represents a diamond followed by a circle.
    /// </summary>
    DiamondCircle,
    /// <summary>
    /// Represents a triangle followed by a vertical line.
    /// </summary>
    TriangleLine,
    /// <summary>
    /// Represents a three-way fork followed by a circle.
    /// </summary>
    BackwardCircleFork,
    /// <summary>
    /// Represents a three-way fork followed by a vertical line followed by a circle.
    /// </summary>
    BackwardCircleLineFork,
  }
}
