
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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Northwoods.GoXam.Tool {

  /// <summary>
  /// The <c>DragZoomingTool</c> lets the user zoom into a diagram by stretching a box
  /// to indicate the new contents of the diagram's viewport (the area of the
  /// model shown by the <see cref="DiagramPanel"/>).
  /// </summary>
  /// <remarks>
  /// <para>
  /// The diagram that is zoomed by this tool is specified by the <see cref="ZoomedDiagram"/> property.
  /// If the value is null, the tool zooms its own <see cref="DiagramTool.Diagram"/>.
  /// </para>
  /// <para>
  /// Although this is a mouse-move tool class handling a mouse-drag in the background, and there is a
  /// <see cref="Northwoods.GoXam.Diagram.DragZoomingTool"/> property,
  /// no such tool is installed initially -- that property is null.
  /// You can install this tool by creating an instance of it and setting that diagram property.
  /// However, although it <see cref="CanStart"/>, the <see cref="DragSelectingTool"/> and
  /// the <see cref="PanningTool"/> are two other background-mouse-drag tools that are normally installed
  /// in a <see cref="Northwoods.GoXam.Diagram"/> and will take precedence over this tool.
  /// </para>
  /// <para>
  /// To make this mode-less tool effective, you can remove the other two background mouse-dragging tools
  /// and install this <c>DragZoomingTool</c> in XAML:
  /// <code>
  ///   &lt;go:Diagram ...&gt;
  ///     &lt;go:Diagram.DragSelectingTool&gt;
  ///       &lt;x:Null /&gt;
  ///     &lt;/go:Diagram.DragSelectingTool&gt;
  ///     &lt;go:Diagram.PanningTool&gt;
  ///       &lt;x:Null /&gt;
  ///     &lt;/go:Diagram.PanningTool&gt;
  ///     &lt;go:Diagram.DragZoomingTool&gt;
  ///       &lt;go:DragZoomingTool /&gt;
  ///     &lt;/go:Diagram.DragZoomingTool&gt;
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
  public class DragZoomingTool : DiagramTool {

    static DragZoomingTool() {
      BoxTemplateProperty = DependencyProperty.Register("BoxTemplate", typeof(DataTemplate), typeof(DragZoomingTool), new FrameworkPropertyMetadata(null));
      ZoomedDiagramProperty = DependencyProperty.Register("ZoomedDiagram", typeof(Diagram), typeof(DragZoomingTool), new FrameworkPropertyMetadata(null));
    }

    /// <summary>
    /// Identifies the <see cref="BoxTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BoxTemplateProperty;
    /// <summary>
    /// Gets or sets the <c>DataTemplate</c> that renders the "rubber-band" box
    /// that the user draws to specify the zoom area.
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
    /// Identifies the <see cref="ZoomedDiagram"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomedDiagramProperty;
    /// <summary>
    /// Gets or sets the <see cref="Northwoods.GoXam.Diagram"/> that will be scaled and scrolled
    /// according to the value of <see cref="ComputeBoxBounds"/> by <see cref="ZoomToRect"/>.
    /// </summary>
    /// <value>
    /// By default this is null, which causes <see cref="ZoomToRect"/> to operate on the current <see cref="DiagramTool.Diagram"/>.
    /// </value>
    public Diagram ZoomedDiagram {
      get { return (Diagram)GetValue(ZoomedDiagramProperty); }
      set { SetValue(ZoomedDiagramProperty, value); }
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
    /// This tool can run when there has been a mouse-drag, far enough away not to be a click.
    /// </summary>
    /// <returns></returns>
    public override bool CanStart() {
      if (!base.CanStart()) return false;

      Diagram diagram = this.Diagram;
      if (diagram == null) return false;

      // require left button & that it has moved far enough away from the mouse down point, so it isn't a click
      if (!IsLeftButtonDown()) return false;

      // don't include the following check when this tool is running modally
      if (diagram.CurrentTool != this) {
        // mouse needs to have moved from the mouse-down point
        if (!IsBeyondDragSize()) return false;
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
    /// <returns>a <see cref="Node"/> that is not bound to data</returns>
    /// <remarks>
    /// If <see cref="BoxTemplate"/> is null, this uses the default template
    /// named "DefaultDragZoomingBoxTemplate".
    /// </remarks>
    protected virtual Node CreateBox() {
      Node box = new Node();  // for DragZoomingTool.CreateBox
      box.Id = "DragZoomingBox";
      DataTemplate template = this.BoxTemplate;
      if (template == null) template = Diagram.FindDefault<DataTemplate>("DefaultDragZoomingBoxTemplate");
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
    /// Upon the mouse-up, call <see cref="ZoomToRect"/>
    /// with the value of <see cref="ComputeBoxBounds"/>
    /// and stop this tool.
    /// </summary>
    public override void DoMouseUp() {
      Diagram diagram = this.Diagram;
      if (this.Active && diagram != null) {
        try {
          diagram.Cursor = Cursors.Wait;
          ZoomToRect(ComputeBoxBounds());
        } finally {
          diagram.Cursor = null;
        }
      }
      StopTool();
    }

    /// <summary>
    /// This just returns a <c>Rect</c> stretching from the
    /// mouse-down point to a point guided by the current mouse point
    /// and the aspect ratio of the <see cref="ZoomedDiagram"/>.
    /// </summary>
    /// <returns></returns>
    protected virtual Rect ComputeBoxBounds() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return new Rect(0, 0, 0, 0);
      Point start = diagram.FirstMousePointInModel;
      Point latest = diagram.LastMousePointInModel;
      double adx = latest.X-start.X;
      double ady = latest.Y-start.Y;

      Diagram observed = this.ZoomedDiagram;
      if (observed == null) observed = diagram;
      if (observed == null) {
        return new Rect(Math.Min(latest.X, start.X), Math.Min(latest.Y, start.Y), Math.Abs(latest.X - start.X), Math.Abs(latest.Y - start.Y));
      }
      Rect vrect = observed.Panel.ViewportBounds;
      if (vrect.Height == 0 || ady == 0) {
        return new Rect(Math.Min(latest.X, start.X), Math.Min(latest.Y, start.Y), Math.Abs(latest.X - start.X), Math.Abs(latest.Y - start.Y));
      }

      double vratio = vrect.Width/vrect.Height;
      double lx;
      double ly;
      if (Math.Abs(adx/ady) < vratio) {
        lx = start.X + adx;
        ly = start.Y + (int)Math.Ceiling(Math.Abs(adx) / vratio) * (ady < 0 ? -1 : 1);
      } else {
        lx = start.X + (int)Math.Ceiling(Math.Abs(ady) * vratio) * (adx < 0 ? -1 : 1);
        ly = start.Y + ady;
      }
      return new Rect(Math.Min(lx, start.X), Math.Min(ly, start.Y), Math.Abs(lx - start.X), Math.Abs(ly - start.Y));
    }

    /// <summary>
    /// This method is called to zoom the <see cref="ZoomedDiagram"/>
    /// to match the given rectangle.
    /// </summary>
    /// <param name="brect">a rectangle in model coordinates</param>
    /// <remarks>
    /// This sets the <see cref="ZoomedDiagram"/>'s <see cref="DiagramPanel"/>'s
    /// <see cref="DiagramPanel.Scale"/> and <see cref="DiagramPanel.Position"/> properties
    /// according to the <paramref name="brect"/> rectangle argument.
    /// </remarks>
    public virtual void ZoomToRect(Rect brect) {
      Diagram observed = this.ZoomedDiagram;
      if (observed == null) observed = this.Diagram;

      Rect vrect = observed.Panel.ViewportBounds;
      // do scale first, so DiagramPanel.NormalizePosition isn't constrained unduly when increasing scale
      observed.Panel.Scale = Math.Min(vrect.Width/brect.Width, 10);
      observed.Panel.Position = new Point(brect.X, brect.Y);

      //?? Shift to zoom out
    }

  }
}
