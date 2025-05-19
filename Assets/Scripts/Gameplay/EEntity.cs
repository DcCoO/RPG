namespace BitBox.Gameplay
{
    [System.Flags]
    public enum EEntity
    {
        None = 0,
        Player = 1 << 0,
        Enemy =  1 << 1,
        Object = 1 << 2,
    }

    public static class EEntityExtensions
    {
        public static bool Contains(this EEntity entity, EEntity other)
        {
            return (entity & other) == other;
        }
    }
}
