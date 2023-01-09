using Newtonsoft.Json;
using System.Numerics;
using System.Runtime.Versioning;
using Translator.Core;
using Translator.Explorer.JSON;
using DataBase = Translator.Core.DataBase<TranslatorAdmin.InterfaceImpls.WinLineItem, TranslatorAdmin.InterfaceImpls.WinUIHandler, TranslatorAdmin.InterfaceImpls.WinTabController, TranslatorAdmin.InterfaceImpls.WinTab>;
using Settings = TranslatorAdmin.InterfaceImpls.WinSettings;

namespace Translator.Explorer
{
	[SupportedOSPlatform("Windows")]
	internal sealed class ContextProvider
	{
		public bool GotCancelled = false;
		private readonly string FileId;
		private readonly bool IsStory;
		private readonly Random Random = new();
		private string _StoryFilePath;
		private Dictionary<Guid, Vector2> NodeForces = new();
		private List<Node> CriteriaInFile = new();
		private readonly ParallelOptions options;
		private readonly string StoryName = "story";
		private readonly string FileName = "character";

		//todo: decouple node force layout and use seperate task for that, update frame each time it finishes
		//todo: read in all files and combine/merge nodes through story files.
		//todo: if going through a node connection into a different file and it is not open, try to open it or switch to it if open already

		public ContextProvider(bool IsStory, bool AutoSelectFile, string FileName, string StoryName, ParallelOptions parallelOptions)
		{
			_StoryFilePath = string.Empty;
			options = parallelOptions;
			this.IsStory = IsStory;
			this.FileName = FileName;
			this.StoryName = StoryName;

			if (((Settings)Settings.Default).StoryPath != string.Empty && AutoSelectFile && FileName != string.Empty && StoryName != string.Empty)
			{
				string storyPathMinusStory = Directory.GetParent(((Settings)Settings.Default).StoryPath)?.FullName ?? string.Empty;

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

			//create an id to differentiate between the different calculated layouts later
			FileId = StoryName + FileName + DataBase.DBVersion;
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
							Filter = "Story Files (*.story)|*.story",
							InitialDirectory = ((Settings)Settings.Default).StoryPath.Length > 0 ? ((Settings)Settings.Default).StoryPath : @"C:\Users\%USER%\Documents",
							RestoreDirectory = false,
							FileName = this.FileName + ".story"
						};
					}
					else//character file
					{
						selectFileDialog = new OpenFileDialog
						{
							Title = $"Choose the character file ({FileName}) for the templates",
							Filter = "Character Files (*.character)|*.character",
							InitialDirectory = ((Settings)Settings.Default).StoryPath.Length > 0 ? ((Settings)Settings.Default).StoryPath : @"C:\Users\%USER%\Documents",
							RestoreDirectory = false,
							FileName = this.FileName + ".character"
						};
					}

