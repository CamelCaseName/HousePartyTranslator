using System.Diagnostics;
using System.Numerics;
using System.Runtime.Versioning;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Translator.Core;

namespace Translator.Explorer
{
	[SupportedOSPlatform("windows")]
	internal class NodeLayout
	{
		private readonly List<Node> nodesA = new();
		private readonly List<Node> nodesB = new();
		private bool CalculatedListA = true;
		private Dictionary<Guid, Vector2> NodeForces = new();
		private float cooldown = 0.9999f;
		private const float maxForce = 0;
		private const float attraction = 0.7f;//attraction force multiplier, between 0 and much
		private float currentMaxForce = maxForce + 0.1f;
		private const float repulsion = 0.0f;//repulsion force multiplier, between 0 and much
		private const int length = 50; //spring length in units aka thedistance an edge should be long
		private readonly CancellationToken cancellationToken;
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
			cancellationToken = cancellation;
		}

		public void Start()
		{
			LogManager.Log("node layout started");
			Started = true;
			_ = Task.Run(() => CalculateForceDirectedLayout(cancellationToken), cancellationToken);
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

				App.MainForm?.Explorer?.Invalidate();
			}
		}

		private void CalculatePositions()
		{
			for (int first = 0; first < Internal.Count; first++)
			{
				Vector2 force = Vector2.Zero;
				//gravity to center
				//float radius = MathF.Sqrt(Internal.Count) + length * 2;
				//Vector2 pos = new(Internal[first].Position.X, Internal[first].Position.Y);
				//force += (pos / pos.Length()) * (MathF.Pow(pos.Length(), 0.7f) - radius) * (pos.Length() < radius ? 1 : -1) * 0.5f;

				for (int second = 0; second < Internal.Count; second++)
				{
					if (first != second && Internal[first].Position != Internal[second].Position)
					{
						Vector2 edge = new(
								Internal[first].Position.X - Internal[second].Position.X,
								Internal[first].Position.Y - Internal[second].Position.Y);
						if (Internal[first].ChildNodes.Contains(Internal[second]))
						{
							//attraction
							Vector2 attractionVec = (edge / edge.Length()) * attraction * (length - edge.Length());

							force += attractionVec / (Internal[first].Mass * 1);
							NodeForces[Internal[second].Guid] += attractionVec / (Internal[second].Mass * 1);
							if (attractionVec.Length() > 1000) Debug.WriteLine($"{attractionVec} force for node {Internal[first]} and {Internal[second]} at {first}:{second}");
						}
						else
						{
							//repulsion
							force += (edge / edge.Length()) * repulsion * (length - edge.Length());
						}

						NodeForces[Internal[first].Guid] = new(force.X, force.Y);
					}
				}
			}

			//apply force to nodes
			for (int i = 0; i < Internal.Count; i++)
			{
				Internal[i].Position.X += cooldown * NodeForces[Internal[i].Guid].X;
				Internal[i].Position.Y += cooldown * NodeForces[Internal[i].Guid].Y;
			}

		}
	}
}
