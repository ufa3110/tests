using Jose;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PaySolutionsTest.Infrastructure.SimilarRest
{
    internal static class SimilarRestHelper
    {
        //тестовое вроде не про надежность хранения ключей, имхо в коде оставить допустимо.
        const string host = "acstopay.online";
        const string key = "ac/1LUdrbivclAeP67iDKX2gPTTNmP0DQdF+0LBcPE/3NWwUqm62u5g6u+GE8uev5w/VMowYXN8ZM+gWPdOuzg==";
        const string keyId = "47e8fde35b164e888a57b6ff27ec020f";

        public static async Task<string> SendQueryAsync(object payload, HttpMethod method, string route = "api/echo", CancellationToken ct = default)
        {
            //можно всё обернуть в try catch, половить ошибки, но в рамках этого запроса неуспешные операции приходят со статусом failed, рантаймы (или таймауты) ловить можно конечно, но немного утяжелит код сейчас.
            var signDate = DateTime.UtcNow.ToString("o");

            HttpClient client = new();
            var builder = new UriBuilder()
            {
                Host = host,
                Scheme = "https",
            };
            client.BaseAddress = builder.Uri;
            var request = new HttpRequestMessage(method, route);

            var protectedHeaders = new Dictionary<string, object>()
            {
                { "kid" , keyId },
                { "signdate" , signDate },
                { "cty" , "application/json" }
            };

            string token1 = Jose.JWT.Encode(payload, Convert.FromBase64String(key), JwsAlgorithm.HS256, protectedHeaders);

            request.Content = new StringContent(token1);
            var response = await client.SendAsync(request, ct);
            var rawResponse = (await response.Content.ReadAsStringAsync(ct)).Split('.').Skip(1).First();

            return Base64UrlEncoder.Decode(rawResponse);
        }
    }
}
