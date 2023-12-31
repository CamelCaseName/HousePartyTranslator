using System;
using System.Collections.Generic;
using Translator.Explorer.Graph;
using Translator.Explorer.JSONItems;

namespace Translator.Explorer.Story
{
    public static class StoryNodeExtractor
    {
        public static NodeList GetAchievements(MainStory story)
        {
            //list to collect all achievement nodes
            var nodes = new NodeList();
            //go through all of them
            foreach (Achievement achievement in story.Achievements ?? new List<Achievement>())
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
            foreach (BackgroundChatter backgroundChatter in story.BackgroundChatter ?? new List<BackgroundChatter>())
            {
                var bgcNode = new Node($"BGC{backgroundChatter.Id}", NodeType.BGC, backgroundChatter.Text ?? string.Empty) { Data = backgroundChatter, DataType = typeof(BackgroundChatter) };

                //criteria
                bgcNode.AddCriteria(backgroundChatter.Critera ?? new());

                //startevents
                bgcNode.AddEvents(backgroundChatter.StartEvents ?? new());

                //responses
                foreach (BackgroundChatterResponse response in backgroundChatter.Responses ?? new())
                {
                    var nodeResponse = new Node($"{response.CharacterName}{response.ChatterId}", NodeType.BGCResponse, "see id", bgcNode) { Data = response, DataType = typeof(Response) };

                    bgcNode.AddChildNode(nodeResponse);
                }
                nodes.Add(bgcNode);
            }

            return nodes;
        }

