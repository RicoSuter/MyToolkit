//-----------------------------------------------------------------------
// <copyright file="Http.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if LEGACY
using Ionic.Zlib;
#else
using System.IO.Compression;
#endif

namespace MyToolkit.Networking
{
    /// <summary>Provides the download progress of a HTTP request. </summary>
    public class HttpProgress
    {
        /// <summary>Gets the number of read bytes. </summary>
        public long ReadBytes { get; internal set; }

        /// <summary>Gets the number of total bytes. </summary>
        public long TotalBytes { get; internal set; }
    }

    /// <summary>Provides static methods to run HTTP requests. </summary>
    public static class Http
    {
        /// <summary>Performs a HTTP GET request. </summary>
        /// <param name="uri">The URI. </param>
        /// <returns>The <see cref="HttpResponse"/>. </returns>
        public static Task<HttpResponse> GetAsync(string uri)
        {
            return GetAsync(uri, CancellationToken.None);
        }

        /// <summary>Performs a HTTP GET request. </summary>
        /// <param name="uri">The URI. </param>
        /// <param name="token">The <see cref="CancellationToken"/>. </param>
        /// <returns>The <see cref="HttpResponse"/>. </returns>
        public static Task<HttpResponse> GetAsync(string uri, CancellationToken token)
        {
            return GetAsync(new HttpGetRequest(uri), token);
        }

        /// <summary>Performs a HTTP GET request. </summary>
        /// <param name="uri">The URI. </param>
        /// <returns>The <see cref="HttpResponse"/>. </returns>
        public static Task<HttpResponse> GetAsync(Uri uri)
        {
            return GetAsync(uri, CancellationToken.None);
        }

        /// <summary>Performs a HTTP GET request. </summary>
        /// <param name="uri">The URI. </param>
        /// <param name="token">The <see cref="CancellationToken"/>. </param>
        /// <returns>The <see cref="HttpResponse"/>. </returns>
        public static Task<HttpResponse> GetAsync(Uri uri, CancellationToken token)
        {
            return GetAsync(new HttpGetRequest(uri), token);
        }

        /// <summary>Performs a HTTP GET request. </summary>
        /// <param name="uri">The URI. </param>
        /// <param name="token">The <see cref="CancellationToken"/>. </param>
        /// <param name="progress">The <see cref="IProgress{T}"/>. </param>
        /// <returns>The <see cref="HttpResponse"/>. </returns>
        public static Task<HttpResponse> GetAsync(Uri uri, CancellationToken token, IProgress<HttpProgress> progress)
        {
            return GetAsync(new HttpGetRequest(uri), token, progress);
        }

        /// <summary>Performs a HTTP GET request. </summary>
        /// <param name="request">The <see cref="HttpGetRequest"/>. </param>
        /// <returns>The <see cref="HttpResponse"/>. </returns>
        public static Task<HttpResponse> GetAsync(HttpGetRequest request)
        {
            return GetAsync(request, CancellationToken.None);
        }

        /// <summary>Performs a HTTP GET request. </summary>
        /// <param name="request">The <see cref="HttpGetRequest"/>. </param>
        /// <param name="token">The <see cref="CancellationToken"/>. </param>
        /// <param name="progress">The <see cref="IProgress{T}"/>. </param>
        /// <returns>The <see cref="HttpResponse"/>. </returns>
        public static async Task<HttpResponse> GetAsync(HttpGetRequest request, CancellationToken token, IProgress<HttpProgress> progress = null)
        {
            var uri = GetQueryString(request, request.Query);
            var handler = CreateHandler(request, uri);
            using (var client = CreateClient(request, handler))
            {
                var result = new HttpResponse(request);
                result.HttpClient = client;
                lock (PendingRequests)
                    PendingRequests.Add(result);

                try
                {
                    var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, token);
                    await CreateResponse(result, uri, request, handler, response, token, progress);

                    result.HttpStatusCode = response.StatusCode;
                    if (!response.IsSuccessStatusCode)
                        result.Exception = new HttpStatusException(response.StatusCode.ToString())
                        {
                            Result = result,
                            HttpStatusCode = result.HttpStatusCode
                        };
                }
                catch (Exception exception)
                {
                    if (result.Exception == null)
                    {
                        result.Exception = exception;
                        throw;
                    }
                }
                finally
                {
                    lock (PendingRequests)
                        PendingRequests.Remove(result);
                }

                if (result.Exception != null)
                    throw result.Exception;

                return result;
            }
        }

