﻿using Newtonsoft.Json;
using System.Runtime.Versioning;
using Translator.Core;
using Translator.Explorer.JSON;
using DataBase = Translator.Core.DataBase<TranslatorApp.InterfaceImpls.WinLineItem, TranslatorApp.InterfaceImpls.WinUIHandler, TranslatorApp.InterfaceImpls.WinTabController, TranslatorApp.InterfaceImpls.WinTab>;
using Settings = TranslatorApp.InterfaceImpls.WinSettings;
using static Translator.Explorer.JSON.StoryEnums;

namespace Translator.Explorer
{
	[SupportedOSPlatform("Windows")]
	internal sealed class ContextProvider
	{
		private List<Node> CriteriaInFile = new();
		private readonly NodeProvider provider;
		private readonly bool IsStory;
		private readonly Random Random = new();
		public readonly string FileName = "character";
		public readonly string StoryName = "story";
		private string _StoryFilePath = string.Empty;
		private string NodeFilePath = string.Empty;
		public bool GotCancelled = false;
		private readonly bool AutoFileSelection = false;
		public readonly NodeLayout Layout;

		public ContextProvider(NodeProvider provider, bool IsStory, bool AutoSelectFile, string FileName, string StoryName, CancellationToken cancellation)
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
			Layout = new NodeLayout(provider, cancellation);

			if (((Settings)Settings.Default).StoryPath != string.Empty && AutoFileSelection)
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
							Filter = AutoFileSelection ? "Character Files (*.character)|*.character" : string.Empty,
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

		public List<Node> Nodes => provider.Nodes;

		public bool ParseFile()
		{
			if (File.Exists(FilePath))
			{
				NodeFilePath = Path.Combine(LogManager.CFGFOLDER_PATH, $"{StoryName + FileName + DataBase.DBVersion}.json");

				//save path
				((Settings)Settings.Default).StoryPath = Path.GetDirectoryName(FilePath) ?? string.Empty;
				//try to laod the saved nodes
				if (File.Exists(NodeFilePath))
				{
					//read in positions if they exist, but only if version is the same
					List<SerializeableNode>? tempList = JsonConvert.DeserializeObject<List<SerializeableNode>>(File.ReadAllText(NodeFilePath));
					//expand the guids back into references
					while (Nodes.Count != 0) Nodes.Clear();
					Nodes.AddRange(Node.ExpandDeserializedNodes(tempList ?? new List<SerializeableNode>()));
				}
				else
				{
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

					CalculateStartingPositions(Nodes);

					//save nodes
					return SaveNodes(NodeFilePath, Nodes);
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
			if (FileName != string.Empty && StoryName != string.Empty)
			{
				if (FilePath != string.Empty)
					StoryFolderPath = Path.GetDirectoryName(FilePath) ?? string.Empty;
				else if (((Settings)Settings.Default).StoryPath != string.Empty)
					StoryFolderPath = ((Settings)Settings.Default).StoryPath;
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
				//read in positions if they exist, but only if version is the same
				List<SerializeableNode>? tempList = JsonConvert.DeserializeObject<List<SerializeableNode>>(File.ReadAllText(NodeFilePath));
				//expand the guids back into references

				while (Nodes.Count != 0) Nodes.Clear();
				Nodes.AddRange(Node.ExpandDeserializedNodes(tempList ?? new List<SerializeableNode>()));
			}
			else
			{
				if (Directory.GetFiles(StoryFolderPath).Length > 0 && (StoryFolderPath.Split("\\")[^1] == StoryName) || !AutoFileSelection)
				{
					//else create new
					foreach (var item in Directory.GetFiles(StoryFolderPath))
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
					CombineNodes(Nodes);
				}
				else
				{
					ParseAllFiles();
					return true;
				}

				CalculateStartingPositions(Nodes);

				//save nodes
				return SaveNodes(NodeFilePath, Nodes);
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

		private void CalculateStartingPositions(List<Node> nodes)
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

		private List<Node> ExpandNodes(List<Node> nodes)
		{
			//temporary list so we dont manipulate the list we read from in the for loops
			var listNodes = new List<Node>();

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

		private List<Node> DissectCharacter(CharacterStory story)
		{
			List<Node> _nodes = new();
			if (story != null && !GotCancelled)
			{
				CriteriaInFile = new List<Node>();

				//get all relevant items from the json
				_nodes.AddRange(StoryNodeExtractor.GetDialogues(story));
				_nodes.AddRange(StoryNodeExtractor.GetGlobalGoodByeResponses(story));
				_nodes.AddRange(StoryNodeExtractor.GetGlobalResponses(story));
				_nodes.AddRange(StoryNodeExtractor.GetBackGroundChatter(story));
				_nodes.AddRange(StoryNodeExtractor.GetQuests(story));
				_nodes.AddRange(StoryNodeExtractor.GetReactions(story));

				//remove duplicates/merge criteria
				//maybe later we load the corresponding strings from the character files and vise versa?
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

		private static void CombineNodes(List<Node> nodes)
		{
			DateTime start = DateTime.Now;
			LogManager.Log("\tstarting to link up nodes");
			//link up different stories and dialogues
			for (int i = 0; i < nodes.Count; i++)
			{
				if (nodes[i].Type == NodeType.Criterion)
				{
					//todo more merging for other links, like triggers or events
					//for that we need more info in the nodes, like what they are and the values
					//so value1, value2, comparetype and event type
					//$"{criterion.Character}|{criterion.Character2}|{criterion.CompareType}|{criterion.DialogueStatus}|{criterion.EqualsValue}|{criterion.Key}|{criterion.Key2}|{criterion.Option}|{criterion.SocialStatus}|{criterion.Value}", 
					//split up the criteria values according to the list above
					var values = nodes[i].Text.Split('|');
					if (values.Length < 10) continue;
					//if we have a chance of finding the dialogue node
					if (values[2] == CompareTypes.Dialogue.ToString())
					{
						for (int k = 0; k < nodes.Count; k++)
						{
							var result = nodes.Find((Node n) => n.Type == NodeType.Dialogue && n.FileName == values[0] && n.ID == values[9]);
							if (result != null)
							{
								nodes[i].AddChildNode(result);
								break;//dialogue can only exist once lol
							}
						}
					}
				}
				else if (nodes[i].Type == NodeType.Event)
				{

				}
			}
			for (int i = 0; i < nodes.Count; i++)
			{
				nodes[i].CalculateMass();
			}

			LogManager.Log($"\tnode interlinking done in {(DateTime.Now - start).TotalSeconds:F2}s");
		}

		private List<Node> DissectStory(MainStory story, string AlternateStoryName = "")
		{
			if (AlternateStoryName == string.Empty) AlternateStoryName = StoryName;
			List<Node> _nodes = new();
			if (story != null && !GotCancelled)
			{
				CriteriaInFile = new List<Node>();

				//add all items in the story
				_nodes.AddRange(StoryNodeExtractor.GetItemOverrides(story));
				//add all item groups with their actions
				_nodes.AddRange(StoryNodeExtractor.GetItemGroups(story));
				//add all items in the story
				_nodes.AddRange(StoryNodeExtractor.GetAchievements(story));
				//add all reactions the player will say
				_nodes.AddRange(StoryNodeExtractor.GetPlayerReactions(story));

				//remove duplicates/merge criteria
				//maybe later we load the corresponding strings from the character files and vise versa?
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

		internal void SaveNodes()
		{
			Layout.Stop();
			SaveNodes(NodeFilePath, Nodes);
		}
	}
}