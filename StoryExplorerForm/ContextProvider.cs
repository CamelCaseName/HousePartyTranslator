using HousePartyTranslator.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    internal class ContextProvider
    {
        private string _StoryFilePath;
        private readonly bool IsStory;
        private List<Node> Nodes;
        private List<Node> CriteriaInFile;
        public bool GotCancelled = false;

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
            Nodes = CalculateForceDirectedLayout(Nodes, 100);
        }

        private List<Node> CalculateForceDirectedLayout(List<Node> nodes, int iterations)
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
            int desiredDistance = 20;
            float gravitationMultiplier = 0.001f;
            float connectionMultiplier = 0.01f;
            float spacialMultiplier = 0.001f;

            //random to decide what to do with overlapping nodes
            Random random = new Random();
            //times to perform calculation, result gets bettor over time
            for (int i = 0; i < iterations; i++)
            {
                //loop through asll ndoes to calculate their coords after one iteration
                foreach (Node node in nodes)
                {
                    //the accelleration a node will perceive in units/iteration. aka offset per iteration
                    float accelerationX = 0;
                    float accelerationY = 0;

                    //pseudogravitational force to keep nodes from shooting off into the unknown (distance to 0 times constant)
                    accelerationX -= node.Position.X * gravitationMultiplier;
                    accelerationY -= node.Position.Y * gravitationMultiplier;

                    //spacial force around each node, so they don't clump together/over each other
                    //calculate distance for each node, then apply a force depending on that distance
                    //dependant on mass to make heavier nodes move less when smaller ones come by
                    foreach (Node nodeForSpacialForce in nodes)
                    {
                        //we do not want to calculate the distance to ourself, duh
                        if (node != nodeForSpacialForce)
                        {
                            long distanceX = nodeForSpacialForce.Position.X - node.Position.X;
                            long distanceY = nodeForSpacialForce.Position.Y - node.Position.Y;
                            //calculate x accelleration if it is not 0 distance
                            if (distanceX > 0.1d || distanceX < -0.1d)
                            {
                                //f = mass times mass divided by r^2, just like coloumb force. 
                                //m1*m2 / r^2 * s / m1 = m2 / r^2 * s
                                //where s is our multiplier (for tuning) and m1 and m2 are the massess of our nodes
                                //f = m*a -> a = f/m
                                accelerationX += nodeForSpacialForce.Mass / (distanceX * distanceX * Math.Sign(distanceX) * spacialMultiplier);
                            }
                            else
                            {
                                //if nodes overlap, we move them apart by a bit
                                accelerationX -= random.Next(-10, 10);
                            }
                            //same 0 check for y acceleration
                            if (distanceY > 0.1d || distanceY < -0.1d)
                            {
                                //same calculations we did for x part of the coordinate, see above
                                accelerationY += nodeForSpacialForce.Mass / (distanceY * distanceY * Math.Sign(distanceY) * spacialMultiplier);
                            }
                            else
                            {
                                //if nodes overlap, we move them apart by a bit
                                accelerationY -= random.Next(-10, 10);
                            }
                        }
                    }

                    //connection force
                    //draws nodes together along their edges if they are too far away apart or pushes them away if too close
                    // calculated for each node we have (parents and childs)
                    foreach (Node child in Enumerable.Concat(node.ChildNodes, node.ParentNodes))
                    {
                        long distanceX = child.Position.X - node.Position.X;
                        long distanceY = child.Position.Y - node.Position.Y;
                        //add difference in position times a tuning multiplier
                        //if we are too close:
                        if (desiredDistance > Math.Sqrt(distanceX * distanceX + distanceY * distanceY))
                        {
                            accelerationX -= distanceX * connectionMultiplier;
                            accelerationY -= distanceY * connectionMultiplier;
                        }
                        else //we are too far apart
                        {
                            accelerationX += distanceX * connectionMultiplier;
                            accelerationY += distanceY * connectionMultiplier;
                        }
                    }

                    //actually move the node by offsetting the position by the given acceleration each iteration
                    node.Position.X += (int)accelerationX;
                    node.Position.Y += (int)accelerationY;
                }
            }
            return nodes;
        }

        private List<Node> CalculateStartingPositions(List<Node> nodes)
        {
            int step = 100;
            int runningTotal = 0;
            //~sidelength of the most square layout we can achieve witrh the number of nodes we have
            int sideLength = (int)(Math.Sqrt(nodes.Count) + 0.5);
            foreach (Node node in nodes)
            {
                //modulo of running total with sidelength gives x coords, repeating after sidelength
                //offset by halfe sidelength to center x
                int x = (runningTotal % sideLength) - sideLength / 2;
                //running total divided by sidelength gives y coords,
                //increments after runningtotal increments sidelength times
                //offset by halfe sidelength to center y
                int y = (runningTotal / sideLength) - sideLength / 2;
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
            List<Node> tempNodes = new List<Node>();

            //go through all given root nodes (considered root as this stage)
            foreach (Node node in nodes)
            {
                //call method again on all parents if they have not yet been added to the list
                if (node.ParentNodes.Count > 0 && !node.ParentsVisited)
                {
                    //set visited to true so we dont end up in infitiy
                    node.ParentsVisited = true;
                    //get combined parent nodes recursively
                    tempNodes.AddRange(CombineNodes(node.ParentNodes));
                }
                //as long as there are children we can go further,
                //and if we have not yet visited them
                else if (node.ChildNodes.Count > 0 && !node.ChildsVisited)
                {
                    //basically the same as all parent nodes,
                    //we set them visited before we go into the recursion,
                    //else we end up in an infinite loop easily
                    node.ChildsVisited = true;
                    //get combined children nodes recuirsively
                    tempNodes.AddRange(CombineNodes(node.ChildNodes));
                }

                //actually adding the node
                if (node.Type == NodeType.Criterion)//if it is a criterion, else is after this bit
                {
                    //if the criterion has already been seen before
                    Node criterionInFile = CriteriaInFile.Find(n => n.ID == node.ID);
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
                        tempNodes.Add(node);
                    }
                }
                else if (!node.Visited)//if we have not yet seen this node, we can add it to the final list
                {
                    //set recusrion end conditional so we dont run forever
                    node.Visited = true;
                    //calculate mass for later use
                    node.CalculateMass();
                    //add it to the final list
                    tempNodes.Add(node);
                }

            }

            //return final list of all nodes in the story
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
                                currentAction.AddParentNode(CreateCriteriaNode(criterion, currentEvent));
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
                                    currentAction.AddParentNode(CreateCriteriaNode(criterion, currentEvent));
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