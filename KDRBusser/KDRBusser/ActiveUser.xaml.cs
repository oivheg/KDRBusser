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
            BackgroundColor = Color.LightGray;
          
            InitializeComponent();
            btnActiveUser.Clicked += BtnActiveUser_clicked;
            //BtnLogout.Clicked += BtnLogout_Clicked;
            //BTNVibration1.Clicked += BTNVibration1_Clicked;
            //BTNVibration2.Clicked += BTNVibration2_Clicked;

            //does nothing right now
            //email = DependencyService.Get<IFCMLoginService>().GetEmail();
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

            // <---------- Test code for addind toolbar items in code --------------->
            //ToolbarItem _item = new ToolbarItem("Log Out", "", () => { });
            //_item.Clicked += ToolbarItem_Clicked;
            //ToolbarItem _item2 = new ToolbarItem("Vibration1", "", () => { });
            //_item.Clicked += ToolbarItem_Clicked;
            //ToolbarItem _item3 = new ToolbarItem("Vibration2", "", () => { });
            //_item.Clicked += ToolbarItem_Clicked;
            //this.ToolbarItems.Add(_item);
            //this.ToolbarItems.Add(_item2);
            //this.ToolbarItems.Add(_item3);
            //<--------------------------------->
            DependencyService.Get<IFCMLoginService>().IsLoading(true);
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
                DependencyService.Get<IFCMLoginService>().IsLoading(false);
                try
                {
                    user = JsonConvert.DeserializeObject<User>(RestApiCommunication.jsonresponse);
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