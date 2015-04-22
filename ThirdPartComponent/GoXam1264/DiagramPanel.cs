
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;

namespace Northwoods.GoXam {

  /// <summary>
  /// The <c>DiagramPanel</c> is the central class of a <see cref="Northwoods.GoXam.Diagram"/>,
  /// holding all of the <see cref="Layer"/>s of <see cref="Part"/>s,
  /// responsible for translation (scrolling) and scaling (zooming) and alignment,
  /// keeping track of the diagram's bounds,
  /// and having various hit-testing methods used to search the diagram for particular elements.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A <c>DiagramPanel</c> must be in the <c>ControlTemplate</c> of a <see cref="Northwoods.GoXam.Diagram"/>.
  /// Both the <see cref="Northwoods.GoXam.Diagram"/> and the <c>DiagramPanel</c> are useless without each other.
  /// </para>
  /// <para>
  /// Because <c>DiagramPanel</c> is a <c>Panel</c>, you can define the layers of this panel
  /// in XAML by placing them nested within the definition of the <c>&lt;DiagramPanel&gt;</c> element.
  /// If you do not define any layers explicitly, the <see cref="InitializeLayers"/> method
  /// will create the standard nine layers: "Background" nodes and links, default ("") nodes and links,
  /// "Foreground" nodes and links, "Tool" nodes and links, and "Adornment" nodes.
  /// </para>
  /// <para>
  /// This class is also responsible for mouse input.
  /// It redirects mouse events to the <see cref="Northwoods.GoXam.Diagram"/>'s <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>.
  /// </para>
  /// <para>
  /// You may not apply any transforms to a <see cref="Layer"/>; that is reserved for GoXam's use.
  /// This panel's <c>Background</c> is also reserved for GoXam's use.
  /// </para>
  /// </remarks>
  public class DiagramPanel : Panel, IScrollInfo {
    /// <summary>
    /// Construct an empty <see cref="DiagramPanel"/>.
    /// </summary>
    /// <remarks>
    /// By default the <c>Background</c> is a solid transparent brush so that it can
    /// handle mouse events in the background.
    /// </remarks>
    public DiagramPanel() {
      this.SizeChanged += DiagramPanel_SizeChanged;
      ClearBackground();  // to get background mouse events
      this.IsVirtualizing = false;  // set in InitialLayoutCompleted, so that CoordinatesTransform works

      this.IgnoresHandledEvents = true;
      // use AddHandler with handledEventsToo == true, so that we can be more confident about getting all mouse events inside nodes
      AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler((s, e) => OnMouseLeftButtonDown(e)), true);
      this.MouseMove += (s, e) => OnMouseMove(e);
      AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler((s, e) => OnMouseLeftButtonUp(e)), true);

      //??AddHandler(MouseRightButtonDownEvent, new MouseButtonEventHandler((s, e) => OnMouseRightButtonDown(e)), true);
      //??AddHandler(MouseRightButtonUpEvent, new MouseButtonEventHandler((s, e) => OnMouseRightButtonUp(e)), true);
      this.MouseRightButtonDown += (s, e) => OnMouseRightButtonDown(e);
      this.MouseRightButtonUp += (s, e) => OnMouseRightButtonUp(e);

