using System.Numerics;
using System.Runtime.Versioning;
using Translator.Core;

namespace Translator.Explorer
{
	[SupportedOSPlatform("windows")]
	internal class NodeLayout
	{
		private readonly List<Node> nodesA = new();
		private readonly List<Node> nodesB = new();
		private bool CalculatedListA = false;
		private static Dictionary<Guid, Vector2> NodeForces = new();
		private static float cooldown = 0.9999f;
		private const float maxForce = 0;
		private const float attraction = 1f;//attraction force multiplier, between 0 and much
		private float currentMaxForce = maxForce + 0.1f;
		private const float repulsion = 1.3f;//repulsion force multiplier, between 0 and much
		private const int length = 200; //spring length in units aka thedistance an edge should be long
		private readonly CancellationToken cancellationToken;
		public bool Started { get; private set; } = false;
		public List<Node> Nodes
		{
			get { return CalculatedListA ? nodesA : nodesB; }
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
			//You just need to think about the 3 separate forces acting on each node,
			//add them together each "frame" to get the movement of each node.

			//Gravity, put a simple force acting towards the centre of the canvas so the nodes dont launch themselves out of frame

			//Node-Node replusion, You can either use coulombs force(which describes particle-particle repulsion)
			//or use the gravitational attraction equation and just reverse it

			//Connection Forces, this ones a little tricky, define a connection as 2 nodes and the distance between them.

			//copy node ids in a dict so we can apply force and not disturb the rest
			//save all forces here
			NodeForces = new Dictionary<Guid, Vector2>();
			Nodes.ForEach(n => NodeForces.Add(n.Guid, new Vector2()));

			while (!token.IsCancellationRequested)
			{
				if (CalculatedListA) lock (nodesB)
					{
						CalculatePositions();
					}
				else lock (nodesA)
					{
						CalculatePositions();
					}

				//we got to wait before we change nodes, so like a reverse lock?
				while (!App.MainForm?.Explorer?.Grapher.DrewNodes ?? false) ;
				//switch to other list once done
				CalculatedListA = !CalculatedListA;
			}
		}

		private void CalculatePositions()
		{
			//calculate new, smaller cooldown so the nodes will move less and less
			cooldown *= cooldown;

			try
			{
				//go through all nodes and apply the forces
				for (int i1 = 0; i1 < Nodes.Count; i1++)
				{
					Node node = Nodes[i1];
					//create position vector for all later calculations
					var nodePosition = new Vector2(node.Position.X, node.Position.Y);
					//reset force
					NodeForces[node.Guid] = new Vector2();

					//calculate repulsion force
					for (int i2 = 0; i2 < Nodes.Count; i2++)
					{
						Node secondNode = Nodes[i2];
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
				}
			}
			catch (OperationCanceledException)
			{
				LogManager.Log("node layout ended");
			}

			//apply force to nodes
			for (int i1 = 0; i1 < Nodes.Count; i1++)
			{
				Node node = Nodes[i1];
				node.Position.X += (int)(cooldown * NodeForces[node.Guid].X);
				node.Position.Y += (int)(cooldown * NodeForces[node.Guid].Y);
			}
		}
	}
}
