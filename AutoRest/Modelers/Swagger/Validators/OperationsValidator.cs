﻿using Microsoft.Rest.Generator.Logging;
using Microsoft.Rest.Generator.Validation;
using Microsoft.Rest.Modeler.Swagger.Model;
using Microsoft.Rest.Modeler.Swagger.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Rest.Modeler.Swagger
{
    public class OperationsValidator : SwaggerObjectValidator, IValidator<Operation>
    {
        public Dictionary<string, SwaggerParameter> Parameters { get; internal set; }

        public string Path { get; internal set; }

        public OperationsValidator(string path, Dictionary<string, SwaggerParameter> parameters)
        {
            Parameters = parameters;
            Path = path;
        }

        public bool IsValid(Operation entity)
        {
            return !ValidationExceptions(entity).Any();
        }

        public IEnumerable<ValidationMessage> ValidationExceptions(Operation entity)
        {
            var consumesValidator = new ConsumesValidator();
            foreach (var exception in consumesValidator.ValidationExceptions(entity.Consumes))
            {
                yield return exception;
            }

            var producesValidator = new ProducesValidator();
            foreach (var exception in producesValidator.ValidationExceptions(entity.Produces))
            {
                yield return exception;
            }

            if (entity.Parameters != null)
            {
                var bodyParameters = new HashSet<string>();

                foreach (var param in entity.Parameters)
                {
                    if (param.In == ParameterLocation.Body)
                        bodyParameters.Add(param.Name);
                    if (param.Reference != null)
                    {
                        var pRef = FindReferencedParameter(param.Reference, Parameters);
                        if (pRef != null && pRef.In == ParameterLocation.Body)
                        {
                            bodyParameters.Add(pRef.Name);
                        }
                    }
                    //if (!string.IsNullOrEmpty(param.Name))
                    //    context.PushTitle(context.Title + "/" + param.Name);
                    var parameterValidator = new ParameterValidator();
                    foreach (var exception in producesValidator.ValidationExceptions(param))
                    {
                        yield return exception;
                    }
                    //if (!string.IsNullOrEmpty(param.Name))
                    //    context.PopTitle();
                }

                if (bodyParameters.Count > 1)
                {
                    yield return new ValidationMessage()
                    {
                        Severity = LogEntrySeverity.Error,
                        Message = string.Format(CultureInfo.InvariantCulture, Resources.TooManyBodyParameters1, string.Join(",", bodyParameters)),
                        Source = entity
                    };
                }

                // TODO: validate path parameters
                //FindAllPathParameters(entity);
            }

            // TODO: call base to check description
            //if (string.IsNullOrEmpty(Description))
            //{
            //    context.LogWarning(Resources.MissingDescription);
            //}

            var responsesValidator = new ResponsesValidator();
            foreach (var exception in responsesValidator.ValidationExceptions(entity.Responses))
            {
                yield return exception;
            }

            if (entity.ExternalDocs != null)
            {
                var externalDocsValidator = new ExternalDocsValidator();
                foreach (var exception in externalDocsValidator.ValidationExceptions(entity.ExternalDocs))
                {
                    yield return exception;
                }
            }

            yield break;
        }

        //private void FindAllPathParameters(Operation entity)
        //{
        //    var parts = Path.Split("/?".ToCharArray());

        //    foreach (var part in parts.Where(p => !string.IsNullOrEmpty(p)))
        //    {
        //        if (part[0] == '{' && part[part.Length - 1] == '}')
        //        {
        //            var pName = part.Trim('{', '}');
        //            var found = FindParameter(entity, pName, Parameters);

        //            if (found == null || found.In != ParameterLocation.Path)
        //            {
        //                context.LogError(string.Format(CultureInfo.InvariantCulture, Resources.NoDefinitionForPathParameter1, pName));
        //            }
        //        }
        //    }
        //}

        private SwaggerParameter FindParameter(Operation entity, string name, IDictionary<string, SwaggerParameter> parameters)
        {
            if (entity.Parameters != null)
            {
                foreach (var param in entity.Parameters)
                {
                    if (name.Equals(param.Name))
                        return param;

                    var pRef = FindReferencedParameter(param.Reference, parameters);

                    if (pRef != null && name.Equals(pRef.Name))
                    {
                        return pRef;
                    }
                }
            }
            return null;
        }

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

    }
}