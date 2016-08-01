# Validation support

## ToC
- [AnonymousParameterTypes](#anonymousparametertypes)
- [AvoidNestedProperties](#avoidnestedproperties)

## Rules
### `AnonymousParameterTypes`
#### Description
This rule appears when you define a model type inline, rather than in the `definitions` section. Since code generation does not have a name to call your model, the class will have a non-descriptive name. Additionally, if it represents the same type as another parameter in a different operation, then it becomes impossible to reuse that same class for both operations.
#### How to fix
Move the schema to the `definitions` section and reference it using `$ref`.
#### Effect on generated code
##### Before
[Spec]
```json5
…
"parameters": [
    {
        "name": "foo",
        "in": "body",
        "schema": {
            "type": "object",
            "properties": {
                …
            }
        }
    }
]
…
```
[Generated code]
```csharp
public class FooParameter1 {
    …
}
```
##### After
[Spec]
```json5
…
"parameters": [
    {
        "name": "foo",
        "in": "body",
        "schema": {
            "$ref": "#/definition/FooCreationSettings"
        }
    }
],
…
"definitions": {
    "FooCreationSettings": {
        "type": "object",
        "properties": {
            …
        }
    }
}
…
```
[Generated code]
```csharp
public class FooCreationSettings {
    …
}
```
### `AvoidNestedProperties`
#### Description
This rule appears when you define a property with the name `properties`, and do not use the [`x-ms-client-flatten` extension](https://github.com/Azure/autorest/blob/master/Documentation/swagger-extensions.md#x-ms-client-flatten). Users often provide feedback that they don't want to create multiple levels of properties to be able to use an operation. By applying the `x-ms-client-flatten` extension, you move the inner `properties` to the top level of your definition.
#### How to fix
Add the `x-ms-client-flatten` extension to the inner `properties` definition.
#### Effect on generated code
##### Before
[Spec]
```json5
…
"definitions": {
    "Foo": {
        "type": "object",
        "properties": {
            "properties": {
                "type": "object",
                "properties": {
                    "bar": {
                        "type": "string"
                    }
                }
            }
        }
    }
}
…
```
[Generated code]
```csharp
public class Foo {
    FooProperties Properties { get; set; } 
}

public class FooProperties {
    string Bar { get; set; } 
}
```
##### After
[Spec]
```json5
…
"definitions": {
    "Foo": {
        "type": "object",
        "properties": {
            "properties": {
                "type": "object",
                "properties": {
                    "bar": {
                        "type": "string"
                    }
                }
            },
            "x-ms-client-flatten": true
        }
    }
}
…
```
[Generated code]
```csharp
public class Foo {
    string Bar { get; set; } 
}
```
### `DefaultInEnum`
#### Description
This rule applies when the value specified by the `default` property does not appear in the `enum` constraint for a schema.
#### How to fix
Add the value from `default` property to the set of values in the `enum` constraint.

_or_

Change the value for the `default` property to one of the values in the `enum` array.
#### Effect on generated code
Failing to pass this validation rule affects the generated code because default values allow users to call an operation without specifying a value for that parameter or property. In that case, the generated code will send the default value instead, which should be invalid for the server. Spec files that have this issue generate code that can easily send bad requests.
### `DescriptiveDescriptionRequired`
#### Description
This rule validates that a description property is not empty. An empty description does not provide value for customers and makes it more difficult to use the generated code as a consumer.
#### How to fix
Replace the empty string with a useful description.
#### Effect on generated code
##### Before
```

```
##### After
```

```

### `DescriptiveDescriptionRequired`
### `ModelTypeIncomplete`
### `NonEmptyClientName`
### `OperationDescriptionRequired`
### `OperationIdNounInVerb`
### `OperationIdSingleUnderscore`
### `ParameterDescriptionRequired`
### `RequiredPropertiesMustExist`
### `ResponseRequired`
### `ValidFormats`
### `XmsPathsInPath`
