using KDRBusser.Classes;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KDRBusser
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActiveUser : ContentPage
    {
        public ActiveUser()
        {
            InitializeComponent();
            btnActiveUser.Clicked += BtnActiveUser_clicked;


        }

         bool isActive = false;
        private void BtnActiveUser_clicked(object sender, EventArgs e)
        {

            //Here we send code to onw server, so that master knows that user is logged in etc

            if (isActive)
            {
                btnActiveUser.BackgroundColor = (Color.ForestGreen);
                btnActiveUser.Text = "At Home";
                isActive = false;
                CommunicateDbAsync(mUser, false, true, false);
            }
            else
            {
                btnActiveUser.BackgroundColor = (Color.Yellow);
                btnActiveUser.Text = "At Work";
                CommunicateDbAsync(mUser, true, true, false);
                isActive = true;
            }
            
        }
        String mUser, tkn;
        int muserId = 22;
        public async void CommunicateDbAsync(String _user, bool _isActive, bool update, bool rmvAppId)
        {

            User user = new User();
            if (update)
            {
                String logout = "";
                user = new User(muserId, _user, tkn, _isActive);


                if (rmvAppId)
                {
                    logout = "?logout=True";
                    user.Active = false;
                }
                string json = JsonConvert.SerializeObject(user);

                // set button to active , change color etc


                //send POST request to REST API

                var content = new StringContent(json);
                var request = new HttpRequestMessage();
               String url = "http://91.189.171.231/restbusser/api/UserAPI/";
               
             
                var client = new HttpClient();
                HttpResponseMessage resposne;
                resposne = await client.PostAsync(url + "UserisActive" + logout, content);
                
            }
            else
            {
                user.Appid = tkn;
                //send request to REST API
                // with "FindUser" as adress string 

            }
        }
    }
}