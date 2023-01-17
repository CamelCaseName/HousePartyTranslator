﻿using System.Xml.Linq;
using Translator.Explorer.JSON;
using static Translator.Explorer.JSON.StoryEnums;

namespace Translator.Explorer
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


	internal sealed class Node
	{
		public static readonly Node NullNode = new("", NodeType.Null, "");

		public List<Node> ChildNodes;
		public bool ChildsVisited = false;
		public Guid Guid = Guid.NewGuid();
		public string ID;
		public Gender Gender = Gender.None;//1 is female, 0 is male only
		public int Mass = 1;
		public List<Node> ParentNodes;
		public bool ParentsVisited = false;
		public PointF Position;
		public string Text;
		public NodeType Type;
		public bool Visited = false;
		public string FileName = string.Empty;

		public Node(string iD, NodeType type, string text, List<Node> parentNodes, List<Node> childNodes)
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
			ParentNodes = new List<Node>() { parentNode };
			ChildNodes = new List<Node>();
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

		public static Node CreateCriteriaNode(ICriterion criterion, Node node)
		{
			//create all criteria nodes the same way so they can possibly be replaced by the actual text later
			return new Node(
				$"{criterion.Character}{criterion.CompareType}{criterion.Value}", 
				NodeType.Criterion, 
				$"{criterion.Character}|{criterion.Character2}|{criterion.CompareType}|{criterion.DialogueStatus}|{criterion.EqualsValue}|{criterion.Key}|{criterion.Key2}|{criterion.Option}|{criterion.SocialStatus}|{criterion.Value}", 
				new List<Node>(), 
				new List<Node>() { node }) 
			{ FileName = node.FileName };
		}

		public static List<Node> ExpandDeserializedNodes(List<SerializeableNode> listToConvert)
		{
			var nodes = new List<Node>();

			//convert all nodes
			foreach (SerializeableNode serialNode in listToConvert)
			{
				nodes.Add(new Node()
				{
					ChildsVisited = serialNode.ChildsVisited,
					Guid = serialNode.Guid,
					ID = serialNode.ID ?? "",
					Mass = serialNode.Mass,
					ParentsVisited = serialNode.ParentsVisited,
					Position = serialNode.Position,
					Text = serialNode.Text ?? "",
					Gender = serialNode.Gender,
					Type = serialNode.Type,
					Visited = serialNode.Visited,
					FileName = serialNode.FileName,
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
				if (serialNode.ChildNodes?.Count > 0)
				{
					foreach (Guid guid in serialNode.ChildNodes)
					{
						nodeToWorkOn.AddChildNode(nodes.Find(n => n.Guid == guid) ?? Node.NullNode);
					}
				}

				//add parents
				if (serialNode.ParentNodes?.Count > 0)
				{
					foreach (Guid guid in serialNode.ParentNodes)
					{
						nodeToWorkOn.AddParentNode(nodes.Find(n => n.Guid == guid) ?? Node.NullNode);
					}
				}
			}

			return nodes;
		}

		public void AddChildNode(Node childNode)
		{
			if (!ChildNodes.Contains(childNode))
			{
				ChildNodes.Add(childNode);
				childNode.AddParentNode(this);
			}
		}

		public void AddCriteria<T>(List<T> criteria) where T : ICriterion
		{
			List<ICriterion> _criteria = criteria.ConvertAll(x => (ICriterion)x);

			foreach (ICriterion criterion in _criteria)
			{
				Node tempNode = CreateCriteriaNode(criterion, this);
				if (criterion.CompareType == CompareTypes.PlayerGender)
				{
					tempNode.Gender = criterion.Value == "Female" ? Gender.Female : criterion.Value == "Male" ? Gender.Male : Gender.None;
				}
				AddParentNode(tempNode);
			}
		}

		public void AddEvents<T>(List<T> events) where T : IEvent
		{
			List<IEvent> _events = events.ConvertAll(x => (IEvent)x);

			foreach (IEvent _event in _events)
			{
				var nodeEvent = new Node(_event.Id ?? "none", NodeType.Event, _event.Value ?? "none", this) { FileName = FileName };

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
			Mass = ChildNodes.Count;
			if (Mass < 1) Mass = 1;
		}

		public void CalculateScaledMass()
		{
			Mass = 1;
			for (int i = 0; i < ChildNodes.Count; i++)
			{
				Mass += (int)(NodeLayoutConstants.IdealLength / MathF.Sqrt((Position.X - ChildNodes[i].Position.X) * (Position.X - ChildNodes[i].Position.X) + (Position.Y - ChildNodes[i].Position.Y) * (Position.Y - ChildNodes[i].Position.Y)));
			}
			for (int i = 0; i < ParentNodes.Count; i++)
			{
				Mass += (int)(NodeLayoutConstants.IdealLength / MathF.Sqrt((Position.X - ParentNodes[i].Position.X) * (Position.X - ParentNodes[i].Position.X) + (Position.Y - ParentNodes[i].Position.Y) * (Position.Y - ParentNodes[i].Position.Y)));
			}
			if (Mass < 1) Mass = 1;
		}

		public void PropagateGender(Gender gender)
		{
			if (ChildNodes.Count > 0)
			{
				for (int i = 0; i < ChildNodes.Count; i++)
				{
					if (ChildNodes[i].Gender != gender) ChildNodes[i].PropagateGender(gender);
				}
				Gender = gender;
			}
		}

		public void RemoveChildNode(Node childNode)
		{
			if (ChildNodes.Contains(childNode))
			{
				_ = ChildNodes.Remove(childNode);
			}
		}

		public void RemoveParentNode(Node parentNode)
		{
			if (ParentNodes.Contains(parentNode))
			{
				_ = ParentNodes.Remove(parentNode);
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
	}
}
