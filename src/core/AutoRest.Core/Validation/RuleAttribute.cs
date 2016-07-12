// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace AutoRest.Core.Validation
{
    /// <summary>
    /// An attribute that describes a rule to apply to the property or class that it decorates
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class RuleAttribute : Attribute
    {
        public Type RuleType { get; }

        public RuleAttribute(Type ruleType)
        {
            this.RuleType = ruleType;
        }

        /// <summary>
        /// Returns a collection of validation messages for <paramref name="entity"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual IEnumerable<ValidationMessage> GetValidationMessages(object entity, Node parent)
        {
            if (typeof(Rule).IsAssignableFrom(this.RuleType))
            {
                var rule = (Rule)Activator.CreateInstance(RuleType);
                rule.Parent = parent;
                if (rule != null)
                {
                    foreach (var message in rule.GetValidationMessages(entity))
                    {
                        yield return message;
                    }
                }
            }
        }
    }
}
