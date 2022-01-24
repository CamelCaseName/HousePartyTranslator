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
        private List<Node> Nodes;
        private List<Node> CriteriaInFile;

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
        public void DissectCharacter(CharacterStory story)
        {

        }

        public void DissectStory(MainStory story)
        {
            CriteriaInFile = new List<Node>();

            //add all items in the story
            Nodes.AddRange(GetItemOverrides(story));
            //add all item groups with their actions
            Nodes.AddRange(GetItemGroups(story));
            //add all items in the story
            Nodes.AddRange(GetAchievements(story));
            //add all reactions the player will say
            Nodes.AddRange(GetPlayerReactions(story));

            //remove duplicates/merge criteria
            //maybe later we load the corresponding strings from the character files and vise versa?
            Nodes = CombineNodes(Nodes);
            //calculate starting positions
            Nodes = CalculateStartingPositions(Nodes);

            //render and do the force driven calculation thingies

            CriteriaInFile.Clear();
        }

        private List<Node> CalculateStartingPositions(List<Node> nodes)
        {
            return nodes;
        }

        private List<Node> CombineNodes(List<Node> nodes)
        {
            List<Node> tempNodes = new List<Node>();

            //go through all given root nodes (considered root as this stage)
            foreach (Node node in nodes)
            {
                if (node.ParentNodes.Count > 0 && !node.parentsVisited)
                {
                    //get combined parent nodes
                    node.parentsVisited = true;
                    tempNodes.AddRange(CombineNodes(node.ParentNodes));
                }
                //as long as there are children we can go further
                else if (node.ChildNodes.Count > 0 && !node.childsVisited)
                {
                    //get combined children nodes
                    node.childsVisited = true;
                    tempNodes.AddRange(CombineNodes(node.ChildNodes));
                }
                //actually add node
                if (node.Type == NodeType.Criteria)
                {
                    //node.visited = true;
                    //if in file
                    Node criterionInFile = CriteriaInFile.Find(n => n.ID == node.ID);
                    if (criterionInFile != null)
                    {
                        //add our parents/childs to existing node
                        criterionInFile.ChildNodes.AddRange(node.ChildNodes);
                        criterionInFile.ParentNodes.AddRange(node.ParentNodes);
                    }
                    else//no node yet, add to list
                    {
                        CriteriaInFile.Add(node);
                        tempNodes.Add(node);
                    }
                }
                else if (!node.visited)
                {
                    node.visited = true;
                    tempNodes.Add(node);
                }

            }
            return tempNodes;
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
                                currentEvent.AddParentNode(CreateCriteriaNode(criterion, currentEvent)); 
                            }
                        }
                        //add action to item
                        currentReaction.AddChildNode(currentEvent);
                    }

                }

                foreach (Critera critera in playerReaction.Critera)

                    if (critera.CompareType == "State")
                    {
                        currentReaction.AddParentNode(CreateCriteriaNode((Criterion)critera, currentReaction));
                    }

                nodes.Add(currentReaction);
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
                nodes.Add(node);
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
                                currentAction.AddParentNode(CreateCriteriaNode(criterion, currentEvent));
                            }
                        }

                        currentAction.AddChildNode(currentEvent);
                    }

                    //add criteria that influence this item
                    foreach (Criterion criterion in itemAction.Criteria)
                    {
                        if (criterion.CompareType == "State")
                        {
                            currentAction.AddParentNode(CreateCriteriaNode(criterion, currentAction));
                        }
                    }

                    //add action to item
                    currentGroup.AddChildNode(currentAction);
                }

                nodes.Add(currentGroup);
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
                                    currentAction.AddParentNode(CreateCriteriaNode(criterion, currentEvent)); 
                                }
                            }

                            currentAction.AddChildNode(currentEvent);
                        }
                    }

                    //add criteria that influence this item
                    foreach (Criterion criterion in itemAction.Criteria)
                    {
                        if (criterion.CompareType == "State")
                        {
                            currentAction.AddParentNode(CreateCriteriaNode(criterion, currentAction)); 
                        }
                    }

                    //add action to item
                    currentItem.AddChildNode(currentAction);
                }

                nodes.Add(currentItem);
            }

            return nodes;
        }

        private Node CreateCriteriaNode(Criterion criterion, Node node)
        {
            return new Node($"{criterion.Character}{criterion.Value}", NodeType.Criteria, 1, $"{criterion.DialogueStatus}: {criterion.Character} - {criterion.Value}", new List<Node>(), new List<Node>() { node });
        }
    }
}