// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Validation;
using AutoRest.Swagger.Model;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AutoRest.Swagger.Validation
{
    internal static class ParameterExtensions
    {
        /// <summary>
        /// Returns the set of reference short names to parameters in the form ["Foo", "Bar"], etc.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal static IEnumerable<string> ParameterReferenceNames(this IEnumerable<SwaggerParameter> parameters)
        {
            return parameters?.Where(p => !string.IsNullOrEmpty(p.Reference)).Select(p => p.Reference.ShortName())
                ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// Returns the set of parameters that are located in the body for a set of parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal static IEnumerable<SwaggerParameter> BodyParameters(this IEnumerable<SwaggerParameter> parameters)
            => parameters?.Where(p => p.In == ParameterLocation.Body) ?? Enumerable.Empty<SwaggerParameter>();

        /// <summary>
        /// Finds the parameters in a dictionary whose keys are in the provided set of strings
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="references"></param>
        /// <returns></returns>
        internal static IEnumerable<SwaggerParameter> SelectValuesByKeys(this Dictionary<string, SwaggerParameter> parameters, IEnumerable<string> references)
            => parameters?.Where(k => references.Contains(k.Key)).Select(k => k.Value) ?? Enumerable.Empty<SwaggerParameter>();

        /// <summary>
        /// Shortens a reference in the form of "#/parameters/{referenceName}" to "referenceName"
        /// </summary>
        /// <param name="reference">The full reference</param>
        /// <returns>The reference name</returns>
        private static string ShortName(this string reference)
        {
            return reference.Substring(Math.Max(reference.LastIndexOf('/') + 1, 0));
        }

        /// <summary>
        /// Takes a collection of references in the form ["#/parameters/foo"] and returns the set of referenced ServiceDefinition SwaggerParameter
        /// </summary>
        /// <param name="references">The collection of references to dereference</param>
        /// <returns></returns>
        internal static IEnumerable<SwaggerParameter> DeRefParameters(this ServiceDefinition definition, IEnumerable<string> references)
        {
            // Get all service definition parameter key/value pairs
            return definition?.Parameters?
                // that are in the set of references
                .SelectValuesByKeys(references)
            ?? Enumerable.Empty<SwaggerParameter>();
        }
    }

    public class OneBodyParameter : TypedRule<IList<SwaggerParameter>>
    {
        /// <summary>
        /// An <paramref name="entity"/> fails this rule if it has more than one body parameter (including ones that are specified by $ref)
        /// </summary>
        /// <param name="entity">The entity to validate</param>
        /// <returns></returns>
        public override bool IsValid(IList<SwaggerParameter> entity, out object[] formatParameters)
        {
            if (entity == null)
            {
                formatParameters = new object[0];
                return true;
            }

            // Get body parameters for this operation
            var operationBodyParameters = entity.BodyParameters();

            // Get the names of parameters referenced by this operation
            var references = entity.ParameterReferenceNames();

            // Get the global body parameters from the ServiceDefinition that are referenced by the operation
            var referencedBodyParameters = this.GetServiceDefinition().DeRefParameters(references).BodyParameters();

            List<string> names = new List<string>();
            names.AddRange(operationBodyParameters.Select(p => p.Name));
            names.AddRange(referencedBodyParameters.Select(p => p.Name + " (global)"));
            formatParameters = new string[] { string.Join(", ", names) };

            // Make sure the total is not greater than 1
            return operationBodyParameters.Count() + referencedBodyParameters.Count() <= 1;
        }

        public override ValidationExceptionName Exception => ValidationExceptionName.OneBodyParameterPermitted;
    }
}
