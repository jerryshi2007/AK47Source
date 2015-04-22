
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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Northwoods.GoXam.Layout;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;

namespace Northwoods.GoXam {

  /// <summary>
  /// A <c>Part</c> is an item in a <see cref="Northwoods.GoXam.Diagram"/>'s <see cref="Northwoods.GoXam.Diagram.Panel"/>'s <see cref="Northwoods.GoXam.Layer"/>,
  /// corresponding to data representing a node or a link.
  /// </summary>
  /// <remarks>
  /// <para>
  /// There are two classes inheriting from this abstract class: <see cref="Node"/> and <see cref="Link"/>.
  /// </para>
  /// <para>
  /// Each <c>Part</c> can only be a child element of a <see cref="Northwoods.GoXam.Layer"/>: either a <see cref="NodeLayer"/>
  /// or a <see cref="LinkLayer"/>.  Parts cannot be nested in the visual tree.  However, a subclass of node,
  /// <see cref="Group"/>, supports the logical nesting of nodes (and links) as members of a group.
  /// You can specify which layer a part should be in by setting its <see cref="LayerName"/> property.
  /// </para>
  /// <para>
  /// A <c>Part</c> is typically created by the <see cref="Northwoods.GoXam.Diagram"/>'s <see cref="Northwoods.GoXam.Diagram.PartManager"/>
  /// in response to the presence or addition of some data to the diagram's <see cref="Northwoods.GoXam.Diagram.Model"/>.
  /// The part manager creates the node or link, sets its <c>Content</c> property to the data (indirectly)
  /// and its <c>ContentTemplate</c> property to a <c>DataTemplate</c> based on the type of part and
  /// the part's <see cref="Category"/>.
  /// The <c>FrameworkElement</c> that results from the application of the template to the data
  /// is accessible as the <see cref="VisualElement"/> property.
  /// </para>
  /// <para>
  /// Each part has a <see cref="Bounds"/> property that describes its position and size in model coordinates.
  /// This property is read-only.
  /// One can change the bounds of a <see cref="Node"/> by setting its
  /// <see cref="Node.Location"/> or <see cref="Node.Position"/> property.
  /// </para>
  /// <para>
  /// A part can be selected or de-selected by setting its <see cref="IsSelected"/> property.
  /// </para>
  /// <para>
  /// Most of the properties pertinent to parts are attached dependency properties rather than regular
  /// dependency properties.
  /// Many are only to be used on the <see cref="VisualElement"/>, not on the part itself nor on nested
  /// elements in the part's visual tree.
  /// The reason for using attached properties is to make it easier to data bind those properties
  /// in the data template.
  /// </para>
  /// <para>
  /// An example is the <see cref="LayerNameProperty"/>.  Consider the following data template for nodes:
  /// <code>
  ///   &lt;DataTemplate x:Key="ExampleTemplate"&gt;
  ///     &lt;Border Background="White" BorderBrush="Blue" BorderThickness="2" CornerRadius="3" Padding="2,0,2,0"
  ///                go:Node.LayerName="{Binding Path=Data.LayerName}"&gt;
  ///       &lt;TextBlock Text="{Binding Path=Data.Name}" /&gt;
  ///     &lt;/Border&gt;
  ///   &lt;/DataTemplate&gt;
  /// </code>
  /// The example assumes that the node data to which each node is bound has at least two properties:
  /// one named "Name" holding the text to be displayed, and one named "LayerName" specifying the name
  /// of the <see cref="Northwoods.GoXam.Layer"/> that the node should be in.
  /// Note that setting the attribute for the layer name refers to the attached property qualified
  /// by the class name ("Node" in this case, in the XMLNS namespace referenced by "go").
  /// </para>
  /// <para>
  /// The <see cref="LayerName"/> CLR property of <see cref="Part"/> gets and sets the value of the
  /// <see cref="LayerNameProperty"/> on the part's <see cref="VisualElement"/>.
  /// Please be aware that such property getters will just return the default value for the property
  /// when the data template has not yet been applied, as will be the case when <see cref="VisualElement"/>
  /// is null.
  /// Such property setters will try applying the template if needed before actually setting
  /// the attached property on the <see cref="VisualElement"/>.
  /// </para>
  /// <para>
  /// Note that the data binding goes indirectly through a property named "Data":
  /// <code>
  ///   &lt;TextBlock Text="{Binding Path=Data.Name}" /&gt;
  /// </code>
  /// This is because the part's <c>Content</c> is not the data itself but a simple data structure
  /// that includes the data as its <see cref="PartManager.PartBinding.Data"/> property.
  /// The other property refers to the <see cref="Part"/> itself.  This permits bindings such as:
  /// <code>
  ///   &lt;DataTrigger Binding="{Binding Path=Part.IsSelected}" Value="True" &gt;
  ///     &lt;Setter Property="Border.BorderBrush" Value="Red" /&gt;
  ///     &lt;Setter Property="go:Node.LayerName" Value="Foreground" /&gt;
  ///   &lt;/DataTrigger&gt;
  /// </code>
  /// This results in the border's brush changing color when the node is selected,
  /// and in the node being moved to the foreground layer when it is selected.
  /// (This requires corresponding changes to the <c>Border</c> element, since the
  /// <c>BorderBrush</c> and <c>go:Node.LayerName</c> properties should not be set
  /// locally if they want to be modified in a style trigger.)
  /// (Also note that this example will not work in Silverlight.)
  /// </para>
  /// <para>
  /// There are many properties, named "...able", that control what operations the user
  /// may perform on this part.  These properties correspond to the same named
  /// properties on <see cref="Northwoods.GoXam.Diagram"/> and <see cref="Northwoods.GoXam.Layer"/> that govern the behavior
  /// for all parts in all layers or for all parts in the given layer.
  /// For example, the <see cref="Copyable"/> property corresponds to the properties
  /// <see cref="Northwoods.GoXam.Diagram.AllowCopy"/> and
  /// <see cref="Northwoods.GoXam.Layer.AllowCopy"/>.
  /// </para>
  /// <para>
  /// For each of these "ability" properties there is a corresponding "Can..." predicate.
  /// For example, the <see cref="CanCopy"/> predicate is false if any of the three
  /// previously named properties is false.
  /// </para>
  /// <para>
  /// As previously mentioned, <see cref="Northwoods.GoXam.Diagram"/> supports the notion of selected parts.
  /// The part class also supports showing visual objects for a part when it gets selected.
  /// These visuals are typically used to show that the part is selected ("selection handles")
  /// or are used to allow the user to manipulate or modify the part with a tool ("tool handles").
  /// These handles are the visual elements of <see cref="Adornment"/> nodes.
  /// The <see cref="UpdateAdornments"/> method is responsible for showing or hiding adornments,
  /// normally depending on whether the part is selected.
  /// </para>
  /// <para>
  /// When the <see cref="SelectionAdorned"/> (attached) property is true, a selected part
  /// automatically gets an <see cref="Adornment"/> node created for it.
  /// Such a node gets a <c>ContentTemplate</c> property that is the value of
  /// <see cref="SelectionAdornmentTemplate"/>.
  /// (If the value is null, a default selection adornment template is used.)
  /// If the node is bound to data, the adornment is also bound to the same data.
  /// </para>
  /// <para>
  /// Tool handles are shown for those tools that need it.
  /// The process of updating adornments for a part will ask each mode-less tool
  /// to <see cref="Northwoods.GoXam.Tool.IDiagramTool.UpdateAdornments"/>.
  /// Most tools might not need special tool handles.  But, for example,
  /// <see cref="Northwoods.GoXam.Tool.ResizingTool"/> naturally will want to
  /// create an adornment with eight resize handles positioned at the corners and
  /// at the middles of the sides of the selected node's visual element,
  /// if the node has <see cref="CanResize"/> returning true.
  /// </para>
  /// <para>
  /// However one does not always want the whole part to get the selection handle
  /// and any tool handles.  Sometimes one wants to emphasize selection by
  /// highlighting a particular element within the part's visual tree.
  /// This can be achieved by setting the <see cref="SelectionElementName"/> property,
  /// and making sure the desired element has the same <c>x:Name</c> attribute value.
  /// For example, the following node template causes resize handles to appear surrounding
  /// the <c>Rectangle</c> element, not around the whole node.
  /// <code>
  ///   &lt;DataTemplate x:Key="ExampleTemplate"&gt;
  ///     &lt;StackPanel go:Node.Resizable="True" go:Node.SelectionElementName="myIcon"&gt;
  ///       &lt;Rectangle x:Name="myIcon" Width="30" Height="30"
  ///                  Fill="LightSalmon" Stroke="Maroon" StrokeThickness="2" /&gt;
  ///       &lt;TextBlock Text="{Binding Path=Data.Name}" HorizontalAlignment="Center" /&gt;
  ///     &lt;/StackPanel&gt;
  ///   &lt;/DataTemplate&gt;
  /// </code>
  /// </para>
  /// <para>
  /// There are some methods for finding and examining particular elements in the part's visual tree:
  /// <see cref="FindDescendant"/>, <see cref="FindNamedDescendant"/>,
  /// <see cref="IsVisibleElement"/>, <see cref="GetRelativeElementPoint"/>,
  /// <see cref="GetElementPoint"/>, and <see cref="GetElementBounds"/>.
  /// </para>
  /// <para>
  /// There are other methods and properties for looking at the relationships between parts:
  /// <see cref="IsTopLevel"/>, <see cref="FindTopLevelPart()"/>, <see cref="IsContainedBy(Part)"/>,
  /// <see cref="ContainingSubGraph"/>, and <see cref="FindCommonContainingSubGraph(Part)"/>.
  /// </para>
  /// <para>
  /// You may not apply any transforms to a <c>Part</c>; that is reserved to GoXam for future use.
  /// </para>
  /// </remarks>
  public abstract class Part : ContentPresenter {

    static Part() {
      // Dependency properties

      // to distinguish different kinds of nodes or links; also used by Adornment to distinguish between tools
      CategoryProperty = DependencyProperty.Register("Category", typeof(String), typeof(Part),
        new FrameworkPropertyMetadata("", OnCategoryChanged));

      // readonly, based on ActualWidth/ActualSize, and for Nodes, their Location

      BoundsProperty = DependencyProperty.Register("Bounds", typeof(Rect), typeof(Part),
        new FrameworkPropertyMetadata(new Rect(Double.NaN, Double.NaN, 0, 0), OnBoundsChanged));
      // internal:
      OffscreenChildSizeProperty = DependencyProperty.RegisterAttached("OffscreenChildSize", typeof(Size), typeof(Node), 
        new PropertyMetadata(Size.Empty));






      // a transient property
      IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(Part),  // if Selectable
        new FrameworkPropertyMetadata(false, OnIsSelectedChanged));

      // a transient DraggingTool property:
      IsDropOntoAcceptedProperty = DependencyProperty.Register("IsDropOntoAccepted", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(false, OnIsDropOntoAcceptedChanged));


      // Attached properties

      // each kind of Layer must have a unique Name in the DiagramPanel
      // must be on the root visual child
      LayerNameProperty = DependencyProperty.RegisterAttached("LayerName", typeof(String), typeof(Part),
        new FrameworkPropertyMetadata("", OnLayerNameChanged));

      // user interaction properties; all must be on the root visual child
      CopyableProperty = DependencyProperty.RegisterAttached("Copyable", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(true, VerifyOnVisualElement<Part>));  // if Selectable
      DeletableProperty = DependencyProperty.RegisterAttached("Deletable", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(true, VerifyOnVisualElement<Part>));  // if Selectable
      EditableProperty = DependencyProperty.RegisterAttached("Editable", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(true, VerifyOnVisualElement<Part>));  // if Selectable
      GroupableProperty = DependencyProperty.RegisterAttached("Groupable", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(true, VerifyOnVisualElement<Part>));  // if Selectable
      MovableProperty = DependencyProperty.RegisterAttached("Movable", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(true, VerifyOnVisualElement<Part>));  // if Selectable
      PrintableProperty = DependencyProperty.RegisterAttached("Printable", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(true, VerifyOnVisualElement<Part>));
      ReshapableProperty = DependencyProperty.RegisterAttached("Reshapable", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(false, VerifyOnVisualElement<Part>));  // if Selectable
      ResizableProperty = DependencyProperty.RegisterAttached("Resizable", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(false, VerifyOnVisualElement<Part>));  // if Selectable
      RotatableProperty = DependencyProperty.RegisterAttached("Rotatable", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(false, VerifyOnVisualElement<Part>));  // if Selectable
      SelectableProperty = DependencyProperty.RegisterAttached("Selectable", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(true, VerifyOnVisualElement<Part>));
      // NB: the UIElement.Visibility property
      VisibleProperty = DependencyProperty.RegisterAttached("Visible", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(true, OnVisibleChanged));

      // must be on the root visual child
      SelectionElementNameProperty = DependencyProperty.RegisterAttached("SelectionElementName", typeof(String), typeof(Part),
        new FrameworkPropertyMetadata("", OnSelectionElementNameChanged));

      // must be on the root visual element
      SelectionAdornedProperty = DependencyProperty.RegisterAttached("SelectionAdorned", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(false, OnSelectionAdornedChanged));
      SelectionAdornmentTemplateProperty = DependencyProperty.RegisterAttached("SelectionAdornmentTemplate", typeof(DataTemplate), typeof(Part),
        new FrameworkPropertyMetadata(null, OnSelectionAdornmentTemplateChanged));
      ResizeAdornmentTemplateProperty = DependencyProperty.RegisterAttached("ResizeAdornmentTemplate", typeof(DataTemplate), typeof(Part),
        new FrameworkPropertyMetadata(null, OnResizeAdornmentTemplateChanged));
      RotateAdornmentTemplateProperty = DependencyProperty.RegisterAttached("RotateAdornmentTemplate", typeof(DataTemplate), typeof(Part),
        new FrameworkPropertyMetadata(null, OnRotateAdornmentTemplateChanged));

      // these text editing properties may be on any TextBlock
      TextEditableProperty = DependencyProperty.RegisterAttached("TextEditable", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(false));
      TextEditAdornmentTemplateProperty = DependencyProperty.RegisterAttached("TextEditAdornmentTemplate", typeof(DataTemplate), typeof(Part),
        new FrameworkPropertyMetadata(null));
      TextEditorProperty = DependencyProperty.RegisterAttached("TextEditor", typeof(ITextEditor), typeof(Part),
        new FrameworkPropertyMetadata(null));
      TextAspectRatioProperty = DependencyProperty.RegisterAttached("TextAspectRatio", typeof(double), typeof(Part),
        new FrameworkPropertyMetadata(1.5));

      // DraggingTool properties:
      // these must be on the root visual element
      DragOverSnapEnabledProperty = DependencyProperty.RegisterAttached("DragOverSnapEnabled", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(false, OnDragOverSnapEnabledChanged));
      DragOverSnapCellSizeProperty = DependencyProperty.RegisterAttached("DragOverSnapCellSize", typeof(Size), typeof(Part),
        new FrameworkPropertyMetadata(new Size(10, 10), OnDragOverSnapCellSizeChanged));
      DragOverSnapCellSpotProperty = DependencyProperty.RegisterAttached("DragOverSnapCellSpot", typeof(Spot), typeof(Part),
        new FrameworkPropertyMetadata(Spot.TopLeft, OnDragOverSnapCellSpotChanged));
      DragOverSnapOriginProperty = DependencyProperty.RegisterAttached("DragOverSnapOrigin", typeof(Point), typeof(Part),
        new FrameworkPropertyMetadata(new Point(0, 0), OnDragOverSnapOriginChanged));

      //DragOverBehaviorProperty = DependencyProperty.RegisterAttached("DragOverBehavior", typeof(DragOverBehavior), typeof(Part),
      //  new FrameworkPropertyMetadata(DragOverBehavior.None, OnDragOverBehaviorChanged));

      DropOntoBehaviorProperty = DependencyProperty.RegisterAttached("DropOntoBehavior", typeof(DropOntoBehavior), typeof(Part),
        new FrameworkPropertyMetadata(DropOntoBehavior.None, OnDropOntoBehaviorChanged));

      // ResizingTool property:
      // this must be on the root visual element
      ResizeCellSizeProperty = DependencyProperty.RegisterAttached("ResizeCellSize", typeof(Size), typeof(Part),
        new FrameworkPropertyMetadata(new Size(Double.NaN, Double.NaN), OnResizeCellSizeChanged));

      // this must be on the root visual element
      TextProperty = DependencyProperty.RegisterAttached("Text", typeof(String), typeof(Part),
        new FrameworkPropertyMetadata("", VerifyOnVisualElement<Part>));

      InDiagramBoundsProperty = DependencyProperty.RegisterAttached("InDiagramBounds", typeof(bool), typeof(Part),
        new FrameworkPropertyMetadata(true, OnInDiagramBoundsChanged));

      // used to limit participation in a particular IDiagramLayout
      // this must be on the root visual element
      LayoutIdProperty = DependencyProperty.RegisterAttached("LayoutId", typeof(String), typeof(Part),
        new FrameworkPropertyMetadata("", VerifyOnVisualElement<Part>));
    }





    internal static readonly IEnumerable<Part> NoParts = new Part[0] { };
    internal static readonly IEnumerable<Node> NoNodes = new Node[0] { };
    internal static readonly IEnumerable<Group> NoGroups = new Group[0] { };
    internal static readonly IEnumerable<Adornment> NoAdornments = new Adornment[0] { };
    internal static readonly IEnumerable<Link> NoLinks = new Link[0] { };

    internal static readonly Cursor SizeWECursor = Cursors.SizeWE;
    internal static readonly Cursor SizeNSCursor = Cursors.SizeNS;

    internal static readonly Cursor SizeNESWCursor = Cursors.SizeNESW;
    internal static readonly Cursor SizeNWSECursor = Cursors.SizeNWSE;





    internal static readonly Cursor SizeAllCursor = Cursors.Hand;
    internal static readonly Cursor ScrollAllCursor = Cursors.Hand;





    internal static void VerifyOnVisualElement<T>(DependencyObject d, DependencyPropertyChangedEventArgs e) where T : Part {
      VerifyParentAs<T>(d, e);
    }

    internal static T VerifyParentAs<T>(DependencyObject d, DependencyPropertyChangedEventArgs e) where T : Part {
      T part = Diagram.FindParent<T>(d);  // must be on root visual child
      if (part == null) {
        var parent = Diagram.FindParent<UIElement>(d);
        // Designers may use a ContentPresenter for design-time data-binding
        if (parent != null && parent.GetType() != typeof(ContentPresenter)) {
          Diagram.Error("Must set the attached property " + e.Property.ToString() + " only on the root VisualElement of a " + typeof(T).Name +
            ".  It was set on " + d.ToString() + " which is a child of " + parent.ToString() + " instead of a " + typeof(T).Name + ".");
        }
      }
      return part;
    }


    /// <summary>
    /// Gets or sets the data that this part is bound to.
    /// </summary>
    /// <value>
    /// For parts for which <see cref="IsBoundToData"/> is true, this returns the data.
    /// You can also set this property to rebind the <c>Content</c> of this part.
    /// For parts that are not bound to data, this will return the <see cref="Part"/> itself.
    /// You cannot set a value that is a <c>UIElement</c>, including parts such as
    /// <see cref="Node"/>s or <see cref="Link"/>s.
    /// </value>
    /// <remarks>
    /// This is overridden by <see cref="Adornment"/> to always be null.
    /// </remarks>
    public virtual Object Data {
      get {
        Object data = this.Content;
        PartManager.PartBinding db = data as PartManager.PartBinding;
        if (db != null) {
          return db.Data;
        } else {  //?? shouldn't set Content to anything but a FrameworkElement or a PartBinding
          return this;
        }
      }
      set {
        _VisualElement = null;  // clear cache
        if (value is UIElement) {
          Diagram.Error("Cannot set Part.Data to a FrameworkElement");
        } else {
          this.Content = new PartManager.PartBinding(this, value);
        }
      }
    }

    /// <summary>
    /// This is true if this part is bound to data.
    /// </summary>
    /// <value>
    /// This will be false for <see cref="Adornment"/>s, for parts 
    /// that are defined as XAML children of a diagram, or for other parts
    /// held by a <see cref="PartsModel"/>.
    /// </value>
    public virtual bool IsBoundToData {
      get {
        Object data = this.Content;
        return data != null && (data is PartManager.PartBinding);
      }
    }

    /// <summary>
    /// Gets the <see cref="IDiagramModel"/> that holds this part's data.
    /// </summary>
    public IDiagramModel Model {
      get;
      internal set;
    }

    /// <summary>
    /// Get the root visual element used in the rendering of this part.
    /// </summary>
    /// <value>
    /// the visual tree child of this part
    /// </value>
    public FrameworkElement VisualElement {
      get {
        if (_VisualElement == null) {
          if (VisualTreeHelper.GetChildrenCount(this) == 0) return null;
          _VisualElement = VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
        }
        return _VisualElement;
      }
    }
    private FrameworkElement _VisualElement;

    internal FrameworkElement EnsuredVisualElement {
      get {
        if (_VisualElement == null) ApplyTemplate();
        return this.VisualElement;
      }
    }


    // force the template to be expanded and made a visual child; this.VisualElement should be non-null afterwards
    internal bool ApplyTemplate() {
      if (_VisualElement == null) {
        Measure(Geo.Unlimited);
        Size sz = this.DesiredSize;
        Arrange(new Rect(0, 0, sz.Width, sz.Height));
        return true;
      }
      return false;
    }

















    /// <summary>
    /// Initialize <c>Visibility</c> according to the value of <see cref="Visible"/>.
    /// </summary>
    public override void OnApplyTemplate() {
      //Diagram.Debug("OnApplyTemplate");
      base.OnApplyTemplate();
      this._SelectionElement = null;  // clear cache reference
      //?? side-effects of OnLayerNameChanged are handled by PartManager
      // do what OnVisibleChanged would have done if it could have
      bool vis = Node.GetVisible(this.VisualElement);
      this.Visibility = (vis ? Visibility.Visible : Visibility.Collapsed);
      if (!vis) DoVisibleChanged(vis);  // only if different from default value
    }


    // Regular properties

    /// <summary>
    /// Gets the <see cref="Northwoods.GoXam.Diagram"/> that this part is in.
    /// </summary>
    /// <seealso cref="Panel"/>
    /// <seealso cref="Layer"/>
    public Diagram Diagram {
      get {
        Layer layer = this.Layer;
        if (layer != null) return layer.Diagram;
        return null;
      }
    }

    /// <summary>
    /// Gets the <see cref="DiagramPanel"/> that this part is in.
    /// </summary>
    /// <seealso cref="Diagram"/>
    /// <seealso cref="Layer"/>
    public DiagramPanel Panel {
      get {
        Layer layer = this.Layer;
        if (layer != null) return layer.Panel;
        return null;
      }
    }

    /// <summary>
    /// Gets the <see cref="Northwoods.GoXam.Layer"/> that this part is in.
    /// </summary>
    /// <seealso cref="Diagram"/>
    /// <seealso cref="Panel"/>
    public Layer Layer { get; internal set; }
    // maintained by the Layer that the Part is in;
    // intentionally not set when adding/removing as a visual child of a Layer


    // Dependency properties

