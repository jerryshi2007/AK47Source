
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
using System.Windows;
using System.Windows.Input;

namespace Northwoods.GoXam.Tool {

  /// <summary>
  /// This abstract tool class is the standard base class for all of the predefined tools.
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
  public abstract class DiagramTool : FrameworkElement, IDiagramTool {

    static DiagramTool() {
      MouseEnabledProperty = DependencyProperty.Register("MouseEnabled", typeof(bool), typeof(DiagramTool),
        new FrameworkPropertyMetadata(true));

      WheelBehaviorProperty = DependencyProperty.Register("WheelBehavior", typeof(WheelBehavior), typeof(DiagramTool),
        new FrameworkPropertyMetadata(WheelBehavior.Standard));
    }

    /// <summary>
    /// Gets the <see cref="Northwoods.GoXam.Diagram"/> that owns this tool and
    /// for which this tool is handling input events.
    /// </summary>
    /// <value>
    /// This property is set automatically by the <see cref="Northwoods.GoXam.Diagram"/>
    /// properties and methods that manage tools.
    /// You should not need to set this property.
    /// </value>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Diagram Diagram { get; set; }


    /// <summary>
    /// Identifies the <see cref="MouseEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MouseEnabledProperty;
    /// <summary>
    /// Gets or sets whether this tool can be started by a mouse event.
    /// </summary>
    /// <value>
    /// The default value is true.
    /// Set this to false to prevent <see cref="CanStart"/> from returning true.
    /// </value>
    /// <remarks>
    /// <para>
    /// Setting this property to false should prevent this tool from being used in a mode-less fashion
    /// with a mouse down/move/up event.
    /// However, even when this property is false, this tool can still be used in a model fashion:
    /// this tool can still be started by explicitly setting the
    /// <see cref="Northwoods.GoXam.Diagram.CurrentTool"/> property to this tool.
    /// </para>
    /// <para>
    /// This property is also useful in Silverlight 3, where there is no <c>&lt;x:Null/&gt;</c> XAML element
    /// by which to set a <see cref="Northwoods.GoXam.Diagram"/> tool to null in XAML.
    /// Instead you can just set this property to false.
    /// </para>
    /// </remarks>
    public bool MouseEnabled {
      get { return (bool)GetValue(MouseEnabledProperty); }
      set { SetValue(MouseEnabledProperty, value); }
    }

    
    /// <summary>
    /// Identifies the <see cref="WheelBehavior"/> property.
    /// </summary>
    public static readonly DependencyProperty WheelBehaviorProperty;
    /// <summary>
    /// Gets or sets the behavior of <see cref="StandardMouseWheel"/>.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Northwoods.GoXam.Tool.WheelBehavior.Standard"/>.
    /// That value is a combination of three enumerated values:
    /// <see cref="Northwoods.GoXam.Tool.WheelBehavior.ScrollsVertically"/> |
    /// <see cref="Northwoods.GoXam.Tool.WheelBehavior.ShiftScrollsHorizontally"/> |
    /// <see cref="Northwoods.GoXam.Tool.WheelBehavior.ControlZooms"/>.
    /// </value>
    public WheelBehavior WheelBehavior {
      get { return (WheelBehavior)GetValue(WheelBehaviorProperty); }
      set { SetValue(WheelBehaviorProperty, value); }
    }

    /// <summary>
    /// The diagram asks each tool to update any adornments the tool might use for a given part.
    /// </summary>
    /// <param name="part"></param>
    /// <remarks>
    /// If the tool uses its own tool handles (<see cref="Northwoods.GoXam.Adornment"/>s),
    /// this should display them or hide them as appropriate.
    /// Typically this should only show them if the part is selected.
    /// </remarks>
    public virtual void UpdateAdornments(Part part) { }


    // States and state transitions

