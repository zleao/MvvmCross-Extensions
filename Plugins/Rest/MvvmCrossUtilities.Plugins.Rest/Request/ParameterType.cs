namespace MvvmCrossUtilities.Plugins.Rest.Request
{
    ///<summary>
    /// Types of parameters that can be added to requests
    ///</summary>
    public enum ParameterType
    {
        Cookie,
        GetOrPost,
        UrlSegment,
        HttpHeader,
        RequestBody,
        QueryString
    }
}
