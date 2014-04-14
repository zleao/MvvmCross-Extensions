using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Location;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Libraries.Portable.Attributes;
using MvvmCrossUtilities.Plugins.Notification;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.OneWay;
using MvvmCrossUtilities.Plugins.Rest.Request;
using MvvmCrossUtilities.Plugins.Rest.Response;
using MvvmCrossUtilities.Samples.AllAround.Core.Models.Rest;

namespace MvvmCrossUtilities.Samples.AllAround.Core.Rest
{
    public class ArticlesViewModel : BaseRestViewModel
    {
        #region Constants

        public const string MESSAGE_MANDATORY = "Mandatory";
        public const string MESSAGE_INVALID = "Invalid";
        public const string MESSAGE_BETWEEN = "Between {0} and {1}";

        #endregion

        #region Properties

        public override string PageTitle
        {
            get { return "REST - Articles"; }
        }

        //api info here: http://wikilocation.org/documentation/#api-articles
        protected override string ServiceUrl
        {
            get { return @"http://api.wikilocation.org/"; }
        }

        protected override string ServiceName
        {
            get { return "articles"; }
        }

        public ObservableCollection<Article> Articles
        {
            get { return _articles; }
            set
            {
                if (_articles != value)
                {
                    _articles = value;
                    RaisePropertyChanged(() => Articles);
                }
            }
        }
        private ObservableCollection<Article> _articles = new ObservableCollection<Article>();

        public IList<string> Locales
        {
            get { return _locales; }
        }
        private readonly IList<string> _locales = new List<string>() { "ar", "bg", "ca", "cs", "da", "de", "en", "eo", "es", "fa", "fi", "fr", "gl", "he", "hu", "id", "it", "ja", "ko", "lt", "ms", "nn", "no", "pl", "pt", "ro", "ru", "simple", "sk", "sl", "sr", "sv", "tr", "uk", "vi", "vo", "war", "zh" };
        //Taken from http://wikilocation.org/documentation/#locales

        public string SelectedLocale
        {
            get { return _locale; }
            set
            {
                if (_locale != value)
                {
                    _locale = value;
                    RaisePropertyChanged(() => SelectedLocale);
                }
            }
        }
        private string _locale;
        
        public bool MakeXmlRequest
        {
            get { return _makeXmlRequest; }
            set
            {
                if (_makeXmlRequest != value)
                {
                    _makeXmlRequest = value;
                    RaisePropertyChanged(() => MakeXmlRequest);
                }
            }
        }
        private bool _makeXmlRequest;

        public string Latitude
        {
            get { return _latitude; }
            set
            {
                if (_latitude != value)
                {
                    _latitude = value;
                    RaisePropertyChanged(() => Latitude);
                }
            }
        }
        private string _latitude;
        public string LatitudeError
        {
            get { return _latitudeError; }
            set
            {
                if (_latitudeError != value)
                {
                    _latitudeError = value;
                    RaisePropertyChanged(() => LatitudeError);
                }
            }
        }
        private string _latitudeError;

        public string Longitude
        {
            get { return _longitude; }
            set
            {
                if (_longitude != value)
                {
                    _longitude = value;
                    RaisePropertyChanged(() => Longitude);
                }
            }
        }
        private string _longitude;
        public string LongitudeError
        {
            get { return _longitudeError; }
            set
            {
                if (_longitudeError != value)
                {
                    _longitudeError = value;
                    RaisePropertyChanged(() => LongitudeError);
                }
            }
        }
        private string _longitudeError;

        public string Radius
        {
            get { return _radius; }
            set
            {
                if (_radius != value)
                {
                    _radius = value;
                    RaisePropertyChanged(() => Radius);
                }
            }
        }
        private string _radius;
        public string RadiusError
        {
            get { return _radiusError; }
            set
            {
                if (_radiusError != value)
                {
                    _radiusError = value;
                    RaisePropertyChanged(() => RadiusError);
                }
            }
        }
        private string _radiusError;
        
        public string MaxResults
        {
            get { return _maxResults; }
            set
            {
                if (_maxResults != value)
                {
                    _maxResults = value;
                    RaisePropertyChanged(() => MaxResults);
                }
            }
        }
        private string _maxResults;
        public string MaxResultsError
        {
            get { return _maxResultsError; }
            set
            {
                if (_maxResultsError != value)
                {
                    _maxResultsError = value;
                    RaisePropertyChanged(() => MaxResultsError);
                }
            }
        }
        private string _maxResultsError;
        
        public string SearchTitle
        {
            get { return _searchTitle; }
            set
            {
                if (_searchTitle != value)
                {
                    _searchTitle = value;
                    RaisePropertyChanged(() => SearchTitle);
                }
            }
        }
        private string _searchTitle;
        
        protected IMvxLocationWatcher GPS
        {
            get { return _gps ?? (_gps = Mvx.Resolve<IMvxLocationWatcher>()); }
        }
        private IMvxLocationWatcher _gps;

