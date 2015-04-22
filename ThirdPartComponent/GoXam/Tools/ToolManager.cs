
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

using System.ComponentModel;
using System.Windows.Input;
using Northwoods.GoXam.Model;

namespace Northwoods.GoXam.Tool {

  /// <summary>
  /// This special <see cref="IDiagramTool"/> is responsible for managing all of
  /// the <see cref="Northwoods.GoXam.Diagram"/>'s mode-less tools.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Mode-less tools are tools that are present in one of the following lists:
  /// <see cref="Northwoods.GoXam.Diagram.MouseDownTools"/>,
  /// <see cref="Northwoods.GoXam.Diagram.MouseMoveTools"/>, or
  /// <see cref="Northwoods.GoXam.Diagram.MouseUpTools"/>.
  /// This <c>ToolManager</c> tool is normally the <see cref="Northwoods.GoXam.Diagram.DefaultTool"/>,
  /// so it is also often the <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>.
  /// </para>
  /// <para>
  /// When this tool is running as the current tool, it handles mouse-down,
  /// mouse-move, and mouse-up events.  For each event it iterates over each of the tools
  /// in the corresponding list, calling its <see cref="IDiagramTool.CanStart"/> predicate.
  /// If that predicate returns true, it starts that tool by making it the diagram's current tool.
  /// It then activates the tool and passes on the event to the tool by calling the
  /// corresponding method (either <see cref="IDiagramTool.DoMouseDown"/>,
  /// <see cref="IDiagramTool.DoMouseMove"/>, or <see cref="IDiagramTool.DoMouseUp"/>).
  /// </para>
  /// <para>
  /// Because this tool is typically the one running as the diagram's current tool
  /// when the user isn't "doing" anything, this tool can also handle other events,
  /// such as mouse wheel events.
  /// In Silverlight it handles key down events.
  /// (WPF handles the standard keyboard events for diagrams via command bindings.)
  /// </para>
  /// <para>
  /// This tool does not utilize any <see cref="Adornment"/>s or tool handles.
  /// </para>
  /// <para>
  /// This tool does not edit the model.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class ToolManager : DiagramTool {

    /// <summary>
    /// Iterate over the <see cref="Northwoods.GoXam.Diagram.MouseDownTools"/>
    /// and start the first one that <see cref="IDiagramTool.CanStart"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A mouse down first tries to give focus to the <see cref="Diagram"/>.
    /// </para>
    /// <para>
    /// Starting a tool replaces the <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>
    /// with the new tool.
    /// Successfully doing so also activates the new tool by calling <see cref="IDiagramTool.DoActivate"/>
    /// and passes on the mouse-down event to it by calling <see cref="IDiagramTool.DoMouseDown"/>.
    /// </para>
    /// <para>
    /// Not finding any startable tools causes this tool manager to activate,
    /// thereby enabling the mouse-move and mouse-up behaviors.
    /// </para>
    /// </remarks>
    public override void DoMouseDown() {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      IDiagramModel model = diagram.Model;

      // automatically try to give focus, so keyboard commands work
      diagram.Focus();

      // check to see if there is an ongoing edit
      UndoManager mgr = (model != null ? model.UndoManager : null);
      if (mgr != null && mgr.TransactionLevel != 0 && mgr.ChecksTransactionLevel) {
        Diagram.Trace("WARNING: In ToolManager.DoMouseDown: UndoManager.EditLevel is not zero");
      }

      foreach (IDiagramTool tool in diagram.MouseDownTools) {
        if (tool.CanStart()) {  // check if applicable
          diagram.CurrentTool = tool;  // calls tool.DoStart()
          // if tool hasn't stopped itself already, pass on the mouse event
          if (diagram.CurrentTool == tool) {
            tool.DoActivate();
            tool.DoMouseDown();
          }
          return;
        }
      }
      DoActivate();  // enable DoMouseMove and DoMouseUp
    }

    /// <summary>
    /// Iterate over the <see cref="Northwoods.GoXam.Diagram.MouseMoveTools"/>
    /// and start the first one that <see cref="IDiagramTool.CanStart"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Starting a tool replaces the <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>
    /// with the new tool.
    /// Successfully doing so also activates the new tool by calling <see cref="IDiagramTool.DoActivate"/>
    /// and passes on the mouse-move event to it by calling <see cref="IDiagramTool.DoMouseMove"/>.
    /// </para>
    /// </remarks>
    public override void DoMouseMove() {



      if (!this.Active) return;  // gotta handle a mouse-down first
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      foreach (IDiagramTool tool in diagram.MouseMoveTools) {
        if (tool.CanStart()) {  // check if applicable
          diagram.CurrentTool = tool;  // calls tool.DoStart()
          // if tool hasn't stopped itself already, pass on the mouse event
          if (diagram.CurrentTool == tool) {
            tool.DoActivate();
            tool.DoMouseMove();
          }
          return;
        }
      }
    }

    /// <summary>
    /// Iterate over the <see cref="Northwoods.GoXam.Diagram.MouseUpTools"/>
    /// and start the first one that <see cref="IDiagramTool.CanStart"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Starting a tool replaces the <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>
    /// with the new tool.
    /// Successfully doing so also activates the new tool by calling <see cref="IDiagramTool.DoActivate"/>
    /// and passes on the mouse-up event to it by calling <see cref="IDiagramTool.DoMouseUp"/>.
    /// </para>
    /// <para>
    /// If no startable tool is found it deactivates this tool manager,
    /// to get ready for a mouse-down and ignore mouse-move and mouse-up events.
    /// </para>
    /// </remarks>
    public override void DoMouseUp() {
      if (!this.Active) return;  // gotta handle a mouse-down first
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      foreach (IDiagramTool tool in diagram.MouseUpTools) {
        if (tool.CanStart()) {  // check if applicable
          diagram.CurrentTool = tool;  // calls tool.DoStart()
          // if tool hasn't stopped itself already, pass on the mouse event
          if (diagram.CurrentTool == tool) {
            tool.DoActivate();
            tool.DoMouseUp();
          }
          return;
        }
      }
      DoDeactivate();  // in case no tool is found that CanStart
    }

    /// <summary>
    /// This just calls <see cref="DiagramTool.StandardMouseWheel"/>
    /// to get the standard scrolling and zooming behavior.
    /// </summary>
    public override void DoMouseWheel() {
      StandardMouseWheel();
    }

    /// <summary>
    /// This just calls <see cref="Northwoods.GoXam.CommandHandler.DoKeyDown"/>
    /// on the diagram's <see cref="Northwoods.GoXam.CommandHandler"/>
    /// to handle standard keyboard command bindings in Silverlight.
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>
    /// In WPF, <c>InputBindings</c> for <c>Commands</c> invoke the commands, not through this mechanism.
    /// </remarks>
    public override void DoKeyDown(KeyEventArgs e) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      CommandHandler hndlr = diagram.CommandHandler;
      if (hndlr != null) hndlr.DoKeyDown(e);
    }

    /// <summary>
    /// This just calls <see cref="Northwoods.GoXam.CommandHandler.DoKeyUp"/>
    /// on the diagram's <see cref="Northwoods.GoXam.CommandHandler"/>.
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>
    /// In WPF, <c>InputBindings</c> for <c>Commands</c> invoke the commands, not through this mechanism.
    /// </remarks>
    public override void DoKeyUp(KeyEventArgs e) {
      Diagram diagram = this.Diagram;
      if (diagram == null) return;
      CommandHandler hndlr = diagram.CommandHandler;
      if (hndlr != null) hndlr.DoKeyUp(e);
    }
  }
}
