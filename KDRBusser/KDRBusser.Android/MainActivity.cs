
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

namespace KDRBusser.Droid
{
    [Activity(Label = "KDRBusser", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

      
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Forms.Init(this, bundle);
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            FirebaseApp.InitializeApp(this);
            LoadApplication(new App());

            DependencyService.Register<RestApiCommunication>();
            DependencyService.Register<FirebaseApp>(); // this probalby the reason it FCM 
            DependencyService.Register<JsonConverter>(); // this probalby the reason it FCM 
            DependencyService.Register<ToastNotification>(); // Register your dependency
           DependencyService.Register<MyFirebaseMessagingService>();
            ToastNotification.Init(this);
             
           
        }
    }
}

