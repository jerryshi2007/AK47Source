
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Northwoods.GoXam.Layout;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;

namespace Northwoods.GoXam {

  /// <summary>
  /// A <c>Diagram</c> is a <c>Control</c> that includes a <see cref="DiagramPanel"/>
  /// that holds some <see cref="Layer"/>s that display <see cref="Part"/>s such as
  /// <see cref="Node"/>s and <see cref="Link"/>s.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Each <c>Diagram</c> has a number of standard objects that it uses to perform its duties.
  /// These objects are accessible via the following properties:
  /// <see cref="Model"/>,
  /// <see cref="PartManager"/>,
  /// <see cref="LayoutManager"/>,
  /// <see cref="CommandHandler"/>,
  /// <see cref="CurrentTool"/>,
  /// <see cref="DefaultTool"/>,
  /// and various other tool properties.
  /// </para>
  /// <para>
  /// The <see cref="Model"/> is the object that holds and describes all of the data
  /// for which <see cref="Part"/>s are created and displayed.
  /// The purpose of the model is to recognize relationships between the data --
  /// in particular link relationships between nodes and group memberships between nodes.
  /// There are different kinds of models, such as
  /// <see cref="Northwoods.GoXam.Model.TreeModel{NodeType,NodeKey}"/>,
  /// <see cref="Northwoods.GoXam.Model.GraphModel{NodeType,NodeKey}"/> and
  /// <see cref="Northwoods.GoXam.Model.GraphLinksModel{NodeType,NodeKey,PortKey,LinkType}"/>.
  /// </para>
  /// <para>
  /// The default model is a <see cref="Northwoods.GoXam.Model.UniversalGraphLinksModel"/>.
  /// You will typically want to replace it with your own specialized model.
  /// The default model does not have an <see cref="Northwoods.GoXam.Model.UndoManager"/> either;
  /// you may want to set <see cref="Northwoods.GoXam.Model.DiagramModel.HasUndoManager"/> to true.
  /// </para>
  /// <para>
  /// The <see cref="PartManager"/> is responsible for creating <see cref="Node"/>s
  /// corresponding to node data in the model, and for creating <see cref="Link"/>s
  /// corresponding to link relationships in the model.
  /// It notices the insertion or deletion of node data or link data or group-memberships,
  /// and updates the diagram appropriately.
  /// It maintains collections of the existing <see cref="Nodes"/> and <see cref="Links"/>.
  /// </para>
  /// <para>
  /// You can control the appearance of the nodes and links in the diagram by setting
  /// several data template properties:
  /// <see cref="NodeTemplate"/>,
  /// <see cref="GroupTemplate"/>,
  /// <see cref="LinkTemplate"/>.
  /// Also, because this is a standard <c>Control</c>, you can set its <c>Control.Template</c>
  /// property to a <c>ControlTemplate</c>.
  /// If you do not set these properties, its uses default templates to define the appearance
  /// of nodes, links, and of the control itself.
  /// If the value of a template property changes, the <see cref="PartManager"/>
  /// will rebuild the <see cref="Part"/>s that are affected.
  /// </para>
  /// <para>
  /// You can see the definitions of the standard templates
  /// in the Generic.XAML file that is in the docs subdirectory of the GoXam installation.
  /// </para>
  /// <para>
  /// The diagram also maintains the notion of selected parts.
  /// The <see cref="SelectedParts"/> property is an <c>ObservableCollection</c> of the currently
  /// selected <see cref="Part"/>s.
  /// The primary selection is held as the <see cref="SelectedPart"/> property.
  /// You can set <see cref="MaximumSelectionCount"/> to control how many parts may
  /// be selected at once.
  /// To select a <see cref="Part"/> you can set its <see cref="Part.IsSelected"/> property.
  /// </para>
  /// <para>
  /// There are many properties, named "Allow...", that control what operations the user
  /// may perform on the parts in the diagram.  These correspond to the same named
  /// properties on <see cref="Layer"/> that govern the behavior for those parts in a particular layer.
  /// Furthermore for some of these properties there are corresponding properties on
  /// <see cref="Part"/>, named "...able", that govern the behavior for that individual part.
  /// For example, the <see cref="AllowCopy"/> property corresponds to
  /// <see cref="Layer.AllowCopy"/> and to the property <see cref="Part.Copyable"/>.
  /// The <see cref="Part.CanCopy"/> predicate is false if any of these properties is false.
  /// </para>
  /// <para>
  /// The <see cref="CommandHandler"/> implements various standard commands,
  /// such as the <see cref="Northwoods.GoXam.CommandHandler.Delete"/> method and the
  /// <see cref="Northwoods.GoXam.CommandHandler.CanDelete"/> predicate.
  /// In WPF these methods are the implementation of the <c>Command</c>s listed as
  /// properties of the <c>Commands</c> static class.
  /// In Silverlight, which does not have <c>Command</c>s, the <see cref="CommandHandler"/>
  /// implements the corresponding keyboard events.
  /// </para>
  /// <para>
  /// The diagram also keeps track of the mouse-down point and last mouse point,
  /// in model coordinates: <see cref="FirstMousePointInModel"/> and <see cref="LastMousePointInModel"/>.
  /// </para>
  /// <para>
  /// The diagram supports modular behavior for mouse events by implementing "tools".
  /// "Mode-less" tools are held in three lists: <see cref="MouseDownTools"/>,
  /// <see cref="MouseMoveTools"/>, and <see cref="MouseUpTools"/>.
  /// The <see cref="CurrentTool"/>, which defaults to an instance of <see cref="ToolManager"/>,
  /// searches these lists when a mouse event happens to find the first tool that can run.
  /// It then makes that tool the new <see cref="CurrentTool"/>, where it can continue to
  /// process mouse events.
  /// When the tool is done, it stops itself, causing the <see cref="DefaultTool"/>
  /// to become the new <see cref="CurrentTool"/>.
  /// </para>
  /// <para>
  /// Standard tools include:
  /// <list>
  /// <item>
  /// <see cref="RelinkingTool"/> for reconnecting an existing <see cref="Link"/>
  /// </item>
  /// <item>
  /// <see cref="LinkReshapingTool"/> for reshaping the route of a <see cref="Link"/>
  /// </item>
  /// <item>
  /// <see cref="ResizingTool"/> for resizing a <see cref="Part"/> or an element inside a part
  /// </item>
  /// <item>
  /// <see cref="RotatingTool"/> for rotating a <see cref="Node"/> or an element inside a node
  /// </item>
  /// <item>
  /// <see cref="LinkingTool"/> for drawing a new <see cref="Link"/>
  /// </item>
  /// <item>
  /// <see cref="DraggingTool"/> for moving or (control-)copying selected <see cref="Part"/>s
  /// </item>
  /// <item>
  /// <see cref="DragSelectingTool"/> for rubber-band selection of some <see cref="Part"/>s within a rectangular area
  /// </item>
  /// <item>
  /// <see cref="PanningTool"/> for panning (scrolling) the diagram
  /// </item>
  /// <item>
  /// <see cref="TextEditingTool"/> for in-place editing of text
  /// </item>
  /// <item>
  /// <see cref="ClickCreatingTool"/> for inserting new <see cref="Node"/>s where the user clicks
  /// </item>
  /// <item>
  /// <see cref="ClickSelectingTool"/> for selecting <see cref="Part"/>s
  /// </item>
  /// </list>
  /// </para>
  /// <para>
  /// The diagram also raises various events when interesting things happen.
  /// These are routed events in WPF, but are just CLR events in Silverlight.
  /// </para>
  /// <para>
  /// The events include:
  /// <list>
  /// <item>
  /// <see cref="ClipboardPasted"/>
  /// </item>
  /// <item>
  /// <see cref="ExternalObjectsDropped"/>
  /// </item>
  /// <item>
  /// <see cref="InitialLayoutCompleted"/>
  /// </item>
  /// <item>
  /// <see cref="LayoutCompleted"/>
  /// </item>
  /// <item>
  /// <see cref="LinkDrawn"/>
  /// </item>
  /// <item>
  /// <see cref="LinkRelinked"/>
  /// </item>
  /// <item>
  /// <see cref="LinkReshaped"/>
  /// </item>
  /// <item>
  /// <see cref="ModelReplaced"/>
  /// </item>
  /// <item>
  /// <see cref="NodeCreated"/>
  /// </item>
  /// <item>
  /// <see cref="NodeResized"/>
  /// </item>
  /// <item>
  /// <see cref="NodeRotated"/>
  /// </item>
  /// <item>
  /// <see cref="SelectionChanged"/>
  /// </item>
  /// <item>
  /// <see cref="SelectionCopied"/>
  /// </item>
  /// <item>
  /// <see cref="SelectionDeleting"/>
  /// </item>
  /// <item>
  /// <see cref="SelectionDeleted"/>
  /// </item>
  /// <item>
  /// <see cref="SelectionMoved"/>
  /// </item>
  /// <item>
  /// <see cref="SelectionGrouped"/>
  /// </item>
  /// <item>
  /// <see cref="SelectionUngrouped"/>
  /// </item>
  /// <item>
  /// <see cref="TextEdited"/>
  /// </item>
  /// <item>
  /// <see cref="TemplateApplied"/>
  /// </item>
  /// <item>
  /// <see cref="TemplatesChanged"/>
  /// </item>
  /// </list>
  /// </para>
  /// <para>
  /// The <see cref="LayoutManager"/> is responsible for positioning all of the <see cref="Node"/>s
  /// and routing all of the <see cref="Link"/>s.
  /// The overall positioning of nodes is done by the value of the <see cref="Layout"/> property.
  /// Some <see cref="Group"/>s can have their own <see cref="Group.Layout"/>s.
  /// </para>
  /// </remarks>
  [ContentProperty("InitialParts")]
  [TemplatePart(Name="Panel", Type=typeof(DiagramPanel))]
  public class Diagram : Control {
    /// <summary>
    /// Create an empty <see cref="Diagram"/> with the standard values for the
    /// <see cref="Model"/>, <see cref="PartManager"/>,
    /// <see cref="LayoutManager"/>, <see cref="Layout"/>,
    /// <see cref="CommandHandler"/>, <see cref="MouseDownTools"/>,
    /// <see cref="MouseMoveTools"/>, <see cref="MouseUpTools"/>,
    /// and the various defined tool properties including <see cref="CurrentTool"/>.
    /// </summary>
    public Diagram() {
      this.Initializing = true;

      this.DefaultStyleKey = typeof(Diagram);


      this.Loaded += (s, e) => { OnLoaded(); };
      this.Unloaded += (s, e) => { OnUnloaded(); };

      this.InitialParts = new List<Part>();

      this.MouseDownTools = new List<IDiagramTool>();
      this.MouseMoveTools = new List<IDiagramTool>();
      this.MouseUpTools = new List<IDiagramTool>();

      ObservableCollection<Part> sel = new ObservableCollection<Part>();
      sel.CollectionChanged += SelectedParts_CollectionChanged;

      SetValue(SelectedPartsProperty, sel);




      // set these default values here so that they will exist after InitializeComponent in a Window or UserControl constructor
      this.PartManager = new PartManager();
      this.Model = new UniversalGraphLinksModel();
      this.InitialModel = true;
      this.LayoutManager = new LayoutManager();
      this.PrintManager = new PrintManager();
      this.CommandHandler = new CommandHandler();

      this.DefaultTool = new ToolManager();
      // mouse-down tools:
      this.RelinkingTool = new RelinkingTool();
      this.LinkReshapingTool = new LinkReshapingTool();
      this.ResizingTool = new ResizingTool();
      this.RotatingTool = new RotatingTool();
      // mouse-move tools:
      this.LinkingTool = new LinkingTool();
      this.DraggingTool = new DraggingTool();
      this.DragSelectingTool = new DragSelectingTool();
      this.PanningTool = new PanningTool();
      // but not DragZoomingTool
      // mouse-up tools:
      this.TextEditingTool = new TextEditingTool();
      this.ClickCreatingTool = new ClickCreatingTool();
      this.ClickSelectingTool = new ClickSelectingTool();

      this.Layout = new DiagramLayout();

      this.Initializing = false;
    }

    static Diagram() {




      //?? FocusVisualStyle

      // Dependency properties:

      ModelProperty = DependencyProperty.Register("Model", typeof(IDiagramModel), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnModelChanged));
      PartsModelProperty = DependencyProperty.Register("PartsModel", typeof(PartsModel), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnPartsModelChanged));
      // PartsModelIsDiagramModel is experimental for now...
      //PartsModelIsDiagramModelProperty = DependencyProperty.Register("PartsModelIsDiagramModel", typeof(bool), typeof(Diagram),
      //  new FrameworkPropertyMetadata(false, OnPartsModelIsDiagramModelChanged));

