namespace AutoRest.Core.Validation
{
    public class Node
    {
        public object Value { get; private set; }

        public Node Parent { get; private set; }

        public object Root { get; private set; }

        public Node(object value) : this(value, null)
        {
            Root = value;
        }

        public Node(object value, Node parent)
        {
            this.Parent = parent;
            this.Root = parent?.Root;
            this.Value = value;
        }
    }
}
