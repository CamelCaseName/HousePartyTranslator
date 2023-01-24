﻿using System.Diagnostics;
using System.Numerics;
using System.Runtime.Versioning;
using Translator.Core;

namespace Translator.Explorer
{
	internal static class StoryExplorerConstants
	{
		public const float Attraction = 0.2f;//Attraction accelleration multiplier, between 0 and 1
		public const float Repulsion = 100.0f;//Repulsion accelleration multiplier, between 0 and much
		public const float Gravity = 0.0003f;
		public static float IdealLength = 120; //spring IdealLength in units aka thedistance an edge should be long
		public static int ColoringDepth = 15;
		public static int Nodesize = 16;
	}

	[SupportedOSPlatform("windows")]
	internal class NodeLayout
	{
		private readonly List<Node> nodesA = new();
		private readonly List<Node> nodesB = new();
		private bool CalculatedListA = true;
		private List<Vector2> NodeForces = new();
		private DateTime StartTime = DateTime.MinValue;
		private DateTime FrameStartTime = DateTime.MinValue;
		private DateTime FrameEndTime = DateTime.MinValue;
		public bool RunOverride = false;
		private CancellationTokenSource cancellationToken = new();
		private readonly CancellationToken outsideToken;
		public int FrameCount { get; private set; }
		public bool Finished => FrameCount > Nodes.Count * Nodes.Count && !RunOverride && Started;
		public bool Started { get; private set; } = false;
		public List<Node> Nodes
		{
			get { return CalculatedListA ? nodesA : nodesB; }
		}
		private List<Node> Internal
		{
			get { return !CalculatedListA ? nodesA : nodesB; }
		}

		public NodeLayout(CancellationToken cancellation)
		{
			outsideToken = cancellation;
		}

		public void Start()
		{
			if (!Started)
			{
				cancellationToken = new();
				outsideToken.Register(() => cancellationToken.Cancel());
				StartTime = DateTime.Now;
				LogManager.Log($"\tnode layout started for {Nodes.Count} nodes");
				Started = true;
				_ = Task.Run(() => CalculateForceDirectedLayout(cancellationToken.Token), cancellationToken.Token);
			}
		}

		public void Stop()
		{
			DateTime end = DateTime.Now;
			LogManager.Log($"\tnode layout ended, rendered for {(end - StartTime).TotalSeconds:F2} seconds and rendered {FrameCount} frames -> {FrameCount / (end - StartTime).TotalSeconds:F2} fps");
			cancellationToken.Cancel();
			Started = false;
			FrameCount = 0;
		}

		public void CalculateForceDirectedLayout(CancellationToken token)
		{
			//save all forces here
			NodeForces = new(Nodes.Count);
			Nodes.ForEach(n => NodeForces.Add(new Vector2()));

			while (!token.IsCancellationRequested && !Finished)
			{
				FrameStartTime = FrameEndTime;
				//sync nodes, initally in b
				if (nodesA.Count != nodesB.Count)
				{
					if (CalculatedListA)
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
				//calculate
				CalculatePositions();
				CalculatePositions();

				//we got to wait before we change nodes, so like a reverse lock?
				while (!App.MainForm?.Explorer?.Grapher.DrewNodes ?? false) ;
				//switch to other list once done
				CalculatedListA = !CalculatedListA;
				++FrameCount;

				//approx 40fps max as more is uneccesary and feels weird (25ms gives ~50fps, 30 gives about 45fps)
				FrameEndTime = DateTime.Now;
				if ((FrameEndTime - FrameStartTime).TotalMilliseconds < 30) Thread.Sleep((int)(30 - (FrameEndTime - FrameStartTime).TotalMilliseconds));

				App.MainForm?.Explorer?.Invalidate();
			}
			Stop();
		}

		//todo move to gpu with opencl
		private void CalculatePositions()
		{
			for (int i = 0; i < NodeForces.Count; i++)
			{
				NodeForces[i] = Vector2.Zero;
			}

			for (int first = 0; first < Internal.Count; first++)
			{

				//Gravity to center
				float radius = MathF.Sqrt(Internal.Count) + StoryExplorerConstants.IdealLength * 2;
				Vector2 pos = new(Internal[first].Position.X, Internal[first].Position.Y);

				//fix any issues before we divide by pos IdealLength
				if (pos.Length() == 0)
				{
					pos.X = pos.X == 0 ? float.MinValue : pos.X;
					pos.Y = pos.Y == 0 ? float.MinValue : pos.Y;
				}

				//can IdealLength ever be absolutely 0?
				//gravity calculation
				NodeForces[first] -= (pos / pos.Length()) * MathF.Pow(MathF.Abs(pos.Length() - radius), 1.5f) * MathF.Sign(pos.Length() - radius) * StoryExplorerConstants.Gravity;

				for (int second = first + 1; second < Internal.Count; second++)
				{
					if (Internal[first].Position != Internal[second].Position)
					{
						Vector2 edge = new(
								Internal[first].Position.X - Internal[second].Position.X,
								Internal[first].Position.Y - Internal[second].Position.Y
								);
						if (Internal[first].ChildNodes.Contains(Internal[second]) || Internal[first].ParentNodes.Contains(Internal[second]))
						{
							//Attraction/spring accelleration on edge
							Vector2 attractionVec = (edge / edge.Length()) * StoryExplorerConstants.Attraction * (edge.Length() - StoryExplorerConstants.IdealLength);

							NodeForces[first] -= attractionVec;
							NodeForces[second] += attractionVec;
						}
						else
						{
							//Repulsion
							NodeForces[first] += (edge / edge.LengthSquared()) * StoryExplorerConstants.Repulsion;
							NodeForces[second] -= (edge / edge.LengthSquared()) * StoryExplorerConstants.Repulsion;
						}
					}
					else
					{
						//move away
						Internal[first].Position.X += 10f;
					}
				}
			}

			//apply accelleration to nodes
			for (int i = 0; i < Internal.Count; i++)
			{
				if (!Internal[i].IsPositionLocked)
				{
					Internal[i].Position.X += NodeForces[i].X / Internal[i].Mass;
					Internal[i].Position.Y += NodeForces[i].Y / Internal[i].Mass;
				}
			}
		}
	}
}
