
__kernel void nbody_kernel(
	float4 parameters /*first is edge length, second attraction, third repulsion
						and fourth gravity */
	,
	__global float4* node_pos /* first 2 is pos, 3rd locked and 4th is mass */,
	__global float4* new_node_pos /* first 2 contain pos */,
	int actual_node_count /*count of all nodes, so that we dont run oob*/,
	__local float4* node_buffer /*the forces for each node*/) {

	__constant float4 pos_mask = (float4)(1.0f, 1.0f, 0.0f, 0.0f);
	float thread_inhibitor = 1.0f;

	// Get the index of the current node
	int global_i = get_global_id(0);
	// fix for the iterations we run too much, for filling up every warp we 
	// run some empty and its faster to keep it parallel
	if (global_i >= actual_node_count) {
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

	// calculate "gravity"
	float diff = length(this_node_pos) - radius;

	//if our length is zero we must move a little or else the normalize breaks
	if (diff == -radius) {
		this_node_pos.x = fmax(this_node_pos.x, 0.0001f);
		this_node_pos.y = fmax(this_node_pos.y, 0.0001f);
	}

	//add gravity to our change we ant to make
	float4 this_node_pos_delta = -1.0f * normalize(this_node_pos) *
		sign(diff) * half_powr(fabs(diff), 1.5f) * parameters.w;


	// perform the calculation
	for (int j_block = 0; j_block < work_block_count; j_block++) {
		// cache one node position to work on
		node_buffer[local_i] = node_pos[j_block * local_node_count + local_i];

		// wait up on other threads
		barrier(CLK_LOCAL_MEM_FENCE);

		// calculate repulsion for all particles in the cache
		for (int j = 0; j < local_node_count; j++) {
			// get position of other node, without getting mass
			float4 other_node_pos = node_buffer[j] * pos_mask;

			// move out of the way if we are on the same pos,
			// would lead to infinite repulsion :)
			if (other_node_pos.x == this_node_pos.x &&
				other_node_pos.y == this_node_pos.y) {
				other_node_pos.x += 10.0f;
			}

			// get edge
			float4 edge = this_node_pos - other_node_pos;
			// calculate repulsion
			this_node_pos_delta += (edge / dot(edge, edge)) * parameters.z;
		}
		// wait up on other threads
		barrier(CLK_LOCAL_MEM_FENCE);
	}

	// apply forces to return channel
	new_node_pos[global_i] +=
		this_node_pos_delta * thread_inhibitor * (1.0f - this_node_locked);
}