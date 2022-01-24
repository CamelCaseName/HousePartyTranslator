using HousePartyTranslator.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    internal class ContextProvider
    {
        private readonly Panel DrawingPanel;
        private string _StoryFilePath;
        private Graphics DrawingSpace;
        private readonly bool IsStory;
        private readonly List<Node> Nodes;

        private int TotalNodes = 0;


        public string FilePath
        {
            get
            {
                return _StoryFilePath;
            }
            set
            {
                if (File.Exists(value))
                {
                    _StoryFilePath = value;
                }
                else
                {
                    OpenFileDialog selectFileDialog;
                    if (IsStory)
                    {
                        selectFileDialog = new OpenFileDialog
                        {
                            Title = "Choose the story file you want to explore",
                            Filter = "Story Files (*.story)|*.story",
                            InitialDirectory = Properties.Settings.Default.story_path.Length > 0 ? Path.GetDirectoryName(Properties.Settings.Default.story_path) : @"C:\Users\%USER%\Documents",
                            RestoreDirectory = false

                        };
                    }
                    else//character file
                    {
                        selectFileDialog = new OpenFileDialog
                        {
                            Title = "Choose the character file you want to explore",
                            Filter = "Character Files (*.character)|*.character",
                            InitialDirectory = Properties.Settings.Default.story_path.Length > 0 ? Path.GetDirectoryName(Properties.Settings.Default.story_path) : @"C:\Users\%USER%\Documents",
                            RestoreDirectory = false
                        };
                    }

                    if (selectFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Properties.Settings.Default.story_path = selectFileDialog.FileName;
                        Properties.Settings.Default.Save();
                        FilePath = selectFileDialog.FileName;
                    }
                    else
                    {
                        //close form if cancelled
                        DrawingPanel.FindForm().Close();
                    }
                }
            }
        }

        public ContextProvider(string FilePath, Panel DrawingSpace, bool IsStory, bool AutoSelectFile)
        {
            Nodes = new List<Node>();
            this.IsStory = IsStory;
            DrawingPanel = DrawingSpace;
            this.DrawingSpace = DrawingPanel.CreateGraphics();
            if (Properties.Settings.Default.story_path != "" && AutoSelectFile)
            {
                this.FilePath = Properties.Settings.Default.story_path;
            }
            else
            {
                this.FilePath = FilePath;
            }
        }

        public bool ParseFile()
        {
            if (File.Exists(FilePath))
            {
                string fileString = File.ReadAllText(FilePath);
                //save path
                Properties.Settings.Default.story_path = FilePath;

                if (IsStory)
                {
                    DissectStory(JsonConvert.DeserializeObject<MainStory>(fileString));
                }
                else
                {
                    DissectCharacter(JsonConvert.DeserializeObject<CharacterStory>(fileString));
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void DissectStory(MainStory story)
        {
            int totalBeforeCombine = 0;
            //add all items in the story
            Nodes.AddRange(GetItemOverrides(story));
            //add all item groups with their actions
            Nodes.AddRange(GetItemGroups(story));
            //add all items in the story
            Nodes.AddRange(GetAchievements(story));
            //add all reactions the player will say
            Nodes.AddRange(GetPlayerReactions(story));

            totalBeforeCombine = TotalNodes;
            TotalNodes = 0;

            //remove duplicates and fix edges (links)
        }

        private List<Node> GetPlayerReactions(MainStory story)
        {
            List<Node> nodes = new List<Node>();
            foreach (PlayerReaction playerReaction in story.PlayerReactions)
            {
                //add items to list
                Node currentReaction = new Node(playerReaction.Id, NodeType.Reaction, 1, playerReaction.Name);
                //get actions for item
                foreach (Event eevent in playerReaction.Events)
                {

                    Node currentEvent = new Node(eevent.Id, NodeType.Event, 1, eevent.Value, currentReaction);
                    //only if correct type
                    if (eevent.EventType == 10)
                    {


                        //add criteria that influence this item
                        foreach (Criterion criterion in eevent.Criteria)
                        {
                            if (criterion.CompareType == "State")
                            {
                                currentEvent.AddParentNode(new Node(criterion.Value, NodeType.Criteria, 1, "*criteria*", new List<Node>(), new List<Node>() { currentEvent }));
                                TotalNodes++;
                            }
                        }
                        //add action to item
                        currentReaction.AddChildNode(currentEvent);
                        TotalNodes++;
                    }

                }

                foreach (Critera critera in playerReaction.Critera)

                    if (critera.CompareType == "State")
                    {
                        currentReaction.AddParentNode(new Node(critera.Value, NodeType.Criteria, 1, "*criteria*", new List<Node>(), new List<Node>() { currentReaction }));
                        TotalNodes++;
                    }

                nodes.Add(currentReaction);
                TotalNodes++;
            }

            return nodes;
        }

        private List<Node> GetAchievements(MainStory story)
        {
            List<Node> nodes = new List<Node>();
            foreach (Achievement achievement in story.Achievements)
            {
                Node node = new Node(achievement.Id, NodeType.Achievement, 1, achievement.Name);
                node.AddChildNode(new Node(achievement.Id + "Description", NodeType.Achievement, 1, achievement.Description, node));
                TotalNodes++;
                nodes.Add(node);
                TotalNodes++;
            }

            return nodes;
        }

        private List<Node> GetItemGroups(MainStory story)
        {
            List<Node> nodes = new List<Node>();
            foreach (ItemGroupBehavior itemGroupBehaviour in story.ItemGroupBehaviors)
            {
                //add items to list
                Node currentGroup = new Node(itemGroupBehaviour.Id, NodeType.ItemGroup, 1, itemGroupBehaviour.Name);
                //get actions for item
                foreach (ItemAction itemAction in itemGroupBehaviour.ItemActions)
                {
                    Node currentAction = new Node(itemAction.ActionName, NodeType.Action, 1, itemAction.ActionName, currentGroup);

                    //add text that is shown when item is taken
                    foreach (OnTakeActionEvent onTakeActionEvent in itemAction.OnTakeActionEvents)
                    {
                        Node currentEvent = new Node(onTakeActionEvent.Id, NodeType.Event, 1, onTakeActionEvent.Value, currentAction);

                        //add criteria that influence this item
                        foreach (Criterion criterion in onTakeActionEvent.Criteria)
                        {
                            if (criterion.CompareType == "State")
                            {
                                currentEvent.AddParentNode(new Node(criterion.Value, NodeType.Criteria, 1, "*criteria*", new List<Node>(), new List<Node>() { currentEvent }));
                                TotalNodes++;
                            }
                        }

                        currentAction.AddChildNode(currentEvent);
                        TotalNodes++;
                    }

                    //add criteria that influence this item
                    foreach (Criterion criterion in itemAction.Criteria)
                    {
                        if (criterion.CompareType == "State")
                        {
                            currentAction.AddParentNode(new Node(criterion.Value, NodeType.Criteria, 1, "*criteria*", new List<Node>(), new List<Node>() { currentAction }));
                            TotalNodes++;
                        }
                    }

                    //add action to item
                    currentGroup.AddChildNode(currentAction);
                    TotalNodes++;
                }

                nodes.Add(currentGroup);
                TotalNodes++;
            }

            return nodes;
        }

        private List<Node> GetItemOverrides(MainStory story)
        {
            List<Node> nodes = new List<Node>();
            foreach (ItemOverride itemOverride in story.ItemOverrides)
            {
                //add items to list
                Node currentItem = new Node(itemOverride.Id, NodeType.Item, 1, itemOverride.DisplayName);
                //get actions for item
                foreach (ItemAction itemAction in itemOverride.ItemActions)
                {
                    Node currentAction = new Node(itemAction.ActionName, NodeType.Action, 1, itemAction.ActionName, currentItem);

                    //add text that is shown when item is taken
                    foreach (OnTakeActionEvent onTakeActionEvent in itemAction.OnTakeActionEvents)
                    {
                        //only if correct type
                        if (onTakeActionEvent.EventType == 10)
                        {
                            Node currentEvent = new Node(onTakeActionEvent.Id, NodeType.Event, 1, onTakeActionEvent.Value, currentAction);

                            //add criteria that influence this item
                            foreach (Criterion criterion in onTakeActionEvent.Criteria)
                            {
                                if (criterion.CompareType == "State")
                                {
                                    currentEvent.AddParentNode(new Node(criterion.Value, NodeType.Criteria, 1, "*criteria*", new List<Node>(), new List<Node>() { currentEvent }));
                                    TotalNodes++;
                                }
                            }

                            currentAction.AddChildNode(currentEvent);
                            TotalNodes++;
                        }
                    }

                    //add criteria that influence this item
                    foreach (Criterion criterion in itemAction.Criteria)
                    {
                        if (criterion.CompareType == "State")
                        {
                            currentAction.AddParentNode(new Node(criterion.Value, NodeType.Criteria, 1, "*criteria*", new List<Node>(), new List<Node>() { currentAction }));
                            TotalNodes++;
                        }
                    }

                    //add action to item
                    currentItem.AddChildNode(currentAction);
                    TotalNodes++;
                }

                nodes.Add(currentItem);
                TotalNodes++;
            }

            return nodes;
        }

        public void DissectCharacter(CharacterStory story)
        {

        }
    }
}