
/*
 *  Copyright (c) Northwoods Software Corporation, 1998-2011. All Rights Reserved.
 *
 *  Restricted Rights: Use, duplication, or disclosure by the L.S.
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
  /// ForceDirected provides an auto-layout algorithm for
  /// graphs which utilizes a force-directed method.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The graph is viewed as
  /// a system of bodies with forces acting between the bodies.  The algorithm
  /// seeks a configuration of the bodies with locally minimal energy, i.e.,
  /// a position such that the sum of the forces on each body is zero.
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
  public class ForceDirectedLayout : DiagramLayout {
    /// <summary>
    /// Constucts a ForceDirected layout class with null network, document.
    /// PerformLayout() will be a no-op until the document has been set.
    /// </summary>
    public ForceDirectedLayout() {
    }

    /// <summary>
    /// Make a copy of a <see cref="ForceDirectedLayout"/>, copying most of the
    /// important properties except for the <see cref="Network"/>.
    /// </summary>
    /// <param name="layout"></param>
    public ForceDirectedLayout(ForceDirectedLayout layout) : base(layout) {
      if (layout != null) {
        this.Network = null;
        this.ArrangementSpacing = layout.ArrangementSpacing;
        this.ArrangesToOrigin = layout.ArrangesToOrigin;
        this.SetsPortSpots = layout.SetsPortSpots;
        this.Comments = layout.Comments;
        this.MaxIterations = layout.MaxIterations;
        this.Epsilon = layout.Epsilon;
        this.InfinityDistance = layout.InfinityDistance;
        this.RandomNumberGenerator = layout.RandomNumberGenerator;
        this.DefaultSpringStiffness = layout.DefaultSpringStiffness;
        this.DefaultSpringLength = layout.DefaultSpringLength;
        this.DefaultElectricalCharge = layout.DefaultElectricalCharge;
        this.DefaultGravitationalMass = layout.DefaultGravitationalMass;
        this.DefaultCommentSpringLength = layout.DefaultCommentSpringLength;
        this.DefaultCommentElectricalCharge = layout.DefaultCommentElectricalCharge;
      }
    }

    static ForceDirectedLayout() {
      NetworkProperty = DependencyProperty.Register("Network", typeof(ForceDirectedNetwork), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(null, OnNetworkChanged));
      ArrangementSpacingProperty = DependencyProperty.Register("ArrangementSpacing", typeof(Size), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(new Size(100, 100), OnPropertyChanged));
      ArrangesToOriginProperty = DependencyProperty.Register("ArrangesToOrigin", typeof(bool), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(false, OnPropertyChanged));
      SetsPortSpotsProperty = DependencyProperty.Register("SetsPortSpots", typeof(bool), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(true, OnPropertyChanged));
      CommentsProperty = DependencyProperty.Register("Comments", typeof(bool), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(true, OnPropertyChanged));
      MaxIterationsProperty = DependencyProperty.Register("MaxIterations", typeof(int), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(100, OnMaxIterationsChanged));
      EpsilonProperty = DependencyProperty.Register("Epsilon", typeof(double), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(1.0, OnEpsilonChanged));
      InfinityDistanceProperty = DependencyProperty.Register("InfinityDistance", typeof(double), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(1000.0, OnInfinityDistanceChanged));
      RandomNumberGeneratorProperty = DependencyProperty.Register("RandomNumberGenerator", typeof(Random), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(null, OnPropertyChanged));
      DefaultSpringStiffnessProperty = DependencyProperty.Register("DefaultSpringStiffness", typeof(double), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(0.05, OnPropertyChanged));
      DefaultSpringLengthProperty = DependencyProperty.Register("DefaultSpringLength", typeof(double), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(50.0, OnPropertyChanged));
      DefaultElectricalChargeProperty = DependencyProperty.Register("DefaultElectricalCharge", typeof(double), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(150.0, OnPropertyChanged));
      DefaultGravitationalMassProperty = DependencyProperty.Register("DefaultGravitationalMass", typeof(double), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(0.0, OnPropertyChanged));
      DefaultCommentSpringLengthProperty = DependencyProperty.Register("DefaultCommentSpringLength", typeof(double), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(20.0, OnPropertyChanged));
      DefaultCommentElectricalChargeProperty = DependencyProperty.Register("DefaultCommentElectricalCharge", typeof(double), typeof(ForceDirectedLayout),
        new FrameworkPropertyMetadata(25.0, OnPropertyChanged));
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      ForceDirectedLayout layout = (ForceDirectedLayout)d;
      layout.InvalidateLayout();
    }


    /// <summary>
    /// Identifies the <see cref="Network"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NetworkProperty;
    /// <summary>
    /// Gets or sets the <see cref="ForceDirectedNetwork"/> that the layout will be performed on.
    /// </summary>
    /// <value>
    /// The initial value is null.
    /// </value>
    public ForceDirectedNetwork Network {
      get { return (ForceDirectedNetwork)GetValue(NetworkProperty); }
      set { SetValue(NetworkProperty, value); }
    }
    private static void OnNetworkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      ForceDirectedLayout layout = (ForceDirectedLayout)d;
      ForceDirectedNetwork net = (ForceDirectedNetwork)e.NewValue;
      if (net != null) net.Layout = layout;
    }

    /// <summary>
    /// Allocate a <see cref="ForceDirectedNetwork"/>.
    /// </summary>
    /// <returns></returns>
    public virtual ForceDirectedNetwork CreateNetwork() {
      ForceDirectedNetwork n = new ForceDirectedNetwork();
      n.Layout = this;
      return n;
    }

    /// <summary>
    /// Create and initialize a <see cref="ForceDirectedNetwork"/> with the given nodes and links.
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="links"></param>
    /// <returns>a <see cref="ForceDirectedNetwork"/></returns>
    /// <remarks>
    /// The network does not include nodes whose category is "Comment" unless <see cref="Comments"/> is true.
    /// </remarks>
    public virtual ForceDirectedNetwork MakeNetwork(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      ForceDirectedNetwork net = CreateNetwork();
      net.AddNodesAndLinks(nodes.Where(n => n.Category != "Comment"), links);
      if (this.Comments) {
        foreach (ForceDirectedVertex v in net.Vertexes) {
          AddComments(v);
        }
      }
      return net;
    }
  
    /// <summary>
    /// Performs force-directed auto-layout.
    /// </summary>
    /// <remarks>
    /// This method can be overridden to customize the layout algorithm,
    /// but care should be taken to ensure that each node and link in the
    /// input network are properly initialized and terminated.
    /// If <see cref="Network"/> is null, this calls <see cref="MakeNetwork"/> to allocate
    /// and initialize a <see cref="ForceDirectedNetwork"/> with the graph that is in the <see cref="Diagram"/>.
    /// After all of the computations are completed, this calls
    /// <see cref="LayoutNodesAndLinks"/> in order to commit the positions
    /// of all of the nodes.
    /// No undo/redo transaction is started or finished by this method.
    /// </remarks>
    public override void DoLayout(IEnumerable<Node> nodes, IEnumerable<Link> links) {
      if (this.Network == null) {
        this.Network = MakeNetwork(nodes, links);
      }

      // Progress update.
      RaiseProgress(0);

      int maxiter = this.MaxIterations;
      if (this.Network.VertexCount > 0) {
        // Remove self-links from the input network.
        this.Network.DeleteSelfEdges();

        // Augment each node in the input network with 
        //  ForceDirected auxilary data.
        foreach (ForceDirectedVertex vertex in this.Network.Vertexes) {
          // Record the charge of the node.
          vertex.Charge = ElectricalCharge(vertex);
          // Record the mass of the node.
          vertex.Mass = GravitationalMass(vertex);
        }

        // Augment each link in the input network with 
        //  ForceDirected auxilary data.
        foreach (ForceDirectedEdge edge in this.Network.Edges) {
          // Record the stiffness of the link.
          edge.Stiffness = SpringStiffness(edge);
          // Record the length of the link.
          edge.Length = SpringLength(edge);
        }

        myIteration = 0;
        if (NeedsClusterLayout()) {
          // split into subnetworks, plus the original network holding any singleton nodes
          ForceDirectedNetwork net = this.Network;
          IEnumerator<ForceDirectedNetwork> subnets = net.SplitIntoSubNetworks<ForceDirectedNetwork>();
          // layout each subnetwork independently
          while (subnets.MoveNext()) {
            ForceDirectedNetwork subnet = subnets.Current;
            this.Network = subnet;
            InitializeClustering();
            LayoutClusters(0, maxiter);
          }
          this.Network = net;
          // layout each subnetwork so that they don't overlap and are separated by InfinityDistance
          subnets.Reset();
          ArrangeConnectedGraphs(subnets, net);
          // merge back into original network
          subnets.Reset();
          while (subnets.MoveNext()) {
            ForceDirectedNetwork subnet = subnets.Current;
            foreach (ForceDirectedVertex vertex in subnet.Vertexes) {
              net.AddVertex(vertex);
            }
            foreach (ForceDirectedEdge edge in subnet.Edges) {
              net.AddEdge(edge);
            }
          }
        }
        // do normal force-directed calculations
        PerformIterations(maxiter);

        // Update the "physical" positions of the nodes and links.
        if (this.Diagram != null && !this.Diagram.CheckAccess()) {
          Diagram.InvokeLater(this.Diagram, UpdateParts);
        } else {
          UpdateParts();
        }
      }
      // Progress update.
      RaiseProgress(1);
      
      // restore in case of future re-use
      this.MaxIterations = maxiter;
      this.Network = null;
    }

    private void UpdateParts() {
      VerifyAccess();
      Diagram diagram = this.Diagram;
      if (diagram != null) diagram.StartTransaction("ForceDirectedLayout");
      LayoutNodesAndLinks();
      if (diagram != null) diagram.CommitTransaction("ForceDirectedLayout");
    }


    /// <summary>
    /// Determines whether a clustering layout should be done before the regular force-directed layout.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// A clustering layout is useful when the graph is somewhat tree-like.
    /// </para>
    /// <para>
    /// For compatibility with older behavior,
    /// this does not examine the graph structure, but only at the positions of the nodes.
    /// By default this looks at the first few nodes in the <see cref="Network"/>.
    /// If several of their <see cref="GenericNetwork{N, L, Y}.Vertex.Bounds"/> intersect,
    /// this will return true; otherwise this will return false.
    /// Hence for a network whose nodes are already "spread out",
    /// no cluster layout will occur.
    /// </para>
    /// </remarks>
    protected virtual bool NeedsClusterLayout() {
      ForceDirectedNetwork.VertexList vertexarray = this.Network.VertexesArray;
      if (vertexarray.Count < 3) return false;
      Rect r = vertexarray[0].Bounds;
      int count = 0;
      int i = 0;
      foreach (ForceDirectedVertex vertex in vertexarray) {
        Rect b = vertex.Bounds;
        if (IntersectsRect(b, r)) {
          count++;
          if (count > 2) return true;  //??
        }
        if (i > 10) return false;  //??
        i++;
      }
      return false;
    }

    /// <summary>
    /// Determine how many additional force-directed layout iterations should occur for this clustered network.
    /// </summary>
    /// <param name="level">the recursion depth for clustered layout</param>
    /// <param name="maxiter"></param>
    /// <returns>a non-negative number that will be temporarily added to <see cref="MaxIterations"/></returns>
    protected virtual int ComputeClusterLayoutIterations(int level, int maxiter) {
      ForceDirectedNetwork.VertexList vertexarray = this.Network.VertexesArray;
      int subiter = Math.Max((int)Math.Min(vertexarray.Count, maxiter*(level+1)/11.0), 10);  //??
      return subiter;
    }

    internal static bool IntersectsRect(Rect a, Rect b) {
      double tw = a.Width;
      if (tw < 0) return false;
      double th = a.Height;
      if (th < 0) return false;
      double rw = b.Width;
      if (rw < 0) return false;
      double rh = b.Height;
      if (rh < 0) return false;

      double tx = a.X;
      double rx = b.X;
      tw += tx;
      rw += rx;
      if (tx > rw || rx > tw) return false;

      double ty = a.Y;
      double ry = b.Y;
      th += ty;
      rh += ry;
      if (ty > rh || ry > th) return false;
      return true;
    }

    internal static Rect UnionRect(Rect a, Rect b) {
      double minx = Math.Min(a.X, b.X);
      double miny = Math.Min(a.Y, b.Y);
      double maxr = Math.Max(a.X+a.Width, b.X+b.Width);
      double maxb = Math.Max(a.Y+a.Height, b.Y+b.Height);
      return new Rect(minx, miny, maxr-minx, maxb-miny);
    }

    internal Rect ComputeBounds(ForceDirectedNetwork net) {
      Rect bounds = new Rect();
      bool first = true;
      foreach (ForceDirectedVertex vertex in net.Vertexes) {
        if (first) {
          first = false;
          bounds = vertex.Bounds;
        } else {
          bounds = UnionRect(bounds, vertex.Bounds);
        }
      }
      return bounds;
    }


    private void InitializeClustering() {
      // assume self-links have already been removed
      ForceDirectedNetwork.VertexList vertexarray = this.Network.VertexesArray;
      foreach (ForceDirectedVertex vertex in vertexarray) {
        int num = 0;
        foreach (ForceDirectedVertex v in vertex.Vertexes) num++;
        vertex.NumConnections = num;  // cache number of nodes connected to this node
        vertex.NumInCluster = 1;
        vertex.Clustereds = null;  // clear any internal state
        vertex.SavedState = null;
      }
    }

    private void LayoutClusters(int level, int maxiter) {
      if (HasClusters(level)) {  // also sorts by # connections
        double oldInf = this.InfinityDistance;
        this.InfinityDistance *= (1.0 + 1.0/(level+1));  //??

        ForceDirectedNetwork oldnet = PushSubNetwork(level);
        int subiter = Math.Max(0, ComputeClusterLayoutIterations(level, maxiter));
        this.MaxIterations += subiter;
        LayoutClusters(level+1, maxiter);
        PerformIterations(subiter);
        PopNetwork(oldnet, level);
        foreach (ForceDirectedVertex vertex in oldnet.Vertexes) {
          SurroundNode(vertex, level);
        }

        this.InfinityDistance = oldInf;
      }
    }

    private bool HasClusters(int level) {
      if (level > 10) return false;
      ForceDirectedNetwork.VertexList vertexarray = this.Network.VertexesArray;
      if (vertexarray.Count < 3) return false;
      vertexarray.Sort(ConnComparer.Default);
      int i = vertexarray.Count-1;
      while (i >= 0 && vertexarray[i].NumConnections <= 1) i--;
      int leaves = vertexarray.Count-i;
      return leaves > 1;
    }

    sealed internal class StateInfo {
      public int NumConnections;
      public Size Size;
      public Point Focus;
    }

    private ForceDirectedNetwork PushSubNetwork(int level) {
      ForceDirectedNetwork oldnet = this.Network;
      ForceDirectedNetwork newnet = new ForceDirectedNetwork();
      // reparent multiply-connected nodes and their links
      foreach (ForceDirectedVertex vertex in oldnet.VertexesArray) {
        if (vertex.NumConnections > 1) {
          newnet.AddVertex(vertex);  // move to new network
          StateInfo info = new StateInfo();  // save state
          info.NumConnections = vertex.NumConnections;
          info.Size = vertex.Size;
          info.Focus = vertex.Focus;
          if (vertex.SavedState == null) vertex.SavedState = new List<StateInfo>();
          vertex.SavedState.Add(info);
          vertex.SavedStateIndex = vertex.SavedState.Count-1;
        } else {
          break;  // assume sorted by decreasing # connections
        }
      }
      foreach (ForceDirectedEdge edge in oldnet.EdgesArray) {
        if (edge.FromVertex.Network == newnet && edge.ToVertex.Network == newnet) {
          newnet.AddEdge(edge);  // move to new network
        } else if (edge.FromVertex.Network == newnet) {
          ForceDirectedNetwork.VertexList clus = edge.FromVertex.Clustereds;
          if (clus == null) {
            clus = new ForceDirectedNetwork.VertexList();
            edge.FromVertex.Clustereds = clus;
          }
          clus.Add(edge.ToVertex);  // add to other node's cluster
          edge.FromVertex.NumConnections--;
          edge.FromVertex.NumInCluster += edge.ToVertex.NumInCluster;
        } else if (edge.ToVertex.Network == newnet) {
          ForceDirectedNetwork.VertexList clus = edge.ToVertex.Clustereds;
          if (clus == null) {
            clus = new ForceDirectedNetwork.VertexList();
            edge.ToVertex.Clustereds = clus;
          }
          clus.Add(edge.FromVertex);  // add to other node's cluster
          edge.ToVertex.NumConnections--;
          edge.ToVertex.NumInCluster += edge.FromVertex.NumInCluster;
        }
      }
      // relax the links connecting clustered nodes
      foreach (ForceDirectedEdge edge in newnet.EdgesArray) {
        int num = edge.FromVertex.NumInCluster + edge.ToVertex.NumInCluster;
        edge.Length *= Math.Max(1, (double)Math.Sqrt(num/(4.0*level+1)));  //??
      }
      // pretend clustered nodes are larger
      foreach (ForceDirectedVertex vertex in newnet.VertexesArray) {
        // update state
        ForceDirectedNetwork.VertexList clus = vertex.Clustereds;
        if (clus != null && clus.Count > 0) {
          StateInfo info = vertex.SavedState[vertex.SavedState.Count-1];
          int cnt = info.NumConnections-vertex.NumConnections;
          if (cnt <= 0) continue;
          double newarea = 0;
          double linklen = 0;
          for (int i = clus.Count-cnt; i < clus.Count; i++) {
            ForceDirectedVertex v = clus[i];
            // just check nodes connected directly to this node
            ForceDirectedEdge edge = null;
            foreach (ForceDirectedEdge e in v.Edges) {
              if (e.GetOtherVertex(v) == vertex) {
                edge = e;
                break;
              }
            }
            if (edge != null) {
              //if (cnt > 4) link.Length *= (double)Math.Sqrt(cnt/4.0);  //??
              linklen += edge.Length;
              newarea += v.Width*v.Height;
            }
          }
          // update Bounds and Focus (maintaining Center)
          Point pt = vertex.Center;
          Size sz = vertex.Size;
          Point foc = vertex.Focus;
          double area = vertex.Width*vertex.Height;
          if (area < 1) area = 1;
          double comp = (double)Math.Sqrt((newarea+area+((linklen*linklen*4)/(cnt*cnt)))/area);  //??
          double fw = (comp-1)*vertex.Width/2;
          double fh = (comp-1)*vertex.Height/2;
          vertex.Bounds = new Rect(pt.X-foc.X-fw, pt.Y-foc.Y-fh, sz.Width+fw*2, sz.Height+fh*2);
          vertex.Focus = new Point(foc.X+fw, foc.Y+fh);
        }
      }
      this.Network = newnet;
      return oldnet;
    }

    private void PopNetwork(ForceDirectedNetwork oldnet, int level) {
      ForceDirectedNetwork.VertexList vertexarray = this.Network.VertexesArray;
      foreach (ForceDirectedVertex vertex in vertexarray) {
        vertex.Network = oldnet;  // reparent node to old network; NOT: oldnet.AddNode(node);
        // pop NumConnections & Bounds/Focus
        if (vertex.SavedState != null) {
          int idx = vertex.SavedStateIndex;
          StateInfo info = vertex.SavedState[idx];
          vertex.NumConnections = info.NumConnections;
          Size sz = info.Size;
          Point foc = info.Focus;
          Point pt = vertex.Center;
          // restore Size and Focus, adapting to new Center (if it had been moved)
          vertex.Bounds = new Rect(pt.X-foc.X, pt.Y-foc.Y, sz.Width, sz.Height);
          vertex.Focus = foc;
          vertex.SavedStateIndex--;  // pop stack without losing StateInfos
        }
      }
      foreach (ForceDirectedEdge edge in this.Network.EdgesArray) {
        edge.Network = oldnet;  // reparent link to old network; NOT: oldnet.AddLink(link);
        //?? restore link.Length
      }
      this.Network = oldnet;
    }

    private void SurroundNode(ForceDirectedVertex vertex, int level) {
      ForceDirectedNetwork.VertexList clus = vertex.Clustereds;
      if (clus == null || clus.Count == 0) return;  // ignore non-clusters
      Point center = vertex.Center;
      Size size = vertex.Size;
      if (vertex.SavedState != null && vertex.SavedState.Count > 0) {
        StateInfo info = vertex.SavedState[0];
        size = info.Size;
      }
      double radius = (double)Math.Sqrt(size.Width*size.Width + size.Height*size.Height)/2;
      // where's it coming from? look at connected non-leaf nodes
      bool external = false;
      double angle = 0;
      int count = 0;
      int externalcount = 0;
      foreach (ForceDirectedVertex v in vertex.Vertexes) {
        if (v.NumConnections <= 1) {
          count++;
        } else {
          external = true;
          externalcount++;
          Point ca = v.Center;
          Point cb = vertex.Center;
          angle += Math.Atan2(cb.Y-ca.Y, cb.X-ca.X);
        }
      }
      if (count == 0) return;
      // average the angles to multi-connected node
      if (externalcount > 0) {
        angle /= externalcount;
      }
      // over what angle should they be laid out around the node
      double ang = 0;
      double offsetang = 0;
      if (external) {
        ang = Math.PI*2/(count+1);
      } else {
        ang = Math.PI*2/count;
      }
      if (count%2 == 0) offsetang = ang/2;
      // now position the leaf nodes around the node
      // reorder according to node size
      if (clus.Count > 1) clus.Sort(SizeComparer.Default);
      int i = (count%2 == 0 ? 0 : 1);
      foreach (ForceDirectedVertex v in clus) {
        if (v.NumConnections > 1) continue;  // shouldn't happen?
        if (IsFixed(v)) continue;
        // position satellite node according to distance given by link.Length plus radii
        ForceDirectedEdge edge = null;
        foreach (ForceDirectedEdge e in v.Edges) {
          edge = e;
          break;
        }
        Size sz = v.Size;
        double r = (double)Math.Sqrt(sz.Width*sz.Width + sz.Height*sz.Height)/2;
        double d = radius + edge.Length + r;
        double a = angle + (ang*(i/2)+offsetang)*(i%2==0 ? 1 : -1);
        v.Center = new Point(center.X + d * (double)Math.Cos(a), center.Y + d * (double)Math.Sin(a));
        i++;
      }
    }

    // the basic loop
    private void PerformIterations(int num) {
      int end = myIteration + num;
      while (myIteration < end) {
        myIteration++;
        // If no change occured, solution found.
        if (!UpdatePositions()) break;
        // Progress update.
        RaiseProgress((double)this.CurrentIteration / (double)this.MaxIterations);
      }
    }

    /// <summary>
    /// Identifies the <see cref="ArrangementSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ArrangementSpacingProperty;
    /// <summary>
    /// Gets or sets the space between which <see cref="ArrangeConnectedGraphs"/> will position the connected graphs
    /// that together compose the network.
    /// </summary>
    /// <value>
    /// This defaults to the Size(100, 100).
    /// </value>
    /// <remarks>
    /// These distances are used during a clustered layout; afterwards the normal force-directed layout
    /// will likely cause the size of any space between connected graphs to change, perhaps considerably.
    /// </remarks>
    public Size ArrangementSpacing {
      get { return (Size)GetValue(ArrangementSpacingProperty); }
      set { SetValue(ArrangementSpacingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ArrangesToOrigin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ArrangesToOriginProperty;
    /// <summary>
    /// Gets or sets whether <see cref="LayoutNodes"/> should move all of the
    /// nodes so that the nodes all fit with the top-left corner at the
    /// <see cref="DiagramLayout.ArrangementOrigin"/>.
    /// </summary>
    /// <value>
    /// By default this is false -- the <see cref="DiagramLayout.ArrangementOrigin"/> is ignored.
    /// When this is true, nodes are moved even though <see cref="IsFixed"/> was true.
    /// </value>
    public bool ArrangesToOrigin {
      get { return (bool)GetValue(ArrangesToOriginProperty); }
      set { SetValue(ArrangesToOriginProperty, value); }
    }

    /// <summary>
    /// During a clustered layout, position each separate graph network so that they do not overlap each other.
    /// </summary>
    /// <param name="subnets">an <c>IEnumerator</c> iterating over a number of <see cref="ForceDirectedNetwork"/>,
    /// each containing a connected graph; this is the result of calling <see cref="GenericNetwork{N, L, Y}.SplitIntoSubNetworks"/></param>
    /// <param name="singletons">a <see cref="ForceDirectedNetwork"/> containing only unconnected individual nodes</param>
    /// <remarks>
    /// <para>
    /// There may be separate component graphs (without any links between them) within the original network.
    /// The <paramref name="subnets"/> enumerator will iterate over a number of <see cref="ForceDirectedNetwork"/>s,
    /// each containing a connected (sub)graph.
    /// If the original network is connected, i.e. if there is at least one path of
    /// undirected links from each node in the original <see cref="Network"/> to every other node,
    /// then there will be only a single <see cref="ForceDirectedNetwork"/>
    /// for the enumerator, and the <paramref name="singletons"/> network will be empty.
    /// </para>
    /// <para>
    /// This method places each separately-laid-out graph in different non-overlapping locations.
    /// By default this ignores <see cref="IsFixed"/>, because it may move "fixed" nodes to
    /// ensure disconnected subgraphs do not overlap.
    /// </para>
    /// </remarks>
    protected virtual void ArrangeConnectedGraphs(IEnumerator<ForceDirectedNetwork> subnets, ForceDirectedNetwork singletons) {
      Size spacing = this.ArrangementSpacing;
      // how many subnetworks are there?
      int subnetcount = 0;
      subnets.Reset();
      while (subnets.MoveNext()) {
        ForceDirectedNetwork subnet = subnets.Current;
        subnetcount++;
      }

      bool first = true;
      Point center = new Point();  // first subnetwork's Center
      Point[] fringe = new Point[subnetcount+singletons.VertexCount+2];
      int fringelen = 0;  // how many FRINGE points are valid
      subnets.Reset();
      while (subnets.MoveNext()) {
        ForceDirectedNetwork subnet = subnets.Current;
        Rect bounds = ComputeBounds(subnet);
        if (first) {
          first = false;
          center = new Point(bounds.X+bounds.Width/2, bounds.Y+bounds.Height/2);
          fringe[0] = new Point(bounds.Right + spacing.Width, bounds.Top);
          fringe[1] = new Point(bounds.Left, bounds.Bottom + spacing.Height);
          fringelen = 2;
        } else {
          // place subnetwork somewhere closest to first subnetwork's center
          int idx = ClosestFringePoint(fringe, fringelen, center, new Size(bounds.Width, bounds.Height), spacing);
          Point old = fringe[idx];
          Point tr = new Point(old.X + bounds.Width + spacing.Width, old.Y);
          Point bl = new Point(old.X, old.Y + bounds.Height + spacing.Height);
          // make room for additional fringe point
          if (idx+1 < fringelen) Array.Copy(fringe, idx+1, fringe, idx+2, fringelen-idx-1);
          fringe[idx] = tr;
          fringe[idx+1] = bl;
          fringelen++;
          // shift subnetwork to fit at OLD point
          double offx = old.X-bounds.X;
          double offy = old.Y-bounds.Y;
          foreach (ForceDirectedVertex vertex in subnet.Vertexes) {
            Point c = vertex.Center;
            vertex.Center = new Point(c.X+offx, c.Y+offy);
          }
        }
      }
      // place singletons
      foreach (ForceDirectedVertex vertex in singletons.Vertexes) {
        Rect bounds = vertex.Bounds;
        if (fringelen < 2) {
          center = new Point(bounds.X+bounds.Width/2, bounds.Y+bounds.Height/2);
          fringe[0] = new Point(bounds.Right + spacing.Width, bounds.Top);
          fringe[1] = new Point(bounds.Left, bounds.Bottom + spacing.Height);
          fringelen = 2;
          continue;
        }
        int idx = ClosestFringePoint(fringe, fringelen, center, new Size(bounds.Width, bounds.Height), spacing);
        Point old = fringe[idx];
        Point tr = new Point(old.X + bounds.Width + spacing.Width, old.Y);
        Point bl = new Point(old.X, old.Y + bounds.Height + spacing.Height);
        // make room for additional fringe point
        if (idx+1 < fringelen) Array.Copy(fringe, idx+1, fringe, idx+2, fringelen-idx-1);
        fringe[idx] = tr;
        fringe[idx+1] = bl;
        fringelen++;
        vertex.Center = new Point(old.X + vertex.Width/2, old.Y + vertex.Height/2);
      }
    }

    private int ClosestFringePoint(Point[] fringe, int fringelen, Point center, Size size, Size spacing) {
      double mindist = 9e19;
      int idx = -1;
      for (int i = 0; i < fringelen; i++) {
        Point p = fringe[i];
        double x = p.X-center.X;
        double y = p.Y-center.Y;
        double sq = x*x+y*y;
        if (sq < mindist) {
          for (int j = i-1; j >= 0; j--) {
            if (fringe[j].Y > p.Y && fringe[j].X-p.X < size.Width+spacing.Width) goto ANOTHER;
          }
          for (int j = i+1; j < fringelen; j++) {
            if (fringe[j].X > p.X && fringe[j].Y-p.Y < size.Height+spacing.Height) goto ANOTHER;
          }
          idx = i;
          mindist = sq;
        }
        ANOTHER: continue;
      }
      return idx;
    }

    /// <summary>
    /// Find associated objects to be positioned along with the
    /// <see cref="ForceDirectedVertex"/>.<see cref="GenericNetwork{N, L, Y}.Vertex.Node"/>.
    /// </summary>
    /// <param name="v"></param>
    /// <remarks>
    /// This method is called for each node in the network, when <see cref="Comments"/> is true.
    /// The standard behavior is to look for <see cref="Northwoods.GoXam.Node"/> objects
    /// whose category is "Comment" and that refer to
    /// the force-directed vertex's <see cref="GenericNetwork{N, L, Y}.Vertex.Node"/>.
    /// You may want to override this method in order to customize how
    /// any associated objects are found and how a new <see cref="ForceDirectedVertex"/>
    /// and <see cref="ForceDirectedEdge"/>
    /// are added to the network to represent the balloon comment.
    /// This method sets the new node's <see cref="ForceDirectedVertex.Charge"/>
    /// to the value of <see cref="DefaultCommentElectricalCharge"/>, and sets the new link's
    /// <see cref="ForceDirectedEdge.Length"/> to the value of
    /// <see cref="DefaultCommentSpringLength"/>.
    /// </remarks>
    protected virtual void AddComments(ForceDirectedVertex v) {
      Node node = v.Node;
      if (node != null) {
        IEnumerable<Node> comments = node.NodesConnected.Where(n => n.Category == "Comment");
        foreach (Node comment in comments) {
          ForceDirectedVertex cv = this.Network.FindVertex(comment);
          if (cv == null) {
            cv = this.Network.AddNode(comment);
            cv.Charge = this.DefaultCommentElectricalCharge;
            ForceDirectedEdge edge = this.Network.LinkVertexes(v, cv, null);
            if (edge != null) {
              edge.Length = this.DefaultCommentSpringLength;
            }
          }
        }
      }
    }

    /*********************************************************************************************/
    /*********************************************************************************************/
  
    private static double SQR(double x) { return x*x; }

    /// <summary>
    /// Returns the distance between two nodes.
    /// The default implementation considers the shortest distance between the two nodes.
    /// If the nodes correspond to top-level Go objects, the width and height of the
    /// GoObject is factored into the distance between the nodes.  If the nodes do not
    /// correspond to top-level Go objects, then the nodes implicitly have a width and
    /// height of zero, and the distance is calculated.
    /// </summary>
    /// <param name="vertexA"></param>
    /// <param name="vertexB"></param>
    /// <returns>Returns the distance between two nodes.</returns>
    protected virtual double GetNodeDistance(ForceDirectedVertex vertexA, ForceDirectedVertex vertexB) {
      Rect rA = vertexA.Bounds;
      double rectAx = rA.X;
      double rectAy = rA.Y;
      double rectAw = rA.Width;
      double rectAh = rA.Height;

      Rect rB = vertexB.Bounds;
      double rectBx = rB.X;
      double rectBy = rB.Y;
      double rectBw = rB.Width;
      double rectBh = rB.Height;

      if ((rectAx + rectAw) < rectBx) {
        if (rectAy > (rectBy + rectBh)) {
          return (double)Math.Sqrt(SQR((rectAx + rectAw) - rectBx) + SQR(rectAy - (rectBy + rectBh)));
        } 
        else if ((rectAy + rectAh) < rectBy) {
          return (double)Math.Sqrt(SQR((rectAx + rectAw) - rectBx) + SQR((rectAy + rectAh) - rectBy));
        } 
        else {
          return Math.Abs((rectAx + rectAw) - rectBx);
        }
      } 
      else if (rectAx > (rectBx + rectBw)) {
        if (rectAy > (rectBy + rectBh)) {
          return (double)Math.Sqrt(SQR(rectAx - (rectBx + rectBw)) + SQR(rectAy - (rectBy + rectBh)));
        } 
        else if ((rectAy + rectAh) < rectBy) {
          return (double)Math.Sqrt(SQR(rectAx - (rectBx + rectBw)) + SQR((rectAy + rectAh) - rectBy));
        } 
        else {
          return Math.Abs(rectAx - (rectBx + rectBw));
        }
      } 
      else {
        if (rectAy > (rectBy + rectBh)) {
          return Math.Abs(rectAy - (rectBy + rectBh));
        } 
        else if ((rectAy + rectAh) < rectBy) {
          return Math.Abs((rectAy + rectAh) - rectBy);
        } 
        else {
          // The two rectangles intersect.
          // Technically, could return 0.0, but that causes division
          //  by zero errors.  Instead, we return 0.1, which is smaller
          //  than any real distance between integer-coordinate points.
          //  Since the centers of the two rectangles are probably not
          //  the same, the forces between the nodes will point in some
          //  direction separating the nodes.
          return 0.1;
        }
      }
    }

    /// <summary>
    /// Returns the stiffness of the spring representing the ForceDirectedEdge link.  The
    /// to and from nodes of link L are acted upon by a force of magnitude
    /// <c>SpringStiffness(L) * (GetNodeDistance(L.from, L.to) - SpringLength(L))</c>.
    /// </summary>
    /// <param name="e"></param>
    /// <returns>Returns the stiffness of the spring representing link,
    /// normally the value of <see cref="DefaultSpringStiffness"/>,
    /// unless the value of <see cref="ForceDirectedEdge.Stiffness"/> had already been set,
    /// in which case it just returns that value.</returns>
    /// <remarks>
    /// Very small positive numbers are normal.
    /// Values larger than that tend to cause "jittery" behavior that makes it hard or impossible to settle down,
    /// since no stable state can be found.
    /// </remarks>
    protected virtual double SpringStiffness(ForceDirectedEdge e) {
      if (e.IsStiffnessSet) return e.Stiffness;
      return this.DefaultSpringStiffness;
    }

    /// <summary>
    /// Returns the length of the spring representing the ForceDirectedEdge link.  The
    /// to and from nodes of a link L are acted upon by a force of magnitude
    /// <c>SpringStiffness(L) * (GetNodeDistance(L.from, L.to) - SpringLength(L))</c>.
    /// </summary>
    /// <param name="e"></param>
    /// <returns>Returns the length of the spring representing link, normally the value of <see cref="DefaultSpringLength"/>,
    /// unless the value of <see cref="ForceDirectedEdge.Length"/> had already been set,
    /// in which case it just returns that value.</returns>
    /// <remarks>
    /// If your nodes are large, you may need to increase this value,
    /// so that many nodes aren't forced to be packed in close to each other.
    /// </remarks>
    protected virtual double SpringLength(ForceDirectedEdge e) {
      if (e.IsLengthSet && e.Length > 0) return e.Length;
      return this.DefaultSpringLength;
    }

    /*********************************************************************************************/
    /*********************************************************************************************/

    /// <summary>
    /// Returns the charge of the point representing the ForceDirectedVertex node.  A node L
    /// and a node V are acted upon by a force of magnitude
    ///  <c>(ElectricalCharge(L) * ElectricalCharge(V)) / (GetNodeDistance(L,V) * GetNodeDistance(L,V))</c>.
    /// A node L is acted upon by forces in the X and Y directions of magnitude
    ///  <c>ElectricalFieldX(L.position) * ElectricalCharge(L)</c> and
    ///  <c>ElectricalFieldY(L.position) * ElectricalCharge(L)</c>.
    /// </summary>
    /// <param name="v"></param>
    /// <returns>Returns the charge of the node,
    /// normally the value of <see cref="DefaultElectricalCharge"/>,
    /// unless the value of <see cref="ForceDirectedVertex.Charge"/> had already been set,
    /// in which case it just returns that value.</returns>
    protected virtual double ElectricalCharge(ForceDirectedVertex v) {
      if (v.IsChargeSet) return v.Charge;
      return this.DefaultElectricalCharge;
    }

    /// <summary>
    /// Returns the electrical field in the X direction
    /// acting on a node at the logical point Point xy.  A node L
    /// is acted upon by a force in the X direction of magnitude
    ///  <c>ElectricalFieldX(L.position) * ElectricalCharge(L)</c>.
    /// </summary>
    /// <param name="xy"></param>
    /// <returns>Returns the electrical field in the X direction, normally zero</returns>
    protected virtual double ElectricalFieldX(Point xy) {
      return 0.0;
    }

    /// <summary>
    /// Returns the electrical field in the Y direction
    /// acting on a node at the logical point <c>Point</c> xy.  A node L
    /// is acted upon by a force in the Y direction of magnitude
    ///  <c>ElectricalFieldY(L.position) * ElectricalCharge(L)</c>.
    /// </summary>
    /// <param name="xy"></param>
    /// <returns>Returns the electrical field in the Y direction, normally zero</returns>
    protected virtual double ElectricalFieldY(Point xy) {
      return 0.0;
    }


    /*********************************************************************************************/
    /*********************************************************************************************/

    /// <summary>
    /// The function GravitationalMass returns the mass of the point
    /// representing the ForceDirectedVertex node.  A node L
    /// is acted upon by forces in the X and Y directions of magnitude
    ///  <c>GravitationalFieldX(L.position) * GravitationalMass(L)</c> and
    ///  <c>GravitationalFieldY(L.position) * GravitationalMass(L)</c>.
    /// </summary>
    /// <param name="v"></param>
    /// <returns>Returns the mass of node,
    /// normally the value of <see cref="DefaultGravitationalMass"/>,
    /// unless the value of <see cref="ForceDirectedVertex.Mass"/> had already been set,
    /// in which case it just returns that value.</returns>
    protected virtual double GravitationalMass(ForceDirectedVertex v) {
      if (v.IsMassSet) return v.Mass;
      return this.DefaultGravitationalMass;
    }
  
    /// <summary>
    /// The function GravitationalFieldX returns the gravitational field
    /// in the X direction acting on a node at the logical point <c>Point</c> xy.  A node L
    /// is acted upon by a force in the X direction of magnitude
    ///  <c>GravitationalFieldX(L.position) * GravitationalMass(L)</c>.
    /// </summary>
    /// <param name="xy"></param>
    /// <returns>Returns the gravitational field in the X direction at point xy, normally zero.</returns>
    protected virtual double GravitationalFieldX(Point xy) {
      return 0.0;
    }
  
    /// <summary>
    /// The function GravitationalFieldY returns the gravitational field
    /// in the Y direction acting on a node at the logical point <c>Point</c> xy.  A node L
    /// is acted upon by a force in the Y direction of magnitude
    ///  <c>GravitationalFieldY(L.position) * GravitationalMass(L)</c>.
    /// </summary>
    /// <param name="xy"></param>
    /// <returns>Returns the gravitational field in the Y direction at point xy, normally zero</returns>
    protected virtual double GravitationalFieldY(Point xy) {
      return 0.0;
    }
  
    /*********************************************************************************************/
    /*********************************************************************************************/

    /// <summary>
    /// This predicate returns true if the node should not be moved
    /// by the layout algorithm but still have an effect on nearby and connected nodes.
    /// The default implementation returns <see cref="ForceDirectedVertex.IsFixed"/>.
    /// </summary>
    /// <param name="v"></param>
    /// <returns>Returns true if node should not be moved by the layout algorithm</returns>
    protected virtual bool IsFixed(ForceDirectedVertex v) {
      return v.IsFixed;
    }
  
    /*********************************************************************************************/
    /*********************************************************************************************/
  
    /// <summary>
    /// Peforms one iteration of the ForceDirected algorithm and updates the positions of
    /// the <see cref="ForceDirectedVertex"/>s (but not the document's nodes).
    /// </summary>
    /// <returns>Returns true if additional iterations are needed to find a solution.</returns>
    protected virtual bool UpdatePositions() {
      // Every node initially has no forces acting upon it.
      ForceDirectedNetwork.VertexList vertexarray = this.Network.VertexesArray;
      if (vertexarray.Count <= 0) return false;

      ForceDirectedVertex firstnode = vertexarray[0];
      firstnode.ForceX = 0;
      firstnode.ForceY = 0;
      double minx = firstnode.Center.X;
      double maxx = minx;
      double miny = firstnode.Center.Y;
      double maxy = miny;
      for (int ni = 1; ni < vertexarray.Count; ni++) {
        ForceDirectedVertex vertex = vertexarray[ni];
        vertex.ForceX = 0;
        vertex.ForceY = 0;
        Point p = vertex.Center;
        minx = Math.Min(minx, p.X);
        maxx = Math.Max(maxx, p.X);
        miny = Math.Min(miny, p.Y);
        maxy = Math.Max(maxy, p.Y);
      }
      bool sortedx = (maxx-minx > maxy-miny);
      if (sortedx)
        vertexarray.Sort(NodeSorterX.Default);
      else
        vertexarray.Sort(NodeSorterY.Default);

      double force = 0, forceX = 0, forceY = 0;

      // Calculate gravitational and electrical forces.
      for (int ni = 0; ni < vertexarray.Count; ni++) {
        ForceDirectedVertex vertexU = vertexarray[ni];
        Point NodeCenterU = vertexU.Center;

        // Calculate the electrical forces in the X and Y directions.
        forceX = vertexU.Charge * ElectricalFieldX(NodeCenterU);
        forceY = vertexU.Charge * ElectricalFieldY(NodeCenterU);

        // Update the total forces acting upon the node.
        vertexU.ForceX += forceX;
        vertexU.ForceY += forceY;

        // Calculate the gravitational forces in the X and Y directions.
        forceX = vertexU.Mass * GravitationalFieldX(NodeCenterU);
        forceY = vertexU.Mass * GravitationalFieldY(NodeCenterU);

        // Update the total forces acting upon the node.
        vertexU.ForceX += forceX;
        vertexU.ForceY += forceY;

        // Scan through remaining nodes in the network.
        double infinity = this.InfinityDistance;
        for (int posV = ni+1; posV < vertexarray.Count; posV++) {
          ForceDirectedVertex vertexV = vertexarray[posV];
          Point NodeCenterV = vertexV.Center;

          if (Math.Abs(NodeCenterU.X-NodeCenterV.X) > infinity) {
            if (sortedx) break;
            continue;
          }
          if (Math.Abs(NodeCenterU.Y-NodeCenterV.Y) > infinity) {
            if (!sortedx) break;
            continue;
          }

          double dist = GetNodeDistance(vertexU, vertexV);

          if (dist < 1) {
            // overlapping nodes
            Random rand = this.RandomNumberGenerator;
            if (rand == null) { rand = new Random(); this.RandomNumberGenerator = rand; }
            if (NodeCenterU.X > NodeCenterV.X) {
              forceX = Math.Abs(vertexV.Bounds.Right - vertexU.Bounds.Left);
              forceX = rand.Next(1 + (int)forceX);
            } else if (NodeCenterU.X < NodeCenterV.X) {
              forceX = Math.Abs(vertexV.Bounds.Left - vertexU.Bounds.Right);
              forceX = -rand.Next(1 + (int)forceX);
            } else {
              double maxw = Math.Max(vertexV.Width, vertexU.Width);
              forceX = rand.Next(1 + (int)maxw)-maxw/2;
            }
            if (NodeCenterU.Y > NodeCenterV.Y) {
              forceY = Math.Abs(vertexV.Bounds.Bottom - vertexU.Bounds.Top);
              forceY = rand.Next(1 + (int)forceY);
            } else if (NodeCenterU.X < NodeCenterV.X) {
              forceY = Math.Abs(vertexV.Bounds.Top - vertexU.Bounds.Bottom);
              forceY = -rand.Next(1 + (int)forceY);
            } else {
              double maxh = Math.Max(vertexV.Height, vertexU.Height);
              forceY = rand.Next(1 + (int)maxh)-maxh/2;
            }
          } else {
            // Calculate the electrical force acting on the nodes.
            force = -(vertexU.Charge * vertexV.Charge) / (dist * dist);

            // Calculate the electrical forces acting on the nodes in the X and Y directions.
            forceX = force * ((double)(NodeCenterV.X - NodeCenterU.X) / dist);
            forceY = force * ((double)(NodeCenterV.Y - NodeCenterU.Y) / dist);
          }

          // Update the total forces acting upon the nodes.
          vertexU.ForceX += forceX;
          vertexU.ForceY += forceY;
          vertexV.ForceX -= forceX;
          vertexV.ForceY -= forceY;
        }
      }

      // Calculate spring forces.
      foreach (ForceDirectedEdge edge in this.Network.Edges) {
        ForceDirectedVertex vertexU = edge.FromVertex;
        ForceDirectedVertex vertexV = edge.ToVertex;
        Point NodeCenterU = vertexU.Center;
        Point NodeCenterV = vertexV.Center;
        double dist = GetNodeDistance(vertexU, vertexV);

        if (dist < 1) {
          Random rand = this.RandomNumberGenerator;
          if (rand == null) { rand = new Random(); this.RandomNumberGenerator = rand; }
          forceX = (NodeCenterU.X > NodeCenterV.X ? 1 : -1) * rand.Next(1 + (int)Math.Max(vertexV.Width, vertexU.Width));
          forceY = (NodeCenterU.Y > NodeCenterV.Y ? 1 : -1) * rand.Next(1 + (int)Math.Max(vertexV.Height, vertexU.Height));
        }
        else
        {
          // Calculate the spring force acting on the nodes.
          force = edge.Stiffness * (dist - edge.Length);

          // Calculate the spring forces acting on the nodes in the X and Y directions.
          forceX = force * ((double)(NodeCenterV.X - NodeCenterU.X) / dist);
          forceY = force * ((double)(NodeCenterV.Y - NodeCenterU.Y) / dist);
        }

        // Update the total forces acting upon the nodes.
        vertexU.ForceX += forceX;
        vertexU.ForceY += forceY;
        vertexV.ForceX -= forceX;
        vertexV.ForceY -= forceY;
      }

      double maxMove = 0;
      double moveLimit = Math.Max(this.InfinityDistance/20, 50);

      // Update the positions of all nodes in the network.
      for (int i = 0; i < vertexarray.Count; i++) {
        ForceDirectedVertex vertex = vertexarray[i];
        // Only move the node if it is not fixed.
        if (!IsFixed(vertex)) {
          Point center = vertex.Center;

          // Calculate the distance to move the node based on the forces acting upon it.
          double dX = vertex.ForceX;
          double dY = vertex.ForceY;

          // Do not allow a node to move more than some number of units.
          double changeX = Math.Min(Math.Max(dX, -moveLimit), moveLimit);
          double changeY = Math.Min(Math.Max(dY, -moveLimit), moveLimit);

          // Set the center of the node.
          vertex.Center = new Point(center.X + changeX, center.Y + changeY);

          // Check to see if any node has moved significantly.
          maxMove = Math.Max(maxMove, SQR(changeX) + SQR(changeY));
        }
      }

      return (maxMove > SQR(this.Epsilon));
    }

    sealed private class NodeSorterX : IComparer<ForceDirectedVertex> {  // nested class
      private NodeSorterX() { }
      private static NodeSorterX myNodeSorter = new NodeSorterX();
      public static NodeSorterX Default {
        get { return myNodeSorter; }
      }
      public int Compare(ForceDirectedVertex a, ForceDirectedVertex b) {        
        if (a == null || b == null || a == b) return 0;
        Point ac = a.Center;
        Point bc = b.Center;
        if (ac.X < bc.X) return -1;
        if (ac.X > bc.X) return 1;
        return 0;
      }
    }

    sealed private class NodeSorterY : IComparer<ForceDirectedVertex> {  // nested class
      private NodeSorterY() { }
      private static NodeSorterY myNodeSorter = new NodeSorterY();
      public static NodeSorterY Default {
        get { return myNodeSorter; }
      }
      public int Compare(ForceDirectedVertex a, ForceDirectedVertex b) {
        if (a == null || b == null || a == b) return 0;
        Point ac = a.Center;
        Point bc = b.Center;
        if (ac.Y < bc.Y) return -1;
        if (ac.Y > bc.Y) return 1;
        return 0;
      }
    }

    sealed private class ConnComparer : IComparer<ForceDirectedVertex> {  // nested class
      private ConnComparer() { }
      private static ConnComparer myConnComparer = new ConnComparer();
      public static ConnComparer Default {
        get { return myConnComparer; }
      }
      public int Compare(ForceDirectedVertex a, ForceDirectedVertex b) {
        if (a == null || b == null || a == b) return 0;
        int an = a.NumConnections;
        int bn = b.NumConnections;
        if (an > bn) return -1;
        if (an < bn) return 1;
        return 0;
      }
    }

    sealed private class SizeComparer : IComparer<ForceDirectedVertex> {  // nested class
      private SizeComparer() { }
      private static SizeComparer mySizeComparer = new SizeComparer();
      public static SizeComparer Default {
        get { return mySizeComparer; }
      }
      public int Compare(ForceDirectedVertex a, ForceDirectedVertex b) {
        if (a == null || b == null || a == b) return 0;
        double asz = a.Width*a.Height;
        double bsz = b.Width*b.Height;
        if (asz > bsz) return -1;
        if (asz < bsz) return 1;
        return 0;
      }
    }


    /*********************************************************************************************/
    /*********************************************************************************************/

    /// <summary>
    /// Updates the physical location of "real" nodes and links to reflect
    /// the layout.
    /// </summary>
    /// <remarks>
    /// This sets any port spots, as directed by <see cref="SetsPortSpots"/>,
    /// and then calls <see cref="LayoutNodes"/> and <see cref="LayoutLinks"/>.
    /// </remarks>
    protected virtual void LayoutNodesAndLinks() {
      SetPortSpotsAll();
      LayoutNodes();
      LayoutLinks();
    }

    /// <summary>
    /// Commit the position of all of the vertex nodes.
    /// </summary>
    /// <remarks>
    /// The position of all of the nodes is affected by whether <see cref="ArrangesToOrigin"/> is true.
    /// </remarks>
    protected virtual void LayoutNodes() {
      Point offset = new Point(0, 0);
      if (this.ArrangesToOrigin) {
        Rect b = ComputeBounds(this.Network);
        Point orig = this.ArrangementOrigin;
        offset.X = orig.X-b.X;
        offset.Y = orig.Y-b.Y;
      }
      foreach (ForceDirectedVertex vertex in this.Network.Vertexes) {
        if (offset.X != 0 || offset.Y != 0) {
          Rect r = vertex.Bounds;
          r.X += offset.X;
          r.Y += offset.Y;
          vertex.Bounds = r;
        }
        vertex.CommitPosition();
      }
    }

    /// <summary>
    /// Commit the position and routing of all of the edge links.
    /// </summary>
    protected virtual void LayoutLinks() {
      foreach (ForceDirectedEdge edge in this.Network.Edges) {
        edge.CommitPosition();
      }
    }

    private void SetPortSpotsAll() {
      if (!this.SetsPortSpots) return;
      foreach (ForceDirectedEdge edge in this.Network.Edges) {
        Link link = edge.Link;
        if (link != null) {
          link.Route.FromSpot = Spot.Default;
          link.Route.ToSpot = Spot.Default;
        }
      }
    }

    /// <summary>
    /// Identifies the <see cref="SetsPortSpots"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SetsPortSpotsProperty;
    /// <summary>
    /// Gets or sets whether the FromSpot and the ToSpot
    /// of every link route should be set to <c>Spot.Default</c>.
    /// </summary>
    /// <value>The default value is true</value>
    [DefaultValue(true)]
    [Description("whether to set Route.FromSpot and Route.ToSpot to Spot.None")]
    public bool SetsPortSpots {
      get { return (bool)GetValue(SetsPortSpotsProperty); }
      set { SetValue(SetsPortSpotsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Comments"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommentsProperty;
    /// <summary>
    /// Gets or sets whether <see cref="AddComments"/> should find all <see cref="Northwoods.GoXam.Node"/>s
    /// whose category is "Comment" and
    /// whose anchors are nodes represented in the <see cref="Network"/> and add <see cref="ForceDirectedVertex"/>s
    /// representing those balloon comments as nodes in the network.
    /// </summary>
    /// <value>The default value is true</value>
    [DefaultValue(true)]
    [Description("whether to add comments as separate ForceDirectedVertexs to the Network")]
    public bool Comments {
      get { return (bool)GetValue(CommentsProperty); }
      set { SetValue(CommentsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MaxIterations"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxIterationsProperty;
    /// <summary>
    /// Gets or sets the maximum number of iterations to perform when doing the
    /// force directed auto layout.
    /// </summary>
    /// <value>The value must be non-negative.  The initial value is 100.</value>
    [DefaultValue(100)]
    [Description("the maximum number of iterations to perform")]
    public int MaxIterations {
      get { return (int)GetValue(MaxIterationsProperty); }
      set { SetValue(MaxIterationsProperty, value); }
    }
    private static void OnMaxIterationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs  e) {
      ForceDirectedLayout layout = (ForceDirectedLayout)d;
      int v = (int)e.NewValue;
      if (v < 0)
        layout.MaxIterations = (int)e.OldValue;
      else
        layout.InvalidateLayout();
    }

    /// <summary>
    /// Gets the current iteration count, valid during a call to <see cref="DoLayout"/>.
    /// </summary>
    public int CurrentIteration {
      get { return myIteration; }
    }

    /// <summary>
    /// Identifies the <see cref="Epsilon"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EpsilonProperty;
    /// <summary>
    /// Gets or sets approximately how far some node must move in order for the iterations to continue.
    /// </summary>
    [DefaultValue(1.0)]
    [Description("how much each iteration should move some node in order to continue")]
    public double Epsilon {
      get { return (double)GetValue(EpsilonProperty); }
      set { SetValue(EpsilonProperty, value); }
    }
    private static void OnEpsilonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      ForceDirectedLayout layout = (ForceDirectedLayout)d;
      double v = (double)e.NewValue;
      if (v <= 0)
        layout.Epsilon = (double)e.OldValue;
      else
        layout.InvalidateLayout();
    }

    /// <summary>
    /// Identifies the <see cref="InfinityDistance"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InfinityDistanceProperty;
    /// <summary>
    /// Gets or sets a threshold for the distance beyond which the electrical charge forces may be ignored.
    /// </summary>
    [DefaultValue(1000.0)]
    [Description("nodes farther than approximately this far apart might have no electrical charge force on each other")]
    public double InfinityDistance {
      get { return (double)GetValue(InfinityDistanceProperty); }
      set { SetValue(InfinityDistanceProperty, value); }
    }
    private static void OnInfinityDistanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      ForceDirectedLayout layout = (ForceDirectedLayout)d;
      double v = (double)e.NewValue;
      if (v <= 1)
        layout.InfinityDistance = (double)e.OldValue;
      else
        layout.InvalidateLayout();
    }

    /// <summary>
    /// Identifies the <see cref="RandomNumberGenerator"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RandomNumberGeneratorProperty;
    /// <summary>
    /// Gets or sets the random number generator used to give an initial push between objects
    /// that are located at the same position.
    /// </summary>
    /// <value>
    /// Initially this is null, which causes a new <c>System.Random</c> to be used.
    /// </value>
    public Random RandomNumberGenerator {
      get { return (Random)GetValue(RandomNumberGeneratorProperty); }
      set { SetValue(RandomNumberGeneratorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DefaultSpringStiffness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultSpringStiffnessProperty;
    /// <summary>
    /// Gets or sets the value returned by <see cref="SpringStiffness"/>.
    /// </summary>
    /// <value>The value is initially 0.05.</value>
    [DefaultValue(0.05)]
    public double DefaultSpringStiffness {
      get { return (double)GetValue(DefaultSpringStiffnessProperty); }
      set { SetValue(DefaultSpringStiffnessProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DefaultSpringLength"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultSpringLengthProperty;
    /// <summary>
    /// Gets or sets the value returned by <see cref="SpringLength"/>.
    /// </summary>
    /// <value>The value is initially 50.0.</value>
    [DefaultValue(50.0)]
    public double DefaultSpringLength {
      get { return (double)GetValue(DefaultSpringLengthProperty); }
      set { SetValue(DefaultSpringLengthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DefaultElectricalCharge"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultElectricalChargeProperty;
    /// <summary>
    /// Gets or sets the value returned by <see cref="ElectricalCharge"/>.
    /// </summary>
    /// <value>The value is initially 150.0.</value>
    [DefaultValue(150.0)]
    public double DefaultElectricalCharge {
      get { return (double)GetValue(DefaultElectricalChargeProperty); }
      set { SetValue(DefaultElectricalChargeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DefaultGravitationalMass"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultGravitationalMassProperty;
    /// <summary>
    /// Gets or sets the value returned by <see cref="GravitationalMass"/>.
    /// </summary>
    /// <value>The value is initially 0.0.</value>
    [DefaultValue(0.0)]
    public double DefaultGravitationalMass {
      get { return (double)GetValue(DefaultGravitationalMassProperty); }
      set { SetValue(DefaultGravitationalMassProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DefaultCommentSpringLength"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultCommentSpringLengthProperty;
    /// <summary>
    /// Gets or sets the value returned by <see cref="SpringLength"/>.
    /// </summary>
    /// <value>The value is initially 20.0.</value>
    [DefaultValue(20.0)]
    public double DefaultCommentSpringLength {
      get { return (double)GetValue(DefaultCommentSpringLengthProperty); }
      set { SetValue(DefaultCommentSpringLengthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DefaultCommentElectricalCharge"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultCommentElectricalChargeProperty;
    /// <summary>
    /// Gets or sets the value returned by <see cref="ElectricalCharge"/>.
    /// </summary>
    /// <value>The value is initially 25.0.</value>
    [DefaultValue(25.0)]
    public double DefaultCommentElectricalCharge {
      get { return (double)GetValue(DefaultCommentElectricalChargeProperty); }
      set { SetValue(DefaultCommentElectricalChargeProperty, value); }
    }

    /*********************************************************************************************/
    /*********************************************************************************************/

    //private variables

    //the number of iterations used in looking for a layout
    private int myIteration;
  }
}

