// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace AutoRest.Core.Validation
{
    public enum ValidationExceptionName
    {
        None = 0,
        DescriptionRequired,
        OnlyJsonInResponse,
        OnlyJsonInRequest,
        RequiredPropertiesMustExist,
        OnlyOneBodyParameterAllowed,
        BodyMustHaveSchema,
        BodyMustNotHaveType,
        HeaderShouldHaveClientName,
        InvalidSchemaParameter,
        ClientNameMustNotBeEmpty,
        DefaultMustAppearInEnum,
        RefsMustNotHaveSiblings,
        PathParametersMustBeDefined,
        FormatMustExist,
        AnonymousTypesDiscouraged,
        OnlyOneUnderscoreInOperationId,
        DefaultResponseRequired,
        XmsPathsMustOverloadPaths,
        DescriptiveDescriptionRequired,
        OperationIdNounsShouldNotAppearInVerb,
    }
}