namespace Cloud_API
{
    public static class StreamsRoutes
    {
        public const string BaseMedia = "/media";
        public const string BaseArchive = "/archive";

        public static class UrlBase
        {
            public const string Upload = "/inbound/";
            public const string Donwload = "/output/{token}";
        }

        //public static class Task
        //{
        //    public const string All = "/all";
        //    public const string Filter = "/filter/{token}";
        //    public const string Erro = "/erro/{token}";

        //    public const string AllCache = "/cache";
        //    public const string FilterCache = "/cache/filter/{token}";

        //}
        public static class DownloadBase
        {
            public const string BaseNoToken = "/download/base";
            public const string Base = "/download/base/{token}";
        }

        public static class UploadBase
        {
            public const string BaseNoToken = "/upload/base";
            public const string Base = "/upload/base/{token}";           
        }

        public static class  DeleteBase
        {
            public const string Base = "/delete/base/{token}";  
        }

  
    }
}
