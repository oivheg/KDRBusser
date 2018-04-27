using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using Firebase;
using Newtonsoft.Json;
using Plugin.Toasts;
using StaffBusser.Classes;
using StaffBusser.Communication;
using StaffBusser.Droid.HelperClass;
using StaffBusser.SharedCode;
using System;
using System.Threading.Tasks;
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

        private Android.Net.Uri filePAth;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
            if (requestCode == 9001)
            {
                var result = Android.Gms.Auth.Api.Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                CheckANDLoginGoogle(result);
            }
            else if (requestCode == 71 && resultCode == Result.Ok && data?.Data != null)
            {
                filePAth = data.Data;
                try
                {
                    Bitmap bitmap = MediaStore.Images.Media.GetBitmap(ContentResolver, filePAth);
                    MediaStore.Images.Media.GetContentUri(filePAth.ToString());

                    ImageSourceConverter c = new ImageSourceConverter();
                    Image _img = ActiveUser.Instance.GetImage();
                    _img.Source = ImageSource.FromFile(data.Data.EncodedPath);
                }
                catch (System.Exception)
                {
                    throw;
                }
            }
        }

        private EditText et;
        private AlertDialog.Builder ad;
        private Android.Gms.Auth.Api.SignIn.GoogleSignInResult GoogleResult;

        private async void CheckANDLoginGoogle(Android.Gms.Auth.Api.SignIn.GoogleSignInResult result)
        {
            if (result.IsSuccess)
            {
                GoogleResult = result;
                // Google Sign In was successful, authenticate with Firebase
                // FirebaseAuthWithGoogle(result.SignInAccount);

                Boolean response = Convert.ToBoolean(await FindUser("oivheg@gmail.com"));
                //bool response = Convert.ToBoolean(   RestApiCommunication.PostMasterKey(Mstr, "ChckKey"));

                if (response)
                {
                    await DependencyService.Get<FCMLoginService>().FirebaseAuthWithGoogleAsync(result.SignInAccount);
                }
                else if (!response)
                {
                    et = new EditText(this);
                    //ad = new AlertDialog.Builder(this);
                    //ad.SetTitle("Input MasterId");
                    //ad.SetPositiveButton("OK", OkActionAsync);
                    //ad.SetView(et); // <----
                    //ad.Show();
                    // Build the dialog.
                    var builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Skriv inn MasterId");

                    // Create empty event handlers, we will override them manually instead of letting the builder handling the clicks.
                    builder.SetPositiveButton("Yes", (EventHandler<DialogClickEventArgs>)null);
                    builder.SetView(et);
                    var dialog = builder.Create();
                    dialog.Show();

                    // Get the buttons.
                    var yesBtn = dialog.GetButton((int)DialogButtonType.Positive);
                    // Get the buttons.
                    yesBtn.Click += async (sender, args) =>
                    {
                        string tmpstring = et.Text.ToString();
                        User Mstr = new User
                        {
                            MasterKey = tmpstring.Trim()
                        };
                        Boolean _correctMaster = await RestApiCommunication.PostMasterKey(Mstr, "ChckKey");
                        if (_correctMaster)
                        {
                            DependencyService.Get<IHelperClass>().IsLoading(true);
                            OkActionAsync();

                            dialog.Dismiss();
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Feil Master ID", " Sjekk Id og Prøv på nytt", "OK");
                        }
                        // Don't dismiss dialog.

                        Console.WriteLine("I am here to stay!");
                    };
                }
            }
            else
            {
                // Google Sign In failed, update UI appropriately
                // [START_EXCLUDE]
                // UpdateUI(null);
                // [END_EXCLUDE]
            }
        }

        private async void OkActionAsync()
        {
            //var myButton = sender as Android.Widget.Button; //this will give you the OK button on the dialog but you're already in here so you don't really need it - just perform the action you want to do directly unless I'm missing something..
            string tmpstring = et.Text.ToString();
            //User Mstr = new User
            //{
            //    MasterKey = tmpstring.Trim()
            //};
            //Boolean response = await RestApiCommunication.PostMasterKey(Mstr, "ChckKey");
            if (et.Text != null)
            {
                await DependencyService.Get<FCMLoginService>().FirebaseAuthWithGoogleAsync(GoogleResult.SignInAccount);
                //do something on ok selected

                User newUser = new User
                {
                    Email = GoogleResult.SignInAccount.Email,
                    UserName = GoogleResult.SignInAccount.GivenName,
                    MasterKey = tmpstring,
                    Appid = DroidHelperClass.Default.GetToken,
                    Active = false,
                };

                // Email = GoogleResult.SignInAccount.Email,
                //UserName = "Username",
                //MasterKey = tmpstring,
                //Appid = FCMLoginService.Instance.GetToken(),
                //Active = false

                await RestApiCommunication.Post(newUser, "CreateUser").ConfigureAwait(false);
            }
        }

        private static async Task<Boolean> FindUser(String email)
        {
            Boolean result = false;
            User user = new User();
            String parameters = "Appid=" + DroidHelperClass.Default.GetToken + "&" + "Email=" + email;
            user = JsonConvert.DeserializeObject<User>(await RestApiCommunication.Get("FindUser?" + parameters));
            if (user.Email != null)
            {
                result = true;
            }

            return result;
        }
    }
}