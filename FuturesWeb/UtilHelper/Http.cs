namespace Futures
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    public static class HttpHelper
    {
        public static async Task<WebResponse> GetResponseAsync(WebRequest request)
        {
            try
            {
                var response = (HttpWebResponse)await request.GetResponseAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    // TODO: redirect to error page.
                    throw new ArgumentException(@"Did not get a response from server.", nameof(request));
                }

                return response;
            }
            catch (Exception ex)

            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static async Task<string> ReadResponseAsync(WebResponse response)
        {
            try
            {
                using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
}

        public static WebRequest CreatePostRequest(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    throw new ArgumentException(@"Value cannot be null or empty.", nameof(url));

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "POST";

                return request;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
}

        public static WebRequest CreateGetRequest(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    throw new ArgumentException(@"Value cannot be null or empty.", nameof(url));

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";

                return request;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}