using Translator.Explorer.JSON;

namespace Translator.Explorer
{
	internal static class StoryNodeExtractor
	{

		public static NodeList GetAchievements(MainStory story)
		{
			//list to collect all achievement nodes
			var nodes = new NodeList();
			//go through all of them
			foreach (Achievement achievement in story.Achievements ?? Enumerable.Empty<Achievement>())
			{
				//node to add the description as child to, needs reference to parent, hence can't be anonymous
				var node = new Node(achievement.Id ?? string.Empty, NodeType.Achievement, achievement.Name ?? string.Empty) { Data = achievement, DataType = typeof(Achievement) };
				node.AddChildNode(new Node(achievement.Id + "Description", NodeType.Achievement, achievement.Description ?? string.Empty, node));
				node.AddChildNode(new Node(achievement.Id + "SteamName", NodeType.Achievement, achievement.SteamName ?? string.Empty, node));
				//add achievement with description child to list
				nodes.Add(node);
			}

			//return list of achievements
			return nodes;
		}

		public static NodeList GetBackGroundChatter(CharacterStory story)
		{
			var nodes = new NodeList();
			foreach (BackgroundChatter backgroundChatter in story.BackgroundChatter ?? Enumerable.Empty<BackgroundChatter>())
			{
				var bgcNode = new Node($"BGC{backgroundChatter.Id}", NodeType.BGC, backgroundChatter.Text ?? string.Empty) { Data = backgroundChatter, DataType = typeof(BackgroundChatter) };

				//criteria
				bgcNode.AddCriteria(backgroundChatter.Critera ?? new());

				//startevents
				bgcNode.AddEvents(backgroundChatter.StartEvents ?? new());

				//responses
				foreach (Response response in backgroundChatter.Responses ?? new())
				{
					var nodeResponse = new Node($"{response.CharacterName}{response.ChatterId}", NodeType.Response, "see id", bgcNode) { Data = response, DataType = typeof(Response) };

					bgcNode.AddChildNode(nodeResponse);
				}
			}

			return nodes;
		}

		public static NodeList GetDialogues(CharacterStory story)
		{
			var nodes = new NodeList();
			var responseDialogueLinks = new List<Tuple<Node, int>>();

			foreach (Dialogue dialogue in story.Dialogues ?? Enumerable.Empty<Dialogue>())
			{
				var nodeDialogue = new Node(dialogue.ID.ToString(), NodeType.Dialogue, dialogue.Text ?? string.Empty) { Data = dialogue, DataType = typeof(Dialogue) };
				int alternateTextCounter = 1;

				//add all alternate texts to teh dialogue
				foreach (AlternateText alternateText in dialogue.AlternateTexts ?? Enumerable.Empty<AlternateText>())
				{
					var nodeAlternateText = new Node($"{dialogue.ID}.{alternateTextCounter}", NodeType.Dialogue, alternateText.Text ?? string.Empty, nodeDialogue) { Data = alternateText, DataType = typeof(AlternateText) };

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
					var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty, nodeDialogue) { Data = response, DataType = typeof(Response) };

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

		public static NodeList GetGlobalGoodByeResponses(CharacterStory story)
		{
			var nodes = new NodeList();

			//add all responses as childs to this dialogue
			foreach (GlobalGoodbyeResponse response in story.GlobalGoodbyeResponses ?? Enumerable.Empty<GlobalGoodbyeResponse>())
			{
				var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty) { Data = response, DataType = typeof(GlobalGoodbyeResponse) };

				nodeResponse.AddCriteria(response.ResponseCriteria ?? new());

				//check all responses to this dialogue
				nodeResponse.AddEvents(response.ResponseEvents ?? new());

				nodes.Add(nodeResponse);
			}

			return nodes;
		}

		public static NodeList GetGlobalResponses(CharacterStory story)
		{
			var nodes = new NodeList();

			foreach (GlobalResponse response in story.GlobalResponses ?? Enumerable.Empty<GlobalResponse>())
			{
				var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty) { Data = response, DataType = typeof(GlobalResponse) };

				nodeResponse.AddCriteria(response.ResponseCriteria ?? new());

				//check all responses to this dialogue
				nodeResponse.AddEvents(response.ResponseEvents ?? new());

				nodes.Add(nodeResponse);
			}

			return nodes;
		}

