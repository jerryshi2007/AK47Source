
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
  public class ForceDirectedNetwork : GenericNetwork<ForceDirectedVertex, ForceDirectedEdge, ForceDirectedLayout> {
    /// <summary>
    /// Constructs an empty network.
    /// </summary>
    /// <remarks>
    /// Use this default constructor to create an empty network.
    /// Call <see cref="GenericNetwork{N, L, Y}.AddNodesAndLinks"/> to automatically add
    /// network nodes and links, or call <see cref="GenericNetwork{N, L, Y}.AddNode(Node)"/> and <see cref="GenericNetwork{N, L, Y}.LinkVertexes"/>
    /// explicitly to have more detailed control over the exact graph that is laid out.
    /// </remarks>
    public ForceDirectedNetwork() { }
  }

  /********************************************************************************************/

  /// <summary>
  /// Holds auto-layout specific link data.
  /// </summary>
  public class ForceDirectedEdge : ForceDirectedNetwork.Edge {

    // force-directed specific properties:

    /// <summary>
    /// stiffness of the link; see the <see cref="ForceDirectedLayout.SpringStiffness"/> method
    /// </summary>
    public double Stiffness {
      get { return myStiffness; }
      set {
        myInternalFlags |= flagSetStiffness;
        myStiffness = value;
      }
    }

    internal bool IsStiffnessSet {
      get { return (myInternalFlags & flagSetStiffness) != 0; }
    }

    /// <summary>
    /// length of the link; see the <see cref="ForceDirectedLayout.SpringLength"/> method
    /// </summary>
    public double Length {
      get { return myLength; }
      set {
        myInternalFlags |= flagSetLength;
        myLength = value;
      }
    }

    internal bool IsLengthSet {
      get { return (myInternalFlags & flagSetLength) != 0; }
    }

    private const int flagSetStiffness = 0x0001;
    private const int flagSetLength = 0x0002;

    // ForceDirectedLinkData state
    private int myInternalFlags;
    private double myStiffness;
    private double myLength;
  }


  /********************************************************************************************/

  /// <summary>
  /// Holds auto-layout specific node data.
  /// </summary>
  public class ForceDirectedVertex : ForceDirectedNetwork.Vertex {

    // force-directed specific properties:

    /// <summary>
    /// whether <see cref="ForceDirectedLayout.IsFixed"/> should return true
    /// </summary>
    public bool IsFixed {
      get { return (myInternalFlags & flagIsFixed) != 0; }
      set {
        if (value)
          myInternalFlags |= flagIsFixed;
        else
          myInternalFlags &= ~flagIsFixed;
      }
    }

    /// <summary>
    /// cumulative force on the node in the X-direction
    /// </summary>
    public double ForceX { get; set; }

    /// <summary>
    /// cumulative force on the node in the Y-direction
    /// </summary>
    public double ForceY { get; set; }

    /// <summary>
    /// charge of the node.  See the <see cref="ForceDirectedLayout.ElectricalCharge"/> method
    /// </summary>
    public double Charge {
      get { return myCharge; }
      set {
        myInternalFlags |= flagSetCharge;
        myCharge = value;
      }
    }

    internal bool IsChargeSet {
      get { return (myInternalFlags & flagSetCharge) != 0; }
    }

    /// <summary>
    /// mass of the node.  See the <see cref="ForceDirectedLayout.GravitationalMass"/> method
    /// </summary>
    public double Mass {
      get { return myMass; }
      set {
        myInternalFlags |= flagSetMass;
        myMass = value;
      }
    }

    internal bool IsMassSet {
      get { return (myInternalFlags & flagSetMass) != 0; }
    }

    internal int NumConnections { get; set; }

    internal int NumInCluster { get; set; }

    internal ForceDirectedNetwork.VertexList Clustereds { get; set; }

    internal List<ForceDirectedLayout.StateInfo> SavedState { get; set; }

    internal int SavedStateIndex { get; set; }

    private const int flagIsFixed = 0x0001;
    private const int flagSetCharge = 0x0002;
    private const int flagSetMass = 0x0004;

    // ForceDirectedNodeData state
    private int myInternalFlags;
    private double myCharge;
    private double myMass;
  }
}
