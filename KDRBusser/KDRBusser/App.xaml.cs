using Xamarin.Forms;

namespace KDRBusser
{
    public partial class App : Application
    {
        public static bool IsUserLoggedIn { get; set; }
        public static string FCmToken { get; set; }
     
        public App()

        {
            //InitializeComponent();
            IsUserLoggedIn = false;
            IsUserLoggedIn = DependencyService.Get<IFCMLoginService>().IsLoggedIn();
            ChangeActivity();
            

        }

        public void ChangeActivity()
        {

            // here i should run check, if FCMUser is logged in.
            if (IsUserLoggedIn)
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
