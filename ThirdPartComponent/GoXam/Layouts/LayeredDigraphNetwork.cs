
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
using System.Windows;
using Northwoods.GoXam;

namespace Northwoods.GoXam.Layout {

  /// <summary>
  /// This provides an abstract view of a network (graph) of nodes and directed links.
  /// These nodes and links correspond to
  /// <see cref="Part"/>s provided in the <see cref="Diagram"/>.
  /// This class provides a framework for manipulating the
  /// state of nodes and links without modifying the structure of the original document.
  /// </summary>
  public class LayeredDigraphNetwork : GenericNetwork<LayeredDigraphVertex, LayeredDigraphEdge, LayeredDigraphLayout> {
    /// <summary>
    /// Constructs an empty network.
    /// </summary>
    /// <remarks>
    /// Use this default constructor to create an empty network.
    /// Call <see cref="GenericNetwork{N, L, Y}.AddNodesAndLinks"/> to automatically add
    /// network nodes and links, or call <see cref="GenericNetwork{N, L, Y}.AddNode(Node)"/> and <see cref="GenericNetwork{N, L, Y}.LinkVertexes"/>
    /// explicitly to have more detailed control over the exact graph that is laid out.
    /// </remarks>
    public LayeredDigraphNetwork() { }
  }

  /********************************************************************************************/

  /// <summary>
  /// Holds auto-layout specific link data.
  /// </summary>
  public class LayeredDigraphEdge : LayeredDigraphNetwork.Edge {

    // layered-digraph specific properties:

    /// <summary>
    /// true if the link is part of the proper digraph; see <see cref="LayeredDigraphLayout.MakeProper"/> for details
    /// </summary>
    public bool Valid { get; set; }

    /// <summary>
    /// true if the link was reversed during cycle removal
    /// </summary>
    public bool Rev { get; set; }

    /// <summary>
    /// true if the link is part of depth first forest; used in <see cref="LayeredDigraphLayout.DepthFirstSearchCycleRemoval"/>
    /// </summary>
    public bool Forest { get; set; }

    /// <summary>
    /// location of the port at the from node of the link; allows the crossing matrix to correctly calculate the crossings for nodes with multiple ports
    /// </summary>
    public int PortFromPos { get; set; }

    /// <summary>
    /// location of the port at the to node of the link; allows the crossing matrix to correctly calculate the crossings for nodes with multiple ports
    /// </summary>
    public int PortToPos { get; set; }

    /// <summary>
    /// approximate column offset of the from port of the link from the from node column used in straightening
    /// </summary>
    public int PortFromColOffset { get; set; }

    /// <summary>
    /// approximate column offset of the to port of the link from the to node column used in straightening
    /// </summary>
    public int PortToColOffset { get; set; }
  }


  /********************************************************************************************/

  /// <summary>
  /// Holds auto-layout specific node data.
  /// </summary>
  public class LayeredDigraphVertex : LayeredDigraphNetwork.Vertex {

    // layered-digraph specific properties:

    /// <summary>
    /// the layer to which the node is assigned; see <see cref="LayeredDigraphLayout.AssignLayers"/> for details
    /// </summary>
    public int Layer { get; set; }

    /// <summary>
    /// the column to which the node is assigned; see <see cref="LayeredDigraphLayout.InitializeColumns"/> for details
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// the index to which the node is assigned; see <see cref="LayeredDigraphLayout.InitializeIndices"/> for details
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// a flag; used in <see cref="LayeredDigraphLayout.GreedyCycleRemoval"/>
    /// </summary>
    public bool Valid { get; set; }

    /// <summary>
    /// time of discovery in depth first search; used in
    /// <see cref="LayeredDigraphLayout.DepthFirstSearchCycleRemoval"/>
    /// </summary>
    public int Discover { get; set; }

    /// <summary>
    /// time of finishing in depth first search; used in
    /// <see cref="LayeredDigraphLayout.DepthFirstSearchCycleRemoval"/>
    /// </summary>
    public int Finish { get; set; }

    /// <summary>
    /// the connected component to which the node is assigned; used by
    /// <see cref="LayeredDigraphLayout.OptimalLinkLengthLayering"/> and
    /// <see cref="LayeredDigraphLayout.ComponentPack"/>
    /// </summary>
    public int Component { get; set; }

    /// <summary>
    /// another <see cref="LayeredDigraphVertex"/> in the same layer
    /// that this node should be near; used by <see cref="LayeredDigraphLayout.CrossingMatrix"/>
    /// </summary>
    public LayeredDigraphVertex Near { get; set; }

    internal int ArtificialType { get; set; }
  }
}
