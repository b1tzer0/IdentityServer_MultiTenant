// ***********************************************************************
// Assembly         : idpMultiTenant1
// Author           : Rick Mathers
// Created          : 08-17-2022
//
// Last Modified By : Rick Mathers
// Last Modified On : 08-17-2022
// ***********************************************************************
// <copyright file="FidoStoredCredentials.cs" company="Megasys Hospitality Solutions">
//     Copyright (c) Megasys Hospitality Solutions. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Fido2NetLib.Objects;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace idpMultiTenant1.FIDO2
{
    /// <summary>
    /// Class FidoStoredCredential.
    /// </summary>
    public class FidoStoredCredential
    {
        /// <summary>
        /// Gets or sets the primary key for this user.
        /// </summary>
        /// <value>The identifier.</value>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        /// <value>The name of the user.</value>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        public virtual byte[] UserId { get; set; }

        /// <summary>
        /// Gets or sets the public key for this user.
        /// </summary>
        /// <value>The public key.</value>
        public virtual byte[] PublicKey { get; set; }

        /// <summary>
        /// Gets or sets the user handle for this user.
        /// </summary>
        /// <value>The user handle.</value>
        public virtual byte[] UserHandle { get; set; }

        /// <summary>
        /// Gets or sets the signature counter.
        /// </summary>
        /// <value>The signature counter.</value>
        public virtual uint SignatureCounter { get; set; }

        /// <summary>
        /// Gets or sets the type of the cred.
        /// </summary>
        /// <value>The type of the cred.</value>
        public virtual string CredType { get; set; }

        /// <summary>
        /// Gets or sets the registration date for this user.
        /// </summary>
        /// <value>The reg date.</value>
        public virtual DateTime RegDate { get; set; }

        /// <summary>
        /// Gets or sets the Authenticator Attestation GUID (AAGUID) for this user.
        /// </summary>
        /// <value>The aa unique identifier.</value>
        /// <remarks>An AAGUID is a 128-bit identifier indicating the type of the authenticator.</remarks>
        public virtual Guid AaGuid { get; set; }

        /// <summary>
        /// Gets or sets the descriptor.
        /// </summary>
        /// <value>The descriptor.</value>
        [NotMapped]
        public PublicKeyCredentialDescriptor Descriptor
        {
            get { return string.IsNullOrWhiteSpace(DescriptorJson) ? null : JsonSerializer.Deserialize<PublicKeyCredentialDescriptor>(DescriptorJson); }
            set { DescriptorJson = JsonSerializer.Serialize(value); }
        }

        /// <summary>
        /// Gets or sets the descriptor json.
        /// </summary>
        /// <value>The descriptor json.</value>
        public virtual string DescriptorJson { get; set; }
    }
}