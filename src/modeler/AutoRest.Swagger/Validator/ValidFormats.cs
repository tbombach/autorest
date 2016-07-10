// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using AutoRest.Core.Validation;
using AutoRest.Swagger.Model;

namespace AutoRest.Swagger.Validation
{
    public class ValidFormats : TypedRule<SwaggerObject>
    {
        public override bool IsValid(SwaggerObject entity)
        {
            bool valid = true;
            //formatParams = new object[0];

            if (entity != null)
            {
                try
                {
                    // TODO: our SwaggerObject typing system needs expanding. Currently, there's no
                    // information about how formats, etc. 
                    entity.ToType();
                }
                catch (NotImplementedException)
                {
                    valid = false;
                }
                //formatParams = new object[] { entity.Type, entity.Format };
            }

            return valid;
        }

        public override ValidationExceptionName Exception
        {
            get
            {
                return ValidationExceptionName.FormatMustExist;
            }
        }
    }
}