    /// <summary>
    /// Gets or sets whether this tool is started and is actively doing something.
    /// </summary>
    /// <remarks>
    /// You can set this to true after your tool is started (i.e. when it is the
    /// <see cref="Northwoods.GoXam.Diagram.CurrentTool"/> and <see cref="DoStart"/>
    /// had been called), but when it is not yet in a state
    /// that it is actually "doing" something, because it is waiting for the right
    /// circumstances.  This is typically only important when the tool is used in
    /// a modal fashion.
    /// </remarks>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Active { get; set; }


    /// <summary>
    /// This predicate is used by the diagram to decide if this tool can be started.
    /// </summary>
    /// <returns>true if <see cref="MouseEnabled"/> is true and
    /// if the <see cref="ToolManager"/> can make this tool the current one and
    /// then call the <see cref="DoStart"/> method</returns>
    /// <remarks>
    /// Overrides of this method should call the base method: if that returns false, the override should return false.
    /// </remarks>
    public virtual bool CanStart() { 
      return this.MouseEnabled;
    }

    /// <summary>
    /// This method is called by the diagram when this tool becomes the current tool.
    /// </summary>
    /// <remarks>
    /// Tool implementations should perform their per-use initialization here, such
    /// as setting up internal data structures, or capturing the mouse.
    /// </remarks>
    public virtual void DoStart() { }

    /// <summary>
    /// This method is called by the diagram after setting <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>,
    /// to make the new tool active.
    /// </summary>
    /// <remarks>
    /// This should set <see cref="Active"/> to true.
    /// This might call <see cref="Northwoods.GoXam.Tool.DiagramTool.StartTransaction"/>,
    /// if this tool's activity involves modification of the model.
    /// </remarks>
    public virtual void DoActivate() {
      this.Active = true;
    }

    /// <summary>
    /// This method is called by the diagram on the old tool when
    /// <see cref="Northwoods.GoXam.Diagram.CurrentTool"/> is set to a new tool.
    /// </summary>
    /// <remarks>
    /// This needs to set <see cref="Active"/> to false.
    /// This might call <see cref="Northwoods.GoXam.Tool.DiagramTool.StopTransaction"/>,
    /// if this tool's activity involves modification of the model.
    /// </remarks>
    public virtual void DoDeactivate() {
      this.Active = false;
    }

    /// <summary>
    /// This method is called by the diagram when this tool stops being the current tool.
    /// </summary>
    /// <remarks>
    /// Tool implementations should perform their per-use cleanup here,
    /// such as releasing mouse capture.
    /// </remarks>
    public virtual void DoStop() { }

    /// <summary>
    /// The diagram will call this method when the user wishes to cancel the
    /// current tool's operation.
    /// </summary>
    /// <remarks>
    /// Typically this is called when the user hits the ESCAPE key.
    /// This should restore the original state and then call
    /// <see cref="StopTool"/>.
    /// </remarks>
    public virtual void DoCancel() {
      StopTool();
    }

    /// <summary>
    /// If the <see cref="Northwoods.GoXam.Diagram.CurrentTool"/> is this tool,
    /// stop this tool and start the <see cref="Northwoods.GoXam.Diagram.DefaultTool"/>
    /// by making it be the new current tool.
    /// </summary>
    public void StopTool() {
      Diagram diagram = this.Diagram;
      if (diagram != null && diagram.CurrentTool == this) {
        // this will call DoStop on the current tool,
        // and then make the DefaultTool current and call DoStart on it
        diagram.CurrentTool = null;  // NOT diagram.DefaultTool, otherwise OnCurrentToolChanged might not be called!
      }
    }


    // Input events

    /// <summary>
    /// The diagram will call this method upon a mouse down event.
    /// </summary>
    /// <remarks>
    /// This is normally overridden for mouse-down tools;
    /// it is not called for mouse-move or mouse-up tools.
    /// However it may also be called when the tool is run in a modal fashion,
    /// when code explicitly sets the diagram's <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>.
    /// </remarks>
    public virtual void DoMouseDown() {
      if (!this.Active && CanStart()) {
        DoActivate();
      }
    }