        /// <summary>Performs a HTTP POST request. </summary>
        /// <param name="uri">The URI. </param>
        /// <returns>The <see cref="HttpResponse"/>. </returns>
        public static Task<HttpResponse> PostAsync(string uri)
        {
            return PostAsync(uri, CancellationToken.None);
        }

        /// <summary>Performs a HTTP POST request. </summary>
        /// <param name="uri">The URI. </param>
        /// <param name="token">The <see cref="CancellationToken"/>. </param>
        /// <returns>The <see cref="HttpResponse"/>. </returns>
        public static Task<HttpResponse> PostAsync(string uri, CancellationToken token)
        {
            return PostAsync(new HttpPostRequest(uri), token);
        }

        /// <summary>
        /// Performs a HTTP POST request. 
        /// </summary>
        /// <param name="uri">The URI. </param>
        /// <returns>The <see cref="HttpResponse"/>. </returns>
        public static Task<HttpResponse> PostAsync(Uri uri)
        {
            return PostAsync(uri, CancellationToken.None);
        }

        /// <summary>
        /// Performs a HTTP POST request. 
        /// </summary>
        /// <param name="uri">The URI. </param>
        /// <param name="token">The <see cref="CancellationToken"/>. </param>
        /// <returns>The <see cref="HttpResponse"/>. </returns>
        public static Task<HttpResponse> PostAsync(Uri uri, CancellationToken token)
        {
            return PostAsync(new HttpPostRequest(uri), token);
        }

        /// <summary>
        /// Performs a HTTP POST request. 
        /// </summary>
        /// <param name="uri">The URI. </param>
        /// <param name="token">The <see cref="CancellationToken"/>. </param>
        /// <param name="progress">The <see cref="IProgress{T}"/>. </param>
        /// <returns>The <see cref="HttpResponse"/>. </returns>
        public static Task<HttpResponse> PostAsync(Uri uri, CancellationToken token, IProgress<HttpProgress> progress)
        {
            return PostAsync(new HttpPostRequest(uri), token, progress);
        }

        /// <summary>
        /// Performs a HTTP POST request. 
        /// </summary>
        /// <param name="request">The <see cref="HttpPostRequest"/>. </param>
        /// <returns>The <see cref="HttpResponse"/>. </returns>
        public static Task<HttpResponse> PostAsync(HttpPostRequest request)
        {
            return PostAsync(request, CancellationToken.None);
        }

        /// <summary>
        /// Performs a HTTP POST request. 
        /// </summary>
        /// <param name="request">The <see cref="HttpPostRequest"/>. </param>
        /// <param name="token">The <see cref="CancellationToken"/>. </param>
        /// <param name="progress">The <see cref="IProgress{T}"/>. </param>
        /// <returns>The <see cref="HttpResponse"/>. </returns>
        public static async Task<HttpResponse> PostAsync(HttpPostRequest request, CancellationToken token, IProgress<HttpProgress> progress = null)
        {
            var uri = GetQueryString(request, request.Query);
            var handler = CreateHandler(request, uri);
            using (var client = CreateClient(request, handler))
            {
                var result = new HttpResponse(request);
                result.HttpClient = client;
                lock (PendingRequests)
                    PendingRequests.Add(result);

                try
                {
                    HttpContent content = null;
                    if (request.RawData != null)
                        content = new ByteArrayContent(request.RawData);
                    else if (request.Files == null || request.Files.Count == 0)
                        content = new ByteArrayContent(request.Encoding.GetBytes(GetQueryString(request.Data)));
                    else
                    {
                        var multipartContent = new MultipartFormDataContent();
                        foreach (var pair in request.Data)
                            multipartContent.Add(new ByteArrayContent(request.Encoding.GetBytes(pair.Value)), pair.Key);
                        
                        foreach (var file in request.Files)
                        {
                            try
                            {
                                var byteContent = new ByteArrayContent(await file.Stream.ReadToEndAsync(0, token, null));
                                byteContent.Headers.ContentType = new MediaTypeHeaderValue(
                                    file.ContentType ?? "application/octet-stream");
                                multipartContent.Add(byteContent, file.Name, file.Filename);
                            }
                            finally
                            {
                                if (file.CloseStream)
                                    file.Stream.Dispose();
                            }
                        }
                        content = multipartContent; 
                    }

                    var response = await client.PostAsync(uri, content, token);
                    await CreateResponse(result, uri, request, handler, response, token, progress);

                    result.HttpStatusCode = response.StatusCode;
                    if (!response.IsSuccessStatusCode)
                        result.Exception = new HttpStatusException(response.StatusCode.ToString())
                        {
                            Result = result, 
                            HttpStatusCode = result.HttpStatusCode
                        };
                }
                catch (Exception ex)
                {
                    if (result.Exception == null)
                    {
                        result.Exception = ex;
                        throw;
                    }
                }
                finally
                {
                    lock (PendingRequests)
                        PendingRequests.Remove(result);
                }

                if (result.Exception != null)
                    throw result.Exception;
                return result; 
            }
        }

