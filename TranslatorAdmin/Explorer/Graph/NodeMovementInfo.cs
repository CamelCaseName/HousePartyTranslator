namespace Translator.Desktop.Explorer.Graph
{
    public record struct NodeMovementInfo(int Index, float PosX, float PosY)
    {
        public static implicit operator (int lockedNodeIndex, float movedNodePos_X, float movedNodePos_Y)(NodeMovementInfo value) => (value.Index, value.PosX, value.PosY);
        public static implicit operator NodeMovementInfo((int lockedNodeIndex, float movedNodePos_X, float movedNodePos_Y) value) => new(value.lockedNodeIndex, value.movedNodePos_X, value.movedNodePos_Y);
    }
}
