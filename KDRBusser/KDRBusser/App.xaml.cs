using Plugin.Connectivity;
using ProgressRingControl.Forms.Plugin;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Connectivity.Abstractions;
using System;
using Xamarin.Forms.Xaml;
using KDRBusser.SharedCode;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace KDRBusser
{
   
    public partial class App : Application
    {
        //is this in use ? IsUserLoggedIn ? probably not anymore
        public static bool IsUserLoggedIn { get; set; }
        public static string FCmToken { get; set; }

        public App()

        {
            InitializeComponent();
            MainPage = new FCmLogin();

            //DependencyService.Get<IHelperClass>().IsLoading(true, "None/Bad network conenction");

            while (!HavNetwork)
            {
                //if (!IsLoading)
                //{
                //    DependencyService.Get<IFCMLoginService>().IsLoading(true, "None/Bad network conenction");
                //    IsLoading = true;
                //}

                HavNetwork = CrossConnectivity.Current.IsConnected;
            }
          
        }

        bool HavNetwork = false;
        bool IsLoading = false;

        protected override void OnStart()
        {
            // Handle when your app starts
            HavNetwork = CrossConnectivity.Current.IsConnected;
            DependencyService.Get<IFCMLoginService>().Init();
            CrossConnectivity.Current.ConnectivityChanged += HandleConnectivityChanged;
        }

        private void HandleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            Type currentPage = this.MainPage.GetType();

            if (e.IsConnected)
            {
              //bool isconencted = true;
                DependencyService.Get<IHelperClass>().IsLoading(false);
            }
            else if (!e.IsConnected)
            {
                //bool isconencted = false;
                DependencyService.Get<IHelperClass>().IsLoading(true, "None/Bad network conenction");
            }



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