      NodeTemplateDictionaryProperty = DependencyProperty.Register("NodeTemplateDictionary", typeof(DataTemplateDictionary), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnNodeTemplateDictionaryChanged));
      NodeTemplateProperty = DependencyProperty.Register("NodeTemplate", typeof(DataTemplate), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnNodeTemplateChanged));
      GroupTemplateDictionaryProperty = DependencyProperty.Register("GroupTemplateDictionary", typeof(DataTemplateDictionary), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnGroupTemplateDictionaryChanged));
      GroupTemplateProperty = DependencyProperty.Register("GroupTemplate", typeof(DataTemplate), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnGroupTemplateChanged));

      // these two properties actually come from the PartManager

      NodesProperty = DependencyProperty.Register("Nodes", typeof(IEnumerable<Node>), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnNodesChanged));
      LinksProperty = DependencyProperty.Register("Links", typeof(IEnumerable<Link>), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnLinksChanged));






      NodesSourceProperty = DependencyProperty.Register("NodesSource", typeof(System.Collections.IEnumerable), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnNodesSourceChanged));
      LinksSourceProperty = DependencyProperty.Register("LinksSource", typeof(System.Collections.IEnumerable), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnLinksSourceChanged));

      LinkTemplateDictionaryProperty = DependencyProperty.Register("LinkTemplateDictionary", typeof(DataTemplateDictionary), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnLinkTemplateDictionaryChanged));
      LinkTemplateProperty = DependencyProperty.Register("LinkTemplate", typeof(DataTemplate), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnLinkTemplateChanged));

      PartManagerProperty = DependencyProperty.Register("PartManager", typeof(PartManager), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnPartManagerChanged));
      UnloadingClearsPartManagerProperty = DependencyProperty.Register("UnloadingClearsPartManager", typeof(bool), typeof(Diagram),
        new FrameworkPropertyMetadata(true));
      // this is still experimental:
      FilterProperty = DependencyProperty.Register("Filter", typeof(PartManagerFilter), typeof(Diagram),
        new FrameworkPropertyMetadata(PartManagerFilter.None));

      PrintManagerProperty = DependencyProperty.Register("PrintManager", typeof(PrintManager), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnPrintManagerChanged));

      LayoutManagerProperty = DependencyProperty.Register("LayoutManager", typeof(LayoutManager), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnLayoutManagerChanged));
      LayoutProperty = DependencyProperty.Register("Layout", typeof(IDiagramLayout), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnLayoutChanged));

      StretchProperty = DependencyProperty.Register("Stretch", typeof(StretchPolicy), typeof(Diagram),
        new FrameworkPropertyMetadata(StretchPolicy.Unstretched, OnStretchChanged));

      InitialStretchProperty = DependencyProperty.Register("InitialStretch", typeof(StretchPolicy), typeof(Diagram),
        new FrameworkPropertyMetadata(StretchPolicy.Unstretched));
      InitialScaleProperty = DependencyProperty.Register("InitialScale", typeof(double), typeof(Diagram),
        new FrameworkPropertyMetadata(Double.NaN));
      InitialPositionProperty = DependencyProperty.Register("InitialPosition", typeof(Point), typeof(Diagram),
        new FrameworkPropertyMetadata(new Point(Double.NaN, Double.NaN)));
      InitialPanelSpotProperty = DependencyProperty.Register("InitialPanelSpot", typeof(Spot), typeof(Diagram),
        new FrameworkPropertyMetadata(Spot.TopLeft));
      InitialDiagramBoundsSpotProperty = DependencyProperty.Register("InitialDiagramBoundsSpot", typeof(Spot), typeof(Diagram),
        new FrameworkPropertyMetadata(Spot.TopLeft));
      InitialCenteredNodeDataProperty = DependencyProperty.Register("InitialCenteredNodeData", typeof(Object), typeof(Diagram),
        new FrameworkPropertyMetadata(null));

      CenteredNodeDataProperty = DependencyProperty.Register("CenteredNodeData", typeof(Object), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnCenteredNodeDataChanged));

      //?? use TextSearch attached properties
      //IsTextSearchEnabledProperty = DependencyProperty.Register("IsTextSearchEnabled", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

      //SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Selector));
      //UnselectedEvent = EventManager.RegisterRoutedEvent("Unselected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Selector));


      SelectedPartsProperty = DependencyProperty.Register("SelectedParts", typeof(ObservableCollection<Part>), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnSelectedPartsChanged));




      MaximumSelectionCountProperty = DependencyProperty.Register("MaximumSelectionCount", typeof(int), typeof(Diagram),
        new FrameworkPropertyMetadata(int.MaxValue, OnMaximumSelectionCountChanged));
      SelectedPartProperty = DependencyProperty.Register("SelectedPart", typeof(Part), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnSelectedPartChanged));
      SelectedNodeProperty = DependencyProperty.Register("SelectedNode", typeof(Node), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnSelectedNodeChanged));
      SelectedGroupProperty = DependencyProperty.Register("SelectedGroup", typeof(Group), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnSelectedGroupChanged));
      SelectedLinkProperty = DependencyProperty.Register("SelectedLink", typeof(Link), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnSelectedLinkChanged));

      //Control.IsTabStopProperty.OverrideMetadata(typeof(ListBox), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
      //KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(ListBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
      //KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ListBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));

      // user interaction properties:
      //IsEnabled, from Control
      //Focusable, from Control; set to true in constructor

      IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(false, OnChangedPermissionProperty));

      // the use of some of the following properties should also depend on IDiagramModel.Modifiable
      AllowClipboardProperty = DependencyProperty.Register("AllowClipboard", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowCopyProperty = DependencyProperty.Register("AllowCopy", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowDeleteProperty = DependencyProperty.Register("AllowDelete", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowDragOutProperty = DependencyProperty.Register("AllowDragOut", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(false, OnChangedPermissionProperty));



      AllowEditProperty = DependencyProperty.Register("AllowEdit", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowGroupProperty = DependencyProperty.Register("AllowGroup", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      //AllowRegroupProperty = DependencyProperty.Register("AllowRegroup", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedProperty));
      AllowUngroupProperty = DependencyProperty.Register("AllowUngroup", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowInsertProperty = DependencyProperty.Register("AllowInsert", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowLinkProperty = DependencyProperty.Register("AllowLink", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowRelinkProperty = DependencyProperty.Register("AllowRelink", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowMoveProperty = DependencyProperty.Register("AllowMove", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowPrintProperty = DependencyProperty.Register("AllowPrint", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowReshapeProperty = DependencyProperty.Register("AllowReshape", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowResizeProperty = DependencyProperty.Register("AllowResize", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowRotateProperty = DependencyProperty.Register("AllowRotate", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowScrollProperty = DependencyProperty.Register("AllowScroll", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowSelectProperty = DependencyProperty.Register("AllowSelect", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowUndoProperty = DependencyProperty.Register("AllowUndo", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));
      AllowZoomProperty = DependencyProperty.Register("AllowZoom", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(true, OnChangedPermissionProperty));

      //?? HandlesScrolling, from Control
      //?? CommandBindings, from Control
      //?? InputBindings, from Control
      //?? InputScope, from Control
      //?? StylusPlugIns, from Control

      CurrentToolProperty = DependencyProperty.Register("CurrentTool", typeof(IDiagramTool), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnCurrentToolChanged));
      DefaultToolProperty = DependencyProperty.Register("DefaultTool", typeof(IDiagramTool), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnDefaultToolChanged));

      RelinkingToolProperty = DependencyProperty.Register("RelinkingTool", typeof(RelinkingTool), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnMouseDownToolChanged<RelinkingTool>));
      LinkReshapingToolProperty = DependencyProperty.Register("LinkReshapingTool", typeof(LinkReshapingTool), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnMouseDownToolChanged<LinkReshapingTool>));
      ResizingToolProperty = DependencyProperty.Register("ResizingTool", typeof(ResizingTool), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnMouseDownToolChanged<ResizingTool>));
      RotatingToolProperty = DependencyProperty.Register("RotatingTool", typeof(RotatingTool), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnMouseDownToolChanged<RotatingTool>));

      LinkingToolProperty = DependencyProperty.Register("LinkingTool", typeof(LinkingTool), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnMouseMoveToolChanged<LinkingTool>));
      DraggingToolProperty = DependencyProperty.Register("DraggingTool", typeof(DraggingTool), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnMouseMoveToolChanged<DraggingTool>));
      DragSelectingToolProperty = DependencyProperty.Register("DragSelectingTool", typeof(DragSelectingTool), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnMouseMoveToolChanged<DragSelectingTool>));
      PanningToolProperty = DependencyProperty.Register("PanningTool", typeof(PanningTool), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnMouseMoveToolChanged<PanningTool>));
      DragZoomingToolProperty = DependencyProperty.Register("DragZoomingTool", typeof(DragZoomingTool), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnMouseMoveToolChanged<DragZoomingTool>));

      TextEditingToolProperty = DependencyProperty.Register("TextEditingTool", typeof(TextEditingTool), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnMouseUpToolChanged<TextEditingTool>));
      ClickCreatingToolProperty = DependencyProperty.Register("ClickCreatingTool", typeof(ClickCreatingTool), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnMouseUpToolChanged<ClickCreatingTool>));
      ClickSelectingToolProperty = DependencyProperty.Register("ClickSelectingTool", typeof(ClickSelectingTool), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnMouseUpToolChanged<ClickSelectingTool>));

      //?? NodeReshapingTool
      //?? DragCreatingTool

      FirstMousePointInModelProperty = DependencyProperty.Register("FirstMousePointInModel", typeof(Point), typeof(Diagram), new FrameworkPropertyMetadata(new Point()));
      LastMousePointInModelProperty = DependencyProperty.Register("LastMousePointInModel", typeof(Point), typeof(Diagram), new FrameworkPropertyMetadata(new Point()));
      LastMouseEventArgsProperty = DependencyProperty.Register("LastMouseEventArgs", typeof(MouseEventArgs), typeof(Diagram), new FrameworkPropertyMetadata(null));

      // DraggingTool properties
      GridSnapEnabledProperty = DependencyProperty.Register("GridSnapEnabled", typeof(bool), typeof(Diagram), new FrameworkPropertyMetadata(false));
      GridSnapCellSizeProperty = DependencyProperty.Register("GridSnapCellSize", typeof(Size), typeof(Diagram),
        new FrameworkPropertyMetadata(new Size(10, 10), OnGridSnapCellSizeChanged));
      GridSnapCellSpotProperty = DependencyProperty.Register("GridSnapCellSpot", typeof(Spot), typeof(Diagram),
        new FrameworkPropertyMetadata(Spot.TopLeft, OnGridSnapCellSpotChanged));
      GridSnapOriginProperty = DependencyProperty.Register("GridSnapOrigin", typeof(Point), typeof(Diagram),
        new FrameworkPropertyMetadata(new Point(0, 0), OnGridSnapOriginChanged));

      GridVisibleProperty = DependencyProperty.Register("GridVisible", typeof(bool), typeof(Diagram),
        new FrameworkPropertyMetadata(false, OnGridVisibleChanged));
      GridPatternProperty = DependencyProperty.Register("GridPattern", typeof(GridPattern), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnGridPatternChanged));
      GridPatternTemplateProperty = DependencyProperty.Register("GridPatternTemplate", typeof(DataTemplate), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnGridPatternTemplateChanged));

      TreePathProperty = DependencyProperty.Register("TreePath", typeof(TreePath), typeof(Diagram),
        new FrameworkPropertyMetadata(TreePath.Destination));

      CommandHandlerProperty = DependencyProperty.Register("CommandHandler", typeof(CommandHandler), typeof(Diagram),
        new FrameworkPropertyMetadata(null, OnCommandHandlerChanged));

      ContextMenuEnabledProperty = DependencyProperty.Register("ContextMenuEnabled", typeof(bool), typeof(Diagram),
        new FrameworkPropertyMetadata(false));


























    }

    /// <summary>
    /// For debugging convenience.
    /// </summary>
    /// <returns></returns>
    public override String ToString() {
      if (this.Name != null) return "Diagram:" + this.Name;
      return base.ToString();
    }

    private static void OnChangedPermissionProperty(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = d as Diagram;
      if (diagram != null) diagram.UpdateCommands();
    }

    internal static void OnChangedInvalidateMeasure(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      ((UIElement)d).InvalidateMeasure();
    }



    /// <summary>
    /// Throw an exception if the current thread does not have access to this <c>DependencyObject</c>.
    /// </summary>
    protected void VerifyAccess() {
      if (!CheckAccess()) Diagram.Error("No access to thread in Diagram " + DiagramName(this));
    }

    // To support data-binding of properties of associated classes such as Layout or ...Tool,
    // in Silverlight we have to add them to the visual tree somehow.
    // For now we add them to a Canvas child of the DiagramPanel, and make sure they are not visible.
    internal void AddLogical(Object x) {
      FrameworkElement elt = x as FrameworkElement;
      if (elt == null) return;
      DiagramPanel panel = this.Panel;
      if (panel == null) return;
      LogicalChildren canvas = panel.Children.LastOrDefault() as LogicalChildren;
      if (canvas == null) {
        canvas = new LogicalChildren();
        canvas.Visibility = Visibility.Collapsed;
        panel.Children.Add(canvas);
      }
      elt.Visibility = Visibility.Collapsed;
      canvas.Children.Add(elt);
    }

    internal void RemoveLogical(Object x) {
      FrameworkElement elt = x as FrameworkElement;
      if (elt == null) return;
      DiagramPanel panel = this.Panel;
      if (panel == null) return;
      LogicalChildren canvas = panel.Children.LastOrDefault() as LogicalChildren;
      if (canvas != null) {
        canvas.Children.Remove(elt);
      }
    }

    private sealed class LogicalChildren : Canvas {}

    internal static void InvokeLater(DependencyObject d, Action a) {
      d.Dispatcher.BeginInvoke(a);
    }












    /// <summary>
    /// This static method looks for a resource of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">the type of the desired resource</typeparam>
    /// <param name="start">a <c>FrameworkElement</c></param>
    /// <param name="name">the name of the desired resource</param>
    /// <returns>
    /// the first object of type <typeparamref name="T"/> found in searching the
    /// <c>ResourceDictionary</c> of the <paramref name="start"/> element or in its parent elements,
    /// or null if no resource by that name is found, or if it is found but is not of the desired type.
    /// </returns>
    public static T FindResource<T>(FrameworkElement start, String name) {
      FrameworkElement fe = start;
      while (fe != null) {
        T result = LookupResource<T>(fe.Resources, name);
        if (result != null) return result;

        fe = fe.Parent as FrameworkElement;
      }
      if (Application.Current != null) {
        T result = LookupResource<T>(Application.Current.Resources, name);
        return result;
      }
      return default(T);
    }

    private static T LookupResource<T>(ResourceDictionary rd, String name) {
      if (rd == null) return default(T);

      Object item = rd[name];
      if (item is T) return (T)item;

      var merged = rd.MergedDictionaries;
      if (merged != null) {
        foreach (ResourceDictionary md in merged.Reverse()) {
          T result = LookupResource<T>(md, name);
          if (result != null) return result;
        }
      }
      return default(T);
    }

    /// <summary>
    /// This static method finds a default resource of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">the type of the desired resource</typeparam>
    /// <param name="name"></param>
    /// <returns>
    /// null if no resource can be found with the name <paramref name="name"/>
    /// or if it does exist but is not of type <typeparamref name="T"/>
    /// </returns>
    public static T FindDefault<T>(String name) {
      if (_genericResourceDictionary == null) {
        System.Reflection.Assembly assembly = typeof(Diagram).Assembly;
        String assemblyname = assembly.FullName;
        String basename = assemblyname.Substring(0, assemblyname.IndexOf(',')) + ".g";
        System.Resources.ResourceManager manager = new System.Resources.ResourceManager(basename, assembly);
        using (System.IO.Stream stream = manager.GetStream("themes/generic.xaml", System.Globalization.CultureInfo.CurrentCulture)) {
          using (System.IO.StreamReader reader = new System.IO.StreamReader(stream)) {
            String xamlstring = reader.ReadToEnd();
            _genericResourceDictionary = System.Windows.Markup.XamlReader.Load(xamlstring) as ResourceDictionary;
          }
        }
      }
      if (_genericResourceDictionary != null) {
        Object item = _genericResourceDictionary[name];
        if (item is T) return (T)item;
      }
      return default(T);
    }
    private static ResourceDictionary _genericResourceDictionary;




















































    /// <summary>
    /// Perform initialization.
    /// </summary>
    /// <remarks>
    /// This finds the <see cref="DiagramPanel"/> template child that becomes the value of <see cref="Panel"/>.
    /// It calls <see cref="Northwoods.GoXam.DiagramPanel.InitializeLayers"/>
    /// to create the various standard layers,
    /// it raises the <see cref="TemplateApplied"/> event so that you
    /// can initialize the <see cref="Panel"/>'s event handlers or establish data-bindings on its properties,
    /// it calls <see cref="Northwoods.GoXam.PartManager.RebuildNodeElements"/>
    /// to create <see cref="Node"/>s and <see cref="Link"/>s from the model's data, and
    /// it calls <see cref="Northwoods.GoXam.LayoutManager.LayoutDiagram(LayoutInitial, bool)"/>
    /// to eventually position the nodes if needed.
    /// </remarks>
    public override void OnApplyTemplate() {
      base.OnApplyTemplate();

      ApplyTemplates(this);  // recursively expand templates

      // search for and remember DiagramPanel inside expanded template
      this.Panel = GetTemplateChild("Panel") as DiagramPanel;
      if (this.Panel == null) {
        this.Panel = FindDescendant<DiagramPanel>(this);
      }
      // if found, initialize the diagram
      DiagramPanel panel = this.Panel;
      if (panel != null) {
        panel.Diagram = this;  // in Silverlight, template expansion's Parent is still null, so can't go up visual tree to find this Diagram

        bool oldinit = this.Initializing;
        this.Initializing = true;
        // create the standard layers for nodes and for links, both regular and temporary
        panel.InitializeLayers1();
        // handle any InitialParts; creates PartsModel if needed
        InitializePartsModel();

        AddLogical(this.PartManager);
        AddLogical(this.LayoutManager);
        AddLogical(this.Layout);
        AddLogical(this.PrintManager);
        AddLogical(this.DefaultTool);
        foreach (IDiagramTool t in this.MouseDownTools) AddLogical(t);
        foreach (IDiagramTool t in this.MouseMoveTools) AddLogical(t);
        foreach (IDiagramTool t in this.MouseUpTools) AddLogical(t);
        //AddLogical(this.GridPattern);  // not needed, already in visual tree
        AddLogical(this.CommandHandler);

        this.Initializing = oldinit;

        // now that the Diagram.Panel exists, raise the TemplateApplied event
        DiagramEventArgs e = new DiagramEventArgs();
        e.RoutedEvent = Diagram.TemplateAppliedEvent;
        RaiseEvent(e);

        // build Nodes and Links (calls RebuildLinkElements), request layout and updated diagram bounds
        Rebuild();

        // avoid having the initial model be IsModified, just due to BAML setting Diagram.Model to a new Model with various properties
        IDiagramModel model = this.Model;
        if (model != null) model.IsModified = false;
      } else {
        Diagram.Trace("WARNING: the Diagram has no DiagramPanel in its ControlTemplate; check the ControlTemplate for the Diagram " + DiagramName(this));
      }
    }


    // recursively make sure all control templates have been applied in this template expansion,
    // until we have gotten to the DiagramPanel
    private void ApplyTemplates(FrameworkElement fe) {
      if (fe == null) return;
      if (fe != this) {

        Control ctrl = fe as Control;
        if (ctrl != null)
          ctrl.ApplyTemplate();
        else
          fe.OnApplyTemplate();



      }
      if (fe is DiagramPanel) return;
      int num = VisualTreeHelper.GetChildrenCount(fe);
      for (int i = 0; i < num; i++) {
        ApplyTemplates(VisualTreeHelper.GetChild(fe, i) as FrameworkElement);
      }
    }


    /// <summary>
    /// Identifies the <see cref="UnloadingClearsPartManager"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UnloadingClearsPartManagerProperty;
    /// <summary>
    /// Gets or sets whether when this Diagram is Unloaded, it should clear out all
    /// of the <see cref="PartManager"/>'s <see cref="Node"/>s and <see cref="Link"/>s.
    /// </summary>
    /// <value>
    /// The initial value is true.
    /// </value>
    /// <remarks>
    /// <para>
    /// If you expect to temporarily remove this diagram from the visual tree
    /// and re-insert it, you will want to set this property to false.
    /// The default value of true accommodates the usage pattern where removing
    /// the diagram from the visual tree is meant to discard the diagram permanently.
    /// </para>
    /// <para>
    /// Silverlight 3 has no Unloaded event, so the <see cref="PartManager"/> is never cleared.
    /// </para>
    /// <para>
    /// If the diagram is inside a <c>TabControl</c>,
    /// this treats the value of <c>UnloadingClearsPartManager</c> as if it were false.
    /// </para>
    /// </remarks>
    public bool UnloadingClearsPartManager {
      get { return (bool)GetValue(UnloadingClearsPartManagerProperty); }
      set { SetValue(UnloadingClearsPartManagerProperty, value); }
    }

    private void OnLoaded() {



      Control tabc = null;
      DependencyObject p = VisualTreeHelper.GetParent(this);
      while (p != null) {
        Control c = p as Control;
        if (c != null && c.GetType().Name.Contains("TabControl")) {
          tabc = c;
          break;
        }
        p = VisualTreeHelper.GetParent(p);
      }
      this.InsideTabControl = (tabc != null);

      if (!this.WasUnloaded) return;
      if (this.InsideTabControl) return;
      this.WasUnloaded = false;
      //Diagram.Debug("reloading " + DiagramName(this));
      if (this.Model != null) this.Model.Changed += DiagramModel_Changed;
      PartManager partmgr = this.PartManager;
      if (partmgr != null) partmgr.RebuildNodeElements();
      RelayoutDiagram();
      UpdateCommands();
    }

    private bool WasUnloaded { get; set; }
    private bool InsideTabControl { get; set; }

    private void OnUnloaded() {
      if (this.WasUnloaded) return;
      if (this.InsideTabControl) return;
      if (!this.UnloadingClearsPartManager) return;
      this.WasUnloaded = true;
      //Diagram.Debug("unloading " + DiagramName(this));
      ClearAll();
      UpdateCommands();
      if (this.Model != null) this.Model.Changed -= DiagramModel_Changed;
    }

    private void ClearAll() {
      this.LastMouseEventArgs = null;
      DiagramPanel panel = this.Panel;
      if (panel != null) panel.CancelUpdate();
      LayoutManager laymgr = this.LayoutManager;
      if (laymgr != null) laymgr.CancelAnimatedLayout();
      PartManager partmgr = this.PartManager;
      if (partmgr != null) partmgr.ClearAll();
    }
    
    /// <summary>
    /// Gets the <see cref="DiagramPanel"/> that implements the basic functionality of this diagram.
    /// </summary>
    /// <remarks>
    /// The <see cref="DiagramPanel"/> should be in the <c>ControlTemplate</c> for this diagram.
    /// </remarks>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DiagramPanel Panel { get; private set; }


    /// <summary>
    /// Gets the list of <see cref="Part"/>s that are automatically included in the diagram
    /// without coming from the <see cref="Model"/>.
    /// </summary>
    /// <remarks>
    /// This property is declared as the ContentProperty for this control,
    /// so that you can define <see cref="Node"/>s and <see cref="Link"/>s
    /// that are nested inside the <see cref="Diagram"/> element in XAML.
    /// </remarks>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public System.Collections.IList InitialParts { get; private set; }

    private void InitializePartsModel() {
      PartsModel pmodel = this.PartsModel;
      if (pmodel == null) {
        pmodel = new PartsModel();
        this.PartsModel = pmodel;
      }
      if (pmodel != null) {
        foreach (Part part in this.InitialParts) {
          Node node = part as Node;
          if (node != null) {
            pmodel.AddNode(node);
          } else {
            Link link = part as Link;
            if (link != null) {
              pmodel.AddLink(link);
            }
          }
        }
      }
    }


    // Model

    /// <summary>
    /// Identifies the <see cref="Model"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ModelProperty;
    /// <summary>
    /// Gets or sets the <see cref="IDiagramModel"/> that organizes the data
    /// to which this diagram is bound.
    /// </summary>
    /// <value>
    /// The initial value is an instance of <see cref="UniversalGraphLinksModel"/>.
    /// </value>
    /// <remarks>
    /// Replacing this value causes all of the bound <see cref="Node"/>s and
    /// <see cref="Link"/>s to be deleted and re-created from the new model data.
    /// The value of <see cref="NodesSource"/> (and possibly <see cref="LinksSource"/>)
    /// will be updated too.
    /// </remarks>
    [Category("Model")]
    public IDiagramModel Model {
      get { return (IDiagramModel)GetValue(ModelProperty); }
      set { SetValue(ModelProperty, value); }
    }
    private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      IDiagramModel oldmodel = (IDiagramModel)e.OldValue;
      if (oldmodel != null) {
        oldmodel.Changed -= diagram.DiagramModel_Changed;
        PartsModel pmodel = oldmodel as PartsModel;
        if (pmodel != null) pmodel.Diagram = null;
        diagram.ClearAll();  // temporarily no model!
      }
      IDiagramModel newmodel = (IDiagramModel)e.NewValue;
      if (newmodel != null) {
        //if (diagram.PartsModelIsDiagramModel && newmodel != diagram.PartsModel) {
        //  Diagram.Error("Cannot set Diagram.Model to a model other than the diagram's PartsModel when PartsModelIsDiagramModel is true, for Diagram " + DiagramName(diagram));
        //  diagram.Model = diagram.PartsModel;
        //  return;
        //}
        PartsModel pmodel = newmodel as PartsModel;
        if (pmodel != null) {
          if (pmodel != diagram.PartsModel && diagram.PartsModel != null) {
            Diagram.Error("new Diagram.Model must not be a PartsModel unless it is the same as Diagram.PartsModel, for Diagram " + DiagramName(diagram));
            diagram.Model = diagram.PartsModel;
          }
          pmodel.Diagram = diagram;
        } else {
          // transfer non-null Diagram.NodesSource to the new model if it doesn't already have one,
          // and if the original model wasn't the default one
          if (diagram.InitialModel && diagram.NodesSource != null) {
            newmodel.NodesSource = diagram.NodesSource;
          }
          if (!diagram.Initializing && diagram.NodesSource != newmodel.NodesSource) {
            diagram.NodesSource = newmodel.NodesSource;
          }
          ILinksModel newlmodel = newmodel as ILinksModel;
          if (newlmodel != null) {
            // transfer non-null Diagram.LinksSource to the new model if it doesn't already have one,
            // and if the original model wasn't the default one
            if (diagram.InitialModel && diagram.LinksSource != null) {
              newlmodel.LinksSource = diagram.LinksSource;
            }
            if (!diagram.Initializing && diagram.LinksSource != newlmodel.LinksSource) {
              diagram.LinksSource = newlmodel.LinksSource;
            }
          } else {  // if new model isn't ILinksModel, Diagram.LinksSource should be null
            diagram.LinksSource = null;
          }
        }
        //diagram.PartsModelIsDiagramModel = (diagram.Model == diagram.PartsModel);
        if (!diagram.Initializing) diagram.Rebuild();
        newmodel.Changed += diagram.DiagramModel_Changed;
      }
      diagram.InitialModel = false;

      // raise the ModelReplaced event
      if (diagram.ModelReplaced != null) diagram.ModelReplaced(diagram, new PropertyChangedEventArgs("Model"));

      diagram.UpdateCommands();
    }

    private bool InitialModel { get; set; }
    internal bool Initializing { get; set; }  // set by ctor, OnApplyTemplate, PartManager setter

    private void Rebuild() {  // called by OnApplyTemplate & On[Parts]ModelChanged (i.e. model replacement)
      // do something to cause everything to be rebuilt
      PartManager partmgr = this.PartManager;
      if (partmgr != null) {
        DiagramPanel panel = this.Panel;
        if (panel != null) {
          panel.IsVirtualizing = false;
          panel.InitialLayoutCompleted = false;
        }
        //DateTime before = DateTime.Now;
        partmgr.RebuildNodeElements();  // also calls RebuildLinkElements
        //Diagram.Debug("rebuild time: " + (DateTime.Now-before).TotalMilliseconds.ToString() + "  nodes: " + this.PartManager.NodesCount.ToString());
        RelayoutDiagram();
      }
    }

    internal bool IsHidingShowing { get; set; }
    internal bool IsCollapsingExpanding { get; set; }

    // used by Route for partly or completely unconnected links
    internal Node RoutingFromNode { get; set; }
    internal FrameworkElement RoutingFromPort { get; set; }
    internal Node RoutingToNode { get; set; }
    internal FrameworkElement RoutingToPort { get; set; }


    /// <summary>
    /// This event is raised when the <see cref="Model"/> property changes value.
    /// </summary>
    [Category("Diagram")]
    public event PropertyChangedEventHandler ModelReplaced;


    /// <summary>
    /// Identifies the <see cref="PartsModel"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PartsModelProperty;
    /// <summary>
    /// Gets or sets the special model used to hold <see cref="Part"/>s
    /// that are not bound to data.
    /// </summary>
    /// <value>
    /// The initial value is an instance of <see cref="PartsModel"/>.
    /// It is first populated when the diagram is initialized by the <see cref="Part"/>s
    /// held by the <see cref="InitialParts"/> collection.
    /// There must always be a value for this property.
    /// </value>
    /// <remarks>
    /// This special model is also used to hold temporary nodes and links
    /// used by tools such as <see cref="LinkingTool"/>.
    /// The parts managed by this model can be added to any layer in the diagram.
    /// </remarks>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PartsModel PartsModel {
      get { return (PartsModel)GetValue(PartsModelProperty); }
      set { SetValue(PartsModelProperty, value); }
    }
    private static void OnPartsModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      PartsModel oldmodel = (PartsModel)e.OldValue;
      if (oldmodel != null) {
        oldmodel.Changed -= diagram.DiagramModel_Changed;
      }
      PartsModel newmodel = (PartsModel)e.NewValue;
      if (newmodel != null) {
        if (newmodel != diagram.Model && diagram.Model is PartsModel && diagram.Model != null) {
          Diagram.Error("new Diagram.PartsModel must be the same model as Diagram.Model if the latter is a PartsModel, for Diagram " + DiagramName(diagram));
          diagram.PartsModel = (PartsModel)diagram.Model;
        }
        newmodel.Diagram = diagram;
        newmodel.Changed += diagram.DiagramModel_Changed;
        if (!diagram.Initializing) diagram.Rebuild();
      } else {
        Diagram.Trace("WARNING: setting Diagram.PartsModel to null may produce inconsistent results");
        diagram.PartsModel = new PartsModel();
      }
    }


    ///// <summary>
    ///// Identifies the <see cref="PartsModelIsDiagramModel"/> dependency property.
    ///// </summary>
    //private /*?? public */ static readonly DependencyProperty PartsModelIsDiagramModelProperty;
    ///// <summary>
    ///// Gets or sets whether the diagram's <see cref="Model"/> is the same as its <see cref="PartsModel"/>.
    ///// </summary>
    ///// <value>
    ///// The default value is false, allowing the diagram to support all kinds of data models.
    ///// When the value is true the user can copy and paste unbound parts,
    ///// such as those defined in XAML as children of the <c>&lt;Diagram&gt;</c>,
    ///// or parts created programmatically without data binding.
    ///// </value>
    ///// <remarks>
    ///// Changing the value to true will replace the <see cref="Model"/> with the diagram's
    ///// current <see cref="PartsModel"/>.
    ///// Changing the value from true to false will replace the <see cref="Model"/>
    ///// with a new <see cref="Northwoods.GoXam.Model.UniversalGraphLinksModel"/>.
    ///// </remarks>
    //private /*?? public */ bool PartsModelIsDiagramModel {
    //  get { return (bool)GetValue(PartsModelIsDiagramModelProperty);  }
    //  set { SetValue(PartsModelIsDiagramModelProperty, value); }
    //}
    //private static void OnPartsModelIsDiagramModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
    //  Diagram diagram = (Diagram)d;
    //  if ((bool)e.NewValue) {
    //    if (diagram.Model != diagram.PartsModel) diagram.Model = diagram.PartsModel;
    //  } else {
    //    if (diagram.Model == diagram.PartsModel) diagram.Model = new UniversalGraphLinksModel();
    //  }
    //}


    /// <summary>
    /// Identifies the <see cref="PartManager"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PartManagerProperty;
    /// <summary>
    /// Gets or sets the <see cref="Northwoods.GoXam.PartManager"/> responsible for creating and updating
    /// <see cref="Part"/>s for all of the data managed by the <see cref="Model"/>.
    /// </summary>
    /// <value>
    /// The initial value is an instance of <see cref="Northwoods.GoXam.PartManager"/>.
    /// There must always be a non-null value for this property.
    /// PartManagers cannot be shared by Diagrams.
    /// </value>
    [Category("Model")]
    public PartManager PartManager {
      get { return (PartManager)GetValue(PartManagerProperty); }
      set { SetValue(PartManagerProperty, value); }
    }
    private static void OnPartManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      PartManager oldmgr = e.OldValue as PartManager;
      if (oldmgr != null) {
        diagram.RemoveLogical(oldmgr);
        oldmgr.Diagram = null;
      }
      PartManager newmgr = e.NewValue as PartManager;
      if (newmgr != null) {
        newmgr.Diagram = diagram;
        diagram.AddLogical(newmgr);
        bool oldinit = diagram.Initializing;
        diagram.Initializing = true;

        diagram.SetValue(NodesProperty, newmgr.Nodes);
        diagram.SetValue(LinksProperty, newmgr.Links);




        diagram.Initializing = oldinit;
      } else {
        Diagram.Trace("WARNING: setting Diagram.PartManager to null may produce inconsistent results");
        diagram.PartManager = new PartManager();
      }
    }

    private void DiagramModel_Changed(Object sender, ModelChangedEventArgs e) {
      PartManager partmgr = this.PartManager;
      if (partmgr == null) return;
      if (!CheckAccess()) {
        Diagram.InvokeLater(this, () => { partmgr.OnModelChanged(e); });
      } else {
        partmgr.OnModelChanged(e);
      }
    }


    /// <summary>
    /// Identifies the <see cref="InitialStretch"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InitialStretchProperty;
    /// <summary>
    /// Gets or sets the initial value for this diagram's <see cref="Panel"/>'s
    /// <see cref="Northwoods.GoXam.DiagramPanel.Stretch"/> property.
    /// </summary>
    /// <value>
    /// The default value is <see cref="StretchPolicy.Unstretched"/>.
    /// Setting this to a different value will cause the diagram to be scaled and positioned
    /// to fit in the available space.
    /// </value>
    [Category("Layout")]
    public StretchPolicy InitialStretch {
      get { return (StretchPolicy)GetValue(InitialStretchProperty); }
      set { SetValue(InitialStretchProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="InitialScale"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InitialScaleProperty;
    /// <summary>
    /// Gets or sets the initial value for this diagram's <see cref="Panel"/>'s
    /// <see cref="Northwoods.GoXam.DiagramPanel.Scale"/> property.
    /// </summary>
    /// <value>
    /// The default value is <c>Double.NaN</c>,
    /// which does not change the initial value of the panel's scale.
    /// Any new value should not be negative or zero.
    /// </value>
    /// <remarks>
    /// This property only has an effect on the initial diagram shown if the
    /// <see cref="InitialStretch"/> property has its default value of <see cref="StretchPolicy.Unstretched"/>.
    /// </remarks>
    [Category("Layout")]
    public double InitialScale {
      get { return (double)GetValue(InitialScaleProperty); }
      set { SetValue(InitialScaleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="InitialPosition"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InitialPositionProperty;
    /// <summary>
    /// Gets or sets the initial value for this diagram's <see cref="Panel"/>'s
    /// <see cref="Northwoods.GoXam.DiagramPanel.Position"/> property, in model coordinates.
    /// </summary>
    /// <value>
    /// The default value is <c>new Point(Double.NaN, Double.NaN)</c>,
    /// which does not change the initial value of this diagram's panel's position.
    /// </value>
    /// <remarks>
    /// This property only has an effect on the initial diagram shown if the
    /// <see cref="InitialStretch"/> property has its default value of <see cref="StretchPolicy.Unstretched"/>.
    /// </remarks>

    [TypeConverter(typeof(Route.PointConverter))]

    [Category("Layout")]
    public Point InitialPosition {
      get { return (Point)GetValue(InitialPositionProperty); }
      set { SetValue(InitialPositionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="InitialPanelSpot"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InitialPanelSpotProperty;
    /// <summary>
    /// Gets or sets the <see cref="Spot"/> point in the panel where
    /// the point specified by <see cref="InitialDiagramBoundsSpot"/> should be positioned.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Spot.TopLeft"/>.
    /// If the <see cref="Spot"/> is not a specific spot, this property
    /// and the <see cref="InitialDiagramBoundsSpot"/> property are ignored.
    /// </value>
    /// <remarks>
    /// <para>
    /// This spot specifies the initial point in the <see cref="Panel"/> at which the
    /// <see cref="InitialDiagramBoundsSpot"/> of the diagram's <see cref="Northwoods.GoXam.DiagramPanel.DiagramBounds"/>
    /// should be located.
    /// The default values of this property and of <see cref="InitialDiagramBoundsSpot"/>
    /// cause the top-left corner of the diagram contents to be positioned at the top-left corner of the panel.
    /// </para>
    /// <para>
    /// This property only has an effect on the initial diagram shown if the
    /// <see cref="InitialStretch"/> property has its default value of <see cref="StretchPolicy.Unstretched"/>
    /// and if the <see cref="InitialPosition"/> property has its default value of a <c>Point</c>
    /// whose X or Y values are <c>Double.NaN</c>,
    /// and if both spots are specific spots (i.e. <see cref="Spot.IsSpot"/> are true).
    /// </para>
    /// </remarks>
    [Category("Layout")]
    public Spot InitialPanelSpot {
      get { return (Spot)GetValue(InitialPanelSpotProperty); }
      set { SetValue(InitialPanelSpotProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="InitialDiagramBoundsSpot"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InitialDiagramBoundsSpotProperty;
    /// <summary>
    /// Gets or sets the <see cref="Spot"/> point in the diagram bounds that
    /// should be positioned at the point specified by <see cref="InitialPanelSpot"/>.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Spot.TopLeft"/>.
    /// If the <see cref="Spot"/> is not a specific spot, this property
    /// and the <see cref="InitialPanelSpot"/> property are ignored.
    /// </value>
    /// <remarks>
    /// <para>
    /// This spot specifies the initial point in the diagram's <see cref="Northwoods.GoXam.DiagramPanel.DiagramBounds"/>
    /// at which the <see cref="InitialPanelSpot"/> of the <see cref="Panel"/>
    /// should be located.
    /// The default values of this property and of <see cref="InitialPanelSpot"/>
    /// cause the top-left corner of the diagram contents to be positioned at the top-left corner of the panel.
    /// </para>
    /// <para>
    /// This property only has an effect on the initial diagram shown if the
    /// <see cref="InitialStretch"/> property has its default value of <see cref="StretchPolicy.Unstretched"/>,
    /// and if the <see cref="InitialPosition"/> property has its default value of a <c>Point</c>
    /// whose X or Y values are <c>Double.NaN</c>,
    /// and if both spots are specific spots (i.e. <see cref="Spot.IsSpot"/> are true).
    /// </para>
    /// </remarks>
    [Category("Layout")]
    public Spot InitialDiagramBoundsSpot {
      get { return (Spot)GetValue(InitialDiagramBoundsSpotProperty); }
      set { SetValue(InitialDiagramBoundsSpotProperty, value); }
    }


    // tree traversing helpers

    // may need to call ApplyTemplate beforehand, in case the Control's Template hasn't been expanded yet
    //internal static T FindChild<T>(DependencyObject v) where T : DependencyObject {
    //  if (v == null) return null;
    //  int numchildren = VisualTreeHelper.GetChildrenCount(v);
    //  for (int i = 0; i < numchildren; i++) {
    //    DependencyObject child = VisualTreeHelper.GetChild(v, i);
    //    T t = child as T;
    //    if (t != null) return t;
    //  }
    //  return null;
    //}

    // may need to call ApplyTemplate beforehand, in case the Control's Template hasn't been expanded yet
    internal static T FindDescendant<T>(DependencyObject v) where T : DependencyObject {
      if (v == null) return null;
      int numchildren = VisualTreeHelper.GetChildrenCount(v);
      for (int i = 0; i < numchildren; i++) {
        DependencyObject c = VisualTreeHelper.GetChild(v, i);
        T child = c as T;
        if (child != null) return child;
      }
      for (int i = 0; i < numchildren; i++) {
        DependencyObject c = VisualTreeHelper.GetChild(v, i);
        T child = FindDescendant<T>(c);
        if (child != null) return child;
      }
      return null;
    }

    //internal static T FindDescendantOrSelf<T>(DependencyObject v) where T : DependencyObject {
    //  T t = v as T;
    //  if (t != null) return t;
    //  return FindDescendant<T>(v);
    //}

    internal static T FindParent<T>(DependencyObject v) where T : DependencyObject {
      if (v == null) return null;
      return VisualTreeHelper.GetParent(v) as T;
    }

    internal static T FindAncestor<T>(DependencyObject v) where T : DependencyObject {
      if (v == null) return null;
      DependencyObject p = VisualTreeHelper.GetParent(v);
      if (p == null) return null;
      T t = p as T;
      if (t != null) return t;
      return FindAncestor<T>(p);
    }

    internal static T FindAncestorOrSelf<T>(DependencyObject v) where T : DependencyObject {
      T t = v as T;
      if (t != null) return t;
      return FindAncestor<T>(v);
    }









































































































































































































































    private static String DiagramName(Diagram d) {
      if (d == null) return "(null)";
      String n = d.Name;
      if (n == null) return "(no Name)";
      return "'" + n + "'";
    }

    internal static void Trace(String msg) { ModelHelper.Trace(msg); }
    internal static void Error(String msg) { ModelHelper.Error("ERROR: " + msg); }


    // nodes

    /// <summary>
    /// Identifies the read-only <see cref="Nodes"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NodesProperty;
    /// <summary>
    /// A read-only collection of the <see cref="Node"/>s in this diagram, including <see cref="Group"/>s.
    /// </summary>
    /// <remarks>
    /// This is a synonym for <see cref="Northwoods.GoXam.PartManager.Nodes"/>.
    /// </remarks>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IEnumerable<Node> Nodes {
      get { return (IEnumerable<Node>)GetValue(NodesProperty); }
    }

    private static void OnNodesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      if (!diagram.Initializing) Diagram.Error("Cannot set Diagram.Nodes property of Diagram " + DiagramName(diagram));
    }




    /// <summary>
    /// Identifies the <see cref="NodesSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NodesSourceProperty;
    /// <summary>
    /// Gets and sets the diagram's <see cref="Model"/>'s <see cref="IDiagramModel.NodesSource"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Setting this property will not automatically create a model for you, if <see cref="Model"/> is null.
    /// You will need to set that <see cref="Model"/> property explicitly, usually in code.
    /// When the value of this property changes, the new value is used as
    /// the new value for the <see cref="IDiagramModel.NodesSource"/> property.
    /// </para>
    /// <para>
    /// When the <see cref="Model"/> is replaced, or when the model's <see cref="IDiagramModel.NodesSource"/> changes,
    /// this dependency property will automatically be set to the new collection value.
    /// </para>
    /// <para>
    /// If you want to data-bind this property, you should use "Mode=TwoWay".
    /// </para>
    /// </remarks>
    public System.Collections.IEnumerable NodesSource {
      get { return (System.Collections.IEnumerable)GetValue(NodesSourceProperty); }
      set { SetValue(NodesSourceProperty, value); }
    }
    private static void OnNodesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      IDiagramModel model = diagram.Model;
      if (model != null && e.NewValue != null) {
        // pass the source on to the model
        model.NodesSource = (System.Collections.IEnumerable)e.NewValue;
      }
    }


    /// <summary>
    /// Identifies the <see cref="Filter"/> dependency property.
    /// </summary>
    private /*?? public */ static readonly DependencyProperty FilterProperty;
    /// <summary>
    /// Gets or sets whether <see cref="Northwoods.GoXam.PartManager.FilterNodeForData"/>
    /// causes node data not to get <see cref="Node"/>s created for them if they are the
    /// tree children of collapsed tree nodes or if they are the members of collapsed
    /// group nodes.
    /// </summary>
    /// <value>
    /// The default value is <see cref="PartManagerFilter.None"/> -- all nodes and links
    /// in the model get realized as <see cref="Node"/>s and <see cref="Link"/>s in the diagram.
    /// </value>
    [Category("Model")]
    internal /*?? public */ PartManagerFilter Filter {
      get { return (PartManagerFilter)GetValue(FilterProperty); }
      set { SetValue(FilterProperty, value); }
    }


    // node appearance, and binding to data

    /// <summary>
    /// Identifies the <see cref="NodeTemplateDictionary"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NodeTemplateDictionaryProperty;
    /// <summary>
    /// Gets or sets a <see cref="DataTemplateDictionary"/> that determines the appearance of <see cref="Node"/>s
    /// that are not <see cref="Group"/>s.
    /// </summary>
    /// <value>
    /// The default value is null, but the default style for <see cref="Diagram"/>
    /// will set this to a shared <see cref="DataTemplateDictionary"/> with reasonable 
    /// (but minimal) data templates for all predefined categories.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used by <see cref="Northwoods.GoXam.PartManager.AddNodeForData(Object, IDiagramModel)"/>.
    /// </para>
    /// <para>
    /// The dictionary is keyed by the node's <see cref="Part.Category"/>.
    /// The value of <see cref="NodeTemplate"/> takes precedence over any
    /// entry in this dictionary with the key an empty string, "", which is the default category.
    /// If <see cref="NodeTemplate"/> is null, you should be sure to have a data template
    /// in this dictionary for the default category, "".
    /// One way of doing that is by setting the <see cref="DataTemplateDictionary.Default"/> property.
    /// </para>
    /// </remarks>
    [Category("Appearance")]
    public DataTemplateDictionary NodeTemplateDictionary {
      get { return (DataTemplateDictionary)GetValue(NodeTemplateDictionaryProperty); }
      set { SetValue(NodeTemplateDictionaryProperty, value); }
    }
    private static void OnNodeTemplateDictionaryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      diagram.RaiseTemplatesChanged("NodeTemplateDictionary");
    }

    /// <summary>
    /// Identifies the <see cref="NodeTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NodeTemplateProperty;
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used to define the default appearance of regular <see cref="Node"/>s,
    /// ones that are neither groups nor link labels.
    /// </summary>
    /// <value>
    /// The default style assigns this property to use a generic node template.
    /// </value>
    /// <remarks>
    /// This is used by <see cref="Northwoods.GoXam.PartManager.AddNodeForData(Object, IDiagramModel)"/>.
    /// </remarks>
    [Category("Appearance")]
    public DataTemplate NodeTemplate {
      get { return (DataTemplate)GetValue(NodeTemplateProperty); }
      set { SetValue(NodeTemplateProperty, value); }
    }
    private static void OnNodeTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      diagram.RaiseTemplatesChanged("NodeTemplate");
    }


    // group appearance, and binding to data

    /// <summary>
    /// Identifies the <see cref="GroupTemplateDictionary"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GroupTemplateDictionaryProperty;
    /// <summary>
    /// Gets or sets a <see cref="DataTemplateDictionary"/> that determines the appearance of all <see cref="Group"/>s.
    /// </summary>
    /// <value>
    /// The default value is null, but the default style for <see cref="Diagram"/>
    /// will set this to a shared <see cref="DataTemplateDictionary"/> with reasonable 
    /// (but minimal) data templates for all predefined categories.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used by <see cref="Northwoods.GoXam.PartManager.AddNodeForData(Object, IDiagramModel)"/>.
    /// Many data templates expand into something containing a <see cref="GroupPanel"/>.
    /// </para>
    /// <para>
    /// The dictionary is keyed by the group's <see cref="Part.Category"/>.
    /// The value of <see cref="GroupTemplate"/> takes precedence over any
    /// entry in this dictionary with the key an empty string, "", which is the default category.
    /// If <see cref="GroupTemplate"/> is null, you should be sure to have a data template
    /// in this dictionary for the default category, "".
    /// One way of doing that is by setting the <see cref="DataTemplateDictionary.Default"/> property.
    /// </para>
    /// </remarks>
    [Category("Appearance")]
    public DataTemplateDictionary GroupTemplateDictionary {
      get { return (DataTemplateDictionary)GetValue(GroupTemplateDictionaryProperty); }
      set { SetValue(GroupTemplateDictionaryProperty, value); }
    }
    private static void OnGroupTemplateDictionaryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      diagram.RaiseTemplatesChanged("GroupTemplateDictionary");
    }

    /// <summary>
    /// Identifies the <see cref="GroupTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GroupTemplateProperty;
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used to define the default appearance of <see cref="Group"/>s.
    /// </summary>
    /// <value>
    /// The default style assigns this property to use a generic group node template.
    /// </value>
    /// <remarks>
    /// This is used by <see cref="Northwoods.GoXam.PartManager.AddNodeForData(Object, IDiagramModel)"/>.
    /// Typically the data template expands into a <see cref="GroupPanel"/> surrounded by a <c>Border</c>.
    /// </remarks>
    [Category("Appearance")]
    public DataTemplate GroupTemplate {
      get { return (DataTemplate)GetValue(GroupTemplateProperty); }
      set { SetValue(GroupTemplateProperty, value); }
    }
    private static void OnGroupTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      diagram.RaiseTemplatesChanged("GroupTemplate");
    }


    // links
    
    /// <summary>
    /// Identifies the read-only <see cref="Links"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LinksProperty;
    /// <summary>
    /// A read-only collection of the <see cref="Link"/>s in this diagram.
    /// </summary>
    /// <remarks>
    /// This is a synonym for the <see cref="Northwoods.GoXam.PartManager.Links"/> property.
    /// </remarks>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IEnumerable<Link> Links {
      get { return (IEnumerable<Link>)GetValue(LinksProperty); }
    }

    private static void OnLinksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      if (!diagram.Initializing) Diagram.Error("Cannot set Diagram.Links property of Diagram " + DiagramName(diagram));
    }




    /// <summary>
    /// Identifies the <see cref="LinksSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LinksSourceProperty;
    /// <summary>
    /// Gets and sets the diagram's <see cref="Model"/>'s <see cref="ILinksModel.LinksSource"/>,
    /// if the model is an <see cref="ILinksModel"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Setting this property will not automatically create a model for you, if <see cref="Model"/> is null.
    /// You will need to set that <see cref="Model"/> property explicitly, usually in code.
    /// When the value of this property changes, the new value is used as
    /// the new value for the <see cref="ILinksModel.LinksSource"/> property.
    /// </para>
    /// <para>
    /// When the <see cref="Model"/> is replaced, or when the model's <see cref="ILinksModel.LinksSource"/> changes,
    /// this dependency property will automatically be set to the new collection value.
    /// </para>
    /// <para>
    /// If you want to data-bind this property, you should use "Mode=TwoWay".
    /// </para>
    /// </remarks>
    public System.Collections.IEnumerable LinksSource {
      get { return (System.Collections.IEnumerable)GetValue(LinksSourceProperty); }
      set { SetValue(LinksSourceProperty, value); }
    }
    private static void OnLinksSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      IDiagramModel model = diagram.Model;
      if (model != null) {
        ILinksModel lmodel = model as ILinksModel;
        if (lmodel != null && e.NewValue != null) {
          // pass the source on to the model
          lmodel.LinksSource = (System.Collections.IEnumerable)e.NewValue;
        }
      }
    }

    // link appearance, and binding to data

    /// <summary>
    /// Identifies the <see cref="LinkTemplateDictionary"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LinkTemplateDictionaryProperty;
    /// <summary>
    /// Gets or sets a <see cref="DataTemplateDictionary"/> that determines the appearance of all <see cref="Link"/>s.
    /// </summary>
    /// <value>
    /// The default value is null, but the default style for <see cref="Diagram"/>
    /// will set this to a shared <see cref="DataTemplateDictionary"/> with reasonable 
    /// (but minimal) data templates for all predefined categories.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is used by <see cref="Northwoods.GoXam.PartManager.AddLinkForData(Object, IDiagramModel)"/>
    /// and <see cref="Northwoods.GoXam.PartManager.AddLinkForData(Object, Object, IDiagramModel)"/>.
    /// Typically most data templates expand into a <see cref="LinkPanel"/>.
    /// </para>
    /// <para>
    /// The dictionary is keyed by the link's <see cref="Part.Category"/>.
    /// The value of <see cref="LinkTemplate"/> takes precedence over any
    /// entry in this dictionary with the key an empty string, "", which is the default category.
    /// If <see cref="LinkTemplate"/> is null, you should be sure to have a data template
    /// in this dictionary for the default category, "".
    /// One way of doing that is by setting the <see cref="DataTemplateDictionary.Default"/> property.
    /// </para>
    /// </remarks>
    [Category("Appearance")]
    public DataTemplateDictionary LinkTemplateDictionary {
      get { return (DataTemplateDictionary)GetValue(LinkTemplateDictionaryProperty); }
      set { SetValue(LinkTemplateDictionaryProperty, value); }
    }
    private static void OnLinkTemplateDictionaryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      diagram.RaiseTemplatesChanged("LinkTemplateDictionary");
    }

    /// <summary>
    /// Identifies the <see cref="LinkTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LinkTemplateProperty;
    /// <summary>
    /// Gets or sets a <c>DataTemplate</c> that determines the default appearance of <see cref="Link"/>s.
    /// </summary>
    /// <value>
    /// The default style assigns this property to use a generic link template.
    /// </value>
    /// <remarks>
    /// This is used by <see cref="Northwoods.GoXam.PartManager.AddLinkForData(Object, IDiagramModel)"/>
    /// and <see cref="Northwoods.GoXam.PartManager.AddLinkForData(Object, Object, IDiagramModel)"/>.
    /// Typically the data template expands into a <see cref="LinkPanel"/>.
    /// </remarks>
    [Category("Appearance")]
    public DataTemplate LinkTemplate {
      get { return (DataTemplate)GetValue(LinkTemplateProperty); }
      set { SetValue(LinkTemplateProperty, value); }
    }
    private static void OnLinkTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      diagram.RaiseTemplatesChanged("LinkTemplate");
    }


    /// <summary>
    /// Raise the <see cref="TemplatesChanged"/> event.
    /// </summary>
    /// <param name="propertyname">the name of the modified dictionary property</param>
    /// <remarks>
    /// <para>
    /// You should call this after dynamically modifying the contents of one of the following dictionaries:
    /// <see cref="NodeTemplateDictionary"/>,
    /// <see cref="GroupTemplateDictionary"/>,
    /// or <see cref="LinkTemplateDictionary"/>.
    /// Pass the name of one of those three properties as the argument.
    /// </para>
    /// <para>
    /// This method is called automatically when you set any of the <see cref="Diagram"/>
    /// properties that are of type <c>DataTemplate</c> or <see cref="DataTemplateDictionary"/>.
    /// </para>
    /// </remarks>
    public void RaiseTemplatesChanged(String propertyname) {
      PartManager partmgr = this.PartManager;
      if (partmgr != null && this.Panel != null) {
        if (propertyname == "LinkTemplate" || propertyname == "LinkTemplateDictionary") {
          partmgr.RebuildLinkElements();
        } else {
          // ComputeDiagramBounds will temporarily find nothing of size,
          // so skip it to avoid repositioning the viewport
          this.Panel.SkipsNextUpdateDiagramBounds = true;
          partmgr.RebuildNodeElements();  // also calls RebuildLinkElements
          // now make sure all of the new Nodes have Locations
          RelayoutDiagram();
        }
      }
      if (this.TemplatesChanged != null) this.TemplatesChanged(this, new PropertyChangedEventArgs(propertyname));
    }

    /// <summary>
    /// This event is raised when one of the <c>DataTemplate</c> properties
    /// of the diagram is changed.
    /// </summary>
    [Category("Diagram")]
    public event PropertyChangedEventHandler TemplatesChanged;


    /// <summary>
    /// Identifies the <see cref="Stretch"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StretchProperty;
    /// <summary>
    /// Gets or sets how the parts are positioned in the panel.
    /// </summary>
    /// <value>
    /// The default value is <see cref="StretchPolicy.Unstretched"/>.
    /// Setting this value will also set the <see cref="DiagramPanel.Stretch"/> property
    /// on the <see cref="Panel"/>.
    /// </value>
    /// <remarks>
    /// Set this property to <see cref="StretchPolicy.Uniform"/>
    /// to cause the whole diagram to appear in the panel, at a small enough scale
    /// that everything fits.
    /// </remarks>
    [Category("Layout")]
    public StretchPolicy Stretch {
      get { return (StretchPolicy)GetValue(StretchProperty); }
      set { SetValue(StretchProperty, value); }
    }
    private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = d as Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;
      panel.Stretch = (StretchPolicy)e.NewValue;
    }


    /// <summary>
    /// Identifies the <see cref="InitialCenteredNodeData"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InitialCenteredNodeDataProperty;
    /// <summary>
    /// Gets or sets the data object for which the corresponding <see cref="Node"/>
    /// will be centered in the viewport after an initial layout has completed.
    /// </summary>
    /// <value>
    /// The default value is null.
    /// </value>
    /// <remarks>
    /// <para>
    /// One cannot scroll the diagram until the diagram and its <see cref="Panel"/>
    /// have been fully initialized, until the nodes and links have been realized,
    /// and until the initial layout has completed, ensuring that all nodes have locations.
    /// Rather than calling <see cref="DiagramPanel.CenterPart(Part)"/> in an
    /// <see cref="InitialLayoutCompleted"/> event handler, you can just set this
    /// property to refer to some data, and <see cref="DiagramPanel.OnInitialLayoutCompleted"/>
    /// will center the node for you.
    /// </para>
    /// <para>
    /// Note that if you set both this property and <see cref="CenteredNodeData"/>
    /// to different data, the latter will take precedence.
    /// </para>
    /// </remarks>
    public Object InitialCenteredNodeData {
      get { return GetValue(InitialCenteredNodeDataProperty); }
      set { SetValue(InitialCenteredNodeDataProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CenteredNodeData"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CenteredNodeDataProperty;
    /// <summary>
    /// Gets or sets the data object for which the corresponding <see cref="Node"/>
    /// will be centered in the viewport.
    /// </summary>
    /// <value>
    /// The default value is null.
    /// </value>
    /// <remarks>
    /// You can cause the currently selected node to be automatically centered by binding:
    /// <code>
    ///   &lt;go:Diagram x:Name="myDiagram" Grid.Row="0"
    ///       CenteredNodeData="{Binding ElementName=myDiagram, Path=SelectedNode.Data}" /&gt;
    /// </code>
    /// This calls <see cref="DiagramPanel.CenterPart(Part)"/> when the value of this property changes or
    /// when a layout completes in <see cref="DiagramPanel.OnLayoutCompleted"/>.
    /// <para>
    /// This property, if non-null, takes precedence over the <see cref="InitialCenteredNodeData"/> property.
    /// </para>
    /// </remarks>
    public Object CenteredNodeData {
      get { return GetValue(CenteredNodeDataProperty); }
      set { SetValue(CenteredNodeDataProperty, value); }
    }
    private static void OnCenteredNodeDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      DiagramPanel panel = diagram.Panel;
      PartManager mgr = diagram.PartManager;
      if (panel != null && mgr != null && !diagram.IsInTransaction) {
        Node node = mgr.FindNodeForData(e.NewValue, diagram.Model);
        if (node != null) {
          panel.CenterPart(node);
        }
      }
    }

    // Selection

    /// <summary>
    /// Identifies the read-only <see cref="SelectedParts"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedPartsProperty;
    /// <summary>
    /// This is an <c>ObservableCollection</c> of all selected <see cref="Part"/>s.
    /// </summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ObservableCollection<Part> SelectedParts {  //?? inefficient, but observability is very nice
      get { return (ObservableCollection<Part>)GetValue(SelectedPartsProperty); }
    }

    private static void OnSelectedPartsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      if (!diagram.Initializing) Diagram.Error("Cannot set Diagram.SelectedParts property of Diagram " + DiagramName(diagram));
    }



    private void SelectedParts_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
      var olditems = e.OldItems;
      var newitems = e.NewItems;
      switch (e.Action) {
        case NotifyCollectionChangedAction.Add: {
            foreach (Part part in e.NewItems) {
              part.IsSelected = true;
              if (this.SelectedPart == null) this.SelectedPart = part;
            }
            break;
          }
        case NotifyCollectionChangedAction.Remove: {
            foreach (Part part in e.OldItems) {
              part.IsSelected = false;
              if (this.SelectedPart == part) this.SelectedPart = null;
            }
            if (this.SelectedPart == null && this.SelectedParts.Count > 0) {
              this.SelectedPart = this.SelectedParts.FirstOrDefault();
            }
            break;
          }
        case NotifyCollectionChangedAction.Replace: {
            foreach (Part part in e.OldItems) {
              part.IsSelected = false;
              if (this.SelectedPart == part) this.SelectedPart = null;
            }
            foreach (Part part in e.NewItems) {
              part.IsSelected = true;
            }
            if (this.SelectedPart == null && this.SelectedParts.Count > 0) {
              this.SelectedPart = this.SelectedParts.FirstOrDefault();
            }
            break;
          }
        case NotifyCollectionChangedAction.Reset: {
            List<Part> deselects = new List<Part>();
            List<Part> selects = new List<Part>();
            foreach (Node part in this.Nodes) {
              bool v = this.SelectedParts.Contains(part);
              bool s = part.IsSelected;
              if (s && !v) deselects.Add(part);
              else if (!s && v) selects.Add(part);
            }
            foreach (Link part in this.Links) {
              bool v = this.SelectedParts.Contains(part);
              bool s = part.IsSelected;
              if (s && !v) deselects.Add(part);
              else if (!s && v) selects.Add(part);
            }
            foreach (Part p in deselects) p.IsSelected = false;
            foreach (Part p in selects) p.IsSelected = true;
            olditems = deselects;
            newitems = selects;
            this.SelectedPart = this.SelectedParts.FirstOrDefault();
            break;
          }
      }

      // raise the SelectionChanged event

      if (this.SelectionChanged != null) {
        this.SelectionChanged(this, new SelectionChangedEventArgs(olditems ?? EmptyList, newitems ?? EmptyList));
      }




      UpdateCommands();
    }

    internal System.Collections.IList EmptyList = new object[0] {};

    /// <summary>
    /// Identifies the <see cref="MaximumSelectionCount"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaximumSelectionCountProperty;
    /// <summary>
    /// Gets or sets the maximum number of selected parts.
    /// </summary>
    /// <value>
    /// The default value is a large positive integer.
    /// Any new value must be non-negative.
    /// </value>
    /// <remarks>
    /// Decreasing this value may cause parts to be removed from <see cref="SelectedParts"/>
    /// in order to meet the new lower limit.
    /// </remarks>
    [Category("Behavior")]
    public int MaximumSelectionCount {
      get { return (int)GetValue(MaximumSelectionCountProperty); }
      set { SetValue(MaximumSelectionCountProperty, value); }
    }
    private static void OnMaximumSelectionCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      int max = Math.Max(0, diagram.MaximumSelectionCount);
      int excess = diagram.SelectedParts.Count() - max;
      if (excess > 0) {
        List<Part> extra = diagram.SelectedParts.Skip(max).ToList();
        foreach (Part p in extra) {
          p.IsSelected = false;
        }
      }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedPart"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedPartProperty;
    /// <summary>
    /// Gets or sets the primary selected <see cref="Part"/>.
    /// </summary>
    /// <value>
    /// This may be null, if no part is selected.
    /// </value>
    /// <remarks>
    /// Changing this value will make the previous value (if any) be not <see cref="Part.IsSelected"/>,
    /// and will select the new value.
    /// </remarks>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Part SelectedPart {
      get { return (Part)GetValue(SelectedPartProperty); }
      set { SetValue(SelectedPartProperty, value); }
    }
    private static void OnSelectedPartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      Part oldpart = e.OldValue as Part;
      if (oldpart != null) {
        oldpart.IsSelected = false;
        Node node = oldpart as Node;
        if (node != null) {
          Group group = oldpart as Group;
          if (group != null) {
            diagram.SelectedGroup = null;
          } else {
            diagram.SelectedNode = null;
          }
        } else {
          Link link = oldpart as Link;
          if (link != null) diagram.SelectedLink = null;
        }
      }
      Part newpart = e.NewValue as Part;
      if (newpart != null) {
        if (newpart.Diagram != diagram) Diagram.Error("Diagram.SelectedPart setter: cannot select a part that is not part of this Diagram: " + newpart.ToString());
        newpart.IsSelected = true;
        Node node = newpart as Node;
        if (node != null) {
          Group group = newpart as Group;
          if (group != null) {
            diagram.SelectedGroup = group;
          } else {
            diagram.SelectedNode = node;
          }
        } else {
          Link link = newpart as Link;
          if (link != null) diagram.SelectedLink = link;
        }
      }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedNode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedNodeProperty;
    /// <summary>
    /// Gets or sets the primary selected <see cref="Node"/>.
    /// </summary>
    /// <value>
    /// This may be null, if <see cref="SelectedPart"/> is not a <see cref="Node"/> or if it is a <see cref="Group"/>.
    /// </value>
    /// <remarks>
    /// <para>
    /// Changing this value will make the previous value (if any) be not <see cref="Part.IsSelected"/>,
    /// and will select the new value.
    /// </para>
    /// <para>
    /// Although <see cref="Group"/> inherits from <see cref="Node"/>,
    /// this property will be null if the selected part is a <see cref="Group"/>
    /// in order to make it easier to data-bind parts of your user interface differently
    /// for groups than for regular nodes.
    /// </para>
    /// </remarks>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Node SelectedNode {
      get { return (Node)GetValue(SelectedNodeProperty); }
      set { SetValue(SelectedNodeProperty, value); }
    }
    private static void OnSelectedNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      diagram.SelectedPart = e.NewValue as Part;
    }

    /// <summary>
    /// Identifies the <see cref="SelectedGroup"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedGroupProperty;
    /// <summary>
    /// Gets or sets the primary selected <see cref="Group"/>.
    /// </summary>
    /// <value>
    /// This may be null, if <see cref="SelectedPart"/> is not a <see cref="Group"/>.
    /// </value>
    /// <remarks>
    /// Changing this value will make the previous value (if any) be not <see cref="Part.IsSelected"/>,
    /// and will select the new value.
    /// </remarks>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Group SelectedGroup {
      get { return (Group)GetValue(SelectedGroupProperty); }
      set { SetValue(SelectedGroupProperty, value); }
    }
    private static void OnSelectedGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      diagram.SelectedPart = e.NewValue as Part;
    }

    /// <summary>
    /// Identifies the <see cref="SelectedLink"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedLinkProperty;
    /// <summary>
    /// Gets or sets the primary selected <see cref="Link"/>.
    /// </summary>
    /// <value>
    /// This may be null, if <see cref="SelectedPart"/> is not a <see cref="Link"/>.
    /// </value>
    /// <remarks>
    /// Changing this value will make the previous value (if any) be not <see cref="Part.IsSelected"/>,
    /// and will select the new value.
    /// </remarks>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Link SelectedLink {
      get { return (Link)GetValue(SelectedLinkProperty); }
      set { SetValue(SelectedLinkProperty, value); }
    }
    private static void OnSelectedLinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      diagram.SelectedPart = e.NewValue as Part;
    }

    /// <summary>
    /// Make the given <paramref name="part"/>
    /// the only selected <see cref="Part"/>.
    /// </summary>
    /// <param name="part">
    /// A <see cref="Part"/> that belongs to this diagram.
    /// If this value is null, this does nothing.
    /// </param>
    public void Select(Part part) {
      VerifyAccess();
      if (part != null && part.Diagram == this) {
        if (!part.IsSelected || this.SelectedParts.Count > 1) {
          ClearSelection();
          part.IsSelected = true;
        }
      }
    }

    /// <summary>
    /// Make all of the <see cref="Part"/>s in the given collection of <paramref name="parts"/>
    /// the only selected parts.
    /// </summary>
    /// <param name="parts">
    /// An <c>IEnumerable&lt;Part&gt;</c> of <see cref="Part"/>s that belong to this diagram.
    /// If this value is null, this does nothing.
    /// </param>
    public void Select(IEnumerable<Part> parts) {
      VerifyAccess();
      if (parts != null) {
        // use ToList since the SelectedParts collection will be changing
        foreach (Part p in this.SelectedParts.Where(p => p.Diagram == this).Except(parts).ToList()) {
          p.IsSelected = false;
        }
        foreach (Part p in parts.Where(p => !p.IsSelected && p.Diagram == this)) {
          p.IsSelected = true;
        }
      }
    }

    /// <summary>
    /// Remove all <see cref="Part"/>s from the collection of <see cref="SelectedParts"/>.
    /// </summary>
    public void ClearSelection() {
      VerifyAccess();
      ObservableCollection<Part> sel = this.SelectedParts;
      if (sel.Count > 0) sel.Clear();
      this.SelectedPart = null;
      this.SelectedNode = null;
      this.SelectedGroup = null;
      this.SelectedLink = null;
    }


    // Transactions

    /// <summary>
    /// Start an undoable transaction (a collection of model changes)
    /// by calling <see cref="IDiagramModel.StartTransaction"/>.
    /// </summary>
    /// <param name="tname">
    /// a description of the changes that are about to happen
    /// </param>
    /// <returns></returns>
    public bool StartTransaction(String tname) {
      IDiagramModel model = this.Model;
      if (model != null) {
        return model.StartTransaction(tname);
      } else {
        return false;
      }
    }

    /// <summary>
    /// Complete an undoable transaction (a collection of model changes)
    /// by calling <see cref="IDiagramModel.CommitTransaction"/>.
    /// </summary>
    /// <param name="tname">
    /// a description of the changes that just happened
    /// </param>
    /// <returns></returns>
    public bool CommitTransaction(String tname) {
      IDiagramModel model = this.Model;
      if (model != null) {
        return model.CommitTransaction(tname);
      } else {
        return false;
      }
    }

    /// <summary>
    /// Abort the current transaction and rollback (undo) any of the changes
    /// by calling <see cref="IDiagramModel.RollbackTransaction"/>.
    /// </summary>
    /// <returns></returns>
    public bool RollbackTransaction() {
      IDiagramModel model = this.Model;
      if (model != null) {
        return model.RollbackTransaction();
      } else {
        return false;
      }
    }

    /// <summary>
    /// This property is true when a transaction has been started on the
    /// <see cref="Model"/>'s <see cref="UndoManager"/>.
    /// </summary>
    public bool IsInTransaction {
      get {
        IDiagramModel model = this.Model;
        if (model != null) {
          return model.IsInTransaction;
        } else {
          return false;
        }
      }
    }


    // Layout

    /// <summary>
    /// Identifies the <see cref="LayoutManager"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LayoutManagerProperty;
    /// <summary>
    /// Gets or sets the <see cref="Northwoods.GoXam.LayoutManager"/> that is responsible
    /// for positioning all of the nodes in the diagram.
    /// </summary>
    /// <value>
    /// Initially this is an instance of <see cref="Northwoods.GoXam.LayoutManager"/>.
    /// There must always be a non-null value for this property.
    /// LayoutManagers cannot be shared by Diagrams.
    /// </value>
    [Category("DiagramLayout")]
    public LayoutManager LayoutManager {
      get { return (LayoutManager)GetValue(LayoutManagerProperty); }
      set { SetValue(LayoutManagerProperty, value); }
    }
    private static void OnLayoutManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      LayoutManager oldmgr = e.OldValue as LayoutManager;
      if (oldmgr != null) {
        diagram.RemoveLogical(oldmgr);
        oldmgr.Diagram = null;
      }
      LayoutManager newmgr = e.NewValue as LayoutManager;
      if (newmgr != null) {
        if (newmgr.Diagram != null) Diagram.Error("Cannot share LayoutManagers between Diagrams: " + newmgr.Diagram.ToString() + " " + diagram.ToString());
        newmgr.Diagram = diagram;
        diagram.AddLogical(newmgr);
      } else {
        Diagram.Trace("WARNING: setting Diagram.LayoutManager to null may produce inconsistent results");
        diagram.LayoutManager = new LayoutManager();
      }
    }

    /// <summary>
    /// Identifies the <see cref="Layout"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LayoutProperty;
    /// <summary>
    /// Gets or sets the <see cref="IDiagramLayout"/> responsible for positioning all of
    /// the top-level nodes in the diagram.
    /// </summary>
    /// <value>
    /// Initially this is an instance of <see cref="Northwoods.GoXam.Layout.DiagramLayout"/>.
    /// This value may be null, in which case there is no automatic layout of top-level nodes,
    /// but there still may be layout of the nodes that are members of groups that have
    /// their own layouts.
    /// </value>
    /// <remarks>
    /// The <see cref="LayoutManager"/> is responsible for performing this layout
    /// after performing any nested layouts that might be defined for some groups
    /// via the <see cref="Group.Layout"/> property.
    /// </remarks>
    [Category("DiagramLayout")]
    public IDiagramLayout Layout {
      get { return (IDiagramLayout)GetValue(LayoutProperty); }
      set { SetValue(LayoutProperty, value); }
    }
    private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      IDiagramLayout oldlayout = e.OldValue as IDiagramLayout;
      if (oldlayout != null) {
        diagram.RemoveLogical(oldlayout);
        oldlayout.Diagram = null;
        oldlayout.Group = null;
      }
      IDiagramLayout layout = e.NewValue as IDiagramLayout;
      if (layout != null) {
        if (layout.Diagram != null && layout.Diagram != diagram) {
          Diagram.Error("IDiagramLayout cannot be shared by multiple Diagrams");
        }
        layout.Diagram = diagram;
        layout.Group = null;
        diagram.AddLogical(layout);
        if (!diagram.Initializing) {
          LayoutManager laymgr = diagram.LayoutManager;
          if (laymgr != null) {
            laymgr.InvalidateLayout(null, LayoutChange.DiagramLayoutChanged);
          }
        }
      }
    }

    /// <summary>
    /// Request a re-layout of all of the nodes and links in this diagram.
    /// </summary>
    /// <remarks>
    /// This just calls <see cref="Northwoods.GoXam.LayoutManager.LayoutDiagram()"/>.
    /// </remarks>
    public void LayoutDiagram() {
      LayoutManager laymgr = this.LayoutManager;
      if (laymgr != null) laymgr.LayoutDiagram();
    }

    internal void UpdateDiagramLayout() {  // request performing any invalid layouts
      LayoutManager laymgr = this.LayoutManager;
      if (laymgr != null) laymgr.UpdateDiagramLayout();
    }

    internal void RelayoutDiagram() {  // called by Rebuild, RaiseTemplatesChanged, and significant model changes (e.g. ChangedNodesSource or ChangedNodeKeyPath)
      LayoutManager laymgr = this.LayoutManager;
      if (laymgr != null) laymgr.LayoutDiagram(laymgr.Initial, false);
    }

    // Printing

    /// <summary>
    /// Identifies the <see cref="PrintManager"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PrintManagerProperty;
    /// <summary>
    /// Gets or sets the <see cref="Northwoods.GoXam.PrintManager"/> that is responsible
    /// for printing the diagram.
    /// </summary>
    /// <value>
    /// Initially this is an instance of <see cref="Northwoods.GoXam.PrintManager"/>.
    /// PrintManagers cannot be shared by Diagrams.
    /// </value>
    [Category("DiagramPrint")]
    public PrintManager PrintManager {
      get { return (PrintManager)GetValue(PrintManagerProperty); }
      set { SetValue(PrintManagerProperty, value); }
    }
    private static void OnPrintManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      PrintManager oldmgr = e.OldValue as PrintManager;
      if (oldmgr != null) {
        diagram.RemoveLogical(oldmgr);
        oldmgr.Diagram = null;
      }
      PrintManager newmgr = e.NewValue as PrintManager;
      if (newmgr != null) {
        if (newmgr.Diagram != null) Diagram.Error("Cannot share PrintManagers between Diagrams: " + newmgr.Diagram.ToString() + " " + diagram.ToString());
        newmgr.Diagram = diagram;
        diagram.AddLogical(oldmgr);
      }
    }

    
    // user interaction

    /// <summary>
    /// Identifies the <see cref="IsReadOnly"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsReadOnlyProperty;
    /// <summary>
    /// Gets or sets whether the user may not modify the diagram.
    /// This is a dependency property.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    [Category("Behavior")]
    public bool IsReadOnly {
      get { return (bool)GetValue(IsReadOnlyProperty); }
      set { SetValue(IsReadOnlyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowClipboard"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowClipboardProperty;
    /// <summary>
    /// Gets or sets whether the user may copy to or paste from the clipboard,
    /// either the system clipboard or the internal one.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowClipboard {
      get { return (bool)GetValue(AllowClipboardProperty); }
      set { SetValue(AllowClipboardProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowCopy"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowCopyProperty;
    /// <summary>
    /// Gets or sets whether the user may copy parts.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowCopy {
      get { return (bool)GetValue(AllowCopyProperty); }
      set { SetValue(AllowCopyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowDelete"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowDeleteProperty;
    /// <summary>
    /// Gets or sets whether the user may delete parts from the diagram.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowDelete {
      get { return (bool)GetValue(AllowDeleteProperty); }
      set { SetValue(AllowDeleteProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowDragOut"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowDragOutProperty;
    /// <summary>
    /// Gets or sets whether the user may start a drag-and-drop in this diagram,
    /// possibly dropping in a different control.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    [Category("Behavior")]
    public bool AllowDragOut {
      get { return (bool)GetValue(AllowDragOutProperty); }
      set { SetValue(AllowDragOutProperty, value); }
    }










































    /// <summary>
    /// Identifies the <see cref="AllowEdit"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowEditProperty;
    /// <summary>
    /// Gets or sets whether the user may do in-place text editing.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowEdit {
      get { return (bool)GetValue(AllowEditProperty); }
      set { SetValue(AllowEditProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowGroup"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowGroupProperty;
    /// <summary>
    /// Gets or sets whether the user may group parts together.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowGroup {
      get { return (bool)GetValue(AllowGroupProperty); }
      set { SetValue(AllowGroupProperty, value); }
    }

    ///// <summary>
    ///// Identifies the <see cref="AllowRegroup"/> dependency property.
    ///// </summary>
    //public static readonly DependencyProperty AllowRegroupProperty;
    ///// <summary>
    ///// Gets or sets whether the user may regroup parts.
    ///// </summary>
    ///// <value>
    ///// The default value is true.
    ///// </value>
    //[Category("Behavior")]
    //public bool AllowRegroup {
    //  get { return (bool)GetValue(AllowRegroupProperty); }
    //  set { SetValue(AllowRegroupProperty, value); }
    //}

    /// <summary>
    /// Identifies the <see cref="AllowUngroup"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowUngroupProperty;
    /// <summary>
    /// Gets or sets whether the user may ungroup existing group nodes.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowUngroup {
      get { return (bool)GetValue(AllowUngroupProperty); }
      set { SetValue(AllowUngroupProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowInsert"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowInsertProperty;
    /// <summary>
    /// Gets or sets whether the user may insert parts into the diagram.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowInsert {
      get { return (bool)GetValue(AllowInsertProperty); }
      set { SetValue(AllowInsertProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowLink"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowLinkProperty;
    /// <summary>
    /// Gets or sets whether the user may draw new links.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowLink {
      get { return (bool)GetValue(AllowLinkProperty); }
      set { SetValue(AllowLinkProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowRelink"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowRelinkProperty;
    /// <summary>
    /// Gets or sets whether the user may reconnect existing links.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowRelink {
      get { return (bool)GetValue(AllowRelinkProperty); }
      set { SetValue(AllowRelinkProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowMove"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowMoveProperty;
    /// <summary>
    /// Gets or sets whether the user may move parts.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowMove {
      get { return (bool)GetValue(AllowMoveProperty); }
      set { SetValue(AllowMoveProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowPrint"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowPrintProperty;
    /// <summary>
    /// Gets or sets whether the user may print parts.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowPrint {
      get { return (bool)GetValue(AllowPrintProperty); }
      set { SetValue(AllowPrintProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowReshape"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowReshapeProperty;
    /// <summary>
    /// Gets or sets whether the user may reshape parts.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowReshape {
      get { return (bool)GetValue(AllowReshapeProperty); }
      set { SetValue(AllowReshapeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowResize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowResizeProperty;
    /// <summary>
    /// Gets or sets whether the user may resize parts.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowResize {
      get { return (bool)GetValue(AllowResizeProperty); }
      set { SetValue(AllowResizeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowRotate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowRotateProperty;
    /// <summary>
    /// Gets or sets whether the user may rotate parts.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowRotate {
      get { return (bool)GetValue(AllowRotateProperty); }
      set { SetValue(AllowRotateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowScroll"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowScrollProperty;
    /// <summary>
    /// Gets or sets whether the user may scroll the diagram,
    /// thereby changing the value of <see cref="DiagramPanel.Position"/>.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowScroll {
      get { return (bool)GetValue(AllowScrollProperty); }
      set { SetValue(AllowScrollProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowSelect"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowSelectProperty;
    /// <summary>
    /// Gets or sets whether the user may select parts.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowSelect {
      get { return (bool)GetValue(AllowSelectProperty); }
      set { SetValue(AllowSelectProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowUndo"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowUndoProperty;
    /// <summary>
    /// Gets or sets whether the user may undo any changes.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowUndo {
      get { return (bool)GetValue(AllowUndoProperty); }
      set { SetValue(AllowUndoProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowZoom"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowZoomProperty;
    /// <summary>
    /// Gets or sets whether the user may zoom the diagram,
    /// thereby changing the value of <see cref="DiagramPanel.Scale"/>.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// </value>
    [Category("Behavior")]
    public bool AllowZoom {
      get { return (bool)GetValue(AllowZoomProperty); }
      set { SetValue(AllowZoomProperty, value); }
    }


    // tools

    /// <summary>
    /// Identifies the <see cref="CurrentTool"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CurrentToolProperty;
    /// <summary>
    /// Gets or sets the current <see cref="IDiagramTool"/> that handles the diagram's input events.
    /// </summary>
    /// <value>
    /// Initially this is set to the value of <see cref="DefaultTool"/>.
    /// Setting this to a null value is treated as if it were set to the <see cref="DefaultTool"/>,
    /// because there should always be a currently running tool, except when the diagram is
    /// being initialized.
    /// </value>
    /// <remarks>
    /// <para>
    /// Setting this property to a new tool stops the previous current tool.
    /// If the old tool was <see cref="IDiagramTool.Active"/>,
    /// this calls its <see cref="IDiagramTool.DoDeactivate"/> method.
    /// Then it calls <see cref="IDiagramTool.DoStop"/> on it.
    /// </para>
    /// <para>
    /// This starts the new tool by calling <see cref="IDiagramTool.DoStart"/> on it.
    /// </para>
    /// <para>
    /// Normally this is a <see cref="ToolManager"/> ready to select and run a mode-less tool
    /// from the lists of mouse tools (<see cref="MouseDownTools"/>, <see cref="MouseMoveTools"/>,
    /// <see cref="MouseUpTools"/>).
    /// </para>
    /// <para>
    /// You can run a modal tool just by setting this property.
    /// When that tool is finished it should call <see cref="DiagramTool.StopTool"/>
    /// or it can set this property to the value of <see cref="DefaultTool"/>.
    /// </para>
    /// <para>
    /// Because this property is frequently set, it cannot be used effectively as a data-binding target.
    /// </para>
    /// </remarks>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IDiagramTool CurrentTool {
      get { return (IDiagramTool)GetValue(CurrentToolProperty); }
      set { SetValue(CurrentToolProperty, value); }
    }
    private static void OnCurrentToolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      IDiagramTool oldtool = (IDiagramTool)e.OldValue;
      IDiagramTool newtool = (IDiagramTool)e.NewValue;
      if (oldtool != null) {
        if (oldtool.Active) oldtool.DoDeactivate();
        oldtool.DoStop();
      }
      if (newtool != null) {
        newtool.Diagram = diagram;
        //Diagram.Debug("Starting " + newtool.GetType().ToString());
        newtool.DoStart();
        // but not DoActivate, since that might depend on conditions
      } else {  // assume null value for new tool means use the DefaultTool instead
        newtool = diagram.DefaultTool;
        if (newtool != null) diagram.CurrentTool = newtool;
      }
    }

    /// <summary>
    /// Identifies the <see cref="DefaultTool"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultToolProperty;
    /// <summary>
    /// Gets or sets the default tool, which is used as the value of <see cref="CurrentTool"/>
    /// both initially as well as whenever a tool stops.
    /// </summary>
    /// <value>
    /// Setting this property also sets the <see cref="CurrentTool"/>
    /// if the old default tool is the currently running tool.
    /// </value>
    /// <remarks>
    /// Normally the default tool is an instance of <see cref="ToolManager"/>,
    /// which manages mode-less mouse tools.
    /// </remarks>
    [Category("Tools")]
    public IDiagramTool DefaultTool {
      get { return (IDiagramTool)GetValue(DefaultToolProperty); }
      set { SetValue(DefaultToolProperty, value); }
    }
    private static void OnDefaultToolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      IDiagramTool oldtool = (IDiagramTool)e.OldValue;
      if (oldtool != null) {
        diagram.RemoveLogical(oldtool);
        oldtool.Diagram = null;
      }
      IDiagramTool newtool = (IDiagramTool)e.NewValue;
      if (newtool == null) {
        diagram.DefaultTool = new ToolManager();
      } else {
        if (newtool.Diagram != null) Diagram.Error("Cannot share DefaultTool between Diagrams");
        newtool.Diagram = diagram;
        diagram.AddLogical(newtool);
      }
      if (diagram.CurrentTool == oldtool) {
        diagram.CurrentTool = newtool;
      }
    }


    /// <summary>
    /// Find a mouse tool of a given type.
    /// </summary>
    /// <typeparam name="T">this must implement <see cref="IDiagramTool"/></typeparam>
    /// <param name="subclass">
    /// If true, a matching tool may be a subclass of <typeparamref name="T"/>;
    /// otherwise the tool must be of exactly the same type.
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// This searches the <see cref="MouseDownTools"/>,
    /// <see cref="MouseMoveTools"/>, and <see cref="MouseUpTools"/> lists.
    /// </remarks>
    public T FindMouseTool<T>(bool subclass) where T : IDiagramTool {
      VerifyAccess();
      foreach (IDiagramTool tool in this.MouseDownTools) {
        if (subclass ? tool is T : tool.GetType() == typeof(T)) return (T)tool;
      }
      foreach (IDiagramTool tool in this.MouseMoveTools) {
        if (subclass ? tool is T : tool.GetType() == typeof(T)) return (T)tool;
      }
      foreach (IDiagramTool tool in this.MouseUpTools) {
        if (subclass ? tool is T : tool.GetType() == typeof(T)) return (T)tool;
      }
      return default(T);
    }


    /// <summary>
    /// Replace a mouse tool of a given type with a new tool.
    /// </summary>
    /// <typeparam name="T">this must implement <see cref="IDiagramTool"/></typeparam>
    /// <param name="newtool">
    /// If the new value is null, any tool that the search finds will just be removed
    /// from the list in which it was found.
    /// </param>
    /// <param name="subclass">
    /// If true, a matching tool may be a subclass of <typeparamref name="T"/>;
    /// otherwise the tool must be of exactly the same type.
    /// </param>
    /// <returns>the old tool that was replaced by the new one</returns>
    /// <remarks>
    /// This searches the <see cref="MouseDownTools"/>,
    /// <see cref="MouseMoveTools"/>, and <see cref="MouseUpTools"/> lists.
    /// The new tool is inserted into the same list in which a matching tool is found,
    /// at the same position as the old tool.
    /// </remarks>
    public T ReplaceMouseTool<T>(IDiagramTool newtool, bool subclass) where T : IDiagramTool {
      VerifyAccess();

      if (newtool != null) {
        newtool.Diagram = this;
      }

      int i;
      IList<IDiagramTool> tools;

      tools = this.MouseDownTools;
      for (i = 0; i < tools.Count; i++) {
        if (subclass ? tools[i] is T : tools[i].GetType() == typeof(T)) {
          T oldtool = (T)tools[i];
          if (newtool == null)
            tools.RemoveAt(i);
          else
            tools[i] = newtool;
          return oldtool;
        }
      }

      tools = this.MouseMoveTools;
      for (i = 0; i < tools.Count; i++) {
        if (subclass ? tools[i] is T : tools[i].GetType() == typeof(T)) {
          T oldtool = (T)tools[i];
          if (newtool == null)
            tools.RemoveAt(i);
          else
            tools[i] = newtool;
          return oldtool;
        }
      }

      tools = this.MouseUpTools;
      for (i = 0; i < tools.Count; i++) {
        if (subclass ? tools[i] is T : tools[i].GetType() == typeof(T)) {
          T oldtool = (T)tools[i];
          if (newtool == null)
            tools.RemoveAt(i);
          else
            tools[i] = newtool;
          return oldtool;
        }
      }
      return default(T);
    }


    // MouseDownTools

    /// <summary>
    /// Gets the list of <see cref="IDiagramTool"/>s searched by the
    /// <see cref="ToolManager"/> to look for tools to run upon a mouse down event.
    /// </summary>
    /// <value>
    /// The initial value is a list of instances of the following tools:
    /// <see cref="RelinkingTool"/>
    /// <see cref="LinkReshapingTool"/>
    /// <see cref="ResizingTool"/>
    /// <see cref="RotatingTool"/>
    /// </value>
    /// <remarks>
    /// When explicitly adding a tool to this list
    /// because it is not one of the types listed above,
    /// be sure to remember to set
    /// its <see cref="IDiagramTool.Diagram"/> backpointer to refer to this diagram.
    /// </remarks>
    [Category("Tools")]
    public IList<IDiagramTool> MouseDownTools { get; private set; }

    private static void OnMouseDownToolChanged<T>(DependencyObject d, DependencyPropertyChangedEventArgs e)
        where T : IDiagramTool {
      Diagram diagram = (Diagram)d;
      T oldtool = (T)e.OldValue;
      if (oldtool != null) {
        diagram.RemoveLogical(oldtool);
        oldtool.Diagram = null;
      }
      T tool = (T)e.NewValue;
      if (tool != null) {
        if (tool.Diagram != null) Diagram.Error("Cannot share tools between Diagrams: " + tool.ToString());
        tool.Diagram = diagram;
        diagram.AddLogical(tool);
      }
      if (diagram.FindMouseTool<T>(true) != null) {
        diagram.ReplaceMouseTool<T>(tool, true);  // TOOL may be null
      } else if (tool != null) {
        diagram.MouseDownTools.Add(tool);
      }
    }

    /// <summary>
    /// Identifies the <see cref="RelinkingTool"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RelinkingToolProperty;
    /// <summary>
    /// Gets or sets the standard mouse-down tool for reconnecting an existing link.
    /// </summary>
    /// <value>
    /// The default value is an instance of <see cref="RelinkingTool"/>.
    /// </value>
    [Category("Tools")]
    public RelinkingTool RelinkingTool {
      get { return (RelinkingTool)GetValue(RelinkingToolProperty); }
      set { SetValue(RelinkingToolProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LinkReshapingTool"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LinkReshapingToolProperty;
    /// <summary>
    /// Gets or sets the standard mouse-down tool for reshaping a link.
    /// </summary>
    /// <value>
    /// The default value is an instance of <see cref="LinkReshapingTool"/>.
    /// </value>
    [Category("Tools")]
    public LinkReshapingTool LinkReshapingTool {
      get { return (LinkReshapingTool)GetValue(LinkReshapingToolProperty); }
      set { SetValue(LinkReshapingToolProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ResizingTool"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ResizingToolProperty;
    /// <summary>
    /// Gets or sets the standard mouse-down tool for resizing a part.
    /// </summary>
    /// <value>
    /// The default value is an instance of <see cref="ResizingTool"/>.
    /// </value>
    [Category("Tools")]
    public ResizingTool ResizingTool {
      get { return (ResizingTool)GetValue(ResizingToolProperty); }
      set { SetValue(ResizingToolProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="RotatingTool"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RotatingToolProperty;
    /// <summary>
    /// Gets or sets the standard mouse-down tool for rotating a part.
    /// </summary>
    /// <value>
    /// The default value is an instance of <see cref="RotatingTool"/>.
    /// </value>
    [Category("Tools")]
    public RotatingTool RotatingTool {
      get { return (RotatingTool)GetValue(RotatingToolProperty); }
      set { SetValue(RotatingToolProperty, value); }
    }


    // MouseMoveTools

    /// <summary>
    /// Gets the list of <see cref="IDiagramTool"/>s searched by the
    /// <see cref="ToolManager"/> to look for tools to run upon a mouse move event.
    /// </summary>
    /// <value>
    /// The initial value is a list of instances of the following tools:
    /// <see cref="LinkingTool"/>
    /// <see cref="DraggingTool"/>
    /// <see cref="DragSelectingTool"/>
    /// <see cref="PanningTool"/>
    /// </value>
    /// <remarks>
    /// When explicitly adding a tool to this list
    /// because it is not one of the types listed above
    /// nor <see cref="DragZoomingTool"/> (which is not normally part of the list),
    /// be sure to remember to set
    /// its <see cref="IDiagramTool.Diagram"/> backpointer to refer to this diagram.
    /// </remarks>
    [Category("Tools")]
    public IList<IDiagramTool> MouseMoveTools { get; private set; }

    private static void OnMouseMoveToolChanged<T>(DependencyObject d, DependencyPropertyChangedEventArgs e)
        where T : IDiagramTool {
      Diagram diagram = (Diagram)d;
      T oldtool = (T)e.OldValue;
      if (oldtool != null) {
        diagram.RemoveLogical(oldtool);
        oldtool.Diagram = null;
      }
      T tool = (T)e.NewValue;
      if (tool != null) {
        if (tool.Diagram != null) Diagram.Error("Cannot share tools between Diagrams: " + tool.ToString());
        tool.Diagram = diagram;
        diagram.AddLogical(tool);
      }
      if (diagram.FindMouseTool<T>(true) != null) {
        diagram.ReplaceMouseTool<T>(tool, true);  // TOOL may be null
      } else if (tool != null) {
        diagram.MouseMoveTools.Add(tool);
      }
    }

    /// <summary>
    /// Identifies the <see cref="LinkingTool"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LinkingToolProperty;
    /// <summary>
    /// Gets or sets the standard mouse-move tool for drawing a new link.
    /// </summary>
    /// <value>
    /// The default value is an instance of <see cref="LinkingTool"/>.
    /// </value>
    [Category("Tools")]
    public LinkingTool LinkingTool {
      get { return (LinkingTool)GetValue(LinkingToolProperty); }
      set { SetValue(LinkingToolProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DraggingTool"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DraggingToolProperty;
    /// <summary>
    /// Gets or sets the standard mouse-move tool for dragging (moving or copying) the selection.
    /// </summary>
    /// <value>
    /// The default value is an instance of <see cref="DraggingTool"/>.
    /// </value>
    [Category("Tools")]
    public DraggingTool DraggingTool {
      get { return (DraggingTool)GetValue(DraggingToolProperty); }
      set { SetValue(DraggingToolProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DragSelectingTool"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DragSelectingToolProperty;
    /// <summary>
    /// Gets or sets the standard mouse-move tool for selecting many objects with a rubber-band rectangle.
    /// </summary>
    /// <value>
    /// The default value is an instance of <see cref="DragSelectingTool"/>.
    /// </value>
    /// <remarks>
    /// <para>
    /// For example, you can change the selection policy in XAML by:
    /// <code>
    ///   &lt;go:Diagram ...&gt;
    ///     &lt;go:Diagram.DragSelectingTool&gt;
    ///       &lt;gotool:DragSelectingTool Include="Intersects" /&gt;
    ///     &lt;/go:Diagram.DragSelectingTool&gt;
    ///   &lt;/go:Diagram&gt;
    /// </code>
    /// </para>
    /// </remarks>
    [Category("Tools")]
    public DragSelectingTool DragSelectingTool {
      get { return (DragSelectingTool)GetValue(DragSelectingToolProperty); }
      set { SetValue(DragSelectingToolProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PanningTool"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PanningToolProperty;
    /// <summary>
    /// Gets or sets the standard mouse-move tool for manual panning.
    /// </summary>
    /// <value>
    /// The default value is an instance of <see cref="PanningTool"/>.
    /// </value>
    /// <remarks>
    /// <para>
    /// Normally the <see cref="DragSelectingTool"/> takes precedence,
    /// because both tools operate on a left-button-mouse-down in the background
    /// followed by a mouse-drag.  But <see cref="DragSelectingTool"/> precedes this
    /// <see cref="PanningTool"/> in the list of <see cref="MouseMoveTools"/>,
    /// so if it is applicable, it is always chosen first.
    /// </para>
    /// <para>
    /// To enable this mode-less panning tool, you can remove the drag-selecting
    /// tool or you can disable that tool by disallowing selection.
    /// Remove the tool in WPF XAML by:
    /// <code>
    ///   &lt;go:Diagram ... DragSelectingTool="{x:Null}" &gt;
    ///     . . .
    ///   &lt;/go:Diagram&gt;
    /// </code>
    /// or remove it in code by:
    /// <code>
    ///   myDiagram.DragSelectingTool = null;
    /// </code>
    /// You can disable user selection by setting the <see cref="AllowSelect"/> property to false.
    /// </para>
    /// </remarks>
    [Category("Tools")]
    public PanningTool PanningTool {
      get { return (PanningTool)GetValue(PanningToolProperty); }
      set { SetValue(PanningToolProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DragZoomingTool"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DragZoomingToolProperty;
    /// <summary>
    /// Gets or sets the non-standard mouse-move tool for manual zooming.
    /// </summary>
    /// <value>
    /// The default value is null.
    /// </value>
    /// <remarks>
    /// <para>
    /// Because this property is normally null, there is no standard mode-less drag-zooming tool.
    /// You can install this tool by creating an instance of it and setting that diagram property.
    /// However, although it can start, the <see cref="DragSelectingTool"/> and
    /// the <see cref="PanningTool"/> are two other background-mouse-drag mode-less tools that are normally
    /// present and earlier in the <see cref="MouseMoveTools"/> and thus will take precedence over this tool.
    /// </para>
    /// <para>
    /// To make this mode-less tool effective, you can remove the other two background mouse-dragging tools
    /// and install the <see cref="Northwoods.GoXam.Tool.DragZoomingTool"/> in XAML:
    /// <code>
    ///   &lt;go:Diagram ... DragSelectingTool="{x:Null}" PanningTool="{x:Null}" &gt;
    ///     &lt;go:Diagram.DragZoomingTool&gt;
    ///       &lt;go:DragZoomingTool /&gt;
    ///     &lt;/go:Diagram.DragZoomingTool&gt;
    ///   &lt;/go:Diagram&gt;
    /// </code>
    /// You could also do the same in code:
    /// <code>
    ///   myDiagram.DragSelectingTool = null;
    ///   myDiagram.PanningTool = null;
    ///   myDiagram.DragZoomingTool = new DragZoomingTool();
    /// </code>
    /// </para>
    /// </remarks>
    [Category("Tools")]
    public DragZoomingTool DragZoomingTool {
      get { return (DragZoomingTool)GetValue(DragZoomingToolProperty); }
      set { SetValue(DragZoomingToolProperty, value); }
    }


    // MouseUpTools

    /// <summary>
    /// Gets the list of <see cref="IDiagramTool"/>s searched by the
    /// <see cref="ToolManager"/> to look for tools to run upon a mouse up event.
    /// </summary>
    /// <value>
    /// The initial value is a list of instances of the following tools:
    /// <see cref="TextEditingTool"/>
    /// <see cref="ClickCreatingTool"/>
    /// <see cref="ClickSelectingTool"/>
    /// </value>
    /// <remarks>
    /// When explicitly adding a tool to this list
    /// because it is not one of the types listed above,
    /// be sure to remember to set
    /// its <see cref="IDiagramTool.Diagram"/> backpointer to refer to this diagram.
    /// </remarks>
    [Category("Tools")]
    public IList<IDiagramTool> MouseUpTools { get; private set; }

    private static void OnMouseUpToolChanged<T>(DependencyObject d, DependencyPropertyChangedEventArgs e)
        where T : IDiagramTool {
      Diagram diagram = (Diagram)d;
      T oldtool = (T)e.OldValue;
      if (oldtool != null) {
        diagram.RemoveLogical(oldtool);
        oldtool.Diagram = null;
      }
      T tool = (T)e.NewValue;
      if (tool != null) {
        if (tool.Diagram != null) Diagram.Error("Cannot share tools between Diagrams: " + tool.ToString());
        tool.Diagram = diagram;
        diagram.AddLogical(tool);
      }
      if (diagram.FindMouseTool<T>(true) != null) {
        diagram.ReplaceMouseTool<T>(tool, true);  // TOOL may be null
      } else if (tool != null) {
        diagram.MouseUpTools.Add(tool);
      }
    }

    /// <summary>
    /// Identifies the <see cref="TextEditingTool"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextEditingToolProperty;
    /// <summary>
    /// Gets or sets the standard tool for in-place text editing.
    /// </summary>
    /// <value>
    /// The default value is an instance of <see cref="TextEditingTool"/>.
    /// </value>
    [Category("Tools")]
    public TextEditingTool TextEditingTool {
      get { return (TextEditingTool)GetValue(TextEditingToolProperty); }
      set { SetValue(TextEditingToolProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ClickCreatingTool"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ClickCreatingToolProperty;
    /// <summary>
    /// Gets or sets the standard mouse-up tool for inserting objects with a mouse click.
    /// </summary>
    /// <value>
    /// The default value is an instance of <see cref="ClickCreatingTool"/>.
    /// </value>
    [Category("Tools")]
    public ClickCreatingTool ClickCreatingTool {
      get { return (ClickCreatingTool)GetValue(ClickCreatingToolProperty); }
      set { SetValue(ClickCreatingToolProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ClickSelectingTool"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ClickSelectingToolProperty;
    /// <summary>
    /// Gets or sets the standard mouse-up tool for selecting objects with a mouse click.
    /// </summary>
    /// <value>
    /// The default value is an instance of <see cref="ClickSelectingTool"/>.
    /// </value>
    [Category("Tools")]
    public ClickSelectingTool ClickSelectingTool {
      get { return (ClickSelectingTool)GetValue(ClickSelectingToolProperty); }
      set { SetValue(ClickSelectingToolProperty, value); }
    }


    //  other mouse properties

    /// <summary>
    /// Identifies the <see cref="FirstMousePointInModel"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FirstMousePointInModelProperty;
    /// <summary>
    /// Gets or sets the point in model coordinates at which a mouse down event occurred.
    /// </summary>
    /// <para>
    /// Because this property is frequently set, it cannot be used effectively as a data-binding target.
    /// </para>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Point FirstMousePointInModel {
      get { return (Point)GetValue(FirstMousePointInModelProperty); }
      set { SetValue(FirstMousePointInModelProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LastMousePointInModel"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LastMousePointInModelProperty;
    /// <summary>
    /// Gets or sets the latest point in model coordinates at which any mouse event occurred.
    /// </summary>
    /// <para>
    /// Because this property is frequently set, it cannot be used effectively as a data-binding target.
    /// </para>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Point LastMousePointInModel {
      get { return (Point)GetValue(LastMousePointInModelProperty); }
      set { SetValue(LastMousePointInModelProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LastMouseEventArgs"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LastMouseEventArgsProperty;
    /// <summary>
    /// Gets or sets the <c>MouseEventArgs</c> describing the latest mouse event.
    /// </summary>
    /// <para>
    /// Because this property is frequently set, it cannot be used effectively as a data-binding target.
    /// </para>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MouseEventArgs LastMouseEventArgs {
      get { return (MouseEventArgs)GetValue(LastMouseEventArgsProperty); }
      set { SetValue(LastMouseEventArgsProperty, value); }
    }


    // keyboard event handlers

    /// <summary>
    /// On each key down event, call <see cref="IDiagramTool.DoKeyDown"/> on the <see cref="CurrentTool"/>.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e) {
      base.OnKeyDown(e);
      //if (e.Key == Key.B) this.Panel.GetPositions().Paint(this);  // for debugging
      IDiagramTool tool = this.CurrentTool;
      if (tool != null) tool.DoKeyDown(e);
    }

    /// <summary>
    /// On each key up event, call <see cref="IDiagramTool.DoKeyUp"/> on the <see cref="CurrentTool"/>.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e) {
      base.OnKeyUp(e);
      IDiagramTool tool = this.CurrentTool;
      if (tool != null) tool.DoKeyUp(e);
    }


    /// <summary>
    /// Keeps track of focus.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnGotFocus(RoutedEventArgs e) {
      base.OnGotFocus(e);
      // doesn't have keyboard focus if it's actually going to some control inside some Part
      try {
        DependencyObject x = e.OriginalSource as DependencyObject;
        while (x != null) {
          if (x == this) {
            this.IsKeyboardFocused = true;
            return;
          }
          if (x is Part) return;
          x = VisualTreeHelper.GetParent(x);
        }
      } catch (Exception) {
        // Silverlight bug: exception with navigating Hyperlink http://silverlight.codeplex.com/workitem/7696 
      }
    }

    /// <summary>
    /// Keeps track of focus.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLostFocus(RoutedEventArgs e) {
      base.OnLostFocus(e);
      this.IsKeyboardFocused = false;
    }

    // unclear if this is the same as in WPF
    internal bool IsKeyboardFocused { get; set; }


    // DraggingTool properties

    /// <summary>
    /// Identifies the <see cref="GridSnapEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GridSnapEnabledProperty;
    /// <summary>
    /// Gets or sets whether the <see cref="DraggingTool"/> snaps the location
    /// of dragged nodes.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    /// <remarks>
    /// This property controls whether the <see cref="GridSnapCellSize"/>,
    /// <see cref="GridSnapCellSpot"/> and <see cref="GridSnapOrigin"/>
    /// properties affect the <see cref="Northwoods.GoXam.Tool.DraggingTool"/>
    /// when its <see cref="Northwoods.GoXam.Tool.DraggingTool.DragOverSnapArea"/>
    /// property includes the diagram.
    /// </remarks>
    [Category("Grid")]
    public bool GridSnapEnabled {
      get { return (bool)GetValue(GridSnapEnabledProperty); }
      set { SetValue(GridSnapEnabledProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="GridSnapCellSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GridSnapCellSizeProperty;
    /// <summary>
    /// Gets or sets the size of the grid cell used when snapping during a drag
    /// if the value of <see cref="GridSnapEnabled"/> is true.
    /// </summary>
    /// <value>
    /// The default <c>Size</c> is 10x10 in model units.
    /// Any new width or height value must be positive but non-infinite numbers.
    /// </value>
    [Category("Grid")]
    public Size GridSnapCellSize {
      get { return (Size)GetValue(GridSnapCellSizeProperty); }
      set { SetValue(GridSnapCellSizeProperty, value); }
    }
    private static void OnGridSnapCellSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Size sz = (Size)e.NewValue;
      if (sz.Width <= 0 || sz.Height <= 0 || Double.IsNaN(sz.Width) || Double.IsNaN(sz.Height) || Double.IsInfinity(sz.Width) || Double.IsInfinity(sz.Height)) {
        Diagram.Error("New Size value for GridSnapCellSize must have positive non-infinite dimensions, preferably greater than one, not: " + sz.ToString());
      }
    }

    /// <summary>
    /// Identifies the <see cref="GridSnapCellSpot"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GridSnapCellSpotProperty;
    /// <summary>
    /// Gets or sets the <see cref="Spot"/> that specifies what point in the grid cell dragged parts snap to,
    /// if the value of <see cref="GridSnapEnabled"/> is true.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Spot.TopLeft"/>,
    /// which means parts get snapped directly to the grid points.
    /// A new value must be a specific spot: <see cref="Spot.IsSpot"/> must be true for any new value.
    /// </value>
    [Category("Grid")]
    public Spot GridSnapCellSpot {
      get { return (Spot)GetValue(GridSnapCellSpotProperty); }
      set { SetValue(GridSnapCellSpotProperty, value); }
    }
    private static void OnGridSnapCellSpotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Spot spot = (Spot)e.NewValue;
      if (spot.IsNoSpot) {
        Diagram.Error("New Spot value for GridSnapCellSpot must be a specific spot, not: " + spot.ToString());
      }
    }

    /// <summary>
    /// Identifies the <see cref="GridSnapOrigin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GridSnapOriginProperty;
    /// <summary>
    /// Gets or sets the snapping grid's coordinates, in model coordinates,
    /// if the value of <see cref="GridSnapEnabled"/> is true.
    /// </summary>
    /// <value>
    /// The default value is <c>Point(0, 0)</c>.
    /// Both X and Y values must be non-infinite numbers.
    /// </value>
    [Category("Grid")]
    public Point GridSnapOrigin {
      get { return (Point)GetValue(GridSnapOriginProperty); }
      set { SetValue(GridSnapOriginProperty, value); }
    }
    private static void OnGridSnapOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Point pt = (Point)e.NewValue;
      if (Double.IsNaN(pt.X) || Double.IsNaN(pt.Y) || Double.IsInfinity(pt.X) || Double.IsInfinity(pt.Y)) {
        Diagram.Error("New Point value for GridSnapOrigin must have non-infinite numbers, not: " + pt.ToString());
      }
    }


    /// <summary>
    /// Identifies the <see cref="GridVisible"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GridVisibleProperty;
    /// <summary>
    /// Gets or sets whether a background grid pattern is visible for the whole diagram.
    /// </summary>
    /// <value>
    /// The default value is false.
    /// </value>
    /// <remarks>
    /// The background grid is created if needed by calling <see cref="Northwoods.GoXam.DiagramPanel.CreateBackgroundGridPattern"/>,
    /// which uses the <see cref="GridPatternTemplate"/> or the default background grid template.
    /// </remarks>
    [Category("Grid")]
    public bool GridVisible {
      get { return (bool)GetValue(GridVisibleProperty); }
      set { SetValue(GridVisibleProperty, value); }
    }
    private static void OnGridVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      DiagramPanel panel = diagram.Panel;
      if (panel != null) panel.InvalidateGrid();
    }

    /// <summary>
    /// Identifies the <see cref="GridPattern"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GridPatternProperty;
    /// <summary>
    /// Gets or sets the <c>UIElement</c> used to render the <see cref="Northwoods.GoXam.GridPattern"/>
    /// for the whole diagram.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This may be specified in XAML.
    /// It may be created automatically from the <see cref="GridPatternTemplate"/>
    /// when <see cref="GridVisible"/> is set to true.
    /// </para>
    /// <para>
    /// Because this property is frequently set, it cannot be used effectively as a data-binding target.
    /// </para>
    /// </remarks>
    [Category("Grid")]
    public GridPattern GridPattern {
      get { return (GridPattern)GetValue(GridPatternProperty); }
      set { SetValue(GridPatternProperty, value); }
    }
    private static void OnGridPatternChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      GridPattern oldpat = (GridPattern)e.OldValue;
      if (oldpat != null) diagram.RemoveLogical(oldpat);
      GridPattern newpat = (GridPattern)e.NewValue;
      if (newpat != null) diagram.AddLogical(newpat);
      DiagramPanel panel = diagram.Panel;
      if (panel != null) panel.InvalidateGrid();
    }

    /// <summary>
    /// Identifies the <see cref="GridPatternTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GridPatternTemplateProperty;
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> used to render the <see cref="GridPattern"/>
    /// for the whole diagram.
    /// </summary>
    /// <remarks>
    /// The grid is only created if needed and made visible when <see cref="GridVisible"/> is true.
    /// </remarks>
    [Category("Grid")]
    public DataTemplate GridPatternTemplate {
      get { return (DataTemplate)GetValue(GridPatternTemplateProperty); }
      set { SetValue(GridPatternTemplateProperty, value); }
    }
    private static void OnGridPatternTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      DiagramPanel panel = diagram.Panel;
      if (panel != null) {
        diagram.GridPattern = null;
        panel.InvalidateGrid();
      }
    }


    /// <summary>
    /// Identifies the <see cref="TreePath"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TreePathProperty;
    /// <summary>
    /// Gets or sets the manner in which tree-structured diagrams are assumed to be defined.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Northwoods.GoXam.Layout.TreePath.Destination"/>,
    /// which assumes that links go from the parent node to its child nodes.
    /// If the value is <see cref="Northwoods.GoXam.Layout.TreePath.Source"/>,
    /// tree operations assume that links go from each child node to its parent node.
    /// </value>
    /// <remarks>
    /// This only affects operations such as deleting and copying nodes by the
    /// <see cref="CommandHandler"/> when <see cref="Northwoods.GoXam.CommandHandler.DeletingInclusions"/>
    /// or <see cref="Northwoods.GoXam.CommandHandler.CopyingInclusions"/> includes tree children,
    /// or when dragging nodes by the <see cref="DraggingTool"/> when
    /// <see cref="Northwoods.GoXam.Tool.DraggingTool.Inclusions"/> includes tree children.
    /// </remarks>
    [Category("Behavior")]
    public TreePath TreePath {
      get { return (TreePath)GetValue(TreePathProperty); }
      set { SetValue(TreePathProperty, value); }
    }


    // commands

    /// <summary>
    /// Identifies the <see cref="CommandHandler"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandHandlerProperty;
    /// <summary>
    /// Gets or sets the <see cref="Northwoods.GoXam.CommandHandler"/> that
    /// implements all of the standard commands.
    /// </summary>
    /// <value>
    /// The initial value is an instance of <see cref="Northwoods.GoXam.CommandHandler"/>.
    /// CommandHandlers cannot be shared by Diagrams.
    /// </value>
    [Category("Behavior")]
    public CommandHandler CommandHandler {
      get { return (CommandHandler)GetValue(CommandHandlerProperty); }
      set { SetValue(CommandHandlerProperty, value); }
    }
    private static void OnCommandHandlerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      Diagram diagram = (Diagram)d;
      CommandHandler oldhandler = e.OldValue as CommandHandler;
      if (oldhandler != null) {



        diagram.RemoveLogical(oldhandler);
        oldhandler.Diagram = null;
      }
      CommandHandler newhandler = e.NewValue as CommandHandler;
      if (newhandler != null) {
        if (newhandler.Diagram != null) Diagram.Error("Cannot share CommandHandlers between Diagrams");
        newhandler.Diagram = diagram;
        diagram.AddLogical(newhandler);



      }
    }

    internal void UpdateCommands() {
      CommandHandler ch = this.CommandHandler;
      if (ch != null) ch.UpdateCommands();
    }



    /// <summary>
    /// Identifies the <see cref="ContextMenuEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ContextMenuEnabledProperty;

    /// <summary>
    /// Gets or sets whether right mouse clicks are ignored so that a context click within the diagram's <see cref="Panel"/> can bring up a <b>ContextMenu</b>.
    /// [Silverlight only]
    /// </summary>
    /// <value>
    /// The default value of this property is false,
    /// which causes right mouse button down and up events to be treated as normal diagram mouse events passed onto the <see cref="CurrentTool"/>.
    /// When this value is true, the current tool will not receive right mouse button events,
    /// but any <b>ContextMenuService.ContextMenu</b> defined on the <see cref="Diagram"/> will be able to be shown.
    /// </value>
    /// <remarks>
    /// Silverlight 3 does not support right mouse button events at all, so this property is ignored for Silverlight 3.
    /// The value of this property has no effect on ContextMenus defined in the DataTemplates for nodes or links.
    /// </remarks>
    public bool ContextMenuEnabled {
      get { return (bool)GetValue(ContextMenuEnabledProperty); }
      set { SetValue(ContextMenuEnabledProperty, value); }
    }

    
    // routed events



    internal void RaiseEvent(DiagramEventArgs e) {
      int evt = e.RoutedEvent;
      switch (evt) {
        case TemplateAppliedEvent:
          if (this.TemplateApplied != null) this.TemplateApplied(this, e);
          break;
        case InitialLayoutCompletedEvent:
          if (this.InitialLayoutCompleted != null) this.InitialLayoutCompleted(this, e);
          break;
        case LayoutCompletedEvent:
          if (this.LayoutCompleted != null) this.LayoutCompleted(this, e);
          break;
        case ClipboardPastedEvent:
          if (this.ClipboardPasted != null) this.ClipboardPasted(this, e);
          break;
        case ExternalObjectsDroppedEvent:
          if (this.ExternalObjectsDropped != null) this.ExternalObjectsDropped(this, e);
          break;
        case LinkDrawnEvent:
          if (this.LinkDrawn != null) this.LinkDrawn(this, e);
          break;
        case LinkRelinkedEvent:
          if (this.LinkRelinked != null) this.LinkRelinked(this, e);
          break;
        case LinkReshapedEvent:
          if (this.LinkReshaped != null) this.LinkReshaped(this, e);
          break;
        case NodeCreatedEvent:
          if (this.NodeCreated != null) this.NodeCreated(this, e);
          break;
        //case NodeReshapedEvent:
        //  if (this.NodeReshaped != null) this.NodeReshaped(this, e);
        //  break;
        case NodeResizedEvent:
          if (this.NodeResized != null) this.NodeResized(this, e);
          break;
        case NodeRotatedEvent:
          if (this.NodeRotated != null) this.NodeRotated(this, e);
          break;
        case SelectionCopiedEvent:
          if (this.SelectionCopied != null) this.SelectionCopied(this, e);
          break;
        case SelectionDeletingEvent:
          if (this.SelectionDeleting != null) this.SelectionDeleting(this, e);
          break;
        case SelectionDeletedEvent:
          if (this.SelectionDeleted != null) this.SelectionDeleted(this, e);
          break;
        case SelectionMovedEvent:
          if (this.SelectionMoved != null) this.SelectionMoved(this, e);
          break;
        case SelectionGroupedEvent:
          if (this.SelectionGrouped != null) this.SelectionGrouped(this, e);
          break;
        case SelectionUngroupedEvent:
          if (this.SelectionUngrouped != null) this.SelectionUngrouped(this, e);
          break;
        case TextEditedEvent:
          if (this.TextEdited != null) this.TextEdited(this, e);
          break;
      }
    }

    internal const int InitialLayoutCompletedEvent = 1;
    internal const int LayoutCompletedEvent = 2;
    internal const int TemplateAppliedEvent = 3;
    internal const int ClipboardPastedEvent = 10;
    internal const int ExternalObjectsDroppedEvent = 11;
    internal const int LinkDrawnEvent = 20;
    internal const int LinkRelinkedEvent = 21;
    internal const int LinkReshapedEvent = 22;
    internal const int NodeCreatedEvent = 30;
    //internal const int NodeReshapedEvent = 32;
    internal const int NodeResizedEvent = 33;
    internal const int NodeRotatedEvent = 34;
    internal const int SelectionCopiedEvent = 40;
    internal const int SelectionDeletingEvent = 41;
    internal const int SelectionDeletedEvent = 42;
    internal const int SelectionMovedEvent = 43;
    internal const int SelectionGroupedEvent = 44;
    internal const int SelectionUngroupedEvent = 45;
    internal const int TextEditedEvent = 51;

    /// <summary>
    /// This event is raised by <see cref="OnApplyTemplate"/>,
    /// after the diagram's <see cref="Panel"/> has been created,
    /// to allow you to establish event handlers or bindings on the <see cref="DiagramPanel"/>.
    /// </summary>
    /// <remarks>
    /// The <c>Loaded</c> event in Silverlight occurs before the template has been expanded.
    /// It also may occur repeatedly as the Diagram is removed and then re-inserted into the visual tree.
    /// The <see cref="InitialLayoutCompleted"/> event may occur repeatedly as the diagram's model is replaced.
    /// </remarks>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> TemplateApplied;
    /// <summary>
    /// This event is raised after the first layout has been performed
    /// and the diagram bounds have been updated.
    /// </summary>
    /// <remarks>
    /// This event not only occurs during initialization, but also when the model is replaced.
    /// One-time initialization of the <see cref="DiagramPanel"/> should be done in the <see cref="TemplateApplied"/> event.
    /// </remarks>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> InitialLayoutCompleted;
    /// <summary>
    /// This event is raised after any layout has been performed
    /// and the diagram bounds have been updated.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> LayoutCompleted;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.CommandHandler.Paste"/>.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> ClipboardPasted;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.Tool.DraggingTool"/>.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> ExternalObjectsDropped;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.Tool.LinkingTool"/>.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> LinkDrawn;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.Tool.RelinkingTool"/>.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> LinkRelinked;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.Tool.LinkReshapingTool"/>.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> LinkReshaped;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.Tool.ClickCreatingTool"/>.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> NodeCreated;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.Tool.ResizingTool"/>.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> NodeResized;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.Tool.RotatingTool"/>.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> NodeRotated;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.Diagram"/>.
    /// </summary>
    [Category("Diagram")]
    public event SelectionChangedEventHandler SelectionChanged;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.Tool.DraggingTool"/>.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> SelectionCopied;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.CommandHandler.Delete"/>,
    /// before the deletion occurs.
    /// </summary>
    /// <remarks>
    /// Set the <c>Handled</c> property on the <see cref="DiagramEventArgs"/> to cancel the deletion.
    /// </remarks>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> SelectionDeleting;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.CommandHandler.Delete"/>,
    /// after the deletion occurs.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> SelectionDeleted;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.Tool.DraggingTool"/>.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> SelectionMoved;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.CommandHandler.Group"/>.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> SelectionGrouped;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.CommandHandler.Ungroup"/>.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> SelectionUngrouped;
    /// <summary>
    /// This event is raised by <see cref="Northwoods.GoXam.Tool.TextEditingTool"/>.
    /// </summary>
    [Category("Diagram")]
    public event EventHandler<DiagramEventArgs> TextEdited;














































































































































































































































































    // License and version information

    /// <summary>
    /// This static/shared property holds the runtime license key that permits distribution
    /// of applications using this control without displaying a licensing watermark.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This should always be set before any <see cref="Diagram"/> or <see cref="DiagramPanel"/> is created.
    /// </para>
    /// <para>
    /// For more details, read the GoXamIntro documentation.
    /// </para>
    /// </remarks>
    public static String LicenseKey {
      get { return _LicenseKey; }
      set {
        if (_LicenseKey != value) {
          String old = _LicenseKey;
          _LicenseKey = value;
          if (old != null) Diagram.Trace("Caution: resetting Diagram.LicenseKey from:\n  " + old + "\n  to:\n  " + _LicenseKey ?? "(null)");
        }
      }
    }
    private static String _LicenseKey;

    /// <summary>
    /// This property returns the Diagram software version, as a string.
    /// </summary>
    public static String VersionName {  // major.minor.baselevel  MAY have extra info, e.g. "9.8.7 beta2"
      get { return "1.2.6"; }  //??? also update AssemblyVersion assembly attribute in AssemblyInfo.cs
    }
  }  // end of Diagram
}
