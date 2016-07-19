
namespace AutoRest.Core.Validation
{
    /// <summary>
    /// Provides context for a rule, such as the ancestors in the validation tree or the root
    /// </summary>
    public class RuleContext
    {
        /// <summary>
        /// Initializes a top level context for rules
        /// </summary>
        /// <param name="root"></param>
        public RuleContext(object root) : this(null)
        {
            this.Root = root;
        }

        /// <summary>
        /// Initializes linked context for rules
        /// </summary>
        /// <param name="root"></param>
        public RuleContext(RuleContext parent)
        {
            this.Parent = parent;
            this.Root = parent?.Root;
        }

        public RuleContext Parent { get; set; }

        public object Root { get; set; }

        public string Key { get; set; }

        public int Index { get; set; }
    }
}
