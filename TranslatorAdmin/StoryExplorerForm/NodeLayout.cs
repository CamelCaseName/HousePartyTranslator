using System.Diagnostics;
using System.Numerics;
using System.Runtime.Versioning;
using Translator.Core;

namespace Translator.Explorer
{
	internal static class NodeLayoutConstants
	{
		public const float Attraction = 0.2f;//Attraction accelleration multiplier, between 0 and 1
		public const float Repulsion = 100.0f;//Repulsion accelleration multiplier, between 0 and much
		public const float Gravity = 0.0003f;
		public const float IdealLength = 120; //spring IdealLength in units aka thedistance an edge should be long
	}

	[SupportedOSPlatform("windows")]
	internal class NodeLayout
	{
		private readonly List<Node> nodesA = new();
		private readonly List<Node> nodesB = new();
		private bool CalculatedListA = true;
		private List<Vector2> NodeForces = new();
		private float cooldown = 1f;
		private DateTime StartTime = DateTime.MinValue;
		private int FrameCount;
		private readonly CancellationTokenSource cancellationToken = new();
		public bool Finished => cooldown == 0;
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
			cancellation.Register(() => cancellationToken.Cancel());
		}

		public void Start()
		{
			StartTime = DateTime.Now;
			LogManager.Log($"\tnode layout started for {Nodes.Count} nodes");
			Started = true;
			_ = Task.Run(() => CalculateForceDirectedLayout(cancellationToken.Token), cancellationToken.Token);
		}

		public void Stop()
		{
			DateTime end = DateTime.Now;
			LogManager.Log($"\tnode layout ended, rendered for {(end - StartTime).TotalSeconds:F2} seconds and rendered {FrameCount} frames -> {FrameCount / (end - StartTime).TotalSeconds:F2} fps");
			cancellationToken.Cancel();
		}

		public void CalculateForceDirectedLayout(CancellationToken token)
		{
			//save all forces here
			NodeForces = new(Nodes.Count);
			Nodes.ForEach(n => NodeForces.Add(new Vector2()));

			while (!token.IsCancellationRequested)
			{
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
				//lock and calculate
				if (CalculatedListA)
					lock (nodesB)
					{
						CalculatePositions();
					}
				else
					lock (nodesA)
					{
						CalculatePositions();
					}

				//we got to wait before we change nodes, so like a reverse lock?
				while (!App.MainForm?.Explorer?.Grapher.DrewNodes ?? false) ;
				//switch to other list once done
				CalculatedListA = !CalculatedListA;
				++FrameCount;

				App.MainForm?.Explorer?.Invalidate();
			}
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
				float radius = MathF.Sqrt(Internal.Count) + NodeLayoutConstants.IdealLength * 2;
				Vector2 pos = new(Internal[first].Position.X, Internal[first].Position.Y);

				//fix any issues before we divide by pos IdealLength
				if (pos.Length() == 0)
				{
					pos.X = pos.X == 0 ? float.MinValue : pos.X;
					pos.Y = pos.Y == 0 ? float.MinValue : pos.Y;
				}

				//can IdealLength ever be absolutely 0?
				//gravity calculation
				NodeForces[first] -= (pos / pos.Length()) * MathF.Pow(MathF.Abs(pos.Length() - radius), 1.5f) * MathF.Sign(pos.Length() - radius) * NodeLayoutConstants.Gravity;

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
							Vector2 attractionVec = (edge / edge.Length()) * NodeLayoutConstants.Attraction * (edge.Length() - NodeLayoutConstants.IdealLength);

							if (attractionVec.Length() > 1000) Debugger.Break();

							NodeForces[first] -= attractionVec / Internal[first].Mass;
							NodeForces[second] += attractionVec / Internal[second].Mass;
						}
						else
						{
							//Repulsion
							NodeForces[first] += (edge / edge.LengthSquared()) * NodeLayoutConstants.Repulsion / Internal[first].Mass;
							NodeForces[second] -= (edge / edge.LengthSquared()) * NodeLayoutConstants.Repulsion / Internal[second].Mass;
						}
					}
					else
					{//move away
						Internal[first].Position.X += 10f;
					}
				}
			}

			//apply accelleration to nodes
			for (int i = 0; i < Internal.Count; i++)
			{
				Internal[i].Position.X += cooldown * NodeForces[i].X;
				Internal[i].Position.Y += cooldown * NodeForces[i].Y;
				if (Internal[i].Position.X > 1 / (NodeLayoutConstants.Gravity * NodeLayoutConstants.Gravity)
					||
					Internal[i].Position.Y > 1 / (NodeLayoutConstants.Gravity * NodeLayoutConstants.Gravity))
				{
					Vector2 avg = new();
					for (int x = 0; x < Internal[i].ChildNodes.Count; x++)
					{
						avg.X += Internal[i].ChildNodes[x].Position.X;
						avg.Y += Internal[i].ChildNodes[x].Position.Y;
					}
					for (int x = 0; x < Internal[i].ParentNodes.Count; x++)
					{
						avg.X += Internal[i].ParentNodes[x].Position.X;
						avg.Y += Internal[i].ParentNodes[x].Position.Y;
					}
					Internal[i].Position.X = avg.X;
					Internal[i].Position.Y = avg.Y;
				}
			}

			//recalculate mass cause the nodes moved
			for (int i = 0; i < Internal.Count; i++)
			{
				//Internal[i].CalculateScaledMass();
			}

			//cooldown -= 0.001f;
			//if (cooldown < 0) { cooldown = 0; }
		}
	}
}
