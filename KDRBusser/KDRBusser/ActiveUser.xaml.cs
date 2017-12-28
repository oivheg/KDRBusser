using KDRBusser.Classes;
using KDRBusser.Communication;
using KDRBusser.SharedCode;
using Newtonsoft.Json;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KDRBusser
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActiveUser : ContentPage
    {
        private bool isActive = false;
        private String mUser = "FromFCMLogin", tkn = "Fromm FCM APPID";

        public ActiveUser()
        {
            BackgroundColor = Color.LightGray;

            InitializeComponent();
            btnActiveUser.Clicked += BtnActiveUser_clicked;
            tkn = DependencyService.Get<IFCMLoginService>().GetToken();
        }

        public static int VibType { get; set; }

        private void BTNVibration2_Clicked(object sender, EventArgs e)
        {
            VibType = 2;
        }

        private void BTNVibration1_Clicked(object sender, EventArgs e)
        {
            VibType = 1;
        }

        //String email = "";
        protected override void OnAppearing()
        {
            base.OnAppearing();

            DependencyService.Get<IHelperClass>().IsLoading(true, "None/Bad network conenction");
            DependencyService.Get<IHelperClass>().IsLoading(true);
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

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            ToolbarItem _item = (ToolbarItem)sender;

            switch (_item.Text)
            {
                case "Log Out":
                    CommunicateDbAsync(mUser, false, true, true);
                    DependencyService.Get<IFCMLoginService>().LogOut();
                    break;

                case "Vibration2":
                    VibType = 2;
                    break;

                case "Vibration1":
                    VibType = 1;
                    break;

                default:
                    break;
            }
        }

        public async void CommunicateDbAsync(String _user, bool _isActive, bool update, bool rmvAppId)
        {
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
                DependencyService.Get<IHelperClass>().IsLoading(false);
                try
                {
                    user = JsonConvert.DeserializeObject<User>(RestApiCommunication.Jsonresponse);
                    isActive = user.Active;
                    lblemploy.Text = user.UserName;
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