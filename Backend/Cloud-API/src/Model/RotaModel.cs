namespace Cloud_API
{
    public static class StreamsRoutes
    {
        public const string BaseMedia = "/api/v1/media";
        public const string BaseArchive = "/api/v1/archive";

        public const string BaseConsultae = "/api/v1/search";

        public static class UrlBase
        {
            public const string Upload = "/api/v1/inbound/";
            public const string Donwload = "/api/v1/output/{token}";
        }

        //public static class Task
        //{
        //    public const string All = "/all";
        //    public const string Filter = "/filter/{token}";
        //    public const string Erro = "/erro/{token}";

        //    public const string AllCache = "/cache";
        //    public const string FilterCache = "/cache/filter/{token}";

        //}

        public static class Consulta
        {
            public const string Base = "/base/{token}";
            public const string BaseNoToken = "/base";
        }

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