        public static NodeList GetDialogues(CharacterStory story)
        {
            var nodes = new NodeList();
            var responseDialogueLinks = new List<Tuple<Node, int>>();

            foreach (Dialogue dialogue in story.Dialogues ?? new List<Dialogue>())
            {
                var nodeDialogue = new Node(dialogue.ID.ToString(), NodeType.Dialogue, dialogue.Text ?? string.Empty) { Data = dialogue, DataType = typeof(Dialogue) };
                int alternateTextCounter = 1;

                //add all alternate texts to teh dialogue
                foreach (AlternateText alternateText in dialogue.AlternateTexts ?? new List<AlternateText>())
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
                foreach (Response response in dialogue.Responses ?? new List<Response>())
                {
                    var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty, nodeDialogue) { Data = response, DataType = typeof(Response) };

                    nodeResponse.AddCriteria(response.ResponseCriteria ?? new());

                    //check all responses to this dialogue
                    nodeResponse.AddEvents(response.ResponseEvents ?? new());

                    if (response.Next != 0)
                        responseDialogueLinks.Add(new Tuple<Node, int>(nodeResponse, response.Next));

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
            foreach (Response response in story.GlobalGoodbyeResponses ?? new List<Response>())
            {
                var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty) { Data = response, DataType = typeof(Response) };

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

            foreach (Response response in story.GlobalResponses ?? new List<Response>())
            {
                var nodeResponse = new Node(response.Id ?? string.Empty, NodeType.Response, response.Text ?? string.Empty) { Data = response, DataType = typeof(Response) };

                nodeResponse.AddCriteria(response.ResponseCriteria ?? new());

                //check all responses to this dialogue
                nodeResponse.AddEvents(response.ResponseEvents ?? new());

                nodes.Add(nodeResponse);
            }

            return nodes;
        }

        public static NodeList GetItemGroupBehaviours(MainStory story)
        {
            //list to collect all item group nodes in the end
            var nodes = new NodeList();
            //go through all item groups to find events
            foreach (ItemGroupBehavior itemGroupBehaviour in story.ItemGroupBehaviors ?? new List<ItemGroupBehavior>())
            {
                if (itemGroupBehaviour is null) continue;
                //create item group node to add events/criteria to
                var nodeGroup = new Node(itemGroupBehaviour.Id ?? string.Empty, NodeType.ItemGroupBehaviour, itemGroupBehaviour.Name ?? string.Empty) { Data = itemGroupBehaviour, DataType = typeof(ItemGroupBehavior) };
                //get actions for item
                foreach (ItemAction itemAction in itemGroupBehaviour.ItemActions ?? new())
                {
                    //node to addevents to
                    var nodeAction = new Node(itemAction.ActionName ?? string.Empty, NodeType.ItemAction, itemAction.ActionName ?? string.Empty, nodeGroup) { Data = itemAction, DataType = typeof(ItemAction) };

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

        public static NodeList GetItemGroups(MainStory story)
        {
            //list to collect all item group nodes in the end
            var nodes = new NodeList();
            //go through all item groups to find events
            foreach (ItemGroup itemGroup in story.ItemGroups ?? new List<ItemGroup>())
            {
                if (itemGroup is null) continue;
                //create item group node to add events/criteria to
                var nodeGroup = new Node(itemGroup.Id ?? string.Empty, NodeType.ItemGroup, itemGroup.Name ?? string.Empty) { Data = itemGroup, DataType = typeof(ItemGroup) };
                //get actions for item
                foreach (string item in itemGroup.ItemsInGroup ?? new())
                {
                    //node to addevents to
                    var nodeItem = new Node(item ?? string.Empty, NodeType.Item, item ?? string.Empty, nodeGroup) { Data = item, DataType = typeof(string) };

                    //add item to item group
                    nodeGroup.AddChildNode(nodeItem);
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
            foreach (ItemOverride itemOverride in story.ItemOverrides ?? new List<ItemOverride>())
            {
                //add items to list
                var nodeItem = new Node(itemOverride.Id ?? string.Empty, NodeType.Item, itemOverride.DisplayName ?? string.Empty) { Data = itemOverride, DataType = typeof(ItemOverride) };
                //get actions for item
                foreach (ItemAction itemAction in itemOverride.ItemActions ?? new())
                {
                    //create action node to add criteria and events to
                    var nodeAction = new Node(itemAction.ActionName ?? string.Empty, NodeType.ItemAction, itemAction.ActionName ?? string.Empty, nodeItem) { Data = itemAction, DataType = typeof(ItemAction) };

                    //add text that is shown when item is taken
                    nodeAction.AddEvents(itemAction.OnTakeActionEvents ?? new());

                    //add criteria that influence this item
                    nodeAction.AddCriteria(itemAction.Criteria ?? new());

                    //add action to item
                    nodeItem.AddChildNode(nodeAction);

                    //add action to node list for later use
                    nodes.Add(nodeAction);
                }

                //get actions for item
                foreach (UseWith use in itemOverride.UseWiths ?? new())
                {
                    //node to add all references to
                    var useNode = new Node(use.ItemName ?? string.Empty, NodeType.Item, use.CustomCantDoThatMessage ?? string.Empty, nodeItem) { Data = use, DataType = typeof(UseWith) };

                    //add criteria that influence this item
                    useNode.AddCriteria(use.Criteria ?? new());

                    //add action to item
                    useNode.AddEvents(use.OnSuccessEvents ?? new());

                    //add note to list for later
                    nodes.Add(useNode);
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
            foreach (EventTrigger playerReaction in story.PlayerReactions ?? new List<EventTrigger>())
            {
                //add items to list
                var nodeReaction = new Node(playerReaction.Id ?? string.Empty, NodeType.EventTrigger, playerReaction.Name ?? string.Empty) { Data = playerReaction, DataType = typeof(EventTrigger) };

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

            foreach (Quest quest in story.Quests ?? new List<Quest>())
            {
                var nodeQuest = new Node(quest.ID ?? string.Empty, NodeType.Quest, quest.Name ?? string.Empty) { Data = quest, DataType = typeof(Quest) };

                //Add details
                if (quest.Details?.Length > 0) nodeQuest.AddChildNode(new Node($"{quest.ID}Description", NodeType.Quest, quest.Details));
                //Add completed details
                if (quest.CompletedDetails?.Length > 0) nodeQuest.AddChildNode(new Node($"{quest.ID}CompletedDetails", NodeType.Quest, quest.CompletedDetails));
                //Add failed details
                if (quest.FailedDetails?.Length > 0) nodeQuest.AddChildNode(new Node($"{quest.ID}FailedDetails", NodeType.Quest, quest.FailedDetails));

                //Add extended details

                foreach (ExtendedDetail detail in quest.ExtendedDetails ?? new List<ExtendedDetail>())
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

            foreach (EventTrigger playerReaction in story.Reactions ?? new List<EventTrigger>())
            {
                //add items to list
                var nodeReaction = new Node(playerReaction.Id ?? string.Empty, NodeType.EventTrigger, playerReaction.Name ?? string.Empty) { Data = playerReaction, DataType = typeof(EventTrigger) };
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
            foreach (CriteriaGroup group in story.CriteriaGroups ?? new List<CriteriaGroup>())
            {
                //add items to list
                var nodeCriteriaGroup = new Node(group.Id!, NodeType.CriteriaGroup, group.Name + " True if " + group.PassCondition) { Data = group, DataType = typeof(CriteriaGroup) };

                foreach (CriteriaList1 criteriaList in group.CriteriaList ?? new List<CriteriaList1>())
                {
                    nodeCriteriaGroup.AddCriteria(criteriaList.CriteriaList ?? new());
                }

                nodes.Add(nodeCriteriaGroup);
            }

            return nodes;
        }

        public static NodeList GetPersonality(CharacterStory story)
        {
            var nodes = new NodeList();
            foreach (Trait valuee in story.Personality?.Values ?? new List<Trait>())
            {
                //add items to list
                var nodeValue = new Node(valuee.Type.ToString()!, NodeType.Personality, story.CharacterName + " " + valuee.Type + " " + valuee.Value) { Data = valuee, DataType = typeof(Trait) };
                nodes.Add(nodeValue);
            }

            return nodes;
        }

        public static NodeList GetItems(CharacterStory story)
        {
            var nodes = new NodeList();
            foreach (StoryItem item in story.StoryItems ?? new List<StoryItem>())
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

        public static NodeList GetValues(CharacterStory story)
        {
            var nodes = new NodeList();
            foreach (string value in story.StoryValues ?? new List<string>())
            {
                //add items to list
                var nodeValue = new Node(value!, NodeType.Value, story.CharacterName + value + ", referenced values: ") { Data = new Value() { value = value }, DataType = typeof(Value) };
                nodes.Add(nodeValue);
            }

            return nodes;
        }

        public static NodeList GetValues(MainStory story)
        {
            var nodes = new NodeList();
            foreach (string value in story.PlayerValues ?? new List<string>())
            {
                //add items to list
                var nodeValue = new Node(value, NodeType.Value, "Player " + value + ", referenced values: ") { Data = new Value() { value = value }, DataType = typeof(Value) };
                nodes.Add(nodeValue);
            }

            return nodes;
        }

        public static NodeList GetGameStartEvents(MainStory story)
        {
            var nodes = new NodeList();
            var nodeEvents = new Node("GameStartEvents"!, NodeType.EventTrigger, "GameStartEvents");
            foreach (GameEvent _event in story.GameStartEvents ?? new())
            {
                var nodeEvent = new Node(_event.Id ?? "none", NodeType.Event, _event.Value ?? "none") { Data = _event, DataType = typeof(GameEvent) };

                nodeEvent.AddCriteria(_event.Criteria ?? new List<Criterion>());

                nodeEvents.AddChildNode(nodeEvent);
            }
            nodes.Add(nodeEvents);
            return nodes;
        }
    }
}