// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoRest.Core.Validation
{
    /// <summary>
    /// A validator that traverses an object graph, applies validation rules, and logs validation messages
    /// </summary>
    public class RecursiveObjectValidator
    {
        private object RootEntity;

        public RecursiveObjectValidator(object entity)
        {
            RootEntity = entity;
        }

        public IEnumerable<ValidationMessage> GetValidationExceptions()
        {
            var rootRuleContext = new RuleContext(RootEntity);
            return RecursiveValidate(RootEntity, new TraversalDepth(rootRuleContext));
        }

        private IEnumerable<ValidationMessage> ValidateListItem(dynamic item, int index, TraversalDepth parentLevel)
        {
            var ruleContext = new RuleContext(parentLevel.RuleContext) { Index = index };
            var currentDepth = new TraversalDepth(ruleContext, parentLevel.CollectionRules);
            return RecursiveValidate(item, currentDepth);
        }

        private IEnumerable<ValidationMessage> ValidateDictionaryEntry(KeyValuePair<string, dynamic> kvp, TraversalDepth parentLevel)
        {
            var ruleContext = new RuleContext(parentLevel.RuleContext) { Key = kvp.Key };
            var currentLevel = new TraversalDepth(ruleContext, parentLevel.CollectionRules);
            return RecursiveValidate(kvp.Value, currentLevel);
        }

        private IEnumerable<ValidationMessage> ValidateProp(PropertyInfo prop, object propValue, TraversalDepth parentLevel)
        {
            var messages = Enumerable.Empty<ValidationMessage>();

            // Get any rules defined on this property and apply them to the property value
            var propRules = prop.GetCustomAttributes<RuleAttribute>(true);
            var collectionRules = prop.GetCustomAttributes<RuleAttribute>(true);

            var ruleContext = new RuleContext(parentLevel.RuleContext) { Key = prop.Name };
            var currentLevel = new TraversalDepth(ruleContext, collectionRules);

            // Get any rules defined on this property and apply them to the property value
            var propValueMessages = propRules.SelectMany(r => r.GetValidationMessages(propValue, currentLevel.RuleContext));
            var propChildrenMessages = RecursiveValidate(propValue, currentLevel);

            // return the messages
            messages.Concat(propValueMessages);
            messages.Concat(propChildrenMessages);
            return messages;
        }

        private IEnumerable<ValidationMessage> ValidateObject(object entity, TraversalDepth currentLevel)
        {
            // Get any rules defined for the class of the entity
            var classRules = entity.GetType().GetCustomAttributes<RuleAttribute>(true);
            // Combine the class rules with any rules that apply to the collection that the entity is part of
            classRules = currentLevel.CollectionRules.Concat(classRules);
            // Apply each rule for the entity
            return classRules.SelectMany(rule => rule.GetValidationMessages(entity, currentLevel.RuleContext));
        }

        private IEnumerable<ValidationMessage> RecursiveValidate(object entity, TraversalDepth parentLevel)
        {
            var messages = Enumerable.Empty<ValidationMessage>();
            if (entity != null)
            {
                if (entity is IList)
                {
                    // Transform the object into a list we can work with
                    IList<dynamic> list = ((IList)entity).Cast<dynamic>().ToList();

                    // Validate each list item
                    var listMessages = list?.SelectMany<dynamic, ValidationMessage>((l, i) => ValidateListItem(l, i, parentLevel));

                    // Add the messages from the list items to our result
                    messages = messages.Concat(listMessages ?? Enumerable.Empty<ValidationMessage>());
                }
                else if (entity is IDictionary)
                {
                    // Transform the object into a dictionary we can work with
                    IDictionary<string, dynamic> dict = ((IDictionary)entity).Cast<dynamic>().ToDictionary(entry => (string)entry.Key, entry => entry.Value);

                    // Validate each dictionary entry
                    var dictMessages = dict?.SelectMany(k => ValidateDictionaryEntry(k, parentLevel)).ToList();

                    // Add the messages from the dictionary entry to our result
                    messages = messages.Concat(dictMessages ?? Enumerable.Empty<ValidationMessage>());
                }
                else if (entity.GetType().IsClass && entity.GetType() != typeof(string))
                {
                    // Get all of the properties of the object to validate
                    var properties = entity.GetValidatableProperties();

                    // Validate each of the properties and the object value itself
                    var propertiesMessages = properties.SelectMany(prop => ValidateProp(prop, prop.GetValue(entity), parentLevel));
                    var entityMessages = ValidateObject(entity, parentLevel);

                    // Add the messages from the object and its properties to the result
                    messages = messages.Concat(entityMessages);
                    messages = messages.Concat(propertiesMessages);
                }
            }
            return messages;
        }
    }

    internal static class RulesExtensions
    {
        private static readonly Type JsonExtensionDataType = typeof(JsonExtensionDataAttribute);

        /// <summary>
        /// Gets an enumerable of properties for <paramref name="entity"/> that can be validated
        /// </summary>
        /// <param name="entity">The object to get properties for</param>
        /// <returns></returns>
        internal static IEnumerable<PropertyInfo> GetValidatableProperties(this object entity)
        {
            var properties = entity?.GetType().GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
                         .Where(prop => !Attribute.IsDefined(prop, JsonExtensionDataType))
                         .Where(prop => prop.PropertyType != typeof(object));
            return properties ?? Enumerable.Empty<PropertyInfo>();
        }

        /// <summary>
        /// Determines if a dictionary can be validated by running rules
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
    }
}
