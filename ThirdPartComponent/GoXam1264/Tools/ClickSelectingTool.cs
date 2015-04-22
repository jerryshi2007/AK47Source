
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

namespace Northwoods.GoXam.Tool {

  /// <summary>
  /// The <c>ClickSelectingTool</c> selects and deselects objects upon a click.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Normally this is the last "mouse-up" mode-less tool.
  /// </para>
  /// <para>
  /// This tool does not utilize any <see cref="Adornment"/>s or tool handles.
  /// </para>
  /// <para>
  /// This tool does not edit the model.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class ClickSelectingTool : DiagramTool {

    /// <summary>
    /// This tool can run when the diagram allows selection and there was a click.
    /// </summary>
    /// <returns></returns>
    public override bool CanStart() {
      if (!base.CanStart()) return false;

      Diagram diagram = this.Diagram;
      
      // heed Diagram.AllowSelect
      if (diagram == null || !diagram.AllowSelect) return false;
      
      // the mouse down point needs to be near the mouse up point
      if (IsBeyondDragSize()) return false;

      // any mouse button will do, not just the left button
      return true;
    }

    /// <summary>
    /// Upon a click, call <see cref="DiagramTool.StandardMouseSelect"/> and stop this tool.
    /// </summary>
    public override void DoMouseUp() {
      if (this.Active) {
        // maybe we'll find something at the input event point
        StandardMouseSelect();
      }

      // all done!
      StopTool();
    }
  }
}
