﻿using StaffBusser.SharedCode;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StaffBusser.Communication
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

        public static async Task<Boolean> PostMasterKey(Object user, String _Command)
        {
            var client = new HttpClient();
            string json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.Unicode, "application/json");
            var request = new HttpRequestMessage();
            Response = await client.PostAsync(Base_URL + _Command, content);
            Jsonresponse = await Response.Content.ReadAsStringAsync();
            if (Jsonresponse.ToLower().Equals("true"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<String> Get(String _Command)
        {
            Jsonresponse = "";
            var request = new HttpRequestMessage();

            var client = new HttpClient();

            Response = await client.GetAsync(Base_URL + _Command);

            if (Response.IsSuccessStatusCode)
            {
                Jsonresponse = await Response.Content.ReadAsStringAsync();
            }
            return await Response.Content.ReadAsStringAsync();
        }
    }
}