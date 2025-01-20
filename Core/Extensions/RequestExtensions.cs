using Newtonsoft.Json;

namespace Donatas.Core.Extensions
{
    public static class RequestExtensions
    {
        // https://msdn.microsoft.com/en-us/magazine/mt238404.aspx Figure 6
        public static T? GetBody<T>(this HttpContent req, T body) =>
            body ?? JsonConvert.DeserializeObject<T>(Task.Run(async () => await req.ReadAsStringAsync().ConfigureAwait(false)).Result);
    }
}