        private static readonly List<HttpResponse> PendingRequests = new List<HttpResponse>();

        /// <summary>
        /// Aborts all pending HTTP requests. 
        /// </summary>
        public static void AbortAllRequests()
        {
            lock (PendingRequests)
            {
                foreach (var r in PendingRequests)
                    r.Abort();
            }
        }

        /// <summary>
        /// Aborts all requests specified by the predicate. 
        /// </summary>
        /// <param name="abortPredicate">Predicate to specify which requests to abort. </param>
        public static void AbortRequests(Func<HttpResponse, bool> abortPredicate)
        {
            lock (PendingRequests)
            {
                foreach (var r in PendingRequests.Where(abortPredicate))
                    r.Abort();
            }
        }

        /// <summary>
        /// Gets the count of pending HTTP requests. 
        /// </summary>
        public static int PendingRequestCount
        {
            get
            {
                lock (PendingRequests)
                    return PendingRequests.Count;
            }
        }

        private static Uri GetQueryString(HttpRequest request, Dictionary<string, string> query)
        {
            if (!request.UseCache)
                request.Query["__dcachetime"] = DateTime.Now.Ticks.ToString();

            var queryString = GetQueryString(query);
            if (string.IsNullOrEmpty(queryString))
                return request.Uri;
            if (request.Uri.AbsoluteUri.Contains("?"))
                return new Uri(string.Format("{0}&{1}", request.Uri.AbsoluteUri, queryString));
            return new Uri(string.Format("{0}?{1}", request.Uri.AbsoluteUri, queryString));
        }

        private static string GetQueryString(Dictionary<string, string> query)
        {
            var queryString = "";
            foreach (var p in query)
                queryString += string.Format("{0}={1}&", Uri.EscapeDataString(p.Key), p.Value == null ? "" : p.Value.EscapeUriString());
            queryString = queryString.Trim('&');
            return queryString;
        }

        private static HttpClient CreateClient(HttpRequest request, HttpClientHandler handler)
        {
            if (request.Credentials != null)
                handler.Credentials = request.Credentials;
            else
            {
                // check if Uri is AuthenticatedUri
#if !LEGACY
                var credentialsProperty = request.Uri.GetType().GetRuntimeProperty("Credentials"); // TODO implement faster?
                if (credentialsProperty != null)
                    handler.Credentials = (ICredentials)credentialsProperty.GetValue(request.Uri, null);
#else
                var credentialsProperty = request.Uri.GetType().GetProperty("Credentials"); // TODO implement faster?
                if (credentialsProperty != null)
                    handler.Credentials = (ICredentials)credentialsProperty.GetValue(request.Uri, null);
#endif
            }

            var client = new HttpClient(handler);
    
            if (request.Timeout != TimeSpan.Zero)
                client.Timeout = request.Timeout;

            if ((request.AutomaticDecompression & DecompressionMethods.GZip) == DecompressionMethods.GZip)
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            if ((request.AutomaticDecompression & DecompressionMethods.Deflate) == DecompressionMethods.Deflate)
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

            foreach (var header in request.Headers)
                client.DefaultRequestHeaders.Add(header.Key, header.Value);

            return client;
        }

        private static HttpClientHandler CreateHandler(HttpRequest request, Uri uri)
        {
            var handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            foreach (var cookie in request.Cookies)
                handler.CookieContainer.Add(uri, cookie);
            return handler;
        }

        private static async Task CreateResponse(
            HttpResponse result, Uri uri, HttpRequest request, 
            HttpClientHandler handler, HttpResponseMessage response,
            CancellationToken token, IProgress<HttpProgress> progress)
        {
            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                    result.Headers.Add(header.Key, header.Value.First());
            }

            foreach (var header in response.Headers)
                result.Headers.Add(header.Key, header.Value.First());

            foreach (var cookie in handler.CookieContainer.GetCookies(uri))
                result.Cookies.Add((Cookie)cookie);

