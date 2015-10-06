//-----------------------------------------------------------------------
// <copyright file="AuthenticatedUri.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Net;

namespace MyToolkit.Networking
{
    /// <summary>Provides an URI with authentication information (username/password). </summary>
    public class AuthenticatedUri : Uri
    {
        /// <summary>Initializes a new instance of the <see cref="AuthenticatedUri"/> class.</summary>
        /// <param name="uriString">The URI string.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public AuthenticatedUri(string uriString, string username = null, string password = null)
            : base(uriString)
        {
            UserName = username;
            Password = password;
        }

        /// <summary>Initializes a new instance of the <see cref="AuthenticatedUri"/> class.</summary>
        /// <param name="uriString">The URI string.</param>
        /// <param name="uriKind">Kind of the URI.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public AuthenticatedUri(string uriString, UriKind uriKind, string username = null, string password = null)
            : base(uriString, uriKind)
        {
            UserName = username;
            Password = password;
        }

        /// <summary>Initializes a new instance of the <see cref="AuthenticatedUri"/> class.</summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public AuthenticatedUri(Uri baseUri, string relativeUri, string username = null, string password = null)
            : base(baseUri, relativeUri)
        {
            UserName = username;
            Password = password;
        }

        /// <summary>Initializes a new instance of the <see cref="AuthenticatedUri"/> class.</summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public AuthenticatedUri(Uri baseUri, Uri relativeUri, string username = null, string password = null)
            : base(baseUri, relativeUri)
        {
            UserName = username;
            Password = password;
        }

        /// <summary>Gets or sets the username. </summary>
        public string UserName { get; set; }

        /// <summary>Gets or sets the password. </summary>
        public string Password { get; set; }

        /// <summary>Gets the the username and password as a credentials object. </summary>
        public ICredentials Credentials
        {
            get { return UserName == null ? null : new NetworkCredential(UserName, Password); }
        }
    }
}