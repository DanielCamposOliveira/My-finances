namespace Cloud_API
{
    public static class StreamsRoutes
    {
        public const string BaseMedia = "/media";
        public const string BaseArchive = "/archive";
        public const string BaseTask = "/info/task";
        public const string UrlBase = "/streams";

        public static class UrlUpload
        {
            public const string Base = "/inbound/";
        }

        public static class Task
        {
            public const string All = "/all";
            public const string Filter = "/filter/{token}";
            public const string Erro = "/erro/{token}";

            public const string AllCache = "/cache";
            public const string FilterCache = "/cache/filter/{token}";

        }
        public static class Download
        {
            public const string Base = "/download/base/{token}";           
        }

        public static class Upload
        {
            public const string BaseNoToken = "/upload/base";
            public const string Base = "/upload/base/{token}";           
        }

        public static class  Delete
        {
            public const string Task = "/delete/task/{token}";
            public const string Base = "/delete/base/{token}";          
            public const string Erro = "/delete/erro/{token}";
        }

        public static class Move
        {
            public const string Erro = "/move/erro/{token}";
        }
    }
}
