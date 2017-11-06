
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
using KDRBusser.SharedCode;
using ProgressRingControl.Forms.Plugin;
using Acr.UserDialogs;
using Plugin.Connectivity;

namespace KDRBusser.Droid
{
    [Activity(Label = "KDRBusser", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        //Theme = "@style/MainTheme"
        protected override void OnCreate(Bundle bundle)
        {
            //base.SetTheme(global::Android.Resource.Style.ThemeHoloLight);
            base.SetTheme(global::Android.Resource.Style.ThemeHoloLight);
            base.OnCreate(bundle);
            ActionBar.SetDisplayShowTitleEnabled(false);
            ActionBar.SetDisplayShowHomeEnabled(false);
            ActionBar.Hide();

          
       //Initialises the idffernet classes.
            Forms.Init(this, bundle);
            ToastNotification.Init(this);
            FirebaseApp.InitializeApp(this);
            UserDialogs.Init(this);

            //seems to be no longer needed, prorably som bug fix update in teh xamarin.forms nuget.
            //make sure the libaries are added to anroid 
            DependencyService.Register<RestApiCommunication>();
            //DependencyService.Register<IHelperClass>();
            DependencyService.Register<FirebaseApp>(); // this probalby the reason it FCM 
            DependencyService.Register<JsonConverter>(); // this probalby the reason it FCM 
            DependencyService.Register<ToastNotification>(); // Register your dependency
            DependencyService.Register<MyFirebaseMessagingService>();
            LoadApplication(new App());
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}

