using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bingo
{
    /// <summary>
    /// Represents a directed labeled graph with a string name at each node
    /// and a string Label for each edge.
    /// </summary>
    class RelationshipGraph
    {
        /*
         *  This data structure contains a list of nodes (each of which has
         *  an adjacency list) and a dictionary (hash table) for efficiently 
         *  finding nodes by name
         */
        public List<GraphNode> nodes { get; private set; }
        private Dictionary<String, GraphNode> nodeDict;

        // constructor builds empty relationship graph
        public RelationshipGraph()
        {
            nodes = new List<GraphNode>();
            nodeDict = new Dictionary<String, GraphNode>();
        }

        // AddNode creates and adds a new node if there isn't already one by that name
        public void AddNode(string name)
        {
            if (!nodeDict.ContainsKey(name))
            {
                GraphNode n = new GraphNode(name);
                nodes.Add(n);
                nodeDict.Add(name, n);
            }
        }

        // AddEdge adds the edge, creating endpoint nodes if necessary.
        // Edge is added to adjacency list of from edges.
        public void AddEdge(string name1, string name2, string relationship)
        {
            AddNode(name1);                     // create the node if it doesn't already exist
            GraphNode n1 = nodeDict[name1];     // now fetch a reference to the node
            AddNode(name2);
            GraphNode n2 = nodeDict[name2];
            GraphEdge e = new GraphEdge(n1, n2, relationship);
            n1.AddIncidentEdge(e);
        }

        // Get a node by name using dictionary
        public GraphNode GetNode(string name)
        {
            if (nodeDict.ContainsKey(name))
                return nodeDict[name];
            else
                return null;
        }

        // Return a text representation of graph
        public void Dump()
        {
            foreach (GraphNode n in nodes)
            {
                Console.Write(n.ToString());
            }
        }

        public void Orphans()
        {
            bool Orphan;

            foreach (GraphNode n in nodes)
            {
                Orphan = true;
                foreach (GraphEdge e in n.incidentEdges)
                {
                    //checks if each node has a parent
                    if (e.Label == "hasParent")
                    {
                        Orphan = false;
                        break;
                    }
                }
                if (Orphan)
                {
                    Console.WriteLine(n.Name);
                }
            }
        }

        public void Siblings(string name)
        {
            GraphNode node = GetNode(name);
            string parent;
            GraphNode pNode;
            List<string> siblings = new List<string>();

            if (node != null)
            {
                Console.WriteLine(name + "'s siblings:");
                foreach (GraphEdge e in node.incidentEdges)
                {
                    if (e.Label == "hasParent")
                    {
                        parent = e.To();
                        pNode = GetNode(parent);

                        foreach (GraphEdge d in pNode.incidentEdges)
                        {
                            if (d.Label == "hasChild" && d.To() != node.Name && !siblings.Contains(d.To()))
                            {
                                Console.WriteLine(d.To());
                                siblings.Add(d.To());
                            }
                        }

                    }
                }

                if (siblings.Count() == 0)
                {
                    Console.WriteLine(name + " has no siblings.");
                }
            }
            else
            {
                Console.WriteLine(name + " not found.");
            }
        }

        public void Descendants(string name, int depth, out bool failed)
        {
            if(depth == 0)
            {
                foreach(GraphNode n in nodes)
                {
                    n.Label = "Unvisited";
                }
            }
            GraphNode node = GetNode(name);
            string label = "child";

            if (depth >= 1)
            {
                label = "grand" + label;
            }
            for (int i = 2; i <= depth; ++i)
            {
                label = "great " + label;
            }

            if (node != null)
            {
                if (node.Label == "Unvisited")
                {
                    failed = false;
                    node.Label = "Visited";
                    foreach (GraphEdge e in node.incidentEdges)
                    {
                        if (e.Label == "hasChild")
                        {
                            if (!failed)
                            {
                                Console.WriteLine(label + " " + e.To());
                                Descendants(e.To(), depth + 1, out failed);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    failed = true;
                }
            }
            else
            {
                failed = true;
                Console.WriteLine(name + " not found.");
            }

            //Alerts the user that a cycle was found
            if(failed && depth == 0)
            {
                Console.WriteLine("Cycle found!");
            }
        }

        public void Bingo(string name, string name2)
        {
            GraphNode node = GetNode(name);
            GraphNode node2 = GetNode(name2);

            if (node != null && node2 != null)
            {
                //resets every node to unvisited
                foreach(GraphNode n in nodes)
                {
                    n.Label = "Unvisited";
                }

                Queue<int> q = new Queue<int>();
                int[] pred = new int[nodes.Count()];

                //sets parent for each node to -1 so it knows that there is no parent
                for (int i = 0; i < nodes.Count(); i++)
                {
                    pred[i] = -1;
                }

                int fromIndex = nodes.IndexOf(node);

                q.Enqueue(fromIndex);

                GraphNode toNode;
                int toIndex;
                List<GraphNode> incidentNodes = new List<GraphNode>();
                node.Label = "Visited";

                while (q.Count > 0)
                {
                    int u = q.Dequeue();
                    //goes through all incident nodes
                    for (int i = 0; i < nodes[u].incidentEdges.Count(); i++)
                    {
                        toNode = GetNode(nodes[u].incidentEdges[i].To());
                        toIndex = nodes.IndexOf(toNode);
                        if (toNode.Label == "Unvisited")
                        {
                            toNode.Label = "Visited";
                            pred[toIndex] = u;

                            if (toNode == node2)
                            {
                                q.Clear();
                                break;
                            }

                            foreach (GraphEdge e in nodes[u].incidentEdges)
                            {
                                incidentNodes.Add(GetNode(e.To()));
                            }

                            //queues each incident edge
                            foreach (GraphNode n in incidentNodes)
                            {
                                q.Enqueue(nodes.IndexOf(n));
                            }

                            incidentNodes.Clear();
                        }
                    }
                }

                List<GraphNode> path = new List<GraphNode>();

                int crawl = nodes.IndexOf(node2);
                path.Add(node2);
                while (pred[crawl] != -1)
                {
                    path.Add(nodes[pred[crawl]]);
                    crawl = pred[crawl];
                }

                
                //finds the shortest path between the 2 nodes
                for (int i = path.Count() - 1; i >= 1; i--)
                {
                    foreach (GraphEdge e in path[i].incidentEdges)
                    {
                        if (e.To() == path[i - 1].Name)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    }
                }

                if(path.Count <= 1)
                {
                    Console.WriteLine("No path found!");
                }
            }
            //Writes to the console what inputs were invalid
            else
            {
                if (node == null && node2 == null)
                {
                    Console.WriteLine(name + " and " + name2 + " do not exist");
                }
                else if (node == null)
                {
                    Console.WriteLine(name + " does not exist");
                }
                else if (node2 == null)
                {
                    Console.WriteLine(name2 + " does not exist");
                }
            }
        }

        // Cousins takes in a name, their level and how removed they are to find all the cousins at that point
        public void Cousins(string name, int number, int removed)
        {
            List<GraphNode> cousins = new List<GraphNode>();

            //finds first set of cousins
            findCousins(name, number + 1, removed + number + 1, cousins, GetNode(name));
            //find second set of cousin
            findCousins(name, removed + number + 1, number + 1, cousins, GetNode(name));

            if (cousins.Count() > 0)
            {
                foreach (GraphNode n in cousins)
                {
                    Console.WriteLine(n.Name);
                }
            }
            else
            {
                Console.WriteLine("No cousins at specified level found");
            }
        }

        //findCousins recursively calls itself to find the cousins
        public void findCousins(string name, int number, int removed, List<GraphNode> cousins, GraphNode source)
        {
            GraphNode node = GetNode(name);
            //checks if it needs to go up
            if (number > 0)
            {
                foreach (GraphEdge e in node.incidentEdges)
                {
                    if (e.Label == "hasParent")
                    {
                        findCousins(e.To(), number - 1, removed, cousins, node);
                    }
                }
            }
            //checks if it should go down
            else if (removed > 0)
            {
                foreach (GraphEdge e in node.incidentEdges)
                {
                    //makes sure the child isn't one of the parents found earlier
                    if (e.Label == "hasChild" && GetNode(e.To()) != source)
                    {
                        findCousins(e.To(), number, removed - 1, cousins, source);
                    }
                }
            }
            else
            {
                //adds any unfound cousins
                if (!cousins.Contains(node))
                {
                    cousins.Add(node);
                }
            }
        }
    }
}
