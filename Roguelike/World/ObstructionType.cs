namespace Roguelike.World
{
    public enum ObstructionType
    {
        /// <summary>
        /// Can walk through, can see through
        /// </summary>
        Open,

        /// <summary>
        /// Cannot walk through, can see through
        /// </summary>
        MovementBlocked,

        /// <summary>
        /// Can walk through, cannot see through
        /// </summary>
        VisionBlocked,

        /// <summary>
        /// Cannot walk through, Cannot see through
        /// </summary>
        FullyBlocked
    }
}
