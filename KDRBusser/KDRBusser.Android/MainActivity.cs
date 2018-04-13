using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Firebase;
using Newtonsoft.Json;
using Plugin.Toasts;
using StaffBusser.Communication;
using StaffBusser.Droid.HelperClass;
using Xamarin.Forms;

namespace StaffBusser.Droid
{
    [Activity(Label = "Staff Busser", Icon = "@mipmap/ic_launcher",
        RoundIcon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
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
            DependencyService.Register<Droid_MyFirebaseMessagingService>();
            DependencyService.Register<MyBroadcastReceiver>();
            LoadApplication(new App());
        }

        public override void OnBackPressed()
        {
            //Include the code here
            return;
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}