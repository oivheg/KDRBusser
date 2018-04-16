using Firebase.Auth;
using Firebase.InstanceID;
using Foundation;
using StaffBusser.Communication;
using StaffBusser.iOS.FCM;
using StaffBusser.SharedCode;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(IOS_FCMLoginService))]

namespace StaffBusser.iOS.FCM
{
    internal class IOS_FCMLoginService : IFCMLoginService
    {
        private Auth auth;

        //public IOS_FCMLoginService()
        //{
        //}
        public async void CreateuserAsync(string email, string password, string masterid, string UserName)
        {
            Classes.User newUser = new Classes.User
            {
                Email = email,
                UserName = UserName.Trim(),
                MasterKey = masterid.Trim(),
                Appid = "not initialised".Trim(),
                Active = false
            };
            await RestApiCommunication.Post(newUser, "CreateUser").ConfigureAwait(false);
            auth.CreateUser(email, password, CreateOnCompletionAsync);

            SharedHelper.ToastedUserAsync("FCM User Created");
        }

        public string GetEmail()
        {
            return auth.CurrentUser.Email;
        }

        public string GetToken()
        {
            String tkn = InstanceId.SharedInstance.Token;
            var mauttoken = auth.CurrentUser.Uid;

            var refreshedToken = InstanceId.SharedInstance.Token;

            if (refreshedToken == null)
            {
                auth.SignOut(out NSError error);
                //mAuth.SignOut();
            }
            return refreshedToken;
        }

        public void Init()
        {
            DependencyService.Get<IHelperClass>().IsLoading(true, "Logger INN");
            auth = Auth.DefaultInstance;
            var listenerHandle = Auth.DefaultInstance.AddAuthStateDidChangeListener(async (auth, user) =>
            {
                if (user != null)
                {
                    //FirebaseApp.InitializeApp(this);
                    // User is signed in
                    //ToastedUserAsync("onAuthStateChanged:signed_in:" + user.Uid);
                    App.IsUserLoggedIn = true;
                    // await SharedHelper.UpdateUserTokenAsync(auth.CurrentUser.Email, GetToken());
                    UpdateTokenAsync();
                    ChangeActivity();
                }
                else
                {
                    // User is signed out
                    //ToastedUserAsync("onAuthStateChanged:signed_out");
                    App.IsUserLoggedIn = false;

                    Xamarin.Forms.Application.Current.MainPage = new FCmLogin();
                }
            });

            //throw new NotImplementedException();
        }

        public void IsLoading(bool IsLoading, string text = "")
        {
            throw new NotImplementedException();
        }

        public void LogInGoogle()
        {
            throw new NotImplementedException();
        }

        public void LogInnUser(string email, string password)
        {
            //throw new NotImplementedException();

            auth.SignIn(email, password, SignInOnCompletion);
        }

        private async void CreateOnCompletionAsync(Firebase.Auth.User user, NSError error)
        {
            // stop animation
            //indicatorView.StopAnimating();

            if (error != null)
            {
                AuthErrorCode errorCode;
                if (IntPtr.Size == 8) // 64 bits devices
                    errorCode = (AuthErrorCode)((long)error.Code);
                else // 32 bits devices
                    errorCode = (AuthErrorCode)((int)error.Code);

                // Posible error codes that SignIn method with email and password could throw
                // Visit https://firebase.google.com/docs/auth/ios/errors for more information
                switch (errorCode)
                {
                    case AuthErrorCode.OperationNotAllowed:
                    case AuthErrorCode.InvalidEmail:
                    case AuthErrorCode.UserDisabled:
                    case AuthErrorCode.WrongPassword:
                    default:
                        // Print error
                        break;
                }
            }
            else
            {
                // await SharedHelper.UpdateUserTokenAsync(auth.CurrentUser.Email, GetToken());
                //ChangeActivity();
                // Do your magic to handle authentication result
            }
        }

        private void SignInOnCompletion(Firebase.Auth.User user, NSError error)
        {
            // stop animation
            //indicatorView.StopAnimating();

            if (error != null)
            {
                var NSERROR = error.UserInfo;
                AuthErrorCode errorCode;
                if (IntPtr.Size == 8) // 64 bits devices
                    errorCode = (AuthErrorCode)((long)error.Code);
                else // 32 bits devices
                    errorCode = (AuthErrorCode)((int)error.Code);

                // Posible error codes that SignIn method with email and password could throw
                // Visit https://firebase.google.com/docs/auth/ios/errors for more information
                switch (errorCode)
                {
                    case AuthErrorCode.OperationNotAllowed:
                    case AuthErrorCode.InvalidEmail:
                    case AuthErrorCode.UserDisabled:
                    case AuthErrorCode.WrongPassword:
                    default:
                        //show some irro message etc
                        //AppDelegate.ShowMessage("Could not login!", error.LocalizedDescription, NavigationController);

                        DependencyService.Get<IHelperClass>().IsLoading(false, "Loading");
                        break;
                }

                return;
            }
            // start ActiveUser Activity
            //NavigationController.PushViewController(new UserViewController("Firebase"), true);

            //await SharedHelper.UpdateUserTokenAsync(auth.CurrentUser.Email, GetToken());
            ChangeActivity();
        }

        private static void ChangeActivity()
        {
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new ActiveUser());
        }

        public void LogOut()
        {
            UpdateTokenAsync();
            auth.SignOut(out NSError error);
        }

        public async void LogoutFirebase()
        {
            await SharedHelper.UpdateUserTokenAsync(auth.CurrentUser.Email, GetToken(), true).ConfigureAwait(false);
            auth.SignOut(out NSError error);
        }

        public async void UpdateTokenAsync() // chaged this to asyc, if there is errrs
        {
            //FCMToken = Token;
            await SharedHelper.UpdateUserTokenAsync(auth.CurrentUser.Email, GetToken()).ConfigureAwait(false);
        }

        public void Createuser(string email, string password, string masterid, string UserName)
        {
            DependencyService.Get<IHelperClass>().IsLoading(true, "Creating User");
            CreateuserAsync(email, password, masterid, UserName);
        }

        public void CancelVIbrations()
        {
            IOS_MyFirebaseMessagingService.CancelVibration();
            // throw new NotImplementedException();
        }

        // private String FCMToken;
    }
}