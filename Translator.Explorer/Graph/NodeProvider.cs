using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using Translator.Explorer.JSONItems;

namespace Translator.Explorer.Graph
{

    //# acts as the intermediary between display and calculation,
    // * keeps track of all nodes
    // * allows the addition and removal of nodes (tbd)
    // * allows to show only the nodes defined by rules (tbd)
    // * allows to only calulate the positions for defined nodes (tbd)
    // * allows to save all nodes back into a story (tbd)
    [SupportedOSPlatform("windows")]
    public sealed class NodeProvider
    {
        private readonly NodeList publicNodes = new();
        private readonly NodeList nodesA = new();
        private readonly NodeList nodesB = new();
        private bool _usingListA = true;
        private readonly List<NodeType> allowedTypes = new();
        public bool UsingListA
        {
            get
            {
                return !frozen || _usingListA;
            }
            set
            {
                if (frozen)
                    _usingListA = value;
            }
        }
        private bool frozen = false;
        private NodeMovementInfo _movingNodeInfo = (-1, 0.0f, 0.0f);
        public NodeMovementInfo MovingNodeInfo => _movingNodeInfo;
        public readonly NodeType[] defaulTypes = {
            NodeType.ItemAction,
            NodeType.Achievement,
            NodeType.BGC,
            NodeType.Dialogue,
            NodeType.AlternateText,
            NodeType.Event,
            NodeType.Item,
            NodeType.Quest,
            NodeType.Response,
            NodeType.ItemGroupBehaviour };
        public bool TextOnlyEvents = false;
        private string[] _filenames = Array.Empty<string>();

        public NodeList Nodes
        {
            get { CheckNodeListSizes(); return UsingListA ? nodesA : nodesB; }
        }

        //only to be used in calcualation, dont set or youll break things
        public NodeList OtherNodes
        {
            get { CheckNodeListSizes(); return !UsingListA ? nodesA : nodesB; }
        }

        public bool MovingNodePositionOverridden { get; private set; }

        public bool MovingNodePositionOverrideEnded { get; private set; }

        public bool Frozen { get => frozen; }

        private void CheckNodeListSizes()
        {
            if (nodesA.Count == nodesB.Count && nodesA.Edges.Count == nodesB.Edges.Count) return;
            if (!frozen) return;

            if (nodesA.Count != nodesB.Count)
            {
                //only recalculate when necessary 
                if (nodesA.Edges.Count != nodesB.Edges.Count)
                {
                    if (UsingListA)
                        //changed edges is in a
                        nodesA.Sync();
                    else
                    {
                        //changed edges is in b, will be copied later
                        nodesB.Sync();
                    }
                }

                MovingNodePositionOverridden = true;
                if (UsingListA)
                {
                    //changed amount is in a
                    nodesB.Clear();
                    nodesB.AddRange(nodesA);
                }
                else
                {
                    //changed amount is in b
                    nodesA.Clear();
                    nodesA.AddRange(nodesB);
                }
            }

            if (nodesA.Edges.Count == nodesB.Edges.Count) return;

            if (UsingListA)
            {
                //changed edges is in a
                if (Math.Abs(nodesA.Edges.Count - nodesB.Edges.Count) > 1) nodesA.Sync();
                nodesB.Edges.Clear();
                nodesB.Edges.AddRange(nodesA.Edges);
            }
            else
            {
                //changed edges is in b
                if (Math.Abs(nodesA.Edges.Count - nodesB.Edges.Count) > 1) nodesA.Sync();
                nodesA.Edges.Clear();
                nodesA.Edges.AddRange(nodesB.Edges);
            }
        }

        public NodeProvider()
        {
        }

        public void SetFileNames(string[] files)
        {
            _filenames = files;
        }

        public void FreezeNodesAsInitial()
        {
            if (!frozen)
            {
                Nodes.Sync();//sync once before we save them finally
                publicNodes.AddRange(Nodes); //copy init
                nodesB.Clear();
                nodesB.AddRange(nodesA);
                frozen = true;
            }
        }

        public void ConsumedNodePositionOverrideEnded()
        {
            MovingNodePositionOverridden = false;
            MovingNodePositionOverrideEnded = false;
            _movingNodeInfo = (-1, 0.0f, 0.0f);
        }

        public void SignalPositionChangeBegin(int index)
        {
            if (index >= 0)
            {
                _movingNodeInfo.Index = index;
                _movingNodeInfo.PosX = Nodes[index].Position.X;
                _movingNodeInfo.PosY = Nodes[index].Position.Y;
                MovingNodePositionOverrideEnded = false;
                MovingNodePositionOverridden = true;
            }
        }

        public void UpdatePositionChange(float x, float y)
        {
            _movingNodeInfo.PosX = x;
            _movingNodeInfo.PosY = y;
        }

        public void EndPositionChange()
        {
            MovingNodePositionOverrideEnded = true;
        }

        public List<PointF> GetPositions()
        {
            return publicNodes.GetPositions();
        }

        public List<PointF> GetVolatilePositions()
        {
            return Nodes.GetPositions();
        }

        public void AddFilter(NodeType allowedType)
        {
            if (!allowedTypes.Contains(allowedType))
                allowedTypes.Add(allowedType);
        }

        public void RemoveFilter(NodeType disallowedType)
        {
            allowedTypes.Remove(disallowedType);
        }

        public void ApplyFilters()
        {
            NodeList filteredNodes = new();
            foreach (Node node in publicNodes)
            {
                if (TextOnlyEvents)
                {
                    if (node.Type == NodeType.Event)
                    {
                        if (node.DataType != typeof(GameEvent) || node.Data is null) continue;

                        var gameEvent = (GameEvent)node.Data!;
                        if (gameEvent.EventType == StoryEnums.GameEvents.DisplayGameMessage)
                            filteredNodes.Add(node);
                        else if (gameEvent.EventType == StoryEnums.GameEvents.Item)
                        {
                            if (gameEvent.Option == 2)
                                filteredNodes.Add(node);
                        }
                        continue;
                    }
                    if (node.Type == NodeType.Dialogue && TextOnlyEvents)
                    {
                        if (_filenames.Contains(node.FileName))
                            filteredNodes.Add(node);
                        else continue;
                    }
                }
                if (allowedTypes.Contains(node.Type))
                    filteredNodes.Add(node);
            }

            nodesA.Clear();
            nodesB.Clear();
            nodesA.AddRange(filteredNodes);
            nodesB.AddRange(filteredNodes);
            nodesA.StrictSync();
            nodesB.StrictSync();
        }

        public void ResetFilters()
        {
            nodesA.Clear();
            nodesB.Clear();
            nodesA.AddRange(publicNodes);
            nodesB.AddRange(publicNodes);
            nodesA.Sync();
            nodesB.Sync();
        }

        public void ApplyDefaultFilter()
        {
            TextOnlyEvents = true;
            foreach (var type in defaulTypes)
            {
                AddFilter(type);
            }
            ApplyFilters();
            TextOnlyEvents = false;
        }
    }
}
