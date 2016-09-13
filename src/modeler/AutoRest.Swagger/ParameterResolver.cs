// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AutoRest.Swagger.Model;
using AutoRest.Swagger.Properties;

namespace AutoRest.Swagger
{
    /// <summary>
    /// Methods for normalizing and evaluating swagger schemas in their context in a swagger spec
    /// </summary>
    public class ParameterResolver : ICloneable
    {
        private readonly ServiceDefinition _serviceDefinition;

        /// <summary>
        /// Create a new schema resolver in the context of the given swagger spec
        /// </summary>
        /// <param name="modeler">The swagger spec modeler</param>
        public ParameterResolver(SwaggerModeler modeler)
        {
            if (modeler == null)
            {
                throw new ArgumentNullException("modeler");
            }

            _serviceDefinition = modeler.ServiceDefinition;
        }

        /// <summary>
        /// Create a new schema resolver in the context of the given swagger spec
        /// </summary>
        /// <param name="modeler">The swagger spec modeler</param>
        public ParameterResolver(ServiceDefinition definition)
        {
            _serviceDefinition = definition;
        }

        /// <summary>
        /// Copy the current context - used to maintain the schema evaluation context when following 
        /// multiple chains of schema references.
        /// </summary>
        /// <returns>A schema resolver at the same depth as the current resolver.</returns>
        public object Clone()
        {
            var resolver = new ParameterResolver(_serviceDefinition);
            return resolver;
        }

        /// <summary>
        /// Unwraps a SwaggerParameter by finding 
        /// </summary>
        /// <param name="schema">The schema to normalize</param>
        /// <returns>A normalized swagger schema</returns>
        public SwaggerParameter Unwrap(SwaggerParameter swaggerParameter)
        {
            if (swaggerParameter == null)
            {
                throw new ArgumentNullException("swaggerParameter");
            }

            // If referencing global parameters serializationProperty
            if (swaggerParameter.Reference != null)
            {
                string referenceKey = swaggerParameter.Reference.StripParameterPath();
                if (!_serviceDefinition.Parameters.ContainsKey(referenceKey))
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.InvariantCulture,
                        Resources.DefinitionDoesNotExist, referenceKey));
                }

                swaggerParameter = _serviceDefinition.Parameters[referenceKey];
            }

            return swaggerParameter;
        }
    }
}