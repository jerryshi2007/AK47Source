
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
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Northwoods.GoXam {


  /// <summary>
  /// This <c>Shape</c> is typically used as the line, perhaps curved or with multiple segments,
  /// representing a <see cref="Link"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This shape must be either the only element of a <see cref="Link"/> <c>DataTemplate</c> or a child
  /// element of a <see cref="LinkPanel"/> which is itself the root element of a Link.
  /// </para>
  /// <para>
  /// In WPF, you must use the <c>&lt;go:LinkShape&gt;</c> element instead of a <c>&lt;Path&gt;</c> element.
  /// In Silverlight 3, you must use the <c>&lt;Path&gt;</c> element instead of a <c>&lt;go:LinkShape&gt;</c> element.
  /// In Silverlight 4, you may use either the <c>&lt;Path&gt;</c> element or a <c>&lt;go:LinkShape&gt;</c> element.
  /// </para>
  /// </remarks>
  public class LinkShape :

    Path



  {
    /// <summary>
    /// Construct a <see cref="LinkShape"/> that will get its geometry from its <see cref="Link"/>'s <see cref="Route"/>.
    /// </summary>
    /// <remarks>
    /// This sets the <see cref="LinkPanel.IsLinkShapeProperty"/> to true.
    /// </remarks>
    public LinkShape() {
      LinkPanel.SetIsLinkShape(this, true);
    }




























  }


  /// <summary>
  /// A <c>Route</c> is an object associated with a <see cref="Link"/> that
  /// computes and remembers the set of points that the link should follow to
  /// connect two nodes.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The only use of a <see cref="Route"/> in XAML is as the value of the
  /// <see cref="Northwoods.GoXam.Link.Route"/> attached property:
  /// <code>
  ///   &lt;DataTemplate x:Key="MyLinkTemplate"&gt;
  ///     &lt;go:LinkPanel go:Link.SelectionElementName="Path" go:Part.SelectionAdorned="True"&gt;
  ///       &lt;go:Link.Route&gt;
  ///         &lt;go:Route Routing="Orthogonal" Corner="10" RelinkableFrom="True" RelinkableTo="True" /&gt;
  ///       &lt;/go:Link.Route&gt;
  ///       &lt;go:LinkShape x:Name="Path" Stroke="Black" StrokeThickness="1" /&gt;
  ///       &lt;Path Fill="Black" go:LinkPanel.ToArrow="Standard" /&gt;
  ///     &lt;/go:LinkPanel&gt;
  ///   &lt;/DataTemplate&gt;
  /// </code>
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
  public class Route : FrameworkElement {  //?? if DependencyObject, cannot set Route in a Style Setter, cannot set its DataContext
    /// <summary>
    /// Construct an empty <see cref="Route"/>.
    /// </summary>
    public Route() {
      ClearCachedInfo();
    }

    static Route() {
      // Dependency properties:

      // Fractional point in the element representing the FromPort,
      // taking precedence over the Nodes.FromSpot attached property on the FromPort;
      // defaults to Spot.Default, meaning get the value from the port element
      FromSpotProperty = DependencyProperty.Register("FromSpot", typeof(Spot), typeof(Route),
        new FrameworkPropertyMetadata(Spot.Default, OnFromSpotChanged));

      // Fractional point in the element representing the ToPort,
      // taking precedence over the Nodes.ToSpot attached property on the ToPort;
      // defaults to Spot.Default, meaning get the value from the port element
      ToSpotProperty = DependencyProperty.Register("ToSpot", typeof(Spot), typeof(Route),
        new FrameworkPropertyMetadata(Spot.Default, OnToSpotChanged));

      FromEndSegmentLengthProperty = DependencyProperty.Register("FromEndSegmentLength", typeof(double), typeof(Route),
        new FrameworkPropertyMetadata(Double.NaN, OnFromEndSegmentLengthChanged));
      ToEndSegmentLengthProperty = DependencyProperty.Register("ToEndSegmentLength", typeof(double), typeof(Route),
        new FrameworkPropertyMetadata(Double.NaN, OnToEndSegmentLengthChanged));

      FromEndSegmentDirectionProperty = DependencyProperty.Register("FromEndSegmentDirection", typeof(LinkEndSegmentDirection), typeof(Route),
        new FrameworkPropertyMetadata(LinkEndSegmentDirection.Absolute, OnFromEndSegmentDirectionChanged));
      ToEndSegmentDirectionProperty = DependencyProperty.Register("ToEndSegmentDirection", typeof(LinkEndSegmentDirection), typeof(Route),
        new FrameworkPropertyMetadata(LinkEndSegmentDirection.Absolute, OnToEndSegmentDirectionChanged));

      FromShortLengthProperty = DependencyProperty.Register("FromShortLength", typeof(double), typeof(Route),
        new FrameworkPropertyMetadata(0.0, OnFromShortLengthChanged));
      ToShortLengthProperty = DependencyProperty.Register("ToShortLength", typeof(double), typeof(Route),
        new FrameworkPropertyMetadata(0.0, OnToShortLengthChanged));

      //FromExtensionProperty = DependencyProperty.Register("FromExtension", typeof(int), typeof(Route),
      //  new FrameworkPropertyMetadata(-1, OnFromExtensionChanged));
      //ToExtensionProperty = DependencyProperty.Register("ToExtension", typeof(int), typeof(Route),
      //  new FrameworkPropertyMetadata(-1, OnToExtensionChanged));

      CurveProperty = DependencyProperty.Register("Curve", typeof(LinkCurve), typeof(Route),
        new FrameworkPropertyMetadata(LinkCurve.None, OnCurveChanged));
      CurvinessProperty = DependencyProperty.Register("Curviness", typeof(double), typeof(Route),
        new FrameworkPropertyMetadata(Double.NaN, OnCurvinessChanged));
      CornerProperty = DependencyProperty.Register("Corner", typeof(double), typeof(Route),
        new FrameworkPropertyMetadata(0.0, OnCornerChanged));
      RoutingProperty = DependencyProperty.Register("Routing", typeof(LinkRouting), typeof(Route),
        new FrameworkPropertyMetadata(LinkRouting.Normal, OnRoutingChanged));
      AdjustingProperty = DependencyProperty.Register("Adjusting", typeof(LinkAdjusting), typeof(Route),
        new FrameworkPropertyMetadata(LinkAdjusting.None, OnAdjustingChanged));
      SmoothnessProperty = DependencyProperty.Register("Smoothness", typeof(double), typeof(Route),
        new FrameworkPropertyMetadata(0.5, OnSmoothnessChanged));

      //?? properties for governing selection handles..., DraggableOrthogonalSegments

      RelinkableFromProperty = DependencyProperty.Register("RelinkableFrom", typeof(bool), typeof(Route),
        new FrameworkPropertyMetadata(false, OnRelinkableFromChanged));  // if Selectable
      RelinkableToProperty = DependencyProperty.Register("RelinkableTo", typeof(bool), typeof(Route),
        new FrameworkPropertyMetadata(false, OnRelinkableToChanged));  // if Selectable

      RelinkFromAdornmentTemplateProperty = DependencyProperty.Register("RelinkFromAdornmentTemplate", typeof(DataTemplate), typeof(Route),
        new FrameworkPropertyMetadata(null, OnRelinkFromAdornmentTemplateChanged));
      RelinkToAdornmentTemplateProperty = DependencyProperty.Register("RelinkToAdornmentTemplate", typeof(DataTemplate), typeof(Route),
        new FrameworkPropertyMetadata(null, OnRelinkToAdornmentTemplateChanged));

      LinkReshapeHandleTemplateProperty = DependencyProperty.Register("LinkReshapeHandleTemplate", typeof(DataTemplate), typeof(Route),
        new FrameworkPropertyMetadata(null, OnLinkReshapeHandleTemplateChanged));
    }

    /// <summary>
    /// Gets the <see cref="Link"/> that owns this <see cref="Route"/>.
    /// </summary>
    public Link Link { get; internal set; }


    /// <summary>
    /// Identifies the <see cref="FromSpot"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FromSpotProperty;
    /// <summary>
    /// Gets or sets the <see cref="Spot"/> at which the route should connect to the port.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Spot.Default"/>.
    /// If this value is not <see cref="Spot.Default"/>, it takes precedence
    /// over the <c>Node.FromSpot</c> attached property on the port element.
    /// </value>
    public Spot FromSpot {
      get { return (Spot)GetValue(FromSpotProperty); }
      set { SetValue(FromSpotProperty, value); }
    }
    private static void OnFromSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      ((Route)d).InvalidateRoute();
    }

    /// <summary>
    /// Identifies the <see cref="ToSpot"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ToSpotProperty;
    /// <summary>
    /// Gets or sets the <see cref="Spot"/> at which the route should connect to the port.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Spot.Default"/>.
    /// If this value is not <see cref="Spot.Default"/>, it takes precedence
    /// over the <c>Node.ToSpot</c> attached property on the port element.
    /// </value>
    public Spot ToSpot {
      get { return (Spot)GetValue(ToSpotProperty); }
      set { SetValue(ToSpotProperty, value); }
    }
    private static void OnToSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      ((Route)d).InvalidateRoute();
    }


    /// <summary>
    /// Identifies the <see cref="FromEndSegmentLength"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FromEndSegmentLengthProperty;
    /// <summary>
    /// Gets or sets the length of the short segment that should start the route.
    /// </summary>
    /// <value>
    /// This defaults to <c>Double.NaN</c>.
    /// If the value is a number, it takes precedence over the <c>Node.FromEndSegmentLength</c>
    /// attached property on the port element.
    /// The effective value is determined by <see cref="GetEndSegmentLength"/>.
    /// </value>
    public double FromEndSegmentLength {
      get { return (double)GetValue(FromEndSegmentLengthProperty); }
      set { SetValue(FromEndSegmentLengthProperty, value); }
    }
    private static void OnFromEndSegmentLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      ((Route)d).InvalidateRoute();
    }

    /// <summary>
    /// Identifies the <see cref="ToEndSegmentLength"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ToEndSegmentLengthProperty;
    /// <summary>
    /// Gets or sets the length of the short segment that should end the route.
    /// </summary>
    /// <value>
    /// This defaults to <c>Double.NaN</c>.
    /// If the value is a number, it takes precedence over the <c>Node.ToEndSegmentLength</c>
    /// attached property on the port element.
    /// The effective value is determined by <see cref="GetEndSegmentLength"/>.
    /// </value>
    public double ToEndSegmentLength {
      get { return (double)GetValue(ToEndSegmentLengthProperty); }
      set { SetValue(ToEndSegmentLengthProperty, value); }
    }
    private static void OnToEndSegmentLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      ((Route)d).InvalidateRoute();
    }

    /// <summary>
    /// Identifies the <see cref="FromEndSegmentDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FromEndSegmentDirectionProperty;
    /// <summary>
    /// Gets or sets how <see cref="GetLinkDirection"/> behaves.
    /// </summary>
    /// <value>
    /// The default value is <see cref="LinkEndSegmentDirection.Absolute"/>.
    /// </value>
    /// <remarks>
    /// Typically when you have orthogonal links and nodes with ports that have particular spots
    /// along the edge of the node, if the whole node is rotated, you would want the connected links to
    /// be re-routed to go "around" to connect to the ports as if the spots had changed appropriately
    /// for that side of the node.
    /// You can get that effect by setting this property to <see cref="LinkEndSegmentDirection.RotatedNode"/>
    /// or <see cref="LinkEndSegmentDirection.RotatedNodeOrthogonal"/>.
    /// </remarks>
    [Category("Appearance")]
    [Description("Whether the angle of the first segment is affected by the node's RotationAngle.")]
    public LinkEndSegmentDirection FromEndSegmentDirection {
      get { return (LinkEndSegmentDirection)GetValue(FromEndSegmentDirectionProperty); }
      set { SetValue(FromEndSegmentDirectionProperty, value); }
    }
    private static void OnFromEndSegmentDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Route r = (Route)d;
      r.InvalidateRoute();
    }

    /// <summary>
    /// Identifies the <see cref="ToEndSegmentDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ToEndSegmentDirectionProperty;
    /// <summary>
    /// Gets or sets how <see cref="GetLinkDirection"/> behaves.
    /// </summary>
    /// <value>
    /// The default value is <see cref="LinkEndSegmentDirection.Absolute"/>.
    /// </value>
    /// <remarks>
    /// Typically when you have orthogonal links and nodes with ports that have particular spots
    /// along the edge of the node, if the whole node is rotated, you would want the connected links to
    /// be re-routed to go "around" to connect to the ports as if the spots had changed appropriately
    /// for that side of the node.
    /// You can get that effect by setting this property to <see cref="LinkEndSegmentDirection.RotatedNode"/>
    /// or <see cref="LinkEndSegmentDirection.RotatedNodeOrthogonal"/>.
    /// </remarks>
    [Category("Appearance")]
    [Description("Whether the angle of the last segment is affected by the node's RotationAngle.")]
    public LinkEndSegmentDirection ToEndSegmentDirection {
      get { return (LinkEndSegmentDirection)GetValue(ToEndSegmentDirectionProperty); }
      set { SetValue(ToEndSegmentDirectionProperty, value); }
    }
    private static void OnToEndSegmentDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Route r = (Route)d;
      r.InvalidateRoute();
    }

    /// <summary>
    /// Identifies the <see cref="FromShortLength"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FromShortLengthProperty;
    /// <summary>
    /// Gets or sets the length of the segment between the far edge of the <c>Arrowhead</c> and the <c>Node</c> at the start of the route.
    /// </summary>
    /// <value>
    /// This defaults to <c>Double.NaN</c>.
    /// </value>
    /// <remarks>
    /// The value for this property cannot exceed the length of the first segment of the Route.
    /// Except in the case where there is only one segment, in which the value cannot exceed half the length of the single segment.
    /// Setting the value to be greater than the limiting length has the same effect as setting it equal to that length.
    /// </remarks>
    public double FromShortLength {
      get { return (double)GetValue(FromShortLengthProperty); }
      set { SetValue(FromShortLengthProperty, value); }
    }
    private static void OnFromShortLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      if (Double.IsInfinity((double)e.NewValue) || Double.IsNaN((double)e.NewValue))
        d.SetValue(FromShortLengthProperty, 0.0);
      Route route = (Route)d;
      route.InvalidateGeometry();
      if (route.Link != null)
        route.Link.Remeasure();
    }

    /// <summary>
    /// Identifies the <see cref="ToShortLength"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ToShortLengthProperty;
    /// <summary>
    /// Gets or sets the length of the segment between the far edge of the <c>Arrowhead</c> and the <c>Node</c> at the end of the route.
    /// </summary>
    /// <value>
    /// This defaults to <c>Double.NaN</c>.
    /// </value>
    /// <remarks>
    /// The value for this property cannot exceed the length of the last segment of the Route.
    /// Except in the case where there is only one segment, in which the value cannot exceed half the length of the single segment.
    /// Setting the value to be greater than the limiting length has the same effect as setting it equal to that length.
    /// </remarks>
    public double ToShortLength {
      get { return (double)GetValue(ToShortLengthProperty); }
      set { SetValue(ToShortLengthProperty, value); }
    }
    private static void OnToShortLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      if (Double.IsInfinity((double)e.NewValue) || Double.IsNaN((double)e.NewValue)) 
        d.SetValue(ToShortLengthProperty, 0.0);
      Route route = (Route)d;
      route.InvalidateGeometry();
      if (route.Link != null)
        route.Link.Remeasure();
    }

    ///// <summary>
    ///// Identifies the <see cref="FromExtension"/> dependency property.
    ///// </summary>
    //public static readonly DependencyProperty FromExtensionProperty;
    ///// <summary>
    ///// Gets or sets a value controlling whether the link should connect directly to the
    ///// "From" port element or whether it should connect to one of its parent elements.
    ///// </summary>
    ///// <value>
    ///// This defaults to <c>-1</c>, meaning to use the value of the <c>Node.PortExtension</c>
    ///// attached property on the port element.
    ///// If the value is non-negative, it takes precedence over the <c>Node.PortExtension</c>
    ///// attached property on the port element.
    ///// </value>
    //public int FromExtension {
    //  get { return (int)GetValue(FromExtensionProperty); }
    //  set { SetValue(FromExtensionProperty, value); }
    //}
    //private static void OnFromExtensionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
    //  ((Route)d).InvalidateRoute();
    //}

    ///// <summary>
    ///// Identifies the <see cref="ToExtension"/> dependency property.
    ///// </summary>
    //public static readonly DependencyProperty ToExtensionProperty;
    ///// <summary>
    ///// Gets or sets a value controlling whether the link should connect directly to the
    ///// "To" port element or whether it should connect to one of its parent elements.
    ///// </summary>
    ///// <value>
    ///// This defaults to <c>-1</c>, meaning to use the value of the <c>Node.PortExtension</c>
    ///// attached property on the port element.
    ///// If the value is non-negative, it takes precedence over the <c>Node.PortExtension</c>
    ///// attached property on the port element.
    ///// </value>
    //public int ToExtension {
    //  get { return (int)GetValue(ToExtensionProperty); }
    //  set { SetValue(ToExtensionProperty, value); }
    //}
    //private static void OnToExtensionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
    //  ((Route)d).InvalidateRoute();
    //}


    /// <summary>
    /// Identifies the <see cref="Curve"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CurveProperty;
    /// <summary>
    /// Gets or sets the way the path is generated from the route's points.
    /// </summary>
    /// <value>
    /// The default value is <see cref="LinkCurve.None"/>.
    /// </value>
    public LinkCurve Curve {
      get { return (LinkCurve)GetValue(CurveProperty); }
      set { SetValue(CurveProperty, value); }
    }
    private static void OnCurveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Route r = (Route)d;
      r.InvalidateRoute();
      LinkCurve oldc = (LinkCurve)e.OldValue;
      LinkCurve newc = (LinkCurve)e.NewValue;
      r.InvalidateOtherJumpOvers(oldc == LinkCurve.JumpGap || oldc == LinkCurve.JumpOver ||
                                 newc == LinkCurve.JumpGap || newc == LinkCurve.JumpOver);
    }

    /// <summary>
    /// Identifies the <see cref="Curviness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CurvinessProperty;
    /// <summary>
    /// Gets or sets how far the control points are offset when the curve is <see cref="LinkCurve.Bezier"/>
    /// or when there are multiple links between the same two ports.
    /// </summary>
    /// <value>
    /// This is used to determine the offset distance for the control points of
    /// a <see cref="LinkCurve.Bezier"/> style link when connecting two ports
    /// whose spots are <see cref="Spot.None"/>, or when there
    /// are multiple links between two ports.
    /// This defaults to NaN.
    /// </value>
    [Category("Appearance")]
    [Description("How curved Bezier links are.")]
    public double Curviness {
      get { return (double)GetValue(CurvinessProperty); }
      set { SetValue(CurvinessProperty, value); }
    }
    private static void OnCurvinessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Route route = (Route)d;
      if (Double.IsInfinity((double)e.NewValue))
        route.SetValue(CurvinessProperty, Double.NaN);
      ((Route)d).InvalidateRoute();
    }

    /// <summary>
    /// Identifies the <see cref="Corner"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CornerProperty;
    /// <summary>
    /// Gets or sets how rounded the corners are for adjacent line segments when the
    /// <see cref="Curve"/> is <see cref="LinkCurve.None"/>,
    /// <see cref="LinkCurve.JumpOver"/>, or <see cref="LinkCurve.JumpGap"/> and the two line segments are orthogonal to each other.
    /// </summary>
    /// <value>
    /// This describes the maximum radius of rounded corners, in model coordinates.
    /// This defaults to 0.
    /// </value>
    [Category("Appearance")]
    [Description("How rounded corners are for orthogonal segments.")]
    public double Corner {
      get { return (double)GetValue(CornerProperty); }
      set { SetValue(CornerProperty, value); }
    }
    private static void OnCornerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Route route = (Route)d;
      route.InvalidateGeometry();
      if (route.Link != null)
        route.Link.Remeasure();
    }

    /// <summary>
    /// Gets whether the segments of the link are always horizontal or vertical.
    /// </summary>
    /// <remarks>
    /// You can change the value of this property by setting <see cref="Routing"/>.
    /// This property primarily controls the behavior of <see cref="ComputePoints"/>,
    /// to position the points of the stroke so that straight line segments will
    /// be horizontal or vertical.
    /// You can have a link drawn with only horizontal and vertical segments if the
    /// link's curve is <see cref="LinkCurve.None"/>, <see cref="LinkCurve.JumpOver"/>,
    /// or <see cref="LinkCurve.JumpGap"/>.
    /// If the curve is <see cref="LinkCurve.Bezier"/>, some of the orthogonally
    /// positioned points act as control points to help form the curve.
    /// The orthogonal <see cref="ComputePoints"/> algorithm adds two segments to
    /// the normal three segments so that it is possible to connect two ports using only
    /// orthogonal lines.
    /// </remarks>
    public bool Orthogonal {
      get { return (this.Routing & LinkRouting.Orthogonal) != 0; }
    }

    /// <summary>
    /// Identifies the <see cref="Routing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RoutingProperty;
    /// <summary>
    /// Gets or sets whether the link's path tries to avoid other nodes.
    /// </summary>
    /// <value>
    /// This defaults to <see cref="LinkRouting.Normal"/>.
    /// </value>
    /// <remarks>
    /// Setting this property to <see cref="LinkRouting.Orthogonal"/> or
    /// <see cref="LinkRouting.AvoidsNodes"/> is not recommended when the <see cref="Curve"/>
    /// is <see cref="LinkCurve.Bezier"/>.
    /// Changing this property will eventually result in a call to <see cref="ComputePoints"/>.
    /// </remarks>
    [Category("Appearance")]
    [Description("Whether a link is orthgonal or tries to avoid crossing over any nodes.")]
    public LinkRouting Routing {
      get { return (LinkRouting)GetValue(RoutingProperty); }
      set { SetValue(RoutingProperty, value); }
    }
    private static void OnRoutingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Route r = (Route)d;
      r.InvalidateRoute();
      LinkRouting newr = (LinkRouting)e.NewValue;
      LinkRouting oldr = (LinkRouting)e.OldValue;
      r.InvalidateOtherJumpOvers(oldr == LinkRouting.Orthogonal || oldr == LinkRouting.AvoidsNodes ||
                                 newr == LinkRouting.Orthogonal || newr == LinkRouting.AvoidsNodes);
    }

    /// <summary>
    /// Identifies the <see cref="Smoothness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SmoothnessProperty;
    /// <summary>
    /// Gets or sets how far control points are from the points of the Route when
    /// Routing is <see cref="LinkRouting.Orthogonal"/> and Curve is <see cref="LinkCurve.Bezier"/>.
    /// </summary>
    /// <value>
    /// This defaults to 0.5.
    /// </value>
    /// <remarks>
    /// Values of this property typically fall in the range of 0.0 to 1.0.  
    /// A value of 0.0 indicates that the control points of the curve are at the end points, which will result in straight line segments.
    /// A value of 1.0 indicates that the control points are one-third of the link's length away from the end point along the direction of the link,
    /// and the same distance away in a perpendicular direction. The distance scales linearly with the value of this property, 
    /// even with negative values and values greater than 1.0.
    /// </remarks>
    [Category("Appearance")]
    [Description("The distance from an Orthogonal and Bezier link to its control points.")]
    public double Smoothness {
      get {
        return (double)GetValue(SmoothnessProperty);
      }
      set { SetValue(SmoothnessProperty, value); }
    }
    private static void OnSmoothnessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Route route = (Route)d;
      route.InvalidateGeometry();
      if (route.Link != null)
        route.Link.Remeasure();
    }

    /// <summary>
    /// Identifies the <see cref="Adjusting"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AdjustingProperty;
    /// <summary>
    /// Gets or sets how <see cref="ComputePoints"/> behaves.
    /// </summary>
    /// <value>
    /// The default value is <see cref="LinkAdjusting.None"/>.
    /// </value>
    /// <remarks>
    /// The effectiveness of this property depends on the value of <see cref="Routing"/>.
    /// For example, if the routing is <see cref="LinkRouting.AvoidsNodes"/>,
    /// this property is ignored, because <see cref="ComputePoints"/>
    /// will compute the route in <see cref="AddOrthoPoints"/>
    /// without calling <see cref="AdjustPoints"/>.
    /// </remarks>
    [Category("Behavior")]
    [Description("How ComputePoints behaves.")]
    public LinkAdjusting Adjusting {
      get { return (LinkAdjusting)GetValue(AdjustingProperty); }
      set { SetValue(AdjustingProperty, value); }
    }
    private static void OnAdjustingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      // no immediate effect -- just used when reshaping
    }


    // other Link properties

    /// <summary>
    /// Identifies the <see cref="RelinkableFrom"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RelinkableFromProperty;
    /// <summary>
    /// Gets or sets whether the user may reconnect the "from" end of this link.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    /// <remarks>
    /// In general one should call <see cref="CanRelinkFrom"/> to see
    /// if a particular link is relinkable, not get this property value.
    /// </remarks>
    public bool RelinkableFrom {
      get { return (bool)GetValue(RelinkableFromProperty); }
      set { SetValue(RelinkableFromProperty, value); }
    }
    private static void OnRelinkableFromChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      // no immediate effect -- just used when adorning and reshaping
    }

    /// <summary>
    /// This predicate is true if the user may reconnect the "from" end of
    /// this link.
    /// </summary>
    /// <returns>
    /// Return true if this link is <see cref="RelinkableFrom"/>,
    /// if this link's layer's <see cref="Layer.AllowRelink"/> is true,
    /// and if this link's diagram's <see cref="Diagram.AllowRelink"/> is true.
    /// </returns>
    public virtual bool CanRelinkFrom() {
      if (!this.RelinkableFrom) return false;
      Link link = this.Link;
      if (link == null) return false;
      Layer layer = link.Layer;
      if (layer == null) return true;
      if (!layer.AllowRelink) return false;
      Diagram diagram = layer.Diagram;
      if (diagram == null) return true;
      if (!diagram.AllowRelink) return false;
      return true;
    }

    /// <summary>
    /// Identifies the <see cref="RelinkFromAdornmentTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RelinkFromAdornmentTemplateProperty;
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used by <see cref="Northwoods.GoXam.Tool.RelinkingTool"/>
    /// to create the <see cref="Adornment"/> for allowing the user to relink the "from" end of a <see cref="Link"/>.
    /// </summary>
    public DataTemplate RelinkFromAdornmentTemplate {
      get { return (DataTemplate)GetValue(RelinkFromAdornmentTemplateProperty); }
      set { SetValue(RelinkFromAdornmentTemplateProperty, value); }
    }
    private static void OnRelinkFromAdornmentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Link link = ((Route)d).Link;
      if (link != null) {
        link.RefreshAdornments();
      }
    }


    /// <summary>
    /// Identifies the <see cref="RelinkableTo"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RelinkableToProperty;  // only on root VisualElement
    /// <summary>
    /// Gets or sets whether the user may reconnect the "to" end of this link.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    /// <remarks>
    /// In general one should call <see cref="CanRelinkTo"/> to see
    /// if a particular link is relinkable, not get this property value.
    /// </remarks>
    public bool RelinkableTo {
      get { return (bool)GetValue(RelinkableToProperty); }
      set { SetValue(RelinkableToProperty, value); }
    }
    private static void OnRelinkableToChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      // no immediate effect -- just used when adorning and reshaping
    }

    /// <summary>
    /// This predicate is true if the user may reconnect the "to" end of
    /// this link.
    /// </summary>
    /// <returns>
    /// Return true if this link is <see cref="RelinkableTo"/>,
    /// if this link's layer's <see cref="Layer.AllowRelink"/> is true,
    /// and if this link's diagram's <see cref="Diagram.AllowRelink"/> is true.
    /// </returns>
    public virtual bool CanRelinkTo() {
      if (!this.RelinkableTo) return false;
      Link link = this.Link;
      if (link == null) return false;
      Layer layer = link.Layer;
      if (layer == null) return true;
      if (!layer.AllowRelink) return false;
      Diagram diagram = layer.Diagram;
      if (diagram == null) return true;
      if (!diagram.AllowRelink) return false;
      return true;
    }

    /// <summary>
    /// Identifies the <see cref="RelinkToAdornmentTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RelinkToAdornmentTemplateProperty;
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used by <see cref="Northwoods.GoXam.Tool.RelinkingTool"/>
    /// to create the <see cref="Adornment"/> for allowing the user to relink the "to" end of a <see cref="Link"/>.
    /// </summary>
    public DataTemplate RelinkToAdornmentTemplate {
      get { return (DataTemplate)GetValue(RelinkToAdornmentTemplateProperty); }
      set { SetValue(RelinkToAdornmentTemplateProperty, value); }
    }
    private static void OnRelinkToAdornmentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Link link = ((Route)d).Link;
      if (link != null) {
        link.RefreshAdornments();
      }
    }


    /// <summary>
    /// Identifies the <see cref="LinkReshapeHandleTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LinkReshapeHandleTemplateProperty;
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used by <see cref="Northwoods.GoXam.Tool.LinkReshapingTool"/>
    /// to create a handle for allowing the user to reshape the route of a <see cref="Link"/>.
    /// </summary>
    /// <remarks>
    /// This <c>DataTemplate</c> only defines the reshape handle, not the whole <see cref="Adornment"/>
    /// for reshaping the link.
    /// </remarks>
    public DataTemplate LinkReshapeHandleTemplate {
      get { return (DataTemplate)GetValue(LinkReshapeHandleTemplateProperty); }
      set { SetValue(LinkReshapeHandleTemplateProperty, value); }
    }
    private static void OnLinkReshapeHandleTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Link link = ((Route)d).Link;
      if (link != null) {
        link.ClearAdornments();
      }
    }


    // the Points in the Route

    /// <summary>
    /// Produce a string rendering that may help in debugging.
    /// </summary>
    /// <returns></returns>
    public override String ToString() {
      String s = this.PointsCount.ToString(System.Globalization.CultureInfo.InvariantCulture) + ": ";
      for (int i = 0; i < this.PointsCount; i++) {
        s += GetPoint(i).ToString() + " ";
      }
      if (this.Routing == LinkRouting.Orthogonal) s += "O";
      if (this.Routing == LinkRouting.AvoidsNodes) s += "A";
      switch (this.Curve) {
        case LinkCurve.None: s += "n"; break;
        case LinkCurve.JumpOver: s += "j"; break;
        case LinkCurve.Bezier: s += "b"; break;
        case LinkCurve.JumpGap: s += "g"; break;
      }
      s += " f: " + this.FromSpot.ToString() + " " + this.FromEndSegmentLength.ToString(System.Globalization.CultureInfo.InvariantCulture);
      s += " t: " + this.ToSpot.ToString() + " " + this.ToEndSegmentLength.ToString(System.Globalization.CultureInfo.InvariantCulture);
      return s;
    }


    /// <summary>
    /// Gets or sets the list of <c>Point</c>s in model coordinates that form the route.
    /// </summary>
    /// <value>
    /// The returned list of points should be treated as read-only.
    /// Instead of modifying it, call <see cref="SetPoint"/>, <see cref="InsertPoint"/>,
    /// <see cref="RemovePoint"/>, or <see cref="ClearPoints"/>.
    /// </value>
    [TypeConverter(typeof(PointListConverter))]
    public IList<Point> Points {
      get {
        return _Points;
      }
      set {
        if (_Points != value && value != null) {
          _Points = new List<Point>(value);

          if (this.Link != null) this.Link.NeedsRerouting = false;

          ValidateRoute();  // if explicitly setting points, route must be OK
          if (this.PointsCount > 0) {
            this.DefaultFromPoint = GetPoint(0);
            this.DefaultToPoint = GetPoint(this.PointsCount-1);
            this.UnsavedRoute = true;
          }
        }
      }
    }
    private List<Point> _Points = new List<Point>();

    /// <summary>
    /// Gets the number of points in the route.
    /// </summary>
    public int PointsCount {
      get { return _Points.Count; }
    }

    /// <summary>
    /// Gets a particular point of the route.
    /// </summary>
    /// <param name="i">the zero-based index of the desired point</param>
    /// <returns>
    /// if the index is negative or beyond the list of <see cref="Points"/>,
    /// this method will return <c>Point(Double.NaN, Double.NaN)</c>
    /// </returns>
    public Point GetPoint(int i) {
      if (i >= 0 && i < _Points.Count) {
        return _Points[i];
      }
      //Diagram.Error("Getting invalid Route point " + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
      return new Point(Double.NaN, Double.NaN);
    }

    /// <summary>
    /// Modify a particular point of the route.
    /// </summary>
    /// <param name="i">the zero-based index of the point; if beyond range this does nothing</param>
    /// <param name="p">the new point, which should not have infinite coordinate values</param>
    public void SetPoint(int i, Point p) {
      if (i >= 0 && i < _Points.Count && _Points[i] != p) {



        _Points[i] = p;
        ValidateRoute();  // if explicitly setting points, route must be OK
      }
    }

    /// <summary>
    /// Insert a point at a particular position in the route.
    /// </summary>
    /// <param name="i">
    /// the zero-based index of the new point;
    /// if less than zero this does nothing;
    /// if greater than the number of points it adds the point at the end
    /// </param>
    /// <param name="p">the new point, which should not have infinite coordinate values</param>
    public void InsertPoint(int i, Point p) {
      if (i >= 0) {



        if (i < _Points.Count) {
          _Points.Insert(i, p);
        } else {
          _Points.Add(p);
        }
        ValidateRoute();  // if explicitly setting points, route must be OK
      }
    }

    /// <summary>
    /// Add a point at the end of the route.
    /// </summary>
    /// <param name="p">the new point, which should not have infinite coordinate values</param>
    public void AddPoint(Point p) {



      _Points.Add(p);
      ValidateRoute();  // if explicitly setting points, route must be OK
    }

    /// <summary>
    /// Remove a particular point from the route.
    /// </summary>
    /// <param name="i">the zero-based index of the point to extract; if beyond range this does nothing</param>
    public void RemovePoint(int i) {
      if (i >= 0 && i < _Points.Count) {
        _Points.RemoveAt(i);
        UpdateVisual();  // route may or may not be OK, but we ought to update in any case
      }
    }

    /// <summary>
    /// Modify all of the points by adding a constant offset to each one.
    /// </summary>
    /// <param name="offset">the values must be a number other than infinity</param>
    public void MovePoints(Point offset) {
      if (offset != new Point(0, 0) &&
          !Double.IsNaN(offset.X) && !Double.IsInfinity(offset.X) &&
          !Double.IsNaN(offset.Y) && !Double.IsInfinity(offset.Y)) {
        for (int i = 0; i < _Points.Count; i++) {
          Point p = _Points[i];
          p.X += offset.X;
          p.Y += offset.Y;
          _Points[i] = p;
        }
        this.DefaultFromPoint = Geo.Add(this.DefaultFromPoint, offset);
        this.DefaultToPoint = Geo.Add(this.DefaultToPoint, offset);
        // Instead of calling ValidateRoute, which calls ClearCachedInfo:
        this.ValidRoute = true;
        if (!Double.IsNaN(_RouteBounds.X) && !Double.IsNaN(_RouteBounds.Y)) {
          _RouteBounds.X += offset.X;
          _RouteBounds.Y += offset.Y;
        }
        if (!Double.IsNaN(_MidPoint.X) && !Double.IsNaN(_MidPoint.Y)) {
          _MidPoint.X += offset.X;
          _MidPoint.Y += offset.Y;
        }
        InvalidateGeometry();
        Link link = this.Link;
        if (link != null) {
          Rect b = link.Bounds;
          b.X += offset.X;
          b.Y += offset.Y;
          if (Double.IsNaN(b.X)) b.X = this.RouteBounds.X;
          if (Double.IsNaN(b.Y)) b.Y = this.RouteBounds.Y;
          link.Bounds = b;
          link.Remeasure();
        }
      }
    }

    /// <summary>
    /// Remove all of the points of the route.
    /// </summary>
    public void ClearPoints() {
      if (_Points.Count > 0) {
        _Points.Clear();
        InvalidateRoute();
      }
    }

    private int InternalFlags { get; set; }
    // Route flags:
    private const int ValidRouteFlag         = 0x0001;
    private const int SuspendsRoutingFlag    = 0x0002;
    private const int DelayedRoutingFlag     = 0x0004;
    private const int ComputingPointsFlag    = 0x0008;
    private const int UnsavedRouteFlag       = 0x0010;
    private const int EffectiveBezierFlag    = 0x0020;

    // whether the points are valid
    private bool ValidRoute { get { return (this.InternalFlags & ValidRouteFlag) != 0; } set { if (value) this.InternalFlags |= ValidRouteFlag; else this.InternalFlags &= ~ValidRouteFlag; } }

    // used by DraggingTool to avoid re-routing links being moved
    internal bool SuspendsRouting { get { return (this.InternalFlags & SuspendsRoutingFlag) != 0; } set { if (value) this.InternalFlags |= SuspendsRoutingFlag; else this.InternalFlags &= ~SuspendsRoutingFlag; } }

    // used during rebuilding to avoid premature AvoidsNodes route computation
    internal bool DelayedRouting { get { return (this.InternalFlags & DelayedRoutingFlag) != 0; } set { if (value) this.InternalFlags |= DelayedRoutingFlag; else this.InternalFlags &= ~DelayedRoutingFlag; } }

    // true during RecomputePoints
    private bool ComputingPoints { get { return (this.InternalFlags & ComputingPointsFlag) != 0; } set { if (value) this.InternalFlags |= ComputingPointsFlag; else this.InternalFlags &= ~ComputingPointsFlag; } }

    // used by DiagramPanel and PartManager
    internal bool UnsavedRoute { get { return (this.InternalFlags & UnsavedRouteFlag) != 0; } set { if (value) this.InternalFlags |= UnsavedRouteFlag; else this.InternalFlags &= ~UnsavedRouteFlag; } }

    // set in ComputePoints
    private bool EffectiveBezier { get { return (this.InternalFlags & EffectiveBezierFlag) != 0; } set { if (value) this.InternalFlags |= EffectiveBezierFlag; else this.InternalFlags &= ~EffectiveBezierFlag; } }

    // Declare the visual out-of-date, and request a re-rendering.
    // A no-op during recalculation of route in UpdatePoints/ComputePoints
    private void UpdateVisual() {
      if (this.ComputingPoints) return;
      ClearCachedInfo();
      Link link = this.Link;
      if (link != null && link.ValidArrange) {
        if (link.Offscreen) {
          Rect b = Rect.Empty;
          // try to update Bounds with the approximately correct value
          // cannot yet depend on any points
          Node from = link.FromNode;
          Node to = link.ToNode;
          if (from != null && to != null) {
            Point fp = from.Location;
            Point tp = to.Location;
            if (!Double.IsNaN(fp.X) && !Double.IsNaN(fp.Y) && !Double.IsNaN(tp.X) && !Double.IsNaN(tp.Y)) {
              b = Geo.Union(from.Bounds, to.Bounds);
              link.Bounds = b;
            }
          } else {
            //??? NOTMOVABLELINK
            if (this.PointsCount > 0) {
              b = Geo.Bounds(_Points);
              link.Bounds = b;
            }
          }
          if (!b.IsEmpty) {
            // if there's a LabelNode, give it an approximate location
            Node lab = link.LabelNode;
            if (lab != null) {
              Rect lb = lab.Bounds;
              if (lab.Offscreen && lb.Width == 0 && lb.Height == 0) {
                lab.RemeasureNow();
                lb = lab.Bounds;
              }
              lab.Position = new Point(b.X+b.Width/2-lb.Width/2, b.Y+b.Height/2-lb.Height/2);
            }
            // do NOT provide approximate but incorrect values for these computed properties:
            //this.RouteBounds = b;
            //this.MidPoint = new Point(b.X+b.Width/2, b.Y+b.Height/2);
            //this.MidAngle = Geo.GetAngle(fp, tp);
          }
        }
        link.Remeasure();  // update not just the Route's geometry, but related things too
      }
    }

    // called when setting the points -- the route is now valid,
    // but we need to update the visuals
    private void ValidateRoute() {
      if (_Points.Count == 0) return;
      //if (!this.ValidRoute) Diagram.Debug("Validating Route: " + this.Link.ToString());
      this.ValidRoute = true;
      UpdateVisual();
      InvalidateOtherJumpOvers(false);
    }

    /// <summary>
    /// Declare that the points in this <see cref="Route"/> are invalid and request
    /// that it call <see cref="RecomputePoints"/> soon.
    /// </summary>
    /// <remarks>
    /// This is called when a node invalidates its connected links (<see cref="Part.InvalidateRelationships()"/>)
    /// or when some Route properties have been changed.
    /// </remarks>
    public void InvalidateRoute() {
      if (this.SuspendsRouting) return;
      //if (this.ValidRoute) Diagram.Debug("Invalidating Route: " + this.Link.ToString());
      // do not clear _Points, because the old points might be needed in computing the new ones
      this.ValidRoute = false;
      UpdateVisual();
    }

    /// <summary>
    /// Recalculate route points by calling <see cref="RecomputePoints"/> only if needed.
    /// </summary>
    /// <remarks>
    /// In case the route has become invalid, e.g. because <see cref="InvalidateRoute"/> was called,
    /// you need to call this method before you can call <see cref="GetPoint"/>.
    /// This method is also called by property getters such as <see cref="DefiningGeometry"/>
    /// and <see cref="MidPoint"/> and <see cref="MidAngle"/>.
    /// </remarks>
    public void UpdatePoints() {
      if (this.ValidRoute) return;
      RecomputePoints();
    }

    /// <summary>
    /// Call <see cref="ComputePoints"/> right now.
    /// </summary>
    /// <remarks>
    /// This method calls <see cref="ComputePoints"/> in order to calculate a new route.
    /// If you just want to make sure the points are up-to-date, call <see cref="UpdatePoints"/> instead.
    /// </remarks>
    public void RecomputePoints() {

      Link link = this.Link;
      if (link != null) {
        bool again = false;
        // see if this route needs to be recomputed because the node might not be laid out yet
        Node from = link.FromNode;
        if (from != null && (!from.ValidMeasure || from.Offscreen)) {
          again = true;
        } else {
          Node to = link.ToNode;
          if (to != null && (!to.ValidMeasure || to.Offscreen)) {
            again = true;
          }
        }
        //Diagram.Debug("NeedsRerouting: " + link.NeedsRerouting.ToString() + " --> " + again.ToString() + " " + Diagram.Str(link));
        link.NeedsRerouting = again;
      }

      bool calc = false;
      try {
        // temporarily turn off updates
        this.ComputingPoints = true;
        //if (link.NeedsRerouting) Diagram.Debug("computing points now anyway");
        // compute new path of points
        calc = ComputePoints();
        this.UnsavedRoute = true;
      } finally {
        this.ComputingPoints = false;
        // done with computing the Points: now allow UpdateVisual to invalidate the measurement of the Link
        if (calc) ValidateRoute();  // finished explicitly computing and setting points of route
      }
    }

    private void ClearCachedInfo() {
      // invalidate the Route's Bounds, MidPoint, and MidAngle properties:
      this.RouteBounds = new Rect(Double.NaN, Double.NaN, 0, 0);  // recompute when needed
      this.MidPoint = new Point(Double.NaN, Double.NaN);  // recompute when needed
      this.MidAngle = Double.NaN;  // recompute when needed
      // invalidate the DefiningGeometry property:
      InvalidateGeometry();
    }

    internal Rect RouteBounds {  // also used by LinkPanel and Parts
      get {
        if (Double.IsNaN(_RouteBounds.X) || Double.IsNaN(_RouteBounds.Y)) {
          UpdatePoints();
          _RouteBounds = ComputeBounds();
          this.PreviousBounds = _RouteBounds;
        }
        return _RouteBounds;
      }
      private set { _RouteBounds = value; }
    }
    private Rect _RouteBounds;
    private Rect PreviousBounds { get; set; }

    private Rect ComputeBounds() {
      int numpts = this.PointsCount;
      Rect bounds;
      if (numpts == 0) {
        bounds = new Rect(Double.NaN, Double.NaN, 0, 0);  // no location means call UpdatePoints/ComputeBounds again
      } else if (numpts == 1) {
        Point p = GetPoint(0);
        bounds = new Rect(p.X, p.Y, 0, 0);
      } else if (numpts == 2) {
        bounds = new Rect(GetPoint(0), GetPoint(1));
      } else {
        if (GetCurve() == LinkCurve.Bezier && numpts >= 4) {
          Point pnt = GetPoint(0);
          double left = pnt.X;
          double top = pnt.Y;
          double right = pnt.X;
          double bottom = pnt.Y;
          Point start, startControl, endControl, end;
          for (int i = 3; i < numpts; i += 3) {
            start = GetPoint(i-3);
            startControl = GetPoint(i-2);
            // if it's the last segment, use the last two points
            if (i+3 >= numpts)
              i = numpts-1;
            endControl = GetPoint(i-1);
            end = GetPoint(i);
            Rect brect = Geo.BezierBounds(start, startControl, endControl, end, 0.1);  //?? epsilon
            left = Math.Min(left, brect.X);
            top = Math.Min(top, brect.Y);
            right = Math.Max(right, brect.X+brect.Width);
            bottom = Math.Max(bottom, brect.Y+brect.Height);
          }
          bounds = new Rect(left, top, right-left, bottom-top);
        } else {
          Rect r = new Rect(GetPoint(0), GetPoint(1));
          for (int i = 2; i < this.PointsCount-1; i++) {
            r.Union(GetPoint(i));
          }
          bounds = r;
        }
      }
      return bounds;
    }


    /// <summary>
    /// Gets the point at the middle of the route.
    /// </summary>
    /// <value>
    /// This is determined by <see cref="ComputeMidPoint"/>.
    /// </value>
    public Point MidPoint {  //?? change to be DependencyProperty
      get {
        if (Double.IsNaN(_MidPoint.X) || Double.IsNaN(_MidPoint.Y)) {
          UpdatePoints();
          _MidPoint = ComputeMidPoint();
        }
        return _MidPoint;
      }
      private set { _MidPoint = value; }
    }
    private Point _MidPoint;

    /// <summary>
    /// Computes the point at the middle of the route.
    /// </summary>
    /// <returns></returns>
    protected virtual Point ComputeMidPoint() {
      IList<Point> pts = (List<Point>)this.Points;
      int numpts = pts.Count;
      if (numpts == 0) return new Point(Double.NaN, Double.NaN);  // no point means call UpdatePoints/ComputeMidPoint again
      if (numpts == 1) return pts[0];
      if (numpts == 2) return new Point(0.5*(pts[0].X + pts[1].X), 0.5*(pts[0].Y+pts[1].Y));

      if (GetCurve() == LinkCurve.Bezier && numpts >= 3 && !(Routing == LinkRouting.Orthogonal) && !(Routing == LinkRouting.AvoidsNodes)) {
        if (numpts == 3) return GetPoint(1);
        int numsegs = (numpts-1)/3;
        int idx = numsegs/2 * 3;
        if (numsegs%2 == 1) {
          Point a = GetPoint(idx);
          Point b = GetPoint(idx+1);
          Point c = GetPoint(idx+2);
          Point d = GetPoint(idx+3);
          Point v, w;
          Geo.BezierMidPoint(a, b, c, d, out v, out w);
          return new Point((v.X+w.X)/2, (v.Y+w.Y)/2);
        } else {
          return GetPoint(idx);
        }
      }

      double distanceAlongRoute = 0;
      Queue<double> segmentDistanceQueue = new Queue<double>();
      for (int i = 0; i < numpts-1; i++) {
        double segmentDistance;
        Point a = GetPoint(i);
        Point b = GetPoint(i+1);

        //Handle computationally simple Orthogonal cases
        if (Geo.IsApproxEqual(a.X, b.X)) {
          segmentDistance = b.Y-a.Y;
          if (segmentDistance < 0)
            segmentDistance = -segmentDistance;
          segmentDistanceQueue.Enqueue(segmentDistance);
          distanceAlongRoute += segmentDistance;
          continue;
        }
        if (Geo.IsApproxEqual(a.Y, b.Y)) {
          segmentDistance = b.X-a.X;
          if (segmentDistance < 0)
            segmentDistance = -segmentDistance;
          segmentDistanceQueue.Enqueue(segmentDistance);
          distanceAlongRoute += segmentDistance;
          continue;
        }

        //Handle non-Orthogonal lines
        segmentDistance = Math.Sqrt(Geo.DistanceSquared(a, b));
        segmentDistanceQueue.Enqueue(segmentDistance);
        distanceAlongRoute += segmentDistance;
      }

      double currentDistance = 0;
      int currentPointIndex = 0;
      double nextSegmentLength = 0;
      while (currentDistance < distanceAlongRoute*0.5) {
        nextSegmentLength = segmentDistanceQueue.Dequeue();
        if (currentDistance + nextSegmentLength > distanceAlongRoute*0.5)
          break;
        else
          currentDistance += nextSegmentLength;
        currentPointIndex++;
      }
      //double midSegmentAngle = Geo.GetAngle(GetPoint(currentPointIndex), GetPoint(currentPointIndex+1));
      Point currentPoint = GetPoint(currentPointIndex);
      Point nextPoint = GetPoint(currentPointIndex + 1);

      //Handle Orthogonal cases
      if (currentPoint.X == nextPoint.X) {
        if (currentPoint.Y > nextPoint.Y)
          return new Point(currentPoint.X, currentPoint.Y - (distanceAlongRoute*0.5 - currentDistance));
        else
          return new Point(currentPoint.X, currentPoint.Y + (distanceAlongRoute*0.5 - currentDistance));
      }
      if (currentPoint.Y == nextPoint.Y) {
        if (currentPoint.X > nextPoint.X)
          return new Point(currentPoint.X - (distanceAlongRoute*0.5 - currentDistance), currentPoint.Y);
        else
          return new Point(currentPoint.X + (distanceAlongRoute*0.5 - currentDistance), currentPoint.Y);
      }

      //Handle non-Orthogonal lines
      double similarTriangleRatio = (distanceAlongRoute/2 - currentDistance) / nextSegmentLength;
      double dx = similarTriangleRatio * (nextPoint.X - currentPoint.X);
      double dy = similarTriangleRatio * (nextPoint.Y - currentPoint.Y);

      return new Point(currentPoint.X + dx, currentPoint.Y + dy);
    }


    /// <summary>
    /// Gets the angle of the route at the <see cref="MidPoint"/>.
    /// </summary>
    /// <value>
    /// This is determined by <see cref="ComputeMidAngle"/>.
    /// </value>
    public double MidAngle {  //?? change to be DependencyProperty
      get {
        if (Double.IsNaN(_MidAngle)) {
          UpdatePoints();
          _MidAngle = ComputeMidAngle();
        }
        return _MidAngle;
      }
      private set { _MidAngle = value; }
    }
    private double _MidAngle;

    /// <summary>
    /// Compute the angle of the route at the <see cref="MidPoint"/>.
    /// </summary>
    /// <returns></returns>
    protected virtual double ComputeMidAngle() {
      IList<Point> pts = (List<Point>)this.Points;
      int numpts = pts.Count;
      if (numpts < 2) return Double.NaN;  // no angle means call UpdatePoints/ComputeMidAngle again

      if (GetCurve() == LinkCurve.Bezier && numpts >= 4 && !(Routing == LinkRouting.Orthogonal) && !(Routing == LinkRouting.AvoidsNodes)) {
        int numsegs = (numpts-1)/3;
        int idx = numsegs/2 * 3;
        if (numsegs%2 == 1) {
          Point a = GetPoint(idx);
          Point b = GetPoint(idx+1);
          Point c = GetPoint(idx+2);
          Point d = GetPoint(idx+3);
          Point v, w;
          Geo.BezierMidPoint(a, b, c, d, out v, out w);
          return Geo.GetAngle(v, w);
        } else {
          if (idx > 0 && idx+1 < numpts)
            return Geo.GetAngle(GetPoint(idx-1), GetPoint(idx+1));
          // else drop through and treat as a non-Bezier-curve
        }
      }

      int midEnd = numpts/2;
      if (numpts % 2 == 0) {  // even number of points means odd number of segments
        // get the middle segment (perhaps the only one)
        Point a = pts[midEnd-1];
        Point b = pts[midEnd];
        return Geo.GetAngle(a, b);
      } else {
        // also find the points on either side of the middle point
        // then figure out if one segment is much longer than the other
        Point a = pts[midEnd-1];
        Point b = pts[midEnd];
        Point c = pts[midEnd+1];
        double d1 = Geo.DistanceSquared(a, b);
        double d2 = Geo.DistanceSquared(b, c);
        if (d1 > d2)
          return Geo.GetAngle(a, b);
        else
          return Geo.GetAngle(b, c);
      }
    }


    private const double RIGHT = 0;
    private const double DOWN = 90;
    private const double LEFT = 180;
    private const double UP = 270;

    // no node/port locations
    internal Point DefaultFromPoint { get; set; }
    internal Point DefaultToPoint { get; set; }
    //internal Point DefaultToPoint {
    //  get { return _DefaultToPoint; }
    //  set {
    //    if (_DefaultToPoint != value) {
    //      Diagram.Debug(Diagram.Str(this.Link) + " old: " + Diagram.Str(_DefaultToPoint) + " new: " + Diagram.Str(value));
    //      Point old = _DefaultToPoint;
    //      _DefaultToPoint = value;
    //    }
    //  }
    //}
    //private Point _DefaultToPoint;

    /// <summary>
    /// Compute all of the points of this route.
    /// </summary>
    /// <returns>true if the points were calculated</returns>
    protected virtual bool ComputePoints() {
      Link link = this.Link;
      if (link == null || !Part.IsVisibleElement(link)) return false;
      Diagram diagram = link.Diagram;
      if (diagram == null) return false;

      Node fromnode = link.FromNode;
      Node origfromnode = fromnode;
      FrameworkElement fromport;
      if (fromnode == null) {
        if (diagram.RoutingFromNode == null) {
          diagram.RoutingFromPort = new Rectangle();
          diagram.RoutingFromPort.Width = 0;
          diagram.RoutingFromPort.Height = 0;
          diagram.RoutingFromNode = new Node();
          diagram.RoutingFromNode.Content = diagram.RoutingFromPort;
          diagram.RoutingFromNode.ApplyTemplate();  // needed to avoid transform exceptions in Silverlight
        }
        diagram.RoutingFromNode.Bounds = new Rect(this.DefaultFromPoint.X, this.DefaultFromPoint.Y, 0, 0);
        fromnode = diagram.RoutingFromNode;
        fromport = diagram.RoutingFromPort;
      } else {
        String fromparam = link.FromPortId;
        fromport = fromnode.FindPort(fromparam, true);
      }
      if (fromport != null) {
        // is fromport not visible?  try containing elements
        FrameworkElement visfromport = Node.FindVisiblePort(fromport);
        if (visfromport == fromnode || !fromnode.Visible) {  // fromport and its parents aren't visible
          Node visfromnode = fromnode.FindVisibleNode(null);  // is fromnode itself visible?
          if (visfromnode != fromnode) {  // no, found a containing group
            fromnode = visfromnode;  // use it instead
            fromport = visfromnode.FindPort(null, true);  // no parameter -- it wouldn't apply to a different node anyway
          } else {
            fromnode = visfromnode;  // might be null if everything is not visible
          }
        } else {  // either fromport or a parent element within the node is visible
          fromport = visfromport;
        }
      }
      if (fromnode == null || fromport == null) return false;
      Point fromloc = fromnode.Position;
      if (Double.IsNaN(fromloc.X) || Double.IsNaN(fromloc.Y)) return false;  // no port or no location for node? forget it!

      Node tonode = link.ToNode;
      Node origtonode = tonode;
      FrameworkElement toport;
      if (tonode == null) {
        if (diagram.RoutingToNode == null) {
          diagram.RoutingToPort = new Rectangle();
          diagram.RoutingToPort.Width = 0;
          diagram.RoutingToPort.Height = 0;
          diagram.RoutingToNode = new Node();
          diagram.RoutingToNode.Content = diagram.RoutingToPort;
          diagram.RoutingToNode.ApplyTemplate();
        }
        diagram.RoutingToNode.Bounds = new Rect(this.DefaultToPoint.X, this.DefaultToPoint.Y, 0, 0);
        tonode = diagram.RoutingToNode;
        toport = diagram.RoutingToPort;
      } else {
        String toparam = link.ToPortId;
        toport = tonode.FindPort(toparam, false);
      }
      if (toport != null) {
        // is toport not visible?  try containing elements
        FrameworkElement vistoport = Node.FindVisiblePort(toport);
        if (vistoport == tonode || !tonode.Visible) {  // toport and its parents aren't visible
          Node vistonode = tonode.FindVisibleNode(null);  // is tonode itself visible?
          if (vistonode != tonode) {  // no, found a containing group
            tonode = vistonode;  // use it instead
            toport = vistonode.FindPort(null, false);  // no parameter -- it wouldn't apply to a different node anyway
          } else {
            tonode = vistonode;  // might be null if everything is not visible
          }
        } else {  // either toport or a parent element within the node is visible
          toport = vistoport;
        }
      }
      if (tonode == null || toport == null) return false;
      Point toloc = tonode.Position;
      if (Double.IsNaN(toloc.X) || Double.IsNaN(toloc.Y)) return false;  // no port or no location for node? forget it!

      int num = this.PointsCount;

      Spot fromspot = GetFromSpot();  // link's Spot takes precedence, if defined
      Spot tospot = GetToSpot();  // link's Spot takes precedence, if defined

      bool selfloop = (fromport == toport);
      bool ortho = this.Orthogonal;
      bool bezier = (this.Curve == LinkCurve.Bezier);
      if (selfloop && !ortho) {
        bezier = true;
        this.EffectiveBezier = true;
      } else {
        this.EffectiveBezier = false;
      }
      bool calc = (this.Adjusting == LinkAdjusting.None) || selfloop;

      if (!ortho && fromspot.IsNone && tospot.IsNone && !selfloop) {
        bool adjusted = false;
        if (!calc && num >= 3) {
          Point pA = GetLinkPoint(fromnode, fromport, Spot.None, true, false, tonode, toport);
          Point pB = GetLinkPoint(tonode, toport, Spot.None, false, false, fromnode, fromport);
          adjusted = AdjustPoints(0, pA, num-1, pB);
        }
        if (!adjusted) {
          ClearPoints();
          if (bezier) {
            CalculateBezierNoSpot(fromnode, fromport, tonode, toport);
          } else {
            CalculateLineNoSpot(fromnode, fromport, tonode, toport);
          }
        }
      } else {  // three segments (four points)
        bool avoidsnodes = (this.Routing == LinkRouting.AvoidsNodes);
        if (calc && ((ortho && avoidsnodes) || selfloop)) ClearPoints();  // to have the GoPort.Get[From/To]... methods produce better results
        double curviness = ComputeCurviness();

        // turn off updating as we remove and add points, and just update the
        // whole object when we're done
        Point fromPoint = GetLinkPoint(fromnode, fromport, fromspot, true, ortho, tonode, toport);
        double Bx = 0;
        double By = 0;
        double fromDir = 0;
        if (ortho || fromspot.IsNotNone || selfloop) {
          // get the length of the beginning segment
          double fromSeg = GetEndSegmentLength(fromnode, fromport, fromspot, true);

          // calculate the end point of the initial segment
          fromDir = GetLinkDirection(fromnode, fromport, fromPoint, fromspot, true, ortho, tonode, toport);
          if (selfloop) {
            fromDir -= (ortho ? 90 : 30);
            if (curviness < 0)
              fromDir -= 180;
          }
          if (fromDir < 0) fromDir += 360;
          else if (fromDir >= 360) fromDir -= 360;

          if (selfloop) fromSeg += Math.Abs(curviness);

          // handle common cases more quickly than by calling Cos and Sin
          if (fromDir == RIGHT) {
            Bx = fromSeg;
          } else if (fromDir == DOWN) {
            By = fromSeg;
          } else if (fromDir == LEFT) {
            Bx = -fromSeg;
          } else if (fromDir == UP) {
            By = -fromSeg;
          } else {
            Bx = fromSeg * Math.Cos(fromDir*Math.PI/180);
            By = fromSeg * Math.Sin(fromDir*Math.PI/180);
          }

          // try to accommodate self-loops on nodes that have no spot
          if (fromspot.IsNoSpot && selfloop) {
            Point ctr = fromnode.GetElementPoint(fromport, Spot.Center);
            fromPoint = GetLinkPointFromPoint(fromnode, fromport, ctr, new Point(ctr.X + Bx*1000, ctr.Y + By*1000), true);
          }
        }

        Point toPoint = GetLinkPoint(tonode, toport, tospot, false, ortho, fromnode, fromport);
        double Cx = 0;
        double Cy = 0;
        double toDir = 0;
        if (ortho || tospot.IsNotNone || selfloop) {
          // get the length of the ending segment
          double toSeg = GetEndSegmentLength(tonode, toport, tospot, false);

          // calculate the start point of the final segment
          toDir = GetLinkDirection(tonode, toport, toPoint, tospot, false, ortho, fromnode, fromport);
          if (selfloop) {
            toDir += (ortho ? 0 : 30);
            if (curviness < 0)
              toDir += 180;
          }
          if (toDir < 0) toDir += 360;
          else if (toDir >= 360) toDir -= 360;

          if (selfloop) toSeg += Math.Abs(curviness);

          // handle common cases more quickly than by calling Cos and Sin
          if (toDir == RIGHT) {
            Cx = toSeg;
          } else if (toDir == DOWN) {
            Cy = toSeg;
          } else if (toDir == LEFT) {
            Cx = -toSeg;
          } else if (toDir == UP) {
            Cy = -toSeg;
          } else {
            Cx = toSeg * Math.Cos(toDir*Math.PI/180);
            Cy = toSeg * Math.Sin(toDir*Math.PI/180);
          }

          // try to accommodate self-loops on nodes that have no spot
          if (tospot.IsNoSpot && selfloop) {
            Point ctr = tonode.GetElementPoint(toport, Spot.Center);
            toPoint = GetLinkPointFromPoint(tonode, toport, ctr, new Point(ctr.X + Cx*1000, ctr.Y + Cy*1000), false);
          }
        }

        // determine intermediate/control points
        Point b = fromPoint;
        if (ortho || fromspot.IsNotNone || selfloop)
          b = new Point(fromPoint.X+Bx, fromPoint.Y+By);
        Point c = toPoint;
        if (ortho || tospot.IsNotNone || selfloop)
          c = new Point(toPoint.X+Cx, toPoint.Y+Cy);

        // only call AdjustPoints when AdjustingStyle is not Calculate, and if it returns true, it was successful
        if (!calc && !ortho && fromspot.IsNoSpot && num > 3 && AdjustPoints(0, fromPoint, num-2, c)) {
          SetPoint(num-1, toPoint);
        } else if (!calc && !ortho && tospot.IsNoSpot && num > 3 && AdjustPoints(1, b, num-1, toPoint)) {
          SetPoint(0, fromPoint);
        } else if (!calc && !ortho && num > 4 && AdjustPoints(1, b, num-2, c)) {
          SetPoint(0, fromPoint);
          SetPoint(num-1, toPoint);
        } else if (!calc && ortho && num >= 6 && !avoidsnodes && AdjustPoints(1, b, num-2, c)) {
          SetPoint(0, fromPoint);
          SetPoint(num-1, toPoint);
        } else {  // calculate standard four (non-ortho) or six (ortho) point stroke
          ClearPoints();

          // add the initial point
          AddPoint(fromPoint);

          if (ortho || fromspot.IsNotNone || selfloop)
            AddPoint(b);

          if (ortho) {
            AddOrthoPoints(b, fromDir, c, toDir, fromnode, tonode);
          }

          if (ortho || tospot.IsNotNone || selfloop)
            AddPoint(c);

          // add the final endpoint
          AddPoint(toPoint);
        }
      }
      // remember endpoints in case the route is invalidated
      /* if (origfromnode == null) */ this.DefaultFromPoint = GetPoint(0);
      /* if (origtonode == null) */ this.DefaultToPoint = GetPoint(this.PointsCount-1);
      //String msg = ""; for (int i = 0; i < this.PointsCount; i++) msg += Diagram.Str(GetPoint(i)); Diagram.Debug("ComputedPoints: " + msg);
      return true;
    }


    // from the GoPort class:

    private Point OrthoPointToward(Point center, Point p) {
      Point c = center;
      if (Math.Abs(p.X-c.X) > Math.Abs(p.Y-c.Y)) {
        if (p.X >= c.X)
          p.X = c.X+9e9;
        else
          p.X = c.X-9e9;
        p.Y = c.Y;
      } else {
        if (p.Y >= c.Y)
          p.Y = c.Y+9e9;
        else
          p.Y = c.Y-9e9;
        p.X = c.X;
      }
      return p;
    }

    /// <summary>
    /// Compute the intersection point for the edge of a particular port element, given a point,
    /// when no particular spot or side has been specified.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="port">the <c>FrameworkElement</c> representing a port on the node</param>
    /// <param name="focus">the point in model coordinates to/from which the link should point, normally the center of the port</param>
    /// <param name="p">often this point is far away from the node, to give a general direction, particularly an orthogonal one</param>
    /// <param name="from">true if the link is coming out of the port; false if going to the port</param>
    /// <returns>the point in model coordinates of the intersection point on the edge of the port</returns>
    protected virtual Point GetLinkPointFromPoint(Node node, FrameworkElement port, Point focus, Point p, bool from) {
      if (node == null || port == null) return focus;
      if (!node.Visible) {
        node = node.FindVisibleNode(null);
        if (node == null) return focus;
        port = node.Port;
      }

      Rect portb = node.GetElementBounds(port);
      if (!portb.Contains(focus)) focus = new Point(portb.X + portb.Width / 2, portb.Y + portb.Height / 2);
      // if the "distant" point is inside the port, just use the port's center, to reduce visual confusion
      if (portb.Contains(p)) return focus;

      // otherwise get the closest edge point on the line from P to CTR
      Point nodeLoc = node.Position;
      Transform portToNode = null;
      if (port != node) {
        var portsParent = Diagram.FindParent<UIElement>(port);
        if (portsParent != null && portsParent != node) portToNode = DiagramPanel.CoordinatesTransform(node, portsParent) as Transform;
      }
      // Convert p1/p2 to node coordinates
      Point p1 = new Point(p.X - nodeLoc.X, p.Y - nodeLoc.Y);
      Point p2 = new Point(focus.X - nodeLoc.X, focus.Y - nodeLoc.Y);
      // get the element whose edge we'll use (which is normally the same element)
      FrameworkElement ext = GetExtension(port, node.VisualElement, from);
      Point result = GetClosestVisualPoint(ext, p2, node, p1, p2, portToNode, null, null);
      result.X += nodeLoc.X;
      result.Y += nodeLoc.Y;
      return result;
    }

    // Recursively explores all children of [fe] and returns the closest intersection point along the long from [p] (the point to be closest to) to the port
    private Point GetClosestVisualPoint(FrameworkElement fe, Point closestPoint, Node node, Point p, Point portsCenter, Transform parentTrans, List<Geometry> parentClippingRegions, List<TransformGroup> parentClippingTransforms) {
      if (fe == null) return closestPoint;
      if (fe.Visibility != Visibility.Visible) return closestPoint; // If the parent's not visible, neither are its children
      Size size = node.GetEffectiveSize(fe);

      // intersects bounding rect?
      if (size.Width == 0 || size.Height == 0 || Double.IsNaN(size.Width) || Double.IsNaN(size.Height) || size.IsEmpty) return closestPoint;

      TransformGroup trans = new TransformGroup();
      List<Point> temp = new List<Point>();
      bool isFilled = false;
      Geometry containment;
      var parent = Diagram.FindParent<UIElement>(fe);
      Transform t = (fe != node && parent != null) ? DiagramPanel.CoordinatesTransform(parent, fe) as Transform : null;
      if (t != null)
        trans.Children.Add(t);
      if (parentTrans != null)
        trans.Children.Add(parentTrans);

      List<Geometry> clippingRegions = null;
      List<TransformGroup> clippingTransforms = null;
      if (parentClippingRegions != null && parentClippingRegions.Count > 0) {
        clippingRegions = new List<Geometry>();
        clippingTransforms = new List<TransformGroup>();
        foreach (Geometry clip in parentClippingRegions)
          clippingRegions.Add(clip);
        foreach (TransformGroup tg in parentClippingTransforms)
          clippingTransforms.Add(tg);
      }
      Geometry clip2 = fe.Clip;
      if (clip2 != null) {
        if (clippingRegions == null) {
          clippingRegions = new List<Geometry>();
          clippingTransforms = new List<TransformGroup>();
        }
        clippingRegions.Add(clip2);
        TransformGroup nextTransform = new TransformGroup();
        nextTransform.Children.Add(trans);
        if (clip2.Transform != null)
          nextTransform.Children.Add(clip2.Transform);
        clippingTransforms.Add(nextTransform);
      }
      Geometry layoutClip = System.Windows.Controls.Primitives.LayoutInformation.GetLayoutClip(fe);
      if (layoutClip != null) {
        if (clippingRegions == null) {
          clippingRegions = new List<Geometry>();
          clippingTransforms = new List<TransformGroup>();
        }
        clippingRegions.Add(layoutClip);
        TransformGroup nextTransform = new TransformGroup();
        if (layoutClip.Transform != null)
          nextTransform.Children.Add(layoutClip.Transform);
        clippingTransforms.Add(nextTransform);
      }











      Matrix mat = trans.Value;
      Matrix invMat = trans.Value;
      if (!invMat.IsIdentity) Geo.Invert(ref invMat);

      containment = new RectangleGeometry();
      containment.SetValue(RectangleGeometry.RectProperty, new Rect(0, 0, size.Width, size.Height));

      //Inverse the transform to assume an angle of 0.0
      Point p1 = invMat.Transform(p);
      Point p2 = invMat.Transform(portsCenter);

      if (fe is Shape) {
        Shape cur = fe as Shape;
        if (cur is Ellipse) {
          Ellipse ell = cur as Ellipse;
          EllipseGeometry ellgeo = new EllipseGeometry();
          ellgeo.SetValue(EllipseGeometry.CenterProperty, new Point(size.Width / 2, size.Height / 2));
          ellgeo.SetValue(EllipseGeometry.RadiusXProperty, size.Width / 2);
          ellgeo.SetValue(EllipseGeometry.RadiusYProperty, size.Height / 2);
          temp = Geo.GetLineEllipseIntersections(p1, p2, ellgeo);

          isFilled = ell.Fill != null;
          containment = ellgeo;
        } else if (cur is Line) {
          Line line = cur as Line;
          LineGeometry linegeo = new LineGeometry();
          linegeo.SetValue(LineGeometry.StartPointProperty, new Point(line.X1, line.Y1));
          linegeo.SetValue(LineGeometry.EndPointProperty, new Point(line.X2, line.Y2));
          Geo.LineIntersectsWithSegment(p1, p2, linegeo.StartPoint, linegeo.EndPoint, out temp);

          isFilled = false; // lines can't be filled
          containment = linegeo;
        } else if (cur is Polyline) {
          Polyline polyline = cur as Polyline;
          PathGeometry polylinegeo = new PathGeometry();

          // Build path geometry out of the polyline
          PathFigureCollection pfc = new PathFigureCollection();
          PathSegmentCollection psc = new PathSegmentCollection();
          PathFigure pf = new PathFigure();
          Point last = new Point();
          if (polyline.Points.Count > 0)
            last = polyline.Points[0];
          pf.StartPoint = last;
          for (int i = 1; i < polyline.Points.Count; i++) {
            LineSegment ls = new LineSegment();
            ls.Point = polyline.Points[i];
            psc.Add(ls);
          }
          pf.Segments = psc;
          pfc.Add(pf);
          polylinegeo.Figures = pfc;

          Geo.GetPolyLineSegIntersections(p1, p2, polyline.Points[0], polyline.Points, out temp);

          isFilled = polyline.Fill != null;
          polylinegeo.FillRule = polyline.FillRule;
          containment = polylinegeo;
        } else if (cur is Rectangle) {
          Rectangle rect = cur as Rectangle;
          RectangleGeometry rectgeo = new RectangleGeometry();
          rectgeo.SetValue(RectangleGeometry.RectProperty, new Rect(0, 0, size.Width, size.Height));
          Geo.GetIntersectionsOnRect(rectgeo.Rect, p1, p2, out temp);

          isFilled = rect.Fill != null;
          containment = rectgeo;
        } else if (cur is Path) {
          Path path = cur as Path;
          ShapeGeomTransPoints(trans, ref mat, ref invMat, ref p1, ref p2, cur);
          Geo.GetIntersectionsOnGeometry(path.Data, p1, p2, out temp);
          isFilled = path.Fill != null;
          containment = path.Data;
        } else if (cur is Polygon) {
          Polygon polygon = cur as Polygon;
          PathGeometry polygongeo = new PathGeometry();

          // Build path geometry out of the polyline
          PathFigureCollection pfc = new PathFigureCollection();
          PathSegmentCollection psc = new PathSegmentCollection();
          PathFigure pf = new PathFigure();
          pf.IsClosed = true;
          Point last = new Point();
          if (polygon.Points.Count > 0) last = polygon.Points[0];
          pf.StartPoint = last;
          for (int i = 1; i < polygon.Points.Count; i++) {
            LineSegment ls = new LineSegment();
            ls.Point = polygon.Points[i];
            psc.Add(ls);
          }
          LineSegment endline = new LineSegment();
          endline.Point = last;
          psc.Add(endline); // make sure it's closed
          pf.Segments = psc;
          pfc.Add(pf);
          polygongeo.Figures = pfc;
          polygongeo.FillRule = polygon.FillRule;

          // Treat exactly like a Polyline since that's really all it is
          Geo.GetIntersectionsOnGeometry(polygongeo, p1, p2, out temp);

          isFilled = polygon.Fill != null;
          containment = polygongeo;










        } else {
          //?? handle unknown Shape classes
        }  // end of Shapes
      } else if (fe is Border) {
        Border cur = fe as Border;
        if (cur != null &&
            (cur.BorderThickness.Bottom + cur.BorderThickness.Top + cur.BorderThickness.Right + cur.BorderThickness.Left > 0
             || cur.Background != null)) {
          Geo.GetIntersectionsOnRect(new Rect(0, 0, size.Width, size.Height), p1, p2, out temp);
          isFilled = true;
        }






      } else if (fe is Panel) {
        // What kind of panel?
        Panel cur = fe as Panel;
        if (cur.Background != null) {
          Geo.GetIntersectionsOnRect(new Rect(0, 0, size.Width, size.Height), p1, p2, out temp);
          isFilled = true;
        }
      } else if (fe is Control) {
        Control c = fe as Control;
        if (c.Background != null)
          Geo.GetIntersectionsOnRect(new Rect(0, 0, size.Width, size.Height), p1, p2, out temp);
        isFilled = c.Background != null;
      } else if (fe is Image) {
        Image c = fe as Image;
        Geo.GetIntersectionsOnRect(new Rect(0, 0, size.Width, size.Height), p1, p2, out temp);
        isFilled = true;
      } else if (fe is TextBlock) {
        TextBlock c = fe as TextBlock;
        Geometry r = System.Windows.Controls.Primitives.LayoutInformation.GetLayoutClip(c);
        Point topleft = new Point(0, 0);
        Point botright = new Point(size.Width, size.Height);
        if (r != null) {
          topleft = new Point(r.Bounds.X, r.Bounds.Y);
          botright = new Point(topleft.X + r.Bounds.Width, topleft.Y + r.Bounds.Y);
        }
        Geo.GetIntersectionsOnRect(new Rect(topleft, botright), p1, p2, out temp);


        isFilled = true;



      }

      if ((clippingRegions == null || clippingRegions.Count == 0) && temp.Count > 0) {
        temp.Add(invMat.Transform(closestPoint)); // Transform old closest by invMat 
        closestPoint = Geo.FindClosest(temp, p1);
        closestPoint = mat.Transform(closestPoint); // Transform back (to node coordinates)
      } else if (temp.Count > 0) { // If there's clipping
        List<Point> invalid = new List<Point>();
        int clipIndex = 0;

        // Get all intersections with clip regions
        if (isFilled)
          foreach (Geometry clip in clippingRegions) {
            Matrix mReg = clippingTransforms[clipIndex++].Value;
            Matrix mInv = mReg;
            Geo.Invert(ref mInv);

            Point p1ClipCoords = mInv.Transform(mat.Transform(p1));
            Point p2ClipCoords = mInv.Transform(mat.Transform(p2));
            Geo.GetIntersectionsOnGeometry(clip, p1ClipCoords, p2ClipCoords, out invalid);

            foreach (Point intersect in invalid) {
              Point intersectContainmentCoords = mReg.Transform(intersect);
              intersectContainmentCoords = invMat.Transform(intersectContainmentCoords);
              temp.Add(intersectContainmentCoords);
            }
          }
        invalid.Clear();
        // Now check which are invalid (invalid if not in the clipping region AND containment region)






          foreach (Point intersect in temp) {
            bool valid = true;
            valid = Geo.GeometryContainsPoint(intersect, containment);

            if (valid) { // If it's in the containment, verify it is in the clipped region
              clipIndex = 0;
              foreach (Geometry clip in clippingRegions) {
                Matrix mReg = clippingTransforms[clipIndex++].Value;
                Matrix mInv = mReg;
                Geo.Invert(ref mInv);

                Point intersectClipCoords = mat.Transform(intersect);
                intersectClipCoords = mInv.Transform(intersectClipCoords);

                valid = Geo.GeometryContainsPoint(intersectClipCoords, clip);
                if (!valid)
                  break;
              }
            }

            if (!valid
                // Silverlight's gives false negatives SOMETIMES so it is conclusive if true, not if false
                && !Geo.OnscreenElementContainsPoint(intersect, fe)
              )
              invalid.Add(intersect);
          }


        // Now remove all invalid points from temp:
        foreach (Point invalidPoint in invalid)
          temp.Remove(invalidPoint);

        // Return the closest point to [p1] in containment coordinates
        temp.Add(invMat.Transform(closestPoint)); // Transform old closest by invMat 
        closestPoint = Geo.FindClosest(temp, p1);
        closestPoint = mat.Transform(closestPoint); // Transform back (to node coordinates)
      }

      if (!isFilled) {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(fe); i++) {
          FrameworkElement child = VisualTreeHelper.GetChild(fe, i) as FrameworkElement;
          if (child != null)
            closestPoint = GetClosestVisualPoint(child, closestPoint, node, p, portsCenter, trans, clippingRegions, clippingTransforms);
        }
      }

      return closestPoint;
    }

    //It is only necessary to account for the Shape.GeometryTransform property if cur is a Path or NodeShape.
    //Paths are defined for the default width and height, regardless of the actual values of the Width and Height properties.
    //The GeometryTranform transforms the Points from the Default Width/Height shape to the actual values of Width/Height.
    //This must be done to find the intersections on the Path correctly when Geo.GetIntersectionsOnGeometry is called below.
    private static void ShapeGeomTransPoints(TransformGroup trans, ref Matrix mat, ref Matrix invMat, ref Point p1, ref Point p2, Shape cur) {
      if (cur.Stretch != Stretch.None) {
        //MatrixTransform mt = cur.GeometryTransform as MatrixTransform;
        //if (mt != null) Diagram.Debug(cur.ToString() + " " + mt.Matrix.ToString());
        TransformGroup tg = new TransformGroup();
        tg.Children.Add(cur.GeometryTransform);
        if (!tg.Value.IsIdentity) {
          tg.Children.Clear();
          tg.Children.Add(cur.GeometryTransform);  //Scales between the default values and the set values of Width/Height
          tg.Children.Add(trans);  //Includes the ParentTransform. Any rotation is stored here.

          //Transform the points back into rotated coordinates.
          p1 = mat.Transform(p1);
          p2 = mat.Transform(p2);

          Matrix value = tg.Value;
          mat = value;
          invMat = mat;
          Geo.Invert(ref invMat);
          //Transforms Points into the coordiates of a Shape with default values of Width/Height and an Angle of 0.0
          p1 = invMat.Transform(p1);
          p2 = invMat.Transform(p2);
        }
      }
    }

    internal LinkInfo FindExistingLinkInfo(FrameworkElement port, Link link) {  // does not update
      Knot knot = Node.GetPortInfo(port);
      if (knot != null) return knot.FindLinkInfo(link);
      return null;
    }

    private LinkInfo FindLinkInfo(FrameworkElement port, Link link) {  // updates if needed
      Knot knot = Node.GetPortInfo(port);
      if (knot == null) {
        knot = new Knot();
        Node.SetPortInfo(port, knot);
        knot.Extension = port;
      }
      return knot.FindLinkInfo(link);
    }

    private LinkInfo FindLinkInfo(FrameworkElement virtualPort, FrameworkElement port, Link link) {  // updates if needed
      Knot knot = Node.GetPortInfo(port);
      if (knot == null) {
        knot = new Knot();
        Node.SetPortInfo(port, knot);
      }
      // update the Extension every time
      knot.Extension = virtualPort;
      return knot.FindLinkInfo(link);
    }

    /// <summary>
    /// Compute the point on a node/port at which the route of a link should end.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="port">the <c>FrameworkElement</c> representing a port on the node</param>
    /// <param name="spot">a <see cref="Spot"/> value describing where the link should connect</param>
    /// <param name="from">true if the link is coming out of the port; false if going to the port</param>
    /// <param name="ortho">whether the link should have orthogonal segments</param>
    /// <param name="othernode">the node at the other end of the link</param>
    /// <param name="otherport">the FrameworkElement port at the other end of the link</param>
    /// <returns></returns>
    public virtual Point GetLinkPoint(Node node, FrameworkElement port, Spot spot, bool from, bool ortho, Node othernode, FrameworkElement otherport) {
      FrameworkElement port1 = GetExtension(port, node.VisualElement, from);

      Point focus;
      Point far = new Point(Double.NaN, Double.NaN);
      if (spot.IsSpot) {  // specific spot on element
        focus = node.GetElementPoint(port, spot);
        // if there's no port extension, just return the FOCUS point
        if (port1 == port) return focus;

        Rect farb = node.Bounds;
        Geo.Inflate(ref farb, 10, 10);
        far = spot.PointInRect(farb);
      } else if (spot.IsSide) {  // a side of an element
        LinkInfo info = FindLinkInfo(port1, port, this.Link);
        if (info != null) {
          focus = info.LinkPoint;  // specific value for that link
          // if there's no port extension, just return the FOCUS point
          if (port1 == port) return focus;

          Rect farb = node.Bounds;
          Geo.Inflate(ref farb, 10, 10);
          switch (info.Side) {
            case Spot.MTop: far = Spot.MiddleTop.PointInRect(farb); break;
            case Spot.MLeft: far = Spot.MiddleLeft.PointInRect(farb); break;
            case Spot.MRight: far = Spot.MiddleRight.PointInRect(farb); break;
            case Spot.MBottom: far = Spot.MiddleBottom.PointInRect(farb); break;
          }
        }
        // no LinkInfo? assume focusing on center of port
        focus = node.GetElementPoint(port, Spot.Center);
      } else {
        // assume focusing on center of port
        focus = node.GetElementPoint(port, Spot.Center);
      }

      if (Double.IsNaN(far.X)) {  // calculate edge intersection
        // no defined spot or side -- need to calculate nearest intersection
        // of port's edge with line to focus point from some "far" point
        if (this.PointsCount > (ortho ? 6 : 2)) {  // try using the existing points in the route
          far = from ? GetPoint(1) : GetPoint(this.PointsCount-2);
          if (ortho) {
            far = OrthoPointToward(focus, far);
          } else {
            // if the far point is inside the port, try another point "farther" along the route
            Rect b = node.GetElementBounds(port1);
            int i = 2;
            while (b.Contains(far) && i < this.PointsCount) {
              far = from ? GetPoint(i) : GetPoint(this.PointsCount-1-i);
              i++;
            }
            if (i >= this.PointsCount) far = othernode.GetElementPoint(otherport, Spot.Center);
          }
        } else {  // otherwise use the center of the other port
          far = othernode.GetElementPoint(otherport, Spot.Center);
          if (ortho) {
            far = OrthoPointToward(focus, far);
          }
        }
      }

      Point p = GetLinkPointFromPoint(node, port, focus, far, from);
      return p;
    }

    /// <summary>
    /// Compute the direction in which a link should go from a given point.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="port">the <c>FrameworkElement</c> representing a port on the node</param>
    /// <param name="linkpoint"></param>
    /// <param name="spot">a <see cref="Spot"/> value describing where the link should connect</param>
    /// <param name="from">true if the link is coming out of the port; false if going to the port</param>
    /// <param name="ortho">whether the link should have orthogonal segments</param>
    /// <param name="othernode">the node at the other end of the link</param>
    /// <param name="otherport">the FrameworkElement port at the other end of the link</param>
    /// <returns>an angle in degrees</returns>
    /// <remarks>
    /// <para>
    /// If the <paramref name="spot"/> is a specific spot or is a side,
    /// this will return the corresponding multiple of 90 degrees.
    /// For example, the spot <see cref="Spot.MiddleLeft"/> will return 180 and a spot near the bottom will return 90.
    /// For spots that are exactly toward the corners, this will return a multiple of 45 degrees.
    /// For example, the spot <see cref="Spot.BottomLeft"/> will return 135.
    /// </para>
    /// <para>
    /// If <see cref="FromEndSegmentDirection"/> or <see cref="ToEndSegmentDirection"/>
    /// is <see cref="LinkEndSegmentDirection.RotatedNode"/>
    /// or <see cref="LinkEndSegmentDirection.RotatedNodeOrthogonal"/>,
    /// these values are rotated according to the <see cref="Node.RotationAngle"/>.
    /// </para>
    /// </remarks>
    protected virtual double GetLinkDirection(Node node, FrameworkElement port, Point linkpoint, Spot spot, bool from, bool ortho, Node othernode, FrameworkElement otherport) {
      double angle = GetLinkDirection1(node, port, linkpoint, spot, from, ortho, othernode, otherport);
      switch (from ? this.FromEndSegmentDirection : this.ToEndSegmentDirection) {
        case LinkEndSegmentDirection.Absolute:
          break;
        case LinkEndSegmentDirection.RotatedNode: {
            double rot = node.RotationAngle;
            angle += rot;
            if (angle >= 360) angle -= 360;
            break;
          }
        case LinkEndSegmentDirection.RotatedNodeOrthogonal: {
            double rot = node.RotationAngle;
            if (45 <= rot && rot < 135) angle += 90;
            else if (135 <= rot && rot < 225) angle += 180;
            else if (225 <= rot && rot < 315) angle += 270;
            if (angle >= 360) angle -= 360;
            break;
          }
      }
      return angle;
    }

    private double GetLinkDirection1(Node node, FrameworkElement port, Point linkpoint, Spot spot, bool from, bool ortho, Node othernode, FrameworkElement otherport) {
      if (spot.IsSpot) {  // specific spot is defined
        if (spot.X > spot.Y) {
          if (spot.X > 1-spot.Y) {
            return 0;
          } else if (spot.X < 1-spot.Y) {
            return 270;
          } else {
            return 315;
          }
        } else if (spot.X < spot.Y) {
          if (spot.X > 1-spot.Y) {
            return 90;
          } if (spot.X < 1-spot.Y) {
            return 180;
          } else {
            return 135;
          }
        } else {
          if (spot.X < 0.5) {
            return 225;
          } else if (spot.X > 0.5) {
            return 45;
          } else {
            return 0;
          }
        }
      }
      if (spot.IsSide) {
        LinkInfo info = FindLinkInfo(port, this.Link);
        if (info != null) {
          switch (info.Side) {
            case Spot.MTop: return 270;
            case Spot.MLeft: return 180;
            default:
            case Spot.MRight: return 0;
            case Spot.MBottom: return 90;
          }
        }
      }
      // no defined spot -- guess direction based on other points
      // in the route, if any
      Point c = node.GetElementPoint(port, Spot.Center);
      Point far;
      if (this.PointsCount > (ortho ? 6 : 2)) {
        far = from ? GetPoint(1) : GetPoint(this.PointsCount-2);
        if (ortho) {
          far = OrthoPointToward(c, far);
        } else {
          far = linkpoint;
        }
      } else {
        far = othernode.GetElementPoint(otherport, Spot.Center);
      }
      if (Math.Abs(far.X-c.X) > Math.Abs(far.Y-c.Y)) {
        if (far.X >= c.X)
          return 0;
        else
          return 180;
      } else {
        if (far.Y >= c.Y)
          return 90;
        else
          return 270;
      }
    }

    /// <summary>
    /// Get the length of the end segment, typically a short distance.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="port">the <c>FrameworkElement</c> representing a port on the node</param>
    /// <param name="spot">a <see cref="Spot"/> value describing where the link should connect</param>
    /// <param name="from">true if the link is coming out of the port; false if going to the port</param>
    /// <returns></returns>
    protected virtual double GetEndSegmentLength(Node node, FrameworkElement port, Spot spot, bool from) {
      if (spot.IsSide) {
        LinkInfo info = FindLinkInfo(port, this.Link);
        if (info != null) return info.EndSegmentLength;  // specific value for that link
      }
      double esl;
      if (from) {
        esl = this.FromEndSegmentLength;
        if (Double.IsNaN(esl)) {
          esl = Node.GetFromEndSegmentLength(port);
        }
      } else {
        esl = this.ToEndSegmentLength;
        if (Double.IsNaN(esl)) {
          esl = Node.GetToEndSegmentLength(port);
        }
      }
      if (Double.IsNaN(esl)) esl = 10;
      return esl;
    }

    internal Spot GetFromSpot() {  // also used by LayeredDigraphLayout
      Spot s = this.FromSpot;
      if (s.IsDefault) {
        Link link = this.Link;
        if (link != null) {
          FrameworkElement port = link.FromPort;
          if (port != null) {
            s = Node.GetFromSpot(port);  // normally, get Spot from the port
          }
        }
      }
      return s;
    }

    internal Spot GetToSpot() {  // also used by LayeredDigraphLayout
      Spot s = this.ToSpot;
      if (s.IsDefault) {
        Link link = this.Link;
        if (link != null) {
          FrameworkElement port = link.ToPort;
          if (port != null) {
            s = Node.GetToSpot(port);  // normally, get Spot from the port
          }
        }
      }
      return s;
    }

    private FrameworkElement GetExtension(FrameworkElement port, FrameworkElement noderoot, bool from) {
      return port;
      //if (port == null || port == noderoot) return noderoot;

      //int idx = (from ? this.FromExtension : this.ToExtension);
      //if (idx < 0) idx = Node.GetPortExtension(port);  // normally, get PortExtension from the port

      //if (idx >= 99) return noderoot;

      //FrameworkElement p = port;
      //for (int i = 0; i < idx; i++) {
      //  if (p == noderoot) break;
      //  p = Diagram.FindParent<FrameworkElement>(p);
      //}
      //return p;
    }


    private LinkCurve GetCurve() {
      if (this.EffectiveBezier) return LinkCurve.Bezier;
      return this.Curve;
    }


    /// <summary>
    /// Returns the <see cref="Corner"/>, if it's a non-negative number, or else 10.
    /// </summary>
    /// <returns></returns>
    protected virtual double ComputeCorner() {
      double c = this.Corner;
      if (Curve == LinkCurve.Bezier) return 0;
      if (Double.IsNaN(c) || c < 0) c = 10;
      return c;
    }

    /// <summary>
    /// Returns the <see cref="Curviness"/>, if it's a number,
    /// or else a computed value based on how many links connect this pair of nodes/ports.
    /// </summary>
    /// <returns></returns>
    protected virtual double ComputeCurviness() {
      double c = this.Curviness;
      if (Double.IsNaN(c)) {
        Link link = this.Link;
        int bundleindex = link.BundleIndex;
        if (bundleindex != 0) {
          LinkBundle bundle = link.Bundle;
          double spacing = (bundle != null ? bundle.Spacing : 10);
          int i = Math.Abs(bundleindex);
          c = spacing/2 + (i-1)/2 * spacing;  // i.e. +/- 5/15/25/etc from center
          if (i%2 == 0) c = -c;  // alternate sides
          if (bundleindex < 0) c = -c;  // other direction
        } else {
          c = 10;
        }
      }
      return c;
    }

    /// <summary>
    /// Returns true if an extra or a different point is needed based on <see cref="Curviness"/>.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// This is only called when the <see cref="Curve"/> is not <see cref="LinkCurve.Bezier"/>.
    /// </remarks>
    protected virtual bool HasCurviness() {
      double c = this.Curviness;
      return !Double.IsNaN(c) || (this.Link.BundleIndex != 0 && !this.Orthogonal);
    }


    // simple single segment stroke, unless there are duplicate links that would be on the same path
    private void CalculateLineNoSpot(Node fromnode, FrameworkElement fromport, Node tonode, FrameworkElement toport) {
      Point frompt = GetLinkPoint(fromnode, fromport, Spot.None, true, false, tonode, toport);
      Point topt = GetLinkPoint(tonode, toport, Spot.None, false, false, fromnode, fromport);
      ClearPoints();
      if (HasCurviness()) {
        double Dx = topt.X-frompt.X;
        double Dy = topt.Y-frompt.Y;
        double rad = ComputeCurviness();
        double off = Math.Abs(rad);
        if (rad < 0) off = -off;

        double Mx = frompt.X + Dx/2;
        double My = frompt.Y + Dy/2;
        double C1x = Mx;
        double C1y = My;
        if (Geo.IsApprox(Dy, 0)) {
          if (Dx > 0)
            C1y -= off;
          else
            C1y += off;
        } else {
          double slope = -Dx/Dy;
          double E = Math.Sqrt(off*off / (slope*slope + 1));
          if (rad < 0) E = -E;
          C1x = (Dy < 0 ? -1 : 1) * E + Mx;
          C1y = slope*(C1x-Mx) + My;
        }
        AddPoint(frompt);
        AddPoint(new Point(C1x, C1y));
        AddPoint(topt);
        //??? adjust end points so that they appear to come from the control points
        //SetPoint(0, GetLinkPoint(fromnode, fromport, Spot.None, true, false, tonode, toport));
        //SetPoint(2, GetLinkPoint(tonode, toport, Spot.None, false, false, fromnode, fromport));
      } else {
        AddPoint(frompt);
        AddPoint(topt);
      }
    }

    // curved stroke (four points == one segment)
    private void CalculateBezierNoSpot(Node fromnode, FrameworkElement fromport, Node tonode, FrameworkElement toport) {
      Point pA = GetLinkPoint(fromnode, fromport, Spot.None, true, false, tonode, toport);
      Point pB = GetLinkPoint(tonode, toport, Spot.None, false, false, fromnode, fromport);

      double Dx = pB.X-pA.X;
      double Dy = pB.Y-pA.Y;
      double rad = ComputeCurviness();
      double slope = 0;  // can't calculate this now, if Dy is close to zero
      double E = 0;

      double Mx = pA.X + Dx/3;
      double My = pA.Y + Dy/3;
      double C1x = Mx;
      double C1y = My;
      if (Geo.IsApprox(Dy, 0)) {
        if (Dx > 0)
          C1y -= rad;
        else
          C1y += rad;
      } else {
        slope = -Dx/Dy;
        E = Math.Sqrt(rad*rad / (slope*slope + 1));
        if (rad < 0) E = -E;
        C1x = (Dy < 0 ? -1 : 1) * E + Mx;
        C1y = slope*(C1x-Mx) + My;
      }

      Mx = pA.X + 2*Dx/3;
      My = pA.Y + 2*Dy/3;
      double C2x = Mx;
      double C2y = My;
      if (Geo.IsApprox(Dy, 0)) {
        if (Dx > 0)
          C2y -= rad;
        else
          C2y += rad;
      } else {
        // slope and E have been initialized earlier, when Dy != 0
        C2x = (Dy < 0 ? -1 : 1) * E + Mx;
        C2y = slope*(C2x-Mx) + My;
      }

      ClearPoints();
      AddPoint(pA);
      AddPoint(new Point(C1x, C1y));
      AddPoint(new Point(C2x, C2y));
      AddPoint(pB);
      // adjust end points so that they appear to come from the control points
      SetPoint(0, GetLinkPoint(fromnode, fromport, Spot.None, true, false, tonode, toport));
      SetPoint(3, GetLinkPoint(tonode, toport, Spot.None, false, false, fromnode, fromport));
      //String msg = ""; for (int i = 0; i < this.PointsCount; i++) msg += Diagram.Str(GetPoint(i)); Diagram.Debug("CalculateBezierNoSpots: " + msg);
    }

    /// <summary>
    /// Adjust all of the existing points in this link's stroke in an inclusive range
    /// according to new first and last stroke points.
    /// </summary>
    /// <param name="startIndex">the zero-based index of the first point to be changed, to be
    /// the value of <paramref name="newFromPoint"/></param>
    /// <param name="newFromPoint"></param>
    /// <param name="endIndex">the zero-based index of the last point to be changed, to be
    /// the value of <paramref name="newToPoint"/></param>
    /// <param name="newToPoint"></param>
    /// <value>
    /// This method should return true if the stroke points were adjusted.  Return false
    /// to tell <see cref="ComputePoints"/> to plot the standard path.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is primarily useful to help maintain intermediate inflection points
    /// in a link when one or both ports moves.
    /// By default this just calls <see cref="RescalePoints"/>, <see cref="StretchPoints"/>,
    /// or <see cref="ModifyEndPoints"/>.
    /// This method is not called when there are no existing points to be adjusted
    /// or when <see cref="Adjusting"/> is <see cref="LinkAdjusting.None"/>.
    /// </para>
    /// <para>
    /// When this link is <see cref="Orthogonal"/>, an <see cref="Adjusting"/> of
    /// <see cref="LinkAdjusting.Scale"/> will just return false to result in
    /// the standard orthogonal path.
    /// An <see cref="Adjusting"/> of <see cref="LinkAdjusting.Stretch"/>
    /// for an orthogonal link is treated as if it were <see cref="LinkAdjusting.End"/>.
    /// </para>
    /// </remarks>
    protected virtual bool AdjustPoints(int startIndex, Point newFromPoint, int endIndex, Point newToPoint) {
      LinkAdjusting s = this.Adjusting;
      if (this.Orthogonal) {
        if (s == LinkAdjusting.Scale)
          return false;
        if (s == LinkAdjusting.Stretch)
          s = LinkAdjusting.End;
      }
      switch (s) {
        case LinkAdjusting.Scale: return RescalePoints(startIndex, newFromPoint, endIndex, newToPoint);
        case LinkAdjusting.Stretch: return StretchPoints(startIndex, newFromPoint, endIndex, newToPoint);
        case LinkAdjusting.End: return ModifyEndPoints(startIndex, newFromPoint, endIndex, newToPoint);
        default: return false;
      }
    }

    /// <summary>
    /// Maintain the same shape for the stroke, but scale and rotate according to
    /// new points <paramref name="newFromPoint"/> and <paramref name="newToPoint"/>.
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="newFromPoint"></param>
    /// <param name="endIndex"></param>
    /// <param name="newToPoint"></param>
    /// <value>
    /// This method should return true if the stroke points were adjusted.  Return false
    /// to tell <see cref="AdjustPoints"/> and <see cref="ComputePoints"/> to plot
    /// the standard stroke path.
    /// </value>
    /// <remarks>
    /// The <paramref name="startIndex"/> point should be set to <paramref name="newFromPoint"/>,
    /// and the <paramref name="endIndex"/> point should be set to <paramref name="newToPoint"/>,
    /// and all the intermediate points should be scaled and rotated accordingly to
    /// maintain the same shape as the original set of points from <paramref name="startIndex"/>
    /// to <paramref name="endIndex"/>, inclusive.
    /// <see cref="AdjustPoints"/> calls this method when <see cref="Adjusting"/>
    /// is <see cref="LinkAdjusting.Scale"/>.
    /// This method should not be used when <see cref="Orthogonal"/> is true.
    /// </remarks>
    protected virtual bool RescalePoints(int startIndex, Point newFromPoint, int endIndex, Point newToPoint) {
      Point oldFromPt = GetPoint(startIndex);
      Point oldToPt = GetPoint(endIndex);
      if (oldFromPt == newFromPoint && oldToPt == newToPoint) return true;

      double Ax = oldFromPt.X;
      double Ay = oldFromPt.Y;
      double Bx = oldToPt.X;
      double By = oldToPt.Y;
      double Dx = Bx-Ax;
      double Dy = By-Ay;
      double oldDist = Math.Sqrt(Dx*Dx + Dy*Dy);
      if (Geo.IsApprox(oldDist, 0))
        return true;
      double oldAngle;
      if (Geo.IsApprox(Dx, 0)) {
        if (Dy < 0)
          oldAngle = -Math.PI/2;
        else
          oldAngle = Math.PI/2;
      } else {
        oldAngle = Math.Atan(Dy/Math.Abs(Dx));
        if (Dx < 0)
          oldAngle = Math.PI-oldAngle;
      }

      double A2x = newFromPoint.X;
      double A2y = newFromPoint.Y;
      double B2x = newToPoint.X;
      double B2y = newToPoint.Y;
      double D2x = B2x-A2x;
      double D2y = B2y-A2y;
      double newDist = Math.Sqrt(D2x*D2x + D2y*D2y);
      double newAngle;
      if (Geo.IsApprox(D2x, 0)) {
        if (D2y < 0)
          newAngle = -Math.PI/2;
        else
          newAngle = Math.PI/2;
      } else {
        newAngle = Math.Atan(D2y/Math.Abs(D2x));
        if (D2x < 0)
          newAngle = Math.PI-newAngle;
      }

      double DistRatio = (newDist/oldDist);
      double AngleDiff = (newAngle-oldAngle);

      SetPoint(startIndex, newFromPoint);
      for (int i = startIndex+1; i < endIndex; i++) {
        Point p = GetPoint(i);
        Dx = p.X-Ax;
        Dy = p.Y-Ay;
        double pDist = Math.Sqrt(Dx*Dx + Dy*Dy);
        if (Geo.IsApprox(pDist, 0))
          continue;
        double pAngle;
        if (Geo.IsApprox(Dx, 0)) {
          if (Dy < 0)
            pAngle = -Math.PI/2;
          else
            pAngle = Math.PI/2;
        } else {
          pAngle = Math.Atan(Dy/Math.Abs(Dx));
          if (Dx < 0)
            pAngle = Math.PI-pAngle;
        }

        double p2Angle = pAngle+AngleDiff;
        double p2Dist = pDist*DistRatio;
        double P2x = A2x+p2Dist*Math.Cos(p2Angle);
        double P2y = A2y+p2Dist*Math.Sin(p2Angle);
        SetPoint(i, new Point(P2x, P2y));
      }
      SetPoint(endIndex, newToPoint);
      return true;
    }

    /// <summary>
    /// Stretch the points of this stroke by interpolating the points
    /// from <paramref name="startIndex"/> to <paramref name="endIndex"/> between the
    /// new points <paramref name="newFromPoint"/> and <paramref name="newToPoint"/>.
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="newFromPoint"></param>
    /// <param name="endIndex"></param>
    /// <param name="newToPoint"></param>
    /// <value>
    /// This method should return true if the stroke points were adjusted.  Return false
    /// to tell <see cref="AdjustPoints"/> and <see cref="ComputePoints"/> to plot
    /// the standard stroke path.
    /// </value>
    /// <remarks>
    /// The <paramref name="startIndex"/> point should be set to <paramref name="newFromPoint"/>,
    /// and the <paramref name="endIndex"/> point should be set to <paramref name="newToPoint"/>,
    /// and all the intermediate points should be interpolated linearly to
    /// stretch or compress the original set of points from <paramref name="startIndex"/>
    /// to <paramref name="endIndex"/>, inclusive, along each of the X and Y dimensions.
    /// <see cref="AdjustPoints"/> calls this method when <see cref="Adjusting"/>
    /// is <see cref="LinkAdjusting.Stretch"/>.
    /// This method should not be used when <see cref="Orthogonal"/> is true.
    /// </remarks>
    protected virtual bool StretchPoints(int startIndex, Point newFromPoint, int endIndex, Point newToPoint) {
      Point a = GetPoint(startIndex);
      Point b = GetPoint(endIndex);
      if (a == newFromPoint && b == newToPoint) return true;

      double Ax = a.X;
      double Ay = a.Y;
      double Bx = b.X;
      double By = b.Y;
      double L = ((Bx-Ax) * (Bx-Ax) + (By-Ay) * (By-Ay));

      double Cx = newFromPoint.X;
      double Cy = newFromPoint.Y;
      double Dx = newToPoint.X;
      double Dy = newToPoint.Y;
      double M = 0;
      double m2 = 1;
      if (Dx-Cx != 0)
        M = (Dy-Cy)/(Dx-Cx);
      else
        M = 9e9;
      if (M != 0)
        m2 = Math.Sqrt(1+(1/(M*M)));

      SetPoint(startIndex, newFromPoint);
      for (int i = startIndex+1; i < endIndex; i++) {
        Point p = GetPoint(i);
        double Px = p.X;
        double Py = p.Y;

        double Q = 0.5;
        if (L != 0)
          Q = ((Ax-Px) * (Ax-Bx) + (Ay-Py) * (Ay-By)) / L;

        // find point on old line
        double Vx = Ax + Q * (Bx-Ax);
        double Vy = Ay + Q * (By-Ay);
        // distance from P to point V, on old line
        double dV = Math.Sqrt((Px-Vx)*(Px-Vx) + (Py-Vy)*(Py-Vy));
        if (Py < M*(Px-Vx) + Vy)
          dV = -dV;
        if (M > 0)
          dV = -dV;

        // find point on new line
        double Wx = Cx + Q * (Dx-Cx);
        double Wy = Cy + Q * (Dy-Cy);

        if (M != 0) {
          // compute new point for P off of new line, distance dV from W
          double x = Wx + dV/m2;
          double y = Wy - (x-Wx)/M;
          SetPoint(i, new Point(x, y));
        } else {
          SetPoint(i, new Point(Wx, (Wy+dV)));
        }
      }
      SetPoint(endIndex, newToPoint);
      return true;
    }

    /// <summary>
    /// Modify only the end points of this stroke to match any new
    /// <paramref name="newFromPoint"/> or <paramref name="newToPoint"/> points;
    /// intermediate points are not changed.
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="newFromPoint"></param>
    /// <param name="endIndex"></param>
    /// <param name="newToPoint"></param>
    /// <value>
    /// This method should return true if the stroke points were adjusted.  Return false
    /// to tell <see cref="AdjustPoints"/> and <see cref="ComputePoints"/> to plot
    /// the standard stroke path.
    /// </value>
    /// <remarks>
    /// The <paramref name="startIndex"/> point should be set to <paramref name="newFromPoint"/>,
    /// and the <paramref name="endIndex"/> point should be set to <paramref name="newToPoint"/>,
    /// and the intermediate points should be not be changed unless needed to maintain
    /// orthogonality when <see cref="Orthogonal"/> is true.
    /// </remarks>
    protected virtual bool ModifyEndPoints(int startIndex, Point newFromPoint, int endIndex, Point newToPoint) {
      if (this.Orthogonal) {
        Point b = GetPoint(startIndex+1);
        Point c = GetPoint(startIndex+2);
        if (Geo.IsApprox(b.X, c.X) && !Geo.IsApprox(b.Y, c.Y)) {
          SetPoint(startIndex+1, new Point(b.X, newFromPoint.Y));
        } else if (Geo.IsApprox(b.Y, c.Y)) {
          SetPoint(startIndex+1, new Point(newFromPoint.X, b.Y));
        }
        b = GetPoint(endIndex-1);
        c = GetPoint(endIndex-2);
        if (Geo.IsApprox(b.X, c.X) && !Geo.IsApprox(b.Y, c.Y)) {
          SetPoint(endIndex-1, new Point(b.X, newToPoint.Y));
        } else if (Geo.IsApprox(b.Y, c.Y)) {
          SetPoint(endIndex-1, new Point(newToPoint.X, b.Y));
        }
      }
      SetPoint(startIndex, newFromPoint);
      SetPoint(endIndex, newToPoint);
      return true;
    }

    internal void MaybeDelayRouting() {
      this.DelayedRouting = (this.Routing == LinkRouting.AvoidsNodes);
    }

    /// <summary>
    /// This method is called by <see cref="ComputePoints"/> when <see cref="Orthogonal"/>
    /// is true and at least one port has a link spot that is not <see cref="Spot.IsNoSpot"/>.
    /// </summary>
    /// <param name="startFrom">
    /// this point will already have been added to the stroke by <see cref="ComputePoints"/> before calling this method
    /// </param>
    /// <param name="fromDir">normally 0, 90, 180, or 270 degrees</param>
    /// <param name="endTo">
    /// <see cref="ComputePoints"/> will add this point after calling this method
    /// </param>
    /// <param name="toDir">normally 0, 90, 180, or 270 degrees</param>
    /// <param name="fromnode">the <see cref="Node"/> that the link is coming from</param>
    /// <param name="tonode">the <see cref="Node"/> that the link is going to</param>
    /// <remarks>
    /// <see cref="ComputePoints"/> is responsible for adding the first two
    /// and the last two points of the stroke, including <paramref name="startFrom"/> and <paramref name="endTo"/>.
    /// This method is responsible for adding any additional points in the middle of the stroke.
    /// This method calls <see cref="GetMidOrthoPosition"/> to determine the
    /// distance of the middle segment between the two ports.
    /// It also tries to avoid the source node and the destination node.
    /// When the <see cref="Routing"/> property is not <see cref="LinkRouting.Normal"/>,
    /// this method uses another, more computationally expensive,
    /// method for determining the proper path of the link, which may have many segments.
    /// </remarks>
    protected virtual void AddOrthoPoints(Point startFrom, double fromDir, Point endTo, double toDir, Node fromnode, Node tonode) {
      if (RIGHT-45 <= fromDir && fromDir < RIGHT+45)
        fromDir = RIGHT;
      else if (DOWN-45 <= fromDir && fromDir < DOWN+45)
        fromDir = DOWN;
      else if (LEFT-45 <= fromDir && fromDir < LEFT+45)
        fromDir = LEFT;
      else
        fromDir = UP;
      if (RIGHT-45 <= toDir && toDir < RIGHT+45)
        toDir = RIGHT;
      else if (DOWN-45 <= toDir && toDir < DOWN+45)
        toDir = DOWN;
      else if (LEFT-45 <= toDir && toDir < LEFT+45)
        toDir = LEFT;
      else
        toDir = UP;

      Point s = startFrom;
      Point t = endTo;

      Rect fromR = fromnode.GetElementBounds(fromnode.VisualElement);  // handle rotated nodes
      Rect toR = tonode.GetElementBounds(tonode.VisualElement);
      if (Double.IsNaN(fromR.X) || Double.IsNaN(fromR.Y) || Double.IsNaN(toR.X) || Double.IsNaN(toR.Y)) return;
      //Diagram.Debug(Diagram.Str(toR) + Diagram.Str(tonode.Position) + Diagram.Str(tonode.Location));  //??
      Geo.Inflate(ref fromR, 8, 8);
      Geo.Inflate(ref toR, 8, 8);
      fromR = Geo.Union(fromR, startFrom);
      toR = Geo.Union(toR, endTo);
      DiagramPanel panel = (this.Link != null ? this.Link.Panel : null);
      if ((this.Routing == LinkRouting.AvoidsNodes) && panel != null &&
          !Geo.Contains(fromR, endTo) && !Geo.Contains(toR, startFrom) &&  // ignore AvoidsNodes if overlapping
          fromnode != tonode && !this.Link.Layer.IsTemporary  // ignore AvoidsNodes if self-loop or temporary
          && !this.DelayedRouting  // or when rebuilding
        ) {
        //Diagram.Debug("AvoidsNodes route search: " + panel.Diagram.PartManager.NodesCount.ToString() + " " + this.Link.ToString());
        PositionArray positions = panel.GetPositions(true, this.Link, null);
        // first try to find a path that is within the bounds of the
        // rectangle formed by the two nodes--this should cover most cases
        // more efficiently than searching the whole document
        Rect minbounds = Geo.Union(fromR, toR);
        Geo.Inflate(ref minbounds, positions.CellSize.Width*2, positions.CellSize.Height*2);
        positions.Propagate(startFrom, fromDir, endTo, toDir, minbounds);  // may set .Abort
        int endval = positions.GetDist(endTo.X, endTo.Y);
        if (!positions.Abort && endval == PositionArray.MaxDistance) {
          // try again with larger area, which should cover a lot of cases where
          // there is another node overlapping with either the source or the destination node
          positions.ClearAllUnoccupied();
          double margin = positions.SmallMargin;
          Geo.Inflate(ref minbounds, positions.CellSize.Width*margin, positions.CellSize.Height*margin);
          positions.Propagate(startFrom, fromDir, endTo, toDir, minbounds);  // may set .Abort
          endval = positions.GetDist(endTo.X, endTo.Y);
        }
        if (!positions.Abort && endval == PositionArray.MaxDistance) {
          // try again with larger area, which should cover a lot of cases where
          // there is another node overlapping with either the source or the destination node
          positions.ClearAllUnoccupied();
          double margin = positions.LargeMargin;
          Geo.Inflate(ref minbounds, positions.CellSize.Width*margin, positions.CellSize.Height*margin);
          positions.Propagate(startFrom, fromDir, endTo, toDir, minbounds);  // may set .Abort
          endval = positions.GetDist(endTo.X, endTo.Y);
        }
        if (!positions.Abort && endval == PositionArray.MaxDistance && positions.WholeDocument) {
          // try again with larger area, the whole document
          positions.ClearAllUnoccupied();
          positions.Propagate(startFrom, fromDir, endTo, toDir, positions.Bounds);  // may set .Abort
          endval = positions.GetDist(endTo.X, endTo.Y);
        }
        // if the propagation succeeded, get the points of the stroke
        if (!positions.Abort && endval < PositionArray.MaxDistance && !positions.IsOccupied(endTo.X, endTo.Y)) {
          TraversePositions(positions, endTo.X, endTo.Y, toDir, true);
          // hook up the first segment with the grid-aligned points produced by TraversePositions
          Point two = GetPoint(2);
          if (this.PointsCount < 4) {
            if (fromDir == RIGHT || fromDir == LEFT) {
              two.X = startFrom.X;
              two.Y = endTo.Y;
            } else {
              two.X = endTo.X;
              two.Y = startFrom.Y;
            }
            SetPoint(2, two);
            InsertPoint(3, two);
          } else {
            Point three = GetPoint(3);
            if (fromDir == RIGHT || fromDir == LEFT) {
              if (Geo.IsApprox(two.X, three.X)) {
                double newx = (fromDir == RIGHT) ? Math.Max(two.X, startFrom.X) : Math.Min(two.X, startFrom.X);
                SetPoint(2, new Point(newx, startFrom.Y));
                SetPoint(3, new Point(newx, three.Y));
              } else if (Geo.IsApprox(two.Y, three.Y)) {
                if (Math.Abs(startFrom.Y-two.Y) <= positions.CellSize.Height/2) {
                  SetPoint(2, new Point(two.X, startFrom.Y));
                  SetPoint(3, new Point(three.X, startFrom.Y));
                }
                InsertPoint(2, new Point(two.X, startFrom.Y));
              } else {
                SetPoint(2, new Point(startFrom.X, two.Y));
              }
            } else if (fromDir == DOWN || fromDir == UP) {
              if (Geo.IsApprox(two.Y, three.Y)) {
                double newy = (fromDir == DOWN) ? Math.Max(two.Y, startFrom.Y) : Math.Min(two.Y, startFrom.Y);
                SetPoint(2, new Point(startFrom.X, newy));
                SetPoint(3, new Point(three.X, newy));
              } else if (Geo.IsApprox(two.X, three.X)) {
                if (Math.Abs(startFrom.X-two.X) <= positions.CellSize.Width/2) {
                  SetPoint(2, new Point(startFrom.X, two.Y));
                  SetPoint(3, new Point(startFrom.X, three.Y));
                }
                InsertPoint(2, new Point(startFrom.X, two.Y));
              } else {
                SetPoint(2, new Point(two.X, startFrom.Y));
              }
            }
          }
          return;
        }
        // otherwise, just depend on default behavior
      }

      Point m2;
      Point m3;
      if (fromDir == RIGHT) {
        if (t.X > s.X ||
            (toDir == UP && t.Y < s.Y && toR.Right > s.X) ||
            (toDir == DOWN && t.Y > s.Y && toR.Right > s.X)) {
          m2 = new Point(t.X, s.Y);
          m3 = new Point(t.X, (s.Y+t.Y)/2);
          if (toDir == LEFT) {
            m2.X = GetMidOrthoPosition(s.X, t.X, false);
            m3.X = m2.X;
            m3.Y = t.Y;
          } else if ((toDir == UP && t.Y < s.Y) || (toDir == DOWN && t.Y > s.Y)) {
            if (s.X < toR.Left)
              m2.X = GetMidOrthoPosition(s.X, toR.Left, false);
            else if (s.X < toR.Right && ((toDir == UP && s.Y < toR.Top) || (toDir == DOWN && s.Y > toR.Bottom)))
              m2.X = GetMidOrthoPosition(s.X, t.X, false);
            else
              m2.X = toR.Right;
            m3.X = m2.X;
            m3.Y = t.Y;
          } else if (toDir == RIGHT && s.X < toR.Left && s.Y > toR.Top && s.Y < toR.Bottom) {
            m2.X = s.X;
            if (s.Y < t.Y)
              m2.Y = Math.Min(t.Y, toR.Top);
            else
              m2.Y = Math.Max(t.Y, toR.Bottom);
            m3.Y = m2.Y;
          }
        } else {
          m2 = new Point(s.X, t.Y);
          m3 = new Point((s.X+t.X)/2, t.Y);
          if (toDir == LEFT ||
              (toDir == DOWN && t.Y < fromR.Top) ||
              (toDir == UP && t.Y > fromR.Bottom)) {
            if (toDir == LEFT && (Geo.Contains(toR, s) || Geo.Contains(fromR, t)))  // close to each other
              m2.Y = GetMidOrthoPosition(s.Y, t.Y, true);
            else if (t.Y < s.Y && (toDir == LEFT || toDir == DOWN))
              m2.Y = GetMidOrthoPosition(fromR.Top, Math.Max(t.Y, toR.Bottom), true);
            else if (t.Y > s.Y && (toDir == LEFT || toDir == UP))
              m2.Y = GetMidOrthoPosition(fromR.Bottom, Math.Min(t.Y, toR.Top), true);
            m3.X = t.X;
            m3.Y = m2.Y;
          }
          if (m2.Y > fromR.Top && m2.Y < fromR.Bottom) {
            if ((t.X >= fromR.Left && t.X <= s.X) || (s.X <= toR.Right && s.X >= t.X)) {
              if (toDir == DOWN || toDir == UP) {
                m2 = new Point(Math.Max((s.X+t.X)/2, s.X), s.Y);
                m3 = new Point(m2.X, t.Y);
              }
            } else {
              if (toDir == UP || ((toDir == RIGHT || toDir == LEFT) && t.Y < s.Y)) {
                m2.Y = Math.Min(t.Y, ((toDir == RIGHT) ? fromR.Top : Math.Min(fromR.Top, toR.Top)));
              } else {
                m2.Y = Math.Max(t.Y, ((toDir == RIGHT) ? fromR.Bottom : Math.Max(fromR.Bottom, toR.Bottom)));
              }
              m3.X = t.X;
              m3.Y = m2.Y;
            }
          }
        }
      } else if (fromDir == LEFT) {
        if (t.X < s.X ||
            (toDir == UP && t.Y < s.Y && toR.Left < s.X) ||
            (toDir == DOWN && t.Y > s.Y && toR.Left < s.X)) {
          m2 = new Point(t.X, s.Y);
          m3 = new Point(t.X, (s.Y+t.Y)/2);
          if (toDir == RIGHT) {
            m2.X = GetMidOrthoPosition(s.X, t.X, false);
            m3.X = m2.X;
            m3.Y = t.Y;
          } else if ((toDir == UP && t.Y < s.Y) || (toDir == DOWN && t.Y > s.Y)) {
            if (s.X > toR.Right)
              m2.X = GetMidOrthoPosition(s.X, toR.Right, false);
            else if (s.X > toR.Left && ((toDir == UP && s.Y < toR.Top) || (toDir == DOWN && s.Y > toR.Bottom)))
              m2.X = GetMidOrthoPosition(s.X, t.X, false);
            else
              m2.X = toR.Left;
            m3.X = m2.X;
            m3.Y = t.Y;
          } else if (toDir == LEFT && s.X > toR.Right && s.Y > toR.Top && s.Y < toR.Bottom) {
            m2.X = s.X;
            if (s.Y < t.Y)
              m2.Y = Math.Min(t.Y, toR.Top);
            else
              m2.Y = Math.Max(t.Y, toR.Bottom);
            m3.Y = m2.Y;
          }
        } else {
          m2 = new Point(s.X, t.Y);
          m3 = new Point((s.X+t.X)/2, t.Y);
          if (toDir == RIGHT ||
              (toDir == DOWN && t.Y < fromR.Top) ||
              (toDir == UP && t.Y > fromR.Bottom)) {
            if (toDir == RIGHT && (Geo.Contains(toR, s) || Geo.Contains(fromR, t)))  // close to each other
              m2.Y = GetMidOrthoPosition(s.Y, t.Y, true);
            else if (t.Y < s.Y && (toDir == RIGHT || toDir == DOWN))
              m2.Y = GetMidOrthoPosition(fromR.Top, Math.Max(t.Y, toR.Bottom), true);
            else if (t.Y > s.Y && (toDir == RIGHT || toDir == UP))
              m2.Y = GetMidOrthoPosition(fromR.Bottom, Math.Min(t.Y, toR.Top), true);
            m3.X = t.X;
            m3.Y = m2.Y;
          }
          if (m2.Y > fromR.Top && m2.Y < fromR.Bottom) {
            if ((t.X <= fromR.Right && t.X >= s.X) || (s.X >= toR.Left && s.X <= t.X)) {
              if (toDir == DOWN || toDir == UP) {
                m2 = new Point(Math.Min((s.X+t.X)/2, s.X), s.Y);
                m3 = new Point(m2.X, t.Y);
              }
            } else {
              if (toDir == UP || ((toDir == RIGHT || toDir == LEFT) && t.Y < s.Y)) {
                m2.Y = Math.Min(t.Y, ((toDir == LEFT) ? fromR.Top : Math.Min(fromR.Top, toR.Top)));
              } else {
                m2.Y = Math.Max(t.Y, ((toDir == LEFT) ? fromR.Bottom : Math.Max(fromR.Bottom, toR.Bottom)));
              }
              m3.X = t.X;
              m3.Y = m2.Y;
            }
          }
        }
      } else if (fromDir == DOWN) {
        if (t.Y > s.Y ||
            (toDir == LEFT && t.X < s.X && toR.Bottom > s.Y) ||
            (toDir == RIGHT && t.X > s.X && toR.Bottom > s.Y)) {
          m2 = new Point(s.X, t.Y);
          m3 = new Point((s.X+t.X)/2, t.Y);
          if (toDir == UP) {
            m2.Y = GetMidOrthoPosition(s.Y, t.Y, true);
            m3.X = t.X;
            m3.Y = m2.Y;
          } else if ((toDir == LEFT && t.X < s.X) || (toDir == RIGHT && t.X > s.X)) {
            if (s.Y < toR.Top)
              m2.Y = GetMidOrthoPosition(s.Y, toR.Top, true);
            else if (s.Y < toR.Bottom && ((toDir == LEFT && s.X < toR.Left) || (toDir == RIGHT && s.X > toR.Right)))
              m2.Y = GetMidOrthoPosition(s.Y, t.Y, true);
            else
              m2.Y = toR.Bottom;
            m3.X = t.X;
            m3.Y = m2.Y;
          } else if (toDir == DOWN && s.Y < toR.Top && s.X > toR.Left && s.X < toR.Right) {
            if (s.X < t.X)
              m2.X = Math.Min(t.X, toR.Left);
            else
              m2.X = Math.Max(t.X, toR.Right);
            m2.Y = s.Y;
            m3.X = m2.X;
          }
        } else {
          m2 = new Point(t.X, s.Y);
          m3 = new Point(t.X, (s.Y+t.Y)/2);
          if (toDir == UP ||
              (toDir == RIGHT && t.X < fromR.Left) ||
              (toDir == LEFT && t.X > fromR.Right)) {
            if (toDir == UP && (Geo.Contains(toR, s) || Geo.Contains(fromR, t)))  // close to each other
              m2.X = GetMidOrthoPosition(s.X, t.X, false);
            else if (t.X < s.X && (toDir == UP || toDir == RIGHT))
              m2.X = GetMidOrthoPosition(fromR.Left, Math.Max(t.X, toR.Right), false);
            else if (t.X > s.X && (toDir == UP || toDir == LEFT))
              m2.X = GetMidOrthoPosition(fromR.Right, Math.Min(t.X, toR.Left), false);
            m3.X = m2.X;
            m3.Y = t.Y;
          }
          if (m2.X > fromR.Left && m2.X < fromR.Right) {
            if ((t.Y >= fromR.Top && t.Y <= s.Y) || (s.Y <= toR.Bottom && s.Y >= t.Y)) {
              if (toDir == RIGHT || toDir == LEFT) {
                m2 = new Point(s.X, Math.Max((s.Y+t.Y)/2, s.Y));
                m3 = new Point(t.X, m2.Y);
              }
            } else {
              if (toDir == LEFT || ((toDir == DOWN || toDir == UP) && t.X < s.X)) {
                m2.X = Math.Min(t.X, ((toDir == DOWN) ? fromR.Left : Math.Min(fromR.Left, toR.Left)));
              } else {
                m2.X = Math.Max(t.X, ((toDir == DOWN) ? fromR.Right : Math.Max(fromR.Right, toR.Right)));
              }
              m3.X = m2.X;
              m3.Y = t.Y;
            }
          }
        }
      } else {  // fromDir == UP
        if (t.Y < s.Y ||
            (toDir == LEFT && t.X < s.X && toR.Top < s.Y) ||
            (toDir == RIGHT && t.X > s.X && toR.Top < s.Y)) {
          m2 = new Point(s.X, t.Y);
          m3 = new Point((s.X+t.X)/2, t.Y);
          if (toDir == DOWN) {
            m2.Y = GetMidOrthoPosition(s.Y, t.Y, true);
            m3.X = t.X;
            m3.Y = m2.Y;
          } else if ((toDir == LEFT && t.X < s.X) || (toDir == RIGHT && t.X >= s.X)) {
            if (s.Y > toR.Bottom)
              m2.Y = GetMidOrthoPosition(s.Y, toR.Bottom, true);
            else if (s.Y > toR.Top && ((toDir == LEFT && s.X < toR.Left) || (toDir == RIGHT && s.X > toR.Right)))
              m2.Y = GetMidOrthoPosition(s.Y, t.Y, true);
            else
              m2.Y = toR.Top;
            m3.X = t.X;
            m3.Y = m2.Y;
          } else if (toDir == UP && s.Y > toR.Bottom && s.X > toR.Left && s.X < toR.Right) {
            if (s.X < t.X)
              m2.X = Math.Min(t.X, toR.Left);
            else
              m2.X = Math.Max(t.X, toR.Right);
            m2.Y = s.Y;
            m3.X = m2.X;
          }
        } else {
          m2 = new Point(t.X, s.Y);
          m3 = new Point(t.X, (s.Y+t.Y)/2);
          if (toDir == DOWN ||
              (toDir == RIGHT && t.X < fromR.Left) ||
              (toDir == LEFT && t.X > fromR.Right)) {
            if (toDir == DOWN && (Geo.Contains(toR, s) || Geo.Contains(fromR, t)))  // close to each other
              m2.X = GetMidOrthoPosition(s.X, t.X, false);
            else if (t.X < s.X && (toDir == DOWN || toDir == RIGHT))
              m2.X = GetMidOrthoPosition(fromR.Left, Math.Max(t.X, toR.Right), false);
            else if (t.X > s.X && (toDir == DOWN || toDir == LEFT))
              m2.X = GetMidOrthoPosition(fromR.Right, Math.Min(t.X, toR.Left), false);
            m3.X = m2.X;
            m3.Y = t.Y;
          }
          if (m2.X > fromR.Left && m2.X < fromR.Right) {
            if ((t.Y <= fromR.Bottom && t.Y >= s.Y) || (s.Y >= toR.Top && s.Y <= t.Y)) {
              if (toDir == RIGHT || toDir == LEFT) {
                m2 = new Point(s.X, Math.Min((s.Y+t.Y)/2, s.Y));
                m3 = new Point(t.X, m2.Y);
              }
            } else {
              if (toDir == LEFT || ((toDir == DOWN || toDir == UP) && t.X < s.X)) {
                m2.X = Math.Min(t.X, ((toDir == UP) ? fromR.Left : Math.Min(fromR.Left, toR.Left)));
              } else {
                m2.X = Math.Max(t.X, ((toDir == UP) ? fromR.Right : Math.Max(fromR.Right, toR.Right)));
              }
              m3.X = m2.X;
              m3.Y = t.Y;
            }
          }
        }
      }

      AddPoint(m2);
      AddPoint(m3);
    }

    /// <summary>
    /// This method is called by <see cref="AddOrthoPoints"/> to determine the distance
    /// of the middle segment between the two ports.
    /// </summary>
    /// <param name="fromPosition">The first point's X or Y coordinate, depending on the direction</param>
    /// <param name="toPosition">The last point's X or Y coordinate, depending on the direction</param>
    /// <param name="vertical">Whether the mid-position is along the vertical axis or horizontal</param>
    /// <returns></returns>
    /// <remarks>
    /// By default this returns the midpoint between the two coordinates.
    /// </remarks>
    protected virtual double GetMidOrthoPosition(double fromPosition, double toPosition, bool vertical) {
      if (HasCurviness()) {
        double c = ComputeCurviness();
        return (fromPosition + toPosition)/2 + c;
      }
      return (fromPosition + toPosition)/2;
    }


    private void TraversePositions(PositionArray positions, double px, double py, double dir, bool first) {
      Size cell = positions.CellSize;
      int val = positions.GetDist(px, py);
      double qx = px;
      double qy = py;

      // try going forward
      double fx = qx;
      double fy = qy;
      if (dir == RIGHT)
        fx += cell.Width;
      else if (dir == DOWN)
        fy += cell.Height;
      else if (dir == LEFT)
        fx -= cell.Width;
      else
        fy -= cell.Height;
      // keep going forward until we can't
      while (val > PositionArray.StartDistance && positions.GetDist(fx, fy) == val-PositionArray.StepDistance) {
        qx = fx;
        qy = fy;
        if (dir == RIGHT)
          fx += cell.Width;
        else if (dir == DOWN)
          fy += cell.Height;
        else if (dir == LEFT)
          fx -= cell.Width;
        else
          fy -= cell.Height;
        val -= PositionArray.StepDistance;
      }

      // line up the points to be in the middle of the grid
      if (first) {
        if (val > PositionArray.StartDistance) {
          if (dir == LEFT || dir == RIGHT)
            qx = (double)Math.Floor(qx/cell.Width)*cell.Width + cell.Width/2;
          else if (dir == DOWN || dir == UP)
            qy = (double)Math.Floor(qy/cell.Height)*cell.Height + cell.Height/2;
        }
      } else {
        qx = (double)Math.Floor(qx/cell.Width)*cell.Width + cell.Width/2;
        qy = (double)Math.Floor(qy/cell.Height)*cell.Height + cell.Height/2;
      }

      if (val > PositionArray.StartDistance) {
        double newdir = dir;
        // try turning right
        double rx = qx;
        double ry = qy;
        if (dir == RIGHT) {
          newdir = DOWN;
          ry += cell.Height;
        } else if (dir == DOWN) {
          newdir = LEFT;
          rx -= cell.Width;
        } else if (dir == LEFT) {
          newdir = UP;
          ry -= cell.Height;
        } else if (dir == UP) {
          newdir = RIGHT;
          rx += cell.Width;
        }
        if (positions.GetDist(rx, ry) == val-PositionArray.StepDistance) {
          TraversePositions(positions, rx, ry, newdir, false);
        } else {
          // try turning left
          double lx = qx;
          double ly = qy;
          if (dir == RIGHT) {
            newdir = UP;
            ly -= cell.Height;
          } else if (dir == DOWN) {
            newdir = RIGHT;
            lx += cell.Width;
          } else if (dir == LEFT) {
            newdir = DOWN;
            ly += cell.Height;
          } else if (dir == UP) {
            newdir = LEFT;
            lx -= cell.Width;
          }
          if (positions.GetDist(lx, ly) == val-PositionArray.StepDistance)
            TraversePositions(positions, lx, ly, newdir, false);
        }
      }
      AddPoint(new Point(qx, qy));
    }


    /// <summary>
    /// Find the index of the segment that is closest to a given point.
    /// </summary>
    /// <param name="q">the <c>Point</c>, in model coordinates</param>
    /// <returns>The index of the segment, from zero to the number of points minus 2</returns>
    /// <remarks>
    /// This assumes the route only has straight line segments.
    /// It ignores any jump-overs or jump-gaps.
    /// </remarks>
    public int FindClosestSegment(Point q) {
      IList<Point> points = this.Points;
      int ci = 0;
      double cdist = Geo.DistanceToLineSegmentSquared(points[0], points[1], q);
      for (int i = 1; i < points.Count-1; i++) {
        double dist = Geo.DistanceToLineSegmentSquared(points[i], points[i+1], q);
        if (dist < cdist) {
          ci = i;
          cdist = dist;
        }
      }
      return ci;
    }


    // the Geometry for the Route

    /// <summary>
    /// Gets the <c>Geometry</c> for this <see cref="Route"/>.
    /// </summary>
    /// <value>
    /// This is the cached value of <see cref="MakeGeometry"/>.
    /// </value>
    public Geometry DefiningGeometry {
      get {
        if (!this.ValidGeometry) {
          UpdatePoints();
          _DefiningGeometry = MakeGeometry();
        }
        return _DefiningGeometry;
      }
    }
    private Geometry _DefiningGeometry;
    private Geometry _EmptyGeometry;

    // called by DiagramPanel when about to measure/arrange the Link
    internal void InvalidateGeometry() {  // used by Link
      _DefiningGeometry = _EmptyGeometry;  // rebuild Geometry when needed
    }
    private bool ValidGeometry {
      get { return _DefiningGeometry != _EmptyGeometry; }
    }

    internal bool JumpsOver {
      get {
        LinkCurve curve = this.Curve;
        return (curve == LinkCurve.JumpOver || curve == LinkCurve.JumpGap);
      }
    }

    internal void InvalidateOtherJumpOvers(bool force) {
      if (force || this.JumpsOver) {
        DiagramPanel panel = (this.Link != null ? this.Link.Panel : null);
        if (panel != null && !panel.DelayedGeometries.ContainsKey(this)) {
          //Diagram.Debug("delaying " + Diagram.Str(this.RouteBounds) + Diagram.Str(this.PreviousBounds));
          panel.DelayedGeometries.Add(this, this.PreviousBounds);
        }
      }
    }

    // maybe invalidate the geometries of other links that jump or gap over this link
    internal void InvalidateOtherJumpOvers(Rect prevbounds, DiagramPanel panel) {
      Link thislink = this.Link;
      if (thislink == null) return;
      Layer thislayer = thislink.Layer;
      if (thislayer != null) {
        if (thislayer.Visibility != Visibility.Visible || thislayer.IsTemporary || thislayer.Panel == null) return;
        if (panel == null) panel = thislayer.Panel;
      }
      bool foundlayer = false;
      foreach (UIElement lay in panel.Children) {
        // only consider visible LinkLayers
        LinkLayer layer = lay as LinkLayer;
        if (layer == null) continue;
        if (layer.Visibility != Visibility.Visible) continue;
        // skip over layers before THISLAYER
        if (layer == thislayer) {
          foundlayer = true;
          bool foundlink = false;
          foreach (Link link in layer.Links) {
            // skip over links before THISLINK
            if (link == thislink) {
              foundlink = true;
            } else if (foundlink) {  // for any Link after THISLINK
              InvalidateJumpOvers(link, prevbounds);
            }
          }
        } else if (foundlayer) {  // for any LinkLayer after THISLAYER
          foreach (Link link in layer.Links) {
            InvalidateJumpOvers(link, prevbounds);
          }
        }
      }
    }

    private void InvalidateJumpOvers(Link otherlink, Rect prevbounds) {
      // OTHERLINK needs to have a JumpOver or JumpGap curve and intersecting bounds
      Route otherroute = otherlink.Route;
      if (otherroute == null) return;
      if (!otherroute.JumpsOver) return;
      // don't use DefiningGeometry property, which might construct the Geometry unnecessarily
      if (!otherroute.ValidGeometry) return;
      if (!Geo.Intersects(this.RouteBounds, otherroute.RouteBounds) && !Geo.Intersects(prevbounds, otherroute.RouteBounds)) return;
      // if it uses the same port, don't jump-over
      if (UsesSamePort(otherlink)) return;
      otherroute.InvalidateGeometry();
      otherlink.InvalidateVisual(otherlink.Path);
    }

    private bool UsesSamePort(Link otherlink) {
      Route otherroute = otherlink.Route;
      int numpts = this.PointsCount;
      int otherpts = otherroute.PointsCount;
      if (numpts > 0 && otherpts > 0) {
        // assume no jump-overs when links come from or go to the same point
        Point start = GetPoint(0);
        Point ostart = otherroute.GetPoint(0);
        if (start == ostart) return true;
        Point end = GetPoint(numpts-1);
        Point oend = otherroute.GetPoint(otherpts-1);
        if (end == oend) return true;
      } else {  // assume no jump-overs when crossing a link from/to the same node
        if (this.Link.FromNode == otherlink.FromNode) return true;
        if (this.Link.ToNode == otherlink.ToNode) return true;
      }
      return false;
    }

    /// <summary>
    /// Produce a <c>Geometry</c> given the points of this <see cref="Route"/>,
    /// depending on the value of <see cref="Curve"/>.
    /// </summary>
    /// <returns></returns>
    protected virtual Geometry MakeGeometry() {
      int numpts = this.PointsCount;
      if (numpts < 2) {
        if (_EmptyGeometry == null) _EmptyGeometry = new LineGeometry();
        return _EmptyGeometry;
      }
      Rect b = this.RouteBounds;
      Point p0 = GetPoint(0);
      //Diagram.Debug("  MakeGeometry: " + Diagram.Str(this.Link) + " " + Diagram.Str(p0) + " ==> " + Diagram.Str(GetPoint(numpts-1)));

      // need to shift all points back by b.X/b.Y, since the Link/LinkLayer will be translating them
      p0.X -= b.X;
      p0.Y -= b.Y;

      if (numpts == 2) {
        Point p1 = GetPoint(1);
        p1.X -= b.X;
        p1.Y -= b.Y;
        if(FromShortLength != 0) p0 = shiftPointByShortLength(p0, true, b);
        if(ToShortLength != 0) p1 = shiftPointByShortLength(p1, false, b);
        LineGeometry geo = new LineGeometry() { StartPoint = p0, EndPoint = p1 };
        return geo;
      } else {
        GeoStream geo = new GeoStream();
        using (StreamGeometryContext g = geo.Open()) {
          if(FromShortLength != 0) p0 = shiftPointByShortLength(p0, true, b);
          g.BeginFigure(p0, false, false);
          LinkCurve curve = GetCurve();
          if (curve == LinkCurve.Bezier && numpts >= 3 && !Geo.IsApproxEqual(Smoothness,0)) {
            if (numpts == 3) {
              Point c = GetPoint(1);
              Point e = GetPoint(2);
              c.X -= b.X;
              c.Y -= b.Y;
              e.X -= b.X;
              e.Y -= b.Y;
              if(ToShortLength != 0) e = shiftPointByShortLength(e, false, b);
              g.BezierTo(c, c, e, true, true);
            } else {
              LinkRouting routing = this.Routing;
              if (routing == LinkRouting.Orthogonal  ||  routing == LinkRouting.AvoidsNodes) {
                  Point[] controlPair1 = new Point[2];
                  controlPair1[1] = Geo.TranslatePoint(GetPoint(1), -b.X, -b.Y);

                  Point[] controlPair2 = new Point[2];
                  Point previousPoint = GetPoint(0);
                  //Precalculate this so it isn't calculated with every call of getOrthogonalBezierControls
                  double controlComponentLength = Smoothness/3;

                  for (int i = 1; i < PointsCount-1; i++) {
                    Point currentPoint = GetPoint(i);

                    Point[] cornerPoints = new Point[] { previousPoint, currentPoint, GetPoint(furthestPoint(GetPoint(i), i, false)) };
                    //If the currentPoint lies on a straight line with the previous and next points, ignore it. One of these conditionals will be true if
                    if (Geo.IsApproxEqual(cornerPoints[0].X, cornerPoints[1].X) && Geo.IsApproxEqual(cornerPoints[1].X, cornerPoints[2].X))
                      continue;
                    if (Geo.IsApproxEqual(cornerPoints[0].Y, cornerPoints[1].Y) && Geo.IsApproxEqual(cornerPoints[1].Y, cornerPoints[2].Y))
                      continue;

                    controlPair2 = getOrthogonalBezierControls(cornerPoints[0], cornerPoints[1], cornerPoints[2], controlComponentLength);
                    controlPair2 = Geo.TranslatePoints(controlPair2, -b.X, -b.Y).ToArray<Point>();

                    if (i == 1) { //special case where there is no intermediate point between the start point and the first corner.
                      controlPair1[1] = Geo.TranslatePoint(new Point((previousPoint.X + currentPoint.X)*0.5, (previousPoint.Y + currentPoint.Y)*0.5), -b.X, -b.Y);
                    }
                    if (i == 2 && Geo.IsApproxEqual(previousPoint, GetPoint(0))) { //case where point 1 is on the line connecting point 2 and point 0, use midpoint of line as control point
                      controlPair1[1] = Geo.TranslatePoint(new Point((previousPoint.X + currentPoint.X)*0.5, (previousPoint.Y + currentPoint.Y)*0.5), -b.X, -b.Y);
                    }

                    //Account for control point overlap due to large values of Smoothness and/or small segments

                    g.BezierTo(controlPair1[1], controlPair2[0], Geo.TranslatePoint(currentPoint, -b.X, -b.Y), true, false);

                    controlPair1 = controlPair2;
                    previousPoint = currentPoint;
                  }


                  //Point endControl = Geo.TranslatePoint(GetPoint(PointsCount-2), -b.X, -b.Y);
                  Point endControl = Geo.TranslatePoint(previousPoint, -b.X, -b.Y);
                  Point endPoint = Geo.TranslatePoint(GetPoint(PointsCount-1), -b.X, -b.Y);
                  //if (Geo.IsApproxEqual(endControl, Geo.TranslatePoint(previousPoint, -b.X, -b.Y)))
                  endControl = new Point(0.5*(endControl.X + endPoint.X), 0.5*(endControl.Y + endPoint.Y));

                  g.BezierTo(controlPair2[1], endControl, endPoint, true, false);
                  
                
              } else {
                Point startControl, endControl, end;
                for (int i = 3; i < numpts; i += 3) {
                  startControl = GetPoint(i-2);
                  // if it's the last segment, use the last two points
                  if (i+3 >= numpts)
                    i = numpts-1;
                  endControl = GetPoint(i-1);
                  end = GetPoint(i);
                  if (i == numpts-1 && ToShortLength != 0) {
                    end = shiftPointByShortLength(end, false, new Rect());
                  }
                  g.BezierTo(new Point(startControl.X-b.X, startControl.Y-b.Y),
                             new Point(endControl.X-b.X, endControl.Y-b.Y),
                             new Point(end.X-b.X, end.Y-b.Y), true, true);
                }
              }
            }
          } else {
            Point from = GetPoint(0);
            int i = 1;
            while (i < numpts) {
              i = furthestPoint(from, i, (i > 1));
              Point to = GetPoint(i);
              if (i >= numpts-1) {
                if (from != to) {
                  if (ToShortLength != 0) to = shiftPointByShortLength(to, false, new Rect());
                  addLine(g, -b.X, -b.Y, from, to);
                }
                break;
              }
              int j = furthestPoint(to, i+1, (i < numpts-3));
              Point next = GetPoint(j);
              from = addLineAndCorner(g, -b.X, -b.Y, from, to, next);
              i = j;
            }
          }
        }
        return geo.Geometry;
      }
    }

    private static double getDistanceBetweenLinkPoints(Point a, Point b) {
      if (Geo.IsApproxEqual(a,b)) return 0.0;
      if (double.IsNaN(a.X)  ||  double.IsInfinity(a.X)  ||  double.IsNaN(a.Y)  ||  double.IsInfinity(a.Y)) return Double.NaN;
      if (double.IsNaN(b.X)  ||  double.IsInfinity(b.X)  ||  double.IsNaN(b.Y)  ||  double.IsInfinity(b.Y)) return Double.NaN;
      double dx = b.X - a.X;
      if (dx < 0) dx = -dx;
      double dy = b.Y - a.Y;
      if (dy < 0) dy = -dy;
      if (Geo.IsApproxEqual(dx, 0)) return dy;
      if (Geo.IsApproxEqual(dy, 0)) return dx;
      return Math.Sqrt(dx*dx + dy*dy);
    }

    private Point[] getOrthogonalBezierControls(Point a, Point b, Point c) {
      return getOrthogonalBezierControls(a, b, c, double.NaN);
    }

    private Point[] getOrthogonalBezierControls(Point a, Point b, Point c, double controlPointDistanceRatio) {
      Point[] controls = new Point[2];

      double length1 = getDistanceBetweenLinkPoints(a, b);
      double length2 = getDistanceBetweenLinkPoints(b, c);

      if (Double.IsNaN(controlPointDistanceRatio)) //set to default value
        controlPointDistanceRatio = Smoothness/3;

      if (Geo.IsApproxEqual(a.Y, b.Y) && Geo.IsApproxEqual(b.X, c.X)) {  // horizontal then vertical
        if (b.X > a.X) {  // right
          if (c.Y > b.Y) {  // right then down
            controls[0] = new Point(b.X - (controlPointDistanceRatio*length1), b.Y - (controlPointDistanceRatio*length1));
            controls[1] = new Point(b.X + (controlPointDistanceRatio*length2), b.Y + (controlPointDistanceRatio*length2));
          } else {  // right then up
            controls[0] = new Point(b.X - (controlPointDistanceRatio*length1), b.Y + (controlPointDistanceRatio*length1));
            controls[1] = new Point(b.X + (controlPointDistanceRatio*length2), b.Y - (controlPointDistanceRatio*length2));
          }
        } else {  // left
          if (c.Y > b.Y) {  // left then down
            controls[0] = new Point(b.X + (controlPointDistanceRatio*length1), b.Y - (controlPointDistanceRatio*length1));
            controls[1] = new Point(b.X - (controlPointDistanceRatio*length2), b.Y + (controlPointDistanceRatio*length2));
          } else {  // left then up
            controls[0] = new Point(b.X + (controlPointDistanceRatio*length1), b.Y + (controlPointDistanceRatio*length1));
            controls[1] = new Point(b.X - (controlPointDistanceRatio*length2), b.Y - (controlPointDistanceRatio*length2));
          }
        }
      }

      if (Geo.IsApproxEqual(a.X, b.X) && Geo.IsApproxEqual(b.Y, c.Y)) {  // vertical then horizontal
        if (b.Y > a.Y) {  // down
          if (c.X > b.X) {  // down then right
            controls[0] = new Point(b.X - (controlPointDistanceRatio*length1), b.Y - (controlPointDistanceRatio*length1));
            controls[1] = new Point(b.X + (controlPointDistanceRatio*length2), b.Y + (controlPointDistanceRatio*length2));
          } else {  // down then left
            controls[0] = new Point(b.X + (controlPointDistanceRatio*length1), b.Y - (controlPointDistanceRatio*length1));
            controls[1] = new Point(b.X - (controlPointDistanceRatio*length2), b.Y + (controlPointDistanceRatio*length2));
          }
        } else {  // up
          if (c.X > b.X) {  // up then right
            controls[0] = new Point(b.X - (controlPointDistanceRatio*length1), b.Y + (controlPointDistanceRatio*length1));
            controls[1] = new Point(b.X + (controlPointDistanceRatio*length2), b.Y - (controlPointDistanceRatio*length2));
          } else {  // up then left
            controls[0] = new Point(b.X + (controlPointDistanceRatio*length1), b.Y + (controlPointDistanceRatio*length1));
            controls[1] = new Point(b.X - (controlPointDistanceRatio*length2), b.Y - (controlPointDistanceRatio*length2));
          }
        }
      }

      if ((Geo.IsApproxEqual(a.X, b.X) && Geo.IsApproxEqual(b.X, c.X))  ||  (Geo.IsApproxEqual(a.Y, b.Y) && Geo.IsApproxEqual(b.Y, c.Y))) {  // parallel segments
        Point midpoint = new Point(0.5*(a.X + c.X), 0.5*(a.Y+c.Y));
        controls[0] = midpoint;
        controls[1] = midpoint;
      }

      return controls;
    }

    private Point getShortLengthOffset(bool from, double segmentLength, double maxOffset, Point a, Point b) {
      if (Geo.IsApproxEqual(a,b)) return new Point();
      if (double.IsNaN(a.X)  ||  double.IsInfinity(a.X)  ||  double.IsNaN(a.Y)  ||  double.IsInfinity(a.Y)) return new Point(Double.NaN, Double.NaN);
      if (double.IsNaN(b.X)  ||  double.IsInfinity(b.X)  ||  double.IsNaN(b.Y)  ||  double.IsInfinity(b.Y)) return new Point(Double.NaN, Double.NaN);
      double dx = b.X - a.X;
      double dy = b.Y - a.Y;

      Point offsetXY = new Point();
      double offset = (from ? FromShortLength : ToShortLength);
      if (offset > maxOffset) offset = maxOffset;
      offsetXY.X = offset*dx/segmentLength;
      offsetXY.Y = offset*dy/segmentLength;
      return offsetXY;
    }

    private Point shiftPointByShortLength(Point pt, bool from, Rect ptOffset) {
      int nmpts = this.PointsCount;
      if (nmpts < 2) return pt;

      Rect bounds = ptOffset;
      Point otherPt = from ? GetPoint(1) : GetPoint(nmpts - 2);
      otherPt = Geo.TranslatePoint(otherPt, -bounds.X, -bounds.Y);  //Translates otherPt to Link coordinates
      double segmentLength = getDistanceBetweenLinkPoints(pt, otherPt);
      double maxOffset = (nmpts == 2) ? segmentLength*0.5 : segmentLength;
      Point offsetXY;
      if (from) {
        offsetXY = getShortLengthOffset(true, segmentLength, maxOffset, pt, otherPt);
      } else {
        offsetXY = getShortLengthOffset(false, segmentLength, maxOffset, otherPt, pt);
        offsetXY = new Point(offsetXY.X * -1, offsetXY.Y * -1);
      }
      if (!Double.IsNaN(offsetXY.X)) pt.X += offsetXY.X;
      if (!Double.IsNaN(offsetXY.Y)) pt.Y += offsetXY.Y;
      return pt;
    }

    private int furthestPoint(Point a, int i, bool oneway) {
      int numpts = this.PointsCount;
      // look for a different point, to give a direction from A to B
      Point b = a;
      while (Geo.IsApproxEqual(a.X, b.X) && Geo.IsApproxEqual(a.Y, b.Y)) {
        if (i >= numpts)
          return numpts-1;
        b = GetPoint(i++);
      }
      // now A != B, so we have a direction
      // make sure the direction is orthogonal
      if (!Geo.IsApproxEqual(a.X, b.X) && !Geo.IsApproxEqual(a.Y, b.Y))
        return i-1;
      // now a.X == b.X || a.Y == b.Y
      // keep going in the orthogonal direction
      Point c = b;
      while ((Geo.IsApproxEqual(a.X, b.X) && Geo.IsApproxEqual(b.X, c.X) && (!oneway || (a.Y >= b.Y ? b.Y >= c.Y : b.Y <= c.Y))) ||
             (Geo.IsApproxEqual(a.Y, b.Y) && Geo.IsApproxEqual(b.Y, c.Y) && (!oneway || (a.X >= b.X ? b.X >= c.X : b.X <= c.X)))) {
        if (i >= numpts)
          return numpts-1;
        c = GetPoint(i++);
      }
      // now B != C, so C is off the A-B line
      return i-2;
    }

    //?? NYI only perpendicular lines supported
    private Point addLineAndCorner(StreamGeometryContext g, double offx, double offy, Point a, Point b, Point c) {
      if (Geo.IsApprox(a.Y, b.Y) && Geo.IsApprox(b.X, c.X)) {  // horizontal then vertical
        double corner = ComputeCorner();
        double dx = Math.Min(corner, Math.Abs(b.X-a.X)/2);
        double dy = Math.Min(dx /* corner */, Math.Abs(c.Y-b.Y)/2);
        dx = dy;  // assume square corners
        if (Geo.IsApprox(dx, 0)) {
          addLine(g, offx, offy, a, b);
          return b;
        }
        Point arc1 = b;
        Point arc2 = b;
        double startangle;
        double sweepangle = 90;
        if (b.X > a.X) {  // right
          arc1.X = b.X - dx;
          if (c.Y > b.Y) {  // right and down
            arc2.Y = b.Y + dy;
            startangle = 270;
          } else {  // right and up
            arc2.Y = b.Y - dy;
            startangle = 90;
            sweepangle = -90;
          }
        } else {  // left
          arc1.X = b.X + dx;
          if (c.Y > b.Y) {  // left and down
            arc2.Y = b.Y + dy;
            startangle = 270;
            sweepangle = -90;
          } else {  // left and up
            arc2.Y = b.Y - dy;
            startangle = 90;
          }
        }
        addLine(g, offx, offy, a, arc1);
        g.ArcTo(new Point(arc2.X+offx, arc2.Y+offy), new Size(dx, dy), startangle, false, sweepangle < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise, true, true);
        return arc2;
      } else if (Geo.IsApprox(a.X, b.X) && Geo.IsApprox(b.Y, c.Y)) { // vertical then horizontal
        double corner = ComputeCorner();
        double dy = Math.Min(corner, Math.Abs(b.Y-a.Y)/2);
        double dx = Math.Min(dy /* corner */, Math.Abs(c.X-b.X)/2);
        dy = dx;  // assume square corners
        if (Geo.IsApprox(dx, 0)) {
          addLine(g, offx, offy, a, b);
          return b;
        }
        Point arc1 = b;
        Point arc2 = b;
        double startangle;
        double sweepangle = 90;
        if (b.Y > a.Y) {  // down
          arc1.Y = b.Y - dy;
          if (c.X > b.X) {  // down and right
            arc2.X = b.X + dx;
            startangle = 180;
            sweepangle = -90;
          } else {  // down and left
            arc2.X = b.X - dx;
            startangle = 0;
          }
        } else {  // up
          arc1.Y = b.Y + dy;
          if (c.X > b.X) {  // up and right
            arc2.X = b.X + dx;
            startangle = 180;
          } else {  // up and left
            arc2.X = b.X - dx;
            startangle = 0;
            sweepangle = -90;
          }
        }
        addLine(g, offx, offy, a, arc1);
        g.ArcTo(new Point(arc2.X+offx, arc2.Y+offy), new Size(dx, dy), startangle, false, sweepangle < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise, true, true);
        return arc2;
      } else {
        addLine(g, offx, offy, a, b);
        return b;
      }
    }

    private void addLine(StreamGeometryContext g, double offx, double offy, Point from, Point to) {
      if (!this.JumpsOver) {
        g.LineTo(new Point(to.X+offx, to.Y+offy), true, true);
      } else {
        double curve = 10;  //??
        double curve2 = curve/2;
        double[] vec = null;
        int numints = getIntersections(from, to, ref vec);
        Point last = from;
        if (numints > 0) {
          if (Geo.IsApprox(from.Y, to.Y)) {
            // vec is sorted by increasing values
            if (from.X < to.X) {
              int j = 0;
              while (j < numints) {
                double closer = Math.Max(from.X, Math.Min(vec[j++]-curve2, to.X-curve));
                g.LineTo(new Point(closer+offx, to.Y+offy), true, true);
                last = new Point(closer+offx, to.Y+offy);
                double farther = Math.Min(closer+curve, to.X);
                while (j < numints) {
                  double next = vec[j];
                  if (next < farther+curve) {
                    j++;
                    farther = Math.Min(next+curve2, to.X);
                  } else {
                    break;  // leave J as is for next lineTo call
                  }
                }
                Point control = new Point((closer+farther)/2+offx, to.Y-curve+offy);
                Point end = new Point(farther+offx, to.Y+offy);
                if (this.Curve == LinkCurve.JumpGap)
                  g.BeginFigure(end, false, false);
                else  //this.Curve == LinkCurve.JumpOver
                  g.BezierTo(new Point(last.X, control.Y), new Point(end.X, control.Y), end, true, true);
                last = end;
              }
            } else {  // from.X > to.X
              int j = numints-1;
              while (j >= 0) {
                double closer = Math.Min(from.X, Math.Max(vec[j--]+curve2, to.X+curve));
                g.LineTo(new Point(closer+offx, to.Y+offy), true, true);
                last = new Point(closer+offx, to.Y+offy);
                double farther = Math.Max(closer-curve, to.X);
                while (j >= 0) {
                  double next = vec[j];
                  if (next > farther-curve) {
                    j--;
                    farther = Math.Max(next-curve2, to.X);
                  } else {
                    break;  // leave J as is for next lineTo call
                  }
                }
                Point control = new Point((closer+farther)/2+offx, to.Y-curve+offy);
                Point end = new Point(farther+offx, to.Y+offy);
                if (this.Curve == LinkCurve.JumpGap)
                  g.BeginFigure(end, false, false);
                else  //this.Curve == LinkCurve.JumpOver
                  g.BezierTo(new Point(last.X, control.Y), new Point(end.X, control.Y), end, true, true);
                last = end;
              }
            }
          } else if (Geo.IsApprox(from.X, to.X)) {
            // vec is sorted by increasing values
            if (from.Y < to.Y) {
              int j = 0;
              while (j < numints) {
                double closer = Math.Max(from.Y, Math.Min(vec[j++]-curve2, to.Y-curve));
                g.LineTo(new Point(to.X+offx, closer+offy), true, true);
                last = new Point(to.X+offx, closer+offy);
                double farther = Math.Min(closer+curve, to.Y);
                while (j < numints) {
                  double next = vec[j];
                  if (next < farther+curve) {
                    j++;
                    farther = Math.Min(next+curve2, to.Y);
                  } else {
                    break;  // leave J as is for next lineTo call
                  }
                }
                Point control = new Point(to.X-curve+offx, (closer+farther)/2+offy);
                Point end = new Point(to.X+offx, farther+offy);
                if (this.Curve == LinkCurve.JumpGap)
                  g.BeginFigure(end, false, false);
                else  //this.Curve == LinkCurve.JumpOver
                  g.BezierTo(new Point(control.X, last.Y), new Point(control.X, end.Y), end, true, true);
                last = end;
              }
            } else {  // from.Y > to.Y
              int j = numints-1;
              while (j >= 0) {
                double closer = Math.Min(from.Y, Math.Max(vec[j--]+curve2, to.Y+curve));
                g.LineTo(new Point(to.X+offx, closer+offy), true, true);
                last = new Point(to.X+offx, closer+offy);
                double farther = Math.Max(closer-curve, to.Y);
                while (j >= 0) {
                  double next = vec[j];
                  if (next > farther-curve) {
                    j--;
                    farther = Math.Max(next-curve2, to.Y);
                  } else {
                    break;  // leave J as is for next lineTo call
                  }
                }
                Point control = new Point(to.X-curve+offx, (closer+farther)/2+offy);
                Point end = new Point(to.X+offx, farther+offy);
                if (this.Curve == LinkCurve.JumpGap)
                  g.BeginFigure(end, false, false);
                else  //this.Curve == LinkCurve.JumpOver
                  g.BezierTo(new Point(control.X, last.Y), new Point(control.X, end.Y), end, true, true);
                last = end;
              }
            }
          }
        }
        g.LineTo(new Point(to.X+offx, to.Y+offy), true, true);
      }
    }

    private int getIntersections(Point A, Point B, ref double[] v) {
      DiagramPanel panel = (this.Link != null ? this.Link.Panel : null);
      if (panel == null) return 0;

      double minx = Math.Min(A.X, B.X);
      double miny = Math.Min(A.Y, B.Y);
      double maxx = Math.Max(A.X, B.X);
      double maxy = Math.Max(A.Y, B.Y);
      int numints = 0;
      foreach (UIElement lay in panel.Children) {
        LinkLayer layer = lay as LinkLayer;
        if (layer != null && layer.Visibility == Visibility.Visible) {
          foreach (Link otherlink in layer.Links) {
            Route otherroute = otherlink.Route;
            if (otherroute == null) continue;
            // done searching for links underneath this one
            if (otherroute == this) {
              if (v != null && numints > 0) {
                Array.Sort<double>(v, 0, numints, Comparer<double>.Default);
              }
              return numints;
            }
            // ignore links that don't also JumpOver or JumpGap
            if (!otherroute.JumpsOver) continue;
            // ignore links that are far away
            if (!Geo.Intersects(this.RouteBounds, otherroute.RouteBounds)) continue;
            // ignore links that connect to the same nodes as this link does
            if (UsesSamePort(otherlink)) continue;
            // ignore link with no or invisible path
            FrameworkElement shape = otherlink.Path;
            if (shape == null) continue;
            if (!Part.IsVisibleElement(shape)) continue;
            numints = getIntersections2(A, B, ref v, numints, otherroute);
          }
        }
      }
      if (v != null && numints > 0) {
        Array.Sort<double>(v, 0, numints, Comparer<double>.Default);
      }
      return numints;
    }

    private int getIntersections2(Point A, Point B, ref double[] v, int numints, Route route) {
      int numpts = route.PointsCount;
      for (int i = 1; i < numpts; i++) {
        Point from = route.GetPoint(i-1);
        Point to = route.GetPoint(i);
        Point result = new Point();
        if (getOrthoSegmentIntersection(A, B, from, to, ref result)) {
          if (v == null) v = new double[50];
          if (numints < v.Length) {
            if (Geo.IsApprox(A.Y, B.Y))
              v[numints++] = result.X;
            else
              v[numints++] = result.Y;
          }
        }
      }
      return numints;
    }

    private bool getOrthoSegmentIntersection(Point A, Point B, Point C, Point D, ref Point result) {
      if (!Geo.IsApprox(A.X, B.X)) {
        // A.Y == B.Y : AB is horizontal, so CD must be vertical
        if (Geo.IsApprox(C.X, D.X) &&
          Math.Min(A.X, B.X) < C.X &&
          Math.Max(A.X, B.X) > C.X &&
          Math.Min(C.Y, D.Y) < A.Y &&
          Math.Max(C.Y, D.Y) > A.Y) {
          result.X = C.X;
          result.Y = A.Y;
          return true;
        }
      } else {
        // A.X == B.X : AB is vertical, so CD must be horizontal
        if (Geo.IsApprox(C.Y, D.Y) &&
          Math.Min(A.Y, B.Y) < C.Y &&
          Math.Max(A.Y, B.Y) > C.Y &&
          Math.Min(C.X, D.X) < A.X &&
          Math.Max(C.X, D.X) > A.X) {
          result.X = A.X;
          result.Y = C.Y;
          return true;
        }
      }
      result.X = 0;
      result.Y = 0;
      return false;
    }


    /// <summary>
    /// Return the index of the first stroke point to get a handle.
    /// </summary>
    public virtual int FirstPickIndex {
      get {
        int n = this.PointsCount;
        if (n <= 2)
          return 0;
        if (this.Orthogonal || GetFromSpot().IsNotNone)
          return 1;
        else
          return 0;
      }
    }

    /// <summary>
    /// Return the index of the last stroke point to get a handle.
    /// </summary>
    public virtual int LastPickIndex {
      get {
        int n = this.PointsCount;
        if (n == 0)
          return 0;
        if (n <= 2)
          return n-1;
        if (this.Orthogonal || GetToSpot().IsNotNone)
          return n-2;
        else
          return n-1;
      }
    }


