using HousePartyTranslator.Helpers;
using HousePartyTranslator.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    public class ContextProvider
    {
        public bool GotCancelled = false;
        private readonly string FileId;
        private readonly bool IsStory;
        private readonly Random Random = new Random();
        private string _StoryFilePath;
        private List<Node> CriteriaInFile;
        private Dictionary<Guid, Vector2> NodeForces;
        private List<Node> Nodes;

        public ContextProvider(bool IsStory, bool AutoSelectFile, string FileName, string StoryName)
        {
            Nodes = new List<Node>();
            this.IsStory = IsStory;

            if (Properties.Settings.Default.story_path != "" && AutoSelectFile && FileName != "" && StoryName != "")
            {
                string storyPathMinusStory = Directory.GetParent(Properties.Settings.Default.story_path).FullName;

                if (IsStory)
                {
                    FilePath = Path.Combine(storyPathMinusStory, StoryName, $"{FileName}.story");
                }
                else
                {
                    FilePath = Path.Combine(storyPathMinusStory, StoryName, $"{FileName}.character");
                }
            }
            else
            {
                FilePath = "";
            }

            //create an id to differentiate between the different calculated layouts later
            FileId = StoryName + FileName + DataBaseManager.DBVersion;
        }

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

                    if (IsStory)//story file
                    {
                        selectFileDialog = new OpenFileDialog
                        {
                            Title = "Choose the story file you want to explore",
                            Filter = "Story Files (*.story)|*.story",
                            InitialDirectory = Properties.Settings.Default.story_path.Length > 0 ? Properties.Settings.Default.story_path : @"C:\Users\%USER%\Documents",
                            RestoreDirectory = false

                        };
                    }
                    else//character file
                    {
                        selectFileDialog = new OpenFileDialog
                        {
                            Title = "Choose the character file you want to explore",
                            Filter = "Character Files (*.character)|*.character",
                            InitialDirectory = Properties.Settings.Default.story_path.Length > 0 ? Properties.Settings.Default.story_path : @"C:\Users\%USER%\Documents",
                            RestoreDirectory = false
                        };
                    }

                    if (selectFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Properties.Settings.Default.story_path = Path.GetDirectoryName(selectFileDialog.FileName);
                        Properties.Settings.Default.Save();
                        FilePath = selectFileDialog.FileName;
                    }
                    else
                    {
                        //close form if cancelled
                        GotCancelled = true;
                    }
                }
            }
        }

        public List<Node> GetNodes()
        {
            return Nodes;
        }

        public bool ParseFile()
        {
            if (File.Exists(FilePath))
            {
                string fileString = File.ReadAllText(FilePath);
                string savedNodesPath = Path.Combine(LogManager.CFGFOLDER_PATH, $"{FileId}.json");

                //save path
                Properties.Settings.Default.story_path = Path.GetDirectoryName(FilePath);
                //try to laod the saved nodes
                if (File.Exists(savedNodesPath))
                {
                    //read in positions if they exist, but only if version is the same
                    List<SerializeableNode> tempList = JsonConvert.DeserializeObject<List<SerializeableNode>>(File.ReadAllText(savedNodesPath));
                    //expand the guids back into references
                    Nodes = new Node().ExpandDeserializedNodes(tempList);
                }
                else
                {
                    //else create new
                    if (IsStory)
                    {
                        DissectStory(JsonConvert.DeserializeObject<MainStory>(fileString));
                    }
                    else
                    {
                        DissectCharacter(JsonConvert.DeserializeObject<CharacterStory>(fileString));
                    }

                    //save nodes
                    File.WriteAllText(savedNodesPath, JsonConvert.SerializeObject(Nodes.ConvertAll(n => (SerializeableNode)n)));
                }

                return Nodes.Count > 0;
            }
            else
            {
                return false;
            }
        }

        private List<Node> CalculateForceDirectedLayout(List<Node> nodes)
        {
            //You just need to think about the 3 separate forces acting on each node,
            //add them together each "frame" to get the movement of each node.

            //Gravity, put a simple force acting towards the centre of the canvas so the nodes dont launch themselves out of frame

            //Node-Node replusion, You can either use coulombs force(which describes particle-particle repulsion)
            //or use the gravitational attraction equation and just reverse it

            //Connection Forces, this ones a little tricky, define a connection as 2 nodes and the distance between them.

            const float maxForce = 0;
            const int iterations = 100;
            float attraction = 750f;//attraction force multiplier, between 0 and much
            float cooldown = 0.97f;
            float currentMaxForce = maxForce + 0.1f;
            float repulsion = 1000f;//repulsion force multiplier, between 0 and much
            int length = 180; //spring length in units aka thedistance an edge should be long

            //copy node ids in a dict so we can apply force and not disturb the rest
            //save all forces here
            NodeForces = new Dictionary<Guid, Vector2>();
            nodes.ForEach(n => NodeForces.Add(n.Guid, new Vector2()));

            //times to perform calculation, result gets bettor over time
            int i = 0;
            while (i < iterations && currentMaxForce > maxForce)
            {
                //calculate new, smaller cooldown so the nodes will move less and less
                cooldown *= cooldown;
                //initialize maximum force/reset it
                currentMaxForce = maxForce + 0.1f;

                //go through all nodes and apply the forces
                foreach (Node node in nodes)
                {
                    //create position vector for all later calculations
                    Vector2 nodePosition = new Vector2(node.Position.X, node.Position.Y);
                    //reset force
                    NodeForces[node.Guid] = new Vector2();

                    //calculate repulsion force
                    foreach (Node secondNode in nodes)
                    {
                        if (node != secondNode && secondNode.Position != node.Position)
                        {
                            Vector2 secondNodePosition = new Vector2(secondNode.Position.X, secondNode.Position.Y);
                            Vector2 difference = (secondNodePosition - nodePosition);
                            //absolute length of difference/distance
                            float distance = difference.Length();
                            //add force like this: f_rep = c_rep / (distance^2) * vec(p_u->p_v)
                            Vector2 repulsionForce = repulsion / (distance * distance) * (difference / distance) / node.Mass;

                            //if the second node is a child of ours
                            if (secondNode.ParentNodes.Contains(node))
                            {
                                //so we can now do the attraction force
                                //formula: c_spring * log(distance / length) * vec(p_u->p_v) - f_rep
                                NodeForces[node.Guid] += (attraction * (float)Math.Log(distance / length) * (difference / distance)) - repulsionForce;
                            }
                            else
                            {
                                //add force to node
                                NodeForces[node.Guid] += repulsionForce;
                            }

                            //add new maximum force or keep it as is if ours is smaller
                            currentMaxForce = Math.Max(currentMaxForce, NodeForces[node.Guid].Length());
                        }
                    }
                }

                //apply force to nodes
                foreach (Node node in nodes)
                {
                    node.Position.X += (int)(cooldown * NodeForces[node.Guid].X);
                    node.Position.Y += (int)(cooldown * NodeForces[node.Guid].Y);
                }

                i++;
            }
            return nodes;
        }

        private List<Node> CalculateStartingPositions(List<Node> nodes)
        {
            int step = 40;
            int runningTotal = 0;
            //~sidelength of the most square layout we can achieve witrh the number of nodes we have
            int sideLength = (int)(Math.Sqrt(nodes.Count) + 0.5);
            foreach (Node node in nodes)
            {
                //modulo of running total with sidelength gives x coords, repeating after sidelength
                //offset by halfe sidelength to center x
                int x = (runningTotal % sideLength) - sideLength / 2 + Random.Next(-(step / 2) + 1, (step / 2) - 1);
                //running total divided by sidelength gives y coords,
                //increments after runningtotal increments sidelength times
                //offset by halfe sidelength to center y
                int y = (runningTotal / sideLength) - sideLength / 2 + Random.Next(-(step / 2) + 1, (step / 2) - 1);
                //set position
                node.SetPosition(new Point(x * step, y * step));
                //increase running total
                runningTotal++;
            }
            return nodes;
        }

        private List<Node> CombineNodes(List<Node> nodes)
        {
            //temporary list so we dont manipulate the list we read from in the for loops
            Dictionary<Guid, Node> tempNodes = new Dictionary<Guid, Node>();

            //go through all given root nodes (considered root as this stage)
            foreach (Node node in nodes)
            {
                if (!node.Visited)//if we have not yet seen this node, we can add it to the final list
                {
                    //set recusrion end conditional so we dont run forever
                    node.Visited = true;
                    //calculate mass for later use
                    node.CalculateMass();
                    //add it to the final list
                    if (!tempNodes.ContainsKey(node.Guid)) tempNodes.Add(node.Guid, node);
                }
                //call method again on all parents if they have not yet been added to the list
                if (node.ParentNodes.Count > 0 && !node.ParentsVisited)
                {
                    //set visited to true so we dont end up in infitiy
                    node.ParentsVisited = true;
                    //get combined parent nodes recursively
                    foreach (Node tempNode in CombineNodes(node.ParentNodes))
                    {
                        if (!tempNodes.ContainsKey(tempNode.Guid)) tempNodes.Add(tempNode.Guid, tempNode);
                    }
                }
                //as long as there are children we can go further,
                //and if we have not yet visited them
                if (node.ChildNodes.Count > 0 && !node.ChildsVisited)
                {
                    //basically the same as all parent nodes,
                    //we set them visited before we go into the recursion,
                    //else we end up in an infinite loop easily
                    node.ChildsVisited = true;
                    //get combined children nodes recuirsively
                    foreach (Node tempNode in CombineNodes(node.ChildNodes))
                    {
                        if (!tempNodes.ContainsKey(tempNode.Guid)) tempNodes.Add(tempNode.Guid, tempNode);
                    }
                }

                //actually adding the node
                if (node.Type == NodeType.Criterion)//if it is a criterion, else is after this bit
                {
                    //if the criterion has already been seen before
                    Node criterionInFile = CriteriaInFile.Find(n => n.Guid == node.Guid);
                    if (criterionInFile != null)//has been seen before
                    {
                        //add the childs and parents of this instance of the criterion to the first instance of this criterion
                        //aka fusion
                        criterionInFile.ChildNodes.AddRange(node.ChildNodes);
                        criterionInFile.ParentNodes.AddRange(node.ParentNodes);
                        //recalculate mass for later use
                        criterionInFile.CalculateMass();
                    }
                    else//node not yet visited, add to list of criteria
                    {
                        //add it to the list of criteria so we can fuse all other instances of this criterion
                        CriteriaInFile.Add(node);
                        //add it to the list of all nodes because it is not on there yet.
                        if (!tempNodes.ContainsKey(node.Guid)) tempNodes.Add(node.Guid, node);
                    }
                }
            }

            //return final list of all nodes in the story
            List<Node> listNodes = new List<Node>();
            foreach (KeyValuePair<Guid, Node> pair in tempNodes)
            {
                listNodes.Add(pair.Value);
            }
            return listNodes;
        }

        private void DissectCharacter(CharacterStory story)
        {
            if (story != null && !GotCancelled)
            {
                CriteriaInFile = new List<Node>();

                //get all relevant items from the json
                Nodes.AddRange(GetDialogues(story));
                Nodes.AddRange(GetGlobalGoodByeResponses(story));
                Nodes.AddRange(GetGlobalResponses(story));
                Nodes.AddRange(GetBackGroundChatter(story));
                Nodes.AddRange(GetQuests(story));
                Nodes.AddRange(GetReactions(story));

                //remove duplicates/merge criteria
                //maybe later we load the corresponding strings from the character files and vise versa?
                Nodes = CombineNodes(Nodes);

                //clear criteria to free memory, we dont need them anyways
                //cant be called recusrively so we cant add it, it would break the combination
                CriteriaInFile.Clear();

                //calculate starting positions
                Nodes = CalculateStartingPositions(Nodes);

                //render and do the force driven calculation thingies
                Nodes = CalculateForceDirectedLayout(Nodes);

            }
        }

        private void DissectStory(MainStory story)
        {
            if (story != null && !GotCancelled)
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

                //clear criteria to free memory, we dont need them anyways
                //cant be called recusrively so we cant add it, it would break the combination
                CriteriaInFile.Clear();

                //calculate starting positions
                Nodes = CalculateStartingPositions(Nodes);

                //render and do the force driven calculation thingies
                Nodes = CalculateForceDirectedLayout(Nodes);

            }
        }

        private List<Node> GetAchievements(MainStory story)
        {
            //list to collect all achievement nodes
            List<Node> nodes = new List<Node>();
            //go through all of them
            foreach (Achievement achievement in story.Achievements)
            {
                //node to add the description as child to, needs reference to parent, hence can't be anonymous
                Node node = new Node(achievement.Id, NodeType.Achievement, achievement.Name);
                node.AddChildNode(new Node(achievement.Id + "Description", NodeType.Achievement, achievement.Description, node));
                //add achievement with description child to list
                nodes.Add(node);
            }

            //return list of achievements
            return nodes;
        }

        private List<Node> GetBackGroundChatter(CharacterStory story)
        {
            List<Node> nodes = new List<Node>();

            foreach (BackgroundChatter backgroundChatter in story.BackgroundChatter)
            {
                Node bgcNode = new Node($"BGC{backgroundChatter.Id}", NodeType.BGC, backgroundChatter.Text);

                //criteria
                bgcNode.AddCriteria(backgroundChatter.Critera);

                //startevents
                bgcNode.AddEvents(backgroundChatter.StartEvents);

                //responses
                foreach (Response response in backgroundChatter.Responses)
                {
                    Node nodeResponse = new Node($"{response.CharacterName}{response.ChatterId}", NodeType.Response, "see id", bgcNode);

                    bgcNode.AddChildNode(nodeResponse);
                }
            }

            return nodes;
        }

        private List<Node> GetDialogues(CharacterStory story)
        {
            List<Node> nodes = new List<Node>();
            List<Tuple<Node, int>> responseDialogueLinks = new List<Tuple<Node, int>>();

            foreach (Dialogue dialogue in story.Dialogues)
            {
                Node nodeDialogue = new Node(dialogue.ID.ToString(), NodeType.Dialogue, dialogue.Text);
                int alternateTextCounter = 1;

                //add all alternate texts to teh dialogue
                foreach (AlternateText alternateText in dialogue.AlternateTexts)
                {
                    Node nodeAlternateText = new Node($"{dialogue.ID}.{alternateTextCounter}", NodeType.Dialogue, alternateText.Text, nodeDialogue);

                    //increasse counter to ensure valid id
                    alternateTextCounter++;

                    nodeAlternateText.AddCriteria(alternateText.Critera);

                    //add alternate to the default text as a child, parent already set on the child
                    nodeDialogue.AddChildNode(nodeAlternateText);
                }

                //some events in here may have strings that are connected to the dialogue closing
                nodeDialogue.AddEvents(dialogue.CloseEvents);

                //add all responses as childs to this dialogue
                foreach (Response response in dialogue.Responses)
                {
                    Node nodeResponse = new Node(response.Id, NodeType.Response, response.Text, nodeDialogue);

                    nodeResponse.AddCriteria(response.ResponseCriteria);

                    //check all responses to this dialogue
                    nodeResponse.AddEvents(response.ResponseEvents);

                    if (response.Next != 0)
                    {
                        responseDialogueLinks.Add(new Tuple<Node, int>(nodeResponse, response.Next));
                    }

                    nodeDialogue.AddChildNode(nodeResponse);
                }

                //add the starting events
                nodeDialogue.AddEvents(dialogue.StartEvents);

                //finally add node
                nodes.Add(nodeDialogue);
            }

            //link up the dialogues and responses/next dialogues
            foreach (Tuple<Node, int> next in responseDialogueLinks)
            {
                Node node = nodes.Find(n => n.ID == next.Item2.ToString());

                if (node != null)
                {
                    node.AddParentNode(next.Item1);
                }
            }

            responseDialogueLinks.Clear();

            return nodes;
        }

        private List<Node> GetGlobalGoodByeResponses(CharacterStory story)
        {
            List<Node> nodes = new List<Node>();

            //add all responses as childs to this dialogue
            foreach (GlobalGoodbyeResponse response in story.GlobalGoodbyeResponses)
            {
                Node nodeResponse = new Node(response.Id, NodeType.Response, response.Text);

                nodeResponse.AddCriteria(response.ResponseCriteria);

                //check all responses to this dialogue
                nodeResponse.AddEvents(response.ResponseEvents);

                nodes.Add(nodeResponse);
            }

            return nodes;
        }

        private List<Node> GetGlobalResponses(CharacterStory story)
        {
            List<Node> nodes = new List<Node>();

            foreach (GlobalResponse response in story.GlobalResponses)
            {
                Node nodeResponse = new Node(response.Id, NodeType.Response, response.Text);

                nodeResponse.AddCriteria(response.ResponseCriteria);

                //check all responses to this dialogue
                nodeResponse.AddEvents(response.ResponseEvents);

                nodes.Add(nodeResponse);
            }

            return nodes;
        }

        private List<Node> GetItemGroups(MainStory story)
        {
            //list to collect all item group nodes in the end
            List<Node> nodes = new List<Node>();
            //go through all item groups to find events
            foreach (ItemGroupBehavior itemGroupBehaviour in story.ItemGroupBehaviors)
            {
                //create item group node to add events/criteria to
                Node nodeGroup = new Node(itemGroupBehaviour.Id, NodeType.ItemGroup, itemGroupBehaviour.Name);
                //get actions for item
                foreach (ItemAction itemAction in itemGroupBehaviour.ItemActions)
                {
                    //node to addevents to
                    Node nodeAction = new Node(itemAction.ActionName, NodeType.Action, itemAction.ActionName, nodeGroup);

                    //add text that is shown when item is taken
                    nodeAction.AddEvents(itemAction.OnTakeActionEvents);

                    //add criteria that influence this item
                    nodeAction.AddCriteria(itemAction.Criteria);

                    //add action to item
                    nodeGroup.AddChildNode(nodeAction);
                }

                //add item gruop with everything to collecting list
                nodes.Add(nodeGroup);
            }

            //return list of all item groups
            return nodes;
        }

        private List<Node> GetItemOverrides(MainStory story)
        {
            //list to store all found root nodes
            List<Node> nodes = new List<Node>();
            //go through all nodes to search them for actions
            foreach (ItemOverride itemOverride in story.ItemOverrides)
            {
                //add items to list
                Node nodeItem = new Node(itemOverride.Id, NodeType.Item, itemOverride.DisplayName);
                //get actions for item
                foreach (ItemAction itemAction in itemOverride.ItemActions)
                {
                    //create action node to add criteria and events to
                    Node nodeAction = new Node(itemAction.ActionName, NodeType.Action, itemAction.ActionName, nodeItem);

                    //add text that is shown when item is taken
                    nodeAction.AddEvents(itemAction.OnTakeActionEvents);

                    //add criteria that influence this item
                    nodeAction.AddCriteria(itemAction.Criteria);

                    //add action to item
                    nodeItem.AddChildNode(nodeAction);
                }

                //add item with all child nodes to collector list
                nodes.Add(nodeItem);
            }

            //duh
            return nodes;
        }

        private List<Node> GetPlayerReactions(MainStory story)
        {
            List<Node> nodes = new List<Node>();
            foreach (PlayerReaction playerReaction in story.PlayerReactions)
            {
                //add items to list
                Node nodeReaction = new Node(playerReaction.Id, NodeType.Reaction, playerReaction.Name);

                //get actions for item
                nodeReaction.AddEvents(playerReaction.Events);

                nodeReaction.AddCriteria(playerReaction.Critera);

                nodes.Add(nodeReaction);
            }

            return nodes;
        }

        private List<Node> GetQuests(CharacterStory story)
        {
            List<Node> nodes = new List<Node>();

            foreach (Quest quest in story.Quests)
            {
                Node nodeQuest = new Node(quest.ID, NodeType.Quest, quest.Name);

                //Add details
                if (quest.Details.Length > 0) nodeQuest.AddChildNode(new Node($"{quest.ID}Description", NodeType.Quest, quest.Details));
                //Add completed details
                if (quest.CompletedDetails.Length > 0) nodeQuest.AddChildNode(new Node($"{quest.ID}CompletedDetails", NodeType.Quest, quest.CompletedDetails));
                //Add failed details
                if (quest.FailedDetails.Length > 0) nodeQuest.AddChildNode(new Node($"{quest.ID}FailedDetails", NodeType.Quest, quest.FailedDetails));

                //Add extended details

                foreach (ExtendedDetail detail in quest.ExtendedDetails)
                {
                    nodeQuest.AddChildNode(new Node($"{quest.ID}Description{detail.Value}", NodeType.Quest, detail.Details));
                }

                nodes.Add(nodeQuest);
            }

            return nodes;
        }

        private List<Node> GetReactions(CharacterStory story)
        {
            List<Node> nodes = new List<Node>();

            foreach (Reaction playerReaction in story.Reactions)
            {
                //add items to list
                Node nodeReaction = new Node(playerReaction.Id, NodeType.Reaction, playerReaction.Name);
                //get actions for item
                nodeReaction.AddEvents(playerReaction.Events);

                nodeReaction.AddCriteria(playerReaction.Critera);

                nodes.Add(nodeReaction);
            }

            return nodes;
        }
    }
}