    /// <summary>
    /// The diagram will call this method upon a mouse move event.
    /// </summary>
    /// <remarks>
    /// This is normally overridden for mouse-move tools;
    /// it is not called for mouse-up tools.
    /// However it may also be called when the tool is run in a modal fashion,
    /// when code explicitly sets the diagram's <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>.
    /// An override of this method usually does nothing when <see cref="Active"/> is false.
    /// </remarks>
    public virtual void DoMouseMove() { }

    /// <summary>
    /// The diagram will call this method upon a mouse up event.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is normally overridden for mouse-up tools.
    /// An override of this method usually does nothing when <see cref="Active"/> is false,
    /// except for calling <see cref="StopTool"/>.
    /// </para>
    /// <para>
    /// Tools normally stop upon a mouse up, by calling <see cref="StopTool"/>.
    /// If you want to handle multiple mouse down-up gestures in one tool activation,
    /// you will need to override this method to only stop the tool when you want.
    /// </para>
    /// </remarks>
    public virtual void DoMouseUp() {
      StopTool();
    }

    /// <summary>
    /// The diagram will call this method as the mouse wheel is rotated.
    /// </summary>
    public virtual void DoMouseWheel() { }


    /// <summary>
    /// The diagram will call this method upon a key down event.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default this just calls <see cref="DoCancel"/> if the key is the ESCAPE key.
    /// </para>
    /// <para>
    /// In WPF this key event method isn't called because of the command mechanism.
    /// If you want this method to be called, override <see cref="DoStart"/> to call
    /// <c>this.Diagram.CommandHandler.RemoveStandardBindings()</c>
    /// and override <see cref="DoStop"/> to call
    /// <c>this.Diagram.CommandHandler.AddStandardBindings()</c>.
    /// In Silverlight there are no commands, so this method is always called.
    /// </para>
    /// </remarks>
    public virtual void DoKeyDown(KeyEventArgs e) {
      if (e != null && e.Key == Key.Escape) {
        DoCancel();
      }
    }

    /// <summary>
    /// The diagram will call this method upon a key up event.
    /// </summary>
    /// <remarks>
    /// In WPF this key event method isn't called due to the command mechanism.
    /// In Silverlight there are no commands, so this method is always called.
    /// </remarks>
    public virtual void DoKeyUp(KeyEventArgs e) { }


    // Transactions

    /// <summary>
    /// Gets or sets the name of the transaction to be committed; if null, the transaction will be rolled back.
    /// </summary>
    /// <value>
    /// The default value is null; <see cref="StartTransaction"/> will also set this to null.
    /// </value>
    /// <remarks>
    /// If this is non-null at the time of a call to <see cref="StopTransaction"/>,
    /// this results in a call to <see cref="Northwoods.GoXam.Diagram.CommitTransaction"/> with this transaction name;
    /// if this is null at that time, this results in a call to <see cref="Northwoods.GoXam.Diagram.RollbackTransaction"/>.
    /// </remarks>
    protected String TransactionResult { get; set; }

    /// <summary>
    /// Call <see cref="Northwoods.GoXam.Diagram.StartTransaction"/> with the given transaction name.
    /// </summary>
    /// <param name="tname"></param>
    /// <returns></returns>
    /// <remarks>
    /// This is normally called in an override of <see cref="DoActivate"/>, if the tool modifies the model,
    /// along with a call to <see cref="StopTransaction"/> in an override of <see cref="DoDeactivate"/>.
    /// </remarks>
    protected virtual bool StartTransaction(String tname) {
      this.TransactionResult = null;
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      return diagram.StartTransaction(tname);
    }

    /// <summary>
    /// If <see cref="TransactionResult"/> is null, call <see cref="Northwoods.GoXam.Diagram.RollbackTransaction"/>,
    /// otherwise call <see cref="Northwoods.GoXam.Diagram.CommitTransaction"/>.
    /// </summary>
    /// <returns>the result of the call to rollback or commit the transaction</returns>
    /// <remarks>
    /// This is normally called in an override of <see cref="DoDeactivate"/>,
    /// if <see cref="StartTransaction"/> was called in <see cref="DoActivate"/>.
    /// </remarks>
    protected virtual bool StopTransaction() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      if (this.TransactionResult == null)
        return diagram.RollbackTransaction();
      else
        return diagram.CommitTransaction(this.TransactionResult);
    }


