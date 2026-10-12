namespace Cloud_API.src.Service.Url
{
    public interface IUrlService
    {
       public Task<IResult> GenerateUniqueToken(HttpContext context);

    }
}
