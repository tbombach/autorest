using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoRest.Core.Utilities.Collections;
using Newtonsoft.Json;
using AutoRest.Core.Utilities;

namespace AutoRest.Core.Validation
{
    internal static class RulesExtensions
    {
        private static readonly Type JsonExtensionDataType = typeof(JsonExtensionDataAttribute);

        /// <summary>
        ///     Gets an enumerable of properties for <paramref name="entity" /> that can be validated
        /// </summary>
        /// <param name="entity">The object to get properties for</param>
        /// <returns></returns>
        internal static IEnumerable<PropertyInfo> GetValidatableProperties(this object entity)
        {
            if (entity == null)
            {
                return Enumerable.Empty<PropertyInfo>();
            }
            return entity.GetType()
                .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => !Attribute.IsDefined(prop, JsonExtensionDataType))
                .Where(prop => prop.PropertyType != typeof(object));
        }

        /// <summary>
        ///     Determines if a dictionary can be validated by running rules
        /// </summary>
        /// <param name="entity">The object to check</param>
        /// <returns></returns>
        internal static bool IsValidatableDictionary(this object entity)
        {
            if (entity == null)
            {
                return false;
            }
            // Dictionaries of type <string, object> cannot be validated, because the object could be infinitely deep.
            // We only want to validate objects that have strong typing for the value type
            var dictType = entity.GetType();
            return dictType.IsGenericType &&
                   dictType.GenericTypeArguments.Count() >= 2 &&
                   dictType.GenericTypeArguments[1] != typeof(object);
        }

        public static IEnumerable<Rule> GetValidationRules(this PropertyInfo property)
        {
            var propertyRules = property.GetCustomAttributes<RuleAttribute>(true).Select(each => each.Rule).ReEnumerable();
            if (propertyRules.IsNullOrEmpty())
            {
                propertyRules = DefaultRules.ReEnumerable();
            }
            return propertyRules.Concat(UniversalRules).ReEnumerable();
        }

        public static IEnumerable<Rule> GetValidationCollectionRules(this PropertyInfo property)
        {
            var collectionRules = property.GetCustomAttributes<CollectionRuleAttribute>(true).Select(each => each.Rule).ReEnumerable();
            return collectionRules.Concat(UniversalRules).ReEnumerable();
        }

        public static IEnumerable<Rule> GetValidationRules(this Type type)
        {
            return type.GetCustomAttributes<RuleAttribute>(true).Select(each => each.Rule).ReEnumerable();
        }

        /// <summary>
        ///     The collection of default rules applies to all properties that do not define rules
        ///     These can impose global conditions on any property that has not been otherwise validated.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Rule> DefaultRules = new[] { new MissingValidator() };

        /// <summary>
        ///     The collection of rules that apply to all properties regardless of other rules.
        /// </summary>
        public static IEnumerable<Rule> UniversalRules = new[] { new NoControlCharacters() };
    }
}