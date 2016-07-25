using Newtonsoft.Json;
using System.Linq;
using System.Reflection;

namespace AutoRest.Core.Validation
{
    public static class PropertyNameResolver
    {
        /// <summary>
        /// Returns the name specified by a JsonProperty attribute if it exists, otherwise the property name
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static string JsonName(PropertyInfo prop)
            => prop?.GetCustomAttributes<JsonPropertyAttribute>(true).Select(p => p.PropertyName).FirstOrDefault()
            ?? prop.Name;

        public static string PropertyName(PropertyInfo prop) => prop.Name;
    }
}
