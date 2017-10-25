using Plugin.Connectivity;
using ProgressRingControl.Forms.Plugin;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Connectivity.Abstractions;
using System;

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
           
            while (!HavNetwork)
            {
                //if (!IsLoading)
                //{
                //    DependencyService.Get<IFCMLoginService>().IsLoading(true, "None/Bad network conenction");
                //    IsLoading = true;
                //}

                HavNetwork = CrossConnectivity.Current.IsConnected;
            }
            DependencyService.Get<IFCMLoginService>().Init();
        }

        bool HavNetwork = false;
        bool IsLoading = false;

        protected override void OnStart()
        {
            // Handle when your app starts
            HavNetwork = CrossConnectivity.Current.IsConnected;
            CrossConnectivity.Current.ConnectivityChanged += HandleConnectivityChanged;
        }

        private void HandleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            Type currentPage = this.MainPage.GetType();

            if (e.IsConnected)
            {
              bool isconencted = true;
                DependencyService.Get<IFCMLoginService>().IsLoading(false);
            }
            else if (!e.IsConnected)
            {
                bool isconencted = false;
                DependencyService.Get<IFCMLoginService>().IsLoading(true, "None/Bad network conenction");
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
