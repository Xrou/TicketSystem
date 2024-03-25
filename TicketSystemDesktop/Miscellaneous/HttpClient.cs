using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;

namespace TicketSystemDesktop
{
    public class HttpClient
    {
        ///<summary>
        ///post some data (Dictionary) from url, includes auth header and returns response
        ///</summary>
        public static (string? response, HttpStatusCode code) Post(string url, Dictionary<string, object> body, bool auth = true)
        {
            var options = new RestClientOptions(Constants.BaseUrl);
            var client = new RestClient(options);
            var request = new RestRequest(url, Method.Post);

            request.AddHeader("Content-Type", "application/json");
            if (auth)
            {
                request.AddHeader("Authorization", "Bearer " + LocalStorage.Get("AccessToken"));
            }

            string stringBody = JsonSerializer.Serialize(body);

            request.AddJsonBody(stringBody);
            var response = client.ExecuteAsync(request).Result;

            return (response.Content, response.StatusCode);
        }

        ///<summary>
        ///get some data from url, includes auth header and returns response
        ///</summary>
        public static (string? response, HttpStatusCode code) Get(string url, KeyValuePair<string, object>[]? parameters = null, bool auth = true)
        {
            var options = new RestClientOptions(Constants.BaseUrl);
            var client = new RestClient(options);
            var request = new RestRequest(url, Method.Get);

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    request.AddParameter(parameter.Key, parameter.Value?.ToString());
                }
            }
            if (auth)
            {
                request.AddHeader("Authorization", "Bearer " + LocalStorage.Get("AccessToken"));
            }

            var response = client.ExecuteAsync(request).Result;

            return (response.Content, response.StatusCode);
        }

        public static (string? response, HttpStatusCode code) UploadFile(string url, string[] filesPaths, bool auth = true)
        {
            var options = new RestClientOptions(Constants.BaseUrl);
            var client = new RestClient(options);
            var request = new RestRequest(url, Method.Post);

            if (auth)
            {
                request.AddHeader("Authorization", "Bearer " + LocalStorage.Get("AccessToken"));
            }
            request.AlwaysMultipartFormData = true;
            foreach (var filePath in filesPaths)
            {
                request.AddFile("uploadedFiles", filePath);
            }
            RestResponse response = client.ExecuteAsync(request).Result;

            return (response.Content, response.StatusCode);
        }
    }
}