            var stream = await response.Content.ReadAsStreamAsync();
            if (response.Content.Headers.ContentEncoding.Any(e => e.ToLower() == "gzip"))
                stream = new Ionic.Zlib.GZipStream(stream, Ionic.Zlib.CompressionMode.Decompress);
            else if (response.Content.Headers.ContentEncoding.Any(e => e.ToLower() == "deflate"))
                stream = new Ionic.Zlib.DeflateStream(stream, Ionic.Zlib.CompressionMode.Decompress);

            if (request.ResponseAsStream)
            {
                result.ResponseStream = stream;
                result.RawResponse = null; 
            }
            else
                result.RawResponse = await stream.ReadToEndAsync(
                    response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1, 
                    token, progress);
        }

        private static string EscapeUriString(this string value)
        {
            const int limit = 32768;
            var sb = new StringBuilder();
            var loops = value.Length / limit;
            for (var i = 0; i <= loops; i++)
            {
                sb.Append(i < loops
                    ? Uri.EscapeDataString(value.Substring(limit*i, limit))
                    : Uri.EscapeDataString(value.Substring(limit*i)));
            }
            return sb.ToString();
        }

        // TODO same as MyToolkit.Utilities.StreamExtensions
        private static Task<byte[]> ReadToEndAsync(this Stream input, long totalBytes, CancellationToken token, IProgress<HttpProgress> progress)
        {
            var source = new TaskCompletionSource<byte[]>();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var buffer = new byte[16 * 1024];
                    using (var ms = new MemoryStream())
                    {
                        int read;
                        long totalRead = 0;
                        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            token.ThrowIfCancellationRequested();
                            ms.Write(buffer, 0, read);
                            totalRead += read; 
                            if (progress != null)
                                progress.Report(new HttpProgress { ReadBytes = totalRead, TotalBytes = totalBytes });
                        }
                        source.SetResult(ms.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    source.SetException(ex);
                }
            }, token);
            return source.Task;
        }

        #region Obsolete

        [Obsolete("Use GetAsync instead. 5/17/2014")]
        public static Task<HttpResponse> Get(string uri, Action<HttpResponse> completed)
        {
            return Get(new HttpGetRequest(uri), completed);
        }

        [Obsolete("Use PostAsync instead. 5/17/2014")]
        public static Task<HttpResponse> Post(string uri, Action<HttpResponse> completed)
        {
            return Post(new HttpPostRequest(uri), completed);
        }

        [Obsolete("Use GetAsync instead. 5/17/2014")]
        public static Task<HttpResponse> Get(string uri, CancellationToken token, Action<HttpResponse> completed)
        {
            return Get(new HttpGetRequest(uri), token, completed);
        }

        [Obsolete("Use PostAsync instead. 5/17/2014")]
        public static Task<HttpResponse> Post(string uri, CancellationToken token, Action<HttpResponse> completed)
        {
            return Post(new HttpPostRequest(uri), token, completed);
        }

        [Obsolete("Use GetAsync instead. 5/17/2014")]
        public static Task<HttpResponse> Get(HttpGetRequest request, Action<HttpResponse> completed)
        {
            return Get(request, CancellationToken.None, completed);
        }

        [Obsolete("Use PostAsync instead. 5/17/2014")]
        public static Task<HttpResponse> Post(HttpPostRequest request, Action<HttpResponse> completed)
        {
            return Post(request, CancellationToken.None, completed);
        }

        [Obsolete("Use GetAsync instead. 5/17/2014")]
        public static Task<HttpResponse> Get(HttpGetRequest request, CancellationToken token, Action<HttpResponse> completed)
        {
            var task = GetAsync(request, token);
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                    completed(new HttpResponse(request) { Exception = t.Exception.InnerException });
                else if (t.IsCanceled)
                    completed(new HttpResponse(request) { Canceled = true });
                else
                    completed(t.Result);
            }, token);
            return task;
        }

        [Obsolete("Use PostAsync instead. 5/17/2014")]
        public static Task<HttpResponse> Post(HttpPostRequest request, CancellationToken token, Action<HttpResponse> completed)
        {
            var task = PostAsync(request, token);
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                    completed(new HttpResponse(request) { Exception = t.Exception.InnerException });
                else if (t.IsCanceled)
                    completed(new HttpResponse(request) { Canceled = true });
                else
                    completed(t.Result);
            }, token);
            return task;
        }

        #endregion
    }
}