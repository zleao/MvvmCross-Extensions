using System.Collections.Generic;
using System.Net;

namespace MvvmCrossUtilities.Plugins.Rest.Request
{
    public class RestRequest
    {
        #region Properties

        public string Url { get; set; }

        public string Resource { get; set; }

        public Method Method { get; set; }

        public DataFormat RequestFormat { get; set; }

        public ICredentials Credentials { get; set; }

        public string DateFormat { get; set; }

        public object Body { get; set; }


        public IDictionary<string, string> Cookies { get; private set; }

        public IDictionary<string, string> Files { get; private set; }

        public IDictionary<string, string> Headers { get; private set; }

        public IDictionary<string, string> UrlSegments { get; private set; }

        public IDictionary<object, string[]> Objects { get; private set; }

        #endregion

        #region Constructor

        public RestRequest(string url, Method method = Method.GET, DataFormat requestFormat = DataFormat.Json)
        {
            Url = url;
            Method = method;
            RequestFormat = requestFormat;

            Cookies = new Dictionary<string, string>();
            Files = new Dictionary<string, string>();
            Headers = new Dictionary<string, string>();
            UrlSegments = new Dictionary<string, string>();
            Objects = new Dictionary<object, string[]>();
        }

        #endregion

        #region Methods

        public void AddCookie(string name, string value)
        {
            if (!Cookies.ContainsKey(name))
                Cookies.Add(name, value);
            else
                Cookies[name] = value;
        }

        public void AddFile(string name, string path)
        {
            if (!Files.ContainsKey(name))
                Files.Add(name, path);
            else
                Files[name] = path;
        }

        public void AddHeader(string name, string value)
        {
            if (!Headers.ContainsKey(name))
                Headers.Add(name, value);
            else
                Headers[name] = value;
        }

        public void AddUrlSegment(string name, string value)
        {
            if (!UrlSegments.ContainsKey(name))
                UrlSegments.Add(name, value);
            else
                UrlSegments[name] = value;
        }

        public void AddObject(object obj, params string[] whiteList)
        {
            Objects.Add(obj, whiteList);
        }

        public void SetCompression(Compression compressionMode)
        {
            switch (compressionMode)
            {
                case Compression.GZip:
                    AddHeader("Accept-Encoding", "gzip");
                    break;
            }
        }

        #endregion
    }
}
