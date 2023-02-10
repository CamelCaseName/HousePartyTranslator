using Newtonsoft.Json;
using System.Runtime.Versioning;
using Translator.Core;
using Translator.Explorer.JSON;
using static Translator.Explorer.JSON.StoryEnums;
using DataBase = Translator.Core.DataBase<Translator.InterfaceImpls.WinLineItem, Translator.InterfaceImpls.WinUIHandler, Translator.InterfaceImpls.WinTabController, Translator.InterfaceImpls.WinTab>;
using Settings = Translator.InterfaceImpls.WinSettings;

namespace Translator.Explorer
{
    [SupportedOSPlatform("Windows")]
    internal sealed class ContextProvider
    {
        private NodeList CriteriaInFile = new();
        private readonly NodeProvider provider;
        private readonly bool IsStory;
        private readonly Random Random = new();
        public readonly string FileName = "social";
        public readonly string StoryName = "story";
        private string _StoryFilePath = string.Empty;
        private string NodeFilePath = string.Empty;
        public bool GotCancelled = false;
        private readonly bool AutoFileSelection = false;
        private readonly NodeList Values = new();
        private readonly NodeList Doors = new();
        private readonly List<PointF> oldPositions = new();

        public ContextProvider(NodeProvider provider, bool IsStory, bool AutoSelectFile, string FileName, string StoryName)
        {
            this.provider = provider;
            _StoryFilePath = string.Empty;
            this.IsStory = IsStory;
            //set autoselect only if valid parameters are supplied
            AutoFileSelection = AutoSelectFile && FileName != string.Empty && StoryName != string.Empty;
            if (AutoFileSelection)
            {
                this.FileName = FileName;
                this.StoryName = StoryName;
            }

            if (Settings.WDefault.StoryPath != string.Empty && AutoFileSelection)
            {
                string storyPathMinusStory = Directory.GetParent(Settings.WDefault.StoryPath)?.FullName ?? string.Empty;

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
                FilePath = string.Empty;
            }

            if (FilePath.Length > 0)
            {
                this.FileName = Path.GetFileNameWithoutExtension(FilePath);
                this.StoryName = Directory.GetParent(FilePath)?.Name ?? string.Empty;
            }
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
                            Title = $"Choose the story file ({StoryName}) for the templates",
                            Filter = AutoFileSelection ? "Story Files (*.story)|*.story" : string.Empty,
                            InitialDirectory = Settings.WDefault.StoryPath.Length > 0 ? Settings.WDefault.StoryPath : @"C:\Users\%USER%\Documents",
                            RestoreDirectory = false,
                            FileName = this.FileName + ".story"
                        };
                    }
                    else//social file
                    {
                        selectFileDialog = new OpenFileDialog
                        {
                            Title = $"Choose the social file ({FileName}) for the templates",
                            Filter = AutoFileSelection ? "Social Files (*.character)|*.character" : string.Empty,
                            InitialDirectory = Settings.WDefault.StoryPath.Length > 0 ? Settings.WDefault.StoryPath : @"C:\Users\%USER%\Documents",
                            RestoreDirectory = false,
                            FileName = this.FileName + ".character"
                        };
                    }

