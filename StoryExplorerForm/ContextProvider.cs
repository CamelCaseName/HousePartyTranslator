using HousePartyTranslator.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Numerics;

namespace HousePartyTranslator
{
    internal class ContextProvider
    {
        private Dictionary<Guid, Vector2> NodeForces;
        private List<Node> CriteriaInFile;
        private List<Node> Nodes;
        private readonly bool IsStory;
        private string _StoryFilePath;
        public bool GotCancelled = false;
        private Random Random = new Random();

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
                        GotCancelled = true;
                    }
                }
            }
        }

        public ContextProvider(string FilePath, bool IsStory, bool AutoSelectFile)
        {
            Nodes = new List<Node>();
            this.IsStory = IsStory;
            if (Properties.Settings.Default.story_path != "" && AutoSelectFile)
            {
                this.FilePath = Properties.Settings.Default.story_path;
            }
            else
            {
                this.FilePath = FilePath;
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

            //clear criteria to free memory, we dont need them anyways
            //cant be called recusrively so we cant add it, it would break the combination
            CriteriaInFile.Clear();

            //calculate starting positions
            Nodes = CalculateStartingPositions(Nodes);

            //render and do the force driven calculation thingies
            Nodes = CalculateForceDirectedLayout(Nodes);
        }

        private List<Node> CalculateForceDirectedLayout(List<Node> nodes)
        {
            //It's fairly easy to code, you just need to think about the 3 separate forces acting on each node,
            //add them together and divide that by the mass of the node to get the movement of each node.

            //Gravity, put a simple force acting towards the centre of the canvas so the nodes dont launch themselves out of frame
            // f = r

            //Node-Node replusion, You can either use coulombs force(which describes particle-particle repulsion)
            //or use the gravitational attraction equation and just reverse it
            //f = m1*m2/r^2 where r is the distance 
            //masses are the amount of edges that are connected

            //Connection Forces, this ones a little tricky, define a connection as 2 nodes and the distance between them.
            //when the actual distance between them is different from the defined distance,
            //add a force in the direction of the connection multiplied by the difference between the defined and the actual distance
            //f = 1* actual - desired, direction is towards child

            //distance an edge should be long, given in units
            const int iterations = 80;
            const float maxForce = 0;
            float attraction = 500f;//attraction force multiplier, between 0 and much
            float cooldown = 0.95f;
            //float gravityMultiplier = 0f; //between 0 and 1
            float repulsion = 300f;//repulsion force multiplier, between 0 and much
            int length = 200; //spring length in units
            float currentMaxForce = maxForce + 0.1f;

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
                            Vector2 repulsionForce = repulsion / (distance * distance) * (difference / distance);

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
            //list to collect all achievement nodes
            List<Node> nodes = new List<Node>();
            //go through all of them
            foreach (Achievement achievement in story.Achievements)
            {
                //node to add the description as child to, needs reference to parent, hence can't be anonymous
                Node node = new Node(achievement.Id, NodeType.Achievement, 1, achievement.Name);
                node.AddChildNode(new Node(achievement.Id + "Description", NodeType.Achievement, 1, achievement.Description, node));
                //add achievement with description child to list
                nodes.Add(node);
            }

            //return list of achievements
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
                Node currentGroup = new Node(itemGroupBehaviour.Id, NodeType.ItemGroup, 1, itemGroupBehaviour.Name);
                //get actions for item
                foreach (ItemAction itemAction in itemGroupBehaviour.ItemActions)
                {
                    //node to addevents to
                    Node currentAction = new Node(itemAction.ActionName, NodeType.Action, 1, itemAction.ActionName, currentGroup);

                    //add text that is shown when item is taken
                    foreach (OnTakeActionEvent onTakeActionEvent in itemAction.OnTakeActionEvents)
                    {
                        //node to add criteria to the event
                        Node currentEvent = new Node(onTakeActionEvent.Id, NodeType.Event, 1, onTakeActionEvent.Value, currentAction);

                        //add criteria that influence this item
                        foreach (Criterion criterion in onTakeActionEvent.Criteria)
                        {
                            //only if state dialogue comparison
                            if (criterion.CompareType == "State")
                            {
                                //add criterion to event
                                currentEvent.AddParentNode(CreateCriteriaNode(criterion, currentEvent));
                            }
                        }

                        //add event to action, can be executed when item is taken
                        currentAction.AddChildNode(currentEvent);
                    }

                    //add criteria that influence this item
                    foreach (Criterion criterion in itemAction.Criteria)
                    {
                        //only if state dialogue comparison
                        if (criterion.CompareType == "State")
                        {
                            //add criterion to action
                            currentAction.AddParentNode(CreateCriteriaNode(criterion, currentAction));
                        }
                    }

                    //add action to item
                    currentGroup.AddChildNode(currentAction);
                }

                //add item gruop with everything to collecting list
                nodes.Add(currentGroup);
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
                Node currentItem = new Node(itemOverride.Id, NodeType.Item, 1, itemOverride.DisplayName);
                //get actions for item
                foreach (ItemAction itemAction in itemOverride.ItemActions)
                {
                    //create action node to add criteria and events to
                    Node currentAction = new Node(itemAction.ActionName, NodeType.Action, 1, itemAction.ActionName, currentItem);

                    //add text that is shown when item is taken
                    foreach (OnTakeActionEvent onTakeActionEvent in itemAction.OnTakeActionEvents)
                    {
                        //only if correct type
                        if (onTakeActionEvent.EventType == 10)
                        {
                            //create event node to add criteria to
                            Node currentEvent = new Node(onTakeActionEvent.Id, NodeType.Event, 1, onTakeActionEvent.Value, currentAction);

                            //add criteria that influence this item
                            foreach (Criterion criterion in onTakeActionEvent.Criteria)
                            {
                                //only if state dialogue comparison
                                if (criterion.CompareType == "State")
                                {
                                    //add to event as criterion
                                    currentEvent.AddParentNode(CreateCriteriaNode(criterion, currentEvent));
                                }
                            }

                            //add event to action as child
                            currentAction.AddChildNode(currentEvent);
                        }
                    }

                    //add criteria that influence this item
                    foreach (Criterion criterion in itemAction.Criteria)
                    {
                        //only if state dialogue comparison, dont care about game vars
                        if (criterion.CompareType == "State")
                        {
                            //add to action as criterion
                            currentAction.AddParentNode(CreateCriteriaNode(criterion, currentAction));
                        }
                    }

                    //add action to item
                    currentItem.AddChildNode(currentAction);
                }

                //add item with all child nodes to collector list
                nodes.Add(currentItem);
            }

            //duh
            return nodes;
        }

        private Node CreateCriteriaNode(Criterion criterion, Node node)
        {
            //create all criteria nodes the same way so they can possibly be replaced by the actual text later
            return new Node($"{criterion.Character}{criterion.Value}", NodeType.Criterion, 1, $"{criterion.DialogueStatus}: {criterion.Character} - {criterion.Value}", new List<Node>(), new List<Node>() { node });
        }
    }
}