					if (selectFileDialog.ShowDialog() == DialogResult.OK)
					{
						((Settings)Settings.Default).StoryPath = Path.GetDirectoryName(selectFileDialog.FileName) ?? string.Empty;
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

		public List<Node> Nodes { get; private set; } = new();

		public bool ParseFile()
		{
			if (File.Exists(FilePath))
			{
				string fileString = File.ReadAllText(FilePath);
				string savedNodesPath = Path.Combine(LogManager.CFGFOLDER_PATH, $"{FileId}.json");

				//save path
				((Settings)Settings.Default).StoryPath = Path.GetDirectoryName(FilePath) ?? string.Empty;
				//try to laod the saved nodes
				if (File.Exists(savedNodesPath))
				{
					//read in positions if they exist, but only if version is the same
					List<SerializeableNode>? tempList = JsonConvert.DeserializeObject<List<SerializeableNode>>(File.ReadAllText(savedNodesPath));
					//expand the guids back into references
					Nodes = Node.ExpandDeserializedNodes(tempList ?? new List<SerializeableNode>());
				}
				else
				{
					//else create new
					if (IsStory)
					{
						Nodes = DissectStory(JsonConvert.DeserializeObject<MainStory>(fileString) ?? new MainStory());
						for (int i = 0; i < Nodes.Count; i++)
						{
							Nodes[i].FileName = StoryName;
						}
					}
					else
					{
						Nodes = DissectCharacter(JsonConvert.DeserializeObject<CharacterStory>(fileString) ?? new CharacterStory());
						for (int i = 0; i < Nodes.Count; i++)
						{
							Nodes[i].FileName = FileName;
						}
					}


					//save nodes
					return SaveNodes(savedNodesPath, Nodes);
				}

				return Nodes.Count > 0;
			}
			else
			{
				return false;
			}
		}

		public bool ParseAllFiles()
		{
			string StoryFolderPath = string.Empty;
			if (((Settings)Settings.Default).StoryPath != string.Empty && FileName != string.Empty && StoryName != string.Empty)
			{
				StoryFolderPath = ((Settings)Settings.Default).StoryPath;
			}
			else
			{
				var folderBrowser = new FolderBrowserDialog()
				{
					RootFolder = Environment.SpecialFolder.MyDocuments,
					Description = "Please select the folder with the story files in it.",
					ShowHiddenFiles = true,
					UseDescriptionForTitle = true
				};

				if (folderBrowser.ShowDialog() == DialogResult.OK)
					StoryFolderPath = folderBrowser.SelectedPath;
			}

			if (!Directory.Exists(StoryFolderPath)) return false;

			string savedNodesPath = Path.Combine(LogManager.CFGFOLDER_PATH, $"{StoryName + DataBase.DBVersion}.json");

			//try to load the saved nodes
			if (File.Exists(savedNodesPath))
			{
				//read in positions if they exist, but only if version is the same
				List<SerializeableNode>? tempList = JsonConvert.DeserializeObject<List<SerializeableNode>>(File.ReadAllText(savedNodesPath));
				//expand the guids back into references
				Nodes = Node.ExpandDeserializedNodes(tempList ?? new List<SerializeableNode>());
			}
			else
			{
				if (Directory.GetFiles(StoryFolderPath).Length > 0)
				{
					//else create new
					foreach (var item in Directory.GetFiles(StoryFolderPath))
					{
						if (Path.GetFileNameWithoutExtension(item) == StoryName)
							Nodes.AddRange(
								DissectStory(
									JsonConvert.DeserializeObject<MainStory>(
										File.ReadAllText(item)) ?? new()));

						else
							Nodes.AddRange(
								DissectCharacter(
									JsonConvert.DeserializeObject<CharacterStory>(
										File.ReadAllText(item)) ?? new()));
					}
				}
				else
				{
					((Settings)Settings.Default).StoryPath = string.Empty;
					ParseAllFiles();
					return true;
				}


				Nodes = CalculateStartingPositions(Nodes);

				//save nodes
				return SaveNodes(savedNodesPath, Nodes);
			}

			return Nodes.Count > 0;
		}

		public static bool SaveNodes(string path, List<Node> nodes)
		{
			if (nodes.Count > 0)
			{
				File.WriteAllText(path, JsonConvert.SerializeObject(nodes.ConvertAll(n => (SerializeableNode)n)));
				return true;
			}
			else return false;
		}

		public List<Node> GetTemplateNodes()
		{
			if (FilePath.Length > 0)
			{
				string fileString = File.ReadAllText(FilePath);
				fileString = fileString[fileString.IndexOf('{')..];
				//else create new
				if (IsStory)
				{
					Nodes = DissectStory(JsonConvert.DeserializeObject<MainStory>(fileString) ?? new MainStory());
				}
				else
				{
					Nodes = DissectCharacter(JsonConvert.DeserializeObject<CharacterStory>(fileString) ?? new CharacterStory());
				}

				return Nodes;
			}
			return new();
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
			const int iterations = 1;
			const float attraction = 1f;//attraction force multiplier, between 0 and much
			float cooldown = 0.9999f;
			float currentMaxForce = maxForce + 0.1f;
			const float repulsion = 1.3f;//repulsion force multiplier, between 0 and much
			const int length = 200; //spring length in units aka thedistance an edge should be long

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
				try
				{
					//go through all nodes and apply the forces
					_ = Parallel.For(0, nodes.Count, options, i1 =>
					{
						Node node = nodes[i1];
						//create position vector for all later calculations
						var nodePosition = new Vector2(node.Position.X, node.Position.Y);
						//reset force
						NodeForces[node.Guid] = new Vector2();

						//calculate repulsion force
						for (int i2 = 0; i2 < nodes.Count; i2++)
						{
							Node secondNode = nodes[i2];
							if (node != secondNode && secondNode.Position != node.Position)
							{
								var secondNodePosition = new Vector2(secondNode.Position.X, secondNode.Position.Y);
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
					});
				}
				catch (OperationCanceledException)
				{
					LogManager.Log("Explorer closed during creation");
					return new();
				}

				//apply force to nodes
				for (int i1 = 0; i1 < nodes.Count; i1++)
				{
					Node node = nodes[i1];
					node.Position.X += (int)(cooldown * NodeForces[node.Guid].X);
					node.Position.Y += (int)(cooldown * NodeForces[node.Guid].Y);
				}

				i++;
			}
			return nodes;
		}

		public static List<Tuple<int, int>> GetEdges(List<Node> _nodes)
		{
			var returnList = new List<Tuple<int, int>>();

			for (int i = 0; i < _nodes.Count; i++)
			{
				for (int j = 0; j < _nodes[i].ChildNodes.Count; j++)
				{
					returnList.Add(new Tuple<int, int>(i, _nodes.FindIndex(n => n == _nodes[i].ChildNodes[j])));
				}
			}

			return returnList;
		}

		private List<Node> CalculateStartingPositions(List<Node> nodes)
		{
			int step = 40;
			int runningTotal = 0;
			//~sidelength of the most square layout we can achieve witrh the number of nodes we have
			int sideLength = (int)(Math.Sqrt(nodes.Count) + 0.5);
			for (int i = 0; i < nodes.Count; i++)
			{
				//modulo of running total with sidelength gives x coords, repeating after sidelength
				//offset by halfe sidelength to center x
				int x = (runningTotal % sideLength) - sideLength / 2 + Random.Next(-(step / 2) + 1, (step / 2) - 1);
				//running total divided by sidelength gives y coords,
				//increments after runningtotal increments sidelength times
				//offset by halfe sidelength to center y
				int y = (runningTotal / sideLength) - sideLength / 2 + Random.Next(-(step / 2) + 1, (step / 2) - 1);
				//set position
				nodes[i].SetPosition(new Point(x * step, y * step));
				//increase running total
				runningTotal++;
			}
			return nodes;
		}

		private List<Node> CombineNodes(List<Node> nodes)
		{
			//temporary list so we dont manipulate the list we read from in the for loops
			var tempNodes = new Dictionary<Guid, Node>();

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
					//set gender of childs of the event this criterion is part of if we have a gender comparison

					//if the criterion has already been seen before
					Node criterionInFile = CriteriaInFile.Find(n => n.Guid == node.Guid) ?? Node.NullNode;
					if (criterionInFile != null)//has been seen before
					{
						//add the childs and parents of this instance of the criterion to the first instance of this criterion
						//aka fusion
						criterionInFile.ChildNodes.AddRange(node.ChildNodes);
						criterionInFile.PropagateGender(criterionInFile.Gender);
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
			var listNodes = new List<Node>();
			foreach (KeyValuePair<Guid, Node> pair in tempNodes)
			{
				listNodes.Add(pair.Value);
			}
			return listNodes;
		}

		private List<Node> DissectCharacter(CharacterStory story)
		{
			List<Node> _nodes = new();
			if (story != null && !GotCancelled)
			{
				CriteriaInFile = new List<Node>();

				//get all relevant items from the json
				_nodes.AddRange(GetDialogues(story));
				_nodes.AddRange(GetGlobalGoodByeResponses(story));
				_nodes.AddRange(GetGlobalResponses(story));
				_nodes.AddRange(GetBackGroundChatter(story));
				_nodes.AddRange(GetQuests(story));
				_nodes.AddRange(GetReactions(story));

				//remove duplicates/merge criteria
				//maybe later we load the corresponding strings from the character files and vise versa?
				_nodes = CombineNodes(_nodes);

				//clear criteria to free memory, we dont need them anyways
				//cant be called recusrively so we cant add it, it would break the combination
				CriteriaInFile.Clear();

				for (int i = 0; i < Nodes.Count; i++)
				{
					_nodes[i].FileName = story.CharacterName ?? string.Empty;
				}
			}
			return _nodes;
		}

		private List<Node> DissectStory(MainStory story)
		{
			List<Node> _nodes = new();
			if (story != null && !GotCancelled)
			{
				CriteriaInFile = new List<Node>();

				//add all items in the story
				_nodes.AddRange(GetItemOverrides(story));
				//add all item groups with their actions
				_nodes.AddRange(GetItemGroups(story));
				//add all items in the story
				_nodes.AddRange(GetAchievements(story));
				//add all reactions the player will say
				_nodes.AddRange(GetPlayerReactions(story));

				//remove duplicates/merge criteria
				//maybe later we load the corresponding strings from the character files and vise versa?
				_nodes = CombineNodes(_nodes);

				//clear criteria to free memory, we dont need them anyways
				//cant be called recusrively so we cant add it, it would break the combination
				CriteriaInFile.Clear();

				for (int i = 0; i < Nodes.Count; i++)
				{
					_nodes[i].FileName = StoryName;
				}
			}
			return _nodes;
		}

		private static List<Node> GetAchievements(MainStory story)
		{
			//list to collect all achievement nodes
			var nodes = new List<Node>();
			//go through all of them
			foreach (Achievement achievement in story.Achievements ?? Enumerable.Empty<Achievement>())
			{
				//node to add the description as child to, needs reference to parent, hence can't be anonymous
				var node = new Node(achievement.Id ?? string.Empty, NodeType.Achievement, achievement.Name ?? string.Empty);
				node.AddChildNode(new Node(achievement.Id + "Description", NodeType.Achievement, achievement.Description ?? string.Empty, node));
				//add achievement with description child to list
				nodes.Add(node);
			}

			//return list of achievements
			return nodes;
		}

		private static List<Node> GetBackGroundChatter(CharacterStory story)
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

		private static List<Node> GetDialogues(CharacterStory story)
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

		private static List<Node> GetGlobalGoodByeResponses(CharacterStory story)
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

		private static List<Node> GetGlobalResponses(CharacterStory story)
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

		private static List<Node> GetItemGroups(MainStory story)
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

		private static List<Node> GetItemOverrides(MainStory story)
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

		private static List<Node> GetPlayerReactions(MainStory story)
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

		private static List<Node> GetQuests(CharacterStory story)
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

		private static List<Node> GetReactions(CharacterStory story)
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