namespace Translator.Explorer.OpenCL;

internal static class Kernels
{
	/*
	 * code to implement in opencl
	 * 
	
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

					NodeForces[first] -= attractionVec / Internal[first].Mass;
					NodeForces[second] += attractionVec / Internal[second].Mass;
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
			Internal[i].Position.X += NodeForces[i].X;
			Internal[i].Position.Y += NodeForces[i].Y;
		}
	}
	 */
	public static string TestKernel = @"
__kernel void TestKernel(__global int* A, __global int* B, __global int* C) {

	// Get the index of the current element
	int i = get_global_id(0);

	// Do the operation
	C[i] = A[i] + B[i];
}
";

}
