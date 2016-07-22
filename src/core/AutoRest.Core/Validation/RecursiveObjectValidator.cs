﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
            return RecursiveValidate(entity, Enumerable.Empty<Rule>()).Select(m => m.AppendToPath(ROOT_PATH_INDICATOR));
        }

        private IEnumerable<ValidationMessage> RecursiveValidate(object entity, IEnumerable<Rule> rules)
        {
            if (entity == null)
            {
                return Enumerable.Empty<ValidationMessage>();
            }

            // ensure that the rules can be re-enumerated without re-evaluating the enumeration.
            var collectionRules = rules.ReEnumerable();

            var list = entity as IList;
            if (list != null)
            {
                // Recursively validate each list item and add the 
                // item index to the location of each validation message
                return list.SelectMany((item, index)
                    => RecursiveValidate(item, collectionRules).Select(each
                        => each.AppendToPath($"[{index}]")));
            }

            var dictionary = entity as IDictionary;
            if (dictionary != null)
            {
                if (!dictionary.IsValidatableDictionary())
                {
                    return Enumerable.Empty<ValidationMessage>();
                }

                // Recursively validate each dictionary entry and add the entry 
                // key to the location of each validation message
                return dictionary.SelectMany((key, value)
                    => RecursiveValidate(value, collectionRules).Select(each
                        => each.AppendToPath((string)key)));
            }

            // if this is a class, validate it's value and it's properties.
            if (entity.GetType().IsClass && entity.GetType() != typeof(string))
            {
                // Validate each property of the object
                var propertyMessages = entity.GetValidatableProperties().SelectMany(p => ValidateProperty(p, p.GetValue(entity)));

                // Validate the value of the object itself
                var valueMessages = ValidateObjectValue(entity, collectionRules);

                // Return the messages for both this value and its properties.
                return valueMessages.Concat(propertyMessages);
            }

            // validate just the value.
            return ValidateObjectValue(entity, collectionRules);
        }

        private IEnumerable<ValidationMessage> ValidateObjectValue(object entity,
            IEnumerable<Rule> collectionRules)
        {
            // Get any rules defined for the class of the entity
            var classRules = entity.GetType().GetValidationRules();

            // Combine the class rules with any rules that apply to the collection that the entity is part of
            classRules = collectionRules.Concat(classRules);

            // Apply each rule for the entity
            return classRules.SelectMany(rule => rule.GetValidationMessages(entity));
        }

        private IEnumerable<ValidationMessage> ValidateProperty(PropertyInfo prop, object value)
        {
            // Get any rules defined on this property and any defined as applying to the collection
            var propertyRules = prop.GetValidationRules();
            var collectionRules = prop.GetValidationCollectionRules();

            // Validate the value of this property against any rules for it
            var propertyMessages = propertyRules.SelectMany(r => r.GetValidationMessages(value)).Select(e => e.AppendToPath(prop.Name));

            // Recursively validate the children of the property (passing any rules that apply to this collection)
            var childrenMessages = RecursiveValidate(value, collectionRules).Select(e => e.AppendToPath(prop.Name));

            return propertyMessages.Concat(childrenMessages);
        }
    }
}