using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Priority_Queue;
using GraphNamespace;

namespace GoalNamespace {

    class NodeInfo
    {
        public float dist = Mathf.Infinity;
        public float heuristic = 0f;
        public Node parent = null;
    }

    public static class PathFind {

        public static Stack<Node> Path { get; set; }
        private static SimplePriorityQueue<Node, float> frontier;
        private static Dictionary<Node, NodeInfo> node_dict;

        

        private static Dictionary<Node, NodeInfo> DijkstraInitialDictLoad(Node start, TileGraph tg)
        {
            node_dict = new Dictionary<Node, NodeInfo>();
            foreach (Node tn in tg.nodes)
            {
                NodeInfo ni = new NodeInfo();
                if (start == tn)
                    ni.dist = 0f;

                node_dict[tn] = ni;
            }

            return node_dict;
        }

        public static Stack<Node> Dijkstra(TileGraph tg, Node start, Node end)
        {
            frontier = new SimplePriorityQueue<Node, float>();
            frontier.Enqueue(start, 0f);
            node_dict = DijkstraInitialDictLoad(start, tg);
            List<Node> Expanded = new List<Node>();

            Node v;
            Node other;
            float edge_weight;
            float dist_to_node;
            float cost_so_far;

            while (frontier.Count > 0)
            {
                v = frontier.Dequeue();
                cost_so_far = node_dict[v].dist;
                Expanded.Add(v);

                //List<Edge> experiment = tg.getAdjacentEdges(v) as List<Edge>;
                foreach (Edge adj_edge in tg.getAdjacentEdges(v))
                {
                    other = adj_edge.GetNeighbor(v);
                    edge_weight = adj_edge.cost;
                    dist_to_node = node_dict[other].dist;
                    if (cost_so_far + edge_weight < dist_to_node)
                    {
                        node_dict[other].dist = cost_so_far + edge_weight;
                        node_dict[other].parent = v;
                    }

                    if (!Expanded.Any(node => node == other) && !frontier.Any(node => node == other))
                        frontier.Enqueue(other, node_dict[other].dist);
                }
            }

            Path = NodeDictToPath(node_dict, start, end);

            return Path;
        }

        private static Stack<Node> NodeDictToPath(Dictionary<Node, NodeInfo> node_dict, Node start, Node end)
        {
            Stack<Node> path = new Stack<Node>();
            bool found_start = false;
            Node currentEnd = end;
            while (!found_start)
            {
                path.Push(currentEnd);
                currentEnd = node_dict[currentEnd].parent;
                if (start == currentEnd)
                {
                    found_start = true;
                }
            }
            return path;
        }

        public static Stack<Node> Astar(TileGraph tg, Node start, Node end)
        {
            frontier = new SimplePriorityQueue<Node, float>();
            frontier.Enqueue(start, 0f);
            node_dict = new Dictionary<Node, NodeInfo>();
            List<Node> Expanded = new List<Node>();

            while (frontier.Count > 0)
            {

            }

            return Path;
        }

    }

}
