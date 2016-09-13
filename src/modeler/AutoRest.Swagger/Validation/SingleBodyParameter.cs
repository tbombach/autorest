// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// 

using AutoRest.Core.Logging;
using AutoRest.Core.Properties;
using System.Linq;
using AutoRest.Core.Validation;
using AutoRest.Swagger.Model;
using System.Collections.Generic;
using System;

namespace AutoRest.Swagger.Validation
{
    public class SingleBodyParameter : TypedRule<IList<SwaggerParameter>>
    {
        /// <summary>
        /// An <paramref name="entity" /> fails this rule if it 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="context"></param>
        /// <param name="formatParameters">The output</param>
        /// <returns></returns>
        public override bool IsValid(IList<SwaggerParameter> parameters, RuleContext context, out object[] formatParameters)
        {
            var definition = context.GetServiceDefinition();
            var resolver = new ParameterResolver(definition);
            var bodyParameters = new List<string>();

            // Go through each
            foreach (var param in parameters)
            {
                try
                {
                SwaggerParameter parameter = null;
                    parameter = resolver.Unwrap(param);
                    // If it's a body parameter, add it to our list
                    if (parameter != null && parameter.In == ParameterLocation.Body)
                    {
                        bodyParameters.Add(parameter.Name);
                    }
                }
                catch (ArgumentException)
                {
                    // If the parameter refrence didn't exist, skip it
                }
            }

            // If we ended up with too many body parameters, this rule fails
            if (parameters.Count(p => p.In == ParameterLocation.Body) > 1)
            {
                formatParameters = new string[] { string.Join(", ", bodyParameters) };
                return false;
            }
            formatParameters = new string[0];
            return true;
        }

        /// <summary>
        ///     The template message for this Rule.
        /// </summary>
        /// <remarks>
        ///     This may contain placeholders '{0}' for parameterized messages.
        /// </remarks>
        public override string MessageTemplate => "Only one body parameter is allowed. The following body parameters were found: {0}";

        /// <summary>
        ///     The severity of this message (ie, debug/info/warning/error/fatal, etc)
        /// </summary>
        public override LogEntrySeverity Severity => LogEntrySeverity.Error;
    }
}