using System;
using Xamarin.Forms;

namespace KDRBusser
{
    public partial class App : Application
    {
        public App()

        {
            //InitializeComponent();

            
              Boolean isloggedin = false;


             isloggedin = DependencyService.Get<IFCMLoginService>().Init();

            // here i should run check, if FCMUser is logged in.
            if (isloggedin)
            {
                //Start activity screen
                MainPage = new ActiveUser();
            }
            else
            {
                // start loging screen.
                MainPage = new FCmLogin();
                
            }
           

           
        }

        public void setIsLoggedIn()
        {

        }
        

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
