
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

namespace KDRBusser.Droid
{
    [Activity(Label = "KDRBusser", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

      
        protected override void OnCreate(Bundle bundle)
        {

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
           // FirebaseApp.InitializeApp(this);
            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            DependencyService.Register<FirebaseApp>(); // this probalby the reason it FCM 
            DependencyService.Register<ToastNotification>(); // Register your dependency
            ToastNotification.Init(this);
            LoadApplication(new App());
        }
    }
}

