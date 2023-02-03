
__kernel void layout_kernel(
	float4 parameters /*first is edge length, second attraction, third repulsion
						and fourth gravity */
	,
	__global float4* node_pos /* first 2 is pos, 3rd locked and 4th is mass */,
	__global float4* new_node_pos /* first 2 contain pos */,
	__global int* node_parents /*contains the indices of a nodes parents*/,
	__global int* node_parent_offset /*starting index for parents for a node */,
	__global int* node_parent_count /*count of the parents for each node */,
	__global int* node_childs /*contains the indices of a nodes childs */,
	__global int* node_child_offset /*starting index for childs for a node */,
	__global int* node_child_count /*count of the childs for each node */,
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
	float radius = half_powr((float)actual_node_count / parameters.x, 2.5f);

	// kind of a setup, get the data for the node that is our "first"
	// get the pos of out node without the mass
	float4 this_node_pos = node_pos[global_i];
	float this_node_locked = this_node_pos.z;
	float this_node_mass = this_node_pos.w;
	this_node_pos *= pos_mask;

	// offset pointers to edges so it points to our links in the 1d array
	__global int* this_node_parents =
		node_parents + (node_parent_offset[global_i] * sizeof(int));
	__global int* this_node_childs =
		node_childs + (node_child_offset[global_i] * sizeof(int));

	// edge counts so we only read our own edges
	int this_node_parent_count = node_parent_count[global_i];
	int this_node_child_count = node_child_count[global_i];

	// calculate "gravity"
	float diff = length(this_node_pos) - radius;
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

			// skip calculations and move out of the way if we are on the same pos,
			// would lead to infinite repulsion :)
			if (other_node_pos.x == this_node_pos.x &&
				other_node_pos.y == this_node_pos.y) {
				this_node_pos_delta.x += 0.1f;
				continue;
			}

			// get edge
			float4 edge = this_node_pos - other_node_pos;
			// calculate repulsion
			this_node_pos_delta += (edge / dot(edge, edge)) * parameters.z;
		}
		/*
		// calculate attraction for all child edges we have
		for (int edge_i = 0; edge_i < this_node_child_count; edge_i++) {
			//get pos and weight of child
			int child_global_i = this_node_childs[edge_i];
			float4 child_node_pos = node_pos[child_global_i];
			float child_mass = child_node_pos.w;

			//filter out weight and lock status
			child_node_pos *= pos_mask;
			//get edge
			float4 child_edge = this_node_pos - child_node_pos;
			// calculate attraction
			float4 attraction_vec = normalize(child_edge) * parameters.y *
				(length(child_edge) - parameters.x);

			// scale attraction by mass and add to forces of us and child
			this_node_pos_delta -= attraction_vec / this_node_mass;
			new_node_pos[child_global_i] += (attraction_vec / child_mass) * thread_inhibitor;
		}

		// repeat for all parent edges we have
		for (int edge_i = 0; edge_i < this_node_parent_count; edge_i++) {
			//get pos and weight of parent
			int parent_global_i = this_node_childs[edge_i];
			float4 parent_node_pos = node_pos[parent_global_i];
			float parent_mass = parent_node_pos.w;

			//filter out weight and lock status
			parent_node_pos *= pos_mask;
			//get edge
			float4 parent_edge = this_node_pos - parent_node_pos;
			// calculate attraction
			float4 attraction_vec = normalize(parent_edge) * parameters.y *
				(length(parent_edge) - parameters.x);

			// scale attraction by mass and add to forces of us and parent
			this_node_pos_delta -= attraction_vec / this_node_mass;
			new_node_pos[parent_global_i] += (attraction_vec / parent_mass) * thread_inhibitor;
		}
		*/
		// wait on other threads so we continue concurrently
		barrier(CLK_LOCAL_MEM_FENCE);
	}

	// apply forces to return channel
	new_node_pos[global_i] += 
		this_node_pos_delta * thread_inhibitor * (1.0f - this_node_locked) +
		(float4)(0.0f, 0.0f, this_node_locked, 0.0f) +
		(float4)(0.0f, 0.0f, 0.0f, this_node_mass);
}