		public static NodeList GetItemGroups(MainStory story)
		{
			//list to collect all item group nodes in the end
			var nodes = new NodeList();
			//go through all item groups to find events
			foreach (ItemGroupBehavior itemGroupBehaviour in story.ItemGroupBehaviors ?? Enumerable.Empty<ItemGroupBehavior>())
			{
				if (itemGroupBehaviour == null) continue;
				//create item group node to add events/criteria to
				var nodeGroup = new Node(itemGroupBehaviour.Id ?? string.Empty, NodeType.ItemGroup, itemGroupBehaviour.Name ?? string.Empty) { Data = itemGroupBehaviour, DataType = typeof(ItemGroupBehavior) };
				//get actions for item
				foreach (ItemAction itemAction in itemGroupBehaviour.ItemActions ?? new())
				{
					//node to addevents to
					var nodeAction = new Node(itemAction.ActionName ?? string.Empty, NodeType.Action, itemAction.ActionName ?? string.Empty, nodeGroup) { Data = itemAction, DataType = typeof(ItemAction) };

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

		public static NodeList GetItemOverrides(MainStory story)
		{
			//list to store all found root nodes
			var nodes = new NodeList();
			//go through all nodes to search them for actions
			foreach (ItemOverride itemOverride in story.ItemOverrides ?? Enumerable.Empty<ItemOverride>())
			{
				//add items to list
				var nodeItem = new Node(itemOverride.DisplayName ?? string.Empty, NodeType.Item, itemOverride.Id ?? string.Empty) { Data = itemOverride, DataType = typeof(ItemOverride) };
				//get actions for item
				foreach (ItemAction itemAction in itemOverride.ItemActions ?? new())
				{
					//create action node to add criteria and events to
					var nodeAction = new Node(itemAction.ActionName ?? string.Empty, NodeType.Action, itemAction.ActionName ?? string.Empty, nodeItem) { Data = itemAction, DataType = typeof(ItemAction) };

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

		public static NodeList GetPlayerReactions(MainStory story)
		{
			var nodes = new NodeList();
			foreach (PlayerReaction playerReaction in story.PlayerReactions ?? Enumerable.Empty<PlayerReaction>())
			{
				//add items to list
				var nodeReaction = new Node(playerReaction.Id ?? string.Empty, NodeType.Reaction, playerReaction.Name ?? string.Empty) { Data = playerReaction, DataType = typeof(PlayerReaction) };

				//get actions for item
				nodeReaction.AddEvents(playerReaction.Events ?? new());

				nodeReaction.AddCriteria(playerReaction.Critera ?? new());

				nodes.Add(nodeReaction);
			}

			return nodes;
		}

		public static NodeList GetQuests(CharacterStory story)
		{
			var nodes = new NodeList();

			foreach (Quest quest in story.Quests ?? Enumerable.Empty<Quest>())
			{
				var nodeQuest = new Node(quest.ID ?? string.Empty, NodeType.Quest, quest.Name ?? string.Empty) { Data = quest, DataType = typeof(Quest) };

				//Add details
				if (quest.Details?.Length > 0) nodeQuest.AddChildNode(new Node($"{quest.ID}Description", NodeType.Quest, quest.Details));
				//Add completed details
				if (quest.CompletedDetails?.Length > 0) nodeQuest.AddChildNode(new Node($"{quest.ID}CompletedDetails", NodeType.Quest, quest.CompletedDetails));
				//Add failed details
				if (quest.FailedDetails?.Length > 0) nodeQuest.AddChildNode(new Node($"{quest.ID}FailedDetails", NodeType.Quest, quest.FailedDetails));

				//Add extended details

				foreach (ExtendedDetail detail in quest.ExtendedDetails ?? Enumerable.Empty<ExtendedDetail>())
				{
					nodeQuest.AddChildNode(new Node($"{quest.ID}Description{detail.Value}", NodeType.Quest, detail.Details ?? string.Empty) { Data = detail, DataType = typeof(ExtendedDetail) });
				}

				nodes.Add(nodeQuest);
			}

			return nodes;
		}

		public static NodeList GetReactions(CharacterStory story)
		{
			var nodes = new NodeList();

			foreach (Reaction playerReaction in story.Reactions ?? Enumerable.Empty<Reaction>())
			{
				//add items to list
				var nodeReaction = new Node(playerReaction.Id ?? string.Empty, NodeType.Reaction, playerReaction.Name ?? string.Empty) { Data = playerReaction, DataType = typeof(Reaction) };
				//get actions for item
				nodeReaction.AddEvents(playerReaction.Events ?? new());

				nodeReaction.AddCriteria(playerReaction.Critera ?? new());

				nodes.Add(nodeReaction);
			}

			return nodes;
		}

		public static NodeList GetCriteriaGroups(MainStory story)
		{
			var nodes = new NodeList();
			foreach (CriteriaGroup group in story.CriteriaGroups ?? Enumerable.Empty<CriteriaGroup>())
			{
				//add items to list
				var nodeCriteriaGroup = new Node(group.Id!, NodeType.CriteriaGroup, group.Name + " True if " + group.PassCondition) { Data = group, DataType = typeof(CriteriaGroup) };

				foreach (var criteriaList in group.CriteriaList ?? Enumerable.Empty<CriteriaList1>())
				{
					nodeCriteriaGroup.AddCriteria(criteriaList.CriteriaList ?? new());
				}

				nodes.Add(nodeCriteriaGroup);
			}

			return nodes;
		}

		internal static NodeList GetPersonality(CharacterStory story)
		{
			var nodes = new NodeList();
			foreach (Valuee valuee in story.Personality?.Values ?? Enumerable.Empty<Valuee>())
			{
				//add items to list
				var nodeValue = new Node(valuee.Type!, NodeType.Personality, story.CharacterName + " " + valuee.Type + " " + valuee.Value) { Data = valuee, DataType = typeof(Valuee) };
				nodes.Add(nodeValue);
			}

			return nodes;
		}

		internal static NodeList GetItems(CharacterStory story)
		{
			var nodes = new NodeList();
			foreach (StoryItem item in story.StoryItems ?? Enumerable.Empty<StoryItem>())
			{
				//add items to list
				var nodeItem = new Node(item.ItemName!, NodeType.Item, item.ItemName!) { Data = item, DataType = typeof(StoryItem) };
				nodeItem.AddCriteria(item.Critera ?? new());
				nodeItem.AddEvents(item.OnRefuseEvents ?? new());
				nodeItem.AddEvents(item.OnAcceptEvents ?? new());
				nodes.Add(nodeItem);
			}

			return nodes;
		}

		internal static NodeList GetValues(CharacterStory story)
		{
			var nodes = new NodeList();
			foreach (string value in story.StoryValues ?? Enumerable.Empty<string>())
			{
				//add items to list
				var nodeValue = new Node(value!, NodeType.Value, story.CharacterName  + value + ", referenced values: ") { Data = value, DataType = typeof(string) };
				nodes.Add(nodeValue);
			}

			return nodes;
		}

		internal static NodeList GetValues(MainStory story)
		{
			var nodes = new NodeList();
			foreach (string value in story.PlayerValues ?? Enumerable.Empty<string>())
			{
				//add items to list
				var nodeValue = new Node(value, NodeType.Value, "Player " + value + ", referenced values: ") { Data = value, DataType = typeof(string) };
				nodes.Add(nodeValue);
			}

			return nodes;
		}
	}
}