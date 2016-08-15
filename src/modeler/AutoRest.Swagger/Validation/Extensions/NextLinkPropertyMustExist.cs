﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using AutoRest.Core.Logging;
using AutoRest.Core.Properties;
using AutoRest.Core.Validation;
using AutoRest.Swagger.Model;
using Newtonsoft.Json.Linq;

namespace AutoRest.Swagger.Validation
{
    public class NextLinkPropertyMustExist : PageableExtensionRule
    {
        private const string ExtensionNextLinkPropertyName = "nextLinkName";

        /// <summary>
        /// An x-ms-pageable extension passes this rule if the value for nextLinkName refers to a string property
        /// that exists on the schema for the 200 response of the parent operation.
        /// </summary>
        /// <param name="pageable"></param>
        /// <param name="context"></param>
        /// <param name="formatParameters"></param>
        /// <returns></returns>
        public override bool IsValid(object pageable, RuleContext context, out object[] formatParameters)
        {
            // Get the name of the property defined by nextLinkName
            var nextLinkPropertyName = (pageable as JContainer)?[ExtensionNextLinkPropertyName]?.ToString();
            if (!string.IsNullOrEmpty(nextLinkPropertyName))
            {
                // Get the list of properties defined for the 200 response schema
                var schemaProperties = Get200ResponseSchema(context)?.Properties;

                // Fail the rule if there's no property that has the name specified in nextLinkName
                if (schemaProperties == null || !schemaProperties.ContainsKey(nextLinkPropertyName))
                {
                    formatParameters = new string[] { nextLinkPropertyName };
                    return false;
                }
            }
            formatParameters = new string[0];
            return true;
        }

        /// <summary>
        /// The template message for this Rule. 
        /// </summary>
        /// <remarks>
        /// This may contain placeholders '{0}' for parameterized messages.
        /// </remarks>
        public override string MessageTemplate => "The property '{0}' specified by nextLinkName does not exist in the 200 response schema.";

        /// <summary>
        /// The severity of this message (ie, debug/info/warning/error/fatal, etc)
        /// </summary>
        public override LogEntrySeverity Severity => LogEntrySeverity.Warning;
    }
}
