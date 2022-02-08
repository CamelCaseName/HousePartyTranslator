using System;
using System.Collections.Generic;
using System.Drawing;

namespace HousePartyTranslator.Helpers
{
    public enum ClickedNodeTypes
    {
        Null,
        Highlight,
        Info,
        OpenInEditor,
        Edit
    }

    public enum NodeType
    {
        Null,
        Item,
        ItemGroup,
        Action,
        Event,
        Criterion,
        Response,
        Dialogue,
        Quest,
        Achievement,
        Reaction,
        BGC
    }

    public class Node
    {
        public Point Position;
        public string ID;
        public string Text;
        public NodeType Type;
        public int Mass = 1;
        public List<Node> ParentNodes;
        public List<Node> ChildNodes;
        public bool Visited = false;
        public bool ChildsVisited = false;
        public bool ParentsVisited = false;
        public Guid Guid = Guid.NewGuid();
        public static readonly Node NullNode = new Node("", NodeType.Null, "");

        public Node(string iD, NodeType type, string text, List<Node> parentNodes, List<Node> childNodes)
        {
            ID = iD;
            Text = text;
            Type = type;
            ParentNodes = parentNodes;
            ChildNodes = childNodes;
        }

        public Node(Point position, string iD, NodeType type, string text, List<Node> parentNodes, List<Node> childNodes)
        {
            Position = position;
            ID = iD;
            Text = text;
            Type = type;
            ParentNodes = parentNodes;
            ChildNodes = childNodes;
        }

        public Node(string iD, NodeType type, string text, List<Node> parentNodes)
        {
            ID = iD;
            Text = text;
            Type = type;
            ParentNodes = parentNodes;
            ChildNodes = new List<Node>();
        }

        public Node(string iD, NodeType type, string text, Node parentNodes, List<Node> childNodes)
        {
            ID = iD;
            Text = text;
            Type = type;
            ParentNodes = new List<Node>() { parentNodes };
            ChildNodes = childNodes;
        }

        public Node(string iD, NodeType type, string text, Node parentNode)
        {
            ID = iD;
            Text = text;
            Type = type;
            ParentNodes = new List<Node>() { parentNode };
            ChildNodes = new List<Node>();
        }

        public Node(string iD, NodeType type, string text, Node parentNode, Node childNode)
        {
            ID = iD;
            Text = text;
            Type = type;
            ParentNodes = new List<Node>() { parentNode };
            ChildNodes = new List<Node>() { childNode };
        }

        public Node(string iD, NodeType type, string text)
        {
            ID = iD;
            Text = text;
            Type = type;
            ParentNodes = new List<Node>();
            ChildNodes = new List<Node>();
        }

        public Node()
        {
            ID = "";
            Text = "";
            Type = NodeType.Null;
            ParentNodes = new List<Node>();
            ChildNodes = new List<Node>();
        }

        public void AddChildNode(Node childNode)
        {
            if (!ChildNodes.Contains(childNode))
            {
                ChildNodes.Add(childNode);
                childNode.AddParentNode(this);
            }
        }

        public void AddParentNode(Node parentNode)
        {
            if (!ParentNodes.Contains(parentNode))
            {
                ParentNodes.Add(parentNode);
                parentNode.AddChildNode(this);
            }
        }

        public void RemoveChildNode(Node childNode)
        {
            if (ChildNodes.Contains(childNode))
            {
                ChildNodes.Remove(childNode);
            }
        }

        public void RemoveParentNode(Node parentNode)
        {
            if (ParentNodes.Contains(parentNode))
            {
                ParentNodes.Remove(parentNode);
            }
        }

        public void SetPosition(Point position)
        {
            Position = position;
        }

        public override string ToString()
        {
            return $"{Type} | parents: {ParentNodes.Count} | childs: {ChildNodes.Count} | {ID} | {Text}";
        }

        public void CalculateMass()
        {
            Mass = ChildNodes.Count + ParentNodes.Count;
            if (Mass < 1) Mass = 1;
        }

        public void AddCriteria<T>(List<T> criteria) where T : ICriterion
        {
            List<ICriterion> _criteria = criteria.ConvertAll(x => (ICriterion)x);

            foreach (ICriterion criterion in _criteria)
            {
                if (criterion.CompareType == "State")
                {
                    AddParentNode(CreateCriteriaNode(criterion, this));
                }
            }
        }

        public void AddEvents<T>(List<T> events) where T : IEvent
        {
            List<IEvent> _events = events.ConvertAll(x => (IEvent)x);

            foreach (IEvent _event in _events)
            {
                if (_event.EventType == 10)
                {
                    Node nodeEvent = new Node(_event.Id, NodeType.Event, _event.Value, this);

                    nodeEvent.AddCriteria(_event.Criteria);

                    AddChildNode(nodeEvent);
                }
            }
        }



        public static Node CreateCriteriaNode(ICriterion criterion, Node node)
        {
            //create all criteria nodes the same way so they can possibly be replaced by the actual text later
            return new Node($"{criterion.Character}{criterion.Value}", NodeType.Criterion, $"{criterion.DialogueStatus}: {criterion.Character} - {criterion.Value}", new List<Node>(), new List<Node>() { node });
        }

        public static List<Node> ExpandDeserializedNodes(List<SerializeableNode> listToConvert)
        {
            List<Node> nodes = new List<Node>();

            //convert all nodes
            foreach (SerializeableNode serialNode in listToConvert)
            {
                nodes.Add(new Node()
                {
                    ChildsVisited = serialNode.ChildsVisited,
                    Guid = serialNode.Guid,
                    ID = serialNode.ID,
                    Mass = serialNode.Mass,
                    ParentsVisited = serialNode.ParentsVisited,
                    Position = serialNode.Position,
                    Text = serialNode.Text,
                    Type = serialNode.Type,
                    Visited = serialNode.Visited,
                    ChildNodes = new List<Node>(),
                    ParentNodes = new List<Node>()
                });
            }

            int index = 0;

            //resolve guids to pointer/references to other nodes in the list
            foreach (SerializeableNode serialNode in listToConvert)
            {
                //get node representing the serial node we are in
                Node nodeToWorkOn = nodes[index++];

                //add children
                if (serialNode.ChildNodes.Count > 0)
                {
                    foreach (Guid guid in serialNode.ChildNodes)
                    {
                        nodeToWorkOn.AddChildNode(nodes.Find(n => n.Guid == guid));
                    }
                }

                //add parents
                if (serialNode.ParentNodes.Count > 0)
                {
                    foreach (Guid guid in serialNode.ParentNodes)
                    {
                        nodeToWorkOn.AddParentNode(nodes.Find(n => n.Guid == guid));
                    }
                }
            }

            return nodes;
        }
    }
}
