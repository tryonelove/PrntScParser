using System;
using System.Text;
using System.Drawing;
using System.Net;
using System.IO;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;

namespace ImageParser
{
    public class Parser
    {
        static string[] USER_AGENTS = {
                "Mozilla/5.0 (Linux; Android 8.0.0; SM-G960F Build/R16NW) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.84 Mobile Safari/537.36",
                "Mozilla/5.0 (Linux; Android 7.0; SM-G892A Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/60.0.3112.107 Mobile Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.246",
                "Mozilla/5.0 (X11; CrOS x86_64 8172.45.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.64 Safari/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_2) AppleWebKit/601.3.9 (KHTML, like Gecko) Version/9.0.2 Safari/601.3.9",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 Safari/537.36",
                "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:15.0) Gecko/20100101 Firefox/15.0.1"
            };
        string _baseUrl;
        public Parser(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        static string GetRandomUserAgent()
        {
            Random rnd = new Random();
            return Parser.USER_AGENTS[rnd.Next(0, USER_AGENTS.Length)];
        }

        string GenerateRandomString(int size = 6, bool lower = false)
        {
            const string charSet = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            Random rnd = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = charSet[rnd.Next(0, charSet.Length)];
                builder.Append(ch);
            }
            var randomString = builder.ToString();
            if (lower)
            {
                return randomString.ToLower();
            }
            return randomString;
        }

        async static Task<string> FindImageUrlAsync(string url)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            string userAgent = GetRandomUserAgent();
            client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            var response = await client.GetAsync(url);
            var pageContents = await response.Content.ReadAsStringAsync();
            return Regex.Match(pageContents, "<img.+?src=[\"'](.+?)[\"'].*?>").Groups[1].Value;
        }

        void DownloadImage(string url, string filename)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(url);

                using (MemoryStream mem = new MemoryStream(data))
                {
                    using (var yourImage = Image.FromStream(mem))
                    {
                        yourImage.Save($"Images/{filename}.jpeg", ImageFormat.Jpeg);
                    }
                }

            }
        }

        public void GetImages(int count)
        {
            for (int i = 0; i < count; i++)
            {
                string randomStr = GenerateRandomString(lower: true);
                var url = _baseUrl + randomStr;
                try
                {
                    var imageUrl = FindImageUrlAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
                    Console.WriteLine(imageUrl);
                    DownloadImage(imageUrl, randomStr);
                    Thread.Sleep(1000);
                }
                catch
                {
                    Console.WriteLine("Captcha");
                    continue;
                }
            }
        }
    }
}