    /// <summary>
    /// Identifies the <see cref="Category"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CategoryProperty;
    /// <summary>
    /// Gets a string identifying the kind of <see cref="Node"/> or <see cref="Group"/> or <see cref="Link"/> or the purpose of the <see cref="Adornment"/>.
    /// </summary>
    /// <value>
    /// The default value is an empty string.
    /// You cannot change this value once the Part has been added to a Diagram.
    /// </value>
    public String Category {
      get { return (String)GetValue(CategoryProperty); }
      set { SetValue(CategoryProperty, value); }
    }
    private static void OnCategoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Part part = (Part)d;
      if (part.Layer != null) {
        Diagram.Error("Cannot set Part.Category after it has been added to a Diagram");
      }
    }


    internal static readonly DependencyProperty OffscreenChildSizeProperty;
    internal static Size GetOffscreenChildSize(DependencyObject d) { return (Size)d.GetValue(OffscreenChildSizeProperty); }
    internal static void SetOffscreenChildSize(DependencyObject d, Size v) { d.SetValue(OffscreenChildSizeProperty, v); }

    internal static void ClearCachedValues(UIElement e) {
      e.ClearValue(OffscreenChildSizeProperty);
    }




    /// <summary>
    /// Identifies the read-only <see cref="Bounds"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BoundsProperty;
    /// <summary>
    /// Gets the bounding rectangle of this object, in model coordinates.
    /// </summary>
    /// <value>
    /// The <c>X</c> and <c>Y</c> values may be <c>Double.NaN</c> if the
    /// part has not yet been positioned, for example, if it is a <see cref="Node"/>
    /// and the node's <see cref="Node.Location"/> has not been set.
    /// This property is read-only -- the <c>Width</c> and <c>Height</c>
    /// are determined by the actual size of the part.
    /// </value>
    /// <seealso cref="Node.Location"/>
    public Rect Bounds {
      get { return (Rect)GetValue(BoundsProperty); }
      internal set {

        SetValue(BoundsProperty, value);



      }
    }



    private static void OnBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Part part = (Part)d;
      //Diagram.Debug("    OnBoundsChanged: " + Diagram.Str(part) + " " + Diagram.Str((Rect)e.OldValue) + Diagram.Str((Rect)e.NewValue));
      
      //?? don't allow programmers to set this property in Silverlight
      //if (!part.Initializing) Diagram.Error("Cannot set Part.Bounds property");

      // update whether this part is a visual child of a layer, based on its Bounds and the DiagramPanel.ViewportBounds
      DiagramPanel panel = part.Panel;
      if (panel != null) {
        // for updating parts in an Overview
        panel.RaisePartBoundsChanged(part);

        Layer layer = Diagram.FindParent<Layer>(part);
        if (layer == null && part.Layer != null) {  // not in a layer, but used to be in a layer
          Rect b = panel.InflatedViewportBounds;
          if (part.IntersectsRect(b)) {  // moving from offscreen to onscreen
            part.EnsureOnscreen();
          }
        }
      }
    }

    internal void EnsureOnscreen() {
      if (!this.Offscreen) return;
      Layer layer = this.Layer;
      if (layer == null) return;
      Node n = this as Node;
      if (n != null) {
        ((NodeLayer)layer).InternalAdd(n);
      } else {
        Link l = this as Link;
        if (l != null) {
          ((LinkLayer)layer).InternalAdd(l);
        }
      }
    }


    /// <summary>
    /// Identifies the <see cref="IsSelected"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty;
    /// <summary>
    /// Gets or sets whether this part is selected.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// Setting this property to true will cause this part to be added to
    /// the diagram's <see cref="Northwoods.GoXam.Diagram.SelectedParts"/> collection.
    /// Setting this property to false will cause this part to be removed
    /// from that collection.
    /// </value>
    /// <remarks>
    /// If <see cref="CanSelect"/> is false, one cannot set this property to true.
    /// Also, the number of selected parts is limited by the value of
    /// <see cref="Northwoods.GoXam.Diagram.MaximumSelectionCount"/>, so this
    /// part cannot be selected if that would violate that limit.
    /// </remarks>
    public bool IsSelected {
      get { return (bool)GetValue(IsSelectedProperty); }
      set { SetValue(IsSelectedProperty, value); }
    }
    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Part part = (Part)d;
      Diagram diagram = part.Diagram;
      if (diagram != null) {
        ObservableCollection<Part> sel = diagram.SelectedParts;
        bool selected = (bool)e.NewValue;
        if (selected) {
          if (!part.CanSelect()) {
            part.IsSelected = false;
          } else {
            int max = diagram.MaximumSelectionCount;
            int count = sel.Count();
            if (count >= max) {
              part.IsSelected = false;
            } else {
              if (!sel.Contains(part)) {
                sel.Add(part);
                part.OnIsSelectedChanged();
              }
            }
          }
        } else {
          sel.Remove(part);
          part.OnIsSelectedChanged();
        }
      }
    }

    /// <summary>
    /// This virtual method is called whenever this part is added or removed from the
    /// collection of <see cref="Northwoods.GoXam.Diagram.SelectedParts"/>.
    /// </summary>
    /// <remarks>
    /// By default this just calls <see cref="Remeasure"/>,
    /// which will eventually call <see cref="UpdateAdornments"/>.
    /// In Silverlight, if the <see cref="Part.VisualElement"/> is a <c>Control</c>,
    /// this will also call <c>VisualStateManager.GoToState</c> with a new state
    /// of either "Selected" or "Unselected".
    /// </remarks>
    protected virtual void OnIsSelectedChanged() {

      Control control = this.VisualElement as Control;
      if (control != null) {
        VisualStateManager.GoToState(control, (this.IsSelected ? "Selected" : "Unselected"), true);
      }

      Remeasure();
    }


    /// <summary>
    /// Get the collection of <see cref="Adornment"/>s associated with this part.
    /// </summary>
    /// <remarks>
    /// Each adornment will have a <see cref="Part.Category"/> that is unique for this part.
    /// </remarks>
    public IEnumerable<Adornment> Adornments {
      get {
        if (_Adornments != null)
          return _Adornments.Values;
        else
          return NoAdornments;
      }
    }
    private Dictionary<String, Adornment> _Adornments;  //?? optimize when there's only one Adornment

    /// <summary>
    /// Find an <see cref="Adornment"/> associated with this part that has a particular category.
    /// </summary>
    /// <param name="category">this must be a non-null string</param>
    /// <returns>null if this no such adornment</returns>
    public Adornment GetAdornment(String category) {
      if (category == null) Diagram.Error("Adornment category must not be null");
      Adornment ad = null;
      if (_Adornments != null) _Adornments.TryGetValue(category, out ad);
      return ad;
    }

    /// <summary>
    /// Associate an <see cref="Adornment"/> with this part.
    /// </summary>
    /// <param name="category">this must be a non-null string</param>
    /// <param name="ad"></param>
    public void SetAdornment(String category, Adornment ad) {
      if (category == null) Diagram.Error("Adornment category must not be null");
      Adornment oldad = null;
      if (_Adornments != null) _Adornments.TryGetValue(category, out oldad);
      if (oldad != ad) {
        if (oldad != null) {
          NodeLayer oldlayer = oldad.Layer as NodeLayer;
          if (oldlayer != null) oldlayer.Remove(oldad);
        }
        if (ad != null) {
          if (_Adornments == null) _Adornments = new Dictionary<String, Adornment>();
          _Adornments[category] = ad;
        } else {
          if (_Adornments != null) {
            _Adornments.Remove(category);
            if (_Adornments.Count == 0) _Adornments = null;
          }
        }
        if (ad != null && this.Diagram != null) {
          DiagramPanel panel = this.Diagram.Panel;
          if (panel != null) {
            NodeLayer layer = panel.AdornmentLayer;
            if (layer != null) layer.Add(ad);
          }
        }
      }
    }

    // remove all adornments from this part
    internal void ClearAdornments() {
      if (_Adornments != null) {
        foreach (String n in _Adornments.Keys.ToList()) {
          SetAdornment(n, null);  // remove Adornment from Layer and remove dictionary entry
        }
      }
    }

    // make sure all existing adornments are deleted and any needed adornments are re-created
    internal void RefreshAdornments() {
      ClearAdornments();
      UpdateAdornments();
    }

    /// <summary>
    /// Create an <see cref="Adornment"/> for a particular <c>FrameworkElement</c>
    /// using a given <c>DataTemplate</c>.
    /// </summary>
    /// <param name="selelt">must not be null</param>
    /// <param name="templ">must not be null</param>
    /// <returns></returns>
    /// <remarks>
    /// If <see cref="IsBoundToData"/> is true, the DataTemplate has access to this part's <see cref="Data"/>.
    /// </remarks>
    public virtual Adornment MakeAdornment(FrameworkElement selelt, DataTemplate templ) {
      if (selelt == null) return null;
      if (templ == null) return null;
      Adornment adornment = new Adornment();  // for Part.MakeAdornment
      adornment.AdornedElement = selelt;
      adornment.Content = new PartManager.PartBinding(adornment, (this.IsBoundToData ? this.Data : null));
      adornment.ContentTemplate = templ;  // for Part.MakeAdornment
      return adornment;
    }


    /// <summary>
    /// Maybe create adornments if needed, or remove them if not.
    /// </summary>
    /// <remarks>
    /// This calls <see cref="UpdateSelectionAdornment"/> and <see cref="UpdateToolAdornments"/>.
    /// This is called by <see cref="OnIsSelectedChanged()"/>, among other places.
    /// </remarks>
    public virtual void UpdateAdornments() {
      UpdateSelectionAdornment();
      UpdateToolAdornments();
    }

    /// <summary>
    /// This is responsible for creating a selection adornment for this part
    /// if this part <see cref="IsSelected"/> and if <see cref="SelectionAdorned"/> is true.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This also removes any selection adornment if this part is no longer selected,
    /// not visible, or if <see cref="SelectionAdorned"/> is false.
    /// </para>
    /// <para>
    /// To create the selection adornment this calls <see cref="MakeAdornment"/>
    /// with the <see cref="SelectionElement"/> and the <see cref="SelectionAdornmentTemplate"/>.
    /// If there is no <see cref="SelectionAdornmentTemplate"/> for this part,
    /// the template defaults to the <c>DataTemplate</c> named "DefaultSelectionAdornmentTemplate".
    /// </para>
    /// </remarks>
    protected virtual void UpdateSelectionAdornment() {
      Adornment adornment = null;
      String category = "Selection";
      if (this.IsSelected) {
        FrameworkElement selelt = this.SelectionElement;
        if (selelt != null && IsVisibleElement(selelt) && this.SelectionAdorned) {
          adornment = GetAdornment(category);
          if (adornment == null) {
            DataTemplate template = this.SelectionAdornmentTemplate;
            if (template == null) template = Diagram.FindDefault<DataTemplate>("DefaultSelectionAdornmentTemplate");
            adornment = MakeAdornment(selelt, template);
            if (adornment != null) {
              adornment.Category = category;
              adornment.LocationSpot = Spot.TopLeft;
            }
          }
          if (adornment != null) {
            Point pos = GetElementPoint(selelt, Spot.TopLeft);
            double angle = GetAngle(selelt);
            adornment.Location = pos;
            adornment.RotationAngle = angle;
            adornment.Remeasure();
          }
        }
      }
      SetAdornment(category, adornment);
    }

    /// <summary>
    /// This is responsible for creating any tool adornments for this part.
    /// </summary>
    /// <remarks>
    /// This delegates the responsibility to each tool in
    /// <see cref="Northwoods.GoXam.Diagram.MouseDownTools"/>
    /// and <see cref="Northwoods.GoXam.Diagram.MouseMoveTools"/>
    /// by calling <see cref="IDiagramTool.UpdateAdornments"/> on each tool.
    /// </remarks>
    protected virtual void UpdateToolAdornments() {
      Diagram diagram = this.Diagram;
      if (diagram != null) {
        foreach (IDiagramTool tool in diagram.MouseDownTools) tool.UpdateAdornments(this);
        foreach (IDiagramTool tool in diagram.MouseMoveTools) tool.UpdateAdornments(this);
      }
    }


    // Attached properties

    /// <summary>
    /// Identifies the <see cref="LayerName"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty LayerNameProperty;
    /// <summary>
    /// Gets the value of the <see cref="LayerName"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static String GetLayerName(DependencyObject d) { return (String)d.GetValue(LayerNameProperty); }
    /// <summary>
    /// Sets the value of the <see cref="LayerName"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetLayerName(DependencyObject d, String v) { d.SetValue(LayerNameProperty, v); }
    private static void OnLayerNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      String newname = (String)e.NewValue;
      Part part = VerifyParentAs<Part>(d, e);
      Node node = part as Node;
      if (node != null) {
        DiagramPanel panel = node.Panel;
        if (panel == null) return;
        NodeLayer oldlayer = node.Layer as NodeLayer;
        NodeLayer newlayer = panel.FindLayer<NodeLayer>(newname);
        if (newlayer != null && newlayer != oldlayer) {
          if (oldlayer != null) oldlayer.Remove(node);
          newlayer.Add(node);
        }
      } else {
        Link link = part as Link;
        if (link != null) {
          DiagramPanel panel = link.Panel;
          if (panel == null) return;
          LinkLayer oldlayer = link.Layer as LinkLayer;
          LinkLayer newlayer = panel.FindLayer<LinkLayer>(newname);
          if (newlayer != null && newlayer != oldlayer) {
            if (oldlayer != null) oldlayer.Remove(link);
            newlayer.Add(link);
          }
        }
      }
    }
    /// <summary>
    /// Gets or sets the name of the <see cref="Northwoods.GoXam.Layer"/> that this part should be in.
    /// </summary>
    /// <value>
    /// The default value is an empty string.
    /// </value>
    /// <remarks>
    /// This value is used to find the <see cref="Northwoods.GoXam.Layer"/>
    /// with the <see cref="Northwoods.GoXam.Layer.Id"/> of the same value.
    /// </remarks>
    public String LayerName {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetLayerName(elt); else return ""; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetLayerName(elt, value); }
    }


    /// <summary>
    /// Identifies the <see cref="Copyable"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty CopyableProperty;
    /// <summary>
    /// Gets the value of the <see cref="Copyable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetCopyable(DependencyObject d) { return (bool)d.GetValue(CopyableProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Copyable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetCopyable(DependencyObject d, bool v) { d.SetValue(CopyableProperty, v); }
    /// <summary>
    /// Gets or sets whether the user may copy this part.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <remarks>
    /// In general one should call <see cref="CanCopy"/> to see
    /// if a particular part is copyable, not get this property value.
    /// </remarks>
    public bool Copyable {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetCopyable(elt); else return true; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetCopyable(elt, value); }
    }

    /// <summary>
    /// This predicate is true if the user may copy
    /// this part.
    /// </summary>
    /// <returns>
    /// Return true if this part is <see cref="Copyable"/>,
    /// if this part's layer's <see cref="Northwoods.GoXam.Layer.AllowCopy"/> is true,
    /// and if this part's diagram's <see cref="Northwoods.GoXam.Diagram.AllowCopy"/> is true.
    /// </returns>
    public bool CanCopy() {
      if (!this.Copyable) return false;
      Layer layer = this.Layer;
      if (layer == null) return true;
      if (!layer.AllowCopy) return false;
      Diagram diagram = layer.Diagram;
      if (diagram == null) return true;
      if (!diagram.AllowCopy) return false;
      return true;
    }

    /// <summary>
    /// Identifies the <see cref="Deletable"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty DeletableProperty;
    /// <summary>
    /// Gets the value of the <see cref="Deletable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetDeletable(DependencyObject d) { return (bool)d.GetValue(DeletableProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Deletable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetDeletable(DependencyObject d, bool v) { d.SetValue(DeletableProperty, v); }
    /// <summary>
    /// Gets or sets whether the user may remove this part from the diagram.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <remarks>
    /// In general one should call <see cref="CanDelete"/> to see
    /// if a particular part is deletable, not get this property value.
    /// </remarks>
    public bool Deletable {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetDeletable(elt); else return true; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetDeletable(elt, value); }
    }

    /// <summary>
    /// This predicate is true if the user may delete
    /// this part.
    /// </summary>
    /// <returns>
    /// Return true if this part is <see cref="Deletable"/>,
    /// if this part's layer's <see cref="Northwoods.GoXam.Layer.AllowDelete"/> is true,
    /// and if this part's diagram's <see cref="Northwoods.GoXam.Diagram.AllowDelete"/> is true.
    /// </returns>
    public bool CanDelete() {
      if (!this.Deletable) return false;
      Layer layer = this.Layer;
      if (layer == null) return true;
      if (!layer.AllowDelete) return false;
      Diagram diagram = layer.Diagram;
      if (diagram == null) return true;
      if (!diagram.AllowDelete) return false;
      return true;
    }

    /// <summary>
    /// Identifies the <see cref="Editable"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty EditableProperty;
    /// <summary>
    /// Gets the value of the <see cref="Editable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetEditable(DependencyObject d) { return (bool)d.GetValue(EditableProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Editable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetEditable(DependencyObject d, bool v) { d.SetValue(EditableProperty, v); }
    /// <summary>
    /// Gets or sets whether the user may in-place text edit this part.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <remarks>
    /// In general one should call <see cref="CanEdit"/> to see
    /// if a particular part is editable, not get this property value.
    /// </remarks>
    public bool Editable {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetEditable(elt); else return true; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetEditable(elt, value); }
    }

    /// <summary>
    /// This predicate is true if the user may in-place edit the text of
    /// this part.
    /// </summary>
    /// <returns>
    /// Return true if this part is <see cref="Editable"/>,
    /// if this part's layer's <see cref="Northwoods.GoXam.Layer.AllowEdit"/> is true,
    /// and if this part's diagram's <see cref="Northwoods.GoXam.Diagram.AllowEdit"/> is true.
    /// </returns>
    public bool CanEdit() {
      if (!this.Editable) return false;
      Layer layer = this.Layer;
      if (layer == null) return true;
      if (!layer.AllowEdit) return false;
      Diagram diagram = layer.Diagram;
      if (diagram == null) return true;
      if (!diagram.AllowEdit) return false;
      return true;
    }

    /// <summary>
    /// Identifies the <see cref="Groupable"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty GroupableProperty;
    /// <summary>
    /// Gets the value of the <see cref="Groupable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetGroupable(DependencyObject d) { return (bool)d.GetValue(GroupableProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Groupable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetGroupable(DependencyObject d, bool v) { d.SetValue(GroupableProperty, v); }
    /// <summary>
    /// Gets or sets whether the user may group this part.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <remarks>
    /// In general one should call <see cref="CanGroup"/> to see
    /// if a particular part is groupable, not get this property value.
    /// </remarks>
    public bool Groupable {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetGroupable(elt); else return true; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetGroupable(elt, value); }
    }

    /// <summary>
    /// This predicate is true if the user may group
    /// this part.
    /// </summary>
    /// <returns>
    /// Return true if this part is <see cref="Groupable"/>,
    /// if this part's layer's <see cref="Northwoods.GoXam.Layer.AllowGroup"/> is true,
    /// and if this part's diagram's <see cref="Northwoods.GoXam.Diagram.AllowGroup"/> is true.
    /// </returns>
    public bool CanGroup() {
      if (!this.Groupable) return false;
      Layer layer = this.Layer;
      if (layer == null) return true;
      if (!layer.AllowGroup) return false;
      Diagram diagram = layer.Diagram;
      if (diagram == null) return true;
      if (!diagram.AllowGroup) return false;
      return true;
    }

    /// <summary>
    /// Identifies the <see cref="Movable"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty MovableProperty;
    /// <summary>
    /// Gets the value of the <see cref="Movable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetMovable(DependencyObject d) { return (bool)d.GetValue(MovableProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Movable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetMovable(DependencyObject d, bool v) { d.SetValue(MovableProperty, v); }
    /// <summary>
    /// Gets or sets whether the user may move this part.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <remarks>
    /// In general one should call <see cref="CanMove"/> to see
    /// if a particular part is movable, not get this property value.
    /// </remarks>
    public bool Movable {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetMovable(elt); else return true; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetMovable(elt, value); }
    }

    /// <summary>
    /// This predicate is true if the user may move
    /// this part.
    /// </summary>
    /// <returns>
    /// Return true if this part is <see cref="Movable"/>,
    /// if this part's layer's <see cref="Northwoods.GoXam.Layer.AllowMove"/> is true,
    /// and if this part's diagram's <see cref="Northwoods.GoXam.Diagram.AllowMove"/> is true.
    /// </returns>
    public bool CanMove() {
      if (!this.Movable) return false;
      Layer layer = this.Layer;
      if (layer == null) return true;
      if (!layer.AllowMove) return false;
      Diagram diagram = layer.Diagram;
      if (diagram == null) return true;
      if (!diagram.AllowMove) return false;
      return true;
    }

    /// <summary>
    /// Identifies the <see cref="Printable"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty PrintableProperty;
    /// <summary>
    /// Gets the value of the <see cref="Printable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetPrintable(DependencyObject d) { return (bool)d.GetValue(PrintableProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Printable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetPrintable(DependencyObject d, bool v) { d.SetValue(PrintableProperty, v); }
    /// <summary>
    /// Gets or sets whether the user may print this part.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <remarks>
    /// In general one should call <see cref="CanPrint"/> to see
    /// if a particular part is printable, not get this property value.
    /// </remarks>
    public bool Printable {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetPrintable(elt); else return true; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetPrintable(elt, value); }
    }

    /// <summary>
    /// This predicate is true if the user may print
    /// this part.
    /// </summary>
    /// <returns>
    /// Return true if this part is <see cref="Printable"/>,
    /// if this part's layer's <see cref="Northwoods.GoXam.Layer.AllowPrint"/> is true,
    /// and if this part's diagram's <see cref="Northwoods.GoXam.Diagram.AllowPrint"/> is true.
    /// </returns>
    public bool CanPrint() {
      if (!this.Printable) return false;
      Layer layer = this.Layer;
      if (layer == null) return true;
      if (!layer.AllowPrint) return false;
      Diagram diagram = layer.Diagram;
      if (diagram == null) return true;
      if (!diagram.AllowPrint) return false;
      return true;
    }

    /// <summary>
    /// Identifies the <see cref="Reshapable"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty ReshapableProperty;
    /// <summary>
    /// Gets the value of the <see cref="Reshapable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetReshapable(DependencyObject d) { return (bool)d.GetValue(ReshapableProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Reshapable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetReshapable(DependencyObject d, bool v) { d.SetValue(ReshapableProperty, v); }
    /// <summary>
    /// Gets or sets whether the user may reshape this part.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    /// <remarks>
    /// In general one should call <see cref="CanReshape"/> to see
    /// if a particular part is reshapable, not get this property value.
    /// </remarks>
    public bool Reshapable {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetReshapable(elt); else return true; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetReshapable(elt, value); }
    }

    /// <summary>
    /// This predicate is true if the user may reshape
    /// this part.
    /// </summary>
    /// <returns>
    /// Return true if this part is <see cref="Reshapable"/>,
    /// if this part's layer's <see cref="Northwoods.GoXam.Layer.AllowReshape"/> is true,
    /// and if this part's diagram's <see cref="Northwoods.GoXam.Diagram.AllowReshape"/> is true.
    /// </returns>
    public bool CanReshape() {
      if (!this.Reshapable) return false;
      Layer layer = this.Layer;
      if (layer == null) return true;
      if (!layer.AllowReshape) return false;
      Diagram diagram = layer.Diagram;
      if (diagram == null) return true;
      if (!diagram.AllowReshape) return false;
      return true;
    }

    /// <summary>
    /// Identifies the <see cref="Resizable"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty ResizableProperty;
    /// <summary>
    /// Gets the value of the <see cref="Resizable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetResizable(DependencyObject d) { return (bool)d.GetValue(ResizableProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Resizable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetResizable(DependencyObject d, bool v) { d.SetValue(ResizableProperty, v); }
    /// <summary>
    /// Gets or sets whether the user may resize this part.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    /// <remarks>
    /// In general one should call <see cref="CanResize"/> to see
    /// if a particular part is resizable, not get this property value.
    /// </remarks>
    public bool Resizable {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetResizable(elt); else return true; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetResizable(elt, value); }
    }

    /// <summary>
    /// This predicate is true if the user may resize
    /// this part.
    /// </summary>
    /// <returns>
    /// Return true if this part is <see cref="Resizable"/>,
    /// if this part's layer's <see cref="Northwoods.GoXam.Layer.AllowResize"/> is true,
    /// and if this part's diagram's <see cref="Northwoods.GoXam.Diagram.AllowResize"/> is true.
    /// </returns>
    public bool CanResize() {
      if (!this.Resizable) return false;
      Layer layer = this.Layer;
      if (layer == null) return true;
      if (!layer.AllowResize) return false;
      Diagram diagram = layer.Diagram;
      if (diagram == null) return true;
      if (!diagram.AllowResize) return false;
      return true;
    }

    /// <summary>
    /// Identifies the <see cref="Rotatable"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty RotatableProperty;
    /// <summary>
    /// Gets the value of the <see cref="Rotatable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetRotatable(DependencyObject d) { return (bool)d.GetValue(RotatableProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Rotatable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetRotatable(DependencyObject d, bool v) { d.SetValue(RotatableProperty, v); }
    /// <summary>
    /// Gets or sets whether the user may rotate this part.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    /// <remarks>
    /// In general one should call <see cref="CanRotate"/> to see
    /// if a particular part is rotatable, not get this property value.
    /// </remarks>
    public bool Rotatable {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetRotatable(elt); else return true; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetRotatable(elt, value); }
    }

    /// <summary>
    /// This predicate is true if the user may rotate
    /// this part.
    /// </summary>
    /// <returns>
    /// Return true if this part is <see cref="Rotatable"/>,
    /// if this part's layer's <see cref="Northwoods.GoXam.Layer.AllowRotate"/> is true,
    /// and if this part's diagram's <see cref="Northwoods.GoXam.Diagram.AllowRotate"/> is true.
    /// </returns>
    public bool CanRotate() {
      if (!this.Rotatable) return false;
      Layer layer = this.Layer;
      if (layer == null) return true;
      if (!layer.AllowRotate) return false;
      Diagram diagram = layer.Diagram;
      if (diagram == null) return true;
      if (!diagram.AllowRotate) return false;
      return true;
    }

    /// <summary>
    /// Identifies the <see cref="Visible"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty VisibleProperty;
    /// <summary>
    /// Gets the value of the <see cref="Visible"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetVisible(DependencyObject d) { return (bool)d.GetValue(VisibleProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Visible"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetVisible(DependencyObject d, bool v) { d.SetValue(VisibleProperty, v); }
    /// <summary>
    /// Gets or sets whether the user may see or pick this part.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <remarks>
    /// <para>
    /// Setting this property also sets the <c>Part.Visibility</c> property to either <c>Visibility.Visible</c>
    /// or <c>Visibility.Collapsed</c>.
    /// </para>
    /// <para>
    /// Caution: because the <c>UIElement.Visibility</c> of the <see cref="Part"/> may be set
    /// independently of this property, the value of this property may not reflect the value of <c>Part.Visibility</c>.
    /// This property is most useful in templates for setting the initial value of <c>Visibility</c> for the part.
    /// </para>
    /// </remarks>
    public bool Visible {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetVisible(elt); else return true; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetVisible(elt, value); }
    }
    private static void OnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Part part = VerifyParentAs<Part>(d, e);
      if (part != null) {
        part.OnVisibleChanged();
      }
    }

    private /*?? protected virtual */ void OnVisibleChanged() {
      bool vis = this.Visible;
      // also done by OnApplyTemplate, if the parent Part was not found during init
      this.Visibility = (vis ? Visibility.Visible : Visibility.Collapsed);
      Remeasure();  // only Remeasure when Visible???
      // maybe we need to re-layout
      this.InvalidateLayout(LayoutChange.VisibleChanged);
      // just in case there's no layout scheduled, make sure we recompute the diagram bounds
      DiagramPanel panel = this.Panel;
      if (panel != null && this.CanIncludeInDiagramBounds()) panel.InvokeUpdateDiagramBounds("VisibleChanged");

      // inform any Overviews
      if (panel != null) panel.RaisePartVisibleChanged(this);
      
      //??? always propagate Visible changes
      //IDiagramModel model = this.Model;
      //if (model != null && model.IsChangingModel) return;
      DoVisibleChanged(vis);
    }

    private void DoVisibleChanged(bool vis) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      // avoid multiple simultaneous propagations
      if (diagram.IsHidingShowing) return;
      diagram.IsHidingShowing = true;
      PropagateVisible(vis);
      diagram.IsHidingShowing = false;
    }


    /// <summary>
    /// Identifies the <see cref="Selectable"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty SelectableProperty;
    /// <summary>
    /// Gets the value of the <see cref="Selectable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetSelectable(DependencyObject d) { return (bool)d.GetValue(SelectableProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Selectable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetSelectable(DependencyObject d, bool v) { d.SetValue(SelectableProperty, v); }
    /// <summary>
    /// Gets or sets whether the user may select this part.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <remarks>
    /// In general one should call <see cref="CanSelect"/> to see
    /// if a particular part is selectable, not get this property value.
    /// </remarks>
    public bool Selectable {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetSelectable(elt); else return true; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetSelectable(elt, value); }
    }

    /// <summary>
    /// This predicate is true if the user may select
    /// this part.
    /// </summary>
    /// <returns>
    /// Return true if this part is <see cref="Selectable"/>,
    /// if this part's layer's <see cref="Northwoods.GoXam.Layer.AllowSelect"/> is true,
    /// and if this part's diagram's <see cref="Northwoods.GoXam.Diagram.AllowSelect"/> is true.
    /// </returns>
    public bool CanSelect() {
      if (!this.Selectable) return false;
      Layer layer = this.Layer;
      if (layer == null) return true;
      if (!layer.AllowSelect) return false;
      Diagram diagram = layer.Diagram;
      if (diagram == null) return true;
      if (!diagram.AllowSelect) return false;
      return true;
    }


    /// <summary>
    /// Identifies the <see cref="SelectionElementName"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty SelectionElementNameProperty;
    /// <summary>
    /// Gets the value of the <see cref="SelectionElementName"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static String GetSelectionElementName(DependencyObject d) { return (String)d.GetValue(SelectionElementNameProperty); }
    /// <summary>
    /// Sets the value of the <see cref="SelectionElementName"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetSelectionElementName(DependencyObject d, String v) { d.SetValue(SelectionElementNameProperty, v); }
    private static void OnSelectionElementNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Part part = VerifyParentAs<Part>(d, e);
      if (part != null) {
        // clear out cached element
        part._SelectionElement = null;
        // re-create adornments, if needed
        part.RefreshAdornments();
      }
    }
    /// <summary>
    /// Gets or sets the name of the element that gets any selection adornment
    /// when selected and that may be resized/reshaped/rotated.
    /// </summary>
    /// <value>
    /// The default value is an empty string,
    /// which means <see cref="SelectionElement"/> will be the <see cref="Part.VisualElement"/>,
    /// i.e. the whole part.
    /// </value>
    /// <remarks>
    /// This is used to find the element that is this part's <see cref="SelectionElement"/>.
    /// </remarks>
    public String SelectionElementName {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetSelectionElementName(elt); else return ""; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetSelectionElementName(elt, value); }
    }

    /// <summary>
    /// Gets the <c>FrameworkElement</c> that gets a selection adornment
    /// when selected and that may be resized/reshaped/rotated.
    /// </summary>
    /// <remarks>
    /// If no element is named by <see cref="SelectionElementName"/>,
    /// this returns the <see cref="Part.VisualElement"/>.
    /// </remarks>
    public FrameworkElement SelectionElement {
      get {
        if (_SelectionElement == null) {
          FrameworkElement vchild = this.VisualElement;
          if (vchild == null) {  // not yet found, keep searching next time
            return null;
          } else {
            String ename = this.SelectionElementName;
            if (ename != null && ename.Length > 0) {
              FrameworkElement elt = FindNamedDescendant(ename);
              if (elt != null) {  // found it: cache it
                _SelectionElement = elt;
              } else {  // not yet found but use this VCHILD for now, keep searching next time
                return null;
              }
            } else {  // always use main visual child
              _SelectionElement = vchild;
            }
          }
        }
        return _SelectionElement;
      }
    }
    private FrameworkElement _SelectionElement;


    /// <summary>
    /// Identifies the <see cref="SelectionAdorned"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty SelectionAdornedProperty;
    /// <summary>
    /// Gets the value of the <see cref="SelectionAdorned"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetSelectionAdorned(DependencyObject d) { return (bool)d.GetValue(SelectionAdornedProperty); }
    /// <summary>
    /// Sets the value of the <see cref="SelectionAdorned"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetSelectionAdorned(DependencyObject d, bool v) { d.SetValue(SelectionAdornedProperty, v); }
    private static void OnSelectionAdornedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Part part = VerifyParentAs<Part>(d, e);
      if (part != null) {
        part.UpdateAdornments();
      }
    }
    /// <summary>
    /// Gets or sets whether the <see cref="SelectionElement"/> gets a
    /// selection adornment when this part is selected.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    /// <remarks>
    /// When this value is false there might not be any visual indication that a part
    /// is selected unless the part changes its appearance.
    /// </remarks>
    public bool SelectionAdorned {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetSelectionAdorned(elt); else return false; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetSelectionAdorned(elt, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectionAdornmentTemplate"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty SelectionAdornmentTemplateProperty;
    /// <summary>
    /// Gets the value of the <see cref="SelectionAdornmentTemplate"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static DataTemplate GetSelectionAdornmentTemplate(DependencyObject d) { return (DataTemplate)d.GetValue(SelectionAdornmentTemplateProperty); }
    /// <summary>
    /// Sets the value of the <see cref="SelectionAdornmentTemplate"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetSelectionAdornmentTemplate(DependencyObject d, DataTemplate v) { d.SetValue(SelectionAdornmentTemplateProperty, v); }
    private static void OnSelectionAdornmentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Part part = VerifyParentAs<Part>(d, e);
      if (part != null) {
        part.RefreshAdornments();
      }
    }
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used to create the selection adornment
    /// for this part when it is selected.
    /// </summary>
    /// <value>
    /// The default value is null.
    /// </value>
    /// <remarks>
    /// This value is used by <see cref="UpdateSelectionAdornment"/>.
    /// </remarks>
    public DataTemplate SelectionAdornmentTemplate {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetSelectionAdornmentTemplate(elt); else return null; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetSelectionAdornmentTemplate(elt, value); }
    }


    /// <summary>
    /// Identifies the <see cref="ResizeAdornmentTemplate"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty ResizeAdornmentTemplateProperty;
    /// <summary>
    /// Gets the value of the <see cref="ResizeAdornmentTemplate"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static DataTemplate GetResizeAdornmentTemplate(DependencyObject d) { return (DataTemplate)d.GetValue(ResizeAdornmentTemplateProperty); }
    /// <summary>
    /// Sets the value of the <see cref="ResizeAdornmentTemplate"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetResizeAdornmentTemplate(DependencyObject d, DataTemplate v) { d.SetValue(ResizeAdornmentTemplateProperty, v); }
    private static void OnResizeAdornmentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Part part = VerifyParentAs<Part>(d, e);
      if (part != null) {
        part.RefreshAdornments();
      }
    }
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used by the <see cref="Northwoods.GoXam.Tool.ResizingTool"/>
    /// when this part is selected.
    /// </summary>
    /// <value>
    /// The default value is null.
    /// </value>
    public DataTemplate ResizeAdornmentTemplate {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetResizeAdornmentTemplate(elt); else return null; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetResizeAdornmentTemplate(elt, value); }
    }


    /// <summary>
    /// Identifies the <see cref="RotateAdornmentTemplate"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty RotateAdornmentTemplateProperty;
    /// <summary>
    /// Gets the value of the <see cref="RotateAdornmentTemplate"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static DataTemplate GetRotateAdornmentTemplate(DependencyObject d) { return (DataTemplate)d.GetValue(RotateAdornmentTemplateProperty); }
    /// <summary>
    /// Sets the value of the <see cref="RotateAdornmentTemplate"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetRotateAdornmentTemplate(DependencyObject d, DataTemplate v) { d.SetValue(RotateAdornmentTemplateProperty, v); }
    private static void OnRotateAdornmentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Part part = VerifyParentAs<Part>(d, e);
      if (part != null) {
        part.RefreshAdornments();
      }
    }
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used by the <see cref="Northwoods.GoXam.Tool.RotatingTool"/>
    /// when this part is selected.
    /// </summary>
    /// <value>
    /// The default value is null.
    /// </value>
    public DataTemplate RotateAdornmentTemplate {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetRotateAdornmentTemplate(elt); else return null; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetRotateAdornmentTemplate(elt, value); }
    }


    /// <summary>
    /// Identifies the <c>TextEditable</c> attached dependency property
    /// that may be on any <c>TextBlock</c>.
    /// </summary>
    public static readonly DependencyProperty TextEditableProperty;
    /// <summary>
    /// Gets the value of the <see cref="TextEditableProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">any <c>TextBlock</c> within the visual tree of the part</param>
    /// <returns>By default this returns false</returns>
    public static bool GetTextEditable(DependencyObject d) { return (bool)d.GetValue(TextEditableProperty); }
    /// <summary>
    /// Sets the value of the <see cref="TextEditableProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">any <c>TextBlock</c> within the visual tree of the part</param>
    /// <param name="v"></param>
    public static void SetTextEditable(DependencyObject d, bool v) { d.SetValue(TextEditableProperty, v); }

    /// <summary>
    /// Identifies the <c>TextEditAdornmentTemplate</c> attached dependency property
    /// that may be on any <c>TextBlock</c>.
    /// </summary>
    public static readonly DependencyProperty TextEditAdornmentTemplateProperty;
    /// <summary>
    /// Gets the value of the <see cref="TextEditAdornmentTemplateProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">any <c>TextBlock</c> within the visual tree of the part</param>
    /// <returns>
    /// A <c>DataTemplate</c>, by default null.
    /// A null value causes the <see cref="Northwoods.GoXam.Tool.TextEditingTool"/>
    /// to use a default template that uses a <c>TextBox</c>.
    /// </returns>
    public static DataTemplate GetTextEditAdornmentTemplate(DependencyObject d) { return (DataTemplate)d.GetValue(TextEditAdornmentTemplateProperty); }
    /// <summary>
    /// Sets the value of the <see cref="TextEditAdornmentTemplateProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">any <c>TextBlock</c> within the visual tree of the part</param>
    /// <param name="v"></param>
    public static void SetTextEditAdornmentTemplate(DependencyObject d, DataTemplate v) { d.SetValue(TextEditAdornmentTemplateProperty, v); }

    /// <summary>
    /// Identifies the <c>TextEditor</c> attached dependency property
    /// that may be on any <c>TextBlock</c>.
    /// </summary>
    public static readonly DependencyProperty TextEditorProperty;
    /// <summary>
    /// Gets the value of the <see cref="TextEditorProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">any <c>TextBlock</c> within the visual tree of the part</param>
    /// <returns>
    /// An <see cref="Northwoods.GoXam.Tool.ITextEditor"/>.
    /// By default this returns null, which causes the <see cref="Northwoods.GoXam.Tool.TextEditingTool"/>
    /// to use an editor that works with the default <see cref="GetTextEditAdornmentTemplate"/> that uses a <c>TextBox</c>.
    /// </returns>
    public static ITextEditor GetTextEditor(DependencyObject d) { return (ITextEditor)d.GetValue(TextEditorProperty); }
    /// <summary>
    /// Sets the value of the <see cref="TextEditorProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">any <c>TextBlock</c> within the visual tree of the part</param>
    /// <param name="v">an <see cref="Northwoods.GoXam.Tool.ITextEditor"/></param>
    public static void SetTextEditor(DependencyObject d, ITextEditor v) { d.SetValue(TextEditorProperty, v); }

    /// <summary>
    /// Identifies the <c>TextAspectRatio</c> attached dependency property
    /// that may be on any <c>TextBlock</c>.
    /// </summary>
    public static readonly DependencyProperty TextAspectRatioProperty;
    /// <summary>
    /// Gets the value of the <see cref="TextAspectRatioProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">any <c>TextBlock</c> within the visual tree of the part</param>
    /// <returns>By default this returns 1.5</returns>
    public static double GetTextAspectRatio(DependencyObject d) { return (double)d.GetValue(TextAspectRatioProperty); }
    /// <summary>
    /// Sets the value of the <see cref="TextAspectRatioProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">any <c>TextBlock</c> within the visual tree of the part</param>
    /// <param name="v">a positive value that is the desired ratio of width to height</param>
    public static void SetTextAspectRatio(DependencyObject d, double v) { d.SetValue(TextAspectRatioProperty, v); }


    /// <summary>
    /// Identifies the <see cref="DragOverSnapEnabled"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty DragOverSnapEnabledProperty;
    /// <summary>
    /// Gets the value of the <see cref="DragOverSnapEnabled"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static bool GetDragOverSnapEnabled(DependencyObject d) { return (bool)d.GetValue(DragOverSnapEnabledProperty); }
    /// <summary>
    /// Sets the value of the <see cref="DragOverSnapEnabled"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetDragOverSnapEnabled(DependencyObject d, bool v) { d.SetValue(DragOverSnapEnabledProperty, v); }
    /// <summary>
    /// Gets or sets whether dragging any parts over this part causes their position to be snapped to grid points.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    public bool DragOverSnapEnabled {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetDragOverSnapEnabled(elt); else return false; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetDragOverSnapEnabled(elt, value); }
    }
    private static void OnDragOverSnapEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
    }

    /// <summary>
    /// Identifies the <see cref="DragOverSnapCellSize"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty DragOverSnapCellSizeProperty;
    /// <summary>
    /// Gets the value of the <see cref="DragOverSnapCellSize"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static Size GetDragOverSnapCellSize(DependencyObject d) { return (Size)d.GetValue(DragOverSnapCellSizeProperty); }
    /// <summary>
    /// Sets the value of the <see cref="DragOverSnapCellSize"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetDragOverSnapCellSize(DependencyObject d, Size v) { d.SetValue(DragOverSnapCellSizeProperty, v); }
    /// <summary>
    /// Gets or sets the size of the grid cell used when snapping during a drag.
    /// </summary>
    /// <value>
    /// The default <c>Size</c> is 10x10 in model units.
    /// Any new width or height value must be positive but non-infinite numbers.
    /// </value>
    public Size DragOverSnapCellSize {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetDragOverSnapCellSize(elt); else return new Size(10, 10); }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetDragOverSnapCellSize(elt, value); }
    }
    private static void OnDragOverSnapCellSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Size sz = (Size)e.NewValue;
      if (sz.Width <= 0 || sz.Height <= 0 || Double.IsNaN(sz.Width) || Double.IsNaN(sz.Height) || Double.IsInfinity(sz.Width) || Double.IsInfinity(sz.Height)) {
        Diagram.Error("New Size value for DragOverSnapCellSize must have positive non-infinite dimensions, preferably greater than one, not: " + sz.ToString());
      }
    }

    /// <summary>
    /// Identifies the <see cref="DragOverSnapCellSpot"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty DragOverSnapCellSpotProperty;
    /// <summary>
    /// Gets the value of the <see cref="DragOverSnapCellSpot"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static Spot GetDragOverSnapCellSpot(DependencyObject d) { return (Spot)d.GetValue(DragOverSnapCellSpotProperty); }
    /// <summary>
    /// Sets the value of the <see cref="DragOverSnapCellSpot"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetDragOverSnapCellSpot(DependencyObject d, Spot v) { d.SetValue(DragOverSnapCellSpotProperty, v); }
    /// <summary>
    /// Gets or sets the <see cref="Spot"/> that specifies what point in the grid cell dragged parts snap to.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Spot.TopLeft"/>,
    /// which means parts get snapped directly to the grid points.
    /// A new value must be a specific spot: <see cref="Spot.IsSpot"/> must be true for any new value.
    /// </value>
    public Spot DragOverSnapCellSpot {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetDragOverSnapCellSpot(elt); else return Spot.TopLeft; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetDragOverSnapCellSpot(elt, value); }
    }
    private static void OnDragOverSnapCellSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Spot spot = (Spot)e.NewValue;
      if (spot.IsNoSpot) {
        Diagram.Error("New Spot value for DragOverSnapCellSpot must be a specific spot, not: " + spot.ToString());
      }
    }

    /// <summary>
    /// Identifies the <see cref="DragOverSnapOrigin"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty DragOverSnapOriginProperty;
    /// <summary>
    /// Gets the value of the <see cref="DragOverSnapOrigin"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static Point GetDragOverSnapOrigin(DependencyObject d) { return (Point)d.GetValue(DragOverSnapOriginProperty); }
    /// <summary>
    /// Sets the value of the <see cref="DragOverSnapOrigin"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetDragOverSnapOrigin(DependencyObject d, Point v) { d.SetValue(DragOverSnapOriginProperty, v); }
    /// <summary>
    /// Gets or sets the snapping grid's coordinates origin or offset.
    /// </summary>
    /// <value>
    /// The default value is <c>Point(0, 0)</c>,
    /// which means that the grid starts at the same point as the position
    /// of the node's <see cref="Node.LocationElement"/>.
    /// The value must have numbers that are not infinite.
    /// </value>
    public Point DragOverSnapOrigin {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetDragOverSnapOrigin(elt); else return new Point(Double.NaN, Double.NaN); }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetDragOverSnapOrigin(elt, value); }
    }
    private static void OnDragOverSnapOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Point p = (Point)e.NewValue;
      if (Double.IsNaN(p.X) || Double.IsNaN(p.Y) || Double.IsInfinity(p.X) || Double.IsInfinity(p.Y)) {
        Diagram.Error("New Point value for DragOverSnapOrigin must have non-infinite dimensions, not: " + p.ToString());
      }
    }


    ///// <summary>
    ///// Identifies the <see cref="DragOverBehavior"/> attached dependency property
    ///// that must only be on the root <see cref="Part.VisualElement"/>.
    ///// </summary>
    //public static readonly DependencyProperty DragOverBehaviorProperty;
    ///// <summary>
    ///// Gets the value of the <see cref="DragOverBehavior"/> attached dependency property.
    ///// </summary>
    ///// <param name="d"></param>
    ///// <returns></returns>
    //public static DragOverBehavior GetDragOverBehavior(DependencyObject d) { return (DragOverBehavior)d.GetValue(DragOverBehaviorProperty); }
    ///// <summary>
    ///// Sets the value of the <see cref="DragOverBehavior"/> attached dependency property.
    ///// </summary>
    ///// <param name="d"></param>
    ///// <param name="v"></param>
    //public static void SetDragOverBehavior(DependencyObject d, DragOverBehavior v) { d.SetValue(DragOverBehaviorProperty, v); }
    ///// <summary>
    ///// Gets or sets the behavior of stationary <see cref="Part"/>s over which the <see cref="Northwoods.GoXam.Tool.DraggingTool"/>
    ///// drags the selection during a move or copy.
    ///// </summary>
    ///// <value>
    ///// The default value is <see cref="Northwoods.GoXam.Tool.DragOverBehavior.None"/>.
    ///// </value>
    //public DragOverBehavior DragOverBehavior {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
    //  get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetDragOverBehavior(elt); else return DragOverBehavior.None; }
    //  set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetDragOverBehavior(elt, value); }
    //}
    //private static void OnDragOverBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
    //}


    /// <summary>
    /// Identifies the <see cref="DropOntoBehavior"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty DropOntoBehaviorProperty;
    /// <summary>
    /// Gets the value of the <see cref="DropOntoBehavior"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static DropOntoBehavior GetDropOntoBehavior(DependencyObject d) { return (DropOntoBehavior)d.GetValue(DropOntoBehaviorProperty); }
    /// <summary>
    /// Sets the value of the <see cref="DropOntoBehavior"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetDropOntoBehavior(DependencyObject d, DropOntoBehavior v) { d.SetValue(DropOntoBehaviorProperty, v); }
    /// <summary>
    /// Gets or sets the behavior when the <see cref="Northwoods.GoXam.Tool.DraggingTool"/>
    /// drops the selection after a move or copy.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Northwoods.GoXam.DropOntoBehavior.None"/>.
    /// </value>
    public DropOntoBehavior DropOntoBehavior {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetDropOntoBehavior(elt); else return DropOntoBehavior.None; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetDropOntoBehavior(elt, value); }
    }
    private static void OnDropOntoBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
    }


    /// <summary>
    /// Identifies the <see cref="IsDropOntoAccepted"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsDropOntoAcceptedProperty;
    /// <summary>
    /// Gets or sets whether the mouse is over this part during a <see cref="Northwoods.GoXam.Tool.DraggingTool"/> drag
    /// and <see cref="DraggingTool.DropOntoEnabled"/> is true.
    /// </summary>
    public bool IsDropOntoAccepted {
      get { return (bool)GetValue(IsDropOntoAcceptedProperty); }
      set { SetValue(IsDropOntoAcceptedProperty, value); }
    }
    private static void OnIsDropOntoAcceptedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Part part = (Part)d;
      part.OnIsDropOntoAcceptedChanged();
    }

    /// <summary>
    /// This virtual method is called whenever the value of <see cref="IsDropOntoAccepted"/> has changed.
    /// </summary>
    /// <remarks>
    /// In Silverlight, if the <see cref="Part.VisualElement"/> is a <c>Control</c>,
    /// this will also call <c>VisualStateManager.GoToState</c> with a new state
    /// of either "DraggedOver" or "Normal".
    /// </remarks>
    protected virtual void OnIsDropOntoAcceptedChanged() {

      Control control = this.VisualElement as Control;
      if (control != null) {
        VisualStateManager.GoToState(control, (this.IsDropOntoAccepted ? "DraggedOver" : "Normal"), true);
      }

    }


    /// <summary>
    /// Identifies the <see cref="ResizeCellSize"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty ResizeCellSizeProperty;
    /// <summary>
    /// Gets the value of the <see cref="ResizeCellSize"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>

    [TypeConverter(typeof(Route.SizeConverter))]

    public static Size GetResizeCellSize(DependencyObject d) { return (Size)d.GetValue(ResizeCellSizeProperty); }
    /// <summary>
    /// Sets the value of the <see cref="ResizeCellSize"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>

    [TypeConverter(typeof(Route.SizeConverter))]

    public static void SetResizeCellSize(DependencyObject d, Size v) { d.SetValue(ResizeCellSizeProperty, v); }
    /// <summary>
    /// Gets or sets the multiple used to resize.
    /// </summary>
    /// <value>
    /// The default <c>Size</c> is NaNxNaN in model units.
    /// A value of NaN means that that the resize width or height multiple is taken from
    /// the <see cref="ResizingTool"/> or from a node behind it that has <see cref="DragOverSnapEnabled"/>
    /// (the <see cref="DragOverSnapCellSize"/>)
    /// or from the diagram if it has <see cref="Northwoods.GoXam.Diagram.GridSnapEnabled"/>
    /// (the <see cref="Northwoods.GoXam.Diagram.GridSnapCellSize"/>).
    /// Any new width or height value must be a positive non-infinite number or NaN.
    /// </value>

    [TypeConverter(typeof(Route.SizeConverter))]

    public Size ResizeCellSize {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetResizeCellSize(elt); else return new Size(10, 10); }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetResizeCellSize(elt, value); }
    }
    private static void OnResizeCellSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Size sz = (Size)e.NewValue;
      if (sz.Width <= 0 || sz.Height <= 0 || Double.IsInfinity(sz.Width) || Double.IsInfinity(sz.Height)) {
        Diagram.Error("New Size value for ResizeCellSize must have positive non-infinite dimensions, preferably greater than one, not: " + sz.ToString());
      }
    }


    /// <summary>
    /// Identifies the <see cref="Text"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty TextProperty;  // must be on root visual element
    /// <summary>
    /// Gets the value of the <see cref="Text"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static String GetText(DependencyObject d) { return (String)d.GetValue(TextProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Text"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetText(DependencyObject d, String v) { d.SetValue(TextProperty, v); }
    /// <summary>
    /// Gets or sets the <see cref="Text"/> string representing this part.
    /// </summary>
    /// <value>
    /// The default value is an empty string.
    /// </value>
    public String Text {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetText(elt); else return ""; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetText(elt, value); }
    }


    /// <summary>
    /// Identifies the <see cref="InDiagramBounds"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty InDiagramBoundsProperty;
    /// <summary>
    /// Gets the value of the <see cref="InDiagramBounds"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetInDiagramBounds(DependencyObject d) { return (bool)d.GetValue(InDiagramBoundsProperty); }
    /// <summary>
    /// Sets the value of the <see cref="InDiagramBounds"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetInDiagramBounds(DependencyObject d, bool v) { d.SetValue(InDiagramBoundsProperty, v); }
    private static void OnInDiagramBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Part part = VerifyParentAs<Part>(d, e);
      if (part != null) {
        DiagramPanel panel = part.Panel;
        if (panel != null) panel.InvokeUpdateDiagramBounds("InDiagramBounds");
      }
    }
    /// <summary>
    /// Gets or sets whether this part is included or is ignored in the computation of the diagram bounds,
    /// by <see cref="DiagramPanel.ComputeDiagramBounds"/>.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <remarks>
    /// You might set this to false for some kinds of background objects.
    /// </remarks>
    public bool InDiagramBounds {
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetInDiagramBounds(elt); else return true; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetInDiagramBounds(elt, value); }
    }

    internal /*?? public virtual */ bool CanIncludeInDiagramBounds() {
      if (this.Visibility != Visibility.Visible) return false;
      Layer layer = this.Layer;
      if (layer == null || layer.IsTemporary) return false;
      return this.InDiagramBounds;
    }


    /// <summary>
    /// Identifies the <see cref="LayoutId"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty LayoutIdProperty;  // must be on root visual element
    /// <summary>
    /// Gets the value of the <see cref="LayoutId"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static String GetLayoutId(DependencyObject d) { return (String)d.GetValue(LayoutIdProperty); }
    /// <summary>
    /// Sets the value of the <see cref="LayoutId"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetLayoutId(DependencyObject d, String v) { d.SetValue(LayoutIdProperty, v); }
    /// <summary>
    /// Gets or sets the <see cref="Northwoods.GoXam.Layout.DiagramLayout.Id"/> which identifies
    /// the layout(s) in a <see cref="Northwoods.GoXam.Layout.MultiLayout"/> in which this part participates.
    /// </summary>
    public String LayoutId {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetLayoutId(elt); else return ""; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetLayoutId(elt, value); }
    }


    // invalidation/updating

    internal void InvalidateVisual(UIElement elt) {
      if (elt == null) elt = this.VisualElement;
      if (elt != null) {

        UIElement fe = elt;
        while (fe != null && fe != this) {
          Part.ClearCachedValues(fe);
          fe.InvalidateMeasure();
          fe = Diagram.FindParent<UIElement>(fe);
        }












        DiagramPanel panel = this.Panel;
        if (panel != null) {
          // request calls to Measure and Arrange by the DiagramPanel
          panel.Invalidate(this, false);
        }
      }
    }

    /// <summary>
    /// Re-measure and re-arrange this part in the near future.
    /// </summary>
    public void Remeasure() {  // and rearrange
      Group group = this as Group;
      if (group != null) {
        GroupPanel gp = group.GroupPanel;
        if (gp != null) {
          InvalidateVisual(gp);
          return;
        }
      }
      InvalidateVisual(null);
    }

    /// <summary>
    /// Re-measure and re-arrange this part right now, even if it's not ready.
    /// </summary>
    internal void RemeasureNow() {  // and rearrange now too
      EnsureOnscreen();
      this.ValidMeasure = false;
      this.ValidArrange = false;
      MeasureArrangeUpdate(this.Panel);
      Node node = this as Node;
      if (node != null) node.UpdateBounds();
    }

    private int InternalFlags { get; set; }
    // Part flags:
    private const int ValidMeasureFlag         = 0x0001;
    private const int ValidArrangeFlag         = 0x0002;
    private const int IsMeasuringArrangingFlag = 0x0004;
    private const int OffscreenFlag            = 0x0008;
    private const int NeedsReroutingFlag       = 0x0010;

    internal bool ValidMeasure { get { return (this.InternalFlags & ValidMeasureFlag) != 0; } set { if (value) this.InternalFlags |= ValidMeasureFlag; else this.InternalFlags &= ~ValidMeasureFlag; } }
    internal bool ValidArrange { get { return (this.InternalFlags & ValidArrangeFlag) != 0; } set { if (value) this.InternalFlags |= ValidArrangeFlag; else this.InternalFlags &= ~ValidArrangeFlag; } }
    //?? optimization && hack for initialization of links inside LinkPanels
    internal bool IsMeasuringArranging { get { return (this.InternalFlags & IsMeasuringArrangingFlag) != 0; } set { if (value) this.InternalFlags |= IsMeasuringArrangingFlag; else this.InternalFlags &= ~IsMeasuringArrangingFlag; } }
    internal bool Offscreen { get { return (this.InternalFlags & OffscreenFlag) != 0; } set { if (value) this.InternalFlags |= OffscreenFlag; else this.InternalFlags &= ~OffscreenFlag; } }
    internal bool NeedsRerouting { get { return (this.InternalFlags & NeedsReroutingFlag) != 0; } set { if (value) this.InternalFlags |= NeedsReroutingFlag; else this.InternalFlags &= ~NeedsReroutingFlag; } }

    internal void InvalidateRelationships(String why, bool arrangeonly) {  //?? why
      if (this.IsMeasuringArranging) return;

      // always invalidate related parts
      if (Part.InvalidatingRelationships > 2) {
        Diagram.InvokeLater(this, () => { InvalidateRelationships(why, arrangeonly); });
        return;
      }

      Part.InvalidatingRelationships++;
      InvalidateOtherPartRelationships(arrangeonly);
      // but for this part, maybe it's already been invalidated
      if (!this.ValidMeasure ||
          (arrangeonly && !this.ValidArrange)) {
        Part.InvalidatingRelationships--;
        return;  // already invalidated
      }
      //if (!(this is Adornment)) Diagram.Debug("?" + why + " " + arrangeonly.ToString() + " " + (this.Data != null ? this.Data.ToString() : ToString()));
      DiagramPanel panel = this.Panel;
      if (panel != null) {
        // request DiagramPanel to maybe Measure and to definitely Arrange
        panel.Invalidate(this, arrangeonly);
      }
      Part.InvalidatingRelationships--;
    }
    internal void InvalidateRelationships(String why) { InvalidateRelationships(why, false); }
    private static int InvalidatingRelationships { get; set; }

    /// <summary>
    /// Declare that this part and any links connected to this node or any groups related to this part
    /// are now invalid and should be re-computed, re-measured, and re-arranged.
    /// </summary>
    public void InvalidateRelationships() {

      Node node = this as Node;
      if (node != null) {
        foreach (UIElement e in node.Ports) {
          ClearCachedValues(e);
        }
      }

      InvalidateRelationships("InvalidateRelationships", false);
    }

    internal abstract void InvalidateOtherPartRelationships(bool arrangeonly);
    internal abstract bool IsReadyToMeasureArrange();
    internal abstract void MeasureMore();

    // a quick test used by DiagramPanel.MinimizeVisuals
    internal abstract bool IntersectsRect(Rect b);

    internal void MeasureArrangeUpdate(DiagramPanel panel) {
      this.IsMeasuringArranging = true;
      if (!this.ValidMeasure) {
        //Diagram.Debug("measure- " + Diagram.Str(this) + " " + Diagram.Str(this.Bounds) + ((this is Adornment) ? " adornment" : ""));
        MeasureMore();  // depending on the type of part, do more stuff
        InvalidateMeasure();  // make sure it's invalid before Measure'ing
        Measure(Geo.Unlimited);
        this.ValidMeasure = true;  // mark as valid
        //Diagram.Debug("measure+ " + Diagram.Str(this) + " " + Diagram.Str(this.DesiredSize) + " " + this.Visible.ToString());
      }
      if (!this.ValidArrange) {
        //Diagram.Debug("arrange- " + Diagram.Str(this) + " " + Diagram.Str(this.Bounds) + ((this is Adornment) ? " adornment" : ""));
        Rect b = this.Bounds;
        Size sz = this.DesiredSize;
        b.Width = sz.Width;
        b.Height = sz.Height;

        Group group = this as Group;
        if (group != null) group.ArrangedBounds = b;

        // don't Arrange when Bounds.Location.X or .Y IsNaN
        if (Double.IsNaN(b.X)) b.X = -999999;
        if (Double.IsNaN(b.Y)) b.Y = -999999;
        InvalidateArrange();  // make sure it's invalid before Arrange'ing
        Arrange(b);
        this.ValidArrange = true;  // mark as valid
        //Diagram.Debug("arrange+ " + Diagram.Str(this) + " " + Diagram.Str(b));
      }
      UpdateAdornments();
      this.IsMeasuringArranging = false;
    }

    internal void InvalidateLayout(LayoutChange why) {
      // no side-effects during undo/redo
      IDiagramModel model = this.Model;
      if (model != null && model.IsChangingModel) return;
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      LayoutManager mgr = diagram.LayoutManager;
      if (mgr != null) mgr.InvalidateLayout(this, why);
    }


    // convenience properties and methods for navigation

    /// <summary>
    /// Search the visual tree for this part and return the first one for 
    /// which the given predicate <paramref name="pred"/> is true.
    /// </summary>
    /// <param name="pred">a predicate of type <c>Predicate&lt;FrameworkElement&gt;</c></param>
    /// <returns>a <c>FrameworkElement</c> or null</returns>
    public FrameworkElement FindDescendant(Predicate<FrameworkElement> pred) {
      return Part.FindElementDownFrom(this, pred);
    }

    /// <summary>
    /// Search the visual tree for this part and return the first one that has a <c>Name</c>
    /// exactly matching the given <paramref name="name"/>.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>a <c>FrameworkElement</c>, or null</returns>
    public FrameworkElement FindNamedDescendant(String name) {
      return Part.FindElementDownFrom(this, x => x.Name == name);
    }

    internal static FrameworkElement FindElementDownFrom(FrameworkElement elt, Predicate<FrameworkElement> pred) {
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(elt); i++) {
        FrameworkElement child = VisualTreeHelper.GetChild(elt, i) as FrameworkElement;
        if (child == null) continue;
        if (pred(child)) return child;
        FrameworkElement found = FindElementDownFrom(child, pred);
        if (found != null) return found;
      }
      return null;
    }


    /// <summary>
    /// This static method takes an element and walks up the visual tree
    /// looking for an element of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">frequently <c>typeof(Node)</c></typeparam>
    /// <param name="elt"></param>
    /// <returns>
    /// the <paramref name="elt"/> if it is of type <typeparamref name="T"/>,
    /// or else one of its parent visuals of that type,
    /// or else null if no such element was found
    /// </returns>
    public static T FindAncestor<T>

      (UIElement elt) where T : UIElement



 {
      return Diagram.FindAncestorOrSelf<T>(elt);
    }

    /// <summary>
    /// This static predicate is true if the given <paramref name="elt"/>'s
    /// <c>Visibility</c> is <c>Visible</c> and each of its visual parents
    /// are also visible elements, up to the containing <see cref="Part"/>.
    /// </summary>
    /// <param name="elt"></param>
    /// <returns></returns>
    /// <remarks>
    /// This predicate ignores the actual location or appearance (except Visibility) of the
    /// part that the given element is part of, as well as ignoring all
    /// properties of the <see cref="Layer"/>, <see cref="Panel"/>, or <see cref="Diagram"/>.
    /// </remarks>
    public static bool IsVisibleElement(UIElement elt) {
      if (elt == null) return false;

      UIElement e = elt;
      while (e != null) {
        if (e.Visibility != Visibility.Visible) return false;
        if (e is Part) return true;
        e = VisualTreeHelper.GetParent(e) as UIElement;
      }









      return true;
    }


    internal abstract void PropagateVisible(bool vis);


    /// <summary>
    /// Find the top-level <see cref="Part"/> for this part.
    /// </summary>
    /// <returns>
    /// This may often return itself, and should not return null.
    /// </returns>
    /// <remarks>
    /// If this part is a member of a group, return that group's top-level part.
    /// If this part is a label node for a link, return that link's top-level part.
    /// </remarks>
    public Part FindTopLevelPart() {
      return FindTopLevelPart(this);
    }

    private static Part FindTopLevelPart(Part p) {
      Group sg = p.ContainingSubGraph;  //?? what if member of more than one group
      if (sg != null) return FindTopLevelPart(sg);
      Node n = p as Node;
      if (n != null) {
        Link ll = n.LabeledLink;
        if (ll != null) return FindTopLevelPart(ll);
      }
      return p;
    }

    /// <summary>
    /// This property is true when this part is not member of any group node
    /// nor is it a label node for a link.
    /// </summary>
    public bool IsTopLevel {
      get {
        if (this.ContainingGroups.FirstOrDefault() != null) return false;
        Node n = this as Node;
        if (n != null && n.IsLinkLabel) return false;
        return true;
      }
    }

    /// <summary>
    /// This predicate is true if this part is a member of the given <paramref name="part"/>.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    /// <remarks>
    /// If this part is a member of a group, this predicate is true if the group is the same as <paramref name="part"/>.
    /// Otherwise see if the group is contained by <paramref name="part"/>.
    /// If this part is a label for a link, see if the link is contained by <paramref name="part"/>.
    /// A part cannot be contained by itself.
    /// </remarks>
    public bool IsContainedBy(Part part) {
      return IsContainedBy(this, part);
    }

    private static bool IsContainedBy(Part p, Part part) {
      if (p == part || part == null) return false;
      if (p.ContainingGroups.Any(g => g == part || g.IsContainedBy(part))) return true;
      Node n = p as Node;
      if (n != null) {
        Link ll = n.LabeledLink;
        if (ll != null) return IsContainedBy(ll, part);
      }
      return false;
    }

    /// <summary>
    /// Find the group that contains both this part and another one.
    /// </summary>
    /// <param name="other"></param>
    /// <returns>
    /// This returns null if the two parts are unrelated in the hierarchy of part membership.
    /// If non-null, the result is a <see cref="Group"/>.
    /// </returns>
    public Group FindCommonContainingSubGraph(Part other) {
      return FindCommonContainingSubGraph(this, other);
    }

    private static Group FindCommonContainingSubGraph(Part a, Part b) {
      if (a == null) return null;
      if (b == null) return null;
      Group asg = a.ContainingSubGraph;
      if (asg == null) return null;
      if (a == b) return asg;
      Group bsg = b.ContainingSubGraph;
      if (bsg == null) return null;
      if (asg == bsg) return bsg;
      if (IsContainedBy(b, asg)) return asg;
      if (IsContainedBy(a, bsg)) return bsg;
      return FindCommonContainingSubGraph(asg, bsg);
    }


    /// <summary>
    /// Get a collection of <see cref="Group"/>s of which this part is a member.
    /// </summary>
    /// <remarks>
    /// This property is useful when the model is an <see cref="IGroupsModel"/>,
    /// including any <see cref="ISubGraphModel"/>.
    /// </remarks>
    public abstract IEnumerable<Group> ContainingGroups { get; }

    /// <summary>
    /// Get the one <see cref="Group"/> (a "subgraph") that this part is a member of, if any.
    /// </summary>
    /// <remarks>
    /// This property is only useful when the model is an <see cref="ISubGraphModel"/>.
    /// </remarks>
    public abstract Group ContainingSubGraph { get; }


    internal static bool IsTransformed(UIElement elt) {
      Point orig = elt.RenderTransformOrigin;
      if (orig.X != 0 || orig.Y != 0) return true;
      Transform xfm = elt.RenderTransform;
      if (xfm == null) return false;
      MatrixTransform mx = xfm as MatrixTransform;
      if (mx == null) return true;
      return !mx.Matrix.IsIdentity;
    }



    /// <summary>
    /// Compute the point of a <paramref name="spot"/> in
    /// a <c>FrameworkElement</c> that is within the visual tree of this part,
    /// relative to the top-left corner of this part.
    /// </summary>
    /// <param name="elt">a <c>FrameworkElement</c> within this part</param>
    /// <param name="spot">
    /// a <see cref="Spot"/>;
    /// if the spot <see cref="Spot.IsNoSpot"/> assume <see cref="Spot.Center"/> instead.
    /// </param>
    /// <returns>a <c>Point</c> in model coordinates, taking any transforms into account, assuming the bounds are at (0,0)</returns>
    /// <remarks>
    /// This can produce a useful value even when the part has not yet been positioned,
    /// i.e. with its (X,Y) being (NaN, NaN), if the part is in the rooted visual tree.
    /// </remarks>
    /// <seealso cref="GetElementPoint"/>
    /// <seealso cref="GetElementBounds"/>
    public Point GetRelativeElementPoint(FrameworkElement elt, Spot spot) {
      Spot s = spot;
      if (s.IsNoSpot) s = Spot.Center;
      Size sz = GetEffectiveSize(elt);
      if (elt == null || elt == this || (elt == this.VisualElement && !IsTransformed(elt))) {
        return new Point(sz.Width*s.X + s.OffsetX, sz.Height*s.Y + s.OffsetY);
      } else {
        Point pt = new Point(sz.Width*s.X + s.OffsetX, sz.Height*s.Y + s.OffsetY);
        GeneralTransform tr = DiagramPanel.CoordinatesTransform(this, elt);
        if (tr != null) pt = tr.Transform(pt);
        return pt;
      }
    }
    
    /// <summary>
    /// Get the actual size of an element, or its desired size if it hasn't been arranged yet,
    /// without measuring or arranging the element.
    /// </summary>
    /// <param name="e">an element that is in the visual tree of this part</param>
    /// <returns></returns>
    internal Size GetEffectiveSize(FrameworkElement e) {
      if (e == null) e = this.VisualElement;
      if (e == null) return new Size(0, 0);

      Group group = this as Group;
      if (group != null) {
        Size absz = new Size(group.ArrangedBounds.Width, group.ArrangedBounds.Height);
        if (absz.Width > 0 && absz.Height > 0 && e == this.VisualElement) {
          //Diagram.Debug(Diagram.Str(this) + " actual: " + Diagram.Str(new Size(e.ActualWidth, e.ActualHeight)) + " desired: " + Diagram.Str(e.DesiredSize) + " arranged: " + Diagram.Str(absz) + " offscreen: " + Diagram.Str(Part.GetOffscreenChildSize(e)));
          return absz;
        }
      }

      double w = e.ActualWidth;
      double h = e.ActualHeight;
      if (w == 0 || h == 0) {  // might not yet have been arranged, so Actual... values might be zero
        Size sz = e.DesiredSize;
        if (w == 0) w = sz.Width;
        if (h == 0) h = sz.Height;
        if (w == 0 && h == 0 && e == this.VisualElement) {  // or if measure was invalidated, maybe DesiredSize is zero (Silverlight)
          Rect b = this.Bounds;
          w = b.Width;
          h = b.Height;
        }

        else if (w == 0 || h == 0) {
          Size s = Part.GetOffscreenChildSize(e);
          if (w == 0 && !double.IsInfinity(s.Width) && !double.IsNaN(s.Width)) w = s.Width;
          if (h == 0 && !double.IsInfinity(s.Height) && !double.IsNaN(s.Height)) h = s.Height;
          if (w == 0 && h == 0) InvalidateVisual(e);
          return new Size(w, h);
        }
      }
      Part.SetOffscreenChildSize(e, new Size(w, h));



      return new Size(w, h);
    }


    ///// <summary>
    ///// Compute the size in model coordinates of a <c>FrameworkElement</c>
    ///// that is within the visual tree of this part.
    ///// </summary>
    ///// <param name="elt">a <c>FrameworkElement</c> within this part</param>
    ///// <returns>
    ///// a <c>Size</c> in model coordinates, taking any transforms into account
    ///// </returns>
    ///// <seealso cref="GetElementPoint"/>
    ///// <seealso cref="GetRelativeElementPoint"/>
    ///// <seealso cref="GetElementBounds"/>
    //public Size GetElementSize(FrameworkElement elt) {
    //  Size sz = GetEffectiveSize(elt);
    //  GeneralTransform tr = DiagramPanel.CoordinatesTransform(this, elt);
    //  Point pt1 = new Point(0, 0);
    //  Point pt3 = new Point(sz.Width, sz.Height);
    //  if (tr != null) {
    //    pt1 = tr.Transform(pt1);
    //    pt3 = tr.Transform(pt3);
    //  }
    //  return new Size(Math.Abs(pt3.X-pt1.X), Math.Abs(pt3.Y-pt1.Y));
    //}

    /// <summary>
    /// Compute the point in model coordinates of a <paramref name="spot"/> in
    /// a <c>FrameworkElement</c> that is within the visual tree of this part.
    /// </summary>
    /// <param name="elt">a <c>FrameworkElement</c> within this part</param>
    /// <param name="spot">
    /// a <see cref="Spot"/>;
    /// if the spot <see cref="Spot.IsNoSpot"/> assume <see cref="Spot.Center"/> instead.
    /// </param>
    /// <returns>
    /// a <c>Point</c> in model coordinates, taking any transforms into account;
    /// if the part has not yet been positioned, the (X,Y) position will be (NaN, NaN).
    /// </returns>
    /// <seealso cref="GetRelativeElementPoint"/>
    /// <seealso cref="GetElementBounds"/>
    public Point GetElementPoint(FrameworkElement elt, Spot spot) {
      Spot s = spot;
      if (s.IsNoSpot) s = Spot.Center;
      Rect b = this.Bounds;
      if (Double.IsNaN(b.X) || Double.IsNaN(b.Y)) {
        Point loc;
        Node node = this as Node;
        if (node != null) {
          loc = node.Location;
        } else {
          loc = new Point(0, 0);
        }
        if (Double.IsNaN(b.X) && !Double.IsNaN(loc.X)) b.X = loc.X;
        if (Double.IsNaN(b.Y) && !Double.IsNaN(loc.Y)) b.Y = loc.Y;
      }
      if (elt == null || elt == this || (elt == this.VisualElement && !IsTransformed(elt))) {
        return new Point(b.X + b.Width*s.X + s.OffsetX, b.Y + b.Height*s.Y + s.OffsetY);
      } else {
        Size sz = GetEffectiveSize(elt);
        Point pt = new Point(sz.Width*s.X + s.OffsetX, sz.Height*s.Y + s.OffsetY);
        GeneralTransform tr = DiagramPanel.CoordinatesTransform(this, elt);
        if (tr != null) pt = tr.Transform(pt);
        pt.X += b.X;
        pt.Y += b.Y;
        return pt;
      }
    }

    /// <summary>
    /// Compute the bounds in model coordinates of a <c>FrameworkElement</c>
    /// that is within the visual tree of this part.
    /// </summary>
    /// <param name="elt">a <c>FrameworkElement</c> within this part</param>
    /// <returns>
    /// a <c>Rect</c> in model coordinates, taking any transforms into account;
    /// if the part has not yet been positioned, the (X,Y) position will be (NaN, NaN).
    /// </returns>
    /// <seealso cref="GetElementPoint"/>
    /// <seealso cref="GetRelativeElementPoint"/>
    public Rect GetElementBounds(FrameworkElement elt) {
      Rect b = this.Bounds;
      if (Double.IsNaN(b.X) || Double.IsNaN(b.Y)) {
        Point loc;
        Node node = this as Node;
        if (node != null) {
          loc = node.Location;
        } else {
          loc = new Point(0, 0);
        }
        if (Double.IsNaN(b.X) && !Double.IsNaN(loc.X)) b.X = loc.X;
        if (Double.IsNaN(b.Y) && !Double.IsNaN(loc.Y)) b.Y = loc.Y;
      }
      if (elt == null || elt == this || (elt == this.VisualElement && !IsTransformed(elt))) return b;

      Size sz = GetEffectiveSize(elt);
      GeneralTransform tr = DiagramPanel.CoordinatesTransform(this, elt);
      Point pt1 = new Point(0, 0);
      Point pt2 = new Point(sz.Width, 0);
      Point pt3 = new Point(sz.Width, sz.Height);
      Point pt4 = new Point(0, sz.Height);
      if (tr != null) {
        pt1 = tr.Transform(pt1);
        pt2 = tr.Transform(pt2);
        pt3 = tr.Transform(pt3);
        pt4 = tr.Transform(pt4);
      }
      Rect r = new Rect(pt1, pt3);
      r = Geo.Union(r, pt2);
      r = Geo.Union(r, pt4);
      r.X += b.X;
      r.Y += b.Y;
      return r;
    }


    // assume there's no transform directly on any Part

    //??? make GetAngle & SetAngle smarter
    //?? too much dependence on RenderTransform instead of LayoutTransform, which doesn't exist in Silverlight

    /// <summary>
    /// Get the angle of an element in the visual tree of this part.
    /// </summary>
    /// <param name="elt">a <c>UIElement</c>, a visual child of this <c>Part</c></param>
    /// <returns>an angle in degrees, starting at zero in the direction of the positive X axis</returns>
    /// <remarks>
    /// At the present time this just looks at any <c>RenderTransform</c> that the element has.
    /// </remarks>
    public virtual double GetAngle(UIElement elt) {
      if (elt == null) elt = this.VisualElement;
      if (elt == null) return 0;
      Transform transform = elt.RenderTransform;
      RotateTransform rt = transform as RotateTransform;
      TransformGroup tg = transform as TransformGroup;
      if (rt == null && tg != null) {
        foreach (Transform t in tg.Children) {
          rt = t as RotateTransform;
          if (rt != null) break;
        }
      }
      if (rt != null) return rt.Angle;
      return 0;
    }

    /// <summary>
    /// Set the angle of an element in the visual tree of this part by
    /// creating or modifying a transform on that element.
    /// </summary>
    /// <param name="elt">the <c>UIElement</c> to be rotated, a visual child of this <c>Part</c></param>
    /// <param name="angle">a double between 0 and 360 degrees</param>
    /// <param name="focus">the spot of the rotation point; normally <c>Spot.Center</c></param>
    /// <remarks>
    /// At the present time this just modifies any <c>RenderTransform</c> that the element has,
    /// and gives it a new <c>RotateTransform</c> if needed.
    /// </remarks>
    public virtual void SetAngle(UIElement elt, double angle, Spot focus) {
      if (elt == null) elt = this.VisualElement;
      if (elt == null) return;
      Part.ClearCachedValues(elt);
      if (Double.IsNaN(angle) || Double.IsInfinity(angle)) angle = 0;
      if (!(this is Adornment)) {
        if (focus.IsNoSpot) focus = Spot.Center;
        elt.RenderTransformOrigin = new Point(focus.X, focus.Y);
      }

      // try to find an existing RotateTransform
      Transform transform = elt.RenderTransform;
      RotateTransform rt = transform as RotateTransform;
      TransformGroup tg = transform as TransformGroup;
      if (rt == null && tg != null) {
        foreach (Transform t in tg.Children) {
          rt = t as RotateTransform;
          if (rt != null) break;
        }
      }

      double oldangle = 0;
      if (rt != null



        ) {
        oldangle = rt.Angle;
        rt.Angle = angle;
      } else if (angle != 0) {
        rt = new RotateTransform();
        rt.Angle = angle;
        if (tg != null) {  // if there's a TransformGroup...



            int idx = -1;
            for (int i = 0; i < tg.Children.Count; i++) {
              if (tg.Children[i] is RotateTransform) { idx = i; break; }
            }
            if (idx >= 0) tg.Children[idx] = rt;












        } else {  // otherwise the RotateTransform is the only transform
          elt.RenderTransform = rt;
        }
      }
      Node node = this as Node;
      if (node != null) {
        if (elt == node.SelectionElement) {
          // save the angle as the RotationAngle attached property
          Node.SetRotationAngle(node.VisualElement, angle);
        } else {
          InvalidateRelationships("rotate");
        }
      }
    }

    /// <summary>
    /// This calls <see cref="SetAngle(UIElement, double, Spot)"/>
    /// to rotate the given element about the center.
    /// </summary>
    /// <param name="elt"></param>
    /// <param name="angle"></param>
    public void SetAngle(UIElement elt, double angle) { SetAngle(elt, angle, Spot.Center); }

  }  // end of Part


  /// <summary>
  /// A <c>Node</c> is a <see cref="Part"/> to which <see cref="Link"/>s may connect
  /// and which may be a member of a <see cref="Group"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Each <c>Node</c> has a <see cref="Location"/> property.
  /// By default this is the same as the <see cref="Part.Bounds"/> X,Y position.
  /// However, you can set the <see cref="Location"/> property,
  /// whereas <see cref="Part.Bounds"/> is a read-only property.
  /// </para>
  /// <para>
  /// Furthermore, the "location" need not be the same as the top-left point of the "bounds".
  /// Often there is a particular FrameworkElement in the node that is the featured element,
  /// and the "location" for the node is actually a point within that element.
  /// You can specify which element provides the node's location by setting the
  /// <see cref="LocationElementName"/> property.
  /// By default the element name is the empty string, meaning to use the visual element of the node.
  /// You can specify which point on that element is the actual location point
  /// by setting the <see cref="LocationSpot"/> property.
  /// By default the location spot is <see cref="Spot.TopLeft"/>,
  /// meaning the top-left point of the location element.
  /// Therefore, the combination of the two default location-specifying properties
  /// result in the location of a node to be the top-left point of the bounds of the whole node,
  /// the same as the position of the <see cref="Part.Bounds"/>.
  /// But it is moderately common to specify a particular shape or image as the location element name,
  /// and it is fairly common to use <see cref="Spot.Center"/> as the location spot.
  /// </para>
  /// <para>
  /// The <see cref="Location"/> property is a CLR property implemented
  /// using the <see cref="LocationProperty"/> attached dependency property
  /// on the node's <see cref="Part.VisualElement"/>.
  /// The reason that most of the node "properties" are implemented as attached dependency
  /// properties rather than regular CLR properties is that it allows for those property values to be data-bound.
  /// It is commonplace to specify the location using a data binding:
  /// <code>
  /// &lt;DataTemplate x:Key="MyNodeTemplate"&gt;
  ///   &lt;Border ...
  ///         go:Node.Location="{Binding Path=Data.Location, Mode=TwoWay}"&gt;
  ///     ...
  ///   &lt;/Border&gt;
  /// &lt;/DataTemplate&gt;
  /// </code>
  /// This data-binds the node's location with a property on the node data named "Location".
  /// Data binding not only keeps FrameworkElement properties in synch with data properties,
  /// but also helps support a model-view architecture and undo/redo.
  /// </para>
  /// <para>
  /// The <see cref="MinLocation"/> and <see cref="MaxLocation"/> properties help limit
  /// how far a node may be dragged.
  /// </para>
  /// <para>
  /// The <see cref="RotationAngle"/> property sets the angle of the node's <see cref="Part.SelectionElement"/>.
  /// </para>
  /// <para>
  /// The most common relationship supported by nodes is a node-to-node link relationship.
  /// There are a number of properties that return collections of <c>Node</c>s that a given
  /// node is connected with, and there are a number of properties that return collections of
  /// <see cref="Link"/>s that a given node connects with to other nodes.
  /// These properties are:
  /// <see cref="NodesConnected"/>, <see cref="NodesInto"/>, <see cref="NodesOutOf"/>,
  /// <see cref="LinksConnected"/>, <see cref="LinksInto"/>, and <see cref="LinksOutOf"/>.
  /// </para>
  /// <para>
  /// Creating new links is achieved by adding or modifying data in the diagram's model.
  /// The precise methods depend on whether the model is an
  /// <see cref="ILinksModel"/> or an <see cref="IConnectedModel"/> or an <see cref="ITreeModel"/>.
  /// </para>
  /// <para>
  /// Nodes also support the ability to provide logical and physical distinctions in the
  /// connection points that links use at a node, if the diagram's model supports it.
  /// These connections points are called "ports".
  /// Some models assume that there is only one port per node.
  /// By default that will be the node's <see cref="Part.VisualElement"/>.
  /// However, one can set the <see cref="PortIdProperty"/> attached property
  /// on any visual element to cause that FrameworkElement to be treated as a "port".
  /// In the case of a single port for a node, you should set the <c>PortId</c> as an empty string:
  /// <code>
  ///   ...
  ///     &lt;... ... go:Node.PortId="" ... &gt;
  ///   ...
  /// </code>
  /// You can get the FrameworkElement that is the default (single) port using the <see cref="Port"/> property.
  /// </para>
  /// <para>
  /// When a node has multiple ports, i.e. multiple FrameworkElements acting as
  /// separate connection points for links, you should set each port's <c>PortId</c>
  /// to a string value that is unique for the node.
  /// When there may be multiple ports on a node, you can get a collection of FrameworkElements
  /// representing ports using the <see cref="Ports"/> property.
  /// Use the <see cref="FindPort"/> method to find a particular port element by name.
  /// There are also methods for finding the collection of nodes or of links that are
  /// connected to a particular port element.
  /// </para>
  /// <para>
  /// There are also several attached properties that can be set to control how links
  /// connect to that FrameworkElement.  These include:
  /// <see cref="FromSpotProperty"/>, <see cref="ToSpotProperty"/>,
  /// <see cref="FromEndSegmentLengthProperty"/>, <see cref="ToEndSegmentLengthProperty"/>.
  /// There are also some attached properties that control whether users may
  /// create new links connecting to a port:
  /// <see cref="LinkableFromProperty"/>, <see cref="LinkableToProperty"/>,
  /// <see cref="LinkableSelfNodeProperty"/>, <see cref="LinkableDuplicatesProperty"/>,
  /// and <see cref="LinkableMaximumProperty"/>.
  /// </para>
  /// <para>
  /// Nodes also support the notion of group membership for models that implement
  /// <see cref="Northwoods.GoXam.Model.ISubGraphModel"/>.
  /// The <see cref="ContainingSubGraph"/> property returns the <see cref="Group"/> that
  /// a node is a member of, or null if there is no container group for that node.
  /// Changing the group membership of a node requires modifying the diagram's model.
  /// </para>
  /// <para>
  /// The <c>Node</c> class also supports the notion of expanding and collapsing a subtree
  /// starting at a given node.  There are two attached properties, <see cref="IsTreeExpandedProperty"/>
  /// and <see cref="WasTreeExpandedProperty"/>, that can be data-bound, as well as two methods,
  /// <see cref="CollapseTree"/> and <see cref="ExpandTree"/>.
  /// </para>
  /// <para>
  /// You may not apply any transforms to a <c>Node</c>; that is reserved to GoXam for future use.
  /// </para>
  /// </remarks>
  [System.Windows.Markup.ContentProperty("Content")]
  public class Node : Part {
    /// <summary>
    /// Constructs a <see cref="Node"/>.
    /// </summary>
    public Node() {
      // needed to keep size of groups up-to-date:
      this.HorizontalAlignment = HorizontalAlignment.Left;
      this.VerticalAlignment = VerticalAlignment.Top;
      this.SizeChanged += SizeChanged_Handler;
    }

    static Node() {
      // Dependency properties

      IsLinkLabelProperty = DependencyProperty.Register("IsLinkLabel", typeof(bool), typeof(Node),
        new FrameworkPropertyMetadata(false, OnIsLinkLabelChanged));

      // PartsModel properties:
      IdProperty = DependencyProperty.Register("Id", typeof(String), typeof(Node),
        new FrameworkPropertyMetadata(null, OnIdChanged));

      PartsModelContainingSubGraphProperty = DependencyProperty.Register("PartsModelContainingSubGraph", typeof(String), typeof(Node),
        new FrameworkPropertyMetadata(null, OnPartsModelContainingSubGraphChanged));

      // a transient property
      IsExpandedTreeProperty = DependencyProperty.Register("IsExpandedTree", typeof(bool), typeof(Node),
        new FrameworkPropertyMetadata(true, OnIsExpandedTreeChanged));

      // Attached properties

      // location properties:

      // The location of an object in the DiagramPanel's document.
      // Must be on the root visual element
      LocationProperty = DependencyProperty.RegisterAttached("Location", typeof(Point), typeof(Node),
        new FrameworkPropertyMetadata(new Point(Double.NaN, Double.NaN), OnLocationChanged));

      // Names the visual element that determines the Node.Location;
      // null or empty string means use the whole visual child tree.
      // Must be on the root visual element
      LocationElementNameProperty = DependencyProperty.RegisterAttached("LocationElementName", typeof(String), typeof(Node),
        new FrameworkPropertyMetadata("", OnLocationElementNameChanged));

      // Fractional point in the location element that is the actual Node.Location
      // defaults to top-left (0, 0)
      // Must be on the root visual element
      LocationSpotProperty = DependencyProperty.RegisterAttached("LocationSpot", typeof(Spot), typeof(Node),
        new FrameworkPropertyMetadata(Spot.TopLeft, OnLocationSpotChanged));

      // DraggingTool properties:
      // All must be on the root visual element
      MinLocationProperty = DependencyProperty.RegisterAttached("MinLocation", typeof(Point), typeof(Node),
        new FrameworkPropertyMetadata(new Point(Double.NegativeInfinity, Double.NegativeInfinity), OnMinLocationChanged));
      MaxLocationProperty = DependencyProperty.RegisterAttached("MaxLocation", typeof(Point), typeof(Node),
        new FrameworkPropertyMetadata(new Point(Double.PositiveInfinity, Double.PositiveInfinity), OnMaxLocationChanged));

      // Must be on the root visual element
      AvoidableProperty = DependencyProperty.RegisterAttached("Avoidable", typeof(bool), typeof(Node),
        new FrameworkPropertyMetadata(true, OnAvoidableChanged));  //?? promote to Part
      AvoidableMarginProperty = DependencyProperty.RegisterAttached("AvoidableMargin", typeof(Thickness), typeof(Node),
        new FrameworkPropertyMetadata(new Thickness(2), OnAvoidableMarginChanged));

      RotationAngleProperty = DependencyProperty.RegisterAttached("RotationAngle", typeof(double), typeof(Node),
        new FrameworkPropertyMetadata(0.0, OnRotationAngleChanged));

      // These must be on the root visual element:
      // see also: IsExpandedTree property
      IsTreeExpandedProperty = DependencyProperty.RegisterAttached("IsTreeExpanded", typeof(bool), typeof(Node),
        new FrameworkPropertyMetadata(true, OnIsTreeExpandedChanged));
      WasTreeExpandedProperty = DependencyProperty.RegisterAttached("WasTreeExpanded", typeof(bool), typeof(Node),
        new FrameworkPropertyMetadata(false, VerifyOnVisualElement<Node>));

      // port properties:
      // All port properties can be on any element representing a port,
      // and there can be more than one such element in a Node.
      // Port properties include PortId, From/ToSpot, From/ToEndSegmentLength, Linkable...

      // Must be on an element representing a port,
      // except that FindPort will return the VisualElement if no element's PortId matches the port parameter
      PortIdProperty = DependencyProperty.RegisterAttached("PortId", typeof(String), typeof(Node),
        new FrameworkPropertyMetadata(null, OnPortIdChanged));  // null means not a port, use "" as the default port name

      //?? internal attached property on each port
      PortInfoProperty = DependencyProperty.RegisterAttached("PortInfo", typeof(Knot), typeof(Node),
        new FrameworkPropertyMetadata(null, OnPortInfoChanged));
      //PortExtensionProperty = DependencyProperty.RegisterAttached("PortExtension", typeof(int), typeof(Node),
      //  new FrameworkPropertyMetadata(0, OnPortExtensionChanged));

      // Fractional point in the element representing a port;
      // defaults to Spot.None
      // Must be on an element representing a port
      FromSpotProperty = DependencyProperty.RegisterAttached("FromSpot", typeof(Spot), typeof(Node),
        new FrameworkPropertyMetadata(Spot.None, OnFromSpotChanged));

      // Fractional point in the element representing a port;
      // defaults to Spot.None
      // Must be on an element representing a port
      ToSpotProperty = DependencyProperty.RegisterAttached("ToSpot", typeof(Spot), typeof(Node),
        new FrameworkPropertyMetadata(Spot.None, OnToSpotChanged));

      // Must be on an element representing a port
      FromEndSegmentLengthProperty = DependencyProperty.RegisterAttached("FromEndSegmentLength", typeof(double), typeof(Node),
        new FrameworkPropertyMetadata(10.0, OnFromEndSegmentLengthChanged));
      // Must be on an element representing a port
      ToEndSegmentLengthProperty = DependencyProperty.RegisterAttached("ToEndSegmentLength", typeof(double), typeof(Node),
        new FrameworkPropertyMetadata(10.0, OnToEndSegmentLengthChanged));

      // LinkingTool properties: these affect LinkingToolBase.IsValid...
      // Must be on an element representing a port if the value is true;
      // if false, may be set on another element, to prevent the user drawing a new link from that element
      LinkableFromProperty = DependencyProperty.RegisterAttached("LinkableFrom", typeof(bool?), typeof(Node), new FrameworkPropertyMetadata(null));  //?? inheriting
      // Must be on an element representing a port if the value is true
      // if false, may be set on another element, to prevent the user drawing a new link to that element
      LinkableToProperty = DependencyProperty.RegisterAttached("LinkableTo", typeof(bool?), typeof(Node), new FrameworkPropertyMetadata(null));
      // Must be on an element representing a port
      LinkableSelfNodeProperty = DependencyProperty.RegisterAttached("LinkableSelfNode", typeof(bool), typeof(Node), new FrameworkPropertyMetadata(false));
      // Must be on an element representing a port
      LinkableDuplicatesProperty = DependencyProperty.RegisterAttached("LinkableDuplicates", typeof(bool), typeof(Node), new FrameworkPropertyMetadata(false));
      // Must be on an element representing a port
      LinkableMaximumProperty = DependencyProperty.RegisterAttached("LinkableMaximum", typeof(int), typeof(Node), new FrameworkPropertyMetadata(Int32.MaxValue));
    }









    internal void SortZOrder() {
      Group g = this as Group;
      if (g != null) {
        NodeLayer nlayer = this.Layer as NodeLayer;
        if (nlayer != null) nlayer.SortZOrder(g);
      }
    }


    /// <summary>
    /// For convenience in debugging,
    /// this returns the the <see cref="Part.Text"/> value if non-empty,
    /// or else <see cref="Id"/> if it is non-null,
    /// or else just this <c>ToString()</c>.
    /// </summary>
    /// <returns></returns>
    public override String ToString() {
      String prefix = "Node:";
      if (this is Group) prefix = "Group:";
      else if (this is Adornment) prefix = "Ad:";
      String txt = this.Text;
      if (txt != null && txt.Length > 0) return prefix + txt;
      String id = this.Id;
      if (id != null) return prefix + id;
      Object data = this.Data;
      if (data != null) {
        if (data is UIElement)
          return prefix + data.GetType().Name;
        else
          return prefix + data.ToString();
      }
      return base.ToString();
    }

    internal void AddBundle(LinkBundle bundle) {
      if (bundle == null) return;
      if (this.Bundles == null) {  // create list if needed
        this.Bundles = new List<LinkBundle>();
        this.Bundles.Add(bundle);
      } else {
        if (!this.Bundles.Contains(bundle)) {  // avoid duplicates in list
          this.Bundles.Add(bundle);
        }
      }
    }

    internal void RemoveBundle(LinkBundle bundle) {
      if (bundle == null) return;
      if (this.Bundles != null) {
        this.Bundles.Remove(bundle);
      }
    }

    internal LinkBundle FindBundle(Object thisparam, Node other, Object otherparam) {
      if (other == null) return null;
      if (this.Bundles == null) return null;
      //?? params should be compared by ILinksModel specific predicate
      return this.Bundles.FirstOrDefault(b => (b.Node1 == this && b.Node2 == other && IsEqualParams(b.Param1, thisparam) && IsEqualParams(b.Param2, otherparam)) ||
                                              (b.Node1 == other && b.Node2 == this && IsEqualParams(b.Param1, otherparam) && IsEqualParams(b.Param2, thisparam)));
    }
    private bool IsEqualParams(Object p1, Object p2) {
      if (p1 == p2) return true;
      return (p1 != null && p1.Equals(p2));
    }

    private List<LinkBundle> Bundles { get; set; }  // all LinkBundles connected to this node


    // invalidation/updating

    internal override void InvalidateOtherPartRelationships(bool arrangeonly) {
      // invalidate container groups, if any
      foreach (Group sg in this.ContainingGroups) {
        sg.Remeasure();
      }

      // invalidate all links connected to this node
      InvalidateConnectedLinks();
    }

    internal void InvalidateConnectedLinks() {
      foreach (Link link in this.LinksConnected) {
        // remove any cached side connection info
        FrameworkElement from = link.FromPort;
        if (from != null) SetPortInfo(from, null);
        FrameworkElement to = link.ToPort;
        if (to != null && to != from) SetPortInfo(to, null);
        // and invalidate that link
        link.InvalidateRelationships("link");
      }
    }

    internal void InvalidateConnectedLinks(HashSet<Part> ignore) {
      foreach (Link link in this.LinksConnected) {
        if (ignore.Contains(link)) continue;
        // remove any cached side connection info
        FrameworkElement from = link.FromPort;
        if (from != null) SetPortInfo(from, null);
        FrameworkElement to = link.ToPort;
        if (to != null && to != from) SetPortInfo(to, null);
        // and invalidate that link
        link.InvalidateRelationships("link");
      }
    }

    // a node is ready to be measured/arranged if is not a link label,
    // or if it is a link label if its link has been arranged
    internal override bool IsReadyToMeasureArrange() {
      if (!this.IsLinkLabel) return true;
      Link l = this.LabeledLink;
      if (l == null) return true;
      return l.ValidArrange;
    }

    internal override void MeasureMore() {

      Path path = this.VisualElement as Path;
      if (path != null && NodePanel.GetFigure(path) != NodeFigure.None) {
        // do what NodePanel.RenderNodeShape does
        path.Stretch = Stretch.Fill;
        path.Data = new NodeGeometry(path).GetGeometry();
        // this also has the side-effect of setting any default Spot1/Spot2 parameters
      }

    }

    internal void MaybeRemeasureNow() {
      Rect b = this.Bounds;
      if (Double.IsNaN(b.X) || Double.IsNaN(b.Y) || b.Width == 0 || b.Height == 0 || this is Group) {
        if (Double.IsNaN(b.X) || Double.IsNaN(b.Y)) this.Location = new Point(0, 0);
        RemeasureNow();  // in case it hasn't been measured/arranged yet, particularly for Groups that were just laid out
      }
    }


    internal override void PropagateVisible(bool vis) {
      Node visiblenode = this;
      while (visiblenode != null && !visiblenode.Visible) visiblenode = visiblenode.ContainingSubGraph;
      // now VISIBLENODE is a visible node, or null if it and no container is visible

      foreach (Link l in this.LinksConnected) {
        if (l.Visible ^ vis) {  // consider changing the visibility of a connected link
          // for the link L to be visible, VISIBLENODE must not be null
          bool allowed = visiblenode != null;
          if (allowed) {
            // for the link L to be visible, VISIBLEOTHER must not be null
            Node other = l.GetOtherNode(this);
            Node visibleother;
            if (other == this) {
              visibleother = visiblenode;
            } else {
              visibleother = other;
              while (visibleother != null && !visibleother.Visible) visibleother = visibleother.ContainingSubGraph;
            }
            // now VISIBLEOTHER is a visible node, or null if it and no container is visible
            allowed = visibleother != null;
          }
          if (allowed) {
            // for the link L to be visible, it must be a top-level link or inside a visible expanded subgraph
            Group sg = l.ContainingSubGraph;
            allowed = (sg == null || (sg.Visible && sg.IsExpandedSubGraph));
          }
          if (l.Visible != allowed) {
            l.Visible = allowed;
            l.PropagateVisible(allowed);
          }
        }
      }
    }


    internal override bool IntersectsRect(Rect b) {
      Rect r = this.Bounds;
      return !Double.IsNaN(r.X) && !Double.IsNaN(r.Y) && Geo.Intersects(b, r);
    }


    // called when the node's Size has changed
    private void SizeChanged_Handler(Object sender, SizeChangedEventArgs e) {
      //Diagram.Debug("Node.SizeChanged: " + Diagram.Str(this) + " " + e.PreviousSize.ToString() + " --> " + e.NewSize.ToString());

      Group group = this as Group;
      if (group != null) {
        group.ArrangedBounds = new Rect(group.ArrangedBounds.X, group.ArrangedBounds.Y, e.NewSize.Width, e.NewSize.Height);
      }

      UpdateBounds();
      Size prev = e.PreviousSize;

      if (prev.Width == 0 && prev.Height == 0) {
        Rect oldb = this.Bounds;
        prev.Width = oldb.Width;
        prev.Height = oldb.Height;
      }

      if (!Geo.IsApprox(prev.Width, e.NewSize.Width) || !Geo.IsApprox(prev.Height, e.NewSize.Height)) {
        InvalidateLayout(this is Group ? LayoutChange.GroupSizeChanged : LayoutChange.NodeSizeChanged);
        // just in case there's no layout scheduled, make sure we recompute the diagram bounds
        if (e.PreviousSize.Width == 0 && e.PreviousSize.Height == 0) {
          DiagramPanel panel = this.Panel;
          if (panel != null && CanIncludeInDiagramBounds()) panel.InvokeUpdateDiagramBounds("SizeChanged");
        }
      }
    }

    // Location or Size has changed, or the meaning of Location has changed (LocationElementName or LocationSpot)
    internal void UpdateBounds() {
      Point loc = this.Location;
      Rect b = this.Bounds;

      double x = loc.X;
      double y = loc.Y;
      if ((Double.IsNaN(x) || Double.IsNaN(y)) && (Double.IsNaN(b.X) || Double.IsNaN(b.Y))) {
        return;
      }

      FrameworkElement elt = this.VisualElement;
      Size sz = GetEffectiveSize(elt);
      double w = sz.Width;
      double h = sz.Height;

      Spot spot = this.LocationSpot;
      if (spot.IsNoSpot) spot = Spot.TopLeft;
      FrameworkElement target = this.LocationElement;
      Rect r;
      if (target != elt) {
        Size tsz = GetEffectiveSize(target);
        double tw = tsz.Width;
        double th = tsz.Height;
        Point cpt = new Point(tw*spot.X + spot.OffsetX, th*spot.Y + spot.OffsetY);
        GeneralTransform tr = DiagramPanel.CoordinatesTransform(this, target);
        if (tr != null) cpt = tr.Transform(cpt);
        r = new Rect(x-cpt.X, y-cpt.Y, w, h);
      } else {
        r = new Rect(x-w*spot.X-spot.OffsetX, y-h*spot.Y-spot.OffsetY, w, h);
      }
      //Diagram.Debug("UpdateBounds: " + Diagram.Str(this) + " " + b.ToString() + " --> " + r.ToString() + " " + Diagram.Str(loc));
      if (!Geo.IsApprox(r.X, b.X) || !Geo.IsApprox(r.Y, b.Y) || !Geo.IsApprox(r.Width, b.Width) || !Geo.IsApprox(r.Height, b.Height)) {
        //Diagram.Debug("  UpdatingBounds: " + Diagram.Str(this) + " " + Diagram.Str(r) + Diagram.Str(loc));
        this.Bounds = r;
        bool samesize = (Geo.IsApprox(w, b.Width) && Geo.IsApprox(h, b.Height));
        InvalidateRelationships("SetBounds", samesize);
      }
    }


    /// <summary>
    /// Initialize <see cref="IsExpandedTree"/> with the possibly bound value of <see cref="GetIsTreeExpanded"/>
    /// on this node's root visual element, and initialize the node's <see cref="Part.Bounds"/>.
    /// </summary>
    public override void OnApplyTemplate() {
      base.OnApplyTemplate();
      this._LocationElement = null;  // clear cached reference
      this._DefaultPort = null;
      // do stuff OnRotationAngleChanged wanted to do, but perhaps couldn't
      SetAngle(this.SelectionElement, Node.GetRotationAngle(this.VisualElement));
      // do stuff OnLocationChanged wanted to do, but couldn't
      UpdateBounds();
      // do stuff OnIsTreeExpandedChanged wanted to do, but wasn't able
      if (!Node.GetIsTreeExpanded(this.VisualElement)) {  // only if different from default value
        DoIsTreeExpandedChanged();
      }
    }


    // Dependency properties

    /// <summary>
    /// Identifies the <see cref="IsLinkLabel"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsLinkLabelProperty;
    /// <summary>
    /// Gets whether this node represents a label node for a link.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// You cannot change this value once the Node has been added to a Diagram.
    /// </value>
    /// <remarks>
    /// For nodes that are bound to data, this is set automatically by the <see cref="PartManager"/>.
    /// </remarks>
    public bool IsLinkLabel {
      get { return (bool)GetValue(IsLinkLabelProperty); }
      set { SetValue(IsLinkLabelProperty, value); }
    }
    private static void OnIsLinkLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = (Node)d;
      if (node.Layer != null) {
        Diagram.Error("Cannot set Node.IsLinkLabel after it has been added to a Diagram");
      }
    }


    /// <summary>
    /// Identifies the <see cref="Id"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IdProperty;
    /// <summary>
    /// Gets or sets the string identifier used by the <see cref="PartsModel"/> to resolve
    /// references from other parts in the <see cref="PartsModel"/>.
    /// </summary>
    /// <value>
    /// The default value is null.
    /// If this node is in a <see cref="PartsModel"/>,
    /// such as when defined as nested elements of a <see cref="Northwoods.GoXam.Diagram"/> element in XAML,
    /// and if you want to refer to this node from other links or nodes,
    /// you will need to provide a unique value for this property.
    /// </value>
    /// <remarks>
    /// This property is only used by unbound nodes that are in a <see cref="PartsModel"/> model.
    /// </remarks>
    public String Id {
      get { return (String)GetValue(IdProperty); }
      set { SetValue(IdProperty, value); }
    }
    private static void OnIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = (Node)d;
      Diagram diagram = node.Diagram;
      if (diagram != null) {
        diagram.PartsModel.DoNodeKeyChanged(node);
      }
    }

    /// <summary>
    /// Identifies the <see cref="PartsModelContainingSubGraph"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PartsModelContainingSubGraphProperty;
    /// <summary>
    /// Gets or sets the identifier naming the containing group node in a <see cref="PartsModel"/>.
    /// </summary>
    /// <value>
    /// The default value is null, indicating that this node is not member of any group.
    /// </value>
    /// <remarks>
    /// This property is only used by unbound nodes that are in a <see cref="PartsModel"/> model.
    /// </remarks>
    public String PartsModelContainingSubGraph {
      get { return (String)GetValue(PartsModelContainingSubGraphProperty); }
      set { SetValue(PartsModelContainingSubGraphProperty, value); }
    }
    private static void OnPartsModelContainingSubGraphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = (Node)d;
      Diagram diagram = node.Diagram;
      if (diagram != null) {
        diagram.PartsModel.DoGroupNodeChanged(node);
      }
    }


    /// <summary>
    /// Identifies the <see cref="IsExpandedTree"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsExpandedTreeProperty;
    /// <summary>
    /// Gets or sets whether this node is considered "expanded"
    /// with respect to showing/hiding nodes that it connects to.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// Changing this value calls either <see cref="ExpandTree"/> or <see cref="CollapseTree"/>.
    /// </value>
    /// <remarks>
    /// If you want to data bind whether this node (subtree) is expanded,
    /// bind the <see cref="IsTreeExpandedProperty"/> and <see cref="WasTreeExpandedProperty"/>.
    /// </remarks>
    public bool IsExpandedTree {
      get { return (bool)GetValue(IsExpandedTreeProperty); }
      set { SetValue(IsExpandedTreeProperty, value); }
    }
    private static void OnIsExpandedTreeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      //Diagram.Debug("IsExpandedTreeChanged: " + Diagram.Str((Node)d) + " " + e.OldValue.ToString() + " --> " + e.NewValue.ToString());
      ((Node)d).OnIsExpandedTreeChanged();
    }

    /// <summary>
    /// This virtual method is called whenever the value of <see cref="IsExpandedTree"/> changes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default this will call <see cref="ExpandTree"/> if <see cref="IsExpandedTree"/>
    /// has become true, else it will call <see cref="CollapseTree"/>.
    /// </para>
    /// <para>
    /// In Silverlight, if the <see cref="Part.VisualElement"/> is a <c>Control</c>,
    /// this will also call <c>VisualStateManager.GoToState</c> with a new state
    /// of either "Expanded" or "Collapsed".
    /// </para>
    /// </remarks>
    protected virtual void OnIsExpandedTreeChanged() {

      Control control = this.VisualElement as Control;
      if (control != null) {
        VisualStateManager.GoToState(control, (this.IsExpandedTree ? "Expanded" : "Collapsed"), true);
      }

      Remeasure();
      // no side-effects during undo/redo
      IDiagramModel model = this.Model;
      if (model != null && model.IsChangingModel) return;
      if (this.IsExpandedTree) {
        ExpandTree();
      } else {
        CollapseTree();
      }
      DiagramPanel panel = this.Panel;
      if (panel != null) panel.InvokeUpdateDiagramBounds("IsExpandedTreeChanged");
    }


    // Attached properties

    // Node location

    /// <summary>
    /// Identifies the <see cref="Location"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>

    [TypeConverter(typeof(Route.PointConverter))]

    public static readonly DependencyProperty LocationProperty;  // must be on root VisualElement
    /// <summary>
    /// Gets the value of the <see cref="Location"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>

    [TypeConverter(typeof(Route.PointConverter))]

    public static Point GetLocation(DependencyObject d) { return (Point)d.GetValue(LocationProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Location"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v">neither coordinate may be Double.NaN</param>
    public static void SetLocation(DependencyObject d, Point v) { d.SetValue(LocationProperty, v); }
    private static void OnLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Point oldloc = (Point)e.OldValue;
      Point newloc = (Point)e.NewValue;
      if (Double.IsNaN(oldloc.X) && Double.IsNaN(newloc.X)) return;
      if (Double.IsNaN(oldloc.Y) && Double.IsNaN(newloc.Y)) return;
      Node node = VerifyParentAs<Node>(d, e);
      // also done by OnApplyTemplate, if the parent Node was not found during init
      if (node != null) {
        //Diagram.Debug("OnLocationChanged: " + Diagram.Str(node) + "  " + Diagram.Str((Point)e.OldValue) + Diagram.Str((Point)e.NewValue));
        node.UpdateBounds();
        node.InvalidateLayout(LayoutChange.NodeLocationChanged);
      }
    }
    /// <summary>
    /// Gets or sets the position of this node based on its <see cref="LocationElement"/>.
    /// </summary>
    /// <value>
    /// The <c>Point</c> is in model coordinates.
    /// </value>
    public Point Location {  // for convenience, treat the attached property on the VisualElement as if it were this Node's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetLocation(elt); else return new Point(Double.NaN, Double.NaN); }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetLocation(elt, value); }  //?? Virtualization
    }

    /// <summary>
    /// Identifies the <see cref="LocationElementName"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty LocationElementNameProperty;  // name of element whose LocationSpot determines the Location; must be on root VisualElement
    /// <summary>
    /// Gets the value of the <see cref="LocationElementName"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static String GetLocationElementName(DependencyObject d) { return (String)d.GetValue(LocationElementNameProperty); }
    /// <summary>
    /// Sets the value of the <see cref="LocationElementName"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetLocationElementName(DependencyObject d, String v) { d.SetValue(LocationElementNameProperty, v); }
    private static void OnLocationElementNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = VerifyParentAs<Node>(d, e);
      if (node != null) {
        // clear out cached element
        node._LocationElement = null;
        node.UpdateBounds();
      }
    }
    /// <summary>
    /// Gets or sets the name of the element whose position defines the location for this node.
    /// </summary>
    /// <value>
    /// The default value is an empty string,
    /// which means <see cref="LocationElement"/> will be the <see cref="Part.VisualElement"/>,
    /// i.e. the whole node.
    /// </value>
    /// <remarks>
    /// This is used to find the element that is this node's <see cref="LocationElement"/>.
    /// </remarks>
    public String LocationElementName {  // for convenience, treat the attached property on the VisualElement as if it were this Node's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetLocationElementName(elt); else return ""; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetLocationElementName(elt, value); }
    }

    /// <summary>
    /// Gets the <c>FrameworkElement</c> that determines the location of this node.
    /// </summary>
    /// <remarks>
    /// If no element is named by <see cref="LocationElementName"/>,
    /// this returns the <see cref="Part.VisualElement"/>.
    /// </remarks>
    public FrameworkElement LocationElement {
      get {
        if (_LocationElement == null) {
          FrameworkElement vchild = this.VisualElement;
          if (vchild == null) {  // not yet found but use this part for now, keep searching next time
            return this;
          } else {
            String ename = this.LocationElementName;
            if (ename != null && ename.Length > 0) {
              FrameworkElement elt = FindNamedDescendant(ename);
              if (elt != null) {  // found it: cache it
                _LocationElement = elt;
              } else {  // not yet found but use this VCHILD for now, keep searching next time
                return vchild;
              }
            } else {  // always use main visual child
              _LocationElement = vchild;
            }
          }
        }
        return _LocationElement;
      }
    }
    private FrameworkElement _LocationElement;

    /// <summary>
    /// Identifies the <see cref="LocationSpot"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty LocationSpotProperty;  // fractional position of Location relative to bounding box, must be between (0,0) and (1,1)
    /// <summary>
    /// Gets the value of the <see cref="LocationSpot"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static Spot GetLocationSpot(DependencyObject d) { return (Spot)d.GetValue(LocationSpotProperty); }
    /// <summary>
    /// Sets the value of the <see cref="LocationSpot"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetLocationSpot(DependencyObject d, Spot v) { d.SetValue(LocationSpotProperty, v); }
    private static void OnLocationSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = VerifyParentAs<Node>(d, e);
      if (node != null) node.UpdateBounds();
    }
    /// <summary>
    /// Gets or sets the spot in the <see cref="LocationElement"/>
    /// that is this node's location point.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Spot.TopLeft"/>.
    /// The value must be a specific <see cref="Spot"/> -- i.e. one for which <see cref="Spot.IsSpot"/> is true.
    /// </value>
    public Spot LocationSpot {
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetLocationSpot(elt); else return Spot.TopLeft; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetLocationSpot(elt, value); }
    }


    /// <summary>
    /// Identifies the <see cref="MinLocation"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty MinLocationProperty;  // must be on root VisualElement
    /// <summary>
    /// Gets the value of the <see cref="MinLocation"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>

    [TypeConverter(typeof(Route.PointConverter))]

    public static Point GetMinLocation(DependencyObject d) { return (Point)d.GetValue(MinLocationProperty); }
    /// <summary>
    /// Sets the value of the <see cref="MinLocation"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>

    [TypeConverter(typeof(Route.PointConverter))]

    public static void SetMinLocation(DependencyObject d, Point v) { d.SetValue(MinLocationProperty, v); }
    private static void OnMinLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = VerifyParentAs<Node>(d, e);
      UpdateLimitedLocation(node, (Point)e.NewValue);
    }
    /// <summary>
    /// Gets or sets the minimum location for this node allowed by <see cref="Northwoods.GoXam.Tool.DraggingTool"/>.
    /// </summary>
    /// <value>
    /// The default value is (-Infinity, -Infinity), thereby imposing no position constraint.
    /// </value>
    public Point MinLocation {  // for convenience, treat the attached property on the VisualElement as if it were this Node's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetMinLocation(elt); else return new Point(Double.NegativeInfinity, Double.NegativeInfinity); }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetMinLocation(elt, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MaxLocation"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty MaxLocationProperty;  // must be on root VisualElement
    /// <summary>
    /// Gets the value of the <see cref="MaxLocation"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>

    [TypeConverter(typeof(Route.PointConverter))]

    public static Point GetMaxLocation(DependencyObject d) { return (Point)d.GetValue(MaxLocationProperty); }
    /// <summary>
    /// Sets the value of the <see cref="MaxLocation"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>

    [TypeConverter(typeof(Route.PointConverter))]

    public static void SetMaxLocation(DependencyObject d, Point v) { d.SetValue(MaxLocationProperty, v); }
    private static void OnMaxLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = VerifyParentAs<Node>(d, e);
      UpdateLimitedLocation(node, (Point)e.NewValue);
    }
    /// <summary>
    /// Gets or sets the maximum location for this node allowed by <see cref="Northwoods.GoXam.Tool.DraggingTool"/>.
    /// </summary>
    /// <value>
    /// The default value is (+Infinity, +Infinity), thereby imposing no position constraint.
    /// </value>
    public Point MaxLocation {  // for convenience, treat the attached property on the VisualElement as if it were this Node's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetMaxLocation(elt); else return new Point(Double.PositiveInfinity, Double.PositiveInfinity); }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetMaxLocation(elt, value); }
    }

    // update Location when MinLocation or MaxLocation are changed
    private static void UpdateLimitedLocation(Node n, Point newloc) {
      if (n == null) return;
      Point min = n.MinLocation;
      Point max = n.MaxLocation;
      if (newloc.X < min.X || newloc.Y < min.Y || newloc.X > max.X || newloc.Y > max.Y) {
        n.Location = new Point(Math.Max(min.X, Math.Min(newloc.X, max.X)), Math.Max(min.Y, Math.Min(newloc.Y, max.Y)));
      }
    }


    /// <summary>
    /// The Position of a <see cref="Node"/> is the point at the top-left corner of the <see cref="Part.Bounds"/>.
    /// </summary>
    /// <value>
    /// Setting this property just changes the value of <see cref="Location"/>.
    /// </value>
    public Point Position {
      get {
        Rect b = this.Bounds;
        return new Point(b.X, b.Y);
      }
      set {
        Rect b = this.Bounds;
        Point loc = this.Location;
        Point p = new Point(value.X+loc.X-b.X, value.Y+loc.Y-b.Y);
        if (Double.IsNaN(p.X) || Double.IsNaN(p.Y)) {
          Spot spot = this.LocationSpot;
          if (spot.IsNoSpot) spot = Spot.TopLeft;
          Point rel = GetRelativeElementPoint(this.LocationElement, spot);
          this.Location = new Point(value.X+rel.X, value.Y+rel.Y);  // temporary Location
        } else {
          this.Location = p;
        }
      }
    }

    /// <summary>
    /// Move a node to a new position, perhaps with animation.
    /// </summary>
    /// <param name="newpos">a new <see cref="Position"/> in model coordinates; not a new <see cref="Location"/></param>
    /// <param name="animated"></param>
    public virtual void Move(Point newpos, bool animated) {
      if (animated) {
        Diagram diagram = this.Diagram;
        if (diagram != null) {
          LayoutManager mgr = diagram.LayoutManager;
          if (mgr != null) {
            mgr.MoveAnimated(this, newpos);
            return;
          }
        }
      }
      this.Position = newpos;
    }

    /// <summary>
    /// Identifies the <see cref="Avoidable"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty AvoidableProperty;  // must be on root VisualElement
    /// <summary>
    /// Gets the value of the <see cref="Avoidable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetAvoidable(DependencyObject d) { return (bool)d.GetValue(AvoidableProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Avoidable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetAvoidable(DependencyObject d, bool v) { d.SetValue(AvoidableProperty, v); }
    private static void OnAvoidableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = VerifyParentAs<Node>(d, e);
      if (node != null) {
        DiagramPanel panel = node.Panel;
        if (panel != null) panel.InvalidatePositionArray(node);
      }
    }
    /// <summary>
    /// Gets or sets whether this node should be avoided when routing orthogonal links that have
    /// <see cref="Route.Routing"/> is <see cref="LinkRouting.AvoidsNodes"/>.
    /// </summary>
    /// <remarks>
    /// The default value is true, except that it should be false for nodes for which <see cref="IsLinkLabel"/> is true
    /// </remarks>
    public bool Avoidable {  // for convenience, treat the attached property on the VisualElement as if it were this Node's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetAvoidable(elt); else return true; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetAvoidable(elt, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AvoidableMargin"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty AvoidableMarginProperty;  // must be on root VisualElement
    /// <summary>
    /// Gets the value of the <see cref="AvoidableMargin"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static Thickness GetAvoidableMargin(DependencyObject d) { return (Thickness)d.GetValue(AvoidableMarginProperty); }
    /// <summary>
    /// Sets the value of the <see cref="AvoidableMargin"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetAvoidableMargin(DependencyObject d, Thickness v) { d.SetValue(AvoidableMarginProperty, v); }
    private static void OnAvoidableMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = VerifyParentAs<Node>(d, e);
      if (node != null) {
        DiagramPanel panel = node.Panel;
        if (panel != null) panel.InvalidatePositionArray(node);
      }
    }
    /// <summary>
    /// Gets or sets the margin around this node that should be reserved when routing orthogonal links that have
    /// <see cref="Route.Routing"/> is <see cref="LinkRouting.AvoidsNodes"/>.
    /// </summary>
    /// <value>
    /// The default value is 2 on all four sides.
    /// </value>
    public Thickness AvoidableMargin {  // for convenience, treat the attached property on the VisualElement as if it were this Node's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetAvoidableMargin(elt); else return new Thickness(); }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetAvoidableMargin(elt, value); }
    }


    /// <summary>
    /// Identifies the <see cref="RotationAngle"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty RotationAngleProperty;  // must be on root VisualElement
    /// <summary>
    /// Gets the value of the <see cref="RotationAngle"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static double GetRotationAngle(DependencyObject d) { return (double)d.GetValue(RotationAngleProperty); }
    /// <summary>
    /// Sets the value of the <see cref="RotationAngle"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetRotationAngle(DependencyObject d, double v) { d.SetValue(RotationAngleProperty, v); }
    private static void OnRotationAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = VerifyParentAs<Node>(d, e);
      if (node != null) {
        // acutally change the angle of the SelectionElement
        FrameworkElement selelt = node.SelectionElement;
        if (selelt != null) {
          node.SetAngle(selelt, (double)e.NewValue);
          node.InvalidateRelationships("RotationAngle");
        }
      }
    }
    /// <summary>
    /// Gets or sets the angle of the node's <see cref="Part.SelectionElement"/>.
    /// </summary>
    /// <remarks>
    /// The default value is 0.0.
    /// </remarks>
    public double RotationAngle {  // for convenience, treat the attached property on the VisualElement as if it were this Node's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetRotationAngle(elt); else return 0.0; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetRotationAngle(elt, value); }
    }

    
    // tree expansion properties

    /// <summary>
    /// Identifies the <c>IsTreeExpanded</c> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty IsTreeExpandedProperty;  // must be on the root visual element
    /// <summary>
    /// Gets the value of the <see cref="IsTreeExpandedProperty"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetIsTreeExpanded(DependencyObject d) { return (bool)d.GetValue(IsTreeExpandedProperty); }
    /// <summary>
    /// Sets the value of the <see cref="IsTreeExpandedProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    /// <remarks>
    /// Changing this attached property toggles the <see cref="Part.Visible"/> property of
    /// outbound links and their "to" nodes.
    /// Setting this attached property also sets the <see cref="IsExpandedTree"/> dependency property.
    /// </remarks>
    public static void SetIsTreeExpanded(DependencyObject d, bool v) { d.SetValue(IsTreeExpandedProperty, v); }
    private static void OnIsTreeExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = VerifyParentAs<Node>(d, e);
      if (node != null) {
        //Diagram.Debug("IsTreeExpanded: " + Diagram.Str(node) + " " + ((bool)e.NewValue).ToString());
        node.DoIsTreeExpandedChanged();
      }
    }
    // don't define IsTreeExpanded as a property on Node, to avoid confusion with IsExpandedTree

    private void DoIsTreeExpandedChanged() {
      FrameworkElement elt = this.VisualElement;
      if (elt == null) return;
      bool vis = Node.GetIsTreeExpanded(elt);

      this.IsExpandedTree = vis;

      if (vis && this.Diagram != null) {
        PartManager mgr = this.Diagram.PartManager;
        if (mgr != null) mgr.RealizeTreeChildren(this);
      }

      foreach (Link link in this.LinksOutOf) {
        Node to = link.ToNode;
        if (to == null || to == this) continue;
        to.Visible = vis;
        foreach (Link l in to.LinksInto) l.Visible = vis;  //???
      }
    }

    /// <summary>
    /// Identifies the <c>WasTreeExpanded</c> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty WasTreeExpandedProperty;  // must be on the root visual element
    /// <summary>
    /// Gets the value of the <see cref="WasTreeExpandedProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetWasTreeExpanded(DependencyObject d) { return (bool)d.GetValue(WasTreeExpandedProperty); }
    /// <summary>
    /// Sets the value of the <see cref="WasTreeExpandedProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetWasTreeExpanded(DependencyObject d, bool v) { d.SetValue(WasTreeExpandedProperty, v); }
    // don't define WasTreeExpanded as a property on Node, to avoid confusion and clutter

    // port properties

    /// <summary>
    /// Identifies the <c>PortId</c> attached dependency property,
    /// for an element inside a <see cref="Node"/>.
    /// </summary>
    public static readonly DependencyProperty PortIdProperty;  // on any element representing a port
    /// <summary>
    /// Gets the value of the <see cref="PortIdProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static String GetPortId(DependencyObject d) { return (String)d.GetValue(PortIdProperty); }
    /// <summary>
    /// Sets the value of the <see cref="PortIdProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetPortId(DependencyObject d, String v) { d.SetValue(PortIdProperty, v); }
    private static void OnPortIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = Diagram.FindAncestor<Node>(d);  // must be within visual tree of a Node
      if (node != null) node.InvalidateConnectedLinks();
    }

    internal static readonly DependencyProperty PortInfoProperty;
    internal static Knot GetPortInfo(DependencyObject d) { return (Knot)d.GetValue(PortInfoProperty); }
    internal static void SetPortInfo(DependencyObject d, Knot v) { d.SetValue(PortInfoProperty, v); }
    private static void OnPortInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      FrameworkElement elt = d as FrameworkElement;
      Knot oldknot = e.OldValue as Knot;
      if (oldknot != null) { oldknot.Node = null; oldknot.Port = null; }
      Knot newknot = e.NewValue as Knot;
      if (newknot != null) { newknot.Node = Diagram.FindAncestor<Node>(d); newknot.Port = elt; }
    }


    ///// <summary>
    ///// Identifies the <c>PortExtension</c> attached dependency property,
    ///// for an element inside a <see cref="Node"/>.
    ///// </summary>
    ///// <remarks>
    ///// Specifies which element to use for determining the link point.
    ///// A value of zero refers to the port element itself.
    ///// A value of one refers to the port's parent element.
    ///// A value of two refers to the port's grand-parent element, etc.
    ///// </remarks>
    //public static readonly DependencyProperty PortExtensionProperty;
    ///// <summary>
    ///// Gets the value of the <see cref="PortExtensionProperty"/> attached dependency property.
    ///// </summary>
    ///// <param name="d"></param>
    ///// <returns></returns>
    //public static int GetPortExtension(DependencyObject d) { return (int)d.GetValue(PortExtensionProperty); }
    ///// <summary>
    ///// Sets the value of the <see cref="PortExtensionProperty"/> attached dependency property.
    ///// </summary>
    ///// <param name="d"></param>
    ///// <param name="v"></param>
    //public static void SetPortExtension(DependencyObject d, int v) { d.SetValue(PortExtensionProperty, v); }
    //private static void OnPortExtensionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
    //  Node node = Diagram.FindAncestor<Node>(d);  // must be within visual tree of a Node
    //  if (node != null) node.InvalidateConnectedLinks();
    //}


    /// <summary>
    /// Identifies the <c>FromSpot</c> attached dependency property,
    /// for an element inside a <see cref="Node"/>.
    /// </summary>
    public static readonly DependencyProperty FromSpotProperty;  // on any element representing a port
    /// <summary>
    /// Gets the value of the <see cref="FromSpotProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static Spot GetFromSpot(DependencyObject d) { return (Spot)d.GetValue(FromSpotProperty); }
    /// <summary>
    /// Sets the value of the <see cref="FromSpotProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetFromSpot(DependencyObject d, Spot v) { d.SetValue(FromSpotProperty, v); }
    private static void OnFromSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = Diagram.FindAncestor<Node>(d);  // must be within visual tree of a Node
      if (node != null) node.InvalidateConnectedLinks();
    }

    /// <summary>
    /// Identifies the <c>ToSpot</c> attached dependency property,
    /// for an element inside a <see cref="Node"/>.
    /// </summary>
    public static readonly DependencyProperty ToSpotProperty;  // on any element representing a port
    /// <summary>
    /// Gets the value of the <see cref="ToSpotProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static Spot GetToSpot(DependencyObject d) { return (Spot)d.GetValue(ToSpotProperty); }
    /// <summary>
    /// Sets the value of the <see cref="ToSpotProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetToSpot(DependencyObject d, Spot v) { d.SetValue(ToSpotProperty, v); }
    private static void OnToSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = Diagram.FindAncestor<Node>(d);  // must be within visual tree of a Node
      if (node != null) node.InvalidateConnectedLinks();
    }


    /// <summary>
    /// Identifies the <c>FromEndSegmentLength</c> attached dependency property,
    /// for an element inside a <see cref="Node"/>.
    /// </summary>
    public static readonly DependencyProperty FromEndSegmentLengthProperty;  // on any element representing a port
    /// <summary>
    /// Gets the value of the <see cref="FromEndSegmentLengthProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static double GetFromEndSegmentLength(DependencyObject d) { return (double)d.GetValue(FromEndSegmentLengthProperty); }
    /// <summary>
    /// Sets the value of the <see cref="FromEndSegmentLengthProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetFromEndSegmentLength(DependencyObject d, double v) { d.SetValue(FromEndSegmentLengthProperty, v); }
    private static void OnFromEndSegmentLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = Diagram.FindAncestor<Node>(d);  // must be within visual tree of a Node
      if (node != null) node.InvalidateConnectedLinks();
    }

    /// <summary>
    /// Identifies the <c>ToEndSegmentLength</c> attached dependency property,
    /// for an element inside a <see cref="Node"/>.
    /// </summary>
    public static readonly DependencyProperty ToEndSegmentLengthProperty;  // on any element representing a port
    /// <summary>
    /// Gets the value of the <see cref="ToEndSegmentLengthProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static double GetToEndSegmentLength(DependencyObject d) { return (double)d.GetValue(ToEndSegmentLengthProperty); }
    /// <summary>
    /// Sets the value of the <see cref="ToEndSegmentLengthProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetToEndSegmentLength(DependencyObject d, double v) { d.SetValue(ToEndSegmentLengthProperty, v); }
    private static void OnToEndSegmentLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Node node = Diagram.FindAncestor<Node>(d);  // must be within visual tree of a Node
      if (node != null) node.InvalidateConnectedLinks();
    }


    // link validity properties

    /// <summary>
    /// Identifies the <c>LinkableFrom</c> attached dependency property,
    /// for an element inside a <see cref="Node"/>.
    /// </summary>
    public static readonly DependencyProperty LinkableFromProperty;  // NULLABLE: on any element representing a port, or on others to limit scope of LinkingTool.FindLinkablePort
    /// <summary>
    /// Gets the value of the <see cref="LinkableFromProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    [TypeConverter(typeof(NullableBoolConverter))]
    public static bool? GetLinkableFrom(DependencyObject d) { return (bool?)d.GetValue(LinkableFromProperty); }
    /// <summary>
    /// Sets the value of the <see cref="LinkableFromProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetLinkableFrom(DependencyObject d, bool? v) { d.SetValue(LinkableFromProperty, v); }

    /// <summary>
    /// Identifies the <c>LinkableTo</c> attached dependency property,
    /// for an element inside a <see cref="Node"/>.
    /// </summary>
    public static readonly DependencyProperty LinkableToProperty;  // NULLABLE: on any element representing a port, or on others to limit scope of LinkingTool.FindLinkablePort
    /// <summary>
    /// Gets the value of the <see cref="LinkableToProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    [TypeConverter(typeof(NullableBoolConverter))]
    public static bool? GetLinkableTo(DependencyObject d) { return (bool?)d.GetValue(LinkableToProperty); }
    /// <summary>
    /// Sets the value of the <see cref="LinkableToProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetLinkableTo(DependencyObject d, bool? v) { d.SetValue(LinkableToProperty, v); }

    /// <summary>
    /// Identifies the <c>LinkableSelfNode</c> attached dependency property,
    /// for an element inside a <see cref="Node"/>.
    /// </summary>
    public static readonly DependencyProperty LinkableSelfNodeProperty;  // on any element representing a port
    /// <summary>
    /// Gets the value of the <see cref="LinkableSelfNodeProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static bool GetLinkableSelfNode(DependencyObject d) { return (bool)d.GetValue(LinkableSelfNodeProperty); }
    /// <summary>
    /// Sets the value of the <see cref="LinkableSelfNodeProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetLinkableSelfNode(DependencyObject d, bool v) { d.SetValue(LinkableSelfNodeProperty, v); }

    /// <summary>
    /// Identifies the <c>LinkableDuplicates</c> attached dependency property,
    /// for an element inside a <see cref="Node"/>.
    /// </summary>
    public static readonly DependencyProperty LinkableDuplicatesProperty;  // on any element representing a port
    /// <summary>
    /// Gets the value of the <see cref="LinkableDuplicatesProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static bool GetLinkableDuplicates(DependencyObject d) { return (bool)d.GetValue(LinkableDuplicatesProperty); }
    /// <summary>
    /// Sets the value of the <see cref="LinkableDuplicatesProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetLinkableDuplicates(DependencyObject d, bool v) { d.SetValue(LinkableDuplicatesProperty, v); }

    /// <summary>
    /// Identifies the <c>LinkableMaximum</c> attached dependency property,
    /// for an element inside a <see cref="Node"/>.
    /// </summary>
    public static readonly DependencyProperty LinkableMaximumProperty;  // on any element representing a port
    /// <summary>
    /// Gets the value of the <see cref="LinkableMaximumProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static int GetLinkableMaximum(DependencyObject d) { return (int)d.GetValue(LinkableMaximumProperty); }
    /// <summary>
    /// Sets the value of the <see cref="LinkableMaximumProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="v"></param>
    public static void SetLinkableMaximum(DependencyObject d, int v) { d.SetValue(LinkableMaximumProperty, v); }


    // convenience properties and methods for navigation
    // these go through the model but operate on/with Parts

    /// <summary>
    /// Gets a collection of <see cref="Link"/>s that are connected to this node.
    /// </summary>
    public IEnumerable<Link> LinksConnected {
      get {
        Object data = this.Data;
        if (data == null) return NoLinks;
        IDiagramModel model = this.Model;
        if (model == null) return NoLinks;
        Diagram diagram = this.Diagram;
        if (diagram == null) return NoLinks;
        PartManager mgr = diagram.PartManager;
        if (mgr == null) return NoLinks;
        ILinksModel lmodel = model as ILinksModel;
        if (lmodel != null) {
          IEnumerable<Object> linksdata = lmodel.GetLinksForNode(data);
          return linksdata.Select(d => mgr.FindLinkForData(d, model)).Where(l => l != null).ToList();
        } else {
          ITreeModel tmodel = model as ITreeModel;
          if (tmodel != null) {
            IEnumerable<Object> children = tmodel.GetChildrenForNode(data);
            IEnumerable<Link> links = children.Select(c => mgr.FindLinkForData(data, c, model)).Where(l => l != null).ToList();
            Object parent = tmodel.GetParentForNode(data);
            if (parent != null) {
              Link plink = mgr.FindLinkForData(parent, data, model);
              if (plink != null) return links.Concat(new Link[1] { plink }).ToList();
            }
            return links.ToList();
          } else {
            IEnumerable<Link> f = model.GetFromNodesForNode(data).Select(d => mgr.FindLinkForData(d, data, model)).Where(l => l != null);
            IEnumerable<Link> t = model.GetToNodesForNode(data).Where(d => d != data).Select(d => mgr.FindLinkForData(data, d, model)).Where(l => l != null);
            return f.Concat(t).ToList();
          }
        }
      }
    }

    /// <summary>
    /// Gets a collection of <see cref="Link"/>s that come into this node.
    /// </summary>
    public IEnumerable<Link> LinksInto {
      get {
        Object data = this.Data;
        if (data == null) return NoLinks;
        IDiagramModel model = this.Model;
        if (model == null) return NoLinks;
        Diagram diagram = this.Diagram;
        if (diagram == null) return NoLinks;
        PartManager mgr = diagram.PartManager;
        if (mgr == null) return NoLinks;
        ILinksModel lmodel = model as ILinksModel;
        if (lmodel != null) {
          IEnumerable<Object> linksdata = lmodel.GetFromLinksForNode(data);
          return linksdata.Select(d => mgr.FindLinkForData(d, model)).Where(l => l != null).ToList();
        } else {
          ITreeModel tmodel = model as ITreeModel;
          if (tmodel != null) {
            Object parent = tmodel.GetParentForNode(data);
            if (parent != null) {
              Link plink = mgr.FindLinkForData(parent, data, model);
              if (plink != null) return new Link[1] { plink };
            }
            return NoLinks;
          } else {
            return model.GetFromNodesForNode(data).Select(d => mgr.FindLinkForData(d, data, model)).Where(l => l != null).ToList();
          }
        }
      }
    }

    /// <summary>
    /// Gets a collection of <see cref="Link"/>s that go out of this node.
    /// </summary>
    public IEnumerable<Link> LinksOutOf {
      get {
        Object data = this.Data;
        if (data == null) return NoLinks;
        IDiagramModel model = this.Model;
        if (model == null) return NoLinks;
        Diagram diagram = this.Diagram;
        if (diagram == null) return NoLinks;
        PartManager mgr = diagram.PartManager;
        if (mgr == null) return NoLinks;
        ILinksModel lmodel = model as ILinksModel;
        if (lmodel != null) {
          IEnumerable<Object> linksdata = lmodel.GetToLinksForNode(data);
          return linksdata.Select(d => mgr.FindLinkForData(d, model)).Where(l => l != null).ToList();
        } else {
          ITreeModel tmodel = model as ITreeModel;
          if (tmodel != null) {
            IEnumerable<Object> children = tmodel.GetChildrenForNode(data);
            return children.Select(c => mgr.FindLinkForData(data, c, model)).Where(l => l != null).ToList();
          } else {
            return model.GetToNodesForNode(data).Select(d => mgr.FindLinkForData(data, d, model)).Where(l => l != null).ToList();
          }
        }
      }
    }

    /// <summary>
    /// Gets a collection of <see cref="Node"/>s that have links connected to this node.
    /// </summary>
    public IEnumerable<Node> NodesConnected {
      get {
        Object data = this.Data;
        if (data == null) return NoNodes;
        IDiagramModel model = this.Model;
        if (model == null) return NoNodes;
        Diagram diagram = this.Diagram;
        if (diagram == null) return NoNodes;
        PartManager mgr = diagram.PartManager;
        if (mgr == null) return NoNodes;
        return model.GetConnectedNodesForNode(data).Select(d => mgr.FindNodeForData(d, model)).Where(n => n != null).ToList();
      }
    }

    /// <summary>
    /// Gets a collection of <see cref="Node"/>s that have links coming into this node.
    /// </summary>
    public IEnumerable<Node> NodesInto {
      get {
        Object data = this.Data;
        if (data == null) return NoNodes;
        IDiagramModel model = this.Model;
        if (model == null) return NoNodes;
        Diagram diagram = this.Diagram;
        if (diagram == null) return NoNodes;
        PartManager mgr = diagram.PartManager;
        if (mgr == null) return NoNodes;
        return model.GetFromNodesForNode(data).Select(d => mgr.FindNodeForData(d, model)).Where(n => n != null).ToList();
      }
    }

    /// <summary>
    /// Gets a collection of <see cref="Node"/>s that have links going out of this node.
    /// </summary>
    public IEnumerable<Node> NodesOutOf {
      get {
        Object data = this.Data;
        if (data == null) return NoNodes;
        IDiagramModel model = this.Model;
        if (model == null) return NoNodes;
        Diagram diagram = this.Diagram;
        if (diagram == null) return NoNodes;
        PartManager mgr = diagram.PartManager;
        if (mgr == null) return NoNodes;
        return model.GetToNodesForNode(data).Select(d => mgr.FindNodeForData(d, model)).Where(n => n != null).ToList();
      }
    }


    internal Node FindVisibleNode(FrameworkElement port) {
      Node n = this;
      if (port == null || !Part.IsVisibleElement(port)) {  // not visible? try containing groups
        while (n != null && n.Visibility != Visibility.Visible) {
          n = n.ContainingSubGraph;
        }
      }
      return n;
    }


    /// <summary>
    /// Find the <c>FrameworkElement</c> in this node whose <c>Node.PortId</c>
    /// attached property matches a given port parameter value.
    /// </summary>
    /// <param name="portid">
    /// This treats null as an empty string.
    /// The method returns the element whose <see cref="GetPortId"/> value equals that string.
    /// If there is no such child element, it searches for an element whose
    /// <see cref="GetPortId"/> value is the empty string.
    /// Finally, when failing to find a port with either the given name or the empty string,
    /// this method returns the root visual element for the node.
    /// </param>
    /// <param name="from"></param>
    /// <returns>a <c>FrameworkElement</c>, or null if no matching element is found</returns>
    public virtual FrameworkElement FindPort(String portid, bool from) {  //?? use Func<Node, Object, bool, FrameworkElement> property
      // treat a null name as an empty string
      if (portid == null) portid = "";
      if (_DefaultPort != null && portid == "") return _DefaultPort;
      FrameworkElement vchild = this.VisualElement;
      if (vchild == null) return null;
      // search for an element with that PortId
      FrameworkElement port = FindDescendant(x => Node.GetPortId(x) == portid);
      if (port != null) {
        if (portid == "") _DefaultPort = port;
        return port;
      }
      // if it wasn't already the empty string, look for one with the empty string
      if (portid != "") {
        port = FindDescendant(x => Node.GetPortId(x) == "");
        if (port != null) {
          _DefaultPort = port;
          return port;
        }
      }
      // ultimately default to the VisualElement
      _DefaultPort = vchild;
      return vchild;  //??? cannot express "there is no port on this node"
    }
    // this is a cached reference to the element with PortID=="",
    // or else to VisualElement
    private FrameworkElement _DefaultPort;

    /// <summary>
    /// Get the port parameter value for a particular <c>FrameworkElement</c>.
    /// </summary>
    /// <param name="port">a <c>FrameworkElement</c> in this node's visual tree</param>
    /// <returns>
    /// the value of <see cref="GetPortId"/>, if it is a non-empty string;
    /// otherwise it returns null
    /// </returns>
    public virtual String GetPortName(FrameworkElement port) {  //?? use Func<Node, FrameworkElement, bool, Object> property
      String id = Node.GetPortId(port);
      if (id != null && id.Length > 0) return id;
      return null;  // assume null and empty string are the same
    }

    /// <summary>
    /// Gets a collection of <c>FrameworkElement</c>s that have a non-null <see cref="GetPortId"/> value.
    /// </summary>
    /// <value>
    /// This will include <see cref="Part.VisualElement"/> itself,
    /// which might not even have a <c>Node.PortId</c> attached property value,
    /// if there aren't any elements with a non-null <c>Node.PortId</c>.
    /// </value>
    public virtual IEnumerable<FrameworkElement> Ports {
      get {
        List<FrameworkElement> ports = new List<FrameworkElement>();
        FrameworkElement vchild = this.VisualElement;
        CollectPorts(vchild, ports);
        if (vchild != null && ports.Count == 0) ports.Add(vchild);
        return ports;
      }
    }

    private void CollectPorts(FrameworkElement elt, List<FrameworkElement> ports) {
      if (elt == null) return;
      if (Node.GetPortId(elt) != null) ports.Add(elt);
      int numchildren = VisualTreeHelper.GetChildrenCount(elt);
      for (int i = 0; i < numchildren; i++) {
        CollectPorts(VisualTreeHelper.GetChild(elt, i) as FrameworkElement, ports);
      }
    }

    /// <summary>
    /// Gets the primary <c>FrameworkElement</c> representing a port.
    /// </summary>
    /// <value>
    /// If there is a <c>FrameworkElement</c> in the visual tree whose <see cref="GetPortId"/>
    /// is the empty string, return it.
    /// Otherwise return the first element with a non-null <c>Node.PortId</c>.
    /// If there is no such element, just return the <see cref="Part.VisualElement"/>.
    /// </value>
    public virtual FrameworkElement Port {
      get {
        if (_DefaultPort != null) return _DefaultPort;
        FrameworkElement p = FindDescendant(x => Node.GetPortId(x) == "");
        if (p != null) {
          _DefaultPort = p;
          return p;
        }
        // try any other identified port
        p = FindDescendant(x => Node.GetPortId(x) != null);
        if (p != null) return p;
        // otherwise use VisualElement
        _DefaultPort = this.VisualElement;
        return _DefaultPort;
      }
    }


    /// <summary>
    /// Returns a collection of <see cref="Link"/>s that are connected to a particular port
    /// in either direction.
    /// </summary>
    /// <param name="portid">
    /// When this argument is null, this just returns <see cref="LinksConnected"/>.
    /// Otherwise it returns those links where either the <see cref="Link.FromPortId"/>
    /// or the <see cref="Link.ToPortId"/> match the <paramref name="portid"/>.
    /// </param>
    /// <returns></returns>
    public IEnumerable<Link> FindLinksConnectedWithPort(String portid) {
      if (portid == null) return this.LinksConnected;
      return this.LinksConnected.Where(l => (portid.Equals(l.FromPortId) && l.FromNode == this) ||
                                            (portid.Equals(l.ToPortId) && l.ToNode == this));
    }

    /// <summary>
    /// Returns a collection of <see cref="Link"/>s that come into a particular port.
    /// </summary>
    /// <param name="portid">
    /// When this argument is null, this just returns <see cref="LinksInto"/>.
    /// Otherwise it returns those links where the <see cref="Link.FromPortId"/>
    /// matches the <paramref name="portid"/>.
    /// </param>
    /// <returns></returns>
    public IEnumerable<Link> FindLinksIntoPort(String portid) {
      if (portid == null) return this.LinksInto;
      return this.LinksInto.Where(l => portid.Equals(l.ToPortId));
    }

    /// <summary>
    /// Returns a collection of <see cref="Link"/>s that go out of a particular port.
    /// </summary>
    /// <param name="portid">
    /// When this argument is null, this just returns <see cref="LinksOutOf"/>.
    /// Otherwise it returns those links where the <see cref="Link.ToPortId"/>
    /// matches the <paramref name="portid"/>.
    /// </param>
    /// <returns></returns>
    public IEnumerable<Link> FindLinksOutOfPort(String portid) {
      if (portid == null) return this.LinksOutOf;
      return this.LinksOutOf.Where(l => portid.Equals(l.FromPortId));
    }

    /// <summary>
    /// Returns a collection of <see cref="Node"/>s that are connected to this node
    /// via links that are connected to a particular port in either direction.
    /// </summary>
    /// <param name="portid">
    /// a port description that is passed to <see cref="FindLinksConnectedWithPort"/>
    /// </param>
    /// <returns></returns>
    public IEnumerable<Node> FindNodesConnectedWithPort(String portid) {
      HashSet<Node> nodes = new HashSet<Node>();
      foreach (Link link in FindLinksConnectedWithPort(portid)) {
        if (link.FromNode == this) nodes.Add(link.ToNode);
        if (link.ToNode == this) nodes.Add(link.FromNode);
      }
      return nodes;
    }

    /// <summary>
    /// Returns a collection of <see cref="Node"/>s that are connected to this node via links that come into a particular port.
    /// </summary>
    /// <param name="portid">
    /// a port description that is passed to <see cref="FindLinksIntoPort"/>
    /// </param>
    /// <returns></returns>
    public IEnumerable<Node> FindNodesIntoPort(String portid) {
      HashSet<Node> nodes = new HashSet<Node>();
      foreach (Link link in FindLinksIntoPort(portid)) {
        nodes.Add(link.FromNode);
      }
      return nodes;
    }

    /// <summary>
    /// Returns a collection of <see cref="Node"/>s that are connected to this node via links that go out of a particular port.
    /// </summary>
    /// <param name="portid">
    /// a port description that is passed to <see cref="FindLinksOutOfPort"/>
    /// </param>
    /// <returns></returns>
    public IEnumerable<Node> FindNodesOutOfPort(String portid) {
      HashSet<Node> nodes = new HashSet<Node>();
      foreach (Link link in FindLinksOutOfPort(portid)) {
        nodes.Add(link.ToNode);
      }
      return nodes;
    }


    /// <summary>
    /// Get a collection of <see cref="Group"/>s of which this part is a member.
    /// </summary>
    public override IEnumerable<Group> ContainingGroups {
      get {
        IDiagramModel model = this.Model;
        ISubGraphModel sgmodel = model as ISubGraphModel;
        if (sgmodel != null) {
          Object data = this.Data;
          if (data == null) return NoGroups;
          Object sgdata = sgmodel.GetGroupForNode(data);
          if (sgdata == null) return NoGroups;
          Diagram diagram = this.Diagram;
          if (diagram == null) return NoGroups;
          PartManager mgr = diagram.PartManager;
          if (mgr == null) return NoGroups;
          Group g = mgr.FindNodeForData(sgdata, model) as Group;
          if (g != null) return new Group[1] { g };
        }  //?? handle ISetsModel
        return NoGroups;
      }
    }

    /// <summary>
    /// Get the group <see cref="Group"/> that this part is a member of, if any.
    /// </summary>
    public override Group ContainingSubGraph {
      get {
        IDiagramModel model = this.Model;
        ISubGraphModel sgmodel = model as ISubGraphModel;
        if (sgmodel != null) {
          Object data = this.Data;
          if (data == null) return null;
          Object sgdata = sgmodel.GetGroupForNode(data);
          if (sgdata == null) return null;
          Diagram diagram = this.Diagram;
          if (diagram == null) return null;
          PartManager mgr = diagram.PartManager;
          if (mgr == null) return null;
          return mgr.FindNodeForData(sgdata, model) as Group;
        }  //?? handle ISetsModel
        return null;
      }
    }

    /// <summary>
    /// If this node is a label for a link, return that <see cref="Link"/>.
    /// </summary>
    public Link LabeledLink {
      get {
        IDiagramModel model = this.Model;
        ILinksModel lmodel = model as ILinksModel;
        if (lmodel != null) {
          Object data = this.Data;
          if (data == null) return null;
          Object linkdata = lmodel.GetLabeledLinkForNode(data);
          if (linkdata == null) return null;
          Diagram diagram = this.Diagram;
          if (diagram == null) return null;
          PartManager mgr = diagram.PartManager;
          if (mgr == null) return null;
          return mgr.FindLinkForData(linkdata, model);
        }  //?? else handle ISetsLinksModel
        return null;
      }
    }


    // trees
    
    /// <summary>
    /// Collapse each of the links coming out of this node,
    /// and recursively collapse the "to" node for each of those links.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This also collapses tree children nodes.
    /// </para>
    /// <para>
    /// This also calls <see cref="SetIsTreeExpanded"/> to set that attached property,
    /// and <see cref="SetWasTreeExpanded"/> to remember whether a tree child node had been expanded,
    /// to support data binding the state of the tree node.
    /// </para>
    /// <para>
    /// This operation is performed within a "Collapse Tree" transaction.
    /// </para>
    /// </remarks>
    public void CollapseTree() {
      const String Action = "Collapse Tree";
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      // avoid multiple simultaneous recursions
      if (diagram.IsCollapsingExpanding) return;
      diagram.IsCollapsingExpanding = true;
      diagram.StartTransaction(Action);
      CollapseTree1();
      diagram.CommitTransaction(Action);
      diagram.IsCollapsingExpanding = false;
    }

    private void CollapseTree1() {
      FrameworkElement ve = this.EnsuredVisualElement;
      if (ve == null) return;
      //Diagram.Debug("CollapseTree: " + Diagram.Str(this) + "  " + this.Diagram.ToString());

      // change the visibility of the children
      bool oldexp = Node.GetIsTreeExpanded(ve);
      Node.SetIsTreeExpanded(ve, false);  // also sets IsExpandedTree carefully

      foreach (Node n in this.NodesOutOf) {
        if (n != this) {
          FrameworkElement nve = n.EnsuredVisualElement;
          if (nve != null) {
            if (Node.GetIsTreeExpanded(nve)) {
              Node.SetWasTreeExpanded(nve, true);
              n.CollapseTree1();
            } else if (!oldexp && n.Visible) {
              Node.SetWasTreeExpanded(nve, true);
              n.CollapseTree1();
              n.Visible = false;
            } else {
              Node.SetWasTreeExpanded(nve, false);
            }
          }
        }
      }

      // maybe delete all children nodes and links to them
      Diagram diagram = this.Diagram;
      if (diagram != null && diagram.PartManager != null) diagram.PartManager.ReleaseTreeChildren(this);
    }

    /// <summary>
    /// Make visible each link coming out of this node and the "to" node it connects to,
    /// and perhaps recursively expand their nodes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This will expand a tree child node only if its <see cref="GetWasTreeExpanded"/> was true.
    /// </para>
    /// <para>
    /// This also calls <see cref="SetIsTreeExpanded"/> to set that attached property,
    /// and if appropriate <see cref="SetWasTreeExpanded"/>,
    /// to support data binding whether the tree is expanded.
    /// </para>
    /// <para>
    /// This operation is performed within an "Expand Tree" transaction.
    /// </para>
    /// </remarks>
    public void ExpandTree() {
      const String Action = "Expand Tree";
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      // avoid multiple simultaneous recursions
      if (diagram.IsCollapsingExpanding) return;
      diagram.IsCollapsingExpanding = true;
      diagram.StartTransaction(Action);
      ExpandTree1();
      diagram.CommitTransaction(Action);
      diagram.IsCollapsingExpanding = false;
    }

    private void ExpandTree1() {
      FrameworkElement ve = this.EnsuredVisualElement;
      if (ve == null) return;
      //Diagram.Debug("ExpandTree: " + Diagram.Str(this) + "  " + this.Diagram.ToString());

      // change the visibility of the children
      bool oldexp = Node.GetIsTreeExpanded(ve);
      Node.SetIsTreeExpanded(ve, true);  // also sets IsExpandedTree carefully

      Point loc = this.Location;
      foreach (Node n in this.NodesOutOf) {
        if (n != this) {
          FrameworkElement nve = n.EnsuredVisualElement;
          Point l = n.Location;
          if (Double.IsNaN(l.X) || Double.IsNaN(l.Y)) n.Location = loc;
          if (nve != null) {
            if (!Node.GetIsTreeExpanded(nve) && Node.GetWasTreeExpanded(nve)) {
              n.ExpandTree1();
            } else if (oldexp && !n.Visible) {
              n.ExpandTree1();
              n.Visible = true;
            }
          }
        }
      }
    }
  }  // end of Node



  /// <summary>
  /// A <c>Group</c> is a <see cref="Node"/> that may logically contain
  /// other <see cref="Node"/>s and <see cref="Link"/>s.
  /// </summary>
  /// <remarks>
  /// <para>
  /// You can get a collection of member nodes using the <see cref="MemberNodes"/> property.
  /// You can get a collection of member links using the <see cref="MemberLinks"/> property.
  /// Changing group membership requires modifying the diagram's model.
  /// </para>
  /// <para>
  /// The member nodes and links are NOT visual children of this group.
  /// Instead they are direct visual children of a <see cref="Northwoods.GoXam.Layer"/>,
  /// just as this group is.
  /// </para>
  /// <para>
  /// If one expects that the rendering of a group should grow and shrink
  /// based on the sizes and positions of its member nodes, the
  /// <see cref="Northwoods.GoXam.Diagram"/>'s <see cref="Northwoods.GoXam.Diagram.GroupTemplate"/>
  /// should make use of a <see cref="Northwoods.GoXam.GroupPanel"/>.
  /// The <see cref="GroupPanel"/> property finds this panel for you.
  /// </para>
  /// <para>
  /// This class also supports the notion of expanding/collapsing.
  /// There are two attached properties, <see cref="IsSubGraphExpandedProperty"/>
  /// and <see cref="WasSubGraphExpandedProperty"/>, that can be data-bound, as well as two methods,
  /// <see cref="CollapseSubGraph"/> and <see cref="ExpandSubGraph"/>.
  /// </para>
  /// <para>
  /// Each group can have its own <see cref="IDiagramLayout"/> to govern
  /// how the member nodes and links are laid out, independently of the
  /// <see cref="Northwoods.GoXam.Diagram"/>'s <see cref="Northwoods.GoXam.Diagram.Layout"/>.
  /// </para>
  /// <para>
  /// You may not apply any transforms to a <c>Group</c>; that is reserved to GoXam for future use.
  /// </para>
  /// </remarks>
  [System.Windows.Markup.ContentProperty("Content")]
  public class Group : Node {
    /// <summary>
    /// Create an empty group with a default <see cref="Layout"/>.
    /// </summary>
    public Group() {
      this.Layout = new DiagramLayout();
    }

    static Group() {
      // Dependency properties

      // a transient property
      IsExpandedSubGraphProperty = DependencyProperty.Register("IsExpandedSubGraph", typeof(bool), typeof(Group),
        new FrameworkPropertyMetadata(true, OnIsExpandedSubGraphChanged));

      // Attached properties

      // Ungroup command properties:
      // Must be on the root visual element
      UngroupableProperty = DependencyProperty.RegisterAttached("Ungroupable", typeof(bool), typeof(Group),
        new FrameworkPropertyMetadata(true, VerifyOnVisualElement<Group>));  // if Selectable
      //?? Regroupable

      // These must be on the root visual element:
      // see also: IsExpandedSubGraph property
      IsSubGraphExpandedProperty = DependencyProperty.RegisterAttached("IsSubGraphExpanded", typeof(bool), typeof(Group),
        new FrameworkPropertyMetadata(true, OnIsSubGraphExpandedChanged));
      WasSubGraphExpandedProperty = DependencyProperty.RegisterAttached("WasSubGraphExpanded", typeof(bool), typeof(Group),
        new FrameworkPropertyMetadata(false, VerifyOnVisualElement<Group>));

      // Must be on the root visual element
      LayoutProperty = DependencyProperty.RegisterAttached("Layout", typeof(IDiagramLayout), typeof(Group),
        new FrameworkPropertyMetadata(null, OnLayoutChanged));
    }









    /// <summary>
    /// Gets the <see cref="GroupPanel"/> representing the group's member nodes and links.
    /// </summary>
    /// <value>
    /// This will be null if this group does not use a <see cref="GroupPanel"/>.
    /// </value>
    /// <remarks>
    /// This group's member <see cref="Node"/>s and <see cref="Link"/>s are not visual children of this group.
    /// However, if there is a <see cref="GroupPanel"/> in the group's visual tree,
    /// it will be measured and arranged to have the same bounds as the collection of member nodes.
    /// </remarks>
    public GroupPanel GroupPanel {
      get {
        if (_GroupPanel == null) {
          _GroupPanel = FindDescendant(x => x is GroupPanel) as GroupPanel;
        }
        return _GroupPanel;
      }
    }
    private GroupPanel _GroupPanel;

    // whether the Bounds of the group depends on the bounding rectangle of the member nodes
    internal bool DependentBounds {
      get { return (this.GroupPanel != null && this.MemberNodes.Count() >= 0); }
    }

    /// <summary>
    /// Move not only this node but also all member nodes and links recursively.
    /// </summary>
    /// <param name="newpos">a new <see cref="Node.Position"/> in model coordinates; not a new <see cref="Node.Location"/></param>
    /// <param name="animated">whether the movement is animated by the diagram's <see cref="DiagramLayout"/></param>
    /// <remarks>
    /// This uses the diagram's <see cref="DraggingTool"/> by calling its <see cref="DraggingTool.MoveParts"/> method
    /// in order to respect grid snapping and to try to avoid rerouting links.
    /// Currently, member nodes and links are not moved with animation.
    /// </remarks>
    public override void Move(Point newpos, bool animated) {
      Diagram diagram = this.Diagram;
      // use DraggingTool.MoveParts to move all of the members too, without any re-routing
      DraggingTool tool = (diagram != null ? diagram.DraggingTool : null);
      if (tool == null) tool = new DraggingTool();

      Point oldpos = this.Position;
      if (Double.IsNaN(oldpos.X) || Double.IsNaN(oldpos.Y)) {
        RemeasureNow();
        oldpos = this.Position;
      }
      // include all member nodes and links, recursively
      Dictionary<Part, DraggingTool.Info> coll = new Dictionary<Part, DraggingTool.Info>();
      foreach (Part p in this.MemberPartsAll) {
        Node n = p as Node;
        if (n != null) {
          coll.Add(n, new DraggingTool.Info() { Point=n.Location });
        } else {
          coll.Add(p, new DraggingTool.Info());
        }
      }
      LayoutManager mgr = (diagram != null ? diagram.LayoutManager : null);
      if (animated && mgr != null) {
        mgr.MoveAnimated(this, newpos);
      } else {
        this.Position = newpos;
      }
      tool.MoveParts(coll, Geo.Subtract(newpos, oldpos));
    }


    // invalidation/updating


    internal Rect ArrangedBounds { get; set; }


    internal override bool IsReadyToMeasureArrange() {
      if (!base.IsReadyToMeasureArrange()) return false;
      foreach (Node m in this.MemberNodes) {
        if (m.Visibility != Visibility.Visible) continue;
        if (!m.ValidArrange) return false;
      }
      foreach (Link m in this.MemberLinks) {
        if (m.Visibility != Visibility.Visible) continue;
        // OK if the link connects to the group itself
        if (!m.ValidArrange && m.FromNode != this && m.ToNode != this) return false;
      }
      return true;
    }

    internal override void MeasureMore() {
      GroupPanel sgc = this.GroupPanel;
      if (sgc != null) {
        // give any containers of the GroupPanel the chance to remeasure/rearrange
        InvalidateVisual(sgc);
      }
    }

    internal override void InvalidateOtherPartRelationships(bool arrangeonly) {
      base.InvalidateOtherPartRelationships(arrangeonly);

      if (!this.IsExpandedSubGraph) {
        // need to update links connected to member nodes, because they need
        // to connect to the group if the nodes were made Collapsed
        HashSet<Part> allmembers = new HashSet<Part>(this.MemberPartsAll);
        InvalidateCollapsedMembers(allmembers);
      }
    }

    private void InvalidateCollapsedMembers(HashSet<Part> allmembers) {
      foreach (Node memnode in this.MemberNodes) {
        memnode.InvalidateConnectedLinks(allmembers);
        Group memgroup = memnode as Group;
        if (memgroup != null) memgroup.InvalidateCollapsedMembers(allmembers);
      }
      foreach (Link memlink in this.MemberLinks) {
        Node label = memlink.LabelNode;
        if (label != null) label.InvalidateConnectedLinks(allmembers);
      }
    }

    internal override void PropagateVisible(bool vis) {
      base.PropagateVisible(vis);
      if (this.IsExpandedSubGraph) {
        foreach (Node n in this.MemberNodes) {
          if (n.Visible != vis) {
            n.Visible = vis;
            n.PropagateVisible(vis);
          }
        }
        foreach (Link l in this.MemberLinks) {
          if (l.Visible != vis) {
            l.Visible = vis;
            l.PropagateVisible(vis);
          }
        }
      }  // else if collapsed, don't need to change visibility of members
    }


    internal override bool IntersectsRect(Rect b) {
      if (base.IntersectsRect(b)) return true;
      foreach (Node m in this.MemberNodes) {
        if (m.IntersectsRect(b)) return true;
      }
      return false;
    }


    /// <summary>
    /// Initialize <see cref="IsExpandedSubGraph"/> with the possibly bound value of <see cref="GetIsSubGraphExpanded"/>
    /// on this group's root visual element, and initialize the <see cref="Layout"/>.
    /// </summary>
    public override void OnApplyTemplate() {
      base.OnApplyTemplate();
      this._GroupPanel = null;  // clear cached reference to nested GroupPanel (if any)
      // do what OnLayoutChanged wanted to do, but couldn't
      IDiagramLayout layout = this.Layout;
      if (layout != null) {
        layout.Diagram = this.Diagram;
        layout.Group = this;
        DiagramLayout dlay = layout as DiagramLayout;
        if (dlay != null) dlay.DataContext = this.DataContext;  //?? support databinding of Layout properties
      }
      // do stuff OnIsSubGraphExpandedChanged wanted to do, but couldn't
      if (!Group.GetIsSubGraphExpanded(this.VisualElement)) {  // only if different from default value
        DoIsSubGraphExpandedChanged();
      }
    }


    // dependency property

    /// <summary>
    /// Identifies the <see cref="IsExpandedSubGraph"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsExpandedSubGraphProperty;
    /// <summary>
    /// Gets or sets whether this group is considered "expanded".
    /// </summary>
    /// <value>
    /// The default value is false.
    /// Changing this value calls either <see cref="ExpandSubGraph"/> or <see cref="CollapseSubGraph"/>.
    /// </value>
    /// <remarks>
    /// If you want to data bind whether this group is expanded,
    /// bind <see cref="IsSubGraphExpandedProperty"/> and <see cref="WasSubGraphExpandedProperty"/>.
    /// </remarks>
    public bool IsExpandedSubGraph {
      get { return (bool)GetValue(IsExpandedSubGraphProperty); }
      set { SetValue(IsExpandedSubGraphProperty, value); }
    }
    private static void OnIsExpandedSubGraphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      ((Group)d).OnIsExpandedSubGraphChanged();
    }

    /// <summary>
    /// This virtual method is called whenever the value of <see cref="IsExpandedSubGraph"/> changes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default this will call <see cref="ExpandSubGraph"/> if <see cref="IsExpandedSubGraph"/>
    /// has become true, else it will call <see cref="CollapseSubGraph"/>.
    /// </para>
    /// <para>
    /// In Silverlight, if the <see cref="Part.VisualElement"/> is a <c>Control</c>,
    /// this will also call <c>VisualStateManager.GoToState</c> with a new state
    /// of either "Expanded" or "Collapsed".
    /// </para>
    /// </remarks>
    protected virtual void OnIsExpandedSubGraphChanged() {

      Control control = this.VisualElement as Control;
      if (control != null) {
        VisualStateManager.GoToState(control, (this.IsExpandedSubGraph ? "Expanded" : "Collapsed"), true);
      }

      Remeasure();
      // no side-effects during undo/redo
      IDiagramModel model = this.Model;
      if (model != null && model.IsChangingModel) return;
      if (this.IsExpandedSubGraph) {
        ExpandSubGraph();
      } else {
        CollapseSubGraph();
      }
      DiagramPanel panel = this.Panel;
      if (panel != null) panel.InvokeUpdateDiagramBounds("IsExpandedSubGraphChanged");
    }


    // attached dependency properties

    /// <summary>
    /// Identifies the <see cref="Ungroupable"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty UngroupableProperty;
    /// <summary>
    /// Gets the value of the <see cref="Ungroupable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetUngroupable(DependencyObject d) { return (bool)d.GetValue(UngroupableProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Ungroupable"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetUngroupable(DependencyObject d, bool v) { d.SetValue(UngroupableProperty, v); }
    /// <summary>
    /// Gets or sets whether the user may delete this group without removing any members.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    /// <remarks>
    /// In general one should call <see cref="CanUngroup"/> to see
    /// if a particular node is ungroupable, not get this property value.
    /// </remarks>
    public bool Ungroupable {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetUngroupable(elt); else return true; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetUngroupable(elt, value); }
    }

    /// <summary>
    /// This predicate is true if the user may ungroup this node.
    /// </summary>
    /// <returns>
    /// Return true if this group is <see cref="Ungroupable"/>,
    /// if this node's layer's <see cref="Northwoods.GoXam.Layer.AllowUngroup"/> is true,
    /// and if this node's diagram's <see cref="Northwoods.GoXam.Diagram.AllowUngroup"/> is true.
    /// </returns>
    public bool CanUngroup() {
      if (!this.Ungroupable) return false;
      Layer layer = this.Layer;
      if (layer == null) return true;
      if (!layer.AllowUngroup) return false;
      Diagram diagram = layer.Diagram;
      if (diagram == null) return true;
      if (!diagram.AllowUngroup) return false;
      return true;
    }


    /// <summary>
    /// Identifies the <c>IsSubGraphExpanded</c> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty IsSubGraphExpandedProperty;  // must be on the root visual element
    /// <summary>
    /// Gets the value of the <see cref="IsSubGraphExpandedProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetIsSubGraphExpanded(DependencyObject d) { return (bool)d.GetValue(IsSubGraphExpandedProperty); }
    /// <summary>
    /// Sets the value of the <see cref="IsSubGraphExpandedProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    /// <remarks>
    /// Changing this attached property toggles the <see cref="Part.Visible"/> property of
    /// member nodes and links.
    /// Setting this attached property also sets the <see cref="IsExpandedSubGraph"/> dependency property.
    /// </remarks>
    public static void SetIsSubGraphExpanded(DependencyObject d, bool v) { d.SetValue(IsSubGraphExpandedProperty, v); }
    private static void OnIsSubGraphExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Group group = VerifyParentAs<Group>(d, e);
      if (group != null) {
        group.DoIsSubGraphExpandedChanged();
      }
    }
    // don't define IsSubGraphExpanded as a property on Node, to avoid confusion with IsExpandedSubGraph

    private void DoIsSubGraphExpandedChanged() {
      FrameworkElement elt = this.VisualElement;
      if (elt == null) return;
      bool vis = Group.GetIsSubGraphExpanded(elt);

      this.IsExpandedSubGraph = vis;

      if (vis && this.Diagram != null) {
        PartManager mgr = this.Diagram.PartManager;
        if (mgr != null) mgr.RealizeSubGraphMembers(this);
      }

      foreach (Link l in this.MemberLinks) l.Visible = vis;
      foreach (Node n in this.MemberNodes) n.Visible = vis;
    }


    /// <summary>
    /// Identifies the <c>WasSubGraphExpanded</c> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty WasSubGraphExpandedProperty;  // must be on the root visual element
    /// <summary>
    /// Gets the value of the <see cref="WasSubGraphExpandedProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static bool GetWasSubGraphExpanded(DependencyObject d) { return (bool)d.GetValue(WasSubGraphExpandedProperty); }
    /// <summary>
    /// Sets the value of the <see cref="WasSubGraphExpandedProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetWasSubGraphExpanded(DependencyObject d, bool v) { d.SetValue(WasSubGraphExpandedProperty, v); }
    // don't define WasSubGraphExpanded as a property on Node, to avoid confusion and clutter


    /// <summary>
    /// Identifies the <see cref="Layout"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty LayoutProperty;
    /// <summary>
    /// Gets the value of the <see cref="LayoutProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static IDiagramLayout GetLayout(DependencyObject d) { return (IDiagramLayout)d.GetValue(LayoutProperty); }
    /// <summary>
    /// Sets the value of the <see cref="LayoutProperty"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetLayout(DependencyObject d, IDiagramLayout v) { d.SetValue(LayoutProperty, v); }
    private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Group group = VerifyParentAs<Group>(d, e);
      if (group != null) {
        // also done by OnApplyTemplate, if the parent Group was not found during init
        Diagram diagram = group.Diagram;
        IDiagramLayout oldlayout = e.OldValue as IDiagramLayout;
        if (oldlayout != null) {
          oldlayout.Diagram = null;
          oldlayout.Group = null;
        }
        IDiagramLayout layout = e.NewValue as IDiagramLayout;
        if (layout != null) {
          if (layout.Diagram != null && layout.Diagram != diagram) {
            Diagram.Error("An IDiagramLayout cannot be shared by multiple Diagrams");
          }
          if (layout.Group != null && layout.Group != group) {
            Diagram.Error("An IDiagramLayout cannot be shared by multiple Nodes");
          }
          layout.Diagram = diagram;
          layout.Group = group;
          DiagramLayout dlay = layout as DiagramLayout;
          if (dlay != null) dlay.DataContext = group.DataContext;  //?? support databinding of Layout properties
          group.InvalidateLayout(LayoutChange.GroupLayoutChanged);
        }
      }
    }
    /// <summary>
    /// Gets or sets the <see cref="IDiagramLayout"/> used to position the member nodes
    /// and route the member links of this <see cref="Group"/>.
    /// </summary>
    /// <value>
    /// Initially this is an instance of <see cref="Northwoods.GoXam.Layout.DiagramLayout"/>.
    /// If the value is null, there is no automatic layout of the member nodes of this group,
    /// and the member nodes may be laid out by the containing group's layout, or else by
    /// the diagram's <see cref="Northwoods.GoXam.Diagram.Layout"/>.
    /// </value>
    public IDiagramLayout Layout {  // for convenience, treat the attached property on the VisualElement as if it were this Part's property
      get { FrameworkElement elt = this.VisualElement; if (elt != null) return GetLayout(elt); else return null; }
      set { FrameworkElement elt = this.EnsuredVisualElement; if (elt != null) SetLayout(elt, value); }
    }


    // members

    /// <summary>
    /// Get a collection of <see cref="Node"/>s that are members of this group.
    /// </summary>
    public IEnumerable<Node> MemberNodes {
      get {
        IDiagramModel model = this.Model;
        IGroupsModel gmodel = model as IGroupsModel;
        if (gmodel != null) {
          Object data = this.Data;
          if (data == null) return NoNodes;
          IEnumerable<Object> nodesdata = gmodel.GetMemberNodesForGroup(data);
          Diagram diagram = this.Diagram;
          if (diagram == null) return NoNodes;
          PartManager mgr = diagram.PartManager;
          if (mgr == null) return NoNodes;
          return nodesdata.Select(d => mgr.FindNodeForData(d, model)).Where(n => n != null).ToList();
        }
        return NoNodes;
      }
    }

    internal List<Link> CachedMemberLinks { get; set; }

    /// <summary>
    /// Get a collection of <see cref="Link"/>s that are members of this group.
    /// </summary>
    public IEnumerable<Link> MemberLinks {
      get {
        IDiagramModel model = this.Model;
        ISubGraphLinksModel sglmodel = model as ISubGraphLinksModel;
        if (sglmodel != null) {
          Object data = this.Data;
          if (data == null) return NoLinks;
          IEnumerable<Object> linksdata = sglmodel.GetMemberLinksForGroup(data);
          Diagram diagram = this.Diagram;
          if (diagram == null) return NoLinks;
          PartManager mgr = diagram.PartManager;
          if (mgr == null) return NoLinks;
          return linksdata.Select(d => mgr.FindLinkForData(d, model)).Where(l => l != null).ToList();
        } else {
          ISubGraphModel sgmodel = model as ISubGraphModel;
          if (sgmodel != null) {
            if (this.CachedMemberLinks == null) return NoLinks;
            return this.CachedMemberLinks;
          } //?? else handle ISetsModel
        }
        return NoLinks;
      }
    }


    internal IEnumerable<Part> MemberPartsAll {
      get {
        IDiagramModel model = this.Model;
        IGroupsModel gmodel = model as IGroupsModel;
        if (gmodel != null) {
          IEnumerable<Node> immediatenodes = this.MemberNodes;
          IEnumerable<Link> immediatelinks = this.MemberLinks;
          IEnumerable<Part> grandchildren = immediatenodes.OfType<Group>().SelectMany(g => g.MemberPartsAll);
          return immediatenodes.OfType<Part>().Concat(immediatelinks.OfType<Part>()).Concat(grandchildren);
        }
        return NoParts;
      }
    }
    
    // containers

    /// <summary>
    /// Collapse each of the member nodes and links,
    /// and recursively collapse any member groups.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This also calls <see cref="SetIsSubGraphExpanded"/> to set that attached property,
    /// and <see cref="SetWasSubGraphExpanded"/> to remember whether a nested group had been expanded,
    /// to support data binding the state of the group.
    /// </para>
    /// <para>
    /// This operation is performed within a "Collapse SubGraph" transaction.
    /// </para>
    /// </remarks>
    public void CollapseSubGraph() {
      const String Action = "Collapse SubGraph";
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      // avoid multiple simultaneous recursions
      if (diagram.IsCollapsingExpanding) return;
      diagram.IsCollapsingExpanding = true;
      diagram.StartTransaction(Action);
      HashSet<Part> members = new HashSet<Part>(this.MemberPartsAll);
      CollapseSubGraph1(members);
      diagram.CommitTransaction(Action);
      diagram.IsCollapsingExpanding = false;
    }

    private void CollapseSubGraph1(HashSet<Part> allmembers) {
      FrameworkElement ve = this.EnsuredVisualElement;
      if (ve == null) return;

      // change the visibility of the members
      bool oldexp = Group.GetIsSubGraphExpanded(ve);
      Group.SetIsSubGraphExpanded(ve, false);  // also sets IsExpandedSubGraph carefully

      foreach (Node n in this.MemberNodes) {
        Group g = n as Group;
        if (g != null) {
          FrameworkElement nve = g.EnsuredVisualElement;
          if (nve != null) {
            if (Group.GetIsSubGraphExpanded(nve)) {
              Group.SetWasSubGraphExpanded(nve, true);
              g.CollapseSubGraph1(allmembers);
            } else if (!oldexp && n.Visible) {  // was collapsed, but member is Visible?
              Group.SetWasSubGraphExpanded(nve, true);
              g.CollapseSubGraph1(allmembers);  // collapse it anyway
              g.Visible = false;   // and make sure it's no longer Visible
            } else {
              Group.SetWasSubGraphExpanded(nve, false);
            }
          }
        }
        n.InvalidateConnectedLinks(allmembers);
      }
      foreach (Link l in this.MemberLinks) {
        Node lab = l.LabelNode;
        if (lab != null) lab.InvalidateConnectedLinks(allmembers);
      }

      InvalidateRelationships("Collapse SubGraph", false);  // invalidate links and remeasure

      // maybe delete all member Nodes and Links
      Diagram diagram = this.Diagram;
      if (diagram != null && diagram.PartManager != null) diagram.PartManager.ReleaseSubGraphMembers(this);
    }

    /// <summary>
    /// Make visible each member node and link,
    /// and perhaps recursively expand nested subgraphs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This will expand a nested member group only if its <see cref="GetWasSubGraphExpanded"/> was true.
    /// </para>
    /// <para>
    /// This also calls <see cref="SetIsSubGraphExpanded"/> to set that attached property,
    /// to support data binding the state of the group.
    /// </para>
    /// <para>
    /// This operation is performed within an "Expand SubGraph" transaction.
    /// </para>
    /// </remarks>
    public void ExpandSubGraph() {
      const String Action = "Expand SubGraph";
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      // avoid multiple simultaneous recursions
      if (diagram.IsCollapsingExpanding) return;
      diagram.IsCollapsingExpanding = true;
      diagram.StartTransaction(Action);
      HashSet<Part> members = new HashSet<Part>(this.MemberPartsAll);
      ExpandSubGraph1(members);
      diagram.CommitTransaction(Action);
      diagram.IsCollapsingExpanding = false;
    }

    private void ExpandSubGraph1(HashSet<Part> allmembers) {
      FrameworkElement ve = this.EnsuredVisualElement;
      if (ve == null) return;

      // change the visibility of the members
      bool oldexp = Group.GetIsSubGraphExpanded(ve);
      Group.SetIsSubGraphExpanded(ve, true);  // also sets IsExpandedSubGraph carefully

      // now update all of the member Nodes and Links
      bool unpositioned = false;
      Point loc = this.Location;
      foreach (Node n in this.MemberNodes) {
        Group g = n as Group;
        if (g != null) {
          FrameworkElement nve = g.EnsuredVisualElement;
          if (nve != null) {
            if (!Group.GetIsSubGraphExpanded(nve) && Group.GetWasSubGraphExpanded(nve)) {
              g.ExpandSubGraph1(allmembers);
            } else if (oldexp && !n.Visible) {  // was expanded, but member is not Visible?
              g.ExpandSubGraph1(allmembers);  // expand it anway
              g.Visible = true;  // and make sure it's Visible
            }
          }
        }
        Rect b = n.Bounds;
        if (Double.IsNaN(b.X) || Double.IsNaN(b.Y)) {
          unpositioned = true;
          if (!Double.IsNaN(loc.X) && !Double.IsNaN(loc.Y)) n.Position = loc;
        }
        n.InvalidateConnectedLinks(allmembers);
      }
      foreach (Link l in this.MemberLinks) {
        Node lab = l.LabelNode;
        if (lab != null) lab.InvalidateConnectedLinks(allmembers);
      }

      if (unpositioned) {
        IDiagramLayout layout = this.Layout;
        if (layout != null) layout.Invalidate(LayoutChange.All, null);
      }
      InvalidateRelationships("Expand SubGraph", false);
    }
  }  // end of Group


  /// <summary>
  /// A <c>Link</c> is a <see cref="Part"/> that represents a relationship between <see cref="Node"/>s.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A link connects the <see cref="FromNode"/> to the <see cref="ToNode"/>.
  /// </para>
  /// <para>
  /// The rendering of a link is determined by the <see cref="Northwoods.GoXam.Diagram"/>'s <see cref="Northwoods.GoXam.Diagram.LinkTemplate"/>.
  /// This <c>DataTemplate</c> may be a simple <c>LinkShape</c> (WPF) or <c>Path</c> (Silverlight).
  /// However it is usually a <see cref="LinkPanel"/>, which supports child elements that act as arrowheads
  /// or text labels or other decoration on the link.
  /// </para>
  /// <para>
  /// A link may also have a <see cref="LabelNode"/>.
  /// In this case there is a separate <see cref="Node"/> that is associated with the link.
  /// The node's <see cref="Node.IsLinkLabel"/> property will be true.
  /// It is usually positioned at the mid-point of the link.
  /// It supports its own link connections and may be independently selected or moved.
  /// The label node may be a <see cref="Group"/>, in which case all of its member nodes
  /// are link label nodes that are positioned by the <see cref="LinkPanel"/>.
  /// </para>
  /// <para>
  /// You may not apply any transforms to a <c>Link</c>; that is reserved to GoXam for future use.
  /// </para>
  /// </remarks>
  [System.Windows.Markup.ContentProperty("Content")]
  public class Link : Part {

    static Link() {
      // Dependency properties
      PartsModelFromNodeProperty = DependencyProperty.Register("PartsModelFromNode", typeof(String), typeof(Link),
        new FrameworkPropertyMetadata(null, OnPartsModelFromNodeChanged));
      PartsModelFromPortIdProperty = DependencyProperty.Register("PartsModelFromPortId", typeof(String), typeof(Link),
        new FrameworkPropertyMetadata(null, OnPartsModelFromPortIdChanged));
      PartsModelToNodeProperty = DependencyProperty.Register("PartsModelToNode", typeof(String), typeof(Link),
        new FrameworkPropertyMetadata(null, OnPartsModelToNodeChanged));
      PartsModelToPortIdProperty = DependencyProperty.Register("PartsModelToPortId", typeof(String), typeof(Link),
        new FrameworkPropertyMetadata(null, OnPartsModelToPortIdChanged));
      PartsModelLabelNodeProperty = DependencyProperty.Register("PartsModelLabelNode", typeof(String), typeof(Link),
        new FrameworkPropertyMetadata(null, OnPartsModelLabelNodeChanged));

      // Attached properties

      // Must be on the root visual element
      RouteProperty = DependencyProperty.RegisterAttached("Route", typeof(Route), typeof(Link),
        new FrameworkPropertyMetadata(null, OnRouteChanged));
    }










    /// <summary>
    /// For convenience in debugging.
    /// </summary>
    /// <returns></returns>
    public override String ToString() {
      String from = this.PartsModelFromNode;
      String to = this.PartsModelToNode;
      if (from != null || to != null) {
        return "Link:" + (from ?? "") + " ==> " + (to ?? "");
      }
      Object data = this.Data;
      if (data != null) return "Link:" + data.ToString();
      return base.ToString();
    }


    /// <summary>
    /// Gets the <c>Shape</c> representing the path used to render the route of this link.
    /// </summary>
    /// <value>
    /// This will be null if this link uses a <see cref="LinkPanel"/> and if
    /// that panel's <see cref="LinkPanel.Implementation"/> is not <see cref="LinkPanelImplementation.Path"/>.
    /// </value>
    public Shape Path {
      get {
        if (_Path == null) {  // if there's no LinkPanel, maybe the whole element is the path
          FrameworkElement ve = this.VisualElement;
          _Path = ve as Shape;  // a LinkShape (WPF) or a Path (Silverlight)
          if (_Path == null) {
            LinkPanel lp = ve as LinkPanel;
            if (lp != null) {
              _Path = lp.Path;
            }
          }
        }
        return _Path;
      }
      internal set { _Path = value; }
    }
    private Shape _Path;  // LinkPanel will set this if LinkType == Path


    // invalidation/updating

    internal override void InvalidateOtherPartRelationships(bool arrangeonly) {
      // invalidate the Route
      Route route = this.Route;
      if (route != null) {
        // this will invalidate the measurement of the Path and any containing LinkPanel
        // which in turn will move any LabelNode and thus invalidate its relationships
        route.InvalidateRoute();
      }
    }

    // a link is ready to be measure/arranged if both of its nodes have been arranged
    internal override bool IsReadyToMeasureArrange() {
      Node f = this.FromNode;
      if (f == null || !f.ValidArrange) return false;
      Node t = this.ToNode;
      if (t == null || !t.ValidArrange) return false;
      return true;
    }

    internal override void MeasureMore() {
      Route route = this.Route;
      if (route != null) {

        Path path = this.Path as Path;
        if (path != null) {
          path.Data = route.DefiningGeometry;
          if (path == this.VisualElement) {  // do what LinkPanel does
            Rect b = route.RouteBounds;
            path.Measure(Geo.Unlimited);
            Size sz = path.DesiredSize;
            b.Width = Math.Max(b.Width, sz.Width);
            b.Height = Math.Max(b.Height, sz.Height);
            this.Bounds = b;
          }  // otherwise LinkPanel can set this.Bounds




          // InvalidateVisual also gives any LinkPanel a chance to re-arrange too, moving any labels, turning any arrowheads
          InvalidateVisual(path);
        } else {  // path == null -- maybe it's a polygon or whatever representing the whole link
          Rect b = route.RouteBounds;
          this.Bounds = b;
          FrameworkElement ve = this.VisualElement;
          if (ve != null) ve.InvalidateMeasure();  // give any LinkPanel a chance to remeasure/rearrange
        }
      }
    }


    internal override void PropagateVisible(bool vis) {
      Node lab = this.LabelNode;
      if (lab != null) {
        if (lab.Visible != vis) {
          lab.Visible = vis;
          lab.PropagateVisible(vis);
        }
      }
    }


    internal override bool IntersectsRect(Rect b) {
      Rect r = this.Bounds;
      if (!Double.IsNaN(r.X) && !Double.IsNaN(r.Y) && Geo.Intersects(r, b)) return true;
      Node from = this.FromNode;
      if (from != null && from.IntersectsRect(b)) return true;
      Node to = this.ToNode;
      if (to != null && to.IntersectsRect(b)) return true;
      return false;
    }


    /// <summary>
    /// The angle of any <see cref="Link"/>'s <see cref="Part.VisualElement"/> or <see cref="Path"/> is zero,
    /// however an element within the visual tree may have an angle.
    /// </summary>
    /// <param name="elt"></param>
    /// <returns></returns>
    public override double GetAngle(UIElement elt) {
      if (elt == null || elt == this.VisualElement || elt == this.Path) return 0;
      return base.GetAngle(elt);
    }

    /// <summary>
    /// You cannot set the angle of the link's <see cref="Part.VisualElement"/> or <see cref="Path"/>,
    /// since they must always be zero, but you can set the angle of other elements in the visual tree.
    /// </summary>
    /// <param name="elt"></param>
    /// <param name="angle"></param>
    /// <param name="focus"></param>
    public override void SetAngle(UIElement elt, double angle, Spot focus) {
      if (elt == null || elt == this.VisualElement || elt == this.Path) return;
      base.SetAngle(elt, angle, focus);
    }


    /// <summary>
    /// Initialize the <see cref="Route"/>.
    /// </summary>
    public override void OnApplyTemplate() {
      base.OnApplyTemplate();
      this._Path = null;  // clear cached reference to nested LinkShape or Path
      // do stuff OnRouteChanged wanted to do, but couldn't
      Route route = this.Route;
      if (route != null) {
        route.Link = this;  // set back-pointer
        route.DataContext = this.DataContext;
      }
    }


    // PartsModel properties

    /// <summary>
    /// Identifies the <see cref="PartsModelFromNode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PartsModelFromNodeProperty;
    /// <summary>
    /// Gets or sets the identifier naming the "from" node for this link in a <see cref="PartsModel"/>.
    /// </summary>
    /// <value>
    /// The default value is null, indicating that this link is not connected to any node.
    /// </value>
    /// <remarks>
    /// This property is only used by unbound links that are in a <see cref="PartsModel"/> model.
    /// </remarks>
    public String PartsModelFromNode {
      get { return (String)GetValue(PartsModelFromNodeProperty); }
      set { SetValue(PartsModelFromNodeProperty, value); }
    }
    private static void OnPartsModelFromNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Link link = (Link)d;
      Diagram diagram = link.Diagram;
      if (diagram != null) {
        diagram.PartsModel.DoLinkPortsChanged(link);
      }
    }

    /// <summary>
    /// Identifies the <see cref="PartsModelFromPortId"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PartsModelFromPortIdProperty;
    /// <summary>
    /// Gets or sets a particular value identifying the "from" port that this link is connected to.
    /// </summary>
    /// <value>
    /// The default value is null.
    /// </value>
    /// <remarks>
    /// This property is only used by unbound links that are in a <see cref="PartsModel"/> model.
    /// </remarks>
    public String PartsModelFromPortId {
      get { return (String)GetValue(PartsModelFromPortIdProperty); }
      set { SetValue(PartsModelFromPortIdProperty, value); }
    }
    private static void OnPartsModelFromPortIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Link link = (Link)d;
      Diagram diagram = link.Diagram;
      if (diagram != null) {
        diagram.PartsModel.DoLinkPortsChanged(link);
      }
    }

    /// <summary>
    /// Identifies the <see cref="PartsModelToNode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PartsModelToNodeProperty;
    /// <summary>
    /// Gets or sets the identifier naming the "to" node for this link in a <see cref="PartsModel"/>.
    /// </summary>
    /// <value>
    /// The default value is null, indicating that this link is not connected to any node.
    /// </value>
    /// <remarks>
    /// This property is only used by unbound links that are in a <see cref="PartsModel"/> model.
    /// </remarks>
    public String PartsModelToNode {
      get { return (String)GetValue(PartsModelToNodeProperty); }
      set { SetValue(PartsModelToNodeProperty, value); }
    }
    private static void OnPartsModelToNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Link link = (Link)d;
      Diagram diagram = link.Diagram;
      if (diagram != null) {
        diagram.PartsModel.DoLinkPortsChanged(link);
      }
    }

    /// <summary>
    /// Identifies the <see cref="PartsModelToPortId"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PartsModelToPortIdProperty;
    /// <summary>
    /// Gets or sets a particular value identifying the "to" port that this link is connected to.
    /// </summary>
    /// <value>
    /// The default value is null.
    /// </value>
    /// <remarks>
    /// This property is only used by unbound links that are in a <see cref="PartsModel"/> model.
    /// </remarks>
    public String PartsModelToPortId {
      get { return (String)GetValue(PartsModelToPortIdProperty); }
      set { SetValue(PartsModelToPortIdProperty, value); }
    }
    private static void OnPartsModelToPortIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Link link = (Link)d;
      Diagram diagram = link.Diagram;
      if (diagram != null) {
        diagram.PartsModel.DoLinkPortsChanged(link);
      }
    }

    /// <summary>
    /// Identifies the <see cref="PartsModelLabelNode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PartsModelLabelNodeProperty;
    /// <summary>
    /// Gets or sets the identifier naming the "label" node for this link in a <see cref="PartsModel"/>.
    /// </summary>
    /// <value>
    /// The default value is null, indicating that this link does not have any node as a label.
    /// </value>
    /// <remarks>
    /// This property is only used by unbound links that are in a <see cref="PartsModel"/> model.
    /// </remarks>
    public String PartsModelLabelNode {
      get { return (String)GetValue(PartsModelLabelNodeProperty); }
      set { SetValue(PartsModelLabelNodeProperty, value); }
    }
    private static void OnPartsModelLabelNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Link link = (Link)d;
      Diagram diagram = link.Diagram;
      if (diagram != null) {
        diagram.PartsModel.DoLinkLabelChanged(link);
      }
    }


    // Route property
    // the Route object is kept on the root visual element of the Link
    // every Link always has a Route, either explicitly assigned or automatically created on demand via the this.Route getter

    /// <summary>
    /// Identifies the <see cref="Route"/> attached dependency property
    /// that must only be on the root <see cref="Part.VisualElement"/>.
    /// </summary>
    public static readonly DependencyProperty RouteProperty;  // only on root VisualElement
    /// <summary>
    /// Gets the value of the <see cref="Route"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <returns></returns>
    public static Route GetRoute(DependencyObject d) { return (Route)d.GetValue(RouteProperty); }
    /// <summary>
    /// Sets the value of the <see cref="Route"/> attached dependency property.
    /// </summary>
    /// <param name="d">the root <see cref="Part.VisualElement"/></param>
    /// <param name="v"></param>
    public static void SetRoute(DependencyObject d, Route v) { d.SetValue(RouteProperty, v); }
    private static void OnRouteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Route route = (Route)e.NewValue;
      if (route == null) {
        SetRoute(d, new Route());  // don't allow null
      } else {
        Link link = VerifyParentAs<Link>(d, e);  // make sure the immediate parent is a Link
        if (link != null) {
          // also done by OnApplyTemplate, if no parent Link was found...
          route.Link = link;  // set back-pointer
          route.DataContext = link.DataContext;
          link._Path = null;  // clear cached path reference
        }
      }
    }

    /// <summary>
    /// Gets or sets the <see cref="Northwoods.GoXam.Route"/> implementing the path that
    /// this link will take.
    /// </summary>
    /// <value>
    /// A <see cref="Northwoods.GoXam.Route"/> that was specified as an attached property on
    /// the <see cref="Part.VisualElement"/>.
    /// If none was defined, this creates one automatically.
    /// </value>
    public Route Route {  // for convenience, treat the attached property on the VisualElement as if it were this Link's property
      get {
        FrameworkElement elt = this.VisualElement;
        if (elt != null) {
          Route r = GetRoute(elt);  // make sure the Route exists and has a back-pointer to this Link
          if (r == null) {
            r = new Route();
            SetRoute(elt, r);
          }
          r.Link = this;
          return r;
        }
        return null;
      }
      set {
        FrameworkElement elt = this.EnsuredVisualElement;
        if (elt != null) SetRoute(elt, value);
      }
    }

    internal LinkBundle Bundle { get; set; }
    internal int BundleIndex { get; set; }

    // other Link properties

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
      get { return this.Route.RelinkableFrom; }
      set { this.Route.RelinkableFrom = value; }
    }
    /// <summary>
    /// This predicate is true if the user may reconnect the "from" end of
    /// this link.
    /// </summary>
    /// <returns>
    /// Return true if this link is <see cref="RelinkableFrom"/>,
    /// if this link's layer's <see cref="Northwoods.GoXam.Layer.AllowRelink"/> is true,
    /// and if this link's diagram's <see cref="Northwoods.GoXam.Diagram.AllowRelink"/> is true.
    /// </returns>
    public bool CanRelinkFrom() { return this.Route.CanRelinkFrom(); }

    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used by the <see cref="Northwoods.GoXam.Tool.RelinkingTool"/>
    /// when this part is selected.
    /// </summary>
    /// <value>
    /// The default value is null, meaning use the default template.
    /// </value>
    public DataTemplate RelinkFromAdornmentTemplate {
      get { return this.Route.RelinkFromAdornmentTemplate; }
      set { this.Route.RelinkFromAdornmentTemplate = value; }
    }

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
      get { return this.Route.RelinkableTo; }
      set { this.Route.RelinkableTo = value; }
    }
    /// <summary>
    /// This predicate is true if the user may reconnect the "to" end of
    /// this link.
    /// </summary>
    /// <returns>
    /// Return true if this link is <see cref="RelinkableTo"/>,
    /// if this link's layer's <see cref="Northwoods.GoXam.Layer.AllowRelink"/> is true,
    /// and if this link's diagram's <see cref="Northwoods.GoXam.Diagram.AllowRelink"/> is true.
    /// </returns>
    public bool CanRelinkTo() { return this.Route.CanRelinkTo(); }

    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used by the <see cref="Northwoods.GoXam.Tool.RelinkingTool"/>
    /// when this part is selected.
    /// </summary>
    /// <value>
    /// The default value is null, meaning use the default template.
    /// </value>
    public DataTemplate RelinkToAdornmentTemplate {
      get { return this.Route.RelinkToAdornmentTemplate; }
      set { this.Route.RelinkToAdornmentTemplate = value; }
    }

    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used by the <see cref="Northwoods.GoXam.Tool.LinkReshapingTool"/>
    /// for each <see cref="Route"/> point's handle.
    /// </summary>
    /// <value>
    /// The default value is null, meaning use the default template.
    /// </value>
    public DataTemplate LinkReshapeHandleTemplate {
      get { return this.Route.LinkReshapeHandleTemplate; }
      set { this.Route.LinkReshapeHandleTemplate = value; }
    }


    // convenience properties for navigation
    // these go through the model but operate on/with Parts

    /// <summary>
    /// Get the data object that this link is coming from.
    /// </summary>
    /// <value>
    /// This is not a <see cref="Node"/> unless this link is in a <see cref="PartsModel"/>.
    /// </value>
    public Object FromData {
      get {
        Object data = this.Data;
        if (data == null) return null;
        PartManager.VirtualLinkData vl = data as PartManager.VirtualLinkData;
        if (vl != null) {
          return vl.From;
        }
        IDiagramModel model = this.Model;
        ILinksModel lmodel = model as ILinksModel;
        if (lmodel != null) {
          return lmodel.GetFromNodeForLink(data);
        }
        return null;
      }
    }

    /// <summary>
    /// Get the <see cref="Node"/> that corresponds to the
    /// <see cref="FromData"/> for this link.
    /// </summary>
    public Node FromNode {
      get {
        Object data = this.FromData;
        Diagram diagram = this.Diagram;
        if (diagram == null) return null;
        PartManager mgr = diagram.PartManager;
        if (mgr == null) return null;
        return mgr.FindNodeForData(data, diagram.Model);
      }
    }

    /// <summary>
    /// Get the name of the port associated with the "from" end of this link.
    /// </summary>
    /// <value>
    /// This only returns a string when the model is an <see cref="Northwoods.GoXam.Model.ILinksModel"/>,
    /// at which time it converts to a string the value returned by <see cref="Northwoods.GoXam.Model.ILinksModel.GetFromParameterForLink"/>.
    /// </value>
    public String FromPortId {
      get {
        IDiagramModel model = this.Model;
        ILinksModel lmodel = model as ILinksModel;
        if (lmodel != null) {
          Object data = this.Data;
          if (data == null) return null;
          Object param = lmodel.GetFromParameterForLink(data);
          return (param != null ? param.ToString() : null);
        }
        return null;
      }
    }

    /// <summary>
    /// Get the <c>FrameworkElement</c> that corresponds to the
    /// <see cref="FromPortId"/> for this link.
    /// </summary>
    public FrameworkElement FromPort {
      get {
        Node node = this.FromNode;
        if (node == null) return null;
        String portparam = this.FromPortId;
        return node.FindPort(portparam, true);
      }
    }


    /// <summary>
    /// Get the data object that this link is going to.
    /// </summary>
    /// <value>
    /// This is not a <see cref="Node"/> unless this link is in a <see cref="PartsModel"/>.
    /// </value>
    public Object ToData {
      get {
        Object data = this.Data;
        if (data == null) return null;
        PartManager.VirtualLinkData vl = data as PartManager.VirtualLinkData;
        if (vl != null) {
          return vl.To;
        }
        IDiagramModel model = this.Model;
        ILinksModel lmodel = model as ILinksModel;
        if (lmodel != null) {
          return lmodel.GetToNodeForLink(data);
        }
        return null;
      }
    }

    /// <summary>
    /// Get the <see cref="Node"/> that corresponds to the
    /// <see cref="ToData"/> for this link.
    /// </summary>
    public Node ToNode {
      get {
        Object data = this.ToData;
        Diagram diagram = this.Diagram;
        if (diagram == null) return null;
        PartManager mgr = diagram.PartManager;
        if (mgr == null) return null;
        return mgr.FindNodeForData(data, diagram.Model);
      }
    }

    /// <summary>
    /// Get the name of the port associated with the "to" end of this link.
    /// </summary>
    /// <value>
    /// This only returns a string when the model is an <see cref="Northwoods.GoXam.Model.ILinksModel"/>,
    /// at which time it converts to a string the value returned by <see cref="Northwoods.GoXam.Model.ILinksModel.GetToParameterForLink"/>.
    /// </value>
    public String ToPortId {
      get {
        IDiagramModel model = this.Model;
        ILinksModel lmodel = model as ILinksModel;
        if (lmodel != null) {
          Object data = this.Data;
          if (data == null) return null;
          Object param = lmodel.GetToParameterForLink(data);
          return (param != null ? param.ToString() : null);
        }
        return null;
      }
    }

    /// <summary>
    /// Get the <c>FrameworkElement</c> that corresponds to the
    /// <see cref="ToPortId"/> for this link.
    /// </summary>
    public FrameworkElement ToPort {
      get {
        Node node = this.ToNode;
        if (node == null) return null;
        String portparam = this.ToPortId;
        return node.FindPort(portparam, true);
      }
    }


    /// <summary>
    /// Given a <see cref="Node"/>, return the node at the other end of this link.
    /// </summary>
    /// <param name="node"></param>
    /// <returns>This may return the same node, if the link is reflexive</returns>
    public Node GetOtherNode(Node node) {
      Node from = this.FromNode;
      if (node == from)
        return this.ToNode;
      else
        return from;
    }

    /// <summary>
    /// Given a <c>FrameworkElement</c> that is a "port", return the port at the other end of this link.
    /// </summary>
    /// <param name="port"></param>
    /// <returns>This may return the same element, if the link is reflexive</returns>
    public FrameworkElement GetOtherPort(FrameworkElement port) {
      FrameworkElement from = this.FromPort;
      if (port == from)
        return this.ToPort;
      else
        return from;
    }


    /// <summary>
    /// Get a collection of <see cref="Group"/>s of which this part is a member.
    /// </summary>
    public override IEnumerable<Group> ContainingGroups {
      get {
        IDiagramModel model = this.Model;
        ISubGraphLinksModel sglmodel = model as ISubGraphLinksModel;
        if (sglmodel != null) {
          Object data = this.Data;
          if (data == null) return NoGroups;
          Object sgdata = sglmodel.GetGroupForLink(data);
          if (sgdata == null) return NoGroups;
          Diagram diagram = this.Diagram;
          if (diagram == null) return NoGroups;
          PartManager mgr = diagram.PartManager;
          if (mgr == null) return NoGroups;
          Group g = mgr.FindNodeForData(sgdata, model) as Group;
          if (g != null) return new Group[1] { g };
        } else {
          ISubGraphModel sgmodel = model as ISubGraphModel;
          if (sgmodel != null) {
            if (this.CachedParentSubGraph != null)
              return new Group[1] { this.CachedParentSubGraph };
            else
              return NoGroups;
          }
        } //?? else handle ISetsLinksModel/ISetsModel
        return NoGroups;
      }
    }

    internal Group CachedParentSubGraph { get; set; }

    /// <summary>
    /// Get the group <see cref="Group"/> that this part is a member of, if any.
    /// </summary>
    public override Group ContainingSubGraph {
      get {
        IDiagramModel model = this.Model;
        ISubGraphLinksModel sglmodel = model as ISubGraphLinksModel;
        if (sglmodel != null) {
          Object data = this.Data;
          if (data == null) return null;
          Object sgdata = sglmodel.GetGroupForLink(data);
          if (sgdata == null) return null;
          Diagram diagram = this.Diagram;
          if (diagram == null) return null;
          PartManager mgr = diagram.PartManager;
          if (mgr == null) return null;
          return mgr.FindNodeForData(sgdata, model) as Group;
        } else {
          ISubGraphModel sgmodel = model as ISubGraphModel;
          if (sgmodel != null) {
            return this.CachedParentSubGraph;
          }
        } //?? else handle ISetsLinksModel/ISetsModel
        return null;
      }
    }

    /// <summary>
    /// Gets the <see cref="Node"/>, if any, that is a label for this link.
    /// </summary>
    public Node LabelNode {
      get {
        IDiagramModel model = this.Model;
        ILinksModel lmodel = model as ILinksModel;
        if (lmodel != null) {
          Object data = this.Data;
          if (data == null) return null;
          Object labdata = lmodel.GetLabelNodeForLink(data);
          if (labdata == null) return null;
          Diagram diagram = this.Diagram;
          if (diagram == null) return null;
          PartManager mgr = diagram.PartManager;
          if (mgr == null) return null;
          return mgr.FindNodeForData(labdata, model);
        }
        return null;
      }
    }

  }  // end of Link


  //?? public enum DragOverBehavior {
  //  None=0,
  //  KeepsOutside=1,
  //  KeepsInside=2,
  //  PushesAside=4
  //}

  /// <summary>
  /// Specify additional behavior when dragging and dropping the selection
  /// onto existing parts in the diagram.
  /// </summary>
  public enum DropOntoBehavior {
    /// <summary>
    /// Don't do anything else besides the standard result of moving or copying the selection.
    /// </summary>
    /// <remarks>
    /// This is the default value for <see cref="Part.DropOntoBehavior"/>.
    /// </remarks>
    None = 0,
    /// <summary>
    /// If the drop occurs on a <see cref="Group"/>, and if the group can accept having
    /// the selected nodes and links added as new members, then add them to that group.
    /// </summary>
    /// <remarks>
    /// This is only effective if <see cref="DraggingTool.DropOntoEnabled"/> is true.
    /// </remarks>
    AddsToGroup = 1,
    /// <summary>
    /// If the drop occurs on a <see cref="Node"/>, new links are created from the 
    /// dropped-onto node to each selected dropped node.
    /// </summary>
    /// <remarks>
    /// This does not draw new links to dropped nodes that were members of dropped groups
    /// (i.e. members of selected groups).
    /// </remarks>
    AddsLinkFromNode = 2,
    /// <summary>
    /// If the drop occurs on a <see cref="Node"/>, new links are created from the 
    /// each selected dropped node to the dropped-onto node.
    /// </summary>
    /// <remarks>
    /// This does not draw new links from dropped nodes that were members of dropped groups
    /// (i.e. members of selected groups).
    /// </remarks>
    AddsLinkToNode = 4,
    /// <summary>
    /// If the drop occurs on a <see cref="Link"/>, each selected dropped node is
    /// connected by a link from the original link's <see cref="Link.FromNode"/>
    /// and by a link to the original link's <see cref="Link.ToNode"/>, and the
    /// original link is removed.
    /// </summary>
    SplicesIntoLink = 8
  }
}
