using System.Numerics;
using System.Runtime.Versioning;
using Translator.Core;

namespace Translator.Explorer
{
	internal static class NodeLayoutConstants
	{
		public const float Attraction = 0.1f;//Attraction accelleration multiplier, between 0 and 1
		public const float Repulsion = 100.0f;//Repulsion accelleration multiplier, between 0 and much
		public const float Gravity = 0.0003f;
		public const float IdealLength = 40; //spring IdealLength in units aka thedistance an edge should be long
	}

	[SupportedOSPlatform("windows")]
	internal class NodeLayout
	{
		private readonly List<Node> nodesA = new();
		private readonly List<Node> nodesB = new();
		private bool CalculatedListA = true;
		private Dictionary<Guid, Vector2> NodeForces = new();
		private float cooldown = 1f;
		private const float maxForce = 0.1f;
		private float currentMaxForce = maxForce + 0.1f;
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
			NodeForces = new Dictionary<Guid, Vector2>();
			Nodes.ForEach(n => NodeForces.Add(n.Guid, new Vector2()));

			while (!token.IsCancellationRequested && currentMaxForce > maxForce)
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
			for (int first = 0; first < Internal.Count; first++)
			{
				Vector2 accelleration = Vector2.Zero;
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
				accelleration -= (pos / pos.Length()) * MathF.Pow(MathF.Abs(pos.Length() - radius), 1.5f) * MathF.Sign(pos.Length() - radius) * NodeLayoutConstants.Gravity;

				for (int second = 0; second < Internal.Count; second++)
				{
					if (first != second && Internal[first].Position != Internal[second].Position)
					{
						Vector2 edge = new(
								Internal[first].Position.X - Internal[second].Position.X,
								Internal[first].Position.Y - Internal[second].Position.Y);
						if (Internal[first].ChildNodes.Contains(Internal[second]) || Internal[first].ParentNodes.Contains(Internal[second]))
						{
							//Attraction/spring accelleration on edge
							Vector2 attractionVec = (edge / edge.Length()) * NodeLayoutConstants.Attraction * (edge.Length() - NodeLayoutConstants.IdealLength);

							accelleration -= attractionVec / Internal[first].Mass;
						}
						else
						{
							//Repulsion
							accelleration += (edge / edge.LengthSquared()) * NodeLayoutConstants.Repulsion / Internal[first].Mass;
						}

						NodeForces[Internal[first].Guid] = new(
							float.IsNaN(accelleration.X) ? 0 : accelleration.X,
							float.IsNaN(accelleration.Y) ? 0 : accelleration.Y
							);
					}
				}
			}

			//apply accelleration to nodes
			for (int i = 0; i < Internal.Count; i++)
			{
				Internal[i].Position.X += cooldown * NodeForces[Internal[i].Guid].X;
				Internal[i].Position.Y += cooldown * NodeForces[Internal[i].Guid].Y;
			}

			//recalculate mass cause the nodes moved
			for (int i = 0; i < Internal.Count; i++)
			{
				//Internal[i].CalculateScaledMass();
			}

			//cooldown -= 0.001f;
			if (cooldown < 0) { cooldown = 0; }
		}
	}
}
