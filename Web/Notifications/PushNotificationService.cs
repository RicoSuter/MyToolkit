using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace MyToolkit.Notifications
{
	public class PushNotificationService
	{
		private string accessToken;
		private string secret;
		private string sid;
		
		public PushNotificationService(string sid, string secret)
		{
			this.sid = sid;
			this.secret = secret;
		}

		private void GetAccessToken()
		{
			var urlEncodedSid = HttpUtility.UrlEncode(String.Format("{0}", this.sid));
			var urlEncodedSecret = HttpUtility.UrlEncode(this.secret);

			var body = String.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com", 
				urlEncodedSid, urlEncodedSecret);

			var client = new WebClient();
			client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

			var response = client.UploadString("https://login.live.com/accesstoken.srf", body);
			var oAuthToken = GetOAuthTokenFromJson(response);
			
			lock (this)
				accessToken = oAuthToken.AccessToken;
		}

		private OAuthToken GetOAuthTokenFromJson(string jsonString)
		{
			using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
			{
				var ser = new DataContractJsonSerializer(typeof(OAuthToken));
				var oAuthToken = (OAuthToken)ser.ReadObject(ms);
				return oAuthToken;
			}
		}

		public HttpStatusCode SendBadgeUpdate(string uri, int count)
		{
			var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
				<badge version=""1"" value=""" + count + @""" />";
			return SendNotification(uri, xml, PushNotificationType.Badge);
		}

		public HttpStatusCode SendTileUpdate(string uri, string xml)
		{
			return SendNotification(uri, xml, PushNotificationType.Tile);
		}

		public HttpStatusCode SendBadgeUpdate(string uri, string xml)
		{
			return SendNotification(uri, xml, PushNotificationType.Badge);
		}

		public HttpStatusCode SendToastNotification(string uri, string xml)
		{
			return SendNotification(uri, xml, PushNotificationType.Toast);
		}

		public HttpStatusCode SendNotification(string uri, string xml, PushNotificationType type)
		{
			if (accessToken == null)
				GetAccessToken();

			try
			{
				byte[] contentInBytes = Encoding.UTF8.GetBytes(xml);

				WebRequest webRequest = HttpWebRequest.Create(uri);
				HttpWebRequest request = webRequest as HttpWebRequest;
				request.ContentType = "text/xml";
				webRequest.Method = "POST";

				var typeString = ""; 
				switch (type)
				{
					case PushNotificationType.Tile: typeString = "wns/tile"; break;
					case PushNotificationType.Badge: typeString = "wns/badge"; break;
					case PushNotificationType.Toast: typeString = "wns/toast"; break;
					default: throw new NotImplementedException();
				}

				webRequest.Headers.Add("X-WNS-Type", typeString);
				webRequest.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));

				Stream requestStream = webRequest.GetRequestStream();
				requestStream.Write(contentInBytes, 0, contentInBytes.Length);
				requestStream.Close();

				HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
				return webResponse.StatusCode;
			}
			catch (WebException webException)
			{
				var exceptionDetails = webException.Response.Headers["WWW-Authenticate"];
				if (exceptionDetails != null && exceptionDetails.Contains("Token expired"))
				{
					GetAccessToken();
					return SendNotification(uri, xml, type);
				}
				else
				{
					throw webException;
				}
			}
		}

		[DataContract]
		public class OAuthToken
		{
			[DataMember(Name = "access_token")]
			public string AccessToken { get; set; }
			[DataMember(Name = "token_type")]
			public string TokenType { get; set; }
		}
	}
}
