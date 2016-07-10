// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using AutoRest.Core.Validation;
using AutoRest.Swagger.Model;

namespace AutoRest.Swagger.Validation
{
    public class OnlyOneBodyParameter : TypedRule<Operation>
    {
        public override bool IsValid(Operation entity)
        {
            bool valid = true;

            if (entity != null && entity.Parameters != null)
            {
                var bodyParameters = new HashSet<string>();

                foreach (var param in entity.Parameters)
                {
                    if (param.In == ParameterLocation.Body)
                        bodyParameters.Add(param.Name);
                    if (param.Reference != null)
                    {
                        /*
                        TODO: get all parameters routed into this class
                        var pRef = FindReferencedParameter(param.Reference, Parameters);
                        if (pRef != null && pRef.In == ParameterLocation.Body)
                        {
                            bodyParameters.Add(pRef.Name);
                        }
                        */
                    }
                }

                if (bodyParameters.Count > 1)
                {
                    valid = false;
                }
                //formatParams = new object[] { string.Join(",", bodyParameters) };
            }

            return valid;
        }

        /*
        private static SwaggerParameter FindReferencedParameter(string reference, IDictionary<string, SwaggerParameter> parameters)
        {
            if (reference != null && reference.StartsWith("#", StringComparison.Ordinal))
            {
                var parts = reference.Split('/');
                if (parts.Length == 3 && parts[1].Equals("parameters"))
                {
                    SwaggerParameter p = null;
                    if (parameters.TryGetValue(parts[2], out p))
                    {
                        return p;
                    }
                }
            }

            return null;
        }
        */


        public override ValidationExceptionName Exception
        {
            get
            {
                return ValidationExceptionName.OnlyOneBodyParameterAllowed;
            }
        }
    }
}
