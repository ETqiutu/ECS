namespace LiteECS
{
    public abstract class Component
    {
        public Entity Entity { get; internal set; }
        public bool Enabled { get; set; } = true;
    }
}