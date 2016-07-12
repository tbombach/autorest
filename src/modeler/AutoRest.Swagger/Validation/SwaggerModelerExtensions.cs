using AutoRest.Core.Validation;
using AutoRest.Swagger.Model;

namespace AutoRest.Swagger.Validation
{
    public static class SwaggerModelerExtensions
    {
        public static ServiceDefinition GetServiceDefinition(this Rule rule)
        {
            Node currentNode = rule.Parent;
            while(currentNode != null && !(currentNode.Value is ServiceDefinition))
            {
                currentNode = currentNode.Parent;
            }
            return currentNode?.Value as ServiceDefinition;
        }
    }
}