        public bool GpsEnabled
        {
            get { return _gpsEnabled; }
            set
            {
                if (_gpsEnabled != value)
                {
                    _gpsEnabled = value;
                    RaisePropertyChanged(() => GpsEnabled);
                }
            }
        }
        private bool _gpsEnabled;

        #endregion

        #region Commands

        public ICommand SetDefaultValuesCommand
        {
            get { return _setDefaultValuesCommand ?? (_setDefaultValuesCommand = new MvxCommand(() => ResetValues())); }
        }
        private ICommand _setDefaultValuesCommand;

        #endregion

        #region Methods

        public void Init()
        {
            ResetValues();

        }

        [DependsOn("GpsEnabled")]
        public void ToggleGps()
        {
            if (GpsEnabled)
            {
                GPS.Start(new MvxLocationOptions() { Accuracy = MvxLocationAccuracy.Fine, MovementThresholdInM = 0, TimeBetweenUpdates = TimeSpan.Zero }, OnNewCoordinates, OnGpsError);
                StartWork("Connecting to GPS");
            }
            else if (GPS.Started)
                GPS.Stop();
        }

        private void OnNewCoordinates(MvxGeoLocation obj)
        {
            if(IsBusy)
                FinishedWork();

            Latitude = obj.Coordinates.Latitude.ToString();
            Longitude = obj.Coordinates.Longitude.ToString();
        }

        private void OnGpsError(MvxLocationError obj)
        {
            if (IsBusy)
                FinishedWork();

            Mvx.Resolve<INotificationService>().Publish(new NotificationGenericMessage(this, obj.Code.ToString(), NotificationModeEnum.Toast, NotificationSeverityEnum.Error));
            GpsEnabled = false;
        }

        protected override bool ValidateData()
        {
            var hasErrors = false;

            ResetErrorMessages();

            if (string.IsNullOrEmpty(Latitude))
            {
                LatitudeError = MESSAGE_MANDATORY;
                hasErrors = true;
            }

            if (string.IsNullOrEmpty(Longitude))
            {
                LongitudeError = MESSAGE_MANDATORY;
                hasErrors = true;
            }

            int intRadius = 250;
            if (!string.IsNullOrEmpty(Radius) && !int.TryParse(Radius, out intRadius))
            {
                LatitudeError = MESSAGE_INVALID;
                hasErrors = true;
            }
            else
            {
                if (intRadius < 0 || intRadius > 20000)
                {
                    RadiusError = string.Format(MESSAGE_BETWEEN, 0, 20000);
                    hasErrors = true;
                }
            }

            int intLimit = 50;
            if (!string.IsNullOrEmpty(MaxResults) && !int.TryParse(MaxResults, out intLimit))
            {
                LatitudeError = MESSAGE_INVALID;
                hasErrors = true;
            }
            else
            {
                if (intLimit < 1 || intLimit > 50)
                {
                    MaxResultsError = string.Format(MESSAGE_BETWEEN, 1, 50);
                    hasErrors = true;
                }
            }
            
            return !hasErrors;
        }

        protected override void MakeRequest()
        {
            MakeRequestFor<ArticlesSearchResult>();
        }

        protected override RestRequest BuildRequest()
        {
            var request = base.BuildRequest();

            request.AddParameter(new Parameter("locale", SelectedLocale));
            request.AddParameter(new Parameter("format", MakeXmlRequest ? "xml" : "json"));
            request.AddParameter(new Parameter("lat", Latitude));
            request.AddParameter(new Parameter("lng", Longitude));
            request.AddParameter(new Parameter("radius", Radius));
            request.AddParameter(new Parameter("limit", MaxResults));
            request.AddParameter(new Parameter("title", SearchTitle));

            return request;
        }

        protected override void RequestSuccessCallback<TResponse>(RestResponse<TResponse> response)
        {
            var typedResponse = response as RestResponse<ArticlesSearchResult>;
            if (typedResponse != null && typedResponse.Data != null && typedResponse.Data.articles != null && typedResponse.Data.articles.Count > 0)
            {
                Articles = new ObservableCollection<Article>(typedResponse.Data.articles);
            }
            else
            {
                Articles.Clear();
                Mvx.Resolve<INotificationService>().Publish(new NotificationGenericMessage(this, "No articles found", NotificationModeEnum.Toast, NotificationSeverityEnum.Info));
            }
        }

        private void ResetValues()
        {
            //Taken from http://wikilocation.org/documentation/
            SelectedLocale = "en";
            MakeXmlRequest = false;
            Radius = "250";
            MaxResults = "50";
            SearchTitle = string.Empty;

            if (!GPS.Started)
            {
                Latitude = "51.500688";
                Longitude = "-0.124411";
            }

            ResetErrorMessages();
        }

        private void ResetErrorMessages()
        {
            LatitudeError = null;
            LongitudeError = null;
            RadiusError = null;
            MaxResultsError = null;
        }

        #endregion
    }
}
