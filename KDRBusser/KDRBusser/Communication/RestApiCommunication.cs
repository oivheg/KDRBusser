using KDRBusser.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KDRBusser.Communication
{
   public class RestApiCommunication
    {
        private static String Base_URL = "http://91.189.171.231/restbusserv/api/UserAPI/";
       private static HttpResponseMessage response { get;  set; }
        public static String jsonresponse { get; set; }
       
        public static async Task Post(Object user , String _Command)
        {

            var client = new HttpClient();
            string json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.Unicode, "application/json");
            var request = new HttpRequestMessage();
            response = await client.PostAsync(Base_URL + _Command, content);
        }

        public static async  Task Get( String _Command)
        {
            jsonresponse = null;
            var request = new HttpRequestMessage();
            //string json = JsonConvert.SerializeObject(user);
            //var content = new StringContent(json, Encoding.UTF8, "application/json");
            var client = new HttpClient();

            response = await client.GetAsync(Base_URL + _Command);

            if (response.IsSuccessStatusCode)
            {
                 jsonresponse = await response.Content.ReadAsStringAsync();
               
            }
        }


       

    }
}
