using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Core.Validation
{
    /// <summary>
    /// Represents a level of depth when traversing an object tree. It lets collection rules pass rules to items
    /// </summary>
    public class TraversalDepth
    {
        public IEnumerable<RuleAttribute> CollectionRules { get; set; }

        public RuleContext RuleContext { get; set; }

        public TraversalDepth(RuleContext context) : this(context, Enumerable.Empty<RuleAttribute>())
        {
        }

        public TraversalDepth(RuleContext context, IEnumerable<RuleAttribute> collectionRules)
        {
            CollectionRules = collectionRules;
            RuleContext = context;
        }
    }

}
