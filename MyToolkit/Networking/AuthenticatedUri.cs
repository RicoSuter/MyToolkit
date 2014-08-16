//-----------------------------------------------------------------------
// <copyright file="AuthenticatedUri.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Net;

namespace MyToolkit.Networking
{
    /// <summary>Provides an URI with authentication information (username/password). </summary>
    public class AuthenticatedUri : Uri
    {
        public AuthenticatedUri(string uriString, string username = null, string password = null)
            : base(uriString)
        {
            UserName = username;
            Password = password;
        }

        public AuthenticatedUri(string uriString, UriKind uriKind, string username = null, string password = null)
            : base(uriString, uriKind)
        {
            UserName = username;
            Password = password;
        }

        public AuthenticatedUri(Uri baseUri, string relativeUri, string username = null, string password = null)
            : base(baseUri, relativeUri)
        {
            UserName = username;
            Password = password;
        }

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