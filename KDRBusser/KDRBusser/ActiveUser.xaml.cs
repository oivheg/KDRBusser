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
        bool isActive = false;
        String mUser = "FromFCMLogin", tkn = "Fromm FCM APPID";
        public ActiveUser()
        {
            InitializeComponent();
            btnActiveUser.Clicked += BtnActiveUser_clicked;
            BtnLogout.Clicked += BtnLogout_Clicked;

            //does nothing right now
            //email = DependencyService.Get<IFCMLoginService>().GetEmail();
        }
        //String email = "";
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CommunicateDbAsync(mUser, false, false, false);
        }

        private void BtnActiveUser_clicked(object sender, EventArgs e)
        {
            //Here we send code to onw server, so that master knows that user is logged in etc
            if (isActive)
            {
                CommunicateDbAsync(mUser, true, true, false);

            }
            else
            {
                CommunicateDbAsync(mUser, false, true, false);
            }
            IsUserActive();

        }

        private void IsUserActive()
        {
            if (isActive)
            {

                ChangeButton(Color.Yellow, "At WORK", false);
            }

            else
            {
                ChangeButton(Color.ForestGreen, "At Home", true);

            }
        }

        private void ChangeButton(Color btncolor, String btnText, Boolean UserActive)
        {
            btnActiveUser.BackgroundColor = (btncolor);
            btnActiveUser.Text = btnText;
            isActive = UserActive;
        }

        private void BtnLogout_Clicked(object sender, EventArgs e)
        {
            // here code for loging user out should rund.
            // this wil also run firebase logout command on each Platfrom. probably use the exsisitng itnerface wiht logout method etc. 
            CommunicateDbAsync(mUser, false, true, true);
            DependencyService.Get<IFCMLoginService>().LogOut();

        }



        public async void CommunicateDbAsync(String _user, bool _isActive, bool update, bool rmvAppId)
        {

            tkn = DependencyService.Get<IFCMLoginService>().GetToken();
            User user = new User();
            if (update)
            {
                String logout = "";
                //_user = "oivheg@gmail.com";
                user = new User(_user, tkn, _isActive);

                if (rmvAppId)
                {
                    logout = "?logout=True";
                    user.Active = false;
                }

                await RestApiCommunication.Post(user, "UserisActive/" + logout);
            }
            else
            {

                //send request to REST API
                // with "FindUser" as adress string 

                String parameters = "Appid=" + tkn;
                if (tkn == null)
                {
                    return;
                }
                await RestApiCommunication.Get("FindUser?" + parameters);
                try
                {
  user = JsonConvert.DeserializeObject<User>(RestApiCommunication.jsonresponse);
                isActive = user.Active;
                IsUserActive();
                }
                catch
                {
                    
                    DependencyService.Get<IFCMLoginService>().LogOut();
                }
              

            }
        }


    }
}