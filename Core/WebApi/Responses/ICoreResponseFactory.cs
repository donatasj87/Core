namespace Donatas.Core.WebApi.Responses
{
    public interface ICoreResponseFactory
    {
        ICoreResponse GetCoreResponse(Exception ex);
    }
}
