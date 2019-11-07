using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace FluxControlAPI.Models.APIs.OpenALPR
{
    class OpenALPR
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> ProcessImage(byte[] bufferBytes)
        {
            string SECRET_KEY = "sk_658a8b17b8e5a26b537bd093";

            byte[] bytes = bufferBytes;
            string imagebase64 = Convert.ToBase64String(bytes);

            var content = new StringContent(imagebase64);

            var response = await client.PostAsync("https://api.openalpr.com/v2/recognize_bytes?recognize_vehicle=1&country=br&secret_key=" + SECRET_KEY, content).ConfigureAwait(false);

            var buffer = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var byteArray = buffer.ToArray();
            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            return responseString;
        }
    }
}