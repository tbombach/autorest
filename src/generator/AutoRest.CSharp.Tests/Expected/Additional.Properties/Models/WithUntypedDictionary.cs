// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
// 
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Fixtures.AdditionalProperties.Models
{
    using System.Linq;

    public partial class WithUntypedDictionary
    {
        /// <summary>
        /// Initializes a new instance of the WithUntypedDictionary class.
        /// </summary>
        public WithUntypedDictionary() { }

        /// <summary>
        /// Initializes a new instance of the WithUntypedDictionary class.
        /// </summary>
        /// <param name="additionalProperties">Unmatched properties from the
        /// message are deserialized this collection</param>
        public WithUntypedDictionary(System.Collections.Generic.IDictionary<string, object> additionalProperties = default(System.Collections.Generic.IDictionary<string, object>), string abc = default(string))
        {
            AdditionalProperties = additionalProperties;
            Abc = abc;
        }

        /// <summary>
        /// Gets or sets unmatched properties from the message are
        /// deserialized this collection
        /// </summary>
        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "abc")]
        public string Abc { get; set; }

    }
}