    // Useful methods

    /// <summary>
    /// Implement the standard behavior for mouse wheel events.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If <see cref="Northwoods.GoXam.Diagram.AllowScroll"/> is true,
    /// turning the mouse wheel causes the diagram to scroll up or down.
    /// If <see cref="IsShiftKeyDown"/> is also true,
    /// the diagram scrolls left or right.
    /// </para>
    /// <para>
    /// If <see cref="IsControlKeyDown"/> is true and if
    /// <see cref="Northwoods.GoXam.Diagram.AllowZoom"/> is true,
    /// turning the mouse wheel changes the diagram's scale,
    /// zooming in or out while trying to keep the point in the model
    /// at the same point as the mouse.
    /// </para>
    /// </remarks>
    protected virtual void StandardMouseWheel() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;
      int delta = GetWheelDelta();
      if (delta == 0) return;
      MouseWheelEventArgs e = diagram.LastMouseEventArgs as MouseWheelEventArgs;
      WheelBehavior behave = this.WheelBehavior;
      bool shift = IsShiftKeyDown();
      bool control = IsControlKeyDown();
      if (((behave & WheelBehavior.ControlZooms) == WheelBehavior.ControlZooms
           && !shift && control && diagram.AllowZoom) ||
          ((behave & WheelBehavior.Zooms) == WheelBehavior.Zooms
           && !shift && !control && diagram.AllowZoom)) {
        // zoom in or out at current mouse point
        Point oldzoom = panel.ZoomPoint;
        panel.ZoomPoint = panel.TransformModelToView(diagram.LastMousePointInModel);
        CommandHandler cmd = diagram.CommandHandler;
        if (cmd != null) {
          if (delta > 0)
            cmd.IncreaseZoom(null);
          else
            cmd.DecreaseZoom(null);
        } else {
          if (delta > 0)
            panel.Scale *= 1.05;
          else
            panel.Scale /= 1.05;
        }
        panel.ZoomPoint = oldzoom;
        if (e != null) e.Handled = true;
      } else if ((behave & WheelBehavior.ScrollsVertically) == WheelBehavior.ScrollsVertically
                 && !shift && !control && diagram.AllowScroll) {
        // scroll up or down
        if (delta > 0)
          panel.MouseWheelUp();
        else
          panel.MouseWheelDown();
        if (e != null) e.Handled = true;
      } else if ((behave & WheelBehavior.ShiftScrollsHorizontally) == WheelBehavior.ShiftScrollsHorizontally
                 && shift && !control && diagram.AllowScroll) {
        // scroll left or right
        if (delta > 0)
          panel.MouseWheelLeft();
        else
          panel.MouseWheelRight();
        if (e != null) e.Handled = true;
      }
    }


    /// <summary>
    /// Implement the standard behavior for selecting parts with the mouse, depending on the control and shift modifier keys.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Control-clicking on a part will select it if it wasn't already, and will deselect if it had been selected.
    /// Shift-clicking on a part will add it to the selection (if it isn't already).
    /// Otherwise, clicking on a part will select it (if it isn't already).
    /// </para>
    /// <para>
    /// Note that there are restrictions on selection.
    /// For example, a part cannot be selected in this manner if <see cref="Part.Selectable"/> is false,
    /// or if <see cref="Northwoods.GoXam.Diagram.MaximumSelectionCount"/> would be exceeded.
    /// </para>
    /// <para>
    /// A left click in the background of the diagram with no modifier keys clears the selection.
    /// </para>
    /// </remarks>
    protected virtual void StandardMouseSelect() {
      Diagram diagram = this.Diagram;
      if (diagram == null || !diagram.AllowSelect) return;
      Part currentPart = FindPartAt(diagram.LastMousePointInModel, true);

      bool control = IsControlKeyDown();
      bool shift = IsShiftKeyDown();
      bool left = IsLeftButtonDown();
      if (currentPart != null) {
        if (control) {  // toggle the part's selection
          currentPart.IsSelected = !currentPart.IsSelected;
        } else if (shift) {  // add the part to the selection
          if (!currentPart.IsSelected) {
            currentPart.IsSelected = true;
          }
        } else {
          if (!currentPart.IsSelected) {
            diagram.Select(currentPart);
          }
        }
      } else if (left && !control && !shift) {  // left click on background with no modifier: clear selection
        diagram.ClearSelection();
      }
    }


    /// <summary>
    /// This calls <c>UIElement.CaptureMouse</c> on the <see cref="Diagram"/>'s <see cref="Northwoods.GoXam.Diagram.Panel"/>.
    /// </summary>
    new protected virtual void CaptureMouse() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;
      panel.CaptureMouse();
    }

    /// <summary>
    /// This calls <c>UIElement.ReleaseMouseCapture</c> on the <see cref="Diagram"/>'s <see cref="Northwoods.GoXam.Diagram.Panel"/>.
    /// </summary>
    protected virtual void ReleaseMouse() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return;
      panel.ReleaseMouse();
    }


    /// <summary>
    /// Find a <see cref="Part"/> at the given point, perhaps requiring it to be <see cref="Part.Selectable"/>.
    /// </summary>
    /// <param name="p">a <c>Point</c> in model coordinates</param>
    /// <param name="selectable">if true, the <see cref="Part"/> found must be selectable; otherwise any Part will do</param>
    /// <returns>
    /// the first (frontmost) <see cref="Part"/> at the location <paramref name="p"/>;
    /// the part could be a <see cref="Node"/> or a <see cref="Link"/>,
    /// including temporary ones such as <see cref="Adornment"/>s.
    /// </returns>
    protected virtual Part FindPartAt(Point p, bool selectable) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      if (selectable)
        return diagram.Panel.FindElementAt<Part>(p, Diagram.FindAncestor<Part>, x => x.CanSelect(), SearchLayers.All);
      else
        return diagram.Panel.FindElementAt<Part>(p, Diagram.FindAncestor<Part>, null, SearchLayers.All);
    }

    internal static FrameworkElement AsToolHandle(FrameworkElement elt) {
      if (NodePanel.GetFigure(elt) != NodeFigure.None)
        return elt;
      else
        return null;
    }

    internal static FrameworkElement FindToolHandle(DependencyObject v) {
      if (v == null || v is Part) return null;
      if (NodePanel.GetFigure(v) != NodeFigure.None) return v as FrameworkElement;
      return FindToolHandle(System.Windows.Media.VisualTreeHelper.GetParent(v));
    }

    /// <summary>
    /// Find an element at the given point that is a tool handle.
    /// </summary>
    /// <param name="p">a <c>Point</c> in model coordinates</param>
    /// <param name="category">the handle's <see cref="Adornment"/>'s <see cref="Part.Category"/></param>
    /// <returns></returns>
    /// <remarks>
    /// A tool handle is an element that is part of an <see cref="Adornment"/> that is used by a tool.
    /// Typically it is a <c>ToolHandle</c> in WPF or a <c>Path</c> in Silverlight,
    /// but may be any FrameworkElement with the <c>go:NodePanel.Figure</c> attached property set to value other than <c>None</c>.
    /// This method checks that the adornment's <see cref="Part.Category"/> matches the given <paramref name="category"/>.
    /// </remarks>
    protected virtual FrameworkElement FindToolHandleAt(Point p, String category) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      FrameworkElement handle = diagram.Panel.FindElementAt<FrameworkElement>(p, FindToolHandle, x => true, SearchLayers.Temporary | SearchLayers.Nodes);
      if (handle != null) {
        Adornment ad = FindAdornment(handle);
        if (ad == null || ad.Category != category) return null;
        return handle;
      }
      return null;
    }

    /// <summary>
    /// Search up the chain of parent visual elements starting with the given element
    /// to find one for which <paramref name="pred"/> is true
    /// </summary>
    /// <param name="elt">a <c>FrameworkElement</c></param>
    /// <param name="pred">
    /// When this predicate is true for an element, return that element.
    /// When this predicate is false for an element, continue up the visual parent chain.
    /// Stop going up the visual parent chain when reaching the <see cref="Part"/>.
    /// </param>
    /// <returns>null if no such element is found</returns>
    protected virtual FrameworkElement FindElementUpFrom(FrameworkElement elt, Predicate<FrameworkElement> pred) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      while (elt != null) {
        if (elt is Part) return null;
        if (pred != null && pred(elt)) return elt;
        // if predicate is false, continue searching up the parent chain
        elt = Diagram.FindParent<FrameworkElement>(elt);
      }
      return null;
    }

    /// <summary>
    /// Search up the chain of visual parent elements starting with the given element
    /// to find one that has <paramref name="pred"/> true and <paramref name="valid"/> true.
    /// </summary>
    /// <param name="elt">a <c>FrameworkElement</c></param>
    /// <param name="pred">
    /// When this predicate and <paramref name="valid"/> are both true, return that element,
    /// When this predicate is false, return null.
    /// Otherwise continue up the parent chain.
    /// Stop going up the visual parent chain when reaching the <see cref="Part"/>.
    /// </param>
    /// <param name="valid">a predicate that must be true for an element for it to be returned by this method</param>
    /// <returns></returns>
    protected virtual FrameworkElement FindElementUpFrom(FrameworkElement elt, Func<FrameworkElement, bool?> pred, Predicate<FrameworkElement> valid) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return null;
      while (elt != null) {
        if (elt is Part) return null;
        bool? predicate = (pred != null ? pred(elt) : true);
        if (predicate == true && valid != null && valid(elt)) return elt;
        if (predicate == false) return null;
        // if predicate is null, continue searching up the parent chain
        elt = Diagram.FindParent<FrameworkElement>(elt);
      }
      return null;
    }

    /// <summary>
    /// Given a handle element, return its containing <see cref="Adornment"/>.
    /// </summary>
    /// <param name="handle">a <c>FrameworkElement</c></param>
    /// <returns>
    /// the containing <see cref="Adornment"/> (which might be several visual levels up)
    /// or null if the element does not belong to an <see cref="Adornment"/>
    /// </returns>
    protected Adornment FindAdornment(FrameworkElement handle) {
      return Diagram.FindAncestor<Adornment>(handle);  // find the placeholder adornment (a Node)
    }

    /// <summary>
    /// Given a handle element in an adornment, return its adornment's <see cref="Adornment.AdornedElement"/>.
    /// </summary>
    /// <param name="handle">a <c>FrameworkElement</c></param>
    /// <returns>a <c>FrameworkElement</c> inside the adorned part, not inside this adornment</returns>
    protected FrameworkElement FindAdornedElement(FrameworkElement handle) {
      Adornment ad = FindAdornment(handle);
      if (ad != null) return ad.AdornedElement;
      return null;
    }

    /// <summary>
    /// Given a handle element in an adornment, return its adornment's <see cref="Adornment.AdornedPart"/>.
    /// </summary>
    /// <param name="handle">a <c>FrameworkElement</c></param>
    /// <returns>a <see cref="Part"/> that is not an adornment</returns>
    protected Part FindAdornedPart(FrameworkElement handle) {
      Adornment ad = FindAdornment(handle);
      if (ad != null) return ad.AdornedPart;
      return null;
    }


    /// <summary>
    /// Return true when the left mouse button is pressed during a mouse button event.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsLeftButtonDown() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return false;
      return panel.IsLeftButtonDown();
    }

    /// <summary>
    /// Return true when the right mouse button is pressed during a mouse button event.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsRightButtonDown() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return false;
      return panel.IsRightButtonDown();
    }


    /// <summary>
    /// Return true when the control key modifier is pressed.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsControlKeyDown() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return false;
      return panel.IsControlKeyDown();
    }

    /// <summary>
    /// Return true when the shift key modifier is pressed.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsShiftKeyDown() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return false;
      return panel.IsShiftKeyDown();
    }

    /// <summary>
    /// Return true when the alt key modifier is pressed.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsAltKeyDown() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return false;
      return panel.IsAltKeyDown();
    }

    /// <summary>
    /// Return true when the last mouse point is far enough away from the first mouse down point
    /// to constitute a drag operation instead of just a potential click.
    /// </summary>
    /// <returns>true if the first and last mouse points are more than two pixels apart in either axis</returns>
    protected virtual bool IsBeyondDragSize() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return false;
      // Get all of the points in screen/view coordinates
      Point first = panel.TransformModelToView(diagram.FirstMousePointInModel);
      Point last = panel.TransformModelToView(diagram.LastMousePointInModel);
      return DiagramPanel.IsBeyondDragSize(last, first);
    }

    /// <summary>
    /// Return true when the last mouse down event occurred very close to and very soon after the previous mouse down event.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsDoubleClick() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return false;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return false;
      return panel.IsDoubleClick();
    }

    /// <summary>
    /// Return how much the wheel has turned.
    /// </summary>
    /// <returns>
    /// If the last mouse event was the mouse wheel turning forward, this returns a positive integer.
    /// If the last mouse event was the mouse wheel turning backward, this returns a negative integer.
    /// Otherwise this returns zero.
    /// </returns>
    protected virtual int GetWheelDelta() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return 0;
      DiagramPanel panel = diagram.Panel;
      if (panel == null) return 0;
      return panel.GetWheelDelta();
    }



    internal bool RaiseEvent(int evt, DiagramEventArgs args)









    {
      if (this.Diagram != null) {
        if (args == null) args = new DiagramEventArgs();
        args.RoutedEvent = evt;
        this.Diagram.RaiseEvent(args);
        return args.Handled;
      }
      return false;
    }
  }


  /// <summary>
  /// This enumeration controls the behavior of the mouse wheel by <see cref="DiagramTool.StandardMouseWheel"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// These enumerated values can be combined as flags.
  /// However, when conflicting flags are combined, the behavior is undetermined.
  /// Flags conflict when they specify different behaviors for the same combination of modifiers:
  /// Control or Shift or Alt (or no modifiers at all).
  /// </para>
  /// <para>
  /// At the current time, the only conflicting flags are for unmodified mouse wheel events:
  /// <see cref="ScrollsVertically"/> or <see cref="Zooms"/>.
  /// </para>
  /// </remarks>
  [Flags]
  public enum WheelBehavior {
    /// <summary>
    /// When <see cref="DiagramTool.WheelBehavior"/> is "None", mouse wheel events are ignored.
    /// </summary>
    None = 0,

    /// <summary>
    /// When there is no modifier, mouse wheel events scroll the diagram up and down.
    /// </summary>
    ScrollsVertically =        0x0101,
    /// <summary>
    /// When there is no modifier, mouse wheel events zoom in and out.
    /// </summary>
    Zooms =                    0x0102,

    /// <summary>
    /// When there is only a Control key modifier, mouse wheel events zoom in and out.
    /// </summary>
    ControlZooms =             0x0201,

    /// <summary>
    /// When there is only a Shift key modifier, mouse wheel events scroll the diagram left and right.
    /// </summary>
    ShiftScrollsHorizontally = 0x0401,

    // ControlShift... = 0x0801,

    /// <summary>
    /// This combination of flags represents the default value for <see cref="DiagramTool.WheelBehavior"/>:
    /// <see cref="ScrollsVertically"/>, <see cref="ControlZooms"/>, and <see cref="ShiftScrollsHorizontally"/>.
    /// </summary>
    Standard = ScrollsVertically | ControlZooms | ShiftScrollsHorizontally,
  }
}