                    if (selectFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Settings.WDefault.StoryPath = Path.GetDirectoryName(selectFileDialog.FileName) ?? string.Empty;
                        Settings.Default.Save();
                        _StoryFilePath = selectFileDialog.FileName;
                    }
                    else
                    {
                        _StoryFilePath = string.Empty;
                        //close form if cancelled
                        GotCancelled = true;
                    }
                }
            }
        }

        public NodeList Nodes => provider.Nodes;

        public bool ParseFile()
        {
            if (File.Exists(FilePath))
            {
                NodeFilePath = Path.Combine(LogManager.CFGFOLDER_PATH, $"{StoryName + FileName + DataBase.DBVersion}.json");

                //save path
                Settings.WDefault.StoryPath = Path.GetDirectoryName(FilePath) ?? string.Empty;
                //try to laod the saved nodes
                if (File.Exists(NodeFilePath))
                {
                    ReadInOldPositions(NodeFilePath);
                }

                //read in nodes and set positions later, is faster than the old system
                string fileString = File.ReadAllText(FilePath);
                //else create new
                if (Path.GetExtension(FilePath) == ".story")
                {
                    while (Nodes.Count != 0) Nodes.Clear();
                    Nodes.AddRange(DissectStory(JsonConvert.DeserializeObject<MainStory>(fileString) ?? new MainStory()));
                }
                else
                {
                    while (Nodes.Count != 0) Nodes.Clear();
                    Nodes.AddRange(DissectCharacter(JsonConvert.DeserializeObject<CharacterStory>(fileString) ?? new CharacterStory()));
                }

                SetStartingPositions(Nodes);

                //save nodes
                return SaveNodes(Nodes, NodeFilePath);
            }
            else
            {
                return false;
            }
        }

        public bool ParseAllFiles()
        {
            string StoryFolderPath = string.Empty;
            if (FileName != string.Empty && StoryName != string.Empty)
            {
                if (FilePath != string.Empty)
                    StoryFolderPath = Path.GetDirectoryName(FilePath) ?? string.Empty;
                else if (Settings.WDefault.StoryPath != string.Empty)
                    StoryFolderPath = Settings.WDefault.StoryPath;
            }
            else
            {
                var folderBrowser = new FolderBrowserDialog()
                {
                    RootFolder = Environment.SpecialFolder.MyDocuments,
                    Description = "Please select the folder with the story files in it.",
                    ShowHiddenFiles = true,
                    UseDescriptionForTitle = true,
                };

                if (folderBrowser.ShowDialog() == DialogResult.OK)
                    StoryFolderPath = folderBrowser.SelectedPath;
            }

            if (!Directory.Exists(StoryFolderPath)) return false;

            NodeFilePath = Path.Combine(LogManager.CFGFOLDER_PATH, $"{StoryName + DataBase.DBVersion}.json");

            //try to load the saved nodes
            if (File.Exists(NodeFilePath))
            {
                ReadInOldPositions(NodeFilePath);
            }

            if (Directory.GetFiles(StoryFolderPath).Length > 0 && (StoryFolderPath.Split("\\")[^1] == StoryName) || !AutoFileSelection)
            {
                //else create new
                foreach (string item in Directory.GetFiles(StoryFolderPath))
                {
                    if (Path.GetExtension(item) == ".story")
                    {
                        Nodes.AddRange(
                            DissectStory(
                                JsonConvert.DeserializeObject<MainStory>(
                                    File.ReadAllText(item)) ?? new(),
                                Path.GetFileNameWithoutExtension(item)));
                    }

                    else
                        Nodes.AddRange(
                            DissectCharacter(
                                JsonConvert.DeserializeObject<CharacterStory>(
                                    File.ReadAllText(item)) ?? new()));
                }
                //read in all first, dumbass me
                InterlinkNodes(Nodes);

                SetStartingPositions(Nodes);

                //save nodes
                return SaveNodes(Nodes, NodeFilePath);
            }
            return Nodes.Count > 0;
        }

        private void ReadInOldPositions(string nodeFilePath)
        {
            oldPositions.Clear();
            var list = JsonConvert.DeserializeObject<List<PointF>>(File.ReadAllText(nodeFilePath));
            if (list == null) return;
            oldPositions.AddRange(list);
        }

        public bool SaveNodes(NodeList nodes, string path = "")
        {
            if (path == string.Empty) path = NodeFilePath;
            if (nodes.Count > 0)
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(nodes.GetPositions()));
                return true;
            }
            else return false;
        }

        public NodeList GetTemplateNodes()
        {
            if (FilePath.Length > 0)
            {
                string fileString = File.ReadAllText(FilePath);
                fileString = fileString[fileString.IndexOf('{')..];
                //else create new
                if (IsStory)
                {
                    while (Nodes.Count != 0) Nodes.Clear();
                    Nodes.AddRange(DissectStory(JsonConvert.DeserializeObject<MainStory>(fileString) ?? new MainStory()));
                }
                else
                {
                    while (Nodes.Count != 0) Nodes.Clear();
                    Nodes.AddRange(DissectCharacter(JsonConvert.DeserializeObject<CharacterStory>(fileString) ?? new CharacterStory()));
                }

                return Nodes;
            }
            return new();
        }

        private void SetStartingPositions(NodeList nodes)
        {
            if (oldPositions.Count == nodes.Count) nodes.SetPositions(oldPositions);
            else
            {
                int step = 40;
                int runningTotal = 0;
                //~sidelength of the most square layout we can achieve witrh the number of nodes we have
                int sideLength = (int)(Math.Sqrt(nodes.Count) + 0.5);
                for (int i = 0; i < nodes.Count; i++)
                {
                    //modulo of running total with sidelength gives x coords, repeating after sidelength
                    //offset by halfe sidelength to center x
                    int x = (runningTotal % sideLength) - sideLength / 2;
                    //running total divided by sidelength gives y coords,
                    //increments after runningtotal increments sidelength times
                    //offset by halfe sidelength to center y
                    int y = (runningTotal / sideLength) - sideLength / 2;
                    //set position
                    nodes[i].Position = new Point(
                        x * step + Random.Next(-(step / 2) + 1, (step / 2) - 1),
                        y * step + Random.Next(-(step / 2) + 1, (step / 2) - 1)
                        );
                    //increase running total
                    runningTotal++;
                }
            }
        }

        private NodeList ExpandNodes(NodeList nodes)
        {
            //temporary list so we dont manipulate the list we read from in the for loops
            var listNodes = new NodeList();

            //go through all given root nodes (considered root as this stage)
            for (int i = 0; i < nodes.Count; i++)
            {
                Node node = nodes[i];
                if (!node.Visited)//if we have not yet seen this node, we can add it to the final list
                {
                    //set recusrion end conditional so we dont run forever
                    node.Visited = true;
                    //calculate mass for later use
                    node.CalculateMass();
                    //add it to the final list
                    if (!listNodes.Contains(node)) listNodes.Add(node);
                }
                //call method again on all parents if they have not yet been added to the list
                if (node.ParentNodes.Count > 0 && !node.ParentsVisited)
                {
                    //set visited to true so we dont end up in infitiy
                    node.ParentsVisited = true;
                    //get combined parent nodes recursively
                    foreach (Node tempNode in ExpandNodes(node.ParentNodes))
                    {
                        if (!listNodes.Contains(tempNode)) listNodes.Add(tempNode);
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
                    foreach (Node tempNode in ExpandNodes(node.ChildNodes))
                    {
                        if (!listNodes.Contains(tempNode)) listNodes.Add(tempNode);
                    }
                }

                //actually adding the node
                if (node.Type == NodeType.Criterion)//if it is a criterion, else is after this bit
                {
                    //add it to the list of all nodes because it is not on there yet.
                    if (!listNodes.Contains(node)) listNodes.Add(node);
                }
            }
            return listNodes;
        }

        private NodeList DissectCharacter(CharacterStory story)
        {
            NodeList _nodes = new();
            if (story != null && !GotCancelled)
            {
                CriteriaInFile = new NodeList();
                //get all relevant items from the json
                _nodes.AddRange(StoryNodeExtractor.GetItems(story));
                _nodes.AddRange(StoryNodeExtractor.GetValues(story));
                _nodes.AddRange(StoryNodeExtractor.GetPersonality(story));
                _nodes.AddRange(StoryNodeExtractor.GetDialogues(story));
                _nodes.AddRange(StoryNodeExtractor.GetGlobalGoodByeResponses(story));
                _nodes.AddRange(StoryNodeExtractor.GetGlobalResponses(story));
                _nodes.AddRange(StoryNodeExtractor.GetBackGroundChatter(story));
                _nodes.AddRange(StoryNodeExtractor.GetQuests(story));
                _nodes.AddRange(StoryNodeExtractor.GetReactions(story));

                //remove duplicates/merge criteria
                //maybe later we load the corresponding strings from the social files and vise versa?
                _nodes = ExpandNodes(_nodes);

                //clear criteria to free memory, we dont need them anyways
                //cant be called recusrively so we cant add it, it would break the combination
                CriteriaInFile.Clear();

                for (int i = 0; i < _nodes.Count; i++)
                {
                    _nodes[i].FileName = story.CharacterName ?? string.Empty;
                }
            }
            return _nodes;
        }

        private void InterlinkNodes(NodeList nodes)
        {
            DateTime start = DateTime.Now;
            LogManager.Log("\tstarting to link up nodes");
            //lists to save new stuff in
            NodeList Socials = new();
            NodeList States = new();
            NodeList Clothing = new();
            NodeList Poses = new();
            NodeList InventoryItems = new();
            NodeList Properties = new();
            NodeList CompareValuesToCheckAgain = new();

            Node? result;
            //link up different stories and dialogues
            for (int i = 0; i < nodes.Count; i++)
            {
                //link all useful criteria and add influencing values as parents
                if (nodes[i].Type == NodeType.Criterion && nodes[i].Data != null)
                {
                    //node is dialogue so data should contain the criteria itself!
                    Criterion criterion = (Criterion)nodes[i].Data!;
                    switch (criterion.CompareType)
                    {
                        case CompareTypes.Clothing:
                        {
                            result = Clothing.Find((Node n) => n.Type == NodeType.Clothing && n.FileName == criterion.Character && n.ID == criterion.Option + criterion.Value);
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            else
                            {
                                //create and add value node, hasnt been referenced yet
                                var clothing = new Node(criterion.Option + criterion.Value, NodeType.Clothing, criterion.Character + "'s  " + ((Clothes)int.Parse(criterion.Value!)).ToString() + " in set " + (criterion.Option == 0 ? "any" : (criterion.Option - 1).ToString())) { FileName = criterion.Character! };
                                Clothing.Add(clothing);
                                nodes[i].AddParentNode(clothing);
                            }
                            break;
                        }
                        case CompareTypes.CompareValues:
                        {
                            result = Values.Find((Node n) => n.Type == NodeType.Value && n.ID == criterion.Key);
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                            }
                            else
                            {
                                CompareValuesToCheckAgain.Add(nodes[i]);
                            }
                            result = Values.Find((Node n) => n.Type == NodeType.Value && n.ID == criterion.Key2);
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                            }
                            else
                            {
                                CompareValuesToCheckAgain.Add(nodes[i]);
                            }
                            break;
                        }
                        case CompareTypes.CriteriaGroup:
                        {
                            result = nodes.Find((Node n) => n.Type == NodeType.CriteriaGroup && n.ID == criterion.Value);
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            break;
                        }
                        case CompareTypes.Dialogue:
                        {
                            result = nodes.Find((Node n) => n.Type == NodeType.Dialogue && n.FileName == criterion.Character && n.ID == criterion.Value);
                            if (result != null)
                            {
                                //dialogue influences this criteria
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            break;
                        }
                        case CompareTypes.Door:
                        {
                            result = Doors.Find((Node n) => n.Type == NodeType.Door && n.ID == criterion.Key);
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var door = new Node(criterion.Key!, NodeType.Door, criterion.Key!);
                                Doors.Add(door);
                                nodes[i].AddParentNode(door);
                            }
                            break;
                        }
                        case CompareTypes.Item:
                        {
                            result = nodes.Find((Node n) => n.Type == NodeType.Item && n.ID == criterion.Key);
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var item = new Node(criterion.Key!, NodeType.Item, criterion.Key!);
                                nodes.Add(item);
                                nodes[i].AddParentNode(item);
                            }
                            break;
                        }
                        case CompareTypes.IsCurrentlyBeingUsed:
                        {
                            result = nodes.Find((Node n) => n.Type == NodeType.Item && n.ID == criterion.Key);
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var item = new Node(criterion.Key!, NodeType.Item, criterion.Key!);
                                nodes.Add(item);
                                nodes[i].AddParentNode(item);
                            }
                            break;
                        }
                        case CompareTypes.IsCurrentlyUsing:
                        {
                            result = nodes.Find((Node n) => n.Type == NodeType.Item && n.ID == criterion.Key);
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var item = new Node(criterion.Key!, NodeType.Item, criterion.Key!);
                                nodes.Add(item);
                                nodes[i].AddParentNode(item);
                            }
                            break;
                        }
                        case CompareTypes.ItemFromItemGroup:
                        {
                            result = nodes.Find((Node n) => n.Type == NodeType.ItemGroup && n.Text == criterion.Key);
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var item = new Node(criterion.Key!, NodeType.Item, criterion.Key!);
                                nodes.Add(item);
                                nodes[i].AddParentNode(item);
                            }
                            break;
                        }
                        case CompareTypes.Personality:
                        {
                            result = nodes.Find((Node n) => n.Type == NodeType.Personality && n.FileName == criterion.Character && n.ID == ((PersonalityTraits)int.Parse(criterion.Key!)).ToString());
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            break;
                        }
                        case CompareTypes.PlayerInventory:
                        {
                            result = InventoryItems.Find((Node n) => n.Type == NodeType.Inventory && n.ID == criterion.Key);
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            else
                            {
                                //create and add item node, hasnt been referenced yet
                                var item = new Node(criterion.Key!, NodeType.Inventory, "Items: " + criterion.Key);
                                InventoryItems.Add(item);
                                nodes[i].AddParentNode(item);
                            }
                            break;
                        }
                        case CompareTypes.Posing:
                        {
                            if (criterion.PoseOption != PoseOptions.CurrentPose) break;

                            result = Poses.Find((Node n) => n.Type == NodeType.Pose && n.ID == criterion.Value);
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            else
                            {
                                //create and add pose node, hasnt been referenced yet
                                var pose = new Node(criterion.Value!, NodeType.Pose, "Pose number " + criterion.Value);
                                Poses.Add(pose);
                                nodes[i].AddParentNode(pose);
                            }
                            break;
                        }
                        case CompareTypes.Property:
                        {
                            result = Properties.Find((Node n) => n.Type == NodeType.Property && n.ID == criterion.Character + "Property" + criterion.Value);
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            else
                            {
                                //create and add property node, hasnt been referenced yet
                                var property = new Node(criterion.Character + "Property" + criterion.Value, NodeType.Property, criterion.Character + ((InteractiveProperties)int.Parse(criterion.Value!)).ToString()) { FileName = criterion.Character! };
                                Properties.Add(property);
                                nodes[i].AddParentNode(property);
                            }
                            break;
                        }
                        case CompareTypes.Quest:
                        {
                            result = nodes.Find((Node n) => n.Type == NodeType.Quest && n.ID == criterion.Key);
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            break;
                        }
                        case CompareTypes.Social:
                        {
                            result = Socials.Find((Node n) => n.Type == NodeType.Social && n.ID == criterion.Character + criterion.SocialStatus + criterion.Character2);
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            else
                            {
                                //create and add property node, hasnt been referenced yet
                                var social = new Node(criterion.Character + criterion.SocialStatus + criterion.Character2, NodeType.Social, criterion.Character + " " + criterion.SocialStatus + " " + criterion.Character2) { FileName = criterion.Character! };
                                Socials.Add(social);
                                nodes[i].AddParentNode(social);
                            }
                            break;
                        }
                        case CompareTypes.State:
                        {
                            result = States.Find((Node n) => n.Type == NodeType.State && n.FileName == criterion.Character && n.Text.AsSpan()[..2].Contains(criterion.Value!.AsSpan(), StringComparison.InvariantCulture));
                            if (result != null)
                            {
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            else
                            {
                                //create and add property node, hasnt been referenced yet
                                var state = new Node(criterion.Character + "State" + criterion.Value, NodeType.State, criterion.Value + "|" + ((InteractiveStates)int.Parse(criterion.Value!)).ToString()) { FileName = criterion.Character! };
                                States.Add(state);
                                nodes[i].AddParentNode(state);
                            }
                            break;
                        }
                        case CompareTypes.Value:
                        {
                            result = Values.Find((Node n) => n.Type == NodeType.Value && n.ID == criterion.Key);
                            if (result != null)
                            {
                                if (!result.Text.Contains(GetSymbolsFromValueFormula(criterion.ValueFormula ?? ValueSpecificFormulas.EqualsValue) + criterion.Value))
                                    result.Text += GetSymbolsFromValueFormula(criterion.ValueFormula ?? ValueSpecificFormulas.EqualsValue) + criterion.Value + ", ";
                                nodes[i].AddParentNode(result);
                                break;
                            }
                            else
                            {
                                //create and add value node, hasnt been referenced yet
                                var value = new Node(criterion.Key!, NodeType.Value, criterion.Character + " value " + criterion.Key + ", referenced values: " + GetSymbolsFromValueFormula(criterion.ValueFormula ?? ValueSpecificFormulas.EqualsValue) + criterion.Value + ", ");
                                Values.Add(value);
                                nodes[i].AddParentNode(value);
                            }
                            break;
                        }
                        default:
                            break;
                    }
                }
                //todo link up events and the thing they perform as a child to the event
                else if (nodes[i].Type == NodeType.Event)
                {

                }
            }

            //check some comparevalue nodes again because the referenced values havent been added yet
            RecheckCompareValues(CompareValuesToCheckAgain);

            //merge doors with items if applicable
            MergeDoors(nodes);

            //add all intermediatary nodes
            nodes.AddRange(Values);
            nodes.AddRange(Doors);
            nodes.AddRange(Socials);
            nodes.AddRange(States);
            nodes.AddRange(Clothing);
            nodes.AddRange(Poses);
            nodes.AddRange(InventoryItems);
            nodes.AddRange(Properties);

            //recalculate weights due to all the new nodes taht have been added and linked to
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].CalculateMass();
            }

            LogManager.Log($"\tnode interlinking done in {(DateTime.Now - start).TotalSeconds:F2}s");
        }

        private void RecheckCompareValues(NodeList CompareValuesToCheckAgain)
        {
            Node? result;
            foreach (var node in CompareValuesToCheckAgain)
            {
                Criterion criterion = (Criterion)node.Data!;
                result = Values.Find((Node n) => n.Type == NodeType.Value && n.ID == criterion.Key);
                if (result != null)
                {
                    node.AddParentNode(result);
                }
                result = Values.Find((Node n) => n.Type == NodeType.Value && n.ID == criterion.Key2);
                if (result != null)
                {
                    node.AddParentNode(result);
                }
            }
        }

        private void MergeDoors(NodeList nodes)
        {
            Node? result;
            foreach (var door in Doors.ToArray())
            {
                result = nodes.Find((Node n) => n.ID == door.ID);
                if (result != null)
                {
                    foreach (var parentNode in door.ParentNodes.ToArray())
                    {
                        parentNode.AddChildNode(result);
                        parentNode.RemoveChildNode(door);
                    }
                    foreach (var childNode in door.ChildNodes.ToArray())
                    {
                        childNode.AddParentNode(result);
                        childNode.RemoveParentNode(door);
                    }
                    Doors.Remove(door);
                }
            }
        }

        private static string GetSymbolsFromValueFormula(ValueSpecificFormulas formula)
        {
            return formula switch
            {
                ValueSpecificFormulas.EqualsValue => "==",
                ValueSpecificFormulas.DoesNotEqualValue => "!=",
                ValueSpecificFormulas.GreaterThanValue => ">",
                ValueSpecificFormulas.LessThanValue => "<",
                _ => string.Empty,
            };
        }

        private NodeList DissectStory(MainStory story, string AlternateStoryName = "")
        {
            if (AlternateStoryName == string.Empty) AlternateStoryName = StoryName;
            NodeList _nodes = new();
            if (story != null && !GotCancelled)
            {
                CriteriaInFile = new NodeList();

                //add all items in the story
                _nodes.AddRange(StoryNodeExtractor.GetItemOverrides(story));
                //add all item groups with their actions
                _nodes.AddRange(StoryNodeExtractor.GetItemGroups(story));
                //add all items in the story
                _nodes.AddRange(StoryNodeExtractor.GetAchievements(story));
                //add all reactions the player will say
                _nodes.AddRange(StoryNodeExtractor.GetPlayerReactions(story));
                //add all criteriagroups
                _nodes.AddRange(StoryNodeExtractor.GetCriteriaGroups(story));
                //gets the playervalues
                _nodes.AddRange(StoryNodeExtractor.GetValues(story));
                //the events which fire at game start
                _nodes.AddRange(StoryNodeExtractor.GetGameStartEvents(story));
                //add all item groups actions
                _nodes.AddRange(StoryNodeExtractor.GetItemGroupBehaviours(story));

                //remove duplicates/merge criteria
                //maybe later we load the corresponding strings from the social files and vise versa?
                _nodes = ExpandNodes(_nodes);

                //clear criteria to free memory, we dont need them anyways
                //cant be called recusrively so we cant add it, it would break the combination
                CriteriaInFile.Clear();

                for (int i = 0; i < _nodes.Count; i++)
                {
                    _nodes[i].FileName = AlternateStoryName;
                }
            }
            return _nodes;
        }
    }
}