
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

using System.ComponentModel;
using System.Windows;

namespace Northwoods.GoXam.Tool {

  //?? autopanning; AutoPanRegion, AutoPanDelay, AutoPanTime

  /// <summary>
  /// The <c>PanningTool</c> supports manual panning, where the user
  /// can shift the <see cref="DiagramPanel"/>'s <see cref="DiagramPanel.Position"/>
  /// by dragging the mouse.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This tool is a standard mouse-move tool.
  /// Although <see cref="CanStart"/> is frequently able to return true,
  /// this tool is normally not run because other mouse-move tools will take precedence.
  /// In particular, the <see cref="DragSelectingTool"/> will run when the
  /// user drags in the diagram's background.
  /// Disable or remove the <see cref="Northwoods.GoXam.Diagram.DragSelectingTool"/>
  /// to enable background panning.
  /// Disable diagram selection or dragging to enable panning everywhere in the diagram.
  /// </para>
  /// <para>
  /// For example, you can enable background panning by removing the drag-selecting tool:
  /// <code>
  ///   &lt;go:Diagram ...
  ///       DragSelectingTool="{x:Null}" &gt;
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
  public class PanningTool : DiagramTool {

    /// <summary>
    /// This tool can run when the diagram allows scrolling and
    /// the mouse has been dragged far enough away from the mouse-down point
    /// to avoid being a click.
    /// </summary>
    /// <returns></returns>
    public override bool CanStart() {
      if (!base.CanStart()) return false;

      Diagram diagram = this.Diagram;

      if (diagram == null) return false;
      if (!diagram.AllowScroll) return false;

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
    /// Establish a scroll cursor and remember the <see cref="OriginalPosition"/>
    /// of the <see cref="DiagramPanel"/>.
    /// </summary>
    public override void DoActivate() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      diagram.Cursor = Part.ScrollAllCursor;
      this.OriginalPosition = diagram.Panel.Position;
      this.Active = true;
    }

    /// <summary>
    /// Restore the diagram's cursor.
    /// </summary>
    public override void DoDeactivate() {
      Diagram diagram = this.Diagram;
      if (diagram != null) {
        diagram.Cursor = null;
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
    /// Reset the <see cref="DiagramPanel"/>'s <see cref="DiagramPanel.Position"/>
    /// and stop this tool.
    /// </summary>
    public override void DoCancel() {
      Diagram diagram = this.Diagram;
      if (diagram != null) {
        diagram.Panel.Position = this.OriginalPosition;
      }
      StopTool();
    }

    /// <summary>
    /// Modify the <see cref="Northwoods.GoXam.Diagram"/>'s <see cref="Northwoods.GoXam.Diagram.Panel"/>'s
    /// <see cref="DiagramPanel.Position"/> according to how much the mouse has moved.
    /// </summary>
    public override void DoMouseMove() {
      Diagram diagram = this.Diagram;
      if (this.Active && diagram != null && diagram.Panel != null) {
        Point pos = diagram.Panel.Position;
        Point first = diagram.FirstMousePointInModel;
        Point last = diagram.LastMousePointInModel;
        diagram.Panel.Position = new Point(pos.X + first.X - last.X, pos.Y + first.Y - last.Y);
      }
    }

    private Point OriginalPosition { get; set; }
  }
}
