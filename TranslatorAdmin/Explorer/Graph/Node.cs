using System;
using System.Collections.Generic;
using System.Drawing;
using Translator.Desktop.Explorer.JSONItems;
using static Translator.Desktop.Explorer.JSONItems.StoryEnums;

namespace Translator.Desktop.Explorer.Graph
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
        CharacterGroup,
        Criterion,
        ItemAction,
        ItemGroupBehaviour,
        ItemGroupInteraction,
        Pose,
        Achievement,
        BGC,
        BGCResponse,
        Clothing,
        CriteriaGroup,
        Cutscene,
        Dialogue,
        AlternateText,
        Door,
        Event,
        EventTrigger,
        Inventory,
        Item,
        ItemGroup,
        Personality,
        Property,
        Quest,
        Response,
        Social,
        State,
        Value
    }

    internal sealed class Node
    {
        public static readonly Node NullNode = new();

        public bool ChildsVisited = false;
        public bool IsPositionLocked = false;
        public bool ParentsVisited = false;
        public bool Visited = false;
        public Gender Gender = Gender.None;
        public Guid Guid = Guid.NewGuid();
        public int Mass = 1;
        public NodeList ChildNodes;
        public NodeList ParentNodes;
        public NodeType Type;
        public object? Data = null;
        public PointF Position = PointF.Empty;
        public string FileName = string.Empty;
        public string ID;
        public string Text;
        public Type DataType = typeof(object);

        public Node(string iD, NodeType type, string text, NodeList parentNodes, NodeList childNodes)
        {
            ID = iD;
            Text = text;
            Type = type;
            ParentNodes = parentNodes;
            ChildNodes = childNodes;
        }

        public Node(string iD, NodeType type, string text, Node parentNode)
        {
            ID = iD;
            Text = text;
            Type = type;
            ParentNodes = new NodeList() { parentNode };
            ChildNodes = new NodeList();
        }

        public Node(string iD, NodeType type, string text)
        {
            ID = iD;
            Text = text;
            Type = type;
            ParentNodes = new NodeList();
            ChildNodes = new NodeList();
        }

        public Node()
        {
            ID = string.Empty;
            Text = string.Empty;
            Type = NodeType.Null;
            ParentNodes = new NodeList();
            ChildNodes = new NodeList();
        }

        public static Node CreateCriteriaNode(Criterion criterion, Node node)
        {
            //create all criteria nodes the same way so they can possibly be replaced by the actual text later
            return new Node(
                $"{criterion.Character}{criterion.CompareType}{criterion.Value}",
                NodeType.Criterion,
                $"{criterion.Character}|{criterion.CompareType}|{criterion.DialogueStatus}|{criterion.Key}|{criterion.Value}",
                new NodeList(),
                new NodeList() { node })
            { FileName = node.FileName };
        }

        public void AddChildNode(Node childNode)
        {
            if (!ChildNodes.Contains(childNode))
            {
                ChildNodes.Add(childNode);
                childNode.AddParentNode(this);
            }
        }

        public void AddCriteria(List<Criterion> criteria)
        {
            foreach (Criterion criterion in criteria)
            {
                Node tempNode = CreateCriteriaNode(criterion, this);
                tempNode.Data = criterion;
                tempNode.DataType = typeof(Criterion);
                if (criterion.CompareType == CompareTypes.PlayerGender)
                {
                    tempNode.Gender = criterion.Value == "Female" ? Gender.Female : criterion.Value == "Male" ? Gender.Male : Gender.None;
                }
                AddParentNode(tempNode);
            }
        }

        public void AddEvents(List<GameEvent> events)
        {
            foreach (GameEvent _event in events)
            {
                var nodeEvent = new Node(_event.Id ?? "none", NodeType.Event, _event.Value ?? "none", this) { FileName = FileName, Data = _event, DataType = typeof(GameEvent) };

                nodeEvent.AddCriteria(_event.Criteria ?? new List<Criterion>());

                AddChildNode(nodeEvent);
            }
        }

        public void AddParentNode(Node parentNode)
        {
            if (!ParentNodes.Contains(parentNode))
            {
                ParentNodes.Add(parentNode);
                parentNode.AddChildNode(this);
                Gender = parentNode.Gender;
                parentNode.PropagateGender(Gender);
            }
        }

        public void CalculateMass()
        {
            Mass = ChildNodes.Count + ParentNodes.Count;
            if (Mass < 1) Mass = 1;
        }

        public void PropagateGender(Gender gender)
        {
            Gender = gender;
            if (ChildNodes.Count > 0)
            {
                for (int i = 0; i < ChildNodes.Count; i++)
                {
                    if (ChildNodes[i].Gender != gender) ChildNodes[i].PropagateGender(gender);
                }
            }
        }

        public void RemoveChildNode(Node childNode)
        {
            if (ChildNodes.Contains(childNode))
            {
                _ = ChildNodes.Remove(childNode);
                childNode.RemoveParentNode(this);
            }
        }

        public void RemoveParentNode(Node parentNode)
        {
            if (ParentNodes.Contains(parentNode))
            {
                _ = ParentNodes.Remove(parentNode);
                parentNode.RemoveChildNode(this);
            }
        }

        public override string ToString()
        {
            return $"{Type} | parents: {ParentNodes.Count} | childs: {ChildNodes.Count} | {ID} | {Text}";
        }
    }
}
