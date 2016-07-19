using AutoRest.Core.Validation;
using AutoRest.Swagger.Model;

namespace AutoRest.Swagger.Validation
{
    public static class SwaggerModelerExtensions
    {
        public static ServiceDefinition GetServiceDefinition(this Rule rule) => rule.Context.Root as ServiceDefinition;
    }
}
