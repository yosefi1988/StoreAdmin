using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace WebApplicationCar.Services
{
    public static class ApiService
    {
        private static readonly HttpClient _client = new HttpClient();

        static ApiService()
        {
            var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];

            if (string.IsNullOrEmpty(baseUrl))
                throw new ConfigurationErrorsException("ApiBaseUrl در Web.config تنظیم نشده است");

            // اگه نسبی بود (مثل /x_car/) → آدرس کامل بساز
            if (baseUrl.StartsWith("/"))
            {
                var request = System.Web.HttpContext.Current?.Request;
                if (request != null)
                {
                    baseUrl = $"{request.Url.Scheme}://{request.Url.Authority}{baseUrl}";
                }
                else
                {
                    throw new ConfigurationErrorsException(
                        "ApiBaseUrl نسبی است اما HttpContext در دسترس نیست");
                }
            }

            _client.BaseAddress = new Uri(baseUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static T Get<T>(string endpoint)
        {
            var response = _client.GetAsync(endpoint).Result;
            response.EnsureSuccessStatusCode();
            var json = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T Post<T>(string endpoint, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = _client.PostAsync(endpoint, content).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}