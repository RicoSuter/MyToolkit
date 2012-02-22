using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MyToolkit.Network;

namespace MyToolkit.Metro.Network
{
	public static class Http
	{
		public static async Task<HttpResponse> Get(string url)
		{
			return await Get(new HttpGetRequest(url));
		}

		public static async Task<HttpResponse> Get(HttpGetRequest request)
		{
			var response = new HttpResponse(request);
			try
			{
				var client = new HttpClient();
				var result = await client.GetAsync(request.Uri.ToString());
				if (result.IsSuccessStatusCode)
				{
					//response.Cookies = 
					response.RawResponse = result.Content.ReadAsByteArray();

				}
				else
				{
					response.Exception = new Exception(result.StatusCode.ToString());
				}
			}
			catch (Exception e)
			{
				response.Exception = e;
			}
			return response;
		}
	}
}
