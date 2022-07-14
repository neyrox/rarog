namespace Engine
{
    public abstract class Node
    {
        public abstract bool NeedWriterLock { get; }
        public abstract Result Execute(Database db);
    }
}
