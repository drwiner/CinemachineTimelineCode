using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace GraphNamespace
{
    public class Edge {
        public Node source, sink;
        public float cost;

        public Edge(Node _source, Node _sink, float _weight)
        {
            source = _source;
            sink = _sink;
            cost = _weight;
        }

        public bool hasNode(Node x)
        {
            if (source == x || sink == x){
                return true;
            }
            return false;
        }

        public bool IsEqual(Edge other)
        {
            if ((source==other.source && sink == other.sink) || (source == other.sink && sink == other.source))
            {
                return true;
            }
            return false;
        }

        public Node GetNeighbor(Node other)
        {
            if (source == other)
            {
                return sink;
            }
            if (sink == other)
            {
                return source;
            }
            Debug.Log("Neighbor not detected.");
            throw new System.Exception();
        }

    }
   

    public class TileGraph : MonoBehaviour
    {
        public List<Node> nodes { get; set; }
        public List<Edge> edges { get; set; }
        public float adjacent_distance = 1f;
        public int numEdges;

        public void InitGraph()
        {
            nodes = new List<Node>();
            edges = new List<Edge>();

            for (int i = 0; i < transform.childCount; i++)
            {

                Node tn = transform.GetChild(i).GetComponent<Node>();

                nodes.Add(tn);

                for (int j = 0; j < transform.childCount; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }

                    Node other_node = transform.GetChild(j).GetComponent<Node>();

                    if (isAdjacent(tn.transform.position, other_node.transform.position))
                    {
                        Edge e = new Edge(tn, other_node, 1f);
                        if (!edges.Any(other_edge => other_edge.IsEqual(e)))
                        {
                            edges.Add(e);
                            //Debug.Log(e);
                        }
                    }

                }
            }

            numEdges = edges.Count;

        }

        public IEnumerable<Edge> getAdjacentEdges(Node tn)
        {
            return edges.Where(e => e.hasNode(tn));
        }

        //private 

        private bool isAdjacent(Vector3 a, Vector3 b)
        {
            float zdist = Mathf.Abs(a.z - b.z);
            float xdist = Mathf.Abs(a.x - b.x);
            if (zdist < adjacent_distance && xdist < adjacent_distance)
            {
                return true;
            }
            return false;
        }

    }

}
