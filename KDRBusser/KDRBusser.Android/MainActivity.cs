
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Gms.Common;
using Xamarin.Forms;
using Plugin.Toasts;
using Firebase;
using Firebase.Messaging;
using Firebase.Iid;
using Android.Util;
using Newtonsoft.Json;
using KDRBusser.Communication;
using ProgressRingControl.Forms.Plugin;
using Acr.UserDialogs;
using Plugin.Connectivity;

namespace KDRBusser.Droid
{
    [Activity(Label = "KDRBusser", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

      
        protected override void OnCreate(Bundle bundle)
        {

           
            base.OnCreate(bundle);
            //base.SetTheme(Resource.Style.MainTheme);
            Forms.Init(this, bundle);
            ToastNotification.Init(this);
            FirebaseApp.InitializeApp(this);
            UserDialogs.Init(this);
            DependencyService.Register<RestApiCommunication>();
            DependencyService.Register<FirebaseApp>(); // this probalby the reason it FCM 
            DependencyService.Register<JsonConverter>(); // this probalby the reason it FCM 
            DependencyService.Register<ToastNotification>(); // Register your dependency
           DependencyService.Register<MyFirebaseMessagingService>();
            //DependencyService.Register<CrossConnectivity>();
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;
            LoadApplication(new App());
           
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}

