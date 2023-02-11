
__kernel void edge_kernel(
	float4 parameters /*first is edge length, second attraction, third repulsion
						and fourth gravity */
	,
	__global float4* node_pos /* first 2 is pos, 3rd locked and 4th is mass */,
	__global float4* new_node_pos /* first 2 contain pos */,
	__global int* this_index,
	__global int* child_index,
	int actual_edge_count /*count of all nodes, so that we dont run oob*/) {

	__constant float4 pos_mask = (float4)(1.0f, 1.0f, 0.0f, 0.0f);
	float thread_inhibitor = 1.0f;

	// Get the index of the current node
	int global_i = get_global_id(0);
	// fix for the iterations we run too much, for filling up every warp we 
	// run some empty and its faster to keep it parallel
	if (global_i >= actual_edge_count) {
		global_i = 0;
		thread_inhibitor = 0.0f;
	}

	int global_node_count = get_global_size(0);
	int local_i = get_local_id(0);
	int local_node_count = get_local_size(0);
	int work_block_count = global_node_count / local_node_count;
	float radius = half_sqrt((float)actual_node_count) * parameters.x / 2;

	// kind of a setup, get the data for the node that is our "first"
	// get the pos of out node without the mass
	float4 this_node_pos = node_pos[global_i];
	float this_node_locked = this_node_pos.z;
	float this_node_mass = this_node_pos.w;
	//fix up pos to be only the position
	this_node_pos *= pos_mask;

	// wait on other threads so we continue concurrently
	barrier(CLK_LOCAL_MEM_FENCE);

	// apply forces to return channel
	new_node_pos[global_i] +=
		this_node_pos_delta * thread_inhibitor * (1.0f - this_node_locked);
}