﻿using System.Collections.Generic;
using RestSharp;

namespace Usergrid.Sdk
{
    internal class UsergridRequest : IUsergridRequest
    {
        private readonly string _application;
        private readonly string _organization;
        private readonly RestClient _restClient;

        public UsergridRequest(string baseUri, string organization, string application)
        {
            _organization = organization;
            _application = application;
            _restClient = new RestClient(baseUri);
        }

        public string AccessToken { get; set; }

        public IRestResponse<T> ExecuteJsonRequest<T>(string resource, Method method, object body = null) where T : new()
        {
            RestRequest request = GetRequest(resource, method);
            AddBodyAsJson(body, request);
            IRestResponse<T> response = _restClient.Execute<T>(request);
            return response;
        }

        public IRestResponse ExecuteJsonRequest(string resource, Method method, object body = null)
        {
            RestRequest request = GetRequest(resource, method);
            AddBodyAsJson(body, request);
            IRestResponse response = _restClient.Execute(request);
            return response;
        }

        public IRestResponse ExecuteMultipartFormDataRequest(string resource, Method method, IDictionary<string, object> formParameters, IDictionary<string, string> fileParameters)
        {
            RestRequest request = GetRequest(resource, method);
            foreach (var parameter in formParameters)
            {
                request.AddParameter(parameter.Key, parameter.Value, ParameterType.GetOrPost);
            }
            foreach (var parameter in fileParameters)
            {
                request.AddFile(parameter.Key, parameter.Value);
            }

            IRestResponse response = _restClient.Execute(request);
            return response;
        }

        private RestRequest GetRequest(string resource, Method method)
        {
            var request = new RestRequest(string.Format("{0}/{1}{2}", _organization, _application, resource), method);
            AddAuthorizationHeader(request);
            return request;
        }

        private static void AddBodyAsJson(object body, RestRequest request)
        {
            if (body != null)
            {
                request.JsonSerializer = new RestSharpJsonSerializer();
                request.RequestFormat = DataFormat.Json;
                request.AddBody(body);
            }
        }

        private void AddAuthorizationHeader(RestRequest request)
        {
            if (AccessToken != null)
                request.AddHeader("Authorization", string.Format("Bearer {0}", AccessToken));
        }
    }
}