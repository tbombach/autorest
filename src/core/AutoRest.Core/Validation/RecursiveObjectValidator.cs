// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoRest.Core.Utilities;
using AutoRest.Core.Utilities.Collections;

namespace AutoRest.Core.Validation
{
    /// <summary>
    ///     A validator that traverses an object graph, applies validation rules, and logs validation messages
    /// </summary>
    public class RecursiveObjectValidator
    {
        private const string ROOT_PATH_INDICATOR = "#";

        public IEnumerable<ValidationMessage> GetValidationExceptions(object entity)
        {
            return RecursiveValidate(entity, new RuleContext(entity), Enumerable.Empty<Rule>()).Select(m => m.AppendToPath(ROOT_PATH_INDICATOR));
        }

        private IEnumerable<ValidationMessage> RecursiveValidate(object entity, RuleContext parentContext)
        {
            return RecursiveValidate(entity, parentContext, Enumerable.Empty<Rule>());
        }

        private IEnumerable<ValidationMessage> RecursiveValidate(object entity, RuleContext parentContext, IEnumerable<Rule> rules)
        {
            var messages = Enumerable.Empty<ValidationMessage>();
            if (entity == null)
            {
                return messages;
            }

            // ensure that the rules can be re-enumerated without re-evaluating the enumeration.
            var collectionRules = rules.ReEnumerable();

            var list = entity as IList;
            var dictionary = entity as IDictionary;
            if (list != null)
            {
                // Recursively validate each list item and add the 
                // item index to the location of each validation message
                var listMessages = list.SelectMany((item, index)
                    => RecursiveValidate(item, parentContext.CreateChild(item, index), collectionRules).Select(each
                        => each.AppendToPath($"[{index}]")));
                messages = messages.Concat(listMessages);
            }

            else if (dictionary != null)
            {
                if (!dictionary.IsValidatableDictionary())
                {
                    return Enumerable.Empty<ValidationMessage>();
                }

                // Recursively validate each dictionary entry and add the entry 
                // key to the location of each validation message
                var dictMessages = dictionary.SelectMany((key, value)
                    => RecursiveValidate(value, parentContext.CreateChild(value, (string)key), collectionRules).Select(each
                        => each.AppendToPath((string)key)));
                messages = messages.Concat(dictMessages);
            }

            // If this is a class, validate its value and its properties.
            else if (entity.GetType().IsClass && entity.GetType() != typeof(string))
            {
                // Validate each property of the object
                var propertyMessages = entity.GetValidatableProperties()
                    .SelectMany(p => ValidateProperty(p, p.GetValue(entity), parentContext));

                // Return the messages for both this value and its properties.
                messages = messages.Concat(propertyMessages);
            }

            // Validate the value of the object itself
            var valueMessages = ValidateObjectValue(entity, collectionRules, parentContext);
            return messages.Concat(valueMessages);
        }

        private IEnumerable<ValidationMessage> ValidateObjectValue(object entity,
            IEnumerable<Rule> collectionRules, RuleContext parentContext)
        {
            // Get any rules defined for the class of the entity
            var classRules = entity.GetType().GetValidationRules();

            // Combine the class rules with any rules that apply to the collection that the entity is part of
            classRules = collectionRules.Concat(classRules);

            // Apply each rule for the entity
            return classRules.SelectMany(rule => rule.GetValidationMessages(entity, parentContext));
        }

        private IEnumerable<ValidationMessage> ValidateProperty(PropertyInfo prop, object value, RuleContext parentContext)
        {
            // Get any rules defined on this property and any defined as applying to the collection
            var propertyRules = prop.GetValidationRules();
            var collectionRules = prop.GetValidationCollectionRules();

            // Validate the value of this property against any rules for it
            var propertyMessages = propertyRules.SelectMany(r => r.GetValidationMessages(value, parentContext)).Select(e => e.AppendToPath(prop.Name));

            // Recursively validate the children of the property (passing any rules that apply to this collection)
            var childrenMessages = RecursiveValidate(value, parentContext.CreateChild(value, prop.Name), collectionRules).Select(e => e.AppendToPath(prop.Name));

            return propertyMessages.Concat(childrenMessages);
        }
    }
}