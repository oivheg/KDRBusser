using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KDRBusser.Communication
{
    public class RestApiCommunication
    {
        private static String Base_URL = "http://91.189.171.231/restbusserv/api/UserAPI/";
        private static HttpResponseMessage Response { get; set; }
        public static String Jsonresponse { get; set; }

        public static async Task Post(Object user, String _Command)
        {
            var client = new HttpClient();
            string json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.Unicode, "application/json");
            var request = new HttpRequestMessage();
            Response = await client.PostAsync(Base_URL + _Command, content);
        }

        public static async Task Get(String _Command)
        {
            Jsonresponse = null;
            var request = new HttpRequestMessage();

            var client = new HttpClient();

            Response = await client.GetAsync(Base_URL + _Command);

            if (Response.IsSuccessStatusCode)
            {
                Jsonresponse = await Response.Content.ReadAsStringAsync();
            }
        }
    }
}