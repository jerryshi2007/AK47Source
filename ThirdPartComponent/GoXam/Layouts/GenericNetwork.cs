
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
using System.Linq;
using System.Windows;
using Northwoods.GoXam;
using Northwoods.GoXam.Tool;

namespace Northwoods.GoXam.Layout {


  /// <summary>
  /// This provides an abstract view of a <see cref="IEnumerable{Part}"/> as a
  /// network (graph) of vertexes and directed edges.  These vertexes and edges correspond to
  /// <see cref="Northwoods.GoXam.Part"/>s (<see cref="Node"/>s or <see cref="Link"/>s)
  /// provided in the <see cref="IEnumerable{Part}"/>.
  /// This class provides a framework for manipulating the
  /// state of nodes and links without modifying the structure of the original model.
  /// </summary>
  /// <typeparam name="V">a Type of <see cref="Vertex"/></typeparam>
  /// <typeparam name="E">a Type of <see cref="Edge"/></typeparam>
  /// <typeparam name="Y">a Type of <see cref="IDiagramLayout"/></typeparam>
  public class GenericNetwork<V, E, Y>
    where V : GenericNetwork<V, E, Y>.Vertex, new()
    where E : GenericNetwork<V, E, Y>.Edge, new()
    where Y : IDiagramLayout {
    /// <summary>
    /// Constructs an empty network.
    /// </summary>
    /// <remarks>
    /// Use this default constructor to create an empty network.
    /// Call <see cref="AddNodesAndLinks"/> to automatically add
    /// network nodes and links, or call <see cref="AddNode(Node)"/> and <see cref="LinkVertexes"/>
    /// explicitly to have more detailed control over the exact graph that is laid out.
    /// </remarks>
    public GenericNetwork() { }

    /// <summary>
    /// Allocate a new instance of <typeparamref name="V"/>.
    /// </summary>
    /// <returns></returns>
    public virtual V CreateVertex() {
      return new V();
    }

    /// <summary>
    /// Allocate a new instance of <typeparamref name="E"/>.
    /// </summary>
    /// <returns></returns>
    public virtual E CreateEdge() {
      return new E();
    }

    /// <summary>
    /// Adds the objects in an <see cref="IEnumerable{Part}"/> to the network.
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="links"></param>
    /// <remarks>
    /// This is usually more convenient than repeatedly calling <see cref="AddNode"/> and <see cref="AddLink"/> appropriately.
    /// </remarks>
    public virtual void AddNodesAndLinks(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      if (nodes == null) return;
      // First, add all Nodes in the collection, that are not links,
      //  as vertexes to the network.
      foreach (Node node in nodes) {
        if (node != null) {
          if (FindVertex(node) != null) continue;
          V pNetworkNode = CreateVertex();
          pNetworkNode.Network = this;
          pNetworkNode.Node = node;
          AddVertex(pNetworkNode);
        }
      }

      // Second, add all node-level Links as links to the network.
      // Note: a link cannot be added to the network unless both of
      //  its to and from nodes are in the network.  Hence, it is possible
      //  to select a Link without selecting the Nodes at its
      //  endpoints, but the Link will not be added to the network.
      if (links != null) {
        foreach (Link link in links) {
          if (link != null) {
            if (FindEdge(link) != null) continue;

            Node fromNode = link.FromNode;
            Node toNode = link.ToNode;
            if (fromNode == toNode) continue;

            // Verify that the top-level Vertexes for the from and to nodes
            //  are in the network and add a link between them.
            V fromVertex = FindGroupVertex(fromNode);
            V toVertex = FindGroupVertex(toNode);
            if (fromVertex != null && toVertex != null) {
              LinkVertexes(fromVertex, toVertex, link);
            }
          }
        }
      }
    }

    private V FindGroupVertex(Node n) {
      if (n == null) return null;
      Node visn = n.FindVisibleNode(null);
      if (visn == null) return null;
      V v = FindVertex(visn);
      if (v != null) return v;
      foreach (Group g in visn.ContainingGroups) {
        v = FindVertex(g);
        if (v != null) return v;
      }
      return null;
    }

    /// <summary>
    /// Removes all vertexes and edges from the network, resulting in an empty network.
    /// </summary>
    public void Clear() {
      myVertexes = new VertexList();
      myEdges = new EdgeList();
      myNodeToVertexDictionary = new Dictionary<Node, V>();
      myLinkToEdgeDictionary = new Dictionary<Link, E>();
    }

    /// <summary>
    /// Adds a <see cref="Vertex"/> to the Network.
    /// </summary>
    /// <param name="vertex"></param>
    public void AddVertex(V vertex) {
      if (vertex == null) return;
      myVertexes.Add(vertex);
      Node obj = vertex.Node;
      if (obj != null) {
        myNodeToVertexDictionary[obj] = vertex;
      }
      vertex.Network = this;
    }

    /// <summary>
    /// This convenience method makes sure there is a <see cref="Vertex"/>
    /// in this network corresponding to a <see cref="Node"/>.
    /// </summary>
    /// <param name="node">a Node</param>
    /// <returns>a <see cref="Vertex"/> in this network</returns>
    public V AddNode(Node node) {
      if (node == null) return null;
      V lnode = FindVertex(node);
      if (lnode == null) {
        lnode = CreateVertex();
        lnode.Node = node;
        AddVertex(lnode);
      }
      return lnode;
    }

    /// <summary>
    /// Removes a <see cref="Vertex"/> from the network.
    /// </summary>
    /// <param name="vertex"></param>
    /// <remarks>
    /// This function also deletes all edges to or from the vertex.
    /// Performs nothing if the edge is not in the network.
    /// </remarks>
    public void DeleteVertex(V vertex) {
      if (vertex == null) return;
      // Find the node in the network and remove.
      int pos = myVertexes.IndexOf(vertex);
      if (pos != -1) {
        RemoveVertex(vertex, pos);

        // Delete all links from predecessors of the node.
        EdgeList a = vertex.SourceEdgesList;
        for (int i = a.Count - 1; i >= 0; i--) {
          E link = a[i];
          DeleteEdge(link);
        }

        // Delete all links to successors of the node.
        a = vertex.DestinationEdgesList;
        for (int i = a.Count - 1; i >= 0; i--) {
          E link = a[i];
          DeleteEdge(link);
        }
      }
    }

    private void RemoveVertex(V vertex, int pos) {
      if (vertex == null) return;

      if (pos < 0) {
        pos = myVertexes.IndexOf(vertex);
      }
      myVertexes.RemoveAt(pos);
      Node obj = vertex.Node;
      if (obj != null) {
        myNodeToVertexDictionary.Remove(obj);
      }
      vertex.Network = null;
    }

    /// <summary>
    /// This convenience method deletes any <see cref="Vertex"/>
    /// corresponding to a <see cref="Node"/>.
    /// </summary>
    /// <param name="node">a Node</param>
    public void DeleteNode(Node node) {
      if (node == null) return;
      V lnode = FindVertex(node);
      if (lnode != null) {
        DeleteVertex(lnode);
      }
    }

    /// <summary>
    /// Returns the <see cref="Vertex"/> which was constructed for the <see cref="Node"/>.
    /// </summary>
    /// <param name="node"></param>
    /// <returns>Returns the network <see cref="Vertex"/> associated with the <see cref="Node"/>,
    /// or null if no such vertex exists.</returns>
    public V FindVertex(Node node) {
      if (node == null) return null;
      V vertex;
      myNodeToVertexDictionary.TryGetValue(node, out vertex);
      return vertex;
    }

    /// <summary>
    /// Adds an <see cref="Edge"/> to the network.
    /// Although this method is provided for completeness,
    /// <see cref="LinkVertexes"/> provides a more efficient manner of linking
    /// nodes already in the network.
    /// </summary>
    /// <param name="edge"></param>
    /// <remarks>
    /// This adds the link to its ToVertex
    /// source links list, and to its FromVertex's
    /// destination links list.
    /// </remarks>
    public void AddEdge(E edge) {
      if (edge == null) return;
      myEdges.Add(edge);
      Link obj = edge.Link;
      if (obj != null && FindEdge(obj) == null) {
        myLinkToEdgeDictionary[obj] = edge;
      }

      V toNode = edge.ToVertex;
      if (toNode != null)
        toNode.AddSourceEdge(edge);

      V fromNode = edge.FromVertex;
      if (fromNode != null)
        fromNode.AddDestinationEdge(edge);

      edge.Network = this;
    }

    /// <summary>
    /// This convenience method takes a <see cref="Link"/>,
    /// and returns an <see cref="Edge"/> that has been added to this network.
    /// </summary>
    /// <param name="link">a <see cref="Link"/></param>
    /// <returns>an <see cref="Edge"/> in this network</returns>
    /// <remarks>
    /// If <see cref="FindEdge"/> returns null, this method creates a new <see cref="Edge"/>,
    /// makes sure the <see cref="Northwoods.GoXam.Link.FromNode"/> and <see cref="Northwoods.GoXam.Link.ToNode"/> have
    /// corresponding <see cref="Vertex"/>es in the network, and adds the edge itself
    /// to the network.
    /// </remarks>
    public E AddLink(Link link) {
      if (link == null) return null;
      Node fromNode = link.FromNode;
      Node toNode = link.ToNode;
      E edge = FindEdge(link);
      if (edge == null) {
        edge = CreateEdge();
        edge.Link = link;

        if (fromNode != null) {
          edge.FromVertex = AddNode(fromNode);
        }

        if (toNode != null) {
          edge.ToVertex = AddNode(toNode);
        }

        AddEdge(edge);
      } else {
        if (fromNode != null) {
          edge.FromVertex = AddNode(fromNode);
        } else {
          edge.FromVertex = null;
        }

        if (toNode != null) {
          edge.ToVertex = AddNode(toNode);
        } else {
          edge.ToVertex = null;
        }
      }
      return edge;
    }

    /// <summary>
    /// Deletes an <see cref="Edge"/> from the network.
    /// </summary>
    /// <param name="edge"></param>
    /// <remarks>
    /// Also removes the edge from its ToVertex's predecessor list
    /// and from its FromVertex's successor list.
    /// Performs nothing if the edge is not in the network.
    /// </remarks>
    public void DeleteEdge(E edge) {
      if (edge == null) return;

      // Remove the link from its to node's predecessor list.
      V toNode = edge.ToVertex;
      if (toNode != null)
        toNode.DeleteSourceEdge(edge);

      // Remove the link from its from node's successor list.
      V fromNode = edge.FromVertex;
      if (fromNode != null)
        fromNode.DeleteDestinationEdge(edge);

      RemoveEdge(edge);
    }

    private void RemoveEdge(E edge) {
      if (edge == null) return;

      // Find the link in the network and remove.
      int pos = myEdges.IndexOf(edge);
      if (pos != -1) {
        myEdges.RemoveAt(pos);
        Link obj = edge.Link;
        if (obj != null && FindEdge(obj) == edge) {
          myLinkToEdgeDictionary.Remove(obj);
        }
        edge.Network = null;
      }
    }

    /// <summary>
    /// This convenience method makes sure a <see cref="Link"/>
    /// does not have a <see cref="Edge"/> in this network.
    /// </summary>
    /// <param name="link"></param>
    /// <remarks>
    /// This just calls <see cref="DeleteEdge"/> if
    /// <see cref="FindEdge"/> finds an edge.
    /// </remarks>
    public void DeleteLink(Link link) {
      if (link == null) return;
      E llink = FindEdge(link);
      if (llink != null) {
        DeleteEdge(llink);
      }
    }


    /// <summary>
    /// Returns the <see cref="Edge"/> which was constructed for the <see cref="Link"/>.
    /// </summary>
    /// <param name="link"></param>
    /// <returns>Returns the edge that was constructed with the link or
    /// null if no such edge exists.</returns>
    public E FindEdge(Link link) {
      if (link == null) return null;
      E edge;
      myLinkToEdgeDictionary.TryGetValue(link, out edge);
      return edge;
    }

    /// <summary>
    /// Links two nodes already in the network and returns the created <see cref="Edge"/>.
    /// </summary>
    /// <param name="fromVertex"></param>
    /// <param name="toVertex"></param>
    /// <param name="link"> the <see cref="Link"/> to which the created edge should correspond (may be null)</param>
    /// <returns>Returns the <see cref="Edge"/> created when fromVertex and toVertex are linked.</returns>
    public E LinkVertexes(V fromVertex, V toVertex, Link link) {
      if (fromVertex == null || toVertex == null) return null;
      // Verify that the from and to nodes are in the network.
      if (fromVertex.Network == this && toVertex.Network == this) {
        E pNetworkLink = CreateEdge();
        pNetworkLink.Link = link;
        pNetworkLink.FromVertex = fromVertex;
        pNetworkLink.ToVertex = toVertex;
        AddEdge(pNetworkLink);
        return pNetworkLink;
      }
      return null;
    }


    /// <summary>
    /// Reverses the direction of an <see cref="Edge"/> in the network.
    /// </summary>
    /// <param name="edge"></param>
    public void ReverseEdge(E edge) {
      if (edge == null) return;
      V fromNode = edge.FromVertex;
      V toNode = edge.ToVertex;

      if (fromNode == null || toNode == null) return;

      fromNode.DeleteDestinationEdge(edge);
      toNode.DeleteSourceEdge(edge);

      edge.ReverseEdge();

      fromNode.AddSourceEdge(edge);
      toNode.AddDestinationEdge(edge);
    }


    /// <summary>
    /// Deletes all <see cref="Edge"/>s whose "to vertex" and "from vertex" are the same vertex.
    /// </summary>
    public void DeleteSelfEdges() {
      EdgeList deleteList = new EdgeList();
      foreach (E link in this.Edges) {
        if (link.FromVertex == link.ToVertex) {
          deleteList.Add(link);
        }
      }
      for (int i = 0; i < deleteList.Count; i++) {
        DeleteEdge(deleteList[i]);
      }
    }

    /// <summary>
    /// Delete all vertexes and edges that have no <see cref="Part"/> associated with them.
    /// </summary>
    public void DeleteArtificialVertexes() {
      VertexList dummies = new VertexList();
      foreach (V n in this.Vertexes) {
        if (n.Node == null)
          dummies.Add(n);
      }
      foreach (V n in dummies) {
        DeleteVertex(n);
      }
      EdgeList dummies2 = new EdgeList();
      foreach (E l in this.Edges) {
        if (l.Link == null)
          dummies2.Add(l);
      }
      foreach (E l in dummies2) {
        DeleteEdge(l);
      }
    }

    internal void DeleteUselessEdges() {
      EdgeList deleteList = new EdgeList();
      foreach (E link in this.Edges) {
        if (link.FromVertex == null || link.ToVertex == null) {
          deleteList.Add(link);
        }
      }
      for (int i = 0; i < deleteList.Count; i++) {
        DeleteEdge(deleteList[i]);
      }
    }

    internal static bool IsSingleton(V n) {
      if (n.SourceEdgesList.Count > 0) return false;
      if (n.DestinationEdgesList.Count > 0) return false;
      return true;
    }

    /// <summary>
    /// Modify this network by splitting it up into separate subnetworks,
    /// each of which has all of its vertexes connected to each other, but not
    /// to any vertexes in any other subnetworks.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerator{G}"/> of networks,
    /// sorted in order of decreasing vertex count.
    /// </returns>
    /// <remarks>
    /// This method will first delete from this network all artifical vertexes and
    /// all edges that do not connect two different vertexes.
    /// Afterwards, this original network may be empty or may contain all of the
    /// singleton vertexes, each of which had no edges connecting it to any other vertexes.
    /// </remarks>
    public IEnumerator<G> SplitIntoSubNetworks<G>() where G :  GenericNetwork<V, E, Y>, new() {
      DeleteArtificialVertexes();
      DeleteUselessEdges();
      DeleteSelfEdges();
      List<G> nets = new List<G>();
      bool found = true;
      while (found) {
        found = false;
        foreach (V n in this.Vertexes) {
          if (IsSingleton(n)) continue;
          G net = new G();
          nets.Add(net);
          TraverseSubnet(net, n);
          found = true;  // start over again with all nodes
          break;
        }
      }
      nets.Sort(new NetworkCountComparer<G>());
      return nets.GetEnumerator();
    }

    private void TraverseSubnet(GenericNetwork<V, E, Y> net, V n) {
      if (n == null) return;
      if (n.Network == net) return;
      RemoveVertex(n, -1);
      net.AddVertex(n);
      foreach (E link in n.SourceEdges) {
        if (link.Network == net) continue;
        RemoveEdge(link);
        net.AddEdge(link);
        TraverseSubnet(net, link.FromVertex);
      }
      foreach (E link in n.DestinationEdges) {
        if (link.Network == net) continue;
        RemoveEdge(link);
        net.AddEdge(link);
        TraverseSubnet(net, link.ToVertex);
      }
    }

    internal class NetworkCountComparer<G> : IComparer<G> where G : GenericNetwork<V, E, Y>, new()  {
      public int Compare(G a, G b) {
        if (a == null || b == null || a == b) return 0;
        if (a.VertexCount < b.VertexCount) return 1;
        if (a.VertexCount == b.VertexCount) return 0;
        return -1;
      }
    }


    /// <summary>
    /// Retrieve all of the <see cref="Part"/> <see cref="Node"/>s and <see cref="Link"/>s from the
    /// <see cref="Vertex"/>s and <see cref="Edge"/>s that are in this network.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{Part}"/> of all of the <see cref="Part"/>s that
    /// correspond to the <see cref="Vertexes"/> and <see cref="Edges"/>.
    /// </returns>
    public IEnumerable<Part> GetNodesAndLinks() {
      List<Part> coll = new List<Part>();
      foreach (V n in this.Vertexes) {
        if (n.Node != null && !coll.Contains(n.Node)) {
          coll.Add(n.Node);
        }
      }
      foreach (E l in this.Edges) {
        if (l.Link != null && !coll.Contains(l.Link)) {
          coll.Add(l.Link);
        }
      }
      return coll;
    }


    /// <summary>
    /// Gets the number of vertexes in this network.
    /// </summary>
    public int VertexCount {
      get { return myVertexes.Count; }
    }

    /// <summary>
    /// Gets the number of edges in this network.
    /// </summary>
    public int EdgeCount {
      get { return myEdges.Count; }
    }

    internal VertexList VertexesArray {
      get { return myVertexes; }
    }

    internal EdgeList EdgesArray {
      get { return myEdges; }
    }

    /// <summary>
    /// Gets the dictionary that maps <see cref="Node"/>s to <see cref="Vertex"/>es.
    /// </summary>
    protected Dictionary<Node, V> NodeToVertexDictionary {
      get { return myNodeToVertexDictionary; }
    }

    /// <summary>
    /// Gets the dictionary that maps <see cref="Link"/>s to <see cref="Edge"/>s.
    /// </summary>
    protected Dictionary<Link, E> LinkToEdgeDictionary {
      get { return myLinkToEdgeDictionary; }
    }
    
    /// <summary>
    /// Gets an enumerator for the network's vertexes.
    /// </summary>
    public Enumerator<V> Vertexes {
      get { return new Enumerator<V>(myVertexes); }
    }

    /// <summary>
    /// Gets an enumerator for the network's edges.
    /// </summary>
    public Enumerator<E> Edges {
      get { return new Enumerator<E>(myEdges); }
    }

    /// <summary>
    /// Gets or sets the <see cref="DiagramLayout"/> in which this
    /// network is being used.
    /// </summary>
    public Y Layout { get; set; }

    //private variables

    //the list of nodes in the network
    private VertexList myVertexes = new VertexList();  //??? replace with HashSet

    //the list of links in the network
    private EdgeList myEdges = new EdgeList();  //??? replace with HashSet

    //a mapping of top-level Nodes to their corresponding vertexes
    private Dictionary<Node, V> myNodeToVertexDictionary = new Dictionary<Node, V>();

    //a mapping of top-level Links to their corresponding edges
    private Dictionary<Link, E> myLinkToEdgeDictionary = new Dictionary<Link, E>();


    /********************************************************************************************/

    internal class EdgeList : List<E> { }
    internal class VertexList : List<V> { }

    /********************************************************************************************/

    /// <summary>
    /// Enumerator implements both <c>IEnumerator</c> and <c>IEnumerable</c>.
    /// </summary>
    public struct Enumerator<T> : IEnumerator<T>, IEnumerable<T>
      where T : class {
      // nested struct
      internal Enumerator(List<T> a) {
        myArray = a;
        myIndex = -1;
        Reset();
      }

      /// <summary>
      /// No resources to cleanup.
      /// </summary>
      public void Dispose() { }


      // IEnumerable: GetEnumerator

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { Enumerator<T> e = this; e.Reset(); return e; }
      
      IEnumerator<T> IEnumerable<T>.GetEnumerator() { Enumerator<T> e = this; e.Reset(); return e; }

      /// <summary>
      /// Gets an enumerator for iterating over the Links
      /// </summary>
      public Enumerator<T> GetEnumerator() { Enumerator<T> e = this; e.Reset(); return e; }
      

      // IEnumerator: Current; MoveNext, Reset

      Object System.Collections.IEnumerator.Current {
        get { return this.Current; }
      }

      /// <summary>
      ///  Gets the current node in the collection of links.
      /// </summary>
      public T Current {
        get {
          if (myIndex >= 0 && myIndex < myArray.Count)
            return myArray[myIndex];
          else
            throw new InvalidOperationException("U.Enumerator is not at a valid position for the List");
        }
      }

      /// <summary>
      /// Advance the enumerator to the next Link.
      /// </summary>
      /// <returns>True if there is a next Link; false if it has finished iterating over the collection.</returns>
      public bool MoveNext() {
        if (myIndex + 1 < myArray.Count) {
          myIndex++;
          return true;
        } else {
          return false;
        }
      }

      /// <summary>
      /// Reset the enumerator to its original position.
      /// </summary>
      public void Reset() {
        myIndex = -1;
      }

      // Enumerator state
      private List<T> myArray;
      private int myIndex;
    }
    
    
    /********************************************************************************************/

    /// <summary>
    /// Holds auto-layout specific link data.
    /// </summary>
    public class Edge {
      /// <summary>
      /// Gets or sets this edge's associated <see cref="Link"/>.
      /// </summary>
      public Link Link {
        get { return myLink; }
        set { myLink = value; }
      }

      /// <summary>
      /// Gets or sets the network that this edge is part of.
      /// </summary>
      public GenericNetwork<V, E, Y> Network {
        get { return myNetwork; }
        set { myNetwork = value; }
      }

      /// <summary>
      /// Gets or sets this edge's "from" <see cref="Vertex"/>.
      /// </summary>
      /// <remarks>
      /// Setting this property does not modify the vertex
      /// by adding an edge to its list of destination edges
      /// or removing it from another vertex's list.
      /// </remarks>
      public V FromVertex {
        get { return myFromVertex; }
        set { myFromVertex = value; }
      }


      /// <summary>
      /// Gets or sets this edge's "to" <see cref="Vertex"/>.
      /// </summary>
      /// <remarks>
      /// Setting this property does not modify the vertex
      /// by adding an edge to its list of source edges
      /// or removing it from another vertex's list.
      /// </remarks>
      public V ToVertex {
        get { return myToVertex; }
        set { myToVertex = value; }
      }


      /// <summary>
      /// Gets the <see cref="Route"/> associated with this edge.
      /// This may be needed by the algorithm to add bends to the link.
      /// </summary>
      public virtual Route Route {
        get {
          if (this.Link != null) return this.Link.Route;
          return null;
        }
      }


      internal void ReverseEdge() {
        V temp = myFromVertex;
        myFromVertex = myToVertex;
        myToVertex = temp;
      }


      /// <summary>
      /// Commits the position of the edge to the corresponding Link.
      /// </summary>
      /// <remarks>
      /// By default this does nothing.
      /// </remarks>
      public virtual void CommitPosition() { }


      /// <summary>
      /// Finds and returns the edge's vertex other than <paramref name="v"/>.
      /// Returns null if <paramref name="v"/> is neither the "from" vertex or "to" vertex.
      /// </summary>
      /// <param name="v"></param>
      public V GetOtherVertex(V v) {
        if (this.ToVertex == v)
          return this.FromVertex;
        else if (this.FromVertex == v)
          return this.ToVertex;
        else
          return null;
      }


      // Link state

      //the network to which the link will belong
      private GenericNetwork<V, E, Y> myNetwork;

      //the Link to which the edge will correspond (usually a Link)
      private Link myLink;

      //the Vertex from which the link will be directed
      private V myFromVertex;

      //the Vertex to which the link will be directed
      private V myToVertex;
    }

    /// <summary>
    /// Holds auto-layout specific node data.
    /// </summary>
    public class Vertex {
      /// <summary>
      /// Gets or sets the <see cref="Node"/> associated with this network vertex.
      /// </summary>
      /// <remarks>
      /// Setting this value to a node will also set the <see cref="Center"/>
      /// property to that object's <see cref="Node"/>.<c>LocationElement</c> center position.
      /// </remarks>
      public Node Node {
        get { return myNode; }
        set {
          myNode = value;
          if (myNode != null) {
            myNode.MaybeRemeasureNow();
            myBounds = myNode.Bounds;
            myFocus = myNode.GetRelativeElementPoint(myNode.LocationElement, Spot.Center);
          }
        }
      }

      /// <summary>
      /// Gets or sets the network that this vertex is part of.
      /// </summary>
      public GenericNetwork<V, E, Y> Network {
        get { return myNetwork; }
        set { myNetwork = value; }
      }

      internal GenericNetwork<V, E, Y>.EdgeList SourceEdgesList {
        get { return mySourceEdges; }
      }
      
      /// <summary>
      /// Adds an edge to the list of predecessors
      /// (i.e., the edge will be coming into this vertex).
      /// </summary>
      /// <param name="sourceEdge"></param>
      public void AddSourceEdge(E sourceEdge) {
        if (!mySourceEdges.Contains(sourceEdge))
          mySourceEdges.Add(sourceEdge);
      }

      /// <summary>
      /// Deletes an edge from the list of predecessors
      /// (i.e., the edge was coming into this vertex).
      /// </summary>
      /// <param name="sourceEdge"></param>
      public void DeleteSourceEdge(E sourceEdge) {
        int pos = mySourceEdges.IndexOf(sourceEdge);
        if (pos != -1) {
          mySourceEdges.RemoveAt(pos);
        }
      }


      internal GenericNetwork<V, E, Y>.EdgeList DestinationEdgesList {
        get { return myDestinationEdges; }
      }

      /// <summary>
      /// Adds an edge to the list successors
      /// (i.e., the edge will be going out from this vertex).
      /// </summary>
      /// <param name="destinationEdge"></param>
      public void AddDestinationEdge(E destinationEdge) {
        if (!myDestinationEdges.Contains(destinationEdge))
          myDestinationEdges.Add(destinationEdge);
      }

      /// <summary>
      /// Deletes an edge from the list of successors
      /// (i.e., the edge was going out from this vertex).
      /// </summary>
      /// <param name="destinationEdge"></param>
      public void DeleteDestinationEdge(E destinationEdge) {
        int pos = myDestinationEdges.IndexOf(destinationEdge);
        if (pos != -1) {
          myDestinationEdges.RemoveAt(pos);
        }
      }


      /// <summary>
      /// Gets or sets the center Point of this vertex.
      /// </summary>
      /// <remarks>
      /// Setting this property does not modify the position of any <see cref="Northwoods.GoXam.Node"/>.
      /// </remarks>
      /// <seealso cref="Position"/>
      /// <seealso cref="Bounds"/>
      /// <seealso cref="Focus"/>
      public Point Center {
        get { return new Point(this.Bounds.X + this.Focus.X, this.Bounds.Y + this.Focus.Y); }
        set {
          myBounds.X = value.X - this.Focus.X;
          myBounds.Y = value.Y - this.Focus.Y;
        }
      }

      /// <summary>
      /// Gets or sets the position (top-left corner) of this vertex.
      /// </summary>
      /// <remarks>
      /// Setting this property does not modify the position of any <see cref="Northwoods.GoXam.Node"/>.
      /// </remarks>
      /// <seealso cref="Center"/>
      /// <seealso cref="Bounds"/>
      /// <seealso cref="Focus"/>
      public Point Position {
        get { return new Point(this.Bounds.X, this.Bounds.Y); }
        set {
          myBounds.X = value.X;
          myBounds.Y = value.Y;
        }
      }

      /// <summary>
      /// Gets or sets the offset of the <see cref="Center"/> from the
      /// <see cref="Bounds"/><c>.Position</c>.
      /// </summary>
      /// <remarks>
      /// Setting this property does not modify the position of any <see cref="Northwoods.GoXam.Node"/>.
      /// </remarks>
      /// <seealso cref="Center"/>
      /// <seealso cref="Position"/>
      /// <seealso cref="Bounds"/>
      public Point Focus {  // Center - Position
        get { return myFocus; }
        set { myFocus = value; }
      }

      /// <summary>
      /// Gets or sets the Bounds of this node.
      /// </summary>
      /// <remarks>
      /// Setting this property does not modify the position of any <see cref="Northwoods.GoXam.Node"/>.
      /// </remarks>
      /// <seealso cref="Center"/>
      /// <seealso cref="Position"/>
      /// <seealso cref="Focus"/>
      public Rect Bounds {
        get { return myBounds; }
        set { myBounds = value; }
      }

      /// <summary>
      /// Gets the <see cref="Bounds"/><c>.Size</c>.
      /// </summary>
      public Size Size {
        get { return new Size(myBounds.Width, myBounds.Height); }
      }

      /// <summary>
      /// Gets the <see cref="Bounds"/><c>.Width</c>.
      /// </summary>
      /// <remarks>
      /// This is always the horizontal distance reserved for this node.
      /// </remarks>
      public double Width {
        get { return this.Bounds.Width; }
      }

      /// <summary>
      /// Gets the <see cref="Bounds"/><c>.Height</c>.
      /// </summary>
      /// <remarks>
      /// This is always the vertical distance reserved for this node.
      /// </remarks>
      public double Height {
        get { return this.Bounds.Height; }
      }

      /// <summary>
      /// Moves the <see cref="Northwoods.GoXam.Node"/> corresponding to this vertex
      /// so that its position is the current <see cref="Position"/>.
      /// </summary>
      /// <remarks>
      /// To make the most common cases look right, the object's <see cref="Node"/>.<c>LocationElement</c>
      /// is centered.  Thus iconic nodes will have the center of the icon be positioned
      /// according to the <see cref="Center"/> of this Node, ignoring any labels.
      /// </remarks>
      public virtual void CommitPosition() {
        Node node = this.Node;
        if (node != null) node.Move(this.Position, true);
      }


      /// <summary>
      /// Gets an enumerator over all of the vertexes that have edges coming into this vertex.
      /// </summary>
      /// <remarks>
      /// The enumerator iterates over the set of all vertexes that have any
      /// destination edges coming into this vertex.
      /// </remarks>
      public GenericNetwork<V, E, Y>.Enumerator<V> SourceVertexes {
        get {
          GenericNetwork<V, E, Y>.VertexList a = new GenericNetwork<V, E, Y>.VertexList();
          foreach (E l in mySourceEdges) {
            V n = l.FromVertex;
            if (n != null && !a.Contains(n))
              a.Add(n);
          }
          return new GenericNetwork<V, E, Y>.Enumerator<V>(a);
        }
      }

      /// <summary>
      /// Gets an enumerator over all of the vertexes that have edges going out of this vertex.
      /// </summary>
      /// <remarks>
      /// The enumerator iterates over the set of all vertexes that have any
      /// source edges going out of this vertex.
      /// </remarks>
      public GenericNetwork<V, E, Y>.Enumerator<V> DestinationVertexes {
        get {
          GenericNetwork<V, E, Y>.VertexList a = new GenericNetwork<V, E, Y>.VertexList();
          foreach (E l in myDestinationEdges) {
            V n = l.ToVertex;
            if (n != null && !a.Contains(n))
              a.Add(n);
          }
          return new GenericNetwork<V, E, Y>.Enumerator<V>(a);
        }
      }

      /// <summary>
      /// Gets an enumerator over all of the vertexes that are connected to this vertex.
      /// </summary>
      /// <remarks>
      /// The enumerator iterates over the set of all vertexes that have any
      /// edges connected to this vertex.
      /// </remarks>
      public GenericNetwork<V, E, Y>.Enumerator<V> Vertexes {
        get {
          GenericNetwork<V, E, Y>.VertexList a = new GenericNetwork<V, E, Y>.VertexList();
          foreach (E l in mySourceEdges) {
            V n = l.FromVertex;
            if (n != null && !a.Contains(n))
              a.Add(n);
          }
          foreach (E l in myDestinationEdges) {
            V n = l.ToVertex;
            if (n != null && !a.Contains(n))
              a.Add(n);
          }
          return new GenericNetwork<V, E, Y>.Enumerator<V>(a);
        }
      }

      /// <summary>
      /// Gets an enumerator that iterates over all of the edges coming into this vertex.
      /// </summary>
      public GenericNetwork<V, E, Y>.Enumerator<E> SourceEdges {
        get { return new GenericNetwork<V, E, Y>.Enumerator<E>(mySourceEdges); }
      }

      /// <summary>
      /// Gets the number of edges going into this vertex.
      /// </summary>
      public int SourceEdgesCount {
        get { return mySourceEdges.Count; }
      }

      /// <summary>
      /// Gets an enumerator that iterates over all of the edges going out of this vertex.
      /// </summary>
      public GenericNetwork<V, E, Y>.Enumerator<E> DestinationEdges {
        get { return new GenericNetwork<V, E, Y>.Enumerator<E>(myDestinationEdges); }
      }

      /// <summary>
      /// Gets the number of edges coming out of this vertex.
      /// </summary>
      public int DestinationEdgesCount {
        get { return myDestinationEdges.Count; }
      }

      /// <summary>
      /// Gets an enumerator over all of the edges going out of or coming into this vertex.
      /// </summary>
      public GenericNetwork<V, E, Y>.Enumerator<E> Edges {
        get {
          GenericNetwork<V, E, Y>.EdgeList a = new GenericNetwork<V, E, Y>.EdgeList();
          foreach (E l in mySourceEdges) {
            a.Add(l);
          }
          foreach (E l in myDestinationEdges) {
            if (!a.Contains(l))
              a.Add(l);
          }
          return new GenericNetwork<V, E, Y>.Enumerator<E>(a);
        }
      }

      /// <summary>
      /// Gets the number of edges connected to this vertex.
      /// </summary>
      public int EdgesCount {
        get { return myDestinationEdges.Count + mySourceEdges.Count; }
      }


      // Vertex state

      //the network to which the node belongs
      private GenericNetwork<V, E, Y> myNetwork;

      //the Node to which the node corresponds (usually a Node, or null if artificial)
      private Node myNode;

      private Point myFocus = new Point(5, 5);
      private Rect myBounds = new Rect(0, 0, 10, 10);

      //the list of predecessor links (i.e., the links are to this node)
      private GenericNetwork<V, E, Y>.EdgeList mySourceEdges = new GenericNetwork<V, E, Y>.EdgeList();

      //the list of successor links (i.e., the links are from this node).
      private GenericNetwork<V, E, Y>.EdgeList myDestinationEdges = new GenericNetwork<V, E, Y>.EdgeList();
    }
  }
}
