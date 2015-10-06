//-----------------------------------------------------------------------
// <copyright file="ClientBaseExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WINRT && !WP8

using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace MyToolkit.Networking
{
	public static class ClientBaseExtensions
	{
		public static string GetIncomingCookies<T>(this ClientBase<T> client) where T : class
		{
			var props = OperationContext.Current.IncomingMessageProperties;
			var prop = (HttpResponseMessageProperty)props[HttpResponseMessageProperty.Name];
			var cookies = prop.Headers[HttpResponseHeader.SetCookie];
			return ConstructCookies(cookies);
		}

		public static void SetOutgoingCookies<T>(this ClientBase<T> client, string cookies) where T : class
		{
			var prop = new HttpRequestMessageProperty();
			prop.Headers.Add(HttpRequestHeader.Cookie, cookies);
			OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = prop;
		}

	    public static EventHandler<SendingRequestEventArgs> SendingRequest(string cookies)
	    {
	        return (s, e) => e.Request.Headers.Add(HttpRequestHeader.Cookie, cookies);
	    }

	    private static string ConstructCookies(string incomingCookies)
		{
			var cookies = incomingCookies.Split(new[] { ',', ';' });
			var buffer = new StringBuilder(incomingCookies.Length * 10);

			foreach (var entry in cookies)
			{
				if (entry.IndexOf("=") > 0 && !entry.Trim().StartsWith("path") && !entry.Trim().StartsWith("expires"))
					buffer.Append(entry).Append("; ");
			}

			if (buffer.Length > 0)
				buffer.Remove(buffer.Length - 2, 2);
			return buffer.ToString();
		}
	}
}

#endif