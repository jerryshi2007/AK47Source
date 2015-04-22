
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

using System.Windows.Input;

namespace Northwoods.GoXam.Tool {

  /// <summary>
  /// This interface specifies the methods the diagram uses to control each tool
  /// and the methods used to handle the standard input events processed through
  /// the diagram.
  /// </summary>
  /// <remarks>
  /// <para>
  /// All existing tools are actually subclasses of the abstract class <see cref="DiagramTool"/>,
  /// which implements this interface.
  /// </para>
  /// <para>
  /// All uses of tools in <see cref="Diagram"/> use this interface,
  /// not the class <see cref="DiagramTool"/>.
  /// However, the <see cref="Diagram"/> class also has a property for each of the
  /// mode-less tools that the diagram might have, for convenience in replacing
  /// or removing those specific tools from the diagram.
  /// </para>
  /// </remarks>
  public interface IDiagramTool {
    /// <summary>
    /// Gets the <see cref="Northwoods.GoXam.Diagram"/> for which this tool is handling input events.
    /// </summary>
    Diagram Diagram { get; set; }

    /// <summary>
    /// The diagram asks each tool to update any adornments the tool might use for a given part.
    /// </summary>
    /// <param name="part"></param>
    /// <remarks>
    /// If the tool uses its own tool handles (<see cref="Northwoods.GoXam.Adornment"/>s),
    /// this should display them or hide them as appropriate.
    /// Typically this should only show them if the part is selected.
    /// </remarks>
    void UpdateAdornments(Part part);


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
    bool Active { get; set; }


    /// <summary>
    /// This predicate is used by the diagram to decide if this tool can be started.
    /// </summary>
    /// <returns>true if the <see cref="ToolManager"/> can make this tool the current one and call
    /// the <see cref="DoStart"/> method</returns>
    bool CanStart();

    /// <summary>
    /// This method is called by the diagram when this tool becomes the current tool.
    /// </summary>
    /// <remarks>
    /// Tool implementations should perform their per-use initialization here, such
    /// as setting up internal data structures, or capturing the mouse.
    /// </remarks>
    void DoStart();

    /// <summary>
    /// This method is called by the diagram after setting <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>,
    /// to make the new tool active.
    /// </summary>
    /// <remarks>
    /// This should set <see cref="Active"/> to true.
    /// This might call <see cref="Northwoods.GoXam.Tool.DiagramTool.StartTransaction"/>,
    /// if this tool's activity involves modification of the model.
    /// </remarks>
    void DoActivate();

    /// <summary>
    /// This method is called by the diagram on the old tool when
    /// <see cref="Northwoods.GoXam.Diagram.CurrentTool"/> is set to a new tool.
    /// </summary>
    /// <remarks>
    /// This needs to set <see cref="Active"/> to false.
    /// This might call <see cref="Northwoods.GoXam.Tool.DiagramTool.StopTransaction"/>,
    /// if this tool's activity involves modification of the model.
    /// </remarks>
    void DoDeactivate();

    /// <summary>
    /// This method is called by the diagram when this tool stops being the current tool.
    /// </summary>
    /// <remarks>
    /// Tool implementations should perform their per-use cleanup here,
    /// such as releasing mouse capture.
    /// </remarks>
    void DoStop();

    /// <summary>
    /// The diagram will call this method when the we wish to cancel the
    /// current tool's operation.
    /// </summary>
    /// <remarks>
    /// Typically this is called when the user hits the ESCAPE key.
    /// This should restore the original state and then call
    /// <see cref="Northwoods.GoXam.Tool.DiagramTool.StopTool"/>.
    /// </remarks>
    void DoCancel();


    /// <summary>
    /// The diagram will call this method upon a mouse down event.
    /// </summary>
    /// <remarks>
    /// This is normally overridden for mouse-down tools;
    /// it is not called for mouse-move or mouse-up tools.
    /// However it may also be called when the tool is run in a modal fashion,
    /// when code explicitly sets the diagram's <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>.
    /// </remarks>
    void DoMouseDown();

    /// <summary>
    /// The diagram will call this method upon a mouse move event.
    /// </summary>
    /// <remarks>
    /// This is normally overridden for mouse-move tools;
    /// it is not called for mouse-up tools.
    /// However it may also be called when the tool is run in a modal fashion,
    /// when code explicitly sets the diagram's <see cref="Northwoods.GoXam.Diagram.CurrentTool"/>.
    /// </remarks>
    void DoMouseMove();

    /// <summary>
    /// The diagram will call this method upon a mouse up event.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is normally overridden for mouse-up tools.
    /// </para>
    /// <para>
    /// Tools normally stop upon a mouse up, by calling <see cref="Northwoods.GoXam.Tool.DiagramTool.StopTool"/>.
    /// If you want to handle multiple mouse down-up gestures in one tool activation,
    /// you will need to override this method to only stop the tool when you want.
    /// </para>
    /// </remarks>
    void DoMouseUp();

    /// <summary>
    /// The diagram will call this method as the mouse wheel is rotated.
    /// </summary>
    void DoMouseWheel();


    /// <summary>
    /// The diagram will call this method upon a key down event.
    /// </summary>
    void DoKeyDown(KeyEventArgs e);

    /// <summary>
    /// The diagram will call this method upon a key up event.
    /// </summary>
    void DoKeyUp(KeyEventArgs e);

    //?? stylus events
  }
}
