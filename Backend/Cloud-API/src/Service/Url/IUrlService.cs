namespace Cloud_API.src.Service.Url
{
    public interface IUrlService
    {
        public Task<IResult> UrlGenerateUpload(HttpContext context);

        public Task<IResult> UrlGenerateDownload(string token, HttpContext context);

    }
}
