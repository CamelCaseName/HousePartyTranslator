using System.Collections.Generic;
using System.Drawing;

namespace HousePartyTranslator.Helpers
{
    public enum NodeType
    {
        Item,
        ItemGroup,
        Action,
        Event,
        Criteria,
        Response,
        Dialogue,
        Quest,
        Achievement,
        Reaction
    }

    public class Node
    {
        public Point Position;
        public string ID;
        public string Text;
        public NodeType Type;
        public int Weight;
        public List<Node> ParentNodes;
        public List<Node> ChildNodes;


        public Node(string iD, NodeType type, int weight, string text, List<Node> parentNodes, List<Node> childNodes)
        {
            ID = iD;
            Text = text;
            Type = type;
            Weight = weight;
            ParentNodes = parentNodes;
            ChildNodes = childNodes;
        }

        public Node(Point position, string iD, NodeType type, int weight, string text, List<Node> parentNodes, List<Node> childNodes)
        {
            Position = position;
            ID = iD;
            Text = text;
            Type = type;
            Weight = weight;
            ParentNodes = parentNodes;
            ChildNodes = childNodes;
        }

        public Node(string iD, NodeType type, int weight, string text, List<Node> parentNodes)
        {
            ID = iD;
            Text = text;
            Type = type;
            Weight = weight;
            ParentNodes = parentNodes;
            ChildNodes = new List<Node>();
        }

        public Node(string iD, NodeType type, int weight, string text, Node parentNodes, List<Node> childNodes)
        {
            ID = iD;
            Text = text;
            Type = type;
            Weight = weight;
            ParentNodes = new List<Node>() { parentNodes };
            ChildNodes = childNodes;
        }

        public Node(string iD, NodeType type, int weight, string text, Node parentNode)
        {
            ID = iD;
            Text = text;
            Type = type;
            Weight = weight;
            ParentNodes = new List<Node>() { parentNode };
            ChildNodes = new List<Node>();
        }

        public Node(string iD, NodeType type, int weight, string text, Node parentNode, Node childNode)
        {
            ID = iD;
            Text = text;
            Type = type;
            Weight = weight;
            ParentNodes = new List<Node>() { parentNode };
            ChildNodes = new List<Node>() { childNode };
        }


        public Node(string iD, NodeType type, int weight, string text)
        {
            ID = iD;
            Text = text;
            Type = type;
            Weight = weight;
            ParentNodes = new List<Node>();
            ChildNodes = new List<Node>();
        }

        public void AddChildNode(Node childNode)
        {
            if (!ChildNodes.Contains(childNode))
            {
                ChildNodes.Remove(childNode);
            }
        }

        public void AddParentNode(Node parentNode)
        {
            if (!ParentNodes.Contains(parentNode))
            {
                ParentNodes.Remove(parentNode);
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
    }
}
