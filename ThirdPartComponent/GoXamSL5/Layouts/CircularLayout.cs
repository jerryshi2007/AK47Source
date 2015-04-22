
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

namespace Northwoods.GoXam.Layout {

  /// <summary>
  /// Position nodes in a circular arrangement.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Although this class inherits from <c>FrameworkElement</c>
  /// in order to support data binding,
  /// it is not really a <c>FrameworkElement</c> or <c>UIElement</c>!
  /// Please ignore all of the properties, methods, and events defined by
  /// <c>FrameworkElement</c> and <c>UIElement</c>.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class CircularLayout : DiagramLayout {
    /// <summary>
    /// Construct a layout with the default values.
    /// </summary>
    public CircularLayout() {
    }

    /// <summary>
    /// Make a copy of a <see cref="CircularLayout"/>, copying most of the
    /// important properties except for the <see cref="Network"/>.
    /// </summary>
    /// <param name="layout"></param>
    /// <remarks>
    /// </remarks>
    public CircularLayout(CircularLayout layout)
      : base(layout) {
      if (layout != null) {
        this.Network = null;
      }
    }

    static CircularLayout() {
      NetworkProperty = DependencyProperty.Register("Network", typeof(CircularNetwork), typeof(CircularLayout),
        new FrameworkPropertyMetadata(null, OnNetworkChanged));

      RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(CircularLayout),
        new FrameworkPropertyMetadata(Double.NaN, OnPropertyChanged));  // may be NaN

      AspectRatioProperty = DependencyProperty.Register("AspectRatio", typeof(double), typeof(CircularLayout),
        new FrameworkPropertyMetadata(1.0, OnPropertyChanged));

      StartAngleProperty = DependencyProperty.Register("StartAngle", typeof(double), typeof(CircularLayout),
        new FrameworkPropertyMetadata(0.0, OnPropertyChanged));

      SweepAngleProperty = DependencyProperty.Register("SweepAngle", typeof(double), typeof(CircularLayout),
        new FrameworkPropertyMetadata(360.0, OnPropertyChanged));

      ArrangementProperty = DependencyProperty.Register("Arrangement", typeof(CircularArrangement), typeof(CircularLayout),
        new FrameworkPropertyMetadata(CircularArrangement.ConstantSpacing, OnPropertyChanged));

      DirectionProperty = DependencyProperty.Register("Direction", typeof(CircularDirection), typeof(CircularLayout),
        new FrameworkPropertyMetadata(CircularDirection.Clockwise, OnPropertyChanged));

      SortingProperty = DependencyProperty.Register("Sorting", typeof(CircularSorting), typeof(CircularLayout),
        new FrameworkPropertyMetadata(CircularSorting.Forwards, OnPropertyChanged));

      ComparerProperty = DependencyProperty.Register("Comparer", typeof(IComparer<CircularVertex>), typeof(CircularLayout),
        new FrameworkPropertyMetadata(null, OnPropertyChanged));

      SpacingProperty = DependencyProperty.Register("Spacing", typeof(double), typeof(CircularLayout),
        new FrameworkPropertyMetadata(6.0, OnPropertyChanged));  // may be NaN

      NodeDiameterFormulaProperty = DependencyProperty.Register("NodeDiameterFormula", typeof(CircularNodeDiameterFormula), typeof(CircularLayout),
        new FrameworkPropertyMetadata(CircularNodeDiameterFormula.Pythagorean, OnPropertyChanged));

