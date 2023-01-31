
__kernel void layout_kernel(
    float4 parameters /*first is edge length, second attraction, third repulsion
                        and fourth gravity */
    ,
    __global float4 *node_pos /* first 2 is pos, 3rd locked and 4th is mass */,
    __global float4 *new_node_pos /* first 2 contain pos */,
    __global int *node_parents /*contains the indices of a nodes parents*/,
    __global int *node_parent_offset /*starting index for parents for a node */,
    __global int *node_parent_count /*count of the parents for each node */,
    __global int *node_childs /*contains the indices of a nodes childs */,
    __global int *node_child_offset /*starting index for childs for a node */,
    __global int *node_child_count /*count of the childs for each node */,
    __local float4 *node_buffer /*the forces for each node*/) {

  __constant float4 pos_mask = (float4)(1.0f, 1.0f, 0.0f, 0.0f);

  // Get the index of the current node
  int global_i = get_global_id(0);
  int global_node_count = get_global_size(0);
  float radius = half_sqrt((float)global_node_count) + parameters.x * 2;
  int local_i = get_local_id(0);
  int local_node_count = get_local_size(0);
  int work_block_count = global_node_count / local_node_count;

  // kind of a setup, get the data for the node that is our "first"
  // get the pos of out node without the mass
  float4 this_node_pos = node_pos[global_i] * pos_mask;
  float this_node_locked = node_pos[global_i].z;
  float this_node_mass = node_pos[global_i].w;
  // offset pointers to edges so it points to our links in the 1d array
  int *this_node_parents =
      node_parents + (node_parent_offset[global_i] * sizeof(int *));
  int *this_node_childs =
      node_childs + (node_child_offset[global_i] * sizeof(int *));
  // edge counts so we only read our own edges
  int this_node_parent_count = node_parent_count[global_i];
  int this_node_child_count = node_child_count[global_i];
  float4 this_node_pos_delta = (float4)(0.0f, 0.0f, 0.0f, 0.0f);

  // perform the calculation
  for (int j_block = 0; j_block < work_block_count; j_block++) {
    // cache one node position to work on
    node_buffer[local_i] = node_pos[j_block * local_node_count + local_i];

    // calculate "gravity"
    float diff = fast_length(this_node_pos) - radius;
    this_node_pos_delta -= fast_normalize(this_node_pos) *
                           pow(abs((int)diff), 1.5f) * sign(diff) *
                           parameters.w;

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
        other_node_pos += (float4)(0.1f, 0.0f, 0.0f, 0.0f);
        continue;
      }

      // get edge
      float4 edge = this_node_pos - other_node_pos;
      // calculate repulsion
      this_node_pos_delta += (edge / dot(edge.x, edge.y)) * parameters.z;
    }

    // calculate attraction for all child edges we have
    for (int edge_i = 0; edge_i < this_node_child_count; edge_i++) {
      float4 child_node_pos = node_pos[this_node_childs[edge_i]] * pos_mask;
      float4 child_edge = this_node_pos - child_node_pos;
      // calculate attraction
      float4 attraction_vec = normalize(child_edge) * parameters.y *
                              (length(child_edge) - parameters.x);

      // scale attraction by mass and add to forces
      this_node_pos_delta -= attraction_vec / this_node_mass;
    }

    // repeat for all parent edges we have
    for (int edge_i = 0; edge_i < this_node_parent_count; edge_i++) {
      float4 parent_node_pos = node_pos[this_node_parents[edge_i]] * pos_mask;
      float4 parent_edge = this_node_pos - parent_node_pos;
      // calculate attraction
      float4 attraction_vec = normalize(parent_edge) * parameters.y *
                              (length(parent_edge) - parameters.x);

      // scale attraction by mass and add to forces
      this_node_pos_delta -= attraction_vec / this_node_mass;
    }

    // wait on other threads so we continue concurrently
    barrier(CLK_LOCAL_MEM_FENCE);
  }

  // apply forces to return channel
  new_node_pos[global_i] =
      this_node_pos + (this_node_pos_delta * (1.0f - this_node_locked));
}