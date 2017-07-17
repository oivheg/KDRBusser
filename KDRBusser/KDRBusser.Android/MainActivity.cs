using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Firebase.Messaging;
using Firebase.Iid;
using Android.Util;
using Xamarin.Forms;
using Plugin.Toasts;
using Android.Gms.Common;
using Firebase;

namespace KDRBusser.Droid
{
    [Activity(Label = "KDRBusser", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

      
        protected override void OnCreate(Bundle bundle)
        {
           
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            //FirebaseApp.InitializeApp(this);
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            //DependencyService.Register<FirebaseApp>();
            
            DependencyService.Register<ToastNotification>(); // Register your dependency
            ToastNotification.Init(this);
            LoadApplication(new App());
        }
    }
}

