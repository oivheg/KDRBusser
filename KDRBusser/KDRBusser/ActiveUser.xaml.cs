using KDRBusser.Classes;
using KDRBusser.Communication;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
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
            // button logout. clicked += BtnLogout_Clicked
            BtnLogout.Clicked += BtnLogout_Clicked;

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

        private void BtnLogout_Clicked(object sender, EventArgs e)
        {
            // here ode for loging user out should rund.
            // this wil also run firebase logout command on each Platfrom. probably use the exsisitng itnerface wiht logout method etc. 
            CommunicateDbAsync(mUser, false, true, true);
            DependencyService.Get<IFCMLoginService>().LogOut();

        }

        String mUser = "FromFCMLogin", tkn = "Fromm FCM APPID";
        
        public async void CommunicateDbAsync(String _user, bool _isActive, bool update, bool rmvAppId)
        {

            tkn = DependencyService.Get<IFCMLoginService>().GetToken();
            User user = new User();
            if (update)
            {
                String logout = "";
                user = new User( _user, tkn, _isActive);


                if (rmvAppId)
                {
                    logout = "?logout=True";
                    user.Active = false;
                }

                // TEst CODE TKN is added manualy.

                //user.Appid = "ddT9UEWD6nc:APA91bERselu5IieP5AWVl0UWVdEUIc3Ienpcx7z6i-pZtjSh5FXYJ8o12NBMw4sH9KB1-Ds3v4xIdFqRuvbYPXmk92byGsng-Zm4Y1eIO-hx0EhWnzm130Vu0g2zP8xaIJxBIy_1Ima";

                //send POST request to REST API
                // old json code, created 415 error
                //var content = new StringContent(json);

                // This is Method from this class, THe prorper way is to use the RestApiCommunication Class, 
                //await HttpRequestHandler(user, "UserisActive/");
                await RestApiCommunication.Post(user, "UserisActive/"+logout);
            }
            else
            {
                user.Appid = tkn;
                //send request to REST API
                // with "FindUser" as adress string 
                await RestApiCommunication.Post(user, "FindUser/");

            }
        }

        //Local method, RestApiCommunication class, replaces this.

            //DEPRECATED
        private static async System.Threading.Tasks.Task HttpRequestHandler(User user, String _Command)
        {
            string json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage();
            String url = "http://91.189.171.231/restbusserv/api/UserAPI/";
            //Test adress on local computer 
            url = "http://10.0.0.159:51080/api/UserAPI/";
            var client = new HttpClient();
            HttpResponseMessage resposne;

            //post
            resposne = await client.PostAsync(url + _Command, content);

            //Get

            //resposne = await client.GetAsync(url + _Command);
        }
    }
}