      RoutingProperty = DependencyProperty.Register("Routing", typeof(CircularLinkRouting), typeof(CircularLayout),
        new FrameworkPropertyMetadata(CircularLinkRouting.Default, OnPropertyChanged));
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      CircularLayout layout = (CircularLayout)d;
      layout.InvalidateLayout();
    }

    /// <summary>
    /// Identifies the <see cref="Radius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RadiusProperty;
    /// <summary>
    /// Gets/sets the horizontal radius of the elliptical arrangement
    /// </summary>
    /// <value>
    /// The default value is <b>NaN</b>.
    /// <b>NaN</b> indicates that the <see cref="Spacing"/> will determing size of ring.
    /// If <see cref="Spacing"/> is also <b>NaN</b>, the effective spacing will be 6.
    /// If <see cref="Spacing"/> is a number, the effective radius will be > Radius if and only if
    /// the spacing between elements would otherwise be less than spacing.
    /// The specified value for <see cref="Radius"/> will be ignored if <see cref="Arrangement"/>==<see cref="CircularArrangement.Packed"/>.
    /// This property must always be positive or <b>NaN</b>.
    /// </value>
    public double Radius {
      get { return (double)GetValue(RadiusProperty); }
      set { SetValue(RadiusProperty, value); }
    }
    private double ERadius = 0;

    /// <summary>
    /// Identifies the <see cref="AspectRatio"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AspectRatioProperty;
    /// <summary>
    /// Gets or sets the ratio of the arrangement's height to its width 
    /// (1 for a circle, &gt;1 for a vertically elongated ellipse).
    /// </summary>
    /// <value>
    /// This is 1 by default.
    /// The value must be a positive number.
    /// </value>
    /// <remarks>
    /// Modifying this value changes the height, but keeps the width and the <see cref="Radius" /> constant.
    /// </remarks>
    public double AspectRatio {
      get { return (double)GetValue(AspectRatioProperty); }
      set { SetValue(AspectRatioProperty, value); }
    }
    private double EAspectRatio = 0;

    /// <summary>
    /// Identifies the <see cref="StartAngle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StartAngleProperty;
    /// <summary>
    /// Gets or sets the angle (in degrees, clockwise from the positive side of the X axis) of the first element.
    /// </summary>
    /// <value>
    /// The default value is 0.
    /// </value>
    public double StartAngle {
      get { return (double)GetValue(StartAngleProperty); }
      set { SetValue(StartAngleProperty, value); }
    }
    private double EStartAngle = 0;

    /// <summary>
    /// Identifies the <see cref="SweepAngle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SweepAngleProperty;
    /// <summary>
    /// Gets or sets the absolute angle (in degrees) between the first and last node.
    /// </summary>
    /// <value>
    /// The default value is 360.
    /// The value must be greater than zero and less than or equal to 360.
    /// If it is not in this range, it will be automatically set to 360.
    /// </value>
    /// <remarks>
    /// Whether the arrangement is clockwise or counterclockwise does not depend on the sign of this value.
    /// The direction can be controlled by setting <see cref="Direction" />.
    /// If 360 is the specified value, the actual value will be less to keep the first and last
    /// elements from overlapping, and the spacing between the first and last nodes will be determined
    /// the same way as for all other adjacent nodes.
    /// </remarks>
    public double SweepAngle {
      get { return (double)GetValue(SweepAngleProperty); }
      set { SetValue(SweepAngleProperty, value); }
    }
    private double ESweepAngle = 360;

    /// <summary>
    /// Identifies the <see cref="Arrangement"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ArrangementProperty;
    /// <summary>
    /// Gets or sets how the nodes are spaced.
    /// If <see cref="Arrangement"/> == <see cref="CircularArrangement.Packed"/>,
    /// the specified <see cref="Radius"/> will be ignored
    /// </summary>
    /// <value>
    /// The default value is <see cref="CircularArrangement.ConstantSpacing"/>.
    /// </value>
    public CircularArrangement Arrangement {
      get { return (CircularArrangement)GetValue(ArrangementProperty); }
      set { SetValue(ArrangementProperty, value); }
    }
    private CircularArrangement EArrangement = CircularArrangement.ConstantSpacing;

    /// <summary>
    /// Identifies the <see cref="Direction"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DirectionProperty;
    /// <summary>
    /// Gets or sets whether the nodes are arranged clockwise or counterclockwise.
    /// </summary>
    /// <value>
    /// The default value is <see cref="CircularDirection.Clockwise"/>.
    /// </value>
    public CircularDirection Direction {
      get { return (CircularDirection)GetValue(DirectionProperty); }
      set { SetValue(DirectionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Sorting"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SortingProperty;
    /// <summary>
    /// Gets or sets if and how the nodes are sorted.
    /// </summary>
    /// <value>
    /// <see cref="CircularSorting.Forwards"/> indicates that the nodes are arranged in the order the layout gets them.
    /// <see cref="CircularSorting.Reverse"/> indicates that the nodes are arranged in the reverse order that the layout gets them.
    /// <see cref="CircularSorting.Ascending"/> and <see cref="CircularSorting.Descending"/> indicate that the nodes
    /// will be sorted using the <see cref="Comparer"/>
    /// <see cref="CircularSorting.Optimized"/> indicates that the nodes will be arranged to minimize link crossings
    /// The default value is <see cref="CircularSorting.Optimized"/>.
    /// </value>
    public CircularSorting Sorting {
      get { return (CircularSorting)GetValue(SortingProperty); }
      set { SetValue(SortingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Comparer"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ComparerProperty;
    /// <summary>
    /// Gets or sets the comparer which sorts the data when <see cref="Sorting"/> is
    /// set to <see cref="CircularSorting.Ascending"/> or <see cref="CircularSorting.Descending"/>.
    /// </summary>
    /// <value>
    /// The default is null, meaning the vertices are not sorted (this is equivalent to specifying
    /// <see cref="CircularSorting.Forwards"/> for the <see cref="Sorting"/> property.)
    /// </value>
    public IComparer<CircularVertex> Comparer {
      get { return (IComparer<CircularVertex>)GetValue(ComparerProperty); }
      set { SetValue(ComparerProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Spacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SpacingProperty;
    /// <summary>
    /// Gets or sets the distance between nodes (if <see cref="Radius"/> is <b>NaN</b>)
    /// or the minimum distance between nodes (if <see cref="Radius"/> is a number).
    /// </summary>
    /// <value>
    /// The default value is 6.
    /// The value may be <b>NaN</b>.
    /// </value>
    /// <remarks>
    /// If <see cref="Spacing"/> is <b>NaN</b>, there is no minimum spacing, allowing nodes to overlap,
    /// unless <see cref="Radius"/> is <b>NaN</b>,
    /// in which case the effective spacing will be 6 to determine an effective radius.
    /// If <see cref="Spacing"/> is a number but <see cref="Radius"/> isn't,
    /// the effective spacing will be Spacing, and this will determine the effective radius.
    /// If both <see cref="Spacing"/> and <see cref="Radius"/> are numbers,
    /// the effective radius will be at least <see cref="Radius"/>,
    /// but may be larger so that the minimum spacing between nodes is <see cref="Spacing"/>.
    /// </remarks>
    public double Spacing {
      get { return (double)GetValue(SpacingProperty); }
      set { SetValue(SpacingProperty, value); }
    }
    private double ESpacing = 0;

    /// <summary>
    /// Identifies the <see cref="NodeDiameterFormula"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NodeDiameterFormulaProperty;
    /// <summary>
    /// Specifies how the diameter of nodes will be calculated.
    /// When a node is not circular, it is not clear what its diameter is.
    /// </summary>
    /// <value>
    /// The default is <see cref="CircularNodeDiameterFormula.Pythagorean"/>.
    /// </value>
    public CircularNodeDiameterFormula NodeDiameterFormula {
      get { return (CircularNodeDiameterFormula)GetValue(NodeDiameterFormulaProperty); }
      set { SetValue(NodeDiameterFormulaProperty, value); }
    }
    private CircularNodeDiameterFormula ENodeDiameterFormula;

    /// <summary>
    /// Identifies the <see cref="Routing"/> dependency property.
    /// </summary>
    internal /*??? public */ static readonly DependencyProperty RoutingProperty;
    /// <summary>
    /// Specifies whether the links should be curved to avoid nodes.
    /// </summary>
    /// <value>
    /// The default is <see cref="CircularLinkRouting.Default"/>.
    /// </value>
    internal /*??? public */ CircularLinkRouting Routing {
      get { return (CircularLinkRouting)GetValue(RoutingProperty); }
      set { SetValue(RoutingProperty, value); }
    }

    private double totalelementsizes = 0; // the sum of the diameters of all the elements of the layout
    private double Yradius = 0;
    private double constdist = 0; // for CircularArrangement.ConstantDistance || ConstantAngle


    // read-only properties describing layout results:

    /// <summary>
    /// Gets the effective X Radius that may have been calculated by the layout.
    /// </summary>
    public double ActualXRadius { get { return ERadius; } }
    /// <summary>
    /// Gets the effective Y Radius that may have been calculated by the layout.
    /// </summary>
    public double ActualYRadius { get { return Yradius; } }
    /// <summary>
    /// Gets the effective Spacing that may have been calculated by the layout.
    /// </summary>
    public double ActualSpacing { get { return ESpacing; } }
    /// <summary>
    /// Returns the coordinates of the center of the laid-out ellipse.
    /// </summary>
    public Point ActualCenter {
      get {
        if (Double.IsNaN(this.ArrangementOrigin.X) || Double.IsNaN(this.ArrangementOrigin.Y)) {
          return new Point(0, 0);  // if no ArrangementOrigin, center at origin
        }
        return new Point(this.ArrangementOrigin.X + this.ActualXRadius, this.ArrangementOrigin.Y + this.ActualYRadius);
      }
    }


    /// <summary>
    /// Identifies the <see cref="Network"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NetworkProperty;
    /// <summary>
    /// Gets or sets the <see cref="CircularNetwork"/> that the layout will be performed on.
    /// </summary>
    /// <value>
    /// The initial value is null.
    /// </value>
    public CircularNetwork Network {
      get { return (CircularNetwork)GetValue(NetworkProperty); }
      set { SetValue(NetworkProperty, value); }
    }
    private static void OnNetworkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      CircularLayout layout = (CircularLayout)d;
      CircularNetwork net = (CircularNetwork)e.NewValue;
      if (net != null) net.Layout = layout;
    }

    /// <summary>
    /// Allocate a <see cref="CircularNetwork"/>.
    /// </summary>
    /// <returns></returns>
    public virtual CircularNetwork CreateNetwork() {
      CircularNetwork n = new CircularNetwork();
      n.Layout = this;
      return n;
    }

    /// <summary>
    /// Create and initialize a <see cref="CircularNetwork"/> with the given nodes and links.
    /// </summary>
    /// <param name="nodes">The nodes in the network</param>
    /// <param name="links">The links in the network</param>
    /// <returns>The <see cref="CircularNetwork"/> created</returns>
    public virtual CircularNetwork MakeNetwork(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      CircularNetwork net = CreateNetwork();
      net.AddNodesAndLinks(nodes, links);
      return net;
    }

    /// <summary>
    /// Finds the effective values, which may differ from actual values
    /// </summary>
    private void SetEffectiveValues(out List<CircularVertex> vertices, out List<CircularVertex> evens, out List<CircularVertex> odds) {
      vertices = Sort(this.Network.Vertexes.ToArray());
      evens = new List<CircularVertex>();
      odds = new List<CircularVertex>();

      // fetch local copies of property values; if invalid, use a valid value instead

      EArrangement = this.Arrangement;
      ENodeDiameterFormula = this.NodeDiameterFormula;

      ERadius = this.Radius;
      if (Double.IsInfinity(ERadius) || ERadius <= 0) {
        ERadius = Double.NaN;
      }

      EAspectRatio = this.AspectRatio;
      if (Double.IsNaN(EAspectRatio) || Double.IsInfinity(EAspectRatio) || EAspectRatio <= 0) {
        EAspectRatio = 1.0;
      }

      EStartAngle = this.StartAngle;
      if (Double.IsNaN(EStartAngle) || Double.IsInfinity(EStartAngle)) {
        EStartAngle = 0;
      }

      ESweepAngle = this.SweepAngle;
      if (Double.IsNaN(ESweepAngle) || Double.IsInfinity(ESweepAngle) || ESweepAngle > 360 || ESweepAngle < 1) {
        ESweepAngle = 360;
      }

      ESpacing = this.Spacing;
      if (Double.IsInfinity(ESpacing)) {
        ESpacing = Double.NaN;
      }

      if (EArrangement == CircularArrangement.Packed &&
          this.ENodeDiameterFormula == CircularNodeDiameterFormula.Circular) {
        // Packing circular nodes is equivalent to what CircularArrangement.ConstantSpacing does
        EArrangement = CircularArrangement.ConstantSpacing;
      } else if (EArrangement == CircularArrangement.Packed && this.ENodeDiameterFormula != CircularNodeDiameterFormula.Circular) {
        this.ENodeDiameterFormula = EArrangement == CircularArrangement.Packed ? CircularNodeDiameterFormula.Circular : this.NodeDiameterFormula;
        EArrangement = this.Arrangement;
      }

      if ((this.Direction == CircularDirection.BidirectionalLeft || this.Direction == CircularDirection.BidirectionalRight)
          && this.Sorting != CircularSorting.Optimized) {
        // bidirectional
        for (int i = 0; ; i += 2) {
          if (i >= vertices.Count) break;
          evens.Add(vertices.ElementAt(i));
          if (i + 1 >= vertices.Count) break;
          odds.Add(vertices.ElementAt(i + 1));
        }
        if (this.Direction == CircularDirection.BidirectionalLeft) {
          // reverse evens, append odds
          CircularVertex[] reversed = evens.ToArray();
          Array.Reverse(reversed);
          vertices = new List<CircularVertex>(reversed);
          vertices.AddRange(odds);
        } else {
          // reverse odds, append evens
          CircularVertex[] reversed = odds.ToArray();
          Array.Reverse(reversed);
          vertices = new List<CircularVertex>(reversed);
          vertices.AddRange(evens);
        }
      }

      int num = vertices.Count();
      totalelementsizes = 0;
      int n = 0;
      for (int i = 0; i < vertices.Count; i++) {
        double angle = EStartAngle + (ESweepAngle * n * (this.Direction == CircularDirection.Clockwise ? 1 : -1)) / num;
        double add = vertices.ElementAt(i).ComputeDiameter(angle);
        if (ESweepAngle < 360 && (i == 0 || i == vertices.Count - 1)) { add /= 2; }
        totalelementsizes += add;
        n++;
      }

      if (Double.IsNaN(ERadius) || EArrangement == CircularArrangement.Packed) {  // radius not set
        // spacing not set
        if (Double.IsNaN(ESpacing)) ESpacing = 6;

        // set radius based on spacing
        if (EArrangement != CircularArrangement.ConstantSpacing && EArrangement != CircularArrangement.Packed) {
          double maxdist = Double.MinValue;
          for (int i = 0; i < num; i++) {
            CircularVertex curr = vertices.ElementAt(i);
            CircularVertex next = vertices.ElementAt(i == num - 1 ? 0 : i + 1);
            maxdist = Math.Max(maxdist, (curr.Diameter + next.Diameter) / 2);
          }
          constdist = (maxdist + ESpacing);
          if (EArrangement == CircularArrangement.ConstantAngle) {
            double angle = 2 * Math.PI / num;
            double minrad = (maxdist + ESpacing) / angle;
            if (EAspectRatio > 1) {
              ERadius = minrad;
              Yradius = ERadius * EAspectRatio;
            } else {
              Yradius = minrad;
              ERadius = Yradius / EAspectRatio;
            }
          } else {
            ERadius = InverseEllipsePerim((constdist * (ESweepAngle >= 360 ? num : num - 1)), EAspectRatio, EStartAngle * Math.PI / 180, ESweepAngle * Math.PI / 180);
          }
        } else {
          // Constant spacing
          ERadius = InverseEllipsePerim((totalelementsizes + (ESweepAngle >= 360 ? num : num - 1) * (EArrangement != CircularArrangement.Packed ? ESpacing : ESpacing * 1.6)), EAspectRatio, EStartAngle * Math.PI / 180, ESweepAngle * Math.PI / 180);
        }
        Yradius = ERadius * EAspectRatio;
      } else {
        // Radius is a #
        Yradius = ERadius * EAspectRatio;
        double circ = EllipsePerim(ERadius, Yradius, EStartAngle * Math.PI / 180, ESweepAngle * Math.PI / 180);
        if (Double.IsNaN(ESpacing)) {
          // spacing ! a #
          if (EArrangement == CircularArrangement.ConstantSpacing || EArrangement == CircularArrangement.Packed) {
            ESpacing = (circ - totalelementsizes) / (ESweepAngle >= 360 ? num : num - 1);
          }
          // else ESpacing doesn't matter
        } else {  // spacing is set
          if (EArrangement == CircularArrangement.ConstantSpacing || EArrangement == CircularArrangement.Packed) {
            // spacing set, const spacing
            double currentspacing = (circ - totalelementsizes) / (ESweepAngle >= 360 ? num : num - 1);

            if (currentspacing < ESpacing) {
              ERadius = InverseEllipsePerim((totalelementsizes + ESpacing * (ESweepAngle >= 360 ? num : num - 1)), EAspectRatio, EStartAngle * Math.PI / 180, ESweepAngle * Math.PI / 180);
              Yradius = ERadius * EAspectRatio;
            } else ESpacing = currentspacing;
          } else {
            // constant spacing (perhaps)
            double maxdist = Double.MinValue;
            for (int i = 0; i < num; i++) {
              CircularVertex curr = vertices.ElementAt(i);
              CircularVertex next = vertices.ElementAt(i == num - 1 ? 0 : i + 1);
              maxdist = Math.Max(maxdist, (curr.Diameter + next.Diameter) / 2);
            }
            double minimumconstdist = (maxdist + ESpacing);
            double minimumneededradius = InverseEllipsePerim((minimumconstdist * (ESweepAngle >= 360 ? num : num - 1)), EAspectRatio, EStartAngle * Math.PI / 180, ESweepAngle * Math.PI / 180);
            if (minimumneededradius > ERadius) {
              ERadius = minimumneededradius;
              Yradius = ERadius * EAspectRatio;
              constdist = minimumconstdist;
            } else {
              constdist = circ / (ESweepAngle >= 360 ? num : num - 1); // may be unnecessary
            }
          }
        }
      }
    }

    /// <summary>
    /// Do a circular layout.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public override void DoLayout(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      // Create Network
      if (this.Network == null) {
        this.Network = MakeNetwork(nodes, links);
      }

      // If there's only one node, put it at the origin
      if (this.Network.Vertexes.Count() == 1) {
        CircularVertex cv = this.Network.Vertexes.ElementAt(0);
        cv.Center = new Point(0, 0);
        return;
      }

      List<CircularVertex> vertices;
      List<CircularVertex> evens; // for bidirectionals, nodes of even index are usually arranged separately from odd index nodes
      List<CircularVertex> odds;

      // this also sets all of the E... "Effective" canonicalized property values
      SetEffectiveValues(out vertices, out evens, out odds);

      if ((this.Direction == CircularDirection.BidirectionalLeft ||
           this.Direction == CircularDirection.BidirectionalRight) &&
          EArrangement == CircularArrangement.Packed) {
        // if Arrangement == Packed and Direction is bidirectional, this is achieved by using a rearrangement of the vertices, and shifting the StartAngle
        PackedLayout(vertices, ESweepAngle, EStartAngle - ESweepAngle / 2, CircularDirection.Clockwise);
      } else if (this.Direction == CircularDirection.BidirectionalLeft || this.Direction == CircularDirection.BidirectionalRight) {
        double dsa = 0; // change in start angle
        // bidirectionals are achieved by laying out the even nodes in one direction, then laying out the odds in the opposite direction.
        // StartAngles must be slightly different.  The switch below computes the difference
        switch (this.EArrangement) {
          case CircularArrangement.ConstantDistance:
            dsa = EllipseAngle(ERadius, Yradius, EStartAngle, constdist) * 180 / Math.PI;
            break;
          case CircularArrangement.ConstantSpacing: {
              double evendia = 0;
              double odddia = 0;
              CircularVertex firsteven = evens.FirstOrDefault();
              if (firsteven != null) evendia = firsteven.ComputeDiameter(Math.PI / 2);
              CircularVertex firstodd = odds.FirstOrDefault();
              if (firstodd != null) odddia = firstodd.ComputeDiameter(Math.PI / 2);
              dsa = EllipseAngle(ERadius, Yradius, EStartAngle, ESpacing + (evendia + odddia) / 2) * 180 / Math.PI;
              break;
            }
          case CircularArrangement.ConstantAngle:
            dsa = ESweepAngle / vertices.Count;
            break;
        }
        // layout evens, then odds
        if (this.Direction == CircularDirection.BidirectionalLeft) {
          switch (this.EArrangement) {
            case CircularArrangement.ConstantDistance: DistanceLayout(evens, ESweepAngle / 2, EStartAngle, CircularDirection.Counterclockwise); break;
            case CircularArrangement.ConstantSpacing: SpacingLayout(evens, ESweepAngle / 2, EStartAngle, CircularDirection.Counterclockwise); break;
            case CircularArrangement.ConstantAngle: AngleLayout(evens, ESweepAngle / 2, EStartAngle, CircularDirection.Counterclockwise); break;
          }
          switch (this.EArrangement) {
            case CircularArrangement.ConstantDistance: DistanceLayout(odds, ESweepAngle / 2, EStartAngle + dsa, CircularDirection.Clockwise); break;
            case CircularArrangement.ConstantSpacing: SpacingLayout(odds, ESweepAngle / 2, EStartAngle + dsa, CircularDirection.Clockwise); break;
            case CircularArrangement.ConstantAngle: AngleLayout(odds, ESweepAngle / 2, EStartAngle + dsa, CircularDirection.Clockwise); break;
          }
        } else {
          switch (this.EArrangement) {
            case CircularArrangement.ConstantDistance: DistanceLayout(odds, ESweepAngle / 2, EStartAngle, CircularDirection.Counterclockwise); break;
            case CircularArrangement.ConstantSpacing: SpacingLayout(odds, ESweepAngle / 2, EStartAngle, CircularDirection.Counterclockwise); break;
            case CircularArrangement.ConstantAngle: AngleLayout(odds, ESweepAngle / 2, EStartAngle, CircularDirection.Counterclockwise); break;
          }
          switch (this.EArrangement) {
            case CircularArrangement.ConstantDistance: DistanceLayout(evens, ESweepAngle / 2, EStartAngle + dsa, CircularDirection.Clockwise); break;
            case CircularArrangement.ConstantSpacing: SpacingLayout(evens, ESweepAngle / 2, EStartAngle + dsa, CircularDirection.Clockwise); break;
            case CircularArrangement.ConstantAngle: AngleLayout(evens, ESweepAngle / 2, EStartAngle + dsa, CircularDirection.Clockwise); break;
          }
        }
      } else {
        // if Direction isn't bidirectional, the nodes are laid out by calling one of these functions.
        switch (this.EArrangement) {
          case CircularArrangement.ConstantDistance: DistanceLayout(vertices, ESweepAngle, EStartAngle, Direction); break;
          case CircularArrangement.ConstantSpacing: SpacingLayout(vertices, ESweepAngle, EStartAngle, Direction); break;
          case CircularArrangement.ConstantAngle: AngleLayout(vertices, ESweepAngle, EStartAngle, Direction); break;
          case CircularArrangement.Packed: PackedLayout(vertices, ESweepAngle, EStartAngle, Direction); break;
        }
      }

      // Update the "physical" positions of the nodes and links.
      if (this.Diagram != null && !this.Diagram.CheckAccess()) {
        Diagram.InvokeLater(this.Diagram, UpdateParts);
      } else {
        UpdateParts();
      }

      this.Network = null;
    }

    /// <summary>
    /// Arranges the items so the angle between any item and
    /// an adjacent item is the same
    /// </summary>
    /// <param name="vertices">The items to arrange</param>
    /// <param name="sweep">The range of the angles of the nodes (in radians)</param>
    /// <param name="start">The angle of the first node</param>
    /// <param name="dir">
    /// Specifies whether the nodes are arranged clockwise or counterclockwise.  Other
    /// values will be assumed to indicate clockwise (this function doesn't do bidirectionals - those
    /// are achieved by calling this function once for even nodes and again in the other direction for
    /// odd nodes.
    /// </param>
    private void AngleLayout(List<CircularVertex> vertices, double sweep, double start, CircularDirection dir) {
      double theta0 = start * Math.PI / 180;
      double _SweepingAngle = sweep * Math.PI / 180;
      //double e = ERadius > Yradius ? Math.Sqrt(ERadius * ERadius - Yradius * Yradius) / ERadius : Math.Sqrt(Yradius * Yradius - ERadius * ERadius) / Yradius;
      int num = vertices.Count();
      for (int i = 0; i < num; i++) {
        double theta = theta0 + (dir == CircularDirection.Clockwise ? (i * _SweepingAngle) / (ESweepAngle >= 360 ? num : num - 1) : -(i * _SweepingAngle) / num);
        CircularVertex cv = vertices.ElementAt(i);
        double w = ERadius * Math.Tan(theta) / Yradius;
        double r = Math.Sqrt((ERadius * ERadius + Yradius * Yradius * w * w) / (1 + w * w));
        cv.Center = new Point(r * Math.Cos(theta), r * Math.Sin(theta));
        cv.ActualAngle = theta * 180 / Math.PI;
      }
    }

    /// <summary>
    /// Arranges the items so the spacing between any item and
    /// an adjacent item is the same
    /// </summary>
    /// <param name="vertices">The items to arrange</param>
    /// <param name="sweep">The range of the angles of the nodes (in radians)</param>
    /// <param name="start">The angle of the first node</param>
    /// <param name="dir">
    /// Specifies whether the nodes are arranged clockwise or counterclockwise.  Other
    /// values will be assumed to indicate clockwise (this function doesn't do bidirectionals - those
    /// are achieved by calling this function once for even nodes and again in the other direction for
    /// odd nodes.
    /// </param>
    private void SpacingLayout(List<CircularVertex> vertices, double sweep, double start, CircularDirection dir) {
      double theta = start * Math.PI / 180;
      int num = vertices.Count();
      for (int i = 0; i < num; i++) {
        CircularVertex curr = vertices.ElementAt(i);
        CircularVertex next = vertices.ElementAt(i == num - 1 ? 0 : i + 1);
        double x = ERadius * Math.Cos(theta);
        double y = Yradius * Math.Sin(theta);
        curr.Center = new Point(x, y);
        curr.ActualAngle = theta * 180 / Math.PI;
        double rad = Math.Sqrt(x * x + y * y);
        double meandiam = (curr.Diameter + next.Diameter) / 2;
        double thetachange = EllipseAngle(
          ERadius,
          Yradius,
          dir == CircularDirection.Clockwise ? theta : -theta,
          meandiam + ESpacing
        );
        theta += dir == CircularDirection.Clockwise ? thetachange : -thetachange;
      }
    }

    /// <summary>
    /// Arranges the items so the distance between any item and
    /// an adjacent item is the same
    /// </summary>
    /// <param name="vertices">The items to arrange</param>
    /// <param name="sweep">The range of the angles of the nodes (in radians)</param>
    /// <param name="start">The angle of the first node</param>
    /// <param name="dir">
    /// Specifies whether the nodes are arranged clockwise or counterclockwise.  Other
    /// values will be assumed to indicate clockwise (this function doesn't do bidirectionals - those
    /// are achieved by calling this function once for even nodes and again in the other direction for
    /// odd nodes.
    /// </param>
    private void DistanceLayout(List<CircularVertex> vertices, double sweep, double start, CircularDirection dir) {
      double theta = start * Math.PI / 180;
      int num = vertices.Count();
      for (int i = 0; i < num; i++) {
        CircularVertex cv = vertices.ElementAt(i);
        cv.Center = new Point(ERadius * Math.Cos(theta), Yradius * Math.Sin(theta));
        cv.ActualAngle = theta * 180 / Math.PI;
        double thetachange = EllipseAngle(
          ERadius,
          Yradius,
          dir == CircularDirection.Clockwise ? theta : -theta,
          constdist
        );
        theta += dir == CircularDirection.Clockwise ? thetachange : -thetachange;
      }
    }

    /// <summary>
    /// Arranges the items so the spacing between any item and
    /// an adjacent item is the same, but takes into account the assumption
    /// that the nodes are rectangular.
    /// </summary>
    /// <param name="vertices">The items to arrange</param>
    /// <param name="sweep">The range of the angles of the nodes (in radians)</param>
    /// <param name="start">The angle of the first node</param>
    /// <param name="dir">
    /// Specifies whether the nodes are arranged clockwise or counterclockwise.  Other
    /// values will be assumed to indicate clockwise (this function doesn't do bidirectionals - those
    /// are achieved by calling this function once for even nodes and again in the other direction for
    /// odd nodes.
    /// </param>
    private void PackedLayout(List<CircularVertex> vertices, double sweep, double start, CircularDirection dir) {
      packediters = 0;
      vertexarrangement = new VertexArrangement();

      // give rand a constant seed value each time so the same result is produced each time
      rand = new Random(0);

      if (sweep < 360) {
        correctlastangle = start + (dir == CircularDirection.Clockwise ? ESweepAngle : -ESweepAngle);
        while (correctlastangle < 0) { correctlastangle += 360; }
        correctlastangle %= 360;
        if (correctlastangle > 180) { correctlastangle -= 360; }
        correctlastangle *= Math.PI / 180;
        PackedLayoutSemi(vertices, sweep, start, dir);
      } else {
        PackedLayoutFull(vertices, sweep, start, dir);
      }
      vertexarrangement.commit(vertices);
      return;
    }

    /// <summary>
    /// Keeps track of the best set of positions for the vertices (i.e., with the minimum overlap
    /// </summary>
    private VertexArrangement vertexarrangement = new VertexArrangement();
    /// <summary>
    /// Keeps track of the number of iterations of the Packed layout
    /// </summary>
    private int packediters = 0;
    /// <summary>
    /// Arranges the items so the spacing between any item and
    /// an adjacent item is the same, but takes into account the assumption
    /// that the nodes are rectangular.  This one assumes 360 degree sweep
    /// </summary>
    /// <param name="vertices">The items to arrange</param>
    /// <param name="sweep">The range of the angles of the nodes (in radians)</param>
    /// <param name="start">The angle of the first node</param>
    /// <param name="dir">
    /// Specifies whether the nodes are arranged clockwise or counterclockwise.  Other
    /// values will be assumed to indicate clockwise (this function doesn't do bidirectionals - those
    /// are achieved by calling this function once for even nodes and again in the other direction for
    /// odd nodes.
    /// </param>
    private void PackedLayoutFull(List<CircularVertex> vertices, double sweep, double start, CircularDirection dir) {
      // do layout
      double x = ERadius * Math.Cos(start * Math.PI / 180);
      double y = Yradius * Math.Sin(start * Math.PI / 180);
      CircularVertex[] verts = vertices.ToArray();
      for (int v = 0; v < verts.Length; v++) {
        verts[v].Center = new Point(x, y);

        if (v == verts.Length - 1) { break; } // avoid index out of bounds
        double newx;
        double newy; // ***** fix problems
        if (!nexth(x, y, verts, v, out newx, out newy, dir)) {
          nextv(x, y, verts, v, out newx, out newy, dir);
        }
        x = newx;
        y = newy;
      }

      // check for gap, find gap sizes
      packediters++;
      if (packediters > 128) {
        return;
      }
      double locfx = verts[0].Center.X;
      double locfy = verts[0].Center.Y;
      double loclx = verts[verts.Length - 1].Center.X;
      double locly = verts[verts.Length - 1].Center.Y;
      double gapx = Math.Abs(locfx - loclx) - ((verts[0].Width + verts[verts.Length - 1].Width) / 2 + ESpacing);
      double gapy = Math.Abs(locfy - locly) - ((verts[0].Height + verts[verts.Length - 1].Height) / 2 + ESpacing);

      // compute gap
      double gap = 0;
      if (Math.Abs(gapy) < 2.5) {
        double hgap = Math.Abs(locfx - loclx);
        double max = (verts[0].Width + verts[verts.Length - 1].Width) / 2;
        if (hgap < max) {
          gap = 0;
        }
        gapx = hgap - max;
      } else {
        // gapy not 0
        if (gapy > 0) { gap = gapy; } else {
          if (Math.Abs(gapx) < 2.5) {
            gap = 0;
          } else gap = gapx;
        }
      }

      // check for excessive overlap
      bool overlap = false;
      if (Math.Abs(loclx) > Math.Abs(locly)) {
        // use x
        overlap = loclx > 0 ^ locfy > locly;
      } else {
        // use y
        overlap = locly > 0 ^ locfx < loclx;
      }
      overlap = dir == CircularDirection.Clockwise ? overlap : !overlap;

      // if there's an excessive overlap, gap must be <0
      if (overlap) {
        gap = -Math.Abs(gap);

        gap = Math.Min(gap, -verts[verts.Length - 1].Width);
        gap = Math.Min(gap, -verts[verts.Length - 1].Height);
      }

      // record this
      vertexarrangement.compare(gap, verts);

      // avoid period 2 stuff

      if (packediters > 36 && packediters % 13 == 0) {
        ERadius *= (packediters % 2 == 0 ? .75 : 1.1) + rand.NextDouble() * .15;
        Yradius = ERadius * EAspectRatio;
      }

      // change radii accordingly
      if (Math.Abs(gap) > 2.5) {
        this.ERadius -= gap / (2 * Math.PI);
        this.Yradius = ERadius * EAspectRatio;
        PackedLayoutFull(vertices, sweep, start, dir);
      }
    }

    private double correctlastangle = 0; // when the sweep angle < 360, this is the target angle of the last node
    /// <summary>
    /// Arranges the items so the spacing between any item and
    /// an adjacent item is the same, but takes into account the assumption
    /// that the nodes are rectangular.  This one assumes partial sweep, i.e., <see cref="SweepAngle" /> is
    /// less than 360.
    /// </summary>
    /// <param name="vertices">The items to arrange</param>
    /// <param name="sweep">The range of the angles of the nodes (in radians)</param>
    /// <param name="start">The angle of the first node</param>
    /// <param name="dir">
    /// Specifies whether the nodes are arranged clockwise or counterclockwise.  Other
    /// values will be assumed to indicate clockwise (this function doesn't do bidirectionals - those
    /// are achieved by calling this function once for even nodes and again in the other direction for
    /// odd nodes.
    /// </param>
    private void PackedLayoutSemi(List<CircularVertex> vertices, double sweep, double start, CircularDirection dir) {
      // do layout
      double x = ERadius * Math.Cos(start * Math.PI / 180);
      double y = Yradius * Math.Sin(start * Math.PI / 180);
      CircularVertex[] verts = vertices.ToArray();
      for (int v = 0; v < verts.Length; v++) {
        verts[v].Center = new Point(x, y);

        if (v == verts.Length - 1) { break; } // avoid index out of bounds
        double newx;
        double newy;
        if (!nexth(x, y, verts, v, out newx, out newy, dir)) {
          nextv(x, y, verts, v, out newx, out newy, dir);
        }
        x = newx;
        y = newy;
      }

      // check for gap, find gap sizes
      packediters++;
      if (packediters > 128) {
        return;
      }

      // compute gap
      CircularVertex last = vertices.ElementAt(vertices.Count - 1);
      double lastangle = Math.Atan2(y, x);
      double diff = dir == CircularDirection.Clockwise ? correctlastangle - lastangle : lastangle - correctlastangle;
      diff = Math.Abs(diff) < Math.Abs(diff - 2 * Math.PI) ? diff : diff - 2 * Math.PI;
      double gap = diff * (ERadius + Yradius) / 2;
      //return;
      // record this
      vertexarrangement.compare2(gap, verts);

      // avoid period 2 stuff

      if (packediters > 36 && packediters % 13 == 0) {
        ERadius *= (packediters % 2 == 0 ? .75 : 1.1) + rand.NextDouble() * .15;
        Yradius = ERadius * EAspectRatio;
      }

      // change radii accordingly
      if (Math.Abs(gap) > 2.5) {
        this.ERadius -= gap / (2 * Math.PI);
        this.Yradius = ERadius * EAspectRatio;
        PackedLayoutSemi(vertices, sweep, start, dir);
      }
    }


    private Random rand; // Packed layout sometimes alternates between 2 radii.  If this happens, a random value is added to stop the cycle.

    /// <summary>
    /// Finds the coords. of the next item in the <see cref="PackedLayout"/> if it's
    /// added horizontally.
    /// </summary>
    /// <param name="x">X coord. of previous node</param>
    /// <param name="y">Y coord. of previous node</param>
    /// <param name="verts">The list of all nodes</param>
    /// <param name="v">The index of the previous node in verts</param>
    /// <param name="newx">Returns the x coord. of the new node</param>
    /// <param name="newy">Returns the y coord. of the new node</param>
    /// <param name="dir">Whether the nodes are arranged Clockwise or Counterclockwise</param>
    /// <returns>true if a node can be added horizontally and still be on the ellipse</returns>
    private bool nexth(double x, double y, CircularVertex[] verts, int v, out double newx, out double newy, CircularDirection dir) {
      newx = 0;
      newy = 0;
      bool inv = false; //true if it changes sides
      if (y >= 0 ^ dir == CircularDirection.Clockwise) {
        // x increases
        newx = x + ((verts[v].Width + verts[v + 1].Width) / 2 + ESpacing);
        if (newx > ERadius) {
          // x decreases
          newx = x - ((verts[v].Width + verts[v + 1].Width) / 2 + ESpacing);
          if (newx < -ERadius) { return false; }
          inv = true;
        }
      } else {

        // x decreases
        newx = x - ((verts[v].Width + verts[v + 1].Width) / 2 + ESpacing);
        if (newx < -ERadius) {
          newx = x + ((verts[v].Width + verts[v + 1].Width) / 2 + ESpacing);
          if (newx > ERadius) { return false; }
          inv = true;
        }
      }

      newy = Math.Sqrt(1 - newx * newx / (ERadius * ERadius)) * Yradius;
      if (y < 0 ^ inv) { newy = -newy; }
      if (Math.Abs(y - newy) > (verts[v].Height + verts[v + 1].Height) / 2) { return false; } else return true;
    }

    /// <summary>
    /// Finds the coords. of the next item in the <see cref="PackedLayout"/> if it's
    /// added vertically.
    /// </summary>
    /// <param name="x">X coord. of previous node</param>
    /// <param name="y">Y coord. of previous node</param>
    /// <param name="verts">The list of all nodes</param>
    /// <param name="v">The index of the previous node in verts</param>
    /// <param name="newx">Returns the x coord. of the new node</param>
    /// <param name="newy">Returns the y coord. of the new node</param>
    /// <param name="dir">Whether the nodes are arranged Clockwise or Counterclockwise</param>
    /// <returns>true if a node can be added vertically and still be on the ellipse</returns>
    private bool nextv(double x, double y, CircularVertex[] verts, int v, out double newx, out double newy, CircularDirection dir) {
      newx = 0;
      newy = 0;
      bool inv = false; //true if it changes sides
      if (x >= 0 ^ dir == CircularDirection.Clockwise) {
        // y decreases
        newy = y - ((verts[v].Height + verts[v + 1].Height) / 2 + ESpacing);
        if (newy < -Yradius) {
          // y increases
          newy = y + ((verts[v].Height + verts[v + 1].Height) / 2 + ESpacing);
          if (newy > Yradius) { return false; }
          inv = true;
        }
      } else {
        // y increases
        newy = y + ((verts[v].Height + verts[v + 1].Height) / 2 + ESpacing);
        if (newy > Yradius) {
          // y decreases
          newy = y - ((verts[v].Height + verts[v + 1].Height) / 2 + ESpacing);
          if (newy < -Yradius) { return false; }
          inv = true;
        }
      }

      newx = Math.Sqrt(1 - newy * newy / (Yradius * Yradius)) * ERadius;
      if (x < 0 ^ inv) { newx = -newx; }
      if (Math.Abs(x - newx) > (verts[v].Width + verts[v + 1].Width) / 2) { return false; } else return true;
    }

    /// <summary>
    /// Represents positions for the vertices and the size of the gap they cause, so the Packed layout can keep
    /// track of which positions worked best
    /// </summary>
    sealed internal class VertexArrangement {
      public double gap = Double.MinValue;
      public double[] xcoords;
      public double[] ycoords;

      /// <summary>
      /// Compares a new gap with the current one.  If it's better, it updates <see cref="gap"/>, <see cref="xcoords"/>,and <see cref="ycoords"/>.
      /// This is for a partial sweep, where overshooting (gap less than 0) is worse than undershooting (gap > 0) because it will cause nodes
      /// to overlap
      /// </summary>
      /// <param name="gap"></param>
      /// <param name="verts"></param>
      public void compare(double gap, CircularVertex[] verts) {
        if ((gap > 0 && this.gap < 0) || (Math.Abs(gap) < Math.Abs(this.gap) && !(gap < 0 && this.gap > 0))) {
          this.gap = gap;
          xcoords = new double[verts.Length];
          ycoords = new double[verts.Length];
          for (int i = 0; i < verts.Length; i++) {
            xcoords[i] = verts[i].Position.X;
            ycoords[i] = verts[i].Position.Y;
          }
        }
      }

      /// <summary>
      /// Compares a new gap with the current one.  If it's better, it updates <see cref="gap"/>, <see cref="xcoords"/>,and <see cref="ycoords"/>.
      /// This is for a partial sweep, where overshooting (gap less than 0) is no worse than undershooting (gap > 0).
      /// </summary>
      /// <param name="gap"></param>
      /// <param name="verts"></param>
      public void compare2(double gap, CircularVertex[] verts) {
        if (Math.Abs(gap) < Math.Abs(this.gap)) {
          this.gap = gap;
          xcoords = new double[verts.Length];
          ycoords = new double[verts.Length];
          for (int i = 0; i < verts.Length; i++) {
            xcoords[i] = verts[i].Position.X;
            ycoords[i] = verts[i].Position.Y;
          }
        }
      }

      /// <summary>
      /// Sets the vertices in verts to the correct positions
      /// </summary>
      /// <param name="verts">The vertices who's positions are set</param>
      public void commit(List<CircularVertex> verts) {
        if (xcoords == null || ycoords == null) { return; }
        for (int i = 0; i < xcoords.Length; i++) {
          verts.ElementAt(i).Position = new Point(xcoords[i], ycoords[i]);
        }
      }
    }


    private const int riemannnum = 600; // # of divisions in riemann sum

    /// <summary>
    /// Finds the perimeter of an ellipse with radii a and b
    /// </summary>
    /// <param name="a">One radius</param>
    /// <param name="b">The other radius</param>
    /// <returns>The circumference</returns>
    private double EllipsePerim(double a, double b) {
      if (Math.Abs(EAspectRatio - 1) < .001) { return 2 * Math.PI * a; } // circular case

      double e = a > b ? Math.Sqrt(a * a - b * b) / a : Math.Sqrt(b * b - a * a) / b; // eccentricity
      double integral = 0;
      double wid = Math.PI / (2 * (riemannnum + 1));
      for (int i = 0; i <= riemannnum; i++) {
        double theta = i * Math.PI / (2 * riemannnum);
        double sin = Math.Sin(theta);
        integral += Math.Sqrt(1 - e * e * sin * sin) * wid;
      }
      return 4 * (a > b ? a : b) * integral;
    }
    /// <summary>
    /// Finds the perimeter of an ellipse with radii a and b
    /// </summary>
    /// <param name="a">One radius</param>
    /// <param name="b">The other radius</param>
    /// <param name="theta0">The angle at which the arc begins</param>
    /// <param name="sweep">The sweep angle of the  arc.</param>
    /// <returns>The circumference</returns>
    private double EllipsePerim(double a, double b, double theta0, double sweep) {
      if (Math.Abs(EAspectRatio - 1) < .001) { return sweep * a; } // circular case

      double e = a > b ? Math.Sqrt(a * a - b * b) / a : Math.Sqrt(b * b - a * a) / b; // eccentricity
      double integral = 0;
      double wid = sweep / (riemannnum + 1);
      for (int i = 0; i <= riemannnum; i++) {
        double theta = theta0 + i * sweep / riemannnum;
        double sin = Math.Sin(theta);
        integral += Math.Sqrt(1 - e * e * sin * sin) * wid;
      }
      return (a > b ? a : b) * integral;
    }

    /// <summary>
    /// Returns X radius of ellipse with specified circumference and Aspect ratio
    /// </summary>
    /// <param name="circ">The perimeter of ellipse</param>
    /// <param name="asprat">The ellipse's aspect ratio</param>
    /// <returns>The X radius</returns>
    private double InverseEllipsePerim(double circ, double asprat) {
      double perim1 = EllipsePerim(1, asprat);
      return circ / perim1;
    }
    /// <summary>
    /// Returns X radius of ellipse with specified circumference and Aspect ratio
    /// </summary>
    /// <param name="circ">The perimeter of ellipse</param>
    /// <param name="asprat">The ellipse's aspect ratio</param>
    /// <param name="theta0">The angle at which the arc begins</param>
    /// <param name="sweep">The sweep angle of the  arc.</param>
    /// <returns></returns>
    private double InverseEllipsePerim(double circ, double asprat, double theta0, double sweep) {
      double perim1 = EllipsePerim(1, asprat, theta0, sweep);
      return circ / perim1;
    }
    /// <summary>
    /// Returns the angle of an elliptical arc of specified length
    /// </summary>
    /// <param name="a">The X radius of the ellipse</param>
    /// <param name="b">The Y radius</param>
    /// <param name="theta0">The angle at which the arc begins</param>
    /// <param name="length">The length of the arc</param>
    /// <returns>The angle of the arc in radians.</returns>
    private double EllipseAngle(double a, double b, double theta0, double length) {
      if (Math.Abs(EAspectRatio - 1) < .001) { return length / a; } // just added *****

      double e = a > b ? Math.Sqrt(a * a - b * b) / a : Math.Sqrt(b * b - a * a) / b; // eccentricity
      double integral = 0;
      double wid = (2 * Math.PI) / (this.Network.VertexCount * 700);
      if (a > b) { theta0 += Math.PI / 2; }
      for (int i = 0; ; i++) {
        double theta = theta0 + i * wid;
        double sin = Math.Sin(theta);
        integral += (a > b ? a : b) * Math.Sqrt(1 - e * e * sin * sin) * wid;
        if (integral >= length) {
          return i * wid;
        }
      }
    }

    /// <summary>
    /// Sorts the vertexes based on whatever <see cref="Sorting"/> specifies.
    /// </summary>
    /// <param name="vertexes">The vertexes to sort</param>
    private List<CircularVertex> Sort(CircularVertex[] vertexes) {
      switch (Sorting) {
        case CircularSorting.Forwards: break;
        case CircularSorting.Reverse: Array.Reverse(vertexes, 0, vertexes.Length); break;
        case CircularSorting.Ascending: if (Comparer != null) { Array.Sort(vertexes, Comparer); } break;
        case CircularSorting.Descending: if (Comparer != null) { Array.Sort(vertexes, Comparer); }
          Array.Reverse(vertexes, 0, vertexes.Length); break;
        case CircularSorting.Optimized: return Optimize(ConnectivityArrange(vertexes));
        //case CircularSorting.GroupedOptimized: return OptimizeGroup(ConnectivityArrange(new List<CircularVertex>(nodes)));
        default: throw new NotImplementedException("This CircularSorting is not yet implemented");
      }
      return new List<CircularVertex>(vertexes);
    }

    /// <summary>
    /// Sorts the nodes based on connectivity.  The oth node is the one with the highest
    /// connectivity, and each subsequent node it the one with the greatest number of links with nodes
    /// already in the list
    /// </summary>
    /// <param name="l">The nodes to sort</param>
    /// <returns>The sorted list</returns>
    /// <remarks>This is used for cross reduction before <see cref="Optimize"/> is called because
    /// it makes the cross reduction much more effective.
    /// </remarks>
    private List<CircularVertex> ConnectivityArrange(CircularVertex[] l) {
      int[] conns = new int[l.Length];
      List<CircularVertex> res = new List<CircularVertex>();
      for (int i = 0; i < l.Length; i++) {
        int maxconn = -1;
        int maxind = -1;
        if (i == 0) {
          for (int j = 0; j < l.Length; j++) {
            CircularVertex vtx = l[j];
            int nconn = vtx.SourceEdgesCount + vtx.DestinationEdgesCount;
            if (nconn > maxconn) {
              maxconn = nconn;
              maxind = j;
            }
          }
        } else for (int j = 0; j < l.Length; j++) {
            int nconn = conns[j];
            if (nconn > maxconn) {
              maxconn = nconn;
              maxind = j;
            }
          }
        res.Add(l[maxind]);
        conns[maxind] = -1;

        CircularVertex v = l[maxind];
        foreach (CircularEdge li in v.SourceEdgesList) {
          CircularVertex f = li.FromVertex;
          int ind = Array.IndexOf(l, f);
          if (ind < 0) { continue; }
          if (conns[ind] >= 0) { conns[ind]++; }
        }
        foreach (CircularEdge li in v.DestinationEdgesList) {
          CircularVertex f = li.ToVertex;
          int ind = Array.IndexOf(l, f);
          if (ind < 0) { continue; }
          if (conns[ind] >= 0) { conns[ind]++; }
        }
      }
      return res;
    }

    /// <summary>
    /// Sorts the nodes to avoid crossing links
    /// </summary>
    /// <param name="v">The nodes to sort</param>
    /// <returns>The sorted nodes</returns>
    private List<CircularVertex> Optimize(List<CircularVertex> v) {
      List<int>[] linkswith = new List<int>[v.Count];
      for (int i = 0; i < linkswith.Length; i++) {
        CircularVertex n = v.ElementAt(i);
        linkswith[i] = new List<int>();
        foreach (CircularEdge l in n.DestinationEdgesList) {
          int ind = v.IndexOf(l.ToVertex);
          if (ind != i && linkswith[i].IndexOf(ind) < 0) { linkswith[i].Add(ind); } // ignore autoconnected
        }
        foreach (CircularEdge l in n.SourceEdgesList) {
          int ind = v.IndexOf(l.FromVertex);
          if (ind != i && linkswith[i].IndexOf(ind) < 0) { linkswith[i].Add(ind); } // ignore autoconnected
        }
      }

      int[] indexes = new int[linkswith.Length];
      List<int> opt = new List<int>();
      List<int> links1 = new List<int>();
      List<int> links2 = new List<int>();
      List<int> monoconnected = new List<int>();
      List<CircularVertex> disconnected = new List<CircularVertex>();
      //int half = (v.Count + 1) / 2;
      for (int i = 0, j = 0; i < linkswith.Length; i++) {

        int conns = linkswith[i].Count;
        if (conns == 1) { monoconnected.Add(i); continue; }
        if (conns == 0) { disconnected.Add(v[i]); continue; } // just added

        if (j == 0) { opt.Add(i); j++; continue; }

        int mincrosses = int.MaxValue;
        int mindist = int.MaxValue;
        int minpos = -1;
        List<int> newposses = new List<int>();
        for (int p = 0; p < opt.Count; p++) {
          if (linkswith[opt[p]].IndexOf(opt[p == opt.Count - 1 ? 0 : p + 1]) < 0) {
            newposses.Add(p == opt.Count - 1 ? 0 : p + 1);
          }
        }
        if (newposses.Count == 0) {
          for (int p = 0; p < opt.Count; p++) {
            newposses.Add(p);
          }
        }
        foreach (int newpos in newposses) {
          int crosses = Crossings(linkswith[i], links1, links2, indexes, newpos, opt);

          int dist = 0;
          foreach (int conn in linkswith[i]) {
            int ind = opt.IndexOf(conn);
            if (ind >= 0) {
              int absdist = Math.Abs(newpos - (ind >= newpos ? ind + 1 : ind));
              dist += absdist < opt.Count + 1 - absdist ? absdist : opt.Count + 1 - absdist;
            }
          }
          for (int m = 0; m < links1.Count; m++) {
            int a = indexes[links1[m]];
            int b = indexes[links2[m]]; // a < b
            if (a >= newpos) { a++; }
            if (b >= newpos) { b++; }
            if (a > b) { int c = b; b = a; a = c; }
            if (!(b - a < (opt.Count + 2) / 2 ^ (a < newpos && newpos <= b))) {
              dist++;
            }
          }

          if (crosses < mincrosses || (crosses == mincrosses && dist < mindist)) {
            mincrosses = crosses;
            mindist = dist;
            minpos = newpos;
          }
        }
        opt.Insert(minpos, i);

        // update indexes
        for (int n = 0; n < opt.Count; n++) {
          indexes[opt[n]] = n;
        }

        // update links
        foreach (int link in linkswith[i]) {
          if (opt.IndexOf(link) >= 0) {
            links1.Add(i);
            links2.Add(link);
          }
        }
        j++;
      }

      // add monoconnected
      bool done = false;
      int cnt = opt.Count;
      while (true) {
        done = true;
        for (int n = 0; n < monoconnected.Count; n++) {
          int i = monoconnected[n];
          int tonode = linkswith[i][0];
          int ind = opt.IndexOf(tonode);
          if (ind >= 0) {
            //insert either before or after
            int low = 0; // links to node @ lower index
            foreach (int link in linkswith[tonode]) {
              int linkind = opt.IndexOf(link);
              if (linkind < 0 || linkind == ind) { continue; }
              int d1 = linkind > ind ? linkind - ind : ind - linkind;
              int d2 = cnt - d1;
              low += linkind < ind ^ d1 > d2 ? 1 : -1;
            }
            opt.Insert(low < 0 ? ind : ind + 1, i);
            monoconnected.RemoveAt(n);
            n--;
          } else done = false;
          /* previous used to be this:
          if (ind >= 0) { opt.Insert(ind, i); monoconnected.RemoveAt(n); n--; }
          else done = false;
          */
        }
        if (done) { break; } else {
          opt.Add(monoconnected[0]);
          monoconnected.RemoveAt(0);
        }
      }

      foreach (int ind in opt) { disconnected.Add(v[ind]); }
      return disconnected;
    }

    // these are used in GroupedOptimize

    /// <summary>
    /// Arranges subsets based on connectivity the same was as <see cref="ConnectivityArrange"/>.
    /// </summary>
    /// <param name="l"></param>
    /// <returns></returns>
    private List<Subset> ConnectivityArrangeSubsets(List<Subset> l) {
      int[] conns = new int[l.Count];
      List<Subset> res = new List<Subset>();
      for (int i = 0; i < l.Count; i++) {

        int maxconn = -1;
        int maxind = -1;
        if (i == 0) {
          for (int j = 0; j < l.Count; j++) {
            Subset n = l[j];
            int nconn = n.Conns.Count;
            if (nconn > maxconn) {
              maxconn = nconn;
              maxind = j;
            }
          }
        } else for (int j = 0; j < l.Count; j++) {

            int nconn = conns[j];
            if (nconn > maxconn) {
              maxconn = nconn;
              maxind = j;
            }
          }
        res.Add(l[maxind]);
        conns[maxind] = -1;

        foreach (CircularVertex li in l[maxind].Conns) {
          Subset f = l.First(s => s.List.Contains(li));
          int ind = l.IndexOf(f);
          if (ind < 0) { continue; }
          if (conns[ind] >= 0) { conns[ind]++; }
        }
      }
      return res;
    }

    /// <summary>
    /// Optimizes each <see cref="Subset"/> the way <see cref="Optimize"/> does, and optimizes the
    /// nodes within each Subset
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    private List<Subset> OptimizeSubsets(List<Subset> v) {
      List<int>[] linkswith = new List<int>[v.Count];
      for (int i = 0; i < linkswith.Length; i++) {
        Subset n = v.ElementAt(i);
        linkswith[i] = new List<int>();
        foreach (CircularVertex l in n.Conns) {
          var ss = v.First(s => s.List.IndexOf(l) >= 0);
          int ind = v.IndexOf(ss);
          if (ind != i && linkswith[i].IndexOf(ind) < 0) { linkswith[i].Add(ind); } // ignore autoconnected
        }
      }

      int[] indexes = new int[linkswith.Length];
      List<int> opt = new List<int>();
      List<int> links1 = new List<int>();
      List<int> links2 = new List<int>();
      List<int> monoconnected = new List<int>();
      List<Subset> disconnected = new List<Subset>();
      //int half = (v.Count + 1) / 2;
      for (int i = 0, j = 0; i < linkswith.Length; i++) {

        int conns = linkswith[i].Count;
        if (conns == 1) { monoconnected.Add(i); continue; }
        if (conns == 0) { disconnected.Add(v[i]); continue; } // just added

        if (j == 0) { opt.Add(i); j++; continue; }

        int mincrosses = int.MaxValue;
        int mindist = int.MaxValue;
        int minpos = -1;
        List<int> newposses = new List<int>();
        for (int p = 0; p < opt.Count; p++) {
          if (linkswith[opt[p]].IndexOf(opt[p == opt.Count - 1 ? 0 : p + 1]) < 0) {
            newposses.Add(p == opt.Count - 1 ? 0 : p + 1);
          }
        }
        if (newposses.Count == 0) {
          for (int p = 0; p < opt.Count; p++) {
            newposses.Add(p);
          }
        }
        foreach (int newpos in newposses) {
          int crosses = Crossings(linkswith[i], links1, links2, indexes, newpos, opt);

          int dist = 0;
          foreach (int conn in linkswith[i]) {
            int ind = opt.IndexOf(conn);
            if (ind >= 0) {
              int absdist = Math.Abs(newpos - (ind >= newpos ? ind + 1 : ind));
              dist += absdist < opt.Count + 1 - absdist ? absdist : opt.Count + 1 - absdist;
            }
          }
          for (int m = 0; m < links1.Count; m++) {
            int a = indexes[links1[m]];
            int b = indexes[links2[m]]; // a < b
            if (a >= newpos) { a++; }
            if (b >= newpos) { b++; }
            if (a > b) { int c = b; b = a; a = c; }
            if (!(b - a < (opt.Count + 2) / 2 ^ (a < newpos && newpos <= b))) {
              dist++;
            }
          }

          if (crosses < mincrosses || (crosses == mincrosses && dist < mindist)) {
            mincrosses = crosses;
            mindist = dist;
            minpos = newpos;
          }
        }
        opt.Insert(minpos, i);

        // update indexes
        for (int n = 0; n < opt.Count; n++) {
          indexes[opt[n]] = n;
        }

        // update links
        foreach (int link in linkswith[i]) {
          if (opt.IndexOf(link) >= 0) {
            links1.Add(i);
            links2.Add(link);
          }
        }
        j++;
      }

      // add monoconnected
      bool done = false;
      int cnt = opt.Count;
      while (true) {
        done = true;
        for (int n = 0; n < monoconnected.Count; n++) {
          int i = monoconnected[n];
          int toSubset = linkswith[i][0];
          int ind = opt.IndexOf(toSubset);
          if (ind >= 0) {
            //insert either before or after
            int low = 0; // links to Subset @ lower index
            foreach (int link in linkswith[toSubset]) {
              int linkind = opt.IndexOf(link);
              if (linkind < 0 || linkind == ind) { continue; }
              int d1 = linkind > ind ? linkind - ind : ind - linkind;
              int d2 = cnt - d1;
              low += linkind < ind ^ d1 > d2 ? 1 : -1;
            }
            opt.Insert(low < 0 ? ind : ind + 1, i);
            monoconnected.RemoveAt(n);
            n--;
          } else done = false;
          /* previous used to be this:
          if (ind >= 0) { opt.Insert(ind, i); monoconnected.RemoveAt(n); n--; }
          else done = false;
          */
        }
        if (done) { break; } else {
          opt.Add(monoconnected[0]);
          monoconnected.RemoveAt(0);
        }
      }

      foreach (int ind in opt) { disconnected.Add(v[ind]); }
      return disconnected;
    }

    /// <summary>
    /// Divides the nodes into subsets
    /// </summary>
    /// <param name="l"></param>
    /// <returns>A list of Subsets</returns>
    private List<Subset> Subsets(List<CircularVertex> l) {
      List<Subset> gs = new List<Subset>();
      int cnt;
      do {
        cnt = l.Count;
        for (int i = 0; i < l.Count; i++) { // might have to be a for *****
          CircularVertex n = l[i];
          var linkgroups = new List<Subset>();
          bool onlygroups = true;
          CircularVertex other = null;
          List<CircularVertex> links = new List<CircularVertex>();
          foreach (CircularEdge li in n.SourceEdgesList) {
            CircularVertex nd = li.FromVertex;
            if (!links.Contains(nd)) { links.Add(nd); } else continue;
            if (nd == n) { continue; }
            if (l.IndexOf(nd) >= 0) {
              if (other != null) {
                onlygroups = false; break;
              } else other = nd;
            }
          }
          foreach (CircularEdge li in n.DestinationEdgesList) {
            CircularVertex nd = li.ToVertex;
            if (!links.Contains(nd)) { links.Add(nd); } else continue;
            if (nd == n) { continue; }
            if (l.IndexOf(nd) >= 0) {
              if (other != null) {
                onlygroups = false; break;
              } else other = nd;
            }
          }
          if (!onlygroups) { continue; }
          foreach (CircularVertex nd in links) {
            if (nd == n) { continue; }
            try {
              linkgroups.Add(gs.First(g => g.Head == nd));
            } catch (Exception) { }
          }

          Subset s = linkgroups.Count == 0 ? new Subset(n) : new Subset(n, linkgroups);
          foreach (Subset g in linkgroups) {
            gs.Remove(g);
          }
          gs.Add(s);
          l.RemoveAt(i); // ***** was Remove(n);
          i--;
        }
      } while (l.Count != cnt);
      foreach (CircularVertex n in l) { // remaining should be put in groups *****
        gs.Add(new Subset(n));
      }

      return gs;
    }

    /// <summary>
    /// Represents a single subset of nodes that can be treated as a single node when optimizing
    /// </summary>
    sealed internal class Subset {
      public List<CircularVertex> List;
      public CircularVertex Head;
      public List<CircularVertex> Conns;
      public Subset(CircularVertex n) {
        List = new List<CircularVertex>(); Conns = new List<CircularVertex>();
        Head = n;
        List.Add(n);
        foreach (CircularEdge li in n.SourceEdgesList) {
          CircularVertex nd = li.FromVertex;
          if (List.IndexOf(nd) < 0) { Conns.Add(nd); }
        }
        foreach (CircularEdge li in n.DestinationEdgesList) {
          CircularVertex nd = li.ToVertex;
          if (List.IndexOf(nd) < 0) { Conns.Add(nd); }
        }
      }
      public Subset(CircularVertex n, List<Subset> gs) {
        List = new List<CircularVertex>(); Conns = new List<CircularVertex>();
        Head = n;
        List.Add(n);
        foreach (Subset g in gs) {
          foreach (CircularVertex nd in g.List) { this.List.Add(nd); }
        }
        foreach (CircularEdge li in n.SourceEdgesList) {
          CircularVertex nd = li.FromVertex;
          if (List.IndexOf(nd) < 0) { Conns.Add(nd); }
        }
        foreach (CircularEdge li in n.DestinationEdgesList) {
          CircularVertex nd = li.ToVertex;
          if (List.IndexOf(nd) < 0) { Conns.Add(nd); }
        }
      }
      public override string ToString() {
        string s = "";
        foreach (CircularVertex n in List) { s += n.ToString() + ", "; }
        return s;
      }
    }

    /// <summary>
    /// Optimizes nodes by dividing them into groups and optimizing each group.
    /// </summary>
    /// <param name="l"></param>
    /// <returns></returns>
    private List<CircularVertex> OptimizeGroup(List<CircularVertex> l) {

      var groups = Subsets(l); // divide the nodes into groups

      groups = OptimizeSubsets(groups); // ***** groups
      var res = new List<CircularVertex>();
      foreach (Subset s in groups) {
        var opt = Optimize(s.List);
        foreach (CircularVertex n in opt) { res.Add(n); }
      }
      return res;
    }

    /// <summary>
    /// Finds the # of link crossings added when a node is inserted at a specified index
    /// </summary>
    /// <param name="newlinks">All the nodes to which the new node links</param>
    /// <param name="links1">One endpoint of all the existing links</param>
    /// <param name="links2">Other endpoint of all existing links</param>
    /// <param name="indexes">The indexes of each element in opt</param>
    /// <param name="node">The new node is inserted before this index (includes 0)</param>
    /// <param name="opt">the list of all the nodes currently in optimized list</param>
    /// <returns># of crossings</returns>
    private int Crossings(List<int> newlinks, List<int> links1, List<int> links2, int[] indexes, int node, List<int> opt) {
      int count = 0;
      for (int i = 0; i < links1.Count; i++) {
        int l1 = links1.ElementAt(i);
        int l2 = links2.ElementAt(i);
        int l1pos = indexes[l1];
        int l2pos = indexes[l2];
        int c = 0, d = 0;
        if (l1pos < l2pos) { c = l1pos; d = l2pos; } else { c = l2pos; d = l1pos; }
        // c < d

        if (c < node && node <= d) {
          // node between c && d
          foreach (int link in newlinks) {
            if (opt.IndexOf(link) < 0) { continue; }
            if (!
              (
              (c < indexes[link] && indexes[link] < d)
              ||
              c == indexes[link] || d == indexes[link]
              )
              ) { count++; }
          }
        } else {
          //node not between c && d
          foreach (int link in newlinks) {
            if (opt.IndexOf(link) < 0) { continue; }
            if (!
              (
              !(c < indexes[link] && indexes[link] < d)
              ||
              c == indexes[link] || d == indexes[link]
              )
              ) { count++; }
          }
        }
      }

      return count;
    }

    private void UpdateParts() {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram != null) diagram.StartTransaction("CircularLayout");
      LayoutNodesAndLinks();
      if (diagram != null) diagram.CommitTransaction("CircularLayout");
      if (diagram != null && diagram.LayoutManager != null) {
        /* *****/
        CircularNetwork net = this.Network;
        diagram.LayoutManager.AddUpdateLinks(this, () => {
          this.Network = net;
          LayoutLinks();
          this.Network = null;
        });
      }
    }

    /// <summary>
    /// Call <see cref="GenericNetwork{N, L, Y}.Vertex.CommitPosition"/> to position each node,
    /// call LayoutComments, and then call
    /// <see cref="GenericNetwork{N, L, Y}.Edge.CommitPosition"/> to route the links.
    /// </summary>
    /// <remarks>
    /// This sets any port spots, as directed by SetPortSpots,
    /// and then calls <see cref="LayoutNodes"/> and <see cref="LayoutLinks"/>.
    /// </remarks>
    protected virtual void LayoutNodesAndLinks() {
      LayoutNodes();
      LayoutLinks();
    }

    /// <summary>
    /// Commit the position of all of the vertex nodes.
    /// </summary>
    protected virtual void LayoutNodes() {
      Point center = this.ActualCenter;
      foreach (CircularVertex v in this.Network.Vertexes) {
        //shift the positions based on ArrangementOrigin
        v.Position = new Point(v.Position.X + center.X, v.Position.Y + center.Y);
        v.CommitPosition();
      }
    }

    /// <summary>
    /// Commit the position and routing of all of the edge links.
    /// </summary>
    protected virtual void LayoutLinks() {
      int cnt = this.Network.Vertexes.Count();
      foreach (CircularEdge edge in this.Network.Edges) {
        if (this.Routing == CircularLinkRouting.Curved) {
          int ind1 = this.Network.VertexesArray.IndexOf(this.Network.VertexesArray.First(v => v.Node == edge.Link.ToNode));
          int ind2 = this.Network.VertexesArray.IndexOf(this.Network.VertexesArray.First(v => v.Node == edge.Link.FromNode));
          double c = (((double)(cnt)) / (Math.Abs(ind1 - ind2)) - 1) * 2 * Math.PI * ERadius / cnt;
          var l = edge.Link;
          l.Route = new Route() { Curve = LinkCurve.Bezier, Curviness = ind1 > ind2 ? c : -c }; // *****
        }
        edge.CommitPosition();
      }
    }
  }
}

