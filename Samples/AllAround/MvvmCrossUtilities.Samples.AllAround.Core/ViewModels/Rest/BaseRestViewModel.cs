using System;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Plugins.Notification;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.OneWay;
using MvvmCrossUtilities.Plugins.Rest;
using MvvmCrossUtilities.Plugins.Rest.Request;
using MvvmCrossUtilities.Plugins.Rest.Response;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;

namespace MvvmCrossUtilities.Samples.AllAround.Core.Rest
{
    public abstract class BaseRestViewModel : AllAroundViewModel
    {
        #region Properties

        protected abstract string ServiceUrl { get; }

        protected abstract string ServiceName { get; }

        protected virtual string ServiceFullUrl { get { return ServiceUrl + "/" + ServiceName; } }

        protected IRestClient RestClient
        {
            get { return _restClient ?? (_restClient = Mvx.Resolve<IRestClient>()); }
        }
        private IRestClient _restClient;

        #endregion

        #region Comands

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new MvxCommand(Refresh)); }
        }
        private ICommand _refreshCommand;

        #endregion

        #region Methods

        private void Refresh()
        {
            if(ValidateData())
                MakeRequest();
        }

        protected abstract bool ValidateData();

        protected abstract void MakeRequest();

        protected virtual RestRequest BuildRequest()
        {
            return new RestRequest(ServiceFullUrl, Method.GET);
        }

        protected void MakeRequestFor<TResponse>()
        {
            try
            {
                StartWork("Getting data from rest service...");

                RestClient.MakeAsyncRequestFor<TResponse>(BuildRequest(), OnRequestSuccess, OnRequestError);
            }
            catch (Exception ex)
            {
                OnRequestError(ex);
            }
        }

        private void OnRequestSuccess<TResponse>(RestResponse<TResponse> response)
        {
            FinishedWork();

            InvokeOnMainThread(() =>
            {
                RequestSuccessCallback(response);
            });
        }

        protected abstract void RequestSuccessCallback<TResponse>(RestResponse<TResponse> response);

        private void OnRequestError(Exception ex)
        {
            FinishedWork();
            Mvx.Resolve<INotificationService>().Publish(new NotificationGenericMessage(null, ex.Message, NotificationModeEnum.Toast, NotificationSeverityEnum.Error));
        }

        #endregion
    }
}
