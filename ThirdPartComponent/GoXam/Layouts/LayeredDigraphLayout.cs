
/*
 *  Copyright (c) Northwoods Software Corporation, 1998-2010. All Rights Reserved.
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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Northwoods.GoXam;

namespace Northwoods.GoXam.Layout {

  /// <summary>
  /// LayeredDigraph provides an auto-layout for layered drawings
  /// of directed graphs.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The method uses a hierarchical approach
  /// for creating drawings of digraphs with nodes arranged in layers.
  /// The layout algorithm consists of four-major steps: Cycle Removal,
  /// Layer Assignment, Crossing Reduction, and Straightening and Packing.
  /// </para>
  /// <para>
  /// Although this class inherits from <c>FrameworkElement</c>
  /// in order to support data binding,
  /// it is not really a <c>FrameworkElement</c> or <c>UIElement</c>!
  /// Please ignore all of the properties, methods, and events defined by
  /// <c>FrameworkElement</c> and <c>UIElement</c>.
  /// </para>
  /// </remarks>
  [DesignTimeVisible(false)]
  public class LayeredDigraphLayout : DiagramLayout {
    /// <summary>
    /// Constructs a LayeredDigraph with null network and document.
    /// PerformLayout() will be a no-op until the network has been set.
    /// </summary>
    public LayeredDigraphLayout() {
    }
  
    /// <summary>
    /// Make a copy of a <see cref="LayeredDigraphLayout"/>, copying most of the
    /// important properties except for the <see cref="Network"/>.
    /// </summary>
    /// <param name="layout"></param>
    public LayeredDigraphLayout(LayeredDigraphLayout layout) : base(layout) {
      if (layout != null) {
        this.Network = null;
        this.LayerSpacing = layout.LayerSpacing;
        this.ColumnSpacing = layout.ColumnSpacing;
        this.Direction = layout.Direction;
        this.CycleRemoveOption = layout.CycleRemoveOption;
        this.LayeringOption = layout.LayeringOption;
        this.InitializeOption = layout.InitializeOption;
        this.Iterations = layout.Iterations;
        this.AggressiveOption = layout.AggressiveOption;
        this.PackOption = layout.PackOption;
        this.SetsPortSpots = layout.SetsPortSpots;
      }
    }

    static LayeredDigraphLayout() {
      NetworkProperty = DependencyProperty.Register("Network", typeof(LayeredDigraphNetwork), typeof(LayeredDigraphLayout),
        new FrameworkPropertyMetadata(null, OnNetworkChanged));
      LayerSpacingProperty = DependencyProperty.Register("LayerSpacing", typeof(double), typeof(LayeredDigraphLayout),
        new FrameworkPropertyMetadata(25.0, OnLayerSpacingChanged));
      ColumnSpacingProperty = DependencyProperty.Register("ColumnSpacing", typeof(double), typeof(LayeredDigraphLayout),
        new FrameworkPropertyMetadata(25.0, OnColumnSpacingChanged));
      DirectionProperty = DependencyProperty.Register("Direction", typeof(double), typeof(LayeredDigraphLayout),
        new FrameworkPropertyMetadata(0.0, OnPropertyChanged));
      CycleRemoveOptionProperty = DependencyProperty.Register("CycleRemoveOption", typeof(LayeredDigraphCycleRemove), typeof(LayeredDigraphLayout),
        new FrameworkPropertyMetadata(LayeredDigraphCycleRemove.DepthFirst, OnPropertyChanged));
      LayeringOptionProperty = DependencyProperty.Register("LayeringOption", typeof(LayeredDigraphLayering), typeof(LayeredDigraphLayout),
        new FrameworkPropertyMetadata(LayeredDigraphLayering.OptimalLinkLength, OnPropertyChanged));
      InitializeOptionProperty = DependencyProperty.Register("InitializeOption", typeof(LayeredDigraphInitIndices), typeof(LayeredDigraphLayout),
        new FrameworkPropertyMetadata(LayeredDigraphInitIndices.DepthFirstOut, OnPropertyChanged));
      IterationsProperty = DependencyProperty.Register("Iterations", typeof(int), typeof(LayeredDigraphLayout),
        new FrameworkPropertyMetadata(4, OnIterationsChanged));
      AggressiveOptionProperty = DependencyProperty.Register("AggressiveOption", typeof(LayeredDigraphAggressive), typeof(LayeredDigraphLayout),
        new FrameworkPropertyMetadata(LayeredDigraphAggressive.Less, OnPropertyChanged));
      PackOptionProperty = DependencyProperty.Register("PackOption", typeof(LayeredDigraphPack), typeof(LayeredDigraphLayout),
        new FrameworkPropertyMetadata(LayeredDigraphPack.Expand | LayeredDigraphPack.Median | LayeredDigraphPack.Straighten, OnPropertyChanged));
      SetsPortSpotsProperty = DependencyProperty.Register("SetsPortSpots", typeof(bool), typeof(LayeredDigraphLayout),
        new FrameworkPropertyMetadata(true, OnPropertyChanged));
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      LayeredDigraphLayout layout = (LayeredDigraphLayout)d;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="Network"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NetworkProperty;
    /// <summary>
    /// Gets or sets the <see cref="LayeredDigraphNetwork"/> that the layout will be performed on.
    /// </summary>
    /// <value>
    /// The initial value is null.
    /// </value>
    public LayeredDigraphNetwork Network {
      get { return (LayeredDigraphNetwork)GetValue(NetworkProperty); }
      set { SetValue(NetworkProperty, value); }
    }
    private static void OnNetworkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      LayeredDigraphLayout layout = (LayeredDigraphLayout)d;
      LayeredDigraphNetwork net = (LayeredDigraphNetwork)e.NewValue;
      if (net != null) net.Layout = layout;
    }

    /// <summary>
    /// Allocate a <see cref="LayeredDigraphNetwork"/>.
    /// </summary>
    /// <returns></returns>
    public virtual LayeredDigraphNetwork CreateNetwork() {
      LayeredDigraphNetwork n = new LayeredDigraphNetwork();
      n.Layout = this;
      return n;
    }

    /// <summary>
    /// Create and initialize a <see cref="LayeredDigraphNetwork"/> with the given nodes and links.
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="links"></param>
    /// <returns>a <see cref="LayeredDigraphNetwork"/></returns>
    public virtual LayeredDigraphNetwork MakeNetwork(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      LayeredDigraphNetwork net = CreateNetwork();
      net.AddNodesAndLinks(nodes, links);
      return net;
    }
  
    /// <summary>
    /// Performs a layered-digraph auto-layout.
    /// </summary>
    /// <remarks>
    /// If <see cref="Network"/> is null, one is automatically allocated
    /// and initialized with the graph that is in the <see cref="Diagram"/>.
    /// After all of the computations are completed, this calls
    /// <see cref="LayoutNodesAndLinks"/> in order to commit the positions
    /// of all of the nodes.
    /// No undo/redo transaction is started or finished by this method.
    /// </remarks>
    public override void DoLayout(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      if (this.Network == null) {
        this.Network = MakeNetwork(nodes, links);
      }
      this.ArrangementOrigin = InitialOrigin(this.ArrangementOrigin);

      ClearCaches();
      // Progress update.
      RaiseProgress(0);

      if (this.Network.VertexCount > 0) {
        // Remove self-links from the input network.
        this.Network.DeleteSelfEdges();

        // Removes cycles from the input network by reversing some number of links.
        RemoveCycles();
        // Progress update.
        RaiseProgress(0.10);

        // Assigns every node in the input network to a layer.
        // The layering satisfies the following:
        //  if L is a link from node U to node V, 
        //   then U.layer > V.layer;
        //   further, U.layer - V.layer >= LinkMinLength(L).
        // In addition, the nodes will satisfy
        //   for all nodes U, U.layer >= 0, with equality in at least one case,
        //   for all nodes U, U.layer <= maxLayer, with equality in at least one case.
        AssignLayersInternal();
        // Progress update.
        RaiseProgress(0.25);

        // Converts the input network into a proper digraph; i.e., artificial nodes and links
        //  are introduced into the network such that ever link is between nodes in adjacent
        //  layers.  This has the effect of breaking up long links into a sequence of artificial
        //  nodes.
        MakeProper();
        // Progress update.
        RaiseProgress(0.30);

        // Assigns every node in the input network an index number,
        //  such that nodes in the same layer will be labeled with 
        //  consecutive indices in left to right order.
        // All consecutive layout operations will preserve or update 
        //  the indices.
        InitializeIndicesInternal();
        // Progress update.
        RaiseProgress(0.35);

        // Assigns every node in the input network a column number,
        //  such that nodes in the same layer will be labeled with 
        //  increasing indices in left to right order.
        // In addition, a node U is assigned to a column such that 
        //  2 * minColumnSpace(U) + 1 columns are "allocated" to node U,
        //  and no two nodes have overlapping "allocations" of columns.
        // All consecutive layout operations will preserve or update 
        //  the columns.
        InitializeColumns();
        // Progress update.
        RaiseProgress(0.40);

        // Reorders nodes within layers to reduce the total number of link 
        //  crossings in the network.
        ReduceCrossings();
        // Progress update.
        RaiseProgress(0.60);

        // Adjusts the columns of nodes in the network to produce a layout which reduces
        //  the number of bends and is tightly packed.
        StraightenAndPack();
        // Progress update.
        RaiseProgress(0.90);

        // Update the "physical" positions of the nodes and links.
        if (this.Diagram != null && !this.Diagram.CheckAccess()) {
          Diagram.InvokeLater(this.Diagram, UpdateParts);
        } else {
          UpdateParts();
        }
      }
      // Progress update.
      RaiseProgress(1);

      this.Network = null;
    }

    private void UpdateParts() {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram != null) diagram.StartTransaction("LayeredDigraphLayout");
      LayoutNodesAndLinks();
      if (diagram != null) diagram.CommitTransaction("LayeredDigraphLayout");
      if (diagram != null && diagram.LayoutManager != null) {
        LayeredDigraphNetwork net = this.Network;
        diagram.LayoutManager.AddUpdateLinks(this, () => {
          this.Network = net;
          LayoutLinks();
          this.Network = null;
        });
      }
    }


    /********************************************************************************************/
    /********************************************************************************************/

    /// <summary>
    /// The function LinkMinLength returns the minimum length of the
    /// link represented by the LayeredDigraphLink link.
    /// The default implementation gives multi-links a minimum length of 2,
    /// and all other links a minimum length of 1.
    /// This function can be overridden to provide "fine-tuning" of the layout.
    /// </summary>
    /// <param name="edge"></param>
    /// <returns>Returns the minimum length of the link represented by link</returns>
    protected virtual int LinkMinLength(LayeredDigraphEdge edge) {
      LayeredDigraphVertex fromVertex = edge.FromVertex;
      LayeredDigraphVertex toVertex = edge.ToVertex;
    
      int links = 0;
      foreach (LayeredDigraphEdge succEdge in fromVertex.DestinationEdgesList) {
        LayeredDigraphVertex succVertex = succEdge.ToVertex;
        if (succVertex == toVertex) {
          links++;
        }
      }
      if (links > 1) {
        return 2;
      }
      return 1;
    }
  
    /// <summary>
    /// The function LinkLengthWeight returns the weight of the link
    /// represented by the LayeredDigraphLink link.  This weight is used by
    /// OptimalLinkLengthLayering to minimize weighted link lengths.
    /// The default implementation gives all links a length weight of 1.
    /// This function can be overridden to provide "fine-tuning" of the layout.
    /// </summary>
    /// <param name="edge"></param>
    /// <returns>Returns the weight of the link represented by link</returns>
    protected virtual double LinkLengthWeight(LayeredDigraphEdge edge) {
      return 1;
    }
  
    /// <summary>
    /// The function LinkStraightenWeight returns the weight of the link
    /// represented by the LayeredDigraphLink link.  This weight is used by
    /// the straightening methods to give priority straightening to those links
    /// with higher weights.
    /// The default implementation gives links between two "real" nodes a weight of 1,
    /// links between a "real" node and an "artifical" node a weight of 4,
    /// and links between two "artificial" nodes a weight of 8.
    /// This function can be overridden to provide "fine-tuning" of the layout.
    /// </summary>
    /// <param name="edge"></param>
    /// <returns>Returns the weight of the link represented by link</returns>
    protected virtual double LinkStraightenWeight(LayeredDigraphEdge edge) {
      LayeredDigraphVertex fromVertex = edge.FromVertex;
      LayeredDigraphVertex toVertex = edge.ToVertex;

      if ((fromVertex.Node == null) && (toVertex.Node == null)) 
        return 8;

      if ((fromVertex.Node == null) || (toVertex.Node == null)) 
        return 4;

      return 1;
    }
    
    /// <summary>
    /// This function returns the minimum space reserved for this node from the center point
    /// for the "depth" of the layer that it is in.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="topleft">whether to return the distance from the <see cref="GenericNetwork{N, L, Y}.Vertex.Position"/>
    /// to the <see cref="GenericNetwork{N, L, Y}.Vertex.Center"/>, or from the <b>Center</b>
    /// to the bottom or right side of the <see cref="GenericNetwork{N, L, Y}.Vertex.Bounds"/></param>
    /// <returns>Returns the minimum space reserved above and below this node, in document coordinates</returns>
    /// <remarks>
    /// The default implementation returns 0 for nodes that do not
    /// correspond to top-level Go objects.  For nodes that do correspond
    /// to top-level Go objects, the layer space is determined by the
    /// width or height of the object depending on the <see cref="Direction"/>.
    /// By default this adds 10 to the space, to account for port end segment lengths.
    /// </remarks>
    protected virtual double NodeMinLayerSpace(LayeredDigraphVertex v, bool topleft) {
      if (v.Node == null) return 0;
      Rect r = v.Bounds;
      Point p = v.Focus;
      if (this.Direction == 90 || this.Direction == 270) {
        if (topleft)
          return p.Y+10;
        else
          return r.Height-p.Y+10;
      } else {
        if (topleft)
          return p.X+10;
        else
          return r.Width-p.X+10;
      }
    }
  
    /// <summary>
    /// The function NodeMinColumnSpace returns the minimum space
    /// reserved to either side of this node.
    /// The default implementation returns 0 for nodes that do not
    /// correspond to top-level Go objects.  For nodes that do correspond
    /// to top-level Go objects, the column space is determined by the
    /// width and height of the object divided by the <see cref="ColumnSpacing"/>.
    /// Note: all sub-classes that override this method should ensure that
    /// nodes that do not correspond to top-level Go objects have a minimum
    /// column space of 0.
    /// This function can be overridden to provide "fine-tuning" of the layout.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="topleft">whether to return the distance from the <see cref="GenericNetwork{N, L, Y}.Vertex.Position"/>
    /// to the <see cref="GenericNetwork{N, L, Y}.Vertex.Center"/>, or from the <b>Center</b>
    /// to the bottom or right side of the <see cref="GenericNetwork{N, L, Y}.Vertex.Bounds"/></param>
    /// <returns>Returns the minimum space reserved to either side of the center of this node, in units of <see cref="ColumnSpacing"/></returns>
    protected virtual int NodeMinColumnSpace(LayeredDigraphVertex v, bool topleft) {
      if (v.Node == null) return 0;
      Rect r = v.Bounds;
      Point p = v.Focus;
      if (this.Direction == 90 || this.Direction == 270) {
        if (topleft)
          return (int)(p.X/this.ColumnSpacing) + 1;
        else
          return (int)((r.Width-p.X)/this.ColumnSpacing) + 1;
      } else {
        if (topleft)
          return (int)(p.Y/this.ColumnSpacing) + 1;
        else
          return (int)((r.Height-p.Y)/this.ColumnSpacing) + 1;
      }
    }

    /// <summary>
    /// The function SaveLayout stores the layer, column, and index of all
    /// nodes in an array of integers.
    /// </summary>
    /// <returns>Returns an integer array representation of the current layout</returns>
    /// <remarks>See also <seealso cref="LayeredDigraphLayout.RestoreLayout"/></remarks>
    protected virtual int[] SaveLayout() {
      if (mySavedLayout == null)
        mySavedLayout = new int[3 * this.Network.VertexCount];
      int i = 0;
    
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        mySavedLayout[i] = vertex.Layer;
        i++;
        mySavedLayout[i] = vertex.Column;
        i++;
        mySavedLayout[i] = vertex.Index;
        i++;
      }
      return mySavedLayout;
    }
   
    /// <summary>
    /// The function RestoreLayout restores the layer, column, and index of all
    /// nodes from an array of integers.
    /// </summary>
    /// <param name="layout"></param>
    /// <remarks>See also <seealso cref="LayeredDigraphLayout.SaveLayout"/></remarks>
    protected virtual void RestoreLayout(int[] layout) {
      int i = 0;
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        vertex.Layer = layout[i];
        i++;
        vertex.Column = layout[i];
        i++;
        vertex.Index = layout[i];
        i++;
      }
    }

    /// <summary>
    /// Computes the crossing matrix between the unfixedLayer and its adjacent layers.
    /// The direction argument indicates which adjacent layers should be taken into
    /// consideration when computing the crossing matrix:
    /// <code>
    /// direction == 0  --  use unfixedLayer - 1 and unfixedLayer + 1
    /// direction > 0  --  use unfixedLayer - 1 (sweeping away from layer 0)
    /// direction &lt; 0  --  use unfixedLayer + 1 (sweeping towards layer 0)
    /// </code>
    /// The resulting integer array can be used as follows:
    /// if index1 and index2 are the indices corresponding to two nodes on the
    /// unfixedLayer and crossmat is the crossing matrix, then
    /// <c>crossmat[index1 * indices[unfixedLayer] + index2]</c>
    /// is the number of crossing that occur if the node corresponding to index1 is
    /// placed to the left of the node corresponding to index2.  If <c>index1 == index2</c>,
    /// then <c>crossmat[index1 * indices[unfixedLayer] + index2]</c> is the number of crossings
    /// between links to and from the node corresponding to index1.
    /// </summary>
    /// <param name="unfixedLayer"></param>
    /// <param name="direction"></param>
    /// <returns>Returns the crossing matrix</returns>
    protected virtual int[] CrossingMatrix(int unfixedLayer, int direction) {
      // Fill unfixedLayerNodes with nodes in the unfixedLayer, ordered by index.
      LayeredDigraphVertex[] unfixedLayerNodes = GetCachedNodeArrayList(unfixedLayer);
      int num = indices[unfixedLayer];

      // Allocate memory for the crossing matrix.
      if (myCrossings == null || myCrossings.Length < num * num)
        myCrossings = new int[num * num];
      int []crossmat = myCrossings;

      // Process all pairs of nodes in the unfixed layer.
      for (int A = 0; A < num; A++) {

        // First, compute the crossings between links to and from unfixedLayerNodes[A].

        // Initially, no crossings discovered.
        int crossing = 0;

        // count unrelated nodes separating a node from the node it is supposed to be Near
        LayeredDigraphVertex vertex = unfixedLayerNodes[A];
        LayeredDigraphVertex near = vertex.Near;
        if (near != null && near.Layer == vertex.Layer) {
          int B = near.Index;
          if (B > A) {
            for (int i = A+1; i < B; i++) {
              LayeredDigraphVertex v = unfixedLayerNodes[i];
              if (v.Near != near || v.ArtificialType != near.ArtificialType) crossing++;
            }
          } else {
            for (int i = A-1; i > B; i--) {
              LayeredDigraphVertex v = unfixedLayerNodes[i];
              if (v.Near != near || v.ArtificialType != near.ArtificialType) crossing++;
            }
          }
        }

        // Compute crossings in positive direction.
        if ((direction > 0) || (direction == 0)) {
          // The first link.
          LayeredDigraphNetwork.EdgeList Links = unfixedLayerNodes[A].SourceEdgesList;
          for (int posA = 0; posA < Links.Count; posA++) {
            LayeredDigraphEdge edgeA = Links[posA];
            if (edgeA.Valid &&
              (edgeA.FromVertex.Layer != unfixedLayer)) {
              // Record the index of the node the link originates from.
              int indexA = edgeA.FromVertex.Index;

              // Record the portToPos and portFromPos for the link.
              int portToPosA = edgeA.PortToPos;
              int portFromPosA = edgeA.PortFromPos;

              // The second link.
              for (int posB = posA+1; posB < Links.Count; posB++) {
                LayeredDigraphEdge edgeB = Links[posB];
                if (edgeB.Valid &&
                  (edgeB.FromVertex.Layer != unfixedLayer)) {
                  // Record the index of the node the link originates from.
                  int indexB = edgeB.FromVertex.Index;
          
                  // Record the portToPos and portFromPos for the link.
                  int portToPosB = edgeB.PortToPos;
                  int portFromPosB = edgeB.PortFromPos;

                  // If the to position of the first link is 
                  //  to the left of the to position of the second link
                  //  and the from position of the first link is 
                  //  to the right of the from position of the second link,
                  //  then a crossing occurs.
                  if ((portToPosA < portToPosB) && 
                    ((indexA > indexB) || ((indexA == indexB) && (portFromPosA > portFromPosB)))) {
                    crossing++;
                  }
                  // If the to position of the second link is 
                  //  to the left of the to position of the first link
                  //  and the from position of the second link is 
                  //  to the right of the from position of the first link,
                  //  then a crossing occurs.
                  if ((portToPosB < portToPosA) && 
                    ((indexB > indexA) || ((indexB == indexA) && (portFromPosB > portFromPosA)))) {
                    crossing++;
                  }
                }
              }
            }
          }
        }

        // Compute crossings in negative direction.
        if ((direction < 0) || (direction == 0)) {
          // The first link.
          LayeredDigraphNetwork.EdgeList Links = unfixedLayerNodes[A].DestinationEdgesList;
          for (int posA = 0; posA < Links.Count; posA++) {
            LayeredDigraphEdge edgeA = Links[posA];
            if (edgeA.Valid &&
              (edgeA.ToVertex.Layer != unfixedLayer)) {
              // Record the index of the node the link goes to.
              int indexA = edgeA.ToVertex.Index;

              // Record the portToPos and portFromPos for the link.
              int portToPosA = edgeA.PortToPos;
              int portFromPosA = edgeA.PortFromPos;

              // The second link.
              for (int posB = posA+1; posB < Links.Count; posB++) {
                LayeredDigraphEdge edgeB = Links[posB];
                if (edgeB.Valid &&
                  (edgeB.ToVertex.Layer != unfixedLayer)) {
                  // Record the index of the node the link goes to.
                  int indexB = edgeB.ToVertex.Index;

                  // Record the portToPos and portFromPos for the link.
                  int portToPosB = edgeB.PortToPos;
                  int portFromPosB = edgeB.PortFromPos;

                  // If the from position of the first link is 
                  //  to the left of the from position of the second link
                  //  and the to position of the first link is 
                  //  to the right of the to position of the second link,
                  //  then a crossing occurs.
                  if ((portFromPosA < portFromPosB) && 
                    ((indexA > indexB) || ((indexA == indexB) && (portToPosA > portToPosB)))) {
                    crossing++;
                  }
                  // If the from position of the second link is 
                  //  to the left of the from position of the first link
                  //  and the to position of the second link is 
                  //  to the right of the to position of the first link,
                  //  then a crossing occurs.
                  if ((portFromPosB < portFromPosA) && 
                    ((indexB > indexA) || ((indexB == indexA) && (portToPosB > portToPosA)))) {
                    crossing++;
                  }
                }
              }
            }
          }
        }

        // Set the crossings of unfixedLayerNodes[A] with itself.
        crossmat[A * num + A] = crossing;

        // Now, compute the crossings between links to and from unfixedLayerNodes[A]
        //  and unfixedLayerNodes[B] for B > A.
        for (int B = A + 1; B < num; B++) {
          // Initially, no crossings discovered.
          // The variable crossingAB records the number of crossings if
          //  unfixedLayerNodes[A] (the A node) is placed to the left
          //  of unfixedLayerNodes[B] (the B node).
          // The variable crossingBA records the number of crossings if
          //  unfixedLayerNodes[A] (the A node) is placed to the right
          //  of unfixedLayerNodes[B] (the B node).
          int crossingAB = 0, crossingBA = 0;

          // Compute crossings in positive direction.
          if ((direction > 0) || (direction == 0)) {
            LayeredDigraphNetwork.EdgeList LinksA = unfixedLayerNodes[A].SourceEdgesList;
            LayeredDigraphNetwork.EdgeList LinksB = unfixedLayerNodes[B].SourceEdgesList;

            // The first link.
            for (int posA = 0; posA < LinksA.Count; posA++) {
              LayeredDigraphEdge edgeA = LinksA[posA];
              if (edgeA.Valid &&
                (edgeA.FromVertex.Layer != unfixedLayer)) {
                // Record the index of the node the link originates from.
                int indexA = edgeA.FromVertex.Index;
            
                // Record the portToPos and portFromPos for the link.
                int portToPosA = edgeA.PortToPos;
                int portFromPosA = edgeA.PortFromPos;
  
                // The second link.
                for (int posB = 0; posB < LinksB.Count; posB++) {
                  LayeredDigraphEdge edgeB = LinksB[posB];
                  if (edgeB.Valid &&
                    (edgeB.FromVertex.Layer != unfixedLayer)) {
                    // Record the index of the node the link originates from.
                    int indexB = edgeB.FromVertex.Index;

                    // Record the portToPos and portFromPos for the link.
                    int portToPosB = edgeB.PortToPos;
                    int portFromPosB = edgeB.PortFromPos;

                    // If the from position of the first link is 
                    //  to the right of the from position of the second link,
                    //  then a crossing occurs if the first node is to the right
                    //  of the second node.
                    if ((indexA < indexB) ||
                      ((indexA == indexB) && (portFromPosA < portFromPosB))) {
                      crossingBA++;
                    }
                    // If the from position of the second link is 
                    //  to the right of the from position of the first link,
                    //  then a crossing occurs if the first node is to the left
                    //  of the second node.
                    if ((indexB < indexA) ||
                      ((indexB == indexA) && (portFromPosB < portFromPosA))) {
                      crossingAB++;
                    }
                  }
                }
              }
            }
          }

          // Compute crossings in positive direction.
          if ((direction < 0) || (direction == 0)) {
            LayeredDigraphNetwork.EdgeList LinksA = unfixedLayerNodes[A].DestinationEdgesList;
            LayeredDigraphNetwork.EdgeList LinksB = unfixedLayerNodes[B].DestinationEdgesList;

            // The first link.
            for (int posA = 0; posA < LinksA.Count; posA++) {
              LayeredDigraphEdge edgeA = LinksA[posA];
              if (edgeA.Valid &&
                (edgeA.ToVertex.Layer != unfixedLayer)) {
                // Record the index of the node the link goes to.
                int indexA = edgeA.ToVertex.Index;

                // Record the portToPos and portFromPos for the link.
                int portToPosA = edgeA.PortToPos;
                int portFromPosA = edgeA.PortFromPos;

                // The second link.
                for (int posB = 0; posB < LinksB.Count; posB++) {
                  LayeredDigraphEdge edgeB = LinksB[posB];
                  if (edgeB.Valid &&
                    (edgeB.ToVertex.Layer != unfixedLayer)) {
                    // Record the index of the node the link goes to.
                    int indexB = edgeB.ToVertex.Index;

                    // Record the portToPos and portFromPos for the link.
                    int portToPosB = edgeB.PortToPos;
                    int portFromPosB = edgeB.PortFromPos;

                    // If the to position of the first link is 
                    //  to the right of the to position of the second link,
                    //  then a crossing occurs if the first node is to the right
                    if ((indexA < indexB) ||
                      ((indexA == indexB) && (portToPosA < portToPosB))) {
                      crossingBA++;
                    }
                    // If the to position of the second link is 
                    //  to the right of the to position of the first link,
                    //  then a crossing occurs if the first node is to the left
                    //  of the second node.
                    if ((indexB < indexA) ||
                      ((indexB == indexA) && (portToPosB < portToPosA))) {
                      crossingAB++;
                    }
                  }
                }
              }
            }
          }

          // Set the crossings of unfixedLayerNode[A] with unfixedLayerNode[B].
          crossmat[A * num + B] = crossingAB;
          crossmat[B * num + A] = crossingBA;
        }
      }
      FreeCachedNodeArrayList(unfixedLayer, unfixedLayerNodes);
      return crossmat;
    }
  

    /********************************************************************************************/

    /// <summary>
    /// Returns the total number of crossings in the network.
    /// Internal method used by <see cref="LayeredDigraphLayout.ReduceCrossings"/>.
    /// </summary>
    /// <returns>Returns the total number of crossings in the network.</returns>
    protected virtual int CountCrossings() {
      int layer;
      int crossings = 0;
      // Sweep from layer 0 to layer maxLayer.
      for (layer = 0; layer <= maxLayer; layer++) {



        int[] crossmat = CrossingMatrix(layer, 1);
        // Since index i <= index j,
        //  summing the entries in the crossing matrix 
        //  yields the total number of crossings in the layer.
        int num = indices[layer];
        for (int i = 0; i < num; i++) {
          for (int j = i; j < num; j++) {
            crossings += crossmat[i * num + j];
          }
        }



      }








      return crossings;
    }






























    /// <summary>
    /// Computes the bends between the unfixedLayer and its adjacent layers.
    /// The "bend" between a node U and a node V connected by a link L is calcluated by
    /// <c>abs((U.column + L.portFromColOffset) - (V.column + L.portToColOffset))</c>
    /// The "weighted bend" between a node U and a node V connected by link L is calculated by
    /// <c>abs((U.column + L.portFromColOffset) - (V.column + L.portToColOffset)) * LinkStraightenWeight(L)</c>
    /// The LinkStraightenWeight attempts to give higher priority to links between
    /// "artificial" nodes; i.e., long links in the final layout will be straighter.
    /// The direction argument indicates which adjacent layers should be taken into
    /// consideration when computing the crossing matrix:
    /// <c>direction == 0  --  use unfixedLayer - 1 and unfixedLayer + 1</c>
    /// <c>direction > 0  --  use unfixedLayer - 1 (sweeping away from layer 0)</c>
    /// <c>direction &lt; 0  --  use unfixedLayer + 1 (sweepeing towards layer 0)</c>
    /// </summary>
    /// <param name="unfixedLayer"></param>
    /// <param name="direction"></param>
    /// <param name="weighted"></param>
    /// <returns>Returns the bends between the unfixedLayer and its adjacent layers.</returns>
    protected virtual double Bends(int unfixedLayer, int direction, bool weighted) {
      // Fill unfixedLayerNodes with nodes in the unfixedLayer, ordered by index.
      LayeredDigraphVertex[] unfixedLayerNodes = GetCachedNodeArrayList(unfixedLayer);
      int num = indices[unfixedLayer];

      // The total bends.
      double bends = 0;

      int index;
      for (index = 0; index < num; index++) {
        // Calculate the predecessor link list, based on direction.
        LayeredDigraphNetwork.EdgeList LinksP = null;
        if ((direction < 0) || (direction == 0)) {
          LinksP = unfixedLayerNodes[index].SourceEdgesList;
        }
        // Calculate the successor link list, based on direction.
        LayeredDigraphNetwork.EdgeList LinksS = null;
        if ((direction > 0) || (direction == 0)) {
          LinksS = unfixedLayerNodes[index].DestinationEdgesList;
        }

        if (LinksP != null) {
          // Contribution to bends from predecessors not on same layer.
          for (int posP = 0; posP < LinksP.Count; posP++) {
            LayeredDigraphEdge edge = LinksP[posP];
            if (edge.Valid &&
              (edge.FromVertex.Layer != unfixedLayer)) {
              double fromColumn = edge.FromVertex.Column + edge.PortFromColOffset;
              double toColumn = edge.ToVertex.Column + edge.PortToColOffset;
              if (weighted) {
                bends += Math.Abs(fromColumn - toColumn) * LinkStraightenWeight(edge);
              } else {
                bends += Math.Abs(fromColumn - toColumn);
              }
            }
          }
        }
    
        if (LinksS != null) {
          // Contribution to bends from successors not on same layer.
          for (int posS = 0; posS < LinksS.Count; posS++) {
            LayeredDigraphEdge edge = LinksS[posS];
            if (edge.Valid &&
              (edge.ToVertex.Layer != unfixedLayer)) {
              double fromColumn = edge.FromVertex.Column + edge.PortFromColOffset;
              double toColumn = edge.ToVertex.Column + edge.PortToColOffset;
              if (weighted) {
                bends += (Math.Abs(fromColumn - toColumn) + 1) * LinkStraightenWeight(edge);
              } else {
                bends += Math.Abs(fromColumn - toColumn);
              }
            }
          }
        }

      }
      FreeCachedNodeArrayList(unfixedLayer, unfixedLayerNodes);
      return bends;
    }

    /// <summary>
    /// Returns the total number of bends in the network.
    /// The "bend" between a node U and a node V connected by a link L is calcluated by
    /// <c>abs((U.column + L.portFromColOffset) - (V.column + L.portToColOffset))</c>
    /// The "weighted bend" between a node U and a node V connected by link L is calculated by
    /// <c>abs((U.column + L.portFromColOffset) - (V.column + L.portToColOffset)) * LinkStraightenWeight(L)</c>
    /// The LinkStraightenWeight attempts to give higher priority to links between
    /// "artificial" nodes; i.e., long links in the final layout will be straighter.
    /// </summary>
    /// <param name="weighted"></param>
    /// <returns>Returns the total number of bends in the network.</returns>
    protected virtual double CountBends(bool weighted) {
      double bends = 0;
      for (int layer = 0; layer <= maxLayer; layer++) {
        bends += Bends(layer, 1, weighted);
      }
    
      return bends;
    }
  
  
    /********************************************************************************************/

    /// <summary>
    /// Adjusts the columns of all nodes such that the leftmost column will be
    /// column 0 and maxColumn is updated appropriately.
    /// </summary>
    protected virtual void Normalize() {
      int minColumn = int.MaxValue;
      maxColumn = -1;
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        minColumn = Math.Min(minColumn, vertex.Column - NodeMinColumnSpace(vertex, true));
        maxColumn = Math.Max(maxColumn, vertex.Column + NodeMinColumnSpace(vertex, false));
      }

      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        vertex.Column -= minColumn;
      }

      maxColumn -= minColumn;
    }
  
    /********************************************************************************************/

    /// <summary>
    /// Computes the array of barycenters (average) columns for the nodes in the 
    /// unfixedLayer based on the columns of predecessors (direction &lt; 0),
    /// successors (direction > 0), or both predecessors and successors (direction == 0)
    /// Elements without a defined barycenter will have an entry of -1.
    /// </summary>
    /// <param name="unfixedLayer"></param>
    /// <param name="direction"></param>
    /// <returns>Returns the array of barycenters (average) columns for the nodes in
    /// the unfixedLayer</returns>
    protected virtual double[] Barycenters(int unfixedLayer, int direction) {
      // Fill unfixedLayerNodes with nodes in the unfixedLayer, ordered by index.
      LayeredDigraphVertex[] unfixedLayerNodes = GetCachedNodeArrayList(unfixedLayer);
      int num = indices[unfixedLayer];

      // Allocate memory of the barycenter array.
      double[] barycenters = new double[num];
  
      int index;
      for (index = 0; index < num; index++) {
        LayeredDigraphVertex vertex = unfixedLayerNodes[index];

        // Calculate the predecessor link list, based on direction.
        LayeredDigraphNetwork.EdgeList LinksP = null;
        if ((direction < 0) || (direction == 0)) {
          LinksP = vertex.SourceEdgesList;
        }
        // Calculate the successor link list, based on direction.
        LayeredDigraphNetwork.EdgeList LinksS = null;
        if ((direction > 0) || (direction == 0)) {
          LinksS = vertex.DestinationEdgesList;
        }

        // Compute the barycenter.
        // The variable total counts the total column position.
        // The variable N counts the number of predecessors or successors,
        // and any node in the same layer that this node is near.
        double total = 0;
        int N = 0;

        // regardless of direction, use the column of the node that should be Near to this node
        if (vertex.Near != null && vertex.Near.Layer == vertex.Layer) {
          total += vertex.Near.Column - 1;  //??? -1
          N++;
        }

        if (LinksP != null) {
          // Contribution to barycenter from predecessors not on same layer.
          for (int posP = 0; posP < LinksP.Count; posP++) {
            LayeredDigraphEdge edge = LinksP[posP];
            if (edge.Valid && !edge.Rev &&
              (edge.FromVertex.Layer != unfixedLayer)) {
              total += edge.FromVertex.Column + edge.PortFromColOffset;
              N++;
            }
          }
        }

        if (LinksS != null) {
          // Contribution to barycenter from successors not on same layer.
          for (int posS = 0; posS < LinksS.Count; posS++) {
            LayeredDigraphEdge edge = LinksS[posS];
            if (edge.Valid && !edge.Rev &&
              (edge.ToVertex.Layer != unfixedLayer)) {
              total += edge.ToVertex.Column + edge.PortToColOffset;
              N++;
            }
          }
        }

        // Calculate the barycenter.
        if (N == 0) {
          barycenters[index] = -1;
        } else {
          barycenters[index] = total / N;
        }
      }
      FreeCachedNodeArrayList(unfixedLayer, unfixedLayerNodes);
      return barycenters;
    }

  
    /********************************************************************************************/

    /// <summary>
    /// Computes the array of median columns for the nodes in the
    /// unfixedLayer based on the columns of predecessors (direction &lt; 0),
    /// successors (direction > 0), or both predecessors and successors (direction == 0).
    /// Elements without a defined median will have an entry of -1.
    /// </summary>
    /// <param name="unfixedLayer"></param>
    /// <param name="direction"></param>
    /// <returns>Returns the array of median columns for the nodes in
    /// the unfixedLayer</returns>
    protected virtual double[] Medians(int unfixedLayer, int direction) {
      // Fill unfixedLayerNodes with nodes in the unfixedLayer, ordered by index.
      LayeredDigraphVertex[] unfixedLayerNodes = GetCachedNodeArrayList(unfixedLayer);
      int num = indices[unfixedLayer];

      // Allocate memory of the median array.
      if (myMedians == null || myMedians.Length < num)
        myMedians = new double[num];
      double []medians = myMedians;

      int index;
      for (index = 0; index < num; index++) {
        LayeredDigraphVertex vertex = unfixedLayerNodes[index];

        // Calculate the predecessor link list, based on direction.
        LayeredDigraphNetwork.EdgeList LinksP = null;
        if ((direction < 0) || (direction == 0)) {
          LinksP = vertex.SourceEdgesList;
        }

        // Calculate the successor link list, based on direction.
        LayeredDigraphNetwork.EdgeList LinksS = null;
        if ((direction > 0) || (direction == 0)) {
          LinksS = vertex.DestinationEdgesList;
        }

        // Compute the median.
        // The variable N counts the number of predecessors or successors not on same layer,
        // and any node in the same layer that this node is near.
        int N = 0;

        if (vertex.Near != null && vertex.Near.Layer == vertex.Layer) N++;

        if (LinksP != null) {
          // Count the total number of predecessors and successors not on same layer..
          for (int posP = 0; posP < LinksP.Count; posP++) {
            LayeredDigraphEdge edge = LinksP[posP];
            if (edge.Valid && !edge.Rev &&
              (edge.FromVertex.Layer != unfixedLayer)) {
              N++;
            }
          }
        }
      
        if (LinksS != null) {
          for (int posS = 0; posS < LinksS.Count; posS++) {
            LayeredDigraphEdge edge = LinksS[posS];
            if (edge.Valid && !edge.Rev &&
              (edge.ToVertex.Layer != unfixedLayer)) {
              N++;
            }
          }
        }

        if (N == 0) {
          medians[index] = -1;
        } else {
          // Allocate an array of integers to hold the columns of the predecessors
          //  and successors.
          if (myColumnsPS == null || myColumnsPS.Length < N) {
            myColumnsPS = new int[2 * N];
          }
  
          // Fill in the myColumnsPS array with the columns of the predecessors not on same layer..
          N = 0;

          // regardless of direction, use the column of the node that should be Near to this node
          if (vertex.Near != null && vertex.Near.Layer == vertex.Layer) {
            myColumnsPS[N] = vertex.Near.Column - 1; //??? -1
            N++;
          }

          if (LinksP != null) {
            for (int posP = 0; posP < LinksP.Count; posP++) {
              LayeredDigraphEdge edge = LinksP[posP];
              if (edge.Valid && !edge.Rev &&
                (edge.FromVertex.Layer != unfixedLayer)) {
                myColumnsPS[N] = edge.FromVertex.Column  + edge.PortFromColOffset;
                N++;
              }
            }
          }

          if (LinksS != null) {
            // Fill in the myColumnsPS array with the columns of the successors not on same layer..
            for (int posS = 0; posS < LinksS.Count; posS++) {
              LayeredDigraphEdge edge = LinksS[posS];
              if (edge.Valid && !edge.Rev &&
                (edge.ToVertex.Layer != unfixedLayer)) {
                myColumnsPS[N] = edge.ToVertex.Column  + edge.PortToColOffset;
                N++;
              }
            }
          }

          // sort the array of columns
          Array.Sort<int>(myColumnsPS, 0, N, Comparer<int>.Default );

          // Calculate the position of the median.
          int M = N / 2;
          // If there is a distinct median, use it.
          if (N % 2 == 1) {
            medians[index] = (double)(myColumnsPS[M]);
            // If there are two medians, take their average.
          } else {
            medians[index] = ((double)(myColumnsPS[M-1]) + (double)(myColumnsPS[M])) / 2;
          }
        }
      }
      FreeCachedNodeArrayList(unfixedLayer, unfixedLayerNodes);
      return medians;
    }
  
    /********************************************************************************************/

    /// <summary>
    /// Uses a depth first search algorithm to set the component of all nodes in a component.
    /// The forward and backward bools indicate the direction to use for a
    /// directed depth first search from node.
    /// **Unset functions only set the component and recurse on nodes whose component is currently
    /// set to the unset value.
    /// Tight** functions only set the component and recurse on nodes which are "tight",
    /// in the sense that the nodes are separated by a link which corresponds to the
    /// minumum link length of the link between the two nodes.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="component"></param>
    /// <param name="forward"></param>
    /// <param name="backward"></param>
    protected virtual void TightComponent(LayeredDigraphVertex v, int component, bool forward, bool backward) {
      if (v.Component != component) {
        v.Component = component;

        if (forward) {
          foreach (LayeredDigraphEdge succEdge in v.DestinationEdgesList) {
            LayeredDigraphVertex succVertex = succEdge.ToVertex;
            int length = v.Layer - succVertex.Layer;
            int minLength = LinkMinLength(succEdge);
            if (length == minLength) {
              TightComponent(succVertex, component, forward, backward);
            }
          }
        }

        if (backward) {
          foreach (LayeredDigraphEdge predEdge in v.SourceEdgesList) {
            LayeredDigraphVertex predVertex = predEdge.FromVertex;
            int length = predVertex.Layer - v.Layer;
            int minLength = LinkMinLength(predEdge);
            if (length == minLength) {
              TightComponent(predVertex, component, forward, backward);
            }
          }
        }
      }
    }
  
    /// <summary>
    /// Uses a depth first search algorithm to set the component of all nodes in a component.
    /// Tight** functions only set the component and recurse on nodes which are "tight",
    /// in the sense that the nodes are separated by a link which corresponds to the
    /// minumum link length of the link between the two nodes.
    /// **Unset functions only set the component and recurse on nodes whose component is currently
    /// set to the unset value.
    /// The forward and backward bools indicate the direction to use for a
    /// directed depth first search from node.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="component"></param>
    /// <param name="unset"></param>
    /// <param name="forward"></param>
    /// <param name="backward"></param>
    protected virtual void TightComponentUnset(LayeredDigraphVertex v, int component, int unset, bool forward, bool backward) {
      if (v.Component == unset) {
        v.Component = component;

        if (forward) {
          foreach (LayeredDigraphEdge succEdge in v.DestinationEdgesList) {
            LayeredDigraphVertex succVertex = succEdge.ToVertex;
            int length = v.Layer - succVertex.Layer;
            int minLength = LinkMinLength(succEdge);
            if (length == minLength) {
              TightComponentUnset(succVertex, component, unset, forward, backward);
            }
          }
        }

        if (backward) {
          foreach (LayeredDigraphEdge predEdge in v.SourceEdgesList) {
            LayeredDigraphVertex predVertex = predEdge.FromVertex;
            int length = predVertex.Layer - v.Layer;
            int minLength = LinkMinLength(predEdge);
            if (length == minLength) {
              TightComponentUnset(predVertex, component, unset, forward, backward);
            }
          }
        }
      }
    }
  
    /// <summary>
    /// Uses a depth first search algorithm to set the component of all nodes in a component.
    /// The forward and backward bools indicate the direction to use for a
    /// directed depth first search from node.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="component"></param>
    /// <param name="forward"></param>
    /// <param name="backward"></param>
    protected virtual void SetComponents(LayeredDigraphVertex v, int component, bool forward, bool backward) {
      if (v.Component != component) {
        v.Component = component;
        if (forward) {
          foreach (LayeredDigraphEdge succEdge in v.DestinationEdgesList) {
            LayeredDigraphVertex succVertex = succEdge.ToVertex;
            SetComponents(succVertex, component, forward, backward);
          }
        }
        if (backward) {
          foreach (LayeredDigraphEdge predEdge in v.SourceEdgesList) {
            LayeredDigraphVertex predVertex = predEdge.FromVertex;
            SetComponents(predVertex, component, forward, backward);
          }
        }
      }
    }
    
    /// <summary>
    /// Uses a depth first search algorithm to set the component of all nodes in a component.
    /// **Unset functions only set the component and recurse on nodes whose component is currently
    /// set to the unset value.
    /// The forward and backward bools indicate the direction to use for a
    /// directed depth first search from node.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="component"></param>
    /// <param name="unset"></param>
    /// <param name="forward"></param>
    /// <param name="backward"></param>
    protected virtual void ComponentUnset(LayeredDigraphVertex v, int component, int unset, bool forward, bool backward) {
      if (v.Component == unset) {
        v.Component = component;
        if (forward) {
          foreach (LayeredDigraphEdge succEdge in v.DestinationEdgesList) {
            LayeredDigraphVertex succVertex = succEdge.ToVertex;
            ComponentUnset(succVertex, component, unset, forward, backward);
          }
        }
        if (backward) {
          foreach (LayeredDigraphEdge predEdge in v.SourceEdgesList) {
            LayeredDigraphVertex predVertex = predEdge.FromVertex;
            ComponentUnset(predVertex, component, unset, forward, backward);
          }
        }
      }
    }
  
  
    /********************************************************************************************/

    /// <summary>
    /// Removes cycles from the input network by reversing some number of links.
    /// </summary>
    /// <remarks>
    /// By default this just calls <see cref="GreedyCycleRemoval"/>
    /// or <see cref="DepthFirstSearchCycleRemoval"/>, as appropriate
    /// given the value of <see cref="CycleRemoveOption"/>.
    /// </remarks>
    protected virtual void RemoveCycles() {
      // Initially, no links are reveresed.
      foreach (LayeredDigraphEdge edge in this.Network.Edges) {
        edge.Rev = false;
      }

      switch (this.CycleRemoveOption) {
        default:
        case LayeredDigraphCycleRemove.Greedy:
          GreedyCycleRemoval(); 
          break;
        case LayeredDigraphCycleRemove.DepthFirst:
          DepthFirstSearchCycleRemoval();
          break;
      }
    }
  
    /********************************************************************************************/
  
    /// <summary>
    /// Removes cycles from the input network using a Greedy-Cycle-Removal algorithm.
    /// The idea is to induce an order on all nodes
    /// in the network (U1, U2, U3, ..., Uk) such that for the majority of links L = (Ui, Uj)
    /// it is true that i &lt; j.  All links L = (Ui, Uj) such that i > j are reversed.
    /// </summary>
    protected virtual void GreedyCycleRemoval() {
      // The array nodes will represent the ordering of the nodes.
      int l = 0;
      int r = this.Network.VertexCount - 1;
      LayeredDigraphVertex[] nodes = new LayeredDigraphVertex[r + 1];

      // Every node is initially valid -- i.e., available to be added to the ordering.
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        vertex.Valid = true;
      }
  
      // While some node is still valid...
      while (GreedyCycleRemovalFindNode(this.Network) != null) {
        LayeredDigraphVertex vertex;
        // Add all sink nodes to the ordering at the rightmost available position,
        //  and invalidate the node.
        vertex = GreedyCycleRemovalFindSink(this.Network);
        while (vertex != null) {
          nodes[r] = vertex;
          r--;
          vertex.Valid = false;
          vertex = GreedyCycleRemovalFindSink(this.Network);
        }
        // Add all source nodes to the ordering at the leftmost available position,
        //  and invalidate the node.
        vertex = GreedyCycleRemovalFindSource(this.Network);
        while (vertex != null) {
          nodes[l] = vertex;
          l++;
          vertex.Valid = false;
          vertex = GreedyCycleRemovalFindSource(this.Network);
        }
        // Add a node which maximizes outdeg - indeg at the leftmost available position,
        //  and invalidate the node.
        // This is the greedy step, which locally maximizes the number of "good" links
        //  for a given number of "bad" links.
        vertex = GreedyCycleRemovalFindNodeMaxDegDiff(this.Network);
        if (vertex != null) {
          nodes[l] = vertex;
          l++;
          vertex.Valid = false;
        }
      }

      // Set the index of each node to the appropriate order position.
      for (int index = 0; index < this.Network.VertexCount; index++) {
        nodes[index].Index = index;
      }

      // Reverse the necessary links.
      foreach (LayeredDigraphEdge edge in this.Network.Edges) {
        int fromIndex = edge.FromVertex.Index;
        int toIndex = edge.ToVertex.Index;
        if (fromIndex > toIndex) {
          this.Network.ReverseEdge(edge);
          edge.Rev = true;
        }
      }
    }
  
    /// <summary>
    /// Finds a valid node in the network.
    /// Returns null if no valid node exists.
    /// Used by GreedyCycleRemoval.
    /// </summary>
    /// <param name="network"></param>
    /// <returns>Returns a valid node in the network or null if no valid nodes exist</returns>
    protected virtual LayeredDigraphVertex GreedyCycleRemovalFindNode(LayeredDigraphNetwork network) {
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        if (vertex.Valid) {
          return vertex;
        }
      }
      return null;
    }

    /// <summary>
    /// Finds a sink node in the network.  A node is considered a sink node if it is
    /// valid and all of its predecessors are invalid.  A valid node with no predecessors
    /// is vacously a sink.
    /// Returns null if no valid sink node exists.
    /// Used by GreedyCycleRemoval.
    /// </summary>
    /// <param name="network"></param>
    /// <returns>Returns a sink node in the network, or null if no valid sink node exists</returns>
    protected virtual LayeredDigraphVertex GreedyCycleRemovalFindSink(LayeredDigraphNetwork network) {
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        if (vertex.Valid) {
          bool sink = true;
          foreach (LayeredDigraphEdge edge in vertex.DestinationEdgesList) {
            if (edge.ToVertex.Valid) {
              sink = false;
            }
          }
          if (sink)
            return vertex;
        }
      }
      return null;
    }

    /// <summary>
    /// Finds a source node in the network.  A node is considered a sink node if it is
    /// valid and all of its successors are invalid.  A valid node with no successors
    /// is vacously a source.
    /// Returns null if no valid source node exists.
    /// Used by GreedyCycleRemoval.
    /// </summary>
    /// <param name="network"></param>
    /// <returns>Returns a source node in the network or null if no sources exist</returns>
    protected virtual LayeredDigraphVertex GreedyCycleRemovalFindSource(LayeredDigraphNetwork network) {
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        if (vertex.Valid) {
          bool source = true;
          foreach (LayeredDigraphEdge edge in vertex.SourceEdgesList) {
            if (edge.FromVertex.Valid) {
              source = false;
            }
          }
          if (source)
            return vertex;
        }
      }
      return null;
    }

    /// <summary>
    /// Finds a valid node in the network that maximizes outdeg - indeg.
    /// The degree difference is computed using valid successors and predecessors.
    /// Returns null if no valid node exists.
    /// Used by GreedyCycleRemoval.
    /// </summary>
    /// <param name="network"></param>
    /// <returns>Returns a valid node in the network that maximized outdeg-indeg or null
    /// if no valid node exists</returns>
    protected virtual LayeredDigraphVertex GreedyCycleRemovalFindNodeMaxDegDiff(LayeredDigraphNetwork network) {
      LayeredDigraphVertex vertexMax = null;
      int max = 0;

      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        if (vertex.Valid) {
          int outdeg = 0;
          foreach (LayeredDigraphEdge succEdge in vertex.DestinationEdgesList) {
            if (succEdge.ToVertex.Valid) {
              outdeg++;
            }
          }
          int indeg = 0;
          foreach (LayeredDigraphEdge predEdge in vertex.SourceEdgesList) {
            if (predEdge.FromVertex.Valid) {
              indeg++;
            }
          }

          if ((vertexMax == null) || (max < (outdeg - indeg))) {
            vertexMax = vertex;
            max = outdeg - indeg;
          }
        }
      }

      return vertexMax;
    }

    /********************************************************************************************/

    /// <summary>
    /// Removes cycles from the input network using a depth first search.
    /// A link not in the depth first forest is reversed if the from-node was discovered
    /// and finished by the depth first search after the to-node was discovered but before
    /// the to-node was finished.
    /// </summary>
    protected virtual void DepthFirstSearchCycleRemoval() {
      // Every node is initially undiscovered and unfinished.
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        vertex.Discover = -1;
        vertex.Finish = -1;
      }
      // Every link is initially not in the depth first forest.
      foreach (LayeredDigraphEdge edge in this.Network.Edges) {
        edge.Forest = false;
      }

      myDepthFirstSearchCycleRemovalTime = 0;

      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        if (vertex.SourceEdgesList.Count == 0) {
          DepthFirstSearchCycleRemovalVisit(vertex);
        }
      }
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        if (vertex.Discover == -1) {
          DepthFirstSearchCycleRemovalVisit(vertex);
        }
      }

      // Scan through all non-forest links and reverse the appropriate links.
      foreach (LayeredDigraphEdge edge in this.Network.Edges) {
        if (!(edge.Forest)) {
          LayeredDigraphVertex fromVertex = edge.FromVertex;
          int Fdiscover = fromVertex.Discover;
          int Ffinish = fromVertex.Finish;

          LayeredDigraphVertex toVertex = edge.ToVertex;
          int Tdiscover = toVertex.Discover;
          int Tfinish = toVertex.Finish;

          if ((Tdiscover < Fdiscover) && (Ffinish < Tfinish)) {
            this.Network.ReverseEdge(edge);
            edge.Rev = true;
          }
        }
      }
    }

    /// <summary>
    /// Peforms the recursive step of the depth first search on node.
    /// Updates the discover and finish time of node.
    /// Updates the forest flag of followed links.
    /// </summary>
    /// <param name="v"></param>
    protected virtual void DepthFirstSearchCycleRemovalVisit(LayeredDigraphVertex v) {
      v.Discover = myDepthFirstSearchCycleRemovalTime;
      myDepthFirstSearchCycleRemovalTime++;
    
      foreach (LayeredDigraphEdge edge in v.DestinationEdgesList) {
        LayeredDigraphVertex vertexS = edge.ToVertex;
        if (vertexS.Discover == -1) {
          edge.Forest = true;
          DepthFirstSearchCycleRemovalVisit(vertexS);
        }
      }

      v.Finish = myDepthFirstSearchCycleRemovalTime;
      myDepthFirstSearchCycleRemovalTime++;  
    }


    /********************************************************************************************/

    void AssignLayersInternal() {
      // Every node is initially unlayered.
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        vertex.Layer = -1;
      }
      maxLayer = -1;
      AssignLayers();
      // Determine actual maximum
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) { // ??? don't think this is needed. It is done at the end of AssignLayers anyway
        maxLayer = Math.Max(maxLayer, vertex.Layer);
      }
    }

    /// <summary>
    /// Assigns every node in the input network to a layer.
    /// The layering satisfies the following:
    /// if L is a link from node U to node V,
    /// then U.layer > V.layer;
    /// further, U.layer - V.layer >= LinkMinLength(L).
    /// This method can be overridden to customize how nodes are assigned layers.
    /// </summary>
    /// <remarks>
    /// By default this just calls <see cref="LongestPathSinkLayering"/>,
    /// <see cref="LongestPathSourceLayering"/>, or
    /// <see cref="OptimalLinkLengthLayering"/> as appropriate given
    /// the <see cref="LayeringOption"/>.
    /// </remarks>
    protected virtual void AssignLayers() {
      switch (this.LayeringOption) {
        case LayeredDigraphLayering.LongestPathSink:
          LongestPathSinkLayering();
          break;
        case LayeredDigraphLayering.LongestPathSource:
          LongestPathSourceLayering();
          break;
        default:
        case LayeredDigraphLayering.OptimalLinkLength:
          OptimalLinkLengthLayering();
          break;
      }
    }

    /// <summary>
    /// Assigns every node in the input network to a layer.
    /// In addition to the requirements described in AssignLayers(),
    /// LongestPathSinkLayering ensures that every sink appears in layer 0
    /// and every node is as close to a sink as possible.
    /// </summary>
    protected virtual void LongestPathSinkLayering() {
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        int layer = LongestPathSinkLayeringLength(vertex);      
        maxLayer = Math.Max(layer, maxLayer);
      }
    }
  
    /// <summary>
    /// Computes the length of the longest path from node to a sink node and sets the
    /// layer of node to that length.
    /// Returns the length of the longest path from node to a sink node.
    /// </summary>
    /// <param name="v"></param>
    /// <returns>Returns the length of the longest path from node to a sink node.</returns>
    protected virtual int LongestPathSinkLayeringLength(LayeredDigraphVertex v) {
      int length = 0;
      if (v.Layer == -1) {
        foreach (LayeredDigraphEdge edge in v.DestinationEdgesList) {
          int minLength = LinkMinLength(edge);
          length = Math.Max(length, LongestPathSinkLayeringLength(edge.ToVertex) + minLength);
        }
        v.Layer = length;
      } else {
        length = v.Layer;
      }

      return length;
    }
  
  
    /********************************************************************************************/

    /// <summary>
    /// Assigns every node in the input network to a layer.
    /// In addition to the requirements described in <see cref="LayeredDigraphLayout.AssignLayers"/>,
    /// LongestPathSourceLayering ensures that every source appears in layer maxLayer
    /// and every node is as close to a source as possible.
    /// </summary>
    protected virtual void LongestPathSourceLayering() {
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        int layer = LongestPathSourceLayeringLength(vertex);      
        maxLayer = Math.Max(layer, maxLayer);
      }

      // Restore the correct orientation of the network, 
      //  since LongestPathSourceLayeringLength assigns layers "backwards".
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        vertex.Layer = maxLayer - vertex.Layer;
      }
    }
  
    /// <summary>
    /// Computes the length of the longest path from node to a source node and sets the
    /// layer of node to that length.
    /// Returns the length of the longest path from node to a source node.
    /// </summary>
    /// <param name="v"></param>
    /// <returns>Returns the length of the longest path from node to a source node.</returns>
    protected virtual int LongestPathSourceLayeringLength(LayeredDigraphVertex v) {
      int length = 0;
      if (v.Layer == -1) {
        foreach (LayeredDigraphEdge edge in v.SourceEdgesList) {
          int minLength = LinkMinLength(edge);
          length = Math.Max(length, LongestPathSourceLayeringLength(edge.FromVertex) + minLength);
        }
        v.Layer = length;
      } else {
        length = v.Layer;
      }

      return length;
    }


    /********************************************************************************************/

    /// <summary>
    /// Assigns every node in the input network to a layer.
    /// In addition to the requirements described in AssignLayers(),
    /// OptimalLinkLengthLayering ensures that nodes are set in layers
    /// to minimize the total weighted link length.
    /// Hence, OptimalLinkLengthLayering minimizes the sum
    /// (U.layer - V.layer) * LinkLengthWeight(L)
    /// over all links L = (U,V).
    /// </summary>
    /// <remarks>See also <seealso cref="LayeredDigraphLayout.LinkMinLength"/>
    /// and <seealso cref="LayeredDigraphLayout.LinkLengthWeight"/></remarks>
    protected virtual void OptimalLinkLengthLayering() {
      // Begin by performing a LongestPathSinkLayering.
      // This ensures that all sinks are in layer 0 and evert node
      //  is as close to a sink as possible.
      LongestPathSinkLayering();
    
      // Every node is initially invalid -- i.e., it hasn't been reached by 
      //  the depth first search.
      // This is mainly an optimization -- depth first searching through a
      //  portion of the network that is already valid will not change
      //  the layering, but it will spend the time calculating that nothing needs
      //  to be done.  By only examining invalid nodes, the running time improves.
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        vertex.Valid = false;
      }
    
      // Perform a depth first search from the sources of the network.
      // At the conclusion of this depth first search, all nodes have been
      //  moved into their appropriate layers relative to their decendents.
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        if (vertex.SourceEdgesList.Count == 0) {
          OptimalLinkLengthLayeringDepthFirstSearch(vertex);
        }
      }
    
      // However, the entire network may have been shifted.
      // Find the minimum layer.
      int minLayer = int.MaxValue;
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        minLayer = Math.Min(minLayer, vertex.Layer);
      }
      // And move evey node so that the minimum layer is now layer 0.
      // Also recompute the maxLayer, since nodes may have been pushed.
      maxLayer = -1;
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        vertex.Layer -= minLayer;
        maxLayer = Math.Max(maxLayer, vertex.Layer);
      }
    }

    /// <summary>
    /// Peforms the depth first search of the network.
    /// After traversing all decendents, the node is "pull"-ed into
    /// the appropriate layer.
    /// </summary>
    /// <param name="v"></param>
    /// <seealso cref="LayeredDigraphLayout.OptimalLinkLengthLayeringPull"/>
    protected virtual void OptimalLinkLengthLayeringDepthFirstSearch(LayeredDigraphVertex v) {
      if (!(v.Valid)) {
        v.Valid = true;
        foreach (LayeredDigraphEdge edge in v.DestinationEdgesList) {
          LayeredDigraphVertex succVertex = edge.ToVertex;
          OptimalLinkLengthLayeringDepthFirstSearch(succVertex);
        }
        OptimalLinkLengthLayeringPull(v);
        OptimalLinkLengthLayeringPush(v);
      }
    }

    /// <summary>
    /// Attempts to move node and it's tight component to a higher layer.
    /// </summary>
    /// <param name="v"></param>
    protected virtual void OptimalLinkLengthLayeringPull(LayeredDigraphVertex v) {
      // Every node is initially unset -- i.e., it hasn't been assigned to 
      //  a component.
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        vertex.Component = -1;
      }

      // Constants for parent and child components.
      const int parentComponent = 0;
      const int childComponent = 1;

      // Form the tight forward component of node's non-tight parents.
      // The idea is to exclude from node's component all those nodes which
      //  are already tight with respect to the parents of node, since
      //  node will move towards it's non-tight parents.
      foreach (LayeredDigraphEdge predEdge in v.SourceEdgesList) {
        int minLength = LinkMinLength(predEdge);
        int length = predEdge.FromVertex.Layer - predEdge.ToVertex.Layer;
        if (length > minLength) {
          TightComponentUnset(predEdge.FromVertex, parentComponent, -1, true, false);
        }
      }

      // Form the tight component of node which excludes nodes in the 
      //  parent component.
      // The tight component of node is the set of nodes which
      //  are decendents or ancestors of node and have a path of "tight" 
      //  links from or to node.
      // A link L from node U to node V is tight if
      //  U.layer - V.layer == LinkMinLength(L)
      // Hence, if node can be shifted to a higher layer, all of these
      //  nodes must also be shifted.
      TightComponentUnset(v, childComponent, -1, true, true);

      // If node is in it's parent component, then it cannot be pulled 
      //  (it would violate some LinkMinLength), 
      //  or it has been cast out of the childComponent and a pull
      //  would not reduce the weighted sum of the links in the network.
      while (v.Component != parentComponent) {
        // Compute the difference in the weights of links into the childComponent and
        //  links out of the childCcomponent.
        double totalDiff = 0;
        // Keep track of the maximum ammount by which the component can be shifted.
        int shift = int.MaxValue;
        // Also compute the node in the childComponent which satisfies the following:
        //  the node has no children in the childComponent
        //  the node minimizes the weighted difference of links into the node
        //   and links out of the node (i.e., the node is "best" in the 
        //   sense that it will most increase the totalDiff next time through
        //   the loop.
        double minDiff = 0;
        LayeredDigraphVertex minDiffVertex = null;

        // Step through all nodes in the network.
        foreach (LayeredDigraphVertex childCompVertex in this.Network.Vertexes) {
          if (childCompVertex.Component == childComponent) {
            // Compute the weighted difference of links into the node
            //  and links out of the node. 
            // Also record if the node has no children in the childComponent.
            double diff = 0;
            bool children = false;

            // Links into the node.
            foreach (LayeredDigraphEdge predEdge in childCompVertex.SourceEdgesList) {
              LayeredDigraphVertex predVertex = predEdge.FromVertex;
              // Update the node difference.
              diff += LinkLengthWeight(predEdge);
              if (predVertex.Component != childComponent) {
                // Update the total difference.
                totalDiff += LinkLengthWeight(predEdge);
                // Update shift if the link has the limiting length.
                int length = predVertex.Layer - childCompVertex.Layer;
                int minLength = LinkMinLength(predEdge);
                shift = Math.Min(shift, length - minLength);
              }
            }

            // Links out of the node.
            foreach (LayeredDigraphEdge succEdge in childCompVertex.DestinationEdgesList) {
              LayeredDigraphVertex succVertex = succEdge.ToVertex;
              // Update the node difference.
              diff -= LinkLengthWeight(succEdge);
              if (succVertex.Component != childComponent) {
                // Update the total difference.
                totalDiff -= LinkLengthWeight(succEdge);
              } else { 
                // Update the children.
                children = true;
              }
            }
  
            // Update the minimum difference node.
            if (((minDiffVertex == null) || (diff < minDiff)) && (!children)) {
              minDiffVertex = childCompVertex;
              minDiff = diff;
            }
          }
        }

        // If the sum of the weights of links into the component is greater than
        //  the sum of the weights out of the component, then shifting the 
        //  component to a higher level will decrease the weighted sum of the 
        //  links in the network.
        // If not, cast out the pMinDiffNode (which will have the greatest effect
        //  in increasing the totalDiff next time through) and recompute.
        if (totalDiff > 0) {
          foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
            if (vertex.Component == childComponent) {
              vertex.Layer += shift;
              // Since the node was shifted, invalidate it so that additional shifts can be made.
              // pNod.Valid = false;
            }
          }
          // Break the while loop.
          v.Component = parentComponent;
        } else {
          minDiffVertex.Component = parentComponent;
        }
      }
    }


    /********************************************************************************************/

    /// <summary>
    /// Attempts to move node and it's tight component to a lower layer.
    /// </summary>
    /// <param name="v"></param>
    protected virtual void OptimalLinkLengthLayeringPush(LayeredDigraphVertex v) {
      // Every node is initially unset -- i.e., it hasn't been assigned to 
      //  a component.
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        vertex.Component = -1;
      }
    
      // Constants for parent and child components.
      const int parentComponent = 0;
      const int childComponent = 1;
    
      // Form the tight forward component of node.
      // The tight forward component of node is the set of nodes which
      //  are decendents of node and have a path of "tight" 
      //  links from or to node.
      // A link L from node U to node V is tight if
      //  U.layer - V.layer == LinkMinLength(L)
      // Hence, if node can be shifted to a lower layer, all of these
      //  nodes must also be shifted.
      TightComponentUnset(v, childComponent, -1, true, false);
    
      // If node is in it's parent component, then it has been cast out of the 
      //  childComponent and a push would not reduce the weighted sum of the 
      //  links in the network.
      while (v.Component != parentComponent) {
        // Compute the difference in the weights of links into the childComponent and
        //  links out of the childCcomponent.
        double totalDiff = 0;
        // Keep track of the maximum ammount by which the component can be shifted.
        int shift = int.MaxValue;
        // Also compute the node in the childComponent which satisfies the following:
        //  the node has no parents in the childComponent
        //  the node maximizes the weighted difference of links into the node
        //   and links out of the node (i.e., the node is "best" in the 
        //   sense that it will most decrease the totalDiff next time through
        //   the loop.
        double maxDiff = 0;
        LayeredDigraphVertex maxDiffVertex = null;
        // Step through all nodes in the network.
        foreach (LayeredDigraphVertex childCompVertex in this.Network.Vertexes) {
          if (childCompVertex.Component == childComponent) {
            // Compute the weighted difference of links into the node
            //  and links out of the node. 
            // Also record if the node has no children in the childComponent.
            double diff = 0;
            bool parents = false;
          
            // Links into the node.
            foreach (LayeredDigraphEdge predEdge in childCompVertex.SourceEdgesList) {
              LayeredDigraphVertex predVertex = predEdge.FromVertex;
              // Update the node difference.
              diff += LinkLengthWeight(predEdge);
              if (predVertex.Component != childComponent) {
                // Update the total difference.
                totalDiff += LinkLengthWeight(predEdge);
              } else {
                // Update the parents.
                parents = true;
              }
            }
            // Links out of the node.
            foreach (LayeredDigraphEdge succEdge in childCompVertex.DestinationEdgesList) {
              LayeredDigraphVertex succVertex = succEdge.ToVertex;
              // Update the node difference.
              diff -= LinkLengthWeight(succEdge);
              if (succVertex.Component != childComponent) {
                // Update the total difference.
                totalDiff -= LinkLengthWeight(succEdge);
                // Update shift if the link has the limiting length.
                int length = childCompVertex.Layer - succVertex.Layer;
                int minLength = LinkMinLength(succEdge);
                shift = Math.Min(shift, length - minLength);
              }
            }
          
            // Update the maximum difference node.
            if (((maxDiffVertex == null) || (diff > maxDiff)) && (!parents)) {
              maxDiffVertex = childCompVertex;
              maxDiff = diff;
            }
          }
        }
      
        // If the sum of the weights of links into the component is greater than
        //  the sum of the weights out of the component, then shifting the 
        //  component to a higher level will decrease the weighted sum of the 
        //  links in the network.
        // If not, cast out the pMinDiffNode (which will have the greatest effect
        //  in increasing the totalDiff next time through) and recompute.
        if (totalDiff < 0) {
          foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
            if (vertex.Component == childComponent) {
              vertex.Layer -= shift;
              // Since the node was shifted, invalidate it so that additional shifts can be made.
              //node.Valid = false;
            }
          }
          // Break the while loop.
          v.Component = parentComponent;
        } else {
          maxDiffVertex.Component = parentComponent;
        }
      }
    }



    /********************************************************************************************/

    /// <summary>
    /// Converts the input network into a proper digraph; i.e., artificial nodes and links
    /// are introduced into the network such that every link is between nodes in adjacent
    /// layers.  This has the effect of breaking up long links into a sequence of artificial
    /// nodes.
    /// </summary>
    /// <remarks>
    /// This method must not change the layer of any existing node, nor add or remove any layers.
    /// </remarks>
    protected virtual void MakeProper() {
      // Initially, all links are invalid.
      // As links are examined, they are either validated or a sequence of 
      //  valid links is added to represent long list.
      LayeredDigraphNetwork.EdgeList linkarray = this.Network.EdgesArray;  // make temp copy of list of links
      for (int li = 0; li < linkarray.Count; li++) {
        linkarray[li].Valid = false;
      }
    
      // Since we are adding links to the network as we examine links,
      //  only calculate ports on invalid links.  This will be the first
      //  set of links, which correspond to "real" links in the network.
      for (int i = 0; i < linkarray.Count; i++) {
        LayeredDigraphEdge edge = linkarray[i];
        if (edge.Valid) continue;
        if ((edge.FromVertex.Node == null || edge.ToVertex.Node == null) && edge.FromVertex.Layer == edge.ToVertex.Layer) continue;
        // Set the portFromColOffset and portToColOffset and portFromPos and portToPos.
        // This allows the crossing matrix to correctly calculate the crossings
        //  for nodes with multiple ports and assists in straightening.
        int portFromColOffset, portToColOffset;
        int portFromPos, portToPos;
        portFromColOffset = portToColOffset = portFromPos = portToPos = 0;
        if (edge.Link != null) {
          Link reallink = edge.Link;
          if (reallink == null) continue;
          Node realFromNode = edge.FromVertex.Node;
          Node realToNode = edge.ToVertex.Node;
          if (realFromNode == null || realToNode == null) continue;
          Node fromNode = reallink.FromNode;
          Node toNode = reallink.ToNode;
          FrameworkElement fromPort = reallink.FromPort;
          FrameworkElement toPort = reallink.ToPort;
          if (edge.Rev) {
            Node dummy = fromNode;
            FrameworkElement dummyelt = fromPort;
            fromNode = toNode;
            fromPort = toPort;
            toNode = dummy;
            toPort = dummyelt;
          }
          Point nodeFromPt = edge.FromVertex.Focus;
          Point nodeToPt = edge.ToVertex.Focus;
          Rect fromR = fromNode.Bounds;
          Point portFromPt = Double.IsNaN(fromR.X) ? nodeFromPt : fromNode.GetRelativeElementPoint(fromPort, Spot.Center);
          if (realFromNode != fromNode && !Double.IsNaN(fromR.X) && fromNode.Visible) {
            Rect realFromR = realFromNode.Bounds;
            if (!Double.IsNaN(realFromR.X)) {
              portFromPt.X += fromR.X-realFromR.X;
              portFromPt.Y += fromR.Y-realFromR.Y;
            }
          }
          Rect toR = toNode.Bounds;
          Point portToPt = Double.IsNaN(toR.X) ? nodeToPt : toNode.GetRelativeElementPoint(toPort, Spot.Center);
          if (realToNode != toNode && !Double.IsNaN(toR.X) && toNode.Visible) {
            Rect realToR = realToNode.Bounds;
            if (!Double.IsNaN(realToR.X)) {
              portToPt.X += toR.X-realToR.X;
              portToPt.Y += toR.Y-realToR.Y;
            }
          }
          if (this.Direction == 90 || this.Direction == 270) {
            portFromColOffset = (int)(Math.Round((double)(portFromPt.X - nodeFromPt.X) / this.ColumnSpacing));
            portFromPos = (int)portFromPt.X;
            portToColOffset = (int)(Math.Round((double)(portToPt.X - nodeToPt.X) / this.ColumnSpacing));
            portToPos = (int)portToPt.X;
          } else {
            portFromColOffset = (int)(Math.Round((double)(portFromPt.Y - nodeFromPt.Y) / this.ColumnSpacing));
            portFromPos = (int)portFromPt.Y;
            portToColOffset = (int)(Math.Round((double)(portToPt.Y - nodeToPt.Y) / this.ColumnSpacing));
            portToPos = (int)portToPt.Y;
          }
          edge.PortFromColOffset = portFromColOffset;
          edge.PortFromPos = portFromPos;
          edge.PortToColOffset = portToColOffset;
          edge.PortToPos = portToPos;
        } else {
          edge.PortFromColOffset = 0;
          edge.PortFromPos = 0;
          edge.PortToColOffset = 0;
          edge.PortToPos = 0;
        }

        LayeredDigraphVertex vertexFrom = edge.FromVertex;
        LayeredDigraphVertex vertexTo = edge.ToVertex;
        int layerFrom = vertexFrom.Layer;
        int layerTo = vertexTo.Layer;
        bool aroundfromnode = false;
        bool aroundtonode = false;
        GoesAround(edge, out aroundfromnode, out aroundtonode);
        // add an artifical node in the to-node's layer when a reversed link is coming back around
        if (aroundtonode) {
          //// Add an artificial node at the appropriate layer.
          LayeredDigraphVertex vertexA = this.Network.CreateVertex();
          vertexA.Node = null;
          vertexA.ArtificialType = 1;
          vertexA.Layer = layerFrom;
          vertexA.Near = vertexFrom;
          this.Network.AddVertex(vertexA);
          // Link up the artifical node to its parent.
          LayeredDigraphEdge edgeA = this.Network.LinkVertexes(vertexFrom, vertexA, edge.Link);
          // not between adjacent layers
          edgeA.Valid = false;
          // Inherit the reversed status of the "real" link.
          edgeA.Rev = edge.Rev;
          edgeA.PortFromColOffset = portFromColOffset;
          edgeA.PortToColOffset = 0;
          edgeA.PortFromPos = portFromPos;
          edgeA.PortToPos = 0;
          vertexFrom = vertexA;
        }
        int loopback = 1;
        if (aroundfromnode) loopback--;
        if (layerFrom - layerTo > loopback && layerFrom > 0) {
          // Pointers to the "artifical" nodes and links added.
          LayeredDigraphVertex vertexA;
          LayeredDigraphEdge edgeA;
          // Invalidate the "real" link.  The remainder of the layout algorithm will
          //  only work on "valid" links, which will be links between adjacent layers.
          edge.Valid = false;
          // Add the first artificial node at the appropriate layer.
          vertexA = this.Network.CreateVertex();
          vertexA.Node = null;
          vertexA.ArtificialType = 2;
          vertexA.Layer = layerFrom - 1;
          this.Network.AddVertex(vertexA);
          // Link up the artifical node to its parent.
          edgeA = this.Network.LinkVertexes(vertexFrom, vertexA, edge.Link);
          // Validate the new link, since it is between adjacent layers.
          edgeA.Valid = true;
          // Inherit the reversed status of the "real" link.
          edgeA.Rev = edge.Rev;
          // The portFromColOffset is inherited from the "real" link,
          //  but portToColOffset is to an artifical node, so is 0.
          edgeA.PortFromColOffset = (aroundtonode ? 0 : portFromColOffset);
          edgeA.PortToColOffset = 0;
          edgeA.PortFromPos = (aroundtonode ? 0 : portFromPos);
          edgeA.PortToPos = 0;
          // Make the new artifical node the from node and loop.
          vertexFrom = vertexA;
          layerFrom--;
          // Add the intermediate artifical nodes.
          while (layerFrom - layerTo > loopback && layerFrom > 0) {
            // Add an artificial node at the appropriate layer.
            vertexA = this.Network.CreateVertex();
            vertexA.Node = null;
            vertexA.ArtificialType = 3;
            vertexA.Layer = layerFrom - 1;
            this.Network.AddVertex(vertexA);
            // Link up the artifical node to its parent.
            edgeA = this.Network.LinkVertexes(vertexFrom, vertexA, edge.Link);
            // Validate the new link, since it is between adjacent layers.
            edgeA.Valid = true;
            // Inherit the reversed status of the "real" link.
            edgeA.Rev = edge.Rev;
            // The portFromColOffset and portToColOffset are to artificial nodes,
            //  so are 0.
            edgeA.PortFromColOffset = 0;
            edgeA.PortToColOffset = 0;
            edgeA.PortFromPos = 0;
            edgeA.PortToPos = 0;
            // Make the new artifical node the from node and loop.
            vertexFrom = vertexA;
            layerFrom--;
          }
        
          // Link up the last artifical node to the "real" to node.
          edgeA = this.Network.LinkVertexes(vertexA, vertexTo, edge.Link);
          // Validate the new link, since it may be between adjacent layers.
          edgeA.Valid = !aroundfromnode;
          if (aroundfromnode) vertexA.Near = vertexTo;
          // Inherit the reversed status of the "real" link.
          edgeA.Rev = edge.Rev;
          // The portToColOffset is inherited from the "real" link,
          //  but portFromColOffset is to an artifical node, so is 0.
          edgeA.PortFromColOffset = 0;
          edgeA.PortToColOffset = portToColOffset;
          edgeA.PortFromPos = 0;
          edgeA.PortToPos = portToPos;
        } else {
          // The "real" link is between adjacent layers, so validate it.
          edge.Valid = true;
        }    
      }
    }

    void GoesAround(LayeredDigraphEdge edge, out bool aroundfromnode, out bool aroundtonode) {
      aroundfromnode = false;
      aroundtonode = false;
      Route pLinkR = edge.Route;
      if (pLinkR != null /* && (pLinkR.Style == GoStrokeStyle.Bezier || pLinkR.Orthogonal) */) {
        FrameworkElement gofromport = pLinkR.Link.FromPort;
        if (gofromport != null) {
          Node gofromnode = pLinkR.Link.FromNode;
          if (pLinkR.GetFromSpot() == OppositeSpotDir(edge, true)) {
            aroundfromnode = true;
          } else if (this.SetsPortSpots && gofromnode != null && gofromnode.Ports.Count() == 1) {
            aroundfromnode = edge.Rev;
          }
        }
        FrameworkElement gotoport = pLinkR.Link.ToPort;
        if (gotoport != null) {
          Node gotonode = pLinkR.Link.ToNode;
          if (pLinkR.GetToSpot() == OppositeSpotDir(edge, false)) {
            aroundtonode = true;
          } else if (this.SetsPortSpots && gotonode != null && gotonode.Ports.Count() == 1) {
            aroundtonode = edge.Rev;
          }
        }
      }
    }

    Spot OppositeSpotDir(LayeredDigraphEdge edge, bool from) {
      if (this.Direction == 90) {
        if (from && !edge.Rev || !from && edge.Rev)
          return Spot.TopCenter;
        else
          return Spot.BottomCenter;
      } else if (this.Direction == 180) {
        if (from && !edge.Rev || !from && edge.Rev)
          return Spot.MiddleRight;
        else
          return Spot.MiddleLeft;
      } else if (this.Direction == 270) {
        if (from && !edge.Rev || !from && edge.Rev)
          return Spot.BottomCenter;
        else
          return Spot.TopCenter;
      } else {
        if (from && !edge.Rev || !from && edge.Rev)
          return Spot.MiddleLeft;
        else
          return Spot.MiddleRight;
      }
    }
  
    /********************************************************************************************/

    void InitializeIndicesInternal() {
      indices = new int[maxLayer+1];
      for (int layer = 0; layer <= maxLayer; layer++) {
        indices[layer] = 0;
      }
      // Every node is initially unset -- i.e., it hasn't been assigned an index.
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        vertex.Index = -1;
      }

      InitializeIndices();
      
      maxIndex = -1;
      minIndexLayer = 0;
      maxIndexLayer = 0;
      for (int layer = 0; layer <= maxLayer; layer++) {
        if (indices[layer] > indices[maxIndexLayer]) {
          maxIndex = indices[layer] - 1;
          maxIndexLayer = layer;
        }
        if (indices[layer] < indices[minIndexLayer]) {
          minIndexLayer = layer;
        }
      }

      // organize all nodes by layer, for faster access
      myLayers = new LayeredDigraphVertex[maxLayer+1][];
      for (int i = 0; i < indices.Length; i++) {
        myLayers[i] = new LayeredDigraphVertex[indices[i]];
      }
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        int layer = vertex.Layer;
        LayeredDigraphVertex[] v = myLayers[layer];
        v[vertex.Index] = vertex;
      }










    }
  
    /// <summary>
    /// Assigns every node in the input network an index number,
    /// such that nodes in the same layer will be labeled with
    /// consecutive indices in left to right order.
    /// All consecutive layout operations will preserve or update
    /// the indices.
    /// In addition, the indices array is initialized such that
    /// indices[layer] indicates the number of nodes in the layer.
    /// Finally, the variables minIndexLayer and maxIndexLayer record
    /// the layers that correspond to the minimum and maximum nodes
    /// in a layer.
    /// </summary>
    /// <remarks>
    /// By default this will just call <see cref="NaiveInitializeIndices"/>,
    /// <see cref="DepthFirstOutInitializeIndices"/>, or
    /// <see cref="DepthFirstInInitializeIndices"/>, as appropriate
    /// given the value of <see cref="InitializeOption"/>.
    /// This method must not change the layer of any existing node, nor add or remove any layers.
    /// </remarks>
    protected virtual void InitializeIndices() {
      switch (this.InitializeOption) {
        default:
        case LayeredDigraphInitIndices.Naive:
          NaiveInitializeIndices();
          break;
        case LayeredDigraphInitIndices.DepthFirstOut:
          DepthFirstOutInitializeIndices(); 
          break;
        case LayeredDigraphInitIndices.DepthFirstIn:
          DepthFirstInInitializeIndices(); 
          break;
      }
    }
  
    /********************************************************************************************/

    /// <summary>
    /// Assigns every node in the input network an index number,
    /// such that nodes in the same layer will be labeled with
    /// consecutive indices in left to right order.
    /// Uses a naive implementation that assigns indices to nodes as they
    /// are encountered in a sweep of the network.  Because of the way
    /// networks are stored, this has the effect of initialy placing all
    /// "artificial" nodes to the right of all "real" nodes.
    /// </summary>
    protected virtual void NaiveInitializeIndices() {
      int layer;
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        layer = vertex.Layer;
        vertex.Index = indices[layer];
        indices[layer]++;
      }
    }
  
  
    /********************************************************************************************/
  
    /// <summary>
    /// Assigns every node in the input network an index number,
    /// such that nodes in the same layer will be labeled with 
    /// consecutive indices in left to right order.
    /// Uses a depth first "outward" (i.e., following links from "from-node" to "to-node")
    /// traversal of the network, assigning indices to nodes as they are discovered.
    /// </summary>
    protected virtual void DepthFirstOutInitializeIndices() {
      int layer;
      // Begin at maxLayer and sweep towards 0.
      // This ensures that the sources are discovered in the correct order.
      for (layer = maxLayer; layer >= 0; layer--) {
        foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
          if ((vertex.Layer == layer) && 
            (vertex.Index == -1)) {
            DepthFirstOutInitializeIndicesVisit(vertex);
          }
        }
      }
    }
  
    /// <summary>
    /// Assigns node the appropriate index and updates the indices array.
    /// Implements the recursive portion of a depth first search.
    /// </summary>
    /// <param name="v"></param>
    protected virtual void DepthFirstOutInitializeIndicesVisit(LayeredDigraphVertex v) {
      int layer = v.Layer;
      v.Index = indices[layer];
      indices[layer]++;
    
      LayeredDigraphNetwork.EdgeList DestinationLinksList = v.DestinationEdgesList;
      // Copy the DestinationLinksList into SuccLinksArray and sort on portFromPos.
      // This ensures that multi-port nodes initialize children in the correct order.
      LayeredDigraphEdge[] SuccLinksArray = DestinationLinksList.ToArray();
      bool exchange = true;
      while (exchange) {
        exchange = false;
        for (int indexS = 0; indexS < (SuccLinksArray.Length - 1); indexS++) {
          LayeredDigraphEdge edgeL = SuccLinksArray[indexS];
          LayeredDigraphEdge edgeR = SuccLinksArray[indexS+1];
          if (edgeL.PortFromColOffset > edgeR.PortFromColOffset) {
            exchange = true;
            SuccLinksArray[indexS] = edgeR;
            SuccLinksArray[indexS+1] = edgeL;
          }
        }
      }
    
      for (int indexS = 0; indexS < SuccLinksArray.Length; indexS++) {
        LayeredDigraphEdge edge = SuccLinksArray[indexS];
        if (edge.Valid) {
          LayeredDigraphVertex vertexS = edge.ToVertex;
          if (vertexS.Index == -1) {
            DepthFirstOutInitializeIndicesVisit(vertexS);
          }
        }
      }
    }
    
    /// <summary>
    /// Assigns every node in the input network an index number,
    /// such that nodes in the same layer will be labeled with
    /// consecutive indices in left to right order.
    /// Uses a depth first "inward" (i.e., following links from "to-node" to "from-node")
    /// traversal of the network, assigning indices to nodes as they are discovered.
    /// </summary>
    protected virtual void DepthFirstInInitializeIndices() {
      int layer;
      // Begin at 0 and sweep towards maxLayer.
      // This ensures that the sinks are discovered in the correct order.
      for (layer = 0; layer <= maxLayer; layer++) {
        foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
          if ((vertex.Layer == layer) && 
            (vertex.Index == -1)) {
            DepthFirstInInitializeIndicesVisit(vertex);
          }
        }
      }
    }
  
    /// <summary>
    /// Assigns node the appropriate index and updates the indices array.
    /// Implements the recursive portion of a depth first search.
    /// </summary>
    /// <param name="v"></param>
    protected virtual void DepthFirstInInitializeIndicesVisit(LayeredDigraphVertex v) {
      int layer = v.Layer;
      v.Index = indices[layer];
      indices[layer]++;
    
      LayeredDigraphNetwork.EdgeList SourceLinksList = v.SourceEdgesList;
      // Copy the SourceLinksList into PredLinksArray and sort on portToPos.
      // This ensures that multi-port nodes initialize parents in the correct order.
      LayeredDigraphEdge[] PredLinksArray = SourceLinksList.ToArray();
      bool exchange = true;
      while (exchange) {
        exchange = false;
        for (int indexP = 0; indexP < (PredLinksArray.Length - 1); indexP++) {
          LayeredDigraphEdge edgeL = PredLinksArray[indexP];
          LayeredDigraphEdge edgeR = PredLinksArray[indexP+1];
          if (edgeL.PortToColOffset > edgeR.PortToColOffset) {
            exchange = true;
            PredLinksArray[indexP] = edgeR;
            PredLinksArray[indexP+1] = edgeL;
          }
        }
      }
    
      for (int indexP = 0; indexP < PredLinksArray.Length; indexP++) {
        LayeredDigraphEdge edge = PredLinksArray[indexP];
        if (edge.Valid) {
          LayeredDigraphVertex vertexP = edge.FromVertex;
          if (vertexP.Index == -1) {
            DepthFirstInInitializeIndicesVisit(vertexP);
          }
        }
      }
    }


  
    /********************************************************************************************/

    /// <summary>
    /// Assigns every node in the input network a column number,
    /// such that nodes in the same layer will be labeled with
    /// increasing indices in left to right order.
    /// </summary>
    /// <remarks>
    /// In addition, a node U is assigned to a column such that
    /// 2 * MinColumnSpace(U) + 1 columns are "allocated" to node U,
    /// and no two nodes have overlapping "allocations" of columns.
    /// All consecutive layout operations will preserve or update
    /// the columns.
    /// This method can be overridden to customize the layout algorithm.
    /// This method must not change the layer of any existing node, nor add or remove any layers.
    /// </remarks>
    protected virtual void InitializeColumns() {
      maxColumn = -1;
      for (int layer = 0; layer <= maxLayer; layer++) {
        // Fill unfixedLayerNodes with nodes in the layer, ordered by index.
        LayeredDigraphVertex[] unfixedLayerNodes = GetCachedNodeArrayList(layer);

        // Assign columns to the unfixedLayerNodes such that "allocated" 
        //  columns do not overlap.
        int column = 0;
        for (int index = 0; index < indices[layer]; index++) {
          LayeredDigraphVertex vertex = unfixedLayerNodes[index];
          column += NodeMinColumnSpace(vertex, true);
          vertex.Column = column;
          column += 1;
          column += NodeMinColumnSpace(vertex, false);
        }

        maxColumn = Math.Max(maxColumn, column - 1);
        FreeCachedNodeArrayList(layer, unfixedLayerNodes);
      }
    }
  
  
  
    /********************************************************************************************/

    /// <summary>
    /// Reorders nodes within layers to reduce the total number of link
    /// crossings in the network.
    /// There are many, many possible implementations of this function.
    /// Basically, some iteration of MedianBarycenterCrossingReduction
    /// and AdjacentExchangeCrossingReductionBendStraighten
    /// sweeping back and forth over the layers is needed.
    /// The default implementation has performed favorably on a large number
    /// of networks, but other options are available.
    /// </summary>
    protected virtual void ReduceCrossings() {
      // Record the current number of crossings and the current layout.
      // These variables will be updated with the "best" crossings and
      //  layouts produced.
      int bestcrossings = CountCrossings();
      int[] bestlayout = SaveLayout();
    
      int count;
      int layer;
      int crossings;
    
      // The first sequence of sweeps.
      for (count = 0; count < this.Iterations; count++) {
        // Sweep from layer 0 to layer maxLayer,
        //  performing directed medianBarycenter and adjacentExchange
        //  crossing reductions on each layer.
        for (layer = 0; layer <= maxLayer; layer++) {
          MedianBarycenterCrossingReduction(layer, 1);
          AdjacentExchangeCrossingReductionBendStraighten(layer, 1, false, 1);
        }
        // Update the bestcrossings and bestlayout is an improvement was made.
        crossings = CountCrossings();
        if (crossings < bestcrossings) {
          bestcrossings = crossings;
          bestlayout = SaveLayout();
        }
        // Sweep from layer maxLayer to layer 0,
        //  performing directed medianBarycenter and adjacentExchange
        //  crossing reductions on each layer.
        for (layer = maxLayer; layer >= 0; layer--) {
          MedianBarycenterCrossingReduction(layer, -1);
          AdjacentExchangeCrossingReductionBendStraighten(layer, -1, false, -1);
        }
        // Update the bestcrossings and bestlayout is an improvement was made.
        crossings = CountCrossings();
        if (crossings < bestcrossings) {
          bestcrossings = crossings;
          bestlayout = SaveLayout();
        }
      }
      // Restore the bestlayout for subsequent operations.
      RestoreLayout(bestlayout);

      // The second sequence of sweeps.
      for (count = 0; count < this.Iterations; count++) {
        // Sweep from layer 0 to layer maxLayer,
        //  performing undirected medianBarycenter and adjacentExchange
        //  crossing reductions on each layer.
        for (layer = 0; layer <= maxLayer; layer++) {
          MedianBarycenterCrossingReduction(layer, 0);
          AdjacentExchangeCrossingReductionBendStraighten(layer, 0, false, 0);
        }
        // Update the bestcrossings and bestlayout is an improvement was made.
        crossings = CountCrossings();
        if (crossings < bestcrossings) {
          bestcrossings = crossings;
          bestlayout = SaveLayout();
        }
        // Sweep from layer maxLayer to layer 0,
        //  performing undirected medianBarycenter and adjacentExchange
        //  crossing reductions on each layer.
        for (layer = maxLayer; layer >= 0; layer--) {
          MedianBarycenterCrossingReduction(layer, 0);
          AdjacentExchangeCrossingReductionBendStraighten(layer, 0, false, 0);
        }
        // Update the bestcrossings and bestlayout is an improvement was made.
        crossings = CountCrossings();
        if (crossings < bestcrossings) {
          bestcrossings = crossings;
          bestlayout = SaveLayout();
        }
      }
      // Restore the bestlayout for subsequent operations.
      RestoreLayout(bestlayout);

      // A final sequence of operations.
      // The idea is that sometimes the bestlayout was found early in
      //  the iterations, and not every operation got a chance to permute it.
      // This final sequence takes the bestlayout and uses all the operations
      //  to move it towards a local minimum number of crossings.
      // If the aggressiveOption is set, then every sub-interval of layers
      //  is examined to reduce the crossings in the whole graph.  The idea
      //  is that some "twisted links" just need to be permuted in the right
      //  direction for the right number of layers.  By considering all of the
      //  sub-intervals, we run accross each one.  Of course, with N layers,
      //  that's N(N+1)/2 sub-intervals to consider, and we keep considering
      //  these sub-intervals until no crossings can be reduced.  Hence, we
      //  leave it as a option.
      // If the aggressiveOption is not set, then only the full interval of layers
      //  is examined.
      bool change;
      int topLayer;
      int botLayer;
      int lastcrossings;
      int recent;
      switch (this.AggressiveOption) {
        case LayeredDigraphAggressive.None: break;
        case LayeredDigraphAggressive.More:
          lastcrossings = bestcrossings + 1;
          while ((recent = CountCrossings()) < lastcrossings) {
            lastcrossings = recent;
            for (topLayer = maxLayer; topLayer >= 0; topLayer--) {
              for (botLayer = 0; botLayer <= topLayer; botLayer++) {
                change = true;
                while (change) {
                  change = false;
                  for (layer = topLayer; layer >= botLayer; layer--) {
                    change = AdjacentExchangeCrossingReductionBendStraighten(layer, -1, false, -1) || change;
                  }  
                }
                crossings = CountCrossings();
                if (crossings >= bestcrossings) {
                  RestoreLayout(bestlayout);
                } else {
                  bestcrossings = crossings;
                  bestlayout = SaveLayout();
                }
            
                change = true;
                while (change) {
                  change = false;
                  for (layer = topLayer; layer >= botLayer; layer--) {
                    change = AdjacentExchangeCrossingReductionBendStraighten(layer, 1, false, 1) || change;
                  }  
                }
                crossings = CountCrossings();
                if (crossings >= bestcrossings) {
                  RestoreLayout(bestlayout);
                } else {
                  bestcrossings = crossings;
                  bestlayout = SaveLayout();
                }
            
                change = true;
                while (change) {
                  change = false;
                  for (layer = botLayer; layer <= topLayer; layer++) {
                    change = AdjacentExchangeCrossingReductionBendStraighten(layer, 1, false, 1) || change;
                  }
                }
                if (crossings >= bestcrossings) {
                  RestoreLayout(bestlayout);
                } else {
                  bestcrossings = crossings;
                  bestlayout = SaveLayout();
                }
            
                change = true;
                while (change) {
                  change = false;
                  for (layer = botLayer; layer <= topLayer; layer++) {
                    change = AdjacentExchangeCrossingReductionBendStraighten(layer, -1, false, -1) || change;
                  }
                }
                if (crossings >= bestcrossings) {
                  RestoreLayout(bestlayout);
                } else {
                  bestcrossings = crossings;
                  bestlayout = SaveLayout();
                }
            
                change = true;
                while (change) {
                  change = false;
                  for (layer = topLayer; layer >= botLayer; layer--) {
                    change = AdjacentExchangeCrossingReductionBendStraighten(layer, 0, false, 0) || change;
                  }
                }
                if (crossings >= bestcrossings) {
                  RestoreLayout(bestlayout);
                } else {
                  bestcrossings = crossings;
                  bestlayout = SaveLayout();
                }
            
                change = true;
                while (change) {
                  change = false;
                  for (layer = botLayer; layer <= topLayer; layer++) {
                    change = AdjacentExchangeCrossingReductionBendStraighten(layer, 0, false, 0) || change;
                  }
                }
                if (crossings >= bestcrossings) {
                  RestoreLayout(bestlayout);
                } else {
                  bestcrossings = crossings;
                  bestlayout = SaveLayout();
                }
              }      
            }
          }
          break;
        default:
        case LayeredDigraphAggressive.Less:
          topLayer = maxLayer;
          botLayer = 0;
          lastcrossings = bestcrossings + 1;
          while ((recent = CountCrossings()) < lastcrossings) {
            lastcrossings = recent;
            change = true;
            while (change) {
              change = false;
              for (layer = topLayer; layer >= botLayer; layer--) {
                change = AdjacentExchangeCrossingReductionBendStraighten(layer, -1, false, -1) || change;
              }  
            }
            crossings = CountCrossings();
            if (crossings >= bestcrossings) {
              RestoreLayout(bestlayout);
            } else {
              bestcrossings = crossings;
              bestlayout = SaveLayout();
            }
            change = true;
            while (change) {
              change = false;
              for (layer = topLayer; layer >= botLayer; layer--) {
                change = AdjacentExchangeCrossingReductionBendStraighten(layer, 1, false, 1) || change;
              }  
            }
            crossings = CountCrossings();
            if (crossings >= bestcrossings) {
              RestoreLayout(bestlayout);
            } else {
              bestcrossings = crossings;
              bestlayout = SaveLayout();
            }
        
            change = true;
            while (change) {
              change = false;
              for (layer = botLayer; layer <= topLayer; layer++) {
                change = AdjacentExchangeCrossingReductionBendStraighten(layer, 1, false, 1) || change;
              }
            }
            if (crossings >= bestcrossings) {
              RestoreLayout(bestlayout);
            } else {
              bestcrossings = crossings;
              bestlayout = SaveLayout();
            }
            change = true;
            while (change) {
              change = false;
              for (layer = botLayer; layer <= topLayer; layer++) {
                change = AdjacentExchangeCrossingReductionBendStraighten(layer, -1, false, -1) || change;
              }
            }
            if (crossings >= bestcrossings) {
              RestoreLayout(bestlayout);
            } else {
              bestcrossings = crossings;
              bestlayout = SaveLayout();
            }
            change = true;
            while (change) {
              change = false;
              for (layer = topLayer; layer >= botLayer; layer--) {
                change = AdjacentExchangeCrossingReductionBendStraighten(layer, 0, false, 0) || change;
              }
            }
            if (crossings >= bestcrossings) {
              RestoreLayout(bestlayout);
            } else {
              bestcrossings = crossings;
              bestlayout = SaveLayout();
            }
            change = true;
            while (change) {
              change = false;
              for (layer = botLayer; layer <= topLayer; layer++) {
                change = AdjacentExchangeCrossingReductionBendStraighten(layer, 0, false, 0) || change;
              }
            }
            if (crossings >= bestcrossings) {
              RestoreLayout(bestlayout);
            } else {
              bestcrossings = crossings;
              bestlayout = SaveLayout();
            }
          }      
          break;
      }
      // Restore the bestlayout.
      RestoreLayout(bestlayout);
    }
  
  
  
    /********************************************************************************************/

    /// <summary>
    /// Reorders nodes within the unfixedLayer to reduce the number of link crossings between
    /// the unfixedLayer and its adjacent layers.  The direction argument indicates which of the
    /// adjacent layers should be taken into consideration when reducing the number of crossings.
    /// <c>direction == 0  --  use unfixedLayer - 1 and unfixedLayer + 1</c>
    /// <c>direction > 0  --  use unfixedLayer - 1 (sweeping away from layer 0)</c>
    /// <c>direction &lt; 0  --  use unfixedLayer + 1 (sweepeing towards layer 0)</c>
    /// The idea is to calculate the median and barycenter for each node in the unfixedLayer,
    /// and to sort the nodes in the unfixedLayer by their median and barycenter values.
    /// Returns true if some change was made to the layer.
    /// </summary>
    /// <param name="unfixedLayer"></param>
    /// <param name="direction"></param>
    /// <returns>Returns true if some change was made to the layer and false otherwise.</returns>
    protected virtual bool MedianBarycenterCrossingReduction(int unfixedLayer, int direction) {
      int index;
      bool layerChange = false;

      // Fill unfixedLayerNodes with nodes in the unfixedLayer, ordered by index.
      LayeredDigraphVertex[] unfixedLayerNodes = GetCachedNodeArrayList(unfixedLayer);
      int num = indices[unfixedLayer];

      // Initialize the medians and barycenters.
      double[] medians = Medians(unfixedLayer, direction);
      double[] barycenters = Barycenters(unfixedLayer, direction);

      // A median or barycenter value of -1 indicates that the node does not 
      //  have a defined median or barycenter (i.e., the node has no neighbors 
      //  in the appropriate adjacent layer).  In that case, use the current column
      //  value as the median or barycenter.
      for (index = 0; index < num; index++) {
        if (barycenters[index] == -1) {
          barycenters[index] = unfixedLayerNodes[index].Column;
        }
        if (medians[index] == -1) {
          medians[index] = unfixedLayerNodes[index].Column;
        }
      }

      // Sort nodes on medians and barycenters.
      // Nodes are permuted within the unfixedLayerNodes.
      bool change = true;
      while (change) {
        change = false;
        for (index = 0; index < (num - 1); index++) {
          if ((medians[index+1] < medians[index]) ||
            ((medians[index+1] == medians[index]) && (barycenters[index+1] < barycenters[index]))) {
            layerChange = true;
            change = true;
            double medianTemp = medians[index];
            medians[index] = medians[index+1];
            medians[index+1] = medianTemp;
            double barycenterTemp = barycenters[index];
            barycenters[index] = barycenters[index+1];
            barycenters[index+1] = barycenterTemp;
            LayeredDigraphVertex vertex = unfixedLayerNodes[index];
            unfixedLayerNodes[index] = unfixedLayerNodes[index+1];
            unfixedLayerNodes[index+1] = vertex;
          }
        }
      }

      // Update the index for each node in the unfixedLayer.
      for (index = 0; index < num; index++) {
        unfixedLayerNodes[index].Index = index;
      }
      // Update the column for each node in the unfixedLayer.
      int column = 0;
      for (index = 0; index < num; index++) {
        LayeredDigraphVertex vertex = unfixedLayerNodes[index];
        column += NodeMinColumnSpace(vertex, true);
        vertex.Column = column;
        column += 1;
        column += NodeMinColumnSpace(vertex, false);
      }
      // Return status of the unfixedLayer.
      FreeCachedNodeArrayList(unfixedLayer, unfixedLayerNodes);
      return layerChange;
    }
  



    /********************************************************************************************/

    /// <summary>
    /// Adjusts the columns of nodes within the unfixedLayer to simultaneously reduce the
    /// number of link crossings and the number of "bends" between the unfixedLayer and its
    /// adjacent layers between the unfixedLayer and its adjacent layers.
    /// The directionCR argument indicates which of the adjacent layers should be taken
    /// into consideration when reducing the number of link crossings.
    /// <code>
    /// direction == 0  --  use unfixedLayer - 1 and unfixedLayer + 1
    /// direction > 0  --  use unfixedLayer - 1 (sweeping away from layer 0)
    /// direction &lt; 0  --  use unfixedLayer + 1 (sweepeing towards layer 0)
    /// </code>
    /// The directionBS argument indicates which of the adjacent layers should be taken
    /// into consideration when reducing the number of bends.
    /// <code>
    /// direction == 0  --  use unfixedLayer - 1 and unfixedLayer + 1
    /// direction > 0  --  use unfixedLayer - 1 (sweeping away from layer 0)
    /// direction &lt; 0  --  use unfixedLayer + 1 (sweepeing towards layer 0)
    /// </code>
    /// The "weighted bend" between a node U and a node V connected by link L is calculated by
    /// <c>abs((U.column + L.portFromColOffset) - (V.column + L.portToColOffset)) * LinkStraightenWeight(L)</c>
    /// The LinkStraightenWeight attempts to give higher priority to links between
    /// "artificial" nodes; i.e., long links in the final layout will be straighter.
    /// The idea is to use a bubble-sort technique to exchange adjacent nodes whenever
    /// doing so reduces the number of link crossings or the number of bends.
    /// This function is used in both crossing reduction and bend straightening.
    /// Returns true if some change was made to the layer.
    /// </summary>
    /// <param name="unfixedLayer">the layer to be reordered</param>
    /// <param name="directionCR">indicates which adjacent layers should be taken into consideration when calculating the crossing matrix</param>
    /// <param name="straighten">indicates whether or not to reorder to nodes to straighten links</param>
    /// <param name="directionBS">indicates which adjacent layers should be taken into consideration when calculating the bends of a link</param>
    /// <returns>Returns true if some change was made to the layer and false otherwise.</returns>
    protected virtual bool AdjacentExchangeCrossingReductionBendStraighten(int unfixedLayer, int directionCR, bool straighten, int directionBS) {
      // Fill unfixedLayerNodes with nodes in the unfixedLayer, ordered by index.
      LayeredDigraphVertex[] unfixedLayerNodes = GetCachedNodeArrayList(unfixedLayer);
      int num = indices[unfixedLayer];
    
      // Initialize the crossing matrix such that 
      //  if index1 and index2 are the indices corresponding to two nodes on the 
      //  unfixedLayer and crossmat is the crossing matrix, then
      //   crossmat[index1 * indices[unfixedLayer] + index2]
      //  is the number of crossing that occur if the node corresponding to index1 is
      //  placed to the left of the node corresponding to index2.
      int[] crossmat = CrossingMatrix(unfixedLayer, directionCR);
    
      // Initialize the barycenters from the negative direction.
      double[] barycentersNeg;
      // If no straightening or direction > 0, 
      //  then set barycenters from the negative direction to -1.
      if ((!straighten) || (directionBS > 0)) {
        barycentersNeg = new double[num];
        for (int index = 0; index < num; index++) {
          barycentersNeg[index] = -1;
        }
      } else {
        barycentersNeg = Barycenters(unfixedLayer, -1);
      }
      // Initialize the barycenters from the positive direction.
      double[] barycentersPos;
      // If no straightening or direction < 0, 
      //  then set barycenters from the positive direction to -1.
      if ((!straighten) || (directionBS < 0)) {
        barycentersPos = new double[num];
        for (int index = 0; index < num; index++) {
          barycentersPos[index] = -1;
        }
      } else {
        barycentersPos = Barycenters(unfixedLayer, 1);
      }
    
      bool layerChange = false;
      bool exchange = true;
      while (exchange) {
        exchange = false;
        int index;
        for (index = 0; index < num - 1; index++) {
          int crossingLR = crossmat[unfixedLayerNodes[index].Index * num + unfixedLayerNodes[index + 1].Index];
          int crossingRL = crossmat[unfixedLayerNodes[index + 1].Index * num + unfixedLayerNodes[index].Index];
          // Calculate the crossing numbers of nodes on the same layer.
          int layerCrossingLR = 0;
          int layerCrossingRL = 0;
        
          // Column of the node to the left.
          int columnL = unfixedLayerNodes[index].Column;
          // Column of the node to the right.
          int columnR = unfixedLayerNodes[index + 1].Column;
          int indexSpaceLeft = NodeMinColumnSpace(unfixedLayerNodes[index], true);
          int indexSpaceRight = NodeMinColumnSpace(unfixedLayerNodes[index], false);
          int indexPlusOneSpaceLeft = NodeMinColumnSpace(unfixedLayerNodes[index+1], true);
          int indexPlusOneSpaceRight = NodeMinColumnSpace(unfixedLayerNodes[index+1], false);
          // Column of the node to the left after an exchange (takes into account nodeMinColumSpace of the two nodes).
          int columnLX = columnL - indexSpaceLeft + indexPlusOneSpaceLeft;
          // Column of the node to the right after an exchange (takes into account nodeMinColumSpace of the two nodes).
          int columnRX = columnR - indexSpaceRight + indexPlusOneSpaceRight;
          // Calculate the bend numbers if the left node (L) and the right node (R) are
          //  order L-R  or R-L.
          double bendLR = 0;
          double bendRL = 0;
          // Calculate the predecessor link list of the left node, based on direction 
          if (straighten && ((directionBS < 0) || (directionBS == 0))) {
            foreach (LayeredDigraphEdge edge in unfixedLayerNodes[index].SourceEdgesList) {
              if (edge.Valid &&
                (edge.FromVertex.Layer != unfixedLayer)) {
                // Update the bends.
                double weight = LinkStraightenWeight(edge);
                int portFromColOffset = edge.PortFromColOffset;
                int portToColOffset = edge.PortToColOffset;
                int fromCol = edge.FromVertex.Column;
                bendLR += (Math.Abs((columnL + portToColOffset) - (fromCol + portFromColOffset)) + 1) * weight;
                bendRL += (Math.Abs((columnRX + portToColOffset) - (fromCol + portFromColOffset)) + 1) * weight;
              }
            }
          }

          // Compute "crossings" of nodes on same layer.
          foreach (LayeredDigraphEdge edge in unfixedLayerNodes[index].SourceEdgesList) {
            if (edge.Valid &&
              (edge.FromVertex.Layer == unfixedLayer)) {
              LayeredDigraphVertex fromVertex = edge.FromVertex;
              int indexFrom = 0;
              while (unfixedLayerNodes[indexFrom] != fromVertex) indexFrom++;
              // Depending on where the from node falls,
              //  we increase the appropriate crossing value.
              // The different values indicate how "strongly" that particular
              //  ordering is desired.
              if (indexFrom < index) {
                layerCrossingLR += 2 * (index - indexFrom);
                layerCrossingRL += 2 * ((index + 1) - indexFrom);
              }
              if (indexFrom == index + 1) {
                layerCrossingLR += 1;
              }
              if (indexFrom > index + 1) {
                layerCrossingLR += 4 * (indexFrom - index);
                layerCrossingRL += 4 * (indexFrom - (index + 1));
              }
            }
          }
        
          // Calculate the successor link list of the left node, based on direction 
          if (straighten && ((directionBS > 0) || (directionBS == 0))) {
            foreach (LayeredDigraphEdge edge in unfixedLayerNodes[index].DestinationEdgesList) {
              if (edge.Valid &&
                (edge.ToVertex.Layer != unfixedLayer)) {
                // Update the bends.
                double weight = LinkStraightenWeight(edge);
                int portFromColOffset = edge.PortFromColOffset;
                int portToColOffset = edge.PortToColOffset;
                int toCol = edge.ToVertex.Column;
                bendLR += (Math.Abs((columnL + portFromColOffset) - (toCol + portToColOffset)) + 1) * weight;
                bendRL += (Math.Abs((columnRX + portFromColOffset) - (toCol + portToColOffset)) + 1) * weight;
              }
            }
          }

          // Compute "crossings" of nodes on same layer.
          foreach (LayeredDigraphEdge edge in unfixedLayerNodes[index].DestinationEdgesList) {
            if (edge.Valid &&
              (edge.ToVertex.Layer == unfixedLayer)) {
              LayeredDigraphVertex toVertex = edge.ToVertex;
              int indexTo = 0;
              while (unfixedLayerNodes[indexTo] != toVertex) indexTo++;
              // Depending on where the to node falls,
              //  we increase the appropriate crossing value.
              // The different values indicate how "strongly" that particular
              //  ordering is desired.
              if (indexTo == index + 1) {
                layerCrossingRL += 1;
              }
            }
          }
        
          // Calculate the predecessor link list of the right node, based on direction 
          if (straighten && ((directionBS < 0) || (directionBS == 0))) {
            foreach (LayeredDigraphEdge edge in unfixedLayerNodes[index+1].SourceEdgesList) {
              if (edge.Valid &&
                (edge.FromVertex.Layer != unfixedLayer)) {
                // Update the bends.
                double weight = LinkStraightenWeight(edge);
                int portFromColOffset = edge.PortFromColOffset;
                int portToColOffset = edge.PortToColOffset;
                int fromCol = edge.FromVertex.Column;
                bendLR += (Math.Abs((columnR + portToColOffset) - (fromCol + portFromColOffset)) + 1) * weight;
                bendRL += (Math.Abs((columnLX + portToColOffset) - (fromCol + portFromColOffset)) + 1) * weight;
              }
            }
          }

          // Compute "crossings" of nodes on same layer.
          foreach (LayeredDigraphEdge edge in unfixedLayerNodes[index+1].SourceEdgesList) {
            if (edge.Valid &&
              (edge.FromVertex.Layer == unfixedLayer)) {
              LayeredDigraphVertex fromVertex = edge.FromVertex;
              int indexFrom = 0;
              while (unfixedLayerNodes[indexFrom] != fromVertex) indexFrom++;
              // Depending on where the from node falls,
              //  we increase the appropriate crossing value.
              // The different values indicate how "strongly" that particular
              //  ordering is desired.
              if (indexFrom < index) {
                layerCrossingLR += 2 * ((index + 1)- indexFrom);
                layerCrossingRL += 2 * (index - indexFrom);
              }
              if (indexFrom == index) {
                layerCrossingRL += 1;
              }
              if (indexFrom > index + 1) {
                layerCrossingLR += 4 * (indexFrom - (index + 1));
                layerCrossingRL += 4 * (indexFrom - index);
              }
            }
          }

          // Calculate the successor link list of the right node, based on direction 
          if (straighten && ((directionBS > 0) || (directionBS == 0))) {
            foreach (LayeredDigraphEdge edge in unfixedLayerNodes[index+1].DestinationEdgesList) {
              if (edge.Valid &&
                (edge.ToVertex.Layer != unfixedLayer)) {
                // Update the bends.
                double weight = LinkStraightenWeight(edge);
                int portFromColOffset = edge.PortFromColOffset;
                int portToColOffset = edge.PortToColOffset;
                int toCol = edge.ToVertex.Column;
                bendLR += (Math.Abs((columnR + portFromColOffset) - (toCol + portToColOffset)) + 1) * weight;
                bendRL += (Math.Abs((columnLX + portFromColOffset) - (toCol + portToColOffset)) + 1) * weight;
              }
            }
          }

          // Compute "crossings" of nodes on same layer.
          foreach (LayeredDigraphEdge edge in unfixedLayerNodes[index+1].DestinationEdgesList) {
            if (edge.Valid &&
              (edge.ToVertex.Layer == unfixedLayer)) {
              LayeredDigraphVertex toVertex = edge.ToVertex;
              int indexTo = 0;
              while (unfixedLayerNodes[indexTo] != toVertex) indexTo++;
              // Depending on where the to node falls,
              //  we increase the appropriate crossing value.
              // The different values indicate how "strongly" that particular
              //  ordering is desired.
              if (indexTo == index) {
                layerCrossingLR += 1;
              }
            }
          }
        
          // Calculate the bend numbers for the left and right nodes, 
          //  based on their barycenter values.
          // The idea is that if neither a shift to the left or the right reduces
          //  the actual number of bends, moving towards the barycenter will
          //  improve the centering and balance of the layout.
          double BbendLR = 0;
          double BbendRL = 0;
          double barycenterNegL = barycentersNeg[unfixedLayerNodes[index].Index];
          double barycenterPosL = barycentersPos[unfixedLayerNodes[index].Index];
          double barycenterNegR = barycentersNeg[unfixedLayerNodes[index + 1].Index];
          double barycenterPosR = barycentersPos[unfixedLayerNodes[index + 1].Index];
        
          // A barycenter of -1 indicates that the barycenter does not exist
          //  for that direction; hence no bends exist.
          if (barycenterNegL != -1) {
            BbendLR += Math.Abs(barycenterNegL - columnL);
            BbendRL += Math.Abs(barycenterNegL - columnRX);
          }
          // A barycenter of -1 indicates that the barycenter does not exist
          //  for that direction; hence no bends exist.
          if (barycenterPosL != -1) {
            BbendLR += Math.Abs(barycenterPosL - columnL);
            BbendRL += Math.Abs(barycenterPosL - columnRX);
          }
          // A barycenter of -1 indicates that the barycenter does not exist
          //  for that direction; hence no bends exist.
          if (barycenterNegR != -1) {
            BbendLR += Math.Abs(barycenterNegR - columnR);
            BbendRL += Math.Abs(barycenterNegR - columnLX);
          }
          // A barycenter of -1 indicates that the barycenter does not exist
          //  for that direction; hence no bends exist.
          if (barycenterPosR != -1) {
            BbendLR += Math.Abs(barycenterPosR - columnR);
            BbendRL += Math.Abs(barycenterPosR - columnLX);
          }
          // Peform an exchange if it improves the number of link crossings
          //  or the number of bends.
          if ((layerCrossingRL < layerCrossingLR-0.5) ||
            ((layerCrossingRL == layerCrossingLR) && (crossingRL < crossingLR-0.5)) || 
            ((layerCrossingRL == layerCrossingLR) && (crossingRL == crossingLR) && (bendRL < bendLR-0.5)) || 
            ((layerCrossingRL == layerCrossingLR) && (crossingRL == crossingLR) && (bendRL == bendLR) && (BbendRL < BbendLR-0.5))) {
            layerChange = true;
            exchange = true;
            unfixedLayerNodes[index].Column = columnRX;
            unfixedLayerNodes[index + 1].Column = columnLX;
            LayeredDigraphVertex vertex = unfixedLayerNodes[index];
            unfixedLayerNodes[index] = unfixedLayerNodes[index+1];
            unfixedLayerNodes[index+1] = vertex;
          }
        }
      }
      // Update the index for each node in the unfixedLayer.
      for (int index = 0; index < num; index++) {
        unfixedLayerNodes[index].Index = index;
      }
      // Return status of the unfixedLayer.
      FreeCachedNodeArrayList(unfixedLayer, unfixedLayerNodes);
      return layerChange;
    }
  


    /********************************************************************************************/

    /// <summary>
    /// Adjusts the columns of nodes in the network to produce a layout which reduces
    /// the number of bends and is tightly packed.
    /// </summary>
    /// <remarks>
    /// The behavior is controlled by the <see cref="PackOption"/>.
    /// </remarks>
    protected virtual void StraightenAndPack() {
      int layer;
      bool change;

      bool expand = ((this.PackOption & LayeredDigraphPack.Expand) != 0);
      bool forceexpand = ((this.PackOption & (LayeredDigraphPack)8 /* LayeredDigraphPack.ForceExpand */) != 0);
      if (this.Network.EdgeCount > 1000 && !forceexpand) expand = false;
      if (expand) {
        // In order to give the straightening methods more "room",
        //  widen the spaces between each node.
        // First, compute the maximum column used by each layer.
        // This will be used to center the nodes in each layer.
        // The idea is that centered nodes are more likely to align 
        //  with their parents and children.
        int[] columns = new int[maxLayer + 1];
        int index;
        for (index = 0; index <= maxLayer; index++) {
          columns[index] = 0;
        }
        foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
          layer = vertex.Layer;
          int column = vertex.Column;
          int minColumnSpace = NodeMinColumnSpace(vertex, false);
          columns[layer] = Math.Max(columns[layer], column + minColumnSpace);
        }
    
        // Next, modify the column of each node so that it is spaced and centered.
        const int mult = 8;
        foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
          layer = vertex.Layer;
          int column = vertex.Column;
          vertex.Column = (((maxColumn - columns[layer]) * mult) / 2) + column * mult;
        }
        maxColumn *= mult;
      }

      if ((this.PackOption & LayeredDigraphPack.Straighten) != 0) {
        // Use the BendStraighten method, sweeping away from the maxIndexLayer,
        //  iterating until no changes occur.
        // The idea is that the layer with the most nodes will have the greatest
        //  influence on the remainder of the nodes.
        change = true;
        while (change) {
          change = false;
          for (layer = maxIndexLayer + 1; layer <= maxLayer; layer++) {
            change = BendStraighten(layer, 1) || change;
          }
          for (layer = maxIndexLayer - 1; layer >= 0; layer--) {
            change = BendStraighten(layer, -1) || change;
          }
          change = BendStraighten(maxIndexLayer, 0) || change;
        }
      }

      if ((this.PackOption & LayeredDigraphPack.Median) != 0) {
        // Use the MedianStraighten method, sweeping away from the maxIndexLayer.
        // The idea is that the layer with the most nodes will have the greatest
        //  influence on the remainder of the nodes.
        for (layer = maxIndexLayer + 1; layer <= maxLayer; layer++) {
          MedianStraighten(layer, 1);
        }
        for (layer = maxIndexLayer - 1; layer >= 0; layer--) {
          MedianStraighten(layer, -1);
        }
        MedianStraighten(maxIndexLayer, 0);
      }

      // Iterate compactification and straightening until bends are not reduced.
      // Since ComponentPack and BendStraighten both are guaranteed not to
      //  increase the number of "bends", this loop will terminate,
      //  usually only after two or three iterations.

      if (expand) {
        // Use the ComponentPack method to tightly pack the nodes. 
        ComponentPack(-1);
        ComponentPack(1);
      }

      if ((this.PackOption & LayeredDigraphPack.Straighten) != 0) {
        // Use the BendStraighten method, sweeping away from the maxIndexLayer,
        //  iterating until no changes occur.
        // The idea is that the layer with the most nodes will have the greatest
        //  influence on the remainder of the nodes.
        change = true;
        while (change) {
          change = false;
          change = BendStraighten(maxIndexLayer, 0) || change;
          for (layer = maxIndexLayer + 1; layer <= maxLayer; layer++) {
            change = BendStraighten(layer, 0) || change;
          }
          for (layer = maxIndexLayer - 1; layer >= 0; layer--) {
            change = BendStraighten(layer, 0) || change;
          }
        }
      }
    }
  
  
    /********************************************************************************************/

    /// <summary>
    /// Adjusts the columns of nodes within the unfixedLayer to reduce the number of "bends"
    /// between the unfixedLayer and its adjacent layers.  The direction argument indicates
    /// which of the adjacent layers should be taken into consideration when reducing the number
    /// of bends.
    /// The "weighted bend" between a node U and a node V connected by link L is calculated by
    /// <c>abs((U.column + L.portFromColOffset) - (V.column + L.portToColOffset)) * LinkStraightenWeight(L)</c>
    /// The LinkStraightenWeight attempts to give higher priority to links between
    /// "artificial" nodes; i.e., long links in the final layout will be straighter.
    /// The idea is to iterate the ShiftBendStraighten and adjacentExchangeBendStraighten
    /// methods until no improvements are made.
    /// </summary>
    /// <param name="unfixedLayer"></param>
    /// <param name="direction"></param>
    /// <returns>Returns true if an improvement was made.</returns>
    protected virtual bool BendStraighten(int unfixedLayer, int direction) {
      bool layerChange = false;
      while (ShiftBendStraighten(unfixedLayer, direction)
             /* ??? || AdjacentExchangeCrossingReductionBendStraighten(unfixedLayer, 0, true, direction) */ ) { 
        layerChange = true;
      }
      return layerChange;
    }  
    
    /********************************************************************************************/

    /// <summary>
    /// Adjusts the columns of nodes within the unfixedLayer to reduce the number of "bends"
    /// between the unfixedLayer and its adjacent layers.  The direction argument indicates
    /// which of the adjacent layers should be taken into consideration when reducing the number
    /// of bends.
    /// The "weighted bend" between a node U and a node V connected by link L is calculated by
    /// <c>abs((U.column + L.portFromColOffset) - (V.column + L.portToColOffset)) * LinkStraightenWeight(L)</c>
    /// The LinkStraightenWeight attempts to give higher priority to links between
    /// "artificial" nodes; i.e., long links in the final layout will be straighter.
    /// The idea is shift nodes to the left and to the right to reduce the bends
    /// ensuring that no two nodes have overlapping "allocations" of columns.
    /// Return true if some change was made to the layer.
    /// </summary>
    /// <param name="unfixedLayer"></param>
    /// <param name="direction"></param>
    /// <returns>Return true if some change was made to the layer and false otherwise.</returns>
    protected virtual bool ShiftBendStraighten(int unfixedLayer, int direction) {
      int index;
      // Fill unfixedLayerNodes with nodes in the unfixedLayer, ordered by index.
      LayeredDigraphVertex[] unfixedLayerNodes = GetCachedNodeArrayList(unfixedLayer);
      int num = indices[unfixedLayer];
    
      // Initialize the barycenters from the negative direction.
      double[] barycentersNeg = Barycenters(unfixedLayer, -1);
      // If direction > 0, the set barycenters from the negative direction to -1.
      if (direction > 0) {
        for (index = 0; index < num; index++) {
          barycentersNeg[index] = -1;
        }
      }
      // Initialize the barycenters from the positive direction.
      double[] barycentersPos = Barycenters(unfixedLayer, 1);
      // If direction < 0, the set barycenters from the positive direction to -1.
      if (direction < 0) {
        for (index = 0; index < num; index++) {
          barycentersPos[index] = -1;
        }
      }

      bool layerChange = false;
      bool shift = true;
      while (shift) {
        shift = false;
        for (index = 0; index < num; index++) {
          // Current column of unfixedLayerNodes[index].
          int columnC = unfixedLayerNodes[index].Column;
          // Minimum column space for unfixedLayerNodes[index].
          int minColumnSpaceLeft = NodeMinColumnSpace(unfixedLayerNodes[index], true);
          int minColumnSpaceRight = NodeMinColumnSpace(unfixedLayerNodes[index], false);
        
          // Calculate the column of a shift to the left.
          // If there are no nodes to the left, or there is enough space between
          //  unfixedLayerNodes[index] and unfixedLayerNodes[index-1],
          //  then columnL = columnC - 1.
          // Otherwise, columnL = columnC and no shift will be taken
          //  (since the current bends will equal the bends after a shift into columnL).
          int columnL;
          if ((index - 1 < 0) ||
              ((columnC - unfixedLayerNodes[index-1].Column - 1) > (minColumnSpaceLeft + NodeMinColumnSpace(unfixedLayerNodes[index-1], false)))) {
            columnL = columnC - 1;
          } else {
            columnL = columnC;
          }

          // Calculate the column of a shift to the right.
          // If there are no nodes to the right, or there is enough space between
          //  unfixedLayerNodes[index] and unfixedLayerNodes[index+1],
          //  then columnR = columnC + 1.
          // Otherwise, columnR = columnC and no shift will be taken
          //  (since the current bends will equal the bends after a shift into columnR).
          int columnR;
          if ((index + 1 >= num) ||
              ((unfixedLayerNodes[index + 1].Column - columnC - 1) > (minColumnSpaceRight + NodeMinColumnSpace(unfixedLayerNodes[index+1], true)))) {
            columnR = columnC + 1;
          } else {
            columnR = columnC;
          }

          // Calculate the bend numbers for the current column, left column, and right column.
          double bendC = 0;
          double bendL = 0;
          double bendR = 0;

          // Calculate the predecessor link list, based on direction.
          if ((direction < 0) || (direction == 0)) {
            foreach (LayeredDigraphEdge edge in unfixedLayerNodes[index].SourceEdgesList) {
              if (edge.Valid &&
                (edge.FromVertex.Layer != unfixedLayer)) {
                // Update the bends.
                double weight = LinkStraightenWeight(edge);
                int portFromColOffset = edge.PortFromColOffset;
                int portToColOffset = edge.PortToColOffset;
                int fromCol = edge.FromVertex.Column;
                bendC += (Math.Abs((columnC + portToColOffset) - (fromCol + portFromColOffset)) + 1) * weight;
                bendL += (Math.Abs((columnL + portToColOffset) - (fromCol + portFromColOffset)) + 1) * weight;
                bendR += (Math.Abs((columnR + portToColOffset) - (fromCol + portFromColOffset)) + 1) * weight;
              }
            }
          }

          // Calculate the successor link list, based on direction.
          if ((direction > 0) || (direction == 0)) {
            foreach (LayeredDigraphEdge edge in unfixedLayerNodes[index].DestinationEdgesList) {
              if (edge.Valid &&
                (edge.ToVertex.Layer != unfixedLayer)) {
                // Update the bends.
                double weight = LinkStraightenWeight(edge);
                int portFromColOffset = edge.PortFromColOffset;
                int portToColOffset = edge.PortToColOffset;
                int toCol = edge.ToVertex.Column;
                bendC += (Math.Abs((columnC + portFromColOffset) - (toCol + portToColOffset)) + 1) * weight;
                bendL += (Math.Abs((columnL + portFromColOffset) - (toCol + portToColOffset)) + 1) * weight;
                bendR += (Math.Abs((columnR + portFromColOffset) - (toCol + portToColOffset)) + 1) * weight;
              }
            }
          }
        
          // Calculate the bend numbers for the current column, left column, 
          //  and right column, based on the barycenter values.
          // The idea is that if neither a shift to the left or the right reduces
          //  the actual number of bends, moving towards the barycenter will
          //  improve the centering and balance of the layout.
          double BbendC = 0;
          double BbendL = 0;
          double BbendR = 0;
          double barycenterNegC = barycentersNeg[unfixedLayerNodes[index].Index];
          double barycenterPosC = barycentersPos[unfixedLayerNodes[index].Index];
          // A barycenter of -1 indicates that the barycenter does not exist
          //  for that direction; hence no bends exist.
          if (barycenterNegC != -1) {
            BbendC += Math.Abs(barycenterNegC - columnC);
            BbendL += Math.Abs(barycenterNegC - columnL);
            BbendR += Math.Abs(barycenterNegC - columnR);
          }
          // A barycenter of -1 indicates that the barycenter does not exist
          //  for that direction; hence no bends exist.
          if (barycenterPosC != -1) {
            BbendC += Math.Abs(barycenterPosC - columnC);
            BbendL += Math.Abs(barycenterPosC - columnL);
            BbendR += Math.Abs(barycenterPosC - columnR);
          }
          // Peform a shift if it improves the bends.
          if ((bendL < bendC) || 
            ((bendL == bendC) && (BbendL < BbendC))) {
            layerChange = true;
            shift = true;
            unfixedLayerNodes[index].Column = columnL;
          }
          // Peform a shift if it improves the bends.
          if ((bendR < bendC) || 
            ((bendR == bendC) && (BbendR < BbendC))) {
            layerChange = true;
            shift = true;
            unfixedLayerNodes[index].Column = columnR;
          }
        }
      }
      FreeCachedNodeArrayList(unfixedLayer, unfixedLayerNodes);
      // Some nodes may have been moved to the left of column 0 or to the right 
      //  of column maxColumn, so normalize the nodes; i.e., the leftmost column will
      //  be column 0 and maxColumn will be updated appropriately.
      Normalize();
    
      // Return status of the unfixedLayer.
      return layerChange;
    }


    /********************************************************************************************/

    /// <summary>
    /// Adjusts the columns of nodes within the unfixedLayer in order to move nodes towards
    /// their median columns.  The direction argument indicates which of the adjacent layers
    /// should be taken into consideration when computing the median column.
    /// The idea is shift nodes to the left and to the right to move nodes towards their
    /// median columns, ensuring that no two nodes have overlapping "allocations" of columns.
    /// Returns true if some change was made to the layer.
    /// </summary>
    /// <param name="unfixedLayer"></param>
    /// <param name="direction"></param>
    /// <returns>Returns true if some change was made to the layer and false otherwise.</returns>
    protected virtual bool MedianStraighten(int unfixedLayer, int direction) {
      int index;
    
      // Fill unfixedLayerNodes with nodes in the unfixedLayer, ordered by index
      LayeredDigraphVertex[] unfixedLayerNodes = GetCachedNodeArrayList(unfixedLayer);
      int num = indices[unfixedLayer];
    
      // Calculate the median value of each node and convert to integer.
      double[] dmedians = Medians(unfixedLayer, direction);
      int[] medians = new int[num];
      for (index = 0; index < num; index++) {
        medians[index] = (int)dmedians[index];
      }
      bool layerChange = false;
      bool change = true;
      while (change) {
        change = false;
        for (index = 0; index < num; index++) {
          // The current column of unfixedLayerNodes[index].
          int column = unfixedLayerNodes[index].Column;
          int minColumnSpaceLeft = NodeMinColumnSpace(unfixedLayerNodes[index], true);
          int minColumnSpaceRight = NodeMinColumnSpace(unfixedLayerNodes[index], false);
          // The new column of unfixedLayerNodes[index].
          int ncolumn = 0;
          // A median value of -1 indicates that the node does not 
          //  have a defined median (i.e., the node has no neighbors 
          //  in the appropriate adjacent layer).  
          // In that case, treat these nodes specially, since they can be
          //  shifted "out of the way" of nodes that have a defined median.
          if (medians[index] == -1) {
            // If index == 0 and index == num - 1,
            //  then there is only one node on the layer
            //  and the node does not move.
            if ((index == 0) && (index == num - 1)) {
              ncolumn = column;
              // If index == 0,
              //  then check if the node is "adjacent" (in the sense of NodeMinColumnSpace values)
              //  to its rightmost neighbor.  If so, move the node to the left.
              // The idea is that the rightmost neighbor may be "moving left", 
              //  so move the node out of the way.
            } 
            else if (index == 0) {
              int rcolumn = unfixedLayerNodes[index + 1].Column;
              if (rcolumn - column == minColumnSpaceRight + NodeMinColumnSpace(unfixedLayerNodes[index+1], true)) {
                ncolumn = column - 1;
              } else {
                ncolumn = column;
              }
            
              // If index == num - 1,
              //  then check if the node is "adjacent" (in the sense of NodeMinColumnSpace values)
              //  to its leftmost neighbor.  If so, move the node to the right.
              // The idea is that the leftmost neighbor may be "moving right", 
              //  so move the node out of the way.
            } 
            else if (index == num - 1) {
              int lcolumn = unfixedLayerNodes[index-1].Column;
              if (column - lcolumn == minColumnSpaceLeft + NodeMinColumnSpace(unfixedLayerNodes[index-1], false)) {
                ncolumn = column + 1;
              } else {
                ncolumn = column;
              }
            
              // Otherwise, position the node between its left and right neighbors.
            } else {
              int lcolumn = unfixedLayerNodes[index-1].Column;
              int nlcolumn = lcolumn + NodeMinColumnSpace(unfixedLayerNodes[index-1], false) + minColumnSpaceLeft + 1;
              int rcolumn = unfixedLayerNodes[index + 1].Column;
              int nrcolumn = rcolumn - NodeMinColumnSpace(unfixedLayerNodes[index+1], true) - minColumnSpaceRight - 1;
              ncolumn = (nlcolumn + nrcolumn) / 2;
            }
          
            // Otherwise, the node has a defined median.
          } else {
            // If index == 0 and index == num - 1,
            //  then there is only one node on the layer
            //  and the node can be moved to its median position.
            if ((index == 0) && (index == num - 1)) {
              ncolumn = medians[index];
              // If index == 0,
              //  then move the node to the position closest to its median that does
              //  not overlap with its rightmost neighbor.
            } 
            else if (index == 0) {
              int rcolumn = unfixedLayerNodes[index + 1].Column;
              int nrcolumn = rcolumn - NodeMinColumnSpace(unfixedLayerNodes[index+1], true) - minColumnSpaceRight - 1;
              ncolumn = Math.Min(medians[index], nrcolumn);
              // If index == num - 1,
              //  then move the node to the position closest to its median that does
              //  not overlap with its leftmost neighbor.
            } 
            else if (index == num - 1) {
              int lcolumn = unfixedLayerNodes[index-1].Column;
              int nlcolumn = lcolumn + NodeMinColumnSpace(unfixedLayerNodes[index-1], false) + minColumnSpaceLeft + 1;
              ncolumn = Math.Max(medians[index], nlcolumn);
              // Otherwise, move the node to the position closest to its median that does
              //  not overlap with either its left or right neighbors.
            } else {
              int lcolumn = unfixedLayerNodes[index-1].Column;
              int nlcolumn = lcolumn + NodeMinColumnSpace(unfixedLayerNodes[index-1], false) + minColumnSpaceLeft + 1;
              int rcolumn = unfixedLayerNodes[index + 1].Column;
              int nrcolumn = rcolumn - NodeMinColumnSpace(unfixedLayerNodes[index+1], true) - minColumnSpaceRight - 1;
              if ((nlcolumn < medians[index]) && (medians[index] < nrcolumn)) {
                ncolumn = medians[index];
              } 
              else if (nlcolumn >= medians[index]) {
                ncolumn = nlcolumn;
              } 
              else if (nrcolumn <= medians[index]) {
                ncolumn = nrcolumn;
              }
            }
          }
        
          // If the node changed position, make the appropriate updates.
          if (ncolumn != column) {
            layerChange = true;
            change = true;
            unfixedLayerNodes[index].Column = ncolumn;
          }
        }
      }
      FreeCachedNodeArrayList(unfixedLayer, unfixedLayerNodes);
      // Some nodes may have been moved to the left of column 0 or to the right 
      //  of column maxColumn, so normalize the nodes; i.e., the leftmost column will
      //  be column 0 and maxColumn will be updated appropriately.
      Normalize();
      // Return status of the unfixedLayer.
      return layerChange;
    }
  
  
    /********************************************************************************************/

    /// <summary>
    /// Adjusts the columns of nodes in the network to produce a layout which is tightly packed.
    /// The idea is that columns which are "un-allocated" through all layers can be eliminated
    /// and the nodes can be shifted into that space.
    /// </summary>
    protected virtual void Pack() {
      // Sweep the network from column 0 to column maxColumn, eliminating empty columns.
      int column;
      for (column = 0; column <= maxColumn; column++) {
        while (PackAux(column, 1)) { }
      }
      // Some columns may have been eliminated, so normalize the nodes; 
      //  i.e., the leftmost column will be column 0 and maxColumn will 
      //  be updated appropriately.
      Normalize();
    }

    /// <summary>
    /// Attempts to remove the argument column by shifting columns into from the argument direction.
    ///   direction > 0 -- columns > argument column are shifted
    ///   direction &lt; 0 -- columns &lt; argument column are shifted
    /// Returns true if the argument column was removed.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="direction"></param>
    /// <returns>Returns true if the argument column was removed and false otherwise.</returns>
    protected virtual bool PackAux(int column, int direction) {
      // Scan all nodes in the network.
      // A shift is possible if no node has been "allocated" the argument column.
      bool doshift = true;
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        int minColumnSpaceLeft = NodeMinColumnSpace(vertex, true);
        int minColumnSpaceRight = NodeMinColumnSpace(vertex, false);
        if ((vertex.Column - minColumnSpaceLeft <= column) && (vertex.Column + minColumnSpaceRight >= column)) {
          doshift = false;
        }
      }
    
      // If no node has been "allocated" the argument column,
      //  shift nodes into the eliminated column from the appropriate direction.
      bool change = false;
      if (doshift) {
        if (direction > 0) {
          foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
            if (vertex.Column > column) {
              vertex.Column -= 1;
              change = true;
            }
          }
        }
        if (direction < 0) {
          foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
            if (vertex.Column < column) {
              vertex.Column += 1;
              change = true;
            }
          }
        }
      }
    
      // Return status of the column.
      return change;
    }
  
  
    /********************************************************************************************/

    /// <summary>
    /// Adjusts the columns of nodes in the network to produce a layout which is tightly packed.
    /// The idea is that two adjacent columns can be "merged" if each layer has at most one of
    /// the two columns "allocated" to a node.
    /// </summary>
    protected virtual void TightPack() {
      // Begin by using the pack method.
      // This (faster) method eliminates some columns and reduces the number of columns
      //  that need to be examined.
      Pack();
      // Sweep the network from column 0 to column maxColumn, merging columns.
      int column;
      for (column = 0; column < maxColumn; column++) {
        while (TightPackAux(column, 1)) { }
      }
      // Some columns may have been eliminated, so normalize the nodes; 
      //  i.e., the leftmost column will be column 0 and maxColumn will 
      //  be updated appropriately.
      Normalize();
    }

    /// <summary>
    /// Attempts to augment the argument column by merging columns into from the argument direction.
    /// direction > 0 -- columns > the argument column are shifted
    /// direction &lt; 0 -- columns &lt; the argument column are shifted
    /// Returns true if the argument column was changed.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="direction"></param>
    /// <returns>Returns true if the argument column was changed and false otherwise.</returns>
    protected virtual bool TightPackAux(int column, int direction) {
      // Set the next column to the appropriate value based on direction.
      int nextcolumn = column;
      if (direction > 0) 
        nextcolumn = column + 1;
      if (direction < 0)
        nextcolumn = column - 1;
    
      // The curCol and nextCol arrays indicate which layers in the current column and
      //  the next column have been "allocated" to nodes.
      // Initially, both the curCol and nextCol are empty.
      int index;
      bool[] curCol = new bool[maxLayer + 1];
      bool[] nextCol = new bool[maxLayer + 1];
      for (index = 0; index <= maxLayer; index++) {
        curCol[index] = false;
        nextCol[index] = false;
      }
    
      // Scan all nodes in the network.
      // If a node has been "allocated" either the current column or the next column,
      //  update the appropriate entry in curCol and nextCol.
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        int minColumnSpaceLeft = NodeMinColumnSpace(vertex, true);
        int minColumnSpaceRight = NodeMinColumnSpace(vertex, false);
        if ((vertex.Column - minColumnSpaceLeft <= column) && (vertex.Column + minColumnSpaceRight >= column)) {
          curCol[vertex.Layer] = true;
        }
        if ((vertex.Column - minColumnSpaceLeft <= nextcolumn) && (vertex.Column + minColumnSpaceRight >= nextcolumn)) {
          nextCol[vertex.Layer] = true;
        }
      }
    
      // A shift is possible if each layer has at most one column "allocated" between
      //  the current column and the next column. A logical NAND verifies that the columns
      //  can be merged at a particular layer.
      bool doshift = true;
      bool change = false;
      for (index = 0; index <= maxLayer; index++) {
        doshift = doshift && (!(curCol[index] && nextCol[index]));
      }
        
      // If each layer has at most one column "allocated" between
      //  the current column and the next column shift nodes into the merged 
      //  column from the appropriate direction.
      if (doshift) {
        if (direction > 0) {
          foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
            if (vertex.Column > column) {
              vertex.Column -= 1;
              change = true;
            }
          }
        }
        if (direction < 0) {
          foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
            if (vertex.Column < column) {
              vertex.Column += 1;
              change = true;
            }
          }
        }
      }
    
      // Return status of the column.
      return change;
    }
  
    /********************************************************************************************/

    /// <summary>
    /// Adjusts the columns of nodes in the network to produce a layout which is tightly packed.
    /// The idea is that the network can be fragmented from a given column in the following way:
    /// all nodes "behind" the column are placed into a single component, and the remainder of
    /// the network is divided into connected components.  Each of these new components can be
    /// examined, and those that can be merged with the given column do so.
    /// </summary>
    /// <param name="direction"></param>
    protected virtual void ComponentPack(int direction) {
      // Begin by using the TightPack method.
      // This (faster) method eliminates some columns and reduces the number of columns
      //  that need to be examined.
      TightPack();

      int column;
      if (direction > 0) {
        // Sweep the network from column 0 to column maxColumn, merging components.
        for (column = 0; column <= maxColumn; column++) {
          // Continue merging components into column
          //  while it reduces the number of "bends".
          int[] bestlayout = SaveLayout();
          double bestbends = CountBends(true);
          double lastbends = bestbends + 1;
          double bends;
          while (bestbends < lastbends) {
            lastbends = bestbends;
            ComponentPackAux(column, 1);
            bends = CountBends(true);
            if (bends > bestbends) {
              RestoreLayout(bestlayout);
            } else if (bends < bestbends) {
              bestbends = bends;
              bestlayout = SaveLayout();
            }
          }
        }
      }
      if (direction < 0) {
        // Sweep the network from column maxColumn to column 0, merging components.
        for (column = maxColumn; column >= 0; column--) {
          // Continue merging components into column
          //  while it reduces the number of "bends".
          int[] bestlayout = SaveLayout();
          double bestbends = CountBends(true);
          double lastbends = bestbends + 1;
          double bends;
          while (bestbends < lastbends) {
            lastbends = bestbends;
            ComponentPackAux(column, -1);
            bends = CountBends(true);
            if (bends > bestbends) {
              RestoreLayout(bestlayout);
            } else if (bends < bestbends) {
              bestbends = bends;
              bestlayout = SaveLayout();
            }
          }
        }
      }

      // Some columns may have been eliminated, so normalize the nodes; 
      //  i.e., the leftmost column will be column 0 and maxColumn will 
      //  be updated appropriately.
      Normalize();
    }

    /// <summary>
    /// Attempts to augment the argument column by merging components into from the argument direction.
    /// direction > 0 -- columns > the argument column are shifted
    /// direction &lt; 0 -- columns &lt; the argument column are shifted
    /// Returns true if the argument column was changed.
    /// </summary>
    /// <param name="column"></param>
    /// <param name="direction"></param>
    /// <returns>Returns true if the argument column was changed and false otherwise.</returns>
    protected virtual bool ComponentPackAux(int column, int direction) {
      // Every node is initially unset -- i.e., it hasn't been assigned to 
      //  a component.
			
      // Add nodes "behind" the argument column to component 0.
      myComponent = 0;
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        vertex.Component = -1;
      }
      if (direction > 0) {
        foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
          if (vertex.Column - NodeMinColumnSpace(vertex, true) <= column) { 
            vertex.Component = myComponent;
          }
        }
      }
      if (direction < 0) {
        foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
          if (vertex.Column + NodeMinColumnSpace(vertex, false) >= column) { 
            vertex.Component = myComponent;
          }
        }
      }

      // The remaining nodes are placed into successive "compact" components.
      myComponent++;
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        if (vertex.Component == -1) {
          ComponentUnset(vertex, myComponent, -1, true, true);
          myComponent++;
        }
      }

      // Create the interference matrix.
      // If interference[component1 * component + component2] is true,
      //  then component1 must be shifted in order for component2 to be shifted.
      // This allows components that must be moved together to be detected.
      int index;
      bool[] interference = new bool[myComponent * myComponent];
      for (index = 0; index < myComponent * myComponent; index++) {
        interference[index] = false;
      }
    
      // Fill in the interference matrix.
      // Begin by filling in a matrix representing the current distribution
      //  of components among layers and columns.
      int[] components = new int[(maxLayer + 1) * (maxColumn + 1)];
      for (index = 0; index < (maxLayer + 1) * (maxColumn + 1); index++) {
        components[index] = -1;
      }
    
      // Scan all nodes in the network.
      // If a node has been "allocated" to some columns,
      //  update the appropriate entries in components.
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        int layer = vertex.Layer;
        int startCol = Math.Max(0, vertex.Column - NodeMinColumnSpace(vertex, true));
        int endCol = Math.Min(maxColumn, vertex.Column + NodeMinColumnSpace(vertex, false));
        for (int col = startCol; col <= endCol; col++) {
          components[layer * (maxColumn + 1) + col] = vertex.Component;
        }
      }
    
      // A component interferes with another component if two different components
      //  in the current column and the "next" column are allocated to the same layer.
      // The "next" column is dependent on the direction.
      for (int layer = 0; layer <= maxLayer; layer++) {
        // In the positive direction, the "next" column is col + 1.
        if (direction > 0) {
          for (int col = 0; col < maxColumn; col++) {
            if ((components[layer * (maxColumn + 1) + col] != -1) &&
              (components[layer * (maxColumn + 1) + col + 1] != -1) &&
              (components[layer * (maxColumn + 1) + col] != components[layer * (maxColumn + 1) + col + 1])) {
              interference[components[layer * (maxColumn + 1) + col] * myComponent + components[layer * (maxColumn + 1) + col + 1]] = true;
            }
          }
        }
        // In the negative direction, the "next" column is col - 1.
        if (direction < 0) {
          for (int col = maxColumn; col > 0; col--) {
            if ((components[layer * (maxColumn + 1) + col] != -1) &&
              (components[layer * (maxColumn + 1) + col - 1] != -1) &&
              (components[layer * (maxColumn + 1) + col] != components[layer * (maxColumn + 1) + col - 1])) {
              interference[components[layer * (maxColumn + 1) + col] * myComponent + components[layer * (maxColumn + 1) + col - 1]] = true;
            }
          }
        }
      }
    
      // Determine which components can be shifted.
      // Initially, all components can be shifted.
      // Using the interference matrix, dependencies are detected and marked.
      // In particular, component 0 can not be shifted,
      //  and it dictates which other components cannot be shifted.
      bool[] doshift = new bool[myComponent];
      for (index = 0; index < myComponent; index++) {
        doshift[index] = true;
      }
      // Use a queue to keep track of which components cannot be shifted.
      List<int> Queue = new List<int>();
      // Initially, only component 0 cannot be shifted.
      Queue.Add(0);
      while (Queue.Count != 0) {
        // Dequeue.
        int head = (Queue[Queue.Count - 1]);
        Queue.RemoveAt(Queue.Count - 1);
        // If this is the first time encountering this component,
        //  mark it as unshiftable and queue the components it interferes with.
        if (doshift[head]) {
          doshift[head] = false;
          for (index = 0; index < myComponent; index++) {
            if (interference[head * myComponent + index]) {
              Queue.Insert(0, index);
            }
          }
        }      
      }

    
      // Components which can be shifted are moved in the appropriate direction.
      bool change = false; 
      if (direction > 0) {
        foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
          if (doshift[vertex.Component]) {
            vertex.Column -= 1;
            change = true;
          }
        }
      }
      if (direction < 0) {
        foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
          if (doshift[vertex.Component]) {
            vertex.Column += 1;
            change = true;
          }
        }
      }
    
      // Return status of the column.
      return change;
    }



    /********************************************************************************************/

    // Layout Nodes and Links

    /// <summary>
    /// Updates the physical location of "real" nodes and links to reflect
    /// the layout.
    /// </summary>
    /// <remarks>
    /// One reason to override this method would be to take advantage of
    /// added functionality of sub-classes of Link, for example, a sub-class
    /// that tracked bend points and allowed them to be repositioned
    /// by the application.
    /// See also <see cref="LayeredDigraphLayout.LayoutNodes"/>
    /// and <see cref="LayeredDigraphLayout.LayoutLinks"/>.
    /// </remarks>
    public virtual void LayoutNodesAndLinks() {
      SetPortSpotsAll();
      LayoutNodes();
      LayoutLinks();
    }

    private void SetPortSpotsAll() {
      if (!this.SetsPortSpots) return;
      Spot fromspot;
      Spot tospot;
      if (this.Direction == 270) {
        fromspot = Spot.MiddleTop;
        tospot = Spot.MiddleBottom;
      } else if (this.Direction == 90) {
        fromspot = Spot.MiddleBottom;
        tospot = Spot.MiddleTop;
      } else if (this.Direction == 180) {
        fromspot = Spot.MiddleLeft;
        tospot = Spot.MiddleRight;
      } else {
        fromspot = Spot.MiddleRight;
        tospot = Spot.MiddleLeft;
      }

      foreach (LayeredDigraphEdge edge in this.Network.Edges) {
        Link link = edge.Link;
        if (link != null) {
          link.Route.FromSpot = fromspot;
          link.Route.ToSpot = tospot;
        }
      }
    }


    /// <summary>
    /// Lays out the nodes.  Called by <see cref="LayeredDigraphLayout.LayoutNodesAndLinks"/>
    /// </summary>
    /// <remarks>
    /// See also <seealso cref="LayeredDigraphLayout.LayoutNodesAndLinks"/>
    /// and <seealso cref="LayeredDigraphLayout.LayoutLinks"/>
    /// </remarks>
    protected virtual void LayoutNodes() {
      // The layerSpace array records the maximum layer spacing needed
      //  by a node in each layer, in document units.
      layerSpaceLeft = new double[maxLayer+1];
      layerSpaceRight = new double[maxLayer+1];
      layerPositions = new double[maxLayer+1];
      layerSpaces = new double[maxLayer+1];

      // Scan all nodes in the network, updating the layerSpace array.
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        int layer = vertex.Layer;
        layerSpaceLeft[layer] = Math.Max(layerSpaceLeft[layer], NodeMinLayerSpace(vertex, true));
        layerSpaceRight[layer] = Math.Max(layerSpaceRight[layer], NodeMinLayerSpace(vertex, false));
      }
    
      // The layerPositions array records the position in document units for each layer.
      // Each layer receives layerSpace[layer] space "above" and "below" it,
      // to accomodate the largest node in the layer, plus LayerSpacing between layers
      double layerPosition = 0;
      double space = this.LayerSpacing;
      for (int layer = 0; layer <= maxLayer; layer++) {
        double s = space;
        // don't reserve LayerSpacing around layers consisting entirely of artificial nodes
        if (layerSpaceLeft[layer]+layerSpaceRight[layer] <= 0) s = 0;
        if (layer > 0) layerPosition += s/2;  // avoid a margin: don't add half a LayerSpacing for first layer
        if (this.Direction == 90 || this.Direction == 0) {
          layerPosition += layerSpaceRight[layer];
          layerPositions[layer] = layerPosition;  // middle of layer
          layerPosition += layerSpaceLeft[layer];
        } else {
          layerPosition += layerSpaceLeft[layer];
          layerPositions[layer] = layerPosition;  // middle of layer
          layerPosition += layerSpaceRight[layer];
        }
        if (layer < maxLayer) layerPosition += s/2;  // space between layers, except when beyond last layer
        layerSpaces[layer] = layerPosition;  // middle of space between layers
      }
      // layerPosition now remembers total depth

      Point orig = this.ArrangementOrigin;

      // convert layerPositions and layerSpaces arrays to use actual document coordinates, not just document units
      for (int layer = 0; layer <= maxLayer; layer++) {
        if (this.Direction == 270) {
          layerPositions[layer] = orig.Y + layerPositions[layer];
          layerSpaces[layer] = orig.Y + layerSpaces[layer];
        } else if (this.Direction == 90) {
          layerPositions[layer] = orig.Y + layerPosition - layerPositions[layer];
          layerSpaces[layer] = orig.Y + layerPosition - layerSpaces[layer];
        } else if (this.Direction == 180) {
          layerPositions[layer] = orig.X + layerPositions[layer];
          layerSpaces[layer] = orig.X + layerSpaces[layer];
        } else {
          layerPositions[layer] = orig.X + layerPosition - layerPositions[layer];
          layerSpaces[layer] = orig.X + layerPosition - layerSpaces[layer];
        }
      }

      // Scan all nodes in the network (both "real" and "artificial"),
      //  and set their locations.
      foreach (LayeredDigraphVertex vertex in this.Network.Vertexes) {
        int layer = vertex.Layer;
        int column = vertex.Column;
        // Place the center of the node depending on the direction of the layout.
        double x, y;
        if (this.Direction == 270) {
          x = orig.X + this.ColumnSpacing * column;
          y = layerPositions[layer];
        } else if (this.Direction == 90) {
          x = orig.X + this.ColumnSpacing * column;
          y = layerPositions[layer];
        } else if (this.Direction == 180) {
          x = layerPositions[layer];
          y = orig.Y + this.ColumnSpacing * column;
        } else {
          x = layerPositions[layer];
          y = orig.Y + this.ColumnSpacing * column;
        }
        // Set the center position, update the position in the network,
        //  and commit the position to the physical location of the node.
        vertex.Center = new Point(x, y);
        vertex.CommitPosition();
      }
    }

    /// <summary>
    /// Routes the links.  Called by <see cref="LayeredDigraphLayout.LayoutNodesAndLinks"/>
    /// </summary>
    /// <remarks>
    /// See also <seealso cref="LayeredDigraphLayout.LayoutNodesAndLinks"/>
    /// and <seealso cref="LayeredDigraphLayout.LayoutNodes"/>
    /// </remarks>
    protected virtual void LayoutLinks() {
      // first, clear out all the existing link points
      foreach (LayeredDigraphEdge edge in this.Network.Edges) {
        Route pLinkR = edge.Route;
        if (pLinkR != null) {
          pLinkR.ClearPoints();
        }
      }
      // second, call GoLink.CalculateStroke on each link, just once per GoLink
      // NB: multiple LayeredDigraphNetworkLinks can refer to the same GoLink
      foreach (LayeredDigraphEdge edge in this.Network.Edges) {
        Route pLinkR = edge.Route;
        if (pLinkR != null) pLinkR.UpdatePoints();
      }
      // Scan all links in the network (both "real" and "artificial"),
      //  and set their locations.
      foreach (LayeredDigraphEdge edge in this.Network.Edges) {
        if (edge.Link == null) continue;

        // The "real" physical link and "real" physical ports.
        Route pLinkR = edge.Route;
        if (pLinkR == null) continue;
        Node gofromnode = edge.FromVertex.Node;
        Node gotonode = edge.ToVertex.Node;
        Link golink = pLinkR.Link;
        FrameworkElement gofromport = golink.FromPort;
        if (gofromnode is Group && gofromnode != golink.FromNode) gofromport = gofromnode.Port;
        FrameworkElement gotoport = golink.ToPort;
        if (gotonode is Group && gotonode != golink.ToNode) gotoport = gotonode.Port;

        // A valid link requires no bends, unless maybe if it's Bezier, to avoid crossing other nodes in the same layers
        if (edge.Valid) {
          // it's a "real" link connecting two "real" GoObject nodes, not needing any intermediate points
          if (pLinkR.Curve == LinkCurve.Bezier && pLinkR.PointsCount == 4) {
            if (edge.Rev) {
              Node tempN = gofromnode;
              gofromnode = gotonode;
              gotonode = tempN;
              FrameworkElement temp = gofromport;
              gofromport = gotoport;
              gotoport = temp;
            }
            // when same column, assume straight
            if (edge.FromVertex.Column == edge.ToVertex.Column) {
              Point p0 = pLinkR.GetLinkPoint(gofromnode, gofromport, pLinkR.GetFromSpot(), true, false, gotonode, gotoport);  //???
              Point p3 = pLinkR.GetLinkPoint(gotonode, gotoport, pLinkR.GetToSpot(), false, false, gofromnode, gofromport);  //???
              Point c1 = new Point((2*p0.X+p3.X)/3, (2*p0.Y+p3.Y)/3);
              Point c2 = new Point((p0.X+2*p3.X)/3, (p0.Y+2*p3.Y)/3);
              pLinkR.ClearPoints();
              pLinkR.AddPoint(p0);
              pLinkR.AddPoint(c1);
              pLinkR.AddPoint(c2);
              pLinkR.AddPoint(p3);
            } else {
              // Not needed when the ports have spots -- check each port separately
              Point p0 = pLinkR.GetPoint(0);
              Point c1 = pLinkR.GetPoint(1);
              Point c2 = pLinkR.GetPoint(2);
              Point p3 = pLinkR.GetPoint(3);
              if (gofromport != null && pLinkR.GetFromSpot().IsNone) {
                if (this.Direction == 90 || this.Direction == 270) {
                  c1.X = p0.X;
                  c1.Y = (p0.Y+p3.Y)/2;
                } else {
                  c1.X = (p0.X+p3.X)/2;
                  c1.Y = p0.Y;
                }
                pLinkR.SetPoint(1, c1);
                pLinkR.SetPoint(0, pLinkR.GetLinkPoint(gofromnode, gofromport, pLinkR.GetFromSpot(), true, false, gotonode, gotoport));  //???
              }
              if (gotoport != null && pLinkR.GetToSpot().IsNone) {
                if (this.Direction == 90 || this.Direction == 270) {
                  c2.X = p3.X;
                  c2.Y = (p0.Y+p3.Y)/2;
                } else {
                  c2.X = (p0.X+p3.X)/2;
                  c2.Y = p3.Y;
                }
                pLinkR.SetPoint(2, c2);
                pLinkR.SetPoint(3, pLinkR.GetLinkPoint(gotonode, gotoport, pLinkR.GetToSpot(), false, false, gofromnode, gofromport));  //???
              }
            }
          }
          continue;
        }

        // skip links connecting nodes within the same layer, including dummy links
        // inserted to support loopbacks
        if (edge.FromVertex.Layer == edge.ToVertex.Layer) continue;

        // now dealing with a !Valid link -- find and route all dummy links
        // corresponding to this link that connect artificial nodes with the
        // real from and to nodes.
        int si = 1;  // insertion index for intermediate stroke points
        bool bezier = false;
        bool ortho = false;
        si = pLinkR.FirstPickIndex+1;
        if (pLinkR.Curve == LinkCurve.Bezier) {
          bezier = true;
          // keep first and last two points
          int numpts = pLinkR.PointsCount;
          if (numpts > 4) {
            Point[] pts = new Point[4] { pLinkR.GetPoint(0), pLinkR.GetPoint(1), pLinkR.GetPoint(numpts-2), pLinkR.GetPoint(numpts-1) };
            pLinkR.Points = pts;
          }
          si = 2;  // insert triplets of bezier points after first control point
        } else if (pLinkR.Orthogonal) {
          ortho = true;
          // keep first and last two points
          int numpts = pLinkR.PointsCount;
          if (numpts > 4) {
            Point[] pts = new Point[4] { pLinkR.GetPoint(0), pLinkR.GetPoint(1), pLinkR.GetPoint(numpts - 2), pLinkR.GetPoint(numpts - 1) };
            pLinkR.Points = pts;
          }
        } else {
          // keep the first and last points only
          int numpts = pLinkR.PointsCount;
          bool nofromspot = pLinkR.GetFromSpot().IsNone;
          bool notospot = pLinkR.GetToSpot().IsNone;
          if (numpts > 2 && nofromspot && notospot) {
            pLinkR.Points = new Point[2] { pLinkR.GetPoint(0), pLinkR.GetPoint(numpts-1) };
          } else if (numpts > 3 && nofromspot && !notospot) {
            pLinkR.Points = new Point[3] { pLinkR.GetPoint(0), pLinkR.GetPoint(numpts-2), pLinkR.GetPoint(numpts-1) };
          } else if (numpts > 3 && !nofromspot && notospot) {
            pLinkR.Points = new Point[3] { pLinkR.GetPoint(0), pLinkR.GetPoint(1), pLinkR.GetPoint(numpts-1) };
          } else if (numpts > 4 && !nofromspot && !notospot) {
            pLinkR.Points = new Point[4] { pLinkR.GetPoint(0), pLinkR.GetPoint(1), pLinkR.GetPoint(numpts-2), pLinkR.GetPoint(numpts-1) };
          }
        }

        // get the from and to nodes.
        LayeredDigraphVertex vertexFrom = edge.FromVertex;
        LayeredDigraphVertex vertexTo = edge.ToVertex;

        // An invalid link requires bends.
        // Add bend points at the positions of the "artificial" nodes.
        // Be smarter about Bezier style and about Orthogonal links.
        if (!edge.Rev) {  // i.e. normal direction
          // Walk the sequence of "artificial" nodes to set the bend points of the links.
          while (vertexFrom != null && vertexFrom != vertexTo) {
            // Find the "artificial" node that corresponds to the next bend in the link.
            LayeredDigraphVertex vertexA = null;
            LayeredDigraphVertex prevVertexA = null;
            // All links between "artificial" nodes have their GoObject
            //  set to the GoObject of the link they are replacing.  Hence,
            //  it suffices to find the successor link of the current node
            //  which has a GoObject equal to the "real" physical link.
            foreach (LayeredDigraphEdge edgeA in vertexFrom.DestinationEdgesList) {
              if (edgeA.Link == edge.Link) {
                vertexA = edgeA.ToVertex;
                prevVertexA = edgeA.FromVertex;
                if (prevVertexA.Node != null) prevVertexA = null;
                if (vertexA.Node == null) break;
              }
            }
            // pNodeA is an artificial node for which we need to add some stroke points
            if (vertexA != vertexTo) {  // not yet end of iteration?
              Point prev = pLinkR.GetPoint(si-1);
              Point next = vertexA.Center;
              if (ortho) {
                if (this.Direction == 180 || this.Direction == 0) {
                  Point p = (prevVertexA != null ? prevVertexA.Center : prev);
                  if (p.Y != next.Y) {
                    double mid = layerSpaces[vertexA.Layer];
                    pLinkR.InsertPoint(si++, new Point(mid, prev.Y));
                    pLinkR.InsertPoint(si++, new Point(mid, next.Y));
                  }
                } else {
                  Point p = (prevVertexA != null ? prevVertexA.Center : prev);
                  if (p.X != next.X) {
                    double mid = layerSpaces[vertexA.Layer];
                    pLinkR.InsertPoint(si++, new Point(prev.X, mid));
                    pLinkR.InsertPoint(si++, new Point(next.X, mid));
                  }
                }
              } else {  // when style is line or bezier
                double depthL = Math.Max(10, layerSpaceLeft[vertexA.Layer]);
                double depthR = Math.Max(10, layerSpaceRight[vertexA.Layer]);
                if (this.Direction == 180) {
                  pLinkR.InsertPoint(si++, new Point(next.X+depthR, next.Y));
                  if (bezier) pLinkR.InsertPoint(si++, next);
                  pLinkR.InsertPoint(si++, new Point(next.X-depthL, next.Y));
                } else if (this.Direction == 90) {
                  pLinkR.InsertPoint(si++, new Point(next.X, next.Y-depthL));
                  if (bezier) pLinkR.InsertPoint(si++, next);
                  pLinkR.InsertPoint(si++, new Point(next.X, next.Y+depthR));
                } else if (this.Direction == 270) {
                  pLinkR.InsertPoint(si++, new Point(next.X, next.Y+depthR));
                  if (bezier) pLinkR.InsertPoint(si++, next);
                  pLinkR.InsertPoint(si++, new Point(next.X, next.Y-depthL));
                } else {
                  pLinkR.InsertPoint(si++, new Point(next.X-depthL, next.Y));
                  if (bezier) pLinkR.InsertPoint(si++, next);
                  pLinkR.InsertPoint(si++, new Point(next.X+depthR, next.Y));
                }
              }
            }
            // Update the from node and iterate.
            vertexFrom = vertexA;
          }
          if (ortho) {  // do the last zig-zag
            Point prev = pLinkR.GetPoint(si-1);
            Point next = pLinkR.GetPoint(si);
            if (this.Direction == 180 || this.Direction == 0) {
              if (prev.Y != next.Y) {
                double mid;
                if (this.Direction == 0)
                  mid = next.X-this.LayerSpacing/2;
                else
                  mid = next.X+this.LayerSpacing/2;
                pLinkR.InsertPoint(si++, new Point(mid, prev.Y));
                pLinkR.InsertPoint(si++, new Point(mid, next.Y));
              }
            } else {
              if (prev.X != next.X) {
                double mid;
                if (this.Direction == 90)
                  mid = next.Y-this.LayerSpacing/2;
                else
                  mid = next.Y+this.LayerSpacing/2;
                pLinkR.InsertPoint(si++, new Point(prev.X, mid));
                pLinkR.InsertPoint(si++, new Point(next.X, mid));
              }
            }
          }
        } else {  // reversed
          // Walk the sequence of "artificial" nodes to set the bend points of the links.
          while (vertexTo != null && vertexFrom != vertexTo) {
            // Find the "artificial" node that corresponds to the next bend in the link.
            LayeredDigraphVertex vertexA = null;
            LayeredDigraphVertex prevVertexA = null;
            // All links between "artificial" nodes have their GoObject
            //  set to the GoObject of the link they are replacing.  Hence,
            //  it suffices to find the predecessor link of the current node
            //  which has a GoObject equal to the "real" physical link.
            foreach (LayeredDigraphEdge edgeA in vertexTo.SourceEdgesList) {
              if (edgeA.Link == edge.Link) {
                vertexA = edgeA.FromVertex;
                prevVertexA = edgeA.ToVertex;
                if (vertexA.Node == null) break;
              }
            }
            // pNodeA is an artificial node for which we need to add some stroke points
            if (vertexA != vertexFrom) {  // not yet end of iteration?
              Point prev = pLinkR.GetPoint(si-1);
              Point next = vertexA.Center;
              if (ortho) {
                if (this.Direction == 180 || this.Direction == 0) {
                  if (si == 2) {  // from real node to first artificial node
                    pLinkR.InsertPoint(si++, new Point(prev.X, prev.Y));
                    pLinkR.InsertPoint(si++, new Point(prev.X, next.Y));
                  } else {
                    Point p = (prevVertexA != null ? prevVertexA.Center : prev);
                    if (p.Y != next.Y) {
                      double mid = layerSpaces[vertexA.Layer-1];
                      pLinkR.InsertPoint(si++, new Point(mid, prev.Y));
                      pLinkR.InsertPoint(si++, new Point(mid, next.Y));
                    }
                  }
                } else {
                  if (si == 2) {  // from real node to first artificial node
                    pLinkR.InsertPoint(si++, new Point(prev.X, prev.Y));
                    pLinkR.InsertPoint(si++, new Point(next.X, prev.Y));
                  } else {
                    Point p = (prevVertexA != null ? prevVertexA.Center : prev);
                    if (p.X != next.X) {
                      double mid = layerSpaces[vertexA.Layer-1];
                      pLinkR.InsertPoint(si++, new Point(prev.X, mid));
                      pLinkR.InsertPoint(si++, new Point(next.X, mid));
                    }
                  }
                }
              } else if (si == 2) {  // for first segment, from pNodeTo to pNodeA
                if (bezier) {  // when style is bezier, for first segment
                  double depthL = Math.Max(10, layerSpaceLeft[vertexTo.Layer]);
                  double depthR = Math.Max(10, layerSpaceRight[vertexTo.Layer]);
                  double f;
                  if (this.Direction == 180) {
                    f = vertexTo.Bounds.X;
                    pLinkR.InsertPoint(si++, new Point(f-depthL, next.Y));
                    pLinkR.InsertPoint(si++, new Point(f, next.Y));
                    pLinkR.InsertPoint(si++, new Point(f+depthR, next.Y));
                  } else if (this.Direction == 90) {
                    f = vertexTo.Bounds.Y+vertexTo.Bounds.Height;
                    pLinkR.InsertPoint(si++, new Point(next.X, f+depthR));
                    pLinkR.InsertPoint(si++, new Point(next.X, f));
                    pLinkR.InsertPoint(si++, new Point(next.X, f-depthL));
                  } else if (this.Direction == 270) {
                    f = vertexTo.Bounds.Y;
                    pLinkR.InsertPoint(si++, new Point(next.X, f-depthL));
                    pLinkR.InsertPoint(si++, new Point(next.X, f));
                    pLinkR.InsertPoint(si++, new Point(next.X, f+depthR));
                  } else {
                    f = vertexTo.Bounds.X+vertexTo.Bounds.Width;
                    pLinkR.InsertPoint(si++, new Point(f+depthR, next.Y));
                    pLinkR.InsertPoint(si++, new Point(f, next.Y));
                    pLinkR.InsertPoint(si++, new Point(f-depthL, next.Y));
                  }
                } else {  // for first segment, regular lines
                  pLinkR.InsertPoint(si++, prev);
                  if (this.Direction == 180 || this.Direction == 0) {
                    //?? make "orthogonal", even though not needed
                    pLinkR.InsertPoint(si++, new Point(prev.X, next.Y));
                    pLinkR.InsertPoint(si++, new Point(next.X, next.Y));
                  } else {
                    //?? make "orthogonal", even though not needed
                    pLinkR.InsertPoint(si++, new Point(next.X, prev.Y));
                    pLinkR.InsertPoint(si++, new Point(next.X, next.Y));
                  }
                }
              } else {  // when style is line or bezier, for intermediate segments
                double depthL = Math.Max(10, layerSpaceLeft[vertexA.Layer]);
                double depthR = Math.Max(10, layerSpaceRight[vertexA.Layer]);
                if (this.Direction == 180) {
                  if (bezier) pLinkR.InsertPoint(si++, new Point(next.X-depthL, next.Y));
                  pLinkR.InsertPoint(si++, next);
                  if (bezier) pLinkR.InsertPoint(si++, new Point(next.X+depthR, next.Y));
                } else if (this.Direction == 90) {
                  if (bezier) pLinkR.InsertPoint(si++, new Point(next.X, next.Y+depthR));
                  pLinkR.InsertPoint(si++, next);
                  if (bezier) pLinkR.InsertPoint(si++, new Point(next.X, next.Y-depthL));
                } else if (this.Direction == 270) {
                  if (bezier) pLinkR.InsertPoint(si++, new Point(next.X, next.Y-depthL));
                  pLinkR.InsertPoint(si++, next);
                  if (bezier) pLinkR.InsertPoint(si++, new Point(next.X, next.Y+depthR));
                } else {
                  if (bezier) pLinkR.InsertPoint(si++, new Point(next.X+depthR, next.Y));
                  pLinkR.InsertPoint(si++, next);
                  if (bezier) pLinkR.InsertPoint(si++, new Point(next.X-depthL, next.Y));
                }
              }
            }
            // Update the to node and iterate.
            vertexTo = vertexA;
          }  // now coming from pNodeFrom
          if (gotoport == null || pLinkR.GetToSpot().IsNotNone) {
            Point prev = pLinkR.GetPoint(si-1);
            Point next = pLinkR.GetPoint(si);
            // make adjustments for reverse links coming "around" the node back into it
            if (ortho) {  // when style is ortho for last segment
              double depth2 = layerSpaceRight[vertexFrom.Layer];
              if (this.Direction == 180 || this.Direction == 0) {
                double s = prev.Y;

                // delete these two InsertPoints if there is an artificial node in the to-node's layer for reverse links
                if (s >= vertexFrom.Bounds.Y && s <= vertexFrom.Bounds.Bottom) {
                  double f = vertexFrom.Center.X + depth2;
                  if (s < (vertexFrom.Bounds.Y+vertexFrom.Bounds.Height/2))
                    s = vertexFrom.Bounds.Y - this.ColumnSpacing/2;
                  else
                    s = vertexFrom.Bounds.Bottom + this.ColumnSpacing/2;
                  pLinkR.InsertPoint(si++, new Point(f, prev.Y));
                  pLinkR.InsertPoint(si++, new Point(f, s));
                }

                pLinkR.InsertPoint(si++, new Point(next.X, s));
                pLinkR.InsertPoint(si++, next);
              } else {
                double s = prev.X;

                // delete these two InsertPoints if there is an artificial node in the to-node's layer for reverse links
                if (s >= vertexFrom.Bounds.X && s <= vertexFrom.Bounds.Right) {
                  double f = vertexFrom.Center.Y + depth2;
                  if (s < (vertexFrom.Bounds.X+vertexFrom.Bounds.Width/2))
                    s = vertexFrom.Bounds.X - this.ColumnSpacing/2;
                  else
                    s = vertexFrom.Bounds.Right + this.ColumnSpacing/2;
                  pLinkR.InsertPoint(si++, new Point(prev.X, f));
                  pLinkR.InsertPoint(si++, new Point(s, f));
                }

                pLinkR.InsertPoint(si++, new Point(s, next.Y));
                pLinkR.InsertPoint(si++, next);
              }
            } else if (bezier) {  // when style is bezier for last segment
              double depthL = Math.Max(10, layerSpaceLeft[vertexFrom.Layer]);
              double depthR = Math.Max(10, layerSpaceRight[vertexFrom.Layer]);
              double f;
              if (this.Direction == 180) {
                f = vertexFrom.Bounds.X+vertexFrom.Bounds.Width;
                pLinkR.SetPoint(si-2, new Point(f, prev.Y));
                pLinkR.SetPoint(si-1, new Point(f+depthR, prev.Y));
              } else if (this.Direction == 90) {
                f = vertexFrom.Bounds.Y;
                pLinkR.SetPoint(si-2, new Point(prev.X, f));
                pLinkR.SetPoint(si-1, new Point(prev.X, f-depthL));
              } else if (this.Direction == 270) {
                f = vertexFrom.Bounds.Y+vertexFrom.Bounds.Height;
                pLinkR.SetPoint(si-2, new Point(prev.X, f));
                pLinkR.SetPoint(si-1, new Point(prev.X, f+depthR));
              } else {
                f = vertexFrom.Bounds.X;
                pLinkR.SetPoint(si-2, new Point(f, prev.Y));
                pLinkR.SetPoint(si-1, new Point(f-depthL, prev.Y));
              }
            } else {  // when style is line for last segment
              //?? make "orthogonal", even though not needed
              if (this.Direction == 180 || this.Direction == 0)
                pLinkR.InsertPoint(si++, new Point(next.X, prev.Y));
              else
                pLinkR.InsertPoint(si++, new Point(prev.X, next.Y));
              pLinkR.InsertPoint(si++, next);
            }
          }
        }
        
        // adjust end points for Bezier links, to look better at the node
        if (golink != null && bezier) {
          if (gofromport != null) {
            if (pLinkR.GetFromSpot().IsNoSpot) {
              Point end = pLinkR.GetPoint(0);
              Point ctrl = pLinkR.GetPoint(2);
              if (end != ctrl) pLinkR.SetPoint(1, new Point((end.X+ctrl.X)/2, (end.Y+ctrl.Y)/2));
            }
            pLinkR.SetPoint(0, pLinkR.GetLinkPoint(gofromnode, gofromport, Spot.None, true, false, gotonode, gotoport));  //???
          }
          if (gotoport != null) {
            if (pLinkR.GetToSpot().IsNoSpot) {
              Point end = pLinkR.GetPoint(pLinkR.PointsCount-1);
              Point ctrl = pLinkR.GetPoint(pLinkR.PointsCount-3);
              if (end != ctrl) pLinkR.SetPoint(pLinkR.PointsCount-2, new Point((end.X+ctrl.X)/2, (end.Y+ctrl.Y)/2));
            }
            pLinkR.SetPoint(pLinkR.PointsCount-1, pLinkR.GetLinkPoint(gotonode, gotoport, Spot.None, false, false, gofromnode, gofromport));  //???
          }
        }

        edge.CommitPosition();
      }

      AvoidOrthogonalOverlaps();
    }

    /// <summary>
    /// Try to avoid overlapping segments of Orthogonal links.
    /// </summary>
    /// <remarks>
    /// This is called as part of <see cref="LayoutLinks"/>.
    /// </remarks>
    protected virtual void AvoidOrthogonalOverlaps() {  //??? this can be improved
      List<Route> orthos = new List<Route>();
      foreach (LayeredDigraphEdge edge in this.Network.Edges) {
        Route pLinkR = edge.Route;
        if (pLinkR != null && pLinkR.Orthogonal && !orthos.Contains(pLinkR)) {
          orthos.Add(pLinkR);
        }
      }
      if (orthos.Count > 0) {
        if (this.Direction == 90 || this.Direction == 270) {
          AdjustOverlapsH(orthos);
        } else {
          AdjustOverlapsV(orthos);
        }
      }
    }

    internal class SegInfo {
      public double Layer;
      public double First;
      public double Last;
      public double ColumnMin;
      public double ColumnMax;
      public int Index;
      public Link Link;
      public int Turns;
    }

    sealed private class SegInfoComparer : IComparer<SegInfo> {
      private SegInfoComparer() { }
      private static SegInfoComparer myDefaultComparer = new SegInfoComparer();
      public static SegInfoComparer Default {
        get { return myDefaultComparer; }
      }
      public int Compare(SegInfo a, SegInfo b) {
        if (a == null || b == null || a == b)
          return 0;
        if (a.Layer < b.Layer) return -1;
        if (a.Layer > b.Layer) return 1;
        if (a.ColumnMin < b.ColumnMin) return -1;
        if (a.ColumnMin > b.ColumnMin) return 1;
        if (a.ColumnMax < b.ColumnMax) return -1;
        if (a.ColumnMax > b.ColumnMax) return 1;
        return 0;
      }
    }

    sealed private class SegInfoComparer2 : IComparer<SegInfo> {
      private SegInfoComparer2(bool f) { myFirst = f; }
      private bool myFirst;

      private static SegInfoComparer2 myDefaultFirstComparer = new SegInfoComparer2(true);
      public static SegInfoComparer2 DefaultFirst {
        get { return myDefaultFirstComparer; }
      }
      private static SegInfoComparer2 myDefaultLastComparer = new SegInfoComparer2(false);
      public static SegInfoComparer2 DefaultLast {
        get { return myDefaultLastComparer; }
      }

      public int Compare(SegInfo a, SegInfo b) {
        if (a == null || b == null || a == b)
          return 0;
        if (myFirst) {
          if (a.First < b.First) return -1;
          if (a.First > b.First) return 1;
        } else {
          if (a.Last < b.Last) return -1;
          if (a.Last > b.Last) return 1;
        }
        if (a.Turns < b.Turns) return 1;
        if (a.Turns > b.Turns) return -1;
        //if ((a.ColumnMax-a.ColumnMin) < (b.ColumnMax-b.ColumnMin)) return -1;
        //if ((a.ColumnMax-a.ColumnMin) > (b.ColumnMax-b.ColumnMin)) return 1;
        if (a.ColumnMin < b.ColumnMin) return -1;
        if (a.ColumnMin > b.ColumnMin) return 1;
        if (a.ColumnMax < b.ColumnMax) return -1;
        if (a.ColumnMax > b.ColumnMax) return 1;
        return 0;
      }
    }

    private static bool IsApprox(double a, double b) {
      double d = a-b;
      return d > -1.0 && d < 1.0;
    }

    private int AdjustOverlapsH(List<Route> routables) {
      double layersize = 2;  // smaller than for document
      int overlaps = 0;
      List<SegInfo> arr = new List<SegInfo>();
      foreach (Route l in routables) {
        if (l != null && l.Orthogonal && l.Curve != LinkCurve.Bezier) {
          for (int i = 2; i < l.PointsCount-3; i++) {
            Point p = l.GetPoint(i);
            Point q = l.GetPoint(i+1);
            if (IsApprox(p.Y, q.Y) && !IsApprox(p.X, q.X)) {  // assuming Vertical orientation for whole layout
              SegInfo info = new SegInfo();
              info.Layer = (double)Math.Floor(p.Y/layersize);
              Point p0 = l.GetPoint(0);
              Point pn = l.GetPoint(l.PointsCount-1);
              info.First = p0.X*p0.X + p0.Y;
              info.Last = pn.X*pn.X + pn.Y;
              info.ColumnMin = Math.Min(p.X, q.X);
              info.ColumnMax = Math.Max(p.X, q.X);
              info.Index = i;
              info.Link = l.Link;
              if (i+2 < l.PointsCount) {
                Point b = l.GetPoint(i-1);
                Point a = l.GetPoint(i+2);
                int t = 0;
                if (b.Y < p.Y) {
                  if (a.Y < p.Y) t = 3;  // down & up
                  else if (p.X < q.X) t = 2;  // crosses segment upward, and jags right
                  else t = 1;  // crosses, and jags left
                } else if (b.Y > p.Y) {
                  if (a.Y > p.Y) t = 0;  // up & down
                  else if (q.X < p.X) t = 2;  // crosses segment downward, and jags left
                  else t = 1;  // crosses, and jags right
                }
                info.Turns = t;
              }
              arr.Add(info);
            }
          }
        }
      }
      if (arr.Count > 1) {
        arr.Sort(SegInfoComparer.Default);
        int firstlayer = 0;
        while (firstlayer < arr.Count) {
          // find all link segments at a layer
          double curlayer = (arr[firstlayer]).Layer;
          int nextlayer = firstlayer+1;
          while (nextlayer < arr.Count && (arr[nextlayer]).Layer == curlayer) nextlayer++;
          if (nextlayer - firstlayer > 1) {
            // now all SegInfos from firstlayer to nextlayer-1 have .Layer == curlayer
            //for (int i = firstlayer; i < nextlayer; i++) {
            //  System.Diagnostics.Trace.Assert((arr[i]).Layer == curlayer);
            //}
            int firstgroup = firstlayer;
            while (firstgroup < nextlayer) {
              // find groups of overlapping link segments
              double curmax = (arr[firstgroup]).ColumnMax;
              int nextgroup = firstlayer+1;
              while (nextgroup < nextlayer && (arr[nextgroup]).ColumnMin < curmax) {
                curmax = Math.Max(curmax, (arr[nextgroup]).ColumnMax);
                nextgroup++;
              }
              int groupcount = nextgroup-firstgroup;
              if (groupcount > 1) {
                // now all SegInfos from firstgroup to nextgroup-1 have some overlap with some other
                // link segment in the same group
                //for (int i = firstgroup; i < nextgroup; i++) {
                //  bool found = false;
                //  for (int j = firstgroup; i < nextgroup; j++) {
                //    if (j == i) continue;
                //    SegInfo a = arr[i];
                //    SegInfo b = arr[j];
                //    if (a.ColumnMax >= b.ColumnMin || b.ColumnMax >= a.ColumnMin) {
                //      found = true;
                //      break;
                //    }
                //  }
                //  System.Diagnostics.Trace.Assert(found);
                //}

                // sort all of the overlapping links by their Last position and Turns number
                arr.Sort(firstgroup, groupcount, SegInfoComparer2.DefaultLast);
                // count how many overlapping links connect to the same port (actually, same port column)
                int samelastportcount = 1;
                double prevportpos = (arr[firstgroup]).Last;
                for (int i = firstgroup; i < nextgroup; i++) {
                  SegInfo si = arr[i];
                  if (si.Last != prevportpos) {
                    samelastportcount++;
                    prevportpos = si.Last;
                  }
                }
                // sort all of the overlapping links by their First position and Turns number
                arr.Sort(firstgroup, groupcount, SegInfoComparer2.DefaultFirst);
                // count how many overlapping links connect to the same port (actually, same port column)
                int samefirstportcount = 1;
                prevportpos = (arr[firstgroup]).First;
                for (int i = firstgroup; i < nextgroup; i++) {
                  SegInfo si = arr[i];
                  if (si.First != prevportpos) {
                    samefirstportcount++;
                    prevportpos = si.First;
                  }
                }
                bool useFirst;
                int sameportcount;
                if (samelastportcount < samefirstportcount) {
                  useFirst = false;
                  sameportcount = samelastportcount;
                  prevportpos = (arr[firstgroup]).Last;
                  // resort by Last position
                  arr.Sort(firstgroup, groupcount, SegInfoComparer2.DefaultLast);
                } else {
                  useFirst = true;
                  sameportcount = samefirstportcount;
                  prevportpos = (arr[firstgroup]).First;
                  // already sorted by First position
                }

                // now spread out overlapping links
                //??? need to do bin packing
                int lay = 0;
                for (int i = firstgroup; i < nextgroup; i++) {
                  SegInfo si = arr[i];
                  if ((useFirst ? si.First : si.Last) != prevportpos) {
                    lay++;
                    prevportpos = (useFirst ? si.First : si.Last);
                  }
                  Route l = si.Link.Route;
                  Point p = l.GetPoint(si.Index);
                  Point q = l.GetPoint(si.Index+1);
                  double dy = 4*(lay-(sameportcount-1)/2.0);  //??? 4
                  if (l.Routing != LinkRouting.AvoidsNodes || IsUnoccupied2(p.X, p.Y+dy, q.X, q.Y+dy)) {
                    overlaps++;
                    l.SetPoint(si.Index, new Point(p.X, p.Y+dy));
                    l.SetPoint(si.Index+1, new Point(q.X, q.Y+dy));
                  }
                }
              }
              firstgroup = nextgroup;
            }
          }
          firstlayer = nextlayer;
        }
      }
      return overlaps;
    }

    private int AdjustOverlapsV(List<Route> routables) {
      double layersize = 2;  // smaller than for document
      int overlaps = 0;
      List<SegInfo> arr = new List<SegInfo>();
      foreach (Route l in routables) {
        if (l != null && l.Orthogonal && l.Curve != LinkCurve.Bezier) {
          for (int i = 2; i < l.PointsCount-3; i++) {
            Point p = l.GetPoint(i);
            Point q = l.GetPoint(i+1);
            if (IsApprox(p.X, q.X) && !IsApprox(p.Y, q.Y)) {  // assuming Horizontal orientation for whole layout
              SegInfo info = new SegInfo();
              info.Layer = (double)Math.Floor(p.X/layersize);
              Point p0 = l.GetPoint(0);
              Point pn = l.GetPoint(l.PointsCount-1);
              info.First = p0.X + p0.Y*p0.Y;
              info.Last = pn.X + pn.Y*pn.Y;
              info.ColumnMin = Math.Min(p.Y, q.Y);
              info.ColumnMax = Math.Max(p.Y, q.Y);
              info.Index = i;
              info.Link = l.Link;
              if (i+2 < l.PointsCount) {
                Point b = l.GetPoint(i-1);
                Point a = l.GetPoint(i+2);
                int t = 0;
                if (b.X < p.X) {
                  if (a.X < p.X) t = 3;
                  else if (p.Y < q.Y) t = 2;
                  else t = 1;
                } else if (b.X > p.X) {
                  if (a.X > p.X) t = 0;
                  else if (q.Y < p.Y) t = 2;
                  else t = 1;
                }
                info.Turns = t;
              }
              arr.Add(info);
            }
          }
        }
      }
      if (arr.Count > 1) {
        arr.Sort(SegInfoComparer.Default);
        int firstlayer = 0;
        while (firstlayer < arr.Count) {
          // find all link segments at a layer
          double curlayer = (arr[firstlayer]).Layer;
          int nextlayer = firstlayer+1;
          while (nextlayer < arr.Count && (arr[nextlayer]).Layer == curlayer) nextlayer++;
          if (nextlayer - firstlayer > 1) {
            // now all SegInfos from firstlayer to nextlayer-1 have .Layer == curlayer
            //for (int i = firstlayer; i < nextlayer; i++) {
            //  System.Diagnostics.Trace.Assert((arr[i]).Layer == curlayer);
            //}
            int firstgroup = firstlayer;
            while (firstgroup < nextlayer) {
              // find groups of overlapping link segments
              double curmax = (arr[firstgroup]).ColumnMax;
              int nextgroup = firstlayer+1;
              while (nextgroup < nextlayer && (arr[nextgroup]).ColumnMin < curmax) {
                curmax = Math.Max(curmax, (arr[nextgroup]).ColumnMax);
                nextgroup++;
              }
              int groupcount = nextgroup-firstgroup;
              if (groupcount > 1) {
                // now all SegInfos from firstgroup to nextgroup-1 have some overlap with some other
                // link segment in the same group
                //for (int i = firstgroup; i < nextgroup; i++) {
                //  bool found = false;
                //  for (int j = firstgroup; i < nextgroup; j++) {
                //    if (j == i) continue;
                //    SegInfo a = arr[i];
                //    SegInfo b = arr[j];
                //    if (a.ColumnMax >= b.ColumnMin || b.ColumnMax >= a.ColumnMin) {
                //      found = true;
                //      break;
                //    }
                //  }
                //  System.Diagnostics.Trace.Assert(found);
                //}

                // sort all of the overlapping links by their Last position and Turns number
                arr.Sort(firstgroup, groupcount, SegInfoComparer2.DefaultLast);
                // count how many overlapping links connect to the same port (actually, same port column)
                int samelastportcount = 1;
                double prevportpos = (arr[firstgroup]).Last;
                for (int i = firstgroup; i < nextgroup; i++) {
                  SegInfo si = arr[i];
                  if (si.Last != prevportpos) {
                    samelastportcount++;
                    prevportpos = si.Last;
                  }
                }
                // sort all of the overlapping links by their First position and Turns number
                arr.Sort(firstgroup, groupcount, SegInfoComparer2.DefaultFirst);
                // count how many overlapping links connect to the same port (actually, same port column)
                int samefirstportcount = 1;
                prevportpos = (arr[firstgroup]).First;
                for (int i = firstgroup; i < nextgroup; i++) {
                  SegInfo si = arr[i];
                  if (si.First != prevportpos) {
                    samefirstportcount++;
                    prevportpos = si.First;
                  }
                }
                bool useFirst;
                int sameportcount;
                if (samelastportcount < samefirstportcount) {
                  useFirst = false;
                  sameportcount = samelastportcount;
                  prevportpos = (arr[firstgroup]).Last;
                  // resort by Last position
                  arr.Sort(firstgroup, groupcount, SegInfoComparer2.DefaultLast);
                } else {
                  useFirst = true;
                  sameportcount = samefirstportcount;
                  prevportpos = (arr[firstgroup]).First;
                  // already sorted by First position
                }

                // now spread out overlapping links
                //??? need to do bin packing
                int lay = 0;
                for (int i = firstgroup; i < nextgroup; i++) {
                  SegInfo si = arr[i];
                  if ((useFirst ? si.First : si.Last) != prevportpos) {
                    lay++;
                    prevportpos = (useFirst ? si.First : si.Last);
                  }
                  Route l = si.Link.Route;
                  Point p = l.GetPoint(si.Index);
                  Point q = l.GetPoint(si.Index+1);
                  double dx = 4*(lay-(sameportcount-1)/2.0);  //??? 4
                  if (l.Routing != LinkRouting.AvoidsNodes || IsUnoccupied2(p.X+dx, p.Y, q.X+dx, q.Y)) {
                    overlaps++;
                    l.SetPoint(si.Index, new Point(p.X+dx, p.Y));
                    l.SetPoint(si.Index+1, new Point(q.X+dx, q.Y));
                  }
                }
              }
              firstgroup = nextgroup;
            }
          }
          firstlayer = nextlayer;
        }
      }
      return overlaps;
    }

    private bool IsUnoccupied2(double px, double py, double qx, double qy) {
      if (this.Diagram == null) return true;
      DiagramPanel panel = this.Diagram.Panel;
      if (panel == null) return true;
      double minx = Math.Min(px, qx);
      double miny = Math.Min(py, qy);
      double maxx = Math.Max(px, qx);
      double maxy = Math.Max(py, qy);
      return panel.IsUnoccupied(new Rect(minx, miny, maxx-minx, maxy-miny), null);
    }


    /// <summary>
    /// Identifies the <see cref="LayerSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LayerSpacingProperty;
    /// <summary>
    /// Gets or sets the size of each layer
    /// </summary>
    /// <value>The value must be positive.  It defaults to 25.</value>
    [DefaultValue(25.0)]
    [Description("the size of each layer")]
    public double LayerSpacing {
      get { return (double)GetValue(LayerSpacingProperty); }
      set { SetValue(LayerSpacingProperty, value); }
    }
    private static void OnLayerSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      LayeredDigraphLayout layout = (LayeredDigraphLayout)d;
      double v = (double)e.NewValue;
      if (v <= 0)
        layout.LayerSpacing = (double)e.OldValue;
      else
        layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="ColumnSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnSpacingProperty;
    /// <summary>
    /// Gets or sets the size of each column
    /// </summary>
    /// <value>The value must be positive.  It defaults to 25.</value>
    [DefaultValue(25.0)]
    [Description("the size of each column")]
    public double ColumnSpacing {
      get { return (double)GetValue(ColumnSpacingProperty); }
      set { SetValue(ColumnSpacingProperty, value); }
    }
    private static void OnColumnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      LayeredDigraphLayout layout = (LayeredDigraphLayout)d;
      double v = (double)e.NewValue;
      if (v <= 0)
        layout.ColumnSpacing = (double)e.OldValue;
      else
        layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="Direction"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DirectionProperty;
    /// <summary>
    /// Gets or sets which direction the graph grows toward.
    /// </summary>
    /// <value>
    /// 0 is towards the right, 90 is downwards, 180 is towards the left, and 270 is upwards.
    /// The default value is zero.
    /// </value>
    [DefaultValue(0)]
    [Description("in which direction is the graph laid out")]
    public double Direction {
      get { return (double)GetValue(DirectionProperty); }
      set { SetValue(DirectionProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="CycleRemoveOption"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CycleRemoveOptionProperty;
    /// <summary>
    /// Gets or sets which cycle removal option is being used.
    /// </summary>
    [DefaultValue(LayeredDigraphCycleRemove.DepthFirst)]
    [Description("which cycle removal option is being used")]
    public LayeredDigraphCycleRemove CycleRemoveOption {
      get { return (LayeredDigraphCycleRemove)GetValue(CycleRemoveOptionProperty); }
      set { SetValue(CycleRemoveOptionProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="LayeringOption"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LayeringOptionProperty;
    /// <summary>
    /// Gets or sets which layering option is being used.
    /// </summary>
    [DefaultValue(LayeredDigraphLayering.OptimalLinkLength)]
    [Description("which layering option is being used")]
    public LayeredDigraphLayering LayeringOption {
      get { return (LayeredDigraphLayering)GetValue(LayeringOptionProperty); }
      set { SetValue(LayeringOptionProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="InitializeOption"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InitializeOptionProperty;
    /// <summary>
    /// Gets or sets which indices initialization option is being used.
    /// </summary>
    [DefaultValue(LayeredDigraphInitIndices.DepthFirstOut)]
    [Description("which indices initialization option is being used")]
    public LayeredDigraphInitIndices InitializeOption {
      get { return (LayeredDigraphInitIndices)GetValue(InitializeOptionProperty); }
      set { SetValue(InitializeOptionProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="Iterations"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IterationsProperty;
    /// <summary>
    /// Gets or sets the number of iterations are to be done.
    /// </summary>
    /// <value>The value must be non-negative.  The default value is 4.</value>
    [DefaultValue(4)]
    [Description("the number of iterations are to be done")]
    public int Iterations {
      get { return (int)GetValue(IterationsProperty); }
      set { SetValue(IterationsProperty, value); }
    }
    private static void OnIterationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      LayeredDigraphLayout layout = (LayeredDigraphLayout)d;
      int v = (int)e.NewValue;
      if (v < 0) 
        layout.Iterations = (int)e.OldValue;
      else
        layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="AggressiveOption"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AggressiveOptionProperty;
    /// <summary>
    /// Gets or sets which Aggressive Option is being used to look for link crossings.
    /// </summary>
    [DefaultValue(LayeredDigraphAggressive.Less)]
    [Description("how aggressive to be about looking for link crossings")]
    public LayeredDigraphAggressive AggressiveOption {
      get { return (LayeredDigraphAggressive)GetValue(AggressiveOptionProperty); }
      set { SetValue(AggressiveOptionProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="PackOption"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PackOptionProperty;
    /// <summary>
    /// Gets or sets the options used by <see cref="StraightenAndPack"/>.
    /// </summary>
    /// <value>is a bitwise combination of <see cref="LayeredDigraphPack"/> values</value>
    [DefaultValue(LayeredDigraphPack.Expand | LayeredDigraphPack.Median | LayeredDigraphPack.Straighten)]
    [Description("flags that can be combined to control how to straighten links and pack nodes together")]
    public LayeredDigraphPack PackOption {
      get { return (LayeredDigraphPack)GetValue(PackOptionProperty); }
      set { SetValue(PackOptionProperty, value); }
    }


    /// <summary>
    /// Identifies the <see cref="SetsPortSpots"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SetsPortSpotsProperty;
    /// <summary>
    /// Gets or sets whether the FromSpot and ToSpot of each link
    /// should be set to values appropriate for the given <see cref="Direction"/>.
    /// </summary>
    /// <value>The default value is true</value>
    [DefaultValue(true)]
    [Description("whether to set Route.FromSpot and Route.ToSpot")]
    public bool SetsPortSpots {
      get { return (bool)GetValue(SetsPortSpotsProperty); }
      set { SetValue(SetsPortSpotsProperty, value); }
    }
  

    /// <summary>
    /// Gets the largest layer value.
    /// </summary>
    public int MaxLayer {
      get { return maxLayer; }
    }
  
    /// <summary>
    /// Gets the largest index value.
    /// </summary>
    public int MaxIndex {
      get { return maxIndex; }
    }
  
    /// <summary>
    /// Gets the largest column value.
    /// </summary>
    public int MaxColumn {
      get { return maxColumn; }
    }
  
    /// <summary>
    /// Gets the smallest index layer
    /// </summary>
    public int MinIndexLayer {
      get { return minIndexLayer; }
    }
  
    /// <summary>
    /// Gets the largest index layer.
    /// </summary>
    public int MaxIndexLayer {
      get { return maxIndexLayer; }
    }
  
    /// <summary>
    /// Returns the indices array.
    /// </summary>
    /// <value>
    /// The value must not be modified.
    /// </value>
    public int[] GetIndices() {
      return indices;
    }



    private void ClearCaches() {
      maxIndex = -1;
      minIndexLayer = 0;
      maxIndexLayer = 0;
      mySavedLayout = null;
      myMedians = null;
      myColumnsPS = null;
      myCrossings = null;
      myLayers = null;
      for (int i = 0; i < myCachedNodeArrayLists.Length; i++)
        myCachedNodeArrayLists[i] = null;
    }

    // try to reuse some of the temporary node arrays
    private LayeredDigraphVertex[] GetCachedNodeArrayList(int unfixedLayer) {
      LayeredDigraphVertex[] unfixedLayerNodes;
      int num = indices[unfixedLayer];

      if (num >= myCachedNodeArrayLists.Length) {
        LayeredDigraphVertex[][] temp = new LayeredDigraphVertex[num + 50][];
        for (int i = 0; i < myCachedNodeArrayLists.Length; i++)
          temp[i] = myCachedNodeArrayLists[i];
        myCachedNodeArrayLists = temp;
      }
      if (myCachedNodeArrayLists[num] == null) {
        unfixedLayerNodes = new LayeredDigraphVertex[num];
      } else {
        unfixedLayerNodes = myCachedNodeArrayLists[num];
        myCachedNodeArrayLists[num] = null;
      }
      foreach (LayeredDigraphVertex vertex in myLayers[unfixedLayer]) {
        unfixedLayerNodes[vertex.Index] = vertex;
      }





      return unfixedLayerNodes;
    }

    private void FreeCachedNodeArrayList(int unfixedLayer, LayeredDigraphVertex[] nodes) {
      myCachedNodeArrayLists[indices[unfixedLayer]] = nodes;
    }


    /*********************************************************************************************/
    /*********************************************************************************************/

    // private variables.

    //After layering is finished, all nodes U's layer field will be <= maxLayer, with equality in at least one case.
    private int maxLayer;
    //After initializing indices, all nodes U's index field will be <= maxIndex, with equality in at least one case.
    private int maxIndex;
    //After initializing columns, all nodes U's column field will be <= maxColumn, with equality in at least one case.
    private int maxColumn;
    //After initializing indices, minIndexLayer indicates the layer which has a minimal number of nodes
    private int minIndexLayer;
    //After initializing indices, maxIndexLayer indicates the layer which has a maximal number of nodes.
    private int maxIndexLayer;
    //After initializing indices, indices[layer] indicates the number of nodes in the layer.
    private int[] indices;
    private double[] layerSpaceLeft;
    private double[] layerSpaceRight;
    private double[] layerPositions;
    private double[] layerSpaces;
  
    private int myDepthFirstSearchCycleRemovalTime;

    private int[] mySavedLayout;
    private double[] myMedians;
    private int[] myColumnsPS;
    private int[] myCrossings;
    private int myComponent;
    private LayeredDigraphVertex[][] myLayers;
    private LayeredDigraphVertex[][] myCachedNodeArrayLists = new LayeredDigraphVertex[100][];
  }


  /// <summary>
  /// This enumeration controls how <see cref="LayeredDigraphLayout.RemoveCycles"/> changes the graph
  /// to make sure it contains no cycles or loops.
  /// </summary>
  public enum LayeredDigraphCycleRemove {
    /// <summary>
    /// Remove cycles using <see cref="LayeredDigraphLayout.DepthFirstSearchCycleRemoval"/>
    /// </summary>
    DepthFirst,
    /// <summary>
    /// Remove cycles using <see cref="LayeredDigraphLayout.GreedyCycleRemoval"/>
    /// </summary>
    Greedy
  }

  /// <summary>
  /// This enumeration controls how <see cref="LayeredDigraphLayout.AssignLayers"/>
  /// assigns each node of the graph to a layer.
  /// </summary>
  public enum LayeredDigraphLayering {
    /// <summary>
    /// Assign layers using <see cref="LayeredDigraphLayout.OptimalLinkLengthLayering"/>
    /// </summary>
    OptimalLinkLength,
    /// <summary>
    /// Assign layers using <see cref="LayeredDigraphLayout.LongestPathSinkLayering"/>
    /// </summary>
    LongestPathSink,
    /// <summary>
    /// Assign layers using <see cref="LayeredDigraphLayout.LongestPathSourceLayering"/>
    /// </summary>
    LongestPathSource
  }

  /// <summary>
  /// This enumeration controls how <see cref="LayeredDigraphLayout.InitializeIndices"/>
  /// determines the array of indices for each layer.
  /// </summary>
  public enum LayeredDigraphInitIndices {
    /// <summary>
    /// Initialize using <see cref="LayeredDigraphLayout.DepthFirstOutInitializeIndices"/>
    /// </summary>
    DepthFirstOut,
    /// <summary>
    /// Initialize using <see cref="LayeredDigraphLayout.DepthFirstInInitializeIndices"/>
    /// </summary>
    DepthFirstIn,
    /// <summary>
    /// Initialize using <see cref="LayeredDigraphLayout.NaiveInitializeIndices"/>
    /// </summary>
    Naive
  }

  /// <summary>
  /// This enumeration controls how much effort <see cref="LayeredDigraphLayout.ReduceCrossings"/>
  /// puts into trying to look for link crossings.
  /// </summary>
  public enum LayeredDigraphAggressive {
    /// <summary>
    /// The fastest, but poorest, crossing reduction algorithm
    /// </summary>
    None,
    /// <summary>
    /// The faster, less agressive crossing reduction algorithm
    /// </summary>
    Less,
    /// <summary>
    /// The slower, more agressive crossing reduction algorithm
    /// </summary>
    More
  }

  /// <summary>
  /// These enumerated values can be bitwise combined as values for
  /// the <see cref="LayeredDigraphLayout.PackOption"/> property, which 
  /// controls how much and what kinds of effort <see cref="LayeredDigraphLayout.StraightenAndPack"/> makes.
  /// </summary>
  [Flags]
  public enum LayeredDigraphPack {
    /// <summary>
    /// Do minimal work in <see cref="LayeredDigraphLayout.StraightenAndPack"/>.
    /// </summary>
    None=0,
    /// <summary>
    /// This option gives more chances for <see cref="LayeredDigraphLayout.StraightenAndPack"/>
    /// to improve the layout of the network, but is very expensive in time for large networks.
    /// </summary>
    Expand=1,
    /// <summary>
    /// This option tries to have <see cref="LayeredDigraphLayout.StraightenAndPack"/>
    /// straighten many of the links that cross layers.
    /// </summary>
    Straighten=2,
    /// <summary>
    /// This option tries to have <see cref="LayeredDigraphLayout.StraightenAndPack"/>
    /// center groups of nodes based on their relationships with nodes in other layers.
    /// </summary>
    Median=4,

    //ForceExpand = 8,  // internal use

    /// <summary>
    /// Enable all options for the <see cref="LayeredDigraphLayout.StraightenAndPack"/> method.
    /// </summary>
    All=0xF
  }
}
