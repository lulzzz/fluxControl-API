﻿using System;
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
        private string SECRET_KEY { get; set; }

        public OpenALPR(string secretKey)
        {
            SECRET_KEY = secretKey;
        }

        public async Task<string> ProcessImage(byte[] bufferBytes)
        {

            byte[] bytes = bufferBytes;
            string imagebase64 = Convert.ToBase64String(bytes);

            var content = new StringContent(imagebase64);

            var response = await client.PostAsync(
                "https://api.openalpr.com/v2/recognize_bytes?recognize_vehicle=0&country=br&secret_key=" + SECRET_KEY, content
                ).ConfigureAwait(false);

            var buffer = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var byteArray = buffer.ToArray();
            var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            return responseString;
        }
    }
}