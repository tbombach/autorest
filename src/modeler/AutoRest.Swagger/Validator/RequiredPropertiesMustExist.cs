// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Validation;
using AutoRest.Swagger.Model;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Swagger.Validation
{
    public class RequiredPropertiesMustExist : TypedRule<Schema>
    {
        public override IEnumerable<ValidationMessage> GetValidationMessages(Schema entity)
        {
            // TODO: this doesn't take into account allOf, $ref or extends. This needs to take into account all
            // possible properties for this schema
            if (entity != null && entity.Required != null)
            {
                foreach (var req in entity.Required.Where(r => !string.IsNullOrEmpty(r)))
                {
                    Schema value = null;
                    if (entity.Properties == null || !entity.Properties.TryGetValue(req, out value))
                    {
                        yield return CreateException(Exception, req);
                    }
                }
            }
            yield break;
        }

        public override ValidationExceptionName Exception
        {
            get
            {
                return ValidationExceptionName.RequiredPropertiesMustExist;
            }
        }
    }
}
