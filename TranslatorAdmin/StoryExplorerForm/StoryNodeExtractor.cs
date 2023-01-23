using Translator.Explorer.JSON;

namespace Translator.Explorer
{
	internal static class StoryNodeExtractor
	{

		public static List<Node> GetAchievements(MainStory story)
		{
			//list to collect all achievement nodes
			var nodes = new List<Node>();
			//go through all of them
			foreach (Achievement achievement in story.Achievements ?? Enumerable.Empty<Achievement>())
			{
				//node to add the description as child to, needs reference to parent, hence can't be anonymous
				var node = new Node(achievement.Id ?? string.Empty, NodeType.Achievement, achievement.Name ?? string.Empty);
				node.AddChildNode(new Node(achievement.Id + "Description", NodeType.Achievement, achievement.Description ?? string.Empty, node));
				node.AddChildNode(new Node(achievement.Id + "SteamName", NodeType.Achievement, achievement.SteamName ?? string.Empty, node));
				//add achievement with description child to list
				nodes.Add(node);
			}

			//return list of achievements
			return nodes;
		}

		public static List<Node> GetBackGroundChatter(CharacterStory story)
		{
			var nodes = new List<Node>();
			foreach (BackgroundChatter backgroundChatter in story.BackgroundChatter ?? Enumerable.Empty<BackgroundChatter>())
			{
				var bgcNode = new Node($"BGC{backgroundChatter.Id}", NodeType.BGC, backgroundChatter.Text ?? string.Empty);

				//criteria
				bgcNode.AddCriteria(backgroundChatter.Critera ?? new());

				//startevents
				bgcNode.AddEvents(backgroundChatter.StartEvents ?? new());

				//responses
				foreach (Response response in backgroundChatter.Responses ?? new())
				{
					var nodeResponse = new Node($"{response.CharacterName}{response.ChatterId}", NodeType.Response, "see id", bgcNode);

					bgcNode.AddChildNode(nodeResponse);
				}
			}

			return nodes;
		}

		public static List<Node> GetDialogues(CharacterStory story)
		{
			var nodes = new List<Node>();
			var responseDialogueLinks = new List<Tuple<Node, int>>();

			foreach (Dialogue dialogue in story.Dialogues ?? Enumerable.Empty<Dialogue>())
			{
				var nodeDialogue = new Node(dialogue.ID.ToString(), NodeType.Dialogue, dialogue.Text ?? string.Empty);
				int alternateTextCounter = 1;

				//add all alternate texts to teh dialogue
				foreach (AlternateText alternateText in dialogue.AlternateTexts ?? Enumerable.Empty<AlternateText>())
				{
					var nodeAlternateText = new Node($"{dialogue.ID}.{alternateTextCounter}", NodeType.Dialogue, alternateText.Text ?? string.Empty, nodeDialogue);

					//increasse counter to ensure valid id
					alternateTextCounter++;

					nodeAlternateText.AddCriteria(alternateText.Critera ?? new());

					//add alternate to the default text as a child, parent already set on the child
					nodeDialogue.AddChildNode(nodeAlternateText);
				}

				//some events in here may have strings that are connected to the dialogue closing
				nodeDialogue.AddEvents(dialogue.CloseEvents ?? new());

				//add all responses as childs to this dialogue
				foreach (Response response in dialogue.Responses ?? Enumerable.Empty<Response>())
				{
					var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty, nodeDialogue);

					nodeResponse.AddCriteria(response.ResponseCriteria ?? new());

					//check all responses to this dialogue
					nodeResponse.AddEvents(response.ResponseEvents ?? new());

					if (response.Next != 0)
					{
						responseDialogueLinks.Add(new Tuple<Node, int>(nodeResponse, response.Next));
					}

					nodeDialogue.AddChildNode(nodeResponse);
				}

				//add the starting events
				nodeDialogue.AddEvents(dialogue.StartEvents ?? new());

				//finally add node
				nodes.Add(nodeDialogue);
			}

			//link up the dialogues and responses/next dialogues
			foreach (Tuple<Node, int> next in responseDialogueLinks)
			{
				Node node = nodes.Find(n => n.ID == next.Item2.ToString()) ?? Node.NullNode;
				if (node == Node.NullNode) continue;

				node.AddParentNode(next.Item1);
			}

			responseDialogueLinks.Clear();

			return nodes;
		}

		public static List<Node> GetGlobalGoodByeResponses(CharacterStory story)
		{
			var nodes = new List<Node>();

			//add all responses as childs to this dialogue
			foreach (GlobalGoodbyeResponse response in story.GlobalGoodbyeResponses ?? Enumerable.Empty<GlobalGoodbyeResponse>())
			{
				var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty);

				nodeResponse.AddCriteria(response.ResponseCriteria ?? new());

				//check all responses to this dialogue
				nodeResponse.AddEvents(response.ResponseEvents ?? new());

				nodes.Add(nodeResponse);
			}

