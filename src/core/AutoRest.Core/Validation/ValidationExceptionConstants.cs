﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AutoRest.Core.Properties;
using System.Collections.Generic;

namespace AutoRest.Core.Validation
{
    public static class ValidationExceptionConstants
    {
        public static class Info
        {
            public static readonly IReadOnlyDictionary<ValidationExceptionName, string> Messages = new Dictionary<ValidationExceptionName, string>
            {
                { ValidationExceptionName.AnonymousTypesDiscouraged, Resources.AnonymousTypesDiscouraged },
            };
        }

        public static class Warnings
        {
            public static readonly IReadOnlyDictionary<ValidationExceptionName, string> Messages = new Dictionary<ValidationExceptionName, string>
            {
                { ValidationExceptionName.DescriptionRequired, Resources.MissingDescription },
                { ValidationExceptionName.OnlyJsonInRequest, Resources.OnlyJSONInRequests1 },
                { ValidationExceptionName.OnlyJsonInResponse, Resources.OnlyJSONInResponses1 },
                { ValidationExceptionName.HeaderShouldHaveClientName, Resources.HeaderShouldHaveClientName },
                { ValidationExceptionName.InvalidSchemaParameter, Resources.InvalidSchemaParameter },
                { ValidationExceptionName.ClientNameMustNotBeEmpty, Resources.EmptyClientName },
                { ValidationExceptionName.RefsMustNotHaveSiblings, Resources.ConflictingRef },
                { ValidationExceptionName.FormatMustExist, Resources.InvalidTypeFormatCombination },
                { ValidationExceptionName.DefaultResponseRequired, Resources.NoDefaultResponse },
                { ValidationExceptionName.XmsPathsMustOverloadPaths, Resources.XMSPathBaseNotInPaths },
                { ValidationExceptionName.DescriptiveDescriptionRequired, Resources.DescriptionNotDescriptive },
                { ValidationExceptionName.OperationIdNounsShouldNotAppearInVerb, Resources.OperationIdNounInVerb },
            };
        }

        public static class Errors
        {
            public static readonly IReadOnlyDictionary<ValidationExceptionName, string> Messages = new Dictionary<ValidationExceptionName, string>
            {
                { ValidationExceptionName.RequiredPropertiesMustExist, Resources.MissingRequiredProperty },
                { ValidationExceptionName.OnlyOneBodyParameterAllowed, Resources.TooManyBodyParameters1 },
                { ValidationExceptionName.BodyMustHaveSchema, Resources.BodyMustHaveSchema },
                { ValidationExceptionName.BodyMustNotHaveType, Resources.BodyWithType },
                { ValidationExceptionName.DefaultMustAppearInEnum, Resources.InvalidDefault },
                { ValidationExceptionName.PathParametersMustBeDefined, Resources.NoDefinitionForPathParameter1 },
                { ValidationExceptionName.OnlyOneUnderscoreInOperationId, Resources.OnlyOneUnderscoreAllowedInOperationId },
            };
        }
    }
}
