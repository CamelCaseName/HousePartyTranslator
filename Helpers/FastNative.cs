using System.Runtime.InteropServices;

namespace HousePartyTranslator.Helpers
{
    public static class FastNative
    {
#if DEBUG
        [DllImport("..\\..\\x64\\Debug\\force_directed_graph.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void do_graph_physics(
            [In, Out] int[] x,
            [In, Out] int[] y,
            [In, Out] int[] mass,
            int count,
            [In, Out] int[] node_1,
            [In, Out] int[] node_2,
            int node_connection_count);
#else
        [DllImport("force_directed_graph.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void do_graph_physics(
            [In, Out] int[] x,
            [In, Out] int[] y,
            [In, Out] int[] mass,
            int count,
            [In, Out] int[] node_1,
            [In, Out] int[] node_2,
            int node_connection_count);
#endif
    }
}
