
__kernel void edge_kernel(
	float4 parameters /*first is edge length, second attraction, third repulsion
											and fourth gravity */
	,
	__global float4* node_pos /* first 2 is pos, 3rd locked and 4th is mass */,
	__global float4* new_node_pos /* first 2 contain pos */,
	__global int* this_index,
	__global int* child_index) {

	__constant float4 pos_mask = (float4)(1.0f, 1.0f, 0.0f, 0.0f);

	// Get the index of the current node
	int global_i = get_global_id(0);

	int node_i = this_index[global_i];
	int child_i = child_index[global_i];

	if (node_i >= 0) {

		// kind of a setup, get the data for the node that is our "first"
		// get the pos of out node without the mass, same for child
		float4 this_node_pos = node_pos[node_i];
		float this_node_locked = this_node_pos.z;
		float this_node_mass = fmax(this_node_pos.w, 1.0f);
		float4 child_node_pos = node_pos[child_i];
		float child_node_locked = child_node_pos.z;
		float child_node_mass = fmax(child_node_pos.w, 1.0f);
		// fix up pos to be only the position
		this_node_pos *= pos_mask;
		child_node_pos *= pos_mask;

		//get edge
		float4 child_edge = this_node_pos - child_node_pos;

		// calculate attraction
		float4 attraction_vec = normalize(child_edge) * parameters.y *
			(length(child_edge) - parameters.x);

		// apply forces to return channel
		new_node_pos[node_i] -= (attraction_vec / this_node_mass) * (1.0f - this_node_locked);
		new_node_pos[child_i] += (attraction_vec / child_node_mass) * (1.0f - child_node_locked);
	}

	// wait on other threads so we continue concurrently
	barrier(CLK_LOCAL_MEM_FENCE);
}