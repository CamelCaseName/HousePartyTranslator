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
		BGC,
		Value,
		Door,
		Inventory,
		State,
		Personality,
		Cutscene,
		Clothing,
		CriteriaGroup,
		Pose,
		Property,
		Social
	}

	internal sealed class Node
	{
		public static readonly Node NullNode = new("", NodeType.Null, "");

		public bool ChildsVisited = false;
		public bool IsPositionLocked = false;
		public bool ParentsVisited = false;
		public bool Visited = false;
		public Gender Gender = Gender.None;
		public Guid Guid = Guid.NewGuid();
		public int Mass = 1;
		public List<Node> ChildNodes;
		public List<Node> ParentNodes;
		public NodeType Type;
		public object? Data = null;
		public PointF Position;
		public string FileName = string.Empty;
		public string ID;
		public string Text;
		public Type DataType = typeof(object);

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
			//todo remove string thing because it is no longer needed
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
					ParentNodes = new List<Node>(),
					IsPositionLocked = serialNode.IsPositionLocked
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
				tempNode.Data = criterion;
				tempNode.DataType = typeof(T);
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
				var nodeEvent = new Node(_event.Id ?? "none", NodeType.Event, _event.Value ?? "none", this) { FileName = FileName, Data = _event, DataType = typeof(T) };

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
