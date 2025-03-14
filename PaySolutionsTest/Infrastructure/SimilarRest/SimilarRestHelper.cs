using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

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

            HttpClient client = new HttpClient();
            //в описании тестового не написан протокол у хоста - убил часа 1.5 на то что на http 405 прилетало.
            var builder = new UriBuilder()
            {
                Host = host,
                Scheme = "https",
            };
            client.BaseAddress = builder.Uri;
            var request = new HttpRequestMessage(method, route);
            

            var encodedPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));

            var protectedHeader = new { alg = "HS256", kid = keyId, signdate = signDate, cty = "application/json" };
            var encodedProtectedHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(protectedHeader)));
            var content = $"{encodedProtectedHeader}.{encodedPayload}."+
                $"{Convert.ToBase64String(HMACSHA256.HashData(Encoding.UTF8.GetBytes(key),
                Encoding.UTF8.GetBytes($"{encodedProtectedHeader}.{encodedPayload}")))}";

            request.Content = new StringContent(JsonConvert.SerializeObject(payload));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", content);

            //увидел jose библиотеку, но уже после того как всё написал, решил оставить так.
            var result = await client.SendAsync(request, ct);
            return await result.Content.ReadAsStringAsync(ct);
        }
    }
}