			return nodes;
		}

		public static List<Node> GetGlobalResponses(CharacterStory story)
		{
			var nodes = new List<Node>();

			foreach (GlobalResponse response in story.GlobalResponses ?? Enumerable.Empty<GlobalResponse>())
			{
				var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty);

				nodeResponse.AddCriteria(response.ResponseCriteria ?? new());

				//check all responses to this dialogue
				nodeResponse.AddEvents(response.ResponseEvents ?? new());

				nodes.Add(nodeResponse);
			}

			return nodes;
		}

		public static List<Node> GetItemGroups(MainStory story)
		{
			//list to collect all item group nodes in the end
			var nodes = new List<Node>();
			//go through all item groups to find events
			foreach (ItemGroupBehavior itemGroupBehaviour in story.ItemGroupBehaviors ?? Enumerable.Empty<ItemGroupBehavior>())
			{
				if (itemGroupBehaviour == null) continue;
				//create item group node to add events/criteria to
				var nodeGroup = new Node(itemGroupBehaviour.Id ?? string.Empty, NodeType.ItemGroup, itemGroupBehaviour.Name ?? string.Empty);
				//get actions for item
				foreach (ItemAction itemAction in itemGroupBehaviour.ItemActions ?? new())
				{
					//node to addevents to
					var nodeAction = new Node(itemAction.ActionName ?? string.Empty, NodeType.Action, itemAction.ActionName ?? string.Empty, nodeGroup);

					//add text that is shown when item is taken
					nodeAction.AddEvents(itemAction.OnTakeActionEvents ?? new());

					//add criteria that influence this item
					nodeAction.AddCriteria(itemAction.Criteria ?? new());

					//add action to item
					nodeGroup.AddChildNode(nodeAction);
				}

				//add item gruop with everything to collecting list
				nodes.Add(nodeGroup);
			}

			//return list of all item groups
			return nodes;
		}

		public static List<Node> GetItemOverrides(MainStory story)
		{
			//list to store all found root nodes
			var nodes = new List<Node>();
			//go through all nodes to search them for actions
			foreach (ItemOverride itemOverride in story.ItemOverrides ?? Enumerable.Empty<ItemOverride>())
			{
				//add items to list
				var nodeItem = new Node(itemOverride.Id ?? string.Empty, NodeType.Item, itemOverride.DisplayName ?? string.Empty);
				//get actions for item
				foreach (ItemAction itemAction in itemOverride.ItemActions ?? new())
				{
					//create action node to add criteria and events to
					var nodeAction = new Node(itemAction.ActionName ?? string.Empty, NodeType.Action, itemAction.ActionName ?? string.Empty, nodeItem);

					//add text that is shown when item is taken
					nodeAction.AddEvents(itemAction.OnTakeActionEvents ?? new());

					//add criteria that influence this item
					nodeAction.AddCriteria(itemAction.Criteria ?? new());

					//add action to item
					nodeItem.AddChildNode(nodeAction);
				}

				//add item with all child nodes to collector list
				nodes.Add(nodeItem);
			}

			//duh
			return nodes;
		}

		public static List<Node> GetPlayerReactions(MainStory story)
		{
			var nodes = new List<Node>();
			foreach (PlayerReaction playerReaction in story.PlayerReactions ?? Enumerable.Empty<PlayerReaction>())
			{
				//add items to list
				var nodeReaction = new Node(playerReaction.Id ?? string.Empty, NodeType.Reaction, playerReaction.Name ?? string.Empty);

				//get actions for item
				nodeReaction.AddEvents(playerReaction.Events ?? new());

				nodeReaction.AddCriteria(playerReaction.Critera ?? new());

				nodes.Add(nodeReaction);
			}

			return nodes;
		}

		public static List<Node> GetQuests(CharacterStory story)
		{
			var nodes = new List<Node>();

			foreach (Quest quest in story.Quests ?? Enumerable.Empty<Quest>())
			{
				var nodeQuest = new Node(quest.ID ?? string.Empty, NodeType.Quest, quest.Name ?? string.Empty);

				//Add details
				if (quest.Details?.Length > 0) nodeQuest.AddChildNode(new Node($"{quest.ID}Description", NodeType.Quest, quest.Details));
				//Add completed details
				if (quest.CompletedDetails?.Length > 0) nodeQuest.AddChildNode(new Node($"{quest.ID}CompletedDetails", NodeType.Quest, quest.CompletedDetails));
				//Add failed details
				if (quest.FailedDetails?.Length > 0) nodeQuest.AddChildNode(new Node($"{quest.ID}FailedDetails", NodeType.Quest, quest.FailedDetails));

				//Add extended details

				foreach (ExtendedDetail detail in quest.ExtendedDetails ?? Enumerable.Empty<ExtendedDetail>())
				{
					nodeQuest.AddChildNode(new Node($"{quest.ID}Description{detail.Value}", NodeType.Quest, detail.Details ?? string.Empty));
				}

				nodes.Add(nodeQuest);
			}

			return nodes;
		}

		public static List<Node> GetReactions(CharacterStory story)
		{
			var nodes = new List<Node>();

			foreach (Reaction playerReaction in story.Reactions ?? Enumerable.Empty<Reaction>())
			{
				//add items to list
				var nodeReaction = new Node(playerReaction.Id ?? string.Empty, NodeType.Reaction, playerReaction.Name ?? string.Empty);
				//get actions for item
				nodeReaction.AddEvents(playerReaction.Events ?? new());

				nodeReaction.AddCriteria(playerReaction.Critera ?? new());

				nodes.Add(nodeReaction);
			}

			return nodes;
		}
	}
}