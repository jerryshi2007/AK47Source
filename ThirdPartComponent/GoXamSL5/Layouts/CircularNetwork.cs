
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

namespace Northwoods.GoXam.Layout {

  /// <summary>
  /// This provides an abstract view of a network (a graph) of nodes and directed links.
  /// These nodes and links correspond to
  /// <see cref="Part"/>s provided in the <see cref="Diagram"/>.
  /// This class provides a framework for manipulating the
  /// state of nodes and links without modifying the structure of the original document.
  /// </summary>
  public class CircularNetwork : GenericNetwork<CircularVertex, CircularEdge, CircularLayout> {
    /// <summary>
    /// Constructs an empty network.
    /// </summary>
    /// <remarks>
    /// Use this default constructor to create an empty network.
    /// Call <see cref="GenericNetwork{N, L, Y}.AddNodesAndLinks"/> to automatically add
    /// network nodes and links, or call <see cref="GenericNetwork{N, L, Y}.AddNode(Node)"/> and <see cref="GenericNetwork{N, L, Y}.LinkVertexes"/>
    /// explicitly to have more detailed control over the exact graph that is laid out.
    /// </remarks>
    public CircularNetwork() { }
  }


  /// <summary>
  /// Holds auto-layout specific edge data.
  /// </summary>
  public class CircularEdge : CircularNetwork.Edge {
    // nothing to add!
  }


  /// <summary>
  /// Holds auto-layout specific vertex data.
  /// </summary>
  public class CircularVertex : CircularNetwork.Vertex {
    /// <summary>
    /// Gets or sets the value used as the vertex's diameter
    /// </summary>
    /// <value>
    /// By default the value depends on the <see cref="CircularLayout.Arrangement"/> property.
    /// Any computed value is cached, to avoid unnecessary expensive computations.
    /// </value>
    public double Diameter {
      get {
        if (Double.IsNaN(_Diameter)) {
          _Diameter = ComputeDiameter(0);
        }
        return _Diameter;
      }
      set {
        _Diameter = value;
      }
    }
    private double _Diameter = Double.NaN;

    /// <summary>
    /// Finds the effective diameter of this CircularVertex, which may depend on the angle
    /// at which the ellipse is being filled
    /// </summary>
    /// <param name="angle">
    /// The angle at which the layout's being filled. This is
    /// only necessary if <see cref="CircularLayout.NodeDiameterFormula"/> == <see cref="CircularNodeDiameterFormula.Circular"/>
    /// </param>
    /// <returns>The diameter, or <b>NaN</b> if unable to compute it</returns>
    public virtual double ComputeDiameter(double angle) {
      var network = this.Network;
      if (network == null) return Double.NaN;
      var layout = network.Layout;
      if (layout == null) return Double.NaN;
      if (layout.Arrangement == CircularArrangement.Packed) {
        if (layout.NodeDiameterFormula == CircularNodeDiameterFormula.Circular) {
          return Math.Max(this.Width, this.Height);
        } else {
          double sin = Math.Abs(Math.Sin(angle));
          double cos = Math.Abs(Math.Cos(angle));
          if (sin == 0) return this.Width;
          if (cos == 0) return this.Height;
          return Math.Min(this.Height / sin, this.Width / cos);
        }
      } else {
        if (layout.NodeDiameterFormula == CircularNodeDiameterFormula.Circular) {
          return Math.Max(this.Width, this.Height);
        } else {
          return Math.Sqrt(this.Width*this.Width + this.Height*this.Height);
        }
      }
    }

    /// <summary>
    /// Gets the computed angle for this particular vertex, given its position on the ellipse.
    /// </summary>
    public double ActualAngle { get; internal set; }

    /// <summary>
    /// Returns a string representing the CircularVertex
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
      return this.Node.Text + string.Format(" ( {0} {1} )", this.Center.X, this.Center.Y);
    }
  }


  /// <summary>
  /// Specifies how the nodes should be spaced in the ring for <see cref="CircularLayout"/>.
  /// </summary>
  public enum CircularArrangement {
    /// <summary>
    /// The spacing between the idealized boundaries of the nodes is constant
    /// </summary>
    ConstantSpacing,
    /// <summary>
    /// The distance between the centers of the nodes is constant
    /// </summary>
    ConstantDistance,
    /// <summary>
    /// The angular distance between the nodes is constant
    /// </summary>
    ConstantAngle,
    /// <summary>
    /// The vertices are arranged as close together as possible considering the <see cref="CircularLayout.Spacing" />,
    /// assuming the nodes are rectangular.
    /// </summary>
    Packed,
  }

  /// <summary>
  /// Represents the direction in which the nodes fill the ring for <see cref="CircularLayout"/>.
  /// </summary>
  public enum CircularDirection {
    /// <summary>
    /// Rings are filled clockwise
    /// </summary>
    Clockwise,
    /// <summary>
    /// Rings are filled counterclockwise
    /// </summary>
    Counterclockwise,
    /// <summary>
    /// The ring is filled by alternating sides; the second node is counterclockwise from first node.
    /// </summary>
    BidirectionalLeft,
    /// <summary>
    /// The ring is filled by alternating sides; the second node is clockwise from first node.
    /// </summary>
    BidirectionalRight,
  }

  /// <summary>
  /// Specifies how to sort the nodes for <see cref="CircularLayout"/>.
  /// </summary>
  public enum CircularSorting {
    /// <summary>
    /// Nodes are arranged in the order given
    /// </summary>
    Forwards,
    /// <summary>
    /// Nodes are arranged in the reverse of the order given
    /// </summary>
    Reverse,
    /// <summary>
    /// Nodes are sorted using the <see cref="CircularLayout.Comparer" />, in ascending order
    /// </summary>
    Ascending,
    /// <summary>
    /// Nodes are sorted using the <see cref="CircularLayout.Comparer" />, in reverse ascending order
    /// </summary>
    Descending,
    /// <summary>
    /// Nodes are ordered to reduce link crossings
    /// </summary>
    Optimized,
    ///// <summary>
    ///// Nodes are optimized after dividing them into subsets
    ///// </summary>
    //GroupedOptimized
  }

  /// <summary>
  /// Specifies a method for finding the size of a node for <see cref="CircularLayout"/>.
  /// </summary>
  public enum CircularNodeDiameterFormula {
    /// <summary>
    /// The effective diameter is sqrt(width^2+height^2).
    /// The corners of square nodes will touch at 45 degrees when <see cref="CircularLayout.Spacing"/> is 0.
    /// </summary>
    Pythagorean,
    /// <summary>
    /// The effective diameter is either the width or height of the node, whichever is larger.
    /// This will cause circular nodes to touch when <see cref="CircularLayout.Spacing"/> is 0.
    /// This is ideal when the nodes are circular.
    /// </summary>
    Circular,
  }

  /// <summary>
  /// Specifies how the links should be routed
  /// </summary>
  internal /*??? public */ enum CircularLinkRouting {
    /// <summary>
    /// Default link routing
    /// </summary>
    Default,
    /// <summary>
    /// Links are curved to avoid nodes
    /// </summary>
    Curved
  }
}

