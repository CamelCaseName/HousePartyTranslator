﻿using System;
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
    }
}
