
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Northwoods.GoXam.Tool {

  /// <summary>
  /// The <c>DragSelectingTool</c> lets the user select multiple parts within
  /// a rectangular area.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This is a standard mouse-move tool.
  /// </para>
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
  /// <para>
  /// This tool does not utilize any <see cref="Adornment"/>s or tool handles.
  /// </para>
  /// <para>
  /// This tool does not edit the model.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class DragSelectingTool : DiagramTool {

    static DragSelectingTool() {
      BoxTemplateProperty = DependencyProperty.Register("BoxTemplate", typeof(DataTemplate), typeof(DragSelectingTool), new FrameworkPropertyMetadata(null));
      IncludeProperty = DependencyProperty.Register("SearchInclusion", typeof(SearchInclusion), typeof(DragSelectingTool), new FrameworkPropertyMetadata(SearchInclusion.Inside));
    }

    /// <summary>
    /// Identifies the <see cref="BoxTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BoxTemplateProperty;
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> that renders the "rubber-band" box
    /// that the user draws to specify the selection area.
    /// </summary>
    /// <value>
    /// By default this is null, which causes <see cref="CreateBox"/>
    /// to use a default template.
    /// </value>
    public DataTemplate BoxTemplate {
      get { return (DataTemplate)GetValue(BoxTemplateProperty); }
      set { SetValue(BoxTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Include"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IncludeProperty;
    /// <summary>
    /// Gets or sets the conditions under which parts are selected.
    /// </summary>
    /// <value>
    /// The default value is <see cref="Northwoods.GoXam.SearchInclusion.Inside"/> --
    /// to be selected the part must be wholly inside the selection box.
    /// </value>
    public SearchInclusion Include {
      get { return (SearchInclusion)GetValue(IncludeProperty); }
      set { SetValue(IncludeProperty, value); }
    }


    /// <summary>
    /// Gets or sets the temporary node acting as the "rubber-band" box
    /// that the user is stretching with a mouse drag.
    /// </summary>
    /// <value>
    /// This is set by <see cref="DoActivate"/> to the node produced by <see cref="CreateBox"/>.
    /// </value>
    protected Node Box { get; set; }


    /// <summary>
    /// This tool can run when the diagram allows selection,
    /// there has been a mouse-drag (far enough away not to be a click),
    /// and there is no selectable part at the mouse-down point.
    /// </summary>
    /// <returns></returns>
    public override bool CanStart() {
      if (!base.CanStart()) return false;

      Diagram diagram = this.Diagram;
      if (diagram == null || !diagram.AllowSelect) return false;

      // require left button & that it has moved far enough away from the mouse down point, so it isn't a click
      if (!IsLeftButtonDown()) return false;

      // don't include the following check when this tool is running modally
      if (diagram.CurrentTool != this) {
        // mouse needs to have moved from the mouse-down point
        if (!IsBeyondDragSize()) return false;

        // don't start if we're over a selectable part
        Part part = FindPartAt(diagram.FirstMousePointInModel, true);
        if (part != null) return false;
      }
      return true;
    }

    /// <summary>
    /// Capture the mouse when starting this tool.
    /// </summary>
    public override void DoStart() {
      CaptureMouse();
    }

    /// <summary>
    /// Call <see cref="CreateBox"/> to create the "rubber-band" box,
    /// remember it as the <see cref="Box"/> property,
    /// and add it to the diagram's <see cref="Northwoods.GoXam.Model.PartsModel"/>.
    /// </summary>
    public override void DoActivate() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      this.Box = CreateBox();
      diagram.PartsModel.SkipsUndoManager = true;
      diagram.PartsModel.AddNode(this.Box);
      this.Active = true;
    }

    /// <summary>
    /// Create a <see cref="Node"/> using <see cref="BoxTemplate"/> as its <c>DataTemplate</c>.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// If <see cref="BoxTemplate"/> is null, this uses the default template
    /// named "DefaultDragSelectingBoxTemplate".
    /// </remarks>
    protected virtual Node CreateBox() {
      Node box = new Node();  // for DragSelectingTool.CreateBox
      box.Id = "DragSelectingBox";
      DataTemplate template = this.BoxTemplate;
      if (template == null) template = Diagram.FindDefault<DataTemplate>("DefaultDragSelectingBoxTemplate");
      box.ContentTemplate = template;  // but not bound to data, so no .Content
      return box;
    }

    /// <summary>
    /// Cleanup any <see cref="Box"/>.
    /// </summary>
    public override void DoDeactivate() {
      Diagram diagram = this.Diagram;
      if (diagram != null && this.Box != null) {
        diagram.PartsModel.RemoveNode(this.Box);
        diagram.PartsModel.SkipsUndoManager = false;
        this.Box = null;
      }
      this.Active = false;
    }

    /// <summary>
    /// Release the mouse capture when stopping this tool.
    /// </summary>
    public override void DoStop() {
      ReleaseMouse();
    }

    /// <summary>
    /// While dragging the mouse, position and size the <see cref="Box"/>
    /// according to the bounds produced by <see cref="ComputeBoxBounds"/>.
    /// </summary>
    public override void DoMouseMove() {
      if (this.Active && this.Box != null) {
        Rect r = ComputeBoxBounds();
        FrameworkElement rect = this.Box.VisualElement;
        rect.Width = r.Width;
        rect.Height = r.Height;
        this.Box.Position = new Point(r.X, r.Y);
        this.Box.Remeasure();
      }
    }

    /// <summary>
    /// Upon the mouse-up, call <see cref="SelectInRect"/>
    /// with the value of <see cref="ComputeBoxBounds"/>
    /// and stop this tool.
    /// </summary>
    public override void DoMouseUp() {
      Diagram diagram = this.Diagram;
      if (this.Active && diagram != null) {
        try {
          diagram.Cursor = Cursors.Wait;
          SelectInRect(ComputeBoxBounds());
        } finally {
          diagram.Cursor = null;
        }
      }
      StopTool();
    }

    /// <summary>
    /// This just returns a <c>Rect</c> stretching from the
    /// mouse-down point to the current mouse point.
    /// </summary>
    /// <returns></returns>
    protected virtual Rect ComputeBoxBounds() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return new Rect(0, 0, 0, 0);
      return new Rect(diagram.FirstMousePointInModel, diagram.LastMousePointInModel);
    }

    /// <summary>
    /// This method is called to select some parts, governed by the value of <see cref="Include"/>.
    /// </summary>
    /// <param name="r"></param>
    /// <remarks>
    /// The normal behavior is to set the diagram's selection collection to only those parts
    /// in the given rectangle <paramref name="r"/> according to the <see cref="Include"/> policy.
    /// However, if the Shift key modifier was used, no parts are deselected --
    /// this adds to the selection the parts in the rectangle not already selected.
    /// If the Control key modifier was used, this toggles the selectedness of the parts in the rectangle.
    /// If the Control key and Shift key modifiers were both used, this deselects the parts in the rectangle.
    /// </remarks>
    public virtual void SelectInRect(Rect r) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      
      // choose the selectable Parts within the rectangle
      HashSet<Part> parts = new HashSet<Part>(diagram.Panel.FindPartsIn<Part>(r, SearchFlags.SelectableParts, this.Include, SearchLayers.Parts));

      if (IsControlKeyDown()) {  // toggle or deselect
        if (IsShiftKeyDown()) {  // deselect only
          foreach (Part p in parts) {
            if (p.IsSelected) p.IsSelected = false;
          }
        } else {  // toggle selectedness of parts
          foreach (Part p in parts) {
            p.IsSelected = !p.IsSelected;
          }
        }
      } else if (IsShiftKeyDown()) {  // extend selection only
        foreach (Part p in parts) {
          if (!p.IsSelected) p.IsSelected = true;
        }
      } else {  // select parts, and unselect all other previously selected parts
        // this tries to avoid deselecting and then reselecting any Part
        List<Part> tounselect = new List<Part>();
        foreach (Part p in diagram.SelectedParts) {
          if (!parts.Contains(p)) tounselect.Add(p);
        }
        foreach (Part p in tounselect) {
          p.IsSelected = false;
        }
        foreach (Part p in parts) {
          if (!p.IsSelected) p.IsSelected = true;
        }
      }
    }

  }
}
