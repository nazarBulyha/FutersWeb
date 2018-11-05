using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FuturesWeb.UtilHelper
{
	public static class HttpHelper
    {
        public static async Task<WebResponse> GetResponseAsync(WebRequest request)
        {
            var response = (HttpWebResponse)await request.GetResponseAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // TODO: redirect to error page.
                throw new ArgumentException(@"Did not get a response from server.", nameof(request));
            }

            return response;
        }

        public static string ReadResponse(WebResponse response)
        {
            using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                return reader.ReadToEnd();
            }
        }

        public static WebRequest CreatePostRequest(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(url));
            }

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";

            return request;
        }

        public static WebRequest CreateGetRequest(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException(@"Value cannot be null or empty.", nameof(url));
            }

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "GET";

            return request;
        }
    }
}