      this.MouseEnter += (s, e) => OnMouseEnter(e);
      this.MouseWheel += (s, e) => OnMouseWheel(e);



    }

    static DiagramPanel() {
      // Dependency properties:

      // The extent of the objects in the document, in model coordinates
      DiagramBoundsProperty = DependencyProperty.Register("DiagramBounds", typeof(Rect), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(new Rect(), OnDiagramBoundsChanged));
      FixedBoundsProperty = DependencyProperty.Register("FixedBounds", typeof(Rect), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(new Rect(Double.NaN, Double.NaN, Double.NaN, Double.NaN), OnFixedBoundsChanged));

      // The panel's top-left corner's position in the document, in model coordinates
      PositionProperty = DependencyProperty.Register("Position", typeof(Point), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(new Point(), OnPositionChanged));

      // The scale at which objects are drawn in the panel.
      // The translation is specified by the Position property.
      ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(1.0, OnScaleChanged));
      MinimumScaleProperty = DependencyProperty.Register("MinimumScale", typeof(double), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(0.0001, OnMinimumScaleChanged));
      MaximumScaleProperty = DependencyProperty.Register("MaximumScale", typeof(double), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(100.0, OnMaximumScaleChanged));

      // extra visible space around the DiagramBounds, in model coordinates
      PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(new Thickness(5, 5, 5, 5), OnPaddingChanged));

      // how the document is aligned in the view, when the document at the current scale is smaller than the view
      HorizontalContentAlignmentProperty = DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(HorizontalAlignment.Left, OnHorizontalContentAlignmentChanged));
      VerticalContentAlignmentProperty = DependencyProperty.Register("VerticalContentAlignment", typeof(VerticalAlignment), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(VerticalAlignment.Top, OnVerticalContentAlignmentChanged));
      StretchProperty = DependencyProperty.Register("Stretch", typeof(StretchPolicy), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(StretchPolicy.Unstretched, OnStretchChanged));

      // The point in DiagramPanel's ("view" or "Control") coordinates at which to zoom when Scale changes
      // default value depends on Horizontal/VerticalContentAlignment
      ZoomPointProperty = DependencyProperty.Register("ZoomPoint", typeof(Point), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(new Point(Double.NaN, Double.NaN)));
      ZoomTimeProperty = DependencyProperty.Register("ZoomTime", typeof(int), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(500));  // milliseconds

      // scrolling
      CanHorizontallyScrollProperty = DependencyProperty.Register("CanHorizontallyScroll", typeof(bool), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(true));
      CanVerticallyScrollProperty = DependencyProperty.Register("CanVerticallyScroll", typeof(bool), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(true));

      ScrollOwnerProperty = DependencyProperty.Register("ScrollOwner", typeof(ScrollViewer), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(null));

      // each "line" of scrolling is some number of pixels; must be positive
      ScrollHorizontalLineChangeProperty = DependencyProperty.Register("ScrollHorizontalLineChange", typeof(double), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(16.0));
      ScrollVerticalLineChangeProperty = DependencyProperty.Register("ScrollVerticalLineChange", typeof(double), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(16.0));

      // autoscrolling
      AutoScrollRegionProperty = DependencyProperty.Register("AutoScrollRegion", typeof(Thickness), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(new Thickness(16, 16, 16, 16)));
      AutoScrollTimeProperty = DependencyProperty.Register("AutoScrollTime", typeof(int), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(250));  // milliseconds
      AutoScrollDelayProperty = DependencyProperty.Register("AutoScrollDelay", typeof(int), typeof(DiagramPanel),
        new FrameworkPropertyMetadata(1000));  // milliseconds






    }



    /// <summary>
    /// Throw an exception if the current thread does not have access to this <c>DependencyObject</c>.
    /// </summary>
    protected void VerifyAccess() {
      if (!CheckAccess()) Diagram.Error("No access to thread");
    }











    /// <summary>
    /// Gets the <see cref="Northwoods.GoXam.Diagram"/> that this panel is in.
    /// </summary>
    public Diagram Diagram { get; internal set; }
        

    // Dependency properties

    /// <summary>
    /// Identifies the <see cref="DiagramBounds"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DiagramBoundsProperty;  // "document" bounds
    /// <summary>
    /// Gets or sets the model-coordinate bounds of the diagram.
    /// </summary>
    /// <value>
    /// This is normally set by <see cref="UpdateDiagramBounds()"/>
    /// with the value returned by <see cref="ComputeDiagramBounds"/>.
    /// </value>
    /// <remarks>
    /// This property affects how far one may scroll.
    /// </remarks>
    public Rect DiagramBounds {
      get { return (Rect)GetValue(DiagramBoundsProperty); }
      set { SetValue(DiagramBoundsProperty, value); }
    }
    private static void OnDiagramBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      DiagramPanel panel = d as DiagramPanel;
      if (panel == null) return;
      //Diagram.Debug("DiagramBounds: " + Diagram.Str((Rect)e.OldValue) + " ==> " + Diagram.Str((Rect)e.NewValue));
      panel.BoundsOrAlignmentChanged();
    }

    /// <summary>
    /// Identifies the <see cref="FixedBounds"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FixedBoundsProperty;
    /// <summary>
    /// Gets or sets a fixed bounding rectangle to be returned by <see cref="ComputeDiagramBounds"/>.
    /// </summary>
    /// <value>
    /// By default this has Double.NaN values, meaning that <see cref="ComputeDiagramBounds"/>
    /// should actually compute the union of all of the parts in the diagram.
    /// If all X/Y/Width/Height values are actual numbers, this value is returned
    /// by <see cref="ComputeDiagramBounds"/>.
    /// </value>
    public Rect FixedBounds {
      get { return (Rect)GetValue(FixedBoundsProperty); }
      set { SetValue(FixedBoundsProperty, value); }
    }
    private static void OnFixedBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      DiagramPanel panel = d as DiagramPanel;
      if (panel == null) return;
      panel.InvokeUpdateDiagramBounds("FixedBounds");
    }


    /// <summary>
    /// Identifies the <see cref="Position"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PositionProperty;
    /// <summary>
    /// Gets or sets the point, in model coordinates, shown at the top-left corner of this panel.
    /// </summary>
    /// <value>
    /// The default value is (0,0).
    /// For example, increasing the X value of this property will cause all of the parts to appear farther to the left;
    /// some parts that are located at X coordinates just less than the new X value will "disappear" off-screen.
    /// This property changes as the user scrolls in any direction.
    /// </value>
    /// <remarks>
    /// The value may be limited by the size and extent of the <see cref="DiagramBounds"/>,
    /// as <see cref="OnPositionChanged(Point, Point)"/> tries to implement policies that limit how far one may scroll.
    /// </remarks>
    public Point Position {
      get { return (Point)GetValue(PositionProperty); }
      set { SetValue(PositionProperty, value); }
    }
    private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      DiagramPanel panel = d as DiagramPanel;
      if (panel == null) return;
      Point newp = (Point)e.NewValue;
      if (Double.IsNaN(newp.X) || Double.IsInfinity(newp.X) || Double.IsNaN(newp.Y) || Double.IsInfinity(newp.Y)) {
        panel.Position = new Point(0, 0);  //??? ClearValue
      } else {
        Point oldp = (Point)e.OldValue;
        //if (Double.IsNaN(oldp.X) || Double.IsNaN(oldp.Y))
        //  Diagram.Debug(Diagram.Str(panel.Diagram) + " position: " + Diagram.Str(oldp) + " ==> " + Diagram.Str(newp));
        panel.OnPositionChanged(oldp, newp);
        panel.UpdateScrollTransform();
      }
    }


    /// <summary>
    /// Identifies the <see cref="Scale"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ScaleProperty;
    /// <summary>
    /// Gets or sets the scale factor at which everything is rendered.
    /// </summary>
    /// <value>
    /// The default value is 1.0 -- one unit in model coordinates is the same size as a device-independent-pixel.
    /// Values larger than 1.0 result in parts that appear larger, as if zooming into the diagram.
    /// Values smaller than 1.0 make everything appear smaller, as if zooming out of the diagram.
    /// Any new value must be a number larger than zero.
    /// </value>
    /// <remarks>
    /// Changing the scale may also result in a change in <see cref="Position"/>,
    /// as <see cref="OnScaleChanged(double, double)"/> tries to keep the diagram's parts aligned in the panel.
    /// </remarks>
    public double Scale {
      get { return (double)GetValue(ScaleProperty); }
      set { SetValue(ScaleProperty, value); }
    }

    private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      DiagramPanel panel = d as DiagramPanel;
      if (panel == null) return;
      double news = (double)e.NewValue;
      double olds = (double)e.OldValue;
      if (news < panel.MinimumScale || news > panel.MaximumScale || Double.IsNaN(news)) {
        panel.Scale = olds;  //??? ClearValue
      } else {
        panel.OnScaleChanged(olds, news);
        panel.UpdateScrollTransform();
      }
    }

    /// <summary>
    /// Identifies the <see cref="MinimumScale"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinimumScaleProperty;
    /// <summary>
    /// Gets or sets the smallest value greater than zero that <see cref="Scale"/> may take.
    /// </summary>
    /// <value>
    /// The default value is 0.0001.
    /// Values must be larger than zero and no greater than one.
    /// This property is only used to limit the range of new values in the <see cref="Scale"/> setter.
    /// </value>
    public double MinimumScale {
      get { return (double)GetValue(MinimumScaleProperty); }
      set { SetValue(MinimumScaleProperty, value); }
    }

    private static void OnMinimumScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      DiagramPanel panel = d as DiagramPanel;
      if (panel == null) return;
      double news = (double)e.NewValue;
      if (news <= 0 || news > 1 || Double.IsNaN(news)) {
        panel.MinimumScale = (double)e.OldValue;  //??? ClearValue
      }
    }

    /// <summary>
    /// Identifies the <see cref="MaximumScale"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaximumScaleProperty;
    /// <summary>
    /// Gets or sets the largest value that <see cref="Scale"/> may take.
    /// </summary>
    /// <value>
    /// The default value is 100.0.
    /// Values must be no less than one.
    /// This property is only used to limit the range of new values in the <see cref="Scale"/> setter.
    /// </value>
    public double MaximumScale {
      get { return (double)GetValue(MaximumScaleProperty); }
      set { SetValue(MaximumScaleProperty, value); }
    }

    private static void OnMaximumScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      DiagramPanel panel = d as DiagramPanel;
      if (panel == null) return;
      double news = (double)e.NewValue;
      if (news < 1 || Double.IsInfinity(news) || Double.IsNaN(news)) {
        panel.MaximumScale = (double)e.OldValue;  //??? ClearValue
      }
    }


    /// <summary>
    /// Identifies the <see cref="Padding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PaddingProperty;
    /// <summary>
    /// Gets or sets the amount of extra "empty" space is reserved around the <see cref="DiagramBounds"/>.
    /// </summary>
    /// <value>
    /// This value is in model coordinates.
    /// The standard <c>ControlTemplate</c> for <see cref="Diagram"/>
    /// uses a <c>TemplateBinding</c> to get this value from the <see cref="Diagram"/>.
    /// </value>
    public Thickness Padding {
      get { return (Thickness)GetValue(PaddingProperty); }
      set { SetValue(PaddingProperty, value); }
    }
    private static void OnPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      DiagramPanel panel = d as DiagramPanel;
      if (panel == null) return;
      panel.InvokeUpdateDiagramBounds("Padding");  // this will call NormalizePosition() when it sets DiagramPanel.Bounds
    }


    /// <summary>
    /// Identifies the <see cref="HorizontalContentAlignment"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HorizontalContentAlignmentProperty;
    /// <summary>
    /// Gets or sets how the parts are positioned in the panel when the <see cref="DiagramBounds"/> width
    /// is smaller than the <see cref="ViewportWidth"/>.
    /// </summary>
    /// <value>
    /// A value of <c>HorizontalAlignment.Stretch</c> will not automatically scroll the diagram contents
    /// to be at the left, at the right, or centered.
    /// The standard <c>ControlTemplate</c> for <see cref="Diagram"/>
    /// uses a <c>TemplateBinding</c> to get this value from the <see cref="Diagram"/>.
    /// </value>
    /// <remarks>
    /// This value also affects how the <see cref="Position"/> changes as the <see cref="Scale"/> changes.
    /// </remarks>
    public HorizontalAlignment HorizontalContentAlignment {
      get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
      set { SetValue(HorizontalContentAlignmentProperty, value); }
    }
    private static void OnHorizontalContentAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      DiagramPanel panel = d as DiagramPanel;
      if (panel == null) return;
      panel.BoundsOrAlignmentChanged();
    }


    /// <summary>
    /// Identifies the <see cref="VerticalContentAlignment"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty VerticalContentAlignmentProperty;
    /// <summary>
    /// Gets or sets how the parts are positioned in the panel when the <see cref="DiagramBounds"/> width
    /// is smaller than the <see cref="ViewportWidth"/>.
    /// </summary>
    /// <value>
    /// A value of <c>VerticalAlignment.Stretch</c> will not automatically scroll the diagram contents
    /// to be at the top, at the bottom, or centered.
    /// The standard <c>ControlTemplate</c> for <see cref="Diagram"/>
    /// uses a <c>TemplateBinding</c> to get this value from the <see cref="Diagram"/>.
    /// </value>
    /// <remarks>
    /// This value also affects how the <see cref="Position"/> changes as the <see cref="Scale"/> changes.
    /// </remarks>
    public VerticalAlignment VerticalContentAlignment {
      get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
      set { SetValue(VerticalContentAlignmentProperty, value); }
    }
    private static void OnVerticalContentAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      DiagramPanel panel = d as DiagramPanel;
      if (panel == null) return;
      panel.BoundsOrAlignmentChanged();
    }


    /// <summary>
    /// Identifies the <see cref="Stretch"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StretchProperty;
    /// <summary>
    /// Gets or sets how the panel's <see cref="Scale"/> property is set,
    /// depending on the size of the <see cref="DiagramBounds"/>.
    /// </summary>
    /// <value>
    /// The standard <c>ControlTemplate</c> for <see cref="Diagram"/>
    /// uses a <c>TemplateBinding</c> to get this value from the <see cref="Diagram"/>.
    /// </value>
    /// <remarks>
    /// Set this property to <see cref="StretchPolicy.Uniform"/>
    /// to cause the whole diagram to appear in the panel, at a small enough scale
    /// that everything fits.
    /// </remarks>
    public StretchPolicy Stretch {
      get { return (StretchPolicy)GetValue(StretchProperty); }
      set { SetValue(StretchProperty, value); }
    }
    private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      DiagramPanel panel = d as DiagramPanel;
      if (panel == null) return;
      panel.BoundsOrAlignmentChanged();
    }


    /// <summary>
    /// Identifies the <see cref="ZoomPoint"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomPointProperty;
    /// <summary>
    /// Gets or sets the apparent focus point used when zooming in or out.
    /// </summary>
    /// <value>
    /// This value is in element coordinates, not in model coordinates.
    /// The default value is (NaN, NaN), which means the actual zoom point used
    /// depends on the <see cref="HorizontalContentAlignment"/> and
    /// <see cref="VerticalContentAlignment"/> properties.
    /// </value>
    /// <remarks>
    /// This is used by <see cref="OnScaleChanged(double, double)"/>.
    /// It is temporarily set by <see cref="Northwoods.GoXam.Tool.DiagramTool.StandardMouseWheel"/>
    /// so that the user can zoom into a diagram at the mouse point when
    /// rotating the mouse wheel with the Control key modifier.
    /// </remarks>

    [TypeConverter(typeof(Route.PointConverter))]

    public Point ZoomPoint {
      get { return (Point)GetValue(ZoomPointProperty); }
      set { SetValue(ZoomPointProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ZoomTime"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomTimeProperty;
    /// <summary>
    /// Gets or sets how quickly to change the <see cref="Scale"/> and/or <see cref="Position"/>
    /// properties when adapting to <see cref="DiagramBounds"/> changes or panel size changes
    /// causing content re-alignment, or in calls to <see cref="MakeVisible"/> or <see cref="CenterPart(Part, Action)"/>.
    /// </summary>
    /// <value>
    /// The time is in milliseconds.
    /// The default is 500 (one half of a second).
    /// </value>
    public int ZoomTime {
      get { return (int)GetValue(ZoomTimeProperty); }
      set { SetValue(ZoomTimeProperty, value); }
    }


    // Autoscrolling
    
    private DispatcherTimer _AutoScrollTimer;
    private Point _AutoScrollPoint;

    /// <summary>
    /// Identifies the <see cref="AutoScrollRegion"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AutoScrollRegionProperty;
    /// <summary>
    /// Gets or sets the margin in the panel where a mouse drag will automatically cause the view to scroll.
    /// </summary>
    /// <value>
    /// The <c>Thickness</c> value defaults to 16,16,16,16.
    /// </value>
    /// <remarks>
    /// When the mouse drag point is within this region on the left or right sides,
    /// the view will automatically scroll horizontally in that direction.  When the point is within
    /// the region on the top or bottom, the view will automatically scroll
    /// vertically in that direction.  You can specify a distance of zero to disable autoscrolling
    /// in a direction; a value of 0,0,0,0 turns off autoscrolling altogether.
    /// </remarks>
    /// <seealso cref="DoAutoScroll"/>
    /// <seealso cref="ScrollHorizontalLineChange"/>
    /// <seealso cref="ScrollVerticalLineChange"/>
    public Thickness AutoScrollRegion {
      get { return (Thickness)GetValue(AutoScrollRegionProperty); }
      set { SetValue(AutoScrollRegionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AutoScrollTime"/> dependency property.
    /// </summary>
    private /*?? public */ static readonly DependencyProperty AutoScrollTimeProperty;
    /// <summary>
    /// Gets or sets how quickly to change the <see cref="Position"/>
    /// when the mouse is in the <see cref="AutoScrollRegion"/>.
    /// </summary>
    /// <value>
    /// The time is in milliseconds.
    /// The default is 250 (one quarter of a second).
    /// The value must not be negative.
    /// </value>
    private /*?? public */ int AutoScrollTime {
      get { return (int)GetValue(AutoScrollTimeProperty); }
      set { SetValue(AutoScrollTimeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AutoScrollDelay"/> dependency property.
    /// </summary>
    private /*?? public */ static readonly DependencyProperty AutoScrollDelayProperty;
    /// <summary>
    /// Gets or sets how long to wait before autoscrolling.
    /// </summary>
    /// <value>
    /// The time is in milliseconds.
    /// The default is 1000 (one second).
    /// The value must not be negative.
    /// </value>
    /// <remarks>
    /// This is helpful in avoiding autoscrolling when the user is dragging something
    /// into the view and doesn't yet intend to autoscroll.
    /// </remarks>
    private /*?? public */ int AutoScrollDelay {
      get { return (int)GetValue(AutoScrollDelayProperty); }
      set { SetValue(AutoScrollDelayProperty, value); }
    }


    /// <summary>
    /// Start or continue automatically scrolling the view during a mouse drag.
    /// </summary>
    /// <param name="modelPnt">the current mouse point, in model coordinates</param>
    /// <remarks>
    /// <para>
    /// As soon <see cref="ComputeAutoScrollPosition"/> returns a new
    /// <see cref="Position"/> value, this method starts a <c>Timer</c>
    /// that waits for <see cref="AutoScrollDelay"/> milliseconds.
    /// After waiting, it repeatedly sets <see cref="Position"/>
    /// to the latest <see cref="ComputeAutoScrollPosition"/> value,
    /// until the position does not change (presumably because the 
    /// <see cref="Northwoods.GoXam.Diagram"/>'s last mouse point is no longer in the autoscroll
    /// margin).
    /// Setting this view's <see cref="Position"/> occurs each
    /// <see cref="AutoScrollTime"/> milliseconds.
    /// </para>
    /// <para>
    /// This method is normally called by those tools that want to support
    /// auto-scrolling during a mouse move, such as dragging.
    /// The timer is stopped when the mouse leaves this view.
    /// </para>
    /// </remarks>
    /// <seealso cref="StopAutoScroll"/>
    public void DoAutoScroll(Point modelPnt) {
      VerifyAccess();
      _AutoScrollPoint = TransformModelToView(modelPnt);
      Point newdocpos = ComputeAutoScrollPosition(_AutoScrollPoint);
      if (!Geo.IsApprox(newdocpos, this.Position)) {
        StartAutoScroll();
      } else {
        StopAutoScroll();
      }
    }

    private void StartAutoScroll() {
      if (_AutoScrollTimer != null) return;
      //Diagram.Debug("timer created");
      _AutoScrollTimer = new DispatcherTimer();
      _AutoScrollTimer.Tick += (s, e) => { autoScrollTick(); };
      if (this.Diagram.CurrentTool != this.Diagram.DraggingTool) {
        _AutoScrollTimer.Interval = new TimeSpan(0, 0, 0, 0, this.AutoScrollDelay);
      } else {
        _AutoScrollTimer.Interval = new TimeSpan(0, 0, 0, 0, this.AutoScrollTime);
      }
      _AutoScrollTimer.Start();
    }

    /// <summary>
    /// Stop any ongoing auto-scroll action.
    /// </summary>
    /// <remarks>
    /// This stops the Timer used to get repeating events to consider scrolling.
    /// </remarks>
    /// <seealso cref="DoAutoScroll"/>
    public void StopAutoScroll() {
      if (_AutoScrollTimer == null) return;
      //Diagram.Debug("timer stopped");
      if (_AutoScrollTimer.IsEnabled) {
        _AutoScrollTimer.Stop();
      }
      _AutoScrollTimer = null;
    }

    private void autoScrollTick() {
      if (_AutoScrollTimer != null && _AutoScrollTimer.IsEnabled) {
        //Diagram.Debug("  tick");
        _AutoScrollTimer = null;  // assume timer is now stopped
        Point newdocpos = ComputeAutoScrollPosition(_AutoScrollPoint);
        if (!Geo.IsApprox(newdocpos, this.Position)) {
          this.Position = newdocpos;
          //??? need to fake a mouse move to keep dragging while autoscrolling
          if (this.Diagram != null) {
            this.Diagram.CurrentTool.DoMouseMove();
          }
          DoUpdateDiagramBounds();  //?? synchronous
          StartAutoScroll();
        }
      }
    }


    /// <summary>
    /// This method is called to determine the next position in the document for this view,
    /// given a point at which the user is dragging the mouse.
    /// </summary>
    /// <param name="viewPnt">
    /// The mouse point, in control coordinates.
    /// </param>
    /// <value>
    /// The return value is in model coordinates.
    /// </value>
    /// <remarks>
    /// This uses the <see cref="AutoScrollRegion"/>, <see cref="ScrollHorizontalLineChange"/>,
    /// and <see cref="ScrollVerticalLineChange"/> properties
    /// to calculate a new <see cref="Position"/>.
    /// The closer the point is to the edge of the view, the larger a multiple of
    /// the <see cref="ScrollHorizontalLineChange"/> or <see cref="ScrollVerticalLineChange"/>
    /// is used as a scroll step in that direction.
    /// </remarks>
    protected virtual Point ComputeAutoScrollPosition(Point viewPnt) {
      Point docpos = this.Position;
      Thickness margin = this.AutoScrollRegion;
      // no autoscroll region? just return current Position
      if (margin.Top <= 0 && margin.Left <= 0 && margin.Right <= 0 && margin.Bottom <= 0) return docpos;
      // get the visible part of the panel
      Rect dispRect = new Rect(0, 0, this.ViewportWidth, this.ViewportHeight);
      // get the current panel position, in element coordinates
      Point viewpos = new Point(0, 0);
      // and compute the new position if viewPnt is near any of the four sides, within the margin
      if (viewPnt.X >= dispRect.X && viewPnt.X < dispRect.X + margin.Left) {
        int deltaX = Math.Max((int)this.ScrollHorizontalLineChange, 1);
        viewpos.X -= deltaX;
        if (viewPnt.X < dispRect.X + margin.Left/2)
          viewpos.X -= deltaX;
        if (viewPnt.X < dispRect.X + margin.Left/4)
          viewpos.X -= 4*deltaX;
      } else if (viewPnt.X <= dispRect.X + dispRect.Width && viewPnt.X > dispRect.X + dispRect.Width - margin.Right) {
        int deltaX = Math.Max((int)this.ScrollHorizontalLineChange, 1);
        viewpos.X += deltaX;
        if (viewPnt.X > dispRect.X + dispRect.Width - margin.Right/2)
          viewpos.X += deltaX;
        if (viewPnt.X > dispRect.X + dispRect.Width - margin.Right/4)
          viewpos.X += 4*deltaX;
      }
      if (viewPnt.Y >= dispRect.Y && viewPnt.Y < dispRect.Y + margin.Top) {
        int deltaY = Math.Max((int)this.ScrollVerticalLineChange, 1);
        viewpos.Y -= deltaY;
        if (viewPnt.Y < dispRect.Y + margin.Top/2)
          viewpos.Y -= deltaY;
        if (viewPnt.Y < dispRect.Y + margin.Top/4)
          viewpos.Y -= 4*deltaY;
      } else if (viewPnt.Y <= dispRect.Y + dispRect.Height && viewPnt.Y > dispRect.Y + dispRect.Height - margin.Bottom) {
        int deltaY = Math.Max((int)this.ScrollVerticalLineChange, 1);
        viewpos.Y += deltaY;
        if (viewPnt.Y > dispRect.Y + dispRect.Height - margin.Bottom/2)
          viewpos.Y += deltaY;
        if (viewPnt.Y > dispRect.Y + dispRect.Height - margin.Bottom/4)
          viewpos.Y += 4*deltaY;
      }
      // don't bother changing DOCPOS if VIEWPOS hasn't changed
      // Also avoids differences due to round-off errors!
      if (!Geo.IsApprox(viewpos, new Point(0, 0))) {
        // but return a Point in model coordinates
        double scale = this.Scale;
        docpos = new Point(docpos.X + viewpos.X/scale, docpos.Y + viewpos.Y/scale);
      }
      //if (this.Position != docpos) Diagram.Debug(Diagram.Str(viewPnt) + Diagram.Str(dispRect) + " " +
      //  Diagram.Str(TransformModelToView(this.Position)) + " --> " + Diagram.Str(viewpos) + "  " + Diagram.Str(this.Position) + " --> " + Diagram.Str(docpos));
      return docpos;
    }


    // layers

    internal void InitializeLayers1() {
      InitializeLayers();
      UpdateScrollTransform();
    }

    /// <summary>
    /// If there are no <see cref="Layer"/>s in this panel's collection of <c>Children</c>,
    /// create the standard set of layers.
    /// </summary>
    /// <remarks>
    /// This creates the following pairs of <see cref="NodeLayer"/>s and <see cref="LinkLayer"/>s,
    /// with <see cref="Layer.Id"/>s of: "Background", default (""), "Foreground", and "Tool".
    /// Finally it creates and adds a <see cref="NodeLayer"/> that is the <see cref="AdornmentLayer"/>,
    /// in front of all of the other layers, for holding selection handles and tool handles.
    /// The "Tool" layers and <see cref="AdornmentLayer"/> are layers holding temporary objects,
    /// so this sets their <see cref="Layer.IsTemporary"/> property to true.
    /// </remarks>
    protected virtual void InitializeLayers() {
      if (this.Children.OfType<Layer>().Count() > 0) return;

      // create initial set of layers
      NodeLayer nlayer = new NodeLayer();
      nlayer.Id = "Background";
      this.Children.Add(nlayer);
      LinkLayer llayer = new LinkLayer();
      llayer.Id = "Background";
      this.Children.Add(llayer);

      nlayer = new NodeLayer();
      nlayer.Id = "";  // default Nodes layer
      this.Children.Add(nlayer);
      llayer = new LinkLayer();
      llayer.Id = "";  // default Links layer
      this.Children.Add(llayer);

      nlayer = new NodeLayer();
      nlayer.Id = "Foreground";
      this.Children.Add(nlayer);
      llayer = new LinkLayer();
      llayer.Id = "Foreground";
      this.Children.Add(llayer);

      nlayer = new NodeLayer();
      nlayer.Id = ToolLayerName;  // temporary nodes for tools
      nlayer.IsTemporary = true;
      this.Children.Add(nlayer);
      llayer = new LinkLayer();
      llayer.Id = ToolLayerName;
      llayer.IsTemporary = true;
      this.Children.Add(llayer);

      nlayer = new NodeLayer();
      nlayer.Id = AdornmentLayerName;  // adornment layer for selection handles et al
      nlayer.IsTemporary = true;
      this.Children.Add(nlayer);
    }

    /// <summary>
    /// Find a <see cref="Layer"/> of the given type <typeparamref name="T"/>
    /// with exactly the same <see cref="Layer.Id"/> as the given <paramref name="name"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T FindLayer<T>(String name) where T : Layer {
      VerifyAccess();
      if (name == null) name = "";
      return this.Children.OfType<T>().FirstOrDefault(l => l.Id == name);
    }

    /// <summary>
    /// Find a <see cref="Layer"/> of the given type <typeparamref name="T"/>
    /// with exactly the same <see cref="Layer.Id"/> as the given <paramref name="name"/>,
    /// or return a default layer of the given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <remarks>
    /// It is an error if it cannot find a layer with the given name nor
    /// with the name "" (an empty string).
    /// </remarks>
    public T GetLayer<T>(String name) where T : Layer {
      if (name == null) name = "";
      T layer = FindLayer<T>(name);
      if (layer == null) {
        layer = FindLayer<T>("");  // try default layer
        if (layer == null) {
          Diagram.Error("Cannot find either a layer named '" + name + "' nor the default layer");
        }
      }
      return layer;
    }

    /// <summary>
    /// Gets the standard layer used to hold <see cref="Adornment"/>s,
    /// which is normally in front of all of the other layers.
    /// </summary>
    public NodeLayer AdornmentLayer {
      get { return FindLayer<NodeLayer>(AdornmentLayerName); }  //?? optimize
    }
    internal const String AdornmentLayerName = "Adornment";


    internal const String ToolLayerName = "Tool";  // NB: also used in Generic.xaml


    // Content alignment

    private Rect AlignmentDiagramBounds {
      get { return this.DiagramBounds; }
    }

    private bool IsAligning { get; set; }

    // called when DiagramBounds or xxxContentAlignment or Stretch changes
    private void BoundsOrAlignmentChanged() {
      OnBoundsAlignmentChanged();
      UpdateScrollTransform();
    }

    /// <summary>
    /// This overridable method is called when the <see cref="DiagramBounds"/>
    /// or <see cref="HorizontalContentAlignment"/> or <see cref="VerticalContentAlignment"/>
    /// or <see cref="Stretch"/> properties change.
    /// </summary>
    /// <remarks>
    /// This may change the <see cref="Position"/> and/or <see cref="Scale"/> in order to
    /// maintain the desired content alignment, depending on the <see cref="Stretch"/> policy.
    /// The <see cref="Scale"/> will not be greater than 1.
    /// This respects the <c>UseLayoutRounding</c> property to round off the <see cref="Position"/>.
    /// This method calls <see cref="OnDiagramBoundsChanged(RoutedPropertyChangedEventArgs{Rect})"/>.
    /// </remarks>
    protected virtual void OnBoundsAlignmentChanged() {
      Rect oldbounds = this.DiagramBounds;
      double scale = ComputeScaleForStretch(this.Stretch);

      Point pos = this.Position;
      Rect bounds = this.DiagramBounds;
      double viewwidth = this.ViewportWidth/scale;
      double viewheight = this.ViewportHeight/scale;
      double x;
      switch (this.HorizontalContentAlignment) {
        case HorizontalAlignment.Left:
          x = Math.Min(pos.X, bounds.Right - viewwidth);
          x = Math.Max(x, bounds.Left);
          pos.X = x;
          break;
        case HorizontalAlignment.Right:
          x = pos.X + viewwidth;
          x = Math.Max(x, bounds.Left + viewwidth);
          x = Math.Min(x, bounds.Right);
          pos.X = x - viewwidth;
          break;
        case HorizontalAlignment.Center:
          if (viewwidth < bounds.Width) {
            x = pos.X + viewwidth/2;
            x = Math.Min(x, bounds.Right - viewwidth/2);
            x = Math.Max(x, bounds.Left + viewwidth/2);
            pos.X = x - viewwidth/2;
          } else {
            pos.X = (bounds.Left+bounds.Right)/2 - viewwidth/2;
          }
          break;
        case HorizontalAlignment.Stretch:
          if (viewwidth >= bounds.Width) {
            if (pos.X > bounds.Left) pos.X = bounds.Left;
            else if (pos.X < bounds.Right - viewwidth) pos.X = bounds.Right - viewwidth;
          }
          break;
      }
      double y;
      switch (this.VerticalContentAlignment) {
        case VerticalAlignment.Top:
          y = Math.Min(pos.Y, bounds.Bottom - viewheight);
          y = Math.Max(bounds.Top, y);
          pos.Y = y;
          break;
        case VerticalAlignment.Bottom:
          y = pos.Y + viewheight;
          y = Math.Max(y, bounds.Top + viewheight);
          y = Math.Min(y, bounds.Bottom);
          pos.Y = y - viewheight;
          break;
        case VerticalAlignment.Center:
          if (viewheight < bounds.Height) {
            y = pos.Y + viewheight/2;
            y = Math.Min(y, bounds.Bottom - viewheight/2);
            y = Math.Max(y, bounds.Top + viewheight/2);
            pos.Y = y - viewheight/2;
          } else {
            pos.Y = (bounds.Top+bounds.Bottom)/2 - viewheight/2;
          }
          break;
        case VerticalAlignment.Stretch:
          if (viewheight >= bounds.Height) {
            if (pos.Y > bounds.Top) pos.Y = bounds.Top;
            else if (pos.Y < bounds.Bottom - viewheight) pos.Y = bounds.Bottom - viewheight;
          }
          break;
      }
      if (Double.IsNaN(pos.X) || Double.IsInfinity(pos.X)) pos.X = 0;
      if (Double.IsNaN(pos.Y) || Double.IsInfinity(pos.Y)) pos.Y = 0;

      if (this.UseLayoutRounding) {
        pos.X = Math.Round(pos.X);
        pos.Y = Math.Round(pos.Y);
      }

      SetScaleAndPosition(scale, pos, (this.InitialLayoutCompleted ? this.ZoomTime : 0),
        () => {
          Rect newbounds = this.DiagramBounds;
          OnDiagramBoundsChanged(new RoutedPropertyChangedEventArgs<Rect>(oldbounds, newbounds));
        });
    }

    private double ComputeScaleForStretch(StretchPolicy policy) {
      if (policy == StretchPolicy.Unstretched) return this.Scale;
      Rect bounds = this.DiagramBounds;
      double xscale = this.ViewportWidth/Math.Max(bounds.Width, 1);
      double yscale = this.ViewportHeight/Math.Max(bounds.Height, 1);
      xscale = Math.Max(0.001, Math.Min(xscale, 1));
      yscale = Math.Max(0.001, Math.Min(yscale, 1));
      if (policy == StretchPolicy.Uniform) {
        return Math.Min(xscale, yscale);
      } else if (policy == StretchPolicy.UniformToFill) {
        return Math.Max(xscale, yscale);
      } else {
        return this.Scale;
      }
    }

    
    private void DiagramPanel_SizeChanged(object sender, SizeChangedEventArgs e) {
      OnSizeChanged(e.PreviousSize, e.NewSize);
    }

    private bool IsResizing { get; set; }

    /// <summary>
    /// This overridable method is called when the panel's actual size changes.
    /// </summary>
    /// <param name="oldsize">a <c>Size</c> in element coordinates</param>
    /// <param name="newsize">a <c>Size</c> in element coordinates</param>
    /// <remarks>
    /// This may change the <see cref="Position"/> and/or <see cref="Scale"/> in order to
    /// maintain the desired content alignment, depending on the <see cref="Stretch"/> policy.
    /// The <see cref="Scale"/> will not be greater than 1.
    /// This method may be called during initialization even if the old and new size values are the same.
    /// This respects the <c>UseLayoutRounding</c> property to round off the <see cref="Position"/>.
    /// This method calls <see cref="OnViewportBoundsChanged"/>.
    /// </remarks>
    protected virtual void OnSizeChanged(Size oldsize, Size newsize) {
      if (Geo.IsApprox(oldsize, newsize)) return;
      this.IsResizing = true;

      // need to calculate the original ViewportBounds, because the size has already been changed
      Rect oldvbounds = this.ViewportBounds;
      double oldscale = this.Scale;
      oldvbounds.Width = oldsize.Width/oldscale;
      oldvbounds.Height = oldsize.Height/oldscale;

      double newscale = ComputeScaleForStretch(this.Stretch);
      //Diagram.Debug(Diagram.Str(oldvbounds) + Diagram.Str(oldsize) + Diagram.Str(newsize) + newscale.ToString());

      Point pos = this.Position;
      Rect bounds = this.DiagramBounds;
      double viewwidth = newsize.Width/newscale;
      double viewheight = newsize.Height/newscale;
      double x;
      switch (this.HorizontalContentAlignment) {
        case HorizontalAlignment.Left:
          x = Math.Min(pos.X, bounds.Right - viewwidth);
          x = Math.Max(x, bounds.Left);
          pos.X = x;
          break;
        case HorizontalAlignment.Right:
          x = pos.X + oldsize.Width/newscale;
          x = Math.Max(x, bounds.Left + viewwidth);
          x = Math.Min(x, bounds.Right);
          pos.X = x - viewwidth;
          break;
        case HorizontalAlignment.Center:
          if (viewwidth < bounds.Width) {
            x = pos.X + (oldsize.Width/2)/newscale;
            x = Math.Min(x, bounds.Right - viewwidth/2);
            x = Math.Max(x, bounds.Left + viewwidth/2);
            pos.X = x - viewwidth/2;
          } else {
            pos.X = (bounds.Left+bounds.Right)/2 - viewwidth/2;
          }
          break;
        case HorizontalAlignment.Stretch:
          if (viewwidth >= bounds.Width) {
            if (pos.X > bounds.Left) pos.X = bounds.Left;
            else if (pos.X < bounds.Right - viewwidth) pos.X = bounds.Right - viewwidth;
          }
          break;
      }
      double y;
      switch (this.VerticalContentAlignment) {
        case VerticalAlignment.Top:
          y = Math.Min(pos.Y, bounds.Bottom - viewheight);
          y = Math.Max(bounds.Top, y);
          pos.Y = y;
          break;
        case VerticalAlignment.Bottom:
          y = pos.Y + oldsize.Height/newscale;
          y = Math.Max(y, bounds.Top + viewheight);
          y = Math.Min(y, bounds.Bottom);
          pos.Y = y - viewheight;
          break;
        case VerticalAlignment.Center:
          if (viewheight < bounds.Height) {
            y = pos.Y + (oldsize.Height/2)/newscale;
            y = Math.Min(y, bounds.Bottom - viewheight/2);
            y = Math.Max(y, bounds.Top + viewheight/2);
            pos.Y = y - viewheight/2;
          } else {
            pos.Y = (bounds.Top+bounds.Bottom)/2 - viewheight/2;
          }
          break;
        case VerticalAlignment.Stretch:
          if (viewheight >= bounds.Height) {
            if (pos.Y > bounds.Top) pos.Y = bounds.Top;
            else if (pos.Y < bounds.Bottom - viewheight) pos.Y = bounds.Bottom - viewheight;
          }
          break;
      }
      if (Double.IsNaN(pos.X) || Double.IsInfinity(pos.X)) pos.X = 0;
      if (Double.IsNaN(pos.Y) || Double.IsInfinity(pos.Y)) pos.Y = 0;

      if (this.UseLayoutRounding) {
        pos.X = Math.Round(pos.X);
        pos.Y = Math.Round(pos.Y);
      }

      SetScaleAndPosition(newscale, pos, (this.InitialLayoutCompleted ? this.ZoomTime : 0),
        () => {
          this.IsResizing = false;
          Rect newvbounds = this.ViewportBounds;
          OnViewportBoundsChanged(new RoutedPropertyChangedEventArgs<Rect>(oldvbounds, newvbounds));
          UpdateScrollTransform();
        });
    }

    /// <summary>
    /// This overridable method is called when the <see cref="Position"/> changes.
    /// </summary>
    /// <param name="oldpos">a <c>Point</c> in model coordinates</param>
    /// <param name="newpos">a <c>Point</c> in model coordinates</param>
    /// <remarks>
    /// This may change the <see cref="Position"/> in order to maintain the desired content alignment.
    /// This method may be called during initialization even if the old and new position values are the same.
    /// This respects the <c>UseLayoutRounding</c> property to round off the <see cref="Position"/>.
    /// This method calls <see cref="OnViewportBoundsChanged"/>.
    /// </remarks>
    protected virtual void OnPositionChanged(Point oldpos, Point newpos) {
      if (Geo.IsApprox(oldpos, newpos)) return;
      if (this.IsAnimating) return;
      if (this.IsAligning) return;
      this.IsAligning = true;

      Rect oldvbounds = this.ViewportBounds;
      oldvbounds.X = oldpos.X;
      oldvbounds.Y = oldpos.Y;
      Point pos = newpos;
      Rect bounds = this.DiagramBounds;
      double scale = this.Scale;
      double viewwidth = oldvbounds.Width;
      double viewheight = oldvbounds.Height;
      switch (this.HorizontalContentAlignment) {
        case HorizontalAlignment.Left:
          pos.X = Math.Max(pos.X, bounds.Left);
          break;
        case HorizontalAlignment.Right:
          pos.X = Math.Min(pos.X, bounds.Right - viewwidth);
          break;
        case HorizontalAlignment.Center:
          if (viewwidth < bounds.Width) {
            double x = pos.X + viewwidth/2;
            x = Math.Min(x, bounds.Right - viewwidth/2);
            x = Math.Max(x, bounds.Left + viewwidth/2);
            pos.X = x - viewwidth/2;
          } else {
            pos.X = (bounds.Left+bounds.Right)/2 - viewwidth/2;
          }
          break;
        case HorizontalAlignment.Stretch:
          if (viewwidth >= bounds.Width) {
            if (pos.X > bounds.Left) pos.X = bounds.Left;
            else if (pos.X < bounds.Right - viewwidth) pos.X = bounds.Right - viewwidth;
          }
          break;
      }
      switch (this.VerticalContentAlignment) {
        case VerticalAlignment.Top:
          pos.Y = Math.Max(bounds.Top, pos.Y);
          break;
        case VerticalAlignment.Bottom:
          pos.Y = Math.Min(pos.Y, bounds.Bottom - viewheight);
          break;
        case VerticalAlignment.Center:
          if (viewheight < bounds.Height) {
            double y = pos.Y + viewheight/2;
            y = Math.Min(y, bounds.Bottom - viewheight/2);
            y = Math.Max(y, bounds.Top + viewheight/2);
            pos.Y = y - viewheight/2;
          } else {
            pos.Y = (bounds.Top+bounds.Bottom)/2 - viewheight/2;
          }
          break;
        case VerticalAlignment.Stretch:
          if (viewheight >= bounds.Height) {
            if (pos.Y > bounds.Top) pos.Y = bounds.Top;
            else if (pos.Y < bounds.Bottom - viewheight) pos.Y = bounds.Bottom - viewheight;
          }
          break;
      }
      if (Double.IsNaN(pos.X) || Double.IsInfinity(pos.X)) pos.X = 0;
      if (Double.IsNaN(pos.Y) || Double.IsInfinity(pos.Y)) pos.Y = 0;

      if (this.UseLayoutRounding) {
        pos.X = Math.Round(pos.X);
        pos.Y = Math.Round(pos.Y);
      }

      this.Position = pos;
      this.IsAligning = false;
      Rect newvbounds = this.ViewportBounds;
      OnViewportBoundsChanged(new RoutedPropertyChangedEventArgs<Rect>(oldvbounds, newvbounds));
    }

    /// <summary>
    /// This overridable method is called when the <see cref="Scale"/> changes.
    /// </summary>
    /// <param name="oldscale">a <c>double</c></param>
    /// <param name="newscale">a <c>double</c></param>
    /// <remarks>
    /// This may change the <see cref="Position"/> in order to maintain the desired content alignment.
    /// This method may be called during initialization even if the old and new scale values are the same.
    /// This respects the <c>UseLayoutRounding</c> property to round off the <see cref="Position"/>.
    /// This method calls <see cref="OnViewportBoundsChanged"/>.
    /// </remarks>
    protected virtual void OnScaleChanged(double oldscale, double newscale) {
      if (this.IsAnimating) return;
      if (this.IsAligning) return;
      this.IsAligning = true;

      Rect oldvbounds = this.ViewportBounds;
      oldvbounds.Width = this.ViewportWidth/oldscale;
      oldvbounds.Height = this.ViewportHeight/oldscale;
      Point z = this.ZoomPoint;  // in element coordinates, not model coordinates
      // if the zoom point is NaN, default it according to the ...ContentAlignment properties
      if (Double.IsNaN(z.X)) {
        switch (this.HorizontalContentAlignment) {
          case HorizontalAlignment.Left: z.X = 0; break;
          case HorizontalAlignment.Right: z.X = this.ViewportWidth-1; break;
          case HorizontalAlignment.Center: z.X = this.ViewportWidth/2; break;
          case HorizontalAlignment.Stretch: z.X = this.ViewportWidth/2; break;
        }
      }
      if (Double.IsNaN(z.Y)) {
        switch (this.VerticalContentAlignment) {
          case VerticalAlignment.Top: z.Y = 0; break;
          case VerticalAlignment.Bottom: z.Y = this.ViewportHeight-1; break;
          case VerticalAlignment.Center: z.Y = this.ViewportHeight/2; break;
          case VerticalAlignment.Stretch: z.Y = this.ViewportHeight/2; break;
        }
      }
      Point oldpos = this.Position;
      Point pos = new Point(oldpos.X+z.X/oldscale-z.X/newscale, oldpos.Y+z.Y/oldscale-z.Y/newscale);
      Rect bounds = this.DiagramBounds;
      double scale = this.Scale;
      double viewwidth = this.ViewportWidth/scale;
      double viewheight = this.ViewportHeight/scale;
      double x;
      switch (this.HorizontalContentAlignment) {
        case HorizontalAlignment.Left:
          x = Math.Min(pos.X, bounds.Right - viewwidth);
          pos.X = Math.Max(x, bounds.Left);
          break;
        case HorizontalAlignment.Right:
          x = Math.Max(pos.X, bounds.Left);
          pos.X = Math.Min(x, bounds.Right - viewwidth);
          break;
        case HorizontalAlignment.Center:
          if (viewwidth < bounds.Width) {
            x = Math.Min(pos.X, bounds.Right - viewwidth);
            pos.X = Math.Max(x, bounds.Left);
          } else {
            x = (bounds.Left+bounds.Right)/2;
            pos.X = x - viewwidth/2;
          }
          break;
        case HorizontalAlignment.Stretch:
          if (viewwidth > bounds.Width) {
            if (pos.X > bounds.Left) pos.X = bounds.Left;
            else if (pos.X < bounds.Right - viewwidth) pos.X = bounds.Right - viewwidth;
          }
          break;
      }
      double y;
      switch (this.VerticalContentAlignment) {
        case VerticalAlignment.Top:
          y = Math.Min(pos.Y, bounds.Bottom - viewheight);
          pos.Y = Math.Max(bounds.Top, y);
          break;
        case VerticalAlignment.Bottom:
          y = Math.Max(pos.Y, bounds.Top);
          pos.Y = Math.Min(y, bounds.Bottom - viewheight);
          break;
        case VerticalAlignment.Center:
          if (viewheight < bounds.Height) {
            y = Math.Min(pos.Y, bounds.Bottom - viewheight);
            pos.Y = Math.Max(bounds.Top, y);
          } else {
            y = (bounds.Top+bounds.Bottom)/2;
            pos.Y = y - viewheight/2;
          }
          break;
        case VerticalAlignment.Stretch:
          if (viewheight > bounds.Height) {
            if (pos.Y > bounds.Top) pos.Y = bounds.Top;
            else if (pos.Y < bounds.Bottom - viewheight) pos.Y = bounds.Bottom - viewheight;
          }
          break;
      }
      if (Double.IsNaN(pos.X) || Double.IsInfinity(pos.X)) pos.X = 0;
      if (Double.IsNaN(pos.Y) || Double.IsInfinity(pos.Y)) pos.Y = 0;

      if (this.UseLayoutRounding) {
        pos.X = Math.Round(pos.X);
        pos.Y = Math.Round(pos.Y);
      }

      this.Position = pos;
      this.IsAligning = false;
      Rect newvbounds = this.ViewportBounds;
      OnViewportBoundsChanged(new RoutedPropertyChangedEventArgs<Rect>(oldvbounds, newvbounds));
    }









    /// <summary>
    /// This event occurs when the viewport changes.
    /// </summary>
    /// <remarks>
    /// The panel raises this event when the <see cref="Position"/>,
    /// <see cref="Scale"/> or size properties change, since those
    /// properties affect the value of <see cref="ViewportBounds"/>.
    /// </remarks>
    public event RoutedPropertyChangedEventHandler<Rect> ViewportBoundsChanged

;







    /// <summary>
    /// Raise the <see cref="ViewportBoundsChanged"/> event.
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>
    /// The X or Y may have changed because of a change to <see cref="Position"/>.
    /// The Width or Height may have changed either because the <see cref="DiagramPanel"/> has changed size
    /// or because the <see cref="Scale"/> has changed value.
    /// If the Width or Height has changed, this invalidates a <see cref="Northwoods.GoXam.Diagram.Layout"/>
    /// that depends on <see cref="Northwoods.GoXam.Layout.LayoutChange.ViewportSizeChanged"/>.
    /// </remarks>
    protected virtual void OnViewportBoundsChanged(RoutedPropertyChangedEventArgs<Rect> e) {
      if (e == null) return;
      Diagram diagram = this.Diagram;
      if (diagram != null &&
          (!Geo.IsApprox(e.NewValue.Width, e.OldValue.Width) || !Geo.IsApprox(e.NewValue.Height, e.OldValue.Height))) {
        LayoutManager mgr = diagram.LayoutManager;
        if (mgr != null) {
          mgr.InvalidateLayout(null, Northwoods.GoXam.Layout.LayoutChange.ViewportSizeChanged);
          if (diagram.Layout != null && !diagram.Layout.ValidLayout) {
            mgr.InvokeLayoutDiagram("ViewportSizeChanged");
          }
        }
      }

      if (this.ViewportBoundsChanged != null) this.ViewportBoundsChanged(this, e);




    }








    /// <summary>
    /// This event occurs when the diagram's bounds or alignment changes.
    /// </summary>
    /// <remarks>
    /// The panel raises this event when the <see cref="DiagramBounds"/>,
    /// the <see cref="FixedBounds"/>, <see cref="HorizontalContentAlignment"/>,
    /// or <see cref="VerticalContentAlignment"/> properties change.
    /// </remarks>
    public event RoutedPropertyChangedEventHandler<Rect> DiagramBoundsChanged

;







    /// <summary>
    /// Raise the <see cref="DiagramBoundsChanged"/> event.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnDiagramBoundsChanged(RoutedPropertyChangedEventArgs<Rect> e) {
      if (e == null) return;

      if (this.DiagramBoundsChanged != null) this.DiagramBoundsChanged(this, e);




    }


    // for use by Overview
    internal event EventHandler PartBoundsChanged;
    internal void RaisePartBoundsChanged(Part part) {
      if (this.PartBoundsChanged != null) this.PartBoundsChanged(part, EventArgs.Empty);
    }

    internal event EventHandler PartVisibleChanged;
    internal void RaisePartVisibleChanged(Part part) {
      if (this.PartVisibleChanged != null) this.PartVisibleChanged(part, EventArgs.Empty);
    }

    internal event EventHandler<ModelChangedEventArgs> AnimatingChanged;
    internal void RaiseAnimatingChanged(Object animator, bool animating) {
      if (this.AnimatingChanged != null) this.AnimatingChanged(animator, new ModelChangedEventArgs("IsAnimating", animator, null, animating));
    }


    // Translation and scaling

    internal void UpdateScrollTransform() {
      UpdateScrollTransform(this.Position, this.Scale, new Size(this.ViewportWidth, this.ViewportHeight), true);
    }

    internal void UpdateScrollTransform(Point pos, double scale, Size panelsize, bool clipandscroll) {
      VerifyAccess();

      if (this.IsResizing) return;
      if (this.UpdatingScrollTransform) return;
      this.UpdatingScrollTransform = true;

      this.GlobalTransform = null;  // just in case the Diagram has been transformed

      UpdateRouteGeometries();

      // scrollbars
      if (clipandscroll) {
        ScrollViewer scroller = this.ScrollOwner;
        if (scroller != null) {
          scroller.InvalidateScrollInfo();
          scroller.InvalidateMeasure();  // needed for Silverlight to collapse scrollbars when no longer needed
        }
      }

      // clip this panel
      Geometry clip = null;
      if (clipandscroll) {
        clip = new RectangleGeometry() { Rect = new Rect(0, 0, panelsize.Width, panelsize.Height) };
      }

      // transform to translate and scale
      TransformGroup tg = new TransformGroup();
      double dx = pos.X;
      double dy = pos.Y;
      tg.Children.Add(new TranslateTransform() { X = -dx, Y = -dy });
      tg.Children.Add(new ScaleTransform() { ScaleX = scale, ScaleY = scale });
      this.Transform = tg;

      // and the inverse transform
      TransformGroup ti = new TransformGroup();
      ti.Children.Add(new ScaleTransform() { ScaleX = 1.0/scale, ScaleY = 1.0/scale });
      ti.Children.Add(new TranslateTransform() { X = dx, Y = dy });
      this.InverseTransform = ti;







      if (clipandscroll) {
        if (_DefaultLayer == null || Diagram.LicenseKey != null) {
          if (_License == null) {
            try {
              _License = new DiagramLicense();
              if (_DefaultLayer != null) {
                this.Children.Remove(_DefaultLayer);
                _DefaultLayer = null;
              }
            } catch (Exception) {
              _License = null;
            }
          }
          if (_License == null && _DefaultLayer == null) _DefaultLayer = new DefaultLayer();
        }
        if (_DefaultLayer != null) {
          _DefaultLayer.Visibility = Visibility.Visible;
          _DefaultLayer.Opacity = 1;
          _DefaultLayer.RenderTransformOrigin = new Point(0, 0);
          _DefaultLayer.RenderTransform = null;
          _DefaultLayer.Clip = null;
          _DefaultLayer.OpacityMask = null;
          int num = this.Children.Count;
          if (num > 0 && this.Children[num-1] != _DefaultLayer) {
            Panel parent = Diagram.FindParent<Panel>(_DefaultLayer);
            if (parent != null) {
              parent.Children.Remove(_DefaultLayer);
            }
            this.Children.Add(_DefaultLayer);
          }
        }
      }

      // clip this panel
      this.Clip = clip;

      GridPattern grid = null;
      Diagram diagram = this.Diagram;
      if (diagram != null) {
        UpdateBackground();
        grid = diagram.GridPattern;
        if (grid != null && diagram.GridVisible) {
          if (clipandscroll) {
            grid.TransformBrush(this);  // scroll/zoom the bitmap right now; replace with something better later
            grid.InvokeBackgroundGridUpdate(this);
          } else {
            grid.Visibility = Visibility.Visible;
            grid.DoUpdateBackgroundGrid(this, new Rect(pos.X, pos.Y, panelsize.Width/scale, panelsize.Height/scale), scale, false);
          }
        }
      }

      // and assign transform to each layer, unless it's supposed to be static
      foreach (UIElement e in this.Children) {
        Layer layer = e as Layer;
        if (layer == null) continue;
        if (layer == _DefaultLayer) {
          layer.RenderTransform = null;
        } else {
          layer.RenderTransform = this.Transform;
        }
      }

      //Diagram.Debug("v: " + Diagram.Str(this.ViewportBounds) + "  a: " + Diagram.Str(this.ActualWidth) + "x" + Diagram.Str(this.ActualHeight) + "  b: " + Diagram.Str(this.DiagramBounds));
      if (this.IsVirtualizing && diagram != null) {
        Rect viewb = new Rect(pos.X, pos.Y, panelsize.Width/scale, panelsize.Height/scale);  // in model coordinates
        // include other selected parts that are off-screen but aren't too far away, but only if width and height > 0
        if (viewb.Width > 0 && viewb.Height > 0) {
          Geo.Inflate(ref viewb, 10, 10);
          bool mayberemove = clipandscroll && !this.DisableOffscreen && !(diagram.CurrentTool is DraggingTool);
          UpdateVisuals(viewb, mayberemove);
        }
      }

      this.UpdatingScrollTransform = false;
    }

    private bool UpdatingScrollTransform { get; set; }
    internal bool DisableOffscreen { get; set; }
    private DefaultLayer _DefaultLayer = null;

    private Transform Transform { get; set; }
    private Transform InverseTransform { get; set; }
    internal bool IsVirtualizing { get; set; }
    internal Dictionary<Route, Rect> DelayedGeometries = new Dictionary<Route, Rect>();

#pragma warning disable 1591
    public Object Unsupported(Object x, Object y) {
      if (x is int) {
        int choice = (int)x;
        switch (choice) {
          case 23: {
            if (y == null) return this.IsVirtualizing;
            if (y is bool) {
              bool val = (bool)y;
              if (val != this.IsVirtualizing) {
                if (val == false) {
                  UpdateAllVisuals();
                }
                this.IsVirtualizing = val;
              }
            }
            break;
          }
          case 24: {
            if (y == null) return this.CachesBitmap;
            if (y is bool) {
              this.CachesBitmap = (bool)y;
            }
            break;
          }









          case 26: {
              if (y == null) return this.Diagram.LayoutManager.AutoUpdatesLayout;
              if (y is bool) {
                this.Diagram.LayoutManager.AutoUpdatesLayout = (bool)y;
              }
              break;
            }
          default:
            break;
        }
      }
      return null;
    }
#pragma warning restore 1591

    internal Rect InflatedViewportBounds {  // in model coordinates
      get {
        Rect b = this.ViewportBounds;
        if (b.Width > 0 || b.Height > 0) Geo.Inflate(ref b, 10, 10);  // include other selected parts that are off-screen but aren't too far away
        return b;
      }
    }
    private bool MayOffscreen(Node n) {
      bool result = true;
      if (!n.ValidMeasure) result = false;
      if (result) {
        foreach (Link l in n.LinksConnected) {
          if (l.NeedsRerouting) {
            l.Route.InvalidateRoute();
            result = false;
          }
          l.Route.UpdatePoints();
        }
      }
      //Diagram.Debug("MayOffscreen " + Diagram.Str(n) + " " + result.ToString());
      return result;
    }

    internal void UpdateAllVisuals() {
      bool old = this.IsVirtualizing;
      this.IsVirtualizing = true;
      UpdateVisuals(new Rect(Double.NegativeInfinity, Double.NegativeInfinity, Double.PositiveInfinity, Double.PositiveInfinity), false);
      this.IsVirtualizing = old;
    }

    private void UpdateVisuals(Rect viewb, bool remove) {
      if (!this.IsVirtualizing) return;
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      int total = 0;
      int invisible = 0;
      if (remove) {  //?? when autoscrolling don't bother removing Parts that are going off-screen
        //double totalmillis = 0;
        NodeLayer adornlayer = this.AdornmentLayer;
        foreach (UIElement e in this.Children) {
          if (e == _DefaultLayer) continue;
          NodeLayer nlay = e as NodeLayer;
          if (nlay != null) {
            if (nlay == adornlayer) continue;  // optimization: skip layer of Adornments
            List<Node> offscreen = new List<Node>();
            try {
              foreach (Node n in nlay.Nodes) {  // might include Adornments -- but unusual not to be in AdornmentLayer
                if (!n.Offscreen && !n.IntersectsRect(viewb)

                && MayOffscreen(n)

) {
                  offscreen.Add(n);
                } else {
                  total++;
                }
              }
            } catch (Exception) {
            }
            if (offscreen.Count > 0) {
              invisible += offscreen.Count;
              offscreen.Reverse();  // Reverse for efficiency when removing
              //DateTime before = DateTime.Now;
              foreach (Node n in offscreen) {
                nlay.InternalRemove(n);
              }
              //totalmillis += (DateTime.Now-before).TotalMilliseconds;
            }
          } else {
            LinkLayer llay = e as LinkLayer;
            if (llay != null) {
              List<Link> offscreen = new List<Link>();
              try {
                foreach (Link l in llay.Links) {
                  if (!l.Offscreen && !l.IntersectsRect(viewb)) {
                    offscreen.Add(l);
                  } else {
                    total++;
                  }
                }
              } catch (Exception) {
              }
              if (offscreen.Count > 0) {
                invisible += offscreen.Count;
                offscreen.Reverse();  // Reverse for efficiency when removing
                //DateTime before = DateTime.Now;
                foreach (Link l in offscreen) {
                  llay.InternalRemove(l);
                }
                //totalmillis += (DateTime.Now-before).TotalMilliseconds;
              }
            }
          }
        }
        //if (totalmillis > 0) Diagram.Debug("offscreen: " + Diagram.Str(totalmillis));
      }
      //?? optimize further
      //DateTime beforeadd = DateTime.Now;
      int visible = 0;
      if (viewb.Width > 0 && viewb.Height > 0) {
        foreach (Node n in diagram.Nodes) {  // does not include any Adornments
          NodeLayer nlay = n.Layer as NodeLayer;
          if (nlay != null && Diagram.FindParent<NodeLayer>(n) == null) {
            if (n.IntersectsRect(viewb)) {
              visible++;
              nlay.InternalAdd(n);
            }
          }
        }
        foreach (Link l in diagram.Links) {
          LinkLayer llay = l.Layer as LinkLayer;
          if (llay != null && Diagram.FindParent<LinkLayer>(l) == null) {
            if (l.IntersectsRect(viewb)) {
              visible++;
              llay.InternalAdd(l);
            }
          }
        }
      }
      //double millis = (DateTime.Now-beforeadd).TotalMilliseconds;
      //if (millis > 0) Diagram.Debug("onscreen: " + Diagram.Str(millis));
      //if (invisible > 0 || visible > 0)
      //  Diagram.Debug("i: " + invisible.ToString() + " v: " + visible.ToString() + " totvis: " + (total+visible).ToString() + "   " + Diagram.Str(viewb) + (remove ? " removing" : ""));
    }

    private void UpdateRouteGeometries() {
      if (this.DelayedGeometries.Count > 0) {
        //Diagram.Debug("UpdateRouteGeometries " + this.DelayedGeometries.Count.ToString());
        foreach (var kvp in this.DelayedGeometries) {
          kvp.Key.InvalidateOtherJumpOvers(kvp.Value, this);
        }
        this.DelayedGeometries.Clear();
      }
    }

    private void UpdateDelayedRoutes() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      PartManager pmgr = diagram.PartManager;
      if (pmgr == null) return;
      UndoManager umgr = null;
      if (diagram.Model != null) umgr = diagram.Model.UndoManager;
      bool xactstarted = false;
      int countDelayed = 0;
      int countUnsaved = 0;
      foreach (Link link in this.Diagram.Links) {
        Route route = link.Route;
        if (route.DelayedRouting) {
          countDelayed++;
          if (!xactstarted && umgr != null) {
            xactstarted = true;
            umgr.StartTransaction("DelayedRouting");
          }
          route.DelayedRouting = false;
          route.InvalidateRoute();
          route.UpdatePoints();  // don't wait
        }
        if (route.UnsavedRoute && pmgr.UpdatesRouteDataPoints) {
          countUnsaved++;
          if (!xactstarted && umgr != null) {
            xactstarted = true;
            umgr.StartTransaction("DelayedRouting");
          }
          route.UnsavedRoute = false;
          pmgr.UpdateRouteDataPointsInternal(link);
        }
      }
      if (xactstarted && umgr != null) {
        if (umgr.CurrentEdit == null || umgr.CurrentEdit.Edits == null || umgr.CurrentEdit.Edits.Count == 0) {
          umgr.RollbackTransaction();
        } else {
          umgr.CommitTransaction("DelayedRouting");
          umgr.CoalesceLastTransaction("DelayedRouting");
        }
      }
      //Diagram.Debug("UpdateDelayedRoutes " + countDelayed.ToString() + " " + countUnsaved.ToString());
    }


    /// <summary>
    /// Convert a <c>Point</c> in model coordinates to element coordinates.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    /// <remarks>
    /// The transformation is affected by the <see cref="Position"/> and the <see cref="Scale"/>.
    /// </remarks>
    public Point TransformModelToView(Point p) {
      VerifyAccess();
      if (this.Transform != null) {
        Point q = this.Transform.Transform(p);
        return q;
      }
      return new Point();
    }

    /// <summary>
    /// Convert a <c>Point</c> in element coordinates to model coordinates.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    /// <remarks>
    /// The transformation is affected by the <see cref="Position"/> and the <see cref="Scale"/>.
    /// </remarks>
    public Point TransformViewToModel(Point p) {
      VerifyAccess();
      if (this.InverseTransform != null) {
        Point q = this.InverseTransform.Transform(p);
        return q;
      }
      return new Point();
    }


    private static GeneralTransform TransformToAncestor(DependencyObject reference, DependencyObject ancestor) {
      Matrix matrix = Matrix.Identity;
      while ((VisualTreeHelper.GetParent(reference) != null) && (reference != ancestor)) {

        var instance = reference as UIElement;
        if (instance != null) {
          TransformGroup next = null;
          Rect layoutRect = LayoutInformation.GetLayoutSlot(instance as FrameworkElement);
          // Update the layoutRect to include margins/alignmnets:
          FrameworkElement fe = instance as FrameworkElement;
          if (fe != null) {
            double offX = 0;
            double offY = 0;
            double width = fe.ActualWidth;
            double height = fe.ActualHeight;
            switch (fe.HorizontalAlignment) {
              case HorizontalAlignment.Center:
                offX += layoutRect.Width / 2 - width / 2;
                break;
              case HorizontalAlignment.Right:
                offX += layoutRect.Width - width;
                break;
            }
            switch (fe.VerticalAlignment) {
              case VerticalAlignment.Center:
                offY += layoutRect.Height / 2 - height / 2;
                break;
              case VerticalAlignment.Bottom:
                offY += layoutRect.Height - height;
                break;
            }
            // Handle Margins
            Thickness margin = fe.Margin;
            offX += margin.Left;
            offY += margin.Top;
            if (layoutRect.Width - (offX + width) < margin.Right)
              offX -= margin.Right - (layoutRect.Width - (offX + width));
            if (layoutRect.Height - (offY + height) < margin.Bottom)
              offY -= margin.Bottom - (layoutRect.Height - (offY + height));
            if (offX < margin.Left)
              offX += Math.Min(margin.Left - offX, offX);
            if (offY < margin.Top)
              offY += Math.Min(margin.Top - offY, offY);

            if (fe.UseLayoutRounding) {
              layoutRect.X += Math.Round(offX);
              layoutRect.Y += Math.Round(offY);
            } else {
              layoutRect.X += offX;
              layoutRect.Y += offY;
            }
            layoutRect.Width = width;
            layoutRect.Height = height;
          }

          Transform renderTransform = instance.RenderTransform;
          Point origin = instance.RenderTransformOrigin;
          if (renderTransform != null) {
            if (origin.X == 0 && origin.Y == 0) {
              next = new TransformGroup() { Children = { renderTransform } };
            } else {
              TranslateTransform tt1 = new TranslateTransform();
              tt1.X = -1 * layoutRect.Width * origin.X;
              tt1.Y = -1 * layoutRect.Height * origin.Y;
              TranslateTransform tt2 = new TranslateTransform();
              tt2.X = layoutRect.Width * origin.X;
              tt2.Y = layoutRect.Height * origin.Y;
              next = new TransformGroup() { Children = { tt1, renderTransform, tt2 } };
            }
          }





          if (next != null) {
            Matrix matrix2 = next.Value;
            matrix = Geo.Multiply(matrix, matrix2);
          }

          matrix.OffsetX += layoutRect.X;
          matrix.OffsetY += layoutRect.Y;




        }
        reference = VisualTreeHelper.GetParent(reference);
      }
      if (reference != ancestor) {
        return null;
      }
      MatrixTransform mt = new MatrixTransform();
      mt.Matrix = matrix;
      return mt;
    }

    internal static GeneralTransform CoordinatesTransform(DependencyObject container, DependencyObject target) {
      try {

        Part part = container as Part;
        if (part == null) part = Diagram.FindAncestor<Part>(target);
        if (part != null) part.EnsureOnscreen();
        //??? the try-catch is required because of frequent "Value does not fall within the expected range" exception
        var viscontainer = container as UIElement;
        var vistarget = target as UIElement;
        if (viscontainer != null && vistarget != null)
          return vistarget.TransformToVisual(viscontainer);
        else
          return TransformToAncestor(target, container);








      } catch (Exception) {
        return TransformToAncestor(target, container);
      }
    }

    GeneralTransform GlobalTransform { get; set; }


    internal Point TransformModelToGlobal(Point p) {
      if (this.Transform == null) return new Point();
      Point q = TransformModelToView(p);  // transform from model to view (actually panel)
      if (this.GlobalTransform == null) {
        this.GlobalTransform = CoordinatesTransform(Application.Current.RootVisual, this);
      }
      if (this.GlobalTransform != null) {
        //Diagram.Debug(Diagram.Str(p) + Diagram.Str(q) + Diagram.Str(this.GlobalTransform.Transform(q)));
        return this.GlobalTransform.Transform(q);
      } else {
        return q;
      }
    }

    internal double MakeBitmapScale { get; set; }  // also used by GridPattern
    private Rect MakeBitmapViewport { get; set; }



    // layout of the Layers that this DiagramPanel contains, but not of the layers' nodes or links
    /// <summary>
    /// Measure each of the <see cref="Layer"/>s in this panel.
    /// </summary>
    /// <param name="availableSize"></param>
    /// <returns></returns>
    protected override Size MeasureOverride(Size availableSize) {
      //Diagram.Debug("measuring panel: " + Diagram.Str(availableSize));

      foreach (UIElement e in this.Children) {
        if (e is GridPattern) continue;
        e.InvalidateMeasure();
        e.Measure(availableSize);
      }

      // availableSize is 0x0 when inside a Viewbox -- assume this panel were actually sized to fit the whole diagram
      Size sz = availableSize;
      if (sz.Width <= 0 || Double.IsInfinity(sz.Width)) {
        Rect b = this.AlignmentDiagramBounds;
        sz.Width = b.Width;
        //Diagram.Debug(Diagram.Str(b) + Diagram.Str(availableSize) + Diagram.Str(sz) + this.Diagram.ToString());
      }
      if (sz.Height <= 0 || Double.IsInfinity(sz.Height)) {
        Rect b = this.AlignmentDiagramBounds;
        sz.Height = b.Height;
      }
      //Diagram.Debug("measured panel: " + Diagram.Str(sz) + " of " + Diagram.Str(availableSize));
      return sz;
    }

    /// <summary>
    /// Arrange each of the <see cref="Layer"/>s in this panel.
    /// </summary>
    /// <param name="finalSize"></param>
    /// <returns></returns>
    protected override Size ArrangeOverride(Size finalSize) {
      //Diagram.Debug("arranging panel: " + Diagram.Str(finalSize));
      foreach (UIElement e in this.Children) {
        if (e.Visibility != Visibility.Visible) continue;
        if (e is Layer) {

          e.InvalidateArrange();

          e.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
        } else {
          GridPattern grid = e as GridPattern;
          if (grid != null) {

            if (this.MakeBitmapScale <= 0) {
              double sc = this.Scale;
              Rect vb = new Rect(0, 0, finalSize.Width/sc, finalSize.Height/sc);
              grid.RenderTransform = new ScaleTransform() { CenterX=0, CenterY=0, ScaleX=sc, ScaleY=sc };
              grid.Measure(new Size(vb.Width, vb.Height));
              grid.Arrange(vb);
              //Diagram.Debug("regular arrange of Grid: " + Diagram.Str(vb));
            } else {
              grid.DoUpdateBackgroundGrid(this, this.MakeBitmapViewport, this.MakeBitmapScale, false);
            }

          } else {



            e.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
          }
        }
      }
      Validate();
      //Diagram.Debug("arranged panel: " + Diagram.Str(finalSize));
      return finalSize;
    }

    private HashSet<Part> Invalids { get; set; }

    // don't bother trying to measure/arrange/update if offscreen
    internal void Uninvalidate(Part part) {  // used by ...Layer.InternalRemove
      if (this.Invalids != null) {
        //Diagram.Debug("Uninvalidate: " + Diagram.Str(part));
        this.Invalids.Remove(part);
        part.ValidMeasure = true;
        part.ValidArrange = true;
      }
    }

    // cause the Part to be measured and arranged
    internal void Invalidate(Part part, bool arrangeonly) {
      if (part == null) return;
      // if it's not in a Layer, never mind for now -- including when offscreen
      if (part.Layer == null) return;  // Add'ing to a Layer will call Invalidate
      if (part.IsMeasuringArranging) return;  // in the process of measuring this part, don't need to do again
      // mark as invalid
      if (!arrangeonly) part.ValidMeasure = false;
      part.ValidArrange = false;
      // remember as invalid, for later measure/arrange
      if (this.Invalids == null) this.Invalids = new HashSet<Part>();
      if (!this.Invalids.Add(part)) return;
      //Diagram.Debug("Invalidate #" + this.Invalids.Count.ToString() + ": " + Diagram.Str(part));
      // this eventually calls ArrangeOverride, which will call Validate to process all the Invalids
      InvalidateArrange();
    }

    // actually measure and arrange all of the invalidated Parts, held in this.Invalids
    private void Validate() {
      //?? limit cascading invalidations, in case there's an infinite loop of A invalidating B which invalidates A again
      // But this might mess up long chains of legitimate invalidations: e.g. link --> label --> link --> label etc.
      for (int i = 0; i < 23; i++) {
        HashSet<Part> validatings = this.Invalids;
        if (validatings == null || validatings.Count == 0) break;
        // accumulate a new set of invalid parts, if there are side-effects of measuring/arranging any part
        // or if we have to wait with measuring/arranging because of unmeasured/unarranged dependencies
        this.Invalids = new HashSet<Part>();
        Validate(validatings, this.Invalids);
      }
    }

    // one pass of the validation loop
    // for Parts that probably should not be measured/arranged yet due to dependencies on other Parts,
    // instead of calling Part.MeasureArrangeUpdate, just add the Part to the DELAY hashset,
    // which will be passed in as the INVALIDS hashset in the next iteration
    private void Validate(HashSet<Part> invalids, HashSet<Part> delay) {  // make sure everything is measured and arranged
      //int numads = invalids.Where(p => (p is Adornment)).Count();
      //Diagram.Debug(invalids.Count.ToString() + " " + numads.ToString() + " " + ((invalids.Count-numads < 10 && invalids.Count > numads) ? invalids.Where(p => !(p is Adornment)).Select(p => Diagram.Str(p)).Aggregate((x, y) => x + ", " + y) : ""));
      // first do all atomic nodes
      foreach (Part part in invalids) {
        Node n = part as Node;
        if (n == null) continue;
        if (n is Group) continue;
        if (n.IsReadyToMeasureArrange()) {
          n.MeasureArrangeUpdate(this);
        } else {
          // try all link labels later, after the links have been measured/arranged
          delay.Add(n);
        }
      }
      // then all group nodes
      foreach (Part part in invalids) {
        Group g = part as Group;
        if (g == null) continue;
        if (g.IsReadyToMeasureArrange()) {
          g.MeasureArrangeUpdate(this);
        } else {
          delay.Add(g);  // try again later
        }
      }
      // then do all links, which might invalidate label nodes
      foreach (Part part in invalids) {
        Link l = part as Link;
        if (l == null) continue;
        if (l.IsReadyToMeasureArrange()) {
          l.MeasureArrangeUpdate(this);
        } else {
          delay.Add(l);
        }
      }
    }


    // DiagramBounds

    /// <summary>
    /// Request that the <see cref="DiagramBounds"/> value be updated,
    /// with a call to <see cref="ComputeDiagramBounds"/>.
    /// </summary>
    public void UpdateDiagramBounds() {
      InvokeUpdateDiagramBounds("Update");
    }

    internal void InvokeUpdateDiagramBounds(String why) {
      VerifyAccess();
      if (why == "Layout") {
        this.LayoutCompleted = true;
        UpdateDelayedRoutes();
      }
      //Diagram.Debug("InvokeUpdateDiagramBounds " + why);
      if (this.UpdateDiagramBoundsReason == null) {
        this.UpdateDiagramBoundsReason = why;
        Diagram.InvokeLater(this, DoUpdateDiagramBounds);
      }
    }

    private String UpdateDiagramBoundsReason { get; set; }
    private bool LayoutCompleted { get; set; }
    internal bool InitialLayoutCompleted { get; set; }
    internal bool SkipsNextUpdateDiagramBounds { get; set; }

    private void DoUpdateDiagramBounds() {
      VerifyAccess();
      // in case some are left over or because of undo/redo
      UpdateRouteGeometries();
      if (this.SkipsNextUpdateDiagramBounds) {
        this.SkipsNextUpdateDiagramBounds = false;
        Diagram.InvokeLater(this, DoUpdateDiagramBounds);
        return;
      }
      try {
        this.DiagramBounds = ComputeDiagramBounds();
      } catch (Exception) {
      }
      //Diagram.Debug(this.UpdateDiagramBoundsReason + " " + Diagram.Str(this.DiagramBounds) + this.LayoutCompleted.ToString() + " " + this.InitialLayoutCompleted.ToString());
      this.UpdateDiagramBoundsReason = null;
      // if LayoutCompleted is true, this is the first time we've computed the diagram bounds since finishing a layout;
      // only raise the Diagram.InitialLayoutCompleted and Diagram.LayoutCompleted events after computing new diagram bounds
      if (this.LayoutCompleted && this.Diagram != null) {
        if (!this.InitialLayoutCompleted) {
          this.InitialLayoutCompleted = true;
          this.IsVirtualizing = true;
          OnInitialLayoutCompleted();
        }
        OnLayoutCompleted();
      }
      this.LayoutCompleted = false;
    }

    /// <summary>
    /// This overridable method is called after the first layout has been performed
    /// and the <see cref="DiagramBounds"/> has been computed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This uses the <see cref="Northwoods.GoXam.Diagram.InitialStretch"/>,
    /// <see cref="Northwoods.GoXam.Diagram.InitialScale"/>,
    /// <see cref="Northwoods.GoXam.Diagram.InitialPosition"/>,
    /// <see cref="Northwoods.GoXam.Diagram.InitialDiagramBoundsSpot"/>,
    /// <see cref="Northwoods.GoXam.Diagram.InitialPanelSpot"/>
    /// properties to set the <see cref="Scale"/> and/or <see cref="Position"/>
    /// properties for the initial appearance of the diagram.
    /// </para>
    /// <para>
    /// If <see cref="Northwoods.GoXam.Diagram.InitialCenteredNodeData"/> has a non-null value,
    /// this calls <see cref="CenterPart(Part)"/> to try to center the <see cref="Node"/>
    /// that is bound to that data.
    /// </para>
    /// <para>
    /// It then raises the <see cref="Northwoods.GoXam.Diagram.InitialLayoutCompleted"/> event.
    /// It also clears out any <see cref="Northwoods.GoXam.Model.UndoManager"/>'s recorded changes.
    /// </para>
    /// </remarks>
    protected virtual void OnInitialLayoutCompleted() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      //Diagram.Debug("OnInitialLayoutCompleted");

      if (diagram.InitialStretch != StretchPolicy.Unstretched) {
        this.Scale = ComputeScaleForStretch(diagram.InitialStretch);
      } else {
        double scale = diagram.InitialScale;
        if (!Double.IsNaN(scale) && scale > 0) {
          this.Scale = scale;
        }
        Point pos = diagram.InitialPosition;
        if (!Double.IsNaN(pos.X) && !Double.IsNaN(pos.Y)) {
          this.Position = pos;
        } else {
          if (diagram.InitialDiagramBoundsSpot.IsSpot && diagram.InitialPanelSpot.IsSpot) {
            // model coordinates
            Point dp = diagram.InitialDiagramBoundsSpot.PointInRect(this.DiagramBounds);
            // control/view coordinates
            Rect v = new Rect(0, 0, this.ViewportWidth, this.ViewportHeight);
            Point p = diagram.InitialPanelSpot.PointInRect(v);
            // model coordinates
            Point q = TransformViewToModel(p);
            Point p0 = this.Position;
            this.Position = new Point(p0.X+dp.X-q.X, p0.Y+dp.Y-q.Y);
          }
        }
      }

      Object nodedata = diagram.InitialCenteredNodeData;
      PartManager mgr = diagram.PartManager;
      if (nodedata != null && mgr != null) {
        Node node = mgr.FindNodeForData(nodedata, diagram.Model);
        if (node != null) {
          CenterPart(node);
        }
      }

      DiagramEventArgs e = new DiagramEventArgs();
      e.RoutedEvent = Diagram.InitialLayoutCompletedEvent;
      diagram.RaiseEvent(e);

      // clear out the UndoManager
      IDiagramModel model = diagram.Model;
      if (model != null) {
        UndoManager undomgr = model.UndoManager;
        if (undomgr != null) {
          undomgr.Clear();
        }
      }
    }

    /// <summary>
    /// This overridable method is called after each layout has been performed
    /// and the <see cref="DiagramBounds"/> has been computed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This raises the <see cref="Northwoods.GoXam.Diagram.LayoutCompleted"/> event.
    /// </para>
    /// <para>
    /// If <see cref="Northwoods.GoXam.Diagram.CenteredNodeData"/> has a non-null value,
    /// this calls <see cref="CenterPart(Part)"/> to try to center the <see cref="Node"/>
    /// that is bound to that data.
    /// </para>
    /// </remarks>
    protected virtual void OnLayoutCompleted() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      Object nodedata = diagram.CenteredNodeData;
      PartManager mgr = diagram.PartManager;
      if (nodedata != null && mgr != null) {
        Node node = mgr.FindNodeForData(nodedata, diagram.Model);
        if (node != null) {
          CenterPart(node);
        }
      }
      DiagramEventArgs e = new DiagramEventArgs();
      e.RoutedEvent = Diagram.LayoutCompletedEvent;
      diagram.RaiseEvent(e);
    }


    /// <summary>
    /// This is called by <see cref="UpdateDiagramBounds()"/> to
    /// determine a new value for <see cref="DiagramBounds"/>.
    /// </summary>
    /// <returns>a <c>Rect</c> in model coordinates</returns>
    /// <remarks>
    /// By default this computes the bounds of all of the visible nodes and links,
    /// excluding any <see cref="Adornment"/>s and parts that have <see cref="Part.InDiagramBounds"/> false,
    /// and then adds the <see cref="Padding"/>.
    /// </remarks>
    protected virtual Rect ComputeDiagramBounds() {  //?? customize & optimize
      VerifyAccess();
      Rect fb = this.FixedBounds;
      if (!Double.IsNaN(fb.X) && !Double.IsNaN(fb.Y) && !Double.IsNaN(fb.Width) && !Double.IsNaN(fb.Height)) {
        return fb;
      }
      Rect b = new Rect(0, 0, 0, 0);
      Diagram diagram = this.Diagram;
      if (diagram != null) {
        Rect b1 = ComputeBounds(diagram.Nodes.Cast<Part>().Where(p => p.CanIncludeInDiagramBounds()));
        Rect b2 = ComputeBounds(diagram.Links.Cast<Part>().Where(p => p.CanIncludeInDiagramBounds()));
        Rect b3;
        if (b1.IsEmpty && b2.IsEmpty) {
          b3 = b;
        } else if (b2.IsEmpty) {
          b3 = b1;
        } else if (b1.IsEmpty) {
          b3 = b2;
        } else {
          b3 = Geo.Union(b1, b2);
        }
        Thickness pad = this.Padding;
        b = new Rect(Math.Floor(b3.X - pad.Left), Math.Floor(b3.Y - pad.Top),
          Math.Ceiling(pad.Left + b3.Width + pad.Right), Math.Ceiling(pad.Top + b3.Height + pad.Bottom));
      }
      return b;
    }


    /// <summary>
    /// Compute the bounding rectangle that surrounds a given collection of parts.
    /// </summary>
    /// <param name="parts"></param>
    /// <returns>
    /// the union of the Bounds of all of the parts in <paramref name="parts"/>,
    /// or <c>Rect.Empty</c> if there are no parts
    /// </returns>
    /// <remarks>
    /// This does not skip parts that are not <c>Visible</c>.
    /// </remarks>
    public virtual Rect ComputeBounds(IEnumerable<Part> parts) {
      if (parts == null) return Rect.Empty;
      double minx = Double.MaxValue;
      double miny = Double.MaxValue;
      double maxx = Double.MinValue;
      double maxy = Double.MinValue;
      foreach (Part part in parts) {
        Rect b = part.GetElementBounds(part.VisualElement);  // support rotation of whole node
        //Diagram.Debug("  " + part.ToString() + " " + Diagram.Str(b));
        if (b.Width == 0 && b.Height == 0 && b.X == 0 && b.Y == 0 || (Double.IsNaN(b.X) || Double.IsNaN(b.Y))) continue;
        if (b.Left < minx) minx = b.Left;
        if (b.Top < miny) miny = b.Top;
        if (b.Right > maxx) maxx = b.Right;
        if (b.Bottom > maxy) maxy = b.Bottom;
      }
      //Diagram.Debug("ComputeBounds for " + Diagram.DiagramName(this.Diagram) + ": " + ((minx == Double.MaxValue) ? "(none)" : Diagram.Str(new Rect(minx, miny, maxx-minx, maxy-miny))));
      if (minx == Double.MaxValue)
        return Rect.Empty;
      else
        return new Rect(minx, miny, maxx-minx, maxy-miny);
    }

    internal static Rect ComputeLocations(IEnumerable<Node> nodes) {
      double minx = Double.MaxValue;
      double miny = Double.MaxValue;
      double maxx = Double.MinValue;
      double maxy = Double.MinValue;
      foreach (Node n in nodes) {
        Point l = n.Location;
        if (Double.IsNaN(l.X) || Double.IsNaN(l.Y)) continue;
        if (l.X < minx) minx = l.X;
        if (l.Y < miny) miny = l.Y;
        if (l.X > maxx) maxx = l.X;
        if (l.Y > maxy) maxy = l.Y;
      }
      if (minx == Double.MaxValue)
        return new Rect(0, 0, 0, 0);
      else
        return new Rect(minx, miny, maxx-minx, maxy-miny);
    }


    // Hit testing and similar geometrical queries

    /// <summary>
    /// Search all parts that are at a given point that meet a given predicate,
    /// and return the first element that matches.
    /// </summary>
    /// <typeparam name="T">the type of element being searched for</typeparam>
    /// <param name="p">a <c>Point</c> in model coordinates</param>
    /// <param name="navig">
    /// This is a function that is given an element at the given point and
    /// returns an element of type <typeparamref name="T"/> to be considered by the predicate <paramref name="pred"/>.
    /// Typically the function will find the ancestor <see cref="Part"/> or <see cref="Node"/>.
    /// </param>
    /// <param name="pred">
    /// This is a predicate that is given an element of type <typeparamref name="T"/>;
    /// If the predicate returns true, this method returns that element;
    /// if it returns false, the search continues for elements at the given point.
    /// If this predicate argument is null, no filtering of elements is done -- the first one found is returned.
    /// </param>
    /// <param name="layers">
    /// The kinds of <see cref="Layer"/>s to search in.
    /// For example, if you are looking for a <see cref="Node"/>, using the value <see cref="SearchLayers.Nodes"/>,
    /// this will speed up the search by ignore layers containing <see cref="Link"/>s.
    /// </param>
    /// <returns>an element of type <typeparamref name="T"/>, or null if none is found</returns>
    public T FindElementAt<T>(Point p, Func<

      UIElement



      , T> navig, Predicate<T> pred, SearchLayers layers)

      where T : UIElement



    {
      if (Double.IsNaN(p.X) || Double.IsNaN(p.Y)) return default(T);
      VerifyAccess();
      Rect viewb = this.InflatedViewportBounds;
      if (!Geo.Contains(viewb, p)) {
        UpdateVisuals(Geo.Union(viewb, p), false);
      }
      //DateTime before = DateTime.Now;
      for (int i = this.Children.Count-1; i >= 0; i--) {
        Layer layer = this.Children[i] as Layer;
        if (layer != null && layer != _DefaultLayer && layer.Visibility == Visibility.Visible) {
          if (((layer is NodeLayer && (layers & SearchLayers.Nodes) != 0) ||
               (layer is LinkLayer && (layers & SearchLayers.Links) != 0)) &&
              (((layers & SearchLayers.Temporary) != 0) || !layer.IsTemporary)) {
            T elt = layer.FindElementAt<T>(p, navig, pred);
            if (elt != null) {
              //Diagram.Debug("FindElementAt " + Diagram.Str(p) + " found " + ((DateTime.Now-before).TotalMilliseconds).ToString());
              return elt;
            }
          }
        }
      }
      //Diagram.Debug("FindElementAt " + Diagram.Str(p) + " null " + ((DateTime.Now-before).TotalMilliseconds).ToString());
      return null;
    }

    /// <summary>
    /// Search all parts that are at a given point that meet a given predicate,
    /// and return a collection of elements that match.
    /// </summary>
    /// <typeparam name="T">the type of element being searched for</typeparam>
    /// <param name="p">a <c>Point</c> in model coordinates</param>
    /// <param name="navig">
    /// This is a function that is given an element at the given point and
    /// returns an element of type <typeparamref name="T"/> to be considered by the predicate <paramref name="pred"/>.
    /// Typically the function will find the ancestor <see cref="Part"/> or <see cref="Node"/>.
    /// </param>
    /// <param name="pred">
    /// This is a predicate that is given an element of type <typeparamref name="T"/>;
    /// if the predicate returns true, this method includes that element in its results.
    /// If this predicate argument is null, no filtering of elements is done -- all are included.
    /// </param>
    /// <param name="layers">
    /// The kinds of <see cref="Layer"/>s to search in.
    /// For example, if you are looking for <see cref="Node"/>s, using the value <see cref="SearchLayers.Nodes"/>,
    /// this will speed up the search by ignoring layers containing <see cref="Link"/>s.
    /// </param>
    /// <returns>a perhaps empty collection of elements of type <typeparamref name="T"/></returns>
    public IEnumerable<T> FindElementsAt<T>(Point p, Func<

      UIElement



      , T> navig, Predicate<T> pred, SearchLayers layers)

      where T : UIElement



    {
      if (Double.IsNaN(p.X) || Double.IsNaN(p.Y)) return Enumerable.Empty<T>();
      VerifyAccess();
      Rect viewb = this.InflatedViewportBounds;
      if (!Geo.Contains(viewb, p)) {
        UpdateVisuals(Geo.Union(viewb, p), false);
      }
      //DateTime before = DateTime.Now;
      List<T> coll = new List<T>();
      for (int i = this.Children.Count-1; i >= 0; i--) {
        Layer layer = this.Children[i] as Layer;
        if (layer != null && layer != _DefaultLayer && layer.Visibility == Visibility.Visible) {
          if (((layer is NodeLayer && (layers & SearchLayers.Nodes) != 0) ||
               (layer is LinkLayer && (layers & SearchLayers.Links) != 0)) &&
              (((layers & SearchLayers.Temporary) != 0) || !layer.IsTemporary)) {
            coll.AddRange(layer.FindElementsAt<T>(p, navig, pred));
          }
        }
      }
      //Diagram.Debug("FindElementsAt " + ((DateTime.Now-before).TotalMilliseconds).ToString());
      return coll;
    }


    /// <summary>
    /// Find elements that are within a distance of a point.
    /// </summary>
    /// <typeparam name="T">the type of element being searched for</typeparam>
    /// <param name="p">a <c>Point</c> in model coordinates</param>
    /// <param name="dist">the distance, in model coordinates</param>
    /// <param name="navig">
    /// This is a function that is given an element at the given point and
    /// returns an element of type <typeparamref name="T"/> to be considered by the predicate <paramref name="pred"/>.
    /// Typically the function will find the ancestor <see cref="Part"/> or <see cref="Node"/>.
    /// </param>
    /// <param name="pred">
    /// This is a predicate that is given an element of type <typeparamref name="T"/>;
    /// if the predicate returns true, this method includes that element in its results.
    /// </param>
    /// <param name="layers">
    /// The kinds of <see cref="Layer"/>s to search in.
    /// For example, if you are looking for <see cref="Node"/>s, using the value <see cref="SearchLayers.Nodes"/>,
    /// this will speed up the search by ignoring layers containing <see cref="Link"/>s.
    /// </param>
    /// <returns>an element of type <typeparamref name="T"/>, or null if none is found</returns>
    public IEnumerable<T> FindElementsNear<T>(Point p, double dist, Func<

      UIElement



      , T> navig, Predicate<T> pred, SearchLayers layers)

      where T : UIElement



    {
      if (Double.IsNaN(p.X) || Double.IsNaN(p.Y)) return Enumerable.Empty<T>();
      return FindElementsWithin<T>(new EllipseGeometry() { Center = p, RadiusX = dist, RadiusY = dist }, navig, pred, layers);
    }

    /// <summary>
    /// Find elements that are within a rectangle.
    /// </summary>
    /// <typeparam name="T">the type of element being searched for</typeparam>
    /// <param name="rect">a <c>Rect</c> in model coordinates</param>
    /// <param name="navig">
    /// This is a function that is given an element at the given point and
    /// returns an element of type <typeparamref name="T"/> to be considered by the predicate <paramref name="pred"/>.
    /// Typically the function will find the ancestor <see cref="Part"/> or <see cref="Node"/>.
    /// </param>
    /// <param name="pred">
    /// This is a predicate that is given an element of type <typeparamref name="T"/>;
    /// if the predicate returns true, this method includes that element in its results.
    /// </param>
    /// <param name="layers">
    /// The kinds of <see cref="Layer"/>s to search in.
    /// For example, if you are looking for <see cref="Node"/>s, using the value <see cref="SearchLayers.Nodes"/>,
    /// this will speed up the search by ignoring layers containing <see cref="Link"/>s.
    /// </param>
    /// <returns>a perhaps empty collection of elements of type <typeparamref name="T"/></returns>
    public IEnumerable<T> FindElementsIn<T>(Rect rect, Func<

      UIElement



      , T> navig, Predicate<T> pred, SearchLayers layers)

      where T : UIElement



    {
      if (Double.IsNaN(rect.X) || Double.IsNaN(rect.Y)) return Enumerable.Empty<T>();
      return FindElementsWithin<T>(new RectangleGeometry() { Rect = rect }, navig, pred, layers);
    }

    /// <summary>
    /// Search all parts that are at inside a given geometry that meet a given predicate,
    /// and return a collection of elements that match.
    /// </summary>
    /// <typeparam name="T">the type of element being searched for</typeparam>
    /// <param name="geo">a <c>Geometry</c> in model coordinates</param>
    /// <param name="navig">
    /// This is a function that is given an element at the given point and
    /// returns an element of type <typeparamref name="T"/> to be considered by the predicate <paramref name="pred"/>.
    /// Typically the function will find the ancestor <see cref="Part"/> or <see cref="Node"/>.
    /// </param>
    /// <param name="pred">
    /// This is a predicate that is given an element of type <typeparamref name="T"/>;
    /// if the predicate returns true, this method includes that element in its results.
    /// </param>
    /// <param name="layers">
    /// The kinds of <see cref="Layer"/>s to search in.
    /// For example, if you are looking for <see cref="Node"/>s, using the value <see cref="SearchLayers.Nodes"/>,
    /// this will speed up the search by ignoring layers containing <see cref="Link"/>s.
    /// </param>
    /// <returns>a perhaps empty collection of elements of type <typeparamref name="T"/></returns>
    internal IEnumerable<T> FindElementsWithin<T>(Geometry geo, Func<

      UIElement



      , T> navig, Predicate<T> pred, SearchLayers layers)

      where T : UIElement



    {
      VerifyAccess();
      Rect viewb = this.InflatedViewportBounds;
      Rect geob = geo.Bounds;
      if (!Geo.Contains(viewb, geob)) {
        UpdateVisuals(Geo.Union(viewb, geob), false);
      }
      //DateTime before = DateTime.Now;
      List<T> coll = new List<T>();
      for (int i = this.Children.Count-1; i >= 0; i--) {
        Layer layer = this.Children[i] as Layer;
        if (layer != null && layer != _DefaultLayer && layer.Visibility == Visibility.Visible) {
          if (((layer is NodeLayer && (layers & SearchLayers.Nodes) != 0) ||
               (layer is LinkLayer && (layers & SearchLayers.Links) != 0)) &&
              (((layers & SearchLayers.Temporary) != 0) || !layer.IsTemporary)) {
            coll.AddRange(layer.FindElementsWithin<T>(geo, navig, pred));
          }
        }
      }
      //Diagram.Debug("FindElementsWithin " + ((DateTime.Now-before).TotalMilliseconds).ToString());
      return coll;
    }

    /// <summary>
    /// Return the front-most <see cref="Part"/> that is at a given point.
    /// </summary>
    /// <typeparam name="T">the Type of part to find</typeparam>
    /// <param name="p">a <c>Point</c> in model coordinates</param>
    /// <param name="pred">
    /// This is a predicate that is given an part of type <typeparamref name="T"/>;
    /// if the predicate returns true, this method returns that part.
    /// If this argument is null, this returns the first part it finds at the point.
    /// </param>
    /// <param name="layers">
    /// The kinds of <see cref="Layer"/>s to search in.
    /// For example, if you are looking for <see cref="Node"/>s, using the value <see cref="SearchLayers.Nodes"/>,
    /// this will speed up the search by ignoring layers containing <see cref="Link"/>s.
    /// </param>
    /// <returns>null if no such part is found</returns>
    public T FindPartAt<T>(Point p, Predicate<T> pred, SearchLayers layers) where T : Part {
      return FindElementAt<T>(p, Part.FindAncestor<T>, pred, layers);
    }

    /// <summary>
    /// Return all of the <see cref="Part"/>s that are at a given point.
    /// </summary>
    /// <typeparam name="T">the Type of part to find</typeparam>
    /// <param name="p">a <c>Point</c> in model coordinates</param>
    /// <param name="layers">
    /// The kinds of <see cref="Layer"/>s to search in.
    /// For example, if you are looking for <see cref="Node"/>s, using the value <see cref="SearchLayers.Nodes"/>,
    /// this will speed up the search by ignoring layers containing <see cref="Link"/>s.
    /// </param>
    /// <returns>
    /// All of the <see cref="Part"/>s of the given type <typeparamref name="T" />
    /// that are at the Point <paramref name="p"/>
    /// and that are in the kinds of layers indicated by <paramref name="layers"/>.
    /// This returns an empty list if no qualifying parts are found.
    /// </returns>
    public IEnumerable<T> FindPartsAt<T>(Point p, SearchLayers layers) where T : Part {
      return FindElementsAt<T>(p, Part.FindAncestor<T>, null, layers);
    }


    /// <summary>
    /// Find parts that are near a point.
    /// </summary>
    /// <typeparam name="T">the type of part being searched for, either the type <see cref="Part"/> or inheriting from it</typeparam>
    /// <param name="p">a <c>Point</c> in model coordinates</param>
    /// <param name="dist">the distance, in model coordinates</param>
    /// <param name="srch">
    /// Which kinds of parts to include; a typical value might be <see cref="SearchFlags.SelectableParts"/>.
    /// </param>
    /// <param name="overlap">
    /// The required geometric relationship with the part;
    /// the typical value is <see cref="SearchInclusion.Inside"/>.
    /// </param>
    /// <param name="layers">
    /// The kinds of <see cref="Layer"/>s to search in.
    /// For example, if you are looking for <see cref="Node"/>s, using the value <see cref="SearchLayers.Nodes"/>,
    /// this will speed up the search by ignoring layers containing <see cref="Link"/>s.
    /// </param>
    /// <returns></returns>
    public IEnumerable<T> FindPartsNear<T>(Point p, double dist, SearchFlags srch, SearchInclusion overlap, SearchLayers layers) where T : Part {
      if (Double.IsNaN(p.X) || Double.IsNaN(p.Y)) return Enumerable.Empty<T>();
      return FindPartsWithin<T>(new EllipseGeometry() { Center = p, RadiusX = dist, RadiusY = dist }, srch, overlap, layers);
    }

    /// <summary>
    /// Find parts that are within a rectangle.
    /// </summary>
    /// <typeparam name="T">the type of part being searched for, either the type <see cref="Part"/> or inheriting from it</typeparam>
    /// <param name="rect">a <c>Rect</c> in model coordinates</param>
    /// <param name="srch">
    /// Which kinds of parts to include; a typical value might be <see cref="SearchFlags.SelectableParts"/>.
    /// </param>
    /// <param name="overlap">
    /// The required geometric relationship with the part;
    /// the typical value is <see cref="SearchInclusion.Inside"/>.
    /// </param>
    /// <param name="layers">
    /// The kinds of <see cref="Layer"/>s to search in.
    /// For example, if you are looking for <see cref="Node"/>s, using the value <see cref="SearchLayers.Nodes"/>,
    /// this will speed up the search by ignoring layers containing <see cref="Link"/>s.
    /// </param>
    /// <returns></returns>
    public IEnumerable<T> FindPartsIn<T>(Rect rect, SearchFlags srch, SearchInclusion overlap, SearchLayers layers) where T : Part {
      if (Double.IsNaN(rect.X) || Double.IsNaN(rect.Y)) return Enumerable.Empty<T>();
      return FindPartsWithin<T>(new RectangleGeometry() { Rect = rect }, srch, overlap, layers);
    }

    /// <summary>
    /// Search all parts that are at inside a given geometry that meet certain criteria
    /// and return a collection of those that match.
    /// </summary>
    /// <typeparam name="T">the type of part being searched for, either the type <see cref="Part"/> or inheriting from it</typeparam>
    /// <param name="geo">a <c>Geometry</c> in model coordinates</param>
    /// <param name="srch">
    /// Which kinds of parts to include; a typical value might be <see cref="SearchFlags.SelectableParts"/>.
    /// </param>
    /// <param name="overlap">
    /// The required geometric relationship with the <paramref name="geo"/> geometry;
    /// the typical value is <see cref="SearchInclusion.Inside"/>.
    /// </param>
    /// <param name="layers">
    /// The kinds of <see cref="Layer"/>s to search in.
    /// For example, if you are looking for <see cref="Node"/>s, using the value <see cref="SearchLayers.Nodes"/>,
    /// this will speed up the search by ignoring layers containing <see cref="Link"/>s.
    /// </param>
    /// <returns>a perhaps empty collection of elements of type <typeparamref name="T"/></returns>
    internal IEnumerable<T> FindPartsWithin<T>(Geometry geo, SearchFlags srch, SearchInclusion overlap, SearchLayers layers) where T : Part {
      VerifyAccess();
      Rect viewb = this.InflatedViewportBounds;
      Rect geob = geo.Bounds;
      if (!Geo.Contains(viewb, geob)) {
        UpdateVisuals(Geo.Union(viewb, geob), false);
      }
      //DateTime before = DateTime.Now;
      List<T> coll = new List<T>();
      for (int i = this.Children.Count-1; i >= 0; i--) {
        Layer layer = this.Children[i] as Layer;
        if (layer != null && layer != _DefaultLayer && layer.Visibility == Visibility.Visible) {
          if (((layer is NodeLayer && (layers & SearchLayers.Nodes) != 0) ||
               (layer is LinkLayer && (layers & SearchLayers.Links) != 0)) &&
              (((layers & SearchLayers.Temporary) != 0) || !layer.IsTemporary)) {
            coll.AddRange(layer.FindPartsWithin<T>(geo, srch, overlap));
          }
        }
      }
      //Diagram.Debug("FindPartsWithin " + ((DateTime.Now-before).TotalMilliseconds).ToString());
      return coll;
    }


    // used by PartManager.Rebuild...

    internal void ClearAll() {
      if (this.Invalids != null) this.Invalids.Clear();
      foreach (UIElement e in this.Children) {
        NodeLayer nlayer = e as NodeLayer;
        if (nlayer != null) {
          nlayer.InternalClear(true);
        } else {
          LinkLayer llayer = e as LinkLayer;
          if (llayer != null) {
            llayer.InternalClear(true);
          }
        }
      }
    }

    internal void ClearLinks() {
      if (this.Invalids != null) {
        // use ToList because of concurrent Removes
        foreach (Link link in this.Invalids.OfType<Link>().ToList()) {
          this.Invalids.Remove(link);
        }
      }
      foreach (UIElement e in this.Children) {
        LinkLayer layer = e as LinkLayer;
        if (layer != null) {
          layer.InternalClear(true);
        }
      }
    }

    internal void SortZOrder() {
      foreach (UIElement e in this.Children) {
        NodeLayer nlayer = e as NodeLayer;
        if (nlayer != null) nlayer.SortZOrder();
      }
    }


    // Background GridPattern
    // the GridPattern itself is available as Diagram.GridPattern

    private void UpdateBackground() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      if (diagram.GridVisible) {
        GridPattern grid = diagram.GridPattern;
        if (grid == null) {  // create any background grid
          grid = CreateBackgroundGridPattern();
          if (grid != null) diagram.GridPattern = grid;  // save as GridPattern
        }
        if (grid != null) {  // make sure it's in the panel
          if (this.Children.Count == 0) {
            diagram.RemoveLogical(grid);
            this.Children.Insert(0, grid);
          } else if (this.Children[0] != grid) {  // remove any other existing GridPattern
            if (this.Children[0] is GridPattern) this.Children.RemoveAt(0);
            diagram.RemoveLogical(grid);
            if (!this.Children.Contains(grid)) this.Children.Insert(0, grid);
          }
          if (this.CachesBitmap)  // show bitmap instead of actual GridPattern
            grid.Visibility = Visibility.Collapsed;
          else
            grid.Visibility = Visibility.Visible;
        }
      } else {  // or else make sure any grid is not visible
        ClearBackground();
        if (this.Children.Count > 0) {
          GridPattern g = this.Children[0] as GridPattern;
          if (g != null) g.Visibility = Visibility.Collapsed;
        }
        // but don't set diagram.GridPattern to null
      }
    }

    internal void InvalidateGrid() {
      Diagram diagram = this.Diagram;
      if (diagram != null) {
        GridPattern grid = diagram.GridPattern;
        if (grid != null) grid.InvalidateGrid();
      }
      UpdateScrollTransform();  // this calls UpdateBackground and eventually GridPattern.DoUpdateBackgroundGrid
    }

    internal void CancelUpdate() {
      Diagram diagram = this.Diagram;
      if (diagram != null) {
        GridPattern grid = diagram.GridPattern;
        if (grid != null) grid.CancelUpdate();
      }
      StopAutoScroll();
    }

    internal bool CachesBitmap {
      get { return _CachesBitmap; }
      set {
        if (_CachesBitmap != value) {
          _CachesBitmap = value;
          InvalidateGrid();
        }
      }
    }
    private bool _CachesBitmap;

    private readonly SolidColorBrush TransparentBackground = new SolidColorBrush(Colors.Transparent);

    internal void ClearBackground() {
      if (this.Background != TransparentBackground) {
        this.Background = TransparentBackground; // to get background mouse events
      }
    }

    /// <summary>
    /// Create the <see cref="GridPattern"/> for the whole diagram,
    /// using the <see cref="Northwoods.GoXam.Diagram.GridPatternTemplate"/> data template.
    /// </summary>
    /// <returns>the unbound <see cref="Node"/> holding the <see cref="GridPattern"/></returns>
    /// <remarks>
    /// If <see cref="Northwoods.GoXam.Diagram.GridPatternTemplate"/> is null, this uses a default template.
    /// </remarks>
    protected virtual GridPattern CreateBackgroundGridPattern() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      DataTemplate template = diagram.GridPatternTemplate;
      if (template == null) template = Diagram.FindDefault<DataTemplate>("DefaultBackgroundGridTemplate");
      if (template == null) return null;
      DependencyObject expansion = template.LoadContent();
      GridPattern grid = expansion as GridPattern;
      if (grid == null) Diagram.Error("Diagram.CreateBackgroundGridPattern did not return a GridPattern, but instead returned: " + (expansion != null ? expansion.ToString() : "null"));
      return grid;
    }


    // IScrollInfo properties and methods
    // The Offset properties range from zero to Extent - Viewport, in control (DIP) units

    /// <summary>
    /// Gets the horizontal offset of the diagram's parts.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    /// <value>
    /// This normally ranges from zero to <see cref="ExtentWidth"/> - <see cref="ViewportWidth"/>,
    /// in control (device independent pixel) units.
    /// </value>
    public double HorizontalOffset {
      get {
        return Math.Floor((this.Position.X-this.AlignmentDiagramBounds.X)*this.Scale);
      }
    }

    /// <summary>
    /// Change the value of <see cref="HorizontalOffset"/> by changing the value of <see cref="Position"/>.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    /// <param name="offset"></param>
    public void SetHorizontalOffset(double offset) {
      this.DisableOffscreen = true;  // disable removing nodes offscreen
      InternalSetHorizontalOffset(offset);
      this.DisableOffscreen = false;
    }

    internal void InternalSetHorizontalOffset(double offset) {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram != null && !diagram.AllowScroll) return;  //?? wrong place to check for AllowScroll
      double v = offset;
      if (Double.IsNaN(v) || v < 0) {
        v = 0;
      } else {
        double extra = this.ExtentWidth-this.ViewportWidth;
        if (v > extra) v = extra;
      }
      Point pos = this.Position;
      Point newpos = new Point(Math.Floor(v/this.Scale+this.AlignmentDiagramBounds.X), pos.Y);
      this.Position = newpos;
    }

    /// <summary>
    /// Gets the vertical offset of the diagram's parts.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    /// <value>
    /// This normally ranges from zero to <see cref="ExtentHeight"/> - <see cref="ViewportHeight"/>,
    /// in control (device independent pixel) units.
    /// </value>
    public double VerticalOffset {
      get {
        return Math.Floor((this.Position.Y-this.AlignmentDiagramBounds.Y)*this.Scale);
      }
    }

    /// <summary>
    /// Change the value of <see cref="VerticalOffset"/> by changing the value of <see cref="Position"/>.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    /// <param name="offset"></param>
    public void SetVerticalOffset(double offset) {
      this.DisableOffscreen = true;  // disable removing nodes offscreen
      InternalSetVerticalOffset(offset);
      this.DisableOffscreen = false;
    }

    internal void InternalSetVerticalOffset(double offset) {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram != null && !diagram.AllowScroll) return;  //?? wrong place to check for AllowScroll
      double v = offset;
      if (Double.IsNaN(v) || v < 0) {
        v = 0;
      } else {
        double extra = this.ExtentHeight-this.ViewportHeight;
        if (v > extra) v = extra;
      }
      Point pos = this.Position;
      Point newpos = new Point(pos.X, Math.Floor(v/this.Scale+this.AlignmentDiagramBounds.Y));
      //Diagram.Debug(Diagram.Str(pos) + " --> " + Diagram.Str(newpos));
      this.Position = newpos;
    }

    /// <summary>
    /// Get the horizontal size of the visible part of this panel,
    /// in control (device independent pixel) units.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public double ViewportWidth {
      get { return this.ActualWidth; }
    }

    /// <summary>
    /// Get the vertical size of the visible part of this panel,
    /// in control (device independent pixel) units.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public double ViewportHeight {
      get { return this.ActualHeight; }
    }

    /// <summary>
    /// Get the horizontal size of the scrollable part of this diagram,
    /// in control (device independent pixel) units.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public double ExtentWidth {
      get {
        Rect b = this.AlignmentDiagramBounds;
        return Math.Max(this.ViewportWidth, Math.Floor(b.Width*this.Scale));
      }
    }

    /// <summary>
    /// Get the vertical size of the scrollable part of this diagram,
    /// in control (device independent pixel) units.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public double ExtentHeight {
      get {
        Rect b = this.AlignmentDiagramBounds;
        return Math.Max(this.ViewportHeight, Math.Floor(b.Height*this.Scale));
      }
    }


    // not part of IScrollInfo

    /// <summary>
    /// Gets the visible part of this panel, in model coordinates.
    /// </summary>
    /// <value>
    /// This is a combination of <see cref="Position"/>, <see cref="ViewportWidth"/>,
    /// <see cref="ViewportHeight"/>, and <see cref="Scale"/>.
    /// </value>
    public Rect ViewportBounds {
      get {
        Point pos = this.Position;
        double sc = this.Scale;
        return new Rect(pos.X, pos.Y, this.ViewportWidth/sc, this.ViewportHeight/sc);
      }
    }


    /// <summary>
    /// Scroll the diagram so that a given element (<paramref name="visual"/>)
    /// is in the viewport.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    /// <param name="visual"></param>
    /// <param name="rectangle">
    /// The region of the element to bring into view.
    /// This value is normally <c>Rect.Empty</c>,
    /// meaning as much of the whole visual element as will fit into the viewport.
    /// </param>
    /// <returns>the result of calling <see cref="DoMakeVisible"/></returns>
    /// <remarks>
    /// If the <see cref="Part"/> is already visible in the viewport, the panel is not scrolled.
    /// If you always want to scroll the panel so that a part is centered,
    /// even when the part is already visible but not centered in the viewport,
    /// call <see cref="CenterPart(Part, Action)"/>.
    /// </remarks>
    public Rect MakeVisible(

        UIElement visual,



        Rect rectangle) {
      if (visual == null) return Rect.Empty;
      VerifyAccess();
      return DoMakeVisible(visual as UIElement, rectangle);
    }

    /// <summary>
    /// This overridable method changes the <see cref="Position"/> so that the
    /// given element <paramref name="elt"/> becomes visible in the viewport.
    /// </summary>
    /// <param name="elt">
    /// a <c>UIElement</c> that is in the visual tree of this panel
    /// </param>
    /// <param name="rectangle">
    /// The region of the element to bring into view;
    /// if this is empty, this defaults to the element's parent <see cref="Part"/>'s Bounds.
    /// </param>
    /// <returns></returns>
    protected virtual Rect DoMakeVisible(UIElement elt, Rect rectangle) {
      Part part = Diagram.FindAncestorOrSelf<Part>(elt);
      if (part == null) return Rect.Empty;
      // get the bounds of the Part
      Rect r = part.GetElementBounds(part.VisualElement);  // support rotation of whole node
      // if a rectangle is requested, translate it to the part's coordinates
      if (!rectangle.IsEmpty) {
        Rect t = rectangle;
        r.X += t.X;
        r.Y += t.Y;
        r.Width = t.Width;
        r.Height = t.Height;
      }
      // get the bounds of the visible extent of the view, in model coordinates
      Rect v = this.ViewportBounds;
      // see if the requested rectangle already fits in the view
      if (r.X >= v.X && r.Y >= v.Y && r.Right <= v.Right && r.Bottom <= v.Bottom) return r;
      // otherwise try to center the rectangle
      SetScaleAndPosition(this.Scale, new Point(r.X + r.Width/2 - v.Width/2, r.Y + r.Height/2 - v.Height/2), this.ZoomTime, null);
      // in case the rectangle doesn't completely fit in the view, clip it
      r.Intersect(this.ViewportBounds);
      return r;
    }

    /// <summary>
    /// Center a <see cref="Part"/>'s bounds within this viewport.
    /// </summary>
    /// <param name="part"></param>
    /// <remarks>
    /// This just calls <see cref="CenterPart(Part, Action)"/> with no after-Action.
    /// </remarks>
    public void CenterPart(Part part) {
      CenterPart(part, null);
    }

    /// <summary>
    /// Center a <see cref="Part"/>'s bounds within this viewport.
    /// </summary>
    /// <param name="part"></param>
    /// <param name="afterwards">an <c>Action</c> to be performed after the animation is finished; may be null</param>
    /// <remarks>
    /// <para>
    /// The <see cref="HorizontalContentAlignment"/> and <see cref="VerticalContentAlignment"/>
    /// properties need to be "Stretch" if the <see cref="DiagramBounds"/> are smaller than the viewport.
    /// But in any case, the <paramref name="part"/> might not be centered if the panel
    /// cannot be scrolled far enough.
    /// </para>
    /// <para>
    /// If you just want to make sure the panel is scrolled so that a part is visible,
    /// to avoid unnecessary scrolling when the part is already visible,
    /// call <see cref="MakeVisible"/>.
    /// </para>
    /// </remarks>
    public void CenterPart(Part part, Action afterwards) {
      if (part == null) return;
      Rect r = part.GetElementBounds(part.VisualElement);  // support rotation of whole node
      if (Double.IsNaN(r.X) || Double.IsNaN(r.Y)) return;
      Rect v = this.ViewportBounds;
      SetScaleAndPosition(this.Scale, new Point(r.X+r.Width/2-v.Width/2, r.Y+r.Height/2-v.Height/2), this.ZoomTime, afterwards);
    }

    /// <summary>
    /// Set the <see cref="Scale"/> and <see cref="Position"/> properties with animation.
    /// </summary>
    /// <param name="newscale">the new positive <c>double</c> value for <see cref="Scale"/></param>
    /// <param name="newpos">the new <c>Point</c> value for <see cref="Position"/></param>
    /// <param name="animationtime">a time in milliseconds; if a very short time, no animation occurs and the properties are simply set</param>
    /// <param name="act">an <c>Action</c> to perform after the animation is finished; null to do nothing afterwards</param>
    public void SetScaleAndPosition(double newscale, Point newpos, int animationtime, Action act) {
      if (this.IsAnimating) {
        CancelStory(null, null);
        this.IsAnimating = false;
        Diagram.InvokeLater(this, () => SetScaleAndPosition(newscale, newpos, animationtime, act));
        return;
      }

      // maybe a no-op
      double oldscale = this.Scale;
      Point oldpos = this.Position;
      if (oldscale == newscale && oldpos == newpos) {
        if (act != null) act();  // remember to perform the Action, even if there's no animation
        return;
      }

      // set the final, perhaps limited, values
      this.Scale = newscale;
      if (!Double.IsNaN(newpos.X) && !Double.IsNaN(newpos.Y)) this.Position = newpos;

      //Diagram.Debug(oldscale.ToString() + " --> " + newscale.ToString() + ": " + this.Scale.ToString() + "   " + Diagram.Str(oldpos) + " --> " + Diagram.Str(newpos) + ": " + Diagram.Str(this.Position) + Diagram.Str((Point)ReadLocalValue(PositionProperty)));

      // get the actual values, in case they were limited
      newscale = this.Scale;
      newpos = this.Position;

      // avoid animations that won't be very visible
      if (animationtime <= 10 ||
          Math.Abs(newscale-oldscale) < 0.02 && Math.Abs(newpos.X-oldpos.X) < 4 && Math.Abs(newpos.Y-oldpos.Y) < 4) {
        if (act != null) act();  // remember to perform the Action, even if there's no animation
        return;
      }

      // set up the two animations in a storyboard
      Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, animationtime));

      DoubleAnimation da = new DoubleAnimation();
      da.From = oldscale;
      da.Duration = duration;
      Storyboard.SetTarget(da, this);
      Storyboard.SetTargetProperty(da, new PropertyPath(DiagramPanel.ScaleProperty));

      PointAnimation pa = new PointAnimation();
      pa.From = oldpos;
      pa.Duration = duration;
      Storyboard.SetTarget(pa, this);
      Storyboard.SetTargetProperty(pa, new PropertyPath(DiagramPanel.PositionProperty));

      Storyboard story = new Storyboard();
      story.Children.Add(da);
      story.Children.Add(pa);
      story.Completed += CancelStory;
      story.Duration = duration;
      story.FillBehavior = FillBehavior.Stop;
      this.CurrentStory = story;
      this.CurrentAction = act;
      this.IsAnimating = true;
      story.Begin();
    }

    private void CancelStory(Object sender, EventArgs e) {
      this.IsAnimating = false;

      if (this.CurrentStory != null) {
        this.CurrentStory.Completed -= CancelStory;
        this.CurrentStory.Stop();




        if (this.CurrentAction != null) this.CurrentAction();
        this.CurrentAction = null;
      }
      this.CurrentStory = null;
    }

    private Storyboard CurrentStory { get; set; }
    private Action CurrentAction { get; set; }

    internal bool IsAnimating {
      get { return _IsAnimating; }
      set {
        if (_IsAnimating != value) {
          bool old = _IsAnimating;
          _IsAnimating = value;
          RaiseAnimatingChanged(this, value);
        }
      }
    }
    private bool _IsAnimating;

    /// <summary>
    /// Increases the <see cref="VerticalOffset"/> by <see cref="ScrollVerticalLineChange"/>.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public void LineDown() { InternalSetVerticalOffset(this.VerticalOffset + this.ScrollVerticalLineChange); }

    /// <summary>
    /// Decreases the <see cref="VerticalOffset"/> by <see cref="ScrollVerticalLineChange"/>.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public void LineUp() { InternalSetVerticalOffset(this.VerticalOffset - this.ScrollVerticalLineChange); }

    /// <summary>
    /// Increases the <see cref="HorizontalOffset"/> by <see cref="ScrollHorizontalLineChange"/>.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public void LineRight() { InternalSetHorizontalOffset(this.HorizontalOffset + this.ScrollHorizontalLineChange); }

    /// <summary>
    /// Decreases the <see cref="HorizontalOffset"/> by <see cref="ScrollHorizontalLineChange"/>.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public void LineLeft() { InternalSetHorizontalOffset(this.HorizontalOffset - this.ScrollHorizontalLineChange); }

    /// <summary>
    /// Increases the <see cref="VerticalOffset"/> by <see cref="ViewportHeight"/>.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public void PageDown() { InternalSetVerticalOffset(this.VerticalOffset + this.ViewportHeight); }

    /// <summary>
    /// Decreases the <see cref="VerticalOffset"/> by <see cref="ViewportHeight"/>.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public void PageUp() { InternalSetVerticalOffset(this.VerticalOffset - this.ViewportHeight); }

    /// <summary>
    /// Increases the <see cref="HorizontalOffset"/> by <see cref="ViewportWidth"/>.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public void PageRight() { InternalSetHorizontalOffset(this.HorizontalOffset + this.ViewportWidth); }

    /// <summary>
    /// Decreases the <see cref="HorizontalOffset"/> by <see cref="ViewportWidth"/>.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public void PageLeft() { InternalSetHorizontalOffset(this.HorizontalOffset - this.ViewportWidth); }

    /// <summary>
    /// Increases the <see cref="VerticalOffset"/> by <see cref="ScrollVerticalLineChange"/> times the number of lines to scroll for mouse wheel rotation.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public void MouseWheelDown() { SetVerticalOffset(this.VerticalOffset + SystemParameters.WheelScrollLines * this.ScrollVerticalLineChange); }

    /// <summary>
    /// Decreases the <see cref="VerticalOffset"/> by <see cref="ScrollVerticalLineChange"/> times the number of lines to scroll for mouse wheel rotation.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public void MouseWheelUp() { SetVerticalOffset(this.VerticalOffset - SystemParameters.WheelScrollLines * this.ScrollVerticalLineChange); }

    /// <summary>
    /// Increases the <see cref="HorizontalOffset"/> by <see cref="ScrollHorizontalLineChange"/> times the number of lines to scroll for mouse wheel rotation.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public void MouseWheelRight() { SetHorizontalOffset(this.HorizontalOffset + SystemParameters.WheelScrollLines * this.ScrollHorizontalLineChange); }

    /// <summary>
    /// Decreases the <see cref="HorizontalOffset"/> by <see cref="ScrollHorizontalLineChange"/> times the number of lines to scroll for mouse wheel rotation.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    public void MouseWheelLeft() { SetHorizontalOffset(this.HorizontalOffset - SystemParameters.WheelScrollLines * this.ScrollHorizontalLineChange); }

    // relationship to the read-only property: Control.HandlesScrolling?

    /// <summary>
    /// Identifies the <see cref="CanHorizontallyScroll"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CanHorizontallyScrollProperty;
    /// <summary>
    /// Gets or sets whether the user can scroll horizontally.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    public bool CanHorizontallyScroll {
      get { return (bool)GetValue(CanHorizontallyScrollProperty); }
      set { SetValue(CanHorizontallyScrollProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CanVerticallyScroll"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CanVerticallyScrollProperty;
    /// <summary>
    /// Gets or sets whether the user can scroll vertically.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    public bool CanVerticallyScroll {
      get { return (bool)GetValue(CanVerticallyScrollProperty); }
      set { SetValue(CanVerticallyScrollProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ScrollOwner"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ScrollOwnerProperty;
    /// <summary>
    /// Gets or sets the <c>ScrollViewer</c> that lets the user scroll this panel.
    /// (Implements <c>IScrollInfo</c>.)
    /// </summary>
    /// <value>
    /// This will be null if there is no <c>ScrollViewer</c> containing this panel.
    /// </value>
    public ScrollViewer ScrollOwner {
      get { return (ScrollViewer)GetValue(ScrollOwnerProperty); }
      set { SetValue(ScrollOwnerProperty, value); }
    }

    // not part of IScrollInfo

    /// <summary>
    /// Identifies the <see cref="ScrollHorizontalLineChange"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ScrollHorizontalLineChangeProperty;
    /// <summary>
    /// Gets or sets the size of a "line" when scrolling horizontally.
    /// </summary>
    /// <value>
    /// The default value is 16.
    /// </value>
    public double ScrollHorizontalLineChange {
      get { return (double)GetValue(ScrollHorizontalLineChangeProperty); }
      set { SetValue(ScrollHorizontalLineChangeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ScrollVerticalLineChange"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ScrollVerticalLineChangeProperty;
    /// <summary>
    /// Gets or sets the size of a "line" when scrolling vertically.
    /// </summary>
    /// <value>
    /// The default value is 16.
    /// </value>
    public double ScrollVerticalLineChange {
      get { return (double)GetValue(ScrollVerticalLineChangeProperty); }
      set { SetValue(ScrollVerticalLineChangeProperty, value); }
    }


    //================================================================================



    private static Diagram FindDiagram(MouseEventArgs e) {
      foreach (UIElement elt in VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(DraggingTool.Root), DraggingTool.Root)) {
        Diagram diagram = elt as Diagram;
        if (diagram != null) return diagram;
      }
      return null;
    }

    private static bool WasLeftButtonDown { get; set; }
    private static bool WasRightButtonDown { get; set; }

    private static bool WasDoubleClick {
      get { return DiagramPanel.ClickState == 3 || DiagramPanel.ClickState == 13; }
    }
    private static int ClickState { get; set; }
    // ClickStates:
    //   0 initial, or after move by more than a couple pixels (drag size), or after double-click
    //   1 first left mouse down
    //   2 left mouse up after left mouse down
    //   3 mouse down soon after mouse up/down without move
    //   11 first right mouse down
    //   12 right mouse up after right mouse down
    //   13 mouse down soon after mouse up/down without move
    private static Point LastClickPoint { get; set; }   // in view/control coords, not in model coords
    private static DateTime LastClickTime { get; set; }

    /// <summary>
    /// Gets or sets whether <see cref="OnMouseLeftButtonDown"/> and <see cref="OnMouseLeftButtonUp"/>
    /// should ignore the mouse button event if it had already been <c>Handled</c>.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    public bool IgnoresHandledEvents { get; set; }

    /// <summary>
    /// Handle a mouse down event.
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>
    /// This sets <see cref="Northwoods.GoXam.Diagram.FirstMousePointInModel"/>,
    /// <see cref="Northwoods.GoXam.Diagram.LastMousePointInModel"/> and <see cref="Northwoods.GoXam.Diagram.LastMouseEventArgs"/>,
    /// and calls <see cref="IDiagramTool.DoMouseDown"/> on the diagram's
    /// <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>.
    /// </remarks>
    protected virtual void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
      DiagramPanel.WasLeftButtonDown = true;
      if (e.Handled && this.IgnoresHandledEvents) return;
      this.GlobalTransform = null;  // just in case the Diagram has been transformed
      Diagram diagram = this.Diagram;
      if (diagram != null) {
        Point pos = e.GetPosition(this);
        //int oldstate = DiagramPanel.ClickState;
        if (DiagramPanel.ClickState == 2 &&
            !IsBeyondDragSize(DiagramPanel.LastClickPoint, pos) &&
            (DateTime.Now - DiagramPanel.LastClickTime).TotalMilliseconds < 250) {
          DiagramPanel.ClickState = 3;  // double-click!
        } else {
          DiagramPanel.ClickState = 1;
        }
        //Diagram.Debug("Down:  " + oldstate.ToString() + " --> " + DiagramPanel.ClickState.ToString() + " " + e.Handled.ToString());
        Point docpos = TransformViewToModel(pos);
        diagram.FirstMousePointInModel = docpos;
        diagram.LastMousePointInModel = docpos;
        diagram.LastMouseEventArgs = e;
        //Diagram.Debug("  Down  " + docpos.ToString() + " control: " + pos.ToString() + " global: " + TransformModelToGlobal(docpos).ToString());
        DraggingTool.Source = null;
        IDiagramTool tool = diagram.CurrentTool;
        if (tool != null) tool.DoMouseDown();
      }
    }

    /// <summary>
    /// Handle a mouse down event.
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>
    /// <para>
    /// This sets <see cref="Northwoods.GoXam.Diagram.FirstMousePointInModel"/>,
    /// <see cref="Northwoods.GoXam.Diagram.LastMousePointInModel"/> and <see cref="Northwoods.GoXam.Diagram.LastMouseEventArgs"/>,
    /// and calls <see cref="IDiagramTool.DoMouseDown"/> on the diagram's
    /// <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>.
    /// </para>
    /// <para>
    /// When <see cref="Northwoods.GoXam.Diagram.ContextMenuEnabled"/> is false, as it is by default,
    /// this also sets <c>e.Handled</c> to true, in order to receive a MouseRightButtonUp event
    /// and to avoid showing the standard "Silverlight" context menu.
    /// However it also disables MouseRightButtonDown events on the <see cref="Diagram"/>,
    /// so any <b>ContextMenu</b> on the <b>Diagram</b> is effectively disabled.
    /// </para>
    /// <para>
    /// When <see cref="Northwoods.GoXam.Diagram.ContextMenuEnabled"/> is true
    /// this method does nothing, and the standard Silverlight context menu behavior occurs.
    /// </para>
    /// </remarks>
    protected virtual void OnMouseRightButtonDown(MouseButtonEventArgs e) {
      DiagramPanel.WasRightButtonDown = true;
      if (e.Handled && this.IgnoresHandledEvents) return;
      this.GlobalTransform = null;  // just in case the Diagram has been transformed
      Diagram diagram = this.Diagram;
      if (diagram != null) {

        if (diagram.ContextMenuEnabled) return;

        e.Handled = true;  // enables MouseRightButtonUp events and disables the standard "Silverlight" context menu
        Point pos = e.GetPosition(this);
        //int oldstate = DiagramPanel.ClickState;
        if (DiagramPanel.ClickState == 12 &&
            !IsBeyondDragSize(DiagramPanel.LastClickPoint, pos) &&
            (DateTime.Now - DiagramPanel.LastClickTime).TotalMilliseconds < 250) {
          DiagramPanel.ClickState = 13;  // double-click!
        } else {
          DiagramPanel.ClickState = 11;
        }
        //Diagram.Debug("RDown  " + oldstate.ToString() + " --> " + DiagramPanel.ClickState.ToString() + " " + e.Handled.ToString());
        Point docpos = TransformViewToModel(pos);
        diagram.FirstMousePointInModel = docpos;
        diagram.LastMousePointInModel = docpos;
        diagram.LastMouseEventArgs = e;
        //Diagram.Debug("RDown: " + docpos.ToString() + " control: " + pos.ToString() + " global: " + TransformModelToGlobal(docpos).ToString());
        DraggingTool.Source = null;
        IDiagramTool tool = diagram.CurrentTool;
        if (tool != null) tool.DoMouseDown();
      }
    }

    /// <summary>
    /// Handle a mouse move event.
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>
    /// This sets <see cref="Northwoods.GoXam.Diagram.LastMousePointInModel"/> and <see cref="Northwoods.GoXam.Diagram.LastMouseEventArgs"/>,
    /// and calls <see cref="IDiagramTool.DoMouseMove"/> on the diagram's
    /// <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>.
    /// </remarks>
    protected virtual void OnMouseMove(MouseEventArgs e) {
      Diagram diagram = this.Diagram;
      if (diagram != null) {
        Point pos = e.GetPosition(this);
        if (IsBeyondDragSize(DiagramPanel.LastClickPoint, pos)) {
          //if (DiagramPanel.ClickState != 0) Diagram.Debug("  Move  " + DiagramPanel.ClickState.ToString() + "  -->  0");
          DiagramPanel.ClickState = 0;
        }
        Point docpos = TransformViewToModel(pos);
        diagram.LastMousePointInModel = docpos;
        diagram.LastMouseEventArgs = e;
        if (SimulatedMouseMove(e, null, new Point())) return;
        IDiagramTool tool = diagram.CurrentTool;
        if (tool != null) tool.DoMouseMove();
      }
    }

    /// <summary>
    /// Handle a mouse up event.
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>
    /// This sets <see cref="Northwoods.GoXam.Diagram.LastMousePointInModel"/> and <see cref="Northwoods.GoXam.Diagram.LastMouseEventArgs"/>,
    /// and calls <see cref="IDiagramTool.DoMouseUp"/> on the diagram's
    /// <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>.
    /// </remarks>
    protected virtual void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
      if (e.Handled && this.IgnoresHandledEvents) {
        DiagramPanel.WasLeftButtonDown = false;
        return;
      }
      Diagram diagram = this.Diagram;
      if (diagram != null) {
        Point pos = e.GetPosition(this);
        Point docpos = TransformViewToModel(pos);
        diagram.LastMousePointInModel = docpos;
        diagram.LastMouseEventArgs = e;
        //Diagram.Debug("Up: " + docpos.ToString() + " control: " + pos.ToString() + " global: " + TransformModelToGlobal(docpos).ToString());
        if (SimulatedMouseUp(e, null, new Point())) {
          DiagramPanel.WasLeftButtonDown = false;
          return;
        }
        IDiagramTool tool = diagram.CurrentTool;
        if (tool != null) tool.DoMouseUp();
        //int oldstate = DiagramPanel.ClickState;
        if (DiagramPanel.ClickState == 1) {  // after a click: remember point and time for next mouse-down to check for double-click
          DiagramPanel.ClickState = 2;
          DiagramPanel.LastClickPoint = pos;
          DiagramPanel.LastClickTime = DateTime.Now;
        } else {
          DiagramPanel.ClickState = 0;
        }
        //Diagram.Debug("  Up  " + oldstate.ToString() + " --> " + DiagramPanel.ClickState.ToString() + " " + e.Handled.ToString());
      }
      DiagramPanel.WasLeftButtonDown = false;
    }

    /// <summary>
    /// Handle a mouse up event.
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>
    /// This sets <see cref="Northwoods.GoXam.Diagram.LastMousePointInModel"/> and <see cref="Northwoods.GoXam.Diagram.LastMouseEventArgs"/>,
    /// and calls <see cref="IDiagramTool.DoMouseUp"/> on the diagram's
    /// <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>.
    /// </remarks>
    protected virtual void OnMouseRightButtonUp(MouseButtonEventArgs e) {
      if (e.Handled && this.IgnoresHandledEvents) {
        DiagramPanel.WasRightButtonDown = false;
        return;
      }
      Diagram diagram = this.Diagram;
      if (diagram != null) {

        if (diagram.ContextMenuEnabled) return;

        // don't set e.Handled = true so that Diagram.MouseRightButtonUp events can be handled
        Point pos = e.GetPosition(this);
        Point docpos = TransformViewToModel(pos);
        diagram.LastMousePointInModel = docpos;
        diagram.LastMouseEventArgs = e;
        //Diagram.Debug("RUp: " + docpos.ToString() + " control: " + pos.ToString() + " global: " + TransformModelToGlobal(docpos).ToString());
        if (SimulatedMouseUp(e, null, new Point())) {
          DiagramPanel.WasRightButtonDown = false;
          return;
        }
        IDiagramTool tool = diagram.CurrentTool;
        if (tool != null) tool.DoMouseUp();
        //int oldstate = DiagramPanel.ClickState;
        if (DiagramPanel.ClickState == 11) {  // after a click: remember point and time for next mouse-down to check for double-click
          DiagramPanel.ClickState = 12;
          DiagramPanel.LastClickPoint = pos;
          DiagramPanel.LastClickTime = DateTime.Now;
        } else {
          DiagramPanel.ClickState = 0;
        }
        //Diagram.Debug("  RUp  " + oldstate.ToString() + " --> " + DiagramPanel.ClickState.ToString() + " " + e.Handled.ToString());
      }
      DiagramPanel.WasRightButtonDown = false;
    }

    /// <summary>
    /// Handle a mouse enter event.
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>
    /// This sets <see cref="Northwoods.GoXam.Diagram.LastMousePointInModel"/> and <see cref="Northwoods.GoXam.Diagram.LastMouseEventArgs"/>.
    /// </remarks>
    protected virtual void OnMouseEnter(MouseEventArgs e) {
      Diagram diagram = this.Diagram;
      if (diagram != null) {
        Point pos = e.GetPosition(this);
        Point docpos = TransformViewToModel(pos);
        diagram.LastMousePointInModel = docpos;
        diagram.LastMouseEventArgs = e;
      }
    }

    /// <summary>
    /// Handle a mouse wheel event.
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>
    /// This sets <see cref="Northwoods.GoXam.Diagram.LastMousePointInModel"/>,
    /// and calls <see cref="IDiagramTool.DoMouseWheel"/> on the diagram's
    /// <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>.
    /// </remarks>
    protected virtual void OnMouseWheel(MouseWheelEventArgs e) {
      Diagram diagram = this.Diagram;
      if (diagram != null) {
        Point pos = e.GetPosition(this);
        Point docpos = TransformViewToModel(pos);
        diagram.LastMousePointInModel = docpos;
        diagram.LastMouseEventArgs = e;
        IDiagramTool tool = diagram.CurrentTool;
        if (tool != null) tool.DoMouseWheel();
      }
    }





























































































































































    /// <summary>
    /// Simulate some mouse events.
    /// </summary>
    /// <param name="modifiers">
    /// a <see cref="GestureModifiers"/> value, some combination of <see cref="GestureModifiers.Control"/>,
    /// <see cref="GestureModifiers.Shift"/>, and <see cref="GestureModifiers.Alt"/>
    /// </param>
    /// <param name="action">
    /// a <see cref="Gesture"/> value, typically <see cref="Gesture.MouseLeftButton"/>;
    /// a value of <see cref="Gesture.None"/> does nothing
    /// </param>
    /// <param name="down">
    /// the Point in model coordinates for the mouse down event;
    /// this value must be within the <see cref="ViewportBounds"/>
    /// </param>
    /// <param name="up">
    /// the Point in model coordinates for the mouse up event;
    /// this value must be within the <see cref="ViewportBounds"/>
    /// </param>
    /// <param name="other">
    /// The <see cref="Northwoods.GoXam.Diagram"/> where the mouse up occurs.
    /// If this is different from this panel's <see cref="Diagram"/>,
    /// a simulated drag-and-drop will occur.
    /// (This does not use real Windows drag-and-drop in WPF.)
    /// This may be null to use this panel's diagram, thus keeping the whole gesture within this one diagram.
    /// </param>
    /// <remarks>
    /// <para>
    /// This method can have the same effect as several calls to "OnMouse..." methods,
    /// without requiring the use of any <b>MouseEventArgs</b>, <b>MouseButtonEventArgs</b>,
    /// or <b>MouseWheelEventArgs</b>.
    /// For the mouse double click gestures, such as <see cref="Gesture.MouseLeftDoubleClick"/>,
    /// this is equivalent to a mouse down, mouse up, mouse down, and a mouse up.
    /// For the mouse button gestures, such as <see cref="Gesture.MouseLeftButton"/>,
    /// this is equivalent to a mouse down, two mouse moves if <paramref name="down"/>
    /// is different than <paramref name="up"/>, and a mouse up.
    /// For the mouse wheel gestures, such as <see cref="Gesture.MouseWheelForward"/>,
    /// this is equivalent to a mouse wheel turn.
    /// </para>
    /// <para>
    /// In all cases the <b>Point</b>s are in model coordinates, not in FrameworkElement coordinates.
    /// The down and up points must be within the current <see cref="ViewportBounds"/>.
    /// </para>
    /// <para>
    /// If you pass a different diagram as the <paramref name="other"/> argument instead of null,
    /// this simulates a drag-and-drop from this diagram to that other diagram.
    /// Even in WPF, the drag-and-drop will be simulated, rather than using real Windows drag-and-drop,
    /// as it would if it did not have permission.
    /// The various methods that take <b>DragEventArgs</b> are not called.
    /// Supplying a different diagram is not supported for wheel and double-click gestures.
    /// </para>
    /// </remarks>
    public void PerformGesture(GestureModifiers modifiers, Gesture action, Point down, Point up, Diagram other) {
      if (action == Gesture.None) return;

      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      if (other == null) other = diagram;

      Rect viewb = this.InflatedViewportBounds;
      if (!Geo.Contains(viewb, down)) Diagram.Error("Simulated mouse down must be within DiagramPanel.ViewportBounds, (" + this.ViewportBounds.ToString() + ") not at: " + down.ToString());

      viewb = other.Panel.InflatedViewportBounds;
      if (!Geo.Contains(viewb, up)) Diagram.Error("Simulated mouse up must be within DiagramPanel.ViewportBounds, (" + this.ViewportBounds.ToString() + ") not at: " + up.ToString());





      try {
        this.CurrentGestureModifiers = modifiers;
        this.CurrentGesture = action;
        diagram.LastMouseEventArgs = null;  // there isn't any MouseEventArgs!
        IDiagramTool tool = diagram.CurrentTool;

        if (action == Gesture.MouseWheelForward || action == Gesture.MouseWheelBackward) {
          if (tool != null) {
            diagram.LastMousePointInModel = down;
            tool.DoMouseWheel();
          }
          return;
        }

        // otherwise a Left or Right mouse button, perhaps double click
        bool dblclick = (action == Gesture.MouseLeftDoubleClick || action == Gesture.MouseRightDoubleClick);
        if (tool != null) {

          DiagramPanel.WasLeftButtonDown = (action == Gesture.MouseLeftButton || action == Gesture.MouseLeftDoubleClick);
          DiagramPanel.WasRightButtonDown = (action == Gesture.MouseRightButton || action == Gesture.MouseRightDoubleClick);
          if (DiagramPanel.WasLeftButtonDown) {
              DiagramPanel.ClickState = 1;
          } else if (DiagramPanel.WasRightButtonDown) {
              DiagramPanel.ClickState = 11;
          }

          diagram.FirstMousePointInModel = down;
          diagram.LastMousePointInModel = down;
          DraggingTool.Source = null;
          tool.DoMouseDown();
          if (dblclick) {
            tool.DoMouseUp();

            if (DiagramPanel.WasLeftButtonDown) {
              DiagramPanel.ClickState = 3;
            } else if (DiagramPanel.WasRightButtonDown) {
              DiagramPanel.ClickState = 13;
            }



            tool.DoMouseDown();
          }
        }

        // perhaps a mouse move
        if (!dblclick && (!Geo.IsApprox(down, up) || diagram != other)) {
          // move the mouse a bit -- might choose 
          tool = diagram.CurrentTool;  // current tool may have changed
          if (tool != null) {
            // make sure the initial mouse move point IsBeyondDragSize
            Point start = new Point(down.X+SystemParameters.MinimumHorizontalDragDistance, down.Y+SystemParameters.MinimumVerticalDragDistance);
            diagram.LastMousePointInModel = start;
            if (!SimulatedMouseMove(null, other, start)) tool.DoMouseMove();
          }

          // move the mouse to the mouse-up point
          tool = diagram.CurrentTool;  // current tool may have changed
          if (tool != null) {

            DiagramPanel.ClickState = 0;

            diagram.LastMousePointInModel = up;
            if (!SimulatedMouseMove(null, other, up)) tool.DoMouseMove();
          }
        }

        // then a mouse up
        tool = diagram.CurrentTool;
        if (tool != null) {
          diagram.LastMousePointInModel = up;
          if (!SimulatedMouseUp(null, other, up)) tool.DoMouseUp();

          DiagramPanel.ClickState = 0;

        }

        DiagramPanel.WasLeftButtonDown = false;
        DiagramPanel.WasRightButtonDown = false;


      } finally {



        this.CurrentGestureModifiers = GestureModifiers.None;
        this.CurrentGesture = Gesture.None;
        ReleaseMouse();  //??? just in case the mouse was captured
      }
    }

    private GestureModifiers CurrentGestureModifiers { get; set; }
    private Gesture CurrentGesture { get; set; }

    /// <summary>
    /// This predicate is true if the mouse button event represents a double-click.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static bool IsDoubleClick(MouseButtonEventArgs e) {

      if (DiagramPanel.WasDoubleClick) return true;
      // also true just before the second mouse down, if it's within the timeout period
      return ((DiagramPanel.ClickState == 2 || DiagramPanel.ClickState == 12) &&
              (DateTime.Now - DiagramPanel.LastClickTime).TotalMilliseconds < 250);



    }

    internal bool IsDoubleClick() {
      //if (this.CurrentGesture != Gesture.None) {
      //  return (this.CurrentGesture == Gesture.MouseLeftDoubleClick ||
      //          this.CurrentGesture == Gesture.MouseRightDoubleClick);
      //}

      if (DiagramPanel.WasDoubleClick) return true;  // also used in WPF
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      return IsDoubleClick(diagram.LastMouseEventArgs as MouseButtonEventArgs);
    }

    internal bool IsLeftButtonDown() {
      if (this.CurrentGesture != Gesture.None) {
        return (this.CurrentGesture == Gesture.MouseLeftButton ||
                this.CurrentGesture == Gesture.MouseLeftDoubleClick);
      }

      Diagram diagram = this.Diagram;
      if (diagram == null) return false;

      return DiagramPanel.WasLeftButtonDown;







    }

    internal bool IsRightButtonDown() {
      if (this.CurrentGesture != Gesture.None) {
        return (this.CurrentGesture == Gesture.MouseRightButton ||
                this.CurrentGesture == Gesture.MouseRightDoubleClick);
      }

      Diagram diagram = this.Diagram;
      if (diagram == null) return false;

      return DiagramPanel.WasRightButtonDown;







    }

    internal int GetWheelDelta() {
      if (this.CurrentGesture != Gesture.None) {
        if (this.CurrentGesture == Gesture.MouseWheelForward) return 1;
        if (this.CurrentGesture == Gesture.MouseWheelBackward) return -1;
        return 0;
      }

      Diagram diagram = this.Diagram;
      if (diagram == null) return 0;
      MouseWheelEventArgs e = diagram.LastMouseEventArgs as MouseWheelEventArgs;
      return (e != null ? e.Delta : 0);
    }

    internal static bool IsBeyondDragSize(Point a, Point b) {
      return (Math.Abs(b.X - a.X) > SystemParameters.MinimumHorizontalDragDistance / 2 ||
              Math.Abs(b.Y - a.Y) > SystemParameters.MinimumVerticalDragDistance / 2);
    }

    internal bool IsControlKeyDown() {
      if (this.CurrentGesture != Gesture.None) {
        return ((this.CurrentGestureModifiers & GestureModifiers.Control) != 0);
      }


      return (Keyboard.Modifiers & ModifierKeys.Control) != 0;



    }

    internal bool IsShiftKeyDown() {
      if (this.CurrentGesture != Gesture.None) {
        return ((this.CurrentGestureModifiers & GestureModifiers.Shift) != 0);
      }


      return (Keyboard.Modifiers & ModifierKeys.Shift) != 0;



    }

    internal bool IsAltKeyDown() {
      if (this.CurrentGesture != Gesture.None) {
        return ((this.CurrentGestureModifiers & GestureModifiers.Alt) != 0);
      }


      return (Keyboard.Modifiers & ModifierKeys.Alt) != 0;



    }

    internal void ReleaseMouse() {

      DiagramPanel.WasLeftButtonDown = false;
      DiagramPanel.WasRightButtonDown = false;

      ReleaseMouseCapture();
    }

    // common methods for simulated drag-and-drop
    private bool SimulatedMouseMove(MouseEventArgs e, Diagram other, Point modelpt) {
      if (DraggingTool.Source != null) {
        Diagram diagram = this.Diagram;
        Diagram curdiag = (e != null ? FindDiagram(e) : other);
        //Diagram.Debug("Found " + (curdiag != null ? curdiag.Name : "(none)") + " " + Diagram.Str(e.GetPosition(DraggingTool.Root)));
        if (curdiag != DraggingTool.CurrentDiagram) {
          if (DraggingTool.CurrentDiagram != null && DraggingTool.CurrentDiagram.DraggingTool != null) {
            DraggingTool.CurrentDiagram.DraggingTool.DoSimulatedDragLeave();
          }
          if (curdiag != null && curdiag.DraggingTool != null) {
            curdiag.DraggingTool.DoSimulatedDragEnter();
          }
        }
        DraggingTool.CurrentDiagram = curdiag;
        if (curdiag == null) return true;
        if (curdiag != diagram) {
          DiagramPanel otherpanel = curdiag.Panel;
          if (otherpanel != null) {
            Point docpos = modelpt;
            if (e != null) {
              Point pos = e.GetPosition(otherpanel);
              docpos = otherpanel.TransformViewToModel(pos);
            }
            curdiag.LastMousePointInModel = docpos;
            curdiag.LastMouseEventArgs = null;
          }
          DraggingTool target = curdiag.DraggingTool;
          if (target != null) target.DoSimulatedDragOver();
          return true;
        }
      }
      return false;
    }

    private bool SimulatedMouseUp(MouseEventArgs e, Diagram other, Point modelpt) {
      if (DraggingTool.Source != null) {
        Diagram diagram = this.Diagram;
        Diagram curdiag = (e != null ? FindDiagram(e) : other);
        //Diagram.Debug("Found up " + (curdiag != null ? curdiag.Name : "(none)") + " " + Diagram.Str(e.GetPosition(DraggingTool.Root)));
        if (curdiag != DraggingTool.CurrentDiagram) {
          if (DraggingTool.CurrentDiagram != null && DraggingTool.CurrentDiagram.DraggingTool != null) {
            DraggingTool.CurrentDiagram.DraggingTool.DoSimulatedDragLeave();
          }
          if (curdiag != null && curdiag.DraggingTool != null) {
            curdiag.DraggingTool.DoSimulatedDragEnter();
          }
        }
        DraggingTool.CurrentDiagram = curdiag;
        if (curdiag == null) {
          DraggingTool.Source.DoCancel();  //?? generalize dropping onto other Controls
          return true;
        }
        if (curdiag != diagram) {
          DiagramPanel otherpanel = curdiag.Panel;
          if (otherpanel != null) {
            Point docpos = modelpt;
            if (e != null) {
              Point pos = e.GetPosition(otherpanel);
              docpos = otherpanel.TransformViewToModel(pos);
            }
            curdiag.LastMousePointInModel = docpos;
            curdiag.LastMouseEventArgs = null;
          }
          DraggingTool target = curdiag.DraggingTool;
          if (target != null) target.DoSimulatedDrop();
          DraggingTool.Source.DoCancel();
          return true;
        }
      }
      return false;
    }
















































































































































    // Bitmaps

    //?? get bitmap of a collection of Parts

    /// <summary>
    /// Generates a bitmap displaying the parts currently visible in the diagram.
    /// </summary>
    /// <returns>a <c>BitmapSource</c>; in Silverlight the contents may not have been rendered until a later time</returns>
    /// <remarks>
    /// This just returns <c>MakeBitmap(new Size(this.ViewportWidth, this.ViewportHeight), 96, this.Position, this.Scale, null)</c>
    /// </remarks>
    public BitmapSource MakeBitmap() {
      return MakeBitmap(new Size(this.ViewportWidth, this.ViewportHeight), 96, this.Position, this.Scale, null);
    }

    /// <summary>
    /// Return a bitmap showing the parts in a particular area, drawn at a given scale.
    /// </summary>
    /// <param name="bmpsize">the size of the resulting bitmap</param>
    /// <param name="dpi">bitmap resolution [ignored in Silverlight]</param>
    /// <param name="viewpos">a <c>Point</c> in model coordinates</param>
    /// <param name="scale">
    /// a value of 1.0 is normal; the value must be a positive number.
    /// Smaller values produce smaller-looking parts.
    /// </param>
    /// <returns>a <c>BitmapSource</c>; in Silverlight the contents may not have been rendered until a later time</returns>
    /// <remarks>
    /// <para>
    /// The diagram must already be visible and fully initialized before this method is able to render anything.
    /// </para>
    /// <para>
    /// In Silverlight the rendering of the contents of the bitmap occurs asynchronously.
    /// Although a <c>BitmapSource</c> is returned immediately, it will not immediately contain the expected bits.
    /// In order to programmatically access the contents of the bitmap,
    /// you should call <see cref="MakeBitmap(Size, double, Point, double, Action{BitmapSource})"/>.
    /// In Silverlight any background GridPattern is not rendered.
    /// </para>
    /// <para>
    /// This just returns <c>MakeBitmap(bmpsize, dpi, viewpos, scale, null)</c>
    /// </para>
    /// </remarks>
    public BitmapSource MakeBitmap(Size bmpsize, double dpi, Point viewpos, double scale) {
      return MakeBitmap(bmpsize, dpi, viewpos, scale, null);
    }

    /// <summary>
    /// Return a bitmap showing the parts in a particular area, drawn at a given scale.
    /// </summary>
    /// <param name="bmpsize">the size of the resulting bitmap, rounded up to the nearest integer</param>
    /// <param name="dpi">bitmap resolution [ignored in Silverlight]</param>
    /// <param name="viewpos">
    /// a <c>Point</c> in model coordinates for the top-left corner of the area to be rendered
    /// </param>
    /// <param name="scale">
    /// a value of 1.0 is normal; the value must be a positive number.
    /// Smaller values produce smaller-looking parts.
    /// </param>
    /// <param name="act">
    /// an <c>Action</c> taking the <c>BitmapSource</c> to be performed later; this may be null
    /// </param>
    /// <returns>a <c>BitmapSource</c>; in Silverlight the contents may not have been rendered until a later time</returns>
    /// <remarks>
    /// <para>
    /// The diagram must already be visible and fully initialized before this method is able to render anything.
    /// </para>
    /// <para>
    /// In Silverlight the rendering of the contents of the bitmap occurs asynchronously.
    /// Although a <c>BitmapSource</c> is returned immediately, it will not immediately contain the expected bits.
    /// You may access the bitmap contents in the <paramref name="act"/> action or thereafter.
    /// In Silverlight any background GridPattern is not rendered.
    /// </para>
    /// </remarks>
    public BitmapSource MakeBitmap(Size bmpsize, double dpi, Point viewpos, double scale, Action<BitmapSource> act) {
      VerifyAccess();
      if (Double.IsNaN(bmpsize.Width) || bmpsize.Width < 1) bmpsize.Width = 1;
      if (Double.IsNaN(bmpsize.Height) || bmpsize.Height < 1) bmpsize.Height = 1;
      if (Double.IsNaN(dpi) || dpi < 1) dpi = 96;
      if (Double.IsNaN(scale) || scale <= 0 || Double.IsInfinity(scale)) scale = 1;

      //??? unable to render grid correctly
      bool ignoregrid = (bmpsize.Width != this.ViewportWidth || bmpsize.Height != this.ViewportHeight || viewpos != this.Position || scale != this.Scale);
      Brush backbrush = this.Background;
      if (this.WritingBitmap <= 0) {
        //Diagram.Debug("MakeBitmap setup");
        this.WritingBitmap = 1;
        this.WasVirtualizing = this.IsVirtualizing;
        // bring everything onscreen, into the visual tree
        UpdateVisuals(new Rect(Double.NegativeInfinity, Double.NegativeInfinity, Double.PositiveInfinity, Double.PositiveInfinity), false);
        this.IsVirtualizing = false;
        // render the background grid
        GridPattern grid = this.Diagram.GridPattern;
        if (grid != null && this.Diagram.GridVisible && !ignoregrid) {
          Size modsz = new Size(bmpsize.Width/scale, bmpsize.Height/scale);
          this.MakeBitmapScale = scale;
          this.MakeBitmapViewport = new Rect(viewpos.X, viewpos.Y, modsz.Width, modsz.Height);
          grid.DoUpdateBackgroundGrid(this, this.MakeBitmapViewport, this.MakeBitmapScale, false);
        }
      } else {
        // upon call to MakeBitmap before previous calls are finished
        this.WritingBitmap++;
      }
      WriteableBitmap bmp = null;
      try {
        bmp = new WriteableBitmap((int)Math.Ceiling(bmpsize.Width), (int)Math.Ceiling(bmpsize.Height));
        // Render/Invalidate calls queued for later
        return bmp;
      } finally {
        Diagram.InvokeLater(this, () => {
          // Render/Invalidate calls delayed so that offscreen Parts can render correctly
          if (bmp == null) return;
          Size modsz = new Size(bmpsize.Width/scale, bmpsize.Height/scale);
          this.MakeBitmapScale = scale;
          this.MakeBitmapViewport = new Rect(viewpos.X, viewpos.Y, modsz.Width, modsz.Height);
          UpdateScrollTransform(viewpos, scale, bmpsize, false);
          GridPattern grid = this.Diagram.GridPattern;
          if (grid != null && ignoregrid) {
            grid.Visibility = Visibility.Collapsed;  // after UpdateScrollTransform
          }
          this.Background = this.Diagram.Background;
          //Measure(modsz);
          //Arrange(new Rect(0, 0, modsz.Width, modsz.Height));
          Point p = TransformModelToView(viewpos);
          bmp.Render(this, new TranslateTransform() { X=-p.X, Y=-p.Y });
          bmp.Invalidate();  //??? how to force completion of rendering
          if (act != null) this.DelayedBitmapActions.Add(() => { act(bmp); });
          this.WritingBitmap--;
          if (this.WritingBitmap <= 0) {
            Diagram.InvokeLater(this, () => {
              //Diagram.Debug("MakeBitmap cleanup " + this.WasVirtualizing.ToString());
              this.MakeBitmapScale = 0;
              this.Background = backbrush;
              InvalidateMeasure();
              UpdateScrollTransform();
              this.IsVirtualizing = this.WasVirtualizing;
              List<Action> acts = this.DelayedBitmapActions.ToList();
              this.DelayedBitmapActions.Clear();
              foreach (Action a in acts) a();
            });
          }
        });
      }






















    }


    private int WritingBitmap { get; set; }
    private bool WasVirtualizing { get; set; }
    private List<Action> DelayedBitmapActions = new List<Action>();



    // printing

    internal DiagramPanel GrabPrintingPanel(Rect viewb, double scale, HashSet<Part> parts) {
      VerifyAccess();
      ReleasePrintingPanel();  // free up any previous printing DiagramPanel

      DiagramPanel panel2 = new DiagramPanel();
      Diagram diagram = this.Diagram;
      this.PrintLayerMap = new Dictionary<Layer, Layer>();
      foreach (UIElement e in this.Children) {
        NodeLayer nlayer = e as NodeLayer;
        if (nlayer != null) {
          if (nlayer.IsTemporary) continue; //?? no Adornments
          if (!nlayer.AllowPrint) continue;
          NodeLayer nlayer2 = new NodeLayer();
          panel2.Children.Add(nlayer2);
          // remember which temporary printing layers correspond to regular diagram layers
          this.PrintLayerMap[nlayer2] = nlayer;
          foreach (Node node in nlayer.Nodes.ToList()) {
            if (!Geo.Intersects(viewb, node.Bounds)) continue;
            if (!node.CanPrint()) continue;
            // if PARTS is a HashSet, only print Nodes in that collection
            if (parts != null && !parts.Contains(node)) continue;
            // temporarily remove from regular diagram/layer and insert into printing layer/panel
            nlayer.InternalRemove(node);
            nlayer2.InternalAdd(node);
          }
        } else {
          LinkLayer llayer = e as LinkLayer;
          if (llayer != null) {
            if (llayer.IsTemporary) continue;
            if (!llayer.AllowPrint) continue;
            LinkLayer llayer2 = new LinkLayer();
            panel2.Children.Add(llayer2);
            this.PrintLayerMap[llayer2] = llayer;
            foreach (Link link in llayer.Links.ToList()) {
              if (!Geo.Intersects(viewb, link.Bounds)) continue;
              if (!link.CanPrint()) continue;
              if (parts != null && !parts.Contains(link)) continue;
              llayer.InternalRemove(link);
              llayer2.InternalAdd(link);
            }
          }
        }
      }
      if (diagram != null && diagram.GridVisible && diagram.GridPattern != null) {
        diagram.GridPattern.PrepareForPrinting(this, viewb, scale);
        this.Children.Remove(diagram.GridPattern);
      }
      this.PrintingPanel = panel2;
      return panel2;
    }

    private DiagramPanel PrintingPanel { get; set; }
    private Dictionary<Layer, Layer> PrintLayerMap { get; set; }

    internal void ReleasePrintingPanel() {
      VerifyAccess();
      if (this.PrintingPanel == null) return;
      foreach (UIElement e in this.PrintingPanel.Children) {
        NodeLayer nlayer2 = e as NodeLayer;
        if (nlayer2 != null) {
          // find original diagram layer corresponding to this temporary printing layer
          NodeLayer nlayer = this.PrintLayerMap[nlayer2] as NodeLayer;
          foreach (Node node in nlayer2.Nodes.ToList()) {
            // restore to original layer
            nlayer2.InternalRemove(node);
            if (nlayer != null) nlayer.InternalAdd(node);
          }
        } else {
          LinkLayer llayer2 = e as LinkLayer;
          if (llayer2 != null) {
            LinkLayer llayer = this.PrintLayerMap[llayer2] as LinkLayer;
            foreach (Link link in llayer2.Links.ToList()) {
              llayer2.InternalRemove(link);
              if (llayer != null) llayer.InternalAdd(link);
            }
          }
        }
      }
      Diagram diagram = this.Diagram;
      if (diagram != null && diagram.GridVisible && diagram.GridPattern != null) {
        Panel canvas = diagram.GridPattern.Parent as Panel;
        if (canvas != null) canvas.Children.Remove(diagram.GridPattern);
        this.Children.Insert(0, diagram.GridPattern);
      }
      this.PrintingPanel = null;
      this.PrintLayerMap = null;
    }


    internal void InvalidateJumpOverLinks(Rect region) {
      foreach (Link l in this.Diagram.Panel.FindPartsIn<Link>(region, SearchFlags.Links, SearchInclusion.Intersects, SearchLayers.Links)) {
        if (l.Route.JumpsOver) {
          l.Route.InvalidateGeometry();
          l.InvalidateVisual(null);
        }
      }
    }

    internal void InvalidateAvoidsNodesLinks(Rect region) {
      foreach (Link l in this.Diagram.Panel.FindPartsIn<Link>(region, SearchFlags.Links, SearchInclusion.Intersects, SearchLayers.Links)) {
        if (l.Route.Routing == LinkRouting.AvoidsNodes) l.Route.InvalidateRoute();
      }
    }

    // for AvoidsNodes:

    // this is not called for Nodes in temporary layers (which includes Tools and Adornments)
    private static /*?? protected virtual */ bool IsAvoidable(Node obj) {
      return obj.Avoidable && !obj.IsLinkLabel;
    }

    private static /*?? protected virtual */ Rect GetAvoidableRectangle(Node obj) {
      Rect b = obj.GetElementBounds(obj.VisualElement);  // support rotation of whole node
      Thickness t = obj.AvoidableMargin;
      Rect v = new Rect(b.X-t.Left, b.Y-t.Top, b.Width+t.Left+t.Right, b.Height+t.Top+t.Bottom);
      //Diagram.Debug("avoidable: " + Diagram.Str(v) + Diagram.Str(obj));
      return v;
    }

    /// <summary>
    /// Returns true if there are any nodes within or
    /// intersecting the given rectangular region.
    /// </summary>
    /// <param name="r">a Rect in model coordinates</param>
    /// <param name="skip">
    /// a predicate that should return true for nodes to be ignored when checking for collisions,
    /// usually the node in the document that you are considering moving;
    /// may be <c>null</c> to consider all non-temporary nodes
    /// </param>
    /// <returns></returns>
    internal /*?? public */ bool IsUnoccupied(Rect r, Predicate<Part> skip) {
      IEnumerable<Node> found = FindPartsIn<Node>(r, SearchFlags.Nodes, SearchInclusion.Intersects, SearchLayers.Nodes);
      if (skip != null)
        return !found.Where(n => !skip(n)).Any();
      else
        return !found.Any();
    }

    //private int GetPositionsCount { get; set; }

    // internal, also used by GoLink.AddOrthoPoints
    internal PositionArray GetPositions(bool clearunoccupied, Link link, Node skip) {
      if (_Positions == null) {
        _Positions = new PositionArray();
      }
      Group subgraph = null;
      if (!_Positions.Invalid) subgraph = (link != null ? link.ContainingSubGraph : null);
      if (_Positions.Invalid || _Positions.SubGraph != subgraph) {
        //Diagram.Debug("GetPositionsArray " + (this.GetPositionsCount++).ToString() + " " + Diagram.Str(link));
        if (subgraph == null) {
          Rect docbounds = ComputeDiagramBounds();
          Geo.Inflate(ref docbounds, 100, 100);
          // make sure the array is big enough, and set all cells to MAX
          _Positions.Initialize(docbounds);
          // now for each object that matters, set the corresponding cell to OCCUPIED
          foreach (Node node in this.Diagram.Nodes) {
            NodeLayer layer = (NodeLayer)node.Layer;
            if (layer == null || layer.Visibility != Visibility.Visible || layer.IsTemporary) continue;
            GetPositions1(node, skip);
          }
        } else {
          IEnumerable<Node> memnodes = link.ContainingSubGraph.MemberNodes.Where(n => n.Visibility == Visibility.Visible).ToList();
          Rect sgbounds = ComputeBounds(memnodes.Cast<Part>());
          if (!sgbounds.IsEmpty) {
            Geo.Inflate(ref sgbounds, 20, 20);
            _Positions.Initialize(sgbounds);
            foreach (Node n in memnodes) {
              GetPositions1(n, skip);
            }
          }
        }
        _Positions.SubGraph = subgraph;
        _Positions.Invalid = false;
      } else if (clearunoccupied) {
        // assume the OCCUPIED cells should be left alone, but clear out
        // any earlier distance calculations
        _Positions.ClearAllUnoccupied();
      }
      return _Positions;
    }

    private void GetPositions1(Node obj, Node skip) {
      if (obj == skip) return;
      if (IsAvoidable(obj)) {
        Rect rect = GetAvoidableRectangle(obj);
        double cellx = _Positions.CellSize.Width;
        double celly = _Positions.CellSize.Height;
        double endx = rect.X+rect.Width;
        double endy = rect.Y+rect.Height;
        for (double i = rect.X; i < endx; i += cellx) {
          for (double j = rect.Y; j < endy; j += celly) {
            _Positions.SetOccupied(i, j);
          }
          _Positions.SetOccupied(i, endy);
        }
        for (double j = rect.Y; j < endy; j += celly) {
          _Positions.SetOccupied(endx, j);
        }
        _Positions.SetOccupied(endx, endy);
      //} else {
      //  Group g = obj as Group;
      //  if (g != null) {
      //    foreach (Node o in g.MemberNodes) {
      //      GetPositions1(o, skip);
      //    }
      //  }
      }
    }

    internal void InvalidatePositionArray(Node obj) {
      if (_Positions != null && !_Positions.Invalid) {
        if (obj == null || IsAvoidable(obj))
          _Positions.Invalid = true;
      }
    }

    private PositionArray _Positions;

    private DiagramLicense _License = null;

    private sealed class DiagramLicense {  // nested class
      public DiagramLicense() {
		  /*
		   * 2011.04.20取消验证
		   */

		  return;
        String msg = DefaultLayer.S1;
        try {
          String compfullname = Assembly.GetExecutingAssembly().FullName;
          String compname = compfullname.Substring(0, compfullname.IndexOf(DefaultLayer.CCOM));

          String key = Diagram.LicenseKey;
          if (key == null || key == "") {
            // don't bother with .LIC file any more
            //key = FindSavedLicenseKey(0, compname);
            //if (key == null || key == "") {
              key = FindSavedLicenseKey(1, compname);
              if (key == null || key == "") {
                key = FindSavedLicenseKey(2, compname);
              }
            //}
          }
          if (key != null && key != "") {
            String assname = null;
            foreach (XmlnsDefinitionAttribute x in Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(XmlnsDefinitionAttribute), true)) {
              if (x.ClrNamespace.Length == 16) assname = x.ClrNamespace;
            }
            msg = DefaultLayer.S2 + DefaultLayer.SSBO + compfullname + DefaultLayer.SSBC;






              msg = DefaultLayer.S4 + key;

              String version = Diagram.VersionName;
              msg += DefaultLayer.S5 + compname + DefaultLayer.SSP + version + DefaultLayer.SSP + DefaultLayer.SSBO + compfullname + DefaultLayer.SSBC;

              String[] vers = version.Split(new char[] { DefaultLayer.CDOT });
              int major = Int32.Parse(vers[0], NumberFormatInfo.InvariantInfo);
              int minor = Int32.Parse(vers[1], NumberFormatInfo.InvariantInfo);

              String machinename = null;
              String username = null;


              Deployment dep = Deployment.Current;
              if (dep == null) msg += DefaultLayer.S6;
              String appname = dep.EntryPointAssembly;














              // adapted from Codec.Decode
              String decoded = "";
              byte[] rand = Encoding.UTF8.GetBytes(assname);

              ICryptoTransform transform = new AesManaged().CreateDecryptor(rand, rand);



              using (MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(key))) {
                byte[] rawbytes = new byte[key.Length+1];
                using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Read)) {
                  cs.Read(rawbytes, 0, rawbytes.Length);
                  int i = 0;
                  while (i < rawbytes.Length && rawbytes[i] != 0) i++;
                  decoded = Encoding.UTF8.GetString(rawbytes, 0, i);
                }
              }

              // adapted from LicenseInfo.Parse
              String[] a = decoded.Split(new char[] { DefaultLayer.CVB });
              if (a.Length > 7) {
                String appn = a[0];
                String comp = a[3];
                int maj = Int32.Parse(a[4], NumberFormatInfo.InvariantInfo);
                int min = Int32.Parse(a[5], NumberFormatInfo.InvariantInfo);
                String machine = a[6];
                String user = a[7];

                if (comp.Equals(compname, StringComparison.OrdinalIgnoreCase) && 
                    (major < maj || (major == maj && minor <= min)) &&
                    (appn.Equals(appname, StringComparison.OrdinalIgnoreCase) ||
                     machine.Equals(machinename, StringComparison.OrdinalIgnoreCase) ||
                     user.Equals(username, StringComparison.OrdinalIgnoreCase))) {
                  return;
                }

                msg += DefaultLayer.S8 + comp + DefaultLayer.SSP + maj.ToString(CultureInfo.InvariantCulture) + DefaultLayer.SDOT + min.ToString(CultureInfo.InvariantCulture);
                msg += DefaultLayer.S9 + appn;
                if (!comp.Equals(compname, StringComparison.OrdinalIgnoreCase)) {
                  msg += DefaultLayer.S10 + compname;
                }
                if (!(major < maj || (major == maj && minor <= min))) {
                  msg += DefaultLayer.S11 + major.ToString(CultureInfo.InvariantCulture) + DefaultLayer.SDOT + minor.ToString(CultureInfo.InvariantCulture);
                }
                if (!appn.Equals(appname, StringComparison.OrdinalIgnoreCase)) {
                  msg += DefaultLayer.S12 + appname;
                }
              }



          }
        } catch (Exception) {
        }
        String msg2 = (msg.IndexOf(DefaultLayer.S13, StringComparison.OrdinalIgnoreCase) > 0 ? DefaultLayer.SNL : DefaultLayer.S14) + msg + DefaultLayer.S15 + DefaultLayer.S16;
        Diagram.Trace(msg2);
        throw new InvalidOperationException(msg2);
      }

      private static String FindSavedLicenseKey(int where, String compname) {
        String line = null;
        //??? look for design-time storage of a generalized license key





















        if (line != null) return line.Trim();
        return "";
      }
    }  // end of DiagramLicense
  }  // end of DiagramPanel


  /// <summary>
  /// This enumeration is used to control what kinds of parts are found,
  /// in calls to <see cref="DiagramPanel.FindPartsNear"/> and <see cref="DiagramPanel.FindPartsIn"/>.
  /// </summary>
  [Flags]
  public enum SearchFlags {
    /// <summary>
    /// Include no parts (not very useful).
    /// </summary>
    None = 0,
    /// <summary>
    /// Include all nodes that are not groups or link label nodes.
    /// </summary>
    SimpleNode = 1,
    /// <summary>
    /// Include all nodes that are groups.
    /// </summary>
    Group = 2,
    /// <summary>
    /// Include all nodes that are link labels.
    /// </summary>
    Label = 4,
    /// <summary>
    /// Include all kinds of nodes, including groups and link label nodes.
    /// </summary>
    Nodes = SimpleNode | Group | Label,
    /// <summary>
    /// Include all links.
    /// </summary>
    Links = 8,
    /// <summary>
    /// Include all parts.
    /// </summary>
    Parts = Nodes | Links,
    /// <summary>
    /// Also require parts to be selectable.
    /// </summary>
    Selectable = 16,
    /// <summary>
    /// Include all selectable parts.
    /// </summary>
    SelectableParts = Nodes | Links | Selectable
  }

  /// <summary>
  /// This enumeration is used to control how parts may overlap the given geometry,
  /// in calls to <see cref="DiagramPanel.FindPartsNear"/> and <see cref="DiagramPanel.FindPartsIn"/>.
  /// </summary>
  public enum SearchInclusion {
    /// <summary>
    /// To match, a part must be completely inside the given geometry.
    /// </summary>
    Inside,
    /// <summary>
    /// To match, a part may overlap the given geometry completely or only partly.
    /// </summary>
    Intersects
  }

  /// <summary>
  /// This enumeration is used to control which layers are considered,
  /// in calls to <see cref="DiagramPanel.FindPartsNear"/>, <see cref="DiagramPanel.FindPartsIn"/>,
  /// <see cref="DiagramPanel.FindElementAt"/>, and <see cref="DiagramPanel.FindElementsAt"/>.
  /// </summary>
  [Flags]
  public enum SearchLayers {
    /// <summary>
    /// Include no layers (not very useful).
    /// </summary>
    None = 0,
    /// <summary>
    /// Include all regular <see cref="NodeLayer"/>s, but not any layers that are <see cref="Layer.IsTemporary"/>.
    /// </summary>
    Nodes = 1,  // but not IsTemporary
    /// <summary>
    /// Include all regular <see cref="LinkLayer"/>s, but not any layers that are <see cref="Layer.IsTemporary"/>.
    /// </summary>
    Links=2,  // but not IsTemporary
    /// <summary>
    /// Include all regular <see cref="Layer"/>s, holding nodes or links, but not any layers that are <see cref="Layer.IsTemporary"/>.
    /// </summary>
    Parts=Nodes | Links,  // but not IsTemporary layers
    /// <summary>
    /// Also include temporary layers.
    /// </summary>
    Temporary = 4,  // only IsTemporary layers
    /// <summary>
    /// Include all layers.
    /// </summary>
    All = Nodes | Links | Temporary
  }


  /// <summary>
  /// This lists the policies used to govern if and how the <see cref="DiagramPanel"/>'s
  /// <see cref="DiagramPanel.Scale"/> is set automatically.
  /// </summary>
  public enum StretchPolicy {
    /// <summary>
    /// The default policy, where the <see cref="DiagramPanel.HorizontalContentAlignment"/>
    /// and <see cref="DiagramPanel.VerticalContentAlignment"/> control the effective positioning
    /// of the parts in the panel when the <see cref="DiagramPanel.DiagramBounds"/> fits
    /// within the panel's viewport.
    /// The <see cref="DiagramPanel.Scale"/> will not change.
    /// </summary>
    None = 0,
    /// <summary>
    /// This is the same as <see cref="None"/>.
    /// </summary>
    Unstretched = 0,
    /// <summary>
    /// The <see cref="DiagramPanel.Position"/> and <see cref="DiagramPanel.Scale"/> are
    /// automatically adjusted so that the <see cref="DiagramPanel.DiagramBounds"/> is fully
    /// visible in the panel.
    /// The new <see cref="DiagramPanel.Scale"/> will not be greater than 1.
    /// </summary>
    Uniform = 1,
    /// <summary>
    /// The <see cref="DiagramPanel.Position"/> and <see cref="DiagramPanel.Scale"/> are
    /// automatically adjusted so that one of the <see cref="DiagramPanel.DiagramBounds"/>'s
    /// dimensions is fully visible in the panel.
    /// The new <see cref="DiagramPanel.Scale"/> will not be greater than 1.
    /// </summary>
    UniformToFill = 2
  }

  /// <summary>
  /// This enumerates the kinds of gestures that are implemented by <see cref="DiagramPanel.PerformGesture"/>.
  /// </summary>
  public enum Gesture {
    /// <summary>
    /// No simulated mouse gesture is occuring.
    /// </summary>
    None=0,
    /// <summary>
    /// A simulated mouse gesture using the left button, a drag or a click.
    /// </summary>
    MouseLeftButton,
    /// <summary>
    /// A simulated mouse double-click gesture using the left button.
    /// </summary>
    MouseLeftDoubleClick,
    /// <summary>
    /// A simulated mouse gesture using the right button, a drag or a click.
    /// </summary>
    MouseRightButton,
    /// <summary>
    /// A simulated mouse double-click gesture using the right button.
    /// </summary>
    MouseRightDoubleClick,
    /// <summary>
    /// A simulated mouse wheel gesture, rolling forward.
    /// </summary>
    MouseWheelForward,
    /// <summary>
    /// A simulated mouse wheel gesture, rolling backward.
    /// </summary>
    MouseWheelBackward
  }

  /// <summary>
  /// These gesture modifier flags are used by <see cref="DiagramPanel.PerformGesture"/>,
  /// as if the corresponding keys were held down during the gesture.
  /// </summary>
  [Flags]
  public enum GestureModifiers {
    /// <summary>
    /// No Control/Shift/Alt modifier for the simulated mouse gesture.
    /// </summary>
    None=0,
    /// <summary>
    /// As if the Control key is being held down during the simulated mouse gesture.
    /// </summary>
    Control = 1,
    /// <summary>
    /// As if the Shift key is being held down during the simulated mouse gesture.
    /// </summary>
    Shift=2,
    /// <summary>
    /// As if the Alt key is being held down during the simulated mouse gesture.
    /// </summary>
    Alt=4
  }
}
