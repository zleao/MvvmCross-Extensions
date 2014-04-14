using System;
using MvvmCrossUtilities.Plugins.Rest.Request;
using MvvmCrossUtilities.Plugins.Rest.Response;
using RestSharp.Deserializers;

#if MONODROID
namespace MvvmCrossUtilities.Plugins.Rest.Droid
#elif MONOTOUCH
namespace MvvmCrossUtilities.Plugins.Rest.Touch
#elif WINDOWS_PHONE
namespace MvvmCrossUtilities.Plugins.Rest.WindowsPhone
#else
namespace MvvmCrossUtilities.Plugins.Rest.SharedFiles
#endif
{
    public class RestClient : IRestClient
    {
        #region IRestClient Members


        public RestResponse MakeRequest(RestRequest restRequest)
        {
#if !WINDOWS_PHONE
            //Build RestSharp request
            var restSharpRequest = BuildRestSharpRequest(restRequest);

            //Create the RestSharp client
            var restSharpClient = GetRestClient(restRequest.Url);

            //Execute the request
            var restSharpResponse = restSharpClient.Execute(restSharpRequest);

            //translates the RestSharp response into something that the core plugin knows
            var restResponse = BuildRestResponse(restSharpResponse);

            return restResponse;
#else
            throw new NotSupportedException();
#endif
        }

        public RestResponse<T> MakeRequestFor<T>(RestRequest restRequest) where T : new()
        {
#if !WINDOWS_PHONE
            //Build RestSharp request
            var restSharpRequest = BuildRestSharpRequest(restRequest);

            //Create the RestSharp client
            var restSharpClient = GetRestClient(restRequest.Url);

            //Execute the request
            var restSharpResponse = restSharpClient.Execute<T>(restSharpRequest);

            //translates the RestSharp response into something that the core plugin knows
            var decodedRestResponse = BuildRestResponse<T>(restSharpResponse);

            return decodedRestResponse;
#else
            throw new NotSupportedException();
#endif
        }


        public void MakeAsyncRequest(RestRequest restRequest, Action<RestResponse> successAction, Action<Exception> errorAction)
        {
            try
            {
                //Build RestSharp request
                var restSharpRequest = BuildRestSharpRequest(restRequest);

                //Create the RestSharp client
                var restSharpClient = GetRestClient(restRequest.Url);

                restSharpClient.ExecuteAsync(restSharpRequest, (r, h) =>
                {
                    var restResponse = BuildRestResponse(r);
                    successAction(restResponse);
                });
            }
            catch (Exception ex)
            {
                errorAction(ex);
            }
        }

        public void MakeAsyncRequestFor<T>(RestRequest restRequest, Action<RestResponse<T>> successAction, Action<Exception> errorAction)
        {
            try
            {
                //Build RestSharp request
                var restSharpRequest = BuildRestSharpRequest(restRequest);

                //Create the RestSharp client
                var restSharpClient = GetRestClient(restRequest.Url);

                restSharpClient.ExecuteAsync<T>(restSharpRequest, (r, h) =>
                {
                    var restResponse = BuildRestResponse<T>(r);
                    successAction(restResponse);
                });
            }
            catch (Exception ex)
            {
                errorAction(ex);
            }
        }

        #endregion

        #region Methods

        private RestSharp.IRestRequest BuildRestSharpRequest(RestRequest restRequest)
        {
            var restSharpRequest = new RestSharp.RestRequest(restRequest.Resource);

            restSharpRequest.Method = (RestSharp.Method)Enum.Parse(typeof(RestSharp.Method), restRequest.Method.ToString());
            restSharpRequest.RequestFormat = (RestSharp.DataFormat)Enum.Parse(typeof(RestSharp.DataFormat), restRequest.RequestFormat.ToString());
            restSharpRequest.DateFormat = restRequest.DateFormat;

            //TODO: solve this mapping
            //restSharpRequest.Credentials = restRequest.Credentials;

            if (restRequest.Body != null)
                restSharpRequest.AddBody(restRequest.Body);

            foreach (var item in restRequest.Cookies)
                restSharpRequest.AddCookie(item.Key, item.Value);

            foreach (var item in restRequest.Files)
                restSharpRequest.AddFile(item.Key, item.Value);

            foreach (var item in restRequest.Headers)
                restSharpRequest.AddHeader(item.Key, item.Value);

            foreach (var item in restRequest.UrlSegments)
                restSharpRequest.AddUrlSegment(item.Key, item.Value);

            foreach (var item in restRequest.Objects)
                restSharpRequest.AddObject(item.Key, item.Value);

            foreach (var item in restRequest.Parameters)
	        {
                restSharpRequest.AddParameter(item.Name, item.Value, GetRestSharpParameterType(item.Type));
	        }

            return restSharpRequest;
        }

        private RestSharp.IRestClient GetRestClient(string url)
        {
            //TODO: maybe we could use allways the same instance...
            var restClient = new RestSharp.RestClient(url);
            restClient.AddHandler("application/x-json", new JsonDeserializer());

            return restClient;
        }

        private RestResponse BuildRestResponse(RestSharp.IRestResponse restSharpResponse)
        {
            var restResponse = new RestResponse();

            restResponse.Content = restSharpResponse.Content;
            restResponse.ContentEncoding = restSharpResponse.ContentEncoding;
            restResponse.ContentLength = restSharpResponse.ContentLength;
            restResponse.ContentType = restSharpResponse.ContentType;
            //TODO: solve this mapping
            //toReturn.Cookies = restSharpResponse.Cookies;
            restResponse.ErrorException = restSharpResponse.ErrorException;
            restResponse.ErrorMessage = restSharpResponse.ErrorMessage;
            //TODO: solve this mapping
            //toReturn.Headers = restSharpResponse.Headers,
            restResponse.RawBytes = restSharpResponse.RawBytes;
            //TODO: solve this mapping
            //toReturn.Request = restRequest;
            restResponse.ResponseStatus = (ResponseStatus)Enum.Parse(typeof(ResponseStatus), restSharpResponse.ResponseStatus.ToString());
            restResponse.ResponseUri = restSharpResponse.ResponseUri;
            restResponse.Server = restSharpResponse.Server;
            restResponse.StatusCode = (ResponseHttpStatusCode)Enum.Parse(typeof(ResponseHttpStatusCode), restSharpResponse.StatusCode.ToString());
            restResponse.StatusDescription = restSharpResponse.StatusDescription;

            return restResponse;
        }

        private RestResponse<T> BuildRestResponse<T>(RestSharp.IRestResponse<T> restSharpResponse)
        {
            var restResponse = new RestResponse<T>();

            restResponse.Content = restSharpResponse.Content;
            restResponse.ContentEncoding = restSharpResponse.ContentEncoding;
            restResponse.ContentLength = restSharpResponse.ContentLength;
            restResponse.ContentType = restSharpResponse.ContentType;
            //TODO: solve this mapping
            //toReturn.Cookies = restSharpResponse.Cookies;
            restResponse.ErrorException = restSharpResponse.ErrorException;
            restResponse.ErrorMessage = restSharpResponse.ErrorMessage;
            //TODO: solve this mapping
            //toReturn.Headers = restSharpResponse.Headers,
            restResponse.RawBytes = restSharpResponse.RawBytes;
            //TODO: solve this mapping
            //toReturn.Request = restRequest;
            restResponse.ResponseStatus = (ResponseStatus)Enum.Parse(typeof(ResponseStatus), restSharpResponse.ResponseStatus.ToString());
            restResponse.ResponseUri = restSharpResponse.ResponseUri;
            restResponse.Server = restSharpResponse.Server;
            restResponse.StatusCode = (ResponseHttpStatusCode)Enum.Parse(typeof(ResponseHttpStatusCode), restSharpResponse.StatusCode.ToString());
            restResponse.StatusDescription = restSharpResponse.StatusDescription;

            restResponse.Data = restSharpResponse.Data;

            return restResponse;
        }

        private RestSharp.ParameterType GetRestSharpParameterType(ParameterType pluginParamType)
        {
            switch (pluginParamType)
            {
                case ParameterType.Cookie:
                    return RestSharp.ParameterType.Cookie;

                case ParameterType.GetOrPost:
                    return RestSharp.ParameterType.GetOrPost;

                case ParameterType.UrlSegment:
                    return RestSharp.ParameterType.UrlSegment;

                case ParameterType.HttpHeader:
                    return RestSharp.ParameterType.HttpHeader;
                
                case ParameterType.RequestBody:
                    return RestSharp.ParameterType.RequestBody;
                
                default:
                    throw new System.NotSupportedException("ParameterType not supported: " + pluginParamType.ToString());
            }
        }

        #endregion
    }
}