#pragma warning disable 1591
    [EditorBrowsableAttribute(EditorBrowsableState.Never)]
    public sealed class PointListConverter : TypeConverter {  // nested class, must be public for Silverlight
      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
      }

      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
      }

      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
        string source = value as string;
        if (source != null) {
          try {
            String[] parsed = source.Split(' ');
            List<Point> pts = new List<Point>();
            for (int i = 0; i < parsed.Length; i++) {
              double x = Double.Parse(parsed[i], System.Globalization.CultureInfo.InvariantCulture);
              i++;
              double y = 0;
              if (i < parsed.Length)
                y = Double.Parse(parsed[i], System.Globalization.CultureInfo.InvariantCulture);
              pts.Add(new Point(x, y));
            }
            return pts;
          } catch (Exception) { }
          return null;
        }
        return base.ConvertFrom(context, culture, value);
      }

      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
        if (destinationType == typeof(string) && value is List<Point>) {
          List<Point> val = (List<Point>)value;
          StringBuilder sb = new StringBuilder();
          for (int i = 0; i < val.Count; i++) {
            Point p = val[i];
            if (i > 0) sb.Append(" ");
            sb.Append(p.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + p.Y.ToString(System.Globalization.CultureInfo.InvariantCulture));
          }
          String s = sb.ToString();
          return s;
        }
        return base.ConvertTo(context, culture, value, destinationType);
      }
    }


    [EditorBrowsableAttribute(EditorBrowsableState.Never)]
    public sealed class PointConverter : TypeConverter {  // nested class, must be public for Silverlight
      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
      }

      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
      }

      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
        string source = value as string;
        if (source != null) {
          try {
            String[] parsed = source.Split(' ');
            double x = ((parsed.Length >= 1) ? Double.Parse(parsed[0], System.Globalization.CultureInfo.InvariantCulture) : 0);
            double y = ((parsed.Length >= 2) ? Double.Parse(parsed[1], System.Globalization.CultureInfo.InvariantCulture) : 0);
            return new Point(x, y);
          } catch (Exception) { }
          return new Point(0, 0);
        }
        return base.ConvertFrom(context, culture, value);
      }

      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
        if (destinationType != typeof(String) && value is Point) {
          Point p = (Point)value;
          return p.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + p.Y.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        return base.ConvertTo(context, culture, value, destinationType);
      }
    }

    [EditorBrowsableAttribute(EditorBrowsableState.Never)]
    public sealed class SizeConverter : TypeConverter {  // nested class, must be public for Silverlight
      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
      }

      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
      }

      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
        string source = value as string;
        if (source != null) {
          try {
            String[] parsed = source.Split(' ');
            double w = ((parsed.Length >= 1) ? Double.Parse(parsed[0], System.Globalization.CultureInfo.InvariantCulture) : 0);
            double h = ((parsed.Length >= 2) ? Double.Parse(parsed[1], System.Globalization.CultureInfo.InvariantCulture) : 0);
            return new Size(w, h);
          } catch (Exception) { }
          return new Size(0, 0);
        }
        return base.ConvertFrom(context, culture, value);
      }

      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
        if (destinationType != typeof(String) && value is Size) {
          Size s = (Size)value;
          return s.Width.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + s.Height.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        return base.ConvertTo(context, culture, value, destinationType);
      }
    }

#pragma warning restore 1591
  }


  internal sealed class LinkBundle {  // also used by Parts and PartManager
    public LinkBundle() {
      this.Spacing = 10;
    }

    public Node Node1 { get; set; }
    public Object Param1 { get; set; }
    public Node Node2 { get; set; }
    public Object Param2 { get; set; }
    public List<Link> Links { get; set; }
    public double Spacing { get; set; }
  }


  internal sealed class PositionArray {  // used by Route and DiagramPanel
    private const int OCCUPIED = 0;
    private const int VERT = 1;
    private const int HORIZ = 2;
    private const int MASK = VERT | HORIZ;
    private const int SHIFT = 2;
    private const int START = 4;
    private const int STEP = 4;
    private const int MAX = int.MaxValue & ~MASK;
    private const int UNOCCUPIED = MAX | MASK;

    internal const int StartDistance = START >> SHIFT;
    internal const int StepDistance = STEP >> SHIFT;
    internal const int MaxDistance = MAX >> SHIFT;

    internal PositionArray() { }

    internal void Initialize(Rect rect) {
      if (rect.Width <= 0 || rect.Height <= 0) return;
      double minx = rect.X;
      double miny = rect.Y;
      double maxx = rect.X+rect.Width;
      double maxy = rect.Y+rect.Height;
      // add a row and a column on all sides
      _MinX = Math.Floor((minx-_CellX)/_CellX)*_CellX;
      _MinY = Math.Floor((miny-_CellY)/_CellY)*_CellY;
      _MaxX = Math.Ceiling((maxx+2*_CellX)/_CellX)*_CellX;
      _MaxY = Math.Ceiling((maxy+2*_CellY)/_CellY)*_CellY;
      int xrange = 1+(int)Math.Ceiling((_MaxX-_MinX)/_CellX);
      int yrange = 1+(int)Math.Ceiling((_MaxY-_MinY)/_CellY);
      if (_Array == null || _UpperBoundX < xrange-1 || _UpperBoundY < yrange-1) {
        _Array = new int[xrange, yrange];
        _UpperBoundX = xrange-1;
        _UpperBoundY = yrange-1;
      }
      SetAll(UNOCCUPIED);
    }

    internal bool Invalid {
      get { return _Invalid; }
      set { _Invalid = value; }
    }

    internal Group SubGraph { get; set; }

    internal bool Abort {
      get { return _Abort; }
      set { _Abort = value; }
    }

    internal Rect Bounds {
      get { return new Rect(_MinX, _MinY, _MaxX-_MinX, _MaxY-_MinY); }
    }

    internal Size CellSize {
      get { return new Size(_CellX, _CellY); }
      set {
        if (value.Width > 0 && value.Height > 0 &&
            (value.Width != _CellX || value.Height != _CellY)) {
          _CellX = value.Width;
          _CellY = value.Height;
          Initialize(new Rect(_MinX, _MinY, _MaxX-_MinX, _MaxY-_MinY));
        }
      }
    }

    internal bool WholeDocument {  // whether to search the whole document for a valid route
      get { return _WholeDocument; }
      set { _WholeDocument = value; }
    }

    internal double SmallMargin {  // how many cells to inflate around the link's nodes' bounds to search
      get { return _SmallMargin; }
      set { _SmallMargin = value; }
    }

    internal double LargeMargin {  // how many cells to inflate around the link's nodes' bounds to search
      get { return _LargeMargin; }
      set { _LargeMargin = value; }
    }

    private bool InBounds(double x, double y) {
      return (_MinX <= x && x <= _MaxX && _MinY <= y && y <= _MaxY);
    }

    internal int GetDist(double x, double y) {
      if (!InBounds(x, y)) return 0;
      x -= _MinX;
      x /= _CellX;
      y -= _MinY;
      y /= _CellY;
      int ix = (int)x;
      int iy = (int)y;
      return _Array[ix, iy] >> SHIFT;
    }

    internal void SetOccupied(double x, double y) {
      if (!InBounds(x, y)) return;
      x -= _MinX;
      x /= _CellX;
      y -= _MinY;
      y /= _CellY;
      int ix = (int)x;
      int iy = (int)y;
      _Array[ix, iy] = OCCUPIED;
    }
    internal void SetAll(int v) {
      if (_Array == null) return;
      for (int ix = 0; ix <= _UpperBoundX; ix++) {
        for (int iy = 0; iy <= _UpperBoundY; iy++) {
          _Array[ix, iy] = v;
        }
      }
    }

    internal void ClearAllUnoccupied() {
      if (_Array == null) return;
      for (int ix = 0; ix <= _UpperBoundX; ix++) {
        for (int iy = 0; iy <= _UpperBoundY; iy++) {
          if (_Array[ix, iy] >= START) {
            _Array[ix, iy] |= MAX;
          }
        }
      }
    }

    internal bool IsOccupied(double x, double y) { return GetDist(x, y) == OCCUPIED; }

    internal bool IsUnoccupied(double x, double y, double w, double h) {
      if (x > _MaxX) return true;
      if (x + w < _MinX) return true;
      if (y > _MaxY) return true;
      if (y + h < _MinY) return true;
      int ix = (int)((x-_MinX)/_CellX);
      int iy = (int)((y-_MinY)/_CellY);
      int iw = (int)(Math.Max(0, w)/_CellX) + 1;
      int ih = (int)(Math.Max(0, h)/_CellY) + 1;
      if (ix < 0) {
        iw += ix;  // decrease width
        ix = 0;
      }
      if (iy < 0) {
        ih += iy;  // decrease height
        iy = 0;
      }
      if (iw < 0) return true;
      if (ih < 0) return true;
      int mx = Math.Min(ix+iw-1, _UpperBoundX);
      int my = Math.Min(iy+ih-1, _UpperBoundY);
      for (int i = ix; i <= mx; i++) {
        for (int j = iy; j <= my; j++) {
          if (_Array[i, j] == OCCUPIED) {
            return false;
          }
        }
      }
      return true;
    }

    private int Ray(int x, int y, int inc, bool vert, int lowx, int hix, int lowy, int hiy) {
      int val = _Array[x, y] & ~MASK;
      if (val >= START && val < MAX) {
        if (vert)
          y += inc;
        else
          x += inc;
        val += STEP;
        while (lowx <= x && x <= hix && lowy <= y && y <= hiy) {
          int oldval = _Array[x, y];
          if (val >= (oldval & ~MASK)) break;
          _Array[x, y] = (val | (oldval & MASK));
          val += STEP;
          if (vert)
            y += inc;
          else
            x += inc;
        }
      }
      if (vert)
        return y;
      else
        return x;
    }

    private void Spread(int x, int y, int inc, bool vert, int lowx, int hix, int lowy, int hiy) {
      if (x < lowx || x > hix || y < lowy || y > hiy)
        return;
      int end = Ray(x, y, inc, vert, lowx, hix, lowy, hiy);
      if (vert) {
        if (inc > 0) {
          for (int yy = y+inc; yy < end; yy += inc) {
            Spread(x, yy, 1, !vert, lowx, hix, lowy, hiy);
            Spread(x, yy, -1, !vert, lowx, hix, lowy, hiy);
          }
        } else {
          for (int yy = y+inc; yy > end; yy += inc) {
            Spread(x, yy, 1, !vert, lowx, hix, lowy, hiy);
            Spread(x, yy, -1, !vert, lowx, hix, lowy, hiy);
          }
        }
      } else {
        if (inc > 0) {
          for (int xx = x+inc; xx < end; xx += inc) {
            Spread(xx, y, 1, !vert, lowx, hix, lowy, hiy);
            Spread(xx, y, -1, !vert, lowx, hix, lowy, hiy);
          }
        } else {
          for (int xx = x+inc; xx > end; xx += inc) {
            Spread(xx, y, 1, !vert, lowx, hix, lowy, hiy);
            Spread(xx, y, -1, !vert, lowx, hix, lowy, hiy);
          }
        }
      }
    }

    private bool IsBarrier(int v) {
      return (v & MASK) == 0;
    }

    private int BreakOut(int x1, int y1, int x2, int y2, int inc, bool vert, int lowx, int hix, int lowy, int hiy) {
      int x = x1;
      int y = y1;
      int oldval = _Array[x, y];
      while (IsBarrier(oldval) && x > lowx && x < hix && y > lowy && y < hiy) {
        if (vert)
          y += inc;
        else
          x += inc;
        oldval = _Array[x, y];
        if (Math.Abs(x-x2) <= 1 && Math.Abs(y-y2) <= 1) {
          this.Abort = true;  // abort--use default algorithm
          return 0;
        }
      }

      x = x1;
      y = y1;
      oldval = _Array[x, y];
      int val = START | MASK;
      _Array[x, y] = val;
      while (IsBarrier(oldval) && x > lowx && x < hix && y > lowy && y < hiy) {
        if (vert)
          y += inc;
        else
          x += inc;
        oldval = _Array[x, y];
        _Array[x, y] = val;
        val += STEP;
      }
      if (vert)
        return y;
      else
        return x;
    }

    private void BreakIn(int x1, int y1, int x2, int y2, int inc, bool vert, int lowx, int hix, int lowy, int hiy) {
      int x = x2;
      int y = y2;
      int oldval = _Array[x, y];
      while (IsBarrier(oldval) && x > lowx && x < hix && y > lowy && y < hiy) {
        if (vert)
          y += inc;
        else
          x += inc;
        oldval = _Array[x, y];
        if (Math.Abs(x-x1) <= 1 && Math.Abs(y-y1) <= 1) {
          this.Abort = true;  // abort--use default algorithm
          return;
        }
      }

      x = x2;
      y = y2;
      oldval = _Array[x, y];
      _Array[x, y] = UNOCCUPIED;
      while (IsBarrier(oldval) && x > lowx && x < hix && y > lowy && y < hiy) {
        if (vert)
          y += inc;
        else
          x += inc;
        oldval = _Array[x, y];
        _Array[x, y] = UNOCCUPIED;
      }
    }

    internal void Propagate(Point p1, double fromDir, Point p2, double toDir, Rect bounds) {
      if (_Array == null) return;
      this.Abort = false;

      double x1 = p1.X;
      double y1 = p1.Y;
      if (!InBounds(x1, y1)) return;
      x1 -= _MinX;
      x1 /= _CellX;
      y1 -= _MinY;
      y1 /= _CellY;

      double x2 = p2.X;
      double y2 = p2.Y;
      if (!InBounds(x2, y2)) return;
      x2 -= _MinX;
      x2 /= _CellX;
      y2 -= _MinY;
      y2 /= _CellY;

      if (Math.Abs(x1-x2) <= 1 && Math.Abs(y1-y2) <= 1) {
        // if start and end are at the same cell, never mind, just use the default algorithm
        this.Abort = true;
        return;
      }

      double bx1 = bounds.X;
      double by1 = bounds.Y;
      double bx2 = bounds.X+bounds.Width;
      double by2 = bounds.Y+bounds.Height;
      bx1 -= _MinX;
      bx1 /= _CellX;
      by1 -= _MinY;
      by1 /= _CellY;
      bx2 -= _MinX;
      bx2 /= _CellX;
      by2 -= _MinY;
      by2 /= _CellY;

      int lowx = Math.Max(0, Math.Min(_UpperBoundX, (int)bx1));
      int hix = Math.Min(_UpperBoundX, Math.Max(0, (int)bx2));
      int lowy = Math.Max(0, Math.Min(_UpperBoundY, (int)by1));
      int hiy = Math.Min(_UpperBoundY, Math.Max(0, (int)by2));

      int ix1 = (int)x1;
      int iy1 = (int)y1;
      int ix2 = (int)x2;
      int iy2 = (int)y2;
      int ix = ix1;
      int iy = iy1;
      int inc = ((fromDir == RIGHT || fromDir == DOWN) ? 1 : -1);
      bool vert = (fromDir == DOWN || fromDir == UP);
      if (vert)
        iy = BreakOut(ix1, iy1, ix2, iy2, inc, vert, lowx, hix, lowy, hiy);
      else
        ix = BreakOut(ix1, iy1, ix2, iy2, inc, vert, lowx, hix, lowy, hiy);
      //Diagram.Debug("BreakOut " + Diagram.Str(new Point(ix1, iy1)) + Diagram.Str(new Point(ix2, iy2)) + inc.ToString() + " " + vert.ToString() + "  --> " + (vert ? iy : ix).ToString());

      if (this.Abort) return;

      BreakIn(ix1, iy1, ix2, iy2, ((toDir == RIGHT || toDir == DOWN) ? 1 : -1), (toDir == DOWN || toDir == UP), lowx, hix, lowy, hiy);
      //Diagram.Debug("  In " + ((toDir == RIGHT || toDir == DOWN) ? 1 : -1).ToString() + " " + (toDir == DOWN || toDir == UP).ToString());

      if (this.Abort) return;

      Spread(ix, iy, 1, false, lowx, hix, lowy, hiy);
      Spread(ix, iy, -1, false, lowx, hix, lowy, hiy);
      Spread(ix, iy, 1, true, lowx, hix, lowy, hiy);
      Spread(ix, iy, -1, true, lowx, hix, lowy, hiy);
    }












































    private const double RIGHT = 0;
    private const double DOWN = 90;
    private const double LEFT = 180;
    private const double UP = 270;

    private bool _Invalid = true;
    private bool _Abort;
    private double _MinX = 1;
    private double _MinY = 1;
    private double _MaxX = -1;
    private double _MaxY = -1;
    private double _CellX = 8;
    private double _CellY = 8;
    private int[,] _Array;
    private int _UpperBoundX;
    private int _UpperBoundY;
    private bool _WholeDocument = false;
    private double _SmallMargin = 22;
    private double _LargeMargin = 111;
  }


  internal sealed class Knot {  // also used by Parts
    public Node Node { get; set; }
    public FrameworkElement Port { get; set; }
    public FrameworkElement Extension { get; set; }












    private double GetSideAngle(Rect rect, Spot spot, double generalangle, bool ortho) {
      int sides = (int)spot.OffsetY;
      switch (sides) {
        case Spot.MBottom: return 90;
        case Spot.MLeft: return 180;
        case Spot.MTop: return 270;
        case Spot.MRight: return 0;
      }
      double A = generalangle;
      switch (sides) {
        case Spot.MBottom | Spot.MTop: if (A > 180) return 270; else return 90;
        case Spot.MLeft | Spot.MRight: if (A > 90 && A <= 270) return 180; else return 0;
      }
      double R = Math.Atan2(rect.Height, rect.Width)*180/Math.PI;
      switch (sides) {
        case Spot.MLeft | Spot.MTop: if (A > R && A <= 180+R) return 180; else return 270;
        case Spot.MTop | Spot.MRight: if (A > 180-R && A <= 360-R) return 270; else return 0;
        case Spot.MRight | Spot.MBottom: if (A > R && A <= 180+R) return 90; else return 0;
        case Spot.MBottom | Spot.MLeft: if (A > 180-R && A <= 360-R) return 180; else return 90;
        case Spot.MLeft | Spot.MTop | Spot.MRight: if (A > 90 && A <= 180+R) return 180; else if (A > 180+R && A <= 360-R) return 270; else return 0;
        case Spot.MTop | Spot.MRight | Spot.MBottom: if (A > 180 && A <= 360-R) return 270; else if (A > R && A <= 180) return 90; else return 0;
        case Spot.MRight | Spot.MBottom | Spot.MLeft: if (A > R && A <= 180-R) return 90; else if (A > 180-R && A <= 270) return 180; else return 0;
        case Spot.MBottom | Spot.MLeft | Spot.MTop: if (A > 180-R && A <= 180+R) return 180; else if (A > 180+R) return 270; else return 90;
      }
      if (ortho && sides != (Spot.MLeft|Spot.MTop|Spot.MRight|Spot.MBottom)) {
        A -= 15;
        if (A < 0) A += 360;
      }
      if (A > R && A < 180-R)
        return 90;
      else if (A >= 180-R && A <= 180+R)
        return 180;
      else if (A > 180+R && A < 360-R)
        return 270;
      else
        return 0;
    }

    // first updates any needed data structures
    internal LinkInfo FindLinkInfo(Link link) {
      if (mySortedLinks == null) GetLinkInfos();
      foreach (LinkInfo info in mySortedLinks) {
        if (info != null && info.Link == link) return info;
      }
      return null;
    }

    //??? what about self-loops
    private LinkInfo[] GetLinkInfos() {
      if (!myRespreading) {
        bool oldRespreading = myRespreading;
        myRespreading = true;
        IEnumerable<Link> connectedlinks = this.Node.LinksConnected;
        int linkscount = connectedlinks.Count();
        mySortedLinks = new LinkInfo[linkscount];
        int i = 0;
        Rect thisrect = this.Node.GetElementBounds(this.Extension);
        foreach (Link l in connectedlinks) {
          Spot spot = Spot.None;
          if (l.FromPort == this.Port) {
            spot = l.Route.GetFromSpot();  // link's Spot takes precedence, if defined
          } else {
            spot = l.Route.GetToSpot();  // link's Spot takes precedence, if defined
          }
          if (!spot.IsSide) continue;
          FrameworkElement otherport = l.GetOtherPort(this.Port);
          Node othernode = Diagram.FindAncestor<Node>(otherport);
          if (othernode == null) continue;
          Rect otherrect = othernode.GetElementBounds(otherport);
          Point otherpoint = Spot.Center.PointInRect(otherrect);
          LinkInfo otherinfo = l.Route.FindExistingLinkInfo(otherport, l);
          if (otherinfo != null) {
            otherpoint = otherinfo.LinkPoint;
          }
          double angle = Geo.GetAngle(Spot.Center.PointInRect(thisrect), otherpoint);
          double dir = GetSideAngle(thisrect, spot, angle, l.Route.Orthogonal);
          int side;
          if (dir == 0) {
            side = Spot.MRight;
            if (angle > 180) angle -= 360;
          } else if (dir == 90) {
            side = Spot.MBottom;
          } else if (dir == 180) {
            side = Spot.MLeft;
          } else {
            side = Spot.MTop;
          }
          LinkInfo info = mySortedLinks[i];
          if (info == null) {
            info = new LinkInfo(l, angle, side);
            mySortedLinks[i] = info;
          } else {
            info.Link = l;
            info.Angle = angle;
            info.Side = side;
          }
          info.OtherPoint = otherpoint;
          i++;
        }
        SortLinkInfos(mySortedLinks);
        // now assign IndexOnSide for each set of LinkInfos with the same Side
        int numlinks = mySortedLinks.Length;
        int currside = -1;
        int numonside = 0;
        for (i = 0; i < numlinks; i++) {
          LinkInfo info = mySortedLinks[i];
          if (info == null) continue;
          if (info.Side != currside) {
            currside = info.Side;
            numonside = 0;
          }
          info.IndexOnSide = numonside;
          numonside++;
        }
        // the highest IndexOnSide now also indicates how many there are
        // with the same Side--assign CountOnSide correspondingly;
        // also compute the LinkPoint
        currside = -1;
        numonside = 0;
        for (i = numlinks-1; i >= 0; i--) {  // backwards!
          LinkInfo info = mySortedLinks[i];
          if (info == null) continue;
          if (info.Side != currside) {
            currside = info.Side;
            numonside = info.IndexOnSide+1;
          }
          info.CountOnSide = numonside;
        }
        AssignLinkPoints(mySortedLinks);
        AssignEndSegmentLengths(mySortedLinks);
        myRespreading = oldRespreading;

        //Diagram.Debug(ToString());
        //foreach (LinkInfo info in mySortedLinks) {
        //  if (info == null) continue;
        //  if (info.Link == null) continue;
        //  Route route = info.Link.Route;
        //  if (route == null) continue;
        //  Point pt1 = route.GetPoint(0);
        //  Point pt2 = route.GetPoint(route.PointsCount-1);
        //  if (!Geo.IsApprox(info.LinkPoint, pt1) && !Geo.IsApprox(info.LinkPoint, pt2)) {
        //    Diagram.Debug("  OUT-OF-DATE: " + info.Link.ToString() + " " + Diagram.Str(pt1) + Diagram.Str(pt2));
        //  }
        //}
      }
      return mySortedLinks;
    }

    // sorting all of the link's other ports in circular order
    private sealed class EndPositionComparer : IComparer<LinkInfo> {  // nested class
      public int Compare(LinkInfo a, LinkInfo b) {
        if (a == b) return 0;
        if (a == null) return -1;
        if (b == null) return 1;
        if (a.Side < b.Side) {
          return -1;
        } else if (a.Side > b.Side) {
          return 1;
        } else {
          if (a.Angle < b.Angle)
            return -1;
          else if (a.Angle > b.Angle)
            return 1;
          else
            return 0;
        }
      }
    }

    private static IComparer<LinkInfo> myComparer = new EndPositionComparer();

    // <summary>
    // Sort an array of angle and side information about the links connected to this port.
    // </summary>
    // <param name="linkinfos">an array of <see cref="LinkInfo"/> that is modified</param>
    // <remarks>
    // By default this just sorts by <see cref="LinkInfo.Side"/> group, and by
    // <see cref="LinkInfo.Angle"/> for each side.
    // </remarks>
    void SortLinkInfos(LinkInfo[] linkinfos) {
      Array.Sort<LinkInfo>(linkinfos, 0, linkinfos.Length, myComparer);
    }

    // <summary>
    // Given a sorted array of angle and side information about the links connected to this port,
    // assign the actual <see cref="LinkInfo.LinkPoint"/>.
    // </summary>
    // <param name="linkinfos">an array of <see cref="LinkInfo"/></param>
    // <remarks>
    // By default this just spreads the link points evenly along each side.
    // </remarks>
    void AssignLinkPoints(LinkInfo[] linkinfos) {
      for (int i = 0; i < linkinfos.Length; i++) {
        LinkInfo info = linkinfos[i];
        if (info == null) continue;
        info.LinkPoint = GetSideLinkPoint(info);
      }
    }

    // <summary>
    // Given a sorted array of angle, side, and link-point information about the links
    // connected to this port, assign the actual
    // <see cref="LinkInfo.EndSegmentLength"/>.
    // </summary>
    // <param name="linkinfos"></param>
    // <remarks>
    // By default this just specifies shorter values for links at the ends of each side,
    // and longer values for links in the middle of each side.
    // </remarks>
    void AssignEndSegmentLengths(LinkInfo[] linkinfos) {
      for (int i = 0; i < linkinfos.Length; i++) {
        LinkInfo info = linkinfos[i];
        if (info == null) continue;
        info.EndSegmentLength = GetEndSegmentLength(info);
      }
    }

    private Point GetSideLinkPoint(LinkInfo info) {
      Node node = this.Node;
      Rect r = node.GetElementBounds(this.Extension);
      Point a;
      Point b;
      switch (info.Side) {
        case Spot.MBottom: a = Spot.BottomRight.PointInRect(r); b = Spot.BottomLeft.PointInRect(r); break;
        case Spot.MLeft: a = Spot.BottomLeft.PointInRect(r); b = Spot.TopLeft.PointInRect(r); break;
        case Spot.MTop: a = Spot.TopLeft.PointInRect(r); b = Spot.TopRight.PointInRect(r); break;
        default:
        case Spot.MRight: a = Spot.TopRight.PointInRect(r); b = Spot.BottomRight.PointInRect(r); break;
      }
      double dx = b.X-a.X;
      double dy = b.Y-a.Y;
      double part = ((double)info.IndexOnSide+1)/((double)info.CountOnSide+1);
      return new Point(a.X + dx*part, a.Y + dy*part);
    }

    private double GetEndSegmentLength(LinkInfo info) {
      Link link = info.Link;
      // get default EndSegmentLength
      double esl;
      if (link.FromPort == this.Port) {
        esl = link.Route.FromEndSegmentLength;
        if (Double.IsNaN(esl)) {
          esl = Node.GetFromEndSegmentLength(this.Port);
        }
      } else {
        esl = link.Route.ToEndSegmentLength;
        if (Double.IsNaN(esl)) {
          esl = Node.GetToEndSegmentLength(this.Port);
        }
      }
      if (Double.IsNaN(esl)) esl = 10;

      int idx = info.IndexOnSide;
      if (idx < 0) return esl;  // if unused, don't extend length
      int count = info.CountOnSide;
      if (count <= 1) return esl;  // if there's just one, don't bother

      bool ortho = link.Route.Orthogonal;
      if (!ortho) return esl;

      bool from = link.FromPort == this.Port;
      Point pc = info.OtherPoint;
      Point thisc = info.LinkPoint;
      // indexed clockwise, not by absolute positions
      if (info.Side == Spot.MLeft || info.Side == Spot.MBottom) idx = count-1 - idx;
      double step = 8;
      bool horiz = (info.Side == Spot.MLeft || info.Side == Spot.MRight);
      if (horiz ? pc.Y < thisc.Y : pc.X < thisc.X)
        return esl + idx * step;
      else if (horiz ? pc.Y == thisc.Y : pc.X == thisc.X)
        return esl;
      else
        return esl + (count-1 - idx) * step;
    }

    private LinkInfo[] mySortedLinks;
    private bool myRespreading;
  }


  // This is used by <see cref="Knot.SortLinkInfos"/>,
  // <see cref="Knot.AssignLinkPoints"/>, and
  // <see cref="Knot.AssignEndSegmentLengths"/>
  // only when <see cref="Knot.LinkPointsSpread"/> is true.
  // This information is transient, just used when calculating link points.
  internal sealed class LinkInfo {
    public LinkInfo(Link l, double a, int s) {
      this.Link = l;
      this.Angle = a;
      this.Side = s;
    }









    // <summary>
    // The <see cref="Link"/> whose link point is being sorted around the port.
    // </summary>
    public Link Link { get; set; }

    // <summary>
    // The effective angle at which the link connects with another node;
    // this value corresponds to the result of calling GetAngle.
    // </summary>
    // <value>
    // The angle in degrees from this port to the other port.
    // </value>
    public double Angle { get; set; }

    // <summary>
    // The side at which the link connects;
    // this value corresponds to the result of calling GetDirection.
    // </summary>
    // <value>
    // One of <see cref="Spot.MiddleRight"/>,
    // <see cref="Spot.MiddleBottom"/>, <see cref="Spot.MiddleLeft"/>,
    // <see cref="Spot.MiddleTop"/>.
    // </value>
    public int Side { get; set; }

    public Point OtherPoint { get; set; }

    // <summary>
    // How many links are connected on this side;
    // computed after calling SortLinkInfos and before calling AssignLinkPoints.
    // </summary>
    public int CountOnSide { get; set; }

    // <summary>
    // The index of this link on this side;
    // computed after calling SortLinkInfos and before calling AssignLinkPoints.
    // </summary>
    public int IndexOnSide { get; set; }

    // <summary>
    // The document point at which the link should terminate;
    // should be set in AssignLinkPoints.
    // </summary>
    public Point LinkPoint { get; set; }

    // <summary>
    // The value of length of the last segment for this link at this port;
    // should be set in AssignEndSegmentLengths.
    // </summary>
    public double EndSegmentLength { get; set; }
  }


  /// <summary>
  /// This enumeration defines how a path is drawn through a <see cref="Route"/>'s points.
  /// </summary>
  /// <remarks>
  /// This is used as the value of <see cref="Route.Curve"/>.
  /// </remarks>
  public enum LinkCurve {
    /// <summary>
    /// Straight line segments between the <see cref="Route"/>'s points.
    /// </summary>
    None = 0,
    /// <summary>
    /// Straight line segments that also hop over other orthogonal link segments that it crosses.
    /// <see cref="JumpGap"/> is similar.
    /// (Only applies when the route is orthogonal.)
    /// </summary>
    JumpOver = 1,
    /// <summary>
    /// A Bezier curve, perhaps using the <see cref="Route.Curviness"/> property.
    /// </summary>
    Bezier = 2,
    /// <summary>
    /// Like <see cref="JumpOver"/>, but leaves a gap in the path when it crosses other orthogonal link segments.
    /// (Only applies when the route is orthogonal.)
    /// </summary>
    JumpGap = 3
  }

  /// <summary>
  /// This enumeration describes how a <see cref="Route"/> computes its points.
  /// </summary>
  /// <remarks>
  /// This is used as the value of <see cref="Route.Adjusting"/>.
  /// </remarks>
  public enum LinkAdjusting {
    /// <summary>
    /// Always discards any old points and recomputes all points according to standard policies.
    /// </summary>
    None = 0,
    /// <summary>
    /// When there are more than the standard number of points in the route,
    /// scale and rotate the intermediate points so that the link's shape stays
    /// approximately the same.
    /// This style, implemented by <see cref="Route.RescalePoints"/>, does not
    /// maintain the horizontal and vertical line segments of an orthogonal link.
    /// Orthogonal links with this adjusting style will instead recalculate
    /// the standard route path, as if the adjusting style were <see cref="None"/>.
    /// </summary>
    Scale,
    /// <summary>
    /// When there are more than the standard number of points in the route,
    /// linearly interpolate the intermediate points along the X and Y dimensions
    /// between the ports.
    /// This style, implemented by <see cref="Route.StretchPoints"/>, does not
    /// maintain the horizontal and vertical line segments of an orthogonal link.
    /// Orthogonal links with this adjusting style will instead only modify the
    /// end points of the existing path, as if the adjusting style were <see cref="End"/>.
    /// </summary>
    Stretch,
    /// <summary>
    /// When there are more than the standard number of points in the route,
    /// or if the route is orthogonal, just modify the end points,
    /// while leaving the intermediate points unchanged.
    /// This style maintains orthogonality for orthogonal links.
    /// </summary>
    End
  }

  /// <summary>
  /// This enumeration describes how the <see cref="Route"/>'s computation of a path
  /// considers parts other than the two <see cref="Node"/>s to which the route's <see cref="Link"/>
  /// is connected.
  /// </summary>
  /// <remarks>
  /// This is used as the value of <see cref="Route.Routing"/>.
  /// </remarks>
  public enum LinkRouting {
    /// <summary>
    /// The route only takes the link's two nodes into account.
    /// </summary>
    Normal = 0,
    /// <summary>
    /// The segments of the route are always either horizontal or vertical;
    /// nodes other than the link's two nodes are ignored.
    /// This routing style is not desirable when the curve is <see cref="LinkCurve.Bezier"/>.
    /// </summary>
    Orthogonal = 1,
    /// <summary>
    /// The route tries to avoid crossing over other nodes.
    /// Currently this routing style also implies that it is <see cref="Orthogonal"/>.
    /// The other nodes must be <see cref="Node.Avoidable"/>.
    /// </summary>
    AvoidsNodes=3
  }

  /// <summary>
  /// This enumeration describes how the <see cref="Route"/> will compute an end segment angle
  /// in <see cref="Route.GetLinkDirection"/>.
  /// </summary>
  /// <remarks>
  /// This is used as the value of <see cref="Route.FromEndSegmentDirection"/>
  /// and <see cref="Route.ToEndSegmentDirection"/>.
  /// </remarks>
  public enum LinkEndSegmentDirection {
    /// <summary>
    /// The angle is the same even if the node has been rotated.
    /// </summary>
    Absolute,
    /// <summary>
    /// The angle is rotated to match the <see cref="Node.RotationAngle"/>.
    /// </summary>
    RotatedNode,
    /// <summary>
    /// The angle is rotated to match the <see cref="Node.RotationAngle"/>,
    /// but only in increments of multiples of 90 degrees.
    /// </summary>
    RotatedNodeOrthogonal,
  }
}
