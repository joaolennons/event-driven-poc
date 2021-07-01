using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace System
{
    public static class HttpRequestExtensions
    {
        public async static Task<T> GetRequestBody<T>(this HttpRequest request) where T : new()
        {
            string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(requestBody);
        }
    }
}
