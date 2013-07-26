using MvvmCrossUtilities.Plugins.Rest.Request;
using MvvmCrossUtilities.Plugins.Rest.Response;
using System;

namespace MvvmCrossUtilities.Plugins.Rest
{
    public interface IRestClient
    {
        /// <summary>
        /// NOT SUPPORTED IN WINDOWS PHONE!!!
        /// </summary>
        /// <param name="restRequest">The rest request.</param>
        /// <returns></returns>
        RestResponse MakeRequest(RestRequest restRequest);

        /// <summary>
        /// NOT SUPPORTED IN WINDOWS PHONE!!!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="restRequest">The rest request.</param>
        /// <returns></returns>
        RestResponse<T> MakeRequestFor<T>(RestRequest restRequest) where T : new();

        void MakeAsyncRequest(RestRequest restRequest, Action<RestResponse> successAction, Action<Exception> errorAction);

        void MakeAsyncRequestFor<T>(RestRequest restRequest, Action<RestResponse<T>> successAction, Action<Exception> errorAction);
    }
}
