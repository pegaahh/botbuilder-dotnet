﻿// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Bot.Schema.Teams
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Messaging extension response
    /// </summary>
    public partial class MessagingExtensionResponse
    {
        /// <summary>
        /// Initializes a new instance of the MessagingExtensionResponse class.
        /// </summary>
        public MessagingExtensionResponse()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the MessagingExtensionResponse class.
        /// </summary>
        public MessagingExtensionResponse(MessagingExtensionResult composeExtension = default(MessagingExtensionResult))
        {
            MessagingExtension = composeExtension;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "composeExtension")]
        public MessagingExtensionResult MessagingExtension { get; set; }